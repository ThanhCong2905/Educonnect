using System.Net;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using Microsoft.EntityFrameworkCore;
using Xunit;
internal static partial class Cases
{
 static LopHocService Classes(Scope s)=>new(s.Db,s.Current,s.Rights,s.Notices,s.Connections);
 static LienKetTaiKhoanService Family(Scope s)=>new(s.Db,s.Current,s.Notices);
 static async Task<Guid> Link(Fixture f){await using var db=f.Db();var l=new LienKetPhuHuynhHocSinh{Id=Guid.NewGuid(),PhuHuynhId=f.P,HocSinhId=f.H,NguoiGuiId=f.P,TrangThai="DaLienKet",GuiLuc=DateTimeOffset.UtcNow.AddMinutes(-1),XacNhanLuc=DateTimeOffset.UtcNow};db.Add(l);await db.SaveChangesAsync();return l.Id;}
 static async Task<Guid> Member(Fixture f,Guid link,bool consent=false,bool approved=false){await using var db=f.Db();var m=new ThanhVienLopHoc{Id=Guid.NewGuid(),LopHocId=f.Class,TaiKhoanId=f.P,NguoiDeNghiId=f.H,LienKetId=link,TrangThaiDuyet=approved?"DaDuyet":"ChoDuyet",MucQuyen=approved?"ChinhSua":"Xem",ChoPhepNopBai=approved,DeNghiLuc=DateTimeOffset.UtcNow.AddMinutes(-2),DongYThamGiaLuc=consent?DateTimeOffset.UtcNow.AddMinutes(-1):null,DuyetLuc=approved?DateTimeOffset.UtcNow:null};db.Add(m);await db.SaveChangesAsync();return m.Id;}
 static async Task<Dictionary<string,string>> Approval(Fixture f,Guid id,string action="DaDuyet",string permission="Xem",bool submit=false)=>new(){{"id",f.Class.ToString()},{"member",id.ToString()},{"action",action},{"permission",permission},{"rowVersion",Convert.ToBase64String(await f.Version<ThanhVienLopHoc>(id))},{"choPhepNopBai",submit.ToString()}};
 internal static async Task FamilyCases(Fixture f,string key){
  switch(key){
  case "family_store":{
   await f.SeedClass();await using var db=f.Db();var email=(await db.Users.SingleAsync(x=>x.Id==f.H)).Email!;await using(var p=f.Scope(f.P))await Family(p).GuiDeNghiAsync(email);var l=await db.Set<LienKetPhuHuynhHocSinh>().AsNoTracking().SingleAsync(x=>x.PhuHuynhId==f.P&&x.HocSinhId==f.H);Assert.Equal("ChoXacNhan",l.TrangThai);await using(var h=f.Scope(f.H))await Family(h).PhanHoiAsync(l.Id,true,Convert.ToBase64String(l.RowVersion));await using var fresh=f.Db();Assert.Equal("DaLienKet",(await fresh.Set<LienKetPhuHuynhHocSinh>().SingleAsync(x=>x.Id==l.Id)).TrangThai);Assert.False(await fresh.Set<ThanhVienLopHoc>().AnyAsync(x=>x.TaiKhoanId==f.P));break;}
  case "member_store":{
   await f.SeedClass();await Link(f);await using var db=f.Db();var email=(await db.Users.SingleAsync(x=>x.Id==f.P)).Email!;await using(var h=f.Scope(f.H))await Classes(h).DeNghiAsync(f.Class,email);var m=await db.Set<ThanhVienLopHoc>().AsNoTracking().SingleAsync(x=>x.LopHocId==f.Class&&x.TaiKhoanId==f.P);await using(var p=f.Scope(f.P)){await Assert.ThrowsAsync<LoiNghiepVu>(()=>p.Rights.KiemTraQuyenXemAsync(f.Class));await Classes(p).PhanHoiLoiMoiAsync(m.Id,true,Convert.ToBase64String(m.RowVersion));await Assert.ThrowsAsync<LoiNghiepVu>(()=>p.Rights.KiemTraQuyenXemAsync(f.Class));}await using(var g=f.Scope(f.G))await Classes(g).DuyetAsync(m.Id,"DaDuyet","Xem",Convert.ToBase64String(await f.Version<ThanhVienLopHoc>(m.Id)));await using(var p=f.Scope(f.P)){Assert.Equal(f.Class,(await p.Rights.KiemTraQuyenXemAsync(f.Class)).Id);await Assert.ThrowsAsync<LoiNghiepVu>(()=>p.Rights.KiemTraQuyenGhiAsync(f.Class));}break;}
  case "family_http":{
   foreach(var agree in new[]{true,false}){await using var d=new Fixture(f.Id+"-"+agree);await d.Init();using var p=await d.Client(d.P);await using var db=d.Db();var email=(await db.Users.SingleAsync(x=>x.Id==d.H)).Email!;using var send=await d.Post(p,"/HocTap/LienKetTaiKhoan/GuiDeNghi",new(){{"email",email}});Assert.Equal(HttpStatusCode.OK,send.StatusCode);var l=await db.Set<LienKetPhuHuynhHocSinh>().AsNoTracking().SingleAsync(x=>x.PhuHuynhId==d.P&&x.HocSinhId==d.H);using var h=await d.Client(d.H);using var r=await d.Post(h,"/HocTap/LienKetTaiKhoan/PhanHoi",new(){{"id",l.Id.ToString()},{"dongY",agree.ToString()},{"rowVersion",Convert.ToBase64String(l.RowVersion)}});Assert.Equal(HttpStatusCode.OK,r.StatusCode);await using var fresh=d.Db();var actual=await fresh.Set<LienKetPhuHuynhHocSinh>().SingleAsync(x=>x.Id==l.Id);Assert.Equal(agree?"DaLienKet":"TuChoi",actual.TrangThai);}break;}
  case "family_wrong":{
   using var p=await f.Client(f.P);await using var db=f.Db();var tutorEmail=(await db.Users.SingleAsync(x=>x.Id==f.G)).Email!;using var r=await f.Post(p,"/HocTap/LienKetTaiKhoan/GuiDeNghi",new(){{"email",tutorEmail}});Assert.Equal(HttpStatusCode.BadRequest,r.StatusCode);break;}
  case "invite_http":{
   await f.SeedClass();await Link(f);await using var db=f.Db();var email=(await db.Users.SingleAsync(x=>x.Id==f.P)).Email!;using var h=await f.Client(f.H);using var r=await f.Post(h,"/HocTap/LopHoc/DeNghi",new(){{"id",f.Class.ToString()},{"email",email}});Assert.Equal(HttpStatusCode.OK,r.StatusCode);using var p=await f.Client(f.P);p.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With","XMLHttpRequest");using var denied=await p.GetAsync("/HocTap/LopHoc/ChiTiet/"+f.Class);Assert.Equal(HttpStatusCode.Forbidden,denied.StatusCode);var m=await db.Set<ThanhVienLopHoc>().SingleAsync(x=>x.LopHocId==f.Class&&x.TaiKhoanId==f.P);Assert.Equal("ChoDuyet",m.TrangThaiDuyet);Assert.Null(m.DongYThamGiaLuc);break;}
  case "invite_consent":{
   for(int dl=0;dl<3;dl++){await using var d=new Fixture(f.Id+"-DL"+dl);await d.Init();await d.SeedClass();var link=await Link(d);var id=await Member(d,link);using var p=await d.Client(dl==2?d.X:d.P);using var r=await d.Post(p,"/HocTap/LopHoc/PhanHoiLoiMoi",new(){{"id",id.ToString()},{"agree",(dl==0).ToString()},{"rowVersion",Convert.ToBase64String(await d.Version<ThanhVienLopHoc>(id))}});Assert.Equal(dl==2?HttpStatusCode.Forbidden:HttpStatusCode.OK,r.StatusCode);await using var db=d.Db();var m=await db.Set<ThanhVienLopHoc>().SingleAsync(x=>x.Id==id);Assert.Equal(dl==1?"TuChoi":"ChoDuyet",m.TrangThaiDuyet);}break;}
  case "member_approve":{
   for(int dl=0;dl<3;dl++){await using var d=new Fixture(f.Id+"-DL"+dl);await d.Init();await d.SeedClass();var link=await Link(d);var id=await Member(d,link,true);using var g=await d.Client(d.G);using var r=await d.Post(g,"/HocTap/LopHoc/Duyet",await Approval(d,id,permission:dl==0?"Xem":"ChinhSua",submit:dl==2));Assert.Equal(HttpStatusCode.OK,r.StatusCode);await using var db=d.Db();var m=await db.Set<ThanhVienLopHoc>().SingleAsync(x=>x.Id==id);Assert.Equal("DaDuyet",m.TrangThaiDuyet);Assert.Equal(dl==0?"Xem":"ChinhSua",m.MucQuyen);Assert.Equal(dl==2,m.ChoPhepNopBai);}break;}
  case "member_wrong_actor":{
   await f.SeedClass();var link=await Link(f);var id=await Member(f,link,true);var g2=await f.User("GiaSu");foreach(var actor in new[]{f.H,f.P,g2}){using var c=await f.Client(actor);using var r=await f.Post(c,"/HocTap/LopHoc/Duyet",await Approval(f,id));Assert.Equal(HttpStatusCode.Forbidden,r.StatusCode);}await using var db=f.Db();Assert.Equal("ChoDuyet",(await db.Set<ThanhVienLopHoc>().SingleAsync(x=>x.Id==id)).TrangThaiDuyet);break;}
  case "member_leave":{
   foreach(var role in new[]{"G","H","P"}){await using var d=new Fixture(f.Id+"-"+role);await d.Init();await d.SeedClass();var link=await Link(d);var id=await Member(d,link,true,true);using var c=await d.Client(role=="G"?d.G:role=="H"?d.H:d.P);using var r=await d.Post(c,"/HocTap/LopHoc/Duyet",await Approval(d,id,"DaRoiLop"));Assert.Equal(HttpStatusCode.OK,r.StatusCode);await using var db=d.Db();Assert.NotNull((await db.Set<ThanhVienLopHoc>().SingleAsync(x=>x.Id==id)).RoiLopLuc);}break;}
  case "member_revoke":{
   await f.SeedAssignment();var link=await Link(f);var id=await Member(f,link,true,true);using var p=await f.Client(f.P);using var g=await f.Client(f.G);using var r=await f.Post(g,"/HocTap/LopHoc/Duyet",await Approval(f,id));Assert.Equal(HttpStatusCode.OK,r.StatusCode);await using var db=f.Db();var room=await db.Set<CuocHoiThoai>().SingleAsync(x=>x.LopHocId==f.Class);using var chat=await f.Post(p,"/HocTap/TinNhan/Gui",new(){{"MaHoiThoai",room.Id.ToString()},{"MaThaoTac",Guid.NewGuid().ToString()},{"NoiDung","Không được gửi"}});Assert.Equal(HttpStatusCode.Forbidden,chat.StatusCode);using var submission=await f.Post(p,"/HocTap/BaiTap/Nop",new(){{"BaiTapGiaoId",f.Assignment.ToString()},{"MaThaoTac",Guid.NewGuid().ToString()},{"NoiDung","Không được nộp"}});Assert.Equal(HttpStatusCode.Forbidden,submission.StatusCode);Assert.False(await db.Set<TinNhan>().AnyAsync(x=>x.NguoiGuiId==f.P));Assert.False(await db.Set<BaiTapNop>().AnyAsync(x=>x.NguoiNopId==f.P));break;}
  case "member_preconditions":{
   for(int dl=0;dl<4;dl++){
    await using var d=new Fixture(f.Id+"-DL"+dl);await d.Init();await d.SeedClass();Guid linkId,memberId;
    if(dl==2){var otherStudent=await d.User("HocSinh");await using var seed=d.Db();var otherLink=new LienKetPhuHuynhHocSinh{Id=Guid.NewGuid(),PhuHuynhId=d.P,HocSinhId=otherStudent,NguoiGuiId=d.P,TrangThai="DaLienKet",GuiLuc=DateTimeOffset.UtcNow.AddMinutes(-2),XacNhanLuc=DateTimeOffset.UtcNow};seed.Add(otherLink);await seed.SaveChangesAsync();linkId=otherLink.Id;memberId=await Member(d,linkId,true);}
    else if(dl==3){
     var p2=await d.User("PhuHuynh");var p3=await d.User("PhuHuynh");await d.Sql("UPDATE dbo.LopHoc SET BenHocId=@p,HocSinhLienKetId=@h WHERE Id=@c",new(){{"@p",d.P},{"@h",d.H},{"@c",d.Class}});
     await using(var seed=d.Db()){var link3=new LienKetPhuHuynhHocSinh{Id=Guid.NewGuid(),PhuHuynhId=p3,HocSinhId=d.H,NguoiGuiId=p3,TrangThai="DaLienKet",GuiLuc=DateTimeOffset.UtcNow.AddMinutes(-2),XacNhanLuc=DateTimeOffset.UtcNow};seed.Add(link3);await seed.SaveChangesAsync();var link2=new LienKetPhuHuynhHocSinh{Id=Guid.NewGuid(),PhuHuynhId=p2,HocSinhId=d.H,NguoiGuiId=p2,TrangThai="DaLienKet",GuiLuc=DateTimeOffset.UtcNow.AddMinutes(-2),XacNhanLuc=DateTimeOffset.UtcNow};seed.Add(link2);await seed.SaveChangesAsync();var existing=new ThanhVienLopHoc{Id=Guid.NewGuid(),LopHocId=d.Class,TaiKhoanId=p3,NguoiDeNghiId=d.H,LienKetId=link3.Id,TrangThaiDuyet="DaDuyet",MucQuyen="Xem",ChoPhepNopBai=false,DeNghiLuc=DateTimeOffset.UtcNow.AddMinutes(-2),DongYThamGiaLuc=DateTimeOffset.UtcNow.AddMinutes(-1),DuyetLuc=DateTimeOffset.UtcNow};seed.Add(existing);var candidate=new ThanhVienLopHoc{Id=Guid.NewGuid(),LopHocId=d.Class,TaiKhoanId=p2,NguoiDeNghiId=d.H,LienKetId=link2.Id,TrangThaiDuyet="ChoDuyet",MucQuyen="Xem",DeNghiLuc=DateTimeOffset.UtcNow.AddMinutes(-2),DongYThamGiaLuc=DateTimeOffset.UtcNow.AddMinutes(-1)};seed.Add(candidate);await seed.SaveChangesAsync();linkId=link2.Id;memberId=candidate.Id;}
    }else{linkId=await Link(d);memberId=await Member(d,linkId,consent:dl!=0);if(dl==1)await d.Sql("UPDATE dbo.LienKetPhuHuynhHocSinh SET TrangThai='DaHuy',KetThucLuc=SYSDATETIMEOFFSET(),NguoiKetThucId=@p WHERE Id=@l",new(){{"@p",d.P},{"@l",linkId}});}
    using var g=await d.Client(d.G);using var response=await d.Post(g,"/HocTap/LopHoc/Duyet",await Approval(d,memberId));Assert.Equal(dl==2?403:409,(int)response.StatusCode);await using var observer=d.Db();var state=await observer.Set<ThanhVienLopHoc>().SingleAsync(x=>x.Id==memberId);Assert.Equal("ChoDuyet",state.TrangThaiDuyet);Assert.Null(state.DuyetLuc);
   }break;}
  case "family_cancel":{
   await f.SeedClass();var link=await Link(f);var member=await Member(f,link,consent:true,approved:true);
   using(var parent=await f.Client(f.P)){using var response=await f.Post(parent,"/HocTap/LienKetTaiKhoan/Huy",new(){{"id",link.ToString()},{"rowVersion",Convert.ToBase64String(await f.Version<LienKetPhuHuynhHocSinh>(link))}},"/TaiKhoan/DangNhap",ajax:false);Assert.Equal(HttpStatusCode.Redirect,response.StatusCode);Assert.Contains("/HocTap/LienKetTaiKhoan",response.Headers.Location?.OriginalString??"");}
   await using(var db=f.Db()){
    var relation=await db.Set<LienKetPhuHuynhHocSinh>().SingleAsync(x=>x.Id==link);
    var membership=await db.Set<ThanhVienLopHoc>().SingleAsync(x=>x.Id==member);
    Assert.Equal("DaHuy",relation.TrangThai);Assert.NotNull(relation.KetThucLuc);
    Assert.Equal("DaRoiLop",membership.TrangThaiDuyet);Assert.NotNull(membership.RoiLopLuc);
   }
   await using(var parent=f.Scope(f.P))await Assert.ThrowsAsync<LoiNghiepVu>(()=>parent.Rights.KiemTraQuyenXemAsync(f.Class));
   break;
  }
  case "member_isolation":{
   await f.SeedClass();var link=await Link(f);await Member(f,link,consent:true,approved:true);
   await using var other=new Fixture(f.Id+"-other");await other.Init();await other.SeedClass();
   using var relative=await other.Client(f.P);
   using var classResponse=await relative.GetAsync("/HocTap/LopHoc/ChiTiet/"+other.Class);
   Assert.Contains((int)classResponse.StatusCode,new[]{403,404});
   await using var otherDb=other.Db();var foreignRoom=await otherDb.Set<CuocHoiThoai>().SingleAsync(x=>x.LopHocId==other.Class);
   using var chat=await other.Post(relative,"/HocTap/TinNhan/Gui",new(){{"MaHoiThoai",foreignRoom.Id.ToString()},{"MaThaoTac",Guid.NewGuid().ToString()},{"NoiDung","không được ghi"}});
   Assert.Contains((int)chat.StatusCode,new[]{403,404});
   Assert.False(await otherDb.Set<TinNhan>().AnyAsync(x=>x.CuocHoiThoaiId==foreignRoom.Id&&x.NguoiGuiId==f.P));
   using var student=await other.Client(f.H);using var own=await student.GetAsync("/HocTap/LopHoc/ChiTiet/"+f.Class);
   Assert.Equal(HttpStatusCode.OK,own.StatusCode);
   break;
  }
  default:throw new InvalidOperationException(key);
  }
 }
}
