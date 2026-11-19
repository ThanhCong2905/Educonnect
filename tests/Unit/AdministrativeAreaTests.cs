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

public sealed class AdministrativeAreaTests
{
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
}
