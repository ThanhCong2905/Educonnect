using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
namespace EduConnect.Mvc.Areas.TimKiem.Services;

public interface ITimKiemStore
{
    IQueryable<HoSoGiaSu> HoSoCongKhai();
    Task<Page<UngVienViewModel>> TimHoSoAsync(BoLocTimKiemViewModel filter, CancellationToken ct);
    Task<Page<UngVienViewModel>> TimNhuCauAsync(BoLocTimKiemViewModel filter, Guid viewer, CancellationToken ct);
    Task<HoSoGiaSuCongKhaiViewModel?> ReadProfileAsync(Guid id, CancellationToken ct);
    Task<UngVienViewModel?> ReadNeedAsync(Guid id, CancellationToken ct);
    Task<List<UngVienViewModel>> LienQuanAsync(Guid owner);
    Task<UngVienViewModel> XemTruocAsync();
    Task<(List<UngVienViewModel> Items, bool HasNeed)> GoiYAsync();
}
