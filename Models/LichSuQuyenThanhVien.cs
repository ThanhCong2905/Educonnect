namespace EduConnect.Mvc.Models;

public sealed class LichSuQuyenThanhVien
{
    public Guid Id { get; set; }
    public Guid ThanhVienId { get; set; }
    public Guid NguoiThucHienId { get; set; }
    public string? MucQuyenCu { get; set; }
    public string MucQuyenMoi { get; set; } = "";
    public bool ChoPhepNopBaiCu { get; set; }
    public bool ChoPhepNopBaiMoi { get; set; }
    public DateTimeOffset ThayDoiLuc { get; set; }
}
