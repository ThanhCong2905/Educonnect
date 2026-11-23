using System.Data;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.NhuCau.Services;

public sealed partial class NhuCauService(AppDbContext db, NguoiDungHienTaiService current)
{
    public Task<List<MonHoc>> MonTenAsync() => db.Set<MonHoc>().AsNoTracking().ToListAsync();
    public async Task<List<NhuCauHoc>> LayAsync()
    {
        var u = await current.HienTaiAsync();
        return await db.Set<NhuCauHoc>().AsNoTracking().Include(x => x.MonHocs).Where(x => x.ChuSoHuuId == u.Id && x.LoaiNhuCau == "TimGiaSu").OrderByDescending(x => x.TaoLuc).ToListAsync();
    }
    public async Task<List<KhungGioRanh>> LichRanhAsync()
    {
        var user = await current.HienTaiAsync();
        return await (from slot in db.Set<KhungGioRanh>()
                      join need in db.Set<NhuCauHoc>() on slot.NhuCauId equals need.Id
                      where need.ChuSoHuuId == user.Id && need.LoaiNhuCau == "TimGiaSu"
                      orderby slot.ThuTrongTuan, slot.GioBatDau
                      select slot).AsNoTracking().ToListAsync();
    }
    public async Task<Guid> LuuAsync(NhuCauInput m)
    {
        var user = await current.HienTaiAsync();
        var input = TaoNhuCauBenHocViewModel.FromInput(m);
        var result = m.Id is Guid id
            ? await CapNhatAsync(id, input, input.RowVersion ?? [], user.Id, default)
            : await TaoNhuCauHocAsync(input, user.Id, default);
        return result.DuLieu;
    }
    public Task<KetQua<Guid>> TaoNhuCauHocAsync(TaoNhuCauBenHocViewModel duLieu, Guid maNguoiThucHien, CancellationToken huy)
        => KetQua<Guid>.ThucHienAsync(async () =>
        {
            var user = await current.KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
            Rules.Check(duLieu.MaNhuCau == null, "Tạo mới không được kèm mã nhu cầu đã tồn tại.");
            return await LuuDuLieuAsync(duLieu.ToInput(), user, huy);
        });

    public Task<KetQua<Guid>> CapNhatAsync(Guid maNhuCau, TaoNhuCauBenHocViewModel duLieu, byte[] rowVersion, Guid maNguoiThucHien, CancellationToken huy)
        => KetQua<Guid>.ThucHienAsync(async () =>
        {
            var user = await current.KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
            Rules.Check(maNhuCau != Guid.Empty && (duLieu.MaNhuCau == null || duLieu.MaNhuCau == maNhuCau), "Mã nhu cầu không hợp lệ.");
            Rules.Check(rowVersion.Length == 8, "Phiên bản không hợp lệ.");
            var input = duLieu.ToInput();
            input.Id = maNhuCau;
            input.RowVersion = Convert.ToBase64String(rowVersion);
            return await LuuDuLieuAsync(input, user, huy);
        });

    private async Task<Guid> LuuDuLieuAsync(NhuCauInput m, NguoiThamGia user, CancellationToken huy)
    {
        Rules.ValidateInput(m);
        Rules.Check(user.LoaiNguoi is "HocSinh" or "PhuHuynh", "Gia sư quản lý khả năng nhận dạy trong hồ sơ, không tạo hoặc sửa nhu cầu nhận lớp.", 403);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, huy);
        await KhoaTaiKhoanAsync(user.Id, huy);
        await GiaoDichNghiepVu.KiemTraTaiKhoanAsync(db, user.Id);
        Rules.Check(m.MaThaoTac != Guid.Empty, "Mã thao tác không hợp lệ. Hãy mở lại biểu mẫu.");
        if (!m.BanNhap)
        {
            var saved = await db.Set<LuotDangNhuCau>().AsNoTracking().SingleOrDefaultAsync(x => x.TaiKhoanId == user.Id && x.MaThaoTac == m.MaThaoTac, huy);
            if (saved != null)
            {
                Rules.Check(m.Id == saved.NhuCauId || (m.Id == null && saved.HanhDong == "Dang"), "Mã thao tác đã dùng cho nhu cầu khác.", 409);
                return saved.NhuCauId;
            }
        }
        var previous = m.Id == null ? null : await db.Set<NhuCauHoc>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == m.Id && x.ChuSoHuuId == user.Id, huy);
        var giuDong = previous != null && await DaCoLopDangHoatDongAsync(previous.Id, huy);
        if (!giuDong) await KiemTraHanMucAsync(user.Id, m.BanNhap, previous?.Id, huy);
        var posting = !giuDong && !m.BanNhap && previous?.TrangThai != "DangMo";
        if (posting) await KiemTraLuotDangAsync(user.Id, huy);
        Rules.Check(!m.ChonMonKhac && m.MonHocIds.Distinct().Count() <= 1, "Mỗi nhu cầu chỉ chọn một môn trong danh sách.", field: nameof(m.MonHocId));
        var subjects = new List<int> { m.MonHocIds.Count == 1 ? m.MonHocIds[0] : m.MonHocId };
        Rules.Check(await db.Set<MonHoc>().CountAsync(x => subjects.Contains(x.Id) && x.DangSuDung, huy) == subjects.Count, "Môn học không hợp lệ.");
        m.MonHocIds = subjects;
        m.MonHocId = subjects[0];
        if (user.LoaiNguoi == "HocSinh")
        {
            m.TenNguoiHoc = null;
            m.TuoiNguoiHoc = null;
        }
        if (!m.BanNhap && !giuDong) await KiemTraTrungAsync(m, user.Id, huy);
        var id = await LuuMotAsync(m, user, huy, giuDong);
        if (posting) GhiLuotDang(id, user.Id, previous == null ? "Dang" : "MoLai", m.MaThaoTac);
        var links = await db.Set<NhuCauMonHoc>().Where(x => x.NhuCauId == id).ToListAsync(huy);
        db.RemoveRange(links.Where(x => !subjects.Contains(x.MonHocId)));
        await db.SaveChangesAsync(huy);
        var existing = links.Select(x => x.MonHocId).ToHashSet();
        db.AddRange(subjects.Where(subject => !existing.Contains(subject))
            .Select(subject => new NhuCauMonHoc { NhuCauId = id, MonHocId = subject }));
        await db.SaveChangesAsync(huy);
        await tx.CommitAsync(huy);
        return id;
    }
    public Task<List<MonHoc>> MonAsync() => db.Set<MonHoc>().AsNoTracking().Where(x => x.DangSuDung && MonHocCatalog.PhoThong.Contains(x.TenMon)).GroupBy(x => x.TenMon).Select(g => g.OrderBy(x => x.Id).First()).ToListAsync();
    public async Task<List<MonHoc>> MonDuocChonAsync()
    {
        var me = await current.HienTaiAsync();
        if (me.LoaiNguoi != "GiaSu")
            return await MonAsync();
        return await (from m in db.Set<MonHoc>() join s in db.Set<HoSoGiaSuMonDay>() on m.Id equals s.MonHocId join h in db.Set<HoSoGiaSu>() on s.HoSoGiaSuId equals h.Id where h.GiaSuId == me.Id && m.DangSuDung select m).AsNoTracking().ToListAsync();
    }
    public async Task<HashSet<Guid>> DaKetNoiAsync()
    {
        var me = await current.HienTaiAsync();
        return (await (from n in db.Set<NhuCauHoc>() join r in db.Set<YeuCauKetNoiLopHoc>() on n.Id equals r.NhuCauId join c in db.Set<LopHoc>() on r.Id equals c.YeuCauKetNoiId where n.ChuSoHuuId == me.Id && c.TrangThai == "DangHoatDong" select n.Id).Distinct().ToListAsync()).ToHashSet();
    }
    public Task<KetQua<NhuCauInput>> LayDeSuaAsync(Guid id)
        => KetQua<NhuCauInput>.ThucHienAsync(async () =>
    {
        Rules.Check(id != Guid.Empty, "Không tìm thấy nhu cầu của bạn.", 404);
        Rules.Check((await current.HienTaiAsync()).LoaiNguoi is "HocSinh" or "PhuHuynh", "Mở Hồ sơ của tôi để quản lý môn dạy.", 403);
        var me = await current.HienTaiAsync();
        var n = await db.Set<NhuCauHoc>().AsNoTracking().Include(x => x.MonHocs).SingleOrDefaultAsync(x => x.Id == id && x.ChuSoHuuId == me.Id) ?? throw new LoiNghiepVu("Không tìm thấy nhu cầu của bạn.", 404);
        return new()
        {
            Id = n.Id,
            RowVersion = Convert.ToBase64String(n.RowVersion),
            MonHocId = n.MonHocId,
            MonHocIds = n.CacMonHocId.ToList(),
            ChonMonKhac = !await db.Set<MonHoc>().AnyAsync(x => x.Id == n.MonHocId && MonHocCatalog.PhoThong.Contains(x.TenMon)),
            TenMonKhac = await db.Set<MonHoc>().Where(x => x.Id == n.MonHocId).Select(x => x.TenMon).SingleAsync(),
            KhoiLop = n.KhoiLop,
            PhanKhoaHoc = n.PhanKhoaHoc,
            MucTieu = n.MucTieu,
            SoBuoiTuan = n.SoBuoiTuan,
            HinhThucHoc = n.HinhThucHoc,
            KhuVuc = n.KhuVuc,
            PhuongXa = n.PhuongXa,
            DiaChiChiTiet = n.DiaChiChiTiet,
            HocPhiDuKien = n.HocPhiDuKien,
            DonViHocPhi = n.DonViHocPhi,
            TenNguoiHoc = n.TenNguoiHoc,
            TuoiNguoiHoc = n.TuoiNguoiHoc,
            BanNhap = n.TrangThai == "BanNhap",
            LichRanh = await db.Set<KhungGioRanh>().Where(x => x.NhuCauId == id).Select(x => new SlotInput { ThuTrongTuan = x.ThuTrongTuan, GioBatDau = x.GioBatDau, GioKetThuc = x.GioKetThuc }).ToListAsync()
        };
    });
    private async Task KiemTraDuLieuAsync(NhuCauInput m, NguoiThamGia u, CancellationToken huy, bool kiemTraTrungLich = true)
    {
        Rules.ValidateInput(m);
        try { Rules.FeeCheck(m.HocPhiDuKien, m.DonViHocPhi); }
        catch (LoiNghiepVu error) { throw new LoiNghiepVu(error.Message, error.Status) { Truong = string.IsNullOrEmpty(m.DonViHocPhi) ? nameof(m.DonViHocPhi) : nameof(m.HocPhiDuKien) }; }
        Rules.Check(string.IsNullOrWhiteSpace(m.KhoiLop) || Enumerable.Range(1,12).Any(grade => m.KhoiLop == $"Lớp {grade}"), "Chọn khối lớp từ lớp 1 đến lớp 12.", field: nameof(m.KhoiLop));
        Rules.Check(!m.CoHocTrucTiep || string.IsNullOrWhiteSpace(m.KhuVuc) || HoSoLuaChon.KhuVuc.Contains(m.KhuVuc), "Chọn tỉnh / thành phố trong danh sách.", field: nameof(m.KhuVuc));
        Rules.Check(!m.CoHocTrucTiep || DiaGioiHanhChinh.HopLe(m.KhuVuc, m.PhuongXa), "Chọn phường / xã thuộc tỉnh / thành phố đã chọn.", field: nameof(m.PhuongXa));
        if (!m.BanNhap || m.LichRanh.Count > 0)
        {
            try { Rules.Slots(m.LichRanh); }
            catch (LoiNghiepVu error) { throw new LoiNghiepVu(error.Message, error.Status) { Truong = nameof(m.LichRanh) }; }
        }
        var subject = await db.Set<MonHoc>().SingleOrDefaultAsync(x => x.Id == m.MonHocId && x.DangSuDung, huy)
            ?? throw new LoiNghiepVu("Môn học không hợp lệ.") { Truong = nameof(m.MonHocId) };
        var grade = PhamViMonHoc.DocKhoi(m.KhoiLop);
        var (from, to) = PhamViMonHoc.Khoi(subject.TenMon);
        Rules.Check(m.BanNhap || grade != 0 || (from, to) == (1, 12),
            "Chọn khối lớp cho môn này.", field: nameof(m.KhoiLop));
        Rules.Check(grade == 0 || grade >= from && grade <= to,
            $"{subject.TenMon} chỉ áp dụng cho lớp {from}–{to}.", field: nameof(m.MonHocId));
        var integrated = PhamViMonHoc.LaKhoaHocTuNhien(subject.TenMon);
        Rules.Check(integrated ? m.BanNhap || grade != 0 && m.PhanKhoaHoc is >= 1 and <= 7 : m.PhanKhoaHoc == 0,
            "Chọn khối lớp và ít nhất một phần kiến thức cho môn Khoa học tự nhiên.", field: nameof(m.PhanKhoaHoc));
        if (!m.BanNhap)
        {
            if (kiemTraTrungLich)
            {
                var otherSlots = await (from slot in db.Set<KhungGioRanh>()
                    join need in db.Set<NhuCauHoc>() on slot.NhuCauId equals need.Id
                    where need.ChuSoHuuId == u.Id && need.TrangThai == "DangMo" && need.Id != m.Id
                    select slot).ToListAsync(huy);
                Rules.Check(!m.LichRanh.Any(slot => otherSlots.Any(other => other.ThuTrongTuan == slot.ThuTrongTuan
                    && slot.GioBatDau < other.GioKetThuc && other.GioBatDau < slot.GioKetThuc)),
                    "Khung giờ này trùng với một nhu cầu đang mở của bạn. Hãy chọn giờ khác hoặc đóng nhu cầu cũ.", field: nameof(m.LichRanh));
                await KiemTraLichLopAsync(m, u.Id, huy);
            }
            Rules.Check(!string.IsNullOrWhiteSpace(m.MucTieu), "Nhập mục tiêu học tập.", field: nameof(m.MucTieu));
            Rules.Check(m.SoBuoiTuan is >= 1 and <= 7, "Số buổi mỗi tuần phải từ 1 đến 7.", field: nameof(m.SoBuoiTuan));
            Rules.Check(!m.CoHocTrucTiep || !string.IsNullOrWhiteSpace(m.KhuVuc), "Chọn khu vực học trực tiếp.", field: nameof(m.KhuVuc));
            if (u.LoaiNguoi == "PhuHuynh")
            {
                Rules.Check(!string.IsNullOrWhiteSpace(m.TenNguoiHoc), "Nhập tên người học.", field: nameof(m.TenNguoiHoc));
                Rules.Check(m.TuoiNguoiHoc is >= 1 and <= 120 || !string.IsNullOrWhiteSpace(m.KhoiLop),
                    "Nhập tuổi hoặc chọn khối lớp của người học.", field: nameof(m.TuoiNguoiHoc));
            }
        }

    }

    private async Task<Guid> LuuMotAsync(NhuCauInput m, NguoiThamGia u, CancellationToken huy, bool giuDong = false)
    {
        await KiemTraDuLieuAsync(m, u, huy, !giuDong);

        var n = m.Id == null ? new NhuCauHoc { Id = Guid.NewGuid(), ChuSoHuuId = u.Id, LoaiNhuCau = "TimGiaSu", TaoLuc = DateTimeOffset.UtcNow } : await db.Set<NhuCauHoc>().SingleOrDefaultAsync(x => x.Id == m.Id && x.ChuSoHuuId == u.Id, huy) ?? throw new LoiNghiepVu("Nhu cầu không thuộc về bạn.", 403);
        if (m.Id == null)
            db.Add(n);
        else
        {
            Rules.Concurrency(db, n, m.RowVersion);
            db.Entry(n).Property(x => x.MucTieu).IsModified = true;
        }
        n.MonHocId = m.MonHocId;
        n.KhoiLop = m.KhoiLop;
        n.PhanKhoaHoc = m.PhanKhoaHoc;
        n.MucTieu = m.MucTieu?.Trim();
        n.SoBuoiTuan = m.SoBuoiTuan;
        n.HinhThucHoc = m.HinhThucHoc;
        n.KhuVuc = m.CoHocTrucTiep ? m.KhuVuc : null;
        n.PhuongXa = m.CoHocTrucTiep && !string.IsNullOrWhiteSpace(m.PhuongXa) ? m.PhuongXa.Trim() : null;
        n.DiaChiChiTiet = m.CoHocTrucTiep ? m.DiaChiChiTiet : null;
        n.HocPhiDuKien = m.HocPhiDuKien;
        n.DonViHocPhi = m.DonViHocPhi;
        n.TenNguoiHoc = u.LoaiNguoi == "PhuHuynh" ? m.TenNguoiHoc : null;
        n.TuoiNguoiHoc = u.LoaiNguoi == "PhuHuynh" ? m.TuoiNguoiHoc : null;
        n.TrangThai = giuDong ? "DaDong" : m.BanNhap ? "BanNhap" : "DangMo";
        if (giuDong) n.LyDoDong = "DaKetNoi";
        if (!m.BanNhap && !giuDong)
        {
            n.XacNhanConNhuCauLuc = DateTimeOffset.UtcNow;
            n.NhacXacNhanLuc = null;
            n.LyDoDong = null;
        }
        await db.SaveChangesAsync(huy);
        await db.Set<KhungGioRanh>().Where(x => x.NhuCauId == n.Id).ExecuteDeleteAsync(huy);
        foreach (var s in m.LichRanh)
            db.Add(new KhungGioRanh { Id = Guid.NewGuid(), NhuCauId = n.Id, ThuTrongTuan = s.ThuTrongTuan, GioBatDau = s.GioBatDau, GioKetThuc = s.GioKetThuc });
        await db.SaveChangesAsync(huy);
        return n.Id;
    }
    public async Task TrangThaiAsync(Guid id, string state, string version)
    {
        Rules.Check(state is "DangMo" or "DaDong", "Trạng thái không hợp lệ.");
        var u = await current.HienTaiAsync();
        Rules.Check(u.LoaiNguoi is "HocSinh" or "PhuHuynh", "Gia sư quản lý khả năng nhận dạy trong hồ sơ.", 403);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await KhoaTaiKhoanAsync(u.Id);
        await GiaoDichNghiepVu.KiemTraTaiKhoanAsync(db, u.Id);
        var n = await db.Set<NhuCauHoc>().SingleOrDefaultAsync(x => x.Id == id && x.ChuSoHuuId == u.Id) ?? throw new LoiNghiepVu("Nhu cầu không thuộc về bạn.", 403);
        Rules.Concurrency(db, n, version);
        if (state == "DangMo")
        {
            Rules.Check(!await DaCoLopDangHoatDongAsync(id), "Nhu cầu đã có lớp đang hoạt động. Bạn có thể chỉnh sửa nhưng chưa thể mở lại.", 409);
            await KiemTraHanMucAsync(u.Id, false, id);
            await KiemTraLuotDangAsync(u.Id);
            Rules.Check(n.TrangThai == "DaDong", "Hãy hoàn thiện bản nháp trước khi mở.");
            Rules.FeeCheck(n.HocPhiDuKien, n.DonViHocPhi);
            var input = (await LayDeSuaAsync(id)).DuLieu;
            input.BanNhap = false;
            await KiemTraDuLieuAsync(input, u, default);
            Rules.Check(await db.Set<MonHoc>().CountAsync(subject => input.MonHocIds.Contains(subject.Id) && subject.DangSuDung) == input.MonHocIds.Count, "Môn học không hợp lệ hoặc đã ngừng sử dụng.", field: "MonHocIds");
            await KiemTraTrungAsync(input, u.Id, default);
        }
        n.TrangThai = state;
        if (state == "DangMo")
        {
            GhiLuotDang(id, u.Id, "MoLai");
            n.XacNhanConNhuCauLuc = DateTimeOffset.UtcNow;
            n.NhacXacNhanLuc = null;
            n.LyDoDong = null;
        }
        else n.LyDoDong = "ChuNhuCauDong";
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
