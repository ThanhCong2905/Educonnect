using EduConnect.Mvc.Areas.TimKiem.ViewModels;

namespace EduConnect.Mvc.Areas.NhuCau.ViewModels;

public sealed class HoSoGiaSuCongKhaiViewModel : UngVienViewModel
{
    public string TenHienThi => HoTen;
}

public sealed class NhuCauCongKhaiViewModel : UngVienViewModel;
