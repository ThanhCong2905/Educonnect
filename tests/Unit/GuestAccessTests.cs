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
