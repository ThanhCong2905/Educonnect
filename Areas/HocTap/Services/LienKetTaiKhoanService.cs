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
        Rules.Check(me.LoaiNguoi == "PhuHuynh", "Chỉ phụ huynh gửi đề nghị liên kết.", 403);
        email = email?.Trim() ?? "";
        Rules.Check(email.Length <= 256 && new EmailAddressAttribute().IsValid(email), "Nhập đúng email học sinh.", field: "email");
        var normalized = email.ToUpperInvariant();
        var student = await (from user in db.Set<TaiKhoan>()
                             join role in db.Set<HocSinh>() on user.Id equals role.TaiKhoanId
                             where user.NormalizedEmail == normalized && user.TrangThai == "HoatDong"
                             select user).SingleOrDefaultAsync();
        Rules.Check(student != null, "Không tìm thấy tài khoản học sinh phù hợp.", 404);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        Rules.Check(!await db.Set<LienKetPhuHuynhHocSinh>().AnyAsync(x => x.PhuHuynhId == me.Id && x.HocSinhId == student!.Id && x.KetThucLuc == null), "Đã có đề nghị hoặc liên kết với tài khoản này.", 409);
        var link = new LienKetPhuHuynhHocSinh { Id = Guid.NewGuid(), PhuHuynhId = me.Id, HocSinhId = student!.Id, NguoiGuiId = me.Id, GuiLuc = DateTimeOffset.UtcNow };
        db.Add(link);
        notices.Tao(student.Id, "LienKetTaiKhoan", link.Id, me.HoTen + " đề nghị liên kết tài khoản.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task PhanHoiAsync(Guid id, bool dongY, string version)
    {
        var me = await current.HienTaiAsync();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var link = await db.Set<LienKetPhuHuynhHocSinh>().SingleOrDefaultAsync(x => x.Id == id && x.HocSinhId == me.Id)
            ?? throw new LoiNghiepVu("Không tìm thấy đề nghị của bạn.", 404);
        Rules.Check(link.TrangThai == "ChoXacNhan" && link.KetThucLuc == null, "Đề nghị đã được xử lý.", 409);
        Rules.Concurrency(db, link, version);
        var now = DateTimeOffset.UtcNow;
        link.TrangThai = dongY ? "DaLienKet" : "TuChoi";
        if (dongY) link.XacNhanLuc = now;
        else { link.KetThucLuc = now; link.NguoiKetThucId = me.Id; }
        notices.Tao(link.PhuHuynhId, "LienKetTaiKhoan", link.Id, me.HoTen + (dongY ? " đã đồng ý liên kết." : " đã từ chối liên kết."));
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task HuyAsync(Guid id, string version)
    {
        var me = await current.HienTaiAsync();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var link = await db.Set<LienKetPhuHuynhHocSinh>().SingleOrDefaultAsync(x => x.Id == id && (x.HocSinhId == me.Id || x.PhuHuynhId == me.Id))
            ?? throw new LoiNghiepVu("Không tìm thấy liên kết của bạn.", 404);
        Rules.Check(link.KetThucLuc == null, "Liên kết đã kết thúc.", 409);
        Rules.Concurrency(db, link, version);
        link.TrangThai = "DaHuy";
        link.KetThucLuc = DateTimeOffset.UtcNow;
        link.NguoiKetThucId = me.Id;
        var members = await db.Set<ThanhVienLopHoc>().Where(x => x.RoiLopLuc == null &&
            (x.LienKetId == id || db.Set<LopHoc>().Any(c => c.Id == x.LopHocId && c.BenHocId == link.PhuHuynhId && c.HocSinhLienKetId == link.HocSinhId && x.TaiKhoanId != c.BenHocId && x.TaiKhoanId != c.GiaSuId))).ToListAsync();
        foreach (var member in members)
        {
            member.RoiLopLuc = link.KetThucLuc;
            member.TrangThaiDuyet = "DaRoiLop";
            notices.Tao(member.TaiKhoanId, "ThanhVien", member.LopHocId, "Quyền thành viên đã thu hồi vì liên kết tài khoản kết thúc.");
        }
        notices.Tao(me.Id == link.HocSinhId ? link.PhuHuynhId : link.HocSinhId, "LienKetTaiKhoan", id, "Liên kết tài khoản đã kết thúc.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
