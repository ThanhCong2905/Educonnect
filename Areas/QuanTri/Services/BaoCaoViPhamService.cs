using System.Data;
using System.Text.Json;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.QuanTri.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.QuanTri.Services;

public sealed class BaoCaoViPhamService(AppDbContext db, NguoiDungHienTaiService current,
    QuyenLopHocService rights, TinNhanService chat, TepService files, QuyenQuanTriService admin, ThongBaoService notices)
{
    public Task<KetQua<Guid>> GuiAsync(TaoBaoCaoViewModel duLieu, Guid maNguoiThucHien, CancellationToken huy)
        => KetQua<Guid>.ThucHienAsync(async () =>
        {
            Rules.ValidateInput(duLieu);
            Rules.Check(duLieu.MaThaoTac != Guid.Empty, "Mã thao tác không hợp lệ.");
            var me = await current.KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
            var uploads = await files.KiemTraAsync(duLieu.Files);
            List<string> written = [];
            try
            {
                await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, huy);
                await db.Database.ExecuteSqlInterpolatedAsync($"SELECT Id FROM dbo.AspNetUsers WITH (UPDLOCK,HOLDLOCK) WHERE Id={me.Id}", huy);
                var saved = await db.Set<BaoCaoViPham>().AsNoTracking().SingleOrDefaultAsync(x => x.NguoiBaoCaoId == me.Id && x.MaThaoTac == duLieu.MaThaoTac, huy);
                if (saved != null)
                {
                    Rules.Check(saved.DoiTuongId == duLieu.DoiTuongId && saved.LoaiDoiTuong == duLieu.LoaiDoiTuong, "Mã thao tác đã được dùng cho báo cáo khác.", 409);
                    return saved.Id;
                }
                var (owner, snapshot) = await DoiTuongAsync(duLieu.LoaiDoiTuong, duLieu.DoiTuongId);
                Rules.Check(owner != me.Id, "Bạn không thể báo cáo nội dung của chính mình.");
                Rules.Check(!await db.Set<BaoCaoViPham>().AnyAsync(x => x.NguoiBaoCaoId == me.Id && x.LoaiDoiTuong == duLieu.LoaiDoiTuong && x.DoiTuongId == duLieu.DoiTuongId && x.TrangThai != "DaXuLy", huy), "Bạn đã có báo cáo đang chờ xử lý cho nội dung này.", 409);
                var report = new BaoCaoViPham
                {
                    NguoiBaoCaoId = me.Id,
                    NguoiBiBaoCaoId = owner,
                    LoaiDoiTuong = duLieu.LoaiDoiTuong,
                    DoiTuongId = duLieu.DoiTuongId,
                    LyDo = duLieu.LyDo.Trim(),
                    MoTa = duLieu.MoTa.Trim(),
                    BanChup = JsonSerializer.Serialize(snapshot),
                    MaThaoTac = duLieu.MaThaoTac,
                    GuiLuc = await GiaoDichNghiepVu.GioMayChuAsync(db, huy)
                };
                db.Add(report);
                await db.SaveChangesAsync(huy);
                written = await files.LuuAsync(uploads, baoCao: report.Id);
                var recipients = await (from c in db.UserClaims
                                        join u in db.Users on c.UserId equals u.Id
                                        where c.ClaimType == QuyenQuanTriService.ClaimType && c.ClaimValue == "XuLyBaoCao" && u.TrangThai == "HoatDong"
                                        select u.Id).Distinct().ToListAsync(huy);
                foreach (var recipient in recipients)
                    notices.Tao(recipient, "BaoCao", report.Id, "Có báo cáo vi phạm mới cần xem xét.");
                await db.SaveChangesAsync(huy);
                await tx.CommitAsync(huy);
                return report.Id;
            }
            catch { TepService.DonDep(written); throw; }
        });

    public async Task<Page<BaoCaoViPham>> LayAsync(int trang = 1, string? state = null, string? q = null, bool quanTri = false, Guid? nguoiGui = null)
    {
        Rules.Check(trang is >= 1 and <= 100000 && (q?.Length ?? 0) <= 100, "Trang hoặc từ khóa không hợp lệ.");
        Rules.Check(state == null || state is "ChoXuLy" or "DangXuLy" or "DaXuLy" ||
            (quanTri && state is ("ChuaXuLy" or "ToiDangXuLy")), "Trạng thái báo cáo không hợp lệ.");
        var me = await current.HienTaiAsync();
        if (quanTri)
            await admin.KiemTraAsync("XuLyBaoCao");
        var query = db.Set<BaoCaoViPham>().AsNoTracking();
        if (!quanTri)
            query = query.Where(report => report.NguoiBaoCaoId == me.Id);
        else if (nguoiGui is Guid reporterId)
            query = query.Where(report => report.NguoiBaoCaoId == reporterId);

        query = state switch
        {
            "ChuaXuLy" => query.Where(report => report.TrangThai != "DaXuLy"),
            "ToiDangXuLy" => query.Where(report => report.TrangThai == "DangXuLy" && report.NguoiXuLyId == me.Id),
            null => query,
            _ => query.Where(report => report.TrangThai == state)
        };
        if (!string.IsNullOrWhiteSpace(q))
        {
            var keyword = q.Trim();
            var reportId = Guid.TryParse(keyword, out var parsed) ? parsed : (Guid?)null;
            var shortCode = quanTri && keyword.Length == 8 && keyword.All(Uri.IsHexDigit) ? keyword.ToLowerInvariant() : null;
            var accounts = db.Users.Where(user => user.HoTen.Contains(keyword) ||
                (user.Email != null && user.Email.Contains(keyword))).Select(user => user.Id);
            query = query.Where(report => report.LyDo.Contains(keyword) || report.MoTa.Contains(keyword) ||
                report.Id == reportId || (shortCode != null && report.Id.ToString().StartsWith(shortCode)) ||
                (quanTri && (accounts.Contains(report.NguoiBaoCaoId) ||
                    accounts.Contains(report.NguoiBiBaoCaoId ?? Guid.Empty))));
        }
        return new Page<BaoCaoViPham>
        {
            Trang = trang,
            KichThuocTrang = 20,
            Total = await query.CountAsync(),
            Items = await query.OrderByDescending(x => x.GuiLuc).ThenBy(x => x.Id).Skip((trang - 1) * 20).Take(20).ToListAsync()
        };
    }

    public async Task<IReadOnlyDictionary<Guid, TaiKhoanBaoCao>> LayTaiKhoanBaoCaoAsync(
        IEnumerable<BaoCaoViPham> reports, Guid? reporterFilter = null)
    {
        await admin.KiemTraAsync("XuLyBaoCao");
        var ids = reports.SelectMany(report => new Guid?[] { report.NguoiBaoCaoId, report.NguoiBiBaoCaoId, report.NguoiXuLyId })
            .Append(reporterFilter).OfType<Guid>().Distinct().ToArray();
        return await db.Users.AsNoTracking().Where(user => ids.Contains(user.Id))
            .Select(user => new TaiKhoanBaoCao(user.Id, user.HoTen)).ToDictionaryAsync(user => user.Id);
    }

    public Task<KetQua<BaoCaoViPham>> LayChiTietAsync(Guid maBaoCao, Guid maNguoiThucHien, CancellationToken huy)
        => KetQua<BaoCaoViPham>.ThucHienAsync(async () =>
        {
            var me = await current.KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
            var report = await db.Set<BaoCaoViPham>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == maBaoCao, huy)
                ?? throw new LoiNghiepVu("Không tìm thấy báo cáo.", 404);
            Rules.Check(report.NguoiBaoCaoId == me.Id || await admin.CoQuyenAsync("XuLyBaoCao"), "Bạn không có quyền xem báo cáo này.", 403);
            return report;
        });

    public async Task<BaoCaoChiTietViewModel> ChiTietAsync(Guid id)
    {
        var me = await current.HienTaiAsync();
        var report = (await LayChiTietAsync(id, me.Id, default)).DuLieu;
        var canAdmin = await admin.CoQuyenAsync("XuLyBaoCao");
        return new BaoCaoChiTietViewModel
        {
            BaoCao = report,
            CoTheXuLy = canAdmin,
            NoiDungGanKem = canAdmin ? DocNoiDung(report.BanChup) : null,
            TaiKhoan = canAdmin ? await LayTaiKhoanBaoCaoAsync([report]) : new Dictionary<Guid, TaiKhoanBaoCao>(),
            SoBaoCaoCuaNguoiGui = canAdmin ? await db.Set<BaoCaoViPham>().CountAsync(x => x.NguoiBaoCaoId == report.NguoiBaoCaoId) : 0,
            BienPhap = canAdmin ? await db.Set<BienPhapQuanTri>().AsNoTracking().Where(x => x.BaoCaoId == id && x.NguonBienPhapId == null)
                .OrderByDescending(x => x.ApDungLuc).ToListAsync() : [],
            LichSu = await db.Set<LichSuXuLyBaoCao>().AsNoTracking().Where(x => x.BaoCaoId == id && (canAdmin || x.HanhDong != "GhiChuNoiBo")).OrderBy(x => x.TaoLuc).ToListAsync(),
            Tep = await db.Set<TepDinhKem>().AsNoTracking().Where(x => x.BaoCaoViPhamId == id).ToListAsync()
        };
    }

    public async Task<(TepDinhKem File, string Path)> TaiTepAsync(Guid id)
    {
        var file = await db.Set<TepDinhKem>().SingleOrDefaultAsync(x => x.Id == id && x.BaoCaoViPhamId != null)
            ?? throw new LoiNghiepVu("Không tìm thấy tệp.", 404);
        var me = await current.HienTaiAsync();
        _ = (await LayChiTietAsync(file.BaoCaoViPhamId!.Value, me.Id, default)).DuLieu;
        Rules.Check(file.TrangThaiQuet == "Sach" || (files.ChoPhepTepChuaQuet && file.TrangThaiQuet == "ChoQuet"), "Tệp chưa khả dụng.", 409);
        return (file, files.DuongDan(file));
    }

    public async Task<(TepDinhKem File, string Path)> TaiTepTinNhanAsync(Guid reportId, Guid fileId)
    {
        await admin.KiemTraAsync("XuLyBaoCao");
        var report = await db.Set<BaoCaoViPham>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == reportId && x.LoaiDoiTuong == "TinNhan")
            ?? throw new LoiNghiepVu("Không tìm thấy báo cáo tin nhắn.", 404);
        Rules.Check(DocNoiDung(report.BanChup)?.Tep?.Any(file => file.Id == fileId) == true,
            "Tệp này không thuộc nội dung báo cáo.", 404);
        var file = await db.Set<TepDinhKem>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == fileId &&
            x.TinNhanId == report.DoiTuongId && x.TrangThaiQuet == "Sach")
            ?? throw new LoiNghiepVu("Tệp tin nhắn không còn khả dụng.", 404);
        return (file, files.DuongDan(file));
    }

    private static NoiDungBaoCao? DocNoiDung(string snapshot)
    {
        try
        {
            var value = JsonSerializer.Deserialize<NoiDungBaoCao>(snapshot);
            return string.IsNullOrWhiteSpace(value?.TieuDe) ? null : value;
        }
        catch (JsonException) { return null; }
    }

    public async Task<bool> BoSungAsync(Guid id, string text, string version, bool xemLai)
    {
        Rules.Check(!string.IsNullOrWhiteSpace(text) && text.Length <= 2000, "Nội dung bổ sung cần từ 1 đến 2000 ký tự.");
        var me = await current.HienTaiAsync();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var report = await db.Set<BaoCaoViPham>().FromSqlInterpolated($"SELECT * FROM dbo.BaoCaoViPham WITH (UPDLOCK,HOLDLOCK) WHERE Id={id}").SingleOrDefaultAsync()
            ?? throw new LoiNghiepVu("Không tìm thấy báo cáo.", 404);
        Rules.Check(me.Id == report.NguoiBaoCaoId || (xemLai && me.Id == report.NguoiBiBaoCaoId), "Bạn chưa có quyền bổ sung thông tin cho báo cáo này.", 403);
        await GiaoDichNghiepVu.KiemTraTaiKhoanAsync(db, me.Id);
        Rules.Concurrency(db, report, version);
        Rules.Check(xemLai ? report.TrangThai == "DaXuLy" : report.TrangThai != "DaXuLy", "Trạng thái báo cáo đã thay đổi.", 409);
        if (xemLai)
            Rules.Check(!await db.Set<BaoCaoViPham>().AnyAsync(x => x.Id != id && x.NguoiBaoCaoId == report.NguoiBaoCaoId && x.LoaiDoiTuong == report.LoaiDoiTuong && x.DoiTuongId == report.DoiTuongId && x.TrangThai != "DaXuLy"), "Đã có báo cáo khác cho nội dung này đang được xem xét.", 409);
        db.Add(new LichSuXuLyBaoCao { BaoCaoId = id, NguoiThucHienId = me.Id, HanhDong = xemLai ? "DeNghiXemLai" : "BoSung", NoiDung = text.Trim(), TaoLuc = await GiaoDichNghiepVu.GioMayChuAsync(db) });
        if (xemLai)
        {
            report.TrangThai = "ChoXuLy";
            report.NguoiXuLyId = null;
        }
        else
            db.Entry(report).Property(x => x.LyDo).IsModified = true;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return me.Id == report.NguoiBaoCaoId;
    }

    public async Task<NoiDungBaoCao> XemTruocAsync(string loai, Guid id)
    {
        var (owner, snapshot) = await DoiTuongAsync(loai, id);
        Rules.Check(owner != (await current.HienTaiAsync()).Id, "Bạn không thể báo cáo nội dung của chính mình.");
        return snapshot;
    }

    private async Task<(Guid Owner, NoiDungBaoCao Snapshot)> DoiTuongAsync(string loai, Guid id)
    {
        var me = await current.HienTaiAsync();
        switch (loai)
        {
            case "TaiKhoan":
                var hasContact = await db.Set<YeuCauLienHe>().AnyAsync(x => x.TrangThai == "ChapNhan" &&
                    ((x.GiaSuId == id && x.BenHocId == me.Id) || (x.BenHocId == id && x.GiaSuId == me.Id)));
                var hasClass = await db.Set<LopHoc>().AnyAsync(x =>
                    (x.GiaSuId == id && x.BenHocId == me.Id) || (x.BenHocId == id && x.GiaSuId == me.Id));
                Rules.Check(hasContact || hasClass || await admin.CoQuyenAsync("XuLyBaoCao") ||
                    await new HoSoCongKhaiQuery(db).CongKhai().AnyAsync(x => x.GiaSuId == id) ||
                    await TimKiemService.NhuCauCongKhai(db).AnyAsync(x => x.ChuSoHuuId == id),
                    "Không tìm thấy tài khoản để báo cáo.", 404);
                var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id)
                    ?? throw new LoiNghiepVu("Không tìm thấy tài khoản.", 404);
                return (user.Id, new NoiDungBaoCao(user.HoTen));
            case "HoSoGiaSu":
                var profile = await new HoSoCongKhaiQuery(db).CongKhai().SingleOrDefaultAsync(x => x.Id == id)
                    ?? throw new LoiNghiepVu("Hồ sơ hiện không công khai.", 404);
                var tutorName = await db.Users.Where(x => x.Id == profile.GiaSuId).Select(x => x.HoTen).SingleAsync();
                return (profile.GiaSuId, new NoiDungBaoCao(tutorName, profile.TrinhDo + "\n" + profile.MoTa));
            case "NhuCauHoc":
                var need = await TimKiemService.NhuCauCongKhai(db).SingleOrDefaultAsync(x => x.Id == id)
                    ?? throw new LoiNghiepVu("Không tìm thấy nhu cầu công khai.", 404);
                return (need.ChuSoHuuId, new NoiDungBaoCao($"{need.TenMon} · {need.HoTen}",
                    string.Join("\n", new[] { need.KhoiLop, need.MucTieu, Rules.Fee(need.HocPhiDuKien, need.DonViHocPhi) }.Where(value => !string.IsNullOrWhiteSpace(value)))));
            case "LopHoc":
                var lop = await rights.KiemTraQuyenXemAsync(id);
                return (me.Id == lop.GiaSuId ? lop.BenHocId : lop.GiaSuId, new NoiDungBaoCao(lop.TenLop, lop.MucTieu));
            case "TinNhan":
                var message = await db.Set<TinNhan>().SingleOrDefaultAsync(x => x.Id == id) ?? throw new LoiNghiepVu("Không tìm thấy tin nhắn.", 404);
                var room = await chat.KiemTraAsync(message.CuocHoiThoaiId);
                if (room.LoaiHoiThoai == "TheoLop")
                {
                    var c = await rights.KiemTraQuyenXemAsync(room.LopHocId!.Value);
                    if (me.Id != c.GiaSuId && me.Id != c.BenHocId)
                        Rules.Check(await db.Set<ThanhVienLopHoc>().AnyAsync(x => x.LopHocId == c.Id && x.TaiKhoanId == me.Id && x.DuyetLuc <= message.GuiLuc && x.RoiLopLuc == null), "Tin nhắn nằm ngoài lịch sử được cấp.", 403);
                }
                var sender = await db.Users.Where(x => x.Id == message.NguoiGuiId).Select(x => x.HoTen).SingleAsync();
                var attachments = await db.Set<TepDinhKem>().AsNoTracking()
                    .Where(x => x.TinNhanId == message.Id && x.TrangThaiQuet == "Sach")
                    .OrderBy(x => x.TaoLuc).ThenBy(x => x.Id)
                    .Select(x => new TepNguonBaoCao(x.Id, x.TenGoc)).ToListAsync();
                return (message.NguoiGuiId, new NoiDungBaoCao(sender, message.NoiDung, message.GuiLuc, attachments));
            case "DanhGiaDoiTac":
                var review = await db.Set<DanhGiaDoiTac>().SingleOrDefaultAsync(x => x.Id == id) ?? throw new LoiNghiepVu("Không tìm thấy đánh giá.", 404);
                var recipientTutor = await db.Set<GiaSu>().AnyAsync(x => x.TaiKhoanId == review.NguoiNhanId);
                Rules.Check(recipientTutor || me.LoaiNguoi == "GiaSu" || me.Id == review.NguoiNhanId || me.Id == review.NguoiGuiId, "Bạn không có quyền xem đánh giá này.", 403);
                var reviewText = string.IsNullOrWhiteSpace(review.PhanHoi) ? review.NhanXet
                    : review.NhanXet + "\n\nPhản hồi: " + review.PhanHoi;
                return (review.NguoiGuiId, new NoiDungBaoCao($"{review.SoSao}/5 sao", reviewText, review.GuiLuc));
            default:
                throw new LoiNghiepVu("Loại đối tượng báo cáo không hợp lệ.");
        }
    }
}
