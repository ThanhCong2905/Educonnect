using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.HocTap.Services;

public sealed partial class LopHocService
{
    public async Task<List<NguoiDuocMoiViewModel>> NguoiDuocMoiAsync(Guid id)
    {
        var lop = await rights.KiemTraQuyenXemAsync(id);
        var me = await current.HienTaiAsync();
        if (me.Id != lop.BenHocId || lop.TrangThai != "DangHoatDong") return [];
        var links = db.Set<LienKetPhuHuynhHocSinh>().Where(x => x.TrangThai == "DaLienKet" && x.KetThucLuc == null);
        IQueryable<Guid> candidates;
        if (me.LoaiNguoi == "HocSinh") candidates = links.Where(x => x.HocSinhId == me.Id).Select(x => x.PhuHuynhId);
        else if (lop.HocSinhLienKetId.HasValue)
        {
            if (!await links.AnyAsync(x => x.PhuHuynhId == me.Id && x.HocSinhId == lop.HocSinhLienKetId)) return [];
            candidates = links.Where(x => x.HocSinhId == lop.HocSinhLienKetId).Select(x => x.PhuHuynhId)
                .Union(links.Where(x => x.PhuHuynhId == me.Id && x.HocSinhId == lop.HocSinhLienKetId).Select(x => x.HocSinhId));
        }
        else candidates = links.Where(x => x.PhuHuynhId == me.Id).Select(x => x.HocSinhId);
        return await (from user in db.Set<TaiKhoan>() join person in db.Set<NguoiThamGia>() on user.Id equals person.Id
                      where candidates.Contains(user.Id) && user.Id != me.Id && user.TrangThai == "HoatDong"
                      && !db.Set<ThanhVienLopHoc>().Any(m => m.LopHocId == id && m.TaiKhoanId == user.Id && m.RoiLopLuc == null && m.TrangThaiDuyet != "TuChoi")
                      orderby person.HoTen
                      select new NguoiDuocMoiViewModel { LopHocId = id, TaiKhoanId = user.Id, Email = user.Email!, HoTen = person.HoTen, VaiTro = person.LoaiNguoi }).ToListAsync();
    }
}
