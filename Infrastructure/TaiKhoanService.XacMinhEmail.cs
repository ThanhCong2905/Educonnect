using System.Globalization;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.ViewModels;

namespace EduConnect.Mvc.Infrastructure;

public sealed partial class TaiKhoanService
{
    private const string LanThuGuiXacMinh = "EduConnect.EmailVerificationLastAttempt";
    public const int GiayChoGuiLaiEmail = 60;

    private Task<bool> ChoXacMinhAsync(TaiKhoan user) => db.UserClaims.AnyAsync(claim =>
        claim.UserId == user.Id && claim.ClaimType == EmailVerificationRequired && claim.ClaimValue == "true");

    private async Task GuiXacMinhNguoiDungAsync(TaiKhoan user)
    {
        var token = await users.GenerateEmailConfirmationTokenAsync(user);
        var link = TaoLienKet("XacMinhEmail", "id", user.Id.ToString("D"), token);
        await email.GuiXacMinhAsync(user.Email!, link);
    }

    public async Task GuiLaiXacMinhEmailAsync(string address)
    {
        if (!CoTheGuiEmailTaiKhoan) return;
        var normalized = users.NormalizeEmail(address.Trim());
        var id = await db.Users.AsNoTracking().Where(user => user.NormalizedEmail == normalized)
            .Select(user => (Guid?)user.Id).SingleOrDefaultAsync();
        if (id == null) return;

        TaiKhoan? account;
        await using (var tx = await db.Database.BeginTransactionAsync())
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT Id FROM dbo.AspNetUsers WITH (UPDLOCK,HOLDLOCK) WHERE Id={id.Value}");
            account = await db.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Id == id.Value);
            if (account is not { TrangThai: "HoatDong", EmailConfirmed: false } || !await ChoXacMinhAsync(account))
                return;

            var claim = await db.UserClaims.SingleOrDefaultAsync(c => c.UserId == id.Value && c.ClaimType == LanThuGuiXacMinh);
            var now = DateTimeOffset.UtcNow;
            if (SoGiayCho(claim?.ClaimValue, now) > 0) return;
            if (claim == null)
                db.UserClaims.Add(new IdentityUserClaim<Guid> { UserId = id.Value, ClaimType = LanThuGuiXacMinh, ClaimValue = now.ToString("O") });
            else
                claim.ClaimValue = now.ToString("O");
            await db.SaveChangesAsync();
            await tx.CommitAsync();
        }
        await GuiXacMinhNguoiDungAsync(account);
    }

    public async Task<TrangThaiXacMinhEmailViewModel> LayTrangThaiXacMinhEmailAsync(string address)
    {
        var normalized = users.NormalizeEmail(address);
        var account = await db.Users.AsNoTracking().Where(user => user.NormalizedEmail == normalized && user.TrangThai == "HoatDong")
            .Select(user => new
            {
                user.EmailConfirmed,
                LastAttempt = db.UserClaims.Where(c => c.UserId == user.Id && c.ClaimType == LanThuGuiXacMinh)
                    .Select(c => c.ClaimValue).FirstOrDefault()
            }).SingleOrDefaultAsync();
        return new TrangThaiXacMinhEmailViewModel(address, account?.EmailConfirmed == true,
            SoGiayCho(account?.LastAttempt, DateTimeOffset.UtcNow), true);
    }

    private static int SoGiayCho(string? value, DateTimeOffset now) =>
        DateTimeOffset.TryParseExact(value, "O", CultureInfo.InvariantCulture, DateTimeStyles.None, out var lastAttempt)
            ? Math.Clamp((int)Math.Ceiling((lastAttempt.AddSeconds(GiayChoGuiLaiEmail) - now).TotalSeconds), 0, GiayChoGuiLaiEmail)
            : 0;

    public async Task<bool> XacMinhEmailAsync(Guid id, string encodedToken)
    {
        string token;
        try { token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken)); }
        catch (FormatException) { return false; }

        var user = await users.FindByIdAsync(id.ToString("D"));
        if (user?.TrangThai != "HoatDong" || !await ChoXacMinhAsync(user))
            return false;
        if (user.EmailConfirmed)
            return await users.VerifyUserTokenAsync(user, users.Options.Tokens.EmailConfirmationTokenProvider,
                UserManager<TaiKhoan>.ConfirmEmailTokenPurpose, token);
        var result = await users.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
            auditLog.Ghi("Authentication", "TaiKhoan.XacMinhEmail", "ThanhCong", dich: user.Id);
        return result.Succeeded;
    }

    private string TaoLienKet(string action, string key, string value, string token)
    {
        var origin = configuration["Authentication:PublicOrigin"]!;
        return QueryHelpers.AddQueryString(origin.TrimEnd('/') + "/TaiKhoan/" + action, new Dictionary<string, string?>
        {
            [key] = value,
            ["ma"] = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token))
        });
    }
}
