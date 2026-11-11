using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Infrastructure;

public sealed class QuyenLopHocService(AppDbContext db, NguoiDungHienTaiService current)
{
    public async Task<LopHoc> KiemTraQuyenXemAsync(Guid id)
    {
        var u = await current.HienTaiAsync();
        return await KiemTraQuyenXemAsync(id, u.Id);
    }
    public async Task<LopHoc> KiemTraQuyenXemAsync(Guid id, Guid nguoiXem)
    {
        var c = await db.Set<LopHoc>().SingleOrDefaultAsync(x => x.Id == id) ?? throw new LoiNghiepVu("Lớp không tồn tại.", 404);
        Rules.Check(c.GiaSuId == nguoiXem || c.BenHocId == nguoiXem || await db.Set<ThanhVienLopHoc>().AnyAsync(x => x.LopHocId == id && x.TaiKhoanId == nguoiXem && x.TrangThaiDuyet == "DaDuyet" && x.RoiLopLuc == null
            && db.Set<LienKetPhuHuynhHocSinh>().Any(l => l.Id == x.LienKetId && l.TrangThai == "DaLienKet" && l.KetThucLuc == null)), "Bạn chưa có quyền hoặc liên kết phù hợp để vào lớp này.", 403);
        return c;
    }
    public async Task<LopHoc> KiemTraQuyenGhiAsync(Guid id, bool tutor = false)
    {
        var c = await KiemTraQuyenXemAsync(id);
        var u = await current.HienTaiAsync();
        Rules.Check(c.TrangThai == "DangHoatDong", "Lớp không còn hoạt động.", 409);
        Rules.Check(tutor ? c.GiaSuId == u.Id : c.BenHocId == u.Id || await db.Set<ThanhVienLopHoc>().AnyAsync(x => x.LopHocId == id && x.TaiKhoanId == u.Id && x.TrangThaiDuyet == "DaDuyet" && x.RoiLopLuc == null && x.MucQuyen == "ChinhSua"), "Bạn không có quyền thực hiện thao tác này.", 403);
        return c;
    }
}
