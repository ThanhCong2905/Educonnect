
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Areas.HocTap.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using System.Threading.RateLimiting;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(o => { o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); o.Filters.Add<UiResponseFilter>(); o.Filters.Add<NhatKyQuanTriFilter>(); o.ModelMetadataDetailsProviders.Add(new VietnameseValidationMetadataProvider()); o.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value, field) => $"{field} chưa đúng định dạng."); o.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(field => $"{field} phải là một số."); });
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o => { o.Cookie.Name = "EduConnect.Workspace"; o.Cookie.HttpOnly = true; o.Cookie.IsEssential = true; o.IdleTimeout = PhienCapNhat.ThoiHan; });
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<NhatKyAnToanService>();
builder.Services.AddScoped<EduConnect.Mvc.Areas.QuanTri.Services.NhatKyAnToanQuery>();
builder.Services.Configure<MvcOptions>(o => o.Filters.Add<NhatKyAnToanMvcFilter>());
builder.Services.AddSignalR(o => { o.MaximumReceiveMessageSize = 4096; o.EnableDetailedErrors = false; o.AddFilter<NhatKyAnToanHubFilter>(); });
builder.Services.AddSingleton<PhienCapNhat>();
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("EduConnect")));
builder.Services.AddIdentity<TaiKhoan, VaiTro>(o =>
{
    o.User.RequireUniqueEmail = true;
    o.User.AllowedUserNameCharacters = "";
    o.Password.RequiredLength = 15;
    o.Password.RequireDigit = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireLowercase = false;
    o.Password.RequireNonAlphanumeric = false;
    o.Lockout.AllowedForNewUsers = false;
    o.Stores.MaxLengthForKeys = 128;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders()
    .AddPasswordValidator<MatKhauValidator>().AddErrorDescriber<LoiTaiKhoan>();
builder.Services.Configure<PasswordHasherOptions>(o => o.IterationCount = 210000);
builder.Services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromMinutes(30));
builder.Services.ConfigureApplicationCookie(o =>
{
    o.Cookie.Name = "EduConnect.Identity";
    o.Cookie.HttpOnly = true;
    o.Cookie.SameSite = SameSiteMode.Lax;
    o.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
    o.LoginPath = "/TaiKhoan/DangNhap";
    o.AccessDeniedPath = "/TaiKhoan/KhongCoQuyen";
    o.ExpireTimeSpan = TimeSpan.FromHours(8);
    o.SlidingExpiration = false;
    o.EventsType = typeof(KiemTraCookieTaiKhoan);
});
builder.Services.AddAuthorization(o => o.FallbackPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser().RequireRole("HocSinh", "PhuHuynh", "GiaSu", "QuanTri").Build());
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = 429;
    o.AddPolicy("account", http => RateLimitPartition.GetFixedWindowLimiter(
        http.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = builder.Configuration.GetValue("Authentication:AccountRequestLimit", 20),
            Window = TimeSpan.FromMinutes(15),
            QueueLimit = 0
        }));
    o.OnRejected = async (c, token) => await c.HttpContext.Response.WriteAsync("Bạn đã gửi nhiều yêu cầu. Vui lòng chờ rồi thử lại.", token);
});
builder.Services.AddScoped<NguoiDungHienTaiService>();
builder.Services.AddScoped<INguoiDungHienTaiService>(sp => sp.GetRequiredService<NguoiDungHienTaiService>());
builder.Services.AddScoped<PhienDangNhapService>();
builder.Services.AddScoped<KiemTraCookieTaiKhoan>();
builder.Services.AddScoped<TaiKhoanService>();
builder.Services.AddScoped<EmailTaiKhoanService>();
builder.Services.AddScoped<ThongBaoService>();
builder.Services.AddScoped<QuyenLopHocService>();
builder.Services.AddScoped<NhuCauService>();
builder.Services.AddScoped<INhuCauService>(sp => sp.GetRequiredService<NhuCauService>());
builder.Services.AddScoped<MonHocCatalog>();
builder.Services.AddScoped<HoSoGiaSuService>();
builder.Services.AddScoped<IHoSoGiaSuService>(sp => sp.GetRequiredService<HoSoGiaSuService>());
builder.Services.AddScoped<TimKiemService>();
builder.Services.AddScoped<ITimKiemService>(sp => sp.GetRequiredService<TimKiemService>());
builder.Services.AddScoped<KetNoiService>();
builder.Services.AddScoped<IKetNoiService>(sp => sp.GetRequiredService<KetNoiService>());
builder.Services.AddScoped<LopHocService>();
builder.Services.AddScoped<LienKetTaiKhoanService>();
builder.Services.AddScoped<LichHocService>();
builder.Services.AddScoped<BaiTapService>();
builder.Services.AddScoped<TinNhanService>();
builder.Services.AddScoped<TepService>();
builder.Services.AddScoped<BangCapService>();
builder.Services.AddScoped<IBangCapService>(sp => sp.GetRequiredService<BangCapService>());
builder.Services.AddScoped<EduConnect.Mvc.Areas.DanhGia.Services.DanhGiaService>();
builder.Services.AddScoped<EduConnect.Mvc.Areas.DanhGia.Services.DanhGiaDoiTacService>();
builder.Services.AddScoped<EduConnect.Mvc.Areas.DanhGia.Services.TienDoService>();
builder.Services.AddScoped<EduConnect.Mvc.Areas.QuanTri.Services.QuyenQuanTriService>();
builder.Services.AddScoped<EduConnect.Mvc.Areas.QuanTri.Services.QuanTriService>();
builder.Services.AddScoped<EduConnect.Mvc.Areas.QuanTri.Services.BaoCaoViPhamService>();
builder.Services.AddHostedService<BaoTriWorker>();
builder.Services.AddHostedService<XuLySuKienWorker>();
var app = builder.Build();
app.UseMiddleware<LoiNghiepVuMiddleware>();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseMiddleware<NhatKyAnToanMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/cap-nhat"))
    {
        var origin = context.Request.Headers.Origin.ToString();
        if (origin.Length > 0 && (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)
            || !string.Equals(uri.GetLeftPart(UriPartial.Authority), $"{context.Request.Scheme}://{context.Request.Host}", StringComparison.OrdinalIgnoreCase)))
        { context.Response.StatusCode = 403; return; }
    }
    await next();
});
app.MapHub<CapNhatThoiGianThucHub>("/cap-nhat");
app.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
public partial class Program
{
}
