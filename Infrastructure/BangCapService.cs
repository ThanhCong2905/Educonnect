using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Infrastructure;

public sealed class BangCapService(AppDbContext db, NguoiDungMauService current, IWebHostEnvironment env)
{
    public async Task<List<TepDinhKem>> CuaToiAsync()
    {
        var u = await current.HienTaiAsync();
        return await (from f in db.Set<TepDinhKem>().AsNoTracking() join h in db.Set<HoSoGiaSu>() on f.HoSoGiaSuId equals h.Id where h.GiaSuId == u.Id select f).OrderByDescending(f => f.TaoLuc).ToListAsync();
    }
    public Task TaiLenAsync(List<IFormFile> uploads) => Task.FromException(new LoiNghiepVu("Thêm bằng cấp bằng nút Lưu hồ sơ để kiểm tra trạng thái và phiên bản."));
    public async Task<(TepDinhKem File, string Path)> TaiAsync(Guid id)
    {
        var u = await current.HienTaiAsync();
        var f = await db.Set<TepDinhKem>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.HoSoGiaSuId != null && x.TrangThaiQuet == "Sach") ?? throw new LoiNghiepVu("Không tìm thấy bằng cấp.", 404);
        Rules.Check(u.LoaiNguoi == "QuanTri" || await db.Set<HoSoGiaSu>().AnyAsync(h => h.Id == f.HoSoGiaSuId && h.GiaSuId == u.Id), "Chỉ chủ hồ sơ được tải bằng cấp.", 403);
        var path = Path.Combine(env.ContentRootPath, "uploads", f.TenLuuTru);
        Rules.Check(File.Exists(path), "Tệp không còn trên máy chủ.", 404);
        return (f, path);
    }
    public Task XoaAsync(Guid id) => Task.FromException(new LoiNghiepVu("Chọn xóa bằng cấp trong hồ sơ rồi bấm Lưu hồ sơ."));
}
