using System.Collections.Concurrent;
using System.Text.Json;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

var fixture=JsonDocument.Parse(File.ReadAllText("tests/ProfileWorkflow/bin/Debug/net10.0/profile-results.json")).RootElement;
var database=fixture.GetProperty("database").GetString()!;
if(!database.StartsWith("EduConnect_Verification_"))throw new Exception("Test database only");
var cs=Environment.GetEnvironmentVariable("EDU_TEST_CONNECTION")??$"Server=lpc:CONGVJP;Database={database};Integrated Security=True;Encrypt=False";
if(new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(cs).InitialCatalog!=database)throw new Exception("Wrong fixture DB");
var tutor=fixture.GetProperty("tutor").GetGuid();var learner=fixture.GetProperty("learner").GetGuid();
var accessor=new Accessor{HttpContext=new DefaultHttpContext{Session=new Session()}};
accessor.HttpContext.Session.SetString("participant",tutor.ToString());
var services=new ServiceCollection();services.AddLogging();services.AddSingleton<IHttpContextAccessor>(accessor);
services.AddDbContext<AppDbContext>(o=>o.UseSqlServer(cs));services.AddScoped<NguoiDungMauService>();
services.AddScoped<QuyenLopHocService>();services.AddScoped<TepService>();services.AddScoped<TinNhanService>();
services.AddSingleton<IWebHostEnvironment>(new Env());
using var provider=services.BuildServiceProvider();using var scope=provider.CreateScope();
var db=scope.ServiceProvider.GetRequiredService<AppDbContext>();var chat=scope.ServiceProvider.GetRequiredService<TinNhanService>();
var room=await db.Set<CuocHoiThoai>().Where(c=>c.GiaSuId==tutor&&c.BenHocId==learner&&c.LoaiHoiThoai=="TruocKetNoi").Select(c=>c.Id).SingleAsync();
var classRoom=await db.Set<CuocHoiThoai>().Where(c=>c.GiaSuId==tutor&&c.BenHocId==learner&&c.LoaiHoiThoai=="TheoLop").SingleAsync();
var phases=new PhienCapNhat();var hub=new FakeHub();
XuLySuKienWorker Worker()=>new(provider.GetRequiredService<IServiceScopeFactory>(),hub,phases,provider.GetRequiredService<ILogger<XuLySuKienWorker>>());
using var worker=Worker();
for(var i=0;i<5;i++)await worker.XuLySuKienDenHanAsync(100,default);
PhienCapNhat.KetNoi Connect(string name,Guid user){phases.CapNhat(name,user);return phases.Them(name,name,user,()=>hub.Aborted.Add(name));}
var tc=Connect("tutor",tutor);tc.HoiThoai[room]=0;var lc=Connect("learner",learner);lc.HoiThoai[room]=0;
var outsider=Guid.NewGuid();db.Add(new NguoiThamGia{Id=outsider,HoTen="Realtime outsider",LoaiNguoi="HocSinh"});await db.SaveChangesAsync();
var oc=Connect("outsider",outsider);oc.HoiThoai[room]=0;
var checks=new List<string>();void Check(bool ok,string message){if(!ok)throw new Exception(message);}
async Task Pass(string name,Func<Task> action){db.ChangeTracker.Clear();hub.Sent.Clear();await action();checks.Add(name);Console.WriteLine("PASS "+name);}
async Task<Guid> Message(string text){var m=new GuiTinNhanViewModel{MaHoiThoai=room,NoiDung=text};await chat.GuiAsync(m);return await db.Set<TinNhan>().Where(x=>x.MaThaoTac==m.MaThaoTac).Select(x=>x.Id).SingleAsync();}
bool SentTo(string group,Guid target)=>hub.Sent.Any(x=>x.Group==group&&x.Data.Contains(target.ToString()));
await Pass("Committed message notifies only authorized subscribers without content payload",async()=>{
 var id=await Message("Realtime committed");Check(!hub.Sent.Any(),"Published before worker");await worker.XuLySuKienDenHanAsync(100,default);
 Check(SentTo(tc.NhomHoiThoai(room),id)&&SentTo(lc.NhomHoiThoai(room),id),"Recipient missing");
 Check(!SentTo(oc.NhomHoiThoai(room),id)&&!oc.HoiThoai.ContainsKey(room),"Outsider received update");
 Check(!hub.Sent.Any(x=>x.Data.Contains("Realtime committed")),"Payload leaked content");
});
await Pass("Rollback after message insert leaves no message or event",async()=>{
 await db.Database.ExecuteSqlRawAsync("CREATE OR ALTER TRIGGER dbo.tr_RealtimeTestRollback ON dbo.SuKienChoXuLy AFTER INSERT AS THROW 51999,'test rollback',1;");
 var m=new GuiTinNhanViewModel{MaHoiThoai=room,NoiDung="Must rollback"};
 try{try{await chat.GuiAsync(m);throw new Exception("Expected failure");}catch(DbUpdateException){}}
 finally{await db.Database.ExecuteSqlRawAsync("DROP TRIGGER dbo.tr_RealtimeTestRollback");db.ChangeTracker.Clear();}
 Check(!await db.Set<TinNhan>().AnyAsync(x=>x.MaThaoTac==m.MaThaoTac),"Rolled back message remained");
 await worker.XuLySuKienDenHanAsync(100,default);Check(!hub.Sent.Any(),"Rollback published update");
});
await Pass("Notification is not published before commit and targets personal group",async()=>{
 var notices=new ThongBaoService(db,scope.ServiceProvider.GetRequiredService<NguoiDungMauService>());var target=Guid.NewGuid();
 await using(var tx=await db.Database.BeginTransactionAsync()){
  notices.Tao(learner,"KetNoi",target,"Private notification");await db.SaveChangesAsync();
  await worker.XuLySuKienDenHanAsync(100,default);Check(!hub.Sent.Any(),"Uncommitted notification published");await tx.CommitAsync();
 }
 await worker.XuLySuKienDenHanAsync(100,default);Check(SentTo(lc.NhomCaNhan,target)&&!SentTo(tc.NhomCaNhan,target),"Wrong notification audience");
});
await Pass("Failed dispatch retries without duplicating business data",async()=>{
 var id=await Message("Retry event");hub.Fail=true;await worker.XuLySuKienDenHanAsync(100,default);hub.Fail=false;
 var ev=await db.Set<SuKienChoXuLy>().AsNoTracking().SingleAsync(x=>x.LoaiSuKien=="TinNhan"&&x.DuLieuToiThieu.Contains(id.ToString()));
 Check(ev.TrangThai=="ChoThuLai"&&ev.SoLanThu==1,"Retry not scheduled");
 await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.SuKienChoXuLy SET ThuLaiLuc=DATEADD(second,-2,SYSDATETIMEOFFSET()) WHERE Id={ev.Id}");
 await worker.XuLySuKienDenHanAsync(100,default);Check(SentTo(lc.NhomHoiThoai(room),id),"Retry not sent");
 Check(await db.Set<TinNhan>().CountAsync(x=>x.Id==id)==1,"Business duplicate");
});
await Pass("Concurrent workers claim an event once",async()=>{
 var id=await Message("Concurrent lease");using var other=Worker();
 await Task.WhenAll(worker.XuLySuKienDenHanAsync(100,default),other.XuLySuKienDenHanAsync(100,default));
 Check(hub.Sent.Count(x=>x.Group==lc.NhomHoiThoai(room)&&x.Data.Contains(id.ToString()))==1,"Concurrent duplicate");
});
await Pass("Expired worker lease is recovered",async()=>{
 var id=await Message("Recover lease");var pattern="%"+id+"%";
 await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.SuKienChoXuLy SET TrangThai='DangXuLy',LeaseId=NEWID(),LeaseHetHanLuc=DATEADD(second,-1,SYSDATETIMEOFFSET()) WHERE LoaiSuKien='TinNhan' AND DuLieuToiThieu LIKE {pattern}");
 await worker.XuLySuKienDenHanAsync(100,default);Check(SentTo(lc.NhomHoiThoai(room),id),"Lease not recovered");
});
await Pass("Role switch and revoked session abort old connections",async()=>{
 phases.CapNhat("learner",tutor);Check(!phases.HopLe(lc)&&hub.Aborted.Contains("learner"),"Role switch kept connection");
 var revoked=Connect("revoked",learner);phases.ThuHoi("revoked");Check(!phases.HopLe(revoked)&&hub.Aborted.Contains("revoked"),"Revoked session remained");
 var id=await Message("After role switch");await worker.XuLySuKienDenHanAsync(100,default);Check(!SentTo(lc.NhomHoiThoai(room),id),"Old session got event");
});
await Pass("Departed member is removed before class message dispatch",async()=>{
 var reader=Guid.NewGuid();db.Add(new NguoiThamGia{Id=reader,HoTen="Realtime member",LoaiNguoi="HocSinh"});await db.SaveChangesAsync();
 var member=new ThanhVienLopHoc{Id=Guid.NewGuid(),LopHocId=classRoom.LopHocId!.Value,TaiKhoanId=reader,NguoiDeNghiId=tutor,TrangThaiDuyet="DaDuyet",MucQuyen="Xem",DeNghiLuc=DateTimeOffset.UtcNow,DuyetLuc=DateTimeOffset.UtcNow};db.Add(member);await db.SaveChangesAsync();
 await chat.KiemTraAsync(classRoom.Id,reader);var rc=Connect("reader",reader);rc.HoiThoai[classRoom.Id]=0;
 member.RoiLopLuc=DateTimeOffset.UtcNow;await db.SaveChangesAsync();
 var id=Guid.NewGuid();db.Add(new SuKienChoXuLy{Id=Guid.NewGuid(),LoaiSuKien="TinNhan",DuLieuToiThieu=JsonSerializer.Serialize(new{room=classRoom.Id,target=id}),TaoLuc=DateTimeOffset.UtcNow,TrangThai="ChoXuLy"});await db.SaveChangesAsync();
 await worker.XuLySuKienDenHanAsync(100,default);Check(!SentTo(rc.NhomHoiThoai(classRoom.Id),id)&&!rc.HoiThoai.ContainsKey(classRoom.Id),"Departed member got update");
});
await Pass("Expired idle session aborts without relying on cookie expiration",()=>{
 var clock=new TestClock();var registry=new PhienCapNhat(clock);var aborted=false;registry.CapNhat("expiry",learner);
 var c=registry.Them("expiry-connection","expiry",learner,()=>aborted=true);clock.Now+=PhienCapNhat.ThoiHan+TimeSpan.FromSeconds(1);registry.DonDep();
 Check(!registry.HopLe(c)&&aborted&&registry.Lay(c.Id)==null,"Expired session remained");return Task.CompletedTask;
});
Console.WriteLine($"{checks.Count} checks passed");Directory.CreateDirectory("docs/qa");File.WriteAllText("docs/qa/realtime-service-results.json",JsonSerializer.Serialize(new{database,checks},new JsonSerializerOptions{WriteIndented=true}));

sealed class FakeHub:IHubContext<CapNhatThoiGianThucHub>{public ConcurrentBag<(string Group,string Data)> Sent=new();public ConcurrentBag<string> Aborted=new();public bool Fail;public IHubClients Clients=>new ClientsImpl(this);public IGroupManager Groups=>new GroupsImpl();sealed class ClientsImpl(FakeHub h):IHubClients{public IClientProxy All=>new Proxy(h,"all");public IClientProxy AllExcept(IReadOnlyList<string> ids)=>All;public IClientProxy Client(string id)=>new Proxy(h,id);public IClientProxy Clients(IReadOnlyList<string> ids)=>All;public IClientProxy Group(string name)=>new Proxy(h,name);public IClientProxy GroupExcept(string name,IReadOnlyList<string> ids)=>Group(name);public IClientProxy Groups(IReadOnlyList<string> names)=>All;public IClientProxy User(string id)=>Client(id);public IClientProxy Users(IReadOnlyList<string> ids)=>All;}sealed class Proxy(FakeHub h,string group):IClientProxy{public Task SendCoreAsync(string method,object?[] args,CancellationToken token=default){if(h.Fail)throw new IOException("Injected dispatch failure");h.Sent.Add((group,JsonSerializer.Serialize(args)));return Task.CompletedTask;}}sealed class GroupsImpl:IGroupManager{public Task AddToGroupAsync(string c,string g,CancellationToken t=default)=>Task.CompletedTask;public Task RemoveFromGroupAsync(string c,string g,CancellationToken t=default)=>Task.CompletedTask;}}
sealed class Accessor:IHttpContextAccessor{public HttpContext? HttpContext{get;set;}}
sealed class Session:ISession{readonly Dictionary<string,byte[]> d=new();public bool IsAvailable=>true;public string Id{get;}=Guid.NewGuid().ToString();public IEnumerable<string> Keys=>d.Keys;public void Clear()=>d.Clear();public Task CommitAsync(CancellationToken t=default)=>Task.CompletedTask;public Task LoadAsync(CancellationToken t=default)=>Task.CompletedTask;public void Remove(string k)=>d.Remove(k);public void Set(string k,byte[] v)=>d[k]=v;public bool TryGetValue(string k,[System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out byte[]? v)=>d.TryGetValue(k,out v);}
sealed class Env:IWebHostEnvironment{public string ApplicationName{get;set;}="Tests";public string EnvironmentName{get;set;}="Development";public string ContentRootPath{get;set;}=Path.Combine(Path.GetTempPath(),"EduConnectRealtimeTests");public IFileProvider ContentRootFileProvider{get;set;}=new NullFileProvider();public string WebRootPath{get;set;}="";public IFileProvider WebRootFileProvider{get;set;}=new NullFileProvider();}
sealed class TestClock:TimeProvider{public DateTimeOffset Now=DateTimeOffset.UtcNow;public override DateTimeOffset GetUtcNow()=>Now;}
