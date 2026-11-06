using System.Text;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Identity;

namespace EduConnect.Mvc.Infrastructure;

public sealed class MatKhauValidator(IConfiguration configuration) : IPasswordValidator<TaiKhoan>
{
    private static readonly HashSet<string> Common = new(StringComparer.OrdinalIgnoreCase)
    {
        "123456789012345", "1234567890123456", "passwordpassword", "password123456789",
        "qwertyuiopasdfgh", "qwertyuiopasdfghjkl", "iloveyouiloveyou", "letmeinletmeinletmein",
        "thisisapassword", "correct horse battery staple"
    };

    public Task<IdentityResult> ValidateAsync(UserManager<TaiKhoan> manager, TaiKhoan user, string? password)
    {
        var length = password?.EnumerateRunes().Count() ?? 0;
        if (length is < 15 or > 64)
            return Task.FromResult(Failure("Mật khẩu cần từ 15 đến 64 ký tự. Bạn có thể dùng một câu dễ nhớ."));

        var blocked = Common.Contains(password!);
        var blocklist = configuration["Authentication:PasswordBlocklistFile"];
        if (!blocked && !string.IsNullOrWhiteSpace(blocklist))
            blocked = File.ReadLines(blocklist).Any(line => string.Equals(line, password, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(blocked
            ? Failure("Mật khẩu này quá phổ biến hoặc có trong danh sách đã lộ. Hãy chọn mật khẩu khác.")
            : IdentityResult.Success);
    }

    private static IdentityResult Failure(string message)
        => IdentityResult.Failed(new IdentityError { Code = "PasswordPolicy", Description = message });
}

public sealed class LoiTaiKhoan : IdentityErrorDescriber
{
    public override IdentityError DuplicateEmail(string email) => new() { Code = nameof(DuplicateEmail), Description = "Email đã được đăng ký." };
    public override IdentityError DuplicateUserName(string userName) => new() { Code = nameof(DuplicateUserName), Description = "Email đã được đăng ký." };
    public override IdentityError InvalidEmail(string? email) => new() { Code = nameof(InvalidEmail), Description = "Email chưa đúng định dạng." };
    public override IdentityError InvalidToken() => new() { Code = nameof(InvalidToken), Description = "Liên kết đã hết hạn hoặc đã được sử dụng. Hãy yêu cầu liên kết mới." };
    public override IdentityError ConcurrencyFailure() => new() { Code = nameof(ConcurrencyFailure), Description = "Tài khoản vừa thay đổi. Vui lòng thử lại." };
}
