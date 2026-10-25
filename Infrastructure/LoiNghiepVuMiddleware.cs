using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Infrastructure;
public sealed class LoiNghiepVuMiddleware(RequestDelegate next,ILogger<LoiNghiepVuMiddleware> log) {
 public async Task InvokeAsync(HttpContext c) {try {await next(c);} catch(Exception ex) when(!c.Response.HasStarted) {
  var sql=ex as SqlException??ex.InnerException as SqlException;
  var status=ex is LoiNghiepVu b?b.Status:ex is DbUpdateConcurrencyException?409:sql?.Number>=51000?400:sql?.Number is 2601 or 2627?409:500;
  var message=ex is LoiNghiepVu?ex.Message:ex is DbUpdateConcurrencyException?"Dữ liệu đã thay đổi. Hãy tải lại trước khi sửa.":sql?.Number>=51000?sql.Message:status==409?"Thao tác trùng hoặc dữ liệu đã thay đổi. Hãy tải lại.":"Chưa thể xử lý yêu cầu. Vui lòng thử lại sau.";
  log.LogWarning(ex,"Request failed {Path}",c.Request.Path);
  c.Response.StatusCode=status;c.Response.ContentType="text/html; charset=utf-8";
  await c.Response.WriteAsync("<!doctype html><html lang='vi'><meta charset='utf-8'><meta name='viewport' content='width=device-width'><title>EduConnect</title><link rel='stylesheet' href='/css/app.css'><main class='error-page'><h1>Chưa thể hoàn tất</h1><p role='alert'>"+System.Net.WebUtility.HtmlEncode(message)+"</p><a class='btn' href='/'>Về trang chủ</a><button onclick='history.back()'>Quay lại biểu mẫu</button></main></html>");
 } }
}
