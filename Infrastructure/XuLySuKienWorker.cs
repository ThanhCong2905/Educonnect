using System.Text.Json;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Infrastructure;

public sealed class XuLySuKienWorker(IServiceScopeFactory scopes, IHubContext<CapNhatThoiGianThucHub> hub,
    PhienCapNhat phien, ILogger<XuLySuKienWorker> log) : BackgroundService
{
    private DateTimeOffset lanKiemTraQuyen;
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));
        while (!token.IsCancellationRequested)
        {
            try
            {
                phien.DonDep();
                await XuLySuKienDenHanAsync(32, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { break; }
            catch (Exception ex) { log.LogWarning(ex, "Chưa xử lý xong cập nhật; sẽ thử lại."); }
            try { if (!await timer.WaitForNextTickAsync(token)) break; }
            catch (OperationCanceledException) { break; }
        }
    }
    public async Task XuLySuKienDenHanAsync(int soLuong, CancellationToken token)
    {
        soLuong = Math.Clamp(soLuong, 1, 100);
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var messages = scope.ServiceProvider.GetRequiredService<TinNhanService>();
        if (DateTimeOffset.UtcNow - lanKiemTraQuyen > TimeSpan.FromSeconds(5))
        {
            await KiemTraKetNoiAsync(messages, token);
            lanKiemTraQuyen = DateTimeOffset.UtcNow;
        }
        var lease = Guid.NewGuid();
        // A single atomic UPDATE claims committed rows. Expired leases can be recovered after a crash.
        List<SuKienChoXuLy> events;
        await using (var claim = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, token))
        {
        events = await db.Set<SuKienChoXuLy>().FromSqlInterpolated($"""
            ;WITH viec AS (
              SELECT TOP ({soLuong}) * FROM dbo.SuKienChoXuLy WITH (UPDLOCK, READPAST, READCOMMITTEDLOCK)
              WHERE (TrangThai IN ('ChoXuLy','ChoThuLai') AND (ThuLaiLuc IS NULL OR ThuLaiLuc<=SYSDATETIMEOFFSET()))
                 OR (TrangThai='DangXuLy' AND LeaseHetHanLuc<=SYSDATETIMEOFFSET())
              ORDER BY TaoLuc,Id
            )
            UPDATE viec SET TrangThai='DangXuLy',LeaseId={lease},
              LeaseHetHanLuc=DATEADD(second,60,SYSDATETIMEOFFSET()),SoLanThu=SoLanThu+1
            OUTPUT inserted.*;
            """).AsNoTracking().ToListAsync(token);
        await claim.CommitAsync(token);
        }
        foreach (var ev in events)
        {
            try
            {
                // Keep one batch from holding a claim beyond its lease and publishing as a stale owner.
                var owned = await db.Database.ExecuteSqlInterpolatedAsync($"""
                    UPDATE dbo.SuKienChoXuLy SET LeaseHetHanLuc=DATEADD(second,60,SYSDATETIMEOFFSET())
                    WHERE Id={ev.Id} AND LeaseId={lease} AND TrangThai='DangXuLy' AND LeaseHetHanLuc>SYSDATETIMEOFFSET()
                    """, token);
                if (owned == 0) continue;
                await PhatCapNhatAsync(ev, db, messages, token);
                await db.Database.ExecuteSqlInterpolatedAsync($"""
                    UPDATE dbo.SuKienChoXuLy SET TrangThai='HoanTat',XuLyXongLuc=SYSDATETIMEOFFSET(),
                      ThuLaiLuc=NULL,LeaseId=NULL,LeaseHetHanLuc=NULL WHERE Id={ev.Id} AND LeaseId={lease}
                    """, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
            catch (Exception ex)
            {
                var seconds = Math.Min(60, 1 << Math.Min(ev.SoLanThu, 5));
                await db.Database.ExecuteSqlInterpolatedAsync($"""
                    UPDATE dbo.SuKienChoXuLy SET TrangThai='ChoThuLai',ThuLaiLuc=DATEADD(second,{seconds},SYSDATETIMEOFFSET()),
                      LeaseId=NULL,LeaseHetHanLuc=NULL WHERE Id={ev.Id} AND LeaseId={lease}
                    """, token);
                log.LogWarning(ex, "Không phát được sự kiện {EventId}; sẽ thử lại.", ev.Id);
            }
        }
    }
    private async Task KiemTraKetNoiAsync(TinNhanService messages, CancellationToken token)
    {
        foreach (var c in phien.DanhSach())
        {
            if (!phien.HopLe(c)) { phien.Xoa(c.Id, true); continue; }
            foreach (var room in c.HoiThoai.Keys)
            {
                try { await messages.KiemTraAsync(room, c.Nguoi); }
                catch (LoiNghiepVu)
                {
                    c.HoiThoai.TryRemove(room, out _);
                    await hub.Groups.RemoveFromGroupAsync(c.Id, c.NhomHoiThoai(room), token);
                }
            }
        }
    }
    private async Task PhatCapNhatAsync(SuKienChoXuLy ev, AppDbContext db, TinNhanService messages, CancellationToken token)
    {
        using var json = JsonDocument.Parse(ev.DuLieuToiThieu);
        var data = json.RootElement;
        var target = data.GetProperty("target").GetGuid();
        var room = ev.LoaiSuKien == "TinNhan" ? data.GetProperty("room").GetGuid() : (Guid?)null;
        var recipient = room == null ? data.GetProperty("recipient").GetGuid() : (Guid?)null;
        foreach (var c in phien.DanhSach())
        {
            if (!phien.HopLe(c)) { phien.Xoa(c.Id, true); continue; }
            if (room.HasValue ? !c.HoiThoai.ContainsKey(room.Value) : c.Nguoi != recipient) continue;
            if (!await db.Set<NguoiThamGia>().AnyAsync(x => x.Id == c.Nguoi, token)) { phien.Xoa(c.Id, true); continue; }
            if (room.HasValue)
            {
                try { await messages.KiemTraAsync(room.Value, c.Nguoi); }
                catch (LoiNghiepVu)
                {
                    c.HoiThoai.TryRemove(room.Value, out _);
                    await hub.Groups.RemoveFromGroupAsync(c.Id, c.NhomHoiThoai(room.Value), token);
                    continue;
                }
            }
            if (!phien.HopLe(c)) { phien.Xoa(c.Id, true); continue; }
            // Each group is scoped to a connection so stale permissions cannot inherit a broad broadcast.
            var group = room.HasValue ? c.NhomHoiThoai(room.Value) : c.NhomCaNhan;
            await hub.Clients.Group(group).SendAsync("CapNhat", new { maSuKien = ev.Id, loai = ev.LoaiSuKien, doiTuongId = target, hoiThoaiId = room }, token);
        }
        // Successful transport dispatch is not a delivery/read receipt. Offline users reload from SQL.
    }
}
