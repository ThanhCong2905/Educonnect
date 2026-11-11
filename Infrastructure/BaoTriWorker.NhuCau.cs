using System.Data;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Infrastructure;

public sealed partial class BaoTriWorker
{
    private async Task XuLyHieuLucNhuCauAsync(CancellationToken huy)
    {
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notices = scope.ServiceProvider.GetRequiredService<ThongBaoService>();
        var now = DateTimeOffset.UtcNow;
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, huy);
        var due = await db.Set<NhuCauHoc>().Where(x => x.TrangThai == "DangMo" && (x.XacNhanConNhuCauLuc ?? x.TaoLuc) <= now.AddDays(-30)
            && (x.NhacXacNhanLuc == null || x.NhacXacNhanLuc <= now.AddDays(-7)))
            .OrderBy(x => x.Id).Take(100).ToListAsync(huy);
        foreach (var need in due)
        {
            if (need.NhacXacNhanLuc == null)
            {
                need.NhacXacNhanLuc = now;
                notices.Tao(need.ChuSoHuuId, "NhuCau", need.Id, "Bạn còn tìm gia sư cho nhu cầu này không? Hãy xác nhận trong 7 ngày để tiếp tục hiển thị.");
            }
            else if (need.NhacXacNhanLuc <= now.AddDays(-7))
            {
                need.TrangThai = "DaDong";
                need.LyDoDong = "HetHieuLucTimKiem";
                notices.Tao(need.ChuSoHuuId, "NhuCau", need.Id, "Nhu cầu đã đóng vì chưa xác nhận còn tìm gia sư. Bạn có thể mở lại khi cần.");
            }
        }
        await db.SaveChangesAsync(huy);
        await tx.CommitAsync(huy);
    }
}
