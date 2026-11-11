using System.Data;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.HocTap.Services;

public sealed partial class LopHocService(AppDbContext db, NguoiDungHienTaiService current, QuyenLopHocService rights, ThongBaoService notices, EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService connections)
{
    public async Task<List<LopHoc>> LayAsync()
    {
        var u = await current.HienTaiAsync();
        return await db.Set<LopHoc>().AsNoTracking().Where(c => c.GiaSuId == u.Id || c.BenHocId == u.Id || db.Set<ThanhVienLopHoc>().Any(m => m.LopHocId == c.Id && m.TaiKhoanId == u.Id && m.TrangThaiDuyet == "DaDuyet" && m.RoiLopLuc == null && db.Set<LienKetPhuHuynhHocSinh>().Any(l => l.Id == m.LienKetId && l.TrangThai == "DaLienKet"))).OrderBy(c => c.TenLop).ToListAsync();
    }
    public Task<LopHoc> ChiTietAsync(Guid id) => rights.KiemTraQuyenXemAsync(id);
    public async Task<List<LichHoc>> LichAsync(Guid id)
    {
        await rights.KiemTraQuyenXemAsync(id);
        return await db.Set<LichHoc>().AsNoTracking().Where(x => x.LopHocId == id).OrderBy(x => x.BatDauLuc).ToListAsync();
    }
    public async Task<List<LichHoc>> LocBuoiAsync(Guid id, string? month, string? state)
    {
        await rights.KiemTraQuyenXemAsync(id);
        var q = db.Set<LichHoc>().AsNoTracking().Where(x => x.LopHocId == id);
        if (!string.IsNullOrEmpty(month))
        {
            Rules.Check(DateTime.TryParseExact(month + "-01", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var date), "Tháng không hợp lệ.");
            var from = Rules.Local(date);
            var to = from.AddMonths(1);
            q = q.Where(x => x.BatDauLuc >= from && x.BatDauLuc < to);
        }
        var now = DateTimeOffset.UtcNow;
        q = state switch
        {
            "cancelled" => q.Where(x => x.TrangThai == "DaHuy" && !db.Set<BuoiHoc>().Any(b => b.LichHocId == x.Id && b.TrangThai == "DaNghi")),
            "replaced" => q.Where(x => x.TrangThai == "DaDieuChinh"),
            "recorded" => q.Where(x => db.Set<BuoiHoc>().Any(b => b.LichHocId == x.Id && b.GhiNhanLuc != null)),
            "rest" => q.Where(x => db.Set<BuoiHoc>().Any(b => b.LichHocId == x.Id && b.TrangThai == "DaNghi")),
            "past" => q.Where(x => x.TrangThai == "DaLenLich" && x.KetThucLuc <= now && !db.Set<BuoiHoc>().Any(b => b.LichHocId == x.Id && b.GhiNhanLuc != null)),
            "ongoing" => q.Where(x => x.TrangThai == "DaLenLich" && x.BatDauLuc <= now && x.KetThucLuc > now),
            "upcoming" => q.Where(x => x.TrangThai == "DaLenLich" && x.BatDauLuc > now),
            _ => q
        };
        return await q.OrderBy(x => x.BatDauLuc).ToListAsync();
    }
    public async Task<List<BuoiHoc>> BuoiAsync(Guid id)
    {
        await rights.KiemTraQuyenXemAsync(id);
        return await (from b in db.Set<BuoiHoc>() join l in db.Set<LichHoc>() on b.LichHocId equals l.Id where l.LopHocId == id select b).AsNoTracking().ToListAsync();
    }
    public async Task<List<ThanhVienLopHoc>> ThanhVienAsync(Guid id)
    {
        await rights.KiemTraQuyenXemAsync(id);
        return await db.Set<ThanhVienLopHoc>().AsNoTracking().Where(x => x.LopHocId == id).ToListAsync();
    }
    public async Task<NguoiDuocMoiViewModel> TimNguoiDuocMoiAsync(Guid id, string email)
    {
        var c = await rights.KiemTraQuyenGhiAsync(id);
        var u = await current.HienTaiAsync();
        Rules.Check(c.BenHocId == u.Id, "Chỉ bên học chính được mời thành viên.", 403);
        email = email?.Trim() ?? "";
        Rules.Check(email.Length <= 256 && new EmailAddressAttribute().IsValid(email), "Nhập đúng email tài khoản muốn mời.");
        var normalized = email.ToUpperInvariant();
        var person = await (from account in db.Set<TaiKhoan>()
                            join participant in db.Set<NguoiThamGia>() on account.Id equals participant.Id
                            where account.NormalizedEmail == normalized && account.TrangThai == "HoatDong" && (participant.LoaiNguoi == "HocSinh" || participant.LoaiNguoi == "PhuHuynh")
                            select new NguoiDuocMoiViewModel { LopHocId = id, TaiKhoanId = account.Id, Email = account.Email!, HoTen = participant.HoTen, VaiTro = participant.LoaiNguoi })
                            .SingleOrDefaultAsync() ?? throw new LoiNghiepVu("Không tìm thấy tài khoản học sinh hoặc phụ huynh có email này.", 404);
        Rules.Check(person.TaiKhoanId != c.BenHocId, "Bạn đã là bên học chính của lớp này.");
        Rules.Check(!await db.Set<ThanhVienLopHoc>().AnyAsync(x => x.LopHocId == id && x.TaiKhoanId == person.TaiKhoanId && x.RoiLopLuc == null && x.TrangThaiDuyet != "TuChoi"), "Tài khoản này đã ở trong lớp hoặc có lời mời đang chờ.", 409);
        var benHoc = await db.Set<NguoiThamGia>().SingleAsync(x => x.Id == c.BenHocId);
        var studentId = benHoc.LoaiNguoi == "HocSinh" ? benHoc.Id
            : c.HocSinhLienKetId ?? (person.VaiTro == "HocSinh" ? person.TaiKhoanId : Guid.Empty);
        Rules.Check(studentId != Guid.Empty, "Hãy liên kết và mời đúng học sinh của lớp trước khi thêm người đại diện.");
        if (benHoc.LoaiNguoi == "PhuHuynh")
            Rules.Check(await db.Set<LienKetPhuHuynhHocSinh>().AnyAsync(x => x.PhuHuynhId == benHoc.Id && x.HocSinhId == studentId && x.TrangThai == "DaLienKet" && x.KetThucLuc == null), "Bạn chưa có liên kết hợp lệ với học sinh của lớp.", 403);
        var parentId = person.VaiTro == "PhuHuynh" ? person.TaiKhoanId : c.BenHocId;
        Rules.Check(person.VaiTro != "HocSinh" || person.TaiKhoanId == studentId, "Chỉ thêm đúng học sinh của lớp.", 403);
        var link = await db.Set<LienKetPhuHuynhHocSinh>().SingleOrDefaultAsync(x => x.PhuHuynhId == parentId && x.HocSinhId == studentId && x.TrangThai == "DaLienKet" && x.KetThucLuc == null)
            ?? throw new LoiNghiepVu("Tài khoản này chưa có liên kết phù hợp với học sinh của lớp.", 403);
        person.LienKetId = link.Id;
        person.HocSinhId = studentId;
        return person;
    }

    public async Task DeNghiAsync(Guid id, string email)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await KhoaThanhVienLopAsync(id);
        var person = await TimNguoiDuocMoiAsync(id, email);
        var user = await current.HienTaiAsync();
        var member = new ThanhVienLopHoc { Id = Guid.NewGuid(), LopHocId = id, TaiKhoanId = person.TaiKhoanId, NguoiDeNghiId = user.Id, LienKetId = person.LienKetId, TrangThaiDuyet = "ChoDuyet", DeNghiLuc = DateTimeOffset.UtcNow };
        db.Add(member);
        notices.Tao(person.TaiKhoanId, "LoiMoiLop", member.Id, user.HoTen + " mời bạn tham gia lớp học.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task<List<LoiMoiThanhVienViewModel>> LoiMoiAsync()
    {
        var user = await current.HienTaiAsync();
        return await (from member in db.Set<ThanhVienLopHoc>().AsNoTracking()
                      join lop in db.Set<LopHoc>() on member.LopHocId equals lop.Id
                      join sender in db.Set<NguoiThamGia>() on member.NguoiDeNghiId equals sender.Id
                      where member.TaiKhoanId == user.Id && member.TrangThaiDuyet == "ChoDuyet" && member.RoiLopLuc == null
                      orderby member.DeNghiLuc descending
                      select new LoiMoiThanhVienViewModel { Id = member.Id, TenLop = lop.TenLop, NguoiMoi = sender.HoTen, TrangThaiLop = lop.TrangThai, TrangThaiDuyet = member.TrangThaiDuyet, DongYThamGiaLuc = member.DongYThamGiaLuc, RowVersion = member.RowVersion }).ToListAsync();
    }

    public async Task PhanHoiLoiMoiAsync(Guid id, bool agree, string version)
    {
        var user = await current.HienTaiAsync();
        var classId = await db.Set<ThanhVienLopHoc>().Where(x => x.Id == id && x.TaiKhoanId == user.Id)
            .Select(x => (Guid?)x.LopHocId).SingleOrDefaultAsync() ?? throw new LoiNghiepVu("Không tìm thấy lời mời của bạn.", 404);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await KhoaThanhVienLopAsync(classId);
        var member = await db.Set<ThanhVienLopHoc>().SingleOrDefaultAsync(x => x.Id == id && x.TaiKhoanId == user.Id)
            ?? throw new LoiNghiepVu("Không tìm thấy lời mời của bạn.", 404);
        var lop = await db.Set<LopHoc>().SingleAsync(x => x.Id == member.LopHocId);
        Rules.Check(lop.TrangThai == "DangHoatDong", "Lớp đã kết thúc, lời mời không còn hiệu lực.", 409);
        Rules.Check(member.TrangThaiDuyet == "ChoDuyet" && member.DongYThamGiaLuc == null && member.RoiLopLuc == null, "Lời mời đã được xử lý.", 409);
        Rules.Concurrency(db, member, version);
        if (agree)
        {
            member.DongYThamGiaLuc = DateTimeOffset.UtcNow;
            notices.Tao(lop.GiaSuId, "ThanhVien", lop.Id, user.HoTen + " đã đồng ý tham gia lớp. Hãy xem đề nghị và cấp quyền.");
        }
        else
            member.TrangThaiDuyet = "TuChoi";
        notices.Tao(lop.BenHocId, "ThanhVien", lop.Id, user.HoTen + (agree ? " đã đồng ý lời mời tham gia lớp." : " đã từ chối lời mời tham gia lớp."));
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
    public async Task DuyetAsync(Guid member, string action, string permission, string version, bool choPhepNopBai = false)
    {
        var classId = await db.Set<ThanhVienLopHoc>().Where(x => x.Id == member).Select(x => (Guid?)x.LopHocId)
            .SingleOrDefaultAsync() ?? throw new LoiNghiepVu("Không có thành viên.", 404);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await KhoaThanhVienLopAsync(classId);
        var m = await db.Set<ThanhVienLopHoc>().SingleOrDefaultAsync(x => x.Id == member) ?? throw new LoiNghiepVu("Không có thành viên.");
        var c = await rights.KiemTraQuyenGhiAsync(m.LopHocId, true);
        Rules.Check(m.TaiKhoanId != c.GiaSuId && m.TaiKhoanId != c.BenHocId, "Không đổi quyền thành viên chính.");
        Rules.Check(action is "DaDuyet" or "TuChoi" or "DaRoiLop", "Thao tác không hợp lệ.");
        Rules.Check(permission is "Xem" or "ChinhSua", "Chọn đúng quyền.");
        Rules.Check(m.RoiLopLuc == null && (m.TrangThaiDuyet is "ChoDuyet" or "DaDuyet"), "Đề nghị này đã được xử lý.", 409);
        Rules.Check(m.TrangThaiDuyet != "ChoDuyet" || m.DongYThamGiaLuc != null, "Người được mời cần đồng ý trước khi gia sư duyệt.", 409);
        Rules.Concurrency(db, m, version);
        if (action == "DaDuyet")
        {
            var link = await db.Set<LienKetPhuHuynhHocSinh>().SingleOrDefaultAsync(x => x.Id == m.LienKetId && x.TrangThai == "DaLienKet" && x.KetThucLuc == null)
                ?? throw new LoiNghiepVu("Liên kết tài khoản không còn hiệu lực.", 409);
            var owner = await db.Set<NguoiThamGia>().SingleAsync(x => x.Id == c.BenHocId);
            var studentId = owner.LoaiNguoi == "HocSinh" ? owner.Id : c.HocSinhLienKetId ?? link.HocSinhId;
            Rules.Check(link.HocSinhId == studentId, "Liên kết không thuộc người học của lớp.", 403);
            if (owner.LoaiNguoi == "PhuHuynh")
                Rules.Check(await db.Set<LienKetPhuHuynhHocSinh>().AnyAsync(x => x.PhuHuynhId == owner.Id && x.HocSinhId == studentId && x.TrangThai == "DaLienKet" && x.KetThucLuc == null), "Bên học không còn liên kết phù hợp.", 409);
            var accounts = await (from memberRow in db.Set<ThanhVienLopHoc>() join person in db.Set<NguoiThamGia>() on memberRow.TaiKhoanId equals person.Id where memberRow.LopHocId == c.Id && memberRow.TrangThaiDuyet == "DaDuyet" && memberRow.RoiLopLuc == null select person.Id).ToListAsync();
            accounts.Add(c.BenHocId); accounts.Add(c.GiaSuId); accounts.Add(m.TaiKhoanId);
            var people = await db.Set<NguoiThamGia>().Where(x => accounts.Contains(x.Id)).ToListAsync();
            Rules.Check(people.Count(x => x.LoaiNguoi == "HocSinh") <= 1 && people.Count(x => x.LoaiNguoi == "PhuHuynh") <= 2, "Lớp chỉ có một học sinh và tối đa hai phụ huynh.", 409);
            c.HocSinhLienKetId = studentId;
        }
        m.TrangThaiDuyet = action;
        m.MucQuyen = action == "DaDuyet" ? permission : m.MucQuyen;
        if (action == "DaDuyet") m.ChoPhepNopBai = permission == "ChinhSua" && choPhepNopBai;
        if (m.DuyetLuc == null) m.DuyetLuc = DateTimeOffset.UtcNow;
        if (action == "DaRoiLop")
            m.RoiLopLuc = DateTimeOffset.UtcNow;
        notices.Tao(m.TaiKhoanId, action == "DaDuyet" ? "ThanhVien" : "LoiMoiLop", action == "DaDuyet" ? c.Id : m.Id,
            action == "DaDuyet" ? "Gia sư đã duyệt quyền tham gia lớp của bạn." : action == "TuChoi" ? "Gia sư đã từ chối đề nghị tham gia lớp." : "Bạn đã được cho rời lớp.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
    private EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService KetNoi => connections;

    private Task KhoaThanhVienLopAsync(Guid id)
        => db.Database.ExecuteSqlInterpolatedAsync($"SELECT Id FROM dbo.LopHoc WITH (UPDLOCK,HOLDLOCK) WHERE Id={id}");

    public Task<KetQua> XacNhanKetThucAsync(Guid maYeuCau, byte[] rowVersion, Guid maNguoiThucHien, CancellationToken huy) =>
        KetNoi.PhanHoiHuyAsync(maYeuCau, true, rowVersion, maNguoiThucHien, huy);

    public Task KetThucAsync(Guid id, string version) =>
        Task.FromException(new LoiNghiepVu("Hãy phản hồi yêu cầu kết thúc bằng phiên bản yêu cầu đang hiển thị.", 409));

    public async Task YeuCauHuyAsync(Guid id, string reason, string? rowVersion = null, CancellationToken huy = default)
    {
        var user = await current.HienTaiAsync();
        var lop = await rights.KiemTraQuyenXemAsync(id);
        var version = rowVersion == null ? lop.RowVersion : Rules.Version(rowVersion);
        var result = await KetNoi.TaoYeuCauHuyAsync(id, reason, version, user.Id, huy);
        if (!result.ThanhCong)
            throw new LoiNghiepVu(result.ThongBao!, result.MaTrangThai);
    }
    public async Task<List<YeuCauHuyKetNoi>> HuyAsync(Guid id)
    {
        await rights.KiemTraQuyenXemAsync(id);
        return await db.Set<YeuCauHuyKetNoi>().AsNoTracking().Where(x => x.LopHocId == id).OrderByDescending(x => x.GuiLuc).ToListAsync();
    }
    public async Task PhanHoiHuyAsync(Guid id, bool agree, string version, CancellationToken huy = default)
    {
        var user = await current.HienTaiAsync();
        (await KetNoi.PhanHoiHuyAsync(id, agree, Rules.Version(version), user.Id, huy)).BaoLoiNeuThatBai();
    }
}
