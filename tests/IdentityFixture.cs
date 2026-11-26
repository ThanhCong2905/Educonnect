using System.Security.Claims;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

internal static class IdentityFixture
{
    public const string Password = "EduConnect kiểm thử 2026!";
    public static Task SignInAsync(HttpContext context, Guid account)
    {
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, account.ToString())], "Test"));
        return Task.CompletedTask;
    }

    public static async Task SeedAsync(AppDbContext db, params NguoiThamGia[] people)
    {
        if (!db.Database.GetDbConnection().Database.StartsWith("EduConnect_Verification_", StringComparison.Ordinal))
            throw new InvalidOperationException("Fixture requires a verification database.");
        foreach (var person in people)
        {
            if (await db.Users.AnyAsync(user => user.Id == person.Id)) continue;
            var email = person.Id.ToString("N") + "@example.test";
            var account = new TaiKhoan
            {
                Id = person.Id, HoTen = person.HoTen, Email = email,
                NormalizedEmail = email.ToUpperInvariant(), UserName = email,
                NormalizedUserName = email.ToUpperInvariant(), EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString()
            };
            account.PasswordHash = new PasswordHasher<TaiKhoan>().HashPassword(account, Password);
            db.Users.Add(account);
            var role = await db.Roles.SingleAsync(role => role.Name == person.LoaiNguoi);
            db.UserRoles.Add(new() { UserId = person.Id, RoleId = role.Id });
            if (person.LoaiNguoi == "QuanTri")
                foreach (var permission in EduConnect.Mvc.Areas.QuanTri.Services.QuyenQuanTriService.DanhSach)
                    db.UserClaims.Add(new() { UserId = person.Id, ClaimType = "EduConnect.AdminPermission", ClaimValue = permission });
        }
        await db.SaveChangesAsync();
        foreach (var person in people)
        {
            if (person.LoaiNguoi == "HocSinh" && !await db.Set<HocSinh>().AnyAsync(student => student.TaiKhoanId == person.Id))
                db.Add(new HocSinh { TaiKhoanId = person.Id });
            if (person.LoaiNguoi == "PhuHuynh" && !await db.Set<PhuHuynh>().AnyAsync(parent => parent.TaiKhoanId == person.Id))
                db.Add(new PhuHuynh { TaiKhoanId = person.Id });
        }
        await db.SaveChangesAsync();
    }

    public static async Task PrepareLoginAsync(AppDbContext db, params Guid[] users)
    {
        if (!db.Database.GetDbConnection().Database.StartsWith("EduConnect_Verification_", StringComparison.Ordinal))
            throw new InvalidOperationException("Fixture requires a verification database.");
        foreach (var id in users)
        {
            var account = await db.Users.SingleAsync(user => user.Id == id);
            account.PasswordHash = new PasswordHasher<TaiKhoan>().HashPassword(account, Password);
            account.EmailConfirmed = true;
        }
        await db.SaveChangesAsync();
    }
}
