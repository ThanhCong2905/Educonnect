# Kết quả Integration — bộ191 TC

Thời điểm: 27/11/2026 13:01:18 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Pass': 106, 'Fail': 73, 'Chưa đủ phạm vi TC': 12}. Kết quả script xUnit: {'Pass': 118, 'Fail': 73}.

**Script Pass chưa đủ phạm vi TC không được tính là TC Pass hoàn chỉnh. Lượt này không có ca Skip; 8 mã thiếu dispatcher được gọi và Fail trước khi chạy scenario nghiệp vụ.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-001 | Một tài khoản chỉ có một vai trò | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-002 | Khóa ghép nhu cầu–môn và khóa ngoại | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-003 | Đề xuất SQL kiểm tra liên hệ và môn | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-004 | Chấp nhận SQL tạo đúng một lớp và dữ liệu con | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-005 | Chấp nhận SQL thất bại phải rollback | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-006 | Procedure lịch và trigger tạo BuoiHoc | Fail | Fail | Chưa có DL02 batch lỗi để chứng minh rollback. |
| IT-007 | Procedure ghi nhận buổi kiểm người thực hiện | Fail | Fail | Đã kiểm actor H1 sai quyền; chưa thay bằng Gia sư G2 như DL02 trong Excel. |
| IT-008 | Nộp bài SQL giữ lịch sử và chống lặp mã | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-009 | Chấm SQL kiểm điểm, quyền và lần mới nhất | Fail | Fail | Đã kiểm H1 sai quyền; chưa có Gia sư G2 riêng như Excel. |
| IT-010 | Procedure lỗi đăng nhập và thời gian chờ | Fail | Fail | TC yêu cầu chờ 0/1/5/15 phút, procedure hiện tại quy định 30/60/120/300 giây. Không tự kết luận SQL sai trước khi đối chiếu thiết kế/SRS. |
| IT-011 | Procedure phiên kiểm chủ, thu hồi và hết hạn | Fail | Fail | Chưa có DL tài khoản BiKhoa và đối chiếu hoạt động nền/chủ động. |
| IT-012 | Principal ứng dụng bị chặn sửa/xóa nhật ký | Chưa đủ phạm vi TC | Pass | Đã kiểm quyền SQL UPDATE/DELETE; chưa INSERT/SELECT một audit fixture thực. |
| IT-013 | EF–SQL round-trip trạng thái, khóa phiên bản và mốc kết thúc | Fail | Fail | Đã round-trip LopHoc và RV; chưa đủ YeuCauHuyKetNoi/thời điểm UTC+07/TamDung. |
| IT-014 | Service tạo nhu cầu nhiều môn và đọc lại | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-015 | Lỗi quan hệ môn không được lưu một phần nhu cầu | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-016 | Hủy request khi chờ khóa SQL không để ghi một phần | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-017 | Binding form tạo nhu cầu từ HTTP | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-018 | Lưu nháp thiếu mục tiêu và lịch | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-019 | Phụ huynh đứng tên nhu cầu của người học | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-020 | Khách hoặc gia sư không tạo nhu cầu học | Fail | Fail | Authorize(Roles=HocSinh,PhuHuynh) ở Controller chặn Gia sư bằng403 trước action; Excel lại yêu cầu422. Không có bằng chứng vượt quyền. |
| IT-021 | Hạn mức nhu cầu mở, nháp và lượt đăng/ngày | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-022 | Chuẩn hóa trùng nhu cầu trước khi ghi | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-023 | Cập nhật thay môn/lịch, không đẩy như bài mới | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-024 | Sai chủ hoặc phiên bản cũ không ghi đè | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-025 | Validation form, địa giới và học phí đi qua đúng khớp nối | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-026 | Đóng/mở lại và xác nhận còn nhu cầu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-027 | Form ghi thiếu CSRF không gọi nghiệp vụ | Chưa đủ phạm vi TC | Pass | Đã kiểm CSRF400 và không ghi nhu cầu; chưa assertion audit CSRF. |
| IT-028 | Service lưu hồ sơ, môn và minh chứng trong cùng giao dịch | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-029 | Service tìm kiếm ghép hồ sơ, môn, phí và đánh giá | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-030 | Gợi ý có lý do, không tự tạo kết nối | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-031 | Multipart hồ sơ giữ đúng liên kết minh chứng | Chưa đủ phạm vi TC | Pass | Service/SQL/file path is tested, but POST multipart through HoSoGiaSuController and binding/CSRF/response are not. |
| IT-032 | Thiếu minh chứng không lưu một phần hồ sơ | Chưa đủ phạm vi TC | Pass | Service-level missing-proof rejection and no partial SQL save are tested; HTTP form and ModelState response are not. |
| IT-033 | Hồ sơ phiên bản cũ không xóa minh chứng mới | Chưa đủ phạm vi TC | Pass | Service-level stale RowVersion protection is tested; the HTTP route/status/body are not. |
| IT-034 | Duyệt hồ sơ làm thay đổi điều kiện công khai | Fail | Fail | Admin service review and guest search are tested, but the admin review Controller route/response are not. |
| IT-035 | Sửa nghề nghiệp/họ tên khi công khai phải theo điều kiện ẩn và duyệt lại | Fail | Fail | Both workflow branches are tested at HoSoGiaSuService/SQL level; the TrangThai/Luu HTTP posts are not. |
| IT-036 | Quyền xem trước và tải minh chứng | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-037 | Khách xem hồ sơ và nhu cầu công khai | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-038 | Lọc nhiều tiêu chí với mức phí đúng môn và đơn vị | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-039 | Hồ sơ và nhu cầu bị ẩn/khóa không lọt kết quả | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-040 | Phân trang và sắp xếp ổn định trên dữ liệu thật | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-041 | Bộ lọc sai kiểu không bị bỏ qua | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-042 | So sánh đủ điều kiện và loại Id trùng | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-043 | So sánh sai số lượng, loại hoặc điều kiện công khai | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-044 | Service gửi/chấp nhận liên hệ ghép hội thoại và thông báo | Fail | Fail | Đã kiểm liên hệ, hội thoại, notification; chưa kiểm đầy đủ payload/outbox chống nhân bản. |
| IT-045 | Service gửi đề xuất hai chiều và kiểm vai trò bên nhận | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-046 | Hai gửi liên hệ đồng thời không nhân bản quan hệ | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-047 | Gửi liên hệ qua HTTP giữ danh tính từ phiên | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-048 | Chặn liên hệ không hợp lệ trước và trong giao dịch | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-049 | Phản hồi liên hệ đúng bên và phiên bản | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-050 | Chưa chấp nhận liên hệ không nhắn tin hoặc đề xuất | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-051 | Form đề xuất đầy đủ từ cả hai phía | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-052 | Chỉ bên gửi được sửa đề xuất đang chờ | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-053 | Thiếu dữ liệu lớp hoặc mảng lịch bị loại | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-054 | Gửi/chấp nhận kiểm lại môn và nhu cầu bị ẩn | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-055 | Từ chối và rút theo đúng bên tham gia | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-056 | Điểm vào chuyển tiếp không bỏ kiểm tra actor và liên hệ | Fail | Fail | Người ngoài nhận404, Excel yêu cầu403; không chứng minh người ngoài tạo được yêu cầu. Đối chiếu quy tắc che sự tồn tại của liên hệ trước khi kết luận lỗi nghiệp vụ. |
| IT-057 | Service chấp nhận ghép yêu cầu, lớp và các bảng con | Fail | Fail | Fixture một lịch; Excel nêu hai lịch dự kiến. |
| IT-058 | Service yêu cầu/phản hồi kết thúc ghi đủ dữ liệu | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-059 | Lỗi sinh lịch phải rollback toàn bộ tạo lớp | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-060 | Xác nhận lặp/đồng thời không tạo hai lớp | Fail | Fail | Đã retry và gọi đồng thời sau lần chấp nhận đầu; chưa race hai xác nhận đầu tiên. |
| IT-061 | Không hai yêu cầu kết thúc đang chờ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-062 | HTTP chấp nhận tạo đúng lớp và redirectUrl | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-063 | Không cho người gửi hoặc người ngoài tự chấp nhận | Fail | Fail | Người ngoài không được đọc yêu cầu nên Service trả404 trước nhánh từ chối403. Kiểm tiếp không tạo lớp, không suy diễn đây là vượt quyền. |
| IT-064 | Khi xác nhận phải kiểm lại dữ liệu đã đổi | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-065 | Cả Gia sư và Bên học đều được yêu cầu kết thúc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-066 | Đồng ý/từ chối trước hạn dùng RV yêu cầu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-067 | Người gửi, người ngoài hoặc RV sai không quyết định kết thúc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-068 | Phản hồi sau hạn không kéo dài vì worker chậm | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-069 | Lớp kết thúc giữ lịch sử và chặn thao tác học tập mới | Fail | Fail | Excel yêu cầu409 cho thao tác ghi; quan sát có403. Phân biệt mã hợp đồng HTTP với việc thực tế vẫn chặn ghi, xem response và bảng sau thao tác. |
| IT-070 | Service đề nghị/xác nhận liên kết không tự cấp quyền lớp | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-071 | Service lời mời → đồng ý → duyệt ghi đủ quyền | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-072 | Liên kết phụ huynh–học sinh qua HTTP | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-073 | Không nhận liên kết sai vai trò, sai người hoặc RV cũ | Fail | Fail | Script mới có DL01; chưa có sai người/RV cũ. |
| IT-074 | Đề nghị thành viên tại action DeNghi thực tế | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-075 | Người nhận tự đồng ý hoặc từ chối lời mời | Fail | Fail | Đã kiểm HTTP/state ba DL; chưa assert riêng quyền mở lớp sau từng DL. |
| IT-076 | Gia sư duyệt Xem/Chỉnh sửa và quyền nộp riêng | Fail | Fail | Đã kiểm ba cấu hình quyền lưu SQL; chưa thao tác đọc/chat/nộp cho cả ba cấu hình. |
| IT-077 | Duyệt lại điều kiện liên kết và hạn mức trong transaction | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-078 | Bên học/thành viên không duyệt quyền người khác | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-079 | Đổi/thu hồi quyền có hiệu lực ở request kế tiếp | Fail | Fail | Đã kiểm cookie cũ không chat/nộp; chưa kiểm GET đọc còn được phép. |
| IT-080 | Gỡ/rời thành viên không gỡ hai bên chính | Fail | Fail | Đã có DL gỡ G1/H1/P1 đối với P1; chưa DL bảo vệ hai thành viên chính. |
| IT-081 | Hủy liên kết thu hồi quyền thành viên phụ thuộc | Chưa đủ phạm vi TC | Pass | Service cancellation and rights revocation are tested; HTTP route/cookie/CSRF are not. |
| IT-082 | Biết Id/là người nhà không mở lớp khác hoặc chat riêng | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-083 | Service thêm batch ghép LichHoc–BuoiHoc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-084 | Service bắt đầu và ghi nhận buổi giữ Id | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-085 | Hai ghi nhận cùng RV không đè mất dữ liệu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-086 | Không nhân đôi buổi bù còn hiệu lực | Chưa đủ phạm vi TC | Pass | The service script asserts duplicate active makeup is rejected and keeps one linked makeup; it does not test the design case where an inactive makeup permits replacement. |
| IT-087 | Form lịch hàng loạt và ngày/giờ local | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-088 | Batch trùng nội bộ hoặc đụng lịch người tham gia phải atomic | Fail | Fail | DL02 dùng lịch hiện hữu cùng lớp; chưa lớp C2 dùng chung G1. |
| IT-089 | Đổi lịch buổi chưa diễn ra giữ buổi và bài tập | Chưa đủ phạm vi TC | Pass | Đã kiểm giữ BuoiHoc/bài nháp/lịch sử; chưa so sánh toàn bộ giờ mới lưu SQL. |
| IT-090 | Không đổi buổi có hoạt động thực tế về tương lai | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-091 | Đến giờ mới bắt đầu được buổi | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-092 | Ghi nhận nội dung, kết quả và tham dự | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-093 | Qua giờ nhưng chưa ghi nhận không tính đã học | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-094 | Nghỉ cần bù chưa biết ngày vẫn được lưu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-095 | Tạo buổi bù giữ liên kết buổi gốc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-096 | Service giao bài và tệp gắn đúng BuoiHoc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-097 | Service nộp/chấm ghép khóa mới nhất và lưu precision | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-098 | Retry cùng mã không tạo lần nộp mới | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-099 | Lưu lần nộp trước, chỉ chấm lần mới nhất | Chưa đủ phạm vi TC | Pass | Chưa phân loại chặt exception của lần chấm bản cũ; chưa đủ bằng chứng để kết luận TC hoàn chỉnh. |
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
| IT-124 | TepService ghép scanner process và file sandbox | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-125 | Service gửi tin, tệp và outbox commit đồng bộ | Chưa đủ phạm vi TC | Pass | Đã kiểm tin và outbox; chưa ghép tệp thật trong giao dịch. |
| IT-126 | Mã thao tác tin nhắn chống retry và thay nội dung | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-127 | Đuôi hợp lệ nhưng cấu trúc giả, macro hoặc traversal bị loại | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-128 | Scanner báo nhiễm/lỗi/timeout không được gắn trạng thái sạch | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-129 | Nhánh Development cho tệp chưa quét không áp sang môi trường khác | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-130 | Thu hồi quyền trong lúc quét phải kiểm lại trước gắn tệp | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |
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
| IT-153 | Tra cứu nhật ký an toàn và quản trị theo nguồn thật | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-154 | Service đăng ký ghép Identity, vai trò và hồ sơ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-155 | Mật khẩu phải được Identity xác minh trước SQL đăng nhập thành công | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-156 | Quên mật khẩu ghép token và mail sandbox | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-157 | Đăng nhập HTTP phát cookie và phiên thật | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-158 | Form đăng ký ba vai trò công khai | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-159 | Trùng email hoặc yêu cầu role quản trị không được cấp | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-160 | Sai mật khẩu, tài khoản khóa và đang chờ | Fail | Fail | Đã kiểm bốn nhánh không phát cookie; chưa so sánh thông báo chống lộ tài khoản. |
| IT-161 | ReturnUrl ngoài miền không gây open redirect | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-162 | Đăng xuất thu hồi phiên và cookie cũ | Fail | Fail | Đã logout, thu hồi SQL và challenge; chưa replay một bản cookie đã sao chép trước logout. |
| IT-163 | Đổi mật khẩu thu hồi các phiên | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-164 | Form quên mật khẩu không lộ email tồn tại | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-165 | Đặt lại với token đúng, sai, hết hạn và đã dùng | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-166 | Hồ sơ cá nhân dùng ConcurrencyStamp và email chỉ đọc | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-167 | Thay/xóa ảnh đại diện ghép metadata, file và quyền xem | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-168 | Phiên hết nhàn rỗi hoặc đã thu hồi không truy cập được | Chưa đủ phạm vi TC | Pass | Đã kiểm ba nhánh AJAX401; chưa assertion challenge của request form thường. |
| IT-169 | Rate limiter/antiforgery không tạo ghi nghiệp vụ ngoài ý muốn | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-170 | Producer lưu notification/outbox trong cùng transaction | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-171 | Audit service/middleware ghép SQL bằng scope độc lập | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-172 | Đọc một/đọc tất cả thông báo đúng người nhận | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-173 | Audit liên thông request thành công/deny/CSRF/tải tệp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-174 | Lỗi lưu audit không hủy giao dịch nghiệp vụ đã commit | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-175 | MVC/filter ánh xạ response đúng kiểu request | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-176 | Hub chỉ nhận phiên thật hợp lệ và kiểm quyền room | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |
| IT-177 | Worker không phát trước commit rồi phát đúng EventId | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |
| IT-178 | Transport lỗi giữ nghiệp vụ và retry có backoff | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |
| IT-179 | Hai worker và lease hết hạn không làm mất sự kiện | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |
| IT-180 | Thu hồi quyền rời group và không nhận tin mới | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |
| IT-181 | Ping/notification nền không gia hạn phiên | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |
| IT-182 | Bảo trì hết hạn đề xuất không tạo lớp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-183 | Hết72 giờ tự kết thúc đúng yêu cầu còn chờ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-184 | Worker và phản hồi đúng hạn cạnh tranh có một kết quả hợp lệ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-185 | Nhắc30 ngày và đóng sau7 ngày không xác nhận | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-186 | Nhắc buổi/bài từ dữ liệu thật không tự chấm hoặc ghi nhận | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-187 | Dừng host không làm mất sự kiện chưa hoàn tất | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |
| IT-188 | Metadata kết thúc lớp khớp EF | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-189 | Một tài khoản chỉ một vai trò | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-190 | Một lớp trên một yêu cầu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-191 | Schema cuối hỗ trợ TamDung | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_002() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 4
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-002-IT35bef99a2d894d61a902e5f28b9d71d2.json`

## IT-005 — Chấp nhận SQL thất bại phải rollback

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Từng DL bị từ chối vì sai quyền/thiếu lịch/trùng lịch; không chỉ kiểm exception rồi bỏ qua dữ liệu đã ghi.
2. Dữ liệu và side effect: R1 không sang DaChapNhan; không LopHoc/LichHoc/BuoiHoc/hội thoại lớp/thông báo mồ côi.

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
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 23
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 28
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_005() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 7
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

Bằng chứng: `Evidence\IT-005-DL0-ITbba97a269b9341dd8bb7555a1fdc943f.json`, `Evidence\IT-005-DL1-IT5803fb4f9ca54151881f08140ab2cd99.json`, `Evidence\IT-005-ITb6157b3928c04b6dac4edd9f363e5e3d.json`

## IT-006 — Procedure lịch và trigger tạo BuoiHoc

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01 commit hai lịch, mỗi lịch có đúng một BuoiHoc. DL02 bị từ chối toàn batch.
2. Dữ liệu và side effect: Không lịch tồn tại mà thiếu BuoiHoc; DL02 không lưu một phần batch.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : Invalid operation. The connection is closed.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.ValidateConnectionForExecute(SqlCommand command)
   at Microsoft.Data.SqlClient.SqlInternalTransaction.Rollback()
   at Microsoft.Data.SqlClient.SqlInternalTransaction.Dispose(Boolean disposing)
   at Microsoft.Data.SqlClient.SqlTransaction.Dispose(Boolean disposing)
   at Microsoft.EntityFrameworkCore.Storage.RelationalTransaction.DisposeAsync()
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.SaveChangesAsync(IList`1 entries, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(IList`1 entriesToSave, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(StateManager stateManager, Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Fixture.SeedProposal() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 56
   at Fixture.SeedProposal() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 56
   at Fixture.SeedClass() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 58
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 31
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_006() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 8
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-006-IT835c3378773a487f9a386a2f72bdc364.json`

## IT-007 — Procedure ghi nhận buổi kiểm người thực hiện

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: G1 được ghi; G2 bị từ chối tại DB, không thể vượt quyền bằng cách bỏ HTTP.
2. Dữ liệu và side effect: Một kết quả hiện hành; không đổi LichHocId hoặc ghi dữ liệu cho sai lớp.

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
   at Fixture.SeedClass() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 58
   at Fixture.SeedLesson(Boolean past) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 59
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 33
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_007() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 9
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

Bằng chứng: `Evidence\IT-007-IT067b9d1d10904ea49848e267bb302a63.json`

## IT-008 — Nộp bài SQL giữ lịch sử và chống lặp mã

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: O1 chỉ tạo một lần nộp; O2 tạo lần mới; số lần nộp tăng trong đúng A1.
2. Dữ liệu và side effect: Giữ cả hai lần; không lẫn bài giao khác; mã thao tác không nhân bản metadata tệp/thông báo.

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
   at Fixture.SeedClass() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 58
   at Fixture.SeedLesson(Boolean past) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 59
   at Fixture.SeedAssignment() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 60
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 37
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_008() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 10
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

Bằng chứng: `Evidence\IT-008-IT568f2aaa9d8a4e68901589a84bcf05e3.json`

## IT-009 — Chấm SQL kiểm điểm, quyền và lần mới nhất

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01 thành công; các DL sai quyền/lần cũ/điểm ngoài miền bị từ chối.
2. Dữ liệu và side effect: Không kết quả chấm cho DL02..04; điểm DL01=8.25, không làm tròn tùy tiện thành 8 hoặc 8.3.

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
   at Fixture.SeedClass() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 58
   at Fixture.SeedLesson(Boolean past) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 59
   at Fixture.SeedAssignment() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 60
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 39
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_009() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 11
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

Bằng chứng: `Evidence\IT-009-IT8bb3ac9bbbd942839c3c64d610a0c8a5.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_010() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 12
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-010-IT17c3145c7ac543d9bd4ea6b6ebd9b3ea.json`

## IT-011 — Procedure phiên kiểm chủ, thu hồi và hết hạn

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Chỉ phiên đúng chủ/hoạt động/chưa thu hồi/còn hạn trả HopLe=1. Request nền không gia hạn; hoạt động chủ động hợp lệ mới gia hạn.
2. Dữ liệu và side effect: Không chuyển chủ phiên hoặc hồi sinh phiên bị thu hồi/hết hạn/tài khoản khóa.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteReaderAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 49
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 51
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_011() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 13
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-011-IT85df7ab40b1b44818ba9379bb464910b.json`

## IT-013 — EF–SQL round-trip trạng thái, khóa phiên bản và mốc kết thúc

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Giá trị sau round-trip đúng kiểu/độ chính xác schema; trạng thái hợp lệ không lỗi CHECK; context cũ phát hiện concurrency.
2. Dữ liệu và side effect: Không mất thời điểm/người/lý do/cách kết thúc; RowVersion mới có 8 byte và đổi sau update; SQL/EF không ánh xạ lệch cột.

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
   at Fixture.SeedClass() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 58
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 55
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_013() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 15
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

Bằng chứng: `Evidence\IT-013-ITe540ab7f50c24350b8a475a5dedfbba4.json`

## IT-014 — Service tạo nhu cầu nhiều môn và đọc lại

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: KetQua.ThanhCong=true; DuLieu=N1 khác Guid.Empty.
2. Dữ liệu và side effect: Một NhuCauHoc, hai NhuCauMonHoc, hai KhungGioRanh, một LuotDangNhuCau; chủ H1; không nhân bản nhu cầu theo môn.

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
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.HienTaiAsync(CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 27
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.KiemTraNguoiThucHienAsync(Nullable`1 id, CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 33
   at EduConnect.Mvc.Areas.NhuCau.Services.NhuCauService.<>c__DisplayClass7_0.<<TaoNhuCauHocAsync>b__0>d.MoveNext() in C:\Khóa luận TN\EduConnect1\Areas\NhuCau\Services\NhuCauService.cs:line 38
--- End of stack trace from previous location ---
   at EduConnect.Mvc.Infrastructure.KetQua`1.ThucHienAsync(Func`1 action) in C:\Khóa luận TN\EduConnect1\Infrastructure\KetQua.cs:line 30
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 18
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 20
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_014() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 16
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-014-ITed4c32bbb1f74d8bb4545f8b6110adba.json`

## IT-015 — Lỗi quan hệ môn không được lưu một phần nhu cầu

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: KetQua.ThanhCong=false, lỗi môn; không trả Guid nhu cầu được xem như tạo thành công.
2. Dữ liệu và side effect: Không còn dòng cha, link môn, lịch hoặc LuotDang của lần ghi lỗi.

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
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.HienTaiAsync(CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 27
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.KiemTraNguoiThucHienAsync(Nullable`1 id, CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 33
   at EduConnect.Mvc.Areas.NhuCau.Services.NhuCauService.<>c__DisplayClass7_0.<<TaoNhuCauHocAsync>b__0>d.MoveNext() in C:\Khóa luận TN\EduConnect1\Areas\NhuCau\Services\NhuCauService.cs:line 38
--- End of stack trace from previous location ---
   at EduConnect.Mvc.Infrastructure.KetQua`1.ThucHienAsync(Func`1 action) in C:\Khóa luận TN\EduConnect1\Infrastructure\KetQua.cs:line 30
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 18
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 20
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_015() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 17
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-015-IT1906c53efcbe44778ddea9ed83d0b48a.json`

## IT-016 — Hủy request khi chờ khóa SQL không để ghi một phần

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Call bị hủy có kiểm soát trước commit, không trả KetQua thành công hoặc rơi vào tiếp tục ghi sau cancel.
2. Dữ liệu và side effect: Không nhu cầu/lịch/link/lượt đăng của B; khóa của A được giải phóng; token được truyền qua khớp Service→EF→SqlClient.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, SqlCommand command, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteReaderInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteReaderAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 194
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_016() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 18
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-016-ITdb8f3f33d80e4d0689991c986a7f8682.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_017() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 19
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-017-ITccaab9f4c9e14c90b1f0b748e40417d5.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_018() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 20
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-018-ITe6be628237bd4c0d876896b02892a71a.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_019() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 21
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-019-ITd6d41701ba1d40ccb8d5eeef6ad75f2f.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_020() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 22
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-020-ITac27b0a62ee24469ad8a34bec690017a.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_022() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 24
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-022-ITbd7267801104479fa40b25204d6a633b.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_023() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 25
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-023-ITfab28a4923b9458284a78c831d6e5d51.json`

## IT-028 — Service lưu hồ sơ, môn và minh chứng trong cùng giao dịch

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Service hoàn tất; giữ liên kết bằng chứng theo đúng từng mục.
2. Dữ liệu và side effect: Một HoSoGiaSu, hai môn; minh chứng trình độ/môn có FK metadata/blob thật; Chưa có kinh nghiệm không yêu cầu minh chứng kinh nghiệm.

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
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.HienTaiAsync(CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 27
   at EduConnect.Mvc.Areas.NhuCau.Services.HoSoGiaSuService.GiaSuAsync() in C:\Khóa luận TN\EduConnect1\Areas\NhuCau\Services\HoSoGiaSuService.cs:line 29
   at EduConnect.Mvc.Areas.NhuCau.Services.HoSoGiaSuService.CapNhatAsync(HoSoInput m) in C:\Khóa luận TN\EduConnect1\Areas\NhuCau\Services\HoSoGiaSuService.cs:line 52
   at Cases.SaveProfileFixture(Fixture f, Boolean submit) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProfileCases.cs:line 61
   at Cases.SaveProfileFixture(Fixture f, Boolean submit) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProfileCases.cs:line 73
   at Cases.ProfileCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProfileCases.cs:line 174
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 339
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_028() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 30
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-028-ITc9dbde2905294dc99ff10fb4060cc729.json`

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
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.HienTaiAsync(CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 27
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.KiemTraNguoiThucHienAsync(Nullable`1 id, CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 33
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.NguoiXemAsync(Nullable`1 id, CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 17
   at EduConnect.Mvc.Areas.TimKiem.Services.TimKiemService.<>c__DisplayClass10_0.<<TimKiemAsync>b__0>d.MoveNext() in C:\Khóa luận TN\EduConnect1\Areas\TimKiem\Services\TimKiemService.cs:line 93
--- End of stack trace from previous location ---
   at EduConnect.Mvc.Infrastructure.KetQua`1.ThucHienAsync(Func`1 action) in C:\Khóa luận TN\EduConnect1\Infrastructure\KetQua.cs:line 30
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 20
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 30
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 371
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_029() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 31
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-029-IT8548dfbc65454f6f9b939259f1c0fd37.json`

## IT-030 — Gợi ý có lý do, không tự tạo kết nối

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: H1 ưu tiên đúng mức tiêu chí và có lý do, không phần trăm; G1 chỉ nhu cầu liên quan môn; H2 nhận không có nhu cầu để gợi ý.
2. Dữ liệu và side effect: Không tự tạo YeuCauLienHe/YeuCauKetNoiLopHoc/LopHoc.

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
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.HienTaiAsync(CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 27
   at EduConnect.Mvc.Areas.TimKiem.Services.TimKiemService.GoiYAsync() in C:\Khóa luận TN\EduConnect1\Areas\TimKiem\Services\TimKiemService.cs:line 407
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 37
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 43
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 371
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_030() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 32
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-030-IT866f0f7d7cb845cb94759a6ad2897742.json`

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
   at Cases.ProfileCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProfileCases.cs:line 214
   at Cases.ProfileCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProfileCases.cs:line 216
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 339
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_034() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 36
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-034-IT33d05d3bd0644c9cb673a8d5dad64f7f.json`

## IT-035 — Sửa nghề nghiệp/họ tên khi công khai phải theo điều kiện ẩn và duyệt lại

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01 bị từ chối theo điều kiện hồ sơ; DL02 lưu được nhưng không công khai trước khi duyệt lại.
2. Dữ liệu và side effect: Không dùng trạng thái cũ để vượt duyệt; lịch sử và minh chứng được giữ; tìm kiếm loại hồ sơ đang ẩn.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, SqlCommand command, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteReaderInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteReaderAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 194
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_035() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 37
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-035-ITf6eb0d62f9e84a27af3203605fb0e107.json`

## IT-044 — Service gửi/chấp nhận liên hệ ghép hội thoại và thông báo

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Service hoàn tất; liên hệ từ ChoPhanHoi sang ChapNhan.
2. Dữ liệu và side effect: Đúng một liên hệ/hội thoại riêng cho cặp H1/G1; thành viên hội thoại đúng hai người; thông báo/outbox đúng người nhận; chưa có lớp.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteReaderAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 194
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_044() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 46
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-044-IT012add0eab2a42128880cb616d888681.json`

## IT-045 — Service gửi đề xuất hai chiều và kiểm vai trò bên nhận

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: KetQua thành công; G1 gửi thì chờ Bên học, H1 gửi thì chờ Gia sư; nhận đúng bên còn lại.
2. Dữ liệu và side effect: Một đề xuất/DL, đầy đủ KhungGioDeXuat; mốc hết hạn 72 giờ; phí/mục tiêu/địa điểm khớp đầu vào; chưa tạo lớp.

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
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.HienTaiAsync(CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 27
   at EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService.GuiDeXuatAsync(DeXuatLopViewModel m, CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Areas\KetNoi\Services\KetNoiService.cs:line 184
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 18
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 18
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 18
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 242
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_045() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 47
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-045-DL0-IT61d7dea4ba434f84b0a785f33f2f3162.json`, `Evidence\IT-045-ITfc60cb8d6ba0480da1781443953bf4e0.json`

## IT-046 — Hai gửi liên hệ đồng thời không nhân bản quan hệ

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Một lần tạo được; lần còn lại nhận từ chối trùng quan hệ có kiểm soát; không exception infrastructure/500.
2. Dữ liệu và side effect: Chỉ một liên hệ đang chờ của cặp, không hai hội thoại/thông báo trùng.

**Quan sát/assertion thực tế:**
```text
Assert.Contains() Failure: Filter not matched in collection
Collection: [Microsoft.Data.SqlClient.SqlException (0x80131904): Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
 ---> System.ComponentModel.Win32Exception (258): The wait operation timed out.
   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, SqlCommand command, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.SqlCommand.CheckThrowSNIException()
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.<>c.<InternalExecuteNonQueryAsync>b__193_1(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at AppPrincipal.ConnectionOpenedAsync(DbConnection c, ConnectionEndEventData e, CancellationToken ct) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 36
   at AppPrincipal.ConnectionOpenedAsync(DbConnection c, ConnectionEndEventData e, CancellationToken ct) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 36
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenInternalAsync(Boolean errorsExpected, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenInternalAsync(Boolean errorsExpected, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenAsync(CancellationToken cancellationToken, Boolean errorsExpected)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.InitializeReaderAsync(AsyncEnumerator enumerator, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.MoveNextAsync()
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService.GuiLienHeAsync(GuiLienHeInput m) in C:\Khóa luận TN\EduConnect1\Areas\KetNoi\Services\KetNoiService.cs:line 60
   at Cases.Capture(Func`1 f) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 67
ClientConnectionId:e2dc02d3-4d4c-4bad-924e-a3ccc33fb754
Error Number:-2,State:0,Class:11, Microsoft.Data.SqlClient.SqlException (0x80131904): Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
 ---> System.ComponentModel.Win32Exception (258): The wait operation timed out.
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
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.HienTaiAsync(CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 27
   at EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService.GuiLienHeAsync(GuiLienHeInput m) in C:\Khóa luận TN\EduConnect1\Areas\KetNoi\Services\KetNoiService.cs:line 59
   at Cases.Capture(Func`1 f) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 67
ClientConnectionId:d33a37cc-829a-4302-b2b9-a3e16388b9ea
Error Number:-2,State:0,Class:11]
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 20
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 20
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 20
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 242
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_046() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 48
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-046-IT02e657b67dee4bc59212fdf6038cc6ff.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 261
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_051() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 53
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-051-G-IT64b1220b2bfd48808b97cedcc00bde6d.json`, `Evidence\IT-051-ITe71dc49ba2164fe4aee7f3d70ee5b5d6.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 261
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_052() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 54
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-052-DL0-IT7eae429386ab4de4ab3c838e3989546c.json`, `Evidence\IT-052-IT2b09094ae04d4813af83261175292c99.json`

## IT-056 — Điểm vào chuyển tiếp không bỏ kiểm tra actor và liên hệ

Phân loại: Sai lệch mã lỗi quyền cần thống nhất.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: H1 hợp lệ: HTTP200 ok=true, tạo yêu cầu ChoGiaSu để Gia sư bổ sung đề xuất; X bị403; không tạo lớp ngay từ yêu cầu sơ bộ.
2. Dữ liệu và side effect: Không bỏ qua liên hệ/ownership, không tạo lớp hoặc cấp quyền chat cho X.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: 403
Actual:   404
```

**Ý nghĩa:** Người ngoài nhận404, Excel yêu cầu403; không chứng minh người ngoài tạo được yêu cầu. Đối chiếu quy tắc che sự tồn tại của liên hệ trước khi kết luận lỗi nghiệp vụ.

Vị trí cần xem trong ứng dụng: `Areas/KetNoi/Services/KetNoiService.cs:113`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 37
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 261
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_056() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 58
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-056-ITb5f0829b43024c0ba63574eec704bb61.json`

## IT-057 — Service chấp nhận ghép yêu cầu, lớp và các bảng con

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Mỗi DL KetQua.ThanhCong=true, trả ClassId khác Guid.Empty; R1=DaChapNhan; lớp giữ đúng GiaSuId=G1,BenHocId=H1 bất kể chiều gửi.
2. Dữ liệu và side effect: Đúng một lớp cho R1; điều kiện lớp khớp bản xác nhận; lịch có BuoiHoc; kênh lớp khác kênh riêng; không tự đóng N1.

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
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 22
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 242
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_057() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 59
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

Bằng chứng: `Evidence\IT-057-IT474d0835df884b9fa4a21ae2ff9dbf9c.json`

## IT-058 — Service yêu cầu/phản hồi kết thúc ghi đủ dữ liệu

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Hai KetQua thành công; yêu cầu được chấp nhận và lớp DaKetThuc.
2. Dữ liệu và side effect: Ghi lý do/người yêu cầu/thời điểm/cách kết thúc; lịch sử, notice/outbox đủ hai phía; không xóa nội dung lớp.

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
   at Fixture.SeedClass() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 58
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 24
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 242
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_058() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 60
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

Bằng chứng: `Evidence\IT-058-ITb2239ac320214144a68a88aaa67f4180.json`

## IT-059 — Lỗi sinh lịch phải rollback toàn bộ tạo lớp

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: KetQua thất bại do trùng lịch, không trả key được xem là lớp đã tạo.
2. Dữ liệu và side effect: Không C1/LichHoc/BuoiHoc/kênh lớp/notice mới; R1 không bị đánh dấu DaChapNhan một phần.

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
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 14
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 261
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_059() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 61
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

Bằng chứng: `Evidence\IT-059-IT627c77a18e574b9d987f79e9de5c3cfe.json`

## IT-060 — Xác nhận lặp/đồng thời không tạo hai lớp

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Lượt chấp nhận đầu trả ClassId C1; lượt đã thấy R1=DaChapNhan trả cùng C1. Lượt cùng RV bị bắt lỗi concurrency phải là lỗi409 có kiểm soát, không tạo C2; ghi rõ thứ tự quan sát của từng call.
2. Dữ liệu và side effect: COUNT(LopHoc theo R1)=1; không nhân đôi lịch/buổi/hội thoại/notification theo thao tác.

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
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 22
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 242
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_060() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 62
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

Bằng chứng: `Evidence\IT-060-IT4cf4ebe9d4094563b29a61bc647a8653.json`

## IT-063 — Không cho người gửi hoặc người ngoài tự chấp nhận

Phân loại: Sai lệch mã lỗi/quyền cần đối chiếu.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 403 ok=false; không lấy MaNguoiThucHien do client giả mạo.
2. Dữ liệu và side effect: R1 còn chờ; không lớp/lịch/buổi/thông báo chấp nhận.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: Forbidden
Actual:   NotFound
```

**Ý nghĩa:** Người ngoài không được đọc yêu cầu nên Service trả404 trước nhánh từ chối403. Kiểm tiếp không tạo lớp, không suy diễn đây là vượt quyền.

Vị trí cần xem trong ứng dụng: `Areas/KetNoi/Services/KetNoiService.cs:199`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.HttpCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\HttpCases.cs:line 43
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 301
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_063() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 65
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-063-IT014911838e9b4dcf88b806871ebb9d76.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 261
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_069() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 71
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-069-ITa5b759df273046a59c933aa645015025.json`

## IT-070 — Service đề nghị/xác nhận liên kết không tự cấp quyền lớp

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Liên kết từ chờ sang DaLienKet, đúng P1/H1; không gọi hoặc giả kết quả Service.
2. Dữ liệu và side effect: Chưa có ThanhVienLopHoc hoặc quyền chat lớp chỉ vì liên kết tài khoản đã xác nhận.

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
   at Fixture.SeedClass() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 58
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 18
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 275
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_070() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 72
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

Bằng chứng: `Evidence\IT-070-IT78e7280ccfa34ab39a540d5231f21076.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 275
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
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

Bằng chứng: `Evidence\IT-071-ITa24e9eb510424287a4103e86c02b5ac5.json`

## IT-072 — Liên kết phụ huynh–học sinh qua HTTP

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200 ok=true; đồng ý=DaLienKet; từ chối không thành liên kết có hiệu lực.
2. Dữ liệu và side effect: Đúng hai tài khoản, không tạo lớp/thành viên; lưu thời điểm và thông báo phản hồi.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: Found
Actual:   InternalServerError
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Fixture.Client(Nullable`1 actor) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 64
   at Fixture.Client(Nullable`1 actor) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 64
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 22
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 22
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 22
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 275
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_072() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 74
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-072-False-IT550c991f28024b6cb94f5d7aa08b440e.json`, `Evidence\IT-072-ITa4aa466ba96b42538ba8006e16bf940f.json`, `Evidence\IT-072-True-ITba35f9c4bcdb47a891bc88dc05285d06.json`

## IT-073 — Không nhận liên kết sai vai trò, sai người hoặc RV cũ

Phân loại: Kỳ vọng TC cần đối chiếu.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Sai vai trò bị 400; sai người 403; RV cũ 409; ok=false.
2. Dữ liệu và side effect: Không tự chuyển vai trò hoặc tạo liên kết sai; dữ liệu hợp lệ trước đó giữ nguyên.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Connection Timeout Expired.  The timeout period elapsed during the post-login phase.  The connection could have timed out while waiting for server to complete the login process and respond; Or it could have timed out while attempting to create multiple active connections.  The duration spent while attempting to connect to this server was - [Pre-Login] initialization=0; handshake=0; [Login] initialization=0; authentication=0; [Post-Login] complete=14014; 
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** GuiDeNghiAsync chỉ tìm email tài khoản Học sinh; nhập Gia sư trả404, trong khi Excel yêu cầu400.

Vị trí cần xem trong ứng dụng: `Areas/HocTap/Services/LienKetTaiKhoanService.cs:25`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, SqlCommand command, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ThrowExceptionAndWarning(Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniError(TdsParserStateObject stateObj, UInt32 error)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniSyncOverAsync()
   at Microsoft.Data.SqlClient.TdsParserStateObject.TryPrepareBuffer()
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.CompleteLogin(Boolean enlistOK)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.LoginNoFailover(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.OpenLoginEnlist(TimeoutTimer timeout, SqlConnectionString connectionOptions, SqlCredential credential, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds..ctor(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, Object providerInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString userConnectionOptions, SessionData reconnectSessionData, Boolean applyTransientFaultHandling, String accessToken, IDbConnectionPool pool, Func`3 accessTokenCallback)
   at Microsoft.Data.ProviderBase.DbConnectionFactory.<>c__DisplayClass28_0.<CreateReplaceConnectionContinuation>b__0(Task`1 _)
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 194
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_073() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 75
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-073-IT27ce04cfe2864019b97a57097e011520.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 275
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_075() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 77
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-075-DL0-IT86d62053cd65471fbd6e8440ac1b35f9.json`, `Evidence\IT-075-DL1-IT3e7f6b4d2d464e92a4c97ac03f7f52bb.json`, `Evidence\IT-075-DL2-IT97cba1c0cd2449feb71a0a0a7e2223c4.json`, `Evidence\IT-075-IT387ddb6d1dcc4641af23289ecf57b283.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 275
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_076() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 78
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-076-DL0-IT5833b0332d4b4716bbf9bfb73e890c17.json`, `Evidence\IT-076-DL1-ITad2221479be2477080a70e229ab0551a.json`, `Evidence\IT-076-ITca259979550345238d0656fa5a7a8467.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 275
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_079() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 81
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-079-ITc492ea48fdbe493bbfc9bb81156e0391.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 275
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_080() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 82
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-080-G-ITc21b11f43b8847d6ae23bef4de75c7f7.json`, `Evidence\IT-080-H-ITdab70007aaf34c729b4d52e44d5cd24c.json`, `Evidence\IT-080-IT247a18c104884ef49ea2269a69aadad5.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 365
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_088() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 90
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-088-DL0-ITcd5381767aa0412fa1ab3e9d2eeceb62.json`, `Evidence\IT-088-ITbc8b213a1df4437797e46b47d1d679a0.json`

## IT-090 — Không đổi buổi có hoạt động thực tế về tương lai

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Các lỗi nghiệp vụ ở form Sua được chuyển ModelState: HTTP422 ok=false, phân biệt buổi có hoạt động/sai quyền/phiên bản; không thay giờ để che lịch sử.
2. Dữ liệu và side effect: Giữ thời gian/ghi nhận/tham dự/bài tập, không che lịch sử hoặc sinh lịch sử sửa giả.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: 422
Actual:   409
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.ScheduleCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ScheduleCases.cs:line 59
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 365
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_090() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 92
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-090-ITd286c782a5754b1eb049f057fa3c633d.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 365
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_093() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 95
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-093-IT9952aa817c3c4bc3aa540bebcfe3321e.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 310
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_108() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 110
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-108-DL0-ITf008e6d7eb274213bff84bc2976304b9.json`, `Evidence\IT-108-DL1-IT522061270c7c44a69002576f884db79a.json`, `Evidence\IT-108-DL2-IT75167ca2fb234aceb560cf65a4ff40f5.json`, `Evidence\IT-108-DL3-IT1da9d39025c44bc6960961c2302d0d93.json`, `Evidence\IT-108-IT7223e0bdc49e4921af6e85aaf49b377c.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 352
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_118() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 120
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-118-IT15ee7e655c3648e39ea94c335fd10890.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 352
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_123() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 125
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-123-ITbb8b8398757e4f28a58301111cb492ba.json`

## IT-124 — TepService ghép scanner process và file sandbox

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Tệp hợp lệ qua scanner, trạng thái Sach; không fake IVirusScanner để khẳng định đã kiểm protocol process.
2. Dữ liệu và side effect: File còn nguyên bytes, tên lưu không theo path người dùng, metadata đúng người/đối tượng khi transaction commit.

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
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.HienTaiAsync(CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 27
   at EduConnect.Mvc.Infrastructure.TepService.LuuAsync(List`1 files, Nullable`1 giao, Nullable`1 nop, Nullable`1 hoSo, Nullable`1 tinNhan, Nullable`1 baoCao) in C:\Khóa luận TN\EduConnect1\Infrastructure\TepService.cs:line 198
   at Cases.FileCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FileCases.cs:line 70
   at Cases.FileCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FileCases.cs:line 73
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 282
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_124() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 126
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-124-ITd342100e5de44ca9971dfb92d530648e.json`

## IT-130 — Thu hồi quyền trong lúc quét phải kiểm lại trước gắn tệp

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Tin nhắn bị từ chối sau kiểm lại quyền; không chấp nhận dựa vào quyền trước lúc quét.
2. Dữ liệu và side effect: Không TinNhan/TepDinhKem/outbox; blob tạm của thao tác thua được dọn; file cũ không bị xóa.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-130
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_130() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 132
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-130-ITf54838e75dce4b13b449b63b079648f2.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 246
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_132() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 134
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-132-ITd2809a64e5264e0283446ad926f6a728.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 242
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_134() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 136
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-134-ITc4edf369e02848a18d3c5f088a439d75.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 209
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_137() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 139
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-137-IT847523490b4a4df98835ab1cdbca7288.json`

## IT-153 — Tra cứu nhật ký an toàn và quản trị theo nguồn thật

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 20 mục/trang, đủ 25 không trùng; filter ngày đúng +07; tách audit AAA với lịch sử quản trị nhưng nối được bằng trace/đối tượng.
2. Dữ liệu và side effect: Không trả PasswordHash, cookie/token/password/plaintext định danh bị băm; việc xem audit cũng được ghi nhận, không sửa/xóa nguồn.

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
   at EduConnect.Mvc.Areas.QuanTri.Services.NhatKyAnToanQuery.LayAsync(NhatKyAnToanFilter filter, CancellationToken token) in C:\Khóa luận TN\EduConnect1\Areas\QuanTri\Services\NhatKyAnToanQuery.cs:line 47
   at Cases.AuditCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\AuditCases.cs:line 133
   at Cases.AuditCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\AuditCases.cs:line 142
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 214
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_153() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 155
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-153-IT90fe068888bc4d6d87a7f3a8d82cfdb7.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 301
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_158() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 160
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-158-IT950d7a56465b4144af254b6d0c370903.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 301
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_159() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 161
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-159-IT275dde2abfae4d73b9c26ecbfe1d5bb3.json`

## IT-160 — Sai mật khẩu, tài khoản khóa và đang chờ

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Không cookie đăng nhập thành công; không lộ tài khoản tồn tại qua thông báo khác nhau; đang chờ không được bỏ qua bằng password đúng.
2. Dữ liệu và side effect: Không phiên thành công; counters account/IP theo procedure; không log định danh thô/password.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : A connection was successfully established with the server, but then an error occurred during the pre-login handshake. (provider: SSL Provider, error: 0 - The wait operation timed out.)
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, SqlCommand command, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.ConsumePreLoginHandshake(SqlConnectionEncryptOption encrypt, Boolean trustServerCert, Boolean integratedSecurity, Boolean& marsCapable, Boolean& fedAuthRequired, Boolean tlsFirst, String serverCertificateFilename)
   at Microsoft.Data.SqlClient.TdsParser.Connect(ServerInfo serverInfo, SqlInternalConnectionTds connHandler, TimeoutTimer timeout, SqlConnectionString connectionOptions, Boolean withFailover)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.LoginNoFailover(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.OpenLoginEnlist(TimeoutTimer timeout, SqlConnectionString connectionOptions, SqlCredential credential, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds..ctor(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, Object providerInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString userConnectionOptions, SessionData reconnectSessionData, Boolean applyTransientFaultHandling, String accessToken, IDbConnectionPool pool, Func`3 accessTokenCallback)
   at Microsoft.Data.ProviderBase.DbConnectionFactory.<>c__DisplayClass28_0.<CreateReplaceConnectionContinuation>b__0(Task`1 _)
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 194
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_160() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 162
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-160-IT82c8146f63ad44af9d72a1530aa70d07.json`

## IT-161 — ReturnUrl ngoài miền không gây open redirect

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01/02 về Home/Index nội bộ; DL03 về URL nội bộ hợp lệ; không Location ra external.
2. Dữ liệu và side effect: Phiên vẫn thuộc H1; không gửi cookie/token trong query của URL bên ngoài.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Connection Timeout Expired.  The timeout period elapsed while attempting to consume the pre-login handshake acknowledgement.  This could be because the pre-login handshake failed or the server was unable to respond back in time.  The duration spent while attempting to connect to this server was - [Pre-Login] initialization=0; handshake=15006; 
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, SqlCommand command, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ThrowExceptionAndWarning(Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniError(TdsParserStateObject stateObj, UInt32 error)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniSyncOverAsync()
   at Microsoft.Data.SqlClient.TdsParser.ConsumePreLoginHandshake(SqlConnectionEncryptOption encrypt, Boolean trustServerCert, Boolean integratedSecurity, Boolean& marsCapable, Boolean& fedAuthRequired, Boolean tlsFirst, String serverCertificateFilename)
   at Microsoft.Data.SqlClient.TdsParser.Connect(ServerInfo serverInfo, SqlInternalConnectionTds connHandler, TimeoutTimer timeout, SqlConnectionString connectionOptions, Boolean withFailover)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.LoginNoFailover(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.OpenLoginEnlist(TimeoutTimer timeout, SqlConnectionString connectionOptions, SqlCredential credential, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds..ctor(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, Object providerInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString userConnectionOptions, SessionData reconnectSessionData, Boolean applyTransientFaultHandling, String accessToken, IDbConnectionPool pool, Func`3 accessTokenCallback)
   at Microsoft.Data.ProviderBase.DbConnectionFactory.<>c__DisplayClass28_0.<CreateReplaceConnectionContinuation>b__0(Task`1 _)
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 194
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_161() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 163
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-161-ITebf17327d2bc42409e23bd5ed582e58c.json`

## IT-162 — Đăng xuất thu hồi phiên và cookie cũ

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Logout redirect nội bộ; request dùng cookie cũ nhận 401, không còn quyền bằng cookie sao chép.
2. Dữ liệu và side effect: PhienDangNhap.ThuHoiLuc được lưu; dữ liệu lớp/tin/bài không bị xóa.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Connection Timeout Expired.  The timeout period elapsed while attempting to consume the pre-login handshake acknowledgement.  This could be because the pre-login handshake failed or the server was unable to respond back in time.  The duration spent while attempting to connect to this server was - [Pre-Login] initialization=10020; handshake=4986; 
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, SqlCommand command, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ThrowExceptionAndWarning(Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniError(TdsParserStateObject stateObj, UInt32 error)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniSyncOverAsync()
   at Microsoft.Data.SqlClient.TdsParser.ConsumePreLoginHandshake(SqlConnectionEncryptOption encrypt, Boolean trustServerCert, Boolean integratedSecurity, Boolean& marsCapable, Boolean& fedAuthRequired, Boolean tlsFirst, String serverCertificateFilename)
   at Microsoft.Data.SqlClient.TdsParser.Connect(ServerInfo serverInfo, SqlInternalConnectionTds connHandler, TimeoutTimer timeout, SqlConnectionString connectionOptions, Boolean withFailover)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.LoginNoFailover(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.OpenLoginEnlist(TimeoutTimer timeout, SqlConnectionString connectionOptions, SqlCredential credential, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds..ctor(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, Object providerInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString userConnectionOptions, SessionData reconnectSessionData, Boolean applyTransientFaultHandling, String accessToken, IDbConnectionPool pool, Func`3 accessTokenCallback)
   at Microsoft.Data.ProviderBase.DbConnectionFactory.<>c__DisplayClass28_0.<CreateReplaceConnectionContinuation>b__0(Task`1 _)
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 194
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_162() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 164
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-162-ITced6d2b3737048ac8659f38dc719cde1.json`

## IT-163 — Đổi mật khẩu thu hồi các phiên

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Đổi thành công, yêu cầu đăng nhập lại; cookie phiên2 bị 401; chỉ mật khẩu mới xác thực được.
2. Dữ liệu và side effect: Cập nhật hash/security stamp và thu hồi phiên; không đổi role/email/lịch sử lớp.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Connection Timeout Expired.  The timeout period elapsed while attempting to consume the pre-login handshake acknowledgement.  This could be because the pre-login handshake failed or the server was unable to respond back in time.  The duration spent while attempting to connect to this server was - [Pre-Login] initialization=2; handshake=15001; 
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, SqlCommand command, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ThrowExceptionAndWarning(Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniError(TdsParserStateObject stateObj, UInt32 error)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniSyncOverAsync()
   at Microsoft.Data.SqlClient.TdsParser.ConsumePreLoginHandshake(SqlConnectionEncryptOption encrypt, Boolean trustServerCert, Boolean integratedSecurity, Boolean& marsCapable, Boolean& fedAuthRequired, Boolean tlsFirst, String serverCertificateFilename)
   at Microsoft.Data.SqlClient.TdsParser.Connect(ServerInfo serverInfo, SqlInternalConnectionTds connHandler, TimeoutTimer timeout, SqlConnectionString connectionOptions, Boolean withFailover)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.LoginNoFailover(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.OpenLoginEnlist(TimeoutTimer timeout, SqlConnectionString connectionOptions, SqlCredential credential, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds..ctor(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, Object providerInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString userConnectionOptions, SessionData reconnectSessionData, Boolean applyTransientFaultHandling, String accessToken, IDbConnectionPool pool, Func`3 accessTokenCallback)
   at Microsoft.Data.ProviderBase.DbConnectionFactory.<>c__DisplayClass28_0.<CreateReplaceConnectionContinuation>b__0(Task`1 _)
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 194
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_163() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 165
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-163-IT9a5a8f6993794b6fb4deccca1b57e52e.json`

## IT-164 — Form quên mật khẩu không lộ email tồn tại

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Cùng trang xác nhận chung; không phản hồi để suy ra tài khoản tồn tại.
2. Dữ liệu và side effect: Chỉ email tồn tại có thư hợp lệ; không tạo tài khoản/phiên/reset hash cho email lạ.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Connection Timeout Expired.  The timeout period elapsed while attempting to consume the pre-login handshake acknowledgement.  This could be because the pre-login handshake failed or the server was unable to respond back in time.  The duration spent while attempting to connect to this server was - [Pre-Login] initialization=1; handshake=15000; 
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, SqlCommand command, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ThrowExceptionAndWarning(Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniError(TdsParserStateObject stateObj, UInt32 error)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniSyncOverAsync()
   at Microsoft.Data.SqlClient.TdsParser.ConsumePreLoginHandshake(SqlConnectionEncryptOption encrypt, Boolean trustServerCert, Boolean integratedSecurity, Boolean& marsCapable, Boolean& fedAuthRequired, Boolean tlsFirst, String serverCertificateFilename)
   at Microsoft.Data.SqlClient.TdsParser.Connect(ServerInfo serverInfo, SqlInternalConnectionTds connHandler, TimeoutTimer timeout, SqlConnectionString connectionOptions, Boolean withFailover)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.LoginNoFailover(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.OpenLoginEnlist(TimeoutTimer timeout, SqlConnectionString connectionOptions, SqlCredential credential, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds..ctor(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, Object providerInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString userConnectionOptions, SessionData reconnectSessionData, Boolean applyTransientFaultHandling, String accessToken, IDbConnectionPool pool, Func`3 accessTokenCallback)
   at Microsoft.Data.ProviderBase.DbConnectionFactory.<>c__DisplayClass28_0.<CreateReplaceConnectionContinuation>b__0(Task`1 _)
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 194
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_164() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 166
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-164-ITb51621f119b44d2fa74350f2db3282ae.json`

## IT-165 — Đặt lại với token đúng, sai, hết hạn và đã dùng

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Chỉ DL01 thành công; các DL khác báo liên kết không hợp lệ/hết hạn/đã dùng trên form, không reset password.
2. Dữ liệu và side effect: Hash đổi đúng một lần; sau reset thu hồi phiên; không reset user khác; không log token thô.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Connection Timeout Expired.  The timeout period elapsed while attempting to consume the pre-login handshake acknowledgement.  This could be because the pre-login handshake failed or the server was unable to respond back in time.  The duration spent while attempting to connect to this server was - [Pre-Login] initialization=1; handshake=15003; 
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, SqlCommand command, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ThrowExceptionAndWarning(Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniError(TdsParserStateObject stateObj, UInt32 error)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniSyncOverAsync()
   at Microsoft.Data.SqlClient.TdsParser.ConsumePreLoginHandshake(SqlConnectionEncryptOption encrypt, Boolean trustServerCert, Boolean integratedSecurity, Boolean& marsCapable, Boolean& fedAuthRequired, Boolean tlsFirst, String serverCertificateFilename)
   at Microsoft.Data.SqlClient.TdsParser.Connect(ServerInfo serverInfo, SqlInternalConnectionTds connHandler, TimeoutTimer timeout, SqlConnectionString connectionOptions, Boolean withFailover)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.LoginNoFailover(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.OpenLoginEnlist(TimeoutTimer timeout, SqlConnectionString connectionOptions, SqlCredential credential, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds..ctor(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, Object providerInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString userConnectionOptions, SessionData reconnectSessionData, Boolean applyTransientFaultHandling, String accessToken, IDbConnectionPool pool, Func`3 accessTokenCallback)
   at Microsoft.Data.ProviderBase.DbConnectionFactory.<>c__DisplayClass28_0.<CreateReplaceConnectionContinuation>b__0(Task`1 _)
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 194
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_165() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 167
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-165-IT1bb66ed117a14de18f21467c5ec047f5.json`

## IT-166 — Hồ sơ cá nhân dùng ConcurrencyStamp và email chỉ đọc

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Lượt mới lưu đúng; stamp cũ bị từ chối trên form; email đăng nhập không đổi theo field giả.
2. Dữ liệu và side effect: Giữ role/quyền/quan hệ lớp; không cập nhật user khác bằng Id gửi kèm.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Connection Timeout Expired.  The timeout period elapsed while attempting to consume the pre-login handshake acknowledgement.  This could be because the pre-login handshake failed or the server was unable to respond back in time.  The duration spent while attempting to connect to this server was - [Pre-Login] initialization=1; handshake=15005; 
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, SqlCommand command, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ThrowExceptionAndWarning(Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniError(TdsParserStateObject stateObj, UInt32 error)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniSyncOverAsync()
   at Microsoft.Data.SqlClient.TdsParser.ConsumePreLoginHandshake(SqlConnectionEncryptOption encrypt, Boolean trustServerCert, Boolean integratedSecurity, Boolean& marsCapable, Boolean& fedAuthRequired, Boolean tlsFirst, String serverCertificateFilename)
   at Microsoft.Data.SqlClient.TdsParser.Connect(ServerInfo serverInfo, SqlInternalConnectionTds connHandler, TimeoutTimer timeout, SqlConnectionString connectionOptions, Boolean withFailover)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.LoginNoFailover(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.OpenLoginEnlist(TimeoutTimer timeout, SqlConnectionString connectionOptions, SqlCredential credential, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds..ctor(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, Object providerInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString userConnectionOptions, SessionData reconnectSessionData, Boolean applyTransientFaultHandling, String accessToken, IDbConnectionPool pool, Func`3 accessTokenCallback)
   at Microsoft.Data.ProviderBase.DbConnectionFactory.<>c__DisplayClass28_0.<CreateReplaceConnectionContinuation>b__0(Task`1 _)
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 194
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_166() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 168
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-166-IT9fd175ed6dec4746a8cb615728236f73.json`

## IT-167 — Thay/xóa ảnh đại diện ghép metadata, file và quyền xem

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Ảnh hợp lệ được lưu/thay; giả định dạng bị từ chối; xóa khiến ảnh không còn được dùng nhưng không xóa tệp người khác.
2. Dữ liệu và side effect: User chỉ đổi ảnh mình; file/hash đúng; không trả physical path; không orphan ảnh khi SQL lưu thất bại.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Connection Timeout Expired.  The timeout period elapsed during the post-login phase.  The connection could have timed out while waiting for server to complete the login process and respond; Or it could have timed out while attempting to create multiple active connections.  The duration spent while attempting to connect to this server was - [Pre-Login] initialization=1; handshake=1; [Login] initialization=0; authentication=0; [Post-Login] complete=14012; 
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, SqlCommand command, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ThrowExceptionAndWarning(Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniError(TdsParserStateObject stateObj, UInt32 error)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniSyncOverAsync()
   at Microsoft.Data.SqlClient.TdsParserStateObject.TryPrepareBuffer()
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.CompleteLogin(Boolean enlistOK)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.LoginNoFailover(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.OpenLoginEnlist(TimeoutTimer timeout, SqlConnectionString connectionOptions, SqlCredential credential, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds..ctor(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, Object providerInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString userConnectionOptions, SessionData reconnectSessionData, Boolean applyTransientFaultHandling, String accessToken, IDbConnectionPool pool, Func`3 accessTokenCallback)
   at Microsoft.Data.ProviderBase.DbConnectionFactory.<>c__DisplayClass28_0.<CreateReplaceConnectionContinuation>b__0(Task`1 _)
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenInternalAsync(Boolean errorsExpected, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenInternalAsync(Boolean errorsExpected, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenAsync(CancellationToken cancellationToken, Boolean errorsExpected)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.SaveChangesAsync(IList`1 entries, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(IList`1 entriesToSave, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(StateManager stateManager, Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Fixture.User(String role) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 51
   at Fixture.User(String role) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 51
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 194
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_167() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 169
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-167-IT677ad10fd74f435caec5678b8b4a1ffb.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_175() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 177
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-175-Form-IT6ac2039ce0204910bf405d2bdef33759.json`, `Evidence\IT-175-IT57a74c33e2c240ee900eb59013f51651.json`

## IT-176 — Hub chỉ nhận phiên thật hợp lệ và kiểm quyền room

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: H1 được tham gia đúng room; anonymous/thu hồi không kết nối hợp lệ; X nhận HubException về quyền.
2. Dữ liệu và side effect: Không add X vào group C1; audit truy cập hub ghi đúng kết quả, không lộ tin hoặc cookie.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-176
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_176() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 178
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-176-IT17c399d7133d42cb93f863fc7a211d8d.json`

## IT-177 — Worker không phát trước commit rồi phát đúng EventId

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Chỉ sau commit mới nhận event CapNhat với maSuKien=E1,loai/doiTuongId/hoiThoaiId đúng contract.
2. Dữ liệu và side effect: E1=HoanTat, có XuLyXongLuc; release lease; notification không nhân đôi. Không dùng SELECT bị khóa để suy ra đã phát.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-177
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_177() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 179
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-177-ITa257c64bd25d4d4dbc9afbebb078ff3f.json`

## IT-178 — Transport lỗi giữ nghiệp vụ và retry có backoff

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Lượt lỗi không đánh HoanTat; sau phục hồi phát đúng E1 rồi hoàn tất.
2. Dữ liệu và side effect: Tin/notice gốc không mất hoặc nhân bản; SoLanThu tăng; lease được trả; client dùng EventId để chống hiển thị lặp.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-178
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_178() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 180
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-178-ITc2cdc7f1503c4666ab3a5943334a0493.json`

## IT-179 — Hai worker và lease hết hạn không làm mất sự kiện

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Event được claim hợp lệ rồi hoàn tất; lease hết hạn được phục hồi; lease còn hạn của worker khác không bị chiếm.
2. Dữ liệu và side effect: Không mất event hoặc ghi HoanTat từ lease không còn sở hữu; không kết luận exactly-once mạng, mọi replay mang cùng EventId.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-179
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_179() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 181
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-179-IT291fd5e06f264834958f965097c0fb56.json`

## IT-180 — Thu hồi quyền rời group và không nhận tin mới

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: P1 bị bỏ group/đóng kết nối theo điều kiện; không nhận payload tin sau thu hồi; HTTP quyền cũ bị từ chối.
2. Dữ liệu và side effect: Không bỏ H1/G1 khỏi lớp; lịch sử cũ giữ theo chính sách quyền; có audit denial.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-180
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_180() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 182
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-180-IT984123fc5bbf445e954282fba4ccf8cf.json`

## IT-181 — Ping/notification nền không gia hạn phiên

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Mốc hoạt động không được kéo dài bởi nền; khi quá30 phút phiên bị từ chối/đóng.
2. Dữ liệu và side effect: Không tạo phiên mới hoặc cập nhật thời điểm như hoạt động chủ động; audit đúng loại.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-181
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_181() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 183
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-181-ITd1b4c60ee5dc42f998f5f30c93abc7af.json`

## IT-187 — Dừng host không làm mất sự kiện chưa hoàn tất

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Worker dừng có kiểm soát; event chưa ack không bị tự đánh HoanTat.
2. Dữ liệu và side effect: E1 còn để retry/lease recovery và xử lý được ở lượt mới; không mất tin/notice đã commit.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-187
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_187() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 189
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-187-IT1fb131c6580f4f05a890732b28b8d2f5.json`

## Giới hạn của lượt thực thi

Đã chạy toàn bộ danh mục 191 mã: 118 Pass, 73 Fail, 0 Skip. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. 8 ca trước đây pending hiện chạy và Fail do dispatcher chưa có implementation. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
