using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
namespace EduConnect.Mvc.Areas.TimKiem.Controllers;

[Area("TimKiem")]
public sealed class TimKiemController(TimKiemService service, NhuCauService needs, KetNoiService contacts, NguoiDungHienTaiService current) : BaseController
{
    [AllowAnonymous, HttpGet]
    public Task<IActionResult> Index(BoLocTimKiemViewModel f) => Tim(f);

    [AllowAnonymous, HttpGet]
    public Task<IActionResult> GiaSu(BoLocTimKiemViewModel f)
    {
        if (User.IsInRole("GiaSu"))
            return Task.FromResult<IActionResult>(RedirectToAction(nameof(NhuCau), f));
        f.LoaiDoiTuong = "GiaSu";
        return Tim(f);
    }

    [AllowAnonymous, HttpGet]
    public Task<IActionResult> NhuCau(BoLocTimKiemViewModel f)
    {
        f.LoaiDoiTuong = "NhuCau";
        return Tim(f);
    }

    [AllowAnonymous, HttpGet]
    public async Task<IActionResult> Tim(BoLocTimKiemViewModel boLoc)
    {
        ValidateInput();
        ViewBag.Subjects = await needs.MonAsync();
        var result = await service.TimKiemAsync(boLoc, (await current.NguoiXemAsync())?.Id, HttpContext.RequestAborted);
        var page = result.DuLieu;
        ViewData["SearchType"] = boLoc.LoaiDoiTuong;
        return View("Index", new SearchPage { Filter = boLoc, Items = page.Items, Total = page.Total });
    }
    [AllowAnonymous, HttpGet]
    public async Task<IActionResult> ChiTiet(Guid id)
    {
        try
        {
            var n = await service.ChiTietAsync(id);
            if (n.LoaiNguoi == "GiaSu")
                return RedirectToAction("ChiTiet", "HoSoGiaSu", new { area = "NhuCau", maHoSo = id });
            var contact = User.Identity?.IsAuthenticated == true ? await contacts.LienHeVoiAsync(n.ChuSoHuuId) : null;
            ViewBag.Contact = contact;
            ViewBag.ContactChat = contact?.TrangThai == "ChapNhan" ? await contacts.HoiThoaiAsync(contact.Id) : null;
            ViewBag.Related = await service.LienQuanAsync(n.ChuSoHuuId, n.Id);
            ViewData["SearchType"] = "NhuCau";
            return View(n);
        }
        catch (LoiNghiepVu ex) when (ex.Status == 401)
        {
            return Challenge(new AuthenticationProperties { RedirectUri = Request.Path + Request.QueryString });
        }
    }
    [AllowAnonymous, HttpGet]
    public async Task<IActionResult> SoSanh([FromQuery(Name = "ids")] IReadOnlyList<Guid> maDoiTuong)
    {
        ValidateInput();
        var result = await service.SoSanhAsync(maDoiTuong, (await current.NguoiXemAsync())?.Id, HttpContext.RequestAborted);
        var items = result.DuLieu.Items;
        ViewData["SearchType"] = items.FirstOrDefault()?.LoaiNguoi == "GiaSu" ? "GiaSu" : "NhuCau";
        return View(items);
    }
}
