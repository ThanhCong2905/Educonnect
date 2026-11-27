using System.Security.Claims;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed class ComparisonMatchTests
{
    [Fact]
    public async Task Needs_rating_sort_is_available_only_to_tutors()
    {
        await using var db = ProposalRegressionTests.Open();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var people = await ProposalRegressionTests.Seed(db);
        var tutor = await Search(db, people.Tutor).TimKiemAsync(new() { LoaiDoiTuong = "NhuCau", SapXep = "DanhGia", TuKhoa = Guid.NewGuid().ToString("N") }, people.Tutor, default);
        Assert.True(tutor.ThanhCong, tutor.ThongBao);
        var guest = await Search(db, null).TimKiemAsync(new() { LoaiDoiTuong = "NhuCau", SapXep = "DanhGia" }, null, default);
        Assert.False(guest.ThanhCong);
        var learnerCount = await Search(db, people.Tutor).TimKiemAsync(new() { LoaiDoiTuong = "NhuCau", SapXep = "HocVien" }, people.Tutor, default);
        Assert.False(learnerCount.ThanhCong);
    }

    private static TimKiemService Search(AppDbContext db, Guid? actor)
    {
        var context = new DefaultHttpContext();
        if (actor.HasValue)
            context.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, actor.ToString()!)], "Test"));
        return new(db, new NguoiDungHienTaiService(db, new HttpContextAccessor { HttpContext = context }));
    }

    [Fact]
    public async Task Comparison_uses_the_viewers_open_needs_and_preserves_selection_order()
    {
        await using var db = ProposalRegressionTests.Open();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var first = await ProposalRegressionTests.Seed(db);
        var second = await ProposalRegressionTests.Seed(db);
        var profiles = await db.Set<HoSoGiaSu>().Where(p => p.GiaSuId == first.Tutor || p.GiaSuId == second.Tutor).ToListAsync();
        var firstProfile = profiles.Single(p => p.GiaSuId == first.Tutor).Id;
        var secondProfile = profiles.Single(p => p.GiaSuId == second.Tutor).Id;
        var subject = await db.Set<HoSoGiaSuMonDay>().SingleAsync(s => s.HoSoGiaSuId == secondProfile);
        subject.PhamViDay = "Lớp 10";
        subject.MatNaKhoi = PhamViMonHoc.BitKhoi(10);
        var need = new NhuCauHoc { ChuSoHuuId = first.Learner, MonHocId = 1, LoaiNhuCau = "TimGiaSu", TrangThai = "DangMo", KhoiLop = "Lớp 12", HinhThucHoc = "TrucTuyen", MucTieu = "Ôn thi cuối cấp" };
        db.Add(need);
        await db.SaveChangesAsync();

        var result = await Search(db, first.Learner).SoSanhAsync([secondProfile, firstProfile], first.Learner, default);
        Assert.True(result.ThanhCong, result.ThongBao);
        Assert.Equal(new[] { secondProfile, firstProfile }, result.DuLieu.Items.Select(item => item.Id));
        Assert.Equal(new[] { 1, 2 }, result.DuLieu.Items.Select(item => item.MucPhuHop));
        Assert.Contains("Trùng môn và khối lớp", result.DuLieu.Items[1].LyDo);

        var stranger = await Search(db, second.Learner).SoSanhAsync([secondProfile, firstProfile], second.Learner, default);
        Assert.All(stranger.DuLieu.Items, item => { Assert.Equal(0, item.MucPhuHop); Assert.Empty(item.LyDo); });
        var guest = await Search(db, null).SoSanhAsync([secondProfile, firstProfile], null, default);
        Assert.All(guest.DuLieu.Items, item => Assert.Equal(0, item.MucPhuHop));

        need.TrangThai = "DaDong";
        await db.SaveChangesAsync();
        var closed = await Search(db, first.Learner).SoSanhAsync([secondProfile, firstProfile], first.Learner, default);
        Assert.All(closed.DuLieu.Items, item => Assert.Equal(0, item.MucPhuHop));
    }
}
