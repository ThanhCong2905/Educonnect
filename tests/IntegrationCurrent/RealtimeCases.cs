using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

internal static partial class Cases
{
    static ServiceProvider RealtimeProvider(Fixture f, RealtimeSink sink, PhienCapNhat registry)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHttpContextAccessor>(new PlainAccessor { HttpContext = new DefaultHttpContext() });
        services.AddSingleton<IWebHostEnvironment>(new TestEnvironment());
        services.AddSingleton(registry);
        services.AddSingleton<IHubContext<CapNhatThoiGianThucHub>>(sink);
        services.AddScoped<AppDbContext>(_ => f.Db(app: true));
        services.AddScoped<NguoiDungHienTaiService>();
        services.AddScoped<QuyenLopHocService>();
        services.AddScoped<TepService>();
        services.AddScoped<TinNhanService>();
        services.AddScoped<PhienDangNhapService>();
        return services.BuildServiceProvider();
    }

    static XuLySuKienWorker NewWorker(IServiceProvider p, RealtimeSink sink, PhienCapNhat registry)
        => new(p.GetRequiredService<IServiceScopeFactory>(), sink, registry, NullLogger<XuLySuKienWorker>.Instance);

    static async Task<LoginSessionContext> LoginSession(Fixture f, Guid actor)
    {
        var client = await f.Client(actor);
        await using var db = f.Db();
        var id = await db.Set<PhienDangNhap>().Where(x => x.TaiKhoanId == actor && x.ThuHoiLuc == null)
            .OrderByDescending(x => x.TaoLuc).Select(x => x.Id).FirstAsync();
        return new LoginSessionContext(client, id);
    }

    static PhienCapNhat.KetNoi Connect(PhienCapNhat registry, RealtimeSink sink, string connection, Guid session, Guid actor, Guid? room = null)
    {
        var id = session.ToString("D");
        registry.CapNhat(id, actor);
        var c = registry.Them(connection, id, actor, () => sink.Aborted.Add(connection));
        if (room.HasValue) c.HoiThoai[room.Value] = 0;
        return c;
    }

    static async Task<Guid> SeedNotice(Fixture f, Guid recipient, string text, Guid? eventId = null)
    {
        var id = eventId ?? Guid.NewGuid();
        await using var scope = f.Scope(recipient);
        scope.Notices.Tao(recipient, "LichHoc", Guid.NewGuid(), text, id);
        await scope.Db.SaveChangesAsync();
        return id;
    }

    static async Task DrainOldQueue(Fixture f)
    {
        await f.Sql("UPDATE dbo.SuKienChoXuLy SET TrangThai='HoanTat',XuLyXongLuc=SYSDATETIMEOFFSET(),ThuLaiLuc=NULL,LeaseId=NULL,LeaseHetHanLuc=NULL WHERE TrangThai IN ('ChoXuLy','ChoThuLai') OR (TrangThai='DangXuLy' AND LeaseHetHanLuc<=SYSDATETIMEOFFSET())");
    }

    internal static async Task RealtimeCases(Fixture f, string key)
    {
        switch (key)
        {
            case "outbox_commit":
            {
                await f.SeedClass(); await DrainOldQueue(f);
                using var login = await LoginSession(f, f.H);
                var registry = new PhienCapNhat(); var sink = new RealtimeSink();
                Connect(registry, sink, "it177-h", login.Session, f.H);
                using var provider = RealtimeProvider(f, sink, registry);
                using var worker = NewWorker(provider, sink, registry);
                var eventId = Guid.NewGuid();
                await using (var producer = f.Scope(f.H))
                await using (var tx = await producer.Db.Database.BeginTransactionAsync())
                {
                    producer.Notices.Tao(f.H, "LichHoc", f.Class, "IT-177 commit barrier", eventId);
                    await producer.Db.SaveChangesAsync();
                    await worker.XuLySuKienDenHanAsync(100, default);
                    Assert.Empty(sink.Sent);
                    await tx.CommitAsync();
                }
                await worker.XuLySuKienDenHanAsync(100, default);
                var sent = Assert.Single(sink.Sent);
                Assert.Contains(eventId.ToString(), sent.Data, StringComparison.OrdinalIgnoreCase);
                using var verify = f.Db();
                var ev = await verify.Set<SuKienChoXuLy>().SingleAsync(x => x.Id == eventId);
                Assert.Equal("HoanTat", ev.TrangThai); Assert.NotNull(ev.XuLyXongLuc); Assert.Null(ev.LeaseId); Assert.Null(ev.LeaseHetHanLuc);
                Assert.Single(await verify.Set<ThongBao>().Where(x => x.MaSuKien == eventId).ToListAsync());
                break;
            }
            case "outbox_retry":
            {
                await f.SeedClass(); await DrainOldQueue(f); using var login = await LoginSession(f, f.H);
                var registry = new PhienCapNhat(); var sink = new RealtimeSink { Fail = true };
                Connect(registry, sink, "it178-h", login.Session, f.H);
                using var provider = RealtimeProvider(f, sink, registry); using var worker = NewWorker(provider, sink, registry);
                var id = await SeedNotice(f, f.H, "IT-178 retry");
                await worker.XuLySuKienDenHanAsync(100, default);
                await using (var db = f.Db())
                {
                    var failed = await db.Set<SuKienChoXuLy>().SingleAsync(x => x.Id == id);
                    Assert.Equal("ChoThuLai", failed.TrangThai); Assert.Equal(1, failed.SoLanThu); Assert.NotNull(failed.ThuLaiLuc);
                    Assert.Null(failed.LeaseId); Assert.Single(await db.Set<ThongBao>().Where(x => x.MaSuKien == id).ToListAsync());
                }
                sink.Fail = false;
                await f.Sql("UPDATE dbo.SuKienChoXuLy SET ThuLaiLuc=DATEADD(second,-1,SYSDATETIMEOFFSET()) WHERE Id=@id", new() { ["@id"] = id });
                await worker.XuLySuKienDenHanAsync(100, default);
                Assert.Contains(sink.Sent, x => x.Data.Contains(id.ToString(), StringComparison.OrdinalIgnoreCase));
                await using var final = f.Db(); Assert.Equal("HoanTat", (await final.Set<SuKienChoXuLy>().SingleAsync(x => x.Id == id)).TrangThai); Assert.Single(await final.Set<ThongBao>().Where(x => x.MaSuKien == id).ToListAsync());
                break;
            }
            case "outbox_lease":
            {
                await f.SeedClass(); await DrainOldQueue(f); using var login = await LoginSession(f, f.H);
                var registry = new PhienCapNhat(); var sink = new RealtimeSink(); Connect(registry, sink, "it179-h", login.Session, f.H);
                using var provider = RealtimeProvider(f, sink, registry); using var first = NewWorker(provider, sink, registry); using var second = NewWorker(provider, sink, registry);
                var ready = await SeedNotice(f, f.H, "IT-179 ready"); var expired = await SeedNotice(f, f.H, "IT-179 expired"); var live = await SeedNotice(f, f.H, "IT-179 live");
                var liveLease = Guid.NewGuid();
                await f.Sql("UPDATE dbo.SuKienChoXuLy SET TrangThai='DangXuLy',LeaseId=@lease,LeaseHetHanLuc=DATEADD(minute,1,SYSDATETIMEOFFSET()) WHERE Id=@id", new() { ["@lease"] = liveLease, ["@id"] = live });
                await f.Sql("UPDATE dbo.SuKienChoXuLy SET TrangThai='DangXuLy',LeaseId=NEWID(),LeaseHetHanLuc=DATEADD(second,-1,SYSDATETIMEOFFSET()) WHERE Id=@id", new() { ["@id"] = expired });
                await Task.WhenAll(first.XuLySuKienDenHanAsync(100, default), second.XuLySuKienDenHanAsync(100, default));
                await using var db = f.Db();
                Assert.Equal("HoanTat", (await db.Set<SuKienChoXuLy>().SingleAsync(x => x.Id == ready)).TrangThai);
                Assert.Equal("HoanTat", (await db.Set<SuKienChoXuLy>().SingleAsync(x => x.Id == expired)).TrangThai);
                var held = await db.Set<SuKienChoXuLy>().SingleAsync(x => x.Id == live); Assert.Equal("DangXuLy", held.TrangThai); Assert.Equal(liveLease, held.LeaseId);
                Assert.Equal(2, sink.Sent.Count(x => x.Data.Contains(ready.ToString(), StringComparison.OrdinalIgnoreCase) || x.Data.Contains(expired.ToString(), StringComparison.OrdinalIgnoreCase)));
                break;
            }
            case "hub_revoke":
            {
                await f.SeedClass(); await DrainOldQueue(f); var link = await Link(f); var member = await Member(f, link, true, true);
                using var login = await LoginSession(f, f.P);
                Guid room; await using (var db = f.Db()) room = await db.Set<CuocHoiThoai>().Where(x => x.LopHocId == f.Class).Select(x => x.Id).SingleAsync();
                var registry = new PhienCapNhat(); var sink = new RealtimeSink(); var connection = Connect(registry, sink, "it180-p", login.Session, f.P, room);
                using var provider = RealtimeProvider(f, sink, registry); using var worker = NewWorker(provider, sink, registry);
                await using (var admin = f.Scope(f.G)) await Classes(admin).DuyetAsync(member, "DaRoiLop", "Xem", Convert.ToBase64String(await f.Version<ThanhVienLopHoc>(member)));
                await f.Scope(f.G).Chat.GuiAsync(new GuiTinNhanViewModel { MaHoiThoai = room, MaThaoTac = Guid.NewGuid(), NoiDung = "Tin sau khi gỡ quyền" });
                await worker.XuLySuKienDenHanAsync(100, default);
                Assert.DoesNotContain(room, connection.HoiThoai.Keys); Assert.Contains("it180-p", sink.Groups.Removed);
                Assert.DoesNotContain(sink.Sent, x => x.Group == connection.NhomHoiThoai(room));
                using var denied = await f.Post(login.Client, "/HocTap/TinNhan/Gui", new() { ["MaHoiThoai"] = room.ToString(), ["MaThaoTac"] = Guid.NewGuid().ToString(), ["NoiDung"] = "Không gửi được" });
                Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);
                using var verify = f.Db(); Assert.NotNull((await verify.Set<ThanhVienLopHoc>().SingleAsync(x => x.Id == member)).RoiLopLuc);
                login.Client.Dispose();
                break;
            }
            case "hub_background":
            {
                using var login = await LoginSession(f, f.H);
                await using var beforeDb = f.Db(); var session = await beforeDb.Set<PhienDangNhap>().SingleAsync(x => x.Id == login.Session); var before = session.HoatDongCuoiLuc;
                login.Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Edu-Background", "1");
                using var background = await login.Client.GetAsync("/Home/ChuaDoc"); Assert.Equal(HttpStatusCode.OK, background.StatusCode);
                await using var afterDb = f.Db(); var after = await afterDb.Set<PhienDangNhap>().SingleAsync(x => x.Id == login.Session);
                Assert.Equal(before, after.HoatDongCuoiLuc); Assert.Null(after.ThuHoiLuc);
                login.Client.DefaultRequestHeaders.Remove("X-Edu-Background");
                using var active = await login.Client.GetAsync("/Home/ChuaDoc"); Assert.Equal(HttpStatusCode.OK, active.StatusCode);
                await using var check = f.Db(); Assert.True((await check.Set<PhienDangNhap>().SingleAsync(x => x.Id == login.Session)).HoatDongCuoiLuc >= before);
                login.Client.Dispose(); break;
            }
            case "async_shutdown":
            {
                await f.SeedClass(); await DrainOldQueue(f); using var login = await LoginSession(f, f.H);
                var registry = new PhienCapNhat(); var sink = new RealtimeSink { Hold = true }; Connect(registry, sink, "it187-h", login.Session, f.H);
                using var provider = RealtimeProvider(f, sink, registry); var worker = NewWorker(provider, sink, registry);
                var eventId = await SeedNotice(f, f.H, "IT-187 shutdown");
                await worker.StartAsync(default);
                Assert.True(await Task.WhenAny(sink.Entered.Task, Task.Delay(TimeSpan.FromSeconds(10))) == sink.Entered.Task, "Worker did not enter transport before shutdown.");
                var stopping = worker.StopAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
                sink.Release.TrySetResult();
                await stopping;
                await using (var afterStop = f.Db())
                {
                    var pending = await afterStop.Set<SuKienChoXuLy>().SingleAsync(x => x.Id == eventId);
                    Assert.NotEqual("HoanTat", pending.TrangThai);
                }
                await f.Sql("UPDATE dbo.SuKienChoXuLy SET LeaseHetHanLuc=DATEADD(second,-1,SYSDATETIMEOFFSET()),ThuLaiLuc=NULL WHERE Id=@id AND TrangThai='DangXuLy'", new() { ["@id"] = eventId });
                sink.Hold = false; using var recovery = NewWorker(provider, sink, registry); await recovery.XuLySuKienDenHanAsync(100, default);
                await using var final = f.Db(); var completed = await final.Set<SuKienChoXuLy>().SingleAsync(x => x.Id == eventId); Assert.Equal("HoanTat", completed.TrangThai); Assert.NotNull(completed.XuLyXongLuc);
                worker.Dispose(); login.Client.Dispose(); break;
            }
            case "hub_session":
            {
                await f.SeedClass(); Guid room; await using (var db = f.Db()) room = await db.Set<CuocHoiThoai>().Where(x => x.LopHocId == f.Class).Select(x => x.Id).SingleAsync();
                var registry = new PhienCapNhat(); var groups = new RealtimeGroups();
                using (var login = await LoginSession(f, f.H))
                await using (var scope = f.Scope(f.H))
                {
                    var hub = new CapNhatThoiGianThucHub(registry, scope.Chat, new PhienDangNhapService(scope.Db, registry)) { Context = HubContext("valid-h", f.H, login.Session), Groups = groups };
                    await hub.OnConnectedAsync(); await hub.ThamGiaHoiThoai(room);
                    Assert.NotNull(registry.Lay("valid-h")); Assert.Contains(registry.Lay("valid-h")!.NhomHoiThoai(room), groups.Added);
                }
                await using (var scope = f.Scope(f.H))
                {
                    var anonymous = new CapNhatThoiGianThucHub(registry, scope.Chat, new PhienDangNhapService(scope.Db, registry)) { Context = HubContext("anonymous", null, null), Groups = groups };
                    await Assert.ThrowsAsync<HubException>(() => anonymous.OnConnectedAsync());
                }
                using (var revokedLogin = await LoginSession(f, f.H))
                {
                    await f.Sql("UPDATE dbo.PhienDangNhap SET ThuHoiLuc=SYSDATETIMEOFFSET() WHERE Id=@id", new() { ["@id"] = revokedLogin.Session });
                    await using var scope = f.Scope(f.H);
                    var revoked = new CapNhatThoiGianThucHub(registry, scope.Chat, new PhienDangNhapService(scope.Db, registry)) { Context = HubContext("revoked", f.H, revokedLogin.Session), Groups = groups };
                    await Assert.ThrowsAsync<HubException>(() => revoked.OnConnectedAsync());
                }
                using (var outsiderLogin = await LoginSession(f, f.X))
                await using (var scope = f.Scope(f.X))
                {
                    var outsider = new CapNhatThoiGianThucHub(registry, scope.Chat, new PhienDangNhapService(scope.Db, registry)) { Context = HubContext("outsider", f.X, outsiderLogin.Session), Groups = groups };
                    await outsider.OnConnectedAsync(); await Assert.ThrowsAsync<HubException>(() => outsider.ThamGiaHoiThoai(room));
                    Assert.DoesNotContain(registry.Lay("outsider")!.NhomHoiThoai(room), groups.Added);
                }
                break;
            }
            case "file_revoke_race":
            {
                await f.SeedAssignment(); var link = await Link(f); var member = await Member(f, link, true, true);
                Guid room; await using (var read = f.Db()) room = await read.Set<CuocHoiThoai>().Where(x => x.LopHocId == f.Class).Select(x => x.Id).SingleAsync();
                var root = Path.Combine(Path.GetTempPath(), "EduConnect-IT-" + f.Tag); Directory.CreateDirectory(root);
                var listener = new TcpListener(IPAddress.Loopback, 0); listener.Start();
                var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously); var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                var port = ((IPEndPoint)listener.LocalEndpoint).Port;
                var server = Task.Run(async () => { using var c = await listener.AcceptTcpClientAsync(); await using var stream = c.GetStream(); var one = new byte[1]; do { if (await stream.ReadAsync(one) == 0) return; } while (one[0] != 0); var h = new byte[4]; while (true) { await stream.ReadExactlyAsync(h); var n = System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(h); if (n == 0) break; var b = new byte[Math.Min(n,65536)]; var left=n; while(left>0){var read=await stream.ReadAsync(b.AsMemory(0,Math.Min(left,b.Length)));if(read==0)throw new EndOfStreamException();left-=read;} } entered.TrySetResult(); await release.Task; await stream.WriteAsync(Encoding.UTF8.GetBytes("stream: OK\0")); });
                try
                {
                    await using var scope = f.Scope(f.P); var env = new TestEnvironment { EnvironmentName="Integration", ContentRootPath=root, WebRootPath=Path.Combine(root,"wwwroot") };
                    var cfg = new Microsoft.Extensions.Configuration.ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?> { ["FileScanning:AllowUnscannedUploads"]="false", ["FileScanning:ClamdPort"]=port.ToString() }).Build();
                    var files = new TepService(scope.Db,scope.Current,scope.Rights,env,configuration:cfg); var chat = new TinNhanService(scope.Db,scope.Current,scope.Rights,files); var op=Guid.NewGuid();
                    var send = chat.GuiAsync(new GuiTinNhanViewModel { MaHoiThoai=room,MaThaoTac=op,NoiDung="race revoke",Files=[Upload("race.pdf",Encoding.ASCII.GetBytes("%PDF-1.7\nvalid\n%%EOF"))] });
                    Assert.True(await Task.WhenAny(entered.Task,Task.Delay(10_000))==entered.Task,"Scanner barrier was not reached.");
                    await using (var owner=f.Scope(f.G)) await Classes(owner).DuyetAsync(member,"DaRoiLop","Xem",Convert.ToBase64String(await f.Version<ThanhVienLopHoc>(member)));
                    release.TrySetResult(); var error=await Record.ExceptionAsync(async()=>await send); Assert.NotNull(error);
                    await using var verify=f.Db(); Assert.False(await verify.Set<TinNhan>().AnyAsync(x=>x.MaThaoTac==op)); Assert.False(await verify.Set<TepDinhKem>().AnyAsync(x=>x.TenGoc=="race.pdf")); Assert.False(await verify.Set<SuKienChoXuLy>().AnyAsync(x=>x.LoaiSuKien=="TinNhan"&&x.DuLieuToiThieu.Contains(op.ToString()))); Assert.Empty(Directory.Exists(Path.Combine(root,"uploads"))?Directory.GetFiles(Path.Combine(root,"uploads")):[]);
                }
                finally { release.TrySetResult(); listener.Stop(); try { await server; } catch { } if(Directory.Exists(root))Directory.Delete(root,true); }
                break;
            }
            default: throw new InvalidOperationException(key);
        }
    }

    static HubCallerContext HubContext(string connection, Guid? actor, Guid? session)
    {
        var claims = new List<Claim>(); if(actor.HasValue) claims.Add(new Claim(ClaimTypes.NameIdentifier, actor.Value.ToString())); if(session.HasValue) claims.Add(new Claim(PhienDangNhapService.SessionClaim,session.Value.ToString()));
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, actor.HasValue ? "Integration" : null)); return new TestHubCallerContext(connection,user);
    }
}

internal sealed class RealtimeSink : IHubContext<CapNhatThoiGianThucHub>
{
    public ConcurrentBag<(string Group,string Data)> Sent {get;}=[]; public ConcurrentBag<string> Aborted {get;}=[]; public bool Fail {get;set;} public bool Hold {get;set;}
    public TaskCompletionSource Entered {get;}=new(TaskCreationOptions.RunContinuationsAsynchronously); public TaskCompletionSource Release {get;}=new(TaskCreationOptions.RunContinuationsAsynchronously);
    public IHubClients Clients => new RealtimeClients(this); public RealtimeGroups Groups {get;} IGroupManager IHubContext<CapNhatThoiGianThucHub>.Groups => Groups;
    public RealtimeSink(){Groups=new RealtimeGroups();}
}
internal sealed class RealtimeClients(RealtimeSink sink):IHubClients
{
    public IClientProxy All=>Group("all"); public IClientProxy AllExcept(IReadOnlyList<string> ids)=>All; public IClientProxy Client(string id)=>Group(id); public IClientProxy Clients(IReadOnlyList<string> ids)=>All; public IClientProxy Group(string name)=>new RealtimeProxy(sink,name); public IClientProxy GroupExcept(string name,IReadOnlyList<string> ids)=>Group(name); public IClientProxy Groups(IReadOnlyList<string> names)=>All; public IClientProxy User(string id)=>Client(id); public IClientProxy Users(IReadOnlyList<string> ids)=>All;
}
internal sealed class RealtimeProxy(RealtimeSink sink,string group):IClientProxy
{
    public async Task SendCoreAsync(string method,object?[] args,CancellationToken token=default){if(sink.Fail)throw new IOException("Injected transport failure");if(sink.Hold){sink.Entered.TrySetResult();await sink.Release.Task;if(token.IsCancellationRequested)throw new OperationCanceledException(token);}sink.Sent.Add((group,JsonSerializer.Serialize(new{method,args})));}
}
internal sealed class RealtimeGroups:IGroupManager
{
    public ConcurrentBag<string> Added {get;}=[]; public ConcurrentBag<string> Removed {get;}=[];
    public Task AddToGroupAsync(string connectionId,string groupName,CancellationToken cancellationToken=default){Added.Add(groupName);return Task.CompletedTask;}
    public Task RemoveFromGroupAsync(string connectionId,string groupName,CancellationToken cancellationToken=default){Removed.Add(connectionId);return Task.CompletedTask;}
}
internal sealed class LoginSessionContext(HttpClient client,Guid session):IDisposable { public HttpClient Client {get;}=client; public Guid Session {get;}=session; public void Dispose()=>Client.Dispose(); }
internal sealed class TestHubCallerContext(string id,ClaimsPrincipal user):HubCallerContext
{
    public override string ConnectionId=>id; public override string? UserIdentifier=>user.FindFirstValue(ClaimTypes.NameIdentifier); public override ClaimsPrincipal User=>user; public override IDictionary<object,object?> Items {get;}=new Dictionary<object,object?>(); public override IFeatureCollection Features {get;}=new Microsoft.AspNetCore.Http.Features.FeatureCollection(); public override CancellationToken ConnectionAborted=>CancellationToken.None; public override void Abort(){}
}
