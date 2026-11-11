using System.Security.Claims;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EduConnect.Mvc.Infrastructure;

public sealed class NhatKyQuanTriFilter(IServiceScopeFactory scopes, ILogger<NhatKyQuanTriFilter> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executed = await next();
        var request = context.HttpContext.Request;
        if (!HttpMethods.IsPost(request.Method) || !context.HttpContext.User.IsInRole("QuanTri"))
            return;
        if (executed.Exception == null && context.ModelState.IsValid)
            return;
        if (!Guid.TryParse(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var actor))
            return;
        try
        {
            await using var scope = scopes.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.ActionDescriptor.RouteValues.TryGetValue("action", out var action);
            context.ActionDescriptor.RouteValues.TryGetValue("controller", out var controller);
            context.ActionArguments.TryGetValue("id", out var target);
            db.Add(new NhatKyQuanTri
            {
                NguoiThucHienId = actor,
                ThoiDiem = DateTimeOffset.UtcNow,
                HanhDong = action ?? "ThaoTac",
                LoaiDoiTuong = controller ?? "QuanTri",
                DoiTuongId = target?.ToString() ?? "",
                KetQua = "KhongThanhCong",
                MaLienKetYeuCau = context.HttpContext.TraceIdentifier
            });
            await db.SaveChangesAsync();
        }
        catch (Exception ex) { logger.LogWarning(ex, "Chưa ghi được nhật ký thao tác quản trị thất bại."); }
    }
}
