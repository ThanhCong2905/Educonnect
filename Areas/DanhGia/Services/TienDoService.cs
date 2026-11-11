using EduConnect.Mvc.Areas.DanhGia.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.DanhGia.Services;

public sealed class TienDoService(AppDbContext db, NguoiDungHienTaiService current, QuyenLopHocService rights)
{
    public async Task<TienDoViewModel> LayAsync(Guid id, DateTimeOffset? tuNgay = null, DateTimeOffset? denNgay = null, int trang = 1, Guid? dot = null)
    {
        Rules.Check(tuNgay == null || denNgay == null || tuNgay <= denNgay, "Ngày bắt đầu phải trước hoặc bằng ngày kết thúc.");
        Rules.Check(trang is >= 1 and <= 100000, "Trang không hợp lệ.");
        var lop = await rights.KiemTraQuyenXemAsync(id);
        var me = await current.HienTaiAsync();
        var dotQuery = db.Set<DotPhanHoi>().AsNoTracking().Where(x => x.LopHocId == id && (dot == null || x.Id == dot));
        var lessons = from b in db.Set<BuoiHoc>().AsNoTracking()
                      join l in db.Set<LichHoc>() on b.LichHocId equals l.Id
                      where l.LopHocId == id && (tuNgay == null || l.BatDauLuc >= tuNgay)
                          && (denNgay == null || l.BatDauLuc < denNgay.Value.AddDays(1))
                      select b;
        var assignments = db.Set<BaiTapGiao>().AsNoTracking().Where(x => x.LopHocId == id && x.TrangThai == "DaGiao"
            && x.BuoiHocId != null && lessons.Any(b => b.Id == x.BuoiHocId));
        var latest = from a in assignments
                     from n in db.Set<BaiTapNop>().Where(n => n.BaiTapGiaoId == a.Id)
                         .OrderByDescending(n => n.NopLuc).ThenByDescending(n => n.Id).Take(1)
                     select n;
        var grades = await db.Set<KetQuaChamBai>().AsNoTracking().Where(x => latest.Any(n => n.Id == x.BaiTapNopId)).ToListAsync();
        var marks = grades.Where(x => x.Diem.HasValue).Select(x => x.Diem!.Value).ToList();
        var diemQuery = from g in db.Set<KetQuaChamBai>().AsNoTracking()
                        join n in latest on g.BaiTapNopId equals n.Id
                        join a in assignments on n.BaiTapGiaoId equals a.Id
                        orderby g.ChamLuc descending, g.Id
                        select new DiemTienDoRow { BaiTapId = a.Id, TieuDe = a.TieuDe, Diem = g.Diem, ChamLuc = g.ChamLuc };
        var dots = await dotQuery.OrderByDescending(x => x.TuNgay).ThenBy(x => x.Id).Skip((trang - 1) * 20).Take(20).ToListAsync();
        var dotIds = dots.Select(x => x.Id).ToList();
        var phieu = await db.Set<PhieuDanhGia>().AsNoTracking()
            .Where(x => x.LopHocId == id && (x.DotPhanHoiId == null || dotIds.Contains(x.DotPhanHoiId.Value)))
            .OrderByDescending(x => x.GuiLuc).ToListAsync();
        var people = await db.Users.Where(x => phieu.Select(p => p.NguoiGuiId).Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x.HoTen);
        return new TienDoViewModel
        {
            Lop = lop,
            TuNgay = tuNgay,
            DenNgay = denNgay,
            SoBuoiDaHoc = await lessons.CountAsync(x => x.TrangThai == "DaHoc"),
            SoBuoiNghi = await lessons.CountAsync(x => x.TrangThai == "DaNghi"),
            SoBuoiChuaGhiNhan = await lessons.CountAsync(x => x.TrangThai != "DaNghi" && x.TrangThai != "DaHuy" && x.GhiNhanLuc == null),
            GiaSuCoMat = await lessons.CountAsync(x => x.TrangThai == "DaHoc" && x.TrangThaiThamDuGiaSu == "CoMat"),
            BenHocCoMat = await lessons.CountAsync(x => x.TrangThai == "DaHoc" && x.TrangThaiThamDuBenHoc == "CoMat"),
            SoBaiDaGiao = await assignments.CountAsync(),
            SoBaiDaNop = await latest.CountAsync(),
            SoBaiNopMuon = await latest.CountAsync(n => assignments.Any(a => a.Id == n.BaiTapGiaoId && n.NopLuc > a.HanNopLuc)),
            SoBaiDaCham = grades.Count,
            DiemTrungBinh = marks.Count == 0 ? null : decimal.Round(marks.Average(), 2),
            Diem = await diemQuery.Take(20).ToListAsync(),
            Buoi = await lessons.OrderByDescending(x => x.GhiNhanLuc).Take(20).ToListAsync(),
            Bai = await assignments.OrderByDescending(x => x.HanNopLuc).Take(20).ToListAsync(),
            Dot = dots,
            TongDot = await dotQuery.CountAsync(),
            Trang = trang,
            Phieu = phieu,
            HoTen = people,
            QuyetDinh = await db.Set<QuyetDinhSauPhanHoi>().AsNoTracking().Where(x => dotIds.Contains(x.DotPhanHoiId)).ToListAsync(),
            CoThePhanHoi = lop.TrangThai == "DangHoatDong" && (me.Id == lop.GiaSuId || me.Id == lop.BenHocId || await db.Set<ThanhVienLopHoc>().AnyAsync(x => x.LopHocId == id && x.TaiKhoanId == me.Id && x.TrangThaiDuyet == "DaDuyet" && x.RoiLopLuc == null && x.MucQuyen == "ChinhSua"))
        };
    }
}
