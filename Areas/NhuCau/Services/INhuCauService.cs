using System.Data;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.NhuCau.Services;

public interface INhuCauService
{
    Task<List<KhungGioRanh>> LichRanhAsync();
    Task XacNhanConNhuCauAsync(Guid id, string version);
    Task<List<MonHoc>> MonTenAsync();
    Task<List<NhuCauHoc>> LayAsync();
    Task<Guid> LuuAsync(NhuCauInput m);
    Task<KetQua<Guid>> TaoNhuCauHocAsync(TaoNhuCauBenHocViewModel duLieu, Guid maNguoiThucHien, CancellationToken huy);
    Task<KetQua<Guid>> CapNhatAsync(Guid maNhuCau, TaoNhuCauBenHocViewModel duLieu, byte[] rowVersion, Guid maNguoiThucHien, CancellationToken huy);
    Task<List<MonHoc>> MonAsync();
    Task<List<MonHoc>> MonDuocChonAsync();
    Task<HashSet<Guid>> DaKetNoiAsync();
    Task<KetQua<NhuCauInput>> LayDeSuaAsync(Guid id);
    Task TrangThaiAsync(Guid id, string state, string version);
}
