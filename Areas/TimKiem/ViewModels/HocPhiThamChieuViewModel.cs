namespace EduConnect.Mvc.Areas.TimKiem.ViewModels;

public sealed class HocPhiThamChieuViewModel
{
    public int MonHocId { get; init; }
    public string DonVi { get; init; } = "";
    public decimal TrungBinh { get; init; }
    public int SoHoSo { get; init; }
    public bool DuSoLieu => SoHoSo >= 3;
}

public sealed record DoiChieuHocPhiViewModel(HocPhiThamChieuViewModel? ThamChieu, decimal? HocPhi, string? DonVi)
{
    public decimal? ChenhLech => ThamChieu?.DuSoLieu == true && HocPhi.HasValue && DonVi == ThamChieu.DonVi
        ? HocPhi.Value - ThamChieu.TrungBinh : null;
}
