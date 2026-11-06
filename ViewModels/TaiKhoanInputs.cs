using System.ComponentModel.DataAnnotations;

namespace EduConnect.Mvc.ViewModels;

public sealed class DangKyInput
{
    [Required(ErrorMessage = "Nhập họ tên."), StringLength(256)]
    public string HoTen { get; set; } = "";
    [Required(ErrorMessage = "Nhập email."), EmailAddress(ErrorMessage = "Email chưa đúng định dạng."), StringLength(256)]
    public string Email { get; set; } = "";
    [Required(ErrorMessage = "Chọn vai trò."), RegularExpression("HocSinh|PhuHuynh|GiaSu", ErrorMessage = "Chọn Học sinh, Phụ huynh hoặc Gia sư.")]
    public string VaiTro { get; set; } = "HocSinh";
    [Required(ErrorMessage = "Nhập mật khẩu."), MaxLength(128), DataType(DataType.Password)]
    public string MatKhau { get; set; } = "";
    [Required(ErrorMessage = "Nhập lại mật khẩu."), Compare(nameof(MatKhau), ErrorMessage = "Hai mật khẩu chưa giống nhau."), DataType(DataType.Password)]
    public string XacNhanMatKhau { get; set; } = "";
}

public sealed class DangNhapInput
{
    [Required(ErrorMessage = "Nhập email."), EmailAddress(ErrorMessage = "Email chưa đúng định dạng."), StringLength(256)]
    public string Email { get; set; } = "";
    [Required(ErrorMessage = "Nhập mật khẩu."), MaxLength(128), DataType(DataType.Password)]
    public string MatKhau { get; set; } = "";
    public string? ReturnUrl { get; set; }
}

public sealed class QuenMatKhauInput
{
    [Required(ErrorMessage = "Nhập email."), EmailAddress(ErrorMessage = "Email chưa đúng định dạng."), StringLength(256)]
    public string Email { get; set; } = "";
}

public sealed class DatLaiMatKhauInput
{
    [Required, EmailAddress, StringLength(256)] public string Email { get; set; } = "";
    [Required, StringLength(4096)] public string Ma { get; set; } = "";
    [Required(ErrorMessage = "Nhập mật khẩu mới."), MaxLength(128), DataType(DataType.Password)] public string MatKhau { get; set; } = "";
    [Required(ErrorMessage = "Nhập lại mật khẩu mới."), Compare(nameof(MatKhau), ErrorMessage = "Hai mật khẩu chưa giống nhau."), DataType(DataType.Password)]
    public string XacNhanMatKhau { get; set; } = "";
}
