# Database EduConnect

Bản đăng nhập Identity dùng `00_EduConnect_Full.sql`, tạo database `KLTN_Full_V2` với 38 bảng. File hợp nhất cấu trúc tài khoản từ SQL được cung cấp với procedure, ràng buộc và nghiệp vụ hiện có. `NguoiThamGia` là view đọc tài khoản và vai trò Identity.

## Cài mới

```powershell
sqlcmd -S lpc:CONGVJP -d master -E -C -I -b -f 65001 -i Database\00_EduConnect_Full.sql
```

Hoặc chạy `Database\Setup.ps1 -Server TEN_SERVER`. Script dừng nếu database đã tồn tại. Đổi server trong `ConnectionStrings:EduConnect` cho đúng máy trước khi chạy ứng dụng. SQL tạo danh mục môn và vai trò. Người dùng tạo tài khoản qua trang đăng ký.

Các file `00_Original_DB02.sql` và script đánh số cũ phục vụ phiên bản trước Identity. Bản cài mới tạo database độc lập. Chuyển dữ liệu cũ sang bản mới cần ánh xạ tài khoản riêng.

## Cấu hình tài khoản

- Đăng ký cấp một vai trò Học sinh, Phụ huynh hoặc Gia sư. Quyền Quản trị không được cấp qua đăng ký.
- Development lưu thư trong `.local-mail`, ngoài thư mục web và Git. Liên kết đặt mật khẩu hết hạn sau 30 phút.
- `Authentication:PublicOrigin` phải đúng địa chỉ người dùng truy cập để liên kết đặt lại mật khẩu hoạt động.
- Development bật `Authentication:DemoPublishProfiles` để gia sư công khai hồ sơ đã hoàn thiện. Khi tắt, hệ thống áp dụng điều kiện duyệt hồ sơ.
- Khi triển khai, cấu hình SMTP qua biến môi trường `Smtp__Host`, `Smtp__Port`, `Smtp__From`, `Smtp__Username`, `Smtp__Password` và dùng HTTPS. Giữ thông tin bí mật ngoài Git.
- Có thể bổ sung danh sách mật khẩu bị lộ qua `Authentication:PasswordBlocklistFile`, mỗi dòng một mật khẩu. Danh sách phổ biến đi kèm hiện có phạm vi hạn chế.

Role SQL `KLTN_App` chứa quyền của ứng dụng. Người vận hành gán role này cho database user khi cấu hình tài khoản SQL chạy ứng dụng. Chuỗi kết nối hiện dùng Windows Authentication.

## Tệp tải lên

ClamAV và chữ ký phải có tại `tools/clamav`. Khi không quét được tệp, ứng dụng từ chối sử dụng tệp. Nội dung tải lên nằm trong `uploads`, SQL lưu metadata. Sao lưu cả database và thư mục này.

## Nhu cầu nhiều môn

`NhuCauMonHoc` liên kết nhu cầu với các môn bằng khóa chính ghép `(NhuCauId, MonHocId)` và hai khóa ngoại. Một nhu cầu dùng chung mục tiêu, học phí, lịch. Cột `NhuCauHoc.MonHocId` giữ môn đại diện để tương thích các procedure và dữ liệu cũ, danh sách môn nằm trong bảng liên kết.

Database đã cài chạy `11_MultiSubjectNeeds.sql` trên đúng database hiện tại. Script chuyển cả môn đại diện và danh sách JSON cũ trong một giao dịch, kiểm tra đủ dữ liệu rồi mới bỏ cột JSON. Script chạy lại không tạo liên kết trùng. File cài mới 00 đã tích hợp, tạo đủ 38 bảng. Các nhu cầu cũ được giữ riêng.
