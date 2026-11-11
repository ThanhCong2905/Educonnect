namespace EduConnect.Mvc.Models;

public sealed class LuotDangNhuCau
{
    public Guid Id { get; set; }
    public Guid NhuCauId { get; set; }
    public Guid TaiKhoanId { get; set; }
    public Guid MaThaoTac { get; set; }
    public string HanhDong { get; set; } = "Dang";
    public DateTimeOffset DangLuc { get; set; }
    public DateOnly NgayDang { get; set; }
}
