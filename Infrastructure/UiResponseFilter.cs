using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
namespace EduConnect.Mvc.Infrastructure;

public sealed class UiResponseFilter(ITempDataDictionaryFactory tempDataFactory) : IAsyncResultFilter, IExceptionFilter
{
    public static bool IsAjax(HttpRequest request) => request.Headers["X-Requested-With"] == "XMLHttpRequest";
    public void OnException(ExceptionContext context)
    {
        if (!IsAjax(context.HttpContext.Request))
            return;
        var ex = context.Exception;
        var sql = ex as SqlException ?? ex.InnerException as SqlException;
        if (ex is not LoiNghiepVu && ex is not DbUpdateConcurrencyException && sql is not { Number: >= 51000 })
            return;
        var status = ex is LoiNghiepVu business ? business.Status : ex is DbUpdateConcurrencyException ? 409 : 400;
        var message = ex is LoiNghiepVu ? ex.Message : ex is DbUpdateConcurrencyException ? "Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu." : sql?.Number >= 51000 ? sql.Message : "Chưa thể lưu thay đổi.";
        context.Result = new JsonResult(new { ok = false, message, errors = new Dictionary<string, string[]> { { "", [message] } } }) { StatusCode = status };
        context.ExceptionHandled = true;
    }
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        if (IsAjax(request))
        {
            var controller = context.Controller as Controller;
            if (!context.ModelState.IsValid)
            {
                context.Result = new JsonResult(new { ok = false, message = "Kiểm tra lại thông tin đã nhập.", errors = context.ModelState.Where(x => x.Value?.Errors.Count > 0).ToDictionary(x => x.Key, x => x.Value!.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? "Giá trị chưa đúng định dạng." : e.ErrorMessage).ToArray()) }) { StatusCode = 422 };
            }
            else if (context.Result is RedirectToActionResult or RedirectResult or RedirectToRouteResult)
            {
                var temp = tempDataFactory.GetTempData(context.HttpContext);
                var message = temp["Success"] as string;
                temp.Remove("Success");
                string? url = context.Result switch
                {
                    RedirectResult redirect => redirect.Url,
                    RedirectToActionResult redirect => controller?.Url.Action(redirect.ActionName, redirect.ControllerName, redirect.RouteValues),
                    RedirectToRouteResult redirect => controller?.Url.RouteUrl(redirect.RouteName, redirect.RouteValues),
                    _ => null
                };
                context.Result = new JsonResult(new
                {
                    ok = true,
                    message,
                    redirectUrl = url
                });
            }
            else if ((request.Headers["X-Edu-Modal"] == "true" || request.Headers["X-Edu-Partial"] == "true") && context.Result is ViewResult view)
            {
                context.Result = new PartialViewResult { ViewName = view.ViewName, ViewData = view.ViewData, TempData = view.TempData, StatusCode = view.StatusCode, ContentType = view.ContentType };
            }
        }
        await next();
    }
}
