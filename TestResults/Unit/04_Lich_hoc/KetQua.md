# Kết quả Unit — Lịch học

12 lượt xUnit: 12 Pass, 0 Fail. Project thực thi: `tests/Unit/EduConnect.UnitTests.csproj`.

Tên bên dưới là tên script/lượt dữ liệu xUnit, chưa được tự gán thành mã UT-001–UT-719 của Excel. Pass nghĩa là assertion trong script này đã đạt; không phải kết luận toàn bộ Unit hoặc nghiệp vụ đã được bao phủ.

| Script và dữ liệu | Kết quả | Thời gian | Vị trí script |
|---|---|---|---|
| ClassCalendarTests.Filters_boundaries_and_splits_overnight_lesson | Passed | 00:00:00.0080719 | [ClassCalendarTests.cs:34](<C:\Khóa luận TN\EduConnect1\tests\Unit\ClassCalendarTests.cs:34>) |
| ClassCalendarTests.Normalizes_requested_week_across_month_and_year(y: 2026, m: 11, d: 22, wy: 2026, wm: 11, wd: 16) | Passed | 00:00:00.0059076 | [ClassCalendarTests.cs:27](<C:\Khóa luận TN\EduConnect1\tests\Unit\ClassCalendarTests.cs:27>) |
| ClassCalendarTests.Normalizes_requested_week_across_month_and_year(y: 2027, m: 1, d: 1, wy: 2026, wm: 12, wd: 28) | Passed | 00:00:00.0003387 | [ClassCalendarTests.cs:27](<C:\Khóa luận TN\EduConnect1\tests\Unit\ClassCalendarTests.cs:27>) |
| ClassCalendarTests.Overlapping_history_uses_separate_lanes_and_adjacent_lessons_reuse_lane | Passed | 00:00:00.0025117 | [ClassCalendarTests.cs:51](<C:\Khóa luận TN\EduConnect1\tests\Unit\ClassCalendarTests.cs:51>) |
| ClassCalendarTests.Past_unrecorded_lesson_has_specific_status | Passed | 00:00:00.0001381 | [ClassCalendarTests.cs:78](<C:\Khóa luận TN\EduConnect1\tests\Unit\ClassCalendarTests.cs:78>) |
| ClassCalendarTests.Rejects_unrepresentable_week_without_date_overflow | Passed | 00:00:00.0006246 | [ClassCalendarTests.cs:85](<C:\Khóa luận TN\EduConnect1\tests\Unit\ClassCalendarTests.cs:85>) |
| ClassCalendarTests.Resolves_schedule_and_lesson_status(state: "DaDieuChinh", lesson: "DaHoc", expected: "Đã đổi lịch") | Passed | 00:00:00.0000030 | [ClassCalendarTests.cs:71](<C:\Khóa luận TN\EduConnect1\tests\Unit\ClassCalendarTests.cs:71>) |
| ClassCalendarTests.Resolves_schedule_and_lesson_status(state: "DaHuy", lesson: "DuKien", expected: "Đã hủy") | Passed | 00:00:00.0000083 | [ClassCalendarTests.cs:71](<C:\Khóa luận TN\EduConnect1\tests\Unit\ClassCalendarTests.cs:71>) |
| ClassCalendarTests.Resolves_schedule_and_lesson_status(state: "DaLenLich", lesson: "DaHoc", expected: "Đã học") | Passed | 00:00:00.0000040 | [ClassCalendarTests.cs:71](<C:\Khóa luận TN\EduConnect1\tests\Unit\ClassCalendarTests.cs:71>) |
| ClassCalendarTests.Resolves_schedule_and_lesson_status(state: "DaLenLich", lesson: "DaNghi", expected: "Đã nghỉ") | Passed | 00:00:00.0001964 | [ClassCalendarTests.cs:71](<C:\Khóa luận TN\EduConnect1\tests\Unit\ClassCalendarTests.cs:71>) |
| ClassCalendarTests.Resolves_schedule_and_lesson_status(state: "DaLenLich", lesson: "DangDienRa", expected: "Đang diễn ra") | Passed | 00:00:00.0092690 | [ClassCalendarTests.cs:71](<C:\Khóa luận TN\EduConnect1\tests\Unit\ClassCalendarTests.cs:71>) |
| ClassCalendarTests.Uses_Vietnam_date_and_Monday_as_week_start | Passed | 00:00:00.0008476 | [ClassCalendarTests.cs:16](<C:\Khóa luận TN\EduConnect1\tests\Unit\ClassCalendarTests.cs:16>) |
