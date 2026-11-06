using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Infrastructure;
using Xunit;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
namespace EduConnect.UnitTests.MT1;

public class InputValidationTests
{
    [Fact]
    [Trait("TestCaseId", "MT1-UT-031")]
    public void MT1_UT_031()
    {
        var model = new NhuCauInput { SoBuoiTuan = 0 };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.False(valid);
        var error = Assert.Single(errors);
        Assert.Contains("SoBuoiTuan", error.MemberNames);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-032")]
    public void MT1_UT_032()
    {
        var model = new NhuCauInput { SoBuoiTuan = 1 };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.True(valid);
        Assert.Empty(errors);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-033")]
    public void MT1_UT_033()
    {
        var model = new NhuCauInput { SoBuoiTuan = 7 };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.True(valid);
        Assert.Empty(errors);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-034")]
    public void MT1_UT_034()
    {
        var model = new NhuCauInput { SoBuoiTuan = 8 };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.False(valid);
        var error = Assert.Single(errors);
        Assert.Contains("SoBuoiTuan", error.MemberNames);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-035")]
    public void MT1_UT_035()
    {
        var model = new NhuCauInput { HinhThucHoc = "Khac" };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.False(valid);
        var error = Assert.Single(errors);
        Assert.Contains("HinhThucHoc", error.MemberNames);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-036")]
    public void MT1_UT_036()
    {
        var model = new NhuCauInput { HinhThucHoc = "" };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.False(valid);
        var error = Assert.Single(errors);
        Assert.Contains("HinhThucHoc", error.MemberNames);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-037")]
    public void MT1_UT_037()
    {
        var model = new NhuCauInput { MucTieu = new string('A', 256) };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.True(valid);
        Assert.Empty(errors);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-038")]
    public void MT1_UT_038()
    {
        var model = new NhuCauInput { MucTieu = new string('A', 257) };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.False(valid);
        var error = Assert.Single(errors);
        Assert.Contains("MucTieu", error.MemberNames);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-039")]
    public void MT1_UT_039()
    {
        var model = new NhuCauInput { TuoiNguoiHoc = 0 };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.False(valid);
        var error = Assert.Single(errors);
        Assert.Contains("TuoiNguoiHoc", error.MemberNames);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-040")]
    public void MT1_UT_040()
    {
        var model = new NhuCauInput { TuoiNguoiHoc = 1 };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.True(valid);
        Assert.Empty(errors);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-041")]
    public void MT1_UT_041()
    {
        var model = new NhuCauInput { TuoiNguoiHoc = 120 };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.True(valid);
        Assert.Empty(errors);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-042")]
    public void MT1_UT_042()
    {
        var model = new NhuCauInput { TuoiNguoiHoc = 121 };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.False(valid);
        var error = Assert.Single(errors);
        Assert.Contains("TuoiNguoiHoc", error.MemberNames);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-043")]
    public void MT1_UT_043()
    {
        var model = new NhuCauInput { TenMonKhac = new string('A', 101) };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.False(valid);
        var error = Assert.Single(errors);
        Assert.Contains("TenMonKhac", error.MemberNames);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-044")]
    public void MT1_UT_044()
    {
        var model = new HoSoInput { HoTen = "", TrinhDo = "Đại học", ChuyenMon = "Toán", KinhNghiem = "2 năm", MoTa = "Dạy Toán", KhuVuc = "Thủ Đức" };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.False(valid);
        var error = Assert.Single(errors);
        Assert.Contains("HoTen", error.MemberNames);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-045")]
    public void MT1_UT_045()
    {
        var model = new BoLocTimKiemViewModel { TuKhoa = new string('A', 100) };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.True(valid);
        Assert.Empty(errors);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-046")]
    public void MT1_UT_046()
    {
        var model = new BoLocTimKiemViewModel { TuKhoa = new string('A', 101) };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.False(valid);
        var error = Assert.Single(errors);
        Assert.Contains("TuKhoa", error.MemberNames);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-068")]
    public void MT1_UT_068()
    {
        var model = new GuiLienHeInput { DoiTuongId = Guid.Parse("00000100-0000-0000-0000-000000000003"), LoiNhan = new string('A', 0) };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.False(valid);
        var error = Assert.Single(errors);
        Assert.Contains("LoiNhan", error.MemberNames);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-069")]
    public void MT1_UT_069()
    {
        var model = new GuiLienHeInput { DoiTuongId = Guid.Parse("00000100-0000-0000-0000-000000000003"), LoiNhan = new string('A', 256) };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.True(valid);
        Assert.Empty(errors);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-070")]
    public void MT1_UT_070()
    {
        var model = new GuiLienHeInput { DoiTuongId = Guid.Parse("00000100-0000-0000-0000-000000000003"), LoiNhan = new string('A', 257) };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.False(valid);
        var error = Assert.Single(errors);
        Assert.Contains("LoiNhan", error.MemberNames);
    }


}
