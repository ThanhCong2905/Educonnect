# EduConnect MVC

Ứng dụng ASP.NET Core MVC .NET 10 cho bốn nghiệp vụ, lưu dữ liệu trên SQL Server. Dự án đã được chuyển từ `D:\Educonnection\EduConnect.Mvc` sang `D:\Educonnect`, độc lập với dự án cũ.

Thư mục `.vscode` sẵn có tại vị trí đích được giữ nguyên. Database và các tài liệu nguồn trong `D:\csdl` không thay đổi.

## Chạy tại máy hiện tại

```powershell
cd D:\Educonnect
dotnet run --no-launch-profile --urls http://127.0.0.1:5280
```

Mở http://127.0.0.1:5280. Trang chủ có mục **Xem trước theo vai trò** để chọn học sinh, phụ huynh hoặc gia sư mẫu. Đây là chế độ thử nghiệm, không phải cơ chế đăng nhập. Chỉ chạy trên máy phát triển, bind loopback như lệnh trên.

Đã tạo database `KLTN_4NghiepVu_V2` trên `CONGVJP`, sử dụng Windows Authentication. Chuỗi mặc định dùng `lpc:CONGVJP` (Shared Memory trên cùng máy), không chứa mật khẩu. Nếu dùng máy SQL khác, cấu hình qua biến môi trường, không sửa credential vào Git:

```powershell
$env:ConnectionStrings__EduConnect='Server=TEN_MAY;Database=KLTN_4NghiepVu_V2;Integrated Security=True;Encrypt=True;TrustServerCertificate=True'
dotnet run --no-launch-profile --urls http://127.0.0.1:5280
```

Trên máy hiện tại .NET SDK là `10.0.402`. Restore khi cần:

```powershell
dotnet restore .\EduConnect.Mvc.csproj --configfile .\NuGet.Config
dotnet build .\EduConnect.Mvc.csproj --no-restore
```

Luôn chỉ rõ project/solution mới; không build project cũ ở thư mục cha để chạy ứng dụng này.

## Database

Quy tắc ngăn lỗi mã hóa và cách kiểm chứng: [Unicode](docs/unicode.md). Chạy SQL bổ sung qua `tools/Invoke-SqlUtf8.ps1`, không dựa vào code page mặc định của máy.

Lệnh cài đặt khuyến nghị cho máy mới (database chưa tồn tại):

```powershell
powershell -ExecutionPolicy Bypass -File .\Database\Setup.ps1
```

Database đang tồn tại và cần cập nhật các điều chỉnh:

```powershell
powershell -ExecutionPolicy Bypass -File .\Database\Setup.ps1 -Existing
```

Chỉ chạy khi cài đặt hoặc có thay đổi dữ liệu/nghiệp vụ SQL, không chạy sau mỗi lần sửa giao diện. Script dùng `sqlcmd -f 65001` để đọc UTF-8. Không chạy riêng `00` lại trên database hiện có. Bộ cài mới dùng `00 + 01 + 02 + 05`; các script `03/04` là lịch sử mở rộng danh mục, không cần cho cài mới hiện tại. `06/07` chỉ sửa dữ liệu từng bị nhập sai mã hóa.

Các script trong `Database` đã được áp dụng trên máy hiện tại. Không chạy lại script `00` trên database hiện có.

1. `00_Original_DB02.sql`: bản sao nguyên văn của DB02, tạo database mới và dữ liệu gốc. Có kiểm tra dừng khi database đã tồn tại.
2. `01_MainLearnerPermission.sql`: giữ nguyên logic procedure chấp nhận đề xuất, đổi quyền bên học chính từ `Xem` sang `ChinhSua` theo xác nhận của người dùng; cập nhật thành viên chính đang tồn tại, bổ sung thành viên chính bị thiếu trong seed gốc. Không đổi quyền thành viên phụ.
3. `02_DemoSupplement.sql`: bổ sung hội thoại của liên hệ đã chấp nhận, lịch rảnh gia sư, nhu cầu của phụ huynh mẫu. Có kiểm tra tồn tại để không tạo trùng.

Ví dụ chạy từng script cho **database mới trên máy khác**, theo đúng thứ tự:

```powershell
sqlcmd -S lpc:CONGVJP -E -C -I -b -f 65001 -i Database\00_Original_DB02.sql
sqlcmd -S lpc:CONGVJP -E -C -I -b -f 65001 -i Database\01_MainLearnerPermission.sql
sqlcmd -S lpc:CONGVJP -E -C -I -b -f 65001 -i Database\02_DemoSupplement.sql
```

Ứng dụng không gọi `EnsureCreated`, migration hoặc seed khi khởi động. Không có script DROP. Tệp thật lưu tại `uploads`, ngoài `wwwroot`; metadata và quyền truy cập nằm trong SQL. Sao lưu cả database và thư mục này cùng nhau.

## Kiểm thử

Kết quả thực tế trong `docs/integration-results.json` và `docs/qa/browser-results.json`. Các kiểm thử tích hợp sử dụng database riêng `EduConnect_Verification_20261025`, không dùng database của dự án cũ.

```powershell
dotnet run --project tests\Integration\Integration.csproj --no-restore
```

Harness từ chối database không có tiền tố `EduConnect_Verification_`. Mỗi lần chạy tạo fixture GUID mới, không xóa dữ liệu. Muốn dùng tên kiểm thử mới, chuẩn bị database từ bản sao script `00`, đổi tên database trong **bản sao**, rồi áp dụng bản sao script `01` với tên tương ứng. Đặt `EDU_TEST_CONNECTION` trước khi chạy. Các tệp `tests/fixture.sql` và `tests/permission.sql` ghi lại lần chuẩn bị kiểm thử đầu tiên.

Kiểm thử trình duyệt dùng Node >=20 và Playwright, Edge cài tại đường dẫn Windows mặc định. Trước tiên chạy app trên cổng `5281` với connection string database kiểm thử; sau đó chạy `tests/browser.cjs`. Kiểm tra AJAX bằng `tests/ajax-pages.cjs`, modal và giữ dữ liệu bằng `tests/ui-modernization.cjs`; luồng học tập bằng `tests/workflow.cjs`. Có thể đặt `EDU_PLAYWRIGHT` trỏ tới package Playwright hiện có. Screenshot được lưu trong `docs/qa`. Không chạy kịch bản ghi dữ liệu trên database đang sử dụng thật.

## Phạm vi và giới hạn

- Giao diện sử dụng Bootstrap 5.3.3, Be Vietnam Pro cục bộ và theme `wwwroot/css/app.css`. Modal tải Razor partial, không nhúng cả trang; validation giữ dữ liệu và liên kết lỗi với trường; toast xác nhận tự ẩn sau 4,5 giây.
- Có bốn nghiệp vụ, bộ chọn người dùng mẫu, hồ sơ gia sư và bằng cấp được lưu cùng một lần, tìm/so sánh, liên hệ, đề xuất, lớp, quyền thành viên, lịch, ghi nhận có lịch sử, bài tập, tệp, tin nhắn và thông báo lưu SQL.
- Không có đăng nhập, Identity, quản trị, thanh toán, video call, kiểm tra, đánh giá sao hoặc luồng học bù riêng. Hồ sơ mẫu đã duyệt có thể sửa trong môi trường thử nghiệm; không triển khai quy trình quản trị duyệt hồ sơ.
- Tìm kiếm/lọc/sắp xếp/phân trang cập nhật bằng AJAX và giữ URL/Back. Tin nhắn gửi bằng AJAX, hội thoại đang mở polling khoảng 3 giây; thông báo khoảng 15 giây, vùng dữ liệu trang chủ khoảng 30 giây. Tạm dừng polling khi tab ẩn; chưa có SignalR. Thông báo được ghi trong cùng transaction với thao tác nghiệp vụ, không giả lập email hay push.
- Tệp kiểm tra số lượng, giới hạn **10.000.000 byte** theo CHECK của DB (nghiêm hơn 10 MiB trong HTML), phần mở rộng và chữ ký nội dung. DOCX được kiểm tra cấu trúc ZIP và không nhận VBA. `Sach` trong bản thử nghiệm thể hiện đã qua kiểm tra định dạng, **chưa tích hợp công cụ quét mã độc**; cần bổ sung scanner trước khi triển khai với tệp không tin cậy ngoài môi trường phát triển.
- Danh sách thông báo hiển thị 200 mục gần nhất; lịch sử chat phân trang 50 tin/trang. Gợi ý trang chủ đối chiếu các nhu cầu đang mở và môn dạy trong hồ sơ gia sư, xếp theo số tiêu chí khớp rồi chọn tối đa 3 người.
- Máy SQL từng trả lỗi bộ nhớ 701 trong một lần kiểm thử trình duyệt. Không thay đổi cấu hình bộ nhớ hay dừng các dịch vụ SQL khác của người dùng.

Đối chiếu cấu trúc và chức năng: `docs/MAPPING.md`, `docs/CHECKLIST.md`.


