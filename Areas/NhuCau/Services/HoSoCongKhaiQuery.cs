using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.NhuCau.Services;

internal sealed class HoSoCongKhaiQuery(AppDbContext db)
{
    public IQueryable<HoSoGiaSu> CongKhai() => db.Set<HoSoGiaSu>().AsNoTracking().Where(h=>h.DangHienThi&&h.TrangThaiDuyet=="DaDuyet"&&!db.Set<BienPhapQuanTri>().Any(b=>b.LoaiDoiTuong=="HoSoGiaSu"&&b.DoiTuongId==h.Id&&b.GoLuc==null&&b.Loai=="AnNoiDung")&&(h.DayTrucTuyen||h.DayTrucTiep)
        &&db.Set<GiaSu>().Any(g=>g.TaiKhoanId==h.GiaSuId)&&db.Set<NguoiThamGia>().Any(u=>u.Id==h.GiaSuId&&u.LoaiNguoi=="GiaSu")
        &&db.Set<HoSoGiaSuMonDay>().Any(d=>d.HoSoGiaSuId==h.Id)
        &&!db.Set<HoSoGiaSuMonDay>().Any(d=>d.HoSoGiaSuId==h.Id&&(d.HocPhiDeXuat==null||d.DonViHocPhi==null||!db.Set<MonHoc>().Any(m=>m.Id==d.MonHocId&&m.DangSuDung))));
    public async Task<List<HoSoGiaSuCongKhaiViewModel>> LayDanhSachAsync(List<HoSoGiaSu> hs, CancellationToken huy)
    {
        var ids=hs.Select(h=>h.Id).ToList();var owners=hs.Select(h=>h.GiaSuId).ToList();var people=await db.Set<NguoiThamGia>().Where(u=>owners.Contains(u.Id)).ToDictionaryAsync(x=>x.Id,x=>x.HoTen, huy);
        var subjects=await (from d in db.Set<HoSoGiaSuMonDay>() join m in db.Set<MonHoc>() on d.MonHocId equals m.Id where ids.Contains(d.HoSoGiaSuId) select new{d.HoSoGiaSuId,Row=new MonDayCongKhai{MonHocId=m.Id,TenMon=m.TenMon,PhamViDay=d.PhamViDay,HocPhi=d.HocPhiDeXuat,DonVi=d.DonViHocPhi}}).ToListAsync(huy);
        return hs.Select(h =>
        {
            var monDay = subjects.Where(s => s.HoSoGiaSuId == h.Id).Select(s => s.Row).OrderBy(s => s.TenMon).ToList();
            return new HoSoGiaSuCongKhaiViewModel
            {
                Id = h.Id, ChuSoHuuId = h.GiaSuId, LoaiNguoi = "GiaSu", HoTen = people[h.GiaSuId],
                TrinhDo = h.TrinhDo, ChuyenMon = h.ChuyenMon, KinhNghiem = h.KinhNghiem, MoTa = h.MoTa,
                KhuVuc = h.KhuVuc, KhuVucGiaSu = h.KhuVuc, DayTrucTuyen = h.DayTrucTuyen, DayTrucTiep = h.DayTrucTiep,
                HinhThucHoc = h.DayTrucTuyen && h.DayTrucTiep ? "CaHai" : h.DayTrucTuyen ? "TrucTuyen" : "TrucTiep",
                TaoLuc = h.DuyetLuc ?? DateTimeOffset.MinValue, MonTheoHoSo = monDay,
                MonDay = monDay.Select(m => m.TenMon).ToList()
            };
        }).ToList();
    }
}
