using System.Security.Claims;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Infrastructure;

public interface INguoiDungHienTaiService
{
    Task<NguoiThamGia> HienTaiAsync(CancellationToken huy = default);
    Task<NguoiThamGia> KiemTraNguoiThucHienAsync(Guid? id, CancellationToken huy);
    Task<List<NguoiThamGia>> DanhSachAsync();
    Task ChonAsync(Guid id);
}
