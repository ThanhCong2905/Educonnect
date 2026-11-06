using System.Collections.Concurrent;

namespace EduConnect.Mvc.Infrastructure;

// Connection bookkeeping for the single-server deployment; it does not grant business permissions.
public sealed class PhienCapNhat(TimeProvider? time = null)
{
    private DateTimeOffset Now => (time ?? TimeProvider.System).GetUtcNow();
    public static readonly TimeSpan ThoiHan = TimeSpan.FromHours(8);
    private readonly ConcurrentDictionary<string, Phien> sessions = new();
    private readonly ConcurrentDictionary<string, KetNoi> connections = new();
    private sealed record Phien(Guid Nguoi, Guid Version, DateTimeOffset HetHan);
    public sealed record KetNoi(string Id, string PhienId, Guid Nguoi, Guid Version, Action Ngat)
    {
        public ConcurrentDictionary<Guid, byte> HoiThoai { get; } = new();
        public string NhomCaNhan => $"nguoi:{Nguoi:N}:{Id}";
        public string NhomHoiThoai(Guid id) => $"hoithoai:{id:N}:{Id}";
    }
    public void CapNhat(string id, Guid nguoi)
    {
        var now = Now;
        sessions.AddOrUpdate(id, _ => new(nguoi, Guid.NewGuid(), now + ThoiHan), (_, old) =>
            new(nguoi, old.Nguoi == nguoi && old.HetHan > now ? old.Version : Guid.NewGuid(), now + ThoiHan));
        foreach (var c in connections.Values.Where(c => c.PhienId == id && !HopLe(c))) Xoa(c.Id, true);
    }
    public KetNoi Them(string connectionId, string sessionId, Guid nguoi, Action abort)
    {
        if (!sessions.TryGetValue(sessionId, out var s) || s.Nguoi != nguoi || s.HetHan <= Now)
            throw new Microsoft.AspNetCore.SignalR.HubException("Phiên đã hết hạn. Hãy tải lại trang.");
        var c = new KetNoi(connectionId, sessionId, nguoi, s.Version, abort);
        connections[connectionId] = c;
        if (!HopLe(c)) { Xoa(connectionId, true); throw new Microsoft.AspNetCore.SignalR.HubException("Phiên đã thay đổi."); }
        return c;
    }
    public bool HopLe(KetNoi c) => sessions.TryGetValue(c.PhienId, out var s)
        && s.Nguoi == c.Nguoi && s.Version == c.Version && s.HetHan > Now;
    public KetNoi? Lay(string id) => connections.GetValueOrDefault(id);
    public KetNoi[] DanhSach() => connections.Values.ToArray();
    public void Xoa(string id, bool abort = false)
    {
        if (connections.TryRemove(id, out var c) && abort) c.Ngat();
    }
    public void ThuHoi(string sessionId)
    {
        sessions.TryRemove(sessionId, out _);
        foreach (var c in connections.Values.Where(c => c.PhienId == sessionId)) Xoa(c.Id, true);
    }
    public void DonDep()
    {
        foreach (var c in connections.Values.Where(c => !HopLe(c))) Xoa(c.Id, true);
        foreach (var pair in sessions.Where(p => p.Value.HetHan <= Now)) sessions.TryRemove(pair);
    }
}
