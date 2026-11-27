# Kết quả Integration — bộ 187 TC

Thời điểm: 27/11/2026 18:58:00 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Pass': 132, 'Fail': 43, 'Chưa đủ phạm vi TC': 12}. Kết quả script xUnit: {'Pass': 144, 'Fail': 43}.

**Kết luận Pass chỉ áp dụng cho các assertion đã triển khai; TC thiếu assertion được đánh dấu riêng. Ca xUnit Skip được ghi là chưa thực thi.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-001 | Một tài khoản chỉ có một vai trò | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-002 | Khóa ghép nhu cầu–môn và khóa ngoại | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-003 | Đề xuất SQL kiểm tra liên hệ và môn | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-004 | Chấp nhận SQL tạo đúng một lớp và dữ liệu con | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-005 | Chấp nhận SQL thất bại phải rollback | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-006 | Procedure lịch và trigger tạo BuoiHoc | Chưa đủ phạm vi TC | Pass | Chưa có DL02 batch lỗi để chứng minh rollback. |
| IT-007 | Procedure ghi nhận buổi kiểm người thực hiện | Chưa đủ phạm vi TC | Pass | Đã kiểm actor H1 sai quyền; chưa thay bằng Gia sư G2 như DL02 trong Excel. |
| IT-008 | Nộp bài SQL giữ lịch sử và chống lặp mã | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-009 | Chấm SQL kiểm điểm, quyền và lần mới nhất | Chưa đủ phạm vi TC | Pass | Đã kiểm H1 sai quyền; chưa có Gia sư G2 riêng như Excel. |
| IT-010 | Procedure lỗi đăng nhập và thời gian chờ | Fail | Fail | TC yêu cầu chờ 0/1/5/15 phút, procedure hiện tại quy định 30/60/120/300 giây. Không tự kết luận SQL sai trước khi đối chiếu thiết kế/SRS. |
| IT-011 | Procedure phiên kiểm chủ, thu hồi và hết hạn | Chưa đủ phạm vi TC | Pass | Chưa có DL tài khoản BiKhoa và đối chiếu hoạt động nền/chủ động. |
| IT-012 | Principal ứng dụng bị chặn sửa/xóa nhật ký | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-013 | EF–SQL round-trip trạng thái, khóa phiên bản và mốc kết thúc | Chưa đủ phạm vi TC | Pass | Đã round-trip LopHoc và RV; chưa đủ YeuCauHuyKetNoi/thời điểm UTC+07/TamDung. |
| IT-014 | Service tạo nhu cầu nhiều môn và đọc lại | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-015 | Lỗi quan hệ môn không được lưu một phần nhu cầu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-016 | Hủy request khi chờ khóa SQL không để ghi một phần | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-017 | Binding form tạo nhu cầu từ HTTP | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-018 | Lưu nháp thiếu mục tiêu và lịch | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-019 | Phụ huynh đứng tên nhu cầu của người học | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-020 | Khách hoặc gia sư không tạo nhu cầu học | Fail | Fail | Authorize(Roles=HocSinh,PhuHuynh) ở Controller chặn Gia sư bằng403 trước action; Excel lại yêu cầu422. Không có bằng chứng vượt quyền. |
| IT-021 | Hạn mức nhu cầu mở, nháp và lượt đăng/ngày | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-022 | Chuẩn hóa trùng nhu cầu trước khi ghi | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-023 | Cập nhật thay môn/lịch, không đẩy như bài mới | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-024 | Sai chủ hoặc phiên bản cũ không ghi đè | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-025 | Validation form, địa giới và học phí đi qua đúng khớp nối | Fail | Fail | Một DL Excel yêu cầu422 nhưng HTTP trả200. Xem fieldUnderTest và http trong Evidence để xác định trường; chưa mặc định mọi DL validation đều sai. |
| IT-026 | Đóng/mở lại và xác nhận còn nhu cầu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-027 | Form ghi thiếu CSRF không gọi nghiệp vụ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-028 | Service lưu hồ sơ, môn và minh chứng trong cùng giao dịch | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-029 | Service tìm kiếm ghép hồ sơ, môn, phí và đánh giá | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-030 | Gợi ý có lý do, không tự tạo kết nối | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-031 | Multipart hồ sơ giữ đúng liên kết minh chứng | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-032 | Thiếu minh chứng không lưu một phần hồ sơ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-033 | Hồ sơ phiên bản cũ không xóa minh chứng mới | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-034 | Duyệt hồ sơ làm thay đổi điều kiện công khai | Fail | Fail | Admin service review and guest search are tested, but the admin review Controller route/response are not. |
| IT-035 | Sửa nghề nghiệp/họ tên khi công khai phải theo điều kiện ẩn và duyệt lại | Chưa đủ phạm vi TC | Pass | Both workflow branches are tested at HoSoGiaSuService/SQL level; the TrangThai/Luu HTTP posts are not. |
| IT-036 | Quyền xem trước và tải minh chứng | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-037 | Khách xem hồ sơ và nhu cầu công khai | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-038 | Lọc nhiều tiêu chí với mức phí đúng môn và đơn vị | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-039 | Hồ sơ và nhu cầu bị ẩn/khóa không lọt kết quả | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-040 | Phân trang và sắp xếp ổn định trên dữ liệu thật | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-041 | Bộ lọc sai kiểu không bị bỏ qua | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-042 | So sánh đủ điều kiện và loại Id trùng | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-043 | So sánh sai số lượng, loại hoặc điều kiện công khai | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-044 | Service gửi/chấp nhận liên hệ ghép hội thoại và thông báo | Chưa đủ phạm vi TC | Pass | Đã kiểm liên hệ, hội thoại, notification; chưa kiểm đầy đủ payload/outbox chống nhân bản. |
| IT-045 | Service gửi đề xuất hai chiều và kiểm vai trò bên nhận | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-046 | Hai gửi liên hệ đồng thời không nhân bản quan hệ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-047 | Gửi liên hệ qua HTTP giữ danh tính từ phiên | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-048 | Chặn liên hệ không hợp lệ trước và trong giao dịch | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-049 | Phản hồi liên hệ đúng bên và phiên bản | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-050 | Chưa chấp nhận liên hệ không nhắn tin hoặc đề xuất | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-051 | Form đề xuất đầy đủ từ cả hai phía | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-052 | Chỉ bên gửi được sửa đề xuất đang chờ | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-053 | Thiếu dữ liệu lớp hoặc mảng lịch bị loại | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-054 | Gửi/chấp nhận kiểm lại môn và nhu cầu bị ẩn | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-055 | Từ chối và rút theo đúng bên tham gia | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-056 | Điểm vào chuyển tiếp không bỏ kiểm tra actor và liên hệ | Fail | Fail | Người ngoài nhận404, Excel yêu cầu403; không chứng minh người ngoài tạo được yêu cầu. Đối chiếu quy tắc che sự tồn tại của liên hệ trước khi kết luận lỗi nghiệp vụ. |
| IT-057 | Service chấp nhận ghép yêu cầu, lớp và các bảng con | Chưa đủ phạm vi TC | Pass | Fixture một lịch; Excel nêu hai lịch dự kiến. |
| IT-058 | Service yêu cầu/phản hồi kết thúc ghi đủ dữ liệu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-059 | Lỗi sinh lịch phải rollback toàn bộ tạo lớp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-060 | Xác nhận lặp/đồng thời không tạo hai lớp | Chưa đủ phạm vi TC | Pass | Đã retry và gọi đồng thời sau lần chấp nhận đầu; chưa race hai xác nhận đầu tiên. |
| IT-061 | Không hai yêu cầu kết thúc đang chờ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-062 | HTTP chấp nhận tạo đúng lớp và redirectUrl | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-063 | Không cho người gửi hoặc người ngoài tự chấp nhận | Fail | Fail | Người ngoài không được đọc yêu cầu nên Service trả404 trước nhánh từ chối403. Kiểm tiếp không tạo lớp, không suy diễn đây là vượt quyền. |
| IT-064 | Khi xác nhận phải kiểm lại dữ liệu đã đổi | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-065 | Cả Gia sư và Bên học đều được yêu cầu kết thúc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-066 | Đồng ý/từ chối trước hạn dùng RV yêu cầu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-067 | Người gửi, người ngoài hoặc RV sai không quyết định kết thúc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-068 | Phản hồi sau hạn không kéo dài vì worker chậm | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-069 | Lớp kết thúc giữ lịch sử và chặn thao tác học tập mới | Fail | Fail | Excel yêu cầu409 cho thao tác ghi; quan sát có403. Phân biệt mã hợp đồng HTTP với việc thực tế vẫn chặn ghi, xem response và bảng sau thao tác. |
| IT-070 | Service đề nghị/xác nhận liên kết không tự cấp quyền lớp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-071 | Service lời mời → đồng ý → duyệt ghi đủ quyền | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-072 | Liên kết phụ huynh–học sinh qua HTTP | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-073 | Không nhận liên kết sai vai trò, sai người hoặc RV cũ | Fail | Fail | Script mới có DL01; chưa có sai người/RV cũ. |
| IT-074 | Đề nghị thành viên tại action DeNghi thực tế | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-075 | Người nhận tự đồng ý hoặc từ chối lời mời | Fail | Fail | Đã kiểm HTTP/state ba DL; chưa assert riêng quyền mở lớp sau từng DL. |
| IT-076 | Gia sư duyệt Xem/Chỉnh sửa và quyền nộp riêng | Fail | Fail | Đã kiểm ba cấu hình quyền lưu SQL; chưa thao tác đọc/chat/nộp cho cả ba cấu hình. |
| IT-077 | Duyệt lại điều kiện liên kết và hạn mức trong transaction | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-078 | Bên học/thành viên không duyệt quyền người khác | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-079 | Đổi/thu hồi quyền có hiệu lực ở request kế tiếp | Fail | Fail | Đã kiểm cookie cũ không chat/nộp; chưa kiểm GET đọc còn được phép. |
| IT-080 | Gỡ/rời thành viên không gỡ hai bên chính | Fail | Fail | Đã có DL gỡ G1/H1/P1 đối với P1; chưa DL bảo vệ hai thành viên chính. |
| IT-081 | Hủy liên kết thu hồi quyền thành viên phụ thuộc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-082 | Biết Id/là người nhà không mở lớp khác hoặc chat riêng | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-083 | Service thêm batch ghép LichHoc–BuoiHoc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-084 | Service bắt đầu và ghi nhận buổi giữ Id | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-085 | Hai ghi nhận cùng RV không đè mất dữ liệu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-086 | Không nhân đôi buổi bù còn hiệu lực | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-087 | Form lịch hàng loạt và ngày/giờ local | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-088 | Batch trùng nội bộ hoặc đụng lịch người tham gia phải atomic | Fail | Fail | DL02 dùng lịch hiện hữu cùng lớp; chưa lớp C2 dùng chung G1. |
| IT-089 | Đổi lịch buổi chưa diễn ra giữ buổi và bài tập | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-090 | Không đổi buổi có hoạt động thực tế về tương lai | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-091 | Đến giờ mới bắt đầu được buổi | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-092 | Ghi nhận nội dung, kết quả và tham dự | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-093 | Qua giờ nhưng chưa ghi nhận không tính đã học | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-094 | Nghỉ cần bù chưa biết ngày vẫn được lưu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-095 | Tạo buổi bù giữ liên kết buổi gốc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-096 | Service giao bài và tệp gắn đúng BuoiHoc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-097 | Service nộp/chấm ghép khóa mới nhất và lưu precision | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-098 | Retry cùng mã không tạo lần nộp mới | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-099 | Lưu lần nộp trước, chỉ chấm lần mới nhất | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-100 | Chặn sai gia sư, điểm ngoài miền và precision vượt hai số lẻ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-101 | Nộp và chấm đồng thời kiểm lại trạng thái trong transaction | Chưa đủ phạm vi TC | Pass | Đã dùng hai scope/Task.WhenAll và kiểm invariant; chưa có barrier tại thời điểm tranh khóa. |
| IT-102 | Multipart giao bài qua Controller | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-103 | Buổi tương lai chỉ lưu nháp, công bố khi đủ điều kiện | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-104 | Bài bắt buộc gắn buổi đúng lớp, không giao vào buổi nghỉ/hủy | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-105 | Gắn buổi cho bài cũ chưa có BuoiHocId | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-106 | Nộp trước và sau hạn đều được ghi đúng thời điểm | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-107 | Đã có kết quả chấm không nhận lần nộp mới | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-108 | Quyền nộp thay không suy ra từ quyền chat | Fail | Fail | Một DL bị422 thay vì403 như Excel; cần xem ModelState và nhánh fallback ChiTiet trong BaiTapController.Nop/UiResponseFilter. Không tự kết luận có nộp bài trái quyền; query dữ liệu và giữ oracle theo Excel. |
| IT-109 | Chấm 0/10 hoặc chỉ nhận xét không quy đổi null thành 0 | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-110 | Sửa điểm dùng RV của kết quả, chống ghi đè | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-111 | Tổng hợp tiến độ từ nguồn thật với phép tính đối chiếu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-112 | Hai bên đánh giá độc lập sau khi kết thúc có buổi thực tế | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-113 | Khoảng ngày và trạng thái rỗng không tạo điểm 0 giả | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-114 | Bỏ kết quả lần cũ và bài nháp khi tổng hợp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-115 | Gửi lại không sửa điểm/nhận xét kể cả bị ẩn | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-116 | Tiến độ và liên kết nguồn tại tổng quan lớp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-117 | Người ngoài không đọc tiến độ/điểm | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-118 | Đọc phiếu và lịch sử phản hồi cũ theo quyền lớp | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-119 | Gửi đánh giá xác nhận quyền riêng tư | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-120 | Thiếu điều kiện đánh giá không lưu dữ liệu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-121 | Biên sao và nhận xét đi qua binding rồi nghiệp vụ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-122 | Đối tác phản hồi giữ lịch sử và phát notice | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-123 | Ẩn đánh giá thay đổi tổng hợp và quyền xem đánh giá Bên học | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-124 | TepService ghép scanner process và file sandbox | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-125 | Service gửi tin, tệp và outbox commit đồng bộ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-126 | Mã thao tác tin nhắn chống retry và thay nội dung | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-127 | Đuôi hợp lệ nhưng cấu trúc giả, macro hoặc traversal bị loại | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-128 | Scanner báo nhiễm/lỗi/timeout không được gắn trạng thái sạch | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-129 | Nhánh Development cho tệp chưa quét không áp sang môi trường khác | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-130 | Thu hồi quyền trong lúc quét phải kiểm lại trước gắn tệp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-131 | Lỗi lưu SQL sau ghi blob không để orphan | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-132 | HTTP gửi tin giữ ownership và nội dung | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-133 | Phân trang cùng timestamp và mốc quyền xem | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-134 | Người ngoài, thành viên Xem và lớp kết thúc không gửi được tin | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-135 | Biên số tệp và dung lượng dùng bytes thật | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-136 | Tải tệp kiểm lại quyền, scan state và hash | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-137 | Service quản trị ghép claim quyền, Identity và dữ liệu SQL | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-138 | Service tạo báo cáo ghép quyền xem, bằng chứng và outbox | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-139 | Gỡ một nguyên nhân chưa đủ để tiếp tục lớp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-140 | Quản trị theo quyền chức năng, không chỉ role | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-141 | Yêu cầu sửa hồ sơ cần lý do và không công khai | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-142 | Báo cáo các loại đối tượng được phép xem | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-143 | Không báo cáo đối tượng không được xem hoặc gửi trùng | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-144 | Bằng chứng chỉ người gửi và reviewer đúng quyền được tải | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-145 | Nhận xử lý báo cáo kiểm quyền và phiên bản | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-146 | Kết luận tách trạng thái xử lý khỏi biện pháp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-147 | Bổ sung/xem lại giữ kết luận và không tự gỡ biện pháp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-148 | Hậu kiểm nhu cầu: yêu cầu sửa/ẩn và xem lại | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-149 | Khóa tài khoản thu hồi phiên và lan truyền trạng thái | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-150 | Không tự khóa hoặc loại bỏ quản trị cuối cùng | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-151 | Phân quyền thu hồi phiên mà không đổi vai trò nghiệp vụ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-152 | Danh mục môn đổi tên/ngừng dùng giữ lịch sử và ảnh hưởng tìm kiếm | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-153 | Tra cứu nhật ký an toàn và quản trị theo nguồn thật | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-154 | Service đăng ký ghép Identity, vai trò và hồ sơ | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-155 | Mật khẩu phải được Identity xác minh trước SQL đăng nhập thành công | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-156 | Quên mật khẩu ghép token và mail sandbox | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-157 | Đăng nhập HTTP phát cookie và phiên thật | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-158 | Form đăng ký ba vai trò công khai | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-159 | Trùng email hoặc yêu cầu role quản trị không được cấp | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-160 | Sai mật khẩu, tài khoản khóa và đang chờ | Chưa đủ phạm vi TC | Pass | Đã kiểm bốn nhánh không phát cookie; chưa so sánh thông báo chống lộ tài khoản. |
| IT-161 | ReturnUrl ngoài miền không gây open redirect | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-162 | Đăng xuất thu hồi phiên và cookie cũ | Chưa đủ phạm vi TC | Pass | Đã logout, thu hồi SQL và challenge; chưa replay một bản cookie đã sao chép trước logout. |
| IT-163 | Đổi mật khẩu thu hồi các phiên | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-164 | Form quên mật khẩu không lộ email tồn tại | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-165 | Đặt lại với token đúng, sai, hết hạn và đã dùng | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-166 | Hồ sơ cá nhân dùng ConcurrencyStamp và email chỉ đọc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-167 | Thay/xóa ảnh đại diện ghép metadata, file và quyền xem | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-168 | Phiên hết nhàn rỗi hoặc đã thu hồi không truy cập được | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-169 | Rate limiter/antiforgery không tạo ghi nghiệp vụ ngoài ý muốn | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-170 | Producer lưu notification/outbox trong cùng transaction | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-171 | Audit service/middleware ghép SQL bằng scope độc lập | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-172 | Đọc một/đọc tất cả thông báo đúng người nhận | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-173 | Audit liên thông request thành công/deny/CSRF/tải tệp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-174 | Lỗi lưu audit không hủy giao dịch nghiệp vụ đã commit | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-175 | MVC/filter ánh xạ response đúng kiểu request | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-176 | Hub chỉ nhận phiên thật hợp lệ và kiểm quyền room | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-177 | Worker không phát trước commit rồi phát đúng EventId | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-178 | Transport lỗi giữ nghiệp vụ và retry có backoff | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-179 | Hai worker và lease hết hạn không làm mất sự kiện | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-180 | Thu hồi quyền rời group và không nhận tin mới | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-181 | Ping/notification nền không gia hạn phiên | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-182 | Bảo trì hết hạn đề xuất không tạo lớp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-183 | Hết72 giờ tự kết thúc đúng yêu cầu còn chờ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-184 | Worker và phản hồi đúng hạn cạnh tranh có một kết quả hợp lệ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-185 | Nhắc30 ngày và đóng sau7 ngày không xác nhận | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-186 | Nhắc buổi/bài từ dữ liệu thật không tự chấm hoặc ghi nhận | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-187 | Dừng host không làm mất sự kiện chưa hoàn tất | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |

## IT-002 — Khóa ghép nhu cầu–môn và khóa ngoại

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01 lỗi unique/PK; DL02 lỗi FK. Không coi kiểm metadata là thực thi hành vi tích hợp.
2. Dữ liệu và side effect: Mỗi DL chỉ giữ link ban đầu; nhu cầu N1 không bị xóa hoặc đổi môn đại diện.

**Quan sát/assertion thực tế:**
```text
Assert.Contains() Failure: Item not found in collection
Collection: [547]
Not found:  2601
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 16
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 390
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_002() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 4
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-002-IT8e8799308ec64b728c7501e996a4ecf3.json`

## IT-010 — Procedure lỗi đăng nhập và thời gian chờ

Phân loại: Sai lệch kỳ vọng TC/SQL cần thống nhất.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Số lỗi sau là 5/6/7/8; thời gian chờ tương ứng 0/1/5/15 phút theo thiết kế SQL.
2. Dữ liệu và side effect: Hai loại khóa được ghi đúng giá trị; trong thời gian đang chờ không cộng thêm lỗi; account/IP khác không bị khóa nhầm.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: 0
Actual:   30
```

**Ý nghĩa:** TC yêu cầu chờ 0/1/5/15 phút, procedure hiện tại quy định 30/60/120/300 giây. Không tự kết luận SQL sai trước khi đối chiếu thiết kế/SRS.

Vị trí cần xem trong ứng dụng: `Database/00_EduConnect_Full.sql:1189`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 46
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 46
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 390
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_010() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 12
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-010-ITe84395f0efce4add96c51891aae46323.json`

## IT-014 — Service tạo nhu cầu nhiều môn và đọc lại

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: KetQua.ThanhCong=true; DuLieu=N1 khác Guid.Empty.
2. Dữ liệu và side effect: Một NhuCauHoc, hai NhuCauMonHoc, hai KhungGioRanh, một LuotDangNhuCau; chủ H1; không nhân bản nhu cầu theo môn.

**Quan sát/assertion thực tế:**
```text
Mỗi nhu cầu chỉ chọn một môn trong danh sách.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 18
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 20
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_014() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 16
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-014-IT1f7d706042b04cdfa2ae677f9c45760a.json`

## IT-017 — Binding form tạo nhu cầu từ HTTP

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: HTTP 200 JSON ok=true, redirectUrl về danh sách nhu cầu; thông báo tạo thành công.
2. Dữ liệu và side effect: Dữ liệu chép đúng tất cả môn/lịch/mục tiêu/phí; đúng một nhu cầu và lượt đăng.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   UnprocessableEntity
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 25
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_017() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 19
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-017-IT4d9852b152b943d7bc5e1eaed1d41761.json`

## IT-018 — Lưu nháp thiếu mục tiêu và lịch

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200 ok=true; được lưu nháp và đọc lại dữ liệu đã nhập.
2. Dữ liệu và side effect: TrangThai=BanNhap; không tự sinh lịch 00:00–00:00; không tăng lượt đăng bài công khai.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   UnprocessableEntity
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 25
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_018() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 20
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-018-IT59cd355e0d06410eb572a409bde50cc6.json`

## IT-019 — Phụ huynh đứng tên nhu cầu của người học

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200 ok=true; biểu mẫu hiển thị người học đã mô tả.
2. Dữ liệu và side effect: ChuSoHuuId=P1; giữ tên/tuổi/lớp; không tự tạo tài khoản Học sinh hoặc gộp nhu cầu của H1.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   UnprocessableEntity
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 25
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_019() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 21
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-019-IT2d2966c946fb4801b0998ab0667eea62.json`

## IT-020 — Khách hoặc gia sư không tạo nhu cầu học

Phân loại: Kỳ vọng TC chưa đúng tầng từ chối.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01 HTTP 401; DL02 từ chối quyền (422 qua ModelState của action lưu), ok=false.
2. Dữ liệu và side effect: Không tạo NhuCauHoc/MonHoc/lịch/lượt đăng của bất kỳ DL bị từ chối.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: UnprocessableEntity
Actual:   Forbidden
```

**Ý nghĩa:** Authorize(Roles=HocSinh,PhuHuynh) ở Controller chặn Gia sư bằng403 trước action; Excel lại yêu cầu422. Không có bằng chứng vượt quyền.

Vị trí cần xem trong ứng dụng: `Areas/NhuCau/Controllers/NhuCauController.cs:11`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 27
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_020() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 22
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-020-ITf66330a181114c29b8e481a82c1c7d2b.json`

## IT-022 — Chuẩn hóa trùng nhu cầu trước khi ghi

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Cùng chủ: 422 báo cập nhật bài đã có. Khác chủ: 200, không gộp hai nhu cầu độc lập.
2. Dữ liệu và side effect: Cùng chủ vẫn một nhu cầu; khác chủ có hai nhu cầu riêng đúng ownership.

**Quan sát/assertion thực tế:**
```text
Mỗi nhu cầu chỉ chọn một môn trong danh sách.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 39
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 39
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_022() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 24
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-022-IT5d16066780024838919ae556827528bf.json`

## IT-023 — Cập nhật thay môn/lịch, không đẩy như bài mới

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200 ok=true, thông báo cập nhật.
2. Dữ liệu và side effect: Giữ Id/TaoLuc/chủ, RV đổi; link/lịch cũ được thay đồng bộ; không cộng lượt đăng mới.

**Quan sát/assertion thực tế:**
```text
Mỗi nhu cầu chỉ chọn một môn trong danh sách.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 31
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 31
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_023() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 25
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-023-IT3d15b1a5713c4d8086ad91c3b07bfd33.json`

## IT-025 — Validation form, địa giới và học phí đi qua đúng khớp nối

Phân loại: Cần xác minh nhánh validation cho phép lưu.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Mỗi DL 422 ok=false; chỉ rõ trường nhập sai; dữ liệu biểu mẫu không bị thay bằng thành công.
2. Dữ liệu và side effect: Không có dòng cha/con/lượt đăng; không biến số sai kiểu thành một giá trị hợp lệ ngầm.

**Quan sát/assertion thực tế:**
```text
Request thiếu CSRF phải được audit là Authorization/TuChoi với lý do CSRFKhongHopLe.
```

**Ý nghĩa:** Một DL Excel yêu cầu422 nhưng HTTP trả200. Xem fieldUnderTest và http trong Evidence để xác định trường; chưa mặc định mọi DL validation đều sai.

Vị trí cần xem trong ứng dụng: `Areas/NhuCau/Services/NhuCauService.cs:66`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 35
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 35
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_025() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 27
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-025-IT6eafb35c74e040efbe6e1e294ec403df.json`

## IT-029 — Service tìm kiếm ghép hồ sơ, môn, phí và đánh giá

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: KetQua thành công; chỉ G1, mỗi gia sư một mục; Total và Items thống nhất.
2. Dữ liệu và side effect: Không kết quả từ hồ sơ chưa duyệt/ẩn hoặc phí môn/đơn vị không tương ứng; không ghi dữ liệu.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.InitializeReaderAsync(AsyncEnumerator enumerator, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.MoveNextAsync()
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Areas.TimKiem.Services.TimKiemService.TimDuLieuAsync(BoLocTimKiemViewModel f, NguoiThamGia me, CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Areas\TimKiem\Services\TimKiemService.cs:line 139
   at EduConnect.Mvc.Areas.TimKiem.Services.TimKiemService.<>c__DisplayClass10_0.<<TimKiemAsync>b__0>d.MoveNext() in C:\Khóa luận TN\EduConnect1\Areas\TimKiem\Services\TimKiemService.cs:line 94
--- End of stack trace from previous location ---
   at EduConnect.Mvc.Infrastructure.KetQua`1.ThucHienAsync(Func`1 action) in C:\Khóa luận TN\EduConnect1\Infrastructure\KetQua.cs:line 30
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 20
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 30
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 376
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_029() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 31
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-029-ITdaeb480861b442baade0c4b4e467924f.json`

## IT-030 — Gợi ý có lý do, không tự tạo kết nối

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: H1 ưu tiên đúng mức tiêu chí và có lý do, không phần trăm; G1 chỉ nhu cầu liên quan môn; H2 nhận không có nhu cầu để gợi ý.
2. Dữ liệu và side effect: Không tự tạo YeuCauLienHe/YeuCauKetNoiLopHoc/LopHoc.

**Quan sát/assertion thực tế:**
```text
Assert.Contains() Failure: Filter not matched in collection
Collection: ["Trùng môn và khối lớp"]
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 41
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 43
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 376
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_030() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 32
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-030-IT7416d67435c3477e9337d1c0a44e2e8a.json`

## IT-034 — Duyệt hồ sơ làm thay đổi điều kiện công khai

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Duyệt 200 ok=true; GET tìm kiếm 200 thấy G1 đúng một lần.
2. Dữ liệu và side effect: Trạng thái DaDuyet, reviewer/thời điểm/lịch sử/thông báo đúng; không công khai chứng minh thư/địa chỉ chi tiết.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   InternalServerError
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.ProfileCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProfileCases.cs:line 227
   at Cases.ProfileCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProfileCases.cs:line 229
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 335
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_034() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 36
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-034-ITfaaac4691b944ee2a56b91bea684d5c3.json`

## IT-038 — Lọc nhiều tiêu chí với mức phí đúng môn và đơn vị

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200; mỗi DL chỉ giữ đối tượng đạt đồng thời các điều kiện; giá 0 không bị xem là chưa nhập.
2. Dữ liệu và side effect: Không lấy mức phí môn khác/đơn vị khác để cho lọt kết quả; không nhân đôi gia sư nhiều môn.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.InitializeReaderAsync(AsyncEnumerator enumerator, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.MoveNextAsync()
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Areas.TimKiem.Services.TimKiemService.TimDuLieuAsync(BoLocTimKiemViewModel f, NguoiThamGia me, CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Areas\TimKiem\Services\TimKiemService.cs:line 139
   at EduConnect.Mvc.Areas.TimKiem.Services.TimKiemService.<>c__DisplayClass10_0.<<TimKiemAsync>b__0>d.MoveNext() in C:\Khóa luận TN\EduConnect1\Areas\TimKiem\Services\TimKiemService.cs:line 94
--- End of stack trace from previous location ---
   at EduConnect.Mvc.Infrastructure.KetQua`1.ThucHienAsync(Func`1 action) in C:\Khóa luận TN\EduConnect1\Infrastructure\KetQua.cs:line 30
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 79
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 90
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 376
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_038() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 40
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-038-IT3005a10309a744bea55178e87679a6f4.json`

## IT-039 — Hồ sơ và nhu cầu bị ẩn/khóa không lọt kết quả

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Danh sách HTTP200 không có các Id không công khai; GET trực tiếp chi tiết đối tượng đã ẩn/khóa/chưa duyệt trả404, không trả nội dung riêng. Người ngoài không được dùng URL trực tiếp để vượt bộ lọc.
2. Dữ liệu và side effect: Biện pháp đã gỡ mới hết hiệu lực; lịch sử/chat/lớp hiện có không bị xóa.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.InitializeReaderAsync(AsyncEnumerator enumerator, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.MoveNextAsync()
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Areas.TimKiem.Services.TimKiemService.TimDuLieuAsync(BoLocTimKiemViewModel f, NguoiThamGia me, CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Areas\TimKiem\Services\TimKiemService.cs:line 139
   at EduConnect.Mvc.Areas.TimKiem.Services.TimKiemService.<>c__DisplayClass10_0.<<TimKiemAsync>b__0>d.MoveNext() in C:\Khóa luận TN\EduConnect1\Areas\TimKiem\Services\TimKiemService.cs:line 94
--- End of stack trace from previous location ---
   at EduConnect.Mvc.Infrastructure.KetQua`1.ThucHienAsync(Func`1 action) in C:\Khóa luận TN\EduConnect1\Infrastructure\KetQua.cs:line 30
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 107
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 130
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 376
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_039() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 41
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-039-ITe71a0c361bff4b699477f9187cb6d9e7.json`

## IT-040 — Phân trang và sắp xếp ổn định trên dữ liệu thật

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Mỗi trang tối đa 9; không trùng/bỏ Id khi dữ liệu không đổi; Total=20; rating G1=4.5; số Bên học đếm tài khoản khác nhau, bỏ lớp DaHuy.
2. Dữ liệu và side effect: Không tính đánh giá ẩn hoặc bài chưa công khai; cùng khóa dùng tiêu chí phụ Id ổn định.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.InitializeReaderAsync(AsyncEnumerator enumerator, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.MoveNextAsync()
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Areas.TimKiem.Services.TimKiemService.TimDuLieuAsync(BoLocTimKiemViewModel f, NguoiThamGia me, CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Areas\TimKiem\Services\TimKiemService.cs:line 139
   at EduConnect.Mvc.Areas.TimKiem.Services.TimKiemService.<>c__DisplayClass10_0.<<TimKiemAsync>b__0>d.MoveNext() in C:\Khóa luận TN\EduConnect1\Areas\TimKiem\Services\TimKiemService.cs:line 94
--- End of stack trace from previous location ---
   at EduConnect.Mvc.Infrastructure.KetQua`1.ThucHienAsync(Func`1 action) in C:\Khóa luận TN\EduConnect1\Infrastructure\KetQua.cs:line 30
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 150
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 158
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 376
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_040() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 42
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-040-IT5af6c19e56a54b91a6ebf1347ce85092.json`

## IT-045 — Service gửi đề xuất hai chiều và kiểm vai trò bên nhận

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: KetQua thành công; G1 gửi thì chờ Bên học, H1 gửi thì chờ Gia sư; nhận đúng bên còn lại.
2. Dữ liệu và side effect: Một đề xuất/DL, đầy đủ KhungGioDeXuat; mốc hết hạn 72 giờ; phí/mục tiêu/địa điểm khớp đầu vào; chưa tạo lớp.

**Quan sát/assertion thực tế:**
```text
EduConnect.Mvc.Infrastructure.LoiNghiepVu : Chỉ gia sư được gửi biểu mẫu đề xuất lớp.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at EduConnect.Mvc.Infrastructure.Rules.Check(Boolean condition, String message, Int32 status, String field) in C:\Khóa luận TN\EduConnect1\Infrastructure\BusinessRules.cs:line 47
   at EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService.GuiDeXuatAsync(DeXuatLopViewModel m, CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Areas\KetNoi\Services\KetNoiService.cs:line 185
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 18
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 18
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 18
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 238
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_045() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 47
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-045-DL0-IT1a4cf5ec9286448d99eb7dbddfe41e15.json`, `Evidence\IT-045-DL1-ITfe38bd4d19a94f88a0221aa6783923e8.json`, `Evidence\IT-045-ITcab0578e436842d7a688efeb2373553c.json`

## IT-051 — Form đề xuất đầy đủ từ cả hai phía

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200 ok=true; redirect về danh sách kết nối; điều kiện hiển thị đúng phía nhận.
2. Dữ liệu và side effect: Đầy đủ đề xuất/lịch/hạn72h; lịch gửi không bị binder thay bằng slot mặc định; không tự chấp nhận.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   UnprocessableEntity
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 29
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 29
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 257
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_051() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 53
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-051-G-IT73defaf63ce04707b5dfa7bdf29ccfd8.json`, `Evidence\IT-051-IT7fcc1965ce3f4f25891477027d60bb78.json`

## IT-052 — Chỉ bên gửi được sửa đề xuất đang chờ

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01 200; DL02/03 422 ok=false, lý do quyền/phiên bản.
2. Dữ liệu và side effect: DL01 thay lịch đồng bộ và mốc chờ mới theo thiết kế; DL02/03 giữ phiên bản mới, không xóa lịch.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: 200
Actual:   422
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 31
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 31
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 257
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_052() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 54
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-052-DL0-ITeb5a52ba60a9427e9f4437f69ac7278b.json`, `Evidence\IT-052-IT33478547c4e149b291fe7e37cebf3822.json`

## IT-055 — Từ chối và rút theo đúng bên tham gia

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01/02 200; sai bên/người ngoài 403; giữ lịch sử kết quả thay vì xóa yêu cầu.
2. Dữ liệu và side effect: R1 lần lượt TuChoi/DaRut; không lớp mới; notification/outbox gửi bên còn lại đúng một lần.

**Quan sát/assertion thực tế:**
```text
Microsoft.EntityFrameworkCore.DbUpdateException : An error occurred while saving the entity changes. See the inner exception for details.
---- Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
-------- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Update.Internal.SqlServerModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.SaveChangesAsync(IList`1 entries, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(IList`1 entriesToSave, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(StateManager stateManager, Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Fixture.SeedProposal() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 56
   at Fixture.SeedProposal() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 56
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 35
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 35
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 257
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_055() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 57
--- End of stack trace from previous location ---
----- Inner Stack Trace -----
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-055-DL0-IT791ea75c82cc480bbfd985d704243b8c.json`, `Evidence\IT-055-DL1-IT53f6c9cdedf64142822bcdd9ef6be973.json`, `Evidence\IT-055-ITbd418816eb544efd8c957ce8a09632dd.json`

## IT-056 — Điểm vào chuyển tiếp không bỏ kiểm tra actor và liên hệ

Phân loại: Sai lệch mã lỗi quyền cần thống nhất.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: H1 hợp lệ: HTTP200 ok=true, tạo yêu cầu ChoGiaSu để Gia sư bổ sung đề xuất; X bị403; không tạo lớp ngay từ yêu cầu sơ bộ.
2. Dữ liệu và side effect: Không bỏ qua liên hệ/ownership, không tạo lớp hoặc cấp quyền chat cho X.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: Found
Actual:   InternalServerError
```

**Ý nghĩa:** Người ngoài nhận404, Excel yêu cầu403; không chứng minh người ngoài tạo được yêu cầu. Đối chiếu quy tắc che sự tồn tại của liên hệ trước khi kết luận lỗi nghiệp vụ.

Vị trí cần xem trong ứng dụng: `Areas/KetNoi/Services/KetNoiService.cs:113`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Fixture.Client(Nullable`1 actor) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 64
   at Fixture.Client(Nullable`1 actor) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 64
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 37
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 257
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_056() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 58
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-056-ITe727042da8a040179ec9758155dead9f.json`

## IT-062 — HTTP chấp nhận tạo đúng lớp và redirectUrl

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200 JSON ok=true, redirectUrl=/HocTap/LopHoc/ChiTiet/C1.
2. Dữ liệu và side effect: Một lớp C1 cùng key trong response; đầy đủ dữ liệu con, lịch/buổi; hội thoại riêng vẫn chỉ H1/G1.

**Quan sát/assertion thực tế:**
```text
Microsoft.EntityFrameworkCore.DbUpdateException : An error occurred while saving the entity changes. See the inner exception for details.
---- Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
-------- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Update.Internal.SqlServerModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.SaveChangesAsync(IList`1 entries, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(IList`1 entriesToSave, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(StateManager stateManager, Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Fixture.SeedProposal() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 56
   at Fixture.SeedProposal() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 56
   at Cases.HttpCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\HttpCases.cs:line 43
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 297
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_062() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 64
--- End of stack trace from previous location ---
----- Inner Stack Trace -----
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-062-IT269d5a86bddd42a2af10e6b61fea7300.json`

## IT-063 — Không cho người gửi hoặc người ngoài tự chấp nhận

Phân loại: Sai lệch mã lỗi/quyền cần đối chiếu.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 403 ok=false; không lấy MaNguoiThucHien do client giả mạo.
2. Dữ liệu và side effect: R1 còn chờ; không lớp/lịch/buổi/thông báo chấp nhận.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Người ngoài không được đọc yêu cầu nên Service trả404 trước nhánh từ chối403. Kiểm tiếp không tạo lớp, không suy diễn đây là vượt quyền.

Vị trí cần xem trong ứng dụng: `Areas/KetNoi/Services/KetNoiService.cs:199`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteReaderAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 190
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_063() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 65
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-063-IT1c9d7e36d64542c5a0880e14716df92a.json`

## IT-069 — Lớp kết thúc giữ lịch sử và chặn thao tác học tập mới

Phân loại: Sai lệch mã từ chối khi lớp đã kết thúc.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: GET 200 với lịch sử; các thao tác ghi bị 409, không chỉ ẩn nút trên UI.
2. Dữ liệu và side effect: Giữ dữ liệu trước kết thúc; không TinNhan/BaiTapNop/tệp/notice mới do thao tác bị chặn.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: 409
Actual:   403
```

**Ý nghĩa:** Excel yêu cầu409 cho thao tác ghi; quan sát có403. Phân biệt mã hợp đồng HTTP với việc thực tế vẫn chặn ghi, xem response và bảng sau thao tác.

Vị trí cần xem trong ứng dụng: `Areas/HocTap/Services/TinNhanService.cs`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 47
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 47
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 257
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_069() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 71
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-069-ITbbe134c46bb74afc89754ac351ce892d.json`

## IT-071 — Service lời mời → đồng ý → duyệt ghi đủ quyền

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Trước đồng ý/duyệt chưa có quyền; sau duyệt đủ quyền Xem đúng C1.
2. Dữ liệu và side effect: Một thành viên; thời điểm đồng ý/duyệt và quyền được lưu, notice đúng người; không cấp chat riêng H1/G1.

**Quan sát/assertion thực tế:**
```text
Microsoft.EntityFrameworkCore.DbUpdateException : An error occurred while saving the entity changes. See the inner exception for details.
---- Microsoft.Data.SqlClient.SqlException : Invalid object name 'LichSuQuyenThanhVien'.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Update.Internal.SqlServerModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.SaveChangesAsync(IList`1 entries, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(IList`1 entriesToSave, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(StateManager stateManager, Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at EduConnect.Mvc.Areas.HocTap.Services.LopHocService.DuyetAsync(Guid member, String action, String permission, String version, Boolean choPhepNopBai) in C:\Khóa luận TN\EduConnect1\Areas\HocTap\Services\LopHocService.cs:line 195
   at EduConnect.Mvc.Areas.HocTap.Services.LopHocService.DuyetAsync(Guid member, String action, String permission, String version, Boolean choPhepNopBai) in C:\Khóa luận TN\EduConnect1\Areas\HocTap\Services\LopHocService.cs:line 196
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 20
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 20
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 20
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 271
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_071() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 73
--- End of stack trace from previous location ---
----- Inner Stack Trace -----
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
```

Bằng chứng: `Evidence\IT-071-ITcb1b7053248e46c58f1c149ad8518aaa.json`

## IT-073 — Không nhận liên kết sai vai trò, sai người hoặc RV cũ

Phân loại: Kỳ vọng TC cần đối chiếu.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Sai vai trò bị 400; sai người 403; RV cũ 409; ok=false.
2. Dữ liệu và side effect: Không tự chuyển vai trò hoặc tạo liên kết sai; dữ liệu hợp lệ trước đó giữ nguyên.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: BadRequest
Actual:   NotFound
```

**Ý nghĩa:** GuiDeNghiAsync chỉ tìm email tài khoản Học sinh; nhập Gia sư trả404, trong khi Excel yêu cầu400.

Vị trí cần xem trong ứng dụng: `Areas/HocTap/Services/LienKetTaiKhoanService.cs:25`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 24
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 24
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 271
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_073() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 75
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-073-ITca8da7c69e5a460e8ec2f24b0c8d89ca.json`

## IT-075 — Người nhận tự đồng ý hoặc từ chối lời mời

Phân loại: Sai lệch mã lỗi/quyền cần đối chiếu.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01/02 200; DL03 403. Đồng ý chỉ chuyển chờ Gia sư duyệt, chưa mở quyền lớp.
2. Dữ liệu và side effect: Lưu dấu đồng ý/từ chối; không tự duyệt hoặc cấp quyền cho X.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: Forbidden
Actual:   NotFound
```

**Ý nghĩa:** PhanHoiLoiMoiAsync tra lời mời thuộc người hiện tại; người ngoài nhận404, Excel yêu cầu403.

Vị trí cần xem trong ứng dụng: `Areas/HocTap/Services/LopHocService.cs:121`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 28
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 28
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 271
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_075() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 77
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-075-DL0-ITa65f44b45d7d4cba9a628a04531b7e89.json`, `Evidence\IT-075-DL1-IT08aaa7248130404096cdb2cef38cfddd.json`, `Evidence\IT-075-DL2-IT400eb64f81304170b8db116d4464b101.json`, `Evidence\IT-075-IT92e1e384abef4872a25a7faebe55071d.json`

## IT-076 — Gia sư duyệt Xem/Chỉnh sửa và quyền nộp riêng

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01 chỉ đọc; DL02 đọc+chat nhưng không nộp; DL03 đọc+chat+nộp khi bài cho phép.
2. Dữ liệu và side effect: Quyền theo đúng ba tổ hợp; thông báo và lịch sử cập nhật; không suy ra quyền nộp từ ChinhSua.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   InternalServerError
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 30
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 30
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 271
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_076() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 78
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-076-DL0-IT7a70d5f30ee24da5b6fed77e9b9f8f99.json`, `Evidence\IT-076-DL1-IT91f0640e42d64ca788b156ae5a243549.json`, `Evidence\IT-076-IT1d4b1b3ba3ed4e639db37630da02031e.json`

## IT-079 — Đổi/thu hồi quyền có hiệu lực ở request kế tiếp

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Sau commit P1 không chat/nộp nữa; chỉ còn quyền đọc được cấp; cookie cũ không giữ quyền cũ.
2. Dữ liệu và side effect: Không tin/bài nộp mới; SQL và quyền realtime phải nhất quán; có notice/lịch sử thay quyền.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   InternalServerError
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 36
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 271
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_079() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 81
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-079-ITa92d70b088cb445db104856819d10d70.json`

## IT-080 — Gỡ/rời thành viên không gỡ hai bên chính

Phân loại: Ứng viên lỗi thiếu luồng nghiệp vụ theo SRS.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01..03 thu hồi quyền thành viên bổ sung; DL04 bị từ chối.
2. Dữ liệu và side effect: Giữ lịch sử/thông tin người rời; không xóa G1/H1 hoặc xóa bài/tin cũ do P1 gửi.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   Forbidden
```

**Ý nghĩa:** DuyetAsync luôn kiểm quyền ghi với giaSuOnly=true; Bên học gỡ thành viên hoặc thành viên tự rời bị403 dù FR-38/QT-08 cho phép. Cần xem cả SRS và thiết kế trước sửa code.

Vị trí cần xem trong ứng dụng: `Areas/HocTap/Services/LopHocService.cs:145`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 34
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 34
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 271
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_080() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 82
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-080-G-IT0fbf3f3026a5426a845d0aebff0904dd.json`, `Evidence\IT-080-H-IT0d33a669b2914afcad7bc0876e6523bc.json`, `Evidence\IT-080-IT7da826c2a272498ba11c746b5643017e.json`

## IT-088 — Batch trùng nội bộ hoặc đụng lịch người tham gia phải atomic

Phân loại: Kỳ vọng TC chưa khớp ánh xạ lỗi lịch.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01/02 HTTP422 JSON ok=false qua ModelState của form Them; có lỗi trùng lịch. DL03 HTTP200 ok=true vì hai khoảng liền kề không chồng lấn.
2. Dữ liệu và side effect: DL01/02 không lưu một phần batch/notice; DL03 đúng số buổi.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: 422
Actual:   409
```

**Ý nghĩa:** Batch trùng bị409, Excel yêu cầu422. LichHocController.ThemLoi giữ Rules.ErrorStatus vào Response.StatusCode; UiResponseFilter giữ status>=400. Không coi chênh mã này là bằng chứng batch đã commit một phần.

Vị trí cần xem trong ứng dụng: `Areas/HocTap/Controllers/LichHocController.cs:12`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.ScheduleCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ScheduleCases.cs:line 29
   at Cases.ScheduleCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ScheduleCases.cs:line 29
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 370
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_088() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 90
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-088-DL0-IT51896ab101784ad2a88b1150fae6a1ef.json`, `Evidence\IT-088-ITc78ae8bf6c004352b52cdf16eb3d6545.json`

## IT-090 — Không đổi buổi có hoạt động thực tế về tương lai

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Các lỗi nghiệp vụ ở form Sua được chuyển ModelState: HTTP422 ok=false, phân biệt buổi có hoạt động/sai quyền/phiên bản; không thay giờ để che lịch sử.
2. Dữ liệu và side effect: Giữ thời gian/ghi nhận/tham dự/bài tập, không che lịch sử hoặc sinh lịch sử sửa giả.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: 422
Actual:   400
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.ScheduleCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ScheduleCases.cs:line 59
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 370
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_090() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 92
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-090-IT8b4b88604b86402583dd1020f8c63ba3.json`

## IT-093 — Qua giờ nhưng chưa ghi nhận không tính đã học

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200; B1 là Chưa ghi nhận; số buổi đã học không tăng vì chỉ qua giờ.
2. Dữ liệu và side effect: Không có ghi nhận/điểm danh giả; dữ liệu chờ nhắc vẫn được giữ.

**Quan sát/assertion thực tế:**
```text
Assert.Contains() Failure: Sub-string not found
String:    "<!doctype html><html lang="vi"><head><met"···
Not found: "ChÆ°a ghi nháº­n"
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.ScheduleCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ScheduleCases.cs:line 37
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 370
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_093() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 95
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-093-ITace85c97cb5942578520660aa14b7013.json`

## IT-108 — Quyền nộp thay không suy ra từ quyền chat

Phân loại: Sai lệch mã từ chối của form nộp bài.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Chỉ DL03 thành công; còn lại 403; hai bên chính vẫn dùng quyền vốn có.
2. Dữ liệu và side effect: Bài nộp thành công ghi đúng người nộp thay/người học; không nộp bài lớp khác.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: 403
Actual:   422
```

**Ý nghĩa:** Một DL bị422 thay vì403 như Excel; cần xem ModelState và nhánh fallback ChiTiet trong BaiTapController.Nop/UiResponseFilter. Không tự kết luận có nộp bài trái quyền; query dữ liệu và giữ oracle theo Excel.

Vị trí cần xem trong ứng dụng: `Areas/HocTap/Controllers/BaiTapController.cs:104`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.LearningCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\LearningCases.cs:line 36
   at Cases.LearningCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\LearningCases.cs:line 36
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 306
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_108() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 110
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-108-DL0-IT6eaebd1bfe5e4b7bacbd67b3ce953c58.json`, `Evidence\IT-108-DL1-IT0444c95225a240d29585ea534e3abc8b.json`, `Evidence\IT-108-DL2-ITfb64006750aa4bb68c4adacd7650f335.json`, `Evidence\IT-108-DL3-ITe4824620153845b49eaa514d43ea0481.json`, `Evidence\IT-108-IT1fdef9382f084e5fa0736808f7ff7b3e.json`

## IT-118 — Đọc phiếu và lịch sử phản hồi cũ theo quyền lớp

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: H1 200 với dữ liệu cũ; X 403; không giả định có POST TaoDot/Gui/DongDot trong Controller hiện tại.
2. Dữ liệu và side effect: Không thay/xóa phiếu, mức hài lòng null vẫn null; không biến phản hồi cũ thành quyết định tự kết thúc lớp.

**Quan sát/assertion thực tế:**
```text
Assert.Contains() Failure: Sub-string not found
String:    "<!doctype html><html lang="vi"><head><met"···
Not found: "Nội dung cũ"
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.ProgressCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProgressCases.cs:line 58
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 348
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_118() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 120
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-118-IT9df71242ffe2416fbb1a4de12bf974ef.json`

## IT-123 — Ẩn đánh giá thay đổi tổng hợp và quyền xem đánh giá Bên học

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Rating G1=4.5,count=2; đánh giá Bên học chỉ Gia sư/chính người nhận/quản trị có quyền được xem, khách không được lộ.
2. Dữ liệu và side effect: Bản ẩn vẫn lưu cho lịch sử; không bị tính vào rating; truy vấn cùng contract giữa tìm kiếm và chi tiết.

**Quan sát/assertion thực tế:**
```text
rating detail missing from HTTP OK: <!doctype html>
<html lang="vi">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>&#x110;&#xE1;nh gi&#xE1; ng&#x1B0;&#x1EDD;i d&#xF9;ng · EduConnect</title>
    <link rel="stylesheet" href="/lib/bootstrap/dist/css/bootstrap.min.css">
    <link rel="stylesheet" href="/css/fonts.css">
    <link rel="stylesheet" href="/css/app.css?v=mAsrX0eE1UWydUxrBnHNegBYUOqjjuSeSgsM09r68HU">
    <link rel="stylesheet" href="/css/profile.css?v=HeU4bNwSZV6X2nI3LqKT0iEgDQSlKL0zBorZHX5S1Ds">
    <link rel="stylesheet" href="/css/notifications.css?v=r2hN5jKsMzzXO-E2YY5ZVVmAm6mogxXxnDo3AUkBeLk">
    <link rel="stylesheet" href="/css/guest.css?v=fuEqLuZjDxhyazhuNDveb_vFB7jyDUttkhZtzMY4_JQ">
<link rel="stylesheet" href="/css/help.css?v=csAUm09dG8Y_ElJmnFxptcFbP09DziJWRkRgY3hoKMo">
</head>
<body>
    <button type="button" class="btn mobile-menu" aria-label="Mở menu" aria-expanded="false" aria-controls="navigation">☰</button>
    <aside class="sidebar" id="navigation">
        <a class="brand" href="/"><img src="/images/auth/logo.png" alt="EduConnect"></a>
        <p class="eyebrow">KHÁM PHÁ EDUCONNECT</p>
        <nav ar
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.ProgressCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProgressCases.cs:line 63
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 348
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_123() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 125
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-123-IT8060540e28ff4fa690bea52d27c93be4.json`

## IT-132 — HTTP gửi tin giữ ownership và nội dung

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: HTTP200 JSON ok=true và redirectUrl về room hiện tại; nội dung khi render được encode, không thực thi script; sender vẫn H1, không lấy actor giả từ form.
2. Dữ liệu và side effect: Đúng một tin; không lưu actor giả hoặc đưa nội dung chat vào audit nhạy cảm.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   InternalServerError
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Fixture.Token(HttpClient c, String url) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 65
   at Fixture.Post(HttpClient c, String url, Dictionary`2 fields, String form, Boolean csrf, Boolean ajax) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 66
   at Cases.ChatCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ChatCases.cs:line 61
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 242
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_132() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 134
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-132-IT12066c48904b4cd68d5dcce1a2a1d553.json`

## IT-134 — Người ngoài, thành viên Xem và lớp kết thúc không gửi được tin

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: X/sai quyền 403; lớp kết thúc 409; các bên có quyền đọc vẫn đọc được lịch sử.
2. Dữ liệu và side effect: Không tin/file/event mới; không xóa lịch sử khi khóa gửi.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   InternalServerError
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 61
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 238
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_134() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 136
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-134-ITb823a8683c0c472186a8ca6e8a5b1a3c.json`

## IT-137 — Service quản trị ghép claim quyền, Identity và dữ liệu SQL

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Danh sách actor/role/trạng thái đúng SQL; role QuanTri không mặc nhiên có mọi quyền chức năng.
2. Dữ liệu và side effect: Không chuyển vai trò/gộp hồ sơ; không tiết lộ PasswordHash/SecurityStamp trong model phản hồi công khai.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.InitializeReaderAsync(AsyncEnumerator enumerator, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.MoveNextAsync()
   at Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync[TSource](IQueryable`1 source, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync[TSource](IQueryable`1 source, CancellationToken cancellationToken)
   at EduConnect.Mvc.Areas.QuanTri.Services.QuanTriService.LayTaiKhoanAsync(Int32 trang, String q, String role, String state) in C:\Khóa luận TN\EduConnect1\Areas\QuanTri\Services\QuanTriService.cs:line 90
   at Cases.AdminCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\AdminCases.cs:line 60
   at Cases.AdminCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\AdminCases.cs:line 64
   at Cases.AdminCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\AdminCases.cs:line 64
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 205
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_137() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 139
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-137-ITc4cc33cad3804d5788956aeeb1e28791.json`

## IT-154 — Service đăng ký ghép Identity, vai trò và hồ sơ

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: IdentityResult.Succeeded=true cho ba vai trò; không tự cấp QuanTri hoặc công khai hồ sơ Gia sư.
2. Dữ liệu và side effect: Mỗi user đúng một role và một hồ sơ nghiệp vụ tương ứng; PasswordHash đúng chuẩn Identity, không lưu password thô.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.InitializeReaderAsync(AsyncEnumerator enumerator, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.MoveNextAsync()
   at Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync[TSource](IQueryable`1 source, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync[TSource](IQueryable`1 source, CancellationToken cancellationToken)
   at Cases.AuthCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\AuthCases.cs:line 55
   at Cases.AuthCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\AuthCases.cs:line 58
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 219
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_154() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 156
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-154-IT850d5628d83344f19c711e29fbffa1f9.json`

## IT-158 — Form đăng ký ba vai trò công khai

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 302 về đăng nhập; chưa được coi là đăng nhập sẵn nếu chưa qua đăng nhập thật.
2. Dữ liệu và side effect: Một user/role/hồ sơ mỗi DL; phụ huynh không tự liên kết học sinh; Gia sư không tự công khai.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: Found
Actual:   OK
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.HttpCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\HttpCases.cs:line 15
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 297
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_158() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 160
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-158-ITd6f1a84751b445fdac4cb1e497daa5e6.json`

## IT-159 — Trùng email hoặc yêu cầu role quản trị không được cấp

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: HTTP 200 biểu mẫu kèm lỗi, không redirect thành công; không tiết lộ hash/SQL stack.
2. Dữ liệu và side effect: Không user/hồ sơ/role mồ côi; DL02 không có claim quản trị được cấp qua form.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.InitializeReaderAsync(AsyncEnumerator enumerator, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.MoveNextAsync()
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Cases.HttpCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\HttpCases.cs:line 17
   at Cases.HttpCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\HttpCases.cs:line 17
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 297
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_159() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 161
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-159-IT91434f0846974410b22532e989edc22c.json`

## IT-175 — MVC/filter ánh xạ response đúng kiểu request

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Form thường 302 Location nội bộ; AJAX thành công 200 JSON ok=true/redirectUrl; AJAX lỗi 422 JSON ok=false/errors.MucTieu.
2. Dữ liệu và side effect: Hai DL thành công có đúng dữ liệu; DL lỗi không ghi; TempData thành công không rò sang request lỗi kế tiếp.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: Found
Actual:   OK
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 43
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 43
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 43
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_175() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 177
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-175-Form-IT899ac5a1e7d74526b5a0fbf772f529a8.json`, `Evidence\IT-175-IT0e2beb7a32fc4b5b908508d195e12f37.json`

## Giới hạn của lượt thực thi

Lượt này chạy 187 TC; kết quả kết luận {'Pass': 132, 'Fail': 43, 'Chưa đủ phạm vi TC': 12}, kết quả xUnit {'Pass': 144, 'Fail': 43}. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
