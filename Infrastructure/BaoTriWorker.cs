using System.Data;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Infrastructure;

public sealed partial class BaoTriWorker(IServiceScopeFactory scopes, ILogger<BaoTriWorker> log, IMaintenanceCycle? cycle = null, IMaintenanceTicks? ticks = null, TimeProvider? timeProvider = null) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken huy)
    {
        using var timer = ticks ?? new PeriodicMaintenanceTicks();
        while (!huy.IsCancellationRequested)
        {
            try
            {
                var now = (timeProvider ?? TimeProvider.System).GetUtcNow();
                await (cycle?.ExpireRequestsAsync(now, huy) ?? XuLyHetHanYeuCauAsync(now, huy));
                await (cycle?.AutoEndAsync(now, huy) ?? XuLyTuHuyKetNoiAsync(now, huy));
                await (cycle?.RemindersAsync(huy) ?? XuLyNhacHocTapAsync(huy));
                await (cycle?.NeedsAsync(huy) ?? XuLyHieuLucNhuCauAsync(huy));
                await (cycle?.FeedbackAsync(huy) ?? XuLyDongDotPhanHoiAsync(huy));
            }
            catch (OperationCanceledException) when (huy.IsCancellationRequested) { break; }
            catch (Exception ex) { log.LogWarning(ex, "Bảo trì chưa hoàn tất; sẽ thử lại."); }
            try
            {
                if (!await timer.WaitAsync(huy))
                    break;
            }
            catch (OperationCanceledException) when (huy.IsCancellationRequested) { break; }
        }
    }

    private async Task XuLyHetHanYeuCauAsync(DateTimeOffset bayGio, CancellationToken huy)
    {
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.ExecuteSqlRawAsync("UPDATE dbo.YeuCauKetNoiLopHoc SET TrangThai='HetHan' WHERE TrangThai IN ('ChoGiaSu','ChoBenHoc') AND HetHanLuc<=SYSDATETIMEOFFSET()", huy);
    }

    private async Task XuLyNhacHocTapAsync(CancellationToken huy)
    {
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notices = scope.ServiceProvider.GetRequiredService<ThongBaoService>();
        await using var tx = await db.Database.BeginTransactionAsync(huy);
        await db.Database.ExecuteSqlRawAsync("DECLARE @r int; EXEC @r=sys.sp_getapplock @Resource=N'EduConnect:HocTap:NhacGhiNhan',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=5000; IF @r<0 THROW 51163,N'Đang xử lý nhắc ghi nhận.',1;", huy);
        var now = await db.Database.SqlQuery<DateTimeOffset>($"SELECT TODATETIMEOFFSET(SYSUTCDATETIME(), '+00:00') AS Value").SingleAsync(huy);
        var cutoff = now.AddHours(-24);
        const string message = "Buổi học đã kết thúc quá 24 giờ nhưng chưa được ghi nhận.";
        var due = await (from s in db.Set<LichHoc>()
                         join c in db.Set<LopHoc>() on s.LopHocId equals c.Id
                         where s.TrangThai == "DaLenLich" && c.TrangThai == "DangHoatDong" && s.KetThucLuc <= cutoff
                         && !db.Set<BuoiHoc>().Any(b => b.LichHocId == s.Id && b.GhiNhanLuc != null)
                         && !db.Set<ThongBao>().Any(n => n.LoaiThongBao == "NhacViec" && n.DoiTuongId == s.Id && n.NguoiNhanId == c.GiaSuId && n.NoiDungTomTat == message)
                         orderby s.KetThucLuc
                         select new { s.Id, c.GiaSuId }).Take(200).ToListAsync(huy);
        foreach (var item in due)
            notices.Tao(item.GiaSuId, "NhacViec", item.Id, message);
        const string overdueMessage="Bài tập đã quá hạn và chưa có bài nộp.";
        var overdue=await (from a in db.Set<BaiTapGiao>() join c in db.Set<LopHoc>() on a.LopHocId equals c.Id
            where a.TrangThai=="DaGiao"&&c.TrangThai=="DangHoatDong"&&a.HanNopLuc<now
                &&!db.Set<BaiTapNop>().Any(n=>n.BaiTapGiaoId==a.Id)
                &&!db.Set<ThongBao>().Any(n=>n.DoiTuongId==a.Id&&n.NoiDungTomTat==overdueMessage)
            orderby a.HanNopLuc select new { a.Id,a.LopHocId }).Take(100).ToListAsync(huy);
        foreach(var item in overdue)
            await notices.TaoChoLopAsync(item.LopHocId,"BaiTap",item.Id,overdueMessage);
        await db.SaveChangesAsync(huy);
        await tx.CommitAsync(huy);
    }

    private async Task XuLyTuHuyKetNoiAsync(DateTimeOffset bayGio, CancellationToken huy)
    {
        List<(Guid Id, Guid Lop)> ids;
        using (var scope = scopes.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var store = scope.ServiceProvider.GetService<IClassEndingStore>() ?? new EfClassEndingStore(db);
            ids = await store.ExpiredAsync(bayGio, huy);
        }
        foreach (var (id, maLop) in ids)
        {
            using var scope = scopes.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var notices = scope.ServiceProvider.GetRequiredService<ThongBaoService>();
            var store = scope.ServiceProvider.GetService<IClassEndingStore>() ?? new EfClassEndingStore(db);
            await using var tx = await store.BeginAsync(huy);
            var lop = await store.LockClassAsync(maLop, huy) ?? throw new InvalidOperationException("Sequence contains no elements");
            var yc = await store.LockRequestAsync(id, huy);
            var now = await store.NowAsync(huy);
            if (yc.TrangThai != "ChoPhanHoi" || yc.HetHanLuc > now || lop.TrangThai != "DangHoatDong")
                continue;
            yc.TrangThai = "TuDongChapNhan";
            yc.PhanHoiLuc = now;
            yc.NguoiPhanHoiId = null;
            lop.TrangThai = "DaKetThuc";
            lop.KetThucLuc = now;
            lop.CachKetThuc = "HetHan";
            foreach (var recipient in new[] { lop.GiaSuId, lop.BenHocId })
                notices.Tao(recipient, "KetNoi", maLop, "Lớp đã kết thúc do yêu cầu không được phản hồi trong 72 giờ.");
            await store.SaveAsync(huy);
            await tx.CommitAsync(huy);
        }
    }
}
