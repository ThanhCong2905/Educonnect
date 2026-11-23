using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.QuanTri.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.QuanTri.Services;

public sealed partial class QuanTriService
{
    private async Task<List<DangKyTheoThang>> LayDangKyTheoThangAsync(DateTimeOffset first, DateTimeOffset now, CancellationToken huy)
    {
        var registered = db.Set<NhatKyAnToan>().Where(log => log.SuKien == "TaiKhoan.DangKy" && log.KetQua == "ThanhCong"
                && log.TaiKhoanDichId != null && log.ThoiDiem >= first && log.ThoiDiem <= now)
            .GroupBy(log => log.TaiKhoanDichId).Select(group => new { Id = group.Key, Luc = group.Min(log => log.ThoiDiem) });
        var users = await (from user in db.Users
                           join userRole in db.UserRoles on user.Id equals userRole.UserId
                           join role in db.Roles on userRole.RoleId equals role.Id
                           join log in registered on (Guid?)user.Id equals log.Id into logs
                           from log in logs.DefaultIfEmpty()
                           let time = user.DongYChinhSachLuc ?? (DateTimeOffset?)log.Luc
                           where (role.Name == "GiaSu" || role.Name == "HocSinh" || role.Name == "PhuHuynh")
                               && time >= first && time <= now
                           select new { role.Name, Time = time!.Value }).ToListAsync(huy);
        var counts = users.ToLookup(entry =>
        {
            var date = entry.Time.ToOffset(first.Offset);
            return (date.Year, date.Month, entry.Name);
        });
        return Enumerable.Range(0, 6).Select(index =>
        {
            var month = first.AddMonths(index);
            return new DangKyTheoThang(month.ToString("MM/yyyy"), counts[(month.Year, month.Month, "GiaSu")].Count(),
                counts[(month.Year, month.Month, "HocSinh")].Count(), counts[(month.Year, month.Month, "PhuHuynh")].Count());
        }).ToList();
    }

    private IQueryable<NhuCauHauKiemRow> LayHangHauKiem(DateTimeOffset tuNgay)
        => from need in db.Set<NhuCauHoc>().AsNoTracking()
           join owner in db.Users on need.ChuSoHuuId equals owner.Id
           join subject in db.Set<MonHoc>() on need.MonHocId equals subject.Id
           where need.LoaiNhuCau == "TimGiaSu" && need.TrangThai != "BanNhap"
           select new NhuCauHauKiemRow
           {
               NhuCau = need, HoTen = owner.HoTen, MonHoc = subject.TenMon,
               DangAn = db.Set<BienPhapQuanTri>().Any(b => b.LoaiDoiTuong == "NhuCauHoc" && b.DoiTuongId == need.Id && b.Loai == "AnNoiDung" && b.GoLuc == null),
               BaoCaoCho = db.Set<BaoCaoViPham>().Count(report => report.LoaiDoiTuong == "NhuCauHoc" && report.DoiTuongId == need.Id && report.TrangThai != "DaXuLy"),
               LuotDangGanDay = db.Set<LuotDangNhuCau>().Count(post => post.NhuCauId == need.Id && post.DangLuc >= tuNgay)
           };

    private async Task<Page<NhuCauHauKiemRow>> LayNhuCauCanXemXetAsync(IQueryable<NhuCauHauKiemRow> query,
        int trang, int kichThuoc, CancellationToken huy)
    {
        var candidates = await query.Where(row => row.NhuCau.TrangThai == "DangMo" && !row.DangAn
                && (row.NhuCau.HocPhiDuKien > 0 || row.BaoCaoCho > 0 || row.LuotDangGanDay >= 3))
            .ToListAsync(huy);
        await DanhDauNhuCauAsync(candidates, huy);
        var alerts = candidates.Where(row => row.DauHieu.Count > 0).ToList();
        return new Page<NhuCauHauKiemRow>
        {
            Trang = trang, KichThuocTrang = kichThuoc, Total = alerts.Count,
            Items = alerts.OrderByDescending(row => row.DauHieu.Count).ThenByDescending(row => row.NhuCau.TaoLuc)
                .ThenBy(row => row.NhuCau.Id).Skip((trang - 1) * kichThuoc).Take(kichThuoc).ToList()
        };
    }

    private async Task DanhDauNhuCauAsync(List<NhuCauHauKiemRow> rows, CancellationToken huy)
    {
        var subjectIds = rows.Where(row => row.NhuCau.HocPhiDuKien > 0).Select(row => row.NhuCau.MonHocId).Distinct().ToList();
        var profiles = new HoSoCongKhaiQuery(db).CongKhai();
        var offers = subjectIds.Count == 0 ? [] : await (from lesson in db.Set<HoSoGiaSuMonDay>().AsNoTracking()
            join profile in profiles on lesson.HoSoGiaSuId equals profile.Id
            where subjectIds.Contains(lesson.MonHocId) && lesson.HocPhiDeXuat > 0
                && (lesson.DonViHocPhi == "VND/Gio" || lesson.DonViHocPhi == "VND/Buoi")
            select new { lesson.MonHocId, lesson.DonViHocPhi, Gia = lesson.HocPhiDeXuat!.Value }).ToListAsync(huy);
        var benchmarks = offers.GroupBy(offer => (offer.MonHocId, offer.DonViHocPhi)).Where(group => group.Count() >= 5)
            .ToDictionary(group => group.Key, group =>
            {
                var prices = group.Select(offer => offer.Gia).Order().ToArray();
                return (Gia: (prices[(prices.Length - 1) / 2] + prices[prices.Length / 2]) / 2, SoMau: prices.Length);
            });
        foreach (var row in rows)
        {
            if (row.BaoCaoCho > 0) row.DauHieu.Add($"{row.BaoCaoCho} báo cáo chưa xử lý xong");
            if (row.LuotDangGanDay >= 3) row.DauHieu.Add($"{row.LuotDangGanDay} lượt đăng hoặc mở lại trong 7 ngày");
            var need = row.NhuCau;
            if (need.HocPhiDuKien is not > 0 || need.DonViHocPhi == null ||
                !benchmarks.TryGetValue((need.MonHocId, need.DonViHocPhi), out var benchmark)) continue;
            var price = need.HocPhiDuKien.Value;
            if (price < benchmark.Gia / 2 || price > benchmark.Gia * 2)
            {
                var level = price < benchmark.Gia / 2 ? "Học phí thấp" : "Học phí cao";
                row.DauHieu.Add($"{level} · trung vị {Rules.Fee(benchmark.Gia, need.DonViHocPhi)} ({benchmark.SoMau} mức phí gia sư)");
            }
        }
    }
}
