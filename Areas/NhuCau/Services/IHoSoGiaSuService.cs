using System.Data;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.NhuCau.Services;

public interface IHoSoGiaSuService
{
    Task<KetQua<HoSoGiaSuCongKhaiViewModel>> LayHoSoAsync(Guid maHoSo, Guid? maNguoiXem, CancellationToken huy);
    Task<HoSoInput> LayDeSuaAsync();
    Task CapNhatAsync(HoSoInput m);
    Task DoiTrangThaiAsync(string action,string version);
    Task DuDieuKienAsync(HoSoGiaSu h);
    Task<List<HoSoGiaSu>> ChoDuyetAsync();
    Task<HoSoInput> XemDuyetAsync(Guid id);
    Task DuyetAsync(Guid id,string version,bool approve,string? reason);
}
