using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Controllers;

[AllowAnonymous]
[Route("huong-dan")]
public sealed class HuongDanController(IConfiguration configuration, AppDbContext db) : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View("TrungTam");

    [HttpGet("hoc-phi")]
    public async Task<IActionResult> HocPhi(string? donVi, int? monHocId, CancellationToken huy)
    {
        var unit = donVi == "VND/Buoi" ? "VND/Buoi" : "VND/Gio";
        var profiles = new HoSoCongKhaiQuery(db).CongKhai();
        var offers = from subject in db.Set<HoSoGiaSuMonDay>()
                     join profile in profiles on subject.HoSoGiaSuId equals profile.Id
                     join mon in db.Set<MonHoc>() on subject.MonHocId equals mon.Id
                     where subject.HocPhiDeXuat > 0 && subject.MatNaKhoi > 0 && mon.DangSuDung
                         && (subject.DonViHocPhi == "VND/Gio" || subject.DonViHocPhi == "VND/Buoi")
                     select new { subject.HocPhiDeXuat, subject.MatNaKhoi, subject.MonHocId, subject.DonViHocPhi, mon.TenMon, profile.TrinhDo };
        var subjects = await offers.Select(x => new { x.MonHocId, x.TenMon }).Distinct().OrderBy(x => x.TenMon).ToListAsync(huy);
        if (!subjects.Any(x => x.MonHocId == monHocId)) monHocId = null;
        var selectedOffers = offers.Where(x => monHocId == null || x.MonHocId == monHocId);
        var currentPrices = await selectedOffers.GroupBy(x => x.DonViHocPhi)
            .Select(g => new { Unit = g.Key!, Min = g.Min(x => x.HocPhiDeXuat!.Value), Max = g.Max(x => x.HocPhiDeXuat!.Value), Count = g.Count() })
            .ToDictionaryAsync(x => x.Unit, x => new MucPhiCongKhai(x.Min, x.Max, x.Count), huy);
        var bands = await selectedOffers.Where(x => x.DonViHocPhi == unit)
            .GroupBy(x => new { x.MatNaKhoi, x.TrinhDo })
            .Select(g => new { g.Key.MatNaKhoi, g.Key.TrinhDo, Min = g.Min(x => x.HocPhiDeXuat!.Value), Max = g.Max(x => x.HocPhiDeXuat!.Value), Count = g.Count() })
            .ToListAsync(huy);
        var degrees = HoSoLuaChon.TrinhDo.Where(degree => bands.Any(b => b.TrinhDo == degree)).ToList();
        var rows = new List<HocPhiTheoLop>();
        for (var grade = 1; grade <= 12; grade++)
        {
            var mask = PhamViMonHoc.BitKhoi(grade);
            var relevant = bands.Where(x => (x.MatNaKhoi!.Value & mask) != 0).ToList();
            MucPhiCongKhai? Summarize(string? degree)
            {
                var selection = relevant.Where(b => degree == null || b.TrinhDo == degree).ToList();
                return selection.Count == 0 ? null : new(selection.Min(x => x.Min), selection.Max(x => x.Max), selection.Sum(x => x.Count));
            }
            rows.Add(new(grade, Summarize(null), degrees.Select(Summarize).ToList()));
        }
        return View(new BangHocPhi(unit, monHocId, subjects.Select(x => (x.MonHocId, x.TenMon)).ToList(), degrees, rows,
            currentPrices, DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7))));
    }

    [HttpGet("{muc}")]
    public IActionResult Trang(string muc)
    {
        var item = HuongDanViewModel.DanhMuc.FirstOrDefault(page => page.Ma == muc);
        if (item == null) return NotFound();

        return View("Index", new HuongDanViewModel(item, configuration["HoTro:Email"],
            configuration.GetValue<int?>("HoTro:NgayLamViecDuyetHoSo")));
    }
}
