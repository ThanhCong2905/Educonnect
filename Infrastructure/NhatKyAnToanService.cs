using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;

namespace EduConnect.Mvc.Infrastructure;

public sealed class NhatKyAnToanService(IHttpContextAccessor accessor)
{
    private static readonly object EventsKey = new();
    public static readonly object InvalidInputKey = new();
    public static readonly object DeniedKey = new();
    public static readonly object DownloadKey = new();
    public static readonly object ObjectKey = new();

    public void Ghi(string nhom, string suKien, string ketQua, string lyDo = "", Guid? taiKhoan = null,
        Guid? dich = null, string? dinhDanh = null)
    {
        if (accessor.HttpContext is not { } http) return;
        var events = Events(http);
        if (events.Any(x => x.Nhom == nhom && x.SuKien == suKien)) return;
        var actor = Guid.TryParse(http.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : (Guid?)null;
        events.Add(new NhatKyAnToan
        {
            Nhom = nhom, SuKien = Clean(suKien, 160), KetQua = ketQua,
            TaiKhoanId = taiKhoan ?? actor, TaiKhoanDichId = dich,
            DinhDanhBam = dinhDanh == null ? null : Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(dinhDanh.Trim().ToUpperInvariant()))),
            DiaChiIp = Clean(http.Connection.RemoteIpAddress?.ToString() ?? "", 45),
            DuongDan = Clean(http.Request.Path.Value ?? "", 512),
            PhuongThuc = Clean(http.Request.Method, 16),
            MaYeuCau = Clean(http.TraceIdentifier, 128), LyDo = Clean(lyDo, 100),
            DoiTuongId = http.Items[ObjectKey] as string
        });
    }

    internal static List<NhatKyAnToan> Events(HttpContext http)
    {
        if (http.Items[EventsKey] is not List<NhatKyAnToan> events)
            http.Items[EventsKey] = events = [];
        return events;
    }

    private static string Clean(string value, int limit)
        => new(value.Where(c => !char.IsControl(c)).Take(limit).ToArray());
}

public sealed class NhatKyAnToanMiddleware(RequestDelegate next, IServiceScopeFactory scopes,
    ILogger<NhatKyAnToanMiddleware> logger)
{
    private static readonly HashSet<string> AuthenticationActions = ["DangNhap", "DangKy", "DangXuat", "QuenMatKhau", "DatLaiMatKhau", "DoiMatKhau"];

    public async Task InvokeAsync(HttpContext http, NhatKyAnToanService audit)
    {
        Exception? failure = null;
        try { await next(http); }
        catch (Exception ex) { failure = ex; throw; }
        finally
        {
            var status = failure == null ? http.Response.StatusCode : Rules.ErrorStatus(failure);
            var controller = http.Request.RouteValues["controller"]?.ToString();
            var action = http.Request.RouteValues["action"]?.ToString() ?? "";
            var denied = http.Items.ContainsKey(NhatKyAnToanService.DeniedKey) || status is 401 or 403;
            var invalid = http.Items.ContainsKey(NhatKyAnToanService.InvalidInputKey);
            var result = denied ? "TuChoi" : invalid || status >= 400 ? "ThatBai" : "ThanhCong";
            var reason = denied ? http.Items[NhatKyAnToanService.DeniedKey] as string ?? "KhongDuQuyen"
                : invalid ? "DuLieuKhongHopLe" : status >= 400 ? $"HTTP{status}" : "";
            if (denied)
                audit.Ghi("Authorization", "KiemTraQuyen", "TuChoi", reason);
            if (controller != null && HttpMethods.IsPost(http.Request.Method))
            {
                var authentication = controller == "TaiKhoan" && AuthenticationActions.Contains(action);
                audit.Ghi(authentication ? "Authentication" : "Accounting", $"{controller}.{action}", result, reason);
                if (result == "ThanhCong" && !authentication && http.User.Identity?.IsAuthenticated == true)
                    audit.Ghi("Authorization", "KiemTraQuyen", "ThanhCong");
            }
            if (controller == "QuanTri" && action is "NhatKyAnToan" or "XemNhatKy")
                audit.Ghi("Accounting", $"QuanTri.{action}", result, reason);
            if (http.Items.ContainsKey(NhatKyAnToanService.DownloadKey))
                audit.Ghi("Accounting", $"{controller}.{action}", result, reason);
            var events = NhatKyAnToanService.Events(http);
            if (events.Count > 0)
            {
                try
                {
                    await using var scope = scopes.CreateAsyncScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.AddRange(events);
                    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    await db.SaveChangesAsync(timeout.Token);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Không ghi được nhật ký an toàn. Mã yêu cầu {RequestId}; sự kiện {Events}",
                        http.TraceIdentifier, string.Join(",", events.Select(x => $"{x.Nhom}/{x.SuKien}/{x.KetQua}")));
                }
            }
        }
    }
}
