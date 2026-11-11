using EduConnect.Mvc.Models;

namespace EduConnect.Mvc.Areas.QuanTri.ViewModels;

public sealed class NhatKyAnToanFilter
{
    public int Trang { get; set; } = 1;
    public string? Q { get; set; }
    public string? Ip { get; set; }
    public string? Nhom { get; set; }
    public string? KetQua { get; set; }
    public DateOnly? TuNgay { get; set; }
    public DateOnly? DenNgay { get; set; }
}

public sealed class NhatKyAnToanRow
{
    public NhatKyAnToan BanGhi { get; set; } = null!;
    public string? HoTen { get; set; }
    public string? Email { get; set; }
    public string? TaiKhoanDich { get; set; }
    public string TenSuKien => BanGhi.SuKien switch
    {
        "TaiKhoan.DangNhap" => "Đăng nhập",
        "TaiKhoan.DangKy" => "Đăng ký tài khoản",
        "TaiKhoan.DangXuat" => "Đăng xuất",
        "TaiKhoan.QuenMatKhau" => "Yêu cầu đặt lại mật khẩu",
        "TaiKhoan.DatLaiMatKhau" => "Đặt lại mật khẩu",
        "TaiKhoan.DoiMatKhau" => "Đổi mật khẩu",
        "TaiKhoan.HoSo" => "Cập nhật thông tin cá nhân",
        "KiemTraQuyen" => "Kiểm tra quyền truy cập",
        "PhienKhongHopLe" => "Phiên đăng nhập không hợp lệ",
        "SignalR.ThamGiaHoiThoai" => "Truy cập hội thoại trực tiếp",
        "QuanTri.NhatKyAnToan" => "Xem nhật ký an toàn",
        "QuanTri.XemNhatKy" => "Xem nhật ký quản trị",
        _ => BanGhi.SuKien
    };
}
