# Kết quả Unit — Địa giới và danh mục

7 lượt xUnit: 7 Pass, 0 Fail. Project thực thi: `tests/Unit/EduConnect.UnitTests.csproj`.

Tên bên dưới là tên script/lượt dữ liệu xUnit, chưa được tự gán thành mã UT-001–UT-719 của Excel. Pass nghĩa là assertion trong script này đã đạt; không phải kết luận toàn bộ Unit hoặc nghiệp vụ đã được bao phủ.

| Script và dữ liệu | Kết quả | Thời gian | Vị trí script |
|---|---|---|---|
| AdministrativeAreaTests.Catalog_covers_all_provinces_without_duplicate_wards | Passed | 00:00:00.0023929 | [AdministrativeAreaTests.cs:15](<C:\Khóa luận TN\EduConnect1\tests\Unit\AdministrativeAreaTests.cs:15>) |
| AdministrativeAreaTests.Common_subjects_come_before_other_subjects | Passed | 00:00:00.0121933 | [AdministrativeAreaTests.cs:37](<C:\Khóa luận TN\EduConnect1\tests\Unit\AdministrativeAreaTests.cs:37>) |
| AdministrativeAreaTests.Ward_must_belong_to_the_selected_province(province: "Hà Nội", ward: "Phường Ba Đình", expected: True) | Passed | 00:00:00.0284175 | [AdministrativeAreaTests.cs:33](<C:\Khóa luận TN\EduConnect1\tests\Unit\AdministrativeAreaTests.cs:33>) |
| AdministrativeAreaTests.Ward_must_belong_to_the_selected_province(province: "Hà Nội", ward: "Quận Ba Đình", expected: False) | Passed | 00:00:00.0000027 | [AdministrativeAreaTests.cs:33](<C:\Khóa luận TN\EduConnect1\tests\Unit\AdministrativeAreaTests.cs:33>) |
| AdministrativeAreaTests.Ward_must_belong_to_the_selected_province(province: "Hà Nội", ward: null, expected: True) | Passed | 00:00:00.0000082 | [AdministrativeAreaTests.cs:33](<C:\Khóa luận TN\EduConnect1\tests\Unit\AdministrativeAreaTests.cs:33>) |
| AdministrativeAreaTests.Ward_must_belong_to_the_selected_province(province: "Hồ Chí Minh", ward: "Phường Ba Đình", expected: False) | Passed | 00:00:00.0000058 | [AdministrativeAreaTests.cs:33](<C:\Khóa luận TN\EduConnect1\tests\Unit\AdministrativeAreaTests.cs:33>) |
| AdministrativeAreaTests.Ward_must_belong_to_the_selected_province(province: null, ward: "Phường Ba Đình", expected: False) | Passed | 00:00:00.0002812 | [AdministrativeAreaTests.cs:33](<C:\Khóa luận TN\EduConnect1\tests\Unit\AdministrativeAreaTests.cs:33>) |
