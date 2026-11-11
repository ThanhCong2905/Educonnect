using EduConnect.Mvc.Areas.QuanTri.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.QuanTri.Services;

public sealed class NhatKyAnToanQuery(AppDbContext db, QuyenQuanTriService rights)
{
    public async Task<Page<NhatKyAnToanRow>> LayAsync(NhatKyAnToanFilter filter, CancellationToken token)
    {
        await rights.KiemTraAsync("XemNhatKy");
        Rules.Check(filter.Trang is >= 1 and <= 100000 && (filter.Q?.Length ?? 0) <= 100
            && (filter.Ip?.Length ?? 0) <= 45 && (filter.TuNgay == null || filter.DenNgay == null || filter.TuNgay <= filter.DenNgay)
            && (filter.TuNgay == null || filter.TuNgay > DateOnly.MinValue)
            && (filter.DenNgay == null || filter.DenNgay < DateOnly.MaxValue)
            && (string.IsNullOrEmpty(filter.Nhom) || new[] { "Authentication", "Authorization", "Accounting" }.Contains(filter.Nhom))
            && (string.IsNullOrEmpty(filter.KetQua) || new[] { "ThanhCong", "ThatBai", "TuChoi" }.Contains(filter.KetQua)), "Bộ lọc nhật ký chưa hợp lệ.");
        var records = db.Set<NhatKyAnToan>().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(filter.Ip)) records = records.Where(x => x.DiaChiIp == filter.Ip.Trim());
        if (!string.IsNullOrEmpty(filter.Nhom)) records = records.Where(x => x.Nhom == filter.Nhom);
        if (!string.IsNullOrEmpty(filter.KetQua)) records = records.Where(x => x.KetQua == filter.KetQua);
        if (filter.TuNgay is { } start)
        {
            var since = new DateTimeOffset(start.ToDateTime(TimeOnly.MinValue), TimeSpan.FromHours(7));
            records = records.Where(x => x.ThoiDiem >= since);
        }
        if (filter.DenNgay is { } end)
        {
            var until = new DateTimeOffset(end.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.FromHours(7));
            records = records.Where(x => x.ThoiDiem < until);
        }
        var rows = from n in records
                   join u in db.Users on n.TaiKhoanId equals u.Id into actors
                   from u in actors.DefaultIfEmpty()
                   join t in db.Users on n.TaiKhoanDichId equals t.Id into targets
                   from t in targets.DefaultIfEmpty()
                   select new NhatKyAnToanRow { BanGhi = n, HoTen = u.HoTen, Email = u.Email, TaiKhoanDich = t.Email };
        if (!string.IsNullOrWhiteSpace(filter.Q))
        {
            var q = filter.Q.Trim();
            rows = rows.Where(x => (x.HoTen != null && x.HoTen.Contains(q)) || (x.Email != null && x.Email.Contains(q))
                || (x.TaiKhoanDich != null && x.TaiKhoanDich.Contains(q)) || x.BanGhi.SuKien.Contains(q)
                || x.BanGhi.MaYeuCau.Contains(q) || x.BanGhi.DinhDanhBam == q);
        }
        return new Page<NhatKyAnToanRow>
        {
            Trang = filter.Trang, KichThuocTrang = 20, Total = await rows.CountAsync(token),
            Items = await rows.OrderByDescending(x => x.BanGhi.ThoiDiem).ThenBy(x => x.BanGhi.Id)
                .Skip((filter.Trang - 1) * 20).Take(20).ToListAsync(token)
        };
    }
}
