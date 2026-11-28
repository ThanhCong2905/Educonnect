using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static ProposalRegressionTests;

public sealed class ProposalSchedulePrefillTests
{
    [Fact]
    public async Task Learning_request_copies_need_schedule_but_saved_tutor_changes_take_precedence()
    {
        await using var db = Open();
        var seed = await Seed(db);
        var need = new NhuCauHoc
        {
            Id = Guid.NewGuid(), ChuSoHuuId = seed.Learner, MonHocId = 1, LoaiNhuCau = "TimGiaSu",
            TrangThai = "DangMo", HinhThucHoc = "TrucTuyen", MucTieu = "Ôn tập buổi tối", SoBuoiTuan = 2,
            HocPhiDuKien = 120000, DonViHocPhi = "VND/Gio", TaoLuc = DateTimeOffset.UtcNow
        };
        db.Add(need);await db.SaveChangesAsync();
        foreach (var day in new byte[] { 1, 3 })
            db.Add(new KhungGioRanh { Id = Guid.NewGuid(), NhuCauId = need.Id, ThuTrongTuan = day, GioBatDau = new(19, 15), GioKetThuc = new(20, 15) });
        await db.SaveChangesAsync();
        var id = await Service(db, seed.Learner).GuiYeuCauAsync(new() { LienHeId = seed.Contact, NhuCauId = need.Id, LoiNhan = "Nhờ gia sư xem lịch trong nhu cầu." });
        var input = await Service(db, seed.Tutor).LayDeXuatAsync(seed.Contact, id);
        Assert.Equal(2, input.LichRanh.Count);
        Assert.Equal(60, input.ThoiLuongPhut);
        Assert.Equal(new byte[] { 1, 3 }, input.LichRanh.Select(slot => slot.ThuTrongTuan));
        Assert.All(input.LichRanh, slot => Assert.Equal(new TimeOnly(19, 15), slot.GioBatDau));
        var choices = await Service(db, seed.Tutor).LichNhuCauDeXuatAsync(seed.Contact);
        Assert.Equal(2, choices[need.Id].Count);
        input.TenLop = "Ôn tập theo lịch đã trao đổi";
        foreach (var slot in input.LichRanh) { slot.GioBatDau = new(17, 30);slot.GioKetThuc = new(18, 30); }
        await Service(db, seed.Tutor).GuiDeXuatAsync(input);
        var saved = await Service(db, seed.Tutor).LayDeXuatAsync(seed.Contact, id);
        Assert.All(saved.LichRanh, slot => Assert.Equal(new TimeOnly(17, 30), slot.GioBatDau));
        var outsider = await Seed(db);
        Assert.Equal(403, (await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, outsider.Tutor).LichNhuCauDeXuatAsync(seed.Contact))).Status);
    }
}
