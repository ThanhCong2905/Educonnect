using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Infrastructure;

public sealed partial class BaoTriWorker
{
    private async Task XuLyDongDotPhanHoiAsync(CancellationToken huy)
    {
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notices = scope.ServiceProvider.GetRequiredService<ThongBaoService>();
        await using var tx = await db.Database.BeginTransactionAsync(huy);
        var closed = await db.Set<DotPhanHoi>().FromSqlRaw("""
            ;WITH due AS (
                SELECT TOP (100) * FROM dbo.DotPhanHoi WITH (UPDLOCK,READPAST,READCOMMITTEDLOCK)
                WHERE TrangThai=N'DangMo' AND HanGui<=SYSDATETIMEOFFSET()
                ORDER BY HanGui,Id
            )
            UPDATE due SET TrangThai=N'DaDong' OUTPUT inserted.*;
            """).AsNoTracking().ToListAsync(huy);
        foreach (var dot in closed)
            await notices.TaoChoLopAsync(dot.LopHocId, "PhanHoi", dot.LopHocId, "Đợt phản hồi đã hết hạn gửi.");
        await db.SaveChangesAsync(huy);
        await tx.CommitAsync(huy);
    }
}
