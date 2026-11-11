namespace EduConnect.Mvc.Models;

public sealed class LienKetPhuHuynhHocSinh
{
    public Guid Id { get; set; }
    public Guid PhuHuynhId { get; set; }
    public Guid HocSinhId { get; set; }
    public Guid NguoiGuiId { get; set; }
    public string TrangThai { get; set; } = "ChoXacNhan";
    public DateTimeOffset GuiLuc { get; set; }
    public DateTimeOffset? XacNhanLuc { get; set; }
    public DateTimeOffset? KetThucLuc { get; set; }
    public Guid? NguoiKetThucId { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
