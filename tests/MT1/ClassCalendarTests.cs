using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Xunit;

public sealed class ClassCalendarTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 15, 18, 0, 0, TimeSpan.Zero);
    private static LichHoc Schedule(string start, string end, string state = "DaLenLich") => new()
    {
        Id = Guid.NewGuid(), BatDauLuc = DateTimeOffset.Parse(start), KetThucLuc = DateTimeOffset.Parse(end),
        TrangThai = state, TieuDe = "Buổi kiểm thử"
    };

    [Fact]
    public void Uses_Vietnam_date_and_Monday_as_week_start()
    {
        var page = LichTuanViewModel.Tao(new(), [], [], null, Now);
        Assert.Equal(new DateOnly(2026, 11, 16), page.HomNay);
        Assert.Equal(page.HomNay, page.TuNgay);
        Assert.Empty(page.SuKien);
    }

    [Theory]
    [InlineData(2026, 11, 22, 2026, 11, 16)]
    [InlineData(2027, 1, 1, 2026, 12, 28)]
    public void Normalizes_requested_week_across_month_and_year(int y, int m, int d, int wy, int wm, int wd)
    {
        var page = LichTuanViewModel.Tao(new(), [], [], new DateOnly(y, m, d), Now);
        Assert.Equal(new DateOnly(wy, wm, wd), page.TuNgay);
    }

    [Fact]
    public void Filters_boundaries_and_splits_overnight_lesson()
    {
        var page = LichTuanViewModel.Tao(new(), [
            Schedule("2026-11-15T23:00+07:00", "2026-11-16T01:00+07:00"),
            Schedule("2026-11-17T23:00+07:00", "2026-11-18T01:00+07:00"),
            Schedule("2026-11-15T22:00+07:00", "2026-11-16T00:00+07:00"),
            Schedule("2026-11-23T00:00+07:00", "2026-11-23T01:00+07:00")
        ], [], null, Now);
        Assert.Equal(3, page.SuKien.Count);
        Assert.Contains(page.SuKien, x => x.Cot == 0 && x.PhutDau == 0 && x.PhutCuoi == 60);
        Assert.Contains(page.SuKien, x => x.Cot == 1 && x.PhutDau == 1380 && x.PhutCuoi == 1440);
        Assert.Contains(page.SuKien, x => x.Cot == 2 && x.PhutDau == 0 && x.PhutCuoi == 60);
        Assert.Equal(0, page.GioDau);
        Assert.Equal(24, page.GioCuoi);
    }

    [Fact]
    public void Overlapping_history_uses_separate_lanes_and_adjacent_lessons_reuse_lane()
    {
        var page = LichTuanViewModel.Tao(new(), [
            Schedule("2026-11-17T19:00+07:00", "2026-11-17T20:00+07:00", "DaHuy"),
            Schedule("2026-11-17T19:30+07:00", "2026-11-17T20:30+07:00"),
            Schedule("2026-11-17T20:30+07:00", "2026-11-17T21:30+07:00")
        ], [], null, Now);
        var first = page.SuKien.OrderBy(x => x.PhutDau).ToList();
        Assert.Equal(2, first[0].SoCot);
        Assert.Equal(2, first[1].SoCot);
        Assert.NotEqual(first[0].CotTrongNgay, first[1].CotTrongNgay);
        Assert.Equal(1, first[2].SoCot);
    }

    [Theory]
    [InlineData("DaHuy", "DuKien", "Đã hủy")]
    [InlineData("DaDieuChinh", "DaHoc", "Đã đổi lịch")]
    [InlineData("DaLenLich", "DaNghi", "Đã nghỉ")]
    [InlineData("DaLenLich", "DaHoc", "Đã học")]
    [InlineData("DaLenLich", "DangDienRa", "Đang diễn ra")]
    public void Resolves_schedule_and_lesson_status(string state, string lesson, string expected)
    {
        var schedule = Schedule("2026-11-17T19:00+07:00", "2026-11-17T20:00+07:00", state);
        Assert.Equal(expected, SuKienLichViewModel.TrangThaiBuoi(schedule, new() { TrangThai = lesson }, Now));
    }

    [Fact]
    public void Past_unrecorded_lesson_has_specific_status()
    {
        var schedule = Schedule("2026-11-14T19:00+07:00", "2026-11-14T20:00+07:00");
        Assert.Equal("Chưa ghi nhận", SuKienLichViewModel.TrangThaiBuoi(schedule, null, Now));
    }

    [Fact]
    public void Rejects_unrepresentable_week_without_date_overflow()
    {
        var error = Assert.Throws<LoiNghiepVu>(() => LichTuanViewModel.Tao(new(), [], [], DateOnly.MaxValue, Now));
        Assert.Equal("week", error.Truong);
    }
}
