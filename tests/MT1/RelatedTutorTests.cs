using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed class RelatedTutorTests
{
    private static TimKiemService Search(AppDbContext db) => new(db,
        new NguoiDungHienTaiService(db, new HttpContextAccessor { HttpContext = new DefaultHttpContext() }));

    [Fact]
    public async Task Related_tutors_use_public_profiles_shared_subject_and_scope_and_comparable_fee_units()
    {
        await using var db = ProposalRegressionTests.Open();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var subject = new MonHoc { TenMon = "Kiểm thử gợi ý " + Guid.NewGuid().ToString("N"), DangSuDung = true };
        db.Add(subject); await db.SaveChangesAsync();
        var profiles = new List<HoSoGiaSu>();
        foreach (var fee in new[] { 100000m, 80000m, 120000m, 90000m, 95000m, 110000m })
        {
            var people = await ProposalRegressionTests.Seed(db);
            var profile = await db.Set<HoSoGiaSu>().SingleAsync(item => item.GiaSuId == people.Tutor);
            var offer = await db.Set<HoSoGiaSuMonDay>().SingleAsync(item => item.HoSoGiaSuId == profile.Id);
            db.Remove(offer); await db.SaveChangesAsync();
            db.Add(new HoSoGiaSuMonDay { HoSoGiaSuId = profile.Id, MonHocId = subject.Id, MatNaKhoi = PhamViMonHoc.BitKhoi(12), PhamViDay = "Lớp 12", HocPhiDeXuat = fee, DonViHocPhi = "VND/Gio" });
            await db.SaveChangesAsync();
            profiles.Add(profile);
        }
        var differentUnit = await db.Set<HoSoGiaSuMonDay>().SingleAsync(item => item.HoSoGiaSuId == profiles[3].Id);
        differentUnit.DonViHocPhi = "VND/Buoi";
        profiles[4].DangHienThi = false;
        var differentGrade = await db.Set<HoSoGiaSuMonDay>().SingleAsync(item => item.HoSoGiaSuId == profiles[5].Id);
        differentGrade.MatNaKhoi = PhamViMonHoc.BitKhoi(10); differentGrade.PhamViDay = "Lớp 10";
        await db.SaveChangesAsync();
        var items = await Search(db).GiaSuLienQuanAsync(profiles[0].Id);
        Assert.Equal(3, items.Count); Assert.Equal(3, items.Select(item => item.HoSo.Id).Distinct().Count());
        Assert.All(items, item => Assert.Equal(subject.Id, item.MonHocId));
        Assert.Equal(-20000m, items.Single(item => item.HoSo.Id == profiles[1].Id).ChenhLech);
        Assert.Equal(20000m, items.Single(item => item.HoSo.Id == profiles[2].Id).ChenhLech);
        Assert.Null(items.Single(item => item.HoSo.Id == profiles[3].Id).ChenhLech);
        Assert.DoesNotContain(items, item => item.HoSo.Id == profiles[0].Id || item.HoSo.Id == profiles[4].Id || item.HoSo.Id == profiles[5].Id);

        var limited = await Search(db).HocPhiThamChieuAsync(profiles[0].Id);
        var hourly = limited.Single(item => item.DonVi == "VND/Gio");
        Assert.Equal(2, hourly.SoHoSo); Assert.Equal(100000m, hourly.TrungBinh); Assert.False(hourly.DuSoLieu);
        Assert.Equal(1, limited.Single(item => item.DonVi == "VND/Buoi").SoHoSo);
        profiles[4].DangHienThi = true; differentGrade.MatNaKhoi = PhamViMonHoc.BitKhoi(12); differentGrade.PhamViDay = "Lớp 12";
        await db.SaveChangesAsync();
        var references = await Search(db).HocPhiThamChieuAsync(profiles[0].Id);
        hourly = references.Single(item => item.DonVi == "VND/Gio");
        Assert.Equal(4, hourly.SoHoSo); Assert.Equal(101250m, hourly.TrungBinh); Assert.True(hourly.DuSoLieu);
        Assert.Equal(-1250m, new DoiChieuHocPhiViewModel(hourly, 100000m, "VND/Gio").ChenhLech);
        profiles[4].DangHienThi = false; differentGrade.MatNaKhoi = PhamViMonHoc.BitKhoi(10); differentGrade.PhamViDay = "Lớp 10";
        await db.SaveChangesAsync();

        profiles[0].DayTrucTuyen = false; profiles[0].DayTrucTiep = true;
        profiles[1].DayTrucTiep = true;
        profiles[2].DayTrucTiep = true; profiles[2].KhuVuc = "Thành phố Hồ Chí Minh";
        await db.SaveChangesAsync();
        var inPerson = await Search(db).GiaSuLienQuanAsync(profiles[0].Id);
        Assert.Single(inPerson); Assert.Equal(profiles[1].Id, inPerson[0].HoSo.Id);

        profiles[0].DangHienThi = false; await db.SaveChangesAsync();
        Assert.Equal(404, (await Assert.ThrowsAsync<LoiNghiepVu>(() => Search(db).GiaSuLienQuanAsync(profiles[0].Id))).Status);
    }

    [Fact]
    public void Price_reference_handles_free_tuition_small_samples_and_different_units()
    {
        var reference = new HocPhiThamChieuViewModel { TrungBinh = 0, SoHoSo = 3, DonVi = "VND/Gio" };
        Assert.Equal(0m, new DoiChieuHocPhiViewModel(reference, 0, "VND/Gio").ChenhLech);
        Assert.Equal(20000m, new DoiChieuHocPhiViewModel(reference, 20000, "VND/Gio").ChenhLech);
        Assert.Null(new DoiChieuHocPhiViewModel(reference, 20000, "VND/Buoi").ChenhLech);
        Assert.Null(new DoiChieuHocPhiViewModel(new() { TrungBinh = 100000, SoHoSo = 2, DonVi = "VND/Gio" }, 50000, "VND/Gio").ChenhLech);
    }
}
