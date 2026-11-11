# EduConnect MVC

Ứng dụng ASP.NET Core MVC, Razor View, Bootstrap, .NET 10 và SQL Server.

## Chạy sau khi clone

Cần .NET 10 SDK, SQL Server và `sqlcmd`. Chuyển sang nhánh chứa phiên bản cần chạy, rồi mở PowerShell ở thư mục có `EduConnect.Mvc.csproj`.

```powershell
$eduSqlServer = 'localhost' # Đổi thành tên instance đang dùng, ví dụ localhost\SQLEXPRESS
sqlcmd -S $eduSqlServer -d master -E -C -I -b -f 65001 -i Database\00_EduConnect_Full.sql
$env:ConnectionStrings__EduConnect = "Server=$eduSqlServer;Database=KLTN_Full_V2;Integrated Security=True;TrustServerCertificate=True"
dotnet run --project EduConnect.Mvc.csproj
```

File `00_EduConnect_Full.sql` tạo database mới với 48 bảng. Nếu đã có database 38 bảng và cần giữ dữ liệu, xem [cách nâng cấp](Database/README.md#nâng-cấp-database-38-bảng). Tài khoản Windows dùng để chạy ứng dụng cần có quyền truy cập database.

Mở http://127.0.0.1:5280 và đăng ký tài khoản Học sinh, Phụ huynh hoặc Gia sư. Biến môi trường trên chỉ áp dụng cho phiên PowerShell đang mở. Git lưu mã nguồn và script SQL. Nếu cần dữ liệu đã nhập, chuyển bản sao SQL Server và thư mục `uploads` riêng.

Trong Development, quên mật khẩu lưu thư nội bộ trong `.local-mail` để kiểm thử. Gia sư có thể công khai hồ sơ đã hoàn thiện bằng cấu hình demo. Đặt `Authentication:PublicOrigin` đúng URL truy cập.

SignalR cập nhật tin nhắn, thông báo và trạng thái nghiệp vụ. Phiên bị thu hồi khi đăng xuất hoặc đặt lại mật khẩu.

## Kiểm thử phiên bản Identity

Xem [cách chạy kiểm thử tài khoản](tests/Authentication/README.md). Các bộ kiểm thử dùng người dùng mẫu Session bên dưới thuộc phiên bản trước, cần chuyển fixture sang Identity trước khi chạy với database mới.

## Kiểm thử

Kiểm thử biên đầu vào (độ dài, miền giá trị, mật khẩu Unicode) chạy bằng `dotnet run --project tests/InputContracts/InputContracts.csproj`. Nếu web đang chạy, thêm `--artifacts-path .codex-local/input-contracts-build` để build riêng, tránh khóa tệp. Bộ này không ghi dữ liệu vào SQL Server.

Kết quả thực tế trong `docs/integration-results.json` và `docs/qa/browser-results.json`. Các kiểm thử tích hợp sử dụng database riêng `EduConnect_Verification_20261025`, không dùng database của dự án cũ. Bộ cài một file đã vượt qua 38 kiểm thử trên database mới riêng biệt; kết quả lưu tại `docs/qa/bootstrap-integration-results.json`.

```powershell
dotnet run --project tests\Integration\Integration.csproj --no-restore
```

Harness từ chối database không có tiền tố `EduConnect_Verification_`. Mỗi lần chạy tạo fixture GUID mới, không xóa dữ liệu. Muốn dùng database kiểm thử mới, đổi tên database trong **bản sao** script `00` rồi chạy bản sao đó; không cần thêm script `01`. Đặt `EDU_TEST_CONNECTION` trước khi chạy. Các tệp `tests/fixture.sql` và `tests/permission.sql` ghi lại lần chuẩn bị kiểm thử đầu tiên.

Kiểm thử trình duyệt dùng Node >=20 và Playwright, Edge cài tại đường dẫn Windows mặc định. Trước tiên chạy app trên cổng `5281` với connection string database kiểm thử; sau đó chạy `tests/browser.cjs`. Kiểm tra AJAX bằng `tests/ajax-pages.cjs`, modal và giữ dữ liệu bằng `tests/ui-modernization.cjs`; luồng học tập bằng `tests/workflow.cjs`. Có thể đặt `EDU_PLAYWRIGHT` trỏ tới package Playwright hiện có. Screenshot được lưu trong `docs/qa`. Không chạy kịch bản ghi dữ liệu trên database đang sử dụng thật.

## Phạm vi và giới hạn

- Giao diện sử dụng Bootstrap 5.3.3, Be Vietnam Pro cục bộ và theme `wwwroot/css/app.css`. Modal tải Razor partial, không nhúng cả trang; validation giữ dữ liệu và liên kết lỗi với trường; toast xác nhận tự ẩn sau 4,5 giây.
- Có đăng ký và đăng nhập Identity, hồ sơ gia sư, tìm kiếm và so sánh, liên hệ, đề xuất lớp, liên kết tài khoản, quyền thành viên, lịch học, nghỉ và học bù, ghi nhận buổi, bài tập, chat và thông báo.
- Có tiến độ học tập, phản hồi theo đợt, quyết định sau phản hồi, đánh giá công khai hai chiều, báo cáo kèm bằng chứng và đề nghị xem lại. Quản trị có sáu quyền riêng cho hồ sơ, báo cáo, tài khoản, phân quyền, nhật ký và danh mục môn.
- Tìm kiếm, lọc, sắp xếp và phân trang cập nhật bằng AJAX. Chat gửi bằng AJAX và nhận cập nhật qua SignalR, có kiểm tra định kỳ khi mất kết nối.
- Tệp kiểm tra số lượng, giới hạn **10.000.000 byte**, phần mở rộng và chữ ký nội dung. DOCX được kiểm tra cấu trúc ZIP và không nhận VBA. Development bật `FileScanning:AllowUnscannedUploads` cho phép thử tải và tải lại tệp `ChoQuet` theo quyền. Ngoài Development, ứng dụng yêu cầu quét ClamAV thành công trước khi sử dụng tệp `Sach`.
- Danh sách thông báo hiển thị 20 mục/trang, có lọc và đánh dấu tất cả đã đọc; lịch sử chat phân trang 50 tin/trang. Gợi ý trang chủ đối chiếu các nhu cầu đang mở và môn dạy trong hồ sơ gia sư, xếp theo số tiêu chí khớp rồi chọn tối đa 3 người.

Đối chiếu cấu trúc và chức năng: `docs/MAPPING.md`, `docs/CHECKLIST.md`.

## Quản trị và các chức năng bổ sung

SQL 00 đã tích hợp cả nâng cấp 13 và 14. Cài mới chỉ chạy `00_EduConnect_Full.sql`. Database 41 bảng đang có dữ liệu dùng `14_DanhGiaBaoCaoQuanTri.sql` sau khi sao lưu. Không chạy lại SQL 00 lên database đang sử dụng.

Tạo quản trị đầu tiên từ thư mục gốc dự án, với cùng connection string của web.

```powershell
dotnet run --project tools/ProvisionAdmin/ProvisionAdmin.csproj
```

Công cụ hỏi email, họ tên và mật khẩu, tạo tài khoản mới với sáu quyền quản trị. Đăng ký trên web chỉ cấp vai trò Học sinh, Phụ huynh hoặc Gia sư. Trang **Tài khoản** cho quản trị có quyền phân công hoặc thu hồi từng quyền. Thay quyền thu hồi phiên của tài khoản liên quan.

Tiến độ và phản hồi mở từ chi tiết lớp. Đánh giá công khai mở khi lớp đã kết thúc và có buổi thực tế đã ghi nhận. Báo cáo xuất hiện cạnh hồ sơ, nhu cầu, lớp, tin nhắn và đánh giá. Bằng chứng riêng chỉ người gửi và quản trị được cấp quyền xem.

Sơ đồ ERD 48 bảng nằm tại `docs/database/EduConnect-ERD-48-Bang.drawio`. Thiết kế dữ liệu bổ sung nằm tại `docs/database/ThietKe-DanhGia-BaoCao-QuanTri.md`. Sơ đồ lớp tổng thể một trang, tám nhóm nằm tại `docs/diagrams/ClassDesign/FullSystem/EduConnect-ClassDesign-Full.drawio`.
