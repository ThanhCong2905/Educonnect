using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Areas.HocTap.Controllers;

[Area("HocTap")]
public sealed class LopHocController(LopHocService service, BaiTapService tasks, TinNhanService chat) : BaseController
{
    public async Task<IActionResult> Index()
    {
        ViewBag.Invitations = await service.LoiMoiAsync();
        return View(await service.LayAsync());
    }

    [HttpGet]
    public async Task<IActionResult> MoiThanhVien(Guid id, string email)
        => View(await service.TimNguoiDuocMoiAsync(id, email));

    [HttpGet]
    public async Task<IActionResult> LoiMoi(Guid id)
        => View((await service.LoiMoiAsync()).FirstOrDefault(x => x.Id == id));

    [HttpPost]
    public async Task<IActionResult> PhanHoiLoiMoi(Guid id, bool agree, string rowVersion)
    {
        await service.PhanHoiLoiMoiAsync(id, agree, rowVersion);
        Success(agree ? "Bạn đã đồng ý lời mời. Đang chờ gia sư duyệt quyền tham gia." : "Bạn đã từ chối lời mời.");
        return RedirectToAction(nameof(Index));
    }
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
    [HttpGet]
    public async Task<IActionResult> HuyKetNoi(Guid id)
    {
        var lop = await service.ChiTietAsync(id);
        ViewBag.Cancellations = await service.HuyAsync(id);
        return View(lop);
    }
    [HttpPost]
    public async Task<IActionResult> DeNghi(Guid id, string email)
    {
        await service.DeNghiAsync(id, email);
        Success("Đã gửi lời mời. Người nhận cần đồng ý trước khi gia sư duyệt.");
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
    public async Task<IActionResult> YeuCauHuy(Guid id, string reason, string rowVersion)
    {
        await service.YeuCauHuyAsync(id, reason, rowVersion, HttpContext.RequestAborted);
        Success();
        return RedirectToAction(nameof(ChiTiet), new
        {
            id
        });
    }
    [HttpPost]
    public async Task<IActionResult> PhanHoiHuy(Guid id, Guid request, bool agree, string rowVersion)
    {
        await service.PhanHoiHuyAsync(request, agree, rowVersion, HttpContext.RequestAborted);
        Success();
        return RedirectToAction(nameof(ChiTiet), new
        {
            id
        });
    }
    [HttpPost]
    public async Task<IActionResult> XacNhanKetThuc(Guid maYeuCau, byte[] rowVersion)
    {
        var actor = await HttpContext.RequestServices.GetRequiredService<EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService>().HienTaiAsync(HttpContext.RequestAborted);
        (await service.XacNhanKetThucAsync(maYeuCau, rowVersion, actor.Id, HttpContext.RequestAborted)).BaoLoiNeuThatBai();
        Success();
        return RedirectToAction(nameof(Index));
    }
}

