using System.Data;
using EduConnect.Mvc.Areas.DanhGia.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.DanhGia.Services;

public sealed class DanhGiaDoiTacService(AppDbContext db, NguoiDungHienTaiService current,
    QuyenLopHocService rights, ThongBaoService notices)
{
    public async Task<Guid> NguoiNhanAsync(Guid id)
    {
        var value = await db.Set<DanhGiaDoiTac>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new LoiNghiepVu("Không tìm thấy đánh giá.", 404);
        _ = await LayAsync(value.NguoiNhanId);
        return value.NguoiNhanId;
    }
    public async Task<DanhGiaDoiTacViewModel> LayDeDanhGiaAsync(Guid lopId)
    {
        var lop = await rights.KiemTraQuyenXemAsync(lopId);
        var me = await current.HienTaiAsync();
        await KiemTraDieuKienAsync(lop, me.Id);
        var value = await db.Set<DanhGiaDoiTac>().AsNoTracking().SingleOrDefaultAsync(x => x.LopHocId == lopId && x.NguoiGuiId == me.Id);
        Rules.Check(value == null, "Bạn đã gửi đánh giá cho lớp này. Mỗi bên chỉ được đánh giá một lần.", 409);
        return new DanhGiaDoiTacViewModel
        {
            MaLop = lopId,
            SoSao = 5
        };
    }

    public async Task<Guid> GuiAsync(DanhGiaDoiTacViewModel input)
    {
        input.NhanXet = input.NhanXet?.Trim() ?? "";
        Rules.ValidateInput(input);
        Rules.Check(input.XacNhanRiengTu, "Vui lòng xác nhận nhận xét không chứa thông tin riêng tư.", field: nameof(input.XacNhanRiengTu));
        var me = await current.HienTaiAsync();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var lop = await GiaoDichNghiepVu.KhoaLopAsync(db, input.MaLop);
        await GiaoDichNghiepVu.KiemTraTaiKhoanAsync(db, me.Id);
        await KiemTraDieuKienAsync(lop, me.Id);
        var recipient = me.Id == lop.GiaSuId ? lop.BenHocId : lop.GiaSuId;
        Rules.Check(!await db.Set<DanhGiaDoiTac>().AnyAsync(x => x.LopHocId == lop.Id && x.NguoiGuiId == me.Id),
            "Bạn đã gửi đánh giá cho lớp này. Mỗi bên chỉ được đánh giá một lần.", 409);
        var review = new DanhGiaDoiTac
        {
            LopHocId = lop.Id,
            NguoiGuiId = me.Id,
            NguoiNhanId = recipient,
            GuiLuc = await GiaoDichNghiepVu.GioMayChuAsync(db),
            SoSao = input.SoSao,
            NhanXet = input.NhanXet.Trim()
        };
        db.Add(review);
        notices.Tao(recipient, "DanhGiaDoiTac", review.Id, me.HoTen + " đã gửi đánh giá về bạn.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return recipient;
    }

    public async Task<Guid> PhanHoiAsync(PhanHoiDoiTacViewModel input)
    {
        Rules.ValidateInput(input);
        var me = await current.HienTaiAsync();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var review = await db.Set<DanhGiaDoiTac>().FromSqlInterpolated($"SELECT * FROM dbo.DanhGiaDoiTac WITH (UPDLOCK,HOLDLOCK) WHERE Id={input.DanhGiaId}").SingleOrDefaultAsync()
            ?? throw new LoiNghiepVu("Không tìm thấy đánh giá.", 404);
        Rules.Check(review.NguoiNhanId == me.Id, "Chỉ người nhận đánh giá được phản hồi.", 403);
        await GiaoDichNghiepVu.KiemTraTaiKhoanAsync(db, me.Id);
        Rules.Concurrency(db, review, input.RowVersion);
        var now = await GiaoDichNghiepVu.GioMayChuAsync(db);
        LuuLichSu(review, me.Id, now);
        review.PhanHoi = input.NoiDung.Trim();
        review.PhanHoiLuc = now;
        notices.Tao(review.NguoiGuiId, "DanhGiaDoiTac", review.Id, me.HoTen + " đã phản hồi đánh giá của bạn.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return review.NguoiNhanId;
    }

    public IQueryable<DanhGiaDoiTac> CongKhai() => CongKhai(db);

    public static IQueryable<DanhGiaDoiTac> CongKhai(AppDbContext db) => db.Set<DanhGiaDoiTac>().AsNoTracking().Where(x =>
        !db.Set<BienPhapQuanTri>().Any(b => b.LoaiDoiTuong == "DanhGiaDoiTac" && b.DoiTuongId == x.Id && b.Loai == "AnNoiDung" && b.GoLuc == null)
        && db.Users.Any(u => u.Id == x.NguoiNhanId && u.TrangThai == "HoatDong"));

    public async Task<DanhGiaCongKhaiPage> LayAsync(Guid taiKhoan, int trang = 1, bool noiBat = false)
    {
        Rules.Check(trang is >= 1 and <= 100000, "Trang không hợp lệ.");
        var me = await current.NguoiXemAsync();
        if (me == null)
            Rules.Check(await new EduConnect.Mvc.Areas.NhuCau.Services.HoSoCongKhaiQuery(db).CongKhai()
                .AnyAsync(profile => profile.GiaSuId == taiKhoan), "Không tìm thấy hồ sơ gia sư công khai.", 404);
        var person = await db.Set<NguoiThamGia>().SingleOrDefaultAsync(x => x.Id == taiKhoan)
            ?? throw new LoiNghiepVu("Không tìm thấy tài khoản.", 404);
        var moderator = me?.LoaiNguoi == "QuanTri" && await db.UserClaims.AnyAsync(x => x.UserId == me.Id && x.ClaimType == "EduConnect.AdminPermission" && x.ClaimValue == "XuLyBaoCao");
        Rules.Check(person.LoaiNguoi == "GiaSu" || me?.LoaiNguoi == "GiaSu" || me?.Id == taiKhoan || moderator,
            "Đánh giá bên học dành cho gia sư và tài khoản liên quan.", 403);
        var query = CongKhai().Where(x => x.NguoiNhanId == taiKhoan);
        var counts = await query.GroupBy(x => x.SoSao)
            .Select(group => new { Stars = group.Key, Total = group.Count() })
            .ToDictionaryAsync(group => group.Stars, group => group.Total);
        var total = counts.Values.Sum();
        const int pageSize = 20;
        var items = noiBat
            ? await query.OrderByDescending(x => x.SoSao).ThenByDescending(x => x.GuiLuc).ThenBy(x => x.Id).Take(3).ToListAsync()
            : await query.OrderByDescending(x => x.GuiLuc).ThenBy(x => x.Id).Skip((trang - 1) * pageSize).Take(pageSize).ToListAsync();
        return new DanhGiaCongKhaiPage
        {
            TaiKhoanId = taiKhoan,
            HoTen = person.HoTen,
            TrungBinh = total > 0 ? counts.Sum(rating => (decimal)rating.Key * rating.Value) / total : null,
            TheoSoSao = counts,
            DanhGia = new Page<DanhGiaDoiTac>
            {
                Trang = trang,
                KichThuocTrang = noiBat ? 3 : pageSize,
                Total = total,
                Items = items
            }
        };
    }

    public async Task<List<LichSuDanhGiaDoiTac>> LichSuAsync(Guid id)
    {
        var me = await current.HienTaiAsync();
        var review = await db.Set<DanhGiaDoiTac>().SingleOrDefaultAsync(x => x.Id == id) ?? throw new LoiNghiepVu("Không tìm thấy đánh giá.", 404);
        var moderator = me.LoaiNguoi == "QuanTri" && await db.UserClaims.AnyAsync(x => x.UserId == me.Id && x.ClaimType == "EduConnect.AdminPermission" && x.ClaimValue == "XuLyBaoCao");
        Rules.Check(me.Id == review.NguoiGuiId || me.Id == review.NguoiNhanId || moderator, "Bạn không có quyền xem lịch sử này.", 403);
        return await db.Set<LichSuDanhGiaDoiTac>().AsNoTracking().Where(x => x.DanhGiaId == id).OrderByDescending(x => x.ThayDoiLuc).ToListAsync();
    }

    private async Task KiemTraDieuKienAsync(LopHoc lop, Guid actor)
    {
        Rules.Check(actor == lop.GiaSuId || actor == lop.BenHocId, "Chỉ hai bên đứng tên lớp được đánh giá người dùng.", 403);
        Rules.Check(lop.TrangThai == "DaKetThuc", "Lớp cần kết thúc trước khi đánh giá công khai.", 409);
        Rules.Check(await db.Set<BuoiHoc>().AnyAsync(b => b.TrangThai == "DaHoc" && b.GhiNhanLuc != null && db.Set<LichHoc>().Any(l => l.Id == b.LichHocId && l.LopHocId == lop.Id)),
            "Lớp cần ít nhất một buổi học thực tế đã ghi nhận.", 409);
    }

    private void LuuLichSu(DanhGiaDoiTac value, Guid actor, DateTimeOffset now)
        => db.Add(new LichSuDanhGiaDoiTac
        {
            DanhGiaId = value.Id,
            SoSao = value.SoSao,
            NhanXet = value.NhanXet,
            PhanHoi = value.PhanHoi,
            NguoiSuaId = actor,
            ThayDoiLuc = now
        });
}
