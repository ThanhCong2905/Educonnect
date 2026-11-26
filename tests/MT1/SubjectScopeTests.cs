using System.Security.Claims;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed class SubjectScopeTests
{
    [Fact]
    public async Task Editing_profile_restores_saved_grades_and_integrated_subject_parts()
    {
        await using var db = ProposalRegressionTests.Open();
        var seed = await ProposalRegressionTests.Seed(db);
        var profile = await db.Set<HoSoGiaSu>().SingleAsync(value => value.GiaSuId == seed.Tutor);
        var subject = await db.Set<MonHoc>().SingleAsync(value => value.TenMon == PhamViMonHoc.KhoaHocTuNhien);
        db.Add(new HoSoGiaSuMonDay
        {
            HoSoGiaSuId = profile.Id, MonHocId = subject.Id, PhamViDay = "Lớp 6, Lớp 7",
            MatNaKhoi = PhamViMonHoc.BitKhoi(6) | PhamViMonHoc.BitKhoi(7),
            PhanKhoaHoc = PhamViMonHoc.BitPhan(6, 1) | PhamViMonHoc.BitPhan(7, 2),
            HocPhiDeXuat = 100000, DonViHocPhi = "VND/Gio"
        });
        await db.SaveChangesAsync();
        var current = Current(db, seed.Tutor);
        var files = new TepService(db, current, new QuyenLopHocService(db, current), new LocalEnv());
        var service = new HoSoGiaSuService(db, current, new MonHocCatalog(db), files, new ThongBaoService(db, current));
        var input = await service.LayDeSuaAsync();
        var restored = Assert.Single(input.MonDay, value => value.MonHocId == subject.Id);
        Assert.Equal(new[] { 6, 7 }, restored.LopDay);
        Assert.Equal(new[] { 62, 73 }, restored.PhanTheoLop);
        Assert.Equal(new[] { 12 }, input.MonDay.Single(value => value.MonHocId == 1).LopDay);
    }

    private static NguoiDungHienTaiService Current(AppDbContext db, Guid? actor = null)
    {
        var context = new DefaultHttpContext();
        if (actor.HasValue)
            context.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, actor.Value.ToString())], "Test"));
        return new(db, new HttpContextAccessor { HttpContext = context });
    }

    [Fact]
    public async Task Need_rejects_chemistry_in_grade_two_and_persists_integrated_study_parts()
    {
        await using var db = ProposalRegressionTests.Open();
        var seed = await ProposalRegressionTests.Seed(db);
        var service = new NhuCauService(db, Current(db, seed.Learner));
        Assert.Contains(await service.MonAsync(), x => x.TenMon == PhamViMonHoc.KhoaHocTuNhien);
        var subjects = await db.Set<MonHoc>().ToDictionaryAsync(x => x.TenMon, x => x.Id);
        var input = new TaoNhuCauBenHocViewModel { MonHocId = subjects["Hóa Học"], KhoiLop = "Lớp 2", MucTieu = "Ôn tập kiến thức", SoBuoiTuan = 1 };
        var invalid = await service.TaoNhuCauHocAsync(input, seed.Learner, default);
        Assert.False(invalid.ThanhCong);
        Assert.Contains("10–12", invalid.ThongBao);

        input.MonHocId = subjects[PhamViMonHoc.KhoaHocTuNhien];
        input.KhoiLop = "Lớp 6";
        input.PhanKhoaHoc = 2;
        var saved = await service.TaoNhuCauHocAsync(input, seed.Learner, default);
        Assert.True(saved.ThanhCong, saved.ThongBao);
        var stored = await db.Set<NhuCauHoc>().SingleAsync(x => x.Id == saved.DuLieu);
        Assert.Equal((byte)2, stored.PhanKhoaHoc);
        Assert.Single(await db.Set<NhuCauMonHoc>().Where(x => x.NhuCauId == stored.Id).ToListAsync());

        var search = new TimKiemService(db, Current(db));
        var matching = await search.TimKiemAsync(new() { LoaiDoiTuong = "NhuCau", MonHocId = input.MonHocId, KhoiLop = "Lớp 6", PhanKhoaHoc = 2 }, null, default);
        var wrong = await search.TimKiemAsync(new() { LoaiDoiTuong = "NhuCau", MonHocId = input.MonHocId, KhoiLop = "Lớp 6", PhanKhoaHoc = 1 }, null, default);
        Assert.Contains(matching.DuLieu.Items, x => x.Id == stored.Id);
        Assert.DoesNotContain(wrong.DuLieu.Items, x => x.Id == stored.Id);
    }

    [Fact]
    public async Task Search_distinguishes_chemistry_offered_at_different_grades_in_one_profile()
    {
        await using var db = ProposalRegressionTests.Open();
        var seed = await ProposalRegressionTests.Seed(db);
        var profile = await db.Set<HoSoGiaSu>().SingleAsync(h => h.GiaSuId == seed.Tutor);
        profile.DuyetLuc = DateTimeOffset.UtcNow;
        var subjects = await db.Set<MonHoc>().ToDictionaryAsync(x => x.TenMon, x => x.Id);
        var scienceId = subjects[PhamViMonHoc.KhoaHocTuNhien];
        db.Add(new HoSoGiaSuMonDay
        {
            HoSoGiaSuId = profile.Id, MonHocId = scienceId, PhamViDay = "Lớp 6, Lớp 8",
            MatNaKhoi = PhamViMonHoc.BitKhoi(6) | PhamViMonHoc.BitKhoi(8),
            PhanKhoaHoc = PhamViMonHoc.BitPhan(6, 1) | PhamViMonHoc.BitPhan(8, 2),
            HocPhiDeXuat = 150000, DonViHocPhi = "VND/Gio"
        });
        db.Add(new HoSoGiaSuMonDay
        {
            HoSoGiaSuId = profile.Id, MonHocId = subjects["Hóa Học"], PhamViDay = "Lớp 10",
            MatNaKhoi = PhamViMonHoc.BitKhoi(10), HocPhiDeXuat = 180000, DonViHocPhi = "VND/Gio"
        });
        await db.SaveChangesAsync();

        var svc = new TimKiemService(db, Current(db));
        async Task<bool> Finds(string keyword)
        {
            var result = await svc.TimKiemAsync(new BoLocTimKiemViewModel { TuKhoa = keyword, SapXep = "HocPhiTang" }, null, default);
            Assert.True(result.ThanhCong, result.ThongBao);
            return result.DuLieu.Items.Any(x => x.Id == profile.Id);
        }
        Assert.True(await Finds("Hóa lớp 6"));
        Assert.False(await Finds("Hóa lớp 8"));
        Assert.True(await Finds("Sinh lớp 8"));
        Assert.True(await Finds("Hóa lớp 10"));
    }
}
