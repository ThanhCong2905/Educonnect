using System.Collections.Generic;
using EduConnect.Mvc.Infrastructure;
using Xunit;

namespace EduConnect.UnitTests.MT1;

public class NhuCauRulesTests
{
    [Fact]
    [Trait("TestCaseId", "MT1-UT-002")]
    public void MT1_UT_002_LichRanhVuot35KhungGio_PhaiBaoLoi()
    {
        // Arrange: 6 ngày, mỗi ngày 6 khung giờ hợp lệ, liền kề và không chồng lấn.
        var slots = new List<SlotInput>();
        for (byte day = 1; day <= 6; day++)
        {
            for (var hour = 1; hour <= 6; hour++)
            {
                slots.Add(new SlotInput
                {
                    ThuTrongTuan = day,
                    GioBatDau = new TimeOnly(hour, 0),
                    GioKetThuc = new TimeOnly(hour + 1, 0)
                });
            }
        }

        Assert.Equal(36, slots.Count);

        // Act: chỉ vi phạm giới hạn số lượng khung giờ.
        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.Slots(slots));

        // Assert: kiểm tra đúng mã trạng thái và thông báo nghiệp vụ.
        Assert.Equal(400, exception.Status);
        Assert.Equal("Cần từ 1 đến 35 khung giờ.", exception.Message);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-001")]
    public void MT1_UT_001_LichRanhRong_PhaiBaoLoi()
    {
        // Arrange: chuẩn bị danh sách lịch rảnh rỗng.
        var slots = new List<SlotInput>();

        // Act: gọi hàm và thu lại ngoại lệ mong đợi.
        var exception = Assert.Throws<LoiNghiepVu>(
            () => Rules.Slots(slots));

        // Assert: kiểm tra thông tin lỗi.
        Assert.Equal(400, exception.Status);
        Assert.Equal(
            "Cần từ 1 đến 35 khung giờ.",
            exception.Message);
    }
}
