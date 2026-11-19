# Kết quả Unit — Dữ liệu đầu vào và kết quả nghiệp vụ

19 lượt xUnit: 19 Pass, 0 Fail. Project thực thi: `tests/Unit/EduConnect.UnitTests.csproj`.

Tên bên dưới là tên script/lượt dữ liệu xUnit, chưa được tự gán thành mã UT-001–UT-719 của Excel. Pass nghĩa là assertion trong script này đã đạt; không phải kết luận toàn bộ Unit hoặc nghiệp vụ đã được bao phủ.

| Script và dữ liệu | Kết quả | Thời gian | Vị trí script |
|---|---|---|---|
| FieldValidationTests.Detailed_address_has_the_same_limit(length: 500, valid: True) | Passed | 00:00:00.0001911 | [FieldValidationTests.cs:23](<C:\Khóa luận TN\EduConnect1\tests\Unit\FieldValidationTests.cs:23>) |
| FieldValidationTests.Detailed_address_has_the_same_limit(length: 501, valid: False) | Passed | 00:00:00.0001477 | [FieldValidationTests.cs:23](<C:\Khóa luận TN\EduConnect1\tests\Unit\FieldValidationTests.cs:23>) |
| FieldValidationTests.Generic_result_preserves_field_and_status | Passed | 00:00:00.0148528 | [FieldValidationTests.cs:31](<C:\Khóa luận TN\EduConnect1\tests\Unit\FieldValidationTests.cs:31>) |
| FieldValidationTests.Goal_length_is_shared_by_need_and_proposal(length: 1, valid: True) | Passed | 00:00:00.0000096 | [FieldValidationTests.cs:13](<C:\Khóa luận TN\EduConnect1\tests\Unit\FieldValidationTests.cs:13>) |
| FieldValidationTests.Goal_length_is_shared_by_need_and_proposal(length: 2000, valid: True) | Passed | 00:00:00.0020076 | [FieldValidationTests.cs:13](<C:\Khóa luận TN\EduConnect1\tests\Unit\FieldValidationTests.cs:13>) |
| FieldValidationTests.Goal_length_is_shared_by_need_and_proposal(length: 2001, valid: False) | Passed | 00:00:00.0003390 | [FieldValidationTests.cs:13](<C:\Khóa luận TN\EduConnect1\tests\Unit\FieldValidationTests.cs:13>) |
| FieldValidationTests.Result_without_data_preserves_field_and_status | Passed | 00:00:00.0035160 | [FieldValidationTests.cs:40](<C:\Khóa luận TN\EduConnect1\tests\Unit\FieldValidationTests.cs:40>) |
| ProfileInputTests.Grade_list_accepts_only_distinct_supported_grades(value: "", valid: False) | Passed | 00:00:00.0000041 | [ProfileInputTests.cs:28](<C:\Khóa luận TN\EduConnect1\tests\Unit\ProfileInputTests.cs:28>) |
| ProfileInputTests.Grade_list_accepts_only_distinct_supported_grades(value: "Lớp 0", valid: False) | Passed | 00:00:00.0000034 | [ProfileInputTests.cs:28](<C:\Khóa luận TN\EduConnect1\tests\Unit\ProfileInputTests.cs:28>) |
| ProfileInputTests.Grade_list_accepts_only_distinct_supported_grades(value: "Lớp 1", valid: True) | Passed | 00:00:00.0000029 | [ProfileInputTests.cs:28](<C:\Khóa luận TN\EduConnect1\tests\Unit\ProfileInputTests.cs:28>) |
| ProfileInputTests.Grade_list_accepts_only_distinct_supported_grades(value: "Lớp 1, Lớp 1", valid: False) | Passed | 00:00:00.0000035 | [ProfileInputTests.cs:28](<C:\Khóa luận TN\EduConnect1\tests\Unit\ProfileInputTests.cs:28>) |
| ProfileInputTests.Grade_list_accepts_only_distinct_supported_grades(value: "Lớp 1, Lớp 12", valid: True) | Passed | 00:00:00.0000133 | [ProfileInputTests.cs:28](<C:\Khóa luận TN\EduConnect1\tests\Unit\ProfileInputTests.cs:28>) |
| ProfileInputTests.Grade_list_accepts_only_distinct_supported_grades(value: "Lớp 13", valid: False) | Passed | 00:00:00.0062449 | [ProfileInputTests.cs:28](<C:\Khóa luận TN\EduConnect1\tests\Unit\ProfileInputTests.cs:28>) |
| ProfileInputTests.Grade_list_accepts_only_distinct_supported_grades(value: "Tất cả", valid: False) | Passed | 00:00:00.0002749 | [ProfileInputTests.cs:28](<C:\Khóa luận TN\EduConnect1\tests\Unit\ProfileInputTests.cs:28>) |
| ProfileInputTests.Introduction_length_matches_profile_contract(length: 100, valid: True) | Passed | 00:00:00.0000252 | [ProfileInputTests.cs:12](<C:\Khóa luận TN\EduConnect1\tests\Unit\ProfileInputTests.cs:12>) |
| ProfileInputTests.Introduction_length_matches_profile_contract(length: 4000, valid: True) | Passed | 00:00:00.0008750 | [ProfileInputTests.cs:12](<C:\Khóa luận TN\EduConnect1\tests\Unit\ProfileInputTests.cs:12>) |
| ProfileInputTests.Introduction_length_matches_profile_contract(length: 4001, valid: False) | Passed | 00:00:00.0000443 | [ProfileInputTests.cs:12](<C:\Khóa luận TN\EduConnect1\tests\Unit\ProfileInputTests.cs:12>) |
| ProfileInputTests.Introduction_length_matches_profile_contract(length: 99, valid: False) | Passed | 00:00:00.0065392 | [ProfileInputTests.cs:12](<C:\Khóa luận TN\EduConnect1\tests\Unit\ProfileInputTests.cs:12>) |
| ProfileInputTests.Profile_catalogs_have_no_duplicates | Passed | 00:00:00.0064352 | [ProfileInputTests.cs:32](<C:\Khóa luận TN\EduConnect1\tests\Unit\ProfileInputTests.cs:32>) |
