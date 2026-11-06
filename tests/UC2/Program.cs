using Microsoft.Extensions.DependencyInjection;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Areas.KetNoi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

var cs=Environment.GetEnvironmentVariable("EDU_TEST_CONNECTION") ?? throw new Exception("EDU_TEST_CONNECTION required");
if(!new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(cs).InitialCatalog.StartsWith("EduConnect_Verification_"))throw new Exception("Test database only");
AppDbContext Db()=>new(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(cs).Options);
var tutor=Guid.Parse("10000000-0000-0000-0000-000000000001");var learner=Guid.Parse("10000000-0000-0000-0000-000000000002");
KetNoiService Service(AppDbContext db,Guid actor){var http=new DefaultHttpContext{Session=new TestSession()};http.Session.SetString("participant",actor.ToString());var user=new NguoiDungHienTaiService(db,new HttpContextAccessor{HttpContext=http});return new(db,user,new ThongBaoService(db,user));}
var count=0;void Check(bool value,string name){if(!value)throw new Exception(name);Console.WriteLine("PASS "+name);count++;}
using var fixture=Db();var original=await fixture.Set<LopHoc>().SingleAsync(x=>x.Id==Guid.Parse("50000000-0000-0000-0000-000000000001"));
var req=await fixture.Set<YeuCauKetNoiLopHoc>().SingleAsync(x=>x.Id==original.YeuCauKetNoiId);var copy=(YeuCauKetNoiLopHoc)fixture.Entry(req).CurrentValues.ToObject();copy.Id=Guid.NewGuid();copy.RowVersion=[];fixture.Add(copy);await fixture.SaveChangesAsync();
var lop=(LopHoc)fixture.Entry(original).CurrentValues.ToObject();lop.Id=Guid.NewGuid();lop.YeuCauKetNoiId=copy.Id;lop.RowVersion=[];fixture.Add(lop);await fixture.SaveChangesAsync();
using(var db=Db())Check(!(await Service(db,tutor).TaoYeuCauHuyAsync(lop.Id,"Test",lop.RowVersion,learner,default)).ThanhCong,"Forged actor denied");
using(var db=Db())Check(!(await Service(db,tutor).TaoYeuCauHuyAsync(lop.Id,"Test",new byte[8],tutor,default)).ThanhCong,"Stale class version denied");
Guid id;using(var db=Db()){var result=await Service(db,tutor).TaoYeuCauHuyAsync(lop.Id,"Hoàn tất mục tiêu",lop.RowVersion,tutor,default);Check(result.ThanhCong,"Create end request");id=result.DuLieu;}
YeuCauHuyKetNoi request;using(var db=Db()){request=await db.Set<YeuCauHuyKetNoi>().AsNoTracking().SingleAsync(x=>x.Id==id);Check(request.HetHanLuc-request.GuiLuc==TimeSpan.FromHours(72),"72 hour deadline");}
using(var db=Db())Check(!(await Service(db,tutor).PhanHoiHuyAsync(id,true,request.RowVersion,tutor,default)).ThanhCong,"Sender cannot accept own request");
using(var db=Db())Check(!(await Service(db,learner).PhanHoiHuyAsync(id,true,new byte[8],learner,default)).ThanhCong,"Stale response denied");
using(var db=Db())Check((await Service(db,learner).PhanHoiHuyAsync(id,true,request.RowVersion,learner,default)).ThanhCong,"Recipient accepts");
using(var db=Db()){var ended=await db.Set<LopHoc>().SingleAsync(x=>x.Id==lop.Id);Check(ended.TrangThai=="DaKetThuc"&&ended.KetThucLuc!=null&&ended.CachKetThuc=="DongY","End metadata persisted");Check((await db.Set<YeuCauHuyKetNoi>().SingleAsync(x=>x.Id==id)).NguoiPhanHoiId==learner,"Responding actor persisted");}
using(var db=Db())Check(!(await Service(db,learner).PhanHoiHuyAsync(id,true,request.RowVersion,learner,default)).ThanhCong,"Repeated response denied");

async Task<(LopHoc Lop,YeuCauHuyKetNoi Request)> Prepare()
{
 using var db=Db();var source=await db.Set<YeuCauKetNoiLopHoc>().SingleAsync(x=>x.Id==original.YeuCauKetNoiId);
 var rr=(YeuCauKetNoiLopHoc)db.Entry(source).CurrentValues.ToObject();rr.Id=Guid.NewGuid();rr.RowVersion=[];db.Add(rr);await db.SaveChangesAsync();
 var cc=(LopHoc)fixture.Entry(original).CurrentValues.ToObject();cc.Id=Guid.NewGuid();cc.YeuCauKetNoiId=rr.Id;cc.RowVersion=[];db.Add(cc);await db.SaveChangesAsync();
 var result=await Service(db,tutor).TaoYeuCauHuyAsync(cc.Id,"Kết thúc",cc.RowVersion,tutor,default);
 return(cc,await db.Set<YeuCauHuyKetNoi>().AsNoTracking().SingleAsync(x=>x.Id==result.DuLieu));
}
var declined=await Prepare();
using(var db=Db())Check((await Service(db,learner).PhanHoiHuyAsync(declined.Request.Id,false,declined.Request.RowVersion,learner,default)).ThanhCong,"Recipient can decline");
using(var db=Db())Check((await db.Set<LopHoc>().SingleAsync(x=>x.Id==declined.Lop.Id)).TrangThai=="DangHoatDong","Decline keeps class active");
var expired=await Prepare();
using(var db=Db())await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.YeuCauHuyKetNoi SET GuiLuc=DATEADD(hour,-73,SYSDATETIMEOFFSET()),HetHanLuc=DATEADD(hour,-1,SYSDATETIMEOFFSET()) WHERE Id={expired.Request.Id}");
using(var db=Db()){var latest=await db.Set<YeuCauHuyKetNoi>().AsNoTracking().SingleAsync(x=>x.Id==expired.Request.Id);Check(!(await Service(db,learner).PhanHoiHuyAsync(latest.Id,true,latest.RowVersion,learner,default)).ThanhCong,"Late response denied");}
var services=new Microsoft.Extensions.DependencyInjection.ServiceCollection();
services.AddDbContext<AppDbContext>(o=>o.UseSqlServer(cs));services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor());services.AddScoped<NguoiDungHienTaiService>();services.AddScoped<ThongBaoService>();
using var provider=services.BuildServiceProvider();var worker=new BaoTriWorker(provider.GetRequiredService<IServiceScopeFactory>(),Microsoft.Extensions.Logging.Abstractions.NullLogger<BaoTriWorker>.Instance);
async Task Sweep()=>await (Task)typeof(BaoTriWorker).GetMethod("XuLyTuHuyKetNoiAsync",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)!.Invoke(worker,[DateTimeOffset.UtcNow,CancellationToken.None])!;
await Sweep();int notifications;
using(var db=Db()){var ended=await db.Set<LopHoc>().SingleAsync(x=>x.Id==expired.Lop.Id);Check(ended.TrangThai=="DaKetThuc"&&ended.CachKetThuc=="HetHan"&&ended.KetThucLuc!=null,"Worker saves automatic end metadata");notifications=await db.Set<ThongBao>().CountAsync(x=>x.DoiTuongId==expired.Lop.Id);Check(notifications==3,"Automatic end notifies both participants");}
await Sweep();using(var db=Db())Check(await db.Set<ThongBao>().CountAsync(x=>x.DoiTuongId==expired.Lop.Id)==notifications,"Repeated worker sweep is idempotent");

var newLearner=Guid.NewGuid();var contact=Guid.NewGuid();
using(var db=Db()) await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.HoSoGiaSu SET DangHienThi=1,TrangThaiDuyet='DaDuyet' WHERE GiaSuId={tutor}");
using(var db=Db()){db.Add(new NguoiThamGia{Id=newLearner,HoTen="UC2 learner",LoaiNguoi="HocSinh"});await db.SaveChangesAsync();db.Add(new YeuCauLienHe{Id=contact,GiaSuId=tutor,BenHocId=newLearner,NguoiGuiId=newLearner,LoiNhan="Test",TrangThai="ChapNhan",GuiLuc=DateTimeOffset.UtcNow.AddMinutes(-1),PhanHoiLuc=DateTimeOffset.UtcNow,NguoiPhanHoiId=tutor});await db.SaveChangesAsync();}
Guid connection;
using(var db=Db()){var result=await Service(db,newLearner).GuiYeuCauAsync(new EduConnect.Mvc.Areas.KetNoi.ViewModels.GuiYeuCauViewModel{LienHeId=contact,LoiNhan="Học Toán"},newLearner,default);Check(result.ThanhCong,"Learner sends connection request");connection=result.DuLieu;}
using(var db=Db()){var result=await Service(db,newLearner).GuiYeuCauAsync(new EduConnect.Mvc.Areas.KetNoi.ViewModels.GuiYeuCauViewModel{LienHeId=contact,LoiNhan="Duplicate"},newLearner,default);Check(!result.ThanhCong&&result.MaLoi=="409","Duplicate request returns business error");}
DateTime start;
using(var db=Db()){var latest=await (from l in db.Set<LichHoc>() join c in db.Set<LopHoc>() on l.LopHocId equals c.Id where c.GiaSuId==tutor select (DateTimeOffset?)l.KetThucLuc).MaxAsync();start=(latest?.DateTime.Date ?? DateTime.Today).AddDays(40);}
byte[] version;
using(var db=Db())version=(await db.Set<YeuCauKetNoiLopHoc>().SingleAsync(x=>x.Id==connection)).RowVersion;
using(var db=Db()){var result=await Service(db,tutor).GuiDeXuatAsync(new EduConnect.Mvc.Areas.KetNoi.ViewModels.DeXuatLopViewModel{LienHeId=contact,Id=connection,RowVersion=Convert.ToBase64String(version),TenLop="UC2 Toán",MonHocId=1,MucTieu="Ôn tập",SoBuoiTuan=1,ThoiLuongPhut=90,BatDauHocLuc=start,HocPhi=150000,DonViHocPhi="VND/Gio",LichRanh=[new(){ThuTrongTuan=(byte)start.DayOfWeek,GioBatDau=new(8,0),GioKetThuc=new(9,30)}]},tutor,default);Check(result.ThanhCong,"Tutor submits structured proposal");}
using(var db=Db())version=(await db.Set<YeuCauKetNoiLopHoc>().SingleAsync(x=>x.Id==connection)).RowVersion;
using(var db=Db()){var result=await Service(db,newLearner).ChapNhanDeXuatAsync(connection,new byte[8],newLearner,default);Check(!result.ThanhCong&&result.MaLoi=="409","Stale proposal rejected");}
async Task<KetQua<Guid>> Accept(){using var db=Db();return await Service(db,newLearner).ChapNhanDeXuatAsync(connection,version,newLearner,default);}
var accepted=await Task.WhenAll(Accept(),Accept());Check(accepted.All(x=>x.ThanhCong)&&accepted[0].DuLieu==accepted[1].DuLieu,"Concurrent acceptance returns one class");
using(var db=Db()){var idClass=accepted[0].DuLieu;Check(await db.Set<LopHoc>().CountAsync(x=>x.YeuCauKetNoiId==connection)==1,"Only one class persisted");Check(await db.Set<ThongBao>().CountAsync(x=>x.DoiTuongId==idClass)==1,"Concurrent acceptance sends one notification");Check(await db.Set<LichHoc>().CountAsync(x=>x.LopHocId==idClass)==1,"Initial schedule persisted once");Check((await db.Set<LopHoc>().SingleAsync(x=>x.Id==idClass)).DonViHocPhi=="VND/Gio","Fee unit retained");}

Guid overlap;
using(var db=Db())overlap=(await Service(db,newLearner).GuiYeuCauAsync(new EduConnect.Mvc.Areas.KetNoi.ViewModels.GuiYeuCauViewModel{LienHeId=contact,LoiNhan="Rollback test"},newLearner,default)).DuLieu;
using(var db=Db()){var row=await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x=>x.Id==overlap);var result=await Service(db,tutor).GuiDeXuatAsync(new EduConnect.Mvc.Areas.KetNoi.ViewModels.DeXuatLopViewModel{LienHeId=contact,Id=overlap,RowVersion=Convert.ToBase64String(row.RowVersion),TenLop="Overlap",MonHocId=1,MucTieu="Rollback",SoBuoiTuan=1,ThoiLuongPhut=90,BatDauHocLuc=start,HocPhi=150000,DonViHocPhi="VND/Gio",LichRanh=[new(){ThuTrongTuan=(byte)start.DayOfWeek,GioBatDau=new(8,0),GioKetThuc=new(9,30)}]},tutor,default);Check(result.ThanhCong,"Proposal can be checked before acceptance");}
using(var db=Db()){var row=await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x=>x.Id==overlap);var result=await Service(db,newLearner).ChapNhanDeXuatAsync(overlap,row.RowVersion,newLearner,default);Check(!result.ThanhCong&&result.MaLoi=="409","Schedule conflict returns business error");}
using(var db=Db()){Check(!await db.Set<LopHoc>().AnyAsync(x=>x.YeuCauKetNoiId==overlap),"Schedule failure rolls back class creation");Check((await db.Set<YeuCauKetNoiLopHoc>().SingleAsync(x=>x.Id==overlap)).TrangThai=="ChoBenHoc","Schedule failure preserves pending proposal");}
Console.WriteLine($"{count} checks passed");
sealed class TestSession:ISession{readonly Dictionary<string,byte[]> values=[];public bool IsAvailable=>true;public string Id=>"uc2-test";public IEnumerable<string> Keys=>values.Keys;public void Clear()=>values.Clear();public Task CommitAsync(CancellationToken t=default)=>Task.CompletedTask;public Task LoadAsync(CancellationToken t=default)=>Task.CompletedTask;public void Remove(string key)=>values.Remove(key);public void Set(string key,byte[] value)=>values[key]=value;public bool TryGetValue(string key,out byte[] value)=>values.TryGetValue(key,out value!);}
