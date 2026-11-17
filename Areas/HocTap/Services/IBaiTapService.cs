using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
namespace EduConnect.Mvc.Areas.HocTap.Services;
public interface IBaiTapService
{
    Task<List<BaiTapGiao>> LayAsync(Guid lop);
    Task<BaiTapGiao> ChiTietAsync(Guid id);
    Task<List<BaiTapNop>> BaiNopAsync(Guid id);
    Task<List<KetQuaChamBai>> KetQuaAsync(Guid id);
    Task GiaoAsync(GiaoBaiTapViewModel m);
    Task NopAsync(NopBaiViewModel m);
    Task ChamAsync(ChamBaiViewModel m);
    Task<Guid> LichCuaBuoiAsync(Guid id);
    Task GanBuoiAsync(Guid assignment, Guid lessonId, string version);
    Task CongBoAsync(Guid id, string version);
}
