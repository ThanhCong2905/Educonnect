# Bộ chạy Integration MT1 — TC 014–086

Bộ chạy có **73 hàm test riêng**, đặt tên `MT1_IT_014`…`MT1_IT_086`.
Mỗi hàm có mã, tên tình huống và kỳ vọng từ sheet **Intergration Test MT1** của bảng TC người dùng. Các ca Unit trong `RulesTests.cs` không được tính vào 73 ca này.

## Chạy từ thư mục gốc Educonnect

Chạy toàn bộ:

```powershell
.\tests\MT1\Run-Integration.ps1
```

Chạy một TC, ví dụ 014:

```powershell
.\tests\MT1\Run-Integration.ps1 -Case MT1-IT-014
```

Ví dụ chạy riêng ca HTTP hoặc rollback:

```powershell
.\tests\MT1\Run-Integration.ps1 -Case MT1-IT-067
.\tests\MT1\Run-Integration.ps1 -Case MT1-IT-081
```

Nếu PowerShell chặn thực thi script, dùng cho tiến trình chạy này:

```powershell
pwsh -NoProfile -ExecutionPolicy Bypass -File .\tests\MT1\Run-Integration.ps1
```

Các lệnh trên tự build, chạy và xuất kết quả; không cần mở ứng dụng hay nhấn thao tác trong SSMS. SQL Server cần đang hoạt động và database kiểm thử đã được tạo từ `Database/00_EduConnect_Full.sql`.

Mặc định:

```text
Server=LAPTOP-3L2V1AFT\SQLEXPRESS
Database=EduConnect_Verification_MT1
Windows Authentication
```

Đổi server bằng tham số `-Connection` nếu cần. Bộ chạy từ chối mọi tên database khác **EduConnect_Verification_MT1**, kể cả khi đổi connection string; không đổi `appsettings.json` của ứng dụng.

## Phân định các ca

| Mã TC | Nhóm | Thực hiện |
|---|---|---|
| 014–042 | Nhu cầu học | Service thật, SQL Server thật; chủ sở hữu, nháp, nhiều môn, phiên bản, trạng thái, rollback |
| 043–060 | Tìm kiếm / chi tiết / so sánh / gợi ý | Fixture theo vai trò; kiểm Id, số lượng, thứ tự, giá–đơn vị và dữ liệu công khai |
| 061–075 | HTTP | WebApplicationFactory chạy MVC thật; đăng nhập Identity, cookie và antiforgery thật |
| 076–084 | Liên hệ / chat | Yêu cầu, phản hồi, thông báo/outbox, chặn tài khoản, đồng thời, rollback và retry |
| 085–086 | HTTP lỗi đầu vào | JSON lỗi trường, mã HTTP và kiểm không ghi sai dữ liệu |

**Mở `IntegrationTests.cs`** để tìm từng hàm, chẳng hạn `MT1_IT_014`, hoặc tìm bằng mã `MT1-IT-014`. Mỗi mã có một `[Fact]` riêng và một dòng trong bảng kết quả.

- `IntegrationTests.cs`: danh sách 73 TC, kỳ vọng, các assertion Service/SQL và liên hệ.
- `HttpIntegrationTests.cs`: gửi request thật qua MVC, kiểm status/body/DB cho các TC HTTP.
- `IntegrationHarness.cs`: tạo tài khoản/vai trò/hồ sơ/môn của từng ca; scope, snapshot và cleanup.
- `Run-Integration.ps1`: chọn một ca hoặc toàn bộ, khóa tránh hai lượt ghi đè cùng báo cáo, xuất kết quả.

## Kết quả nằm ở đâu?

Trong **TestResults/MT1** có ba file cố định cho lượt chạy gần nhất:

| File | Nội dung |
|---|---|
| `BangKetQua-Integration-MT1.csv` | Bảng dễ đọc bằng Excel: RunId, mã TC, tên, trạng thái, Actual ngắn và minh chứng |
| `MT1-integration.trx` | Kết quả gốc của xUnit/VSTest: từng TC, assertion lỗi, stack trace, thời gian và lý do skip |
| `integration-details.jsonl` | Mỗi TC một dòng JSON: actor/fixture, HTTP, snapshot SQL trước cleanup, lỗi và thời gian |

Chạy riêng một ca sẽ thay ba file này bằng kết quả của **ca vừa chọn**. Chạy toàn bộ sẽ tạo bảng đủ 73 mã; kết quả không được gộp từ nhiều lượt cũ. `RunId` là mã chung để đối chiếu đúng một lượt chạy.

- **Pass**: các assertion của TC đều đạt và cleanup thành công.
- **Fail**: có assertion không khớp hoặc có lỗi chuẩn bị/cleanup; đọc `Actual` và TRX/JSONL để phân biệt.
- **Chờ đặc tả**: TC được xUnit ghi `Skipped`, không tính là Pass.

Ba ca đang chờ đặc tả: **019** (giữ/xóa địa chỉ trực tuyến), **049** (so khớp từ khóa/dấu/hoa thường), **059** (bộ lọc khoảng học phí). Không tự chọn hành vi hiện tại của phần mềm làm kỳ vọng cho ba ca này.

Bộ chạy giữ kỳ vọng trong bảng TC. **035** kiểm kiểu lỗi Service, **068** kiểm form trả lại và **071** kiểm HTTP409 đúng hợp đồng đã ghi trong bảng.

### Đối chiếu thiết kế cho TC026,027,029,031 (07/11/2026)

Căn cứ: SRS (2) FR-01/03/04; sơ đồ lớp tổng thể (1), `TaoNhuCauBenHocViewModel.MonHocIds`, `NhuCauInput.MonHocIds`, `TaoNhuCauHocAsync` và `CapNhatAsync`; sequence **SC_UC1.1A bước3.4–3.19**, **SC_UC1.1b bước1.2–1.7 và2.4–2.19**.

Thiết kế biểu mẫu nhận danh sách môn, luồng tạo trả Id và luồng sửa nhận Id/phiên bản. Không thấy quy định tách một nhu cầu cho mỗi môn hoặc cấm sửa nhiều môn. Vì vậy các TC được chỉnh để kiểm **danh sách môn qua hợp đồng lưu–đọc lại**, quyền sở hữu, lịch và phiên bản:

- **026:** gửi hai môn hợp lệ; dùng Id trả về để đọc lại đủ hai môn và đúng hai khung giờ đã gửi.
- **027:** gửi Id môn lặp; kết quả đọc lại có đúng hai Id phân biệt, không mất môn và không nhân đôi lịch.
- **029:** gửi danh sách gồm10môn hợp lệ; đọc lại đủ10môn và đúng hai khung giờ. **10 là dữ liệu thử, không khẳng định giới hạn tối đa** vì tài liệu chưa nêu giới hạn này.
- **031:** sửa một nhu cầu của đúng chủ từ môn cũ sang hai môn hợp lệ khác; giữ Id/chủ, thay đúng danh sách môn, đổi RowVersion và giữ lịch theo dữ liệu gửi; không tăng số nhu cầu sau thao tác sửa.

Lưu ý truy vết: entity `NhuCauHoc`/bảng trong tài liệu vẫn có `MonHocId` đơn, chưa mô tả đầy đủ ánh xạ danh sách môn. Các TC trên không lấy số bản ghi nhu cầu, số dòng bảng liên kết hoặc tên bảng bổ sung từ code làm chuẩn thiết kế. Phần mô hình lưu trữ cần được đồng bộ trong tài liệu riêng.

## Fixture và tính độc lập

Mỗi TC tạo GUID và dấu `MT1IT_<guid>` riêng. Id môn được lấy sau INSERT, không giả định môn Toán luôn có Id101. Service dùng principal riêng theo actor; lớp lấy người hiện tại vẫn đọc tài khoản/vai trò thật từ database. HTTP đăng nhập bằng Identity và cookie thật, không dùng chức năng chọn người demo.

Mỗi lần kiểm dữ liệu sau Act dùng context mới. Hai lời gọi của TC080 dùng hai context/kết nối độc lập và đồng bộ thời điểm bắt đầu. TC037 gây lỗi ghi lịch sau lần lưu nhu cầu đầu trong giao dịch. TC081 dùng SQL trigger thử theo đúng người nhận của ca để gây lỗi INSERT thông báo; xóa trigger trong `finally` và kiểm không có liên hệ/sự kiện/thông báo được commit dở.

Snapshot được ghi trước khi cleanup. Cleanup chỉ xóa các Id thuộc fixture của TC, theo thứ tự khóa ngoại. Bộ chạy không reset/truncate toàn database. Hai worker nền bị tắt trong host kiểm thử để không xử lý fixture/outbox giữa chừng; Service, MVC, Identity và SQL vẫn chạy thật.

## Chạy trực tiếp bằng dotnet test

```powershell
$env:EDU_TEST_CONNECTION='Server=LAPTOP-3L2V1AFT\SQLEXPRESS;Database=EduConnect_Verification_MT1;Integrated Security=True;Encrypt=False;TrustServerCertificate=True'
$env:MT1_EVIDENCE=(Join-Path (Get-Location) 'TestResults\MT1')
dotnet test tests/MT1/MT1.csproj --filter 'Suite=MT1Integration&FullyQualifiedName~.MT1_IT_014' --logger 'trx;LogFileName=MT1-integration.trx' --results-directory TestResults/MT1
```

Nên dùng script PowerShell để có bảng CSV, RunId và kết quả đầy đủ của đúng lượt chạy. Bộ SQL **001–013** tiếp tục dùng `tests/MT1-Integration.sql`, không nằm trong lệnh chạy C# này.
