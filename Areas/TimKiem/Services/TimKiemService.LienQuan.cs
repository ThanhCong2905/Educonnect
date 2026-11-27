using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.TimKiem.Services;

public sealed partial class TimKiemService
{
    public async Task<List<GiaSuLienQuanViewModel>> GiaSuLienQuanAsync(Guid maHoSo, CancellationToken huy = default)
    {
        var source = await HoSoCongKhai().SingleOrDefaultAsync(profile => profile.Id == maHoSo, huy)
            ?? throw new LoiNghiepVu("Hồ sơ không còn công khai.", 404);
        var subjects = await db.Set<HoSoGiaSuMonDay>().AsNoTracking()
            .Where(subject => subject.HoSoGiaSuId == maHoSo).OrderBy(subject => subject.MonHocId).ToListAsync(huy);
        var matches = new List<(HoSoGiaSu Profile, HoSoGiaSuMonDay Offer, HoSoGiaSuMonDay Source)>();
        foreach (var subject in subjects)
        {
            var query = GiaSuCungMon(source, subject);
            var sameUnit = query.Where(row => row.Offer.DonViHocPhi == subject.DonViHocPhi);
            var rows = await sameUnit.Where(row => row.Offer.HocPhiDeXuat < subject.HocPhiDeXuat)
                .OrderByDescending(row => row.Offer.HocPhiDeXuat).ThenBy(row => row.Profile.Id).Take(2)
                .Concat(sameUnit.Where(row => row.Offer.HocPhiDeXuat > subject.HocPhiDeXuat)
                    .OrderBy(row => row.Offer.HocPhiDeXuat).ThenBy(row => row.Profile.Id).Take(2))
                .Concat(sameUnit.Where(row => row.Offer.HocPhiDeXuat == subject.HocPhiDeXuat)
                    .OrderBy(row => row.Profile.KhuVuc == source.KhuVuc ? 0 : 1).ThenBy(row => row.Profile.Id).Take(2))
                .Concat(query.Where(row => row.Offer.DonViHocPhi != subject.DonViHocPhi)
                    .OrderBy(row => row.Profile.KhuVuc == source.KhuVuc ? 0 : 1).ThenBy(row => row.Profile.Id).Take(2))
                .ToListAsync(huy);
            matches.AddRange(rows.Select(row => (row.Profile, row.Offer, subject)));
        }
        if (matches.Count == 0) return [];
        var profiles = matches.Select(match => match.Profile).DistinctBy(profile => profile.Id).ToList();
        var publicRows = (await HoSoRowsAsync(profiles, huy)).ToDictionary(profile => profile.Id);
        var candidates = matches.Select(match =>
        {
            var scopeMatches = match.Source.MatNaKhoi is > 0 && match.Offer.MatNaKhoi is > 0
                && (match.Source.MatNaKhoi.Value & match.Offer.MatNaKhoi.Value) != 0
                && (match.Source.PhanKhoaHoc == 0 && match.Offer.PhanKhoaHoc == 0
                    || (match.Source.PhanKhoaHoc & match.Offer.PhanKhoaHoc) != 0);
            var priceMatches = scopeMatches && match.Source.HocPhiDeXuat.HasValue && match.Offer.HocPhiDeXuat.HasValue
                && match.Source.DonViHocPhi == match.Offer.DonViHocPhi;
            var profile = publicRows[match.Profile.Id];
            var offer = profile.MonTheoHoSo.Single(subject => subject.MonHocId == match.Offer.MonHocId);
            return new GiaSuLienQuanViewModel
            {
                HoSo = profile, MonHocId = offer.MonHocId, TenMon = offer.TenMon, PhamViDay = offer.PhamViDay,
                HocPhi = offer.HocPhi, DonVi = offer.DonVi, HocPhiGoc = match.Source.HocPhiDeXuat,
                ChenhLech = priceMatches ? offer.HocPhi - match.Source.HocPhiDeXuat : null,
                TrungPhamVi = scopeMatches,
                CungKhuVuc = !string.IsNullOrWhiteSpace(source.KhuVuc) && source.KhuVuc == profile.KhuVuc
            };
        }).OrderByDescending(item => item.ChenhLech.HasValue).ThenByDescending(item => item.TrungPhamVi)
            .ThenByDescending(item => item.CungKhuVuc).ThenBy(item => Math.Abs(item.ChenhLech ?? decimal.MaxValue))
            .ThenByDescending(item => item.HoSo.DiemDanhGia).ThenByDescending(item => item.HoSo.SoDanhGia)
            .ThenBy(item => item.HoSo.Id).DistinctBy(item => item.HoSo.Id).ToList();
        var result = new List<GiaSuLienQuanViewModel>();
        foreach (var alternative in new[] { candidates.FirstOrDefault(item => item.ChenhLech < 0), candidates.FirstOrDefault(item => item.ChenhLech > 0) })
            if (alternative != null && result.All(item => item.HoSo.Id != alternative.HoSo.Id)) result.Add(alternative);
        result.AddRange(candidates.Where(candidate => result.All(item => item.HoSo.Id != candidate.HoSo.Id)).Take(3 - result.Count));
        return result;
    }

    private IQueryable<MonDayLienQuan> GiaSuCungMon(HoSoGiaSu source, HoSoGiaSuMonDay subject) =>
        from offer in db.Set<HoSoGiaSuMonDay>().AsNoTracking()
        join profile in HoSoCongKhai() on offer.HoSoGiaSuId equals profile.Id
        where profile.Id != source.Id && profile.GiaSuId != source.GiaSuId && offer.MonHocId == subject.MonHocId
            && (subject.MatNaKhoi == null || offer.MatNaKhoi == null || (offer.MatNaKhoi.Value & subject.MatNaKhoi.Value) != 0)
            && (subject.PhanKhoaHoc == 0 || offer.PhanKhoaHoc == 0 || (offer.PhanKhoaHoc & subject.PhanKhoaHoc) != 0)
            && (source.DayTrucTuyen && profile.DayTrucTuyen
                || source.DayTrucTiep && profile.DayTrucTiep && source.KhuVuc != null && profile.KhuVuc == source.KhuVuc)
        select new MonDayLienQuan { Profile = profile, Offer = offer };

    private sealed class MonDayLienQuan
    {
        public HoSoGiaSu Profile { get; init; } = null!;
        public HoSoGiaSuMonDay Offer { get; init; } = null!;
    }
}
