using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect.Mvc.Areas.HocTap.Controllers;

[Area("HocTap")]
public sealed class LienKetTaiKhoanController(LienKetTaiKhoanService service) : BaseController
{
    public async Task<IActionResult> Index() => View(await service.LayLienKetAsync());

    [HttpPost]
    public async Task<IActionResult> GuiDeNghi(string email)
    {
        await service.GuiDeNghiAsync(email);
        Success("Đã gửi đề nghị liên kết.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> PhanHoi(Guid id, bool dongY, string rowVersion)
    {
        await service.PhanHoiAsync(id, dongY, rowVersion);
        Success(dongY ? "Đã liên kết tài khoản." : "Đã từ chối đề nghị.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Huy(Guid id, string rowVersion)
    {
        await service.HuyAsync(id, rowVersion);
        Success("Đã kết thúc liên kết.");
        return RedirectToAction(nameof(Index));
    }
}
