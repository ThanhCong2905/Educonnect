# Đối chiếu màn hình và hành động

Ký hiệu: `[x]` đã có mã và luồng; mức xác minh cụ thể xem JSON kết quả. Không coi mọi chức năng đã có mã là đã được kiểm thử toàn bộ tổ hợp.

Quy trình gia sư mới: [hồ sơ theo môn](profile-workflow.md), [31 kiểm thử thực tế](qa/profile-results.json). Đã kiểm tra thêm Edge desktop/mobile cho gửi duyệt–duyệt–hiển thị–ẩn và quyền truy cập. Các JSON kiểm thử cũ không phải kết quả cho quy trình mới.

- [x] Trang chủ: tìm kiếm, chọn vai trò/người mẫu, gợi ý theo môn và lý do phù hợp, gợi ý tạo nhu cầu, số lớp truy vấn SQL.
- [x] Điều hướng: tìm kiếm trước nhu cầu, breadcrumb, một menu tin nhắn, một avatar sidebar, footer, responsive.
- [x] Nhu cầu: tạo/sửa, nháp, mở/đóng/mở lại, chọn nhiều môn và tạo một nhu cầu riêng cho mỗi môn; một mức phí/một đơn vị trên mỗi nhu cầu, lịch tuần nhiều dòng, chặn chồng lấn.
- [x] Hồ sơ gia sư: giá/đơn vị và phạm vi theo môn, hình thức dạy, gửi duyệt, yêu cầu sửa có lý do, tự bật hiển thị, xem trước chính chủ, minh chứng chỉ chủ/quản trị. Bỏ môn vẫn giữ lịch sử; không tạo nhu cầu nhận lớp mới.
- [x] Phụ huynh: thông tin người học riêng tư; sao chép vào lớp qua procedure; không tạo tài khoản học sinh.
- [x] Tìm kiếm: một hồ sơ/gia sư, phí đúng môn/đơn vị, hồ sơ đã duyệt và hiển thị; từ khóa, phạm vi, hình thức, khu vực, trình độ, sắp xếp/phân trang. Lịch rảnh chỉ lọc nhu cầu học của người học.
- [x] Tìm kiếm hiển thị từng nhu cầu công khai riêng, kể cả nhiều nhu cầu cùng một gia sư; phân trang/tổng số theo nhu cầu. Chi tiết có grid các nhu cầu khác của cùng người đăng, kèm học phí/lịch riêng.
- [x] So sánh 1–3 lựa chọn có kiểm tra ở máy chủ; không có chức năng đối chiếu nhu cầu trong trang tìm kiếm.
- [x] Liên hệ: gửi, nhận, chấp nhận, từ chối, rút; chỉ mở hội thoại sau khi chấp nhận.
- [x] Kết nối: bên học gửi lời nhắn; gia sư đề xuất lớp; điều kiện và khung giờ có cấu trúc; sửa gửi xác nhận lại; hạn 72 giờ.
- [x] Chấp nhận đúng bên học; procedure bảo đảm một lớp cho một đề xuất; thông tin thỏa thuận là bản chụp.
- [x] Lớp: tổng quan, lịch, buổi học, bài tập, tin nhắn; thành viên/quyền trong menu tùy chọn.
- [x] Quyền thành viên: đề nghị, gia sư duyệt/từ chối, quyền `Xem`/`ChinhSua`, rời lớp.
- [x] Kết thúc lớp do hai bên xác nhận; yêu cầu hủy, phản hồi và tự chấp nhận sau hạn theo DB.
- [x] Lịch: chọn nhiều ngày theo tháng, giờ chung, chỉnh riêng từng ngày, xem trước, lưu nguyên giao dịch, kiểm tra chồng lấn.
- [x] Sửa/hủy lịch chưa bắt đầu; không có nút học bù riêng hoặc tạm dừng lớp.
- [x] Buổi học: danh sách xuất phát từ lịch; chi tiết; ghi nhận sau giờ kết thúc, chặn lịch hủy; không diễn ra có lý do; lịch sử sửa và rowversion.
- [x] Bài tập: giao bài, nội dung/tệp, hạn nộp, lịch sử nhiều lần nộp trước khi chấm, nộp muộn, chấm lần mới nhất, nhận xét không bắt buộc điểm.
- [x] Tệp: tối đa 5, kiểm tra dung lượng/định dạng/chữ ký, lưu ngoài webroot, tải qua kiểm tra quyền.
- [x] Tin nhắn: tìm cuộc hội thoại và nội dung, hộp thư riêng chỉ chứa hội thoại cá nhân, chat lớp chỉ truy cập trong lớp, phân trang, chống gửi lặp, lưu SQL.
- [x] Thông báo: sự kiện gắn với hành động nghiệp vụ, chưa đọc/đã đọc, điều hướng theo đối tượng, chỉ chủ sở hữu được đọc.
- [x] POST có antiforgery; Service kiểm tra quyền và trạng thái; lỗi không lộ stack trace/connection string.

## Xác minh đã chạy

- Build .NET/Razor: thành công, không warning/error ở lần build cuối được ghi nhận.
- 38 kiểm thử tích hợp với SQL Server riêng: xem `integration-results.json`.
- 3 kiểm tra trình duyệt lưu hồ sơ và tệp bằng cấp trong cùng biểu mẫu, giữ dữ liệu khi tệp sai và xóa tệp khi lưu: `qa/profile-save-results.json`.
- 4 kiểm tra bố cục liên hệ, modal gửi kết nối, ba nhu cầu cùng gia sư và grid liên quan desktop/mobile: `qa/contact-cards-results.json`.
- 9 kiểm tra AJAX: tìm kiếm/sắp xếp, phân trang/Back/so sánh, chống phản hồi cũ, trang chủ, thông báo đã đọc, chat riêng/lớp hai phiên, giữ bản nháp/vị trí đọc: `qa/ajax-results.json`.
- 24 kiểm tra Edge/HTTP/giao diện ở 1440, 768 và 390px: `qa/browser-results.json`; không có lỗi JavaScript hoặc tràn ngang toàn trang.
- 8 kiểm tra partial modal/giữ dữ liệu/validation/chọn hình thức/thêm dòng/toast/so sánh mobile: `qa/modernization-results.json`.
- 8 kiểm tra trạng thái hồ sơ/liên hệ, bỏ gạch chân hover, dấu sao, phát hiện giờ trùng, xóa hết khung giờ bản nháp và lịch khởi tạo rỗng: `qa/form-usability-results.json`.
- 5 bước luồng đề xuất → tạo lớp → giao bài → nộp bài → chấm bài bằng Edge: `qa/workflow-results.json`.

## Phần cần hoàn thiện trước khi dùng ngoài môi trường thử nghiệm

- Xác thực thật, triển khai HTTPS/quản lý secrets theo môi trường, quy trình duyệt hồ sơ: ngoài phạm vi hiện tại.
- Công cụ quét mã độc cho tệp; kiểm tra chữ ký hiện có không thay thế antivirus.
- Tin nhắn cập nhật qua AJAX polling khoảng 3 giây, thông báo 15 giây và dữ liệu trang chủ 30 giây khi tab hiện. SignalR/email/push và nhắc lịch tự động chưa triển khai.
- Biểu mẫu phụ và so sánh dùng Razor PartialView trong Bootstrap modal qua endpoint hiện có; không dùng iframe trong modal. Form trực tiếp vẫn là phương án dự phòng khi không có JavaScript. Lịch có chọn từng ngày và theo thứ.
- Danh sách buổi học có lọc tháng/trạng thái tại máy chủ; các khoảng thời gian hiển thị theo UTC+7.
- Với JavaScript: lỗi nhập liệu được trả theo trường, giữ nguyên DOM và dữ liệu; lỗi nghiệp vụ hiện trong biểu mẫu. Toast chỉ xác nhận ngắn, tự ẩn sau 4,5 giây. Lỗi hệ thống hoặc truy cập trực tiếp bằng GET vẫn có thể dùng trang lỗi chung.
- Chưa có kiểm thử tải, nhiều trình duyệt đồng thời, tác vụ scanner hoặc đầy ổ đĩa. Xung đột rowversion và rollback lịch đã được kiểm thử bằng SQL thật.
