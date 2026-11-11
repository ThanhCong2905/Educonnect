using System.Security.Claims;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.SignalR;

namespace EduConnect.Mvc.Infrastructure;

public sealed class NhatKyAnToanHubFilter(IServiceScopeFactory scopes, ILogger<NhatKyAnToanHubFilter> logger) : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext context,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            var result = await next(context);
            if (context.HubMethodName == nameof(CapNhatThoiGianThucHub.ThamGiaHoiThoai))
                await GhiAsync(context.Context, context.HubMethodName, "ThanhCong", context.HubMethodArguments.FirstOrDefault() as Guid?);
            return result;
        }
        catch (HubException)
        {
            await GhiAsync(context.Context, context.HubMethodName, "TuChoi", context.HubMethodArguments.FirstOrDefault() as Guid?);
            throw;
        }
    }

    public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        try { await next(context); }
        catch (HubException)
        {
            await GhiAsync(context.Context, "KetNoi", "TuChoi", category: "Authentication");
            throw;
        }
    }

    private async Task GhiAsync(HubCallerContext caller, string method, string result, Guid? target = null, string category = "Authorization")
    {
        var http = caller.GetHttpContext();
        var actor = Guid.TryParse(caller.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var user) ? user : (Guid?)null;
        var record = new NhatKyAnToan
        {
            Nhom = category, SuKien = "SignalR." + method, KetQua = result,
            TaiKhoanId = actor, DiaChiIp = http?.Connection.RemoteIpAddress?.ToString() ?? "",
            DuongDan = "/cap-nhat", PhuongThuc = "SignalR", MaYeuCau = Guid.NewGuid().ToString("D"),
            LyDo = result == "TuChoi" ? "PhienHoacQuyenHoiThoaiKhongHopLe" : "",
            DoiTuongId = target?.ToString()
        };
        try
        {
            await using var scope = scopes.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Add(record);
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await db.SaveChangesAsync(timeout.Token);
        }
        catch (Exception ex) { logger.LogError(ex, "Không ghi được nhật ký SignalR {Event} {Result} {RequestId}", record.SuKien, result, record.MaYeuCau); }
    }
}
