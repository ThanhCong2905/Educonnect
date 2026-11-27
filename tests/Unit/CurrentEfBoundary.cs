using System.Reflection;
using System.Text.Json;
using System.Security.Claims;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Areas.DanhGia.Services;
using EduConnect.Mvc.Areas.QuanTri.Services;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Xunit;
namespace EduConnect.UnitTests.Current;

// EF InMemory is a technical test double for predicates/tracking, never proof of SQL behavior.
// Force model creation in Arrange: the selected method runs without calling OnModelCreating.
internal static class CurrentEfBoundary
{
 private static readonly Guid U1=Guid.Parse("00000000-0000-0000-0000-000000000001"),U2=Guid.Parse("00000000-0000-0000-0000-000000000002"),N1=Guid.Parse("00000000-0000-0000-0000-000000000101");
 private static readonly DateTimeOffset T=DateTimeOffset.Parse("2026-11-24T10:00:00Z");
 private static AppDbContext Context(){var options=new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).ConfigureWarnings(w=>w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options;var db=new AppDbContext(options);_ = db.Model;return db;}
 private static object? Call(Type type,string name,object? sut,params object?[] args)=>CurrentDesignTests.Call(type,name,sut,args);
 private static async Task<object?> Await(object? task){await(Task)task!;return task!.GetType().GetProperty("Result")?.GetValue(task);}
 private static void Seed(AppDbContext db,params object[] rows){foreach(var user in rows.OfType<TaiKhoan>()){user.Email??=user.Id+"@example.vn";user.NormalizedEmail??=user.Email.ToUpperInvariant();}db.AddRange(rows);db.SaveChanges();db.ChangeTracker.Clear();}
 private static void NoWrites(AppDbContext db)=>Assert.DoesNotContain(db.ChangeTracker.Entries(),e=>e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);
 private sealed class NullText{public override string? ToString()=>null;}
 public static void Run(int n,string dl)=>RunAsync(n,dl).GetAwaiter().GetResult();
 private static async Task RunAsync(int n,string dl)
 {
  using var db=Context();using var cts=new CancellationTokenSource();var ct=cts.Token;
  if(n is >=1 and <=4){
   if(n==2)Seed(db,new MonHoc{Id=1,TenMon="Toán",DangSuDung=true},new MonHoc{Id=2,TenMon="Cờ vua",DangSuDung=false});
   if(n==3)Seed(db,new MonHoc{Id=8,TenMon="Toán Học",DangSuDung=true},new MonHoc{Id=3,TenMon="Toán Học",DangSuDung=true},new MonHoc{Id=4,TenMon="Ngữ Văn",DangSuDung=true},new MonHoc{Id=5,TenMon="Cờ vua",DangSuDung=true},new MonHoc{Id=6,TenMon="Vật Lý",DangSuDung=false});
   var sut=new NhuCauService(db,null!,null!);var rows=n<=2?await sut.MonTenAsync():await sut.MonAsync();
   Assert.Equal(n==2?new[]{1,2}:n==3?new[]{3,4}:Array.Empty<int>(),rows.Select(m=>m.Id).Order().ToArray());
   if(n==2){Assert.Equal("Cờ vua",rows.Single(m=>m.Id==2).TenMon);Assert.False(rows.Single(m=>m.Id==2).DangSuDung);}NoWrites(db);return;
  }
  if(n is 6 or 7){
   var sut=new NhuCauService(db,null!,null!);var op=Guid.NewGuid();var t0=DateTimeOffset.UtcNow;
   Call(typeof(NhuCauService),"GhiLuotDang",sut,N1,U1,n==6?"Dang":"MoLai",n==6?null:op);var t1=DateTimeOffset.UtcNow;
   var entity=Assert.Single(db.ChangeTracker.Entries<LuotDangNhuCau>()).Entity;
   Assert.Equal(N1,entity.NhuCauId);Assert.Equal(U1,entity.TaiKhoanId);Assert.Equal(n==6?"Dang":"MoLai",entity.HanhDong);Assert.NotEqual(Guid.Empty,entity.Id);Assert.NotEqual(Guid.Empty,entity.MaThaoTac);if(n==7)Assert.Equal(op,entity.MaThaoTac);Assert.InRange(entity.DangLuc,t0,t1);Assert.Equal(DateOnly.FromDateTime(entity.DangLuc.ToOffset(TimeSpan.FromHours(7)).DateTime),entity.NgayDang);Assert.Equal(EntityState.Added,db.Entry(entity).State);return;
  }
  if(n is >=9 and <=27){
   var h=new HoSoGiaSu{Id=N1,GiaSuId=U1,DangHienThi=true,TrangThaiDuyet="DaDuyet",DayTrucTuyen=true};
   var person=new NguoiThamGia{Id=U1,HoTen="Gia sư A",LoaiNguoi="GiaSu"};var g=new GiaSu{TaiKhoanId=U1};var subject=new MonHoc{Id=1,TenMon="Toán",DangSuDung=true};
   var lesson=new HoSoGiaSuMonDay{HoSoGiaSuId=N1,MonHocId=1,HocPhiDeXuat=100000,DonViHocPhi="VND/Gio"};
   if(n==10)h.DangHienThi=false;if(n==11)h.TrangThaiDuyet="BanNhap";if(n==13)h.DayTrucTuyen=false;if(n==16)person.LoaiNguoi="HocSinh";if(n==18)lesson.HocPhiDeXuat=null;if(n==19)lesson.DonViHocPhi=null;if(n==21)subject.DangSuDung=false;if(n==23){h.DayTrucTuyen=false;h.DayTrucTiep=true;}
   var seed=new List<object>{h};if(n!=14)seed.Add(g);if(n!=15)seed.Add(person);if(n!=17)seed.Add(lesson);if(n!=20)seed.Add(subject);
   if(n==22)seed.Add(new HoSoGiaSuMonDay{HoSoGiaSuId=N1,MonHocId=2,HocPhiDeXuat=100000,DonViHocPhi="VND/Gio"});
   if(n is 12 or >=24 and <=27)seed.Add(new BienPhapQuanTri{Id=Guid.NewGuid(),LoaiDoiTuong=n==26?"NhuCauHoc":"HoSoGiaSu",DoiTuongId=n==25?Guid.NewGuid():N1,Loai=n==27?"TamDungLop":"AnNoiDung",GoLuc=n==24?T:null});
   Seed(db,seed.ToArray());var type=typeof(NhuCauService).Assembly.GetType("EduConnect.Mvc.Areas.NhuCau.Services.HoSoCongKhaiQuery")!;var sut=Activator.CreateInstance(type,BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance,null,[db],null)!;
   var result=((IQueryable<HoSoGiaSu>)Call(type,"CongKhai",sut)!).ToList();if(n==9||n>=23)Assert.Equal(N1,Assert.Single(result).Id);else Assert.Empty(result);NoWrites(db);return;
  }
  if(n is >=28 and <=35){
   var need=new NhuCauHoc{Id=N1,ChuSoHuuId=U1,LoaiNhuCau="TimGiaSu",TrangThai="DangMo",MonHocId=1,KhoiLop="Lớp 8",MucTieu="Ôn Toán",HinhThucHoc="TrucTiep",KhuVuc="Hà Nội",PhuongXa="Phường Ba Đình",HocPhiDuKien=100000,DonViHocPhi="VND/Gio",TaoLuc=T};
   var rows=new List<object>();if(n!=29){rows.Add(need);if(n!=32)rows.Add(new TaiKhoan{Id=U1,HoTen="An"});if(n!=33)rows.Add(new NguoiThamGia{Id=U1,HoTen="An",LoaiNguoi="HocSinh"});if(n!=34)rows.Add(new MonHoc{Id=1,TenMon="Toán Học"});}if(n==31)need.LoaiNhuCau="LoaiKhac";if(n==35)rows.Add(new BienPhapQuanTri{Id=Guid.NewGuid(),DoiTuongId=N1,LoaiDoiTuong="NhuCauHoc",Loai="AnNoiDung",GoLuc=T});
   var n2=Guid.NewGuid();if(n==28){rows.Add(new TaiKhoan{Id=U2,HoTen="Bình",AnhDaiDien="avatar"});rows.Add(new NguoiThamGia{Id=U2,HoTen="Bình",LoaiNguoi="PhuHuynh"});rows.Add(new NhuCauHoc{Id=n2,ChuSoHuuId=U2,LoaiNhuCau="TimGiaSu",TrangThai="DangMo",MonHocId=1});foreach(var state in new[]{"BanNhap","DaDong","DangMo"}){var id=Guid.NewGuid();rows.Add(new NhuCauHoc{Id=id,ChuSoHuuId=U1,LoaiNhuCau="TimGiaSu",TrangThai=state,MonHocId=1});if(state=="DangMo")rows.Add(new BienPhapQuanTri{Id=Guid.NewGuid(),DoiTuongId=id,LoaiDoiTuong="NhuCauHoc",Loai="AnNoiDung"});}var tutor=Guid.NewGuid();rows.Add(new TaiKhoan{Id=tutor,HoTen="Gia sư"});rows.Add(new NguoiThamGia{Id=tutor,LoaiNguoi="GiaSu",HoTen="Gia sư"});rows.Add(new NhuCauHoc{Id=Guid.NewGuid(),ChuSoHuuId=tutor,LoaiNhuCau="TimGiaSu",TrangThai="DangMo",MonHocId=1});}
   Seed(db,rows.ToArray());var sut=new TimKiemService(db,null!);var result=((IQueryable<UngVienViewModel>)Call(typeof(TimKiemService),"NhuCauCongKhai",sut)!).ToList();
   if(n is 29 or >=31 and <=34)Assert.Empty(result);else{var first=result.Single(x=>x.Id==N1);Assert.Equal(U1,first.ChuSoHuuId);Assert.Equal("An",first.HoTen);Assert.Equal("HocSinh",first.LoaiNguoi);Assert.Equal("Toán Học",first.TenMon);Assert.Equal(1,first.MonHocId);Assert.Null(first.AnhDaiDienUrl);Assert.Equal("Lớp 8",first.KhoiLop);Assert.Equal("Ôn Toán",first.MucTieu);Assert.Equal("TrucTiep",first.HinhThucHoc);Assert.Equal("Hà Nội",first.KhuVuc);Assert.Equal("Phường Ba Đình",first.PhuongXa);Assert.Equal(100000m,first.HocPhiDuKien);Assert.Equal("VND/Gio",first.DonViHocPhi);Assert.Equal(T,first.TaoLuc);if(n==28){Assert.Equal(2,result.Count);Assert.Equal("/TaiKhoan/Anh/"+U2,result.Single(x=>x.Id==n2).AnhDaiDienUrl);}else Assert.Single(result);}NoWrites(db);return;
  }
  if(n is >=36 and <=40){
   var n2=Guid.Parse("00000000-0000-0000-0000-000000000102");var rows=new List<UngVienViewModel>();if(n!=40)rows.Add(new NhuCauCongKhaiViewModel{Id=N1,MonHocId=n is 36?1:n is 39?9:3,TenMon="Tên cũ"});if(n==39)rows.Add(new NhuCauCongKhaiViewModel{Id=n2,MonHocId=3,TenMon="Cũ"});
   if(n is 36 or 39)Seed(db,new MonHoc{Id=1,TenMon=n==36?"Toán Học":"Toán"},new MonHoc{Id=2,TenMon=n==36?"Ngữ Văn":"Lý"},new NhuCauMonHoc{NhuCauId=N1,MonHocId=2},new NhuCauMonHoc{NhuCauId=N1,MonHocId=1},new KhungGioRanh{Id=Guid.NewGuid(),NhuCauId=N1,ThuTrongTuan=1,GioBatDau=new(18,0),GioKetThuc=new(19,0)});
   if(n is 37 or 39)Seed(db,new MonHoc{Id=3,TenMon=n==37?"Tiếng Anh":"Anh"});
   await Await(Call(typeof(TimKiemService),"SlotsAsync",new TimKiemService(db,null!),rows,ct));
   if(n==40)Assert.Empty(rows);else{var r=rows[0];Assert.Equal(n is 36 or 39?new[]{1,2}:new[]{3},r.MonHocIds);Assert.Equal(n==36?"Toán Học, Ngữ Văn":n==37?"Tiếng Anh":n==38?"Tên cũ":"Toán, Lý",r.TenMon);if(n is 36 or 39){var slot=Assert.Single(r.Slots);Assert.Equal(1,slot.ThuTrongTuan);Assert.Equal(new TimeOnly(18,0),slot.GioBatDau);Assert.Equal(new TimeOnly(19,0),slot.GioKetThuc);}else Assert.Empty(r.Slots);if(n==39){Assert.Equal(new[]{3},rows[1].MonHocIds);Assert.Equal("Anh",rows[1].TenMon);Assert.Empty(rows[1].Slots);}}NoWrites(db);return;
  }
  if(n is 43 or 44){var other=Guid.NewGuid();var a=new KhungGioDeXuat{Id=Guid.NewGuid(),YeuCauKetNoiId=N1,ThuTrongTuan=1,GioBatDau=new(18,0),GioKetThuc=new(19,0)};var b=new KhungGioDeXuat{Id=Guid.NewGuid(),YeuCauKetNoiId=N1,ThuTrongTuan=2,GioBatDau=new(19,0),GioKetThuc=new(20,0)};Seed(db,a,b,new KhungGioDeXuat{Id=Guid.NewGuid(),YeuCauKetNoiId=other});var result=await new KetNoiService(db,null!,null!).KhungAsync(n==43?N1:Guid.NewGuid());if(n==44)Assert.Empty(result);else{Assert.Equal(new[]{a.Id,b.Id}.Order(),result.Select(x=>x.Id).Order());var r=result.Single(x=>x.Id==a.Id);Assert.Equal(a.ThuTrongTuan,r.ThuTrongTuan);Assert.Equal(a.GioBatDau,r.GioBatDau);Assert.Equal(a.GioKetThuc,r.GioKetThuc);}NoWrites(db);return;}
  if(n==46){var e=await Assert.ThrowsAsync<LoiNghiepVu>(()=>new EduConnect.Mvc.Areas.HocTap.Services.LopHocService(null!,null!,null!,null!,null!).KetThucAsync(N1,"AAAAAAAAAAE="));Assert.Equal(409,e.Status);Assert.Equal("Hãy phản hồi yêu cầu kết thúc bằng phiên bản yêu cầu đang hiển thị.",e.Message);return;}
  if(n is 41 or 42){
   var sut=new ThongBaoService(db,null!);var ev=Guid.NewGuid();var start=DateTimeOffset.UtcNow;sut.Tao(U1,"NhuCau",N1,"Thông báo A",n==41?ev:null);var end=DateTimeOffset.UtcNow;
   Assert.Equal(2,db.ChangeTracker.Entries().Count());var box=Assert.Single(db.ChangeTracker.Entries<SuKienChoXuLy>()).Entity;var note=Assert.Single(db.ChangeTracker.Entries<ThongBao>()).Entity;Assert.NotEqual(Guid.Empty,box.Id);if(n==41)Assert.Equal(ev,box.Id);Assert.Equal(box.Id,note.MaSuKien);Assert.Equal("NhuCau",box.LoaiSuKien);Assert.Equal("ChoXuLy",box.TrangThai);using var json=JsonDocument.Parse(box.DuLieuToiThieu);Assert.Equal(U1,json.RootElement.GetProperty("recipient").GetGuid());Assert.Equal(N1,json.RootElement.GetProperty("target").GetGuid());Assert.Equal(U1,note.NguoiNhanId);Assert.Equal(N1,note.DoiTuongId);Assert.Equal("Thông báo A",note.NoiDungTomTat);Assert.NotEqual(Guid.Empty,note.Id);Assert.InRange(box.TaoLuc,start,end);Assert.Equal(box.TaoLuc,note.TaoLuc);Assert.All(db.ChangeTracker.Entries(),e=>Assert.Equal(EntityState.Added,e.State));return;
  }
  if(n is 66 or 67){
   if(n==66)Seed(db,new TaiKhoan{Id=U1,HoTen="A",TrangThai="HoatDong"},new TaiKhoan{Id=U2,HoTen="B",TrangThai="BiKhoa"});
   var reviews=new[]{new DanhGiaDoiTac{Id=N1,NguoiNhanId=U1},new DanhGiaDoiTac{Id=Guid.NewGuid(),NguoiNhanId=U2},new DanhGiaDoiTac{Id=Guid.NewGuid(),NguoiNhanId=U1},new DanhGiaDoiTac{Id=Guid.NewGuid(),NguoiNhanId=U1}};Seed(db,n==67?[reviews[0]]:reviews.Cast<object>().ToArray());
   if(n==66)Seed(db,new BienPhapQuanTri{Id=Guid.NewGuid(),LoaiDoiTuong="DanhGiaDoiTac",DoiTuongId=reviews[2].Id,Loai="AnNoiDung"},new BienPhapQuanTri{Id=Guid.NewGuid(),LoaiDoiTuong="DanhGiaDoiTac",DoiTuongId=reviews[3].Id,Loai="AnNoiDung",GoLuc=T});
   var rows=DanhGiaDoiTacService.CongKhai(db).ToList();if(n==67)Assert.Empty(rows);else Assert.Equal(new[]{reviews[0].Id,reviews[3].Id}.Order(),rows.Select(x=>x.Id).Order());NoWrites(db);return;
  }
  if(n==68){var value=new DanhGiaDoiTac{Id=N1,SoSao=4,NhanXet="Dạy dễ hiểu",PhanHoi="Cảm ơn"};var sut=new DanhGiaDoiTacService(db,null!,null!,null!);Call(typeof(DanhGiaDoiTacService),"LuuLichSu",sut,value,U1,T);var history=Assert.Single(db.ChangeTracker.Entries<LichSuDanhGiaDoiTac>()).Entity;Assert.Equal(N1,history.DanhGiaId);Assert.Equal(4,history.SoSao);Assert.Equal("Dạy dễ hiểu",history.NhanXet);Assert.Equal("Cảm ơn",history.PhanHoi);Assert.Equal(U1,history.NguoiSuaId);Assert.Equal(T,history.ThayDoiLuc);Assert.Equal(EntityState.Added,db.Entry(history).State);Assert.Equal("Cảm ơn",value.PhanHoi);return;}
  if(n is >=69 and <=71){
   var r=Guid.NewGuid();if(n!=69)Seed(db,new VaiTro{Id=r,Name=n==70?null:"QuanTri"},new Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>{UserId=U1,RoleId=r});if(n==71){var other=Guid.NewGuid();Seed(db,new VaiTro{Id=other,Name="HocSinh"},new Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>{UserId=U2,RoleId=other});}
   var sut=new QuanTriService(db,null!,null!,null!,null!,null!);if(n==71)Assert.Equal("QuanTri",await Await(Call(typeof(QuanTriService),"VaiTroAsync",sut,U1)));else{var e=await Assert.ThrowsAsync<LoiNghiepVu>(()=>Await(Call(typeof(QuanTriService),"VaiTroAsync",sut,U1)));Assert.Equal(404,e.Status);Assert.Equal("Không tìm thấy tài khoản và vai trò hợp lệ.",e.Message);}NoWrites(db);return;
  }
  if(n is >=72 and <=75){
   var http=n==73?null:new DefaultHttpContext{TraceIdentifier=n==75?"":"REQ-1"};var sut=new QuyenQuanTriService(db,null!,new HttpContextAccessor{HttpContext=http});var start=DateTimeOffset.UtcNow;
   if(n==74)sut.GhiNhatKy(U1,"X","TaiKhoan",new NullText());else if(n==75)sut.GhiNhatKy(U1,"X","TaiKhoan",42);else sut.GhiNhatKy(U1,"PhanQuyenQuanTri","TaiKhoan",U2,"ThanhCong","Cấp quyền");var end=DateTimeOffset.UtcNow;
   var e=Assert.Single(db.ChangeTracker.Entries<NhatKyQuanTri>()).Entity;Assert.Equal(U1,e.NguoiThucHienId);Assert.Equal("TaiKhoan",e.LoaiDoiTuong);Assert.Equal(n==74?"":n==75?"42":U2.ToString(),e.DoiTuongId);Assert.Equal(n>=74?"X":"PhanQuyenQuanTri",e.HanhDong);Assert.Equal("ThanhCong",e.KetQua);Assert.Equal(n>=74?null:"Cấp quyền",e.ChiTiet);Assert.InRange(e.ThoiDiem,start,end);if(n==73)Assert.True(Guid.TryParse(e.MaLienKetYeuCau,out _));else Assert.Equal(n==75?"":"REQ-1",e.MaLienKetYeuCau);Assert.Equal(EntityState.Added,db.Entry(e).State);return;
  }
  if(n is >=76 and <=81){
   var type=new[]{"TaiKhoan","HoSoGiaSu","NhuCauHoc","TinNhan","DanhGiaDoiTac","KhongHopLe"}[n-76];Seed(db,new HoSoGiaSu{Id=N1,GiaSuId=n==77?U1:U2},new NhuCauHoc{Id=N1,ChuSoHuuId=n==78?U1:U2},new TinNhan{Id=N1,NguoiGuiId=n==79?U1:U2},new DanhGiaDoiTac{Id=N1,NguoiGuiId=n==80?U1:U2},new BienPhapQuanTri{Id=Guid.NewGuid(),LoaiDoiTuong="TinNhan",DoiTuongId=N1,Loai="AnNoiDung"});var sut=new QuanTriService(db,null!,null!,null!,null!,null!);if(n==81){var e=await Assert.ThrowsAsync<LoiNghiepVu>(()=>Await(Call(typeof(QuanTriService),"ChuSoHuuAsync",sut,type,N1)));Assert.Equal(400,e.Status);Assert.Equal("Loại đối tượng không hợp lệ.",e.Message);}else Assert.Equal(n==76?N1:U1,await Await(Call(typeof(QuanTriService),"ChuSoHuuAsync",sut,type,N1)));NoWrites(db);return;
  }
  if(n==82){var m=new BienPhapQuanTri{Id=N1,Loai="AnNoiDung"};var now=DateTimeOffset.Parse("2026-11-24T08:00:00Z");Call(typeof(QuanTriService),"Go",null,m,U1,now,dl=="DL01"?"  Đã khắc phục  ":"Đã khắc phục");Assert.Equal(now,m.GoLuc);Assert.Equal(U1,m.NguoiGoId);Assert.Equal("Đã khắc phục",m.LyDoGo);Assert.Equal(N1,m.Id);Assert.Equal("AnNoiDung",m.Loai);return;}
  if(n is 290 or 294 or 295 or 296){
   if(n is 294 or 295)Seed(db,new NguoiThamGia{Id=U1,LoaiNguoi="HocSinh",HoTen="Bình"},new NguoiThamGia{Id=U2,LoaiNguoi="HocSinh",HoTen="An"});var third=Guid.NewGuid();if(n==295)Seed(db,new NguoiThamGia{Id=third,LoaiNguoi="GiaSu",HoTen="Cường"});
   var accessor=new HttpContextAccessor{HttpContext=new DefaultHttpContext{User=new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier,U1.ToString())],"Test"))}};var sut=new NguoiDungHienTaiService(db,accessor);
   if(n==290){var e=await Assert.ThrowsAsync<LoiNghiepVu>(()=>sut.HienTaiAsync(ct));Assert.Equal(403,e.Status);Assert.Equal("Tài khoản hiện không thể truy cập.",e.Message);}if(n==294)Assert.Equal(U1,(await sut.HienTaiAsync(ct)).Id);if(n==295)Assert.Equal(new[]{third,U2,U1},(await sut.DanhSachAsync()).Select(x=>x.Id));if(n==296)Assert.Empty(await sut.DanhSachAsync());NoWrites(db);return;
  }
  throw new InvalidOperationException($"Missing fixture UT-{n:000}");
 }
}
