using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EduConnect.Mvc.Infrastructure;

public sealed class NhatKyAnToanMvcFilter : IAsyncActionFilter, IAsyncAlwaysRunResultFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var target = context.ActionArguments.FirstOrDefault(x =>
            x.Value is Guid && (x.Key.Equals("id", StringComparison.OrdinalIgnoreCase) || x.Key.StartsWith("ma", StringComparison.OrdinalIgnoreCase)));
        if (target.Value is Guid id)
            context.HttpContext.Items[NhatKyAnToanService.ObjectKey] = id.ToString();
        var result = await next();
        if (!result.ModelState.IsValid)
            context.HttpContext.Items[NhatKyAnToanService.InvalidInputKey] = true;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is AntiforgeryValidationFailedResult)
            context.HttpContext.Items[NhatKyAnToanService.DeniedKey] = "CSRFKhongHopLe";
        if (context.Result is FileResult && context.RouteData.Values["action"]?.ToString() != "Anh")
            context.HttpContext.Items[NhatKyAnToanService.DownloadKey] = true;
        await next();
    }
}
