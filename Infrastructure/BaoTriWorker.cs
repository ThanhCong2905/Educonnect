using System.Data;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Infrastructure;

public sealed class BaoTriWorker(IServiceScopeFactory scopes, ILogger<BaoTriWorker> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken huy)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
        while (!huy.IsCancellationRequested)
        {
            try
            {
                var now = DateTimeOffset.UtcNow;
                await XuLyHetHanYeuCauAsync(now, huy);
                await XuLyTuHuyKetNoiAsync(now, huy);
                await XuLyNhacHocTapAsync(huy);
            }
            catch (OperationCanceledException) when (huy.IsCancellationRequested) { break; }
            catch (Exception ex) { log.LogWarning(ex, "Bảo trì chưa hoàn tất; sẽ thử lại."); }
            try
            {
                if (!await timer.WaitForNextTickAsync(huy))
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
                         && !db.Set<BuoiHoc>().Any(b => b.LichHocId == s.Id)
                         && !db.Set<ThongBao>().Any(n => n.LoaiThongBao == "NhacViec" && n.DoiTuongId == s.Id && n.NguoiNhanId == c.GiaSuId && n.NoiDungTomTat == message)
                         orderby s.KetThucLuc
                         select new { s.Id, c.GiaSuId }).Take(200).ToListAsync(huy);
        foreach (var item in due)
            notices.Tao(item.GiaSuId, "NhacViec", item.Id, message);
        await db.SaveChangesAsync(huy);
        await tx.CommitAsync(huy);
    }

    private async Task XuLyTuHuyKetNoiAsync(DateTimeOffset bayGio, CancellationToken huy)
    {
        List<(Guid Id, Guid Lop)> ids;
        using (var scope = scopes.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var rows = await db.Set<YeuCauHuyKetNoi>().AsNoTracking()
                .Where(x => x.TrangThai == "ChoPhanHoi" && x.HetHanLuc <= bayGio)
                .Select(x => new { x.Id, x.LopHocId }).ToListAsync(huy);
            ids = rows.Select(x => (x.Id, x.LopHocId)).ToList();
        }
        foreach (var (id, maLop) in ids)
        {
            using var scope = scopes.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var notices = scope.ServiceProvider.GetRequiredService<ThongBaoService>();
            await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, huy);
            var lop = await db.Set<LopHoc>().FromSqlInterpolated($"SELECT * FROM dbo.LopHoc WITH (UPDLOCK,HOLDLOCK) WHERE Id={maLop}").SingleAsync(huy);
            var yc = await db.Set<YeuCauHuyKetNoi>().FromSqlInterpolated($"SELECT * FROM dbo.YeuCauHuyKetNoi WITH (UPDLOCK,HOLDLOCK) WHERE Id={id}").SingleAsync(huy);
            var now = await db.Database.SqlQuery<DateTimeOffset>($"SELECT TODATETIMEOFFSET(SYSUTCDATETIME(), '+00:00') AS Value").SingleAsync(huy);
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
            await db.SaveChangesAsync(huy);
            await tx.CommitAsync(huy);
        }
    }
}
