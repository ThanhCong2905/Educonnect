using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Infrastructure;
using Xunit;

namespace EduConnect.UnitTests.MT1;

public class HocPhiTests
{
    [Fact]
    [Trait("TestCaseId", "MT1-UT-014")]
    public void MT1_UT_014()
    {
        decimal? value = null;
        string? unit = null;

        Assert.Null(Record.Exception(() => Rules.FeeCheck(value, unit)));
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-015")]
    public void MT1_UT_015()
    {
        decimal? value = 150000m;
        string? unit = null;

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.FeeCheck(value, unit));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Nhập đồng thời học phí và đơn vị.", exception.Message);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-016")]
    public void MT1_UT_016()
    {
        decimal? value = null;
        string? unit = "VND/Gio";

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.FeeCheck(value, unit));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Nhập đồng thời học phí và đơn vị.", exception.Message);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-017")]
    public void MT1_UT_017()
    {
        decimal? value = -0.01m;
        string? unit = "VND/Gio";

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.FeeCheck(value, unit));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Học phí không âm, tối đa hai chữ số thập phân; chọn đúng đơn vị.", exception.Message);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-018")]
    public void MT1_UT_018()
    {
        decimal? value = 150000.001m;
        string? unit = "VND/Gio";

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.FeeCheck(value, unit));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Học phí không âm, tối đa hai chữ số thập phân; chọn đúng đơn vị.", exception.Message);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-019")]
    public void MT1_UT_019()
    {
        decimal? value = 150000m;
        string? unit = "USD/Gio";

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.FeeCheck(value, unit));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Học phí không âm, tối đa hai chữ số thập phân; chọn đúng đơn vị.", exception.Message);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-020")]
    public void MT1_UT_020()
    {
        decimal? value = 150000.25m;
        string? unit = "VND/Gio";

        Assert.Null(Record.Exception(() => Rules.FeeCheck(value, unit)));
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-021")]
    public void MT1_UT_021()
    {
        decimal? value = 0m;
        string? unit = "VND/Gio";

        Assert.Null(Record.Exception(() => Rules.FeeCheck(value, unit)));
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-022")]
    public void MT1_UT_022()
    {
        decimal? value = 9999999999999999.99m;
        string? unit = "VND/Gio";

        Assert.Null(Record.Exception(() => Rules.FeeCheck(value, unit)));
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-023")]
    public void MT1_UT_023()
    {
        decimal? value = 10000000000000000m;
        string? unit = "VND/Gio";

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.FeeCheck(value, unit));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Học phí không âm, tối đa hai chữ số thập phân; chọn đúng đơn vị.", exception.Message);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-024")]
    public void MT1_UT_024()
    {
        decimal? value = null;
        string? unit = "";

        Assert.Null(Record.Exception(() => Rules.FeeCheck(value, unit)));
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-025")]
    public void MT1_UT_025()
    {
        decimal? value = 150000m;
        string? unit = " ";

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.FeeCheck(value, unit));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Học phí không âm, tối đa hai chữ số thập phân; chọn đúng đơn vị.", exception.Message);
    }


}
