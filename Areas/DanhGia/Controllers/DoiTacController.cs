using EduConnect.Mvc.Areas.DanhGia.Services;
using EduConnect.Mvc.Areas.DanhGia.ViewModels;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect.Mvc.Areas.DanhGia.Controllers;

[Area("DanhGia")]
public sealed class DoiTacController(DanhGiaDoiTacService service) : BaseController
{
    public async Task<IActionResult> Xem(Guid id)
    {
        return RedirectToAction(nameof(ChiTiet), new
        {
            id = await service.NguoiNhanAsync(id)
        });
    }
    public async Task<IActionResult> ChiTiet(Guid id, int trang = 1) => View(await service.LayAsync(id, trang));

    [HttpGet]
    public async Task<IActionResult> Gui(Guid maLop) => View(await service.LayDeSuaAsync(maLop));

    [HttpPost]
    public async Task<IActionResult> Gui(DanhGiaDoiTacViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var person = await service.GuiAsync(model);
                Success("Đã lưu đánh giá.");
                return RedirectToAction(nameof(ChiTiet), new
                {
                    id = person
                });
            }
            catch (LoiNghiepVu ex) { ModelState.AddModelError(ex.Truong ?? "", ex.Message); }
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> PhanHoi(PhanHoiDoiTacViewModel model)
    {
        ValidateInput();
        var person = await service.PhanHoiAsync(model);
        Success("Đã lưu phản hồi.");
        return RedirectToAction(nameof(ChiTiet), new
        {
            id = person
        });
    }

    public async Task<IActionResult> LichSu(Guid id) => View(await service.LichSuAsync(id));
}
