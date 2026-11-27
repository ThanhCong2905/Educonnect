namespace EduConnect.Mvc.Infrastructure;

public sealed class DieuHuongSauDangNhap(RequestDelegate next)
{
    private const string DichKey = "EduConnect.LoginDestination";
    private const string HetHanKey = "EduConnect.LoginDestinationExpires";

    public static void GhiNhan(HttpContext context, string destination)
    {
        context.Session.SetString(DichKey, destination);
        context.Session.SetString(HetHanKey, DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds().ToString());
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var destination = context.Session.GetString(DichKey);
        var valid = long.TryParse(context.Session.GetString(HetHanKey), out var expires)
            && expires > DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var returning = valid && context.User.Identity?.IsAuthenticated == true
            && HttpMethods.IsGet(context.Request.Method)
            && destination == context.Request.PathBase + context.Request.Path + context.Request.QueryString;
        if (!valid || returning)
        {
            context.Session.Remove(DichKey);
            context.Session.Remove(HetHanKey);
        }
        if (!returning)
        {
            await next(context);
            return;
        }

        try { await next(context); }
        catch (LoiNghiepVu error) when (error.Status is 403 or 404 && !context.Response.HasStarted)
        {
            context.Response.Redirect(context.Request.PathBase.Add(new PathString("/")).Value!);
            return;
        }
        var location = context.Response.Headers.Location.ToString();
        var destinationPath = Uri.TryCreate(location, UriKind.Absolute, out var deniedUri)
            ? deniedUri.AbsolutePath : location.Split('?')[0];
        var denied = destinationPath == context.Request.PathBase.Add(new PathString("/TaiKhoan/KhongCoQuyen")).Value;
        if (!context.Response.HasStarted && (context.Response.StatusCode is 403 or 404 || denied))
            context.Response.Redirect(context.Request.PathBase.Add(new PathString("/")).Value!);
    }
}
