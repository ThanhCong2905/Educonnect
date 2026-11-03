using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Areas.TimKiem.Controllers;

[Area("TimKiem")]
public sealed class TimKiemController(TimKiemService service, NhuCauService needs, KetNoiService contacts, NguoiDungMauService current) : BaseController
{
    public Task<IActionResult> Index(BoLocTimKiemViewModel f) => Tim(f);

    public async Task<IActionResult> Tim(BoLocTimKiemViewModel boLoc)
    {
        ValidateInput();
        ViewBag.Subjects = await needs.MonAsync();
        var result = await service.TimKiemAsync(boLoc, (await current.HienTaiAsync()).Id, HttpContext.RequestAborted);
        var page = result.DuLieu;
        return View("Index", new SearchPage { Filter = boLoc, Items = page.Items, Total = page.Total });
    }
    public async Task<IActionResult> ChiTiet(Guid id)
    {
        var n = await service.ChiTietAsync(id);
        if (n.LoaiNguoi == "GiaSu")
            return RedirectToAction("ChiTiet", "HoSoGiaSu", new { area = "NhuCau", maHoSo = id });
        var contact = await contacts.LienHeVoiAsync(n.ChuSoHuuId);
        ViewBag.Contact = contact;
        ViewBag.ContactChat = contact?.TrangThai == "ChapNhan" ? await contacts.HoiThoaiAsync(contact.Id) : null;
        ViewBag.Related = await service.LienQuanAsync(n.ChuSoHuuId);
        return View(n);
    }
    public async Task<IActionResult> SoSanh([FromQuery(Name = "ids")] IReadOnlyList<Guid> maDoiTuong)
    {
        ValidateInput();
        var result = await service.SoSanhAsync(maDoiTuong, (await current.HienTaiAsync()).Id, HttpContext.RequestAborted);
        return View(result.DuLieu.Items);
    }
}
