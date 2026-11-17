using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Infrastructure;
using Xunit;

public sealed class FieldValidationTests
{
    [Theory]
    [InlineData(1, true)]
    [InlineData(2000, true)]
    [InlineData(2001, false)]
    public void Goal_length_is_shared_by_need_and_proposal(int length, bool valid)
    {
        var goal = new string('a', length);
        Assert.Equal(valid, IsValidProperty(new NhuCauInput(), nameof(NhuCauInput.MucTieu), goal));
        Assert.Equal(valid, IsValidProperty(new DeXuatLopViewModel(), nameof(DeXuatLopViewModel.MucTieu), goal));
    }

    [Theory]
    [InlineData(500, true)]
    [InlineData(501, false)]
    public void Detailed_address_has_the_same_limit(int length, bool valid)
    {
        var address = new string('a', length);
        Assert.Equal(valid, IsValidProperty(new NhuCauInput(), nameof(NhuCauInput.DiaChiChiTiet), address));
        Assert.Equal(valid, IsValidProperty(new DeXuatLopViewModel(), nameof(DeXuatLopViewModel.DiaDiem), address));
    }

    [Fact]
    public async Task Generic_result_preserves_field_and_status()
    {
        var original = new LoiNghiepVu("Nhập mục tiêu học.", 400) { Truong = "MucTieu" };
        var result = await KetQua<int>.ThucHienAsync(() => Task.FromException<int>(original));
        Assert.Equal("MucTieu", result.Truong);
        Assert.Same(original, Assert.Throws<LoiNghiepVu>(result.BaoLoiNeuThatBai));
    }

    [Fact]
    public async Task Result_without_data_preserves_field_and_status()
    {
        var original = new LoiNghiepVu("Chọn tệp minh chứng.", 409) { Truong = "MinhChungTrinhDoId" };
        var result = await KetQua.ThucHienAsync(() => Task.FromException(original));
        Assert.Same(original, Assert.Throws<LoiNghiepVu>(result.BaoLoiNeuThatBai));
    }

    private static bool IsValidProperty(object model, string member, string value)
        => Validator.TryValidateProperty(value, new ValidationContext(model) { MemberName = member }, []);
}
