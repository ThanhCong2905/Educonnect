using System.Data;
using System.Text.Json;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.KetNoi.Services;

public interface IKetNoiService
{
    Task<Dictionary<Guid, LopHoc>> LopTheoLienHeAsync();
    Task<Dictionary<Guid, Guid>> HoiThoaiLienHeAsync();
    Task<Guid?> HoiThoaiAsync(Guid id);
    Task<(List<YeuCauLienHe> Contacts, List<YeuCauKetNoiLopHoc> Requests)> LayAsync();
    Task<YeuCauLienHe?> LienHeVoiAsync(Guid other);
    Task GuiLienHeAsync(GuiLienHeInput m);
    Task PhanHoiLienHeAsync(Guid id, string action, string version);
    Task<Guid> GuiYeuCauAsync(GuiYeuCauInput m, CancellationToken huy = default);
    Task<List<NhuCauHoc>> NhuCauDeXuatAsync(Guid contact);
    Task<List<MonHoc>> MonDeXuatAsync(Guid contact);
    Task<DeXuatLopViewModel> LayDeXuatAsync(Guid contact, Guid? id);
    Task<Guid> GuiDeXuatAsync(DeXuatLopViewModel m, CancellationToken huy = default);
    Task<YeuCauKetNoiLopHoc> ChiTietAsync(Guid id);
    Task<List<KhungGioDeXuat>> KhungAsync(Guid id);
    Task<Guid> ChapNhanAsync(Guid id, string version);
    Task<Guid> ChapNhanDeXuatAsync(Guid id, string version, CancellationToken huy = default);
    Task PhanHoiAsync(Guid id, string action, string version, CancellationToken huy = default);
    Task TuChoiDeXuatAsync(Guid maYeuCau, string rowVersion);
    Task RutYeuCauAsync(Guid maYeuCau, string rowVersion);
    Task<KetQua<Guid>> GuiYeuCauAsync(GuiYeuCauViewModel duLieu, Guid maNguoiThucHien, CancellationToken huy);
    Task<KetQua<Guid>> GuiDeXuatAsync(DeXuatLopViewModel duLieu, Guid maNguoiThucHien, CancellationToken huy);
    Task<KetQua<Guid>> ChapNhanDeXuatAsync(Guid maYeuCau, byte[] rowVersion, Guid maNguoiThucHien, CancellationToken huy);
    Task<KetQua> TuChoiDeXuatAsync(Guid maYeuCau, string lyDo, byte[] rowVersion, Guid maNguoiThucHien, CancellationToken huy);
    Task<KetQua> RutYeuCauAsync(Guid maYeuCau, byte[] rowVersion, Guid maNguoiThucHien, CancellationToken huy);
    Task<KetQua<Guid>> TaoYeuCauHuyAsync(Guid maLop, string lyDo, byte[] rowVersion, Guid maNguoiThucHien, CancellationToken huy);
    Task<KetQua> PhanHoiHuyAsync(Guid maYeuCau, bool dongY, byte[] rowVersion, Guid maNguoiThucHien, CancellationToken huy);
}
