using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Xunit;

internal sealed class Mt1Case : IAsyncDisposable
{
    public const string Password = "MT1 integration password 2026!";
    public static string Connection
    {
        get
        {
            var value = Environment.GetEnvironmentVariable("EDU_TEST_CONNECTION") ??
                @"Server=LAPTOP-3L2V1AFT\SQLEXPRESS;Database=EduConnect_Verification_MT1;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
            var cs = new SqlConnectionStringBuilder(value);
            if (!string.Equals(cs.InitialCatalog, "EduConnect_Verification_MT1", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Bộ chạy chỉ chấp nhận EduConnect_Verification_MT1; không chạy trên DB ứng dụng.");
            return cs.ConnectionString;
        }
    }
    public static string Root { get; } = FindRoot();
    private static string FindRoot()
    {
        for (var d = new DirectoryInfo(AppContext.BaseDirectory); d != null; d = d.Parent)
            if (File.Exists(Path.Combine(d.FullName, "EduConnect.Mvc.csproj")))
                return d.FullName;
        throw new DirectoryNotFoundException("Không tìm thấy thư mục ứng dụng.");
    }
    public static string Evidence => Environment.GetEnvironmentVariable("MT1_EVIDENCE") ?? Path.Combine(Root, "TestResults", "MT1");
    public string Id
    {
        get;
    }
    public string Tag { get; } = "MT1IT_" + Guid.NewGuid().ToString("N");
    public List<Guid> Users { get; } = [];
    public List<int> Subjects { get; } = [];
    public Dictionary<Guid, string> Roles { get; } = [];
    public Guid HS, PH, GS, HS2;
    public Guid P1;
    public int Math => Subjects[0];
    public int Literature => Subjects[1];
    public Dictionary<string, Guid> Profiles { get; } = [];
    public Dictionary<string, object?> Proof { get; } = [];
    public Mt1Factory? Factory;
    private Mt1Case(string id)
    {
        Id = id;
        Proof["caseId"] = id;
        Proof["tag"] = Tag;
        Proof["database"] = new SqlConnectionStringBuilder(Connection).InitialCatalog;
    }
    public AppDbContext Db(params IInterceptor[] interceptors)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(Connection);
        if (interceptors.Length > 0)
            options.AddInterceptors(interceptors);
        return new(options.Options);
    }
    public static async Task<Mt1Case> Create(string id)
    {
        var c = new Mt1Case(id);
        try
        {
            await using var db = c.Db();
            if (!await db.Database.CanConnectAsync())
                throw new InvalidOperationException("Không kết nối được SQL kiểm thử.");
            c.HS = await c.User("HocSinh", "HS-A");
            c.PH = await c.User("PhuHuynh", "PH-A");
            c.GS = await c.User("GiaSu", "GS-A");
            c.HS2 = await c.User("HocSinh", "HS-B");
            foreach (var name in new[] { "Toán", "Ngữ văn" })
            {
                var m = new MonHoc { TenMon = name, DangSuDung = true };
                db.Add(m);
                await db.SaveChangesAsync();
                c.Subjects.Add(m.Id);
            }
            c.P1 = await c.Profile(c.GS, c.Math, 150000, "VND/Gio", true, false, "Thủ Đức");
            c.Profiles["P1"] = c.P1;
            c.Proof["actors"] = new
            {
                c.HS,
                c.PH,
                c.GS,
                c.HS2
            };
            c.Proof["ownedUsers"] = c.Users;
            c.Proof["subjects"] = c.Subjects;
            return c;
        }
        catch { await c.DisposeAsync(); throw; }
    }
    public async Task<Guid> User(string role, string label)
    {
        var u = new TaiKhoan { Id = Guid.NewGuid(), HoTen = Tag + " " + label, UserName = Tag + label + "@example.invalid", Email = Tag + label + "@example.invalid", TrangThai = "HoatDong", SecurityStamp = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() };
        u.NormalizedUserName = u.UserName.ToUpperInvariant();
        u.NormalizedEmail = u.Email.ToUpperInvariant();
        u.PasswordHash = new PasswordHasher<TaiKhoan>().HashPassword(u, Password);
        Users.Add(u.Id);
        Roles[u.Id] = role;
        await using var db = Db();
        db.Add(u);
        await db.SaveChangesAsync();
        var roleId = await db.Roles.Where(x => x.NormalizedName == role.ToUpperInvariant()).Select(x => x.Id).SingleAsync();
        db.Add(new IdentityUserRole<Guid> { UserId = u.Id, RoleId = roleId });
        if (role == "GiaSu")
            db.Add(new GiaSu { TaiKhoanId = u.Id });
        else if (role == "PhuHuynh")
            db.Add(new PhuHuynh { TaiKhoanId = u.Id });
        else
            db.Add(new HocSinh { TaiKhoanId = u.Id });
        await db.SaveChangesAsync();
        return u.Id;
    }
    public async Task<Guid> Profile(Guid actor, int subject, decimal fee, string unit, bool online, bool direct, string area, bool visible = true, string approved = "DaDuyet")
    {
        var h = new HoSoGiaSu { Id = Guid.NewGuid(), GiaSuId = actor, TrinhDo = "Đại học", ChuyenMon = Tag + " Toán", KinhNghiem = "3 năm", MoTa = Tag + " Ôn tập", KhuVuc = area, DayTrucTuyen = online, DayTrucTiep = direct, DangHienThi = visible, TrangThaiDuyet = approved, DuyetLuc = DateTimeOffset.UtcNow };
        await using var db = Db();
        db.Add(h);
        await db.SaveChangesAsync();
        db.Add(new HoSoGiaSuMonDay { HoSoGiaSuId = h.Id, MonHocId = subject, PhamViDay = "Lớp 8", HocPhiDeXuat = fee, DonViHocPhi = unit });
        await db.SaveChangesAsync();
        return h.Id;
    }
    public async Task ProfileSet()
    {
        foreach (var label in new[] { "P2", "P3", "P4", "P5", "P6", "P7" })
        {
            var actor = await User("GiaSu", label);
            var subject = label == "P3" ? Literature : Math;
            if (label == "P6")
                subject = await Subject(false);
            var id = await Profile(actor, subject, label == "P2" ? 200000 : label == "P7" ? 300000 : 120000, label == "P2" ? "VND/Buoi" : "VND/Gio", label != "P2", label == "P2", label is "P2" or "P3" ? "Hà Nội" : "Thủ Đức", label != "P4" && label != "P5", label == "P5" ? "ChoDuyet" : "DaDuyet");
            Profiles[label] = id;
            if (label == "P7")
            {
                await using var db = Db();
                db.Add(new HoSoGiaSuMonDay { HoSoGiaSuId = id, MonHocId = Literature, PhamViDay = "Lớp 8", HocPhiDeXuat = 100000, DonViHocPhi = "VND/Gio" });
                await db.SaveChangesAsync();
            }
        }
        Proof["profiles"] = Profiles;
    }
    public async Task<int> Subject(bool active = true)
    {
        await using var db = Db();
        var m = new MonHoc { TenMon = Tag + Subjects.Count, DangSuDung = active };
        db.Add(m);
        await db.SaveChangesAsync();
        Subjects.Add(m.Id);
        return m.Id;
    }
    public TaoNhuCauBenHocViewModel Input() => new() { MonHocIds = [Math], KhoiLop = "Lớp 8", HocLuc = "Trung bình", MucTieu = Tag, SoBuoiTuan = 2, HinhThucHoc = "TrucTuyen", HocPhiDuKien = 150000, DonViHocPhi = "VND/Gio", LichRanh = [new() { ThuTrongTuan = 1, GioBatDau = new(19, 0), GioKetThuc = new(20, 0) }, new() { ThuTrongTuan = 3, GioBatDau = new(19, 0), GioKetThuc = new(20, 0) }] };
    public Mt1Scope Scope(Guid? actor = null, params IInterceptor[] interceptors) => new(this, actor ?? HS, interceptors);
    public async Task<Guid> CreateNeed(TaoNhuCauBenHocViewModel? input = null, Guid? actor = null)
    {
        await using var s = Scope(actor);
        var r = await s.Needs.TaoNhuCauHocAsync(input ?? Input(), actor ?? HS, default);
        Assert.True(r.ThanhCong, $"Fixture tạo nhu cầu lỗi {r.MaTrangThai}: {r.ThongBao}");
        return r.DuLieu;
    }
    public async Task<List<Guid>> NeedSet()
    {
        var ids = new List<Guid>();
        for (var i = 0; i < 12; i++)
        {
            var m = Input();
            m.LichRanh = [new() { ThuTrongTuan = 1, GioBatDau = new(18, 0), GioKetThuc = new(20, 0) }];
            var id = await CreateNeed(m);
            ids.Add(id);
            await using var db = Db();
            await db.Set<NhuCauHoc>().Where(x => x.Id == id).ExecuteUpdateAsync(s => s.SetProperty(x => x.TaoLuc, new DateTimeOffset(2026, 10, 28, 12, 0, 0, TimeSpan.FromHours(7)).AddMinutes(-i)));
        }
        var closed = await CreateNeed();
        var draft = Input();
        draft.BanNhap = true;
        var other = Input();
        other.MonHocIds = [Literature];
        await CreateNeed(draft);
        await CreateNeed(other);
        await using (var db = Db())
            await db.Set<NhuCauHoc>().Where(x => x.Id == closed).ExecuteUpdateAsync(s => s.SetProperty(x => x.TrangThai, "DaDong"));
        Proof["needIdsOrdered"] = ids;
        return ids;
    }
    public async Task<NhuCauHoc> ReadNeed(Guid id)
    {
        await using var db = Db();
        return await db.Set<NhuCauHoc>().AsNoTracking().SingleAsync(x => x.Id == id);
    }
    public async Task<List<KhungGioRanh>> ReadSlots(Guid id)
    {
        await using var db = Db();
        return await db.Set<KhungGioRanh>().AsNoTracking().Where(x => x.NhuCauId == id).OrderBy(x => x.ThuTrongTuan).ToListAsync();
    }
    public async Task<int> NeedCount()
    {
        await using var db = Db();
        return await db.Set<NhuCauHoc>().CountAsync(x => Users.Contains(x.ChuSoHuuId));
    }
    public async Task<YeuCauLienHe> Contact()
    {
        await using var db = Db();
        return await db.Set<YeuCauLienHe>().AsNoTracking().SingleAsync(x => x.GiaSuId == GS && x.BenHocId == HS);
    }
    public async Task<Guid> AcceptContact()
    {
        await using (var s = Scope())
            await s.Contacts.GuiLienHeAsync(new()
            {
                DoiTuongId = GS,
                LoiNhan = "Muốn học Toán"
            });
        var c = await Contact();
        await using var tutor = Scope(GS);
        await tutor.Contacts.PhanHoiLienHeAsync(c.Id, "ChapNhan", Convert.ToBase64String(c.RowVersion));
        return (await tutor.Contacts.HoiThoaiAsync(c.Id))!.Value;
    }
    public async Task<HttpClient> Client(Guid? actor)
    {
        Factory ??= new Mt1Factory();
        var client = Factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, BaseAddress = new Uri("https://localhost"), HandleCookies = true });
        if (actor.HasValue)
        {
            await using var db = Db();
            var email = await db.Users.Where(x => x.Id == actor).Select(x => x.Email).SingleAsync();
            var token = await Token(client, "/TaiKhoan/DangNhap");
            using var r = await client.PostAsync("/TaiKhoan/DangNhap", new FormUrlEncodedContent(new Dictionary<string, string> { { "Email", email! }, { "MatKhau", Password }, { "__RequestVerificationToken", token } }));
            Assert.Equal(HttpStatusCode.Redirect, r.StatusCode);
        }
        return client;
    }
    public static async Task<string> Token(HttpClient client, string path)
    {
        using var r = await client.GetAsync(path);
        var html = await r.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var match = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        Assert.True(match.Success, "Không tìm thấy token trên " + path);
        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }
    public async Task RecordHttp(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        body = Regex.Replace(body, "(name=\"__RequestVerificationToken\"[^>]*value=\")[^\"]+", "$1[redacted]");
        Proof["http"] = new
        {
            status = (int)response.StatusCode,
            location = response.Headers.Location?.ToString(),
            contentType = response.Content.Headers.ContentType?.ToString(),
            body
        };
    }
    public async Task<string> Snapshot()
    {
        await using var db = Db();
        var needs = await db.Set<NhuCauHoc>().AsNoTracking().Where(x => Users.Contains(x.ChuSoHuuId)).Select(x => new { x.Id, x.ChuSoHuuId, x.MonHocId, x.TrangThai, x.MucTieu, x.KhuVuc, x.DiaChiChiTiet, x.TenNguoiHoc, x.TuoiNguoiHoc, x.RowVersion }).ToListAsync();
        var ids = needs.Select(x => x.Id).ToList();
        var slots = await db.Set<KhungGioRanh>().AsNoTracking().Where(x => ids.Contains(x.NhuCauId)).ToListAsync();
        var contacts = await db.Set<YeuCauLienHe>().AsNoTracking().Where(x => Users.Contains(x.BenHocId) || Users.Contains(x.GiaSuId)).ToListAsync();
        var messages = await db.Set<TinNhan>().CountAsync(x => Users.Contains(x.NguoiGuiId));
        var notices = await db.Set<ThongBao>().CountAsync(x => Users.Contains(x.NguoiNhanId));
        Proof["actualDatabase"] = new
        {
            needs,
            slots,
            contacts,
            messages,
            notices
        };
        return $"Nhu cầu={needs.Count}; lịch={slots.Count}; liên hệ={contacts.Count}; tin nhắn={messages}; thông báo={notices}.";
    }
    public async ValueTask DisposeAsync()
    {
        if (Factory != null)
            await Factory.DisposeAsync();
        if (Users.Count == 0 && Subjects.Count == 0)
            return;
        await using var db = Db();
        await using var tx = await db.Database.BeginTransactionAsync();
        var contacts = await db.Set<YeuCauLienHe>().Where(x => Users.Contains(x.GiaSuId) || Users.Contains(x.BenHocId)).Select(x => x.Id).ToListAsync();
        var rooms = await db.Set<CuocHoiThoai>().Where(x => Users.Contains(x.GiaSuId ?? Guid.Empty) || Users.Contains(x.BenHocId ?? Guid.Empty)).Select(x => x.Id).ToListAsync();
        var messages = await db.Set<TinNhan>().Where(x => Users.Contains(x.NguoiGuiId)).Select(x => x.Id).ToListAsync();
        var events = await db.Set<ThongBao>().Where(x => Users.Contains(x.NguoiNhanId)).Select(x => x.MaSuKien).ToListAsync();
        await db.Set<ThongBao>().Where(x => Users.Contains(x.NguoiNhanId)).ExecuteDeleteAsync();
        foreach (var user in Users)
            await db.Set<SuKienChoXuLy>().Where(x => x.DuLieuToiThieu.Contains(user.ToString())).ExecuteDeleteAsync();
        foreach (var room in rooms)
            await db.Set<SuKienChoXuLy>().Where(x => x.DuLieuToiThieu.Contains(room.ToString())).ExecuteDeleteAsync();
        await db.Set<ThongBao>().Where(x => Users.Contains(x.NguoiNhanId)).ExecuteDeleteAsync();
        await db.Set<SuKienChoXuLy>().Where(x => events.Contains(x.Id)).ExecuteDeleteAsync();
        await db.Set<TepDinhKem>().Where(x => Users.Contains(x.NguoiTaiLenId)).ExecuteDeleteAsync();
        await db.Set<TinNhan>().Where(x => messages.Contains(x.Id)).ExecuteDeleteAsync();
        await db.Set<CuocHoiThoai>().Where(x => rooms.Contains(x.Id)).ExecuteDeleteAsync();
        await db.Set<YeuCauKetNoiLopHoc>().Where(x => Users.Contains(x.GiaSuId) || Users.Contains(x.BenHocId)).ExecuteDeleteAsync();
        await db.Set<YeuCauLienHe>().Where(x => contacts.Contains(x.Id)).ExecuteDeleteAsync();
        var needs = await db.Set<NhuCauHoc>().Where(x => Users.Contains(x.ChuSoHuuId)).Select(x => x.Id).ToListAsync();
        await db.Set<KhungGioRanh>().Where(x => needs.Contains(x.NhuCauId)).ExecuteDeleteAsync();
        await db.Set<NhuCauMonHoc>().Where(x => needs.Contains(x.NhuCauId)).ExecuteDeleteAsync();
        await db.Set<NhuCauHoc>().Where(x => needs.Contains(x.Id)).ExecuteDeleteAsync();
        var profiles = await db.Set<HoSoGiaSu>().Where(x => Users.Contains(x.GiaSuId)).Select(x => x.Id).ToListAsync();
        await db.Set<HoSoGiaSuMonDay>().Where(x => profiles.Contains(x.HoSoGiaSuId)).ExecuteDeleteAsync();
        await db.Set<HoSoGiaSu>().Where(x => profiles.Contains(x.Id)).ExecuteDeleteAsync();
        await db.Set<PhienDangNhap>().Where(x => Users.Contains(x.TaiKhoanId)).ExecuteDeleteAsync();
        await db.Set<IdentityUserRole<Guid>>().Where(x => Users.Contains(x.UserId)).ExecuteDeleteAsync();
        await db.Set<GiaSu>().Where(x => Users.Contains(x.TaiKhoanId)).ExecuteDeleteAsync();
        await db.Set<HocSinh>().Where(x => Users.Contains(x.TaiKhoanId)).ExecuteDeleteAsync();
        await db.Set<PhuHuynh>().Where(x => Users.Contains(x.TaiKhoanId)).ExecuteDeleteAsync();
        await db.Users.Where(x => Users.Contains(x.Id)).ExecuteDeleteAsync();
        await db.Set<MonHoc>().Where(x => Subjects.Contains(x.Id)).ExecuteDeleteAsync();
        await tx.CommitAsync();
        Proof["cleanup"] = "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.";
    }
}

internal sealed class Mt1Scope : IAsyncDisposable
{
    public AppDbContext Db
    {
        get;
    }
    public NguoiDungHienTaiService Current
    {
        get;
    }
    public NhuCauService Needs
    {
        get;
    }
    public TimKiemService Search
    {
        get;
    }
    public KetNoiService Contacts
    {
        get;
    }
    public TinNhanService Chat
    {
        get;
    }
    public Mt1Scope(Mt1Case c, Guid actor, IInterceptor[] interceptors)
    {
        Db = c.Db(interceptors);
        var http = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, actor.ToString()), new Claim(ClaimTypes.Role, c.Roles[actor])], "MT1-service-fixture")) };
        Current = new(Db, new Mt1HttpContextAccessor { HttpContext = http });
        Needs = new(Db, Current, new MonHocCatalog(Db));
        Search = new(Db, Current);
        Contacts = new(Db, Current, new ThongBaoService(Db, Current));
        var rights = new QuyenLopHocService(Db, Current);
        Chat = new(Db, Current, rights, new TepService(Db, Current, rights, new Mt1Environment()));
    }
    public ValueTask DisposeAsync() => Db.DisposeAsync();
}
internal sealed class Mt1Environment : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = "EduConnect.Mvc"; public string EnvironmentName { get; set; } = "Integration";
    public string ContentRootPath { get; set; } = Mt1Case.Root; public string WebRootPath { get; set; } = Path.Combine(Mt1Case.Root, "wwwroot");
    public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = new Microsoft.Extensions.FileProviders.NullFileProvider();
    public Microsoft.Extensions.FileProviders.IFileProvider WebRootFileProvider { get; set; } = new Microsoft.Extensions.FileProviders.NullFileProvider();
}
internal sealed class Mt1Factory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(Mt1Case.Root).UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?> { { "ConnectionStrings:EduConnect", Mt1Case.Connection }, { "Authentication:AccountRequestLimit", "10000" } }));
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<Microsoft.EntityFrameworkCore.Infrastructure.IDbContextOptionsConfiguration<AppDbContext>>();
            services.AddDbContext<AppDbContext>(o => o.UseSqlServer(Mt1Case.Connection));
            foreach (var s in services.Where(x => x.ServiceType == typeof(IHostedService) && (x.ImplementationType == typeof(BaoTriWorker) || x.ImplementationType == typeof(XuLySuKienWorker))).ToList())
                services.Remove(s);
        });
    }
}
internal sealed class Mt1InjectedFailure(string entity) : SaveChangesInterceptor
{
    public int Hits
    {
        get; private set;
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData e, InterceptionResult<int> result, CancellationToken token = default)
    {
        if (e.Context!.ChangeTracker.Entries().Any(x => x.State == EntityState.Added && x.Metadata.ClrType.Name == entity))
        {
            Hits++;
            throw new Mt1WriteException("MT1 fault before saving " + entity);
        }
        return ValueTask.FromResult(result);
    }
}
internal sealed class Mt1WriteException(string message) : Exception(message);

internal sealed class Mt1HttpContextAccessor : IHttpContextAccessor
{
    public HttpContext? HttpContext
    {
        get; set;
    }
}
