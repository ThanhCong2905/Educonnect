# AJAX trong EduConnect

- `ajax-pages.js`: điều hướng GET theo vùng, pushState/popstate, hủy request cũ, giữ lựa chọn so sánh giữa các trang.
- Tìm kiếm/lọc/sắp xếp/phân trang dùng endpoint Index hiện có và trả Razor partial qua header X-Edu-Partial.
- Trang chủ render bằng Razor lần đầu. Chỉ vùng hoạt động/gợi ý được cập nhật sau mỗi 30 giây hoặc nút Cập nhật, giữ nguyên bộ chọn vai trò và thanh tìm kiếm.
- Chat riêng và chat lớp dùng chung cơ chế gửi AJAX, polling hội thoại khoảng 3 giây. Giữ bản nháp khi polling, nhớ bản nháp theo hội thoại khi chuyển người, không tự cuộn xuống khi đang đọc tin cũ. Gửi từ trang lịch sử sẽ trở về các tin mới nhất.
- Số chưa đọc và danh sách thông báo đang mở cập nhật khoảng 15 giây. Tạm dừng polling khi tab ẩn, kiểm tra ngay khi quay lại.
- Modal lưu thành công ở trang chủ/tìm kiếm cập nhật vùng hiện tại thay vì tải lại toàn trang.
- Lỗi mạng giữ nội dung hiện tại. Form gửi tin lỗi vẫn giữ nội dung và mã chống gửi lặp để thử lại.
- Quyền truy cập, chống giả mạo và ràng buộc nghiệp vụ vẫn được xử lý ở máy chủ. Không thêm bảng hoặc đổi schema.
- Đây là polling HTTP, chưa dùng SignalR/WebSocket; chu kỳ thực tế phụ thuộc thời gian phản hồi.

Kiểm thử: `tests/ajax-pages.cjs` bằng Node >=20 và Playwright/Edge, trên phiên ứng dụng dùng database kiểm thử ở cổng 5281. Kết quả: `docs/qa/ajax-results.json`.
