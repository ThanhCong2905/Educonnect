# Điều chỉnh giao diện và tương tác — 28/10/2026

Phạm vi đã được người dùng duyệt sau báo cáo khảo sát: giữ MVC, Razor, Bootstrap 5.3.3, màu chủ đạo, cấu trúc trang, vị trí chức năng và nghiệp vụ. Không đổi Controller, Service, Model, ViewModel, API, SQL hoặc package.

## Thay đổi

- CSS dùng chung: hover màu/viền 150 ms, giữ màu mục menu đang chọn, focus bàn phím; nút disabled/active dùng màu thương hiệu thay vì xanh mặc định Bootstrap. Nút phụ nền trắng; nút hủy lịch/yêu cầu hủy dùng đỏ `#B42318`, nền hover `#FFF8F7`. Trạng thái duyệt xanh `#EFF8F3`/`#236447`, cần sửa/chờ duyệt vàng `#FFF3CD`/`#664D03`; giữ nguyên nhãn chữ.
- Trang chủ, thẻ tìm kiếm và quản lý nhu cầu: câu rỗng theo vai trò, nhãn lớp thống nhất, so sánh dùng “hồ sơ”/“nhu cầu”. Giữ nguyên grid và bốn thống kê.
- Hồ sơ và quản trị: làm rõ trạng thái duyệt, ưu tiên Bật hiển thị khi đã duyệt; bỏ câu xem trước trùng và rút gọn trạng thái chưa có minh chứng. Giữ hướng dẫn riêng tư, tệp và giá thỏa thuận.
- Form AJAX: đọc action qua `getAttribute` để không bị nút `name="action"` che thuộc tính URL. Không đổi tham số server. Loading theo thao tác, giữ chiều rộng nút và phục hồi nội dung nút sau lỗi.
- Modal: ghi nhận dữ liệu ban đầu; đóng modal đã thay đổi sẽ hỏi ngay trong modal, cho tiếp tục nhập hoặc chủ động bỏ thay đổi. Modal chưa sửa đóng bình thường. Trong lúc gửi chặn đóng; khi phản hồi thành công cho đóng bình thường. Không lưu bản nháp vào DB/localStorage.
- Tìm kiếm AJAX: phục hồi focus về trường tương ứng hoặc tiêu đề kết quả, thông báo số kết quả qua vùng live; giữ URL và lịch sử.
- Menu mobile: đóng bằng Escape, chạm/click bên ngoài hoặc khi focus đi ra ngoài; Escape trả focus về nút menu. Không thêm thư viện.
- Chat: giữ tin nhắn riêng và trong lớp tách biệt, giữ iframe hiện có. Điều chỉnh chiều cao/flex để tài liệu iframe không cuộn lồng; header xuống hàng hợp lý ở màn hình nhỏ. Cuộn đến tin mới và trường lỗi tôn trọng giảm chuyển động.
- Toast thành công giữ thời lượng 4,5 giây. Lỗi vẫn ở biểu mẫu để sửa; không xóa hướng dẫn bắt buộc hoặc cảnh báo quan trọng.

## File sản phẩm đã sửa

- `wwwroot/css/app.css`
- `wwwroot/js/forms.js`, `app.js`, `overlays.js`, `ajax-pages.js`
- `Views/Home/Index.cshtml`
- `Views/Shared/_Candidate.cshtml`, `_CompareBar.cshtml`
- `Areas/NhuCau/Views/HoSoGiaSu/Sua.cshtml`, `NhuCau/Index.cshtml`
- `Areas/TimKiem/Views/TimKiem/ChiTiet.cshtml`
- `Areas/HocTap/Views/TinNhan/Index.cshtml`, `LopHoc/ChiTiet.cshtml`, `LichHoc/Sua.cshtml`, `LichHoc/Buoi.cshtml`
- `Areas/QuanTri/Views/HoSo/Index.cshtml`, `ChiTiet.cshtml`

## Kiểm chứng

Dùng bản sao mã nguồn riêng tại `%TEMP%/EduConnect-UxAudit-20261028`, chỉ bản sao này bỏ đăng ký `BaoTriWorker`. Program.cs trong dự án chính giữ nguyên. Bản khảo sát chỉ đọc dữ liệu; trình duyệt kiểm thử chỉ cho POST chọn người mẫu (session) đến server, chặn và mô phỏng phản hồi cho mọi POST nghiệp vụ, gồm cả đọc thông báo. Không chạy nâng cấp SQL hoặc các bộ kiểm thử ghi dữ liệu.

Chụp cùng 17 màn hình trước/sau: trang chủ, nhu cầu, liên hệ, kết nối, tin nhắn, danh sách lớp, lớp tổng quan, chat lớp, bài tập, thông báo, tìm kiếm, quản lý hồ sơ, xem trước hồ sơ, thêm lịch, buổi học, danh sách kiểm duyệt và chi tiết kiểm duyệt. Ảnh ở `%TEMP%/edu-ux-before` và `%TEMP%/edu-ux-after`.

`tests/ux-polish.cjs` kiểm tra AJAX/focus/history, so sánh, menu mobile, màu trạng thái, modal không sửa/giữ nháp/bỏ nháp, đóng modal sau phản hồi thành công, form liên hệ có trường action, toast, giảm chuyển động, cảm ứng mô phỏng, không tràn ngang và không tràn tài liệu iframe tại 390/768/1440px. Kết quả đã lưu tại [qa/ux-polish-results.json](qa/ux-polish-results.json).

Chạy test chỉ với bản khảo sát đã tắt worker trên cổng 5281; test không tự khởi chạy hay tắt worker của ứng dụng chính. Không chạy test vào phiên ứng dụng thông thường có tác vụ nền. Cần Playwright có sẵn và Microsoft Edge; không thêm dependency cho ứng dụng.

## Giới hạn

Các phản hồi ghi nghiệp vụ được mô phỏng nên đây không phải kiểm thử end-to-end lưu dữ liệu, tải bằng cấp, duyệt hồ sơ hay tạo lớp. Không chạy lại toàn bộ kiểm thử nghiệp vụ. Cảm ứng được mô phỏng trong trình duyệt, chưa kiểm tra điện thoại vật lý, Safari hoặc trình đọc màn hình. Trạng thái dữ liệu không có trong seed (ví dụ hồ sơ đang chờ duyệt) được kiểm tra ở mã/view, không tự tạo dữ liệu để chụp. Cảnh báo bản nháp áp dụng khi đóng modal, không phải khôi phục sau đóng trình duyệt hoặc mất điện.
