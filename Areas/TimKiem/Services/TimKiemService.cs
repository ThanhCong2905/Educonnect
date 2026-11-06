using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.TimKiem.Services;

public sealed class TimKiemService(AppDbContext db, NguoiDungHienTaiService current)
{
    private readonly HoSoCongKhaiQuery profiles = new(db);
    public IQueryable<HoSoGiaSu> HoSoCongKhai() => profiles.CongKhai();
    private IQueryable<UngVienViewModel> NhuCauCongKhai() => from n in db.Set<NhuCauHoc>()
                                                             join u in db.Set<NguoiThamGia>() on n.ChuSoHuuId equals u.Id
                                                             join m in db.Set<MonHoc>() on n.MonHocId equals m.Id
                                                             where n.LoaiNhuCau == "TimGiaSu" && n.TrangThai == "DangMo" && (u.LoaiNguoi == "HocSinh" || u.LoaiNguoi == "PhuHuynh")
                                                             select new NhuCauCongKhaiViewModel { Id = n.Id, ChuSoHuuId = u.Id, HoTen = u.HoTen, LoaiNguoi = u.LoaiNguoi, TenMon = m.TenMon, MonHocId = m.Id, KhoiLop = n.KhoiLop, MucTieu = n.MucTieu, HinhThucHoc = n.HinhThucHoc, KhuVuc = n.KhuVuc, HocPhiDuKien = n.HocPhiDuKien, DonViHocPhi = n.DonViHocPhi, TaoLuc = n.TaoLuc };
    private IQueryable<HoSoGiaSu> LocHoSo(BoLocTimKiemViewModel f)
    {
        var q = HoSoCongKhai();
        var key = HoSoGiaSuService.ChuanHoa(f.TuKhoa);
        var area = HoSoGiaSuService.ChuanHoa(f.KhuVuc);
        if (key != "")
            q = q.Where(h => db.Set<NguoiThamGia>().Any(u => u.Id == h.GiaSuId && EF.Functions.Collate(u.HoTen, "Vietnamese_100_CI_AS").Contains(key)) || EF.Functions.Collate(h.ChuyenMon, "Vietnamese_100_CI_AS").Contains(key) || EF.Functions.Collate(h.MoTa, "Vietnamese_100_CI_AS").Contains(key) || db.Set<HoSoGiaSuMonDay>().Any(d => d.HoSoGiaSuId == h.Id && db.Set<MonHoc>().Any(m => m.Id == d.MonHocId && EF.Functions.Collate(m.TenMon, "Vietnamese_100_CI_AS").Contains(key))));
        if (area != "")
            q = q.Where(h => EF.Functions.Collate(h.KhuVuc, "Vietnamese_100_CI_AS").Contains(area));
        if (!string.IsNullOrWhiteSpace(f.TrinhDo))
            q = q.Where(h => h.TrinhDo.Contains(f.TrinhDo));
        if (f.HinhThucHoc == "TrucTuyen")
            q = q.Where(h => h.DayTrucTuyen);
        if (f.HinhThucHoc == "TrucTiep")
            q = q.Where(h => h.DayTrucTiep);
        var ids = f.MonHocIds.Concat(f.MonHocId.HasValue ? [f.MonHocId.Value] : Array.Empty<int>()).Distinct().ToList();
        var byPrice = f.SapXep is "HocPhiTang" or "HocPhiGiam";
        return q.Where(h => db.Set<HoSoGiaSuMonDay>().Any(d => d.HoSoGiaSuId == h.Id
            && (ids.Count == 0 || ids.Contains(d.MonHocId))
            && (string.IsNullOrEmpty(f.KhoiLop) || EF.Functions.Collate(d.PhamViDay, "Vietnamese_100_CI_AS").Contains(f.KhoiLop))
            && (f.HocPhiToiDa == null || (d.HocPhiDeXuat <= f.HocPhiToiDa && d.DonViHocPhi == f.DonViHocPhi))
            && (!byPrice || (d.HocPhiDeXuat != null && d.DonViHocPhi == f.DonViHocPhi))));
    }
    private async Task<List<UngVienViewModel>> HoSoRowsAsync(List<HoSoGiaSu> hs, CancellationToken huy = default)
        => (await profiles.LayDanhSachAsync(hs, huy)).Cast<UngVienViewModel>().ToList();
    public async Task<SearchPage> TimAsync(BoLocTimKiemViewModel f)
    {
        var user = await current.HienTaiAsync();
        var page = (await TimKiemAsync(f, user.Id, default)).DuLieu;
        return new()
        {
            Filter = f,
            Items = page.Items,
            Total = page.Total
        };
    }

    public Task<KetQua<Page<UngVienViewModel>>> TimKiemAsync(BoLocTimKiemViewModel boLoc, Guid? maNguoiXem, CancellationToken huy)
        => KetQua<Page<UngVienViewModel>>.ThucHienAsync(async () =>
        {
            var me = await current.KiemTraNguoiThucHienAsync(maNguoiXem, huy);
            return await TimDuLieuAsync(boLoc, me, huy);
        });

    private async Task<Page<UngVienViewModel>> TimDuLieuAsync(BoLocTimKiemViewModel f, NguoiThamGia me, CancellationToken huy)
    {
        Rules.Check(f.Trang is >= 1 and <= 100000, "Trang không hợp lệ.");
        Rules.Check(f.HocPhiToiDa == null || f.HocPhiToiDa >= 0, "Học phí tối đa không âm.");
        Rules.Check(f.DonViHocPhi is "VND/Gio" or "VND/Buoi", "Chọn đúng đơn vị học phí.");
        if (me.LoaiNguoi != "GiaSu")
        {
            var q = LocHoSo(f);
            var total = await q.CountAsync(huy);
            var ids = f.MonHocIds.Concat(f.MonHocId.HasValue ? [f.MonHocId.Value] : Array.Empty<int>()).ToList();
            var priced = from h in q select new { h, Price = db.Set<HoSoGiaSuMonDay>().Where(d => d.HoSoGiaSuId == h.Id && (ids.Count == 0 || ids.Contains(d.MonHocId)) && (string.IsNullOrEmpty(f.KhoiLop) || EF.Functions.Collate(d.PhamViDay, "Vietnamese_100_CI_AS").Contains(f.KhoiLop)) && d.DonViHocPhi == f.DonViHocPhi && (f.HocPhiToiDa == null || d.HocPhiDeXuat <= f.HocPhiToiDa)).Min(d => d.HocPhiDeXuat), Name = db.Set<NguoiThamGia>().Where(u => u.Id == h.GiaSuId).Select(u => u.HoTen).First() };
            var ordered = f.SapXep switch
            {
                "HocPhiTang" => priced.OrderBy(x => x.Price).ThenBy(x => x.h.Id),
                "HocPhiGiam" => priced.OrderByDescending(x => x.Price).ThenBy(x => x.h.Id),
                "Ten" => priced.OrderBy(x => x.Name).ThenBy(x => x.h.Id),
                _ => priced.OrderByDescending(x => x.h.DuyetLuc).ThenBy(x => x.h.Id)
            };
            var rows = await HoSoRowsAsync(await ordered.Skip((f.Trang - 1) * 9).Take(9).Select(x => x.h).ToListAsync(huy), huy);
            if (ids.Count > 0)
            foreach (var row in rows)
                row.MonTheoHoSo = row.MonTheoHoSo.Where(m => ids.Contains(m.MonHocId)).ToList();
            return new()
            {
                Trang = f.Trang,
                Items = rows,
                Total = total
            };
        }
        Rules.Check(f.ThuTrongTuan == null || f.ThuTrongTuan <= 6, "Thứ không hợp lệ.");
        Rules.Check(f.GioBatDau == null || f.GioKetThuc == null || f.GioBatDau < f.GioKetThuc, "Giờ bắt đầu phải trước giờ kết thúc.");
        var n = NhuCauCongKhai().Where(x => x.ChuSoHuuId != me.Id);
        if (!string.IsNullOrWhiteSpace(f.TuKhoa))
            n = n.Where(x => x.HoTen.Contains(f.TuKhoa) || (x.TenMon.Contains(f.TuKhoa) || db.Set<NhuCauMonHoc>().Any(d => d.NhuCauId == x.Id && db.Set<MonHoc>().Any(m => m.Id == d.MonHocId && m.TenMon.Contains(f.TuKhoa)))) || (x.MucTieu != null && x.MucTieu.Contains(f.TuKhoa)));
        var subjectIds = f.MonHocIds.Concat(f.MonHocId.HasValue ? [f.MonHocId.Value] : Array.Empty<int>()).Distinct().ToList();
        if (subjectIds.Count > 0)
            n = n.Where(x => subjectIds.Contains(x.MonHocId) || db.Set<NhuCauMonHoc>().Any(d => d.NhuCauId == x.Id && subjectIds.Contains(d.MonHocId)));
        if (!string.IsNullOrWhiteSpace(f.KhoiLop))
            n = n.Where(x => x.KhoiLop == f.KhoiLop);
        if (!string.IsNullOrWhiteSpace(f.HinhThucHoc))
            n = n.Where(x => x.HinhThucHoc == f.HinhThucHoc);
        if (!string.IsNullOrWhiteSpace(f.KhuVuc))
            n = n.Where(x => x.KhuVuc != null && x.KhuVuc.Contains(f.KhuVuc));
        if (f.HocPhiToiDa != null)
            n = n.Where(x => x.HocPhiDuKien <= f.HocPhiToiDa && x.DonViHocPhi == f.DonViHocPhi);
        if (f.ThuTrongTuan != null || f.GioBatDau != null || f.GioKetThuc != null)
            n = n.Where(x => db.Set<KhungGioRanh>().Any(s => s.NhuCauId == x.Id && (f.ThuTrongTuan == null || s.ThuTrongTuan == f.ThuTrongTuan) && (f.GioBatDau == null || s.GioBatDau <= f.GioBatDau) && (f.GioKetThuc == null || s.GioKetThuc >= f.GioKetThuc)));
        if (f.SapXep is "HocPhiTang" or "HocPhiGiam")
            n = n.Where(x => x.HocPhiDuKien != null && x.DonViHocPhi == f.DonViHocPhi);
        var count = await n.CountAsync(huy);
        var sorted = f.SapXep switch
        {
            "HocPhiTang" => n.OrderBy(x => x.HocPhiDuKien).ThenBy(x => x.Id),
            "HocPhiGiam" => n.OrderByDescending(x => x.HocPhiDuKien).ThenBy(x => x.Id),
            "Ten" => n.OrderBy(x => x.HoTen).ThenBy(x => x.Id),
            _ => n.OrderByDescending(x => x.TaoLuc).ThenBy(x => x.Id)
        };
        var items = await sorted.Skip((f.Trang - 1) * 9).Take(9).ToListAsync(huy);
        await SlotsAsync(items, huy);
        return new()
        {
            Trang = f.Trang,
            Items = items,
            Total = count
        };
    }
    private async Task SlotsAsync(List<UngVienViewModel> rows, CancellationToken huy = default)
    {
        var ids = rows.Select(x => x.Id).ToList();
        var subjects = await (from link in db.Set<NhuCauMonHoc>()
                              join subject in db.Set<MonHoc>() on link.MonHocId equals subject.Id
                              where ids.Contains(link.NhuCauId)
                              orderby subject.Id
                              select new { link.NhuCauId, subject.Id, subject.TenMon }).ToListAsync(huy);
        var legacyIds = rows.Where(row => !subjects.Any(x => x.NhuCauId == row.Id)).Select(row => row.MonHocId).Distinct().ToList();
        var legacySubjects = await db.Set<MonHoc>().Where(subject => legacyIds.Contains(subject.Id)).ToListAsync(huy);
        foreach (var row in rows)
        {
            var selected = subjects.Where(x => x.NhuCauId == row.Id).ToList();
            row.MonHocIds = selected.Select(x => x.Id).ToList();
            if (selected.Count > 0)
                row.TenMon = string.Join(", ", selected.Select(x => x.TenMon));
            else
            {
                row.MonHocIds = [row.MonHocId];
                row.TenMon = legacySubjects.FirstOrDefault(x => x.Id == row.MonHocId)?.TenMon ?? row.TenMon;
            }
        }
        var slots = await db.Set<KhungGioRanh>().Where(s => ids.Contains(s.NhuCauId)).ToListAsync(huy);
        foreach (var row in rows)
            row.Slots = slots.Where(s => s.NhuCauId == row.Id).Select(s => new SlotInput { ThuTrongTuan = s.ThuTrongTuan, GioBatDau = s.GioBatDau, GioKetThuc = s.GioKetThuc }).ToList();
    }
    public async Task<UngVienViewModel> ChiTietAsync(Guid id, CancellationToken huy = default)
    {
        var h = await HoSoCongKhai().SingleOrDefaultAsync(x => x.Id == id, huy);
        if (h != null)
            return (await HoSoRowsAsync([h], huy)).Single();
        var n = await NhuCauCongKhai().SingleOrDefaultAsync(x => x.Id == id, huy) ?? throw new LoiNghiepVu("Hồ sơ hoặc nhu cầu đang ẩn, chưa được duyệt hoặc không còn công khai.", 404);
        await SlotsAsync([n], huy);
        return n;
    }
    public async Task<UngVienViewModel> XemTruocAsync()
    {
        var me = await current.HienTaiAsync();
        Rules.Check(me.LoaiNguoi == "GiaSu", "Chỉ gia sư xem trước hồ sơ của mình.", 403);
        var h = await db.Set<HoSoGiaSu>().AsNoTracking().SingleOrDefaultAsync(x => x.GiaSuId == me.Id) ?? throw new LoiNghiepVu("Hãy lưu hồ sơ trước.");
        return (await HoSoRowsAsync([h])).Single();
    }
    public async Task<List<UngVienViewModel>> LienQuanAsync(Guid owner)
    {
        var rows = await NhuCauCongKhai().Where(x => x.ChuSoHuuId == owner).OrderByDescending(x => x.TaoLuc).ToListAsync();
        await SlotsAsync(rows);
        return rows;
    }
    public async Task<List<UngVienViewModel>> SoSanhAsync(Guid[] ids)
    {
        var user = await current.HienTaiAsync();
        return (await SoSanhAsync(ids, user.Id, default)).DuLieu.Items;
    }

    public Task<KetQua<SoSanhViewModel>> SoSanhAsync(IReadOnlyList<Guid> maDoiTuong, Guid? maNguoiXem, CancellationToken huy)
        => KetQua<SoSanhViewModel>.ThucHienAsync(async () =>
        {
            await current.KiemTraNguoiThucHienAsync(maNguoiXem, huy);
            var ids = maDoiTuong.Distinct().ToList();
            Rules.Check(ids.Count is >= 1 and <= 3, "Chọn từ 1 đến 3 đối tượng.");
            var rows = new List<UngVienViewModel>();
            foreach (var id in ids)
                rows.Add(await ChiTietAsync(id, huy));
            return new SoSanhViewModel
            {
                MaDoiTuong = ids,
                HoSo = rows.OfType<HoSoGiaSuCongKhaiViewModel>().ToList(),
                NhuCau = rows.OfType<NhuCauCongKhaiViewModel>().ToList()
            };
        });
    public async Task<(List<UngVienViewModel> Items, bool HasNeed)> GoiYAsync()
    {
        var me = await current.HienTaiAsync();
        var own = await db.Set<NhuCauHoc>().AsNoTracking().Include(x => x.MonHocs).Where(x => x.ChuSoHuuId == me.Id && x.LoaiNhuCau == "TimGiaSu" && x.TrangThai == "DangMo").ToListAsync();
        if (me.LoaiNguoi == "GiaSu")
        {
            var subjects = await (from d in db.Set<HoSoGiaSuMonDay>() join h in db.Set<HoSoGiaSu>() on d.HoSoGiaSuId equals h.Id where h.GiaSuId == me.Id select d.MonHocId).ToListAsync();
            if (subjects.Count == 0)
                return ([], false);
            var q = NhuCauCongKhai().Where(x => subjects.Contains(x.MonHocId) || db.Set<NhuCauMonHoc>().Any(d => d.NhuCauId == x.Id && subjects.Contains(d.MonHocId)));
            var rows = await q.OrderByDescending(x => x.TaoLuc).Take(3).ToListAsync();
            await SlotsAsync(rows);
            foreach (var n in rows)
                n.LyDo = ["Cùng môn " + n.TenMon];
            return (rows, subjects.Count > 0);
        }
        if (own.Count == 0)
            return ([], false);
        var candidates = await HoSoRowsAsync(await HoSoCongKhai().ToListAsync());
        foreach (var c in candidates)
        {
            var match = own.FirstOrDefault(n => c.MonTheoHoSo.Any(m => n.CacMonHocId.Contains(m.MonHocId)));
            if (match != null)
            {
                c.LyDo.Add("Cùng môn " + c.MonTheoHoSo.First(m => match.CacMonHocId.Contains(m.MonHocId)).TenMon);
                if ((match.HinhThucHoc == "TrucTuyen" && c.DayTrucTuyen) || (match.HinhThucHoc == "TrucTiep" && c.DayTrucTiep))
                    c.LyDo.Add("Cùng hình thức học");
            }
        }
        return (candidates.Where(x => x.LyDo.Count > 0).OrderByDescending(x => x.LyDo.Count).ThenByDescending(x => x.TaoLuc).ThenBy(x => x.Id).Take(3).ToList(), own.Count > 0);
    }
}
