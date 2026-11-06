using EduConnect.Mvc.Areas.TimKiem.Controllers;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Infrastructure;
using Xunit;

namespace EduConnect.UnitTests.MT1;

public class TimKiemControllerTests
{
    [Fact]
    [Trait("TestCaseId", "MT1-UT-049")]
    public async Task MT1_UT_049_ModelStateSai_DungTruocKhiDungPhuThuoc()
    {
        // Null là sentinel: nhánh lỗi phải dừng trước khi truy cập phụ thuộc.
        var controller = new TimKiemController(null!, null!, null!, null!);
        var filter = new BoLocTimKiemViewModel { Trang = 1 };
        controller.ModelState.AddModelError("TuKhoa", "Quá dài");

        var exception = await Assert.ThrowsAsync<LoiNghiepVu>(
            () => controller.Index(filter));

        Assert.Equal(400, exception.Status);
        Assert.Contains("Quá dài", exception.Message);
    }
}
