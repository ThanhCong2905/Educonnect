using EduConnect.Mvc.Areas.NhuCau.ViewModels;

namespace EduConnect.Mvc.Areas.TimKiem.ViewModels;

public sealed class SoSanhViewModel
{
    public IReadOnlyList<Guid> MaDoiTuong { get; init; } = [];
    public IReadOnlyList<HoSoGiaSuCongKhaiViewModel> HoSo { get; init; } = [];
    public IReadOnlyList<NhuCauCongKhaiViewModel> NhuCau { get; init; } = [];
    public List<UngVienViewModel> Items => MaDoiTuong.Select(id =>
        HoSo.Cast<UngVienViewModel>().Concat(NhuCau).Single(x => x.Id == id)).ToList();
}
