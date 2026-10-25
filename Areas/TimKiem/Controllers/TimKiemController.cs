using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Areas.TimKiem.Controllers;

[Area("TimKiem")]
public sealed class TimKiemController(TimKiemService service, NhuCauService needs, KetNoiService contacts) : BaseController
{
    public async Task<IActionResult> Index(BoLocTimKiemViewModel f)
    {
        ValidateInput();
        ViewBag.Subjects = await needs.MonAsync();
        return View(await service.TimAsync(f));
    }
    public async Task<IActionResult> ChiTiet(Guid id)
    {
        var n = await service.ChiTietAsync(id);
        var contact = await contacts.LienHeVoiAsync(n.ChuSoHuuId);
        ViewBag.Contact = contact;
        ViewBag.ContactChat = contact?.TrangThai == "ChapNhan" ? await contacts.HoiThoaiAsync(contact.Id) : null;
        ViewBag.Related = await service.LienQuanAsync(n.ChuSoHuuId);
        return View(n);
    }
    public async Task<IActionResult> SoSanh(Guid[] ids) => View(await service.SoSanhAsync(ids));
}
