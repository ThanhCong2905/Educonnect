namespace EduConnect.Mvc.Infrastructure;

public sealed class LoiNghiepVuMiddleware(RequestDelegate next, ILogger<LoiNghiepVuMiddleware> log)
{
    public async Task InvokeAsync(HttpContext c)
    {
        try
        {
            await next(c);
        }
        catch (Exception ex) when (!c.Response.HasStarted)
        {
            var status = Rules.ErrorStatus(ex);
            var message = Rules.ErrorMessage(ex);
            log.LogWarning(ex, "Request failed {Path}", c.Request.Path);
            c.Response.StatusCode = status;
            c.Response.ContentType = "text/html; charset=utf-8";
            await c.Response.WriteAsync("<!doctype html><html lang='vi'><meta charset='utf-8'><meta name='viewport' content='width=device-width'><title>EduConnect</title><link rel='stylesheet' href='/css/app.css'><main class='error-page'><h1>Chưa thể hoàn tất</h1><p role='alert'>" + System.Net.WebUtility.HtmlEncode(message) + "</p><a class='btn' href='/'>Về trang chủ</a><button onclick='history.back()'>Quay lại biểu mẫu</button></main></html>");
        }
    }
}
