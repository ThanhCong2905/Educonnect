namespace EduConnect.Mvc.Areas.HocTap.ViewModels;

public sealed class NguoiDuocMoiViewModel
{
    public Guid LienKetId { get; set; }
    public Guid HocSinhId { get; set; }
    public Guid LopHocId { get; init; }
    public Guid TaiKhoanId { get; init; }
    public string Email { get; init; } = "";
    public string HoTen { get; init; } = "";
    public string VaiTro { get; init; } = "";
}

public sealed class LoiMoiThanhVienViewModel
{
    public Guid Id { get; init; }
    public string TenLop { get; init; } = "";
    public string NguoiMoi { get; init; } = "";
    public string TrangThaiLop { get; init; } = "";
    public string TrangThaiDuyet { get; init; } = "";
    public DateTimeOffset? DongYThamGiaLuc { get; init; }
    public byte[] RowVersion { get; init; } = [];
}
