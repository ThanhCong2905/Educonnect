# Đối chiếu hợp đồng UC1.1–UC1.4

Chỉ chạy trên CSDL SQL Server kiểm thử đã có schema hiện tại. Chương trình từ chối tên DB không bắt đầu bằng `EduConnect_Verification_`, tạo dữ liệu với GUID riêng và không xóa dữ liệu có sẵn.

```powershell
$env:EDU_TEST_CONNECTION='Server=lpc:CONGVJP;Database=EduConnect_Verification_Unified_Final_20261028_213526;Integrated Security=True;Encrypt=False'
dotnet run --project tests/DesignContracts/DesignContracts.csproj --no-launch-profile
```

14 nhóm kiểm tra gồm chữ ký Controller/Service đã chốt trong bảng MT1, giao dịch tạo nhiều môn, quyền người thực hiện, quyền sở hữu, rowVersion, hủy yêu cầu, dữ liệu hồ sơ công khai, hai nhánh tìm kiếm và thứ tự so sánh. Kết quả và ID fixture nằm trong `bin/Debug/net10.0/results.json`.

Để kiểm tra HTTP/biểu mẫu, chạy một bản ứng dụng riêng kết nối đúng DB kiểm thử trên cổng 5283. Không trỏ vào bản ứng dụng thường hoặc DB chính.

```powershell
$env:ConnectionStrings__EduConnect=$env:EDU_TEST_CONNECTION
dotnet run --project EduConnect.Mvc.csproj --no-launch-profile --urls http://127.0.0.1:5283
# Terminal khác, tại thư mục dự án:
node tests/DesignContracts/browser.cjs
```

Trình duyệt sử dụng fixture riêng, gửi dữ liệu thật vào DB kiểm thử và kiểm tra URL cũ/mới, partial view, nhận danh sách mã so sánh, tạo/sửa bằng AJAX, giữ dữ liệu khi xung đột và phiên bản sai định dạng. Kết quả nằm trong `bin/Debug/net10.0/browser-results.json`.

CT03, CT12 và CT13 chưa có đủ chữ ký trong sơ đồ lớp để kết luận khớp thiết kế. Bộ này không tự đặt chữ ký thiết kế cho các mục đó và không thay thế kiểm thử chức năng toàn hệ thống.
