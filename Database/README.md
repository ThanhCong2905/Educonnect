# Cài đặt cơ sở dữ liệu

Chỉ dùng **00_Original_DB02.sql** khi cài mới. File đã hợp nhất các thay đổi cần cho tính năng hiện tại, kể cả quy trình hồ sơ, duyệt/ẩn hiện, phí theo môn và metadata tệp tin nhắn. Các file SQL khác là script nâng cấp hoặc sửa dữ liệu cũ, không phải database riêng và không cần chạy thêm khi cài mới.

1. Kết nối SQL Server trong SSMS.
2. Mở `00_Original_DB02.sql` với encoding UTF-8 và chạy toàn bộ file. Database mặc định là `KLTN_4NghiepVu_V2`; nếu đổi tên, thay tất cả tên database trong file trước khi chạy.
3. Cập nhật `ConnectionStrings:EduConnect` trong cấu hình ứng dụng để trỏ tới đúng server và database vừa tạo. Kiểm tra cả cấu hình Development hoặc biến môi trường nếu có ghi đè.
4. Chạy ứng dụng bằng `dotnet run`.

Có thể dùng dòng lệnh thay SSMS:

```powershell
sqlcmd -S TEN_SERVER -d master -E -C -I -b -f 65001 -i .\Database\00_Original_DB02.sql
```

Không chạy file cài mới trên database đang sử dụng. Script phát hiện database đã tồn tại; với sqlcmd phải giữ `-b` để dừng ngay khi có lỗi. Trong SSMS, các batch DDL vẫn có thể báo lỗi biên dịch sau thông báo database đã tồn tại; không tiếp tục chạy từng đoạn để vượt qua kiểm tra này.

Dữ liệu người dùng mẫu dùng cơ chế chọn vai trò bằng Session của phiên bản hiện tại, không phải hệ thống tài khoản ASP.NET Identity trong sơ đồ thiết kế. SQL không tự bổ sung các tính năng chưa được lập trình.

Tệp bằng cấp, ảnh và tài liệu tải lên được lưu trong thư mục `uploads`; SQL lưu metadata. Khi chuyển hệ thống có dữ liệu, cần sao lưu/khôi phục cả database SQL Server và thư mục này. Script cài mới không mang theo dữ liệu thực tế đang có.
