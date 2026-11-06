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
        await notices.TaoChoLopAsync(c.Id, "LichHoc", c.Id, "Lớp có lịch học mới.", u.Id);
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
        var u = await current.HienTaiAsync();
        Rules.Check(m.TrangThai is "DaDieuChinh" or "DaHuy", "Trạng thái không hợp lệ.");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await db.Database.ExecuteSqlInterpolatedAsync($"""
            DECLARE @first uniqueidentifier = CASE WHEN {c.GiaSuId} < {c.BenHocId} THEN {c.GiaSuId} ELSE {c.BenHocId} END;
            DECLARE @second uniqueidentifier = CASE WHEN {c.GiaSuId} < {c.BenHocId} THEN {c.BenHocId} ELSE {c.GiaSuId} END;
            DECLARE @locked uniqueidentifier;
            SELECT @locked=Id FROM dbo.NguoiThamGia WITH(UPDLOCK,HOLDLOCK) WHERE Id=@first;
            SELECT @locked=Id FROM dbo.NguoiThamGia WITH(UPDLOCK,HOLDLOCK) WHERE Id=@second;
            SELECT @locked=Id FROM dbo.LopHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id={c.Id};
            SELECT @locked=Id FROM dbo.LichHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id={s.Id};
            """);
        await db.Entry(c).ReloadAsync();
        await db.Entry(s).ReloadAsync();
        Rules.Check(c.TrangThai == "DangHoatDong", "Lớp không còn hoạt động.", 409);
        Rules.Check(s.RowVersion.SequenceEqual(Rules.Version(m.RowVersion)), "Lịch đã thay đổi. Hãy tải lại.", 409);
        Rules.Check(s.TrangThai == "DaLenLich" && s.BatDauLuc > DateTimeOffset.UtcNow && !await db.Set<BuoiHoc>().AnyAsync(b => b.LichHocId == s.Id), "Không sửa hoặc hủy lịch đã bắt đầu, đã thay thế, đã hủy hay đã ghi nhận.", 409);
        if (m.TrangThai != "DaHuy")
        {
            var start = Rules.Local(m.Lich.BatDauLuc);
            var end = Rules.Local(m.Lich.KetThucLuc);
            Rules.Check(start < end && start > DateTimeOffset.UtcNow && !string.IsNullOrWhiteSpace(m.Lich.TieuDe) && m.Lich.TieuDe.Length <= 256, "Tiêu đề hoặc giờ học không hợp lệ.");
            Rules.Check(!await (from l in db.Set<LichHoc>() join cl in db.Set<LopHoc>() on l.LopHocId equals cl.Id where l.Id != s.Id && l.TrangThai == "DaLenLich" && cl.TrangThai == "DangHoatDong" && (cl.GiaSuId == c.GiaSuId || cl.BenHocId == c.BenHocId) && l.BatDauLuc < end && start < l.KetThucLuc select l).AnyAsync(), "Lịch bị chồng lấn.", 409);
            db.Add(new LichHoc { Id = Guid.NewGuid(), LopHocId = c.Id, LichGocId = s.Id, LaBuoiBu = s.LaBuoiBu, TrangThai = "DaLenLich", TieuDe = m.Lich.TieuDe.Trim(), BatDauLuc = start, KetThucLuc = end });
        }
        Rules.Concurrency(db, s, m.RowVersion);
        s.TrangThai = m.TrangThai;
        await notices.TaoChoLopAsync(c.Id, "LichHoc", c.Id, "Lịch học đã được " + (m.TrangThai == "DaHuy" ? "hủy." : "điều chỉnh."), u.Id);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
    public async Task TaoBuoiBuAsync(SuaLichInput m)
    {
        var s = await LayAsync(m.Id);
        var c = await rights.KiemTraQuyenGhiAsync(s.LopHocId, true);
        var u = await current.HienTaiAsync();
        var start = Rules.Local(m.Lich.BatDauLuc);
        var end = Rules.Local(m.Lich.KetThucLuc);
        Rules.Check(start < end && start > DateTimeOffset.UtcNow && !string.IsNullOrWhiteSpace(m.Lich.TieuDe) && m.Lich.TieuDe.Length <= 256, "Tiêu đề hoặc giờ học bù không hợp lệ.");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await db.Database.ExecuteSqlInterpolatedAsync($"""
            DECLARE @first uniqueidentifier = CASE WHEN {c.GiaSuId} < {c.BenHocId} THEN {c.GiaSuId} ELSE {c.BenHocId} END;
            DECLARE @second uniqueidentifier = CASE WHEN {c.GiaSuId} < {c.BenHocId} THEN {c.BenHocId} ELSE {c.GiaSuId} END;
            DECLARE @locked uniqueidentifier;
            SELECT @locked=Id FROM dbo.NguoiThamGia WITH(UPDLOCK,HOLDLOCK) WHERE Id=@first;
            SELECT @locked=Id FROM dbo.NguoiThamGia WITH(UPDLOCK,HOLDLOCK) WHERE Id=@second;
            SELECT @locked=Id FROM dbo.LopHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id={c.Id};
            SELECT @locked=Id FROM dbo.LichHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id={s.Id};
            """);
        await db.Entry(c).ReloadAsync();
        await db.Entry(s).ReloadAsync();
        Rules.Check(c.TrangThai == "DangHoatDong", "Lớp không còn hoạt động.", 409);
        Rules.Check(s.TrangThai != "DaDieuChinh" && s.RowVersion.SequenceEqual(Rules.Version(m.RowVersion)), "Lịch gốc đã thay đổi. Hãy tải lại.", 409);
        Rules.Check(!await (from l in db.Set<LichHoc>() join cl in db.Set<LopHoc>() on l.LopHocId equals cl.Id where l.TrangThai == "DaLenLich" && cl.TrangThai == "DangHoatDong" && (cl.GiaSuId == c.GiaSuId || cl.BenHocId == c.BenHocId) && l.BatDauLuc < end && start < l.KetThucLuc select l).AnyAsync(), "Lịch học bù bị chồng lấn.", 409);
        db.Add(new LichHoc { Id = Guid.NewGuid(), LopHocId = c.Id, LichGocId = s.Id, LaBuoiBu = true, TrangThai = "DaLenLich", TieuDe = m.Lich.TieuDe.Trim(), BatDauLuc = start, KetThucLuc = end });
        await notices.TaoChoLopAsync(c.Id, "LichHoc", c.Id, "Lớp có buổi học bù mới.", u.Id);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
    public async Task GhiNhanAsync(GhiNhanBuoiHocViewModel m)
    {
        var s = await LayAsync(m.LichHocId);
        var c = await rights.KiemTraQuyenGhiAsync(s.LopHocId, true);
        var u = await current.HienTaiAsync();
        Rules.Check(s.TrangThai == "DaLenLich" && s.KetThucLuc <= DateTimeOffset.UtcNow, "Chỉ ghi nhận sau giờ kết thúc và lịch chưa bị hủy hoặc thay thế.", 409);
        Rules.Check(m.KetQuaBuoiHoc is "DaDienRa" or "KhongDienRa", "Kết quả không hợp lệ.");
        Rules.Check(m.KetQuaBuoiHoc != "DaDienRa" || !string.IsNullOrWhiteSpace(m.NoiDung), "Cần nội dung giảng dạy.");
        Rules.Check(m.KetQuaBuoiHoc != "KhongDienRa" || m.TrangThaiGiaSu != "CoMat" || m.TrangThaiBenHoc != "CoMat", "Buổi không diễn ra cần ghi nhận bên vắng và lý do.");
        Rules.Check(m.TrangThaiGiaSu is "CoMat" or "VangCoBaoTruoc" or "VangKhongBaoTruoc" && m.TrangThaiBenHoc is "CoMat" or "VangCoBaoTruoc" or "VangKhongBaoTruoc", "Trạng thái tham dự không hợp lệ.");
        Rules.Check(!string.IsNullOrWhiteSpace(m.NhanXet) && m.NhanXet.Length <= 2000 && (m.NoiDung?.Length ?? 0) <= 10000, "Cần nhận xét tối đa 2000 ký tự và nội dung tối đa 10000 ký tự.");
        var rv = Rules.SqlVersion(m.RowVersion);
        await using var tx = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.usp_GhiNhanBuoiHoc @LichHocId={m.LichHocId},@GiaSuId={u.Id},@KetQuaBuoiHoc={m.KetQuaBuoiHoc},@NhanXet={m.NhanXet},@TrangThaiGiaSu={m.TrangThaiGiaSu},@TrangThaiBenHoc={m.TrangThaiBenHoc},@NoiDung={m.NoiDung},@RowVersionCu={rv}");
        await notices.TaoChoLopAsync(c.Id, "LichHoc", c.Id, "Gia sư đã cập nhật ghi nhận buổi học.", u.Id);
        if (m.TrangThaiGiaSu == "VangKhongBaoTruoc" && !await db.Set<ThongBao>().AnyAsync(x => x.NguoiNhanId == c.BenHocId && x.DoiTuongId == s.Id && x.NoiDungTomTat == "Gia sư vắng không báo trước."))
            notices.Tao(c.BenHocId, "NhacViec", s.Id, "Gia sư vắng không báo trước.");
        if (m.TrangThaiBenHoc == "VangKhongBaoTruoc" && !await db.Set<ThongBao>().AnyAsync(x => x.NguoiNhanId == c.GiaSuId && x.DoiTuongId == s.Id && x.NoiDungTomTat == "Bên học vắng không báo trước."))
            notices.Tao(c.GiaSuId, "NhacViec", s.Id, "Bên học vắng không báo trước.");
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

