using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Areas.NhuCau.Controllers;

[Area("NhuCau")]
public sealed class NhuCauController(NhuCauService service, NguoiDungMauService current) : BaseController
{
    private async Task MonChoBieuMauAsync(NhuCauInput model)
    {
        var me = await current.HienTaiAsync();
        var subjects = await service.MonDuocChonAsync();
        ViewData["AllowOtherSubjects"] = me.LoaiNguoi != "GiaSu";
        if (me.LoaiNguoi != "GiaSu" && model.MonHocIds.Count > 0)
            subjects.AddRange((await service.MonTenAsync()).Where(x => model.MonHocIds.Contains(x.Id)));
        ViewBag.Subjects = subjects.DistinctBy(x => x.Id).ToList();
    }
    public async Task<IActionResult> Index()
    {
        if ((await current.HienTaiAsync()).LoaiNguoi == "GiaSu") return RedirectToAction("Sua", "HoSoGiaSu");
        ViewBag.Subjects = await service.MonTenAsync();
        ViewBag.Linked = await service.DaKetNoiAsync();
        return View(await service.LayAsync());
    }
    public async Task<IActionResult> Sua(Guid? id)
    {
        var model = await service.LayDeSuaAsync(id);
        await MonChoBieuMauAsync(model);
        if (model.MonHocIds.Count > 0)
            model.ChonMonKhac = false;
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Luu(NhuCauInput m)
    {
        if (Request.HasFormContentType && !Request.Form.Keys.Any(key => key.StartsWith("LichRanh[")))
            m.LichRanh = [];
        if (ModelState.IsValid)
            try
            {
                await service.LuuAsync(m);
                Success();
                return RedirectToAction(nameof(Index));
            }
            catch (LoiNghiepVu ex) { ModelState.AddModelError("", ex.Message); }
        await MonChoBieuMauAsync(m);
        return View("Sua", m);
    }
    [HttpPost]
    public async Task<IActionResult> TrangThai(Guid id, string state, string rowVersion)
    {
        await service.TrangThaiAsync(id, state, rowVersion);
        Success();
        return RedirectToAction(nameof(Index));
    }
}
