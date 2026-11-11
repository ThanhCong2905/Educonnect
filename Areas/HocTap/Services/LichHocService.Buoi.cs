using System.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.HocTap.Services;

public sealed partial class LichHocService
{
    public async Task BatDauBuoiAsync(Guid id, string version)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var schedule = await LayAsync(id);
        await rights.KiemTraQuyenGhiAsync(schedule.LopHocId, true);
        Rules.Check(schedule.TrangThai == "DaLenLich" && schedule.BatDauLuc <= DateTimeOffset.UtcNow && schedule.KetThucLuc > DateTimeOffset.UtcNow,
            "Chỉ bắt đầu trong khung giờ đã lên lịch.", 409);
        var lesson = await db.Set<BuoiHoc>().SingleAsync(x => x.LichHocId == id);
        Rules.Check(lesson.TrangThai == "DuKien", "Buổi học đã thay đổi trạng thái.", 409);
        Rules.Concurrency(db, lesson, version);
        lesson.TrangThai = "DangDienRa";
        lesson.CapNhatLuc = DateTimeOffset.UtcNow;
        lesson.PhienBan++;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task ThongBaoNghiAsync(Guid id, string reason, bool canHocBu, string version)
    {
        reason = reason?.Trim() ?? "";
        Rules.Check(reason.Length is > 0 and <= 2000, "Nhập lý do nghỉ tối đa 2000 ký tự.", field: "reason");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var schedule = await LayAsync(id);
        var lop = await rights.KiemTraQuyenGhiAsync(schedule.LopHocId, true);
        var me = await current.HienTaiAsync();
        var lesson = await db.Set<BuoiHoc>().SingleAsync(x => x.LichHocId == id);
        Rules.Check(lesson.TrangThai is "DuKien" or "DangDienRa", "Buổi đã hoàn tất, nghỉ hoặc hủy.", 409);
        Rules.Check(!await db.Set<BaiTapGiao>().AnyAsync(x => x.BuoiHocId == lesson.Id && x.TrangThai == "DaGiao"),
            "Buổi này đã có bài tập công bố. Hãy kiểm tra hoạt động trước khi báo nghỉ.", 409);
        Rules.Concurrency(db, lesson, version);
        lesson.TrangThai = "DaNghi";
        lesson.LyDoNghi = reason;
        lesson.CanHocBu = canHocBu;
        lesson.NguoiThongBaoNghiId = me.Id;
        lesson.ThongBaoNghiLuc = DateTimeOffset.UtcNow;
        lesson.CapNhatLuc = DateTimeOffset.UtcNow;
        lesson.PhienBan++;
        schedule.TrangThai = "DaHuy";
        await notices.TaoChoLopAsync(lop.Id, "LichHoc", lop.Id, "Buổi học nghỉ: " + reason + (canHocBu ? " · Chờ xếp lịch bù." : " · Không học bù."), me.Id);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
