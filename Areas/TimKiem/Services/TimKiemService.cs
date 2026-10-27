using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.TimKiem.Services;

public sealed class TimKiemService(AppDbContext db, NguoiDungMauService current)
{
    public IQueryable<HoSoGiaSu> HoSoCongKhai() => db.Set<HoSoGiaSu>().AsNoTracking().Where(h=>h.DangHienThi&&h.TrangThaiDuyet=="DaDuyet"&&(h.DayTrucTuyen||h.DayTrucTiep)
        &&db.Set<GiaSu>().Any(g=>g.TaiKhoanId==h.GiaSuId)&&db.Set<NguoiThamGia>().Any(u=>u.Id==h.GiaSuId&&u.LoaiNguoi=="GiaSu")
        &&db.Set<HoSoGiaSuMonDay>().Any(d=>d.HoSoGiaSuId==h.Id)
        &&!db.Set<HoSoGiaSuMonDay>().Any(d=>d.HoSoGiaSuId==h.Id&&(d.HocPhiDeXuat==null||d.DonViHocPhi==null||!db.Set<MonHoc>().Any(m=>m.Id==d.MonHocId&&m.DangSuDung))));
    private IQueryable<UngVienViewModel> NhuCauCongKhai()=>from n in db.Set<NhuCauHoc>() join u in db.Set<NguoiThamGia>() on n.ChuSoHuuId equals u.Id join m in db.Set<MonHoc>() on n.MonHocId equals m.Id
        where n.LoaiNhuCau=="TimGiaSu"&&n.TrangThai=="DangMo"&&(u.LoaiNguoi=="HocSinh"||u.LoaiNguoi=="PhuHuynh")
        select new UngVienViewModel{Id=n.Id,ChuSoHuuId=u.Id,HoTen=u.HoTen,LoaiNguoi=u.LoaiNguoi,TenMon=m.TenMon,MonHocId=m.Id,KhoiLop=n.KhoiLop,MucTieu=n.MucTieu,HinhThucHoc=n.HinhThucHoc,KhuVuc=n.KhuVuc,HocPhiDuKien=n.HocPhiDuKien,DonViHocPhi=n.DonViHocPhi,TaoLuc=n.TaoLuc};
    private IQueryable<HoSoGiaSu> LocHoSo(BoLocTimKiemViewModel f)
    {
        var q=HoSoCongKhai();var key=HoSoGiaSuService.ChuanHoa(f.TuKhoa);var area=HoSoGiaSuService.ChuanHoa(f.KhuVuc);
        if(key!="")q=q.Where(h=>db.Set<NguoiThamGia>().Any(u=>u.Id==h.GiaSuId&&EF.Functions.Collate(u.HoTen,"Vietnamese_100_CI_AS").Contains(key))||EF.Functions.Collate(h.ChuyenMon,"Vietnamese_100_CI_AS").Contains(key)||EF.Functions.Collate(h.MoTa,"Vietnamese_100_CI_AS").Contains(key)||db.Set<HoSoGiaSuMonDay>().Any(d=>d.HoSoGiaSuId==h.Id&&db.Set<MonHoc>().Any(m=>m.Id==d.MonHocId&&EF.Functions.Collate(m.TenMon,"Vietnamese_100_CI_AS").Contains(key))));
        if(area!="")q=q.Where(h=>EF.Functions.Collate(h.KhuVuc,"Vietnamese_100_CI_AS").Contains(area));
        if(!string.IsNullOrWhiteSpace(f.TrinhDo))q=q.Where(h=>h.TrinhDo.Contains(f.TrinhDo));
        if(f.HinhThucHoc=="TrucTuyen")q=q.Where(h=>h.DayTrucTuyen);
        if(f.HinhThucHoc=="TrucTiep")q=q.Where(h=>h.DayTrucTiep);
        var ids=f.MonHocIds.Concat(f.MonHocId.HasValue?[f.MonHocId.Value]:Array.Empty<int>()).Distinct().ToList();
        var byPrice=f.SapXep is "HocPhiTang" or "HocPhiGiam";
        return q.Where(h=>db.Set<HoSoGiaSuMonDay>().Any(d=>d.HoSoGiaSuId==h.Id
            &&(ids.Count==0||ids.Contains(d.MonHocId))
            &&(string.IsNullOrEmpty(f.KhoiLop)||EF.Functions.Collate(d.PhamViDay,"Vietnamese_100_CI_AS").Contains(f.KhoiLop))
            &&(f.HocPhiToiDa==null||(d.HocPhiDeXuat<=f.HocPhiToiDa&&d.DonViHocPhi==f.DonViHocPhi))
            &&(!byPrice||(d.HocPhiDeXuat!=null&&d.DonViHocPhi==f.DonViHocPhi))));
    }
    private async Task<List<UngVienViewModel>> HoSoRowsAsync(List<HoSoGiaSu> hs)
    {
        var ids=hs.Select(h=>h.Id).ToList();var owners=hs.Select(h=>h.GiaSuId).ToList();var people=await db.Set<NguoiThamGia>().Where(u=>owners.Contains(u.Id)).ToDictionaryAsync(x=>x.Id,x=>x.HoTen);
        var subjects=await (from d in db.Set<HoSoGiaSuMonDay>() join m in db.Set<MonHoc>() on d.MonHocId equals m.Id where ids.Contains(d.HoSoGiaSuId) select new{d.HoSoGiaSuId,Row=new MonDayCongKhai{MonHocId=m.Id,TenMon=m.TenMon,PhamViDay=d.PhamViDay,HocPhi=d.HocPhiDeXuat,DonVi=d.DonViHocPhi}}).ToListAsync();
        return hs.Select(h=>new UngVienViewModel{Id=h.Id,ChuSoHuuId=h.GiaSuId,LoaiNguoi="GiaSu",HoTen=people[h.GiaSuId],TrinhDo=h.TrinhDo,ChuyenMon=h.ChuyenMon,KinhNghiem=h.KinhNghiem,MoTa=h.MoTa,KhuVuc=h.KhuVuc,KhuVucGiaSu=h.KhuVuc,DayTrucTuyen=h.DayTrucTuyen,DayTrucTiep=h.DayTrucTiep,HinhThucHoc=h.DayTrucTuyen&&h.DayTrucTiep?"CaHai":h.DayTrucTuyen?"TrucTuyen":"TrucTiep",TaoLuc=h.DuyetLuc??DateTimeOffset.MinValue,MonTheoHoSo=subjects.Where(s=>s.HoSoGiaSuId==h.Id).Select(s=>s.Row).OrderBy(s=>s.TenMon).ToList()}).ToList();
    }
    public async Task<SearchPage> TimAsync(BoLocTimKiemViewModel f)
    {
        var me=await current.HienTaiAsync();Rules.Check(f.Trang is >=1 and <=100000,"Trang không hợp lệ.");
        Rules.Check(f.HocPhiToiDa==null||f.HocPhiToiDa>=0,"Học phí tối đa không âm.");
        Rules.Check(f.DonViHocPhi is "VND/Gio" or "VND/Buoi","Chọn đúng đơn vị học phí.");
        if(me.LoaiNguoi!="GiaSu")
        {
            var q=LocHoSo(f);var total=await q.CountAsync();
            var ids=f.MonHocIds.Concat(f.MonHocId.HasValue?[f.MonHocId.Value]:Array.Empty<int>()).ToList();
            var priced=from h in q select new{h,Price=db.Set<HoSoGiaSuMonDay>().Where(d=>d.HoSoGiaSuId==h.Id&&(ids.Count==0||ids.Contains(d.MonHocId))&&(string.IsNullOrEmpty(f.KhoiLop)||EF.Functions.Collate(d.PhamViDay,"Vietnamese_100_CI_AS").Contains(f.KhoiLop))&&d.DonViHocPhi==f.DonViHocPhi&&(f.HocPhiToiDa==null||d.HocPhiDeXuat<=f.HocPhiToiDa)).Min(d=>d.HocPhiDeXuat),Name=db.Set<NguoiThamGia>().Where(u=>u.Id==h.GiaSuId).Select(u=>u.HoTen).First()};
            var ordered=f.SapXep switch{"HocPhiTang"=>priced.OrderBy(x=>x.Price).ThenBy(x=>x.h.Id),"HocPhiGiam"=>priced.OrderByDescending(x=>x.Price).ThenBy(x=>x.h.Id),"Ten"=>priced.OrderBy(x=>x.Name).ThenBy(x=>x.h.Id),_=>priced.OrderByDescending(x=>x.h.DuyetLuc).ThenBy(x=>x.h.Id)};
            var rows=await HoSoRowsAsync(await ordered.Skip((f.Trang-1)*9).Take(9).Select(x=>x.h).ToListAsync());
            if(ids.Count>0)foreach(var row in rows)row.MonTheoHoSo=row.MonTheoHoSo.Where(m=>ids.Contains(m.MonHocId)).ToList();
            return new(){Filter=f,Items=rows,Total=total};
        }
        Rules.Check(f.ThuTrongTuan==null||f.ThuTrongTuan<=6,"Thứ không hợp lệ.");Rules.Check(f.GioBatDau==null||f.GioKetThuc==null||f.GioBatDau<f.GioKetThuc,"Giờ bắt đầu phải trước giờ kết thúc.");
        var n=NhuCauCongKhai().Where(x=>x.ChuSoHuuId!=me.Id);
        if(!string.IsNullOrWhiteSpace(f.TuKhoa))n=n.Where(x=>x.HoTen.Contains(f.TuKhoa)||x.TenMon.Contains(f.TuKhoa)||(x.MucTieu!=null&&x.MucTieu.Contains(f.TuKhoa)));
        if(f.MonHocId!=null)n=n.Where(x=>x.MonHocId==f.MonHocId);
        if(!string.IsNullOrWhiteSpace(f.KhoiLop))n=n.Where(x=>x.KhoiLop==f.KhoiLop);
        if(!string.IsNullOrWhiteSpace(f.HinhThucHoc))n=n.Where(x=>x.HinhThucHoc==f.HinhThucHoc);
        if(!string.IsNullOrWhiteSpace(f.KhuVuc))n=n.Where(x=>x.KhuVuc!=null&&x.KhuVuc.Contains(f.KhuVuc));
        if(f.HocPhiToiDa!=null)n=n.Where(x=>x.HocPhiDuKien<=f.HocPhiToiDa&&x.DonViHocPhi==f.DonViHocPhi);
        if(f.ThuTrongTuan!=null||f.GioBatDau!=null||f.GioKetThuc!=null)n=n.Where(x=>db.Set<KhungGioRanh>().Any(s=>s.NhuCauId==x.Id&&(f.ThuTrongTuan==null||s.ThuTrongTuan==f.ThuTrongTuan)&&(f.GioBatDau==null||s.GioBatDau<=f.GioBatDau)&&(f.GioKetThuc==null||s.GioKetThuc>=f.GioKetThuc)));
        if(f.SapXep is "HocPhiTang" or "HocPhiGiam")n=n.Where(x=>x.HocPhiDuKien!=null&&x.DonViHocPhi==f.DonViHocPhi);
        var count=await n.CountAsync();var sorted=f.SapXep switch{"HocPhiTang"=>n.OrderBy(x=>x.HocPhiDuKien).ThenBy(x=>x.Id),"HocPhiGiam"=>n.OrderByDescending(x=>x.HocPhiDuKien).ThenBy(x=>x.Id),"Ten"=>n.OrderBy(x=>x.HoTen).ThenBy(x=>x.Id),_=>n.OrderByDescending(x=>x.TaoLuc).ThenBy(x=>x.Id)};
        var items=await sorted.Skip((f.Trang-1)*9).Take(9).ToListAsync();await SlotsAsync(items);return new(){Filter=f,Items=items,Total=count};
    }
    private async Task SlotsAsync(List<UngVienViewModel> rows)
    {
        var ids=rows.Select(x=>x.Id).ToList();var slots=await db.Set<KhungGioRanh>().Where(s=>ids.Contains(s.NhuCauId)).ToListAsync();
        foreach(var row in rows)row.Slots=slots.Where(s=>s.NhuCauId==row.Id).Select(s=>new SlotInput{ThuTrongTuan=s.ThuTrongTuan,GioBatDau=s.GioBatDau,GioKetThuc=s.GioKetThuc}).ToList();
    }
    public async Task<UngVienViewModel> ChiTietAsync(Guid id)
    {
        var h=await HoSoCongKhai().SingleOrDefaultAsync(x=>x.Id==id);
        if(h!=null)return (await HoSoRowsAsync([h])).Single();
        var n=await NhuCauCongKhai().SingleOrDefaultAsync(x=>x.Id==id)??throw new LoiNghiepVu("Hồ sơ hoặc nhu cầu đang ẩn, chưa được duyệt hoặc không còn công khai.",404);
        await SlotsAsync([n]);return n;
    }
    public async Task<UngVienViewModel> XemTruocAsync()
    {
        var me=await current.HienTaiAsync();Rules.Check(me.LoaiNguoi=="GiaSu","Chỉ gia sư xem trước hồ sơ của mình.",403);
        var h=await db.Set<HoSoGiaSu>().AsNoTracking().SingleOrDefaultAsync(x=>x.GiaSuId==me.Id)??throw new LoiNghiepVu("Hãy lưu hồ sơ trước.");
        return (await HoSoRowsAsync([h])).Single();
    }
    public async Task<List<UngVienViewModel>> LienQuanAsync(Guid owner)
    {
        var rows=await NhuCauCongKhai().Where(x=>x.ChuSoHuuId==owner).OrderByDescending(x=>x.TaoLuc).ToListAsync();await SlotsAsync(rows);return rows;
    }
    public async Task<List<UngVienViewModel>> SoSanhAsync(Guid[] ids)
    {
        Rules.Check(ids.Distinct().Count() is >=1 and <=3,"Chọn từ 1 đến 3 đối tượng.");var rows=new List<UngVienViewModel>();
        foreach(var id in ids.Distinct())rows.Add(await ChiTietAsync(id));return rows;
    }
    public async Task<(List<UngVienViewModel> Items,bool HasNeed)> GoiYAsync()
    {
        var me=await current.HienTaiAsync();var own=await db.Set<NhuCauHoc>().AsNoTracking().Where(x=>x.ChuSoHuuId==me.Id&&x.LoaiNhuCau=="TimGiaSu"&&x.TrangThai=="DangMo").ToListAsync();
        if(me.LoaiNguoi=="GiaSu")
        {
            var subjects=await (from d in db.Set<HoSoGiaSuMonDay>() join h in db.Set<HoSoGiaSu>() on d.HoSoGiaSuId equals h.Id where h.GiaSuId==me.Id select d.MonHocId).ToListAsync();
            var q=NhuCauCongKhai();if(subjects.Count>0)q=q.Where(x=>subjects.Contains(x.MonHocId));
            var rows=await q.OrderByDescending(x=>x.TaoLuc).Take(3).ToListAsync();await SlotsAsync(rows);foreach(var n in rows)if(subjects.Contains(n.MonHocId))n.LyDo=["Cùng môn "+n.TenMon];return(rows,subjects.Count>0);
        }
        var candidates=await HoSoRowsAsync(await HoSoCongKhai().ToListAsync());
        foreach(var c in candidates){var match=own.FirstOrDefault(n=>c.MonTheoHoSo.Any(m=>m.MonHocId==n.MonHocId));if(match!=null){c.LyDo.Add("Cùng môn "+c.MonTheoHoSo.First(m=>m.MonHocId==match.MonHocId).TenMon);if((match.HinhThucHoc=="TrucTuyen"&&c.DayTrucTuyen)||(match.HinhThucHoc=="TrucTiep"&&c.DayTrucTiep))c.LyDo.Add("Cùng hình thức học");}}
        return(candidates.OrderByDescending(x=>x.LyDo.Count).ThenByDescending(x=>x.TaoLuc).ThenBy(x=>x.Id).Take(3).ToList(),own.Count>0);
    }
}
