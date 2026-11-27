namespace EduConnect.Mvc.Areas.TimKiem.ViewModels;

public sealed class GiaSuLienQuanViewModel
{
    public UngVienViewModel HoSo { get; init; } = new();
    public int MonHocId { get; init; }
    public string TenMon { get; init; } = "";
    public string PhamViDay { get; init; } = "";
    public decimal? HocPhi { get; init; }
    public string? DonVi { get; init; }
    public decimal? HocPhiGoc { get; init; }
    public decimal? ChenhLech { get; init; }
    public bool CungKhuVuc { get; init; }
    public bool TrungPhamVi { get; init; }
}
