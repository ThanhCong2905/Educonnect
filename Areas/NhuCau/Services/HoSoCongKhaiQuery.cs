using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.NhuCau.Services;

internal sealed class HoSoCongKhaiQuery(AppDbContext db)
{
    public IQueryable<HoSoGiaSu> CongKhai() => db.Set<HoSoGiaSu>().AsNoTracking().Where(h => h.DangHienThi && h.TrangThaiDuyet == "DaDuyet" && !db.Set<BienPhapQuanTri>().Any(b => b.LoaiDoiTuong == "HoSoGiaSu" && b.DoiTuongId == h.Id && b.GoLuc == null && b.Loai == "AnNoiDung") && (h.DayTrucTuyen || h.DayTrucTiep)
        && db.Set<GiaSu>().Any(g => g.TaiKhoanId == h.GiaSuId) && db.Set<NguoiThamGia>().Any(u => u.Id == h.GiaSuId && u.LoaiNguoi == "GiaSu")
        && db.Set<HoSoGiaSuMonDay>().Any(d => d.HoSoGiaSuId == h.Id)
        && !db.Set<HoSoGiaSuMonDay>().Any(d => d.HoSoGiaSuId == h.Id && (d.HocPhiDeXuat == null || d.DonViHocPhi == null || !db.Set<MonHoc>().Any(m => m.Id == d.MonHocId && m.DangSuDung))));
    public async Task<List<HoSoGiaSuCongKhaiViewModel>> LayDanhSachAsync(List<HoSoGiaSu> hs, CancellationToken huy)
    {
        var ids = hs.Select(h => h.Id).ToList();
        var owners = hs.Select(h => h.GiaSuId).ToList();
        var people = await db.Users.AsNoTracking().Where(u => owners.Contains(u.Id)).Select(u => new { u.Id, u.HoTen, u.AnhDaiDien }).ToDictionaryAsync(x => x.Id, huy);
        var subjects = await (from d in db.Set<HoSoGiaSuMonDay>() join m in db.Set<MonHoc>() on d.MonHocId equals m.Id where ids.Contains(d.HoSoGiaSuId) select new { d.HoSoGiaSuId, Row = new MonDayCongKhai { MonHocId = m.Id, TenMon = m.TenMon, PhamViDay = d.PhamViDay, HocPhi = d.HocPhiDeXuat, DonVi = d.DonViHocPhi } }).ToListAsync(huy);
        return hs.Select(h =>
        {
            var monDay = subjects.Where(s => s.HoSoGiaSuId == h.Id).Select(s => s.Row).OrderBy(s => s.TenMon).ToList();
            return HoSoCongKhaiMapper.Map(h, people[h.GiaSuId].HoTen, people[h.GiaSuId].AnhDaiDien, monDay);
        }).ToList();
    }
}
