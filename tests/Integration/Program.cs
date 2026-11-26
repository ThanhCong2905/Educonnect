using Microsoft.Extensions.Configuration;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
var cs=Environment.GetEnvironmentVariable("EDU_TEST_CONNECTION")??"Server=lpc:CONGVJP;Database=EduConnect_Verification_20261025;Integrated Security=True;Encrypt=False";
var connection=new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(cs);
if(!connection.InitialCatalog.StartsWith("EduConnect_Verification_"))throw new Exception("Refusing non-test database");
using var db=new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(cs).Options);
var http=new DefaultHttpContext{Session=new TestSession()};var current=new NguoiDungHienTaiService(db,new HttpContextAccessor{HttpContext=http});
var notices=new ThongBaoService(db,current);var rights=new QuyenLopHocService(db,current);var needs=new NhuCauService(db, current);var profiles=new HoSoGiaSuService(db,current,new MonHocCatalog(db),new TepService(db,current,rights,new TestEnv(),configuration:new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?> { ["FileScanning:AllowUnscannedUploads"]="true" }).Build()),notices);var contacts=new KetNoiService(db,current,notices);var search=new TimKiemService(db,current);var schedules=new LichHocService(db,current,rights,notices);var classes=new LopHocService(db,current,rights,notices,contacts);var files=new TepService(db,current,rights,new TestEnv(),configuration:new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?> { ["FileScanning:AllowUnscannedUploads"]="true" }).Build());var chat=new TinNhanService(db,current,rights,files);var tasks=new BaiTapService(db,current,rights,notices,files);
var report=new List<string>();
async Task Pass(string name,Func<Task> run){db.ChangeTracker.Clear();await run();report.Add(name);Console.WriteLine("PASS "+name);}
async Task Denied(Func<Task> run){try{await run();throw new Exception("Expected rejection");}catch(LoiNghiepVu){}catch(DbUpdateConcurrencyException){}catch(Microsoft.Data.SqlClient.SqlException ex) when(ex.Number>=51000){}finally{db.ChangeTracker.Clear();}}
void Check(bool value,string text){if(!value)throw new Exception(text);}
var tutor=Guid.NewGuid();var learner=Guid.NewGuid();var parent=Guid.NewGuid();var stranger=Guid.NewGuid();var profile=Guid.NewGuid();
await IdentityFixture.SeedAsync(db,new NguoiThamGia{Id=tutor,HoTen="Test tutor",LoaiNguoi="GiaSu"},new NguoiThamGia{Id=learner,HoTen="Test learner",LoaiNguoi="HocSinh"},new NguoiThamGia{Id=parent,HoTen="Test parent",LoaiNguoi="PhuHuynh"},new NguoiThamGia{Id=stranger,HoTen="Test outsider",LoaiNguoi="HocSinh"});await db.SaveChangesAsync();db.Add(new GiaSu{TaiKhoanId=tutor});await db.SaveChangesAsync();db.Add(new HoSoGiaSu{Id=profile,GiaSuId=tutor,TrinhDo="Đại học",ChuyenMon="Toán",KinhNghiem="2 năm",MoTa="Test",KhuVuc="Hà Nội",TrangThaiDuyet="DaDuyet",DangHienThi=true,DayTrucTuyen=true});await db.SaveChangesAsync();db.Add(new HoSoGiaSuMonDay{HoSoGiaSuId=profile,MonHocId=1,PhamViDay="Lớp 6, Lớp 7, Lớp 8, Lớp 9, Lớp 10, Lớp 11, Lớp 12",HocPhiDeXuat=100000,DonViHocPhi="VND/Gio"});await db.SaveChangesAsync();
await IdentityFixture.SignInAsync(http, tutor);Guid tutorNeed=Guid.Empty,parentNeed=Guid.Empty;
await Pass("Tutor cannot publish an unregistered subject",()=>Denied(()=>needs.LuuAsync(new(){MonHocId=2,MonHocIds=[2],MucTieu="Test"})));
await Pass("Availability overlap rejected",()=>Denied(()=>needs.LuuAsync(new(){MonHocId=1,KhoiLop="Lớp 12",MonHocIds=[1],MucTieu="Test",LichRanh=[new(),new()]})));
await Pass("Learner need saved in SQL",async()=>{await IdentityFixture.SignInAsync(http,learner);tutorNeed=await needs.LuuAsync(new(){MonHocId=1,KhoiLop="Lớp 12",MonHocIds=[1],MucTieu="Test open need",HocPhiDuKien=200000,DonViHocPhi="VND/Gio"});Check(await db.Set<NhuCauHoc>().AnyAsync(x=>x.Id==tutorNeed),"Missing need");});
await Pass("Learner cannot manage tutor profile",()=>Denied(()=>profiles.LayDeSuaAsync()));
await Pass("Stale need update is rejected",async()=>{var stale=(await needs.LayDeSuaAsync(tutorNeed)).DuLieu;var fresh=(await needs.LayDeSuaAsync(tutorNeed)).DuLieu;fresh.MucTieu="Updated";await needs.LuuAsync(fresh);db.ChangeTracker.Clear();stale.MucTieu="Stale";await Denied(()=>needs.LuuAsync(stale));});
await Pass("Slot-only edits also check the parent rowversion",async()=>{var stale=(await needs.LayDeSuaAsync(tutorNeed)).DuLieu;var fresh=(await needs.LayDeSuaAsync(tutorNeed)).DuLieu;fresh.LichRanh[0].ThuTrongTuan=2;await needs.LuuAsync(fresh);db.ChangeTracker.Clear();stale.LichRanh[0].ThuTrongTuan=3;await Denied(()=>needs.LuuAsync(stale));});
await IdentityFixture.SignInAsync(http, parent);
await Pass("Parent learner data stays private in search",async()=>{parentNeed=await needs.LuuAsync(new(){MonHocId=1,KhoiLop="Lớp 12",MonHocIds=[1],MucTieu="Parent test",TenNguoiHoc="PRIVATE_CHILD_NAME",TuoiNguoiHoc=12,HocPhiDuKien=100000,DonViHocPhi="VND/Gio"});await IdentityFixture.SignInAsync(http, tutor);var p=await search.ChiTietAsync(parentNeed);Check(!JsonSerializer.Serialize(p).Contains("PRIVATE_CHILD_NAME"),"Leaked child");});
await IdentityFixture.SignInAsync(http, learner);
await Pass("Connection before accepted contact rejected",()=>Denied(()=>contacts.GuiYeuCauAsync(new(){LienHeId=Guid.NewGuid(),LoiNhan="Test"})));
Guid contactId=Guid.Empty;
await Pass("Contact accepted and conversation created",async()=>{await contacts.GuiLienHeAsync(new(){DoiTuongId=tutor,LoiNhan="Test contact"});var contact=(await contacts.LayAsync()).Contacts.Single();contactId=contact.Id;await IdentityFixture.SignInAsync(http, tutor);await contacts.PhanHoiLienHeAsync(contact.Id,"ChapNhan",Convert.ToBase64String(contact.RowVersion));Check(await db.Set<CuocHoiThoai>().AnyAsync(x=>x.LienHeId==contactId),"No conversation");});
await IdentityFixture.SignInAsync(http, learner);
await Pass("Learner cannot create a class proposal",()=>Denied(()=>contacts.GuiDeXuatAsync(new(){LienHeId=contactId,TenLop="Invalid",MucTieu="Test"})));
var room=await db.Set<CuocHoiThoai>().Where(x=>x.LienHeId==contactId).Select(x=>x.Id).SingleAsync();
await IdentityFixture.SignInAsync(http, stranger);
await Pass("Outsider cannot read conversation",()=>Denied(()=>chat.LichSuAsync(room)));
await IdentityFixture.SignInAsync(http, learner);
await Pass("Message retry creates one durable message",async()=>{var m=new GuiTinNhanViewModel{MaHoiThoai=room,NoiDung="Persistent test",MaThaoTac=Guid.NewGuid()};await chat.GuiAsync(m);await chat.GuiAsync(m);Check(await db.Set<TinNhan>().CountAsync(x=>x.MaThaoTac==m.MaThaoTac)==1,"Duplicate message");});
Guid request=Guid.Empty,classId=Guid.Empty;
await IdentityFixture.SignInAsync(http, tutor);
var start=DateTime.UtcNow.AddHours(7).Date.AddDays(7);
await Pass("Tutor proposal has structured weekly slots",async()=>{await contacts.GuiDeXuatAsync(new(){LienHeId=contactId,TenNguoiHoc="Người học kiểm thử",TenLop="Integration class",MucTieu="Math",MonHocId=1,SoBuoiTuan=1,ThoiLuongPhut=90,BatDauHocLuc=start,HocPhi=200000,DonViHocPhi="VND/Gio",LichRanh=[new(){ThuTrongTuan=(byte)start.DayOfWeek,GioBatDau=new(19,0),GioKetThuc=new(20,30)}]});request=(await contacts.LayAsync()).Requests.Single().Id;});
await IdentityFixture.SignInAsync(http, learner);
await Pass("Accept twice creates one class and grants learner edit",async()=>{var r=await contacts.ChiTietAsync(request);var v=Convert.ToBase64String(r.RowVersion);classId=await contacts.ChapNhanAsync(request,v);await contacts.ChapNhanAsync(request,v);Check(await db.Set<LopHoc>().CountAsync(x=>x.YeuCauKetNoiId==request)==1,"Duplicate class");Check(await db.Set<ThanhVienLopHoc>().AnyAsync(x=>x.LopHocId==classId&&x.TaiKhoanId==learner&&x.MucQuyen=="ChinhSua"),"Wrong permission");});
await IdentityFixture.SignInAsync(http, tutor);
await Pass("Batch scheduling is atomic on overlap",async()=>{var before=await db.Set<LichHoc>().CountAsync(x=>x.LopHocId==classId);var day=start.AddDays(1);await Denied(()=>schedules.ThemHangLoatAsync(new(){LopHocId=classId,Buoi=[new(){TieuDe="A",BatDauLuc=day.AddHours(10),KetThucLuc=day.AddHours(11)},new(){TieuDe="B",BatDauLuc=day.AddHours(10).AddMinutes(30),KetThucLuc=day.AddHours(12)}]}));Check(await db.Set<LichHoc>().CountAsync(x=>x.LopHocId==classId)==before,"Partial batch saved");});
var future=await db.Set<LichHoc>().AsNoTracking().FirstAsync(x=>x.LopHocId==classId);
await Pass("Future lesson cannot be recorded",()=>Denied(()=>schedules.GhiNhanAsync(new(){LichHocId=future.Id,NhanXet="Invalid",NoiDung="Invalid"})));
await Pass("Cancelled lesson cannot be recorded",async()=>{var id=Guid.NewGuid();db.Add(new LichHoc{Id=id,LopHocId=classId,TieuDe="Cancelled fixture",BatDauLuc=DateTimeOffset.UtcNow.AddDays(-3),KetThucLuc=DateTimeOffset.UtcNow.AddDays(-3).AddHours(1),TrangThai="DaHuy"});await db.SaveChangesAsync();await Denied(()=>schedules.GhiNhanAsync(new(){LichHocId=id,NhanXet="Invalid",NoiDung="Invalid"}));});
var past=Guid.NewGuid();db.Add(new LichHoc{Id=past,LopHocId=classId,TieuDe="Past fixture",BatDauLuc=DateTimeOffset.UtcNow.AddDays(-1).AddHours(-2),KetThucLuc=DateTimeOffset.UtcNow.AddDays(-1),TrangThai="DaLenLich"});await db.SaveChangesAsync();
await Pass("Record lesson and retain previous version",async()=>{await schedules.GhiNhanAsync(new(){LichHocId=past,RowVersion=Convert.ToBase64String((await schedules.GhiNhanChiTietAsync(past)).Record!.RowVersion),NoiDung="Math lesson",NhanXet="Good"});var (r,_)=await schedules.GhiNhanChiTietAsync(past);await schedules.GhiNhanAsync(new(){LichHocId=past,NoiDung="Updated lesson",NhanXet="Updated",RowVersion=Convert.ToBase64String(r!.RowVersion)});db.ChangeTracker.Clear();var (latest,history)=await schedules.GhiNhanChiTietAsync(past);Check(latest!.PhienBan==r!.PhienBan+1&&history.Any(entry=>entry.PhienBan==r.PhienBan),"No record history");});
var lessonId=(await schedules.GhiNhanChiTietAsync(past)).Record!.Id;
await Pass("Assign homework",()=>tasks.GiaoAsync(new(){BuoiHocId=lessonId,LopHocId=classId,TieuDe="Test homework",NoiDung="Solve x+1=2",HanNopLuc=start.AddDays(7)}));
var assignment=await db.Set<BaiTapGiao>().Where(x=>x.LopHocId==classId).Select(x=>x.Id).SingleAsync();
await IdentityFixture.SignInAsync(http, learner);await Pass("Principal learner submits homework",()=>tasks.NopAsync(new(){BaiTapGiaoId=assignment,NoiDung="x=1"}));
var submission=await db.Set<BaiTapNop>().Where(x=>x.BaiTapGiaoId==assignment).Select(x=>x.Id).SingleAsync();await IdentityFixture.SignInAsync(http, tutor);
await Pass("Homework feedback without score",async()=>{await tasks.ChamAsync(new(){BaiTapNopId=submission,NhanXet="Correct",Diem=null});Check(await db.Set<KetQuaChamBai>().AnyAsync(x=>x.BaiTapNopId==submission&&x.Diem==null),"Score required unexpectedly");});
await Pass("Files checked by content signature",async()=>{using var stream=new MemoryStream("Not a PDF"u8.ToArray());await Denied(()=>files.KiemTraAsync([new FormFile(stream,0,stream.Length,"Files","fake.pdf")]));});
await Pass("Score precision greater than two decimals rejected",()=>Denied(()=>tasks.ChamAsync(new(){BaiTapNopId=submission,NhanXet="Bad precision",Diem=1.234m})));
await IdentityFixture.SignInAsync(http, learner);await Pass("No resubmission after grading",()=>Denied(()=>tasks.NopAsync(new(){BaiTapGiaoId=assignment,NoiDung="new answer"})));
await IdentityFixture.SignInAsync(http, stranger);await Pass("Outsider cannot access class or homework",async()=>{await Denied(()=>classes.ChiTietAsync(classId));await Denied(()=>tasks.ChiTietAsync(assignment));});
db.Add(new LienKetPhuHuynhHocSinh{Id=Guid.NewGuid(),PhuHuynhId=parent,HocSinhId=learner,NguoiGuiId=parent,TrangThai="DaLienKet",GuiLuc=DateTimeOffset.UtcNow,XacNhanLuc=DateTimeOffset.UtcNow});await db.SaveChangesAsync();
await Pass("Viewer member can read but cannot submit",async()=>{await IdentityFixture.SignInAsync(http, learner);await classes.DeNghiAsync(classId,(await db.Users.SingleAsync(user => user.Id == parent)).Email!);await IdentityFixture.SignInAsync(http, tutor);var member=(await classes.ThanhVienAsync(classId)).Single(x=>x.TaiKhoanId==parent);await IdentityFixture.SignInAsync(http,parent);await classes.PhanHoiLoiMoiAsync(member.Id,true,Convert.ToBase64String(member.RowVersion));await IdentityFixture.SignInAsync(http,tutor);member.RowVersion=(await db.Set<ThanhVienLopHoc>().AsNoTracking().SingleAsync(row=>row.Id==member.Id)).RowVersion;await classes.DuyetAsync(member.Id,"DaDuyet","Xem",Convert.ToBase64String(member.RowVersion));await IdentityFixture.SignInAsync(http, parent);await classes.ChiTietAsync(classId);await Denied(()=>rights.KiemTraQuyenGhiAsync(classId));await Denied(()=>tasks.NopAsync(new(){BaiTapGiaoId=assignment,NoiDung="viewer cannot submit"}));});
await Pass("Parent class retains a private learner snapshot",async()=>{await IdentityFixture.SignInAsync(http, parent);await contacts.GuiLienHeAsync(new(){DoiTuongId=tutor,LoiNhan="Parent contact"});var contact=(await contacts.LayAsync()).Contacts.Single(x=>x.GiaSuId==tutor);await IdentityFixture.SignInAsync(http, tutor);await contacts.PhanHoiLienHeAsync(contact.Id,"ChapNhan",Convert.ToBase64String(contact.RowVersion));await IdentityFixture.SignInAsync(http, parent);await contacts.GuiYeuCauAsync(new(){LienHeId=contact.Id,NhuCauId=parentNeed,LoiNhan="Connect for child"});var req=(await contacts.LayAsync()).Requests.Single();await IdentityFixture.SignInAsync(http, tutor);var next=start.AddDays(21);await contacts.GuiDeXuatAsync(new(){Id=req.Id,RowVersion=Convert.ToBase64String(req.RowVersion),LienHeId=contact.Id,NhuCauId=parentNeed,TenNguoiHoc="PRIVATE_CHILD_NAME",TenLop="Parent class",MucTieu="Math",BatDauHocLuc=next,SoBuoiTuan=1,ThoiLuongPhut=90,LichRanh=[new(){ThuTrongTuan=(byte)next.DayOfWeek,GioBatDau=new(19,0),GioKetThuc=new(20,30)}]});await IdentityFixture.SignInAsync(http, parent);req=await contacts.ChiTietAsync(req.Id);var id=await contacts.ChapNhanAsync(req.Id,Convert.ToBase64String(req.RowVersion));var m=(await needs.LayDeSuaAsync(parentNeed)).DuLieu;m.TenNguoiHoc="Changed after class";await needs.LuuAsync(m);db.ChangeTracker.Clear();var c=await classes.ChiTietAsync(id);Check(c.TenNguoiHoc=="PRIVATE_CHILD_NAME"&&c.BenHocId==parent&&c.TuoiNguoiHoc==12,"Learner snapshot changed or parent replaced");});
await Pass("Notifications cannot be read by another participant",async()=>{await IdentityFixture.SignInAsync(http, parent);var notice=(await notices.LayAsync()).First();await IdentityFixture.SignInAsync(http, stranger);await Denied(()=>notices.DocAsync(notice.Id));});
await Pass("Data survives a new DbContext",async()=>{await using var fresh=new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(cs).Options);Check(await fresh.Set<LopHoc>().AnyAsync(x=>x.Id==classId)&&await fresh.Set<TinNhan>().AnyAsync(x=>x.CuocHoiThoaiId==room),"Missing durable data");});
await Pass("One-subject drafts and atomic rejection of invalid subjects",async()=>{
 await IdentityFixture.SignInAsync(http,learner);
 var before=await db.Set<NhuCauHoc>().CountAsync(x=>x.ChuSoHuuId==learner);
 await needs.LuuAsync(new(){MonHocIds=[2],KhoiLop="Lớp 12",MucTieu="Single subject",BanNhap=true,LichRanh=[]});
 await Denied(()=>needs.LuuAsync(new(){MonHocIds=[1,2],MucTieu="Multiple subjects",BanNhap=true,LichRanh=[]}));
 await Denied(()=>needs.LuuAsync(new(){MonHocIds=[int.MaxValue],MucTieu="Invalid subject",BanNhap=true,LichRanh=[]}));
 Check(await db.Set<NhuCauHoc>().CountAsync(x=>x.ChuSoHuuId==learner)==before+1,"Invalid input wrote partial needs");
});
await Pass("Class chat excluded from private inbox and viewer cannot send",async()=>{await IdentityFixture.SignInAsync(http, parent);Check((await chat.LayAsync()).All(c=>c.LoaiHoiThoai=="TruocKetNoi"),"Class room leaked into inbox");var group=await chat.TheoLopAsync(classId);Check(group.HasValue,"Missing class room");Check(!await chat.CoTheGuiAsync(group!.Value),"Viewer can send");await Denied(()=>chat.GuiAsync(new(){MaHoiThoai=group.Value,NoiDung="Not allowed",MaThaoTac=Guid.NewGuid()}));await IdentityFixture.SignInAsync(http, tutor);Check(await chat.CoTheGuiAsync(group.Value),"Tutor cannot send");await IdentityFixture.SignInAsync(http, learner);Check(await chat.CoTheGuiAsync(group.Value),"Principal learner cannot send");});

await Pass("Tutor picker uses profile subjects and tutor cannot create demand", async () =>
{
    await IdentityFixture.SignInAsync(http, tutor);
    var choices = await needs.MonDuocChonAsync();
    Check(choices.Count == 1 && choices[0].Id == 1, "Picker contains unregistered subjects");
    await Denied(() => needs.LuuAsync(new() { MonHocIds = [1], BanNhap = true }));
});
await Pass("Vietnamese and emoji survive EF and SQL", async () =>
{
    var value = "Nguyễn Thị Ánh, Toán, ă â ê ô ơ ư đ Ắ Ễ Ợ 👍 " + Guid.NewGuid().ToString("N");
    var subject = new MonHoc { TenMon = value, DangSuDung = true };
    db.Add(subject); await db.SaveChangesAsync();
    await using var fresh = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(cs).Options);
    Check(await fresh.Set<MonHoc>().Where(row => row.Id == subject.Id).Select(row => row.TenMon).SingleAsync() == value, "Unicode changed");
});
await Pass("Search retains separate public needs of one learner", async () =>
{
    var owner = Guid.NewGuid(); var tag = "SearchNeed" + owner.ToString("N");
    await IdentityFixture.SeedAsync(db, new NguoiThamGia { Id = owner, HoTen = tag, LoaiNguoi = "HocSinh" });
    await IdentityFixture.SignInAsync(http, owner);
    var ids = new List<Guid>();
    for (var day = 1; day <= 3; day++)
        ids.Add(await needs.LuuAsync(new() { MonHocIds = [1], KhoiLop = "Lớp 12", MucTieu = tag + day, SoBuoiTuan = 1, LichRanh = [new() { ThuTrongTuan = (byte)day }] }));
    await IdentityFixture.SignInAsync(http, tutor);
    var results = await search.TimAsync(new() { TuKhoa = tag });
    Check(results.Total == 3 && results.Items.Select(row => row.Id).Order().SequenceEqual(ids.Order()), "Search collapsed separate needs");
    var related = await search.LienQuanAsync(owner);
    Check(related.Count == 3 && related.All(row => row.Slots.Count == 1), "Related needs lost schedules");
});
var unifiedTutor = Guid.NewGuid();
var subjectA = new MonHoc { TenMon = "Suggest A " + unifiedTutor, DangSuDung = true };
var subjectB = new MonHoc { TenMon = "Suggest B " + unifiedTutor, DangSuDung = true };
db.AddRange(subjectA, subjectB); await db.SaveChangesAsync();
await IdentityFixture.SeedAsync(db, new NguoiThamGia { Id = unifiedTutor, HoTen = "Unified tutor", LoaiNguoi = "GiaSu" });
db.Add(new GiaSu { TaiKhoanId = unifiedTutor }); await db.SaveChangesAsync();
Guid certificateId = Guid.Empty; string certificatePath = "";
IFormFile Certificate(string name, bool valid = true)
{
    var bytes = valid ? Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+nmVQAAAAASUVORK5CYII=") : "not an image"u8.ToArray();
    return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "TepTrinhDo", name);
}
var certificates = new BangCapService(db, current, new TestEnv(), new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?> { ["FileScanning:AllowUnscannedUploads"]="true" }).Build());
await Pass("Profile and required private evidence save atomically", async () =>
{
    await IdentityFixture.SignInAsync(http, unifiedTutor);
    await profiles.CapNhatAsync(new()
    {
        HoTen = "Unified tutor", TrinhDo = "Đại học", KinhNghiem = "Chưa có", KhuVuc = "Hà Nội",
        MoTa = new string('a', 120), DayTrucTuyen = true, TepTrinhDo = Certificate("degree.png"),
        MonDay = [
            new() { MonHocId = subjectA.Id, LopDay = [12], HocPhiDeXuat = 100000, DonViHocPhi = "VND/Gio" },
            new() { MonHocId = subjectB.Id, LopDay = [12], HocPhiDeXuat = 120000, DonViHocPhi = "VND/Gio" }
        ]
    });
    var ownerProfile = await db.Set<HoSoGiaSu>().SingleAsync(row => row.GiaSuId == unifiedTutor);
    var file = await db.Set<TepDinhKem>().SingleAsync(row => row.HoSoGiaSuId == ownerProfile.Id);
    certificateId = file.Id; certificatePath = (await certificates.TaiAsync(file.Id)).Path;
    Check(File.Exists(certificatePath), "Certificate bytes missing");
    await IdentityFixture.SignInAsync(http, learner);
    await Denied(() => certificates.TaiAsync(file.Id));
});
await Pass("Invalid evidence leaves the existing profile unchanged", async () =>
{
    await IdentityFixture.SignInAsync(http, unifiedTutor);
    var form = await profiles.LayDeSuaAsync(); form.HoTen = "Should not save"; form.TepTrinhDo = Certificate("invalid.png", false);
    await Denied(() => profiles.CapNhatAsync(form));
    Check((await profiles.LayDeSuaAsync()).HoTen == "Unified tutor" && File.Exists(certificatePath), "Partial profile or file update");
});
await Pass("Replacing degree evidence removes only the selected file", async () =>
{
    var form = await profiles.LayDeSuaAsync(); form.XoaTepIds = [certificateId]; form.TepTrinhDo = Certificate("replacement.png");
    await profiles.CapNhatAsync(form);
    var saved = await certificates.CuaToiAsync();
    Check(saved.Count == 1 && saved[0].TenGoc == "replacement.png" && !File.Exists(certificatePath), "Replacement failed");
});
await Pass("Suggestions match registered subjects and opposite roles", async () =>
{
    var owner = Guid.NewGuid();
    await IdentityFixture.SeedAsync(db, new NguoiThamGia { Id = owner, HoTen = "Suggestion learner", LoaiNguoi = "HocSinh" });
    await IdentityFixture.SignInAsync(http, owner);
    await needs.LuuAsync(new() { MonHocIds = [subjectA.Id], KhoiLop = "Lớp 12", MucTieu = "Suggestion A", LichRanh = [new() { ThuTrongTuan = 1 }] });
    await needs.LuuAsync(new() { MonHocIds = [subjectB.Id], KhoiLop = "Lớp 12", MucTieu = "Suggestion B", LichRanh = [new() { ThuTrongTuan = 2 }] });
    await IdentityFixture.SignInAsync(http, unifiedTutor);
    var forTutor = await search.GoiYAsync();
    Check(forTutor.Items.Any(row => row.ChuSoHuuId == owner) && forTutor.Items.All(row => row.LoaiNguoi != "GiaSu"), "Tutor suggestions use wrong role or subjects");
    var profileRow = await db.Set<HoSoGiaSu>().SingleAsync(row => row.GiaSuId == unifiedTutor);
    profileRow.TrangThaiDuyet = "DaDuyet"; profileRow.DangHienThi = true; await db.SaveChangesAsync();
    await IdentityFixture.SignInAsync(http, owner);
    var forLearner = await search.GoiYAsync();
    Check(forLearner.Items.Any(row => row.ChuSoHuuId == unifiedTutor) && forLearner.Items.All(row => row.LoaiNguoi == "GiaSu"), "Learner suggestions use wrong role");
});
var output=Path.Combine(AppContext.BaseDirectory,"integration-results.json");await File.WriteAllTextAsync(output,JsonSerializer.Serialize(new{passed=report.Count,checks=report,database=connection.InitialCatalog},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine($"{report.Count} checks passed. {output}");
sealed class TestSession:ISession {readonly Dictionary<string,byte[]> items=[];public bool IsAvailable=>true;public string Id=>"integration";public IEnumerable<string> Keys=>items.Keys;public void Clear()=>items.Clear();public Task CommitAsync(CancellationToken t=default)=>Task.CompletedTask;public Task LoadAsync(CancellationToken t=default)=>Task.CompletedTask;public void Remove(string k)=>items.Remove(k);public void Set(string k,byte[] v)=>items[k]=v;public bool TryGetValue(string k,out byte[] v)=>items.TryGetValue(k,out v!);}
sealed class TestEnv:IWebHostEnvironment {public string ApplicationName{get;set;}="Integration";public string EnvironmentName{get;set;}="Development";public string WebRootPath{get;set;}="";public IFileProvider WebRootFileProvider{get;set;}=new NullFileProvider();public string ContentRootPath{get;set;}=Path.Combine(Path.GetTempPath(),"EduConnectIntegration");public IFileProvider ContentRootFileProvider{get;set;}=new NullFileProvider();}
