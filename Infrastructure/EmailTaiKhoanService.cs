using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace EduConnect.Mvc.Infrastructure;

public sealed record ThuTaiKhoan(string Email, string TieuDe, string NoiDung, string LienKet, DateTimeOffset TaoLuc);

public sealed class EmailTaiKhoanService(IWebHostEnvironment environment, IConfiguration configuration,
    ILogger<EmailTaiKhoanService> logger)
{
    private string Inbox => Path.Combine(environment.ContentRootPath, ".local-mail");
    private bool HopThuThu => environment.IsDevelopment() && configuration.GetValue<bool>("Authentication:DevelopmentMailbox");
    public bool CoSmtp => !string.IsNullOrWhiteSpace(configuration["Smtp:Host"]) &&
        MailAddress.TryCreate(configuration["Smtp:From"], out _) &&
        (string.IsNullOrWhiteSpace(configuration["Smtp:Username"]) || !string.IsNullOrWhiteSpace(configuration["Smtp:Password"]));
    public bool SanSang => HopThuThu || CoSmtp;
    public bool ChapNhanDiaChi(string address) =>
        MailAddress.TryCreate(address, out var parsed) &&
        (HopThuThu || !parsed.Host.EndsWith(".test", StringComparison.OrdinalIgnoreCase));

    public Task<bool> GuiDatLaiMatKhauAsync(string email, string link) =>
        GuiAsync(email, link, "Đặt lại mật khẩu EduConnect", "Mở liên kết để đặt mật khẩu mới. Liên kết có hiệu lực trong 30 phút.");

    public Task<bool> GuiXacMinhAsync(string email, string link) =>
        GuiAsync(email, link, "Xác minh email EduConnect", "Mở liên kết để xác minh email đăng ký EduConnect. Liên kết có hiệu lực trong 30 phút.");

    private async Task<bool> GuiAsync(string email, string link, string subject, string body)
    {
        if (!SanSang || !ChapNhanDiaChi(email)) return false;
        try
        {
            if (HopThuThu)
            {
                Directory.CreateDirectory(Inbox);
                var mail = new ThuTaiKhoan(email, subject, body, link, DateTimeOffset.UtcNow);
                var temporary = Path.Combine(Inbox, Guid.NewGuid().ToString("N") + ".tmp");
                await File.WriteAllTextAsync(temporary, JsonSerializer.Serialize(mail));
                File.Move(temporary, Path.ChangeExtension(temporary, ".json"));
                return true;
            }

            using var smtp = new SmtpClient(configuration["Smtp:Host"]!, configuration.GetValue("Smtp:Port", 587)) { EnableSsl = true };
            var username = configuration["Smtp:Username"];
            if (!string.IsNullOrWhiteSpace(username)) smtp.Credentials = new NetworkCredential(username, configuration["Smtp:Password"]);
            using var message = new MailMessage(configuration["Smtp:From"]!, email, subject, body + "\n\n" + link);
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            await smtp.SendMailAsync(message, timeout.Token);
            return true;
        }
        catch (SmtpException ex)
        {
            logger.LogWarning("Không gửi được email tài khoản qua SMTP. Mã phản hồi {Status}.", ex.StatusCode);
        }
        catch (Exception ex) when (ex is IOException or OperationCanceledException)
        {
            logger.LogWarning("Không gửi được email tài khoản. Lỗi {ErrorType}.", ex.GetType().Name);
        }
        return false;
    }
}
