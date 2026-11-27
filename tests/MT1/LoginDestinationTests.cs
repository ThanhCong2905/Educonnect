using System.Security.Claims;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
using Xunit;

public sealed class LoginDestinationTests
{
    private static DefaultHttpContext Context()
    {
        var context = new DefaultHttpContext
        {
            Session = new MemorySession(),
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())], "Test"))
        };
        context.Request.Method = "GET";
        context.Request.Path = "/KetNoi/KetNoi/ChiTiet/missing";
        DieuHuongSauDangNhap.GhiNhan(context, context.Request.Path);
        return context;
    }

    [Theory]
    [InlineData(403)]
    [InlineData(404)]
    public async Task Unavailable_login_destination_returns_home_once(int status)
    {
        var context = Context();
        var middleware = new DieuHuongSauDangNhap(c => { c.Response.StatusCode = status; return Task.CompletedTask; });
        await middleware.InvokeAsync(context);
        Assert.Equal(302, context.Response.StatusCode);
        Assert.Equal("/", context.Response.Headers.Location);
        context.Response.Clear();
        await middleware.InvokeAsync(context);
        Assert.Equal(status, context.Response.StatusCode);
    }

    [Fact]
    public async Task Missing_private_object_after_login_returns_home()
    {
        var context = Context();
        await new DieuHuongSauDangNhap(_ => throw new LoiNghiepVu("Không tìm thấy yêu cầu.", 404)).InvokeAsync(context);
        Assert.Equal("/", context.Response.Headers.Location);
    }

    [Theory]
    [InlineData("/TaiKhoan/KhongCoQuyen?ReturnUrl=x")]
    [InlineData("https://educonnect.example/TaiKhoan/KhongCoQuyen?ReturnUrl=x")]
    public async Task Cookie_access_denied_redirect_after_login_returns_home(string location)
    {
        var context = Context();
        await new DieuHuongSauDangNhap(c => { c.Response.Redirect(location); return Task.CompletedTask; }).InvokeAsync(context);
        Assert.Equal("/", context.Response.Headers.Location);
    }

    [Fact]
    public async Task Valid_destination_is_preserved()
    {
        var context = Context();
        await new DieuHuongSauDangNhap(c => { c.Response.StatusCode = 200; return Task.CompletedTask; }).InvokeAsync(context);
        Assert.Equal(200, context.Response.StatusCode);
        Assert.False(context.Response.Headers.ContainsKey("Location"));
    }

    [Fact]
    public async Task Other_requests_do_not_consume_pending_destination()
    {
        var context = Context();
        var path = context.Request.Path;
        context.Request.Path = "/Home/ChuaDoc";
        var middleware = new DieuHuongSauDangNhap(c => { c.Response.StatusCode = 404; return Task.CompletedTask; });
        await middleware.InvokeAsync(context);
        Assert.Equal(404, context.Response.StatusCode);
        context.Request.Path = path;
        await middleware.InvokeAsync(context);
        Assert.Equal("/", context.Response.Headers.Location);
    }

    [Fact]
    public async Task Server_errors_are_not_hidden_as_successful_navigation()
    {
        var context = Context();
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new DieuHuongSauDangNhap(_ => throw new InvalidOperationException("Database unavailable")).InvokeAsync(context));
    }

    private sealed class MemorySession : ISession
    {
        private readonly Dictionary<string, byte[]> values = [];
        public bool IsAvailable => true;
        public string Id => "LoginDestinationTests";
        public IEnumerable<string> Keys => values.Keys;
        public void Clear() => values.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => values.Remove(key);
        public void Set(string key, byte[] value) => values[key] = value;
        public bool TryGetValue(string key, out byte[] value) => values.TryGetValue(key, out value!);
    }
}
