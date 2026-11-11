using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Areas.QuanTri.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var config=new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
var services=new ServiceCollection();
services.AddSingleton<IConfiguration>(config);
services.AddLogging();
services.AddDbContext<AppDbContext>(o=>o.UseSqlServer(config.GetConnectionString("EduConnect")));
services.AddIdentityCore<TaiKhoan>(o=>
{
    o.User.RequireUniqueEmail=true;
    o.User.AllowedUserNameCharacters="";
    o.Password.RequiredLength=15;
    o.Password.RequireDigit=o.Password.RequireUppercase=o.Password.RequireLowercase=o.Password.RequireNonAlphanumeric=false;
}).AddRoles<VaiTro>().AddEntityFrameworkStores<AppDbContext>().AddPasswordValidator<MatKhauValidator>();
await using var provider=services.BuildServiceProvider();
await using var scope=provider.CreateAsyncScope();
var db=scope.ServiceProvider.GetRequiredService<AppDbContext>();
var users=scope.ServiceProvider.GetRequiredService<UserManager<TaiKhoan>>();

Console.Write("Email quản trị mới: ");
var email=Console.ReadLine()?.Trim()??"";
if(email.Length>256||!new EmailAddressAttribute().IsValid(email)) throw new InvalidOperationException("Email không hợp lệ.");
Console.Write("Họ tên: ");
var name=Console.ReadLine()?.Trim()??"";
if(string.IsNullOrWhiteSpace(name)||name.Length>256) throw new InvalidOperationException("Họ tên cần từ 1 đến 256 ký tự.");
Console.Write("Mật khẩu: ");
var password=ReadPassword();
Console.Write("Nhập lại mật khẩu: ");
if(password!=ReadPassword()) throw new InvalidOperationException("Hai mật khẩu chưa giống nhau.");
await using var transaction=await db.Database.BeginTransactionAsync();
await GiaoDichNghiepVu.KhoaQuanTriAsync(db);
if(await users.FindByEmailAsync(email)!=null) throw new InvalidOperationException("Email đã có tài khoản. Hãy dùng email khác cho quản trị mới.");
var user=new TaiKhoan { Id=Guid.NewGuid(),HoTen=name,Email=email,UserName=email };
Ensure(await users.CreateAsync(user,password));
Ensure(await users.AddToRoleAsync(user,"QuanTri"));
var claims=QuyenQuanTriService.DanhSach.Select(x=>new Claim(QuyenQuanTriService.ClaimType,x)).ToList();
claims.Add(new Claim("EduConnect.AdminConfigured","true"));
Ensure(await users.AddClaimsAsync(user,claims));
await transaction.CommitAsync();
Console.WriteLine("Đã tạo tài khoản quản trị. Đăng nhập trên web bằng email vừa nhập.");

static void Ensure(IdentityResult result)
{
    if(!result.Succeeded) throw new InvalidOperationException(string.Join(Environment.NewLine,result.Errors.Select(x=>x.Description)));
}

static string ReadPassword()
{
    var value=new List<char>();
    while(true)
    {
        var key=Console.ReadKey(intercept:true);
        if(key.Key==ConsoleKey.Enter) { Console.WriteLine(); return new string(value.ToArray()); }
        if(key.Key==ConsoleKey.Backspace) { if(value.Count>0) value.RemoveAt(value.Count-1); }
        else if(!char.IsControl(key.KeyChar)) value.Add(key.KeyChar);
    }
}
