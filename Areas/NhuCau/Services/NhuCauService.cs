using System.Data;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.NhuCau.Services;
public sealed class NhuCauService(AppDbContext db,NguoiDungMauService current,MonHocCatalog catalog) {
 public Task<List<MonHoc>> MonTenAsync()=>db.Set<MonHoc>().AsNoTracking().ToListAsync();
 public async Task<List<NhuCauHoc>> LayAsync() {var u=await current.HienTaiAsync();return await db.Set<NhuCauHoc>().AsNoTracking().Where(x=>x.ChuSoHuuId==u.Id).OrderByDescending(x=>x.TaoLuc).ToListAsync();}
 public async Task<Guid> LuuAsync(NhuCauInput m) {
  await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
  var subjects=m.MonHocIds.Distinct().ToList();
  if(m.ChonMonKhac)subjects.Add(await catalog.LayHoacThemAsync(m.TenMonKhac));
  subjects=subjects.Distinct().ToList();
  Rules.Check(subjects.Count>0&&subjects.Count<=10,"Chọn ít nhất một môn học, tối đa 10 môn.");
  Rules.Check(m.Id==null||subjects.Count==1,"Mỗi nhu cầu hiện có thuộc một môn. Để thêm nhiều môn, hãy tạo nhu cầu mới.");
  Guid first=Guid.Empty;
  foreach(var subject in subjects){m.MonHocId=subject;var id=await LuuMotAsync(m);if(first==Guid.Empty)first=id;}
  await tx.CommitAsync();return first;
 }
 public Task<List<MonHoc>> MonAsync()=>db.Set<MonHoc>().AsNoTracking().Where(x=>x.DangSuDung&&MonHocCatalog.PhoThong.Contains(x.TenMon)).GroupBy(x=>x.TenMon).Select(g=>g.OrderBy(x=>x.Id).First()).ToListAsync();
 public async Task<List<MonHoc>> MonDuocChonAsync(){var me=await current.HienTaiAsync();if(me.LoaiNguoi!="GiaSu")return await MonAsync();return await (from m in db.Set<MonHoc>() join s in db.Set<HoSoGiaSuMonDay>() on m.Id equals s.MonHocId join h in db.Set<HoSoGiaSu>() on s.HoSoGiaSuId equals h.Id where h.GiaSuId==me.Id&&m.DangSuDung select m).AsNoTracking().ToListAsync();}
 public async Task<HashSet<Guid>> DaKetNoiAsync(){var me=await current.HienTaiAsync();return (await (from n in db.Set<NhuCauHoc>() join r in db.Set<YeuCauKetNoiLopHoc>() on n.Id equals r.NhuCauId join c in db.Set<LopHoc>() on r.Id equals c.YeuCauKetNoiId where n.ChuSoHuuId==me.Id select n.Id).Distinct().ToListAsync()).ToHashSet();}
 public async Task<NhuCauInput> LayDeSuaAsync(Guid? id) {if(id==null)return new();var me=await current.HienTaiAsync();var n=await db.Set<NhuCauHoc>().AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id&&x.ChuSoHuuId==me.Id)??throw new LoiNghiepVu("Không tìm thấy nhu cầu của bạn.",404);return new(){Id=n.Id,RowVersion=Convert.ToBase64String(n.RowVersion),MonHocId=n.MonHocId,MonHocIds=[n.MonHocId],ChonMonKhac=!await db.Set<MonHoc>().AnyAsync(x=>x.Id==n.MonHocId&&MonHocCatalog.PhoThong.Contains(x.TenMon)),TenMonKhac=await db.Set<MonHoc>().Where(x=>x.Id==n.MonHocId).Select(x=>x.TenMon).SingleAsync(),KhoiLop=n.KhoiLop,HocLuc=n.HocLuc,MucTieu=n.MucTieu,SoBuoiTuan=n.SoBuoiTuan,HinhThucHoc=n.HinhThucHoc,KhuVuc=n.KhuVuc,DiaChiChiTiet=n.DiaChiChiTiet,HocPhiDuKien=n.HocPhiDuKien,DonViHocPhi=n.DonViHocPhi,TenNguoiHoc=n.TenNguoiHoc,TuoiNguoiHoc=n.TuoiNguoiHoc,BanNhap=n.TrangThai=="BanNhap",LichRanh=await db.Set<KhungGioRanh>().Where(x=>x.NhuCauId==id).Select(x=>new SlotInput{ThuTrongTuan=x.ThuTrongTuan,GioBatDau=x.GioBatDau,GioKetThuc=x.GioKetThuc}).ToListAsync()};}
 private async Task<Guid> LuuMotAsync(NhuCauInput m) {var u=await current.HienTaiAsync();Rules.FeeCheck(m.HocPhiDuKien,m.DonViHocPhi);if(!m.BanNhap||m.LichRanh.Count>0)Rules.Slots(m.LichRanh);
  Rules.Check(await db.Set<MonHoc>().AnyAsync(x=>x.Id==m.MonHocId&&x.DangSuDung),"Môn học không hợp lệ.");
  if(!m.BanNhap){Rules.Check(!string.IsNullOrWhiteSpace(m.MucTieu)&&m.SoBuoiTuan is >=1 and <=7,"Cần mục tiêu và số buổi từ 1 đến 7.");Rules.Check(m.HinhThucHoc!="TrucTiep"||!string.IsNullOrWhiteSpace(m.KhuVuc),"Cần khu vực học trực tiếp.");if(u.LoaiNguoi=="PhuHuynh")Rules.Check(!string.IsNullOrWhiteSpace(m.TenNguoiHoc)&&m.TuoiNguoiHoc is >=1 and <=120,"Nhập tên và tuổi người học.");}

  if(u.LoaiNguoi=="GiaSu")Rules.Check(await (from h in db.Set<HoSoGiaSu>() join s in db.Set<HoSoGiaSuMonDay>() on h.Id equals s.HoSoGiaSuId where h.GiaSuId==u.Id&&s.MonHocId==m.MonHocId select s).AnyAsync(),"Chỉ chọn môn đã đăng ký trong hồ sơ.");
  var n=m.Id==null?new NhuCauHoc{Id=Guid.NewGuid(),ChuSoHuuId=u.Id,LoaiNhuCau=u.LoaiNguoi=="GiaSu"?"NhanLop":"TimGiaSu",TaoLuc=DateTimeOffset.UtcNow}:await db.Set<NhuCauHoc>().SingleOrDefaultAsync(x=>x.Id==m.Id&&x.ChuSoHuuId==u.Id)??throw new LoiNghiepVu("Nhu cầu không thuộc về bạn.",403);
  if(m.Id==null)db.Add(n);else {Rules.Concurrency(db,n,m.RowVersion);db.Entry(n).Property(x=>x.MucTieu).IsModified=true;}
  n.MonHocId=m.MonHocId;n.KhoiLop=m.KhoiLop;n.HocLuc=m.HocLuc;n.MucTieu=m.MucTieu?.Trim();n.SoBuoiTuan=m.SoBuoiTuan;n.HinhThucHoc=m.HinhThucHoc;n.KhuVuc=m.HinhThucHoc=="TrucTiep"?m.KhuVuc:null;n.DiaChiChiTiet=m.HinhThucHoc=="TrucTiep"?m.DiaChiChiTiet:null;n.HocPhiDuKien=m.HocPhiDuKien;n.DonViHocPhi=m.DonViHocPhi;n.TenNguoiHoc=u.LoaiNguoi=="PhuHuynh"?m.TenNguoiHoc:null;n.TuoiNguoiHoc=u.LoaiNguoi=="PhuHuynh"?m.TuoiNguoiHoc:null;n.TrangThai=m.BanNhap?"BanNhap":"DangMo";
  await db.SaveChangesAsync();await db.Set<KhungGioRanh>().Where(x=>x.NhuCauId==n.Id).ExecuteDeleteAsync();
  foreach(var s in m.LichRanh)db.Add(new KhungGioRanh{Id=Guid.NewGuid(),NhuCauId=n.Id,ThuTrongTuan=s.ThuTrongTuan,GioBatDau=s.GioBatDau,GioKetThuc=s.GioKetThuc});await db.SaveChangesAsync();return n.Id;
 }
 public async Task TrangThaiAsync(Guid id,string state,string version) {Rules.Check(state is "DangMo" or "DaDong","Trạng thái không hợp lệ.");var u=await current.HienTaiAsync();await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);var n=await db.Set<NhuCauHoc>().SingleOrDefaultAsync(x=>x.Id==id&&x.ChuSoHuuId==u.Id)??throw new LoiNghiepVu("Nhu cầu không thuộc về bạn.",403);Rules.Concurrency(db,n,version);if(state=="DangMo") {Rules.Check(n.TrangThai=="DaDong","Hãy hoàn thiện bản nháp trước khi mở.");if(u.LoaiNguoi=="GiaSu")Rules.Check(await (from h in db.Set<HoSoGiaSu>() join s in db.Set<HoSoGiaSuMonDay>() on h.Id equals s.HoSoGiaSuId where h.GiaSuId==u.Id&&s.MonHocId==n.MonHocId select s).AnyAsync(),"Môn không còn trong hồ sơ.");}n.TrangThai=state;await db.SaveChangesAsync();await tx.CommitAsync();}
}

