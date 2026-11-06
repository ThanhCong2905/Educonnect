
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Areas.HocTap.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(o => { o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); o.Filters.Add<UiResponseFilter>(); o.ModelMetadataDetailsProviders.Add(new VietnameseValidationMetadataProvider()); o.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value, field) => $"{field} chưa đúng định dạng."); o.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(field => $"{field} phải là một số."); });
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o => { o.Cookie.Name = "EduConnect.Preview"; o.Cookie.HttpOnly = true; o.Cookie.IsEssential = true; o.IdleTimeout = PhienCapNhat.ThoiHan; });
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR(o => { o.MaximumReceiveMessageSize = 4096; o.EnableDetailedErrors = false; });
builder.Services.AddSingleton<PhienCapNhat>();
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("EduConnect")));
builder.Services.AddScoped<NguoiDungMauService>();
builder.Services.AddScoped<ThongBaoService>();
builder.Services.AddScoped<QuyenLopHocService>();
builder.Services.AddScoped<NhuCauService>();
builder.Services.AddScoped<MonHocCatalog>();
builder.Services.AddScoped<HoSoGiaSuService>();
builder.Services.AddScoped<TimKiemService>();
builder.Services.AddScoped<KetNoiService>();
builder.Services.AddScoped<LopHocService>();
builder.Services.AddScoped<LichHocService>();
builder.Services.AddScoped<BaiTapService>();
builder.Services.AddScoped<TinNhanService>();
builder.Services.AddScoped<TepService>();
builder.Services.AddScoped<BangCapService>();
builder.Services.AddHostedService<BaoTriWorker>();
builder.Services.AddHostedService<XuLySuKienWorker>();
var app = builder.Build();
app.UseMiddleware<LoiNghiepVuMiddleware>();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/cap-nhat"))
    {
        // Session cookies are the current preview identity. Reject cross-origin hub requests too.
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
