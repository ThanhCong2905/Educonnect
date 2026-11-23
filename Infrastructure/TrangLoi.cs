using System.Net;

namespace EduConnect.Mvc.Infrastructure;

internal static class TrangLoi
{
    public static bool LaTrangWeb(HttpRequest request) => !UiResponseFilter.IsAjax(request)
        && !request.Path.StartsWithSegments("/cap-nhat")
        && request.Headers.Accept.ToString().Contains("text/html", StringComparison.OrdinalIgnoreCase);

    public static Task HienThiAsync(HttpContext context, string? detail = null)
    {
        var (title, message) = context.Response.StatusCode switch
        {
            400 => ("Yêu cầu không hợp lệ", "Hãy mở lại trang rồi thực hiện thao tác một lần nữa."),
            401 => ("Bạn cần đăng nhập", "Hãy đăng nhập để tiếp tục sử dụng EduConnect."),
            403 => ("Bạn chưa có quyền truy cập", "Tài khoản này chưa được cấp quyền vào trang."),
            404 => ("Không tìm thấy trang", "Kiểm tra lại địa chỉ hoặc chọn mục khác từ trang chủ."),
            405 => ("Không thể mở bằng cách này", "Hãy vào trang chủ và mở lại chức năng từ menu."),
            409 => ("Thông tin đã thay đổi", "Mở lại trang để xem dữ liệu mới nhất trước khi thao tác."),
            413 => ("Dữ liệu gửi lên quá lớn", "Kiểm tra giới hạn dung lượng trên biểu mẫu rồi thử lại."),
            429 => ("Bạn đã gửi quá nhiều yêu cầu", "Vui lòng chờ một lúc rồi thử lại."),
            >= 500 => ("Hệ thống tạm thời gián đoạn", "Vui lòng thử lại sau."),
            _ => ("Chưa thể thực hiện yêu cầu", "Hãy mở lại chức năng từ trang chủ.")
        };
        var request = context.Request;
        var classPage = context.Response.StatusCode == 403 && request.Path.StartsWithSegments("/HocTap/LopHoc")
            && context.User.Identity?.IsAuthenticated == true && !context.User.IsInRole("QuanTri");
        var tutorSearch = context.Response.StatusCode == 403 && request.Path.Equals("/TaiKhoan/KhongCoQuyen", StringComparison.OrdinalIgnoreCase)
            && (context.User.IsInRole("HocSinh") || context.User.IsInRole("PhuHuynh"))
            && request.Query["ReturnUrl"].ToString().StartsWith("/TimKiem/TimKiem/NhuCau", StringComparison.OrdinalIgnoreCase);
        var login = context.Response.StatusCode == 401;
        string Url(string path) => WebUtility.HtmlEncode(request.PathBase.Add(new PathString(path)).ToUriComponent());
        var href = Url(classPage ? "/HocTap/LopHoc/Index" : tutorSearch ? "/TimKiem/TimKiem/GiaSu" : login ? "/TaiKhoan/DangNhap" : "/");
        var action = classPage ? "Về danh sách lớp" : tutorSearch ? "Tìm gia sư" : login ? "Đăng nhập" : "Về trang chủ";
        var home = Url("/");
        var secondary = classPage || tutorSearch || login ? $"<a class=\"error-link\" href=\"{home}\">Trang chủ</a>" : "";
        if (tutorSearch) message = "Mục tìm nhu cầu học dành cho Gia sư. Bạn có thể tiếp tục ở mục Tìm gia sư.";
        var safeMessage = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(detail) ? message : detail);
        context.Response.ContentType = "text/html; charset=utf-8";
        context.Response.Headers.CacheControl = "no-store";
        return context.Response.WriteAsync($"""
            <!doctype html>
            <html lang="vi">
            <head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
            <title>{title} | EduConnect</title><link rel="stylesheet" href="{Url("/css/fonts.css")}"><link rel="stylesheet" href="{Url("/css/error.css")}"></head>
            <body><main class="error-panel"><a class="error-brand" href="{home}">EduConnect</a>
            <h1>{title}</h1><p>{safeMessage}</p><div class="error-actions"><a class="error-action" href="{href}">{action}</a>{secondary}</div></main></body>
            </html>
            """);
    }
}
