# Kết quả Unit — Controller và middleware

14 lượt xUnit: 14 Pass, 0 Fail. Project thực thi: `tests/Unit/EduConnect.UnitTests.csproj`.

Tên bên dưới là tên script/lượt dữ liệu xUnit, chưa được tự gán thành mã UT-001–UT-719 của Excel. Pass nghĩa là assertion trong script này đã đạt; không phải kết luận toàn bộ Unit hoặc nghiệp vụ đã được bao phủ.

| Script và dữ liệu | Kết quả | Thời gian | Vị trí script |
|---|---|---|---|
| ControllerGuardTests.Cancelled_end_confirmation_stops_before_accessing_connection_service | Passed | 00:00:00.0088974 | [ControllerGuardTests.cs:65](<C:\Khóa luận TN\EduConnect1\tests\Unit\ControllerGuardTests.cs:65>) |
| ControllerGuardTests.Invalid_end_confirmation_stops_before_loading_actor | Passed | 00:00:00.0023282 | [ControllerGuardTests.cs:44](<C:\Khóa luận TN\EduConnect1\tests\Unit\ControllerGuardTests.cs:44>) |
| ControllerGuardTests.Invalid_model_stops_before_accessing_services(action: "ChapNhan") | Passed | 00:00:00.0006668 | [ControllerGuardTests.cs:22](<C:\Khóa luận TN\EduConnect1\tests\Unit\ControllerGuardTests.cs:22>) |
| ControllerGuardTests.Invalid_model_stops_before_accessing_services(action: "GuiYeuCau") | Passed | 00:00:00.0251369 | [ControllerGuardTests.cs:22](<C:\Khóa luận TN\EduConnect1\tests\Unit\ControllerGuardTests.cs:22>) |
| ControllerGuardTests.Invalid_model_stops_before_accessing_services(action: "HuyKetNoi") | Passed | 00:00:00.0007750 | [ControllerGuardTests.cs:22](<C:\Khóa luận TN\EduConnect1\tests\Unit\ControllerGuardTests.cs:22>) |
| ControllerGuardTests.Invalid_model_stops_before_accessing_services(action: "PhanHoiHuy") | Passed | 00:00:00.0015424 | [ControllerGuardTests.cs:22](<C:\Khóa luận TN\EduConnect1\tests\Unit\ControllerGuardTests.cs:22>) |
| ControllerGuardTests.Invalid_model_stops_before_accessing_services(action: "Rut") | Passed | 00:00:00.0009109 | [ControllerGuardTests.cs:22](<C:\Khóa luận TN\EduConnect1\tests\Unit\ControllerGuardTests.cs:22>) |
| ControllerGuardTests.Invalid_model_stops_before_accessing_services(action: "TuChoi") | Passed | 00:00:00.0005758 | [ControllerGuardTests.cs:22](<C:\Khóa luận TN\EduConnect1\tests\Unit\ControllerGuardTests.cs:22>) |
| ControllerGuardTests.Invalid_search_model_stops_before_loading_catalog | Passed | 00:00:00.0038291 | [ControllerGuardTests.cs:56](<C:\Khóa luận TN\EduConnect1\tests\Unit\ControllerGuardTests.cs:56>) |
| ControllerGuardTests.Middleware_maps_business_error_to_http_status(status: 400) | Passed | 00:00:00.0122752 | [ControllerGuardTests.cs:82](<C:\Khóa luận TN\EduConnect1\tests\Unit\ControllerGuardTests.cs:82>) |
| ControllerGuardTests.Middleware_maps_business_error_to_http_status(status: 403) | Passed | 00:00:00.0000420 | [ControllerGuardTests.cs:82](<C:\Khóa luận TN\EduConnect1\tests\Unit\ControllerGuardTests.cs:82>) |
| ControllerGuardTests.Middleware_maps_business_error_to_http_status(status: 404) | Passed | 00:00:00.0000577 | [ControllerGuardTests.cs:82](<C:\Khóa luận TN\EduConnect1\tests\Unit\ControllerGuardTests.cs:82>) |
| ControllerGuardTests.Middleware_maps_business_error_to_http_status(status: 409) | Passed | 00:00:00.0004357 | [ControllerGuardTests.cs:82](<C:\Khóa luận TN\EduConnect1\tests\Unit\ControllerGuardTests.cs:82>) |
| ControllerGuardTests.Unexpected_error_does_not_expose_internal_details | Passed | 00:00:00.0008909 | [ControllerGuardTests.cs:96](<C:\Khóa luận TN\EduConnect1\tests\Unit\ControllerGuardTests.cs:96>) |
