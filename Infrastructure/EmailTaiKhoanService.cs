using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace EduConnect.Mvc.Infrastructure;

public sealed record ThuTaiKhoan(string Email, string TieuDe, string NoiDung, string LienKet, DateTimeOffset TaoLuc);

public sealed class EmailTaiKhoanService(IWebHostEnvironment environment, IConfiguration configuration)
{
    private string Inbox => Path.Combine(environment.ContentRootPath, ".local-mail");

    public async Task GuiDatLaiMatKhauAsync(string email, string link)
    {
        if (environment.IsDevelopment() && configuration.GetValue<bool>("Authentication:DevelopmentMailbox"))
        {
            Directory.CreateDirectory(Inbox);
            var mail = new ThuTaiKhoan(email, "Đặt lại mật khẩu EduConnect", "Liên kết có hiệu lực trong 30 phút và chỉ dùng được một lần.", link, DateTimeOffset.UtcNow);
            var temporary = Path.Combine(Inbox, Guid.NewGuid().ToString("N") + ".tmp");
            await File.WriteAllTextAsync(temporary, JsonSerializer.Serialize(mail));
            File.Move(temporary, Path.ChangeExtension(temporary, ".json"));
            return;
        }

        var host = configuration["Smtp:Host"] ?? throw new InvalidOperationException("Cần cấu hình SMTP để gửi email tài khoản.");
        var from = configuration["Smtp:From"] ?? throw new InvalidOperationException("Cần cấu hình địa chỉ gửi email.");
        using var smtp = new SmtpClient(host, configuration.GetValue("Smtp:Port", 587)) { EnableSsl = true };
        var username = configuration["Smtp:Username"];
        if (!string.IsNullOrEmpty(username)) smtp.Credentials = new NetworkCredential(username, configuration["Smtp:Password"]);
        using var message = new MailMessage(from, email, "Đặt lại mật khẩu EduConnect", "Mở liên kết để đặt mật khẩu mới. Liên kết có hiệu lực trong 30 phút.\n\n" + link);
        await smtp.SendMailAsync(message);
    }

}
