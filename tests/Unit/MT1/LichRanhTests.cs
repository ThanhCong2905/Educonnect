using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Infrastructure;
using Xunit;

namespace EduConnect.UnitTests.MT1;

public class LichRanhTests
{
    [Fact]
    [Trait("TestCaseId", "MT1-UT-003")]
    public void MT1_UT_003()
    {
        var slots = new List<SlotInput>
        {
            new SlotInput { ThuTrongTuan = 7, GioBatDau = TimeOnly.Parse("19:00"), GioKetThuc = TimeOnly.Parse("20:00") }
        };

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.Slots(slots));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Thứ hoặc giờ của khung giờ không hợp lệ.", exception.Message);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-004")]
    public void MT1_UT_004()
    {
        var slots = new List<SlotInput>
        {
            new SlotInput { ThuTrongTuan = 1, GioBatDau = TimeOnly.Parse("19:00"), GioKetThuc = TimeOnly.Parse("19:00") }
        };

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.Slots(slots));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Thứ hoặc giờ của khung giờ không hợp lệ.", exception.Message);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-005")]
    public void MT1_UT_005()
    {
        var slots = new List<SlotInput>
        {
            new SlotInput { ThuTrongTuan = 1, GioBatDau = TimeOnly.Parse("20:00"), GioKetThuc = TimeOnly.Parse("19:00") }
        };

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.Slots(slots));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Thứ hoặc giờ của khung giờ không hợp lệ.", exception.Message);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-006")]
    public void MT1_UT_006()
    {
        var slots = new List<SlotInput>
        {
            new SlotInput { ThuTrongTuan = 1, GioBatDau = TimeOnly.Parse("18:00"), GioKetThuc = TimeOnly.Parse("19:30") },
            new SlotInput { ThuTrongTuan = 1, GioBatDau = TimeOnly.Parse("19:00"), GioKetThuc = TimeOnly.Parse("20:00") }
        };

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.Slots(slots));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Các khung giờ cùng ngày không được chồng lấn.", exception.Message);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-007")]
    public void MT1_UT_007()
    {
        var slots = new List<SlotInput>
        {
            new SlotInput { ThuTrongTuan = 1, GioBatDau = TimeOnly.Parse("18:00"), GioKetThuc = TimeOnly.Parse("19:00") },
            new SlotInput { ThuTrongTuan = 1, GioBatDau = TimeOnly.Parse("19:00"), GioKetThuc = TimeOnly.Parse("20:00") }
        };

        Assert.Null(Record.Exception(() => Rules.Slots(slots)));
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-008")]
    public void MT1_UT_008()
    {
        var slots = new List<SlotInput>
        {
            new SlotInput { ThuTrongTuan = 1, GioBatDau = TimeOnly.Parse("18:00"), GioKetThuc = TimeOnly.Parse("19:00") },
            new SlotInput { ThuTrongTuan = 2, GioBatDau = TimeOnly.Parse("18:00"), GioKetThuc = TimeOnly.Parse("19:00") }
        };

        Assert.Null(Record.Exception(() => Rules.Slots(slots)));
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-009")]
    public void MT1_UT_009()
    {
        var slots = new List<SlotInput>
        {
            new SlotInput { ThuTrongTuan = 0, GioBatDau = TimeOnly.Parse("00:00"), GioKetThuc = TimeOnly.Parse("01:00") }
        };

        Assert.Null(Record.Exception(() => Rules.Slots(slots)));
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-010")]
    public void MT1_UT_010()
    {
        var slots = new List<SlotInput>();
        for (byte day = 0; day <= 6; day++)
        {
            for (var hour = 1; hour <= 5; hour++)
            {
                slots.Add(new SlotInput { ThuTrongTuan = day,
                    GioBatDau = new TimeOnly(hour, 0),
                    GioKetThuc = new TimeOnly(hour + 1, 0) });
            }
        }
        Assert.Equal(35, slots.Count);

        Assert.Null(Record.Exception(() => Rules.Slots(slots)));
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-011")]
    public void MT1_UT_011()
    {
        var slots = new List<SlotInput>
        {
            new SlotInput { ThuTrongTuan = 6, GioBatDau = TimeOnly.Parse("22:00"), GioKetThuc = TimeOnly.Parse("23:00") }
        };

        Assert.Null(Record.Exception(() => Rules.Slots(slots)));
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-012")]
    public void MT1_UT_012()
    {
        var slots = new List<SlotInput>
        {
            new SlotInput { ThuTrongTuan = 1, GioBatDau = TimeOnly.Parse("18:00"), GioKetThuc = TimeOnly.Parse("19:00") },
            new SlotInput { ThuTrongTuan = 1, GioBatDau = TimeOnly.Parse("18:00"), GioKetThuc = TimeOnly.Parse("19:00") }
        };

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.Slots(slots));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Các khung giờ cùng ngày không được chồng lấn.", exception.Message);
    }

    [Fact]
    [Trait("TestCaseId", "MT1-UT-013")]
    public void MT1_UT_013()
    {
        var slots = new List<SlotInput>
        {
            new SlotInput { ThuTrongTuan = 1, GioBatDau = TimeOnly.Parse("18:00"), GioKetThuc = TimeOnly.Parse("21:00") },
            new SlotInput { ThuTrongTuan = 1, GioBatDau = TimeOnly.Parse("19:00"), GioKetThuc = TimeOnly.Parse("20:00") }
        };

        var exception = Assert.Throws<LoiNghiepVu>(() => Rules.Slots(slots));
        Assert.Equal(400, exception.Status);
        Assert.Equal("Các khung giờ cùng ngày không được chồng lấn.", exception.Message);
    }


}
