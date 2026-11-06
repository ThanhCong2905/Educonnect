using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using EduConnect.Mvc.Areas.KetNoi.Services;
namespace EduConnect.Mvc.Areas.NhuCau.Controllers;

[Area("NhuCau")]
[RequestSizeLimit(52000000)]
public sealed class HoSoGiaSuController(HoSoGiaSuService service, NhuCauService needs, BangCapService certificates, EduConnect.Mvc.Areas.TimKiem.Services.TimKiemService search, KetNoiService contacts, NguoiDungHienTaiService current) : BaseController
{
    public async Task<IActionResult> ChiTiet(Guid maHoSo)
    {
        var result = await service.LayHoSoAsync(maHoSo, (await current.HienTaiAsync()).Id, HttpContext.RequestAborted);
        var profile = result.DuLieu;
        ViewData["PublicProfile"] = true;
        var contact = await contacts.LienHeVoiAsync(profile.ChuSoHuuId);
        ViewBag.Contact = contact;
        ViewBag.ContactChat = contact?.TrangThai == "ChapNhan" ? await contacts.HoiThoaiAsync(contact.Id) : null;
        return View("~/Areas/TimKiem/Views/TimKiem/ChiTiet.cshtml", profile);
    }

    [HttpPost]
    public async Task<IActionResult> TrangThai(string operation, string rowVersion)
    {
        await service.DoiTrangThaiAsync(operation,rowVersion);
        Success();return RedirectToAction(nameof(Sua));
    }
    public async Task<IActionResult> XemTruoc()
    {
        ViewBag.Preview=true;ViewBag.Related=new List<EduConnect.Mvc.Areas.TimKiem.ViewModels.UngVienViewModel>();
        return View("~/Areas/TimKiem/Views/TimKiem/ChiTiet.cshtml",await search.XemTruocAsync());
    }
    public async Task<IActionResult> Sua()
    {
        ViewBag.Certificates = await certificates.CuaToiAsync();
        ViewBag.Subjects = (await needs.MonAsync()).Concat(await needs.MonDuocChonAsync()).DistinctBy(x => x.Id).OrderBy(x => x.TenMon).ToList();
        return View(await service.LayDeSuaAsync());
    }
    [HttpPost]
    public async Task<IActionResult> Luu(HoSoInput m)
    {
        if (ModelState.IsValid)
            try
            {
                await service.CapNhatAsync(m);
                Success();
                return RedirectToAction(nameof(Sua));
            }
            catch (LoiNghiepVu ex) { ModelState.AddModelError("", ex.Message); }
        ViewBag.Certificates = await certificates.CuaToiAsync();
        ViewBag.Subjects = (await needs.MonAsync()).Concat(await needs.MonDuocChonAsync()).DistinctBy(x => x.Id).OrderBy(x => x.TenMon).ToList();
        return View("Sua", m);
    }
    [HttpPost]
    public async Task<IActionResult> TaiBangCap(List<IFormFile> files)
    {
        await certificates.TaiLenAsync(files);
        Success("Đã tải bằng cấp lên hồ sơ.");
        return RedirectToAction(nameof(Sua));
    }
    public async Task<IActionResult> BangCap(Guid id)
    {
        var (f, path) = await certificates.TaiAsync(id);
        Response.Headers.XContentTypeOptions = "nosniff";
        return PhysicalFile(path, f.LoaiNoiDung, f.TenGoc);
    }
    [HttpPost]
    public async Task<IActionResult> XoaBangCap(Guid id)
    {
        await certificates.XoaAsync(id);
        Success("Đã xóa tệp.");
        return RedirectToAction(nameof(Sua));
    }
}
