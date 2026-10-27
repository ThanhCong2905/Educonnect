# Hồ sơ gia sư và tìm kiếm theo môn

Áp dụng từ 27/10/2026. Tài liệu này thay thế các mô tả gia sư đăng nhiều nhu cầu nhận lớp trong tài liệu cũ.

## Luồng sử dụng

Gia sư quản lý một hồ sơ tại **Hồ sơ của tôi**. Mỗi môn có phạm vi dạy, mức phí đề xuất và đơn vị riêng (`VND/Gio`, `VND/Buoi`). Có thể thêm môn Khác ngay trong hàng môn. Khu vực là văn bản được chuẩn hóa khoảng trắng; không có bảng địa giới hoặc lịch rảnh gia sư. Gia sư chọn trực tuyến, trực tiếp hoặc cả hai.

Hồ sơ mới ở trạng thái **Bản nháp / Đang ẩn**. Lưu thông tin và bằng cấp trong cùng giao dịch. Gửi duyệt khóa chỉnh sửa. Gia sư có thể hủy gửi duyệt để sửa lại. Quản trị xem toàn bộ hồ sơ và minh chứng, duyệt hoặc yêu cầu chỉnh sửa kèm lý do. Sau khi duyệt hồ sơ vẫn ẩn; gia sư tự bật hiển thị. Muốn sửa hồ sơ đang công khai phải xác nhận ẩn trước; lưu thay đổi đưa hồ sơ về bản nháp và cần gửi duyệt lại.

`TrangThaiDuyet` và `DangHienThi` độc lập. Không có trạng thái “đang mở nhu cầu nhận lớp” trong quy trình gia sư mới. Học sinh/phụ huynh tiếp tục tạo nhu cầu tìm gia sư; gia sư tìm các nhu cầu học này. Máy chủ từ chối tạo/sửa/đổi trạng thái nhu cầu kiểu cũ bằng vai trò gia sư.

## MVC và dịch vụ

| Thành phần | Trách nhiệm |
|---|---|
| `HoSoGiaSuController` | Hiển thị form, lưu thống nhất, đổi trạng thái, xem trước của chính chủ, tải minh chứng có kiểm tra quyền |
| `HoSoGiaSuService` | Vai trò, kiểm tra dữ liệu, giao dịch hồ sơ/môn/tệp, chuyển trạng thái, kiểm duyệt và RowVersion |
| `QuanTri/HoSoController` | Danh sách chờ duyệt, xem toàn bộ hồ sơ và quyết định duyệt |
| `TimKiemService` | Lọc hồ sơ công khai, DTO không có minh chứng/lý do riêng tư, đếm và phân trang theo hồ sơ |
| `KetNoiService` | Kiểm tra lại hồ sơ trong giao dịch tạo liên hệ, giữ nguyên các tương tác đã tồn tại |
| `HoSoLock` | Khóa giao dịch `sp_getapplock` theo ID gia sư, dùng chung khi sửa/ẩn/duyệt/liên hệ |
| `BangCapService` | Đọc minh chứng cho chủ hồ sơ hoặc quản trị; chặn API tải lên/xóa rời, dùng Lưu hồ sơ |

Kiến trúc vẫn là Razor View → Controller → Service → EF Core/SQL Server, Bootstrap và AJAX. Không thêm repository, interface, hệ thống tài khoản hoặc bảng mới.

## Quy tắc tìm kiếm

Chỉ lấy hồ sơ gia sư đã duyệt, bật hiển thị, có hình thức dạy và ít nhất một môn đang sử dụng; mọi môn phải có phí/đơn vị hợp lệ. Một hồ sơ trả về một thẻ dù nhiều môn khớp. Thẻ khi lọc môn chỉ hiển thị các môn đã chọn; khi không lọc và tại chi tiết hiển thị từng môn cùng giá tương ứng, không gộp giá giữa các đơn vị.

Điều kiện môn, phạm vi, phí tối đa và đơn vị được áp dụng trên **cùng hàng môn dạy**. Ví dụ Toán 200.000/giờ và Văn 100.000/buổi không khớp tìm Toán tối đa 150.000/giờ. Danh sách ID môn nhận nhiều giá trị theo điều kiện khớp ít nhất một môn; giao diện bộ lọc hiện chọn một môn. Sắp xếp phí dùng mức nhỏ nhất trong các môn phù hợp cùng đơn vị đã chọn. Đếm trước phân trang, mỗi trang 9 hồ sơ, thứ tự có ID để ổn định. Không lọc lịch rảnh gia sư.

Từ khóa tìm tên, chuyên môn, giới thiệu hoặc tên môn. Khu vực được thu gọn khoảng trắng đầu/cuối/giữa; so sánh không phân biệt hoa thường, có phân biệt dấu tiếng Việt (`Vietnamese_100_CI_AS`). Đây là khớp văn bản, không phải khoảng cách địa lý. Gợi ý người học dựa trên môn trong hồ sơ; gợi ý gia sư ưu tiên môn và hình thức của nhu cầu học, không có phần trăm phù hợp hoặc AI xếp hạng.

`usp_TimKiemGiaSu` được cập nhật để không trả nhiều nhu cầu nhận lớp; từ chối bộ lọc lịch cũ. Giao diện MVC dùng LINQ, không gọi thủ tục tìm kiếm này. Các thủ tục tạo đề xuất/chấp nhận lớp vẫn giữ nguyên.

## Quyền riêng tư và tương tác cũ

Xem trước dùng lại view công khai, chỉ lấy hồ sơ của gia sư trong session, không nhận ID chủ khác từ client. Người ngoài truy cập hồ sơ ẩn nhận 404. DTO công khai không chứa tệp bằng cấp, lý do kiểm duyệt hoặc địa chỉ chi tiết. Minh chứng nằm ngoài wwwroot và chỉ tải qua endpoint kiểm tra quyền.

Ẩn hồ sơ chặn liên hệ mới kể cả gửi trực tiếp POST. Liên hệ đang chờ, liên hệ đã chấp nhận, hội thoại, yêu cầu kết nối và lớp cũ vẫn tồn tại. Hai bên đã được chấp nhận tiếp tục nhắn tin và đề xuất lớp khi hồ sơ ẩn. Mức phí hồ sơ chỉ tham khảo; phí đã thỏa thuận nằm riêng trên yêu cầu/lớp, không đồng bộ khi sửa hồ sơ.

Giao dịch ẩn và gửi liên hệ dùng cùng khóa: giao dịch liên hệ hoàn tất trước thì giữ liên hệ; giao dịch ẩn hoàn tất trước thì từ chối liên hệ sau. RowVersion bảo vệ cập nhật/duyệt bản cũ, kể cả chỉ thay môn hoặc tệp.

**Giới hạn danh tính:** hệ thống hiện dùng session chọn người mẫu, chưa có đăng nhập xác thực. Kiểm tra vai trò máy chủ hoạt động theo session này, nhưng người dùng môi trường demo có thể chọn vai trò quản trị. Không xem đây là cơ chế phân quyền dùng trên Internet; cần triển khai xác thực và quản lý tài khoản trước khi đưa vào sử dụng thật.

## Cơ sở dữ liệu và nâng cấp

Giữ nguyên 24 bảng. Thêm vào `HoSoGiaSu`: `DangHienThi`, `DayTrucTuyen`, `DayTrucTiep`, `GuiDuyetLuc`, `DuyetLuc`, `NguoiDuyetId`. Thêm vào `HoSoGiaSuMonDay`: `HocPhiDeXuat decimal(18,2)` và `DonViHocPhi varchar(16)`. Dùng RowVersion có sẵn; không tạo bảng lịch rảnh, địa điểm hay đánh giá.

Script 08 cập nhật CHECK trạng thái/role/thông báo, CHECK cặp phí–đơn vị và hiển thị, FK người duyệt, chỉ mục lọc; cập nhật view/thủ tục tìm kiếm, thêm trigger chặn liên hệ vào hồ sơ ẩn. Bỏ trigger cũ buộc giữ môn vì nhu cầu nhận dạy cũ, không xóa nhu cầu/lớp. Role `QuanTri` dùng bảng người tham gia hiện có.

Chuyển đổi lần đầu: chỉ sao chép giá khi cùng gia sư–môn có duy nhất một cặp phí/đơn vị hợp lệ. Nhiều cặp mâu thuẫn hoặc không có giá cũ → để NULL, gia sư nhập lại trước gửi duyệt. Không tự thêm môn từ nhu cầu cũ vào hồ sơ. Suy ra hình thức dạy từ nhu cầu nhận lớp cũ để gia sư kiểm tra lại. Hồ sơ cũ chuyển về **ẩn/bản nháp**, phải hoàn thiện và duyệt lại. Extended property `ProfileWorkflowV1` ngăn chuyển đổi lại khi chạy lại 08.

- Máy mới: chạy `00_Original_DB02.sql` đúng một lần; file đã chứa nâng cấp 08.
- DB đang dùng: chạy `08_ProfilePreflight.sql`, sao lưu DB và uploads, rồi chạy `08_ProfileWorkflow.sql` trên đúng database. Có thể dùng `Setup.ps1 -Existing` sau sao lưu.
- Không chạy lại 00 trên DB đang có dữ liệu. Các file 01–07 là lịch sử nâng cấp cũ, không phải danh sách phải chạy tiếp sau 08.
- Mã ứng dụng mới cần schema 08. Nếu khôi phục backup trước 08, phải triển khai lại phiên bản code tương ứng hoặc chạy nâng cấp lại; không chỉ xóa cột.

## Kiểm chứng ngày 27/10/2026

`tests/ProfileWorkflow` chạy 31 kiểm thử dịch vụ trên `EduConnect_Verification_20261025`, gồm quyền, trạng thái, riêng tư, giá cùng môn/đơn vị, SQL sorting, phân trang 11 gia sư/2 môn thành 9+2, lớp giữ giá và hai thứ tự giao dịch đồng thời. Kết quả tại [profile-results.json](qa/profile-results.json). Test tạo dữ liệu riêng và chỉ cho phép DB có tiền tố `EduConnect_Verification_`.

`tests/profile-ui.cjs` chạy Edge headless: thêm/bỏ môn, nhập môn khác, lưu, gửi duyệt, quản trị duyệt, gia sư bật hiển thị, xác nhận ẩn, chặn chi tiết ẩn và trang quản trị với người học, kiểm tra màn hình 390px và lỗi JavaScript. SQL00 đã cài thành công trên DB mới `EduConnect_Verification_ProfileFresh_20261027`. Build không cảnh báo/lỗi. Chưa đo hiệu năng trên dữ liệu lớn hoặc kiểm thử xác thực thực tế vì ứng dụng chưa có đăng nhập.

DB chính `KLTN_4NghiepVu_V2` đã sao lưu COPY_ONLY/CHECKSUM và RESTORE VERIFYONLY thành công, rồi chạy 08. Giữ nguyên 6 nhu cầu (3 nhận dạy cũ), 2 liên hệ, 1 yêu cầu kết nối, 1 lớp; số dòng và CHECKSUM_AGG trước/sau của bốn bảng này không đổi. CHECKSUM là kiểm tra hỗ trợ, bản sao lưu là cơ sở phục hồi. Vẫn 24 bảng. Hồ sơ mẫu chuyển ẩn/bản nháp; môn Toán chưa có giá nguồn, môn giải tích có hai giá mâu thuẫn nên đều NULL; giá Hóa được chuyển. Backup và log nằm `.backups/`, không đưa vào Git.

## Tài liệu cần đối chiếu

[Sơ đồ hiện tại](diagrams/ProfileWorkflow.drawio), tài liệu này, README, MAPPING và CHECKLIST đã bổ sung luồng mới. Hai file Word trong docs và các sequence diagram UC1.1/UC1.2/UC2.1 cũ chưa được vẽ/biên soạn lại; các mô tả gia sư tạo nhu cầu nhận lớp trong đó đã lỗi thời. Không tìm thấy bản SRS gốc được quản lý trong workspace; cần cập nhật bản nộp chính thức và ERD/SRS ngoài workspace thủ công. Sơ đồ mới mô tả phần thay đổi, không thay thế ERD toàn bộ 24 bảng.


