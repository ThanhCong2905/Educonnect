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
        var area = input.HinhThucHoc == "TrucTiep" ? input.KhuVuc : null;
        var address = input.HinhThucHoc == "TrucTiep" ? input.DiaChiChiTiet : null;
        var candidates = await persistence.FindDuplicateCandidatesAsync(input, owner, huy);
        var wanted = input.LichRanh.Select(x => (x.ThuTrongTuan, x.GioBatDau, x.GioKetThuc)).OrderBy(x => x.ThuTrongTuan).ThenBy(x => x.GioBatDau).ToArray();
        foreach (var candidateId in candidates)
        {
            var candidate = await persistence.FindOwnedAsync(candidateId, owner, false, huy)
                ?? throw new InvalidOperationException("Duplicate candidate disappeared while the owner's lock was held.");
            var sameText = new[]
            {
                (candidate.TenNguoiHoc, input.TenNguoiHoc), (candidate.KhoiLop, input.KhoiLop),
                (candidate.MucTieu, input.MucTieu), (candidate.HocLuc, input.HocLuc),
                (candidate.KhuVuc, area), (candidate.PhuongXa, input.HinhThucHoc == "TrucTiep" ? input.PhuongXa : null), (candidate.DiaChiChiTiet, address)
            }.All(pair => string.Equals(HoSoGiaSuService.ChuanHoa(pair.Item1).Normalize(),
                HoSoGiaSuService.ChuanHoa(pair.Item2).Normalize(), StringComparison.OrdinalIgnoreCase));
            if (!sameText || !candidate.CacMonHocId.ToHashSet().SetEquals(ids)) continue;
            var slots = await persistence.ReadSlotsAsync(candidate.Id, huy);
            var existing = slots.Select(x => (x.ThuTrongTuan, x.GioBatDau, x.GioKetThuc)).OrderBy(x => x.ThuTrongTuan).ThenBy(x => x.GioBatDau).ToArray();
            Rules.Check(!wanted.SequenceEqual(existing), "Bạn đã có nhu cầu đang mở với cùng thông tin. Hãy cập nhật nhu cầu đó.", 409);
        }
    }

    private Task KhoaTaiKhoanAsync(Guid id, CancellationToken huy = default)
        => db.Database.ExecuteSqlInterpolatedAsync($"SELECT Id FROM dbo.AspNetUsers WITH(UPDLOCK,HOLDLOCK) WHERE Id={id}", huy);

    private async Task KiemTraHanMucAsync(Guid owner, bool draft, Guid? excluding, CancellationToken huy = default)
    {
        var state = draft ? "BanNhap" : "DangMo";
        var count = await persistence.CountNeedsAsync(owner, state, excluding, huy);
        Rules.Check(count < (draft ? 5 : 3), draft ? "Bạn có tối đa 5 bản nháp. Hãy cập nhật bản đã có." : "Bạn có tối đa 3 nhu cầu đang mở. Hãy đóng nhu cầu không còn tìm gia sư.", 409);
    }

    private async Task KiemTraLuotDangAsync(Guid owner, CancellationToken huy = default)
    {
        var date = DateOnly.FromDateTime(DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)).DateTime);
        Rules.Check(await persistence.CountPostingsAsync(owner, date, huy) < 3,
            "Bạn đã dùng hết 3 lượt đăng hoặc mở lại hôm nay. Bạn vẫn có thể sửa nhu cầu hiện có.", 429);
    }

    private void GhiLuotDang(Guid need, Guid owner, string action, Guid? operation = null)
    {
        var now = DateTimeOffset.UtcNow;
        persistence.AddPosting(new LuotDangNhuCau { Id = Guid.NewGuid(), NhuCauId = need, TaiKhoanId = owner, MaThaoTac = operation ?? Guid.NewGuid(), HanhDong = action, DangLuc = now, NgayDang = DateOnly.FromDateTime(now.ToOffset(TimeSpan.FromHours(7)).DateTime) });
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
