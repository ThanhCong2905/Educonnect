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
            if (TrangLoi.LaTrangWeb(c.Request))
                await TrangLoi.HienThiAsync(c, message);
            else if (UiResponseFilter.IsAjax(c.Request))
                await c.Response.WriteAsJsonAsync(new { ok = false, message });
        }
    }
}
