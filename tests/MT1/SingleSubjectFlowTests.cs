using System.Security.Claims;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed class SingleSubjectFlowTests
{
    [Theory]
    [InlineData(19, 30, false)]
    [InlineData(20, 0, true)]
    public async Task Open_needs_reject_overlap_but_allow_touching_boundaries(int hour, int minute, bool allowed)
    {
        await using var db = ProposalRegressionTests.Open();
        var seed = await ProposalRegressionTests.Seed(db);
        var http = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, seed.Learner.ToString())], "Test")) };
        var service = new NhuCauService(db, new NguoiDungHienTaiService(db, new HttpContextAccessor { HttpContext = http }));
        var first = await service.TaoNhuCauHocAsync(Input(19, 0, "Ôn tập môn học"), seed.Learner, default);
        Assert.True(first.ThanhCong, first.ThongBao);
        var second = await service.TaoNhuCauHocAsync(Input(hour, minute, "Củng cố kiến thức"), seed.Learner, default);
        Assert.Equal(allowed, second.ThanhCong);
        if (!allowed) Assert.Contains("trùng", second.ThongBao);
        Assert.Equal(allowed ? 2 : 1, await db.Set<NhuCauHoc>().CountAsync(n => n.ChuSoHuuId == seed.Learner));
    }

    [Fact]
    public async Task Scalar_subject_survives_conversion_and_multiple_subjects_are_rejected()
    {
        Assert.Equal(2, TaoNhuCauBenHocViewModel.FromInput(new NhuCauInput { MonHocId = 2 }).ToInput().MonHocId);
        await using var db = ProposalRegressionTests.Open();
        var seed = await ProposalRegressionTests.Seed(db);
        var http = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, seed.Learner.ToString())], "Test")) };
        var service = new NhuCauService(db, new NguoiDungHienTaiService(db, new HttpContextAccessor { HttpContext = http }));
        var input = Input(19, 0, "Ôn tập");
        input.MonHocIds = [1, 2];
        var result = await service.TaoNhuCauHocAsync(input, seed.Learner, default);
        Assert.False(result.ThanhCong);
        Assert.Contains("một môn", result.ThongBao);
        Assert.False(await db.Set<NhuCauHoc>().AnyAsync(n => n.ChuSoHuuId == seed.Learner));
    }

    [Fact]
    public async Task Editing_excludes_own_schedule_and_reopening_checks_other_open_needs()
    {
        await using var db = ProposalRegressionTests.Open();
        var seed = await ProposalRegressionTests.Seed(db);
        var http = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, seed.Learner.ToString())], "Test")) };
        var service = new NhuCauService(db, new NguoiDungHienTaiService(db, new HttpContextAccessor { HttpContext = http }));
        var created = await service.TaoNhuCauHocAsync(Input(19, 0, "Ôn tập ban đầu"), seed.Learner, default);
        Assert.True(created.ThanhCong, created.ThongBao);
        var edit = TaoNhuCauBenHocViewModel.FromInput((await service.LayDeSuaAsync(created.DuLieu)).DuLieu);
        edit.MonHocIds = [2];
        edit.MonHocId = 2;
        edit.KhoiLop = "Lớp 12";
        var updated = await service.CapNhatAsync(created.DuLieu, edit, edit.RowVersion!, seed.Learner, default);
        Assert.True(updated.ThanhCong, updated.ThongBao);
        Assert.Equal(2, (await db.Set<NhuCauMonHoc>().SingleAsync(n => n.NhuCauId == created.DuLieu)).MonHocId);
        var stored = await db.Set<NhuCauHoc>().SingleAsync(n => n.Id == created.DuLieu);
        await service.TrangThaiAsync(stored.Id, "DaDong", Convert.ToBase64String(stored.RowVersion));
        var second = await service.TaoNhuCauHocAsync(Input(19, 0, "Nhu cầu học tiếp theo"), seed.Learner, default);
        Assert.True(second.ThanhCong, second.ThongBao);
        var error = await Assert.ThrowsAsync<LoiNghiepVu>(() => service.TrangThaiAsync(stored.Id, "DangMo", Convert.ToBase64String(stored.RowVersion)));
        Assert.Contains("trùng", error.Message);
    }

    private static TaoNhuCauBenHocViewModel Input(int hour, int minute, string goal) => new()
    {
        MonHocId = 1, MucTieu = goal, SoBuoiTuan = 1,
        LichRanh = [new SlotInput { ThuTrongTuan = 1, GioBatDau = new(hour, minute), GioKetThuc = new(hour + 1, minute) }]
    };
}
