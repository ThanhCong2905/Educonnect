using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Models;

namespace EduConnect.Mvc.Areas.TimKiem.Services;

public static class GoiYRules
{
    public static IQueryable<UngVienViewModel> ChoGiaSu(IQueryable<UngVienViewModel> needs,
        IReadOnlyCollection<int> subjects, IQueryable<NhuCauMonHoc> subjectLinks)
        => needs.Where(x => subjects.Contains(x.MonHocId)
                || subjectLinks.Any(d => d.NhuCauId == x.Id && subjects.Contains(d.MonHocId)))
            .OrderByDescending(x => x.TaoLuc).Take(3);
    // The same calculation used after loading public profiles in GoiYAsync.
    public static List<UngVienViewModel> ChoBenHoc(IReadOnlyList<NhuCauHoc> own, List<UngVienViewModel> candidates)
    {
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
        return candidates.Where(x => x.LyDo.Count > 0).OrderByDescending(x => x.LyDo.Count).ThenByDescending(x => x.TaoLuc).ThenBy(x => x.Id).Take(3).ToList();
    }
}
