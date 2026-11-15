using System.Security.Claims;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static ProposalRegressionTests;

public sealed class AdministrativeAreaTests
{
    private static NguoiDungHienTaiService Current(AppDbContext db, Guid id)
        => new(db, new HttpContextAccessor { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, id.ToString())], "Test")) } });

    [Fact]
    public void Catalog_covers_all_provinces_without_duplicate_wards()
    {
        Assert.Equal(34, HoSoLuaChon.KhuVuc.Length);
        Assert.Equal(3321, HoSoLuaChon.KhuVuc.Sum(province => DiaGioiHanhChinh.PhuongXa(province).Count));
        Assert.All(HoSoLuaChon.KhuVuc, province =>
        {
            var wards = DiaGioiHanhChinh.PhuongXa(province);
            Assert.NotEmpty(wards);
            Assert.Equal(wards.Count, wards.Distinct().Count());
        });
    }

    [Theory]
    [InlineData("Hà Nội", "Phường Ba Đình", true)]
    [InlineData("Hồ Chí Minh", "Phường Ba Đình", false)]
    [InlineData("Hà Nội", "Quận Ba Đình", false)]
    [InlineData(null, "Phường Ba Đình", false)]
    [InlineData("Hà Nội", null, true)]
    public void Ward_must_belong_to_the_selected_province(string? province, string? ward, bool expected)
        => Assert.Equal(expected, DiaGioiHanhChinh.HopLe(province, ward));

    [Fact]
    public void Common_subjects_come_before_other_subjects()
    {
        var names = new[] { "Hóa Học", "Tiếng Anh", "Ngữ Văn", "Toán Học" };
        Assert.Equal(new[] { "Toán Học", "Ngữ Văn", "Tiếng Anh", "Hóa Học" }, names.OrderBy(MonHocCatalog.ThuTuHienThi));
        Assert.Equal(MonHocCatalog.ThuTuHienThi("Ngữ Văn"), MonHocCatalog.ThuTuHienThi("Tiếng Việt"));
    }

    [Fact]
    public async Task Need_roundtrip_and_search_keep_ward_without_exposing_private_address()
    {
        await using var db = Open();
        var seed = await Seed(db);
        var current = Current(db, seed.Learner);
        var needs = new NhuCauService(db, current, new MonHocCatalog(db));
        var input = new TaoNhuCauBenHocViewModel
        {
            MonHocIds = [1], MucTieu = "Ôn tập khu vực " + seed.Learner, HinhThucHoc = "TrucTiep",
            KhuVuc = "Hà Nội", PhuongXa = "Phường Ba Đình", DiaChiChiTiet = "Địa chỉ riêng tư 123"
        };
        var result = await needs.TaoNhuCauHocAsync(input, seed.Learner, default);
        Assert.True(result.ThanhCong, result.ThongBao);
        var stored = (await needs.LayDeSuaAsync(result.DuLieu)).DuLieu;
        Assert.Equal(input.PhuongXa, stored.PhuongXa);
        Assert.Equal(input.DiaChiChiTiet, stored.DiaChiChiTiet);
        Assert.Equal(stored.PhuongXa, TaoNhuCauBenHocViewModel.FromInput(stored).ToInput().PhuongXa);
        var search = new TimKiemService(db, Current(db, seed.Tutor));
        var found = await search.TimKiemAsync(new() { KhuVuc = "Hà Nội", PhuongXa = "Phường Ba Đình", TuKhoa = input.MucTieu }, seed.Tutor, default);
        Assert.Equal(result.DuLieu, Assert.Single(found.DuLieu.Items).Id);
        Assert.DoesNotContain(input.DiaChiChiTiet, System.Text.Json.JsonSerializer.Serialize(found.DuLieu.Items));
        var otherWard = await search.TimKiemAsync(new() { KhuVuc = "Hà Nội", PhuongXa = "Phường Ngọc Hà", TuKhoa = input.MucTieu }, seed.Tutor, default);
        Assert.Empty(otherWard.DuLieu.Items);
    }

    [Fact]
    public async Task Invalid_ward_returns_field_error_without_saving()
    {
        await using var db = Open();
        var seed = await Seed(db);
        var needs = new NhuCauService(db, Current(db, seed.Learner), new MonHocCatalog(db));
        var result = await needs.TaoNhuCauHocAsync(new()
        {
            MonHocIds = [1], MucTieu = "Ôn tập", HinhThucHoc = "TrucTiep", KhuVuc = "Hồ Chí Minh", PhuongXa = "Phường Ba Đình"
        }, seed.Learner, default);
        Assert.False(result.ThanhCong);
        Assert.Equal("PhuongXa", result.Truong);
        Assert.False(await db.Set<NhuCauHoc>().AnyAsync(x => x.ChuSoHuuId == seed.Learner));
    }
}
