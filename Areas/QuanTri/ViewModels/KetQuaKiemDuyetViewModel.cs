namespace EduConnect.Mvc.Areas.QuanTri.ViewModels;

public sealed class KetQuaKiemDuyetViewModel
{
    public Guid BaoCaoId
    {
        get; set;
    }
    public string KetLuan { get; set; } = "";
    public string? KetQua
    {
        get; set;
    }
    public string TrangThai { get; set; } = "";
    public string RowVersion { get; set; } = "";
}
