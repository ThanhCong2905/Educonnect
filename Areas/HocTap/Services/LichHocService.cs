using System.Data;
using System.Text.Json;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.HocTap.Services;

public sealed class LichHocService(AppDbContext db, NguoiDungMauService current, QuyenLopHocService rights, ThongBaoService notices)
{
    public async Task ThemHangLoatAsync(LichHangLoatInput m)
    {
        var c = await rights.KiemTraQuyenGhiAsync(m.LopHocId, true);
        var u = await current.HienTaiAsync();
        Rules.Check(m.Buoi.Count is > 0 and <= 100, "Chọn từ 1 đến 100 buổi.");
        foreach (var b in m.Buoi)
            Rules.Check(b.BatDauLuc < b.KetThucLuc && Rules.Local(b.BatDauLuc) > DateTimeOffset.UtcNow && !string.IsNullOrWhiteSpace(b.TieuDe) && b.TieuDe.Length <= 256, "Giờ học không hợp lệ hoặc đã qua.");
        var json = JsonSerializer.Serialize(m.Buoi.Select(b => new { b.TieuDe, BatDauLuc = Rules.Local(b.BatDauLuc), KetThucLuc = Rules.Local(b.KetThucLuc) }));
        await using var tx = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.usp_ThemLichHocHangLoat @LopHocId={m.LopHocId},@GiaSuId={u.Id},@DanhSachBuoiJson={json}");
        notices.Tao(c.BenHocId, "LichHoc", c.Id, "Lớp có lịch học mới.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
    public async Task<LichHoc> LayAsync(Guid id)
    {
        var s = await db.Set<LichHoc>().SingleOrDefaultAsync(x => x.Id == id) ?? throw new LoiNghiepVu("Không có lịch học.", 404);
        await rights.KiemTraQuyenXemAsync(s.LopHocId);
        return s;
    }
    public async Task SuaAsync(SuaLichInput m)
    {
        var s = await LayAsync(m.Id);
        var c = await rights.KiemTraQuyenGhiAsync(s.LopHocId, true);
        Rules.Check(m.TrangThai is "DaDieuChinh" or "DaHuy", "Trạng thái không hợp lệ.");
        Rules.Check(s.TrangThai != "DaHuy" && s.BatDauLuc > DateTimeOffset.UtcNow && !await db.Set<BuoiHoc>().AnyAsync(b => b.LichHocId == s.Id), "Không sửa hoặc hủy lịch đã bắt đầu, đã hủy hay đã ghi nhận.");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        if (m.TrangThai != "DaHuy")
        {
            var start = Rules.Local(m.Lich.BatDauLuc);
            var end = Rules.Local(m.Lich.KetThucLuc);
            Rules.Check(start < end && start > DateTimeOffset.UtcNow, "Giờ học không hợp lệ.");
            Rules.Check(!await (from l in db.Set<LichHoc>() join cl in db.Set<LopHoc>() on l.LopHocId equals cl.Id where l.Id != s.Id && l.TrangThai != "DaHuy" && cl.TrangThai == "DangHoatDong" && (cl.GiaSuId == c.GiaSuId || cl.BenHocId == c.BenHocId) && l.BatDauLuc < end && start < l.KetThucLuc select l).AnyAsync(), "Lịch bị chồng lấn.");
            s.BatDauLuc = start;
            s.KetThucLuc = end;
            s.TieuDe = m.Lich.TieuDe;
        }
        Rules.Concurrency(db, s, m.RowVersion);
        s.TrangThai = m.TrangThai;
        notices.Tao(c.BenHocId, "LichHoc", c.Id, "Lịch học đã được " + (m.TrangThai == "DaHuy" ? "hủy." : "điều chỉnh."));
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
    public async Task GhiNhanAsync(GhiNhanBuoiHocViewModel m)
    {
        var s = await LayAsync(m.LichHocId);
        var c = await rights.KiemTraQuyenGhiAsync(s.LopHocId, true);
        var u = await current.HienTaiAsync();
        Rules.Check(s.TrangThai != "DaHuy" && s.KetThucLuc <= DateTimeOffset.UtcNow, "Chỉ ghi nhận sau giờ kết thúc và lịch chưa bị hủy.");
        Rules.Check(m.KetQuaBuoiHoc is "DaDienRa" or "KhongDienRa", "Kết quả không hợp lệ.");
        Rules.Check(m.KetQuaBuoiHoc != "DaDienRa" || !string.IsNullOrWhiteSpace(m.NoiDung), "Cần nội dung giảng dạy.");
        Rules.Check(m.KetQuaBuoiHoc != "KhongDienRa" || m.TrangThaiGiaSu != "CoMat" || m.TrangThaiBenHoc != "CoMat", "Buổi không diễn ra cần ghi nhận bên vắng và lý do.");
        var rv = Rules.SqlVersion(m.RowVersion);
        await using var tx = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.usp_GhiNhanBuoiHoc @LichHocId={m.LichHocId},@GiaSuId={u.Id},@KetQuaBuoiHoc={m.KetQuaBuoiHoc},@NhanXet={m.NhanXet},@TrangThaiGiaSu={m.TrangThaiGiaSu},@TrangThaiBenHoc={m.TrangThaiBenHoc},@NoiDung={m.NoiDung},@RowVersionCu={rv}");
        notices.Tao(c.BenHocId, "LichHoc", c.Id, "Gia sư đã cập nhật ghi nhận buổi học.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
    public async Task<(BuoiHoc? Record, List<LichSuGhiNhanBuoiHoc> History)> GhiNhanChiTietAsync(Guid id)
    {
        await LayAsync(id);
        var b = await db.Set<BuoiHoc>().AsNoTracking().SingleOrDefaultAsync(x => x.LichHocId == id);
        return (b, b == null ? [] : await db.Set<LichSuGhiNhanBuoiHoc>().AsNoTracking().Where(x => x.BuoiHocId == b.Id).OrderByDescending(x => x.PhienBan).ToListAsync());
    }
}

