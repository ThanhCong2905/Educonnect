
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
builder.Services.AddSession(o => { o.Cookie.Name = "EduConnect.Preview"; o.Cookie.HttpOnly = true; o.Cookie.IsEssential = true; o.IdleTimeout = TimeSpan.FromHours(8); });
builder.Services.AddHttpContextAccessor();
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
var app = builder.Build();
app.UseMiddleware<LoiNghiepVuMiddleware>();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
public partial class Program
{
}
