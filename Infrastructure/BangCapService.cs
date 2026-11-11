using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Infrastructure;

public sealed class BangCapService(AppDbContext db, NguoiDungHienTaiService current, IWebHostEnvironment env, IConfiguration? configuration = null) : IBangCapService
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
        var allowUnscanned = env.IsDevelopment() && configuration?.GetValue<bool>("FileScanning:AllowUnscannedUploads") == true;
        var f = await db.Set<TepDinhKem>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.HoSoGiaSuId != null && (x.TrangThaiQuet == "Sach" || (allowUnscanned && x.TrangThaiQuet == "ChoQuet"))) ?? throw new LoiNghiepVu("Không tìm thấy bằng cấp khả dụng.", 404);
        var reviewer=u.LoaiNguoi=="QuanTri"&&await db.UserClaims.AnyAsync(x=>x.UserId==u.Id&&x.ClaimType=="EduConnect.AdminPermission"&&x.ClaimValue=="DuyetHoSo");
        Rules.Check(reviewer || await db.Set<HoSoGiaSu>().AnyAsync(h => h.Id == f.HoSoGiaSuId && h.GiaSuId == u.Id), "Bạn chưa có quyền tải bằng cấp này.", 403);
        var path = Path.Combine(env.ContentRootPath, "uploads", f.TenLuuTru);
        Rules.Check(File.Exists(path), "Tệp không còn trên máy chủ.", 404);
        return (f, path);
    }
    public Task XoaAsync(Guid id) => Task.FromException(new LoiNghiepVu("Chọn xóa bằng cấp trong hồ sơ rồi bấm Lưu hồ sơ."));
}
