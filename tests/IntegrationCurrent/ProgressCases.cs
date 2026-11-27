using System.Net;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Areas.DanhGia.Services;
using EduConnect.Mvc.Areas.DanhGia.ViewModels;
using Microsoft.EntityFrameworkCore;
using Xunit;
internal static partial class Cases
{
 static DanhGiaDoiTacService Reviews(Scope s)=>new(s.Db,s.Current,s.Rights,s.Notices);
 static DanhGiaDoiTacViewModel ReviewInput(Fixture f)=>new(){MaLop=f.Class,SoSao=5,NhanXet="Gia sư hướng dẫn rõ ràng",XacNhanRiengTu=true};
 static Dictionary<string,string> ReviewForm(Fixture f)=>new(){{"MaLop",f.Class.ToString()},{"SoSao","5"},{"NhanXet","Gia sư hướng dẫn rõ ràng"},{"XacNhanRiengTu","true"}};
 static async Task EndedClassWithLesson(Fixture f){await f.SeedLesson();await using(var g=f.Scope(f.G))await g.Schedules.GhiNhanAsync(RecordModel(f,Convert.ToBase64String(await f.Version<BuoiHoc>(f.Lesson))));await f.Sql("UPDATE dbo.LopHoc SET TrangThai='DaKetThuc',KetThucLuc=SYSDATETIMEOFFSET(),CachKetThuc=N'DongY' WHERE Id=@c",new(){{"@c",f.Class}});}
 static async Task SeedProgress(Fixture f){await f.SeedLesson();var firstLesson=f.Lesson;var firstSchedule=f.Schedule;await f.Sql("UPDATE dbo.LichHoc SET BatDauLuc=DATEADD(day,-5,SYSDATETIMEOFFSET()),KetThucLuc=DATEADD(hour,1,DATEADD(day,-5,SYSDATETIMEOFFSET())) WHERE Id=@s",new(){{"@s",firstSchedule}});await using(var g=f.Scope(f.G)){var m=RecordModel(f,Convert.ToBase64String(await f.Version<BuoiHoc>(f.Lesson)));m.TrangThaiBenHoc="CoMat";await g.Schedules.GhiNhanAsync(m);}
  await using(var seed=f.Db()){for(int i=0;i<2;i++){var s=new LichHoc{Id=Guid.NewGuid(),LopHocId=f.Class,TieuDe="Buổi "+i,BatDauLuc=DateTimeOffset.UtcNow.AddDays(-4+i),KetThucLuc=DateTimeOffset.UtcNow.AddDays(-4+i).AddHours(1),TrangThai="DaLenLich"};seed.Add(s);await seed.SaveChangesAsync();if(i==0){var b=await seed.Set<BuoiHoc>().SingleAsync(x=>x.LichHocId==s.Id);f.Schedule=s.Id;f.Lesson=b.Id;await using var g=f.Scope(f.G);var m=RecordModel(f,Convert.ToBase64String(b.RowVersion));m.KetQuaBuoiHoc="KhongDienRa";m.NoiDung=null;await g.Schedules.GhiNhanAsync(m);}}}
  f.Schedule=firstSchedule;f.Lesson=firstLesson;await using(var seed=f.Db()){for(int i=0;i<3;i++){var a=new BaiTapGiao{Id=Guid.NewGuid(),LopHocId=f.Class,BuoiHocId=firstLesson,NguoiGiaoId=f.G,TieuDe="Bài "+i,NoiDung="Nội dung bài",TrangThai="DaGiao",HanNopLuc=DateTimeOffset.UtcNow.AddDays(3)};seed.Add(a);await seed.SaveChangesAsync();var n=new BaiTapNop{Id=Guid.NewGuid(),BaiTapGiaoId=a.Id,NguoiNopId=f.H,NoiDung="Đã nộp",MaThaoTac=Guid.NewGuid(),LanNop=1,NopLuc=DateTimeOffset.UtcNow};seed.Add(n);await seed.SaveChangesAsync();if(i<2){seed.Add(new KetQuaChamBai{Id=Guid.NewGuid(),BaiTapNopId=n.Id,NguoiChamId=f.G,Diem=i==0?8:6,NhanXet="Đã chấm",ChamLuc=DateTimeOffset.UtcNow});await seed.SaveChangesAsync();}if(i==0){f.Assignment=a.Id;f.Submission=n.Id;}}}}
 static void ProgressExpected(TienDoViewModel p){Assert.Equal(1,p.SoBuoiDaHoc);Assert.Equal(1,p.SoBuoiNghi);Assert.Equal(1,p.SoBuoiChuaGhiNhan);Assert.Equal(3,p.SoBaiDaGiao);Assert.Equal(3,p.SoBaiDaNop);Assert.Equal(2,p.SoBaiDaCham);Assert.Equal(7m,p.DiemTrungBinh);}
 internal static async Task ProgressCases(Fixture f,string key){switch(key){
 case "review_immutable":{
  await EndedClassWithLesson(f);var first=ReviewInput(f);first.SoSao=4;first.NhanXet="Nhận xét ban đầu";Guid reviewId;
  await using(var learner=f.Scope(f.H))await Reviews(learner).GuiAsync(first);
  await using(var db=f.Db()){var review=await db.Set<DanhGiaDoiTac>().SingleAsync(x=>x.LopHocId==f.Class&&x.NguoiGuiId==f.H);reviewId=review.Id;db.Add(new BienPhapQuanTri{Id=Guid.NewGuid(),LoaiDoiTuong="DanhGiaDoiTac",DoiTuongId=review.Id,Loai="AnNoiDung",LyDo="Kiểm tra quyền riêng tư",NguoiApDungId=f.A,ApDungLuc=DateTimeOffset.UtcNow});await db.SaveChangesAsync();}
  foreach(var edited in new[]{(SoSao:1,NhanXet:"Nội dung sửa lần một"),(SoSao:5,NhanXet:"Nội dung sửa lần hai")}){var changed=ReviewInput(f);changed.SoSao=edited.SoSao;changed.NhanXet=edited.NhanXet;await using var learner=f.Scope(f.H);var e=await Assert.ThrowsAsync<LoiNghiepVu>(()=>Reviews(learner).GuiAsync(changed));Assert.Equal(409,e.Status);}
  await using(var verify=f.Db()){var rows=await verify.Set<DanhGiaDoiTac>().Where(x=>x.LopHocId==f.Class&&x.NguoiGuiId==f.H).ToListAsync();var kept=Assert.Single(rows);Assert.Equal(reviewId,kept.Id);Assert.Equal(4,kept.SoSao);Assert.Equal("Nhận xét ban đầu",kept.NhanXet);Assert.Single(await verify.Set<BienPhapQuanTri>().Where(x=>x.DoiTuongId==reviewId&&x.GoLuc==null).ToListAsync());}
  break;}
 case "progress_latest":{
  await f.SeedAssignment();
  await using(var seed=f.Db()){seed.Add(new BaiTapGiao{Id=Guid.NewGuid(),LopHocId=f.Class,BuoiHocId=f.Lesson,NguoiGiaoId=f.G,TieuDe="Active task 2",NoiDung="Task",HanNopLuc=DateTimeOffset.UtcNow.AddDays(4),TrangThai="DaGiao"});seed.Add(new BaiTapGiao{Id=Guid.NewGuid(),LopHocId=f.Class,BuoiHocId=f.Lesson,NguoiGiaoId=f.G,TieuDe="Draft task",NoiDung="Draft",HanNopLuc=DateTimeOffset.UtcNow.AddDays(4),TrangThai="BanNhap"});await seed.SaveChangesAsync();}
  await using(var student=f.Scope(f.H))await student.Tasks.NopAsync(new(){BaiTapGiaoId=f.Assignment,MaThaoTac=Guid.NewGuid(),NoiDung="Older submission"});
  Guid oldSubmission;await using(var db=f.Db())oldSubmission=await db.Set<BaiTapNop>().Where(x=>x.BaiTapGiaoId==f.Assignment).Select(x=>x.Id).SingleAsync();
  // Legacy fixture: the database has a grade on an older submission and a later ungraded submission.
  // Current write rules prevent creating this state, so seed the historical grade directly.
  await using(var student=f.Scope(f.H))await student.Tasks.NopAsync(new(){BaiTapGiaoId=f.Assignment,MaThaoTac=Guid.NewGuid(),NoiDung="Latest submission, not graded"});
  await using(var db=f.Db()){
   var latest=await db.Set<BaiTapNop>().Where(x=>x.BaiTapGiaoId==f.Assignment&&x.Id!=oldSubmission).SingleAsync();
   await db.Set<BaiTapNop>().Where(x=>x.Id==latest.Id).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.NopLuc,DateTimeOffset.UtcNow.AddMinutes(1)));
   db.Add(new KetQuaChamBai{Id=Guid.NewGuid(),BaiTapNopId=oldSubmission,NguoiChamId=f.G,Diem=8,NhanXet="Legacy grade",ChamLuc=DateTimeOffset.UtcNow});
   await db.SaveChangesAsync();
   Assert.NotEqual(oldSubmission,latest.Id);
  }
  Guid secondAssignment;await using(var db=f.Db())secondAssignment=await db.Set<BaiTapGiao>().Where(x=>x.LopHocId==f.Class&&x.TieuDe=="Active task 2").Select(x=>x.Id).SingleAsync();
  await using(var student=f.Scope(f.H))await student.Tasks.NopAsync(new(){BaiTapGiaoId=secondAssignment,MaThaoTac=Guid.NewGuid(),NoiDung="Submitted, not graded"});
  await using(var student=f.Scope(f.H)){
   var progress=await new TienDoService(student.Db,student.Rights).LayAsync(f.Class);
   Assert.Equal(2,progress.SoBaiDaGiao);Assert.Equal(2,progress.SoBaiDaNop);Assert.Equal(0,progress.SoBaiDaCham);Assert.Null(progress.DiemTrungBinh);Assert.Empty(progress.Diem);Assert.DoesNotContain(progress.Bai,x=>x.TieuDe=="Draft task");
  }
  await using(var db=f.Db()){
   Assert.Equal(2,await db.Set<BaiTapNop>().CountAsync(x=>x.BaiTapGiaoId==f.Assignment));
   Assert.True(await db.Set<KetQuaChamBai>().AnyAsync(x=>x.BaiTapNopId==oldSubmission));
  }
  break;
 }
 case "progress_store":case "progress_date":case "progress_http":{
  await SeedProgress(f);if(key=="progress_http"){foreach(var actor in new[]{f.H,f.G}){using var c=await f.Client(actor);using var r=await c.GetAsync("/HocTap/LopHoc/ChiTiet/"+f.Class+"?tab=overview");await f.Record(r);Assert.Equal(200,(int)r.StatusCode);var body=await r.Content.ReadAsStringAsync();Assert.Contains(f.Schedule.ToString(),body,StringComparison.OrdinalIgnoreCase);Assert.Contains(f.Assignment.ToString(),body,StringComparison.OrdinalIgnoreCase);Assert.Contains("Buổi đã học</span><strong>1</strong><small>1 nghỉ · 1 chưa ghi nhận",body,StringComparison.Ordinal);Assert.Contains("Bài đã nộp</span><strong>3 <small>/ 3",body,StringComparison.Ordinal);Assert.Contains("Bài đã chấm</span><strong>2</strong>",body,StringComparison.Ordinal);Assert.Contains("Điểm trung bình</span><strong>7</strong>",body,StringComparison.Ordinal);}await using var h=f.Scope(f.H);ProgressExpected(await new TienDoService(h.Db,h.Rights).LayAsync(f.Class));break;}
  await using var s=f.Scope(f.H);var service=new TienDoService(s.Db,s.Rights);if(key=="progress_store")ProgressExpected(await service.LayAsync(f.Class));else{var date=DateTimeOffset.UtcNow.AddDays(-5).Date;var p=await service.LayAsync(f.Class,new DateTimeOffset(date,TimeSpan.Zero),new DateTimeOffset(date,TimeSpan.Zero));Assert.Equal(1,p.SoBuoiDaHoc);Assert.Equal(0,p.SoBuoiNghi);var empty=await service.LayAsync(f.Class,DateTimeOffset.UtcNow.AddDays(30),DateTimeOffset.UtcNow.AddDays(31));Assert.Equal(0,empty.SoBuoiDaHoc+empty.SoBaiDaGiao+empty.SoBaiDaCham);Assert.Null(empty.DiemTrungBinh);var e=await Assert.ThrowsAsync<LoiNghiepVu>(()=>service.LayAsync(f.Class,DateTimeOffset.UtcNow.AddDays(2),DateTimeOffset.UtcNow));Assert.Equal(400,e.Status);}break;}
 case "legacy_feedback":{
  await f.SeedClass();var id=Guid.NewGuid();await using(var seed=f.Db()){seed.Add(new PhieuDanhGia{Id=id,LopHocId=f.Class,NguoiGuiId=f.H,MocDanhGia="legacy-"+f.Tag,NoiDung="Phản hồi lịch sử chưa đồng ý kết thúc",MucDoHaiLong=null,GuiLuc=DateTimeOffset.UtcNow.AddDays(-5)});await seed.SaveChangesAsync();seed.Add(new LichSuPhanHoi{Id=Guid.NewGuid(),PhieuDanhGiaId=id,NoiDung="Nội dung cũ",MucDoHaiLong=null,NguoiSuaId=f.H,ThayDoiLuc=DateTimeOffset.UtcNow.AddDays(-4)});await seed.SaveChangesAsync();}
  using(var h=await f.Client(f.H)){using var r=await h.GetAsync("/DanhGia/DanhGia/LichSu?id="+id);Assert.Equal(HttpStatusCode.OK,r.StatusCode);var body=await r.Content.ReadAsStringAsync();Assert.Contains("Nội dung cũ",body,StringComparison.Ordinal);}
  using(var x=await f.Client(f.X)){using var r=await x.GetAsync("/DanhGia/DanhGia/LichSu?id="+id);Assert.Equal(HttpStatusCode.Forbidden,r.StatusCode);}
  await using var verify=f.Db();var saved=await verify.Set<PhieuDanhGia>().SingleAsync(x=>x.Id==id);Assert.Null(saved.MucDoHaiLong);Assert.Null(saved.QuyetDinhDeXuat);Assert.Equal("Phản hồi lịch sử chưa đồng ý kết thúc",saved.NoiDung);Assert.Single(await verify.Set<LichSuPhanHoi>().Where(x=>x.PhieuDanhGiaId==id).ToListAsync());Assert.False(await verify.Set<QuyetDinhSauPhanHoi>().AnyAsync(x=>x.DotPhanHoiId==id));break;}
 case "review_visibility":{
  await f.SeedClass();Guid visible,hidden;await using(var seed=f.Db()){visible=Guid.NewGuid();hidden=Guid.NewGuid();seed.AddRange(new DanhGiaDoiTac{Id=visible,LopHocId=f.Class,NguoiGuiId=f.H,NguoiNhanId=f.G,SoSao=5,NhanXet="Visible rating "+f.Tag,GuiLuc=DateTimeOffset.UtcNow},new DanhGiaDoiTac{Id=hidden,LopHocId=f.Class,NguoiGuiId=f.X,NguoiNhanId=f.G,SoSao=4,NhanXet="Hidden rating "+f.Tag,GuiLuc=DateTimeOffset.UtcNow.AddSeconds(-1)},new BienPhapQuanTri{Id=Guid.NewGuid(),LoaiDoiTuong="DanhGiaDoiTac",DoiTuongId=hidden,Loai="AnNoiDung",LyDo="Kiểm tra ẩn",NguoiApDungId=f.A,ApDungLuc=DateTimeOffset.UtcNow});await seed.SaveChangesAsync();}
  using(var guest=await f.Client()){using var detail=await guest.GetAsync("/DanhGia/DoiTac/ChiTiet?id="+f.G);Assert.Equal(HttpStatusCode.OK,detail.StatusCode);var body=await detail.Content.ReadAsStringAsync();Assert.True(body.Contains("5.0",StringComparison.Ordinal),$"rating detail missing from HTTP {detail.StatusCode}: {body[..Math.Min(body.Length,1200)]}");Assert.Contains("1 lượt đánh giá",body,StringComparison.Ordinal);Assert.Contains("Visible rating "+f.Tag,body,StringComparison.Ordinal);Assert.DoesNotContain("Hidden rating "+f.Tag,body,StringComparison.Ordinal);
   using var search=await guest.GetAsync("/TimKiem/TimKiem/GiaSu?TuKhoa="+Uri.EscapeDataString(f.Tag));Assert.Equal(HttpStatusCode.OK,search.StatusCode);var searchBody=await search.Content.ReadAsStringAsync();Assert.Contains("5.0",searchBody,StringComparison.Ordinal);Assert.Contains("(1 đánh giá)",searchBody,StringComparison.Ordinal);}
  await using var verify=f.Db();Assert.True(await verify.Set<DanhGiaDoiTac>().AnyAsync(x=>x.Id==hidden));Assert.Equal(1,await EduConnect.Mvc.Areas.DanhGia.Services.DanhGiaDoiTacService.CongKhai(verify).CountAsync(x=>x.NguoiNhanId==f.G));break;}
 case "review_store":{
  await EndedClassWithLesson(f);foreach(var actor in new[]{f.G,f.H}){await using var s=f.Scope(actor);var m=ReviewInput(f);m.SoSao=actor==f.G?5:4;Assert.Equal(actor==f.G?f.H:f.G,await Reviews(s).GuiAsync(m));}await using var db=f.Db();var rows=await db.Set<DanhGiaDoiTac>().Where(x=>x.LopHocId==f.Class).ToListAsync();Assert.Equal(2,rows.Count);Assert.All(rows,x=>Assert.NotEqual(x.NguoiGuiId,x.NguoiNhanId));break;}
 case "review_http":{
  await EndedClassWithLesson(f);using var h=await f.Client(f.H);using var r=await f.Post(h,"/DanhGia/DoiTac/Gui",ReviewForm(f));Assert.Equal(200,(int)r.StatusCode);await using var db=f.Db();var review=await db.Set<DanhGiaDoiTac>().SingleAsync(x=>x.LopHocId==f.Class);Assert.Equal(f.H,review.NguoiGuiId);Assert.Equal(f.G,review.NguoiNhanId);Assert.Equal(5,review.SoSao);break;}
 case "review_preconditions":case "review_validation":{
  for(int dl=0;dl<4;dl++){await using var d=new Fixture(f.Id+"-DL"+dl);await d.Init();if(key=="review_preconditions"&&dl<2){await d.SeedClass();if(dl==1)await d.Sql("UPDATE dbo.LopHoc SET TrangThai='DaKetThuc',KetThucLuc=SYSDATETIMEOFFSET(),CachKetThuc=N'DongY' WHERE Id=@c",new(){{"@c",d.Class}});}else await EndedClassWithLesson(d);var actor=d.H;var form=ReviewForm(d);if(key=="review_preconditions"){if(dl==2){var link=await Link(d);await Member(d,link,true,true);actor=d.P;}if(dl==3)form["XacNhanRiengTu"]="false";}else{if(dl<2)form["SoSao"]=dl==0?"0":"6";if(dl>=2)form["NhanXet"]=new string('x',dl==2?9:2001);}using var c=await d.Client(actor);using var r=await d.Post(c,"/DanhGia/DoiTac/Gui",form);Assert.Equal(422,(int)r.StatusCode);await using var db=d.Db();Assert.False(await db.Set<DanhGiaDoiTac>().AnyAsync(x=>x.LopHocId==d.Class));}break;}
 case "review_response":{
  await EndedClassWithLesson(f);await using(var h=f.Scope(f.H))await Reviews(h).GuiAsync(ReviewInput(f));await using var db=f.Db();var v=await db.Set<DanhGiaDoiTac>().AsNoTracking().SingleAsync(x=>x.LopHocId==f.Class);var old=Convert.ToBase64String(v.RowVersion);for(int dl=0;dl<3;dl++){var actor=dl==1?f.X:f.G;using var c=await f.Client(actor);using var r=await f.Post(c,"/DanhGia/DoiTac/PhanHoi",new(){{"DanhGiaId",v.Id.ToString()},{"NoiDung","Cảm ơn phản hồi"},{"RowVersion",old}});var count=await db.Set<LichSuDanhGiaDoiTac>().CountAsync(x=>x.DanhGiaId==v.Id);Assert.Equal(new[]{200,403,409}[dl],(int)r.StatusCode);}await using var fresh=f.Db();Assert.Single(await fresh.Set<LichSuDanhGiaDoiTac>().Where(x=>x.DanhGiaId==v.Id).ToListAsync());Assert.Equal("Cảm ơn phản hồi",(await fresh.Set<DanhGiaDoiTac>().SingleAsync(x=>x.Id==v.Id)).PhanHoi);break;}
 default:throw new InvalidOperationException(key);
 }}
}
