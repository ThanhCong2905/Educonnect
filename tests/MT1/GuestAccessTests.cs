using System.Security.Claims;
using EduConnect.Mvc.Areas.DanhGia.Services;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.TimKiem.Controllers;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed class GuestAccessTests
{
    private static NguoiDungHienTaiService Reader(EduConnect.Mvc.Data.AppDbContext db, Guid? actor = null)
    {
        var http = new DefaultHttpContext();
        if (actor.HasValue)
            http.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, actor.Value.ToString())], "Test"));
        return new(db, new HttpContextAccessor { HttpContext = http });
    }

    [Fact]
    public async Task Anonymous_reader_has_no_account_and_cannot_supply_an_actor_id()
    {
        var current = Reader(null!);
        Assert.Null(await current.NguoiXemAsync());
        var error = await Assert.ThrowsAsync<LoiNghiepVu>(() => current.NguoiXemAsync(Guid.NewGuid()));
        Assert.Equal(401, error.Status);
        Assert.Equal(401, (await Assert.ThrowsAsync<LoiNghiepVu>(() => current.HienTaiAsync())).Status);
    }

    [Theory]
    [InlineData("Public", 1)]
    [InlineData("Hidden", 0)]
    [InlineData("Draft", 0)]
    [InlineData("Locked", 0)]
    public async Task Guest_search_only_returns_public_approved_active_tutors(string state, int expected)
    {
        await using var db = ProposalRegressionTests.Open();
        await using var tx = await db.Database.BeginTransactionAsync();
        var seed = await ProposalRegressionTests.Seed(db);
        var tag = Guid.NewGuid().ToString("N");
        var user = await db.Users.SingleAsync(x => x.Id == seed.Tutor);
        user.HoTen = tag;
        var profile = await db.Set<HoSoGiaSu>().SingleAsync(x => x.GiaSuId == seed.Tutor);
        if (state == "Hidden") profile.DangHienThi = false;
        if (state == "Draft")
        {
            profile.DangHienThi = false;
            profile.TrangThaiDuyet = "BanNhap";
        }
        if (state == "Locked") user.TrangThai = "BiKhoa";
        await db.SaveChangesAsync();
        var search = new TimKiemService(db, Reader(db));
        var result = await search.TimKiemAsync(new() { TuKhoa = tag, LoaiDoiTuong = "GiaSu" }, null, default);
        Assert.True(result.ThanhCong);
        Assert.Equal(expected, result.DuLieu.Total);
        Assert.All(result.DuLieu.Items, item => Assert.Equal("GiaSu", item.LoaiNguoi));
    }

    [Fact]
    public async Task Needs_search_checks_the_actor_in_the_service()
    {
        await using var db = ProposalRegressionTests.Open();
        await using var tx = await db.Database.BeginTransactionAsync();
        var seed = await ProposalRegressionTests.Seed(db);
        var guest = new TimKiemService(db, Reader(db));
        Assert.True((await guest.TimKiemAsync(new() { LoaiDoiTuong = "NhuCau" }, null, default)).ThanhCong);
        Assert.Equal(401, (await guest.TimKiemAsync(new(), seed.Tutor, default)).MaTrangThai);
        var student = new TimKiemService(db, Reader(db, seed.Learner));
        Assert.Equal(403, (await student.TimKiemAsync(new() { LoaiDoiTuong = "NhuCau" }, seed.Learner, default)).MaTrangThai);
    }

    [Fact]
    public async Task Tutor_cannot_search_tutors()
    {
        await using var db = ProposalRegressionTests.Open();
        await using var tx = await db.Database.BeginTransactionAsync();
        var seed = await ProposalRegressionTests.Seed(db);
        var tag = Guid.NewGuid().ToString("N");
        (await db.Users.SingleAsync(x => x.Id == seed.Tutor)).HoTen = tag;
        await db.SaveChangesAsync();
        var search = new TimKiemService(db, Reader(db, seed.Tutor));
        var result = await search.TimKiemAsync(new() { LoaiDoiTuong = "GiaSu", TuKhoa = tag }, seed.Tutor, default);
        Assert.False(result.ThanhCong);
        Assert.Equal(403, result.MaTrangThai);
    }

    [Fact]
    public async Task Guest_can_compare_public_tutors_and_open_public_learning_needs()
    {
        await using var db = ProposalRegressionTests.Open();
        await using var tx = await db.Database.BeginTransactionAsync();
        var first = await ProposalRegressionTests.Seed(db);
        var second = await ProposalRegressionTests.Seed(db);
        var ids = await db.Set<HoSoGiaSu>().Where(x => x.GiaSuId == first.Tutor || x.GiaSuId == second.Tutor).Select(x => x.Id).ToListAsync();
        var search = new TimKiemService(db, Reader(db));
        var comparison = await search.SoSanhAsync(ids, null, default);
        Assert.True(comparison.ThanhCong);
        Assert.Equal(2, comparison.DuLieu.Items.Count);
        var need = new NhuCauHoc { Id = Guid.NewGuid(), ChuSoHuuId = first.Learner, MonHocId = 1,
            LoaiNhuCau = "TimGiaSu", TrangThai = "DangMo", HinhThucHoc = "TrucTuyen", MucTieu = "Ôn tập kiến thức" };
        db.Add(need);
        await db.SaveChangesAsync();
        Assert.Equal(need.Id, (await search.ChiTietAsync(need.Id)).Id);
        need.TrangThai = "DaDong";
        await db.SaveChangesAsync();
        Assert.Equal(404, (await Assert.ThrowsAsync<LoiNghiepVu>(() => search.ChiTietAsync(need.Id))).Status);
        Assert.False((await search.SoSanhAsync([ids[0], need.Id], null, default)).ThanhCong);
    }

    [Fact]
    public async Task Guest_reviews_are_limited_to_public_tutor_profiles()
    {
        await using var db = ProposalRegressionTests.Open();
        await using var tx = await db.Database.BeginTransactionAsync();
        var seed = await ProposalRegressionTests.Seed(db);
        var current = Reader(db);
        var reviews = new DanhGiaDoiTacService(db, current, new QuyenLopHocService(db, current), new ThongBaoService(db, current));
        Assert.Equal(seed.Tutor, (await reviews.LayAsync(seed.Tutor)).TaiKhoanId);
        Assert.Equal(404, (await Assert.ThrowsAsync<LoiNghiepVu>(() => reviews.LayAsync(seed.Learner))).Status);
        (await db.Set<HoSoGiaSu>().SingleAsync(x => x.GiaSuId == seed.Tutor)).DangHienThi = false;
        await db.SaveChangesAsync();
        Assert.Equal(404, (await Assert.ThrowsAsync<LoiNghiepVu>(() => reviews.LayAsync(seed.Tutor))).Status);
    }

    [Fact]
    public void Needs_endpoint_allows_public_read_access()
    {
        var action = typeof(TimKiemController).GetMethod(nameof(TimKiemController.NhuCau))!;
        Assert.Single(action.GetCustomAttributes(typeof(AllowAnonymousAttribute), true));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    public async Task Guest_comparison_rejects_invalid_selection_count(int count)
    {
        var service = new TimKiemService(null!, Reader(null!));
        var result = await service.SoSanhAsync(Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToList(), null, default);
        Assert.Equal(400, result.MaTrangThai);
    }
}
