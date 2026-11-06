using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Text.Json;
var fixture=JsonDocument.Parse(await File.ReadAllTextAsync("tests/ProfileWorkflow/bin/Debug/net10.0/profile-results.json")).RootElement;
var database=fixture.GetProperty("database").GetString()!;if(!database.StartsWith("EduConnect_Verification_"))throw new Exception("Test DB only");
var cs=Environment.GetEnvironmentVariable("EDU_TEST_CONNECTION")??$"Server=lpc:CONGVJP;Database={database};Integrated Security=True;Encrypt=False";
if(new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(cs).InitialCatalog!=database)throw new Exception("Fixture DB mismatch");
using var db=new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(cs).Options);
var current=new NguoiDungMauService(db,new Accessor{HttpContext=new DefaultHttpContext{Session=new Session()}});var rights=new QuyenLopHocService(db,current);var env=new Env();var files=new TepService(db,current,rights,env);var chat=new TinNhanService(db,current,rights,files);
var tutor=fixture.GetProperty("tutor").GetGuid();var learner=fixture.GetProperty("learner").GetGuid();var outsider=Guid.NewGuid();var reader=Guid.NewGuid();var editor=Guid.NewGuid();
var room=await db.Set<CuocHoiThoai>().Where(c=>c.GiaSuId==tutor&&c.BenHocId==learner&&c.LoaiHoiThoai=="TruocKetNoi").Select(c=>c.Id).SingleAsync();var c=await db.Set<LopHoc>().SingleAsync(c=>c.GiaSuId==tutor&&c.BenHocId==learner);var classId=c.Id;c.TrangThai="DangHoatDong";
db.AddRange(new NguoiThamGia{Id=outsider,HoTen="Attachment outsider",LoaiNguoi="HocSinh"},new NguoiThamGia{Id=reader,HoTen="Attachment reader",LoaiNguoi="PhuHuynh"},new NguoiThamGia{Id=editor,HoTen="Attachment editor",LoaiNguoi="HocSinh"});await db.SaveChangesAsync();
foreach(var member in new[]{reader,editor})db.Add(new ThanhVienLopHoc{Id=Guid.NewGuid(),LopHocId=classId,TaiKhoanId=member,NguoiDeNghiId=tutor,TrangThaiDuyet="DaDuyet",MucQuyen=member==reader?"Xem":"ChinhSua",DeNghiLuc=DateTimeOffset.UtcNow,DuyetLuc=DateTimeOffset.UtcNow});await db.SaveChangesAsync();
var checks=new List<string>();void Check(bool b,string message){if(!b)throw new Exception(message);}async Task User(Guid id){db.ChangeTracker.Clear();await current.ChonAsync(id);}async Task Pass(string name,Func<Task> f){await f();checks.Add(name);Console.WriteLine("PASS "+name);}async Task Denied(Func<Task> f){try{await f();throw new Exception("Expected denial");}catch(LoiNghiepVu){}finally{db.ChangeTracker.Clear();}}
var png=Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+nmVQAAAAASUVORK5CYII=");
IFormFile FileOf(string name,byte[]? bytes=null,long? length=null){var data=bytes??png;return new FormFile(new MemoryStream(data),0,length??data.Length,"Files",name);}
await User(tutor);var first=new GuiTinNhanViewModel{MaHoiThoai=room,Files=[FileOf("ảnh.png")]};Guid firstMessage=Guid.Empty,fileId=Guid.Empty;
await Pass("Private chat accepts file-only message",async()=>{await chat.GuiAsync(first);var m=await db.Set<TinNhan>().SingleAsync(m=>m.MaThaoTac==first.MaThaoTac);firstMessage=m.Id;Check(m.NoiDung=="","Unexpected text");var f=await db.Set<TepDinhKem>().SingleAsync(f=>f.TinNhanId==m.Id);fileId=f.Id;Check(f.NguoiTaiLenId==tutor&&f.KichThuoc==png.Length,"Bad metadata");Check(File.Exists((await chat.TaiTepAsync(f.Id)).Path),"Bytes missing");});
await Pass("Same token and files do not duplicate upload",async()=>{await chat.GuiAsync(first);Check(await db.Set<TinNhan>().CountAsync(m=>m.MaThaoTac==first.MaThaoTac)==1&&await db.Set<TepDinhKem>().CountAsync(f=>f.TinNhanId==firstMessage)==1,"Duplicate write");});
await Pass("Same token with different file rejected",()=>Denied(()=>chat.GuiAsync(new(){MaHoiThoai=room,MaThaoTac=first.MaThaoTac,Files=[FileOf("different.png")]})));
await Pass("Other participant can download and feed includes attachments",async()=>{await User(learner);await chat.TaiTepAsync(fileId);var data=JsonSerializer.Serialize(await chat.DuLieuAsync(room,null,null,null));Check(data.Contains("attachments")&&data.Contains(fileId.ToString())&&!data.Contains("TenLuuTru"),"Missing file or storage leak");});
await Pass("Outsider cannot read feed or download",async()=>{await User(outsider);await Denied(()=>chat.TaiTepAsync(fileId));await Denied(()=>chat.DuLieuAsync(room,null,null,null));await User(learner);});
await Pass("Empty text and no files rejected",()=>Denied(()=>chat.GuiAsync(new(){MaHoiThoai=room})));
await Pass("More than five files rejected",()=>Denied(()=>chat.GuiAsync(new(){MaHoiThoai=room,Files=Enumerable.Range(0,6).Select(i=>FileOf(i+".png")).ToList()})));
await Pass("Over 10000000 bytes rejected",()=>Denied(()=>chat.GuiAsync(new(){MaHoiThoai=room,Files=[FileOf("large.png",length:10000001)]})));
await Pass("Empty, unsupported and disguised files rejected",async()=>{await Denied(()=>chat.GuiAsync(new(){MaHoiThoai=room,Files=[FileOf("empty.png",[])]}));await Denied(()=>chat.GuiAsync(new(){MaHoiThoai=room,Files=[FileOf("program.exe")]}));await Denied(()=>chat.GuiAsync(new(){MaHoiThoai=room,Files=[FileOf("bad.png","not PNG"u8.ToArray())]}));});
await Pass("Exactly 10000000 bytes accepted",async()=>{var large=new byte[10000000];png.CopyTo(large,0);await chat.GuiAsync(new(){MaHoiThoai=room,Files=[FileOf("boundary.png",large)]});});
await Pass("Text plus five files accepted",()=>chat.GuiAsync(new(){MaHoiThoai=room,NoiDung="Tài liệu <b>plain</b>",Files=Enumerable.Range(0,5).Select(i=>FileOf(i+".png")).ToList()}));
await Pass("PDF-only message accepted",()=>chat.GuiAsync(new(){MaHoiThoai=room,Files=[FileOf("document.pdf","%PDF-1.4 fixture"u8.ToArray())]}));
await User(learner);var classRoom=(await chat.TheoLopAsync(classId))!.Value;Guid classFile=Guid.Empty;
await Pass("Class chat accepts file-only message",async()=>{var m=new GuiTinNhanViewModel{MaHoiThoai=classRoom,Files=[FileOf("class.png")]};await chat.GuiAsync(m);var id=await db.Set<TinNhan>().Where(x=>x.MaThaoTac==m.MaThaoTac).Select(x=>x.Id).SingleAsync();classFile=(await chat.TepTheoTinAsync(classRoom,[id]))[id].Single().Id;});
await Pass("Read-only member downloads but cannot upload",async()=>{await User(reader);await chat.TaiTepAsync(classFile);await Denied(()=>chat.GuiAsync(new(){MaHoiThoai=classRoom,Files=[FileOf("denied.png")]}));});
await Pass("Editor can upload to class",async()=>{await User(editor);await chat.GuiAsync(new(){MaHoiThoai=classRoom,Files=[FileOf("editor.png")]});});
await Pass("Class membership does not grant private-file access",()=>Denied(()=>chat.TaiTepAsync(fileId)));
await Pass("Departed member cannot download class file",async()=>{await User(tutor);var m=await db.Set<ThanhVienLopHoc>().SingleAsync(m=>m.LopHocId==classId&&m.TaiKhoanId==editor);m.RoiLopLuc=DateTimeOffset.UtcNow;m.TrangThaiDuyet="DaRoiLop";await db.SaveChangesAsync();await User(editor);await Denied(()=>chat.TaiTepAsync(classFile));});
await Pass("Closed class blocks upload but keeps file history",async()=>{await User(tutor);var closed=await db.Set<LopHoc>().SingleAsync(c=>c.Id==classId);closed.TrangThai="DaKetThuc";await db.SaveChangesAsync();await Denied(()=>chat.GuiAsync(new(){MaHoiThoai=classRoom,Files=[FileOf("closed.png")]}));await chat.TaiTepAsync(classFile);});
await Pass("Private upload remains independent of closed class",()=>chat.GuiAsync(new(){MaHoiThoai=room,Files=[FileOf("private.png")]}));
await User(tutor);var reset=await db.Set<LopHoc>().SingleAsync(c=>c.Id==classId);reset.TrangThai="DangHoatDong";await db.SaveChangesAsync();
await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory,"results.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,database,tutor,learner,outsider,reader,editor,room,classRoom,classId,fileId,classFile,uploadRoot=env.ContentRootPath},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(checks.Count+" checks passed");
sealed class Accessor:IHttpContextAccessor{public HttpContext? HttpContext{get;set;}}
sealed class Session:ISession{readonly Dictionary<string,byte[]> data=[];public bool IsAvailable=>true;public string Id=>"attachment-test";public IEnumerable<string> Keys=>data.Keys;public void Clear()=>data.Clear();public Task CommitAsync(CancellationToken t=default)=>Task.CompletedTask;public Task LoadAsync(CancellationToken t=default)=>Task.CompletedTask;public void Remove(string k)=>data.Remove(k);public void Set(string k,byte[] v)=>data[k]=v;public bool TryGetValue(string k,out byte[] v)=>data.TryGetValue(k,out v!);}
sealed class Env:IWebHostEnvironment{public string ApplicationName{get;set;}="ChatAttachments";public string EnvironmentName{get;set;}="Development";public string WebRootPath{get;set;}="";public IFileProvider WebRootFileProvider{get;set;}=new NullFileProvider();public string ContentRootPath{get;set;}=Path.Combine(Path.GetTempPath(),"EduConnectChatAttachments");public IFileProvider ContentRootFileProvider{get;set;}=new NullFileProvider();}
