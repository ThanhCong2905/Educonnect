using System.Data;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.HocTap.Services;

public sealed class LopHocService(AppDbContext db, NguoiDungHienTaiService current, QuyenLopHocService rights, ThongBaoService notices, EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService connections)
{
    public async Task<List<LopHoc>> LayAsync()
    {
        var u = await current.HienTaiAsync();
        return await db.Set<LopHoc>().AsNoTracking().Where(c => c.GiaSuId == u.Id || c.BenHocId == u.Id || db.Set<ThanhVienLopHoc>().Any(m => m.LopHocId == c.Id && m.TaiKhoanId == u.Id && m.TrangThaiDuyet == "DaDuyet" && m.RoiLopLuc == null)).OrderBy(c => c.TenLop).ToListAsync();
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
            "cancelled" => q.Where(x => x.TrangThai == "DaHuy"),
            "replaced" => q.Where(x => x.TrangThai == "DaDieuChinh"),
            "recorded" => q.Where(x => db.Set<BuoiHoc>().Any(b => b.LichHocId == x.Id)),
            "past" => q.Where(x => x.TrangThai == "DaLenLich" && x.KetThucLuc <= now && !db.Set<BuoiHoc>().Any(b => b.LichHocId == x.Id)),
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
    public async Task DeNghiAsync(Guid id, Guid person)
    {
        var c = await rights.KiemTraQuyenGhiAsync(id);
        var u = await current.HienTaiAsync();
        Rules.Check(c.BenHocId == u.Id, "Chỉ bên học chính đề nghị thành viên.", 403);
        Rules.Check(person != c.GiaSuId && person != c.BenHocId && await db.Set<NguoiThamGia>().AnyAsync(x => x.Id == person && x.LoaiNguoi != "GiaSu"), "Chọn người học hoặc phụ huynh khác.");
        Rules.Check(!await db.Set<ThanhVienLopHoc>().AnyAsync(x => x.LopHocId == id && x.TaiKhoanId == person && x.RoiLopLuc == null && x.TrangThaiDuyet != "TuChoi"), "Người này đã có đề nghị hoặc ở trong lớp.");
        await using var tx = await db.Database.BeginTransactionAsync();
        db.Add(new ThanhVienLopHoc { Id = Guid.NewGuid(), LopHocId = id, TaiKhoanId = person, NguoiDeNghiId = u.Id, TrangThaiDuyet = "ChoDuyet", DeNghiLuc = DateTimeOffset.UtcNow });
        notices.Tao(c.GiaSuId, "ThanhVien", id, "Có đề nghị thêm thành viên.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
    public async Task DuyetAsync(Guid member, string action, string permission, string version)
    {
        var m = await db.Set<ThanhVienLopHoc>().SingleOrDefaultAsync(x => x.Id == member) ?? throw new LoiNghiepVu("Không có thành viên.");
        var c = await rights.KiemTraQuyenGhiAsync(m.LopHocId, true);
        Rules.Check(m.TaiKhoanId != c.GiaSuId && m.TaiKhoanId != c.BenHocId, "Không đổi quyền thành viên chính.");
        Rules.Check(action is "DaDuyet" or "TuChoi" or "DaRoiLop", "Thao tác không hợp lệ.");
        Rules.Check(permission is "Xem" or "ChinhSua", "Chọn đúng quyền.");
        Rules.Concurrency(db, m, version);
        m.TrangThaiDuyet = action;
        m.MucQuyen = action == "DaDuyet" ? permission : m.MucQuyen;
        m.DuyetLuc = DateTimeOffset.UtcNow;
        if (action == "DaRoiLop")
            m.RoiLopLuc = DateTimeOffset.UtcNow;
        await using var tx = await db.Database.BeginTransactionAsync();
        notices.Tao(m.TaiKhoanId, "ThanhVien", c.Id, "Đề nghị/quyền thành viên lớp đã được cập nhật.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
    private EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService KetNoi => connections;

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
