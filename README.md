# EduConnect MVC

Ứng dụng ASP.NET Core MVC, Razor View, Bootstrap, .NET 10 và SQL Server.

## Chạy ứng dụng

Cài database 38 bảng theo [hướng dẫn database](Database/README.md), kiểm tra `ConnectionStrings:EduConnect`, sau đó chạy.

```powershell
dotnet run --no-launch-profile --urls http://127.0.0.1:5280
```

Mở trang đăng ký để tạo tài khoản Học sinh, Phụ huynh hoặc Gia sư. Bản này xác thực bằng ASP.NET Identity và cookie, có phiên đăng nhập trong SQL. Database mặc định là `KLTN_Full_V2` trên `lpc:CONGVJP`, dùng Windows Authentication.

Trong Development, quên mật khẩu lưu thư nội bộ trong `.local-mail` để kiểm thử. Gia sư có thể công khai hồ sơ đã hoàn thiện bằng cấu hình demo. Đặt `Authentication:PublicOrigin` đúng URL truy cập.

SignalR cập nhật tin nhắn, thông báo và trạng thái nghiệp vụ. Phiên bị thu hồi khi đăng xuất hoặc đặt lại mật khẩu.

## Kiểm thử phiên bản Identity

Xem [cách chạy kiểm thử tài khoản](tests/Authentication/README.md). Các bộ kiểm thử dùng người dùng mẫu Session bên dưới thuộc phiên bản trước, cần chuyển fixture sang Identity trước khi chạy với database mới.

## Kiểm thử

Kết quả thực tế trong `docs/integration-results.json` và `docs/qa/browser-results.json`. Các kiểm thử tích hợp sử dụng database riêng `EduConnect_Verification_20261025`, không dùng database của dự án cũ. Bộ cài một file đã vượt qua 38 kiểm thử trên database mới riêng biệt; kết quả lưu tại `docs/qa/bootstrap-integration-results.json`.

```powershell
dotnet run --project tests\Integration\Integration.csproj --no-restore
```

Harness từ chối database không có tiền tố `EduConnect_Verification_`. Mỗi lần chạy tạo fixture GUID mới, không xóa dữ liệu. Muốn dùng database kiểm thử mới, đổi tên database trong **bản sao** script `00` rồi chạy bản sao đó; không cần thêm script `01`. Đặt `EDU_TEST_CONNECTION` trước khi chạy. Các tệp `tests/fixture.sql` và `tests/permission.sql` ghi lại lần chuẩn bị kiểm thử đầu tiên.

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
