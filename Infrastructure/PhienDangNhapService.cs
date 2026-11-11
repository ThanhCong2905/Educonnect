using System.Security.Claims;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Infrastructure;

public sealed class PhienDangNhapService(AppDbContext db, PhienCapNhat realtime)
{
    public const string SessionClaim = "educonnect:session";

    public async Task<bool> HopLeAsync(Guid session, Guid user, bool active, CancellationToken token = default)
    {
        await db.Database.OpenConnectionAsync(token);
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "EXEC dbo.usp_KiemTraPhien @PhienId,@TaiKhoanId,@LaHoatDongChuDong";
        command.Parameters.Add(new SqlParameter("PhienId", session));
        command.Parameters.Add(new SqlParameter("TaiKhoanId", user));
        command.Parameters.Add(new SqlParameter("LaHoatDongChuDong", active));
        var valid = Convert.ToBoolean(await command.ExecuteScalarAsync(token));
        if (!valid) await ThuHoiAsync(session, "Phiên hết hạn hoặc tài khoản không còn hoạt động");
        return valid;
    }

    public async Task ThuHoiAsync(Guid session, string reason)
    {
        var now = DateTimeOffset.UtcNow;
        await db.Set<PhienDangNhap>().Where(x => x.Id == session && x.ThuHoiLuc == null)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.ThuHoiLuc, now).SetProperty(x => x.LyDoThuHoi, reason));
        realtime.ThuHoi(session.ToString("D"));
    }

    public async Task ThuHoiTatCaAsync(Guid user, string reason)
    {
        var sessions = await db.Set<PhienDangNhap>().Where(x => x.TaiKhoanId == user && x.ThuHoiLuc == null).Select(x => x.Id).ToListAsync();
        foreach (var id in sessions) await ThuHoiAsync(id, reason);
    }
}

public sealed class KiemTraCookieTaiKhoan(AppDbContext db, PhienDangNhapService sessions, PhienCapNhat realtime) : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var principal = context.Principal;
        if (!Guid.TryParse(principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            || !Guid.TryParse(principal.FindFirstValue(PhienDangNhapService.SessionClaim), out var sessionId))
        {
            await RejectAsync(context);
            return;
        }

        var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == userId, context.HttpContext.RequestAborted);
        var role = await (from ur in db.UserRoles join r in db.Roles on ur.RoleId equals r.Id where ur.UserId == userId select r.Name).SingleOrDefaultAsync();
        var valid = user?.TrangThai == "HoatDong"
            && user.SecurityStamp == principal.FindFirstValue("AspNet.Identity.SecurityStamp")
            && role != null && principal.IsInRole(role)
            && await sessions.HopLeAsync(sessionId, userId, IsActive(context.Request), context.HttpContext.RequestAborted);
        if (!valid)
        {
            await sessions.ThuHoiAsync(sessionId, "Tài khoản hoặc quyền truy cập đã thay đổi");
            await RejectAsync(context);
            return;
        }
        realtime.CapNhat(sessionId.ToString("D"), userId);
    }

    private static bool IsActive(HttpRequest request)
        => !request.Path.StartsWithSegments("/cap-nhat")
        && !request.Path.Value!.EndsWith("/ChuaDoc", StringComparison.OrdinalIgnoreCase)
        && !request.Path.Value.EndsWith("/DuLieu", StringComparison.OrdinalIgnoreCase)
        && !request.Headers.ContainsKey("X-Edu-Background");

    private static async Task RejectAsync(CookieValidatePrincipalContext context)
    {
        context.HttpContext.RequestServices.GetRequiredService<NhatKyAnToanService>()
            .Ghi("Authentication", "PhienKhongHopLe", "ThatBai", "PhienHetHanHoacBiThuHoi",
                taiKhoan: Guid.TryParse(context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var actor) ? actor : null);
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
    }

    public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
    {
        context.HttpContext.Items[NhatKyAnToanService.DeniedKey] = "ChuaDangNhap";
        if (IsApi(context.Request))
        {
            context.Response.StatusCode = 401;
            return context.Response.WriteAsJsonAsync(new { ok = false, message = "Phiên đã hết hạn. Vui lòng đăng nhập lại." });
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    }

    public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
    {
        context.HttpContext.Items[NhatKyAnToanService.DeniedKey] = "KhongDuQuyen";
        if (IsApi(context.Request))
        {
            context.Response.StatusCode = 403;
            return context.Response.WriteAsJsonAsync(new { ok = false, message = "Bạn không có quyền thực hiện thao tác này." });
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    }

    private static bool IsApi(HttpRequest request)
        => request.Headers.XRequestedWith == "XMLHttpRequest" || request.Path.StartsWithSegments("/cap-nhat");
}
