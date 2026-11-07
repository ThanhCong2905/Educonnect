using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.TimKiem.Services;

public interface ITimKiemService
{
    IQueryable<HoSoGiaSu> HoSoCongKhai();
    Task<SearchPage> TimAsync(BoLocTimKiemViewModel f);
    Task<KetQua<Page<UngVienViewModel>>> TimKiemAsync(BoLocTimKiemViewModel boLoc, Guid? maNguoiXem, CancellationToken huy);
    Task<UngVienViewModel> ChiTietAsync(Guid id, CancellationToken huy = default);
    Task<UngVienViewModel> XemTruocAsync();
    Task<List<UngVienViewModel>> LienQuanAsync(Guid owner);
    Task<List<UngVienViewModel>> SoSanhAsync(Guid[] ids);
    Task<KetQua<SoSanhViewModel>> SoSanhAsync(IReadOnlyList<Guid> maDoiTuong, Guid? maNguoiXem, CancellationToken huy);
    Task<(List<UngVienViewModel> Items, bool HasNeed)> GoiYAsync();
}
