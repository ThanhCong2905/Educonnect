using System.Security.Claims;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Infrastructure;

public sealed class NguoiDungHienTaiService(AppDbContext db, IHttpContextAccessor context) : INguoiDungHienTaiService
{
    public async Task<NguoiThamGia> HienTaiAsync(CancellationToken huy = default)
    {
        var principal = context.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true
            || !Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
            throw new LoiNghiepVu("Vui lòng đăng nhập để tiếp tục.", 401);

        return await db.Set<NguoiThamGia>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, huy)
            ?? throw new LoiNghiepVu("Tài khoản hiện không thể truy cập.", 403);
    }

    public async Task<NguoiThamGia> KiemTraNguoiThucHienAsync(Guid? id, CancellationToken huy)
    {
        var user = await HienTaiAsync(huy);
        Rules.Check(id == null || id == user.Id, "Người thực hiện không khớp phiên hiện tại.", 403);
        return user;
    }

    public Task<List<NguoiThamGia>> DanhSachAsync()
        => db.Set<NguoiThamGia>().AsNoTracking().OrderBy(x => x.LoaiNguoi).ThenBy(x => x.HoTen).ToListAsync();

    [Obsolete("Sử dụng đăng nhập Identity để thiết lập người dùng.")]
    public Task ChonAsync(Guid id)
        => Task.FromException(new LoiNghiepVu("Vui lòng đăng nhập bằng tài khoản của bạn.", 403));
}
