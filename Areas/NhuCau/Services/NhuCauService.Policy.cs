using System.Data;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.NhuCau.Services;

public sealed partial class NhuCauService
{
    private async Task KiemTraTrungAsync(NhuCauInput input, Guid owner, CancellationToken huy)
    {
        var ids = input.MonHocIds.Distinct().ToList();
        var area = input.CoHocTrucTiep ? input.KhuVuc : null;
        var address = input.CoHocTrucTiep ? input.DiaChiChiTiet : null;
        var candidates = await db.Set<NhuCauHoc>().Where(x => x.ChuSoHuuId == owner && x.Id != input.Id && (x.TrangThai == "DangMo" || db.Set<YeuCauKetNoiLopHoc>().Any(r => r.NhuCauId == x.Id && db.Set<LopHoc>().Any(c => c.YeuCauKetNoiId == r.Id && c.TrangThai == "DangHoatDong")))
            && x.PhanKhoaHoc == input.PhanKhoaHoc && x.TuoiNguoiHoc == input.TuoiNguoiHoc && x.HinhThucHoc == input.HinhThucHoc && x.HocPhiDuKien == input.HocPhiDuKien
            && x.DonViHocPhi == input.DonViHocPhi && x.SoBuoiTuan == input.SoBuoiTuan)
            .Include(x => x.MonHocs).AsNoTracking().ToListAsync(huy);
        var wanted = input.LichRanh.Select(x => (x.ThuTrongTuan, x.GioBatDau, x.GioKetThuc)).OrderBy(x => x.ThuTrongTuan).ThenBy(x => x.GioBatDau).ToArray();
        foreach (var candidate in candidates)
        {
            var sameText = new[]
            {
                (candidate.TenNguoiHoc, input.TenNguoiHoc), (candidate.KhoiLop, input.KhoiLop),
                (candidate.MucTieu, input.MucTieu),
                (candidate.KhuVuc, area), (candidate.PhuongXa, input.CoHocTrucTiep ? input.PhuongXa : null), (candidate.DiaChiChiTiet, address)
            }.All(pair => string.Equals(HoSoGiaSuService.ChuanHoa(pair.Item1).Normalize(),
                HoSoGiaSuService.ChuanHoa(pair.Item2).Normalize(), StringComparison.OrdinalIgnoreCase));
            if (!sameText || !candidate.CacMonHocId.ToHashSet().SetEquals(ids)) continue;
            var slots = await db.Set<KhungGioRanh>().Where(x => x.NhuCauId == candidate.Id).ToListAsync(huy);
            var existing = slots.Select(x => (x.ThuTrongTuan, x.GioBatDau, x.GioKetThuc)).OrderBy(x => x.ThuTrongTuan).ThenBy(x => x.GioBatDau).ToArray();
            Rules.Check(!wanted.SequenceEqual(existing), "Bạn đã có nhu cầu cùng thông tin đang mở hoặc đã có lớp hoạt động. Hãy cập nhật nhu cầu đó.", 409);
        }
    }

    private Task<bool> DaCoLopDangHoatDongAsync(Guid id, CancellationToken huy = default)
        => (from request in db.Set<YeuCauKetNoiLopHoc>()
            join lop in db.Set<LopHoc>() on request.Id equals lop.YeuCauKetNoiId
            where request.NhuCauId == id && lop.TrangThai == "DangHoatDong"
            select lop.Id).AnyAsync(huy);

    private async Task KiemTraLichLopAsync(NhuCauInput input, Guid owner, CancellationToken huy)
    {
        var now = await GiaoDichNghiepVu.GioMayChuAsync(db, huy);
        var schedules = await (from schedule in db.Set<LichHoc>()
                               join lop in db.Set<LopHoc>() on schedule.LopHocId equals lop.Id
                               where lop.BenHocId == owner && lop.TrangThai == "DangHoatDong"
                                   && schedule.TrangThai == "DaLenLich" && schedule.KetThucLuc > now
                               select schedule).AsNoTracking().ToListAsync(huy);
        foreach (var schedule in schedules)
        {
            var start = schedule.BatDauLuc.ToOffset(TimeSpan.FromHours(7));
            var end = schedule.KetThucLuc.ToOffset(TimeSpan.FromHours(7));
            for (var day = start.Date; day <= end.Date; day = day.AddDays(1))
            {
                var from = day == start.Date ? start.TimeOfDay : TimeSpan.Zero;
                var to = day == end.Date ? end.TimeOfDay : TimeSpan.FromDays(1);
                Rules.Check(!input.LichRanh.Any(slot => slot.ThuTrongTuan == (byte)day.DayOfWeek
                    && slot.GioBatDau.ToTimeSpan() < to && from < slot.GioKetThuc.ToTimeSpan()),
                    "Khung giờ này trùng với lịch của một lớp đang hoạt động. Hãy chọn giờ khác.", field: nameof(input.LichRanh));
            }
        }
    }

    private Task KhoaTaiKhoanAsync(Guid id, CancellationToken huy = default)
        => db.Database.ExecuteSqlInterpolatedAsync($"SELECT Id FROM dbo.AspNetUsers WITH(UPDLOCK,HOLDLOCK) WHERE Id={id}", huy);

    private async Task KiemTraHanMucAsync(Guid owner, bool draft, Guid? excluding, CancellationToken huy = default)
    {
        var state = draft ? "BanNhap" : "DangMo";
        var count = await db.Set<NhuCauHoc>().CountAsync(x => x.ChuSoHuuId == owner && x.TrangThai == state && x.Id != excluding, huy);
        Rules.Check(count < (draft ? 5 : 3), draft ? "Bạn có tối đa 5 bản nháp. Hãy cập nhật bản đã có." : "Bạn có tối đa 3 nhu cầu đang mở. Hãy đóng nhu cầu không còn tìm gia sư.", 409);
    }

    private async Task KiemTraLuotDangAsync(Guid owner, CancellationToken huy = default)
    {
        var date = DateOnly.FromDateTime(DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)).DateTime);
        Rules.Check(await db.Set<LuotDangNhuCau>().CountAsync(x => x.TaiKhoanId == owner && x.NgayDang == date, huy) < 3,
            "Bạn đã dùng hết 3 lượt đăng hoặc mở lại hôm nay. Bạn vẫn có thể sửa nhu cầu hiện có.", 429);
    }

    private void GhiLuotDang(Guid need, Guid owner, string action, Guid? operation = null)
    {
        var now = DateTimeOffset.UtcNow;
        db.Add(new LuotDangNhuCau { Id = Guid.NewGuid(), NhuCauId = need, TaiKhoanId = owner, MaThaoTac = operation ?? Guid.NewGuid(), HanhDong = action, DangLuc = now, NgayDang = DateOnly.FromDateTime(now.ToOffset(TimeSpan.FromHours(7)).DateTime) });
    }

    public async Task XacNhanConNhuCauAsync(Guid id, string version)
    {
        var me = await current.HienTaiAsync();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await KhoaTaiKhoanAsync(me.Id);
        await GiaoDichNghiepVu.KiemTraTaiKhoanAsync(db, me.Id);
        var need = await db.Set<NhuCauHoc>().SingleOrDefaultAsync(x => x.Id == id && x.ChuSoHuuId == me.Id)
            ?? throw new LoiNghiepVu("Không tìm thấy nhu cầu của bạn.", 404);
        Rules.Check(need.TrangThai == "DangMo", "Nhu cầu không còn mở.", 409);
        Rules.Concurrency(db, need, version);
        need.XacNhanConNhuCauLuc = DateTimeOffset.UtcNow;
        need.NhacXacNhanLuc = null;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
