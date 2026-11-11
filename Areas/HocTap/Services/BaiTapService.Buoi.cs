using System.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.HocTap.Services;

public sealed partial class BaiTapService
{
    public async Task<Guid> LichCuaBuoiAsync(Guid id)
    {
        var lesson = await db.Set<BuoiHoc>().SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new LoiNghiepVu("Buổi học không tồn tại.", 404);
        var schedule = await db.Set<LichHoc>().SingleAsync(x => x.Id == lesson.LichHocId);
        await rights.KiemTraQuyenXemAsync(schedule.LopHocId);
        return schedule.Id;
    }

    public async Task GanBuoiAsync(Guid assignment, Guid lessonId, string version)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var task = await ChiTietAsync(assignment);
        await rights.KiemTraQuyenGhiAsync(task.LopHocId, true);
        Rules.Check(task.BuoiHocId == null, "Bài tập đã có buổi học.", 409);
        Rules.Check(await (from b in db.Set<BuoiHoc>() join l in db.Set<LichHoc>() on b.LichHocId equals l.Id where b.Id == lessonId && l.LopHocId == task.LopHocId && b.TrangThai != "DaNghi" && b.TrangThai != "DaHuy" select b).AnyAsync(), "Chọn buổi hợp lệ trong đúng lớp.");
        Rules.Concurrency(db, task, version);
        task.BuoiHocId = lessonId;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task CongBoAsync(Guid id, string version)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var task = await ChiTietAsync(id);
        var lop = await rights.KiemTraQuyenGhiAsync(task.LopHocId, true);
        Rules.Check(task.TrangThai == "BanNhap" && task.HanNopLuc > DateTimeOffset.UtcNow, "Bài đã giao hoặc hạn nộp đã qua.", 409);
        Rules.Check(await db.Set<BuoiHoc>().AnyAsync(x => x.Id == task.BuoiHocId && (x.TrangThai == "DangDienRa" || x.TrangThai == "DaHoc")), "Chỉ giao bài trong buổi đang diễn ra hoặc đã học.", 409);
        Rules.Concurrency(db, task, version);
        task.TrangThai = "DaGiao";
        await notices.TaoChoLopAsync(lop.Id, "BaiTap", task.Id, "Có bài tập mới.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
