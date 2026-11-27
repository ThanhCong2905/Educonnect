using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.TimKiem.Services;

public sealed partial class TimKiemService
{
    public async Task<List<HocPhiThamChieuViewModel>> HocPhiThamChieuAsync(Guid maHoSo, CancellationToken huy = default)
    {
        var source = await HoSoCongKhai().SingleOrDefaultAsync(profile => profile.Id == maHoSo, huy)
            ?? throw new LoiNghiepVu("Hồ sơ không còn công khai.", 404);
        var subjects = await db.Set<HoSoGiaSuMonDay>().AsNoTracking()
            .Where(subject => subject.HoSoGiaSuId == maHoSo && subject.MatNaKhoi > 0).ToListAsync(huy);
        var result = new List<HocPhiThamChieuViewModel>();
        foreach (var subject in subjects)
        {
            var groups = await GiaSuCungMon(source, subject)
                .Where(row => row.Offer.MatNaKhoi > 0 && row.Offer.HocPhiDeXuat >= 0
                    && (subject.PhanKhoaHoc == 0 && row.Offer.PhanKhoaHoc == 0
                        || (row.Offer.PhanKhoaHoc & subject.PhanKhoaHoc) != 0))
                .GroupBy(row => row.Offer.DonViHocPhi)
                .Select(group => new { Unit = group.Key!, Average = group.Average(row => row.Offer.HocPhiDeXuat!.Value), Count = group.Count() })
                .ToListAsync(huy);
            result.AddRange(groups.Select(group => new HocPhiThamChieuViewModel
            {
                MonHocId = subject.MonHocId, DonVi = group.Unit,
                TrungBinh = decimal.Round(group.Average, 0, MidpointRounding.AwayFromZero), SoHoSo = group.Count
            }));
        }
        return result;
    }
}
