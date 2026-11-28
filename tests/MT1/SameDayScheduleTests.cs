using System.Security.Claims;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static ProposalRegressionTests;

public sealed class SameDayScheduleTests
{
    [Fact]
    public async Task Same_day_allows_separate_and_adjacent_sessions_but_rejects_overlap()
    {
        await using var db = Open();var seed = await Seed(db);
        var request = await Service(db, seed.Tutor).GuiDeXuatAsync(new()
        {
            LienHeId = seed.Contact, TenNguoiHoc = "Người học", TenLop = "Kiểm thử lịch cùng ngày", MonHocId = 1,
            MucTieu = "Ôn tập", HocPhi = 0, ThoiLuongPhut = 60, BatDauHocLuc = DateTime.Today.AddDays(7),
            LichRanh = [new() { ThuTrongTuan = 1, GioBatDau = new(19, 0), GioKetThuc = new(20, 0) }]
        });
        var proposal = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(row => row.Id == request);
        var classroom = await Service(db, seed.Learner).ChapNhanDeXuatAsync(request, Convert.ToBase64String(proposal.RowVersion));
        var user = new NguoiDungHienTaiService(db, new HttpContextAccessor { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, seed.Tutor.ToString())], "Test")) } });
        var schedules = new LichHocService(db, user, new QuyenLopHocService(db, user), new ThongBaoService(db, user));
        var day = DateTime.Today.AddYears(10);
        var previous = await db.Set<LichHoc>().CountAsync(row => row.LopHocId == classroom);
        await schedules.ThemHangLoatAsync(new() { LopHocId = classroom, Buoi = [
            new() { TieuDe = "Buổi sáng 1", BatDauLuc = day.AddHours(9), KetThucLuc = day.AddHours(10) },
            new() { TieuDe = "Buổi sáng 2", BatDauLuc = day.AddHours(10), KetThucLuc = day.AddHours(11) },
            new() { TieuDe = "Buổi chiều", BatDauLuc = day.AddHours(14), KetThucLuc = day.AddHours(15) }
        ] });
        Assert.Equal(previous + 3, await db.Set<LichHoc>().CountAsync(row => row.LopHocId == classroom));
        var error = await Assert.ThrowsAnyAsync<Exception>(() => schedules.ThemHangLoatAsync(new() { LopHocId = classroom, Buoi = [new() { TieuDe = "Trùng giờ", BatDauLuc = day.AddHours(9).AddMinutes(30), KetThucLuc = day.AddHours(10).AddMinutes(30) }] }));
        Assert.Equal(409, Rules.ErrorStatus(error));
        Assert.Equal(previous + 3, await db.Set<LichHoc>().CountAsync(row => row.LopHocId == classroom));
    }
}
