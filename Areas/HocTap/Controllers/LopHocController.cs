using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Areas.HocTap.Controllers;

[Area("HocTap")]
public sealed class LopHocController(LopHocService service, BaiTapService tasks, TinNhanService chat) : BaseController
{
    public async Task<IActionResult> Index() => View(await service.LayAsync());
    public async Task<IActionResult> ChiTiet(Guid id, string tab = "overview", string? month = null, string? state = null)
    {
        var c = await service.ChiTietAsync(id);
        ViewBag.Tab = tab;
        ViewBag.Month = month;
        ViewBag.State = state;
        ViewBag.Schedules = tab == "lessons" ? await service.LocBuoiAsync(id, month, state) : await service.LichAsync(id);
        ViewBag.Records = await service.BuoiAsync(id);
        ViewBag.Tasks = await tasks.LayAsync(id);
        ViewBag.Chat = await chat.TheoLopAsync(id);
        ViewBag.Cancellations = await service.HuyAsync(id);
        return View(c);
    }
    public async Task<IActionResult> ThanhVien(Guid id)
    {
        ViewBag.Class = await service.ChiTietAsync(id);
        return View(await service.ThanhVienAsync(id));
    }
    [HttpPost]
    public async Task<IActionResult> DeNghi(Guid id, Guid person)
    {
        await service.DeNghiAsync(id, person);
        Success();
        return RedirectToAction(nameof(ThanhVien), new
        {
            id
        });
    }
    [HttpPost]
    public async Task<IActionResult> Duyet(Guid id, Guid member, string action, string permission, string rowVersion)
    {
        await service.DuyetAsync(member, action, permission, rowVersion);
        Success();
        return RedirectToAction(nameof(ThanhVien), new
        {
            id
        });
    }
    [HttpPost]
    public async Task<IActionResult> KetThuc(Guid id, string rowVersion)
    {
        await service.KetThucAsync(id, rowVersion);
        Success();
        return RedirectToAction(nameof(ChiTiet), new
        {
            id
        });
    }
    [HttpPost]
    public async Task<IActionResult> YeuCauHuy(Guid id, string reason)
    {
        await service.YeuCauHuyAsync(id, reason);
        Success();
        return RedirectToAction(nameof(ChiTiet), new
        {
            id
        });
    }
    [HttpPost]
    public async Task<IActionResult> PhanHoiHuy(Guid id, Guid request, bool agree, string rowVersion)
    {
        await service.PhanHoiHuyAsync(request, agree, rowVersion);
        Success();
        return RedirectToAction(nameof(ChiTiet), new
        {
            id
        });
    }
}

