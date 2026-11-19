# Unit Test độc lập

Project: `tests/Unit/EduConnect.UnitTests.csproj`, .NET 10 / xUnit.
Chạy tại thư mục EduConnect1:

```powershell
$env:UNIT_EVIDENCE = "$PWD/TestResults/Unit/01_Quy_tac_chung"
dotnet test tests/Unit/EduConnect.UnitTests.csproj --logger "trx;LogFileName=Unit.trx" --results-directory TestResults/Unit
```

Các nhóm: RulesTests, ProfileInputTests/FieldValidationTests, ControllerGuardTests,
ClassCalendarTests, AdministrativeAreaTests, GuestAccessTests và InputBoundaryTests.
Chỉ gồm kiểm thử trong bộ nhớ; không chạy SQL Server, ứng dụng web, worker nền hoặc gửi email.
Nguồn test độc lập được tách từ các test đang có; 30 kiểm tra biên console được chuyển thành xUnit riêng.
Không sửa assertion để làm Pass. Test metadata chỉ chứng minh attribute, không chứng minh HTTP pipeline.
Các lời gọi dùng dependency null chỉ kiểm nhánh dừng sớm, không bao phủ đường đi sử dụng dependency.

Các tên RULES-/Input_boundary là định danh script, không được tự gán thành mã UT-001–UT-719
trong Excel mới. 135 lượt dữ liệu xUnit của project này chưa chứng minh chạy đủ 719 TC/1.342 dữ liệu.
Các TC Excel cần script và đối chiếu nhánh/đầu vào/kỳ vọng chính xác trước khi ghi verdict theo mã mới.
