using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using Xunit;

public sealed class ProfileInputTests
{
    [Theory]
    [InlineData(99, false)]
    [InlineData(100, true)]
    [InlineData(4000, true)]
    [InlineData(4001, false)]
    public void Introduction_length_matches_profile_contract(int length, bool valid)
    {
        var input = new HoSoInput { HoTen = "Gia sư", TrinhDo = "Đại học", KinhNghiem = "Chưa có", KhuVuc = "Hà Nội", MoTa = new string('a', length) };
        var errors = new List<ValidationResult>();
        Assert.Equal(valid, Validator.TryValidateObject(input, new ValidationContext(input), errors, true));
        if (!valid) Assert.Contains(errors, error => error.MemberNames.Contains(nameof(input.MoTa)));
    }

    [Theory]
    [InlineData("Lớp 1", true)]
    [InlineData("Lớp 1, Lớp 12", true)]
    [InlineData("Lớp 0", false)]
    [InlineData("Lớp 13", false)]
    [InlineData("Lớp 1, Lớp 1", false)]
    [InlineData("Tất cả", false)]
    [InlineData("", false)]
    public void Grade_list_accepts_only_distinct_supported_grades(string value, bool valid)
        => Assert.Equal(valid, HoSoLuaChon.PhamViHopLe(value));

    [Fact]
    public void Profile_catalogs_have_no_duplicates()
    {
        Assert.Equal(34, HoSoLuaChon.KhuVuc.Distinct().Count());
        Assert.Equal(5, HoSoLuaChon.TrinhDo.Distinct().Count());
        Assert.Equal(5, HoSoLuaChon.KinhNghiem.Distinct().Count());
    }
}
