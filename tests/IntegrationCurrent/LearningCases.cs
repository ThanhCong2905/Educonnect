using System.Net;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using Microsoft.EntityFrameworkCore;
using Xunit;
internal static partial class Cases
{
 static Dictionary<string,string> SubmitForm(Fixture f)=>new(){{"BaiTapGiaoId",f.Assignment.ToString()},{"MaThaoTac",Guid.NewGuid().ToString()},{"NoiDung","Bài làm kiểm thử tích hợp"}};
 static Dictionary<string,string> GradeForm(Fixture f,Guid submission,string score,string? rv=null)=>new(){{"BaiTapGiaoId",f.Assignment.ToString()},{"BaiTapNopId",submission.ToString()},{"Diem",score},{"NhanXet","Nhận xét bài làm"},{"RowVersion",rv??""}};
 static Dictionary<string,string> AssignmentForm(Fixture f,bool draft=false)=>new(){{"LopHocId",f.Class.ToString()},{"BuoiHocId",f.Lesson.ToString()},{"TieuDe","Bài tập ôn phân số"},{"NoiDung","Giải các bài tập phân số"},{"HanNopLuc",DateTime.Today.AddDays(7).AddHours(21).ToString("s")},{"LuuNhap",draft.ToString()}};
 internal static async Task LearningCases(Fixture f,string key){switch(key){
 case "assignment_draft":{
  await f.SeedLesson(false);using var g=await f.Client(f.G);using(var r=await f.Post(g,"/HocTap/BaiTap/Giao",AssignmentForm(f,true)))Assert.Equal(200,(int)r.StatusCode);using(var r=await f.Post(g,"/HocTap/BaiTap/Giao",AssignmentForm(f,false)))Assert.InRange((int)r.StatusCode,400,499);await using var db=f.Db();var task=await db.Set<BaiTapGiao>().SingleAsync(x=>x.LopHocId==f.Class);Assert.Equal("BanNhap",task.TrangThai);await f.Sql("UPDATE dbo.LichHoc SET BatDauLuc=DATEADD(minute,-5,SYSDATETIMEOFFSET()),KetThucLuc=DATEADD(minute,55,SYSDATETIMEOFFSET()) WHERE Id=@id",new(){{"@id",f.Schedule}});await using(var s=f.Scope(f.G))await s.Schedules.BatDauBuoiAsync(f.Schedule,Convert.ToBase64String(await f.Version<BuoiHoc>(f.Lesson)));using(var r=await f.Post(g,"/HocTap/BaiTap/CongBo",new(){{"id",task.Id.ToString()},{"rowVersion",Convert.ToBase64String(task.RowVersion)}}))Assert.Equal(200,(int)r.StatusCode);await using var fresh=f.Db();Assert.Equal("DaGiao",(await fresh.Set<BaiTapGiao>().SingleAsync(x=>x.Id==task.Id)).TrangThai);break;}
 case "assignment_wrong_lesson":{
  for(int dl=0;dl<4;dl++){await using var d=new Fixture(f.Id+"-DL"+dl);await d.Init();await d.SeedLesson(false);var target=d.Lesson;if(dl==1){await using var other=new Fixture(d.Id+"-Other");await other.Init();await other.SeedLesson();target=other.Lesson;}if(dl>=2){await using var setup=d.Scope(d.G);if(dl==2)await setup.Schedules.ThongBaoNghiAsync(d.Schedule,"Lý do nghỉ của fixture",true,Convert.ToBase64String(await d.Version<BuoiHoc>(d.Lesson)));else await setup.Schedules.SuaAsync(new(){Id=d.Schedule,RowVersion=Convert.ToBase64String(await d.Version<LichHoc>(d.Schedule)),TrangThai="DaHuy",LyDo="Fixture hủy"});}using var g=await d.Client(d.G);var form=AssignmentForm(d,true);if(dl==0)form.Remove("BuoiHocId");else form["BuoiHocId"]=target.ToString();using var r=await d.Post(g,"/HocTap/BaiTap/Giao",form);Assert.InRange((int)r.StatusCode,400,499);await using var db=d.Db();Assert.False(await db.Set<BaiTapGiao>().AnyAsync(x=>x.LopHocId==d.Class));}break;}
 case "assignment_legacy":{
  for(int dl=0;dl<2;dl++){await using var d=new Fixture(f.Id+"-DL"+dl);await d.Init();await d.SeedAssignment();await d.Sql("""
IF DB_NAME()<>N'EduConnect_Verification_Integration_20261125_v2' THROW 50000,N'Refusing non-test DB',1;
DECLARE @trigger sysname=(SELECT TOP(1) t.name FROM sys.triggers t JOIN sys.sql_modules m ON m.object_id=t.object_id WHERE t.parent_id=OBJECT_ID(N'dbo.BaiTapGiao') AND m.definition LIKE N'%Bài mới phải gắn buổi học%');
IF @trigger IS NULL THROW 50000,N'Legacy fixture trigger not found',1;
BEGIN TRY
 BEGIN TRAN;
 DECLARE @disable nvarchar(max)=N'DISABLE TRIGGER dbo.'+QUOTENAME(@trigger)+N' ON dbo.BaiTapGiao'; EXEC sys.sp_executesql @disable;
 UPDATE dbo.BaiTapGiao SET BuoiHocId=NULL WHERE Id=@a;
 DECLARE @enable nvarchar(max)=N'ENABLE TRIGGER dbo.'+QUOTENAME(@trigger)+N' ON dbo.BaiTapGiao'; EXEC sys.sp_executesql @enable;
 COMMIT;
 SELECT name,is_disabled FROM sys.triggers WHERE name=@trigger;
END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK;THROW;END CATCH;
""",new(){{"@a",d.Assignment}});var original=d.Class;var lesson=d.Lesson;if(dl==1){await using var other=new Fixture(d.Id+"-Other");await other.Init();await other.SeedLesson();lesson=other.Lesson;}using var g=await d.Client(d.G);using var r=await d.Post(g,"/HocTap/BaiTap/GanBuoi",new(){{"id",d.Assignment.ToString()},{"buoiHocId",lesson.ToString()},{"rowVersion",Convert.ToBase64String(await d.Version<BaiTapGiao>(d.Assignment))}});if(dl==0)Assert.Equal(200,(int)r.StatusCode);else Assert.InRange((int)r.StatusCode,400,499);await using var db=d.Db();Assert.Equal(dl==0?(Guid?)lesson:null,(await db.Set<BaiTapGiao>().SingleAsync(x=>x.Id==d.Assignment)).BuoiHocId);}break;}
 case "submission_http":{
  await f.SeedAssignment();using var h=await f.Client(f.H);using(var r=await f.Post(h,"/HocTap/BaiTap/Nop",SubmitForm(f)))Assert.Equal(200,(int)r.StatusCode);await f.Sql("UPDATE dbo.BaiTapGiao SET HanNopLuc=DATEADD(minute,-1,SYSDATETIMEOFFSET()) WHERE Id=@a",new(){{"@a",f.Assignment}});using(var r=await f.Post(h,"/HocTap/BaiTap/Nop",SubmitForm(f)))Assert.Equal(200,(int)r.StatusCode);await using var db=f.Db();var a=await db.Set<BaiTapGiao>().SingleAsync(x=>x.Id==f.Assignment);var ns=await db.Set<BaiTapNop>().Where(x=>x.BaiTapGiaoId==f.Assignment).OrderBy(x=>x.LanNop).ToListAsync();Assert.Equal(2,ns.Count);Assert.True(ns[1].NopLuc>a.HanNopLuc);using var detail=await h.GetAsync("/HocTap/BaiTap/ChiTiet/"+f.Assignment);Assert.Equal(HttpStatusCode.OK,detail.StatusCode);var html=System.Net.WebUtility.HtmlDecode(await detail.Content.ReadAsStringAsync());f.Proof["lateSubmissionHtml"] = html;Assert.Contains("Nộp muộn",html);break;}
 case "submission_after_grade":{
  await f.SeedAssignment();var rows=await SubmitSql(f,Guid.NewGuid());var id=(Guid)rows[0]["Id"]!;await GradeSql(f,id,f.G,8);using var h=await f.Client(f.H);using var r=await f.Post(h,"/HocTap/BaiTap/Nop",SubmitForm(f));Assert.Equal(409,(int)r.StatusCode);await using var db=f.Db();Assert.Single(await db.Set<BaiTapNop>().Where(x=>x.BaiTapGiaoId==f.Assignment).ToListAsync());break;}
 case "submission_permissions":{
  for(int dl=0;dl<4;dl++){await using var d=new Fixture(f.Id+"-DL"+dl);await d.Init();await d.SeedAssignment();var link=await Link(d);var member=await Member(d,link,true,true);await d.Sql("UPDATE dbo.ThanhVienLopHoc SET MucQuyen=@p,ChoPhepNopBai=@s WHERE Id=@m",new(){{"@p",dl==0?"Xem":"ChinhSua"},{"@s",dl>=2},{"@m",member}});if(dl==3)await d.Sql("UPDATE dbo.LienKetPhuHuynhHocSinh SET TrangThai='DaHuy',KetThucLuc=SYSDATETIMEOFFSET(),NguoiKetThucId=@u WHERE Id=@id",new(){{"@u",d.P},{"@id",link}});using var p=await d.Client(d.P);using var r=await d.Post(p,"/HocTap/BaiTap/Nop",SubmitForm(d));Assert.Equal(dl==2?200:403,(int)r.StatusCode);await using var db=d.Db();Assert.Equal(dl==2?1:0,await db.Set<BaiTapNop>().CountAsync(x=>x.BaiTapGiaoId==d.Assignment));}break;}
 case "grading_http":{
  foreach(var score in new[]{"0","10",""}){await using var d=new Fixture(f.Id+"-"+score);await d.Init();await d.SeedAssignment();var rows=await SubmitSql(d,Guid.NewGuid());var id=(Guid)rows[0]["Id"]!;using var g=await d.Client(d.G);using var r=await d.Post(g,"/HocTap/BaiTap/Cham",GradeForm(d,id,score));Assert.Equal(200,(int)r.StatusCode);await using var db=d.Db();var k=await db.Set<KetQuaChamBai>().SingleAsync(x=>x.BaiTapNopId==id);Assert.Equal(score==""?(decimal?)null:decimal.Parse(score),k.Diem);}break;}
 case "grading_concurrency":{
  await f.SeedAssignment();var rows=await SubmitSql(f,Guid.NewGuid());var id=(Guid)rows[0]["Id"]!;await GradeSql(f,id,f.G,7.5m);await using var db=f.Db();var old=await db.Set<KetQuaChamBai>().AsNoTracking().SingleAsync(x=>x.BaiTapNopId==id);using var g=await f.Client(f.G);using(var r=await f.Post(g,"/HocTap/BaiTap/Cham",GradeForm(f,id,"8",Convert.ToBase64String(old.RowVersion))))Assert.Equal(200,(int)r.StatusCode);using(var r=await f.Post(g,"/HocTap/BaiTap/Cham",GradeForm(f,id,"9",Convert.ToBase64String(old.RowVersion))))Assert.Equal(409,(int)r.StatusCode);await using var fresh=f.Db();Assert.Equal(8,(await fresh.Set<KetQuaChamBai>().SingleAsync(x=>x.Id==old.Id)).Diem);break;}
 default:throw new InvalidOperationException(key);
 }}
}
