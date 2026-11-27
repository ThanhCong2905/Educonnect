using System.Text.Json;using Xunit;using Xunit.Abstractions;using Xunit.Sdk;
[TestCaseOrderer("BottomUpOrder", "IntegrationCurrent")] public sealed class IntegrationTests {
[Fact(DisplayName="IT-001 | Một tài khoản chỉ có một vai trò")][Trait("TestCaseId","IT-001")][Trait("Group","00")][Trait("Stage","BU1")] public async Task IT_001() => await Run("IT-001","db_role");
[Fact(DisplayName="IT-002 | Khóa ghép nhu cầu–môn và khóa ngoại")][Trait("TestCaseId","IT-002")][Trait("Group","00")][Trait("Stage","BU1")] public async Task IT_002() => await Run("IT-002","db_need");
[Fact(DisplayName="IT-003 | Đề xuất SQL kiểm tra liên hệ và môn")][Trait("TestCaseId","IT-003")][Trait("Group","00")][Trait("Stage","BU1")] public async Task IT_003() => await Run("IT-003","db_contact");
[Fact(DisplayName="IT-004 | Chấp nhận SQL tạo đúng một lớp và dữ liệu con")][Trait("TestCaseId","IT-004")][Trait("Group","00")][Trait("Stage","BU1")] public async Task IT_004() => await Run("IT-004","db_accept");
[Fact(DisplayName="IT-005 | Chấp nhận SQL thất bại phải rollback")][Trait("TestCaseId","IT-005")][Trait("Group","00")][Trait("Stage","BU1")] public async Task IT_005() => await Run("IT-005","db_accept_fail");
[Fact(DisplayName="IT-006 | Procedure lịch và trigger tạo BuoiHoc")][Trait("TestCaseId","IT-006")][Trait("Group","00")][Trait("Stage","BU1")] public async Task IT_006() => await Run("IT-006","db_schedule");
[Fact(DisplayName="IT-007 | Procedure ghi nhận buổi kiểm người thực hiện")][Trait("TestCaseId","IT-007")][Trait("Group","00")][Trait("Stage","BU1")] public async Task IT_007() => await Run("IT-007","db_lesson");
[Fact(DisplayName="IT-008 | Nộp bài SQL giữ lịch sử và chống lặp mã")][Trait("TestCaseId","IT-008")][Trait("Group","00")][Trait("Stage","BU1")] public async Task IT_008() => await Run("IT-008","db_submit");
[Fact(DisplayName="IT-009 | Chấm SQL kiểm điểm, quyền và lần mới nhất")][Trait("TestCaseId","IT-009")][Trait("Group","00")][Trait("Stage","BU1")] public async Task IT_009() => await Run("IT-009","db_grade");
[Fact(DisplayName="IT-010 | Procedure lỗi đăng nhập và thời gian chờ")][Trait("TestCaseId","IT-010")][Trait("Group","00")][Trait("Stage","BU1")] public async Task IT_010() => await Run("IT-010","db_auth");
[Fact(DisplayName="IT-011 | Procedure phiên kiểm chủ, thu hồi và hết hạn")][Trait("TestCaseId","IT-011")][Trait("Group","00")][Trait("Stage","BU1")] public async Task IT_011() => await Run("IT-011","db_session");
[Fact(DisplayName="IT-012 | Principal ứng dụng bị chặn sửa/xóa nhật ký")][Trait("TestCaseId","IT-012")][Trait("Group","00")][Trait("Stage","BU1")] public async Task IT_012() => await Run("IT-012","db_audit");
[Fact(DisplayName="IT-013 | EF–SQL round-trip trạng thái, khóa phiên bản và mốc kết thúc")][Trait("TestCaseId","IT-013")][Trait("Group","00")][Trait("Stage","BU2")] public async Task IT_013() => await Run("IT-013","db_orm");
[Fact(DisplayName="IT-014 | Service tạo nhu cầu nhiều môn và đọc lại")][Trait("TestCaseId","IT-014")][Trait("Group","01")][Trait("Stage","BU2")] public async Task IT_014() => await Run("IT-014","need_store");
[Fact(DisplayName="IT-015 | Lỗi quan hệ môn không được lưu một phần nhu cầu")][Trait("TestCaseId","IT-015")][Trait("Group","01")][Trait("Stage","BU2")] public async Task IT_015() => await Run("IT-015","need_rollback");
[Fact(DisplayName="IT-016 | Hủy request khi chờ khóa SQL không để ghi một phần")][Trait("TestCaseId","IT-016")][Trait("Group","01")][Trait("Stage","BU2")] public async Task IT_016() => await Run("IT-016","need_cancel");
[Fact(DisplayName="IT-017 | Binding form tạo nhu cầu từ HTTP")][Trait("TestCaseId","IT-017")][Trait("Group","01")][Trait("Stage","BU3")] public async Task IT_017() => await Run("IT-017","need_create_http");
[Fact(DisplayName="IT-018 | Lưu nháp thiếu mục tiêu và lịch")][Trait("TestCaseId","IT-018")][Trait("Group","01")][Trait("Stage","BU3")] public async Task IT_018() => await Run("IT-018","need_draft");
[Fact(DisplayName="IT-019 | Phụ huynh đứng tên nhu cầu của người học")][Trait("TestCaseId","IT-019")][Trait("Group","01")][Trait("Stage","BU3")] public async Task IT_019() => await Run("IT-019","need_parent");
[Fact(DisplayName="IT-020 | Khách hoặc gia sư không tạo nhu cầu học")][Trait("TestCaseId","IT-020")][Trait("Group","01")][Trait("Stage","BU3")] public async Task IT_020() => await Run("IT-020","need_auth");
[Fact(DisplayName="IT-021 | Hạn mức nhu cầu mở, nháp và lượt đăng/ngày")][Trait("TestCaseId","IT-021")][Trait("Group","01")][Trait("Stage","BU3")] public async Task IT_021() => await Run("IT-021","need_limit");
[Fact(DisplayName="IT-022 | Chuẩn hóa trùng nhu cầu trước khi ghi")][Trait("TestCaseId","IT-022")][Trait("Group","01")][Trait("Stage","BU3")] public async Task IT_022() => await Run("IT-022","need_duplicate");
[Fact(DisplayName="IT-023 | Cập nhật thay môn/lịch, không đẩy như bài mới")][Trait("TestCaseId","IT-023")][Trait("Group","01")][Trait("Stage","BU3")] public async Task IT_023() => await Run("IT-023","need_update");
[Fact(DisplayName="IT-024 | Sai chủ hoặc phiên bản cũ không ghi đè")][Trait("TestCaseId","IT-024")][Trait("Group","01")][Trait("Stage","BU3")] public async Task IT_024() => await Run("IT-024","need_owner_version");
[Fact(DisplayName="IT-025 | Validation form, địa giới và học phí đi qua đúng khớp nối")][Trait("TestCaseId","IT-025")][Trait("Group","01")][Trait("Stage","BU3")] public async Task IT_025() => await Run("IT-025","need_fields");
[Fact(DisplayName="IT-026 | Đóng/mở lại và xác nhận còn nhu cầu")][Trait("TestCaseId","IT-026")][Trait("Group","01")][Trait("Stage","BU3")] public async Task IT_026() => await Run("IT-026","need_close");
[Fact(DisplayName="IT-027 | Form ghi thiếu CSRF không gọi nghiệp vụ")][Trait("TestCaseId","IT-027")][Trait("Group","01")][Trait("Stage","BU3")] public async Task IT_027() => await Run("IT-027","need_csrf");
[Fact(DisplayName="IT-028 | Service lưu hồ sơ, môn và minh chứng trong cùng giao dịch")][Trait("TestCaseId","IT-028")][Trait("Group","02")][Trait("Stage","BU2")] public async Task IT_028() => await Run("IT-028","profile_store");
[Fact(DisplayName="IT-029 | Service tìm kiếm ghép hồ sơ, môn, phí và đánh giá")][Trait("TestCaseId","IT-029")][Trait("Group","02")][Trait("Stage","BU2")] public async Task IT_029() => await Run("IT-029","search_store");
[Fact(DisplayName="IT-030 | Gợi ý có lý do, không tự tạo kết nối")][Trait("TestCaseId","IT-030")][Trait("Group","02")][Trait("Stage","BU2")] public async Task IT_030() => await Run("IT-030","search_suggest");
[Fact(DisplayName="IT-031 | Multipart hồ sơ giữ đúng liên kết minh chứng")][Trait("TestCaseId","IT-031")][Trait("Group","02")][Trait("Stage","BU3")] public async Task IT_031() => await Run("IT-031","profile_http");
[Fact(DisplayName="IT-032 | Thiếu minh chứng không lưu một phần hồ sơ")][Trait("TestCaseId","IT-032")][Trait("Group","02")][Trait("Stage","BU3")] public async Task IT_032() => await Run("IT-032","profile_missing");
[Fact(DisplayName="IT-033 | Hồ sơ phiên bản cũ không xóa minh chứng mới")][Trait("TestCaseId","IT-033")][Trait("Group","02")][Trait("Stage","BU3")] public async Task IT_033() => await Run("IT-033","profile_rv");
[Fact(DisplayName="IT-034 | Duyệt hồ sơ làm thay đổi điều kiện công khai")][Trait("TestCaseId","IT-034")][Trait("Group","02")][Trait("Stage","BU3")] public async Task IT_034() => await Run("IT-034","profile_review");
[Fact(DisplayName="IT-035 | Sửa nghề nghiệp/họ tên khi công khai phải theo điều kiện ẩn và duyệt lại")][Trait("TestCaseId","IT-035")][Trait("Group","02")][Trait("Stage","BU3")] public async Task IT_035() => await Run("IT-035","profile_hidden");
[Fact(DisplayName="IT-036 | Quyền xem trước và tải minh chứng")][Trait("TestCaseId","IT-036")][Trait("Group","02")][Trait("Stage","BU3")] public async Task IT_036() => await Run("IT-036","profile_view_auth");
[Fact(DisplayName="IT-037 | Khách xem hồ sơ và nhu cầu công khai")][Trait("TestCaseId","IT-037")][Trait("Group","02")][Trait("Stage","BU3")] public async Task IT_037() => await Run("IT-037","search_guest");
[Fact(DisplayName="IT-038 | Lọc nhiều tiêu chí với mức phí đúng môn và đơn vị")][Trait("TestCaseId","IT-038")][Trait("Group","02")][Trait("Stage","BU3")] public async Task IT_038() => await Run("IT-038","search_filter");
[Fact(DisplayName="IT-039 | Hồ sơ và nhu cầu bị ẩn/khóa không lọt kết quả")][Trait("TestCaseId","IT-039")][Trait("Group","02")][Trait("Stage","BU3")] public async Task IT_039() => await Run("IT-039","search_visibility");
[Fact(DisplayName="IT-040 | Phân trang và sắp xếp ổn định trên dữ liệu thật")][Trait("TestCaseId","IT-040")][Trait("Group","02")][Trait("Stage","BU3")] public async Task IT_040() => await Run("IT-040","search_pages");
[Fact(DisplayName="IT-041 | Bộ lọc sai kiểu không bị bỏ qua")][Trait("TestCaseId","IT-041")][Trait("Group","02")][Trait("Stage","BU3")] public async Task IT_041() => await Run("IT-041","search_binding");
[Fact(DisplayName="IT-042 | So sánh đủ điều kiện và loại Id trùng")][Trait("TestCaseId","IT-042")][Trait("Group","02")][Trait("Stage","BU3")] public async Task IT_042() => await Run("IT-042","search_compare");
[Fact(DisplayName="IT-043 | So sánh sai số lượng, loại hoặc điều kiện công khai")][Trait("TestCaseId","IT-043")][Trait("Group","02")][Trait("Stage","BU3")] public async Task IT_043() => await Run("IT-043","search_compare_reject");
[Fact(DisplayName="IT-044 | Service gửi/chấp nhận liên hệ ghép hội thoại và thông báo")][Trait("TestCaseId","IT-044")][Trait("Group","03")][Trait("Stage","BU2")] public async Task IT_044() => await Run("IT-044","contact_store");
[Fact(DisplayName="IT-045 | Service gửi đề xuất hai chiều và kiểm vai trò bên nhận")][Trait("TestCaseId","IT-045")][Trait("Group","03")][Trait("Stage","BU2")] public async Task IT_045() => await Run("IT-045","proposal_store");
[Fact(DisplayName="IT-046 | Hai gửi liên hệ đồng thời không nhân bản quan hệ")][Trait("TestCaseId","IT-046")][Trait("Group","03")][Trait("Stage","BU2")] public async Task IT_046() => await Run("IT-046","contact_race");
[Fact(DisplayName="IT-047 | Gửi liên hệ qua HTTP giữ danh tính từ phiên")][Trait("TestCaseId","IT-047")][Trait("Group","03")][Trait("Stage","BU3")] public async Task IT_047() => await Run("IT-047","contact_http");
[Fact(DisplayName="IT-048 | Chặn liên hệ không hợp lệ trước và trong giao dịch")][Trait("TestCaseId","IT-048")][Trait("Group","03")][Trait("Stage","BU3")] public async Task IT_048() => await Run("IT-048","contact_reject");
[Fact(DisplayName="IT-049 | Phản hồi liên hệ đúng bên và phiên bản")][Trait("TestCaseId","IT-049")][Trait("Group","03")][Trait("Stage","BU3")] public async Task IT_049() => await Run("IT-049","contact_reply");
[Fact(DisplayName="IT-050 | Chưa chấp nhận liên hệ không nhắn tin hoặc đề xuất")][Trait("TestCaseId","IT-050")][Trait("Group","03")][Trait("Stage","BU3")] public async Task IT_050() => await Run("IT-050","contact_gate_chat");
[Fact(DisplayName="IT-051 | Form đề xuất đầy đủ từ cả hai phía")][Trait("TestCaseId","IT-051")][Trait("Group","03")][Trait("Stage","BU3")] public async Task IT_051() => await Run("IT-051","proposal_http");
[Fact(DisplayName="IT-052 | Chỉ bên gửi được sửa đề xuất đang chờ")][Trait("TestCaseId","IT-052")][Trait("Group","03")][Trait("Stage","BU3")] public async Task IT_052() => await Run("IT-052","proposal_edit");
[Fact(DisplayName="IT-053 | Thiếu dữ liệu lớp hoặc mảng lịch bị loại")][Trait("TestCaseId","IT-053")][Trait("Group","03")][Trait("Stage","BU3")] public async Task IT_053() => await Run("IT-053","proposal_missing");
[Fact(DisplayName="IT-054 | Gửi/chấp nhận kiểm lại môn và nhu cầu bị ẩn")][Trait("TestCaseId","IT-054")][Trait("Group","03")][Trait("Stage","BU3")] public async Task IT_054() => await Run("IT-054","proposal_linked_subject");
[Fact(DisplayName="IT-055 | Từ chối và rút theo đúng bên tham gia")][Trait("TestCaseId","IT-055")][Trait("Group","03")][Trait("Stage","BU3")] public async Task IT_055() => await Run("IT-055","proposal_decline_withdraw");
[Fact(DisplayName="IT-056 | Điểm vào chuyển tiếp không bỏ kiểm tra actor và liên hệ")][Trait("TestCaseId","IT-056")][Trait("Group","03")][Trait("Stage","BU3")] public async Task IT_056() => await Run("IT-056","proposal_old_api");
[Fact(DisplayName="IT-057 | Service chấp nhận ghép yêu cầu, lớp và các bảng con")][Trait("TestCaseId","IT-057")][Trait("Group","04")][Trait("Stage","BU2")] public async Task IT_057() => await Run("IT-057","class_store");
[Fact(DisplayName="IT-058 | Service yêu cầu/phản hồi kết thúc ghi đủ dữ liệu")][Trait("TestCaseId","IT-058")][Trait("Group","04")][Trait("Stage","BU2")] public async Task IT_058() => await Run("IT-058","end_store");
[Fact(DisplayName="IT-059 | Lỗi sinh lịch phải rollback toàn bộ tạo lớp")][Trait("TestCaseId","IT-059")][Trait("Group","04")][Trait("Stage","BU2")] public async Task IT_059() => await Run("IT-059","class_rollback");
[Fact(DisplayName="IT-060 | Xác nhận lặp/đồng thời không tạo hai lớp")][Trait("TestCaseId","IT-060")][Trait("Group","04")][Trait("Stage","BU2")] public async Task IT_060() => await Run("IT-060","class_idempotent");
[Fact(DisplayName="IT-061 | Không hai yêu cầu kết thúc đang chờ")][Trait("TestCaseId","IT-061")][Trait("Group","04")][Trait("Stage","BU2")] public async Task IT_061() => await Run("IT-061","end_duplicate");
[Fact(DisplayName="IT-062 | HTTP chấp nhận tạo đúng lớp và redirectUrl")][Trait("TestCaseId","IT-062")][Trait("Group","04")][Trait("Stage","BU3")] public async Task IT_062() => await Run("IT-062","class_http");
[Fact(DisplayName="IT-063 | Không cho người gửi hoặc người ngoài tự chấp nhận")][Trait("TestCaseId","IT-063")][Trait("Group","04")][Trait("Stage","BU3")] public async Task IT_063() => await Run("IT-063","class_receiver");
[Fact(DisplayName="IT-064 | Khi xác nhận phải kiểm lại dữ liệu đã đổi")][Trait("TestCaseId","IT-064")][Trait("Group","04")][Trait("Stage","BU3")] public async Task IT_064() => await Run("IT-064","class_recheck");
[Fact(DisplayName="IT-065 | Cả Gia sư và Bên học đều được yêu cầu kết thúc")][Trait("TestCaseId","IT-065")][Trait("Group","04")][Trait("Stage","BU3")] public async Task IT_065() => await Run("IT-065","end_request_http");
[Fact(DisplayName="IT-066 | Đồng ý/từ chối trước hạn dùng RV yêu cầu")][Trait("TestCaseId","IT-066")][Trait("Group","04")][Trait("Stage","BU3")] public async Task IT_066() => await Run("IT-066","end_reply_http");
[Fact(DisplayName="IT-067 | Người gửi, người ngoài hoặc RV sai không quyết định kết thúc")][Trait("TestCaseId","IT-067")][Trait("Group","04")][Trait("Stage","BU3")] public async Task IT_067() => await Run("IT-067","end_auth_rv");
[Fact(DisplayName="IT-068 | Phản hồi sau hạn không kéo dài vì worker chậm")][Trait("TestCaseId","IT-068")][Trait("Group","04")][Trait("Stage","BU3")] public async Task IT_068() => await Run("IT-068","end_expired_reply");
[Fact(DisplayName="IT-069 | Lớp kết thúc giữ lịch sử và chặn thao tác học tập mới")][Trait("TestCaseId","IT-069")][Trait("Group","04")][Trait("Stage","BU3")] public async Task IT_069() => await Run("IT-069","ended_readonly");
[Fact(DisplayName="IT-070 | Service đề nghị/xác nhận liên kết không tự cấp quyền lớp")][Trait("TestCaseId","IT-070")][Trait("Group","05")][Trait("Stage","BU2")] public async Task IT_070() => await Run("IT-070","family_store");
[Fact(DisplayName="IT-071 | Service lời mời → đồng ý → duyệt ghi đủ quyền")][Trait("TestCaseId","IT-071")][Trait("Group","05")][Trait("Stage","BU2")] public async Task IT_071() => await Run("IT-071","member_store");
[Fact(DisplayName="IT-072 | Liên kết phụ huynh–học sinh qua HTTP")][Trait("TestCaseId","IT-072")][Trait("Group","05")][Trait("Stage","BU3")] public async Task IT_072() => await Run("IT-072","family_http");
[Fact(DisplayName="IT-073 | Không nhận liên kết sai vai trò, sai người hoặc RV cũ")][Trait("TestCaseId","IT-073")][Trait("Group","05")][Trait("Stage","BU3")] public async Task IT_073() => await Run("IT-073","family_wrong");
[Fact(DisplayName="IT-074 | Đề nghị thành viên tại action DeNghi thực tế")][Trait("TestCaseId","IT-074")][Trait("Group","05")][Trait("Stage","BU3")] public async Task IT_074() => await Run("IT-074","invite_http");
[Fact(DisplayName="IT-075 | Người nhận tự đồng ý hoặc từ chối lời mời")][Trait("TestCaseId","IT-075")][Trait("Group","05")][Trait("Stage","BU3")] public async Task IT_075() => await Run("IT-075","invite_consent");
[Fact(DisplayName="IT-076 | Gia sư duyệt Xem/Chỉnh sửa và quyền nộp riêng")][Trait("TestCaseId","IT-076")][Trait("Group","05")][Trait("Stage","BU3")] public async Task IT_076() => await Run("IT-076","member_approve");
[Fact(DisplayName="IT-077 | Duyệt lại điều kiện liên kết và hạn mức trong transaction")][Trait("TestCaseId","IT-077")][Trait("Group","05")][Trait("Stage","BU3")] public async Task IT_077() => await Run("IT-077","member_preconditions");
[Fact(DisplayName="IT-078 | Bên học/thành viên không duyệt quyền người khác")][Trait("TestCaseId","IT-078")][Trait("Group","05")][Trait("Stage","BU3")] public async Task IT_078() => await Run("IT-078","member_wrong_actor");
[Fact(DisplayName="IT-079 | Đổi/thu hồi quyền có hiệu lực ở request kế tiếp")][Trait("TestCaseId","IT-079")][Trait("Group","05")][Trait("Stage","BU3")] public async Task IT_079() => await Run("IT-079","member_revoke");
[Fact(DisplayName="IT-080 | Gỡ/rời thành viên không gỡ hai bên chính")][Trait("TestCaseId","IT-080")][Trait("Group","05")][Trait("Stage","BU3")] public async Task IT_080() => await Run("IT-080","member_leave");
[Fact(DisplayName="IT-081 | Hủy liên kết thu hồi quyền thành viên phụ thuộc")][Trait("TestCaseId","IT-081")][Trait("Group","05")][Trait("Stage","BU3")] public async Task IT_081() => await Run("IT-081","family_cancel");
[Fact(DisplayName="IT-082 | Biết Id/là người nhà không mở lớp khác hoặc chat riêng")][Trait("TestCaseId","IT-082")][Trait("Group","05")][Trait("Stage","BU3")] public async Task IT_082() => await Run("IT-082","member_isolation");
[Fact(DisplayName="IT-083 | Service thêm batch ghép LichHoc–BuoiHoc")][Trait("TestCaseId","IT-083")][Trait("Group","06")][Trait("Stage","BU2")] public async Task IT_083() => await Run("IT-083","schedule_store");
[Fact(DisplayName="IT-084 | Service bắt đầu và ghi nhận buổi giữ Id")][Trait("TestCaseId","IT-084")][Trait("Group","06")][Trait("Stage","BU2")] public async Task IT_084() => await Run("IT-084","lesson_store");
[Fact(DisplayName="IT-085 | Hai ghi nhận cùng RV không đè mất dữ liệu")][Trait("TestCaseId","IT-085")][Trait("Group","06")][Trait("Stage","BU2")] public async Task IT_085() => await Run("IT-085","lesson_record_race");
[Fact(DisplayName="IT-086 | Không nhân đôi buổi bù còn hiệu lực")][Trait("TestCaseId","IT-086")][Trait("Group","06")][Trait("Stage","BU2")] public async Task IT_086() => await Run("IT-086","lesson_makeup_duplicate");
[Fact(DisplayName="IT-087 | Form lịch hàng loạt và ngày/giờ local")][Trait("TestCaseId","IT-087")][Trait("Group","06")][Trait("Stage","BU3")] public async Task IT_087() => await Run("IT-087","schedule_http");
[Fact(DisplayName="IT-088 | Batch trùng nội bộ hoặc đụng lịch người tham gia phải atomic")][Trait("TestCaseId","IT-088")][Trait("Group","06")][Trait("Stage","BU3")] public async Task IT_088() => await Run("IT-088","schedule_conflict");
[Fact(DisplayName="IT-089 | Đổi lịch buổi chưa diễn ra giữ buổi và bài tập")][Trait("TestCaseId","IT-089")][Trait("Group","06")][Trait("Stage","BU3")] public async Task IT_089() => await Run("IT-089","schedule_change");
[Fact(DisplayName="IT-090 | Không đổi buổi có hoạt động thực tế về tương lai")][Trait("TestCaseId","IT-090")][Trait("Group","06")][Trait("Stage","BU3")] public async Task IT_090() => await Run("IT-090","schedule_change_reject");
[Fact(DisplayName="IT-091 | Đến giờ mới bắt đầu được buổi")][Trait("TestCaseId","IT-091")][Trait("Group","06")][Trait("Stage","BU3")] public async Task IT_091() => await Run("IT-091","lesson_start_http");
[Fact(DisplayName="IT-092 | Ghi nhận nội dung, kết quả và tham dự")][Trait("TestCaseId","IT-092")][Trait("Group","06")][Trait("Stage","BU3")] public async Task IT_092() => await Run("IT-092","lesson_record_http");
[Fact(DisplayName="IT-093 | Qua giờ nhưng chưa ghi nhận không tính đã học")][Trait("TestCaseId","IT-093")][Trait("Group","06")][Trait("Stage","BU3")] public async Task IT_093() => await Run("IT-093","lesson_unrecorded");
[Fact(DisplayName="IT-094 | Nghỉ cần bù chưa biết ngày vẫn được lưu")][Trait("TestCaseId","IT-094")][Trait("Group","06")][Trait("Stage","BU3")] public async Task IT_094() => await Run("IT-094","lesson_absence");
[Fact(DisplayName="IT-095 | Tạo buổi bù giữ liên kết buổi gốc")][Trait("TestCaseId","IT-095")][Trait("Group","06")][Trait("Stage","BU3")] public async Task IT_095() => await Run("IT-095","lesson_makeup");
[Fact(DisplayName="IT-096 | Service giao bài và tệp gắn đúng BuoiHoc")][Trait("TestCaseId","IT-096")][Trait("Group","07")][Trait("Stage","BU2")] public async Task IT_096() => await Run("IT-096","assignment_store");
[Fact(DisplayName="IT-097 | Service nộp/chấm ghép khóa mới nhất và lưu precision")][Trait("TestCaseId","IT-097")][Trait("Group","07")][Trait("Stage","BU2")] public async Task IT_097() => await Run("IT-097","submission_store");
[Fact(DisplayName="IT-098 | Retry cùng mã không tạo lần nộp mới")][Trait("TestCaseId","IT-098")][Trait("Group","07")][Trait("Stage","BU2")] public async Task IT_098() => await Run("IT-098","submission_idempotent");
[Fact(DisplayName="IT-099 | Lưu lần nộp trước, chỉ chấm lần mới nhất")][Trait("TestCaseId","IT-099")][Trait("Group","07")][Trait("Stage","BU2")] public async Task IT_099() => await Run("IT-099","submission_latest");
[Fact(DisplayName="IT-100 | Chặn sai gia sư, điểm ngoài miền và precision vượt hai số lẻ")][Trait("TestCaseId","IT-100")][Trait("Group","07")][Trait("Stage","BU2")] public async Task IT_100() => await Run("IT-100","grading_invalid");
[Fact(DisplayName="IT-101 | Nộp và chấm đồng thời kiểm lại trạng thái trong transaction")][Trait("TestCaseId","IT-101")][Trait("Group","07")][Trait("Stage","BU2")] public async Task IT_101() => await Run("IT-101","submission_grade_race");
[Fact(DisplayName="IT-102 | Multipart giao bài qua Controller")][Trait("TestCaseId","IT-102")][Trait("Group","07")][Trait("Stage","BU3")] public async Task IT_102() => await Run("IT-102","assignment_http");
[Fact(DisplayName="IT-103 | Buổi tương lai chỉ lưu nháp, công bố khi đủ điều kiện")][Trait("TestCaseId","IT-103")][Trait("Group","07")][Trait("Stage","BU3")] public async Task IT_103() => await Run("IT-103","assignment_draft");
[Fact(DisplayName="IT-104 | Bài bắt buộc gắn buổi đúng lớp, không giao vào buổi nghỉ/hủy")][Trait("TestCaseId","IT-104")][Trait("Group","07")][Trait("Stage","BU3")] public async Task IT_104() => await Run("IT-104","assignment_wrong_lesson");
[Fact(DisplayName="IT-105 | Gắn buổi cho bài cũ chưa có BuoiHocId")][Trait("TestCaseId","IT-105")][Trait("Group","07")][Trait("Stage","BU3")] public async Task IT_105() => await Run("IT-105","assignment_legacy");
[Fact(DisplayName="IT-106 | Nộp trước và sau hạn đều được ghi đúng thời điểm")][Trait("TestCaseId","IT-106")][Trait("Group","07")][Trait("Stage","BU3")] public async Task IT_106() => await Run("IT-106","submission_http");
[Fact(DisplayName="IT-107 | Đã có kết quả chấm không nhận lần nộp mới")][Trait("TestCaseId","IT-107")][Trait("Group","07")][Trait("Stage","BU3")] public async Task IT_107() => await Run("IT-107","submission_after_grade");
[Fact(DisplayName="IT-108 | Quyền nộp thay không suy ra từ quyền chat")][Trait("TestCaseId","IT-108")][Trait("Group","07")][Trait("Stage","BU3")] public async Task IT_108() => await Run("IT-108","submission_permissions");
[Fact(DisplayName="IT-109 | Chấm 0/10 hoặc chỉ nhận xét không quy đổi null thành 0")][Trait("TestCaseId","IT-109")][Trait("Group","07")][Trait("Stage","BU3")] public async Task IT_109() => await Run("IT-109","grading_http");
[Fact(DisplayName="IT-110 | Sửa điểm dùng RV của kết quả, chống ghi đè")][Trait("TestCaseId","IT-110")][Trait("Group","07")][Trait("Stage","BU3")] public async Task IT_110() => await Run("IT-110","grading_concurrency");
[Fact(DisplayName="IT-111 | Tổng hợp tiến độ từ nguồn thật với phép tính đối chiếu")][Trait("TestCaseId","IT-111")][Trait("Group","08")][Trait("Stage","BU2")] public async Task IT_111() => await Run("IT-111","progress_store");
[Fact(DisplayName="IT-112 | Hai bên đánh giá độc lập sau khi kết thúc có buổi thực tế")][Trait("TestCaseId","IT-112")][Trait("Group","08")][Trait("Stage","BU2")] public async Task IT_112() => await Run("IT-112","review_store");
[Fact(DisplayName="IT-113 | Khoảng ngày và trạng thái rỗng không tạo điểm 0 giả")][Trait("TestCaseId","IT-113")][Trait("Group","08")][Trait("Stage","BU2")] public async Task IT_113() => await Run("IT-113","progress_date");
[Fact(DisplayName="IT-114 | Bỏ kết quả lần cũ và bài nháp khi tổng hợp")][Trait("TestCaseId","IT-114")][Trait("Group","08")][Trait("Stage","BU2")] public async Task IT_114() => await Run("IT-114","progress_latest");
[Fact(DisplayName="IT-115 | Gửi lại không sửa điểm/nhận xét kể cả bị ẩn")][Trait("TestCaseId","IT-115")][Trait("Group","08")][Trait("Stage","BU2")] public async Task IT_115() => await Run("IT-115","review_immutable");
[Fact(DisplayName="IT-116 | Tiến độ và liên kết nguồn tại tổng quan lớp")][Trait("TestCaseId","IT-116")][Trait("Group","08")][Trait("Stage","BU3")] public async Task IT_116() => await Run("IT-116","progress_http");
[Fact(DisplayName="IT-117 | Người ngoài không đọc tiến độ/điểm")][Trait("TestCaseId","IT-117")][Trait("Group","08")][Trait("Stage","BU3")] public async Task IT_117() => await Run("IT-117","progress_auth");
[Fact(DisplayName="IT-118 | Đọc phiếu và lịch sử phản hồi cũ theo quyền lớp")][Trait("TestCaseId","IT-118")][Trait("Group","08")][Trait("Stage","BU3")] public async Task IT_118() => await Run("IT-118","legacy_feedback");
[Fact(DisplayName="IT-119 | Gửi đánh giá xác nhận quyền riêng tư")][Trait("TestCaseId","IT-119")][Trait("Group","08")][Trait("Stage","BU3")] public async Task IT_119() => await Run("IT-119","review_http");
[Fact(DisplayName="IT-120 | Thiếu điều kiện đánh giá không lưu dữ liệu")][Trait("TestCaseId","IT-120")][Trait("Group","08")][Trait("Stage","BU3")] public async Task IT_120() => await Run("IT-120","review_preconditions");
[Fact(DisplayName="IT-121 | Biên sao và nhận xét đi qua binding rồi nghiệp vụ")][Trait("TestCaseId","IT-121")][Trait("Group","08")][Trait("Stage","BU3")] public async Task IT_121() => await Run("IT-121","review_validation");
[Fact(DisplayName="IT-122 | Đối tác phản hồi giữ lịch sử và phát notice")][Trait("TestCaseId","IT-122")][Trait("Group","08")][Trait("Stage","BU3")] public async Task IT_122() => await Run("IT-122","review_response");
[Fact(DisplayName="IT-123 | Ẩn đánh giá thay đổi tổng hợp và quyền xem đánh giá Bên học")][Trait("TestCaseId","IT-123")][Trait("Group","08")][Trait("Stage","BU3")] public async Task IT_123() => await Run("IT-123","review_visibility");
[Fact(DisplayName="IT-124 | TepService ghép scanner process và file sandbox")][Trait("TestCaseId","IT-124")][Trait("Group","09")][Trait("Stage","BU2")] public async Task IT_124() => await Run("IT-124","file_store");
[Fact(DisplayName="IT-125 | Service gửi tin, tệp và outbox commit đồng bộ")][Trait("TestCaseId","IT-125")][Trait("Group","09")][Trait("Stage","BU2")] public async Task IT_125() => await Run("IT-125","chat_store");
[Fact(DisplayName="IT-126 | Mã thao tác tin nhắn chống retry và thay nội dung")][Trait("TestCaseId","IT-126")][Trait("Group","09")][Trait("Stage","BU2")] public async Task IT_126() => await Run("IT-126","chat_retry");
[Fact(DisplayName="IT-127 | Đuôi hợp lệ nhưng cấu trúc giả, macro hoặc traversal bị loại")][Trait("TestCaseId","IT-127")][Trait("Group","09")][Trait("Stage","BU2")] public async Task IT_127() => await Run("IT-127","file_content");
[Fact(DisplayName="IT-128 | Scanner báo nhiễm/lỗi/timeout không được gắn trạng thái sạch")][Trait("TestCaseId","IT-128")][Trait("Group","09")][Trait("Stage","BU2")] public async Task IT_128() => await Run("IT-128","file_scanner");
[Fact(DisplayName="IT-129 | Nhánh Development cho tệp chưa quét không áp sang môi trường khác")][Trait("TestCaseId","IT-129")][Trait("Group","09")][Trait("Stage","BU2")] public async Task IT_129() => await Run("IT-129","file_development");
[Fact(DisplayName="IT-130 | Thu hồi quyền trong lúc quét phải kiểm lại trước gắn tệp")][Trait("TestCaseId","IT-130")][Trait("Group","09")][Trait("Stage","BU2")] public async Task IT_130() => await Run("IT-130","file_revoke_race");
[Fact(DisplayName="IT-131 | Lỗi lưu SQL sau ghi blob không để orphan")][Trait("TestCaseId","IT-131")][Trait("Group","09")][Trait("Stage","BU2")] public async Task IT_131() => await Run("IT-131","file_transaction_failure");
[Fact(DisplayName="IT-132 | HTTP gửi tin giữ ownership và nội dung")][Trait("TestCaseId","IT-132")][Trait("Group","09")][Trait("Stage","BU3")] public async Task IT_132() => await Run("IT-132","chat_http");
[Fact(DisplayName="IT-133 | Phân trang cùng timestamp và mốc quyền xem")][Trait("TestCaseId","IT-133")][Trait("Group","09")][Trait("Stage","BU3")] public async Task IT_133() => await Run("IT-133","chat_history");
[Fact(DisplayName="IT-134 | Người ngoài, thành viên Xem và lớp kết thúc không gửi được tin")][Trait("TestCaseId","IT-134")][Trait("Group","09")][Trait("Stage","BU3")] public async Task IT_134() => await Run("IT-134","chat_readonly");
[Fact(DisplayName="IT-135 | Biên số tệp và dung lượng dùng bytes thật")][Trait("TestCaseId","IT-135")][Trait("Group","09")][Trait("Stage","BU3")] public async Task IT_135() => await Run("IT-135","file_boundary");
[Fact(DisplayName="IT-136 | Tải tệp kiểm lại quyền, scan state và hash")][Trait("TestCaseId","IT-136")][Trait("Group","09")][Trait("Stage","BU3")] public async Task IT_136() => await Run("IT-136","file_download");
[Fact(DisplayName="IT-137 | Service quản trị ghép claim quyền, Identity và dữ liệu SQL")][Trait("TestCaseId","IT-137")][Trait("Group","10")][Trait("Stage","BU2")] public async Task IT_137() => await Run("IT-137","admin_store");
[Fact(DisplayName="IT-138 | Service tạo báo cáo ghép quyền xem, bằng chứng và outbox")][Trait("TestCaseId","IT-138")][Trait("Group","10")][Trait("Stage","BU2")] public async Task IT_138() => await Run("IT-138","report_store");
[Fact(DisplayName="IT-139 | Gỡ một nguyên nhân chưa đủ để tiếp tục lớp")][Trait("TestCaseId","IT-139")][Trait("Group","10")][Trait("Stage","BU2")] public async Task IT_139() => await Run("IT-139","measure_release");
[Fact(DisplayName="IT-140 | Quản trị theo quyền chức năng, không chỉ role")][Trait("TestCaseId","IT-140")][Trait("Group","10")][Trait("Stage","BU3")] public async Task IT_140() => await Run("IT-140","admin_access");
[Fact(DisplayName="IT-141 | Yêu cầu sửa hồ sơ cần lý do và không công khai")][Trait("TestCaseId","IT-141")][Trait("Group","10")][Trait("Stage","BU3")] public async Task IT_141() => await Run("IT-141","admin_profile_reject");
[Fact(DisplayName="IT-142 | Báo cáo các loại đối tượng được phép xem")][Trait("TestCaseId","IT-142")][Trait("Group","10")][Trait("Stage","BU3")] public async Task IT_142() => await Run("IT-142","report_http");
[Fact(DisplayName="IT-143 | Không báo cáo đối tượng không được xem hoặc gửi trùng")][Trait("TestCaseId","IT-143")][Trait("Group","10")][Trait("Stage","BU3")] public async Task IT_143() => await Run("IT-143","report_visibility");
[Fact(DisplayName="IT-144 | Bằng chứng chỉ người gửi và reviewer đúng quyền được tải")][Trait("TestCaseId","IT-144")][Trait("Group","10")][Trait("Stage","BU3")] public async Task IT_144() => await Run("IT-144","report_evidence");
[Fact(DisplayName="IT-145 | Nhận xử lý báo cáo kiểm quyền và phiên bản")][Trait("TestCaseId","IT-145")][Trait("Group","10")][Trait("Stage","BU3")] public async Task IT_145() => await Run("IT-145","report_claim");
[Fact(DisplayName="IT-146 | Kết luận tách trạng thái xử lý khỏi biện pháp")][Trait("TestCaseId","IT-146")][Trait("Group","10")][Trait("Stage","BU3")] public async Task IT_146() => await Run("IT-146","report_resolve");
[Fact(DisplayName="IT-147 | Bổ sung/xem lại giữ kết luận và không tự gỡ biện pháp")][Trait("TestCaseId","IT-147")][Trait("Group","10")][Trait("Stage","BU3")] public async Task IT_147() => await Run("IT-147","report_reopen");
[Fact(DisplayName="IT-148 | Hậu kiểm nhu cầu: yêu cầu sửa/ẩn và xem lại")][Trait("TestCaseId","IT-148")][Trait("Group","10")][Trait("Stage","BU3")] public async Task IT_148() => await Run("IT-148","need_moderation");
[Fact(DisplayName="IT-149 | Khóa tài khoản thu hồi phiên và lan truyền trạng thái")][Trait("TestCaseId","IT-149")][Trait("Group","10")][Trait("Stage","BU3")] public async Task IT_149() => await Run("IT-149","account_lock");
[Fact(DisplayName="IT-150 | Không tự khóa hoặc loại bỏ quản trị cuối cùng")][Trait("TestCaseId","IT-150")][Trait("Group","10")][Trait("Stage","BU3")] public async Task IT_150() => await Run("IT-150","account_protect");
[Fact(DisplayName="IT-151 | Phân quyền thu hồi phiên mà không đổi vai trò nghiệp vụ")][Trait("TestCaseId","IT-151")][Trait("Group","10")][Trait("Stage","BU3")] public async Task IT_151() => await Run("IT-151","admin_permissions");
[Fact(DisplayName="IT-152 | Danh mục môn đổi tên/ngừng dùng giữ lịch sử và ảnh hưởng tìm kiếm")][Trait("TestCaseId","IT-152")][Trait("Group","10")][Trait("Stage","BU3")] public async Task IT_152() => await Run("IT-152","admin_subject");
[Fact(DisplayName="IT-153 | Tra cứu nhật ký an toàn và quản trị theo nguồn thật")][Trait("TestCaseId","IT-153")][Trait("Group","10")][Trait("Stage","BU3")] public async Task IT_153() => await Run("IT-153","audit_query");
[Fact(DisplayName="IT-154 | Service đăng ký ghép Identity, vai trò và hồ sơ")][Trait("TestCaseId","IT-154")][Trait("Group","11")][Trait("Stage","BU2")] public async Task IT_154() => await Run("IT-154","auth_store");
[Fact(DisplayName="IT-155 | Mật khẩu phải được Identity xác minh trước SQL đăng nhập thành công")][Trait("TestCaseId","IT-155")][Trait("Group","11")][Trait("Stage","BU2")] public async Task IT_155() => await Run("IT-155","auth_verify");
[Fact(DisplayName="IT-156 | Quên mật khẩu ghép token và mail sandbox")][Trait("TestCaseId","IT-156")][Trait("Group","11")][Trait("Stage","BU2")] public async Task IT_156() => await Run("IT-156","auth_reset_mail");
[Fact(DisplayName="IT-157 | Đăng nhập HTTP phát cookie và phiên thật")][Trait("TestCaseId","IT-157")][Trait("Group","11")][Trait("Stage","BU3")] public async Task IT_157() => await Run("IT-157","auth_login_http");
[Fact(DisplayName="IT-158 | Form đăng ký ba vai trò công khai")][Trait("TestCaseId","IT-158")][Trait("Group","11")][Trait("Stage","BU3")] public async Task IT_158() => await Run("IT-158","auth_register_http");
[Fact(DisplayName="IT-159 | Trùng email hoặc yêu cầu role quản trị không được cấp")][Trait("TestCaseId","IT-159")][Trait("Group","11")][Trait("Stage","BU3")] public async Task IT_159() => await Run("IT-159","auth_register_reject");
[Fact(DisplayName="IT-160 | Sai mật khẩu, tài khoản khóa và đang chờ")][Trait("TestCaseId","IT-160")][Trait("Group","11")][Trait("Stage","BU3")] public async Task IT_160() => await Run("IT-160","auth_lock_counter");
[Fact(DisplayName="IT-161 | ReturnUrl ngoài miền không gây open redirect")][Trait("TestCaseId","IT-161")][Trait("Group","11")][Trait("Stage","BU3")] public async Task IT_161() => await Run("IT-161","auth_return_url");
[Fact(DisplayName="IT-162 | Đăng xuất thu hồi phiên và cookie cũ")][Trait("TestCaseId","IT-162")][Trait("Group","11")][Trait("Stage","BU3")] public async Task IT_162() => await Run("IT-162","auth_logout");
[Fact(DisplayName="IT-163 | Đổi mật khẩu thu hồi các phiên")][Trait("TestCaseId","IT-163")][Trait("Group","11")][Trait("Stage","BU3")] public async Task IT_163() => await Run("IT-163","auth_change_password");
[Fact(DisplayName="IT-164 | Form quên mật khẩu không lộ email tồn tại")][Trait("TestCaseId","IT-164")][Trait("Group","11")][Trait("Stage","BU3")] public async Task IT_164() => await Run("IT-164","auth_forgot_http");
[Fact(DisplayName="IT-165 | Đặt lại với token đúng, sai, hết hạn và đã dùng")][Trait("TestCaseId","IT-165")][Trait("Group","11")][Trait("Stage","BU3")] public async Task IT_165() => await Run("IT-165","auth_reset_http");
[Fact(DisplayName="IT-166 | Hồ sơ cá nhân dùng ConcurrencyStamp và email chỉ đọc")][Trait("TestCaseId","IT-166")][Trait("Group","11")][Trait("Stage","BU3")] public async Task IT_166() => await Run("IT-166","auth_profile");
[Fact(DisplayName="IT-167 | Thay/xóa ảnh đại diện ghép metadata, file và quyền xem")][Trait("TestCaseId","IT-167")][Trait("Group","11")][Trait("Stage","BU3")] public async Task IT_167() => await Run("IT-167","auth_avatar");
[Fact(DisplayName="IT-168 | Phiên hết nhàn rỗi hoặc đã thu hồi không truy cập được")][Trait("TestCaseId","IT-168")][Trait("Group","11")][Trait("Stage","BU3")] public async Task IT_168() => await Run("IT-168","auth_session_expired");
[Fact(DisplayName="IT-169 | Rate limiter/antiforgery không tạo ghi nghiệp vụ ngoài ý muốn")][Trait("TestCaseId","IT-169")][Trait("Group","11")][Trait("Stage","BU3")] public async Task IT_169() => await Run("IT-169","auth_rate_limit");
[Fact(DisplayName="IT-170 | Producer lưu notification/outbox trong cùng transaction")][Trait("TestCaseId","IT-170")][Trait("Group","12")][Trait("Stage","BU2")] public async Task IT_170() => await Run("IT-170","notice_store");
[Fact(DisplayName="IT-171 | Audit service/middleware ghép SQL bằng scope độc lập")][Trait("TestCaseId","IT-171")][Trait("Group","12")][Trait("Stage","BU2")] public async Task IT_171() => await Run("IT-171","audit_store");
[Fact(DisplayName="IT-172 | Đọc một/đọc tất cả thông báo đúng người nhận")][Trait("TestCaseId","IT-172")][Trait("Group","12")][Trait("Stage","BU3")] public async Task IT_172() => await Run("IT-172","notice_http");
[Fact(DisplayName="IT-173 | Audit liên thông request thành công/deny/CSRF/tải tệp")][Trait("TestCaseId","IT-173")][Trait("Group","12")][Trait("Stage","BU3")] public async Task IT_173() => await Run("IT-173","audit_http");
[Fact(DisplayName="IT-174 | Lỗi lưu audit không hủy giao dịch nghiệp vụ đã commit")][Trait("TestCaseId","IT-174")][Trait("Group","12")][Trait("Stage","BU3")] public async Task IT_174() => await Run("IT-174","audit_failure");
[Fact(DisplayName="IT-175 | MVC/filter ánh xạ response đúng kiểu request")][Trait("TestCaseId","IT-175")][Trait("Group","12")][Trait("Stage","BU3")] public async Task IT_175() => await Run("IT-175","mvc_contract");
[Fact(DisplayName="IT-176 | Hub chỉ nhận phiên thật hợp lệ và kiểm quyền room")][Trait("TestCaseId","IT-176")][Trait("Group","12")][Trait("Stage","BU4")] public async Task IT_176() => await Run("IT-176","hub_session");
[Fact(DisplayName="IT-177 | Worker không phát trước commit rồi phát đúng EventId")][Trait("TestCaseId","IT-177")][Trait("Group","12")][Trait("Stage","BU4")] public async Task IT_177() => await Run("IT-177","outbox_commit");
[Fact(DisplayName="IT-178 | Transport lỗi giữ nghiệp vụ và retry có backoff")][Trait("TestCaseId","IT-178")][Trait("Group","12")][Trait("Stage","BU4")] public async Task IT_178() => await Run("IT-178","outbox_retry");
[Fact(DisplayName="IT-179 | Hai worker và lease hết hạn không làm mất sự kiện")][Trait("TestCaseId","IT-179")][Trait("Group","12")][Trait("Stage","BU4")] public async Task IT_179() => await Run("IT-179","outbox_lease");
[Fact(DisplayName="IT-180 | Thu hồi quyền rời group và không nhận tin mới")][Trait("TestCaseId","IT-180")][Trait("Group","12")][Trait("Stage","BU4")] public async Task IT_180() => await Run("IT-180","hub_revoke");
[Fact(DisplayName="IT-181 | Ping/notification nền không gia hạn phiên")][Trait("TestCaseId","IT-181")][Trait("Group","12")][Trait("Stage","BU4")] public async Task IT_181() => await Run("IT-181","hub_background");
[Fact(DisplayName="IT-182 | Bảo trì hết hạn đề xuất không tạo lớp")][Trait("TestCaseId","IT-182")][Trait("Group","12")][Trait("Stage","BU4")] public async Task IT_182() => await Run("IT-182","worker_proposal_expire");
[Fact(DisplayName="IT-183 | Hết72 giờ tự kết thúc đúng yêu cầu còn chờ")][Trait("TestCaseId","IT-183")][Trait("Group","12")][Trait("Stage","BU4")] public async Task IT_183() => await Run("IT-183","worker_end");
[Fact(DisplayName="IT-184 | Worker và phản hồi đúng hạn cạnh tranh có một kết quả hợp lệ")][Trait("TestCaseId","IT-184")][Trait("Group","12")][Trait("Stage","BU4")] public async Task IT_184() => await Run("IT-184","worker_end_race");
[Fact(DisplayName="IT-185 | Nhắc30 ngày và đóng sau7 ngày không xác nhận")][Trait("TestCaseId","IT-185")][Trait("Group","12")][Trait("Stage","BU4")] public async Task IT_185() => await Run("IT-185","worker_need_expire");
[Fact(DisplayName="IT-186 | Nhắc buổi/bài từ dữ liệu thật không tự chấm hoặc ghi nhận")][Trait("TestCaseId","IT-186")][Trait("Group","12")][Trait("Stage","BU4")] public async Task IT_186() => await Run("IT-186","worker_learning_reminder");
[Fact(DisplayName="IT-187 | Dừng host không làm mất sự kiện chưa hoàn tất")][Trait("TestCaseId","IT-187")][Trait("Group","12")][Trait("Stage","BU4")] public async Task IT_187() => await Run("IT-187","async_shutdown");
static async Task Run(string id,string key){await using var f=new Fixture(id);var doc=JsonDocument.Parse(await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory,"Manifest.json")));var c=doc.RootElement.GetProperty("cases").EnumerateArray().Single(x=>x.GetProperty("id").GetString()==id);f.Proof["baseline"]=c.Clone();try {await f.Init();switch(key){
case "admin_store":
case "report_store":
case "measure_release":
case "report_http":
case "report_visibility":
case "report_evidence":
case "report_claim":
case "report_resolve":
case "report_reopen":
case "need_moderation":
case "account_lock":
case "account_protect":
case "admin_permissions":
case "admin_subject":
await Cases.AdminCases(f,key);break;
case "audit_query":
case "audit_store":
case "audit_http":
case "audit_failure":
await Cases.AuditCases(f,key);break;
case "auth_store":
case "auth_verify":
case "auth_reset_mail":
case "auth_forgot_http":
case "auth_reset_http":
case "auth_profile":
case "auth_avatar":
case "auth_rate_limit":
await Cases.AuthCases(f,key);break;
case "contact_store":
case "proposal_store":
case "contact_race":
case "class_store":
case "end_store":
case "class_idempotent":
case "end_duplicate":
case "schedule_store":
case "assignment_store":
case "submission_store":
case "submission_idempotent":
case "submission_latest":
case "grading_invalid":
case "submission_grade_race":
case "assignment_http":
case "chat_store":
case "chat_retry":
case "chat_readonly":
await Cases.BusinessCases(f,key);break;
case "chat_http":
case "chat_history":
case "file_download":
await Cases.ChatCases(f,key);break;
case "contact_gate_chat":
case "proposal_http":
case "proposal_edit":
case "proposal_missing":
case "proposal_linked_subject":
case "proposal_decline_withdraw":
case "proposal_old_api":
case "class_rollback":
case "class_recheck":
case "end_request_http":
case "end_reply_http":
case "end_auth_rv":
case "end_expired_reply":
case "ended_readonly":
await Cases.ConnectionCases(f,key);break;
case "family_store":
case "member_store":
case "family_http":
case "family_wrong":
case "invite_http":
case "invite_consent":
case "member_approve":
case "member_preconditions":
case "member_wrong_actor":
case "member_revoke":
case "member_leave":
case "family_cancel":
case "member_isolation":
await Cases.FamilyCases(f,key);break;
case "file_store":
case "file_content":
case "file_scanner":
case "file_development":
case "file_transaction_failure":
case "file_boundary":
await Cases.FileCases(f,key);break;
case "search_guest":
case "search_binding":
case "search_compare":
case "search_compare_reject":
case "contact_http":
case "contact_reject":
case "contact_reply":
case "class_http":
case "class_receiver":
case "progress_auth":
case "auth_login_http":
case "auth_register_http":
case "auth_register_reject":
case "auth_lock_counter":
case "auth_return_url":
case "auth_logout":
case "auth_change_password":
case "auth_session_expired":
await Cases.HttpCases(f,key);break;
case "assignment_draft":
case "assignment_wrong_lesson":
case "assignment_legacy":
case "submission_http":
case "submission_after_grade":
case "submission_permissions":
case "grading_http":
case "grading_concurrency":
await Cases.LearningCases(f,key);break;
case "need_store":
case "need_rollback":
case "need_cancel":
case "need_create_http":
case "need_draft":
case "need_parent":
case "need_auth":
case "need_limit":
case "need_duplicate":
case "need_update":
case "need_owner_version":
case "need_fields":
case "need_close":
case "need_csrf":
case "mvc_contract":
await Cases.NeedCases(f,key);break;
case "notice_store":
case "notice_http":
await Cases.NoticeCases(f,key);break;
case "profile_store":
case "profile_http":
case "profile_missing":
case "profile_rv":
case "profile_review":
case "profile_hidden":
case "profile_view_auth":
case "admin_access":
case "admin_profile_reject":
await Cases.ProfileCases(f,key);break;
case "progress_store":
case "review_store":
case "progress_date":
case "progress_latest":
case "review_immutable":
case "progress_http":
case "legacy_feedback":
case "review_http":
case "review_preconditions":
case "review_validation":
case "review_response":
case "review_visibility":
await Cases.ProgressCases(f,key);break;
case "file_revoke_race":
case "hub_session":
case "outbox_commit":
case "outbox_retry":
case "outbox_lease":
case "hub_revoke":
case "hub_background":
case "async_shutdown":
await Cases.RealtimeCases(f,key);break;
case "lesson_store":
case "lesson_record_race":
case "lesson_makeup_duplicate":
case "schedule_http":
case "schedule_conflict":
case "schedule_change":
case "schedule_change_reject":
case "lesson_start_http":
case "lesson_record_http":
case "lesson_unrecorded":
case "lesson_absence":
case "lesson_makeup":
await Cases.ScheduleCases(f,key);break;
case "search_store":
case "search_suggest":
case "search_filter":
case "search_visibility":
case "search_pages":
await Cases.SearchCases(f,key);break;
case "db_role":
case "db_need":
case "db_contact":
case "db_accept":
case "db_accept_fail":
case "db_schedule":
case "db_lesson":
case "db_submit":
case "db_grade":
case "db_auth":
case "db_session":
case "db_audit":
case "db_orm":
await Cases.SqlContracts(f,key);break;
case "worker_proposal_expire":
case "worker_end":
case "worker_end_race":
case "worker_need_expire":
case "worker_learning_reminder":
await Cases.WorkerCases(f,key);break;
default:throw new InvalidOperationException("No implementation: "+id); } f.Proof["assertions"]="Passed";}catch(Exception ex){f.Proof["exception"]=ex.ToString();throw;}}}
public sealed class BottomUpOrder:ITestCaseOrderer {
 public IEnumerable<T> OrderTestCases<T>(IEnumerable<T> cases) where T:ITestCase {
 var doc=JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"Manifest.json")));
 var ranks=doc.RootElement.GetProperty("cases").EnumerateArray().ToDictionary(c=>c.GetProperty("id").GetString()!.Replace('-','_'),c=>c.GetProperty("executionOrder").GetInt32());
 return cases.OrderBy(c=>ranks.GetValueOrDefault(c.TestMethod.Method.Name,int.MaxValue));}}
