using System.ComponentModel.DataAnnotations;
using System.Data;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.HocTap.Services;

public sealed record LienKetTaiKhoanRow(LienKetPhuHuynhHocSinh LienKet, string HoTen, string VaiTro);

public sealed class LienKetTaiKhoanService(AppDbContext db, NguoiDungHienTaiService current, ThongBaoService notices)
{
    public async Task<List<LienKetTaiKhoanRow>> LayLienKetAsync()
    {
        var me = await current.HienTaiAsync();
        return await (from link in db.Set<LienKetPhuHuynhHocSinh>().AsNoTracking()
                      join person in db.Set<NguoiThamGia>() on
                          (link.PhuHuynhId == me.Id ? link.HocSinhId : link.PhuHuynhId) equals person.Id
                      where link.PhuHuynhId == me.Id || link.HocSinhId == me.Id
                      orderby link.GuiLuc descending
                      select new LienKetTaiKhoanRow(link, person.HoTen, person.LoaiNguoi)).ToListAsync();
    }

    public async Task GuiDeNghiAsync(string email)
    {
        var me = await current.HienTaiAsync();
        Rules.Check(me.LoaiNguoi is "PhuHuynh" or "HocSinh", "Chỉ phụ huynh và học sinh được gửi đề nghị liên kết.", 403);
        var receiverRole = me.LoaiNguoi == "PhuHuynh" ? "HocSinh" : "PhuHuynh";
        var receiverLabel = receiverRole == "HocSinh" ? "học sinh" : "phụ huynh";
        email = email?.Trim() ?? "";
        Rules.Check(email.Length <= 256 && new EmailAddressAttribute().IsValid(email), $"Nhập đúng email {receiverLabel}.", field: "email");
        var normalized = email.ToUpperInvariant();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var receiver = await (from user in db.Set<TaiKhoan>()
                              join person in db.Set<NguoiThamGia>() on user.Id equals person.Id
                              where user.NormalizedEmail == normalized && user.TrangThai == "HoatDong" && person.LoaiNguoi == receiverRole
                              select user).SingleOrDefaultAsync();
        Rules.Check(receiver != null, $"Không tìm thấy tài khoản {receiverLabel} phù hợp.", 404);
        var parentId = me.LoaiNguoi == "PhuHuynh" ? me.Id : receiver!.Id;
        var studentId = me.LoaiNguoi == "HocSinh" ? me.Id : receiver!.Id;
        Rules.Check(!await db.Set<LienKetPhuHuynhHocSinh>().AnyAsync(x => x.PhuHuynhId == parentId && x.HocSinhId == studentId && x.KetThucLuc == null), "Đã có đề nghị hoặc liên kết với tài khoản này.", 409);
        var link = new LienKetPhuHuynhHocSinh { Id = Guid.NewGuid(), PhuHuynhId = parentId, HocSinhId = studentId, NguoiGuiId = me.Id, GuiLuc = DateTimeOffset.UtcNow };
        db.Add(link);
        notices.Tao(receiver!.Id, "LienKetTaiKhoan", link.Id, me.HoTen + " đề nghị liên kết tài khoản.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task PhanHoiAsync(Guid id, bool dongY, string version)
    {
        var me = await current.HienTaiAsync();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var link = await db.Set<LienKetPhuHuynhHocSinh>().SingleOrDefaultAsync(x => x.Id == id && (x.HocSinhId == me.Id || x.PhuHuynhId == me.Id) && x.NguoiGuiId != me.Id)
            ?? throw new LoiNghiepVu("Không tìm thấy đề nghị của bạn.", 404);
        Rules.Check(link.TrangThai == "ChoXacNhan" && link.KetThucLuc == null, "Đề nghị đã được xử lý.", 409);
        Rules.Concurrency(db, link, version);
        var now = DateTimeOffset.UtcNow;
        link.TrangThai = dongY ? "DaLienKet" : "TuChoi";
        if (dongY)
            link.XacNhanLuc = now;
        else
        {
            link.KetThucLuc = now;
            link.NguoiKetThucId = me.Id;
        }
        notices.Tao(link.NguoiGuiId, "LienKetTaiKhoan", link.Id, me.HoTen + (dongY ? " đã đồng ý liên kết." : " đã từ chối liên kết."));
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task<bool> HuyAsync(Guid id, string version)
    {
        var me = await current.HienTaiAsync();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var link = await db.Set<LienKetPhuHuynhHocSinh>().SingleOrDefaultAsync(x => x.Id == id && (x.HocSinhId == me.Id || x.PhuHuynhId == me.Id))
            ?? throw new LoiNghiepVu("Không tìm thấy liên kết của bạn.", 404);
        Rules.Check(link.KetThucLuc == null, "Liên kết đã kết thúc.", 409);
        Rules.Check(link.TrangThai == "DaLienKet" || (link.TrangThai == "ChoXacNhan" && link.NguoiGuiId == me.Id),
            "Bạn có thể đồng ý hoặc từ chối đề nghị đang chờ.", 409);
        Rules.Concurrency(db, link, version);
        var pending = link.TrangThai == "ChoXacNhan";
        link.TrangThai = "DaHuy";
        link.KetThucLuc = DateTimeOffset.UtcNow;
        link.NguoiKetThucId = me.Id;
        if (!pending)
        {
            var members = await db.Set<ThanhVienLopHoc>().Where(x => x.RoiLopLuc == null &&
                (x.LienKetId == id || db.Set<LopHoc>().Any(c => c.Id == x.LopHocId && c.BenHocId == link.PhuHuynhId && c.HocSinhLienKetId == link.HocSinhId && x.TaiKhoanId != c.BenHocId && x.TaiKhoanId != c.GiaSuId))).ToListAsync();
            foreach (var member in members)
            {
                member.RoiLopLuc = link.KetThucLuc;
                member.TrangThaiDuyet = "DaRoiLop";
                notices.Tao(member.TaiKhoanId, "ThanhVien", member.LopHocId, "Quyền thành viên đã thu hồi vì liên kết tài khoản kết thúc.");
            }
        }
        notices.Tao(me.Id == link.HocSinhId ? link.PhuHuynhId : link.HocSinhId, "LienKetTaiKhoan", id,
            pending ? "Đề nghị liên kết đã được rút." : "Liên kết tài khoản đã kết thúc.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return pending;
    }
}
