using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Infrastructure;
using Xunit;

namespace EduConnect.UnitTests.MT1;

public class PhienBanTests
{
    [Fact]
    [Trait("TestCaseId", "MT1-UT-026")]
    public void MT1_UT_026()
    {
        string? value = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

        Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, Rules.Version(value));
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-027")]
    public void MT1_UT_027()
    {
        string? value = null;

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.Version(value));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Phiên bản không hợp lệ.", exception.Message);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-028")]
    public void MT1_UT_028()
    {
        string? value = "not-base64!";

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.Version(value));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Phiên bản không hợp lệ.", exception.Message);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-029")]
    public void MT1_UT_029()
    {
        string? value = Convert.ToBase64String(new byte[7]);

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.Version(value));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Phiên bản không hợp lệ.", exception.Message);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-030")]
    public void MT1_UT_030()
    {
        string? value = "";

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.Version(value));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Phiên bản không hợp lệ.", exception.Message);
    }


}
