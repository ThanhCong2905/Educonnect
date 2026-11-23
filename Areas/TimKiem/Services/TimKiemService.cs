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
    internal static IQueryable<UngVienViewModel> NhuCauCongKhai(AppDbContext db) => from n in db.Set<NhuCauHoc>()
                                                             join u in db.Set<NguoiThamGia>() on n.ChuSoHuuId equals u.Id
                                                             join account in db.Users on u.Id equals account.Id
                                                             join m in db.Set<MonHoc>() on n.MonHocId equals m.Id
                                                             where n.LoaiNhuCau == "TimGiaSu" && n.TrangThai == "DangMo" && !db.Set<BienPhapQuanTri>().Any(b => b.LoaiDoiTuong == "NhuCauHoc" && b.DoiTuongId == n.Id && b.GoLuc == null && b.Loai == "AnNoiDung") && (u.LoaiNguoi == "HocSinh" || u.LoaiNguoi == "PhuHuynh")
                                                             select new NhuCauCongKhaiViewModel { AnhDaiDienUrl = account.AnhDaiDien == null ? null : "/TaiKhoan/Anh/" + account.Id, Id = n.Id, ChuSoHuuId = u.Id, HoTen = u.HoTen, LoaiNguoi = u.LoaiNguoi, TenMon = m.TenMon, MonHocId = m.Id, KhoiLop = n.KhoiLop, PhanKhoaHoc = n.PhanKhoaHoc, MucTieu = n.MucTieu, HinhThucHoc = n.HinhThucHoc, KhuVuc = n.KhuVuc, PhuongXa = n.PhuongXa, HocPhiDuKien = n.HocPhiDuKien, DonViHocPhi = n.DonViHocPhi, TaoLuc = n.TaoLuc };
    private async Task ApDungTenMonThuongGoiAsync(BoLocTimKiemViewModel filter, CancellationToken huy)
    {
        if (filter.MonHocId.HasValue || filter.MonHocIds.Count != 0 || filter.PhanKhoaHoc.HasValue || filter.TuKhoa is null) return;
        var keyword = filter.TuKhoa.Trim().ToLowerInvariant().Replace("hoá", "hóa");
        var separator = keyword.LastIndexOf(" lớp ", StringComparison.Ordinal);
        if (separator < 0 || !int.TryParse(keyword.AsSpan(separator + 5), out var grade) || grade is < 6 or > 12) return;
        if (filter.KhoiLop != null && PhamViMonHoc.DocKhoi(filter.KhoiLop) != grade) return;
        var part = keyword[..separator] switch
        {
            "lý" or "lí" or "vật lý" or "vật lí" => 1,
            "hóa" or "hóa học" => 2,
            "sinh" or "sinh học" => 3,
            _ => 0
        };
        if (part == 0) return;
        var candidates = await db.Set<MonHoc>().Where(x => x.DangSuDung).OrderBy(x => x.Id).ToListAsync(huy);
        var subject = candidates.FirstOrDefault(x => grade <= 9 ? PhamViMonHoc.LaKhoaHocTuNhien(x.TenMon) : PhamViMonHoc.Khoi(x.TenMon) == (10, 12)
            && (x.TenMon.Equals(PhamViMonHoc.TenPhan[part - 1], StringComparison.OrdinalIgnoreCase)
                || part == 1 && x.TenMon.Equals("Vật Lý", StringComparison.OrdinalIgnoreCase)));
        if (subject == null) return;
        filter.MonHocId = subject.Id;
        filter.KhoiLop = $"Lớp {grade}";
        filter.PhanKhoaHoc = grade <= 9 ? part : null;
        filter.TuKhoa = null;
    }

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
        if (!string.IsNullOrWhiteSpace(f.PhuongXa))
            q = q.Where(h => h.PhuongXa == f.PhuongXa);
        if (f.HinhThucHoc == "TrucTuyen")
            q = q.Where(h => h.DayTrucTuyen);
        if (f.HinhThucHoc == "TrucTiep")
            q = q.Where(h => h.DayTrucTiep);
        var ids = f.MonHocIds.Concat(f.MonHocId.HasValue ? [f.MonHocId.Value] : Array.Empty<int>()).Distinct().ToList();
        var grade = PhamViMonHoc.DocKhoi(f.KhoiLop);
        var gradeMask = grade == 0 ? 0L : PhamViMonHoc.BitKhoi(grade);
        var partMask = PhamViMonHoc.BoLocPhan(grade, f.PhanKhoaHoc ?? 0);
        var byPrice = f.SapXep is "HocPhiTang" or "HocPhiGiam";
        return q.Where(h => db.Set<HoSoGiaSuMonDay>().Any(d => d.HoSoGiaSuId == h.Id
            && (ids.Count == 0 || ids.Contains(d.MonHocId))
            && (gradeMask == 0 || d.MatNaKhoi != null && (d.MatNaKhoi.Value & gradeMask) != 0
                || d.MatNaKhoi == null && (", " + d.PhamViDay + ",").Contains(", " + f.KhoiLop + ","))
            && (partMask == 0 || (d.PhanKhoaHoc & partMask) != 0)
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
            var me = await current.NguoiXemAsync(maNguoiXem, huy);
            return await TimDuLieuAsync(boLoc, me, huy);
        });

    private async Task<Page<UngVienViewModel>> TimDuLieuAsync(BoLocTimKiemViewModel f, NguoiThamGia? me, CancellationToken huy)
    {
        Rules.ValidateInput(f);
        var needs = f.LoaiDoiTuong == "NhuCau" || f.LoaiDoiTuong == null && me?.LoaiNguoi == "GiaSu";
        Rules.Check(!needs || me == null || me.LoaiNguoi == "GiaSu", "Tài khoản này sử dụng chức năng tìm gia sư.", 403);
        Rules.Check(needs || me?.LoaiNguoi != "GiaSu", "Tài khoản gia sư sử dụng chức năng tìm nhu cầu học.", 403);
        f.LoaiDoiTuong = needs ? "NhuCau" : "GiaSu";
        await ApDungTenMonThuongGoiAsync(f, huy);
        var grade = PhamViMonHoc.DocKhoi(f.KhoiLop);
        Rules.Check(string.IsNullOrWhiteSpace(f.KhoiLop) || grade != 0, "Chọn khối lớp trong danh sách.", field: nameof(f.KhoiLop));
        if (f.MonHocId is int subjectId)
        {
            var subjectName = await db.Set<MonHoc>().Where(x => x.Id == subjectId && x.DangSuDung).Select(x => x.TenMon).SingleOrDefaultAsync(huy);
            Rules.Check(subjectName != null, "Môn học không khả dụng.", field: nameof(f.MonHocId));
            var (from, to) = PhamViMonHoc.Khoi(subjectName!);
            Rules.Check(grade == 0 || grade >= from && grade <= to, "Môn học không phù hợp với khối lớp.", field: nameof(f.MonHocId));
            Rules.Check(f.PhanKhoaHoc == null || PhamViMonHoc.LaKhoaHocTuNhien(subjectName!), "Bộ lọc phần kiến thức áp dụng cho Khoa học tự nhiên.", field: nameof(f.PhanKhoaHoc));
        }
        else Rules.Check(f.PhanKhoaHoc == null, "Chọn môn Khoa học tự nhiên trước.", field: nameof(f.PhanKhoaHoc));
        Rules.Check(DiaGioiHanhChinh.HopLe(f.KhuVuc, f.PhuongXa), "Chọn phường / xã thuộc tỉnh / thành phố đã chọn.", field: nameof(f.PhuongXa));
        Rules.Check(f.Trang is >= 1 and <= 100000, "Trang không hợp lệ.");
        Rules.Check(f.HocPhiToiDa == null || (f.HocPhiToiDa >= 0 && f.HocPhiToiDa <= 9999999999999999m && decimal.Truncate(f.HocPhiToiDa.Value) == f.HocPhiToiDa), "Học phí tối đa phải là số VND nguyên không âm.", field: nameof(f.HocPhiToiDa));
        Rules.Check(f.DonViHocPhi is "VND/Gio" or "VND/Buoi", "Chọn đúng đơn vị học phí.");
        Rules.Check(!needs || f.SapXep is not ("DanhGia" or "HocVien"), "Sắp xếp theo đánh giá và số bên học chỉ áp dụng danh sách gia sư.", field: nameof(f.SapXep));
        var personal = f.MucPhuHop.HasValue || f.SapXep == "PhuHop";
        Rules.Check(!personal || me?.LoaiNguoi is "GiaSu" or "HocSinh" or "PhuHuynh", "Đăng nhập để lọc theo độ phù hợp.", 403);
        Rules.Check(!needs || !f.SoSaoToiThieu.HasValue || me?.LoaiNguoi == "GiaSu",
            "Đánh giá người đăng chỉ hiển thị với gia sư.", 403);
        var matches = personal ? await DoPhuHopAsync(me!, huy) : new Dictionary<Guid, int>();
        var minimum = f.MucPhuHop ?? 1;
        var matching = matches.Where(x => x.Value >= minimum).Select(x => x.Key).ToArray();
        var rank4 = matches.Where(x => x.Value == 4).Select(x => x.Key).ToArray();
        var rank3 = matches.Where(x => x.Value == 3).Select(x => x.Key).ToArray();
        var rank2 = matches.Where(x => x.Value == 2).Select(x => x.Key).ToArray();
        var publicReviews = EduConnect.Mvc.Areas.DanhGia.Services.DanhGiaDoiTacService.CongKhai(db);
        var stars = f.SoSaoToiThieu;
        if (!needs)
        {
            var q = LocHoSo(f);
            if (personal) q = q.Where(h => matching.Contains(h.Id));
            if (stars.HasValue)
                q = q.Where(h => publicReviews.Where(r => r.NguoiNhanId == h.GiaSuId).Average(r => (decimal?)r.SoSao) >= stars.Value);
            var total = await q.CountAsync(huy);
            var ids = f.MonHocIds.Concat(f.MonHocId.HasValue ? [f.MonHocId.Value] : Array.Empty<int>()).ToList();
            var gradeMask = grade == 0 ? 0L : PhamViMonHoc.BitKhoi(grade);
            var partMask = PhamViMonHoc.BoLocPhan(grade, f.PhanKhoaHoc ?? 0);
            var reviews = EduConnect.Mvc.Areas.DanhGia.Services.DanhGiaDoiTacService.CongKhai(db);
            var priced = from h in q
                         select new
                         {
                             h,
                             Price = db.Set<HoSoGiaSuMonDay>().Where(d => d.HoSoGiaSuId == h.Id
                                 && (ids.Count == 0 || ids.Contains(d.MonHocId))
                                 && (gradeMask == 0 || d.MatNaKhoi != null && (d.MatNaKhoi.Value & gradeMask) != 0
                || d.MatNaKhoi == null && (", " + d.PhamViDay + ",").Contains(", " + f.KhoiLop + ","))
            && (partMask == 0 || (d.PhanKhoaHoc & partMask) != 0)
                                 && d.DonViHocPhi == f.DonViHocPhi
                                 && (f.HocPhiToiDa == null || d.HocPhiDeXuat <= f.HocPhiToiDa)).Min(d => d.HocPhiDeXuat),
                             Name = db.Set<NguoiThamGia>().Where(u => u.Id == h.GiaSuId).Select(u => u.HoTen).First(),
                             Rating = reviews.Where(review => review.NguoiNhanId == h.GiaSuId).Select(review => (decimal?)review.SoSao).Average(),
                             ReviewCount = reviews.Count(review => review.NguoiNhanId == h.GiaSuId),
                             LearnerCount = db.Set<LopHoc>().Where(lop => lop.GiaSuId == h.GiaSuId && lop.TrangThai != "DaHuy").Select(lop => lop.BenHocId).Distinct().Count()
                         };
            var ordered = f.SapXep switch
            {
                "HocPhiTang" => priced.OrderBy(x => x.Price).ThenBy(x => x.h.Id),
                "HocPhiGiam" => priced.OrderByDescending(x => x.Price).ThenBy(x => x.h.Id),
                "Ten" => priced.OrderBy(x => x.Name).ThenBy(x => x.h.Id),
                "DanhGia" => priced.OrderByDescending(x => x.Rating ?? 0).ThenByDescending(x => x.ReviewCount).ThenBy(x => x.h.Id),
                "HocVien" => priced.OrderByDescending(x => x.LearnerCount).ThenBy(x => x.h.Id),
                "PhuHop" => priced.OrderByDescending(x => rank4.Contains(x.h.Id) ? 4 : rank3.Contains(x.h.Id) ? 3 : rank2.Contains(x.h.Id) ? 2 : 1)
                    .ThenByDescending(x => x.h.DuyetLuc).ThenBy(x => x.h.Id),
                _ => priced.OrderByDescending(x => x.h.DuyetLuc).ThenBy(x => x.h.Id)
            };
            var rows = await HoSoRowsAsync(await ordered.Skip((f.Trang - 1) * 9).Take(9).Select(x => x.h).ToListAsync(huy), huy);
            GanDoPhuHop(rows, matches);
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
        var n = NhuCauCongKhai(db);
        if (me != null) n = n.Where(x => x.ChuSoHuuId != me.Id);
        if (!string.IsNullOrWhiteSpace(f.TuKhoa))
            n = n.Where(x => x.HoTen.Contains(f.TuKhoa) || (x.TenMon.Contains(f.TuKhoa) || db.Set<NhuCauMonHoc>().Any(d => d.NhuCauId == x.Id && db.Set<MonHoc>().Any(m => m.Id == d.MonHocId && m.TenMon.Contains(f.TuKhoa)))) || (x.MucTieu != null && x.MucTieu.Contains(f.TuKhoa)));
        var subjectIds = f.MonHocIds.Concat(f.MonHocId.HasValue ? [f.MonHocId.Value] : Array.Empty<int>()).Distinct().ToList();
        if (subjectIds.Count > 0)
            n = n.Where(x => subjectIds.Contains(x.MonHocId) || db.Set<NhuCauMonHoc>().Any(d => d.NhuCauId == x.Id && subjectIds.Contains(d.MonHocId)));
        if (!string.IsNullOrWhiteSpace(f.KhoiLop))
            n = n.Where(x => x.KhoiLop == f.KhoiLop);
        if (f.PhanKhoaHoc is int part)
        {
            var partMask = 1 << (part - 1);
            n = n.Where(x => (x.PhanKhoaHoc & partMask) != 0);
        }
        if (!string.IsNullOrWhiteSpace(f.HinhThucHoc))
            n = n.Where(x => x.HinhThucHoc == f.HinhThucHoc || x.HinhThucHoc == "CaHai");
        if (!string.IsNullOrWhiteSpace(f.KhuVuc))
            n = n.Where(x => x.KhuVuc != null && x.KhuVuc.Contains(f.KhuVuc));
        if (!string.IsNullOrWhiteSpace(f.PhuongXa))
            n = n.Where(x => x.PhuongXa == f.PhuongXa);
        if (f.HocPhiToiDa != null)
            n = n.Where(x => x.HocPhiDuKien <= f.HocPhiToiDa && x.DonViHocPhi == f.DonViHocPhi);
        if (f.ThuTrongTuan != null || f.GioBatDau != null || f.GioKetThuc != null)
            n = n.Where(x => db.Set<KhungGioRanh>().Any(s => s.NhuCauId == x.Id && (f.ThuTrongTuan == null || s.ThuTrongTuan == f.ThuTrongTuan) && (f.GioBatDau == null || s.GioBatDau <= f.GioBatDau) && (f.GioKetThuc == null || s.GioKetThuc >= f.GioKetThuc)));
        if (f.SapXep is "HocPhiTang" or "HocPhiGiam")
            n = n.Where(x => x.HocPhiDuKien != null && x.DonViHocPhi == f.DonViHocPhi);
        if (personal) n = n.Where(x => matching.Contains(x.Id));
        if (stars.HasValue)
            n = n.Where(x => publicReviews.Where(r => r.NguoiNhanId == x.ChuSoHuuId).Average(r => (decimal?)r.SoSao) >= stars.Value);
        var count = await n.CountAsync(huy);
        var sorted = f.SapXep switch
        {
            "HocPhiTang" => n.OrderBy(x => x.HocPhiDuKien).ThenBy(x => x.Id),
            "HocPhiGiam" => n.OrderByDescending(x => x.HocPhiDuKien).ThenBy(x => x.Id),
            "Ten" => n.OrderBy(x => x.HoTen).ThenBy(x => x.Id),
            "PhuHop" => n.OrderByDescending(x => rank4.Contains(x.Id) ? 4 : rank3.Contains(x.Id) ? 3 : rank2.Contains(x.Id) ? 2 : 1)
                .ThenByDescending(x => x.TaoLuc).ThenBy(x => x.Id),
            _ => n.OrderByDescending(x => x.TaoLuc).ThenBy(x => x.Id)
        };
        var items = await sorted.Skip((f.Trang - 1) * 9).Take(9).ToListAsync(huy);
        await SlotsAsync(items, huy);
        GanDoPhuHop(items, matches);
        if (me?.LoaiNguoi == "GiaSu")
            await DiemNguoiDangAsync(items, huy);
        return new()
        {
            Trang = f.Trang,
            Items = items,
            Total = count
        };
    }
    private async Task DiemNguoiDangAsync(List<UngVienViewModel> rows, CancellationToken huy = default)
    {
        if (rows.Count == 0) return;
        var owners = rows.Select(row => row.ChuSoHuuId).Distinct().ToList();
        var ratings = await EduConnect.Mvc.Areas.DanhGia.Services.DanhGiaDoiTacService.CongKhai(db)
            .Where(review => owners.Contains(review.NguoiNhanId))
            .GroupBy(review => review.NguoiNhanId)
            .Select(group => new { Id = group.Key, Rating = group.Average(review => (decimal)review.SoSao), Count = group.Count() })
            .ToDictionaryAsync(group => group.Id, huy);
        foreach (var row in rows)
        {
            if (!ratings.TryGetValue(row.ChuSoHuuId, out var rating)) continue;
            row.DiemDanhGia = rating.Rating;
            row.SoDanhGia = rating.Count;
        }
    }

    private async Task SlotsAsync(List<UngVienViewModel> rows, CancellationToken huy = default)
    {
        var ids = rows.Select(x => x.Id).ToList();
        var subjects = await (from link in db.Set<NhuCauMonHoc>()
                              join subject in db.Set<MonHoc>() on link.MonHocId equals subject.Id
                              where ids.Contains(link.NhuCauId)
                              orderby subject.Id
                              select new
                              {
                                  link.NhuCauId,
                                  subject.Id,
                                  subject.TenMon
                              }).ToListAsync(huy);
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
        var viewer = await current.NguoiXemAsync(null, huy);
        var n = await NhuCauCongKhai(db).SingleOrDefaultAsync(x => x.Id == id, huy) ?? throw new LoiNghiepVu("Hồ sơ hoặc nhu cầu đang ẩn, chưa được duyệt hoặc không còn công khai.", 404);
        Rules.Check(viewer == null || viewer.LoaiNguoi == "GiaSu" || viewer.Id == n.ChuSoHuuId, "Nhu cầu học dành cho gia sư và người đăng.", 403);
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
    public async Task<List<UngVienViewModel>> LienQuanAsync(Guid owner, Guid? boQuaId = null)
    {
        var rows = await NhuCauCongKhai(db).Where(x => x.ChuSoHuuId == owner && x.Id != boQuaId).OrderByDescending(x => x.TaoLuc).ToListAsync();
        await SlotsAsync(rows);
        if ((await current.NguoiXemAsync())?.LoaiNguoi == "GiaSu")
            await DiemNguoiDangAsync(rows);
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
            await current.NguoiXemAsync(maNguoiXem, huy);
            var ids = maDoiTuong.Distinct().ToList();
            Rules.Check(ids.Count is >= 2 and <= 3, "Chọn từ 2 đến 3 đối tượng để so sánh.");
            var rows = new List<UngVienViewModel>();
            foreach (var id in ids)
                rows.Add(await ChiTietAsync(id, huy));
            Rules.Check(rows.All(x => x is HoSoGiaSuCongKhaiViewModel) || rows.All(x => x is NhuCauCongKhaiViewModel),
                "Chỉ so sánh các đối tượng cùng loại.");
            return new SoSanhViewModel
            {
                MaDoiTuong = ids,
                HoSo = rows.OfType<HoSoGiaSuCongKhaiViewModel>().ToList(),
                NhuCau = rows.OfType<NhuCauCongKhaiViewModel>().ToList()
            };
        });
    private async Task<Dictionary<Guid, int>> DoPhuHopAsync(NguoiThamGia viewer, CancellationToken token)
    {
        var ranks = new Dictionary<Guid, int>();
        if (viewer.LoaiNguoi == "GiaSu")
        {
            var subjects = await (from profile in db.Set<HoSoGiaSu>()
                                  join subject in db.Set<HoSoGiaSuMonDay>() on profile.Id equals subject.HoSoGiaSuId
                                  where profile.GiaSuId == viewer.Id
                                  select new { subject, profile.KhuVuc, profile.PhuongXa, profile.DayTrucTiep })
                .AsNoTracking().ToListAsync(token);
            var taught = subjects.ToLookup(x => x.subject.MonHocId);
            var ids = taught.Select(x => x.Key).ToArray();
            if (ids.Length == 0) return ranks;
            var candidates = NhuCauCongKhai(db).Where(x => x.ChuSoHuuId != viewer.Id && ids.Contains(x.MonHocId))
                .Select(x => new { x.Id, x.MonHocId, x.KhoiLop, x.PhanKhoaHoc, x.HinhThucHoc, x.KhuVuc, x.PhuongXa });
            await foreach (var need in candidates.AsAsyncEnumerable().WithCancellation(token))
                ranks[need.Id] = taught[need.MonHocId].Max(x => XepMuc(need.KhoiLop, need.PhanKhoaHoc,
                    x.subject.PhamViDay, x.subject.MatNaKhoi, x.subject.PhanKhoaHoc, need.HinhThucHoc, x.DayTrucTiep,
                    need.KhuVuc, need.PhuongXa, x.KhuVuc, x.PhuongXa));
            return ranks;
        }

        var ownNeeds = await db.Set<NhuCauHoc>().AsNoTracking().Include(x => x.MonHocs)
            .Where(x => x.ChuSoHuuId == viewer.Id && x.LoaiNhuCau == "TimGiaSu" && x.TrangThai == "DangMo")
            .ToListAsync(token);
        var wanted = ownNeeds.SelectMany(need => need.CacMonHocId.Select(id => new { id, need })).ToLookup(x => x.id);
        var wantedIds = wanted.Select(x => x.Key).ToArray();
        if (wantedIds.Length == 0) return ranks;
        var tutors = from profile in HoSoCongKhai()
                     join subject in db.Set<HoSoGiaSuMonDay>() on profile.Id equals subject.HoSoGiaSuId
                     where wantedIds.Contains(subject.MonHocId)
                     select new { profile.Id, profile.KhuVuc, profile.PhuongXa, profile.DayTrucTiep, subject.MonHocId,
                         subject.PhamViDay, subject.MatNaKhoi, subject.PhanKhoaHoc };
        await foreach (var tutor in tutors.AsAsyncEnumerable().WithCancellation(token))
        {
            var rank = wanted[tutor.MonHocId].Max(x => XepMuc(x.need.KhoiLop, x.need.PhanKhoaHoc,
                tutor.PhamViDay, tutor.MatNaKhoi, tutor.PhanKhoaHoc, x.need.HinhThucHoc, tutor.DayTrucTiep,
                x.need.KhuVuc, x.need.PhuongXa, tutor.KhuVuc, tutor.PhuongXa));
            ranks[tutor.Id] = Math.Max(ranks.GetValueOrDefault(tutor.Id), rank);
        }
        return ranks;
    }

    private static int XepMuc(string? khoiLop, byte phanKhoaHoc, string phamViDay, long? matNaKhoi, long phanDay,
        string hinhThucHoc, bool dayTrucTiep, string? khuVucHoc, string? phuongHoc, string? khuVucDay, string? phuongDay)
    {
        var grade = PhamViMonHoc.DocKhoi(khoiLop);
        var gradeMatches = grade != 0 && (matNaKhoi.HasValue || HoSoLuaChon.LopDay(phamViDay).Contains(khoiLop)) &&
            PhamViMonHoc.PhuHop(grade, phanKhoaHoc, matNaKhoi, phanDay);
        if (!gradeMatches) return 1;
        if (hinhThucHoc is not ("TrucTiep" or "CaHai") || !dayTrucTiep || !CungDiaGioi(khuVucHoc, khuVucDay)) return 2;
        return CungDiaGioi(phuongHoc, phuongDay) ? 4 : 3;
    }

    private static bool CungDiaGioi(string? first, string? second)
        => !string.IsNullOrWhiteSpace(first) && string.Equals(first.Trim(), second?.Trim(), StringComparison.OrdinalIgnoreCase);

    private static void GanDoPhuHop(IEnumerable<UngVienViewModel> rows, IReadOnlyDictionary<Guid, int> matches)
    {
        foreach (var row in rows)
        {
            switch (matches.GetValueOrDefault(row.Id))
            {
                case 1: row.LyDo = ["Trùng môn"]; break;
                case 2: row.LyDo = ["Trùng môn và khối lớp"]; break;
                case 3: row.LyDo = ["Trùng môn, khối lớp và tỉnh/thành"]; break;
                case 4: row.LyDo = ["Trùng môn, khối lớp và phường/xã"]; break;
            }
        }
    }

    public async Task<(List<UngVienViewModel> Items, bool HasNeed)> GoiYAsync()
    {
        var viewer = await current.HienTaiAsync();
        var filter = new BoLocTimKiemViewModel { MucPhuHop = 1, SapXep = "PhuHop" };
        var page = await TimDuLieuAsync(filter, viewer, default);
        return (page.Items.Take(3).ToList(), page.Total > 0);
    }
}
