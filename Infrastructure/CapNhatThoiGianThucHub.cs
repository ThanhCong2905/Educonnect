using EduConnect.Mvc.Areas.HocTap.Services;
using Microsoft.AspNetCore.SignalR;

namespace EduConnect.Mvc.Infrastructure;

public sealed class CapNhatThoiGianThucHub(PhienCapNhat phien, TinNhanService messages) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var http = Context.GetHttpContext()!;
        await http.Session.LoadAsync(Context.ConnectionAborted);
        if (!Guid.TryParse(http.Session.GetString("participant"), out var user))
            throw new HubException("Hãy mở ứng dụng để thiết lập phiên trước khi kết nối.");
        var c = phien.Them(Context.ConnectionId, http.Session.Id, user, Context.Abort);
        await Groups.AddToGroupAsync(c.Id, c.NhomCaNhan);
        await base.OnConnectedAsync();
    }
    public async Task ThamGiaHoiThoai(Guid id)
    {
        var c = HienTai();
        try { await messages.KiemTraAsync(id, c.Nguoi); }
        catch (LoiNghiepVu) { throw new HubException("Bạn không có quyền xem hội thoại này."); }
        if (c.HoiThoai.Count >= 10 && !c.HoiThoai.ContainsKey(id)) throw new HubException("Đã mở quá nhiều hội thoại.");
        if (!phien.HopLe(c)) { Context.Abort(); throw new HubException("Phiên đã thay đổi."); }
        c.HoiThoai[id] = 0;
        await Groups.AddToGroupAsync(c.Id, c.NhomHoiThoai(id));
    }
    public async Task RoiHoiThoai(Guid id)
    {
        var c = HienTai();
        c.HoiThoai.TryRemove(id, out _);
        await Groups.RemoveFromGroupAsync(c.Id, c.NhomHoiThoai(id));
    }
    public Guid KiemTraPhien() => HienTai().Nguoi;
    private PhienCapNhat.KetNoi HienTai()
    {
        var c = phien.Lay(Context.ConnectionId);
        if (c == null || !phien.HopLe(c)) { Context.Abort(); throw new HubException("Phiên đã hết hạn hoặc đã đổi tài khoản."); }
        return c;
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        phien.Xoa(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
