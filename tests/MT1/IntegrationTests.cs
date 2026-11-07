using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

// Oracle: Intergration Test MT1 trong bảng TC người dùng ngày 07/11/2026.
// Assertion giữ theo TC, không đổi ứng dụng hoặc kỳ vọng chỉ để kết quả xanh.
[Trait("Suite", "MT1Integration")]
public partial class IntegrationTests(ITestOutputHelper output)
{

    #region TC 014 — Nhu cầu học

    // MT1-IT-014: Lưu nhu cầu học sinh trực tuyến
    // Kỳ vọng: Trả Guid khác rỗng; DB thêm 1 NhuCauHoc TimGiaSu/DangMo đúng chủ và 2 slot; TenNguoiHoc/TuoiNguoiHoc=null.
    [Fact(DisplayName = "MT1-IT-014 · Lưu nhu cầu học sinh trực tuyến")]
    public Task MT1_IT_014() => Run("MT1-IT-014", c => Service(c, 14));

    // MT1-IT-015: Phụ huynh mô tả người học
    // Kỳ vọng: 1 nhu cầu của PH-A; tên/tuổi người học được lưu; không tạo tài khoản AspNetUsers mới cho người học.
    [Fact(DisplayName = "MT1-IT-015 · Phụ huynh mô tả người học")]
    public Task MT1_IT_015() => Run("MT1-IT-015", c => Service(c, 15));

    // MT1-IT-016: Gia sư bị từ chối tạo nhu cầu học
    // Kỳ vọng: LoiNghiepVu Status=403; không thêm nhu cầu/slot. Gia sư quản lý hồ sơ, không có luồng tạo nhu cầu nhận lớp.
    [Fact(DisplayName = "MT1-IT-016 · Gia sư bị từ chối tạo nhu cầu học")]
    public Task MT1_IT_016() => Run("MT1-IT-016", c => Service(c, 16));

    // MT1-IT-017: Đăng học trực tiếp có khu vực
    // Kỳ vọng: Lưu đúng khu vực/địa điểm; trạng thái DangMo.
    [Fact(DisplayName = "MT1-IT-017 · Đăng học trực tiếp có khu vực")]
    public Task MT1_IT_017() => Run("MT1-IT-017", c => Service(c, 17));

    // MT1-IT-018: Học trực tiếp thiếu khu vực
    // Kỳ vọng: LoiNghiepVu400; không có nhu cầu/slot mới.
    [Fact(DisplayName = "MT1-IT-018 · Học trực tiếp thiếu khu vực")]
    public Task MT1_IT_018() => Run("MT1-IT-018", c => Service(c, 18));

    // MT1-IT-019: Trực tuyến:quy tắc xử lý địa chỉ và dữ liệu công khai
    // Kỳ vọng: Không công khai địa chỉ chi tiết. Quy tắc giữ/xóa KhuVuc và DiaChiChiTiet khi chọn trực tuyến cần được xác nhận trong đặc tả trước khi đánh giá phần dữ liệu lưu.
    [Fact(DisplayName = "MT1-IT-019 · Trực tuyến:quy tắc xử lý địa chỉ và dữ liệu công khai", Skip = "Chưa đặc tả quy tắc giữ/xóa địa chỉ trực tuyến; chưa thể kết luận toàn bộ ca.")]
    public Task MT1_IT_019() => Run("MT1-IT-019", c => Service(c, 19));

    // MT1-IT-020: Đăng thiếu mục tiêu
    // Kỳ vọng: LoiNghiepVu400; không ghi nhu cầu/slot.
    [Fact(DisplayName = "MT1-IT-020 · Đăng thiếu mục tiêu")]
    public Task MT1_IT_020() => Run("MT1-IT-020", c => Service(c, 20));

    // MT1-IT-021: Đăng thiếu số buổi
    // Kỳ vọng: LoiNghiepVu400; không ghi dữ liệu; phân biệt với Validator nullable.
    [Fact(DisplayName = "MT1-IT-021 · Đăng thiếu số buổi")]
    public Task MT1_IT_021() => Run("MT1-IT-021", c => Service(c, 21));

    // MT1-IT-022: Lưu nháp chưa có mục tiêu/lịch
    // Kỳ vọng: Lưu BanNhap, không có slot; không xuất hiện trong truy vấn công khai.
    [Fact(DisplayName = "MT1-IT-022 · Lưu nháp chưa có mục tiêu/lịch")]
    public Task MT1_IT_022() => Run("MT1-IT-022", c => Service(c, 22));

    // MT1-IT-023: Nháp vẫn kiểm học phí
    // Kỳ vọng: LoiNghiepVu400; không lưu nháp sai học phí.
    [Fact(DisplayName = "MT1-IT-023 · Nháp vẫn kiểm học phí")]
    public Task MT1_IT_023() => Run("MT1-IT-023", c => Service(c, 23));

    // MT1-IT-024: Phụ huynh thiếu tên
    // Kỳ vọng: Từ chối 400; không ghi dữ liệu.
    [Fact(DisplayName = "MT1-IT-024 · Phụ huynh thiếu tên")]
    public Task MT1_IT_024() => Run("MT1-IT-024", c => Service(c, 24));

    // MT1-IT-025: Phụ huynh có tên và lớp nhưng thiếu tuổi
    // Kỳ vọng: Chấp nhận họ tên người học + lớp khi các dữ liệu còn lại hợp lệ theo SRS FR-01; không bắt buộc đồng thời tuổi và lớp. Nhu cầu thuộc PH-A; TuoiNguoiHoc lưu NULL.
    [Fact(DisplayName = "MT1-IT-025 · Phụ huynh có tên và lớp nhưng thiếu tuổi")]
    public Task MT1_IT_025() => Run("MT1-IT-025", c => Service(c, 25));

    // MT1-IT-026: Tạo nhu cầu với danh sách hai môn hợp lệ.
    // Căn cứ: ViewModel/NhuCauInput.MonHocIds; SC_UC1.1A 3.4–3.19, SC_UC1.1b 1.2–1.7.
    // Kỳ vọng: Id trả về đọc lại đủ hai môn, đúng chủ/trạng thái và hai khung giờ; không suy ra một bản ghi nhu cầu cho mỗi môn.
    [Fact(DisplayName = "MT1-IT-026 · Lưu và đọc lại nhu cầu có hai môn")]
    public Task MT1_IT_026() => Run("MT1-IT-026", c => Service(c, 26));

    // MT1-IT-027: Trùng Id môn được chuẩn hóa
    // Kỳ vọng: Đọc lại đủ hai Id môn phân biệt, không lặp môn hoặc nhân đôi lịch; không đếm số nhu cầu theo số môn.
    [Fact(DisplayName = "MT1-IT-027 · Trùng Id môn được chuẩn hóa")]
    public Task MT1_IT_027() => Run("MT1-IT-027", c => Service(c, 27));

    // MT1-IT-028: Không chọn môn
    // Kỳ vọng: LoiNghiepVu400; không tạo dữ liệu.
    [Fact(DisplayName = "MT1-IT-028 · Không chọn môn")]
    public Task MT1_IT_028() => Run("MT1-IT-028", c => Service(c, 28));

    // MT1-IT-029: Danh sách đầu vào gồm 10 môn hợp lệ (dữ liệu thử, không phải giới hạn thiết kế).
    // Kỳ vọng: Đọc lại đủ 10 môn của Id được trả về, đúng hai lịch đầu vào, không nhân bản lịch theo số môn.
    [Fact(DisplayName = "MT1-IT-029 · Lưu đầy đủ danh sách 10 môn hợp lệ")]
    public Task MT1_IT_029() => Run("MT1-IT-029", c => Service(c, 29));

    // MT1-IT-030: Vượt 10 môn
    // Kỳ vọng: LoiNghiepVu400; không tạo dữ liệu.
    [Fact(DisplayName = "MT1-IT-030 · Vượt 10 môn")]
    public Task MT1_IT_030() => Run("MT1-IT-030", c => Service(c, 30));

    // MT1-IT-031: Cập nhật danh sách môn của nhu cầu thuộc đúng chủ.
    // Căn cứ: CapNhatAsync nhận ViewModel.MonHocIds, Id và RowVersion; SC_UC1.1b 2.4–2.19.
    // Kỳ vọng: Giữ Id/chủ, thay đúng tập môn, RowVersion đổi, giữ lịch theo dữ liệu gửi; không từ chối chỉ vì có nhiều môn.
    [Fact(DisplayName = "MT1-IT-031 · Cập nhật danh sách môn của nhu cầu")]
    public Task MT1_IT_031() => Run("MT1-IT-031", c => Service(c, 31));

    // MT1-IT-032: Sửa đúng chủ và lịch
    // Kỳ vọng: Id/chủ giữ nguyên; mục tiêu trim đúng; lịch cũ bị thay toàn bộ; rowversion mới.
    [Fact(DisplayName = "MT1-IT-032 · Sửa đúng chủ và lịch")]
    public Task MT1_IT_032() => Run("MT1-IT-032", c => Service(c, 32));

    // MT1-IT-033: Đọc form của người khác
    // Kỳ vọng: 404; không trả model hoặc dữ liệu riêng tư.
    [Fact(DisplayName = "MT1-IT-033 · Đọc form của người khác")]
    public Task MT1_IT_033() => Run("MT1-IT-033", c => Service(c, 33));

    // MT1-IT-034: Ghi nhu cầu của người khác
    // Kỳ vọng: 403; nhu cầu và lịch của HS-A không đổi.
    [Fact(DisplayName = "MT1-IT-034 · Ghi nhu cầu của người khác")]
    public Task MT1_IT_034() => Run("MT1-IT-034", c => Service(c, 34));

    // MT1-IT-035: Sửa bằng phiên bản cũ
    // Kỳ vọng: A thành công; B DbUpdateConcurrencyException; DB giữ mục tiêu A/lịch A; HTTP kiểm 409 ở ca WAF.
    [Fact(DisplayName = "MT1-IT-035 · Sửa bằng phiên bản cũ")]
    public Task MT1_IT_035() => Run("MT1-IT-035", c => Service(c, 35));

    // MT1-IT-036: Chỉ thay lịch cũng kiểm phiên bản
    // Kỳ vọng: B bị xung đột; không ghi đè lịch A; rowversion cha phải đổi dù chỉ sửa slot.
    [Fact(DisplayName = "MT1-IT-036 · Chỉ thay lịch cũng kiểm phiên bản")]
    public Task MT1_IT_036() => Run("MT1-IT-036", c => Service(c, 36));

    // MT1-IT-037: Rollback sau lần ghi nhu cầu
    // Kỳ vọng: Sau dispose transaction, context mới thấy 0 nhu cầu/slot theo mã ca; không nhận kết quả thành công.
    [Fact(DisplayName = "MT1-IT-037 · Rollback sau lần ghi nhu cầu")]
    public Task MT1_IT_037() => Run("MT1-IT-037", c => Service(c, 37));

    // MT1-IT-038: Rollback khi môn thứ hai sai
    // Kỳ vọng: LoiNghiepVu400 ở môn 112; cả nhu cầu/slot môn 101 đã ghi trong cùng transaction đều rollback.
    [Fact(DisplayName = "MT1-IT-038 · Rollback khi môn thứ hai sai")]
    public Task MT1_IT_038() => Run("MT1-IT-038", c => Service(c, 38));

    // MT1-IT-039: Đóng nhu cầu
    // Kỳ vọng: N01 thành DaDong, đổi rowversion; không còn trong tìm kiếm/chi tiết công khai.
    [Fact(DisplayName = "MT1-IT-039 · Đóng nhu cầu")]
    public Task MT1_IT_039() => Run("MT1-IT-039", c => Service(c, 39));

    // MT1-IT-040: Mở lại nhu cầu đã đóng
    // Kỳ vọng: N01 thành DangMo và xuất hiện lại nếu thỏa truy vấn.
    [Fact(DisplayName = "MT1-IT-040 · Mở lại nhu cầu đã đóng")]
    public Task MT1_IT_040() => Run("MT1-IT-040", c => Service(c, 40));

    // MT1-IT-041: Không mở bản nháp bằng đổi trạng thái
    // Kỳ vọng: LoiNghiepVu400: Hãy hoàn thiện bản nháp trước khi mở.; không đổi trạng thái.
    [Fact(DisplayName = "MT1-IT-041 · Không mở bản nháp bằng đổi trạng thái")]
    public Task MT1_IT_041() => Run("MT1-IT-041", c => Service(c, 41));

    // MT1-IT-042: Lịch chồng bị chặn ở Service
    // Kỳ vọng: LoiNghiepVu400 về chồng lấn; 0 dòng mới. Chỉ chứng minh từ chối trước ghi, không thay ca fault-injection rollback.
    [Fact(DisplayName = "MT1-IT-042 · Lịch chồng bị chặn ở Service")]
    public Task MT1_IT_042() => Run("MT1-IT-042", c => Service(c, 42));
    #endregion

    #region TC 043 — Tìm kiếm/chi tiết/so sánh

    // MT1-IT-043: Học sinh tìm hồ sơ gia sư
    // Kỳ vọng: Chỉ P1, P2, P3, P7 đủ điều kiện được hiển thị; không P4 ẩn/P5 chờ duyệt/P6 môn ngừng dùng; không trả nhu cầu học.
    [Fact(DisplayName = "MT1-IT-043 · Học sinh tìm hồ sơ gia sư")]
    public Task MT1_IT_043() => Run("MT1-IT-043", c => Service(c, 43));

    // MT1-IT-044: Gia sư tìm nhu cầu đang mở
    // Kỳ vọng: Total=12; Items=9 nhu cầu Toán mở; không N13 đóng/N14 nháp/N15 môn khác.
    [Fact(DisplayName = "MT1-IT-044 · Gia sư tìm nhu cầu đang mở")]
    public Task MT1_IT_044() => Run("MT1-IT-044", c => Service(c, 44));

    // MT1-IT-045: Phân trang trang2
    // Kỳ vọng: Total=12; 3 dòng còn lại, không trùng trang1; thứ tự theo TaoLuc rồi Id.
    [Fact(DisplayName = "MT1-IT-045 · Phân trang trang2")]
    public Task MT1_IT_045() => Run("MT1-IT-045", c => Service(c, 45));

    // MT1-IT-046: Lọc đúng môn và học phí cùng dòng
    // Kỳ vọng: Có P1; không P2 khác đơn vị; không P7 (Toán 300000 dù Văn 100000); không ghép phí của môn khác.
    [Fact(DisplayName = "MT1-IT-046 · Lọc đúng môn và học phí cùng dòng")]
    public Task MT1_IT_046() => Run("MT1-IT-046", c => Service(c, 46));

    // MT1-IT-047: Lọc nhiều môn
    // Kỳ vọng: Kết quả chứa hồ sơ có ít nhất 1 môn đã chọn; môn hiển thị thuộc tập chọn; không yêu cầu có đủ cả 2 nếu chưa được đặc tả khác.
    [Fact(DisplayName = "MT1-IT-047 · Lọc nhiều môn")]
    public Task MT1_IT_047() => Run("MT1-IT-047", c => Service(c, 47));

    // MT1-IT-048: Lọc hình thức và khu vực
    // Kỳ vọng: Chuẩn hóa khoảng trắng; trả hồ sơ trực tuyến ở Thủ Đức; không hồ sơ chỉ trực tiếp/khu vực khác.
    [Fact(DisplayName = "MT1-IT-048 · Lọc hình thức và khu vực")]
    public Task MT1_IT_048() => Run("MT1-IT-048", c => Service(c, 48));

    // MT1-IT-049: Từ khóa và dấu tiếng Việt
    // Kỳ vọng: Hai lượt khác hoa/thường đối chiếu quy tắc tìm từ khóa đã xác nhận. Không tự giả định không dấu tương đương có dấu; ghi riêng tập Id từng lượt.
    [Fact(DisplayName = "MT1-IT-049 · Từ khóa và dấu tiếng Việt", Skip = "Chưa chốt quy tắc tìm từ khóa/dấu/hoa thường.")]
    public void MT1_IT_049()
    {
    }

    // MT1-IT-050: Lọc lịch chứa trọn khoảng
    // Kỳ vọng: Có N01. Thử 17–20 hoặc 19–21: không N01; đầu/cuối chạm biên được chấp nhận.
    [Fact(DisplayName = "MT1-IT-050 · Lọc lịch chứa trọn khoảng")]
    public Task MT1_IT_050() => Run("MT1-IT-050", c => Service(c, 50));

    // MT1-IT-051: Sắp xếp phí cùng đơn vị
    // Kỳ vọng: P1(150000) trước P7(300000); không đem phí/buổi của P2 so với phí/giờ.
    [Fact(DisplayName = "MT1-IT-051 · Sắp xếp phí cùng đơn vị")]
    public Task MT1_IT_051() => Run("MT1-IT-051", c => Service(c, 51));

    // MT1-IT-052: Trang không hợp lệ
    // Kỳ vọng: LoiNghiepVu400: Trang không hợp lệ.; không trả trang ngầm.
    [Fact(DisplayName = "MT1-IT-052 · Trang không hợp lệ")]
    public Task MT1_IT_052() => Run("MT1-IT-052", c => Service(c, 52));

    // MT1-IT-053: Không tìm thấy và trang vượt kết quả
    // Kỳ vọng: Lượt 1 Total=0, Items=[]; lượt 2 Total=12, Items=[]; không 500.
    [Fact(DisplayName = "MT1-IT-053 · Không tìm thấy và trang vượt kết quả")]
    public Task MT1_IT_053() => Run("MT1-IT-053", c => Service(c, 53));

    // MT1-IT-054: Chi tiết công khai không lộ thông tin trẻ
    // Kỳ vọng: DTO công khai không có tên bé/tuổi/địa chỉ chi tiết; P1 có môn, phạm vi, phí và đơn vị rõ ràng.
    [Fact(DisplayName = "MT1-IT-054 · Chi tiết công khai không lộ thông tin trẻ")]
    public Task MT1_IT_054() => Run("MT1-IT-054", c => Service(c, 54));

    // MT1-IT-055: Chi tiết đã ẩn hoặc nhu cầu đóng
    // Kỳ vọng: 404, thông báo công khai hiện hành; không trả dữ liệu cũ qua Id.
    [Fact(DisplayName = "MT1-IT-055 · Chi tiết đã ẩn hoặc nhu cầu đóng")]
    public Task MT1_IT_055() => Run("MT1-IT-055", c => Service(c, 55));

    // MT1-IT-056: So sánh 3 Id phân biệt
    // Kỳ vọng: Đúng 3 đối tượng và cùng nhóm tiêu chí; phí khác đơn vị giữ nhãn riêng, không quy đổi ngầm.
    [Fact(DisplayName = "MT1-IT-056 · So sánh 3 Id phân biệt")]
    public Task MT1_IT_056() => Run("MT1-IT-056", c => Service(c, 56));

    // MT1-IT-057: So sánh Id lặp
    // Kỳ vọng: Chỉ 2 đối tượng phân biệt; không báo vượt giới hạn 3.
    [Fact(DisplayName = "MT1-IT-057 · So sánh Id lặp")]
    public Task MT1_IT_057() => Run("MT1-IT-057", c => Service(c, 57));

    // MT1-IT-058: So sánh chứa đối tượng vừa đóng
    // Kỳ vọng: LoiNghiepVu404: Hồ sơ hoặc nhu cầu đang ẩn, chưa được duyệt hoặc không còn công khai.; không trả bảng so sánh một phần.
    [Fact(DisplayName = "MT1-IT-058 · So sánh chứa đối tượng vừa đóng")]
    public Task MT1_IT_058() => Run("MT1-IT-058", c => Service(c, 58));

    // MT1-IT-059: Khoảng học phí từ–đến theo thiết kế
    // Kỳ vọng: Nếu hợp đồng lọc khoảng được xác nhận: chỉ ứng viên có phí từ 100000 đến 200000 và cùng đơn vị VND/Gio. Chưa đánh giá hai cận khi hợp đồng đầu vào chưa thống nhất.
    [Fact(DisplayName = "MT1-IT-059 · Khoảng học phí từ–đến theo thiết kế", Skip = "Chưa thống nhất bộ lọc HocPhiTu/HocPhiDen với HocPhiToiDa.")]
    public void MT1_IT_059()
    {
    }

    // MT1-IT-060: Gợi ý và lý do trên dữ liệu thực
    // Kỳ vọng: Lý do hiển thị khớp dữ liệu và quy tắc được mô tả ở P02. Không tính hoặc hiển thị điểm phần trăm. Không phát sinh yêu cầu liên hệ, yêu cầu kết nối hoặc lớp chỉ từ thao tác xem gợi ý.
    [Fact(DisplayName = "MT1-IT-060 · Gợi ý và lý do trên dữ liệu thực")]
    public Task MT1_IT_060() => Run("MT1-IT-060", c => Service(c, 60));
    #endregion

    #region TC 061 — HTTP

    // MT1-IT-061: GET tìm kiếm HTML
    // Kỳ vọng: 200; text/html; vùng thẻ kết quả có P1, không chỉ kiểm tên trong toàn HTML.
    [Fact(DisplayName = "MT1-IT-061 · GET tìm kiếm HTML")]
    public Task MT1_IT_061() => Run("MT1-IT-061", c => Http(c, 61));

    // MT1-IT-062: Model binding lọc môn
    // Kỳ vọng: 200; Total=12; 9 thẻ nhu cầu Toán; danh mục môn có Văn vẫn hợp lệ.
    [Fact(DisplayName = "MT1-IT-062 · Model binding lọc môn")]
    public Task MT1_IT_062() => Run("MT1-IT-062", c => Http(c, 62));

    // MT1-IT-063: Trang0 qua middleware
    // Kỳ vọng: 400; HTML có Trang không hợp lệ.; không 500.
    [Fact(DisplayName = "MT1-IT-063 · Trang0 qua middleware")]
    public Task MT1_IT_063() => Run("MT1-IT-063", c => Http(c, 63));

    // MT1-IT-064: POST nhu cầu thường hợp lệ
    // Kỳ vọng: 302 tới Index; DB đúng 1 nhu cầu mới và 2 slot theo marker riêng.
    [Fact(DisplayName = "MT1-IT-064 · POST nhu cầu thường hợp lệ")]
    public Task MT1_IT_064() => Run("MT1-IT-064", c => Http(c, 64));

    // MT1-IT-065: POST thiếu token
    // Kỳ vọng: 400; 0 dòng nhu cầu/slot mới; action không thực thi.
    [Fact(DisplayName = "MT1-IT-065 · POST thiếu token")]
    public Task MT1_IT_065() => Run("MT1-IT-065", c => Http(c, 65));

    // MT1-IT-066: POST AJAX thành công
    // Kỳ vọng: 200 JSON ok=true, message, redirectUrl; DB thêm đúng 1 nhu cầu và 2 slot; không nhầm 302.
    [Fact(DisplayName = "MT1-IT-066 · POST AJAX thành công")]
    public Task MT1_IT_066() => Run("MT1-IT-066", c => Http(c, 66));

    // MT1-IT-067: POST AJAX validation lỗi
    // Kỳ vọng: 422 JSON ok=false; errors có SoBuoiTuan; không ghi DB.
    [Fact(DisplayName = "MT1-IT-067 · POST AJAX validation lỗi")]
    public Task MT1_IT_067() => Run("MT1-IT-067", c => Http(c, 67));

    // MT1-IT-068: POST thường validation lỗi
    // Kỳ vọng: 200 HTML form Sua với lỗi và dữ liệu đã nhập; không ghi DB; không kỳ vọng 422 của AJAX.
    [Fact(DisplayName = "MT1-IT-068 · POST thường validation lỗi")]
    public Task MT1_IT_068() => Run("MT1-IT-068", c => Http(c, 68));

    // MT1-IT-069: Bind đủ tên trường lịch
    // Kỳ vọng: DB lưu đúng ngày 1, 19–20; không dùng giờ mặc định 20:30.
    [Fact(DisplayName = "MT1-IT-069 · Bind đủ tên trường lịch")]
    public Task MT1_IT_069() => Run("MT1-IT-069", c => Http(c, 69));

    // MT1-IT-070: Base64 phiên bản sai
    // Kỳ vọng: Báo lỗi phiên bản sai định dạng; không ghi nhu cầu hoặc lịch. Phân biệt với xung đột phiên bản hợp lệ nhưng đã cũ; chỉ kiểm mã HTTP cụ thể khi đặc tả quy định.
    [Fact(DisplayName = "MT1-IT-070 · Base64 phiên bản sai")]
    public Task MT1_IT_070() => Run("MT1-IT-070", c => Http(c, 70));

    // MT1-IT-071: Xung đột phiên bản thật qua HTTP
    // Kỳ vọng: B HTTP409 qua filter/middleware; DB giữ dữ liệu A; không hiện lưu thành công.
    [Fact(DisplayName = "MT1-IT-071 · Xung đột phiên bản thật qua HTTP")]
    public Task MT1_IT_071() => Run("MT1-IT-071", c => Http(c, 71));

    // MT1-IT-072: AJAX tìm kiếm partial
    // Kỳ vọng: 200 HTML partial vùng kết quả; không lặp toàn bộ layout/head; đúng bộ lọc.
    [Fact(DisplayName = "MT1-IT-072 · AJAX tìm kiếm partial")]
    public Task MT1_IT_072() => Run("MT1-IT-072", c => Http(c, 72));

    // MT1-IT-073: AJAX so sánh modal
    // Kỳ vọng: 200 partial HTML chứa 2 ứng viên; không chèn layout lồng nhau.
    [Fact(DisplayName = "MT1-IT-073 · AJAX so sánh modal")]
    public Task MT1_IT_073() => Run("MT1-IT-073", c => Http(c, 73));

    // MT1-IT-074: AJAX so sánh 4 đối tượng
    // Kỳ vọng: 400 JSON ok=false với lỗi giới hạn; không partial thành công.
    [Fact(DisplayName = "MT1-IT-074 · AJAX so sánh 4 đối tượng")]
    public Task MT1_IT_074() => Run("MT1-IT-074", c => Http(c, 74));

    // MT1-IT-075: Phiên chưa xác thực
    // Kỳ vọng: Yêu cầu đăng nhập theo NFR-12; không tự gán một người thực hiện. Không tạo nhu cầu/lịch. Phải tách lỗi xác thực khỏi lỗi thiếu token chống giả mạo.
    [Fact(DisplayName = "MT1-IT-075 · Phiên chưa xác thực")]
    public Task MT1_IT_075() => Run("MT1-IT-075", c => Http(c, 75));
    #endregion

    #region TC 076 — Liên hệ và nhắn tin

    // MT1-IT-076: Gửi lời mời liên hệ hợp lệ
    // Kỳ vọng: Tạo 1 YeuCauLienHe ChoPhanHoi, lời nhắn đã trim; 1 sự kiện+1 thông báo choGS-A, cùng transaction; không tự tạo Lop Hoc.
    [Fact(DisplayName = "MT1-IT-076 · Gửi lời mời liên hệ hợp lệ")]
    public Task MT1_IT_076() => Run("MT1-IT-076", c => Service(c, 76));

    // MT1-IT-077: Liên hệ chính mình
    // Kỳ vọng: 400; 0 liên hệ/sự kiện/thông báo mới.
    [Fact(DisplayName = "MT1-IT-077 · Liên hệ chính mình")]
    public Task MT1_IT_077() => Run("MT1-IT-077", c => Service(c, 77));

    // MT1-IT-078: Hai bên đều là học sinh
    // Kỳ vọng: LoiNghiepVu400: Liên hệ cần một gia sư và một bên học.; không tạo liên hệ/sự kiện/thông báo.
    [Fact(DisplayName = "MT1-IT-078 · Hai bên đều là học sinh")]
    public Task MT1_IT_078() => Run("MT1-IT-078", c => Service(c, 78));

    // MT1-IT-079: Hồ sơ gia sư ẩn
    // Kỳ vọng: LoiNghiepVu409: Hồ sơ gia sư đã ẩn hoặc chưa đủ điều kiện nhận liên hệ mới.; không tạo liên hệ/thông báo.
    [Fact(DisplayName = "MT1-IT-079 · Hồ sơ gia sư ẩn")]
    public Task MT1_IT_079() => Run("MT1-IT-079", c => Service(c, 79));

    // MT1-IT-080: Gửi liên hệ trùng
    // Kỳ vọng: Chỉ 1 quan hệ liên hệ hiện hành; request còn lại báo trùng; không 2 thông báo cho cùng sự kiện.
    [Fact(DisplayName = "MT1-IT-080 · Gửi liên hệ trùng")]
    public Task MT1_IT_080() => Run("MT1-IT-080", c => Service(c, 80));

    // MT1-IT-081: Rollback thông báo khi lưu lỗi
    // Kỳ vọng: Không có YeuCauLienHe, SuKienChoXuLy, ThongBao mồ côi saurollback.
    [Fact(DisplayName = "MT1-IT-081 · Rollback thông báo khi lưu lỗi")]
    public Task MT1_IT_081() => Run("MT1-IT-081", c => Service(c, 81));

    // MT1-IT-082: Chặn hai thao tác khi người nhận bị khóa
    // Kỳ vọng: Cả hai thao tác bị từ chối vì tài khoản bị khóa; thông báo lý do và gợi ý chọn người khác; không có tin nhắn hoặc yêu cầu kết nối mới.
    [Fact(DisplayName = "MT1-IT-082 · Chặn hai thao tác khi người nhận bị khóa")]
    public Task MT1_IT_082() => Run("MT1-IT-082", c => Service(c, 82));

    // MT1-IT-083: Nhắn tin trước kết nối sau khi chấp nhận liên hệ
    // Kỳ vọng: Lời mời tạo YeuCauLienHe ChoPhanHoi và thông báo. Chưa chấp nhận thì bị từ chối nhắn tin và gửi yêu cầu kết nối. Sau ChapNhan nhắn được hai chiều dù chưa có lớp. Việc nhắn tin không tự tạo lớp/yêu cầu kết nối.
    [Fact(DisplayName = "MT1-IT-083 · Nhắn tin trước kết nối sau khi chấp nhận liên hệ")]
    public Task MT1_IT_083() => Run("MT1-IT-083", c => Service(c, 83));

    // MT1-IT-084: Gửi lại tin sau lỗi không tạo trùng
    // Kỳ vọng: Nội dung được giữ sau lỗi. Khi gửi lại thành công, mỗi thao tác chỉ tạo một tin nhắn; trường hợp đã ghi không có bản ghi trùng.
    [Fact(DisplayName = "MT1-IT-084 · Gửi lại tin sau lỗi không tạo trùng")]
    public Task MT1_IT_084() => Run("MT1-IT-084", c => Service(c, 84));
    #endregion

    #region TC 085 — HTTP bổ sung

    // MT1-IT-085: AJAX liên hệ ModelState sai theo sơ đồ
    // Kỳ vọng: HTTP 422; JSON lỗi trường LoiNhan; dừng trước xử lý tạo liên hệ; không thêm yêu cầu, sự kiện hoặc thông báo. Theo sequence UC1.4 bước4.
    [Fact(DisplayName = "MT1-IT-085 · AJAX liên hệ ModelState sai theo sơ đồ")]
    public Task MT1_IT_085() => Run("MT1-IT-085", c => Http(c, 85));

    // MT1-IT-086: Bind sai định dạng tham số tìm kiếm
    // Kỳ vọng: 400 JSON lỗi định dạng; không truy vấn TimAsync và không trả500; phân biệt lỗi binding với không tìm thấy môn.
    [Fact(DisplayName = "MT1-IT-086 · Bind sai định dạng tham số tìm kiếm")]
    public Task MT1_IT_086() => Run("MT1-IT-086", c => Http(c, 86));
    #endregion

    private async Task Run(string id, Func<Mt1Case, Task> test)
    {
        var watch = Stopwatch.StartNew();
        Mt1Case? c = null;
        Exception? failure = null;
        string snapshot = "";
        try
        {
            c = await Mt1Case.Create(id);
            await test(c);
        }
        catch (Exception ex) { failure = ex; }
        finally
        {
            if (c != null)
            {
                try
                {
                    snapshot = await c.Snapshot();
                }
                catch (Exception ex) { failure = failure == null ? ex : new AggregateException(failure, ex); }
                try
                {
                    if (Environment.GetEnvironmentVariable("MT1_CAPTURE_FIXTURE_CASE") == id)
                        c.Proof["cleanup"] = "Tạm giữ fixture để chụp minh chứng SQL; dọn ngay sau chụp.";
                    else
                        await c.DisposeAsync();
                }
                catch (Exception ex) { failure = failure == null ? ex : new AggregateException(failure, ex); }
            }
            var row = new
            {
                runId = Environment.GetEnvironmentVariable("MT1_RUN_ID"),
                caseId = id,
                status = failure == null ? "Pass" : "Fail",
                actual = failure?.ToString() ?? "Các assertion đạt; " + snapshot + " Đã dọn fixture.",
                proof = c?.Proof,
                elapsedMs = watch.ElapsedMilliseconds,
                utc = DateTimeOffset.UtcNow
            };
            Directory.CreateDirectory(Mt1Case.Evidence);
            await File.AppendAllTextAsync(Path.Combine(Mt1Case.Evidence, "integration-details.jsonl"), JsonSerializer.Serialize(row) + Environment.NewLine);
            output.WriteLine($"{id}: {row.status}. Chi tiết: integration-details.jsonl; RunId={row.runId}");
        }
        if (failure != null)
            ExceptionDispatchInfo.Capture(failure).Throw();
    }

    private static async Task ContactCases(Mt1Case c, int n)
    {
        await using var s = c.Scope();
        if (n is 82 or 83 or 84)
        {
            Guid room;
            if (n == 83)
            {
                await s.Contacts.GuiLienHeAsync(new()
                {
                    DoiTuongId = c.GS,
                    LoiNhan = "Muốn học Toán"
                });
                var pending = await c.Contact();
                Assert.Null(await s.Contacts.HoiThoaiAsync(pending.Id));
                await Assert.ThrowsAsync<LoiNghiepVu>(() => s.Contacts.GuiYeuCauAsync(new GuiYeuCauInput { LienHeId = pending.Id, LoiNhan = "Chưa chấp nhận" }));
                await using var tutor = c.Scope(c.GS);
                await tutor.Contacts.PhanHoiLienHeAsync(pending.Id, "ChapNhan", Convert.ToBase64String(pending.RowVersion));
                room = (await tutor.Contacts.HoiThoaiAsync(pending.Id))!.Value;
            }
            else
                room = await c.AcceptContact();
            if (n == 82)
            {
                await using (var db = c.Db())
                    await db.Users.Where(x => x.Id == c.GS).ExecuteUpdateAsync(a => a.SetProperty(x => x.TrangThai, "BiKhoa"));
                var message = await Record.ExceptionAsync(() => s.Chat.GuiAsync(new() { MaHoiThoai = room, MaThaoTac = Guid.NewGuid(), NoiDung = "Không gửi tới người khóa" }));
                var contact = await c.Contact();
                var request = await Record.ExceptionAsync(() => s.Contacts.GuiYeuCauAsync(new GuiYeuCauInput { LienHeId = contact.Id, LoiNhan = "Xin kết nối" }));
                await using var db2 = c.Db();
                var count = await db2.Set<TinNhan>().CountAsync(x => x.CuocHoiThoaiId == room);
                var requests = await db2.Set<YeuCauKetNoiLopHoc>().CountAsync(x => x.LienHeId == contact.Id);
                c.Proof["blockedRecipient"] = new
                {
                    messageRejected = message != null,
                    requestRejected = request != null,
                    count,
                    requests
                };
                Assert.NotNull(message);
                Assert.NotNull(request);
                Assert.Equal(0, count);
                Assert.Equal(0, requests);
            }
            else
            {
                var key = Guid.NewGuid();
                await s.Chat.GuiAsync(new()
                {
                    MaHoiThoai = room,
                    MaThaoTac = key,
                    NoiDung = "Em muốn hỏi lịch học"
                });
                if (n == 84)
                {
                    await using var retry = c.Scope();
                    await retry.Chat.GuiAsync(new()
                    {
                        MaHoiThoai = room,
                        MaThaoTac = key,
                        NoiDung = "Em muốn hỏi lịch học"
                    });
                    c.Proof["retry"] = "Mất phản hồi sau commit: gửi lại cùng mã thao tác bằng scope mới.";
                }
                else
                {
                    await using var tutor = c.Scope(c.GS);
                    await tutor.Chat.GuiAsync(new()
                    {
                        MaHoiThoai = room,
                        MaThaoTac = Guid.NewGuid(),
                        NoiDung = "Gia sư trả lời"
                    });
                    Assert.True(await tutor.Chat.CoTheGuiAsync(room));
                }
                await using var db = c.Db();
                Assert.Equal(n == 84 ? 1 : 2, await db.Set<TinNhan>().CountAsync(x => x.CuocHoiThoaiId == room));
                Assert.False(await db.Set<LopHoc>().AnyAsync(x => c.Users.Contains(x.BenHocId)));
                Assert.False(await db.Set<YeuCauKetNoiLopHoc>().AnyAsync(x => c.Users.Contains(x.BenHocId)));
                c.Proof["chat"] = new
                {
                    room,
                    key
                };
            }
            return;
        }
        if (n == 79)
        {
            await using var db = c.Db();
            await db.Set<HoSoGiaSu>().Where(x => x.Id == c.P1).ExecuteUpdateAsync(a => a.SetProperty(x => x.DangHienThi, false));
        }
        if (n == 81)
        {
            var trigger = "MT1_IT081_" + Guid.NewGuid().ToString("N");
            await using var setup = c.Db();
            var sql = $"CREATE TRIGGER dbo.[{trigger}] ON dbo.ThongBao AFTER INSERT AS BEGIN SET NOCOUNT ON; IF EXISTS(SELECT 1 FROM inserted WHERE NguoiNhanId='{c.GS}') THROW 51998,'MT1 IT081 injected SQL notice failure',1; END";
            await setup.Database.ExecuteSqlRawAsync(sql);
            try
            {
                await using var failing = c.Scope();
                var error = await Assert.ThrowsAsync<DbUpdateException>(() => failing.Contacts.GuiLienHeAsync(new() { DoiTuongId = c.GS, LoiNhan = "Muốn học Toán" }));
                Assert.Equal(51998, Assert.IsType<Microsoft.Data.SqlClient.SqlException>(error.InnerException).Number);
            }
            finally
            {
                var dropSql = "DROP TRIGGER dbo.[" + trigger + "]";
                await setup.Database.ExecuteSqlRawAsync(dropSql);
            }
            c.Proof["fault"] = "SQL AFTER INSERT trigger từ chối ghi ThongBao; trigger đã xóa trong finally.";
            await using var db = c.Db();
            Assert.False(await db.Set<YeuCauLienHe>().AnyAsync(x => x.BenHocId == c.HS));
            Assert.False(await db.Set<ThongBao>().AnyAsync(x => x.NguoiNhanId == c.GS));
            Assert.False(await db.Set<SuKienChoXuLy>().AnyAsync(x => x.DuLieuToiThieu.Contains(c.GS.ToString())));
            return;
        }
        if (n == 80)
        {
            var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            async Task<Exception?> Send()
            {
                await using var scope = c.Scope();
                await start.Task;
                return await Record.ExceptionAsync(() => scope.Contacts.GuiLienHeAsync(new() { DoiTuongId = c.GS, LoiNhan = "Muốn học Toán" }));
            }
            var a = Send();
            var b = Send();
            start.SetResult();
            var errors = await Task.WhenAll(a, b);
            c.Proof["concurrentErrors"] = errors.Select(x => x?.Message);
            Assert.Single(errors, x => x == null);
            Assert.Single(errors, x => x is LoiNghiepVu);
        }
        else if (n is 77 or 78 or 79)
        {
            var ex = await Assert.ThrowsAsync<LoiNghiepVu>(() => s.Contacts.GuiLienHeAsync(new() { DoiTuongId = n == 77 ? c.HS : n == 78 ? c.HS2 : c.GS, LoiNhan = "Muốn học Toán" }));
            Assert.Equal(n == 79 ? 409 : 400, ex.Status);
            await using var db = c.Db();
            Assert.False(await db.Set<YeuCauLienHe>().AnyAsync(x => x.BenHocId == c.HS));
            Assert.False(await db.Set<ThongBao>().AnyAsync(x => c.Users.Contains(x.NguoiNhanId)));
            return;
        }
        else
            await s.Contacts.GuiLienHeAsync(new()
            {
                DoiTuongId = c.GS,
                LoiNhan = "  Muốn học Toán lớp 8  "
            });
        var saved = await c.Contact();
        Assert.Equal("ChoPhanHoi", saved.TrangThai);
        Assert.Equal(n == 80 ? "Muốn học Toán" : "Muốn học Toán lớp 8", saved.LoiNhan);
        await using var verify = c.Db();
        Assert.Equal(1, await verify.Set<YeuCauLienHe>().CountAsync(x => x.BenHocId == c.HS && x.GiaSuId == c.GS));
        var notice = await verify.Set<ThongBao>().SingleAsync(x => x.DoiTuongId == saved.Id && x.NguoiNhanId == c.GS);
        Assert.True(await verify.Set<SuKienChoXuLy>().AnyAsync(x => x.Id == notice.MaSuKien));
        Assert.False(await verify.Set<LopHoc>().AnyAsync(x => x.BenHocId == c.HS));
        c.Proof["contactId"] = saved.Id;
    }
    // Kiểm hợp đồng đọc lại, không lấy cách chia bảng vật lý làm oracle.
    private static async Task AssertSelectedSubjects(Mt1Case c, Guid id,
        IReadOnlyCollection<int> expectedSubjects, TaoNhuCauBenHocViewModel sent)
    {
        Assert.NotEqual(Guid.Empty, id);
        await using var read = c.Scope();
        var result = await read.Needs.LayDeSuaAsync(id);
        Assert.True(result.ThanhCong, result.ThongBao);
        var actual = result.DuLieu;
        var expected = expectedSubjects.Distinct().Order().ToArray();
        // Lưu cả hai danh sách trước assertion để ca lỗi vẫn có minh chứng đủ môn thừa/thiếu.
        c.Proof["subjectRoundTrip"] = new
        {
            id,
            submitted = sent.MonHocIds.ToArray(),
            expected,
            actual = actual.MonHocIds.ToArray()
        };
        Assert.Equal(id, actual.Id);
        Assert.Equal(expected.Length, actual.MonHocIds.Count);
        Assert.Equal(expected, actual.MonHocIds.Order());
        var entity = await c.ReadNeed(id);
        Assert.Equal(c.HS, entity.ChuSoHuuId);
        Assert.Equal("DangMo", entity.TrangThai);
        Assert.Equal(sent.MucTieu?.Trim(), entity.MucTieu);
        var slots = await c.ReadSlots(id);
        Assert.Equal(sent.LichRanh.Count, slots.Count);
        Assert.Equal(sent.LichRanh.Select(x => (x.ThuTrongTuan, x.GioBatDau, x.GioKetThuc)).Order(),
            slots.Select(x => (x.ThuTrongTuan, x.GioBatDau, x.GioKetThuc)).Order());
        c.Proof["subjectRoundTrip"] = new
        {
            id,
            submitted = sent.MonHocIds.ToArray(),
            expected,
            actual = actual.MonHocIds.ToArray(),
            entity.ChuSoHuuId,
            entity.TrangThai,
            slots = slots.Select(x => new { x.ThuTrongTuan, x.GioBatDau, x.GioKetThuc })
        };
    }

    private static async Task Service(Mt1Case c, int n)
    {
        var m = c.Input();
        await using var s = c.Scope();
        async Task Reject(TaoNhuCauBenHocViewModel input, int status = 400, Guid? actor = null)
        {
            var before = await c.NeedCount();
            await using var scope = c.Scope(actor);
            var r = await scope.Needs.TaoNhuCauHocAsync(input, actor ?? c.HS, default);
            c.Proof["businessResult"] = new
            {
                r.ThanhCong,
                r.MaTrangThai,
                r.ThongBao
            };
            Assert.False(r.ThanhCong);
            Assert.Equal(status, r.MaTrangThai);
            Assert.Equal(before, await c.NeedCount());
        }
        async Task<Page<UngVienViewModel>> Search(BoLocTimKiemViewModel? filter = null, Guid? actor = null)
        {
            await using var scope = c.Scope(actor);
            filter ??= new();
            filter.TuKhoa ??= c.Tag;
            var r = await scope.Search.TimKiemAsync(filter, actor ?? c.HS, default);
            Assert.True(r.ThanhCong, r.ThongBao);
            var p = r.DuLieu;
            c.Proof["search"] = new
            {
                p.Total,
                p.Trang,
                ids = p.Items.Select(x => x.Id),
                filter
            };
            return p;
        }
        switch (n)
        {
            case 14:
                {
                    var id = await c.CreateNeed();
                    var r = await c.ReadNeed(id);
                    Assert.Equal(c.HS, r.ChuSoHuuId);
                    Assert.Equal("TimGiaSu", r.LoaiNhuCau);
                    Assert.Equal("DangMo", r.TrangThai);
                    Assert.Null(r.TenNguoiHoc);
                    Assert.Null(r.TuoiNguoiHoc);
                    Assert.Equal(2, (await c.ReadSlots(id)).Count);
                    break;
                }
            case 15:
                {
                    m.TenNguoiHoc = "Bé An";
                    m.TuoiNguoiHoc = 13;
                    var r = await c.ReadNeed(await c.CreateNeed(m, c.PH));
                    Assert.Equal(c.PH, r.ChuSoHuuId);
                    Assert.Equal("Bé An", r.TenNguoiHoc);
                    Assert.Equal(13, r.TuoiNguoiHoc);
                    await using var db = c.Db();
                    Assert.Equal(c.Users.Count, await db.Users.CountAsync(x => c.Users.Contains(x.Id)));
                    break;
                }
            case 16:
                await Reject(m, 403, c.GS);
                break;
            case 17:
                {
                    m.HinhThucHoc = "TrucTiep";
                    m.KhuVuc = "Thủ Đức";
                    m.DiaChiChiTiet = "Địa điểm đã nhập";
                    var r = await c.ReadNeed(await c.CreateNeed(m));
                    Assert.Equal(m.KhuVuc, r.KhuVuc);
                    Assert.Equal(m.DiaChiChiTiet, r.DiaChiChiTiet);
                    Assert.Equal("DangMo", r.TrangThai);
                    break;
                }
            case 18:
                m.HinhThucHoc = "TrucTiep";
                m.KhuVuc = null;
                await Reject(m);
                break;
            case 19:
                {
                    m.KhuVuc = "Q1";
                    m.DiaChiChiTiet = "PRIVATE_ADDRESS_019";
                    var view = await s.Search.ChiTietAsync(await c.CreateNeed(m));
                    Assert.DoesNotContain(m.DiaChiChiTiet, JsonSerializer.Serialize(view));
                    c.Proof["scopeNote"] = "Chỉ xác nhận không công khai địa chỉ; quy tắc giữ/xóa địa chỉ lưu chưa đặc tả.";
                    break;
                }
            case 20:
                m.MucTieu = " ";
                await Reject(m);
                break;
            case 21:
                m.SoBuoiTuan = null;
                await Reject(m);
                break;
            case 22:
                {
                    m.BanNhap = true;
                    m.MucTieu = null;
                    m.LichRanh = [];
                    var id = await c.CreateNeed(m);
                    Assert.Equal("BanNhap", (await c.ReadNeed(id)).TrangThai);
                    Assert.Empty(await c.ReadSlots(id));
                    Assert.DoesNotContain((await Search(actor: c.GS)).Items, x => x.Id == id);
                    break;
                }
            case 23:
                m.BanNhap = true;
                m.HocPhiDuKien = -1;
                await Reject(m);
                break;
            case 24:
                m.TenNguoiHoc = null;
                m.TuoiNguoiHoc = 13;
                await Reject(m, 400, c.PH);
                break;
            case 25:
                {
                    m.TenNguoiHoc = "Bé An";
                    m.TuoiNguoiHoc = null;
                    var r = await c.ReadNeed(await c.CreateNeed(m, c.PH));
                    Assert.Equal("Bé An", r.TenNguoiHoc);
                    Assert.Equal("Lớp 8", r.KhoiLop);
                    Assert.Null(r.TuoiNguoiHoc);
                    break;
                }
            case 26:
            case 27:
                {
                    m.MonHocIds = n == 26 ? [c.Math, c.Literature] : [c.Math, c.Math, c.Literature];
                    var id = await c.CreateNeed(m);
                    await AssertSelectedSubjects(c, id, [c.Math, c.Literature], m);
                    break;
                }
            case 28:
                m.MonHocIds = [];
                m.MonHocId = 0;
                await Reject(m);
                break;
            case 29:
            case 30:
                {
                    m.MonHocIds = [];
                    for (var i = 0; i < (n == 29 ? 10 : 11); i++)
                        m.MonHocIds.Add(await c.Subject());
                    if (n == 30)
                        await Reject(m);
                    else
                    {
                        var id = await c.CreateNeed(m);
                        await AssertSelectedSubjects(c, id, m.MonHocIds, m);
                        c.Proof["designScope"] = "10 là kích thước dữ liệu thử. Tài liệu đã đối chiếu không quy định tối đa 10 môn.";
                    }
                    break;
                }
            case 31:
            case 32:
            case 33:
            case 34:
            case 35:
            case 36:
            case 39:
            case 40:
            case 41:
                {
                    if (n == 41)
                        m.BanNhap = true;
                    var id = await c.CreateNeed(m);
                    var row = await c.ReadNeed(id);
                    var input = (await s.Needs.LayDeSuaAsync(id)).DuLieu;
                    var edit = TaoNhuCauBenHocViewModel.FromInput(input);
                    var v1 = row.RowVersion.ToArray();
                    if (n == 31)
                    {
                        var addedSubject = await c.Subject();
                        edit.MonHocIds = [c.Literature, addedSubject];
                        var countBefore = await c.NeedCount();
                        var r = await s.Needs.CapNhatAsync(id, edit, v1, c.HS, default);
                        Assert.True(r.ThanhCong, r.ThongBao);
                        Assert.Equal(id, r.DuLieu);
                        await AssertSelectedSubjects(c, id, edit.MonHocIds, edit);
                        var updatedVersion = (await c.ReadNeed(id)).RowVersion;
                        Assert.False(v1.SequenceEqual(updatedVersion));
                        Assert.Equal(countBefore, await c.NeedCount());
                    }
                    if (n == 32)
                    {
                        edit.MucTieu = "  Mục tiêu đã sửa  ";
                        edit.LichRanh = [new() { ThuTrongTuan = 5, GioBatDau = new(18, 0), GioKetThuc = new(19, 30) }];
                        var r = await s.Needs.CapNhatAsync(id, edit, v1, c.HS, default);
                        Assert.True(r.ThanhCong, r.ThongBao);
                        var updated = await c.ReadNeed(id);
                        Assert.Equal(c.HS, updated.ChuSoHuuId);
                        Assert.Equal("Mục tiêu đã sửa", updated.MucTieu);
                        Assert.False(v1.SequenceEqual(updated.RowVersion));
                        Assert.Equal(5, (await c.ReadSlots(id)).Single().ThuTrongTuan);
                    }
                    if (n == 33)
                    {
                        await using var other = c.Scope(c.HS2);
                        var r = await other.Needs.LayDeSuaAsync(id);
                        Assert.False(r.ThanhCong);
                        Assert.Equal(404, r.MaTrangThai);
                    }
                    if (n == 34)
                    {
                        await using var other = c.Scope(c.HS2);
                        var r = await other.Needs.CapNhatAsync(id, edit, v1, c.HS2, default);
                        Assert.False(r.ThanhCong);
                        Assert.Equal(403, r.MaTrangThai);
                        Assert.Equal(v1, (await c.ReadNeed(id)).RowVersion);
                    }
                    if (n is 35 or 36)
                    {
                        var a = TaoNhuCauBenHocViewModel.FromInput(input);
                        a.MucTieu = "Nội dung A";
                        a.LichRanh = [new() { ThuTrongTuan = 2, GioBatDau = new(18, 0), GioKetThuc = new(20, 0) }];
                        await using (var first = c.Scope())
                            Assert.True((await first.Needs.CapNhatAsync(id, a, v1, c.HS, default)).ThanhCong);
                        var afterA = await c.ReadNeed(id);
                        edit.MucTieu = n == 35 ? "Nội dung B" : input.MucTieu;
                        edit.LichRanh = [new() { ThuTrongTuan = 6, GioBatDau = new(18, 0), GioKetThuc = new(20, 0) }];
                        await using var second = c.Scope();
                        var r = await second.Needs.CapNhatAsync(id, edit, v1, c.HS, default);
                        c.Proof["versions"] = new
                        {
                            v1 = Convert.ToBase64String(v1),
                            v2 = Convert.ToBase64String(afterA.RowVersion),
                            r.MaTrangThai
                        };
                        Assert.False(r.ThanhCong);
                        Assert.Equal(409, r.MaTrangThai);
                        Assert.Equal(afterA.MucTieu, (await c.ReadNeed(id)).MucTieu);
                        Assert.Equal(2, (await c.ReadSlots(id)).Single().ThuTrongTuan);
                        if (n == 35)
                            Assert.IsType<DbUpdateConcurrencyException>(Record.Exception(() => { _ = r.DuLieu; }));
                    }
                    if (n is 39 or 40)
                    {
                        await s.Needs.TrangThaiAsync(id, "DaDong", Convert.ToBase64String(v1));
                        var closed = await c.ReadNeed(id);
                        Assert.Equal("DaDong", closed.TrangThai);
                        Assert.False(v1.SequenceEqual(closed.RowVersion));
                        Assert.DoesNotContain((await Search(actor: c.GS)).Items, x => x.Id == id);
                        if (n == 40)
                        {
                            await using var fresh = c.Scope();
                            await fresh.Needs.TrangThaiAsync(id, "DangMo", Convert.ToBase64String(closed.RowVersion));
                            Assert.Equal("DangMo", (await c.ReadNeed(id)).TrangThai);
                            Assert.Contains((await Search(actor: c.GS)).Items, x => x.Id == id);
                        }
                    }
                    if (n == 41)
                    {
                        Assert.Equal(400, (await Assert.ThrowsAsync<LoiNghiepVu>(() => s.Needs.TrangThaiAsync(id, "DangMo", Convert.ToBase64String(v1)))).Status);
                        Assert.Equal("BanNhap", (await c.ReadNeed(id)).TrangThai);
                    }
                    break;
                }
            case 37:
                {
                    var fault = new Mt1InjectedFailure(nameof(KhungGioRanh));
                    await using (var failing = c.Scope(c.HS, fault))
                        await Assert.ThrowsAsync<Mt1WriteException>(() => failing.Needs.TaoNhuCauHocAsync(m, c.HS, default));
                    Assert.Equal(1, fault.Hits);
                    Assert.Equal(0, await c.NeedCount());
                    c.Proof["fault"] = "Lỗi SaveChanges khung giờ sau lần ghi nhu cầu đầu; toàn bộ nhu cầu rollback.";
                    break;
                }
            case 38:
                m.MonHocIds = [c.Math, await c.Subject(false)];
                await Reject(m);
                Assert.Equal(0, await c.NeedCount());
                c.Proof["scopeNote"] = "Không suy ra thứ tự ghi môn nếu toàn bộ môn đã được validate trước.";
                break;
            case 42:
                m.LichRanh.Add(new()
                {
                    ThuTrongTuan = 1,
                    GioBatDau = new(19, 30),
                    GioKetThuc = new(21, 0)
                });
                await Reject(m);
                break;
            case 43:
                {
                    await c.ProfileSet();
                    var p = await Search();
                    Assert.Equal(4, p.Total);
                    Assert.Equal(new[] { "P1", "P2", "P3", "P7" }.Select(k => c.Profiles[k]).Order(), p.Items.Select(x => x.Id).Order());
                    Assert.Equal(p.Items.Count, p.Items.Select(x => x.ChuSoHuuId).Distinct().Count());
                    Assert.All(p.Items, x => Assert.IsType<HoSoGiaSuCongKhaiViewModel>(x));
                    break;
                }
            case 44:
            case 45:
            case 53:
                {
                    var ids = await c.NeedSet();
                    var p = await Search(new()
                    {
                        MonHocId = c.Math,
                        Trang = n == 44 ? 1 : n == 45 ? 2 : 99
                    }, c.GS);
                    Assert.Equal(12, p.Total);
                    Assert.Equal(n == 44 ? 9 : n == 45 ? 3 : 0, p.Items.Count);
                    if (n != 53)
                        Assert.Equal(ids.Skip(n == 44 ? 0 : 9).Take(n == 44 ? 9 : 3), p.Items.Select(x => x.Id));
                    else
                    {
                        var empty = await Search(new()
                        {
                            MonHocId = c.Math,
                            TuKhoa = c.Tag + "absent"
                        }, c.GS);
                        Assert.Equal(0, empty.Total);
                        Assert.Empty(empty.Items);
                    }
                    break;
                }
            case 46:
                {
                    await c.ProfileSet();
                    var p = await Search(new()
                    {
                        MonHocId = c.Math,
                        HocPhiToiDa = 200000,
                        DonViHocPhi = "VND/Gio"
                    });
                    Assert.Equal(c.P1, Assert.Single(p.Items).Id);
                    break;
                }
            case 47:
                {
                    await c.ProfileSet();
                    var p = await Search(new()
                    {
                        MonHocIds = [c.Math, c.Literature]
                    });
                    Assert.Equal(4, p.Total);
                    Assert.All(p.Items, x => Assert.All(x.MonTheoHoSo, a => Assert.Contains(a.MonHocId, new[] { c.Math, c.Literature })));
                    break;
                }
            case 48:
                {
                    await c.ProfileSet();
                    var p = await Search(new()
                    {
                        HinhThucHoc = "TrucTuyen",
                        KhuVuc = "  Thủ   Đức  "
                    });
                    Assert.Equal(new[] { c.P1, c.Profiles["P7"] }.Order(), p.Items.Select(x => x.Id).Order());
                    break;
                }
            case 50:
                {
                    m.LichRanh = [new() { ThuTrongTuan = 1, GioBatDau = new(18, 0), GioKetThuc = new(20, 0) }];
                    var id = await c.CreateNeed(m);
                    foreach (var (start, end, found) in new[] { (18, 20, true), (19, 20, true), (17, 20, false), (19, 21, false) })
                        Assert.Equal(found, (await Search(new()
                        {
                            MonHocId = c.Math,
                            ThuTrongTuan = 1,
                            GioBatDau = new(start, 0),
                            GioKetThuc = new(end, 0)
                        }, c.GS)).Items.Any(x => x.Id == id));
                    break;
                }
            case 51:
                {
                    await c.ProfileSet();
                    Assert.Equal(new[] { c.P1, c.Profiles["P7"] }, (await Search(new()
                    {
                        MonHocId = c.Math,
                        SapXep = "HocPhiTang",
                        DonViHocPhi = "VND/Gio"
                    })).Items.Select(x => x.Id));
                    break;
                }
            case 52:
                foreach (var page in new[] { 0, 100001 })
                {
                    var r = await s.Search.TimKiemAsync(new()
                    {
                        Trang = page
                    }, c.HS, default);
                    Assert.False(r.ThanhCong);
                    Assert.Equal(400, r.MaTrangThai);
                }
                break;
            case 54:
                {
                    m.TenNguoiHoc = "PRIVATE_CHILD_054";
                    m.TuoiNguoiHoc = 13;
                    m.HinhThucHoc = "TrucTiep";
                    m.KhuVuc = "Thủ Đức";
                    m.DiaChiChiTiet = "PRIVATE_ADDRESS_054";
                    var json = JsonSerializer.Serialize(await s.Search.ChiTietAsync(await c.CreateNeed(m, c.PH)));
                    Assert.DoesNotContain("PRIVATE_CHILD_054", json);
                    Assert.DoesNotContain("PRIVATE_ADDRESS_054", json);
                    Assert.DoesNotContain("TuoiNguoiHoc", json);
                    var p = await s.Search.ChiTietAsync(c.P1);
                    Assert.Contains(p.MonTheoHoSo, x => x.MonHocId == c.Math && x.HocPhi == 150000 && x.DonVi == "VND/Gio");
                    break;
                }
            case 55:
                {
                    var id = await c.CreateNeed();
                    await using (var db = c.Db())
                    {
                        await db.Set<NhuCauHoc>().Where(x => x.Id == id).ExecuteUpdateAsync(a => a.SetProperty(x => x.TrangThai, "DaDong"));
                        await db.Set<HoSoGiaSu>().Where(x => x.Id == c.P1).ExecuteUpdateAsync(a => a.SetProperty(x => x.DangHienThi, false));
                    }
                    foreach (var target in new[] { id, c.P1 })
                        Assert.Equal(404, (await Assert.ThrowsAsync<LoiNghiepVu>(() => s.Search.ChiTietAsync(target))).Status);
                    break;
                }
            case 56:
            case 57:
            case 58:
                {
                    await c.ProfileSet();
                    var ids = n == 57 ? new[] { c.P1, c.P1, c.Profiles["P2"] } : new[] { c.P1, c.Profiles["P2"], c.Profiles["P3"] };
                    if (n == 58)
                    {
                        await using var db = c.Db();
                        await db.Set<HoSoGiaSu>().Where(x => x.Id == c.Profiles["P2"]).ExecuteUpdateAsync(a => a.SetProperty(x => x.DangHienThi, false));
                    }
                    var r = await s.Search.SoSanhAsync(ids, c.HS, default);
                    c.Proof["comparison"] = new
                    {
                        r.ThanhCong,
                        r.MaTrangThai
                    };
                    if (n == 58)
                    {
                        Assert.False(r.ThanhCong);
                        Assert.Equal(404, r.MaTrangThai);
                    }
                    else
                    {
                        Assert.True(r.ThanhCong);
                        Assert.Equal(ids.Distinct(), r.DuLieu.Items.Select(x => x.Id));
                        Assert.Equal(n == 57 ? 2 : 3, r.DuLieu.Items.Count);
                        Assert.Contains(r.DuLieu.Items.SelectMany(x => x.MonTheoHoSo), x => x.DonVi == "VND/Buoi");
                    }
                    break;
                }
            case 60:
                {
                    await c.ProfileSet();
                    await c.CreateNeed();
                    var p = await s.Search.GoiYAsync();
                    Assert.True(p.HasNeed);
                    Assert.NotEmpty(p.Items);
                    Assert.All(p.Items, x => { Assert.NotEmpty(x.LyDo); Assert.All(x.LyDo, a => Assert.DoesNotContain("%", a)); Assert.Contains(x.MonTheoHoSo, a => a.MonHocId == c.Math); });
                    await using (var tutor = c.Scope(c.GS))
                    {
                        var other = await tutor.Search.GoiYAsync();
                        Assert.NotEmpty(other.Items);
                        Assert.All(other.Items, x => Assert.Contains(c.Math, x.MonHocIds));
                    }
                    await using var db = c.Db();
                    Assert.False(await db.Set<YeuCauLienHe>().AnyAsync(x => c.Users.Contains(x.NguoiGuiId)));
                    Assert.False(await db.Set<YeuCauKetNoiLopHoc>().AnyAsync(x => c.Users.Contains(x.NguoiGuiId)));
                    Assert.False(await db.Set<LopHoc>().AnyAsync(x => c.Users.Contains(x.BenHocId)));
                    c.Proof["reasons"] = p.Items.Select(x => new { x.Id, x.LyDo });
                    break;
                }
            case >= 76 and <= 84:
                await ContactCases(c, n);
                break;
            default:
                throw new InvalidOperationException("Chưa có implementation cho " + c.Id);
        }
    }

}
