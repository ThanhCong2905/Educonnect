namespace EduConnect.Mvc.Models;

public sealed class LichSuDieuChinhLich
{
    public Guid Id { get; set; }
    public Guid LichHocId { get; set; }
    public DateTimeOffset BatDauCu { get; set; }
    public DateTimeOffset KetThucCu { get; set; }
    public DateTimeOffset BatDauMoi { get; set; }
    public DateTimeOffset KetThucMoi { get; set; }
    public string LyDo { get; set; } = "";
    public Guid NguoiSuaId { get; set; }
    public DateTimeOffset ThayDoiLuc { get; set; }
}
