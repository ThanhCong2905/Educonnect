using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
namespace EduConnect.Mvc.Areas.HocTap.Services;
public interface ILopHocService
{
    Task<List<LopHoc>> LayAsync();
    Task<LopHoc> ChiTietAsync(Guid id);
    Task<DanhGiaDoiTac?> DanhGiaDaGuiAsync(Guid id);
    Task<List<LichHoc>> LichAsync(Guid id);
    Task<List<LichHoc>> LocBuoiAsync(Guid id, string? month, string? state);
    Task<List<BuoiHoc>> BuoiAsync(Guid id);
    Task<List<ThanhVienLopHoc>> ThanhVienAsync(Guid id);
    Task<NguoiDuocMoiViewModel> TimNguoiDuocMoiAsync(Guid id, string email);
    Task DeNghiAsync(Guid id, string email);
    Task<List<LoiMoiThanhVienViewModel>> LoiMoiAsync();
    Task PhanHoiLoiMoiAsync(Guid id, bool agree, string version);
    Task DuyetAsync(Guid member, string action, string permission, string version, bool choPhepNopBai = false);
    Task<KetQua> XacNhanKetThucAsync(Guid maYeuCau, byte[] rowVersion, Guid maNguoiThucHien, CancellationToken huy);
    Task KetThucAsync(Guid id, string version);
    Task YeuCauHuyAsync(Guid id, string reason, string? rowVersion = null, CancellationToken huy = default);
    Task<List<YeuCauHuyKetNoi>> HuyAsync(Guid id);
    Task PhanHoiHuyAsync(Guid id, bool agree, string version, CancellationToken huy = default);
    Task<List<NguoiDuocMoiViewModel>> NguoiDuocMoiAsync(Guid id);
}
