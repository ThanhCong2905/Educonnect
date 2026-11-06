# Kiểm thử tài khoản

Bộ kiểm thử chạy qua HTTP và trình duyệt Edge. Dữ liệu kiểm tra được tạo trong database riêng có tên bắt đầu bằng `EduConnect_Verification_`. Script từ chối chạy khi thiếu tên database này.

1. Tạo database thử từ `Database/00_EduConnect_Full.sql`, thay tên `KLTN_Full_V2` bằng tên database thử trong một bản sao của script.
2. Chạy ứng dụng Development tại cổng 5299 với chuỗi kết nối tới database thử. Đặt `Authentication__PublicOrigin=http://127.0.0.1:5299` và `Authentication__AccountRequestLimit=1000` để bộ ca liên tiếp không bị bộ giới hạn đăng ký chặn.
3. Chạy bằng Node có Playwright và Edge trên Windows.

```powershell
$env:AUTH_TEST_DATABASE='EduConnect_Verification_Auth_20261106_v3'
$env:AUTH_TEST_URL='http://127.0.0.1:5299'
node tests/Authentication/browser.cjs
```

Máy kiểm thử dùng `sqlcmd` với Windows Authentication, server `lpc:CONGVJP`. Sửa server trong helper SQL nếu chạy trên máy khác.

Kết quả từng ca và ảnh màn hình nằm trong `.backups/login/auth-results`. Bộ ca kiểm tra ba vai trò, chống giả mạo quyền, quyền sở hữu nhu cầu, công khai hồ sơ demo, quên mật khẩu, dùng lại token, thu hồi cookie, SignalR khi đăng xuất, thời gian chờ đăng nhập sai và giao diện điện thoại.

Bộ kiểm thử thao tác thời gian phiên, khóa tài khoản giả và xóa bộ đếm đăng nhập sai trong database thử. Chỉ chạy với dữ liệu kiểm thử. Các bộ fixture Session cũ cần chuyển sang tài khoản Identity trước khi dùng với schema mới.
