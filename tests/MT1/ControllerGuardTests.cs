using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.Controllers;
using EduConnect.Mvc.Areas.KetNoi.Controllers;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.Controllers;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public sealed class ControllerGuardTests
{
    [Theory]
    [InlineData("GuiYeuCau")]
    [InlineData("ChapNhan")]
    [InlineData("TuChoi")]
    [InlineData("Rut")]
    [InlineData("HuyKetNoi")]
    [InlineData("PhanHoiHuy")]
    public async Task Invalid_model_stops_before_accessing_services(string action)
    {
        var controller = new KetNoiController(null!, null!)
        {
            ControllerContext = new() { HttpContext = new DefaultHttpContext() }
        };
        controller.ModelState.AddModelError("RowVersion", "Phiên bản dữ liệu không hợp lệ.");
        var error = await Assert.ThrowsAsync<LoiNghiepVu>(() => action switch
        {
            "GuiYeuCau" => controller.GuiYeuCau(new GuiYeuCauViewModel()),
            "ChapNhan" => controller.ChapNhan(new ChapNhanDeXuatViewModel()),
            "TuChoi" => controller.TuChoi(Guid.NewGuid(), "", []),
            "Rut" => controller.Rut(Guid.NewGuid(), []),
            "HuyKetNoi" => controller.HuyKetNoi(Guid.NewGuid(), "", []),
            "PhanHoiHuy" => controller.PhanHoiHuy(new PhanHoiHuyViewModel()),
            _ => throw new ArgumentOutOfRangeException(nameof(action))
        });
        Assert.Equal(400, error.Status);
        Assert.Contains("Phiên bản dữ liệu", error.Message);
    }

    [Fact]
    public async Task Invalid_end_confirmation_stops_before_loading_actor()
    {
        var controller = new LopHocController(null!, null!, null!, null!)
        {
            ControllerContext = new() { HttpContext = new DefaultHttpContext() }
        };
        controller.ModelState.AddModelError("rowVersion", "Phiên bản dữ liệu không hợp lệ.");
        var error = await Assert.ThrowsAsync<LoiNghiepVu>(() => controller.XacNhanKetThuc(Guid.NewGuid(), []));
        Assert.Equal(400, error.Status);
    }

    [Fact]
    public async Task Invalid_search_model_stops_before_loading_catalog()
    {
        var controller = new TimKiemController(null!, null!, null!, null!);
        controller.ModelState.AddModelError("Trang", "Trang phải lớn hơn 0.");
        var error = await Assert.ThrowsAsync<LoiNghiepVu>(() => controller.Tim(new BoLocTimKiemViewModel()));
        Assert.Equal(400, error.Status);
    }

    [Fact]
    public void Cancelled_end_confirmation_stops_before_accessing_connection_service()
    {
        var service = new LopHocService(null!, null!, null!, null!, null!);
        using var source = new CancellationTokenSource();
        source.Cancel();
        var error = Assert.Throws<OperationCanceledException>(() =>
        {
            _ = service.XacNhanKetThucAsync(Guid.NewGuid(), new byte[8], Guid.NewGuid(), source.Token);
        });
        Assert.Equal(source.Token, error.CancellationToken);
    }

    [Theory]
    [InlineData(400)]
    [InlineData(403)]
    [InlineData(404)]
    [InlineData(409)]
    public async Task Middleware_maps_business_error_to_http_status(int status)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Headers.Accept = "text/html";
        var middleware = new LoiNghiepVuMiddleware(_ => Task.FromException(new LoiNghiepVu("<script>không hợp lệ</script>", status)), NullLogger<LoiNghiepVuMiddleware>.Instance);
        await middleware.InvokeAsync(context);
        Assert.Equal(status, context.Response.StatusCode);
        context.Response.Body.Position = 0;
        var html = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Contains("&lt;script&gt;", html);
        Assert.DoesNotContain("<script>", html);
    }

    [Fact]
    public async Task Unexpected_error_does_not_expose_internal_details()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new LoiNghiepVuMiddleware(_ => Task.FromException(new InvalidOperationException("private-database-detail")), NullLogger<LoiNghiepVuMiddleware>.Instance);
        await middleware.InvokeAsync(context);
        Assert.Equal(500, context.Response.StatusCode);
        context.Response.Body.Position = 0;
        Assert.DoesNotContain("private-database-detail", await new StreamReader(context.Response.Body).ReadToEndAsync());
    }
}
