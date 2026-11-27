# Unit Test — bộ TC hiện tại

Project: `tests/Unit/EduConnect.UnitTests.csproj`, xUnit / .NET 10.
Phạm vi: 79 Unit C#, 360 TC `UT-001..UT-360`, 389 bộ dữ liệu.

Chạy từ thư mục EduConnect1:

```powershell
.\tests\Unit\Run-UnitTests.ps1
```

Script build ứng dụng trước rồi build project test. Test tham chiếu DLL ứng dụng vừa build ở `bin/Debug/net10.0`; cách này tránh kéo bước StaticWebAssets của Web SDK vào project test. Không thay file `.csproj` hay code của ứng dụng.

Chỉ các test có trait `Suite=CurrentDesign` được tính vào bộ TC này. Kết quả chính tại `TestResults/Unit/KetQua.md`, JSON/CSV cùng thư mục và TRX theo từng nhóm UC. UC5 không có Unit được chọn trong danh mục hiện tại.

Các file:

- `CurrentDesignTests.cs`: một `[Fact]` cho mỗi TC/DL, trait TC/nhóm để chạy riêng, assertion của các unit thuần; reflection chỉ gọi method thật đang kiểm thử.
- `CurrentDesignManifest.json`: đối chiếu đủ TC/DL với design Unit ID, input, kỳ vọng và method test.
- `CurrentEfBoundary.cs`: fixture truy vấn/tracking bằng EF InMemory; model chuẩn bị trong Arrange. Chạy predicates/joins/projections thật, không giả sẵn đáp án. Không kiểm SQL translation/constraint/lock ở đây.
- `CurrentRelationalBoundary.cs`: interceptor chặn mọi mở kết nối và mọi command; kiểm SQL/parameter/token của wrapper và trả reader fixture. Không chạy SQL Server.
- `CurrentAuditBoundary.cs`: scope/logger/HubCallerContext/SaveChanges giả lập, kiểm entity audit và nhánh lỗi ghi nhật ký.
- `CurrentStaticBoundary.cs`: Lib.Harmony 2.4.2 shim `DateTimeOffset.UtcNow`, `File.Delete`, runtime Assembly resource API trong tiến trình test; không patch method của ứng dụng. Dispose tháo shim sau mỗi lượt.
- `Run-UnitTests.ps1`: build và chạy theo nhóm, ghi đè TRX/log tại cùng vị trí, đặt biến môi trường riêng cho testhost để tránh inline bỏ qua shim.
- `Update-UnitReports.py`: tổng hợp TRX theo nhóm thành báo cáo/JSON/CSV, kiểm đủ 389 dòng và không trùng TC/DL; dùng Python 3, thư viện chuẩn.

Chạy riêng một ca:

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --no-build --no-restore --filter "Suite=CurrentDesign&TestCaseId=UT-128" --environment COMPlus_JitNoInline=1 --environment DOTNET_ReadyToRun=0 --environment DOTNET_TieredCompilation=0
```

Các biến môi trường chỉ có hiệu lực trong testhost. Test không chạy song song vì có shim/static state. Khi xem trong Test Explorer, cần cấu hình các biến môi trường tương tự hoặc dùng script trên.

Sau khi chạy riêng một ca, TRX riêng đó không thay kết quả tổng hợp cả nhóm. Chạy runner toàn bộ rồi cập nhật báo cáo để có bộ kết quả đồng nhất.

Lưu ý UT-354: Excel yêu cầu thêm `MaYeuCau.IsUnicode=false`, trong khi Process HT-U55 và code đều chỉ đặt độ dài 128. Giữ kết quả Fail theo Excel và phân loại lỗi kỳ vọng TC; không sửa code để làm test xanh.
