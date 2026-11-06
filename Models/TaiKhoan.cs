using Microsoft.AspNetCore.Identity;

namespace EduConnect.Mvc.Models;

public sealed class TaiKhoan : IdentityUser<Guid>
{
    public string HoTen { get; set; } = "";
    public string? AnhDaiDien { get; set; }
    public string TrangThai { get; set; } = "HoatDong";
}

public sealed class VaiTro : IdentityRole<Guid> { }

public sealed class HocSinh
{
    public Guid TaiKhoanId { get; set; }
}

public sealed class PhuHuynh
{
    public Guid TaiKhoanId { get; set; }
}

public sealed class PhienDangNhap
{
    public Guid Id { get; set; }
    public Guid TaiKhoanId { get; set; }
    public DateTimeOffset TaoLuc { get; set; }
    public DateTimeOffset HoatDongCuoiLuc { get; set; }
    public DateTimeOffset? ThuHoiLuc { get; set; }
    public string? LyDoThuHoi { get; set; }
    public byte[] RowVersion { get; set; } = [];
}

public sealed class TrangThaiThuDangNhap
{
    public string LoaiKhoa { get; set; } = "";
    public string GiaTriKhoa { get; set; } = "";
    public int SoLanSai { get; set; }
    public DateTimeOffset LanSaiCuoiLuc { get; set; }
    public DateTimeOffset? ChoDenLuc { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
