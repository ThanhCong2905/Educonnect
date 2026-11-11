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
        var goal = input.MucTieu?.Trim();
        var area = input.HinhThucHoc == "TrucTiep" ? input.KhuVuc : null;
        var address = input.HinhThucHoc == "TrucTiep" ? input.DiaChiChiTiet : null;
        var candidates = await db.Set<NhuCauHoc>().Where(x => x.ChuSoHuuId == owner && x.Id != input.Id && x.TrangThai == "DangMo"
            && x.TenNguoiHoc == input.TenNguoiHoc && x.TuoiNguoiHoc == input.TuoiNguoiHoc && x.KhoiLop == input.KhoiLop
            && x.MucTieu == goal && x.HocLuc == input.HocLuc && x.HinhThucHoc == input.HinhThucHoc && x.HocPhiDuKien == input.HocPhiDuKien
            && x.DonViHocPhi == input.DonViHocPhi && x.SoBuoiTuan == input.SoBuoiTuan && x.KhuVuc == area && x.DiaChiChiTiet == address
            && db.Set<NhuCauMonHoc>().Count(m => m.NhuCauId == x.Id) == ids.Count
            && !db.Set<NhuCauMonHoc>().Any(m => m.NhuCauId == x.Id && !ids.Contains(m.MonHocId)))
            .Select(x => x.Id).ToListAsync(huy);
        var wanted = input.LichRanh.Select(x => (x.ThuTrongTuan, x.GioBatDau, x.GioKetThuc)).OrderBy(x => x.ThuTrongTuan).ThenBy(x => x.GioBatDau).ToArray();
        foreach (var id in candidates)
        {
            var slots = await db.Set<KhungGioRanh>().Where(x => x.NhuCauId == id).ToListAsync(huy);
            var existing = slots.Select(x => (x.ThuTrongTuan, x.GioBatDau, x.GioKetThuc)).OrderBy(x => x.ThuTrongTuan).ThenBy(x => x.GioBatDau).ToArray();
            Rules.Check(!wanted.SequenceEqual(existing), "Bạn đã có nhu cầu đang mở với cùng thông tin. Hãy cập nhật nhu cầu đó.", 409);
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
