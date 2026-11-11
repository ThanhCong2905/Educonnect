namespace EduConnect.Mvc.Models;

public sealed class NhatKyAnToan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset ThoiDiem { get; set; } = DateTimeOffset.UtcNow;
    public string Nhom { get; set; } = "";
    public string SuKien { get; set; } = "";
    public string KetQua { get; set; } = "";
    public Guid? TaiKhoanId { get; set; }
    public Guid? TaiKhoanDichId { get; set; }
    public string? DinhDanhBam { get; set; }
    public string DiaChiIp { get; set; } = "";
    public string DuongDan { get; set; } = "";
    public string PhuongThuc { get; set; } = "";
    public string MaYeuCau { get; set; } = "";
    public string LyDo { get; set; } = "";
    public string? DoiTuongId { get; set; }
}
