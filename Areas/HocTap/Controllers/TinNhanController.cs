using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Areas.HocTap.Controllers;

[Area("HocTap")]
public sealed class TinNhanController(TinNhanService service) : BaseController
{
    public async Task<IActionResult> Lop(Guid id, string? message, int page = 1)
    {
        var room = await service.KiemTraAsync(id);
        if (room.LoaiHoiThoai != "TheoLop")
            return RedirectToAction(nameof(Index), new
            {
                id
            });
        ViewBag.InClass = true;
        ViewBag.CanSend = await service.CoTheGuiAsync(id);
        ViewBag.Conversations = new List<EduConnect.Mvc.Models.CuocHoiThoai>();
        ViewBag.Conversation = room;
        ViewBag.MessageQuery = message;
        ViewBag.Page = page;
        var messages=await service.LichSuAsync(id,message,page);
        ViewBag.Attachments=await service.TepTheoTinAsync(id,messages.Select(m=>m.Id).ToList());
        return View("Index", messages);
    }
    public async Task<IActionResult> Index(Guid? id, string? q, string? message, int page = 1)
    {
        if (id.HasValue)
        {
            var selected = await service.KiemTraAsync(id.Value);
            if (selected.LoaiHoiThoai == "TheoLop")
                return RedirectToAction("ChiTiet", "LopHoc", new
                {
                    id = selected.LopHocId,
                    tab = "chat"
                });
        }
        ViewBag.InClass = false;
        ViewBag.CanSend = true;
        var cs = await service.LayAsync(q);
        ViewBag.Conversations = cs;
        ViewBag.Query = q;
        ViewBag.MessageQuery = message;
        ViewBag.Page = page;
        if (id == null)
            id = cs.FirstOrDefault()?.Id;
        ViewBag.Conversation = id == null ? null : await service.KiemTraAsync(id.Value);
        var messages=id==null?new List<EduConnect.Mvc.Models.TinNhan>():await service.LichSuAsync(id.Value,message,page);
        ViewBag.Attachments=id==null?new Dictionary<Guid,List<EduConnect.Mvc.Models.TepDinhKem>>():await service.TepTheoTinAsync(id.Value,messages.Select(m=>m.Id).ToList());
        return View(messages);
    }
    [HttpGet]
    public async Task<IActionResult> DuLieu(Guid id, Guid? before, Guid? after, string? message)
    {
        ValidateInput();
        return Json(await service.DuLieuAsync(id, before, after, message));
    }
    [HttpPost]
    [RequestSizeLimit(52000000)]
    public async Task<IActionResult> Gui(GuiTinNhanViewModel m)
    {
        ValidateInput();
        await service.GuiAsync(m);
        var room = await service.KiemTraAsync(m.MaHoiThoai);
        return RedirectToAction(room.LoaiHoiThoai == "TheoLop" ? "Lop" : nameof(Index), new
        {
            id = m.MaHoiThoai
        });
    }
    [HttpGet]
    public async Task<IActionResult> Tai(Guid id,bool xem=false)
    {
        var (file,path)=await service.TaiTepAsync(id);
        Response.Headers.XContentTypeOptions="nosniff";
        Response.Headers.CacheControl="private, no-store";
        if(xem&&file.LoaiNoiDung is "image/jpeg" or "image/png")return PhysicalFile(path,file.LoaiNoiDung);
        return PhysicalFile(path,file.LoaiNoiDung,file.TenGoc);
    }
}
