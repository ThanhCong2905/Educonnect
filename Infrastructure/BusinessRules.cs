using System.Globalization;
using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Infrastructure;

public sealed class LoiNghiepVu(string message, int status = 400) : Exception(message)
{
    public string? Truong { get; init; }
    public int Status { get; } = status;
}
public static class Rules
{
    public static string Date(DateTimeOffset? value) => value.HasValue ? Date(value.Value) : "—";
    public static void ValidateInput(object input)
    {
        var errors = new List<ValidationResult>();
        if (Validator.TryValidateObject(input, new ValidationContext(input), errors, true)) return;
        var error = errors[0];
        throw new LoiNghiepVu(error.ErrorMessage ?? "Dữ liệu nhập chưa hợp lệ.")
        {
            Truong = error.MemberNames.FirstOrDefault()
        };
    }
    public static int ErrorStatus(Exception ex)
    {
        if (ex is LoiNghiepVu business) return business.Status;
        if (ex is DbUpdateConcurrencyException) return 409;
        var sql = ex as Microsoft.Data.SqlClient.SqlException ?? ex.InnerException as Microsoft.Data.SqlClient.SqlException;
        return sql?.Number switch
        {
            51032 or 51035 => 404,
            51140 or 51146 or 51160 or 51161 => 403,
            1205 or 51033 or 51034 or 51037 or 51038 or 51039 or 51141 or 51144 or 51145 or 51147 or 51149 or 51150 or 51162 or 51163 or 2601 or 2627 => 409,
            >= 51000 => 400,
            _ => 500
        };
    }

    public static string ErrorMessage(Exception ex) => ex is LoiNghiepVu ? ex.Message : ex is DbUpdateConcurrencyException ? "Dữ liệu đã thay đổi. Hãy tải lại trước khi sửa." : (ex as Microsoft.Data.SqlClient.SqlException ?? ex.InnerException as Microsoft.Data.SqlClient.SqlException) is { Number: >= 51000 } sql ? sql.Message : ErrorStatus(ex) == 409 ? "Dữ liệu đã thay đổi hoặc thao tác bị trùng. Hãy tải lại." : "Chưa thể xử lý yêu cầu. Vui lòng thử lại sau.";

    public static Microsoft.Data.SqlClient.SqlParameter SqlVersion(string? version) => new("RowVersionValue", System.Data.SqlDbType.Binary, 8) { Value = string.IsNullOrEmpty(version) ? DBNull.Value : Version(version) };
    public static void Check(bool condition, string message, int status = 400, string? field = null)
    {
        if (!condition)
            throw new LoiNghiepVu(message, status) { Truong = field };
    }
    public static byte[] Version(string? value)
    {
        try
        {
            var b = Convert.FromBase64String(value ?? "");
            Check(b.Length == 8, "Phiên bản không hợp lệ.");
            return b;
        }
        catch (FormatException) { throw new LoiNghiepVu("Phiên bản không hợp lệ."); }
    }
    public static DateTimeOffset Local(DateTime value) => new(DateTime.SpecifyKind(value, DateTimeKind.Unspecified), TimeSpan.FromHours(7));
    public static string Date(DateTimeOffset value) => value.ToOffset(TimeSpan.FromHours(7)).ToString("dd/MM/yyyy HH:mm");
    public static string Fee(decimal? n, string? unit) => n.HasValue ? n.Value.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " đ / " + (unit == "VND/Buoi" ? "buổi" : "giờ") : "Chưa xác định";
    public static string Label(string? x) => x switch
    {
        "PhanQuyen" => "Thay đổi quyền quản trị",
        "Duyet" => "Duyệt hồ sơ",
        "NhanXuLy" => "Tiếp nhận báo cáo",
        "ThanhCong" => "Thành công",
        "KhongThanhCong" => "Không thành công",
        "GoBienPhap" => "Gỡ biện pháp",
        "NhanXuLyBaoCao" => "Tiếp nhận báo cáo",
        "CapNhatMonHoc" => "Cập nhật môn học",
        "PhanQuyenQuanTri" => "Thay đổi quyền quản trị",
        "PhanHoi" => "Phản hồi quá trình học",
        "BaoCao" => "Báo cáo vi phạm",
        "KiemDuyet" => "Kiểm duyệt",
        "NhacViec" => "Nhắc việc",
        "HoSo" => "Hồ sơ",
        "LoiMoiLop" => "Lời mời vào lớp",
        "LienKetTaiKhoan" => "Liên kết tài khoản",
        "NhuCau" => "Nhu cầu",
        "KetNoi" => "Liên hệ và kết nối",
        "ThanhVien" => "Thành viên",
        "QuanTri" => "Quản trị",
        "TamDung" => "Tạm dừng",
        "DaHoc" => "Đã học",
        "DaNghi" => "Đã nghỉ",
        "DuKien" => "Dự kiến",
        "DangDienRa" => "Đang diễn ra",
        "TiepTuc" => "Tiếp tục",
        "DieuChinh" => "Điều chỉnh",
        "Dung" => "Dừng",
        "HoatDong" => "Hoạt động",
        "BiKhoa" => "Bị khóa",
        "ChoXuLy" => "Chờ xử lý",
        "DangXuLy" => "Đang xử lý",
        "DaXuLy" => "Đã xử lý",
        "XuLy" => "Đã xử lý vi phạm",
        "BacBo" => "Chưa đủ căn cứ xử lý",
        "TaiKhoan" => "Tài khoản",
        "HoSoGiaSu" => "Hồ sơ gia sư",
        "NhuCauHoc" => "Nhu cầu học",
        "TinNhan" => "Tin nhắn",
        "LopHoc" => "Lớp học",
        "DanhGiaDoiTac" => "Đánh giá công khai",
        "KhoaTaiKhoan" => "Khóa tài khoản",
        "AnNoiDung" => "Ẩn nội dung",
        "TamDungLop" => "Tạm dừng lớp",
        "DuyetHoSo" => "Duyệt hồ sơ",
        "XuLyBaoCao" => "Xử lý báo cáo",
        "QuanLyTaiKhoan" => "Quản lý tài khoản",
        "QuanLyQuyen" => "Cấp quyền quản trị",
        "XemNhatKy" => "Xem nhật ký",
        "QuanLyDanhMuc" => "Quản lý danh mục",
        "BoSung" => "Bổ sung thông tin",
        "DeNghiXemLai" => "Đề nghị xem lại",
        "ChoDuyet" => "Chờ duyệt",
        "DaDuyet" => "Đã duyệt",
        "CanChinhSua" => "Cần chỉnh sửa",
        "CaHai" => "Trực tuyến và trực tiếp",
        "DangMo" => "Đang mở",
        "BanNhap" => "Bản nháp",
        "DaDong" => "Đã đóng",
        "ChoPhanHoi" => "Chờ phản hồi",
        "ChapNhan" => "Đã chấp nhận",
        "TuChoi" => "Từ chối",
        "DaRut" => "Đã rút",
        "ChoGiaSu" => "Chờ gia sư",
        "ChoBenHoc" => "Chờ bên học",
        "DaChapNhan" => "Đã tạo lớp",
        "HetHan" => "Hết hạn",
        "DangHoatDong" => "Đang học",
        "DaKetThuc" => "Đã kết thúc",
        "DaHuy" => "Đã hủy",
        "TrucTuyen" => "Trực tuyến",
        "TrucTiep" => "Trực tiếp",
        "GiaSu" => "Gia sư",
        "HocSinh" => "Học sinh",
        "PhuHuynh" => "Phụ huynh",
        "DaLenLich" => "Đã lên lịch",
        "DaDieuChinh" => "Đã điều chỉnh",
        _ => x ?? ""
    };

    public static void Slots(IReadOnlyList<SlotInput> slots)
    {
        Check(slots.Count is > 0 and <= 35, "Cần từ 1 đến 35 khung giờ.");
        foreach (var s in slots)
            Check(s.ThuTrongTuan <= 6 && s.GioBatDau < s.GioKetThuc, "Thứ hoặc giờ của khung giờ không hợp lệ.");
        for (int i = 0; i < slots.Count; i++)
            for (int j = i + 1; j < slots.Count; j++)
                Check(slots[i].ThuTrongTuan != slots[j].ThuTrongTuan || slots[i].GioBatDau >= slots[j].GioKetThuc || slots[j].GioBatDau >= slots[i].GioKetThuc, "Các khung giờ cùng ngày không được chồng lấn.");
    }
    public static void FeeCheck(decimal? value, string? unit)
    {
        Check((value == null) == string.IsNullOrEmpty(unit), "Nhập đồng thời học phí và đơn vị.");
        if (value != null)
            Check(value >= 0 && value <= 9999999999999999.99m && decimal.Round(value.Value, 2) == value && (unit == "VND/Gio" || unit == "VND/Buoi"), "Học phí không âm, tối đa hai chữ số thập phân; chọn đúng đơn vị.");
    }
    public static void Concurrency(AppDbContext db, object entity, string? version) => db.Entry(entity).Property("RowVersion").OriginalValue = Version(version);
}
public sealed class SlotInput
{
    public byte ThuTrongTuan { get; set; } = 1; public TimeOnly GioBatDau { get; set; } = new(19, 0); public TimeOnly GioKetThuc { get; set; } = new(20, 30);
}
