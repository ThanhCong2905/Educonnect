using System.Net;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;
internal static partial class Cases
{
 static GhiNhanBuoiHocViewModel RecordModel(Fixture f,string rv)=>new(){LichHocId=f.Schedule,RowVersion=rv,KetQuaBuoiHoc="DaDienRa",NoiDung="Ã”n phÃ¢n sá»‘",NhanXet="HoÃ n thÃ nh",TrangThaiGiaSu="CoMat",TrangThaiBenHoc="VangKhongBaoTruoc"};
 static Dictionary<string,string> RecordForm(Fixture f,string rv)=>new(){{"LichHocId",f.Schedule.ToString()},{"RowVersion",rv},{"KetQuaBuoiHoc","DaDienRa"},{"NoiDung","Ã”n phÃ¢n sá»‘"},{"NhanXet","HoÃ n thÃ nh"},{"TrangThaiGiaSu","CoMat"},{"TrangThaiBenHoc","VangKhongBaoTruoc"}};
 static Dictionary<string,string> ScheduleForm(Fixture f,DateTime a,DateTime b)=>new(){{"LopHocId",f.Class.ToString()},{"Buoi[0].TieuDe","A"},{"Buoi[0].BatDauLuc",a.ToString("yyyy-MM-ddTHH:mm:ss")},{"Buoi[0].KetThucLuc",a.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss")},{"Buoi[1].TieuDe","B"},{"Buoi[1].BatDauLuc",b.ToString("yyyy-MM-ddTHH:mm:ss")},{"Buoi[1].KetThucLuc",b.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss")}};
 internal static async Task ScheduleCases(Fixture f,string key){switch(key){
 case "lesson_makeup":{
  await f.SeedLesson();await f.Sql("UPDATE dbo.LichHoc SET BatDauLuc=DATEADD(day,-2,SYSDATETIMEOFFSET()),KetThucLuc=DATEADD(hour,-1,SYSDATETIMEOFFSET()) WHERE Id=@s",new(){{"@s",f.Schedule}});
  var absent=new GhiNhanBuoiHocViewModel{LichHocId=f.Schedule,RowVersion=Convert.ToBase64String(await f.Version<BuoiHoc>(f.Lesson)),KetQuaBuoiHoc="KhongDienRa",NhanXet="Gia sư vắng, cần học bù",TrangThaiGiaSu="VangKhongBaoTruoc",TrangThaiBenHoc="CoMat",CanHocBu=true};
  await using(var tutor=f.Scope(f.G))await tutor.Schedules.GhiNhanAsync(absent);
  var start=DateTime.Today.AddDays(10).AddHours(19);var rv=Convert.ToBase64String(await f.Version<LichHoc>(f.Schedule));
  await using(var tutor=f.Scope(f.G))await tutor.Schedules.TaoBuoiBuAsync(new(){Id=f.Schedule,RowVersion=rv,Lich=new(){TieuDe="Học bù",BatDauLuc=start,KetThucLuc=start.AddHours(1)}});
  await using var verify=f.Db();var original=await verify.Set<BuoiHoc>().SingleAsync(x=>x.Id==f.Lesson);var makeup=await verify.Set<LichHoc>().SingleAsync(x=>x.LichGocId==f.Schedule&&x.LaBuoiBu);
  Assert.Equal("DaNghi",original.TrangThai);Assert.True(original.CanHocBu);Assert.Equal(f.Schedule,makeup.LichGocId);Assert.True(makeup.LaBuoiBu);Assert.Equal("DaLenLich",makeup.TrangThai);Assert.True(await verify.Set<BuoiHoc>().AnyAsync(x=>x.LichHocId==makeup.Id));
  break;}
 case "lesson_store":{
  await f.SeedLesson(false);await f.Sql("UPDATE dbo.LichHoc SET BatDauLuc=DATEADD(minute,-10,SYSDATETIMEOFFSET()),KetThucLuc=DATEADD(second,4,SYSDATETIMEOFFSET()) WHERE Id=@s",new(){{"@s",f.Schedule}});await using(var g=f.Scope(f.G))await g.Schedules.BatDauBuoiAsync(f.Schedule,Convert.ToBase64String(await f.Version<BuoiHoc>(f.Lesson)));await using(var db=f.Db())Assert.Equal("DangDienRa",(await db.Set<BuoiHoc>().SingleAsync(x=>x.Id==f.Lesson)).TrangThai);var deadline=DateTime.UtcNow.AddSeconds(10);while(true){var clock=await f.Sql("SELECT CASE WHEN KetThucLuc<SYSDATETIMEOFFSET() THEN 1 ELSE 0 END AS Ready FROM dbo.LichHoc WHERE Id=@s",new(){{"@s",f.Schedule}});if((int)clock[0]["Ready"]! == 1)break;Assert.True(DateTime.UtcNow<deadline,"Buá»•i khÃ´ng qua giá» trong deadline test");await Task.Delay(100);}await using(var g=f.Scope(f.G))await g.Schedules.GhiNhanAsync(RecordModel(f,Convert.ToBase64String(await f.Version<BuoiHoc>(f.Lesson))));await using(var db=f.Db()){var row=await db.Set<BuoiHoc>().SingleAsync(x=>x.LichHocId==f.Schedule);Assert.Equal(f.Lesson,row.Id);Assert.Equal("DaHoc",row.TrangThai);}break;}
 case "lesson_record_race":{
  await f.SeedLesson();var rv=Convert.ToBase64String(await f.Version<BuoiHoc>(f.Lesson));await using var a=f.Scope(f.G);await using var b=f.Scope(f.G);var ma=RecordModel(f,rv);var mb=RecordModel(f,rv);mb.NhanXet="Báº£n hai";var gate=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);async Task<Exception?> Go(Func<Task> run){await gate.Task;return await Capture(run);}var ta=Go(()=>a.Schedules.GhiNhanAsync(ma));var tb=Go(()=>b.Schedules.GhiNhanAsync(mb));gate.SetResult();var results=await Task.WhenAll(ta,tb);Assert.Single(results,x=>x==null);var error=Assert.Single(results,x=>x!=null);Assert.True(error is LoiNghiepVu or SqlException or DbUpdateConcurrencyException,error!.ToString());if(error is SqlException sql)Assert.Equal(51150,sql.Number);await using var db=f.Db();Assert.Single(await db.Set<BuoiHoc>().Where(x=>x.LichHocId==f.Schedule).ToListAsync());break;}
 case "schedule_http":case "schedule_conflict":{
  if(key=="schedule_http"){await f.SeedClass();using var g=await f.Client(f.G);var day=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).AddMonths(2);using var r=await f.Post(g,"/HocTap/LichHoc/Them",ScheduleForm(f,day.AddDays(-1).AddHours(19),day.AddHours(19)));Assert.Equal(HttpStatusCode.OK,r.StatusCode);await using var db=f.Db();var rows=await db.Set<LichHoc>().Where(x=>x.LopHocId==f.Class).OrderBy(x=>x.BatDauLuc).ToListAsync();Assert.Equal(2,rows.Count);Assert.Equal(day.AddDays(-1).AddHours(19),rows[0].BatDauLuc.ToOffset(TimeSpan.FromHours(7)).DateTime);Assert.Equal(day.AddHours(19),rows[1].BatDauLuc.ToOffset(TimeSpan.FromHours(7)).DateTime);break;}
  for(int dl=0;dl<3;dl++){await using var d=new Fixture(f.Id+"-DL"+dl);await d.Init();await d.SeedClass();var day=DateTime.Today.AddDays(8).AddHours(19);if(dl==1){await using var seed=d.Db();seed.Add(new LichHoc{Id=Guid.NewGuid(),LopHocId=d.Class,TieuDe="ÄÃ£ cÃ³",BatDauLuc=new(day,TimeSpan.FromHours(7)),KetThucLuc=new(day.AddHours(1),TimeSpan.FromHours(7)),TrangThai="DaLenLich"});await seed.SaveChangesAsync();}using var g=await d.Client(d.G);using var r=await d.Post(g,"/HocTap/LichHoc/Them",ScheduleForm(d,day,day.AddMinutes(dl==2?60:30)));Assert.Equal(dl==2?200:422,(int)r.StatusCode);await using var db=d.Db();Assert.Equal(dl==2?2:dl==1?1:0,await db.Set<LichHoc>().CountAsync(x=>x.LopHocId==d.Class));}break;}
 case "schedule_change":{
  await f.SeedLesson(false);await using(var db=f.Db()){db.Add(new BaiTapGiao{Id=Guid.NewGuid(),LopHocId=f.Class,BuoiHocId=f.Lesson,NguoiGiaoId=f.G,TieuDe="NhÃ¡p",NoiDung="NhÃ¡p",HanNopLuc=DateTimeOffset.UtcNow.AddDays(10),TrangThai="BanNhap"});await db.SaveChangesAsync();}using var g=await f.Client(f.G);var day=DateTime.Today.AddDays(7).AddHours(19);using var r=await f.Post(g,"/HocTap/LichHoc/Sua",new(){{"Id",f.Schedule.ToString()},{"RowVersion",Convert.ToBase64String(await f.Version<LichHoc>(f.Schedule))},{"TrangThai","DaDieuChinh"},{"LyDo","Äá»•i giá» theo thá»a thuáº­n"},{"Lich.TieuDe","Giá» má»›i"},{"Lich.BatDauLuc",day.ToString("s")},{"Lich.KetThucLuc",day.AddHours(1).ToString("s")}});Assert.Equal(HttpStatusCode.OK,r.StatusCode);await using var fresh=f.Db();var saved=await fresh.Set<LichHoc>().SingleAsync(x=>x.Id==f.Schedule);Assert.Equal("Giá» má»›i",saved.TieuDe);Assert.Equal(new DateTimeOffset(DateTime.SpecifyKind(day,DateTimeKind.Unspecified),TimeSpan.FromHours(7)),saved.BatDauLuc);Assert.Equal(new DateTimeOffset(DateTime.SpecifyKind(day.AddHours(1),DateTimeKind.Unspecified),TimeSpan.FromHours(7)),saved.KetThucLuc);Assert.Equal("DaLenLich",saved.TrangThai);Assert.Equal(f.Lesson,(await fresh.Set<BuoiHoc>().SingleAsync(x=>x.LichHocId==f.Schedule)).Id);Assert.Single(await fresh.Set<BaiTapGiao>().Where(x=>x.BuoiHocId==f.Lesson).ToListAsync());var history=await fresh.Set<LichSuDieuChinhLich>().SingleAsync(x=>x.LichHocId==f.Schedule);Assert.False(string.IsNullOrWhiteSpace(history.LyDo));break;}
 case "lesson_start_http":{
  await f.SeedLesson(false);using var g=await f.Client(f.G);using(var r=await f.Post(g,"/HocTap/LichHoc/BatDauBuoi",new(){{"id",f.Schedule.ToString()},{"rowVersion",Convert.ToBase64String(await f.Version<BuoiHoc>(f.Lesson))}}))Assert.NotEqual(HttpStatusCode.OK,r.StatusCode);await f.Sql("UPDATE dbo.LichHoc SET BatDauLuc=DATEADD(minute,-5,SYSDATETIMEOFFSET()),KetThucLuc=DATEADD(minute,55,SYSDATETIMEOFFSET()) WHERE Id=@s",new(){{"@s",f.Schedule}});using(var r=await f.Post(g,"/HocTap/LichHoc/BatDauBuoi",new(){{"id",f.Schedule.ToString()},{"rowVersion",Convert.ToBase64String(await f.Version<BuoiHoc>(f.Lesson))}}))Assert.Equal(HttpStatusCode.OK,r.StatusCode);await using var db=f.Db();Assert.Equal("DangDienRa",(await db.Set<BuoiHoc>().SingleAsync(x=>x.Id==f.Lesson)).TrangThai);break;}
 case "lesson_record_http":{
  await f.SeedLesson();using var g=await f.Client(f.G);using var r=await f.Post(g,"/HocTap/LichHoc/GhiNhan",RecordForm(f,Convert.ToBase64String(await f.Version<BuoiHoc>(f.Lesson))));Assert.Equal(HttpStatusCode.OK,r.StatusCode);await using var db=f.Db();var b=await db.Set<BuoiHoc>().SingleAsync(x=>x.Id==f.Lesson);Assert.Equal("DaHoc",b.TrangThai);Assert.Equal("Ã”n phÃ¢n sá»‘",b.NoiDungGiangDay);Assert.Equal("VangKhongBaoTruoc",b.TrangThaiThamDuBenHoc);Assert.Equal("CoMat",b.TrangThaiThamDuGiaSu);break;}
 case "lesson_unrecorded":{
  await f.SeedLesson();using var h=await f.Client(f.H);using var r=await h.GetAsync("/HocTap/LichHoc/Buoi/"+f.Schedule);await f.Record(r);Assert.Equal(HttpStatusCode.OK,r.StatusCode);Assert.Contains("ChÆ°a ghi nháº­n",WebUtility.HtmlDecode(await r.Content.ReadAsStringAsync()),StringComparison.OrdinalIgnoreCase);await using var db=f.Db();var b=await db.Set<BuoiHoc>().SingleAsync(x=>x.Id==f.Lesson);Assert.Null(b.GhiNhanLuc);Assert.NotEqual("DaHoc",b.TrangThai);await using var current=f.Scope(f.H);Assert.Equal(0,(await new EduConnect.Mvc.Areas.DanhGia.Services.TienDoService(current.Db,current.Rights).LayAsync(f.Class)).SoBuoiDaHoc);break;}
 case "lesson_absence":{
  await f.SeedLesson(false);using var g=await f.Client(f.G);using var r=await f.Post(g,"/HocTap/LichHoc/ThongBaoNghi",new(){{"id",f.Schedule.ToString()},{"rowVersion",Convert.ToBase64String(await f.Version<BuoiHoc>(f.Lesson))},{"reason","Gia sÆ° cÃ³ viá»‡c"},{"canHocBu","true"}});Assert.Equal(HttpStatusCode.OK,r.StatusCode);await using var db=f.Db();var b=await db.Set<BuoiHoc>().SingleAsync(x=>x.Id==f.Lesson);Assert.Equal("DaNghi",b.TrangThai);Assert.True(b.CanHocBu);Assert.False(await db.Set<LichHoc>().AnyAsync(x=>x.LichGocId==f.Schedule));break;}
 case "lesson_makeup_duplicate":{
  await f.SeedLesson();
  var version=Convert.ToBase64String(await f.Version<BuoiHoc>(f.Lesson));
  await using(var tutor=f.Scope(f.G)) await tutor.Schedules.ThongBaoNghiAsync(f.Schedule,"Tutor absence; makeup required",true,version);
  var originalVersion=Convert.ToBase64String(await f.Version<LichHoc>(f.Schedule));
  SuaLichInput Input(string title,int day)=>new(){Id=f.Schedule,RowVersion=originalVersion,TrangThai="DaLenLich",Lich=new(){TieuDe=title,BatDauLuc=DateTime.Today.AddDays(day).AddHours(19),KetThucLuc=DateTime.Today.AddDays(day).AddHours(20)}};
  await using(var tutor=f.Scope(f.G)) await tutor.Schedules.TaoBuoiBuAsync(Input("Makeup first",12));
  await using(var tutor=f.Scope(f.G)){var error=await Assert.ThrowsAsync<LoiNghiepVu>(()=>tutor.Schedules.TaoBuoiBuAsync(Input("Duplicate makeup",14)));Assert.Equal(409,error.Status);}
  await using(var db=f.Db()){var original=await db.Set<BuoiHoc>().SingleAsync(x=>x.Id==f.Lesson);Assert.Equal("DaNghi",original.TrangThai);Assert.True(original.CanHocBu);var makeups=await db.Set<LichHoc>().Where(x=>x.LichGocId==f.Schedule&&x.LaBuoiBu&&x.TrangThai=="DaLenLich").ToListAsync();Assert.Single(makeups);Assert.Single(await db.Set<BuoiHoc>().Where(x=>x.LichHocId==makeups[0].Id).ToListAsync());}
  break;
 }
 case "schedule_change_reject":{
  await f.SeedLesson(false);
  await f.Sql("UPDATE dbo.LichHoc SET BatDauLuc=DATEADD(minute,-5,SYSDATETIMEOFFSET()),KetThucLuc=DATEADD(minute,55,SYSDATETIMEOFFSET()) WHERE Id=@s",new(){{"@s",f.Schedule}});
  await using(var tutor=f.Scope(f.G)) await tutor.Schedules.BatDauBuoiAsync(f.Schedule,Convert.ToBase64String(await f.Version<BuoiHoc>(f.Lesson)));
  DateTimeOffset oldStart,oldEnd;string oldTitle;await using(var before=f.Db()){var old=await before.Set<LichHoc>().SingleAsync(x=>x.Id==f.Schedule);oldStart=old.BatDauLuc;oldEnd=old.KetThucLuc;oldTitle=old.TieuDe;}
  var start=DateTime.Today.AddDays(14).AddHours(19);
  using var g=await f.Client(f.G);
  using var response=await f.Post(g,"/HocTap/LichHoc/Sua",new(){{"Id",f.Schedule.ToString()},{"RowVersion",Convert.ToBase64String(await f.Version<LichHoc>(f.Schedule))},{"TrangThai","DaLenLich"},{"LyDo","Attempt to change a lesson with recorded activity"},{"Lich.TieuDe","Should not be saved"},{"Lich.BatDauLuc",start.ToString("s")},{"Lich.KetThucLuc",start.AddHours(1).ToString("s")}});
  Assert.Equal(422,(int)response.StatusCode);
  await using(var after=f.Db()){
   var schedule=await after.Set<LichHoc>().SingleAsync(x=>x.Id==f.Schedule);var lesson=await after.Set<BuoiHoc>().SingleAsync(x=>x.Id==f.Lesson);
   Assert.Equal(oldStart,schedule.BatDauLuc);Assert.Equal(oldEnd,schedule.KetThucLuc);Assert.Equal(oldTitle,schedule.TieuDe);
   Assert.Equal("DangDienRa",lesson.TrangThai);Assert.Empty(await after.Set<LichSuDieuChinhLich>().Where(x=>x.LichHocId==f.Schedule).ToListAsync());
  }
  break;
 }
 default:throw new InvalidOperationException(key);
 }}
}
