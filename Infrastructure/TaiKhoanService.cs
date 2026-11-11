using System.Data;
using System.Security.Claims;
using System.Text;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EduConnect.Mvc.Infrastructure;

public sealed partial class TaiKhoanService(AppDbContext db, UserManager<TaiKhoan> users, SignInManager<TaiKhoan> signIn,
    PhienDangNhapService sessions, PhienCapNhat realtime, EmailTaiKhoanService email, IConfiguration configuration)
{
    private static readonly string DummyHash = new PasswordHasher<TaiKhoan>().HashPassword(new TaiKhoan(), Guid.NewGuid().ToString("N"));
    private static readonly string[] PublicRoles = ["HocSinh", "PhuHuynh", "GiaSu"];

    public async Task<IdentityResult> DangKyAsync(DangKyInput input)
    {
        if (!PublicRoles.Contains(input.VaiTro))
            return IdentityResult.Failed(new IdentityError { Description = "Vai trò đăng ký không hợp lệ." });

        await using var tx = await db.Database.BeginTransactionAsync();
        var address = input.Email.Trim();
        var user = new TaiKhoan { Id = Guid.NewGuid(), HoTen = input.HoTen.Trim(), Email = address, UserName = address };
        try
        {
            var created = await users.CreateAsync(user, input.MatKhau);
            if (!created.Succeeded) return created;
            var assigned = await users.AddToRoleAsync(user, input.VaiTro);
            if (!assigned.Succeeded) return assigned;
            switch (input.VaiTro)
            {
                case "HocSinh": db.Add(new HocSinh { TaiKhoanId = user.Id }); break;
                case "PhuHuynh": db.Add(new PhuHuynh { TaiKhoanId = user.Id }); break;
                case "GiaSu": db.Add(new GiaSu { TaiKhoanId = user.Id }); break;
            }
            await db.SaveChangesAsync();
            await tx.CommitAsync();
            return IdentityResult.Success;
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            return IdentityResult.Failed(new IdentityError { Description = "Email đã được đăng ký." });
        }
    }

    public async Task<string?> DangNhapAsync(DangNhapInput input, HttpContext http)
    {
        var normalized = users.NormalizeEmail(input.Email.Trim());
        var ip = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        if (await DangChoAsync(normalized, ip))
            return "Bạn đã thử đăng nhập nhiều lần. Vui lòng chờ rồi thử lại.";

        var user = await users.FindByEmailAsync(input.Email.Trim());
        var correct = user != null
            ? await users.CheckPasswordAsync(user, input.MatKhau)
            : users.PasswordHasher.VerifyHashedPassword(new TaiKhoan(), DummyHash, input.MatKhau) != PasswordVerificationResult.Failed;
        if (!correct || user?.TrangThai != "HoatDong")
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.usp_GhiNhanDangNhapSai @TaiKhoanChuanHoa={normalized},@DiaChiIp={ip}");
            return "Email hoặc mật khẩu chưa đúng, hoặc tài khoản hiện không thể đăng nhập.";
        }

        var roles = await users.GetRolesAsync(user);
        if (roles.Count != 1 || !new[] { "HocSinh", "PhuHuynh", "GiaSu", "QuanTri" }.Contains(roles[0]))
            return "Tài khoản chưa được thiết lập vai trò hợp lệ.";

        Guid sessionId;
        try
        {
            await db.Database.OpenConnectionAsync();
            await using var command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = "EXEC dbo.usp_DangNhapThanhCong @TaiKhoanId,@TaiKhoanChuanHoa,@DiaChiIp";
            command.Parameters.Add(new SqlParameter("TaiKhoanId", user.Id));
            command.Parameters.Add(new SqlParameter("TaiKhoanChuanHoa", normalized));
            command.Parameters.Add(new SqlParameter("DiaChiIp", ip));
            sessionId = (Guid)(await command.ExecuteScalarAsync())!;
        }
        catch (SqlException ex) when (ex.Number is 51040 or 51041)
        {
            return "Chưa thể đăng nhập. Vui lòng chờ rồi thử lại.";
        }

        if (Guid.TryParse(http.User.FindFirstValue(PhienDangNhapService.SessionClaim), out var oldSession))
            await sessions.ThuHoiAsync(oldSession, "Đăng nhập phiên mới");
        try
        {
            await signIn.SignInWithClaimsAsync(user, new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            }, [new Claim(PhienDangNhapService.SessionClaim, sessionId.ToString("D"))]);
            realtime.CapNhat(sessionId.ToString("D"), user.Id);
        }
        catch
        {
            await sessions.ThuHoiAsync(sessionId, "Không thiết lập được cookie");
            throw;
        }
        return null;
    }

    private Task<bool> DangChoAsync(string account, string ip)
    {
        var now = DateTimeOffset.UtcNow;
        return db.Set<TrangThaiThuDangNhap>().AsNoTracking().AnyAsync(x =>
            ((x.LoaiKhoa == "TaiKhoan" && x.GiaTriKhoa == account) || (x.LoaiKhoa == "DiaChiIp" && x.GiaTriKhoa == ip))
            && x.LanSaiCuoiLuc > now.AddMinutes(-15) && x.ChoDenLuc > now);
    }

    public async Task QuenMatKhauAsync(string address)
    {
        var user = await users.FindByEmailAsync(address.Trim());
        if (user?.TrangThai != "HoatDong") return;
        var token = await users.GeneratePasswordResetTokenAsync(user);
        var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var origin = configuration["Authentication:PublicOrigin"] ?? throw new InvalidOperationException("Cần cấu hình địa chỉ công khai của ứng dụng.");
        var link = QueryHelpers.AddQueryString(origin.TrimEnd('/') + "/TaiKhoan/DatLaiMatKhau", new Dictionary<string, string?>
        {
            ["email"] = user.Email,
            ["ma"] = encoded
        });
        await email.GuiDatLaiMatKhauAsync(user.Email!, link);
    }

    public async Task<IdentityResult> DatLaiMatKhauAsync(DatLaiMatKhauInput input)
    {
        var user = await users.FindByEmailAsync(input.Email.Trim());
        if (user?.TrangThai != "HoatDong") return InvalidLink();
        string token;
        try { token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(input.Ma)); }
        catch (FormatException) { return InvalidLink(); }

        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var result = await users.ResetPasswordAsync(user, token, input.MatKhau);
        if (!result.Succeeded) return result;
        await sessions.ThuHoiTatCaAsync(user.Id, "Đặt lại mật khẩu");
        await tx.CommitAsync();
        return result;
    }

    public async Task DangXuatAsync(HttpContext http)
    {
        if (Guid.TryParse(http.User.FindFirstValue(PhienDangNhapService.SessionClaim), out var session))
            await sessions.ThuHoiAsync(session, "Đăng xuất");
        await signIn.SignOutAsync();
        http.Session.Clear();
    }

    private static IdentityResult InvalidLink()
        => IdentityResult.Failed(new IdentityError { Description = "Liên kết không hợp lệ hoặc đã hết hạn. Hãy yêu cầu liên kết mới." });
}
