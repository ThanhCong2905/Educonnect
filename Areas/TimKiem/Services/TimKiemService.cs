using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.TimKiem.Services;

public sealed class TimKiemService(AppDbContext db, NguoiDungMauService current)
{
    private IQueryable<UngVienViewModel> PublicQuery() => from n in db.Set<NhuCauHoc>() join u in db.Set<NguoiThamGia>() on n.ChuSoHuuId equals u.Id join m in db.Set<MonHoc>() on n.MonHocId equals m.Id where n.TrangThai == "DangMo" && (u.LoaiNguoi != "GiaSu" || db.Set<HoSoGiaSu>().Any(h => h.GiaSuId == u.Id && h.TrangThaiDuyet == "DaDuyet" && db.Set<HoSoGiaSuMonDay>().Any(s => s.HoSoGiaSuId == h.Id && s.MonHocId == n.MonHocId))) select new UngVienViewModel { Id = n.Id, ChuSoHuuId = u.Id, HoTen = u.HoTen, LoaiNguoi = u.LoaiNguoi, TenMon = m.TenMon, MonHocId = m.Id, KhoiLop = n.KhoiLop, MucTieu = n.MucTieu, HinhThucHoc = n.HinhThucHoc, KhuVuc = n.KhuVuc, HocPhiDuKien = n.HocPhiDuKien, DonViHocPhi = n.DonViHocPhi, TaoLuc = n.TaoLuc, TrinhDo = db.Set<HoSoGiaSu>().Where(h => h.GiaSuId == u.Id).Select(h => h.TrinhDo).FirstOrDefault(), KinhNghiem = db.Set<HoSoGiaSu>().Where(h => h.GiaSuId == u.Id).Select(h => h.KinhNghiem).FirstOrDefault(), MoTa = db.Set<HoSoGiaSu>().Where(h => h.GiaSuId == u.Id).Select(h => h.MoTa).FirstOrDefault() };
    public async Task<SearchPage> TimAsync(BoLocTimKiemViewModel f)
    {
        var me = await current.HienTaiAsync();
        Rules.Check(f.Trang is >= 1 and <= 100000, "Trang không hợp lệ.");
        Rules.Check(f.ThuTrongTuan == null || f.ThuTrongTuan <= 6, "Thứ không hợp lệ.");
        Rules.Check(f.GioBatDau == null || f.GioKetThuc == null || f.GioBatDau < f.GioKetThuc, "Giờ bắt đầu phải trước giờ kết thúc.");
        var tutor = me.LoaiNguoi != "GiaSu";
        var q = PublicQuery().Where(n => n.ChuSoHuuId != me.Id && (tutor ? n.LoaiNguoi == "GiaSu" : n.LoaiNguoi != "GiaSu"));
        if (!string.IsNullOrWhiteSpace(f.TuKhoa))
            q = q.Where(n => n.HoTen.Contains(f.TuKhoa) || n.TenMon.Contains(f.TuKhoa) || (n.MucTieu != null && n.MucTieu.Contains(f.TuKhoa)));
        if (f.MonHocId != null)
            q = q.Where(n => n.MonHocId == f.MonHocId);
        if (!string.IsNullOrWhiteSpace(f.KhoiLop))
            q = q.Where(n => n.KhoiLop == f.KhoiLop);
        if (!string.IsNullOrWhiteSpace(f.HinhThucHoc))
            q = q.Where(n => n.HinhThucHoc == f.HinhThucHoc);
        if (!string.IsNullOrWhiteSpace(f.KhuVuc))
            q = q.Where(n => n.KhuVuc != null && n.KhuVuc.Contains(f.KhuVuc));
        if (f.HocPhiToiDa != null)
            q = q.Where(n => n.HocPhiDuKien <= f.HocPhiToiDa && n.DonViHocPhi == f.DonViHocPhi);
        if (f.ThuTrongTuan != null || f.GioBatDau != null || f.GioKetThuc != null)
            q = q.Where(n => db.Set<KhungGioRanh>().Any(s => s.NhuCauId == n.Id && (f.ThuTrongTuan == null || s.ThuTrongTuan == f.ThuTrongTuan) && (f.GioBatDau == null || s.GioBatDau <= f.GioBatDau) && (f.GioKetThuc == null || s.GioKetThuc >= f.GioKetThuc)));
        if (f.SapXep is "HocPhiTang" or "HocPhiGiam")
            q = q.Where(n => n.HocPhiDuKien != null && n.DonViHocPhi == f.DonViHocPhi);
        // Mỗi kết quả tương ứng một nhu cầu công khai.
        var total = await q.CountAsync();

        var chosen = q;
        var ordered = f.SapXep switch
        {
            "HocPhiTang" => chosen.OrderBy(n => n.HocPhiDuKien).ThenBy(n => n.Id),
            "HocPhiGiam" => chosen.OrderByDescending(n => n.HocPhiDuKien).ThenBy(n => n.Id),
            "Ten" => chosen.OrderBy(n => n.HoTen).ThenBy(n => n.Id),
            _ => chosen.OrderByDescending(n => n.TaoLuc).ThenBy(n => n.Id)
        };
        var rows = await ordered.Skip((f.Trang - 1) * 9).Take(9).ToListAsync();
        await SlotsAsync(rows);
        return new()
        {
            Filter = f,
            Items = rows,
            Total = total
        };
    }
    private async Task SlotsAsync(List<UngVienViewModel> rows)
    {
        var ids = rows.Select(n => n.Id).ToArray();
        var slots = await db.Set<KhungGioRanh>().Where(s => ids.Contains(s.NhuCauId)).ToListAsync();
        foreach (var n in rows)
            n.Slots = slots.Where(s => s.NhuCauId == n.Id).Select(s => new SlotInput { ThuTrongTuan = s.ThuTrongTuan, GioBatDau = s.GioBatDau, GioKetThuc = s.GioKetThuc }).ToList();
    }
    public async Task<UngVienViewModel> ChiTietAsync(Guid id)
    {
        var row = await PublicQuery().SingleOrDefaultAsync(n => n.Id == id) ?? throw new LoiNghiepVu("Nhu cầu không còn công khai.", 404);
        await SlotsAsync([row]);
        if (row.LoaiNguoi == "GiaSu")
        {
            var h = await db.Set<HoSoGiaSu>().AsNoTracking().SingleAsync(x => x.GiaSuId == row.ChuSoHuuId);
            row.ChuyenMon = h.ChuyenMon;
            row.KhuVucGiaSu = h.KhuVuc;
            row.MonDay = await (from d in db.Set<HoSoGiaSuMonDay>() join m in db.Set<MonHoc>() on d.MonHocId equals m.Id where d.HoSoGiaSuId == h.Id select m.TenMon + " · " + d.PhamViDay).ToListAsync();
        }
        return row;
    }
    public async Task<List<UngVienViewModel>> LienQuanAsync(Guid owner)
    {
        var rows = await PublicQuery().Where(n => n.ChuSoHuuId == owner).OrderByDescending(n => n.TaoLuc).ToListAsync();
        await SlotsAsync(rows);
        return rows;
    }
    public async Task<List<UngVienViewModel>> SoSanhAsync(Guid[] ids)
    {
        Rules.Check(ids.Distinct().Count() is >= 1 and <= 3, "Chọn từ 1 đến 3 đối tượng.");
        var rows = await PublicQuery().Where(n => ids.Contains(n.Id)).ToListAsync();
        Rules.Check(rows.Count == ids.Distinct().Count(), "Một nhu cầu đã đóng. Vui lòng chọn lại.");
        await SlotsAsync(rows);
        return rows;
    }
    public async Task<(List<UngVienViewModel> Items, bool HasNeed)> GoiYAsync()
    {
        var me = await current.HienTaiAsync();
        var tutor = me.LoaiNguoi == "GiaSu";
        var own = await db.Set<NhuCauHoc>().AsNoTracking().Where(n => n.ChuSoHuuId == me.Id && n.TrangThai == "DangMo").ToListAsync();
        var subjects = own.Select(n => n.MonHocId).ToHashSet();
        if (tutor)
        {
            var registered = await (from h in db.Set<HoSoGiaSu>() join m in db.Set<HoSoGiaSuMonDay>() on h.Id equals m.HoSoGiaSuId where h.GiaSuId == me.Id select m.MonHocId).ToListAsync();
            subjects.UnionWith(registered);
        }
        var query = PublicQuery().Where(n => n.ChuSoHuuId != me.Id && (tutor ? n.LoaiNguoi != "GiaSu" : n.LoaiNguoi == "GiaSu"));
        if (subjects.Count > 0)
            query = query.Where(n => subjects.Contains(n.MonHocId));
        var rows = await query.ToListAsync();
        await SlotsAsync(rows);
        var ownIds = own.Select(n => n.Id).ToList();
        var slots = await db.Set<KhungGioRanh>().AsNoTracking().Where(s => ownIds.Contains(s.NhuCauId)).ToListAsync();
        foreach (var candidate in rows)
        {
            var best = new List<string>();
            if (subjects.Contains(candidate.MonHocId))
                best.Add("Cùng môn " + candidate.TenMon);
            foreach (var need in own.Where(n => n.MonHocId == candidate.MonHocId))
            {
                var reasons = new List<string> { "Cùng môn " + candidate.TenMon };
                if (!string.IsNullOrWhiteSpace(candidate.KhoiLop) && candidate.KhoiLop == need.KhoiLop)
                    reasons.Add("Cùng khối lớp");
                if (candidate.HinhThucHoc == need.HinhThucHoc)
                    reasons.Add("Cùng hình thức học");
                if (candidate.Slots.Any(s => slots.Any(a => a.NhuCauId == need.Id && a.ThuTrongTuan == s.ThuTrongTuan && a.GioBatDau < s.GioKetThuc && s.GioBatDau < a.GioKetThuc)))
                    reasons.Add("Trùng lịch rảnh");
                if (reasons.Count > best.Count)
                    best = reasons;
            }
            candidate.LyDo = best;
        }
        return (rows.OrderByDescending(n => n.LyDo.Count).ThenByDescending(n => n.TaoLuc).ThenBy(n => n.Id).DistinctBy(n => n.ChuSoHuuId).Take(3).ToList(), own.Count > 0);
    }
}
