# Kiểm thử tài khoản

Bộ kiểm thử chạy qua HTTP và trình duyệt Edge. Dữ liệu kiểm tra được tạo trong database riêng có tên bắt đầu bằng `EduConnect_Verification_`. Script từ chối chạy khi thiếu tên database này.

1. Tạo database thử từ `Database/00_EduConnect_Full.sql`, thay tên `KLTN_Full_V2` bằng tên database thử trong một bản sao của script.
2. Chạy ứng dụng Development tại cổng 5299 với chuỗi kết nối tới database thử. Đặt `Authentication__PublicOrigin=http://127.0.0.1:5299`, bật `Authentication__DevelopmentMailbox` và dùng `Authentication__AccountRequestLimit=120` để chạy liên tiếp nhiều ca từ localhost. Ca lưu minh chứng cần ClamAV hoặc `FileScanning__AllowUnscannedUploads=true` trên tiến trình kiểm thử Development.
3. Chạy bằng Node có Playwright và Edge trên Windows.

```powershell
$env:AUTH_TEST_DATABASE='EduConnect_Verification_Auth_20261106_v3'
$env:AUTH_TEST_URL='http://127.0.0.1:5299'
node tests/Authentication/browser.cjs
```

Máy kiểm thử dùng `sqlcmd` với Windows Authentication, server `lpc:CONGVJP`. Sửa server trong helper SQL nếu chạy trên máy khác.

Kết quả từng ca và ảnh màn hình nằm trong `.backups/login/auth-results`. Có thể đổi thư mục bằng `AUTH_TEST_EVIDENCE`. Bộ ca kiểm tra quyền khách, ba vai trò người dùng, quyền sở hữu nhu cầu, quy tắc một môn mỗi nhu cầu, điều kiện công khai hồ sơ, đặt lại mật khẩu, thu hồi phiên, SignalR khi đăng xuất và giao diện điện thoại.

Bộ kiểm thử thao tác thời gian phiên, khóa tài khoản giả và xóa bộ đếm đăng nhập sai trong database thử. Chỉ chạy với dữ liệu kiểm thử. Các bộ fixture Session cũ cần chuyển sang tài khoản Identity trước khi dùng với schema mới.

Kiểm tra thông báo tự tắt sau 5 giây, vị trí ngoài biểu mẫu, tạm dừng khi rê chuột và đóng thủ công bằng `node tests/Authentication/notifications.cjs`. Dùng `AUTH_TEST_URL` để chọn địa chỉ ứng dụng thử.
