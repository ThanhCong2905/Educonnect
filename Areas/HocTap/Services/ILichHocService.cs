using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
namespace EduConnect.Mvc.Areas.HocTap.Services;
public interface ILichHocService
{
    Task ThemHangLoatAsync(LichHangLoatInput m);
    Task<LopHoc> LopDuocXepLichAsync(Guid id);
    Task<LichHoc> LayAsync(Guid id);
    Task SuaAsync(SuaLichInput m);
    Task TaoBuoiBuAsync(SuaLichInput m);
    Task GhiNhanAsync(GhiNhanBuoiHocViewModel m);
    Task<(BuoiHoc? Record, List<LichSuGhiNhanBuoiHoc> History)> GhiNhanChiTietAsync(Guid id);
    Task<List<LichSuDieuChinhLich>> LichSuDieuChinhAsync(Guid id);
    Task BatDauBuoiAsync(Guid id, string version);
    Task ThongBaoNghiAsync(Guid id, string reason, bool canHocBu, string version);
}
