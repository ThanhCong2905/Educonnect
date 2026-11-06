using EduConnect.Mvc.Areas.HocTap.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EduConnect.Mvc.Infrastructure;

[Authorize]
public sealed class CapNhatThoiGianThucHub(PhienCapNhat phien, TinNhanService messages, PhienDangNhapService sessions) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (!Guid.TryParse(Context.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var user)
            || !Guid.TryParse(Context.User.FindFirstValue(PhienDangNhapService.SessionClaim), out var session)
            || !await sessions.HopLeAsync(session, user, false, Context.ConnectionAborted))
            throw new HubException("Phiên đã hết hạn. Vui lòng đăng nhập lại.");
        phien.CapNhat(session.ToString("D"), user);
        var c = phien.Them(Context.ConnectionId, session.ToString("D"), user, Context.Abort);
        await Groups.AddToGroupAsync(c.Id, c.NhomCaNhan);
        await base.OnConnectedAsync();
    }
    public async Task ThamGiaHoiThoai(Guid id)
    {
        var c = await HienTaiAsync();
        try { await messages.KiemTraAsync(id, c.Nguoi); }
        catch (LoiNghiepVu) { throw new HubException("Bạn không có quyền xem hội thoại này."); }
        if (c.HoiThoai.Count >= 10 && !c.HoiThoai.ContainsKey(id)) throw new HubException("Đã mở quá nhiều hội thoại.");
        if (!phien.HopLe(c)) { Context.Abort(); throw new HubException("Phiên đã thay đổi."); }
        c.HoiThoai[id] = 0;
        await Groups.AddToGroupAsync(c.Id, c.NhomHoiThoai(id));
    }
    public async Task RoiHoiThoai(Guid id)
    {
        var c = await HienTaiAsync();
        c.HoiThoai.TryRemove(id, out _);
        await Groups.RemoveFromGroupAsync(c.Id, c.NhomHoiThoai(id));
    }
    public async Task<Guid> KiemTraPhien() => (await HienTaiAsync()).Nguoi;
    private async Task<PhienCapNhat.KetNoi> HienTaiAsync()
    {
        var c = phien.Lay(Context.ConnectionId);
        if (c == null || !phien.HopLe(c)
            || !Guid.TryParse(c.PhienId, out var session)
            || !await sessions.HopLeAsync(session, c.Nguoi, false, Context.ConnectionAborted))
        {
            Context.Abort();
            throw new HubException("Phiên đã hết hạn. Vui lòng đăng nhập lại.");
        }
        return c;
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        phien.Xoa(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
