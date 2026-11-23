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

File `00_EduConnect_Full.sql` tạo database mới với 50 bảng. Database đã đến script 27 chạy `Database/28_HinhThucNhuCau.sql` để nhu cầu có thể chọn cả học trực tuyến và trực tiếp. Database cũ hơn xem [hướng dẫn nâng cấp](Database/README.md). Tài khoản Windows chạy ứng dụng cần có quyền truy cập database.

Mở http://127.0.0.1:5280 và đăng ký tài khoản Học sinh, Phụ huynh hoặc Gia sư. Biến môi trường trên chỉ áp dụng cho phiên PowerShell đang mở. Git lưu mã nguồn và script SQL. Nếu cần dữ liệu đã nhập, chuyển bản sao SQL Server và thư mục `uploads` riêng.

Trong Development, quên mật khẩu lưu thư nội bộ trong `.local-mail` để kiểm thử. Hồ sơ gia sư cần đủ thông tin, minh chứng và được quản trị duyệt trước khi bật hiển thị. Đặt `Authentication:PublicOrigin` đúng URL truy cập.

Khi đưa hệ thống lên môi trường chạy thật, cấu hình SMTP qua `Smtp__Host`, `Smtp__Port`, `Smtp__From`, `Smtp__Username` và `Smtp__Password`. Máy chủ gửi phải hỗ trợ STARTTLS. Lưu mật khẩu SMTP trong kho bí mật của môi trường chạy. Khi SMTP được cấu hình, tài khoản đăng ký mới cần xác minh email trước lần đăng nhập đầu tiên. Tài khoản đã tồn tại tiếp tục đăng nhập như trước. Chức năng Quên mật khẩu chỉ gửi thư cho tài khoản có email đã xác minh.

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

- Giao diện sử dụng Bootstrap 5.3.3, Be Vietnam Pro cục bộ và theme `wwwroot/css/app.css`. Modal tải Razor partial, không nhúng cả trang; validation giữ dữ liệu và liên kết lỗi với trường; toast xác nhận tự ẩn sau 5 giây.
- Có đăng ký và đăng nhập Identity, hồ sơ gia sư, tìm kiếm và so sánh, liên hệ, đề xuất lớp, liên kết tài khoản, quyền thành viên, lịch học, nghỉ và học bù, ghi nhận buổi, bài tập, chat và thông báo.
- Có tiến độ học tập trong Tổng quan lớp. Sau khi lớp kết thúc và có buổi thực tế đã ghi nhận, Gia sư và Bên học đứng tên lớp mỗi bên được đánh giá một lần, gồm số sao và nhận xét. Phản hồi theo đợt cũ được giữ để xem lịch sử. Có báo cáo kèm bằng chứng và đề nghị xem lại. Quản trị có sáu quyền riêng cho hồ sơ, báo cáo, tài khoản, phân quyền, nhật ký và danh mục môn.
- Tìm kiếm, lọc, sắp xếp và phân trang cập nhật bằng AJAX. Chat gửi bằng AJAX và nhận cập nhật qua SignalR, có kiểm tra định kỳ khi mất kết nối.
- Tệp tải lên được kiểm tra số lượng, giới hạn **10.000.000 byte**, phần mở rộng và chữ ký nội dung, rồi quét bằng ClamAV. DOCX được kiểm tra cấu trúc ZIP và không nhận VBA. Cấu hình mặc định yêu cầu quét thành công, kể cả Development. Xem [cách chuẩn bị và cập nhật ClamAV](Database/README.md#tệp-đính-kèm-và-clamav).
- Danh sách thông báo hiển thị 20 mục/trang, có lọc và đánh dấu tất cả đã đọc; lịch sử chat phân trang 50 tin/trang. Gợi ý trang chủ đối chiếu các nhu cầu đang mở và môn dạy trong hồ sơ gia sư, xếp theo số tiêu chí khớp rồi chọn tối đa 3 người.

Đối chiếu cấu trúc và chức năng: `docs/MAPPING.md`, `docs/CHECKLIST.md`.

## Quản trị và các chức năng bổ sung

SQL 00 đã tích hợp các nâng cấp đến 28. Cài mới chỉ chạy `00_EduConnect_Full.sql`. Database đang có dữ liệu chạy các bản nâng cấp còn thiếu theo thứ tự trong `Database/README.md`. Không chạy lại SQL 00 lên database đang sử dụng.

Tạo quản trị đầu tiên từ thư mục gốc dự án, với cùng connection string của web.

```powershell
dotnet run --project tools/ProvisionAdmin/ProvisionAdmin.csproj
```

Công cụ hỏi email, họ tên và mật khẩu, tạo tài khoản mới với sáu quyền quản trị. Đăng ký trên web chỉ cấp vai trò Học sinh, Phụ huynh hoặc Gia sư. Trang **Tài khoản** cho quản trị có quyền phân công hoặc thu hồi từng quyền. Thay quyền thu hồi phiên của tài khoản liên quan.

Guest có hai lựa chọn tìm kiếm. Học sinh và Phụ huynh chỉ tìm gia sư, Gia sư chỉ tìm nhu cầu học. Guest được xem hồ sơ gia sư công khai, đọc đánh giá và so sánh 2–3 gia sư. Guest cũng được xem nhu cầu đang công khai. Các thao tác liên hệ, kết nối và truy cập lớp kiểm tra quyền ở máy chủ.

Tiến độ nằm trong Tổng quan lớp, kèm buổi sắp tới và các buổi đã ghi nhận. Đề nghị kết thúc lớp được gửi cho bên còn lại phản hồi, lớp tiếp tục hoạt động trong thời gian chờ. Báo cáo xuất hiện cạnh hồ sơ, nhu cầu, lớp, tin nhắn và đánh giá. Bằng chứng riêng chỉ người gửi và quản trị được cấp quyền xem.

Sơ đồ ERD hiện tại gồm 50 bảng tại `docs/database/EduConnect-ERD-50-Bang.drawio`. Thiết kế dữ liệu bổ sung nằm tại `docs/database/ThietKe-DanhGia-BaoCao-QuanTri.md`. Sơ đồ lớp tổng thể một trang, tám nhóm nằm tại `docs/diagrams/ClassDesign/FullSystem/EduConnect-ClassDesign-Full.drawio`.
