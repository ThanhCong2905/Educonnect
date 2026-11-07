using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Areas.KetNoi.Controllers;

[Area("KetNoi")]
public sealed class KetNoiController(KetNoiService service, NhuCauService needs) : BaseController
{
    public async Task<IActionResult> Index()
    {
        var (cs, rs) = await service.LayAsync();
        ViewBag.Contacts = cs;
        ViewBag.Requests = rs;
        ViewBag.ConnectedClasses = await service.LopTheoLienHeAsync();
        ViewBag.Needs = await needs.LayAsync();
        return View();
    }
    public async Task<IActionResult> LienHe()
    {
        var (cs, rs) = await service.LayAsync();
        ViewBag.Chats = await service.HoiThoaiLienHeAsync();
        ViewBag.Subjects = await needs.MonTenAsync();
        ViewBag.Contacts = cs;
        ViewBag.Requests = rs;
        ViewBag.ConnectedClasses = await service.LopTheoLienHeAsync();
        ViewBag.Needs = await needs.LayAsync();
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> GuiLienHe(GuiLienHeInput m)
    {
        ValidateInput();
        await service.GuiLienHeAsync(m);
        Success("Đã gửi yêu cầu liên hệ.");
        return RedirectToAction(nameof(LienHe));
    }
    [HttpPost]
    public async Task<IActionResult> PhanHoiLienHe(Guid id, string action, string rowVersion)
    {
        await service.PhanHoiLienHeAsync(id, action, rowVersion);
        Success();
        return RedirectToAction(nameof(LienHe));
    }
    [HttpPost]
    public async Task<IActionResult> GuiYeuCau(GuiYeuCauViewModel m)
    {
        ValidateInput();
        var result = await service.GuiYeuCauAsync(m, await ActorAsync(), HttpContext.RequestAborted);
        if (!result.ThanhCong)
            throw new LoiNghiepVu(result.ThongBao!, result.MaTrangThai);
        Success();
        return RedirectToAction(nameof(Index));
    }
    private async Task ProposalData(Guid contact)
    {
        ViewBag.Subjects = await needs.MonDuocChonAsync();
        var all = await service.LayAsync();
        var c = all.Contacts.Single(x => x.Id == contact);
        ViewBag.Learner = c.BenHocId;
    }
    public async Task<IActionResult> DeXuat(Guid contact, Guid? id)
    {
        var m = await service.LayDeXuatAsync(contact, id);
        await ProposalData(contact);
        return View(m);
    }
    [HttpPost]
    [ActionName("DeXuat")]
    public async Task<IActionResult> GuiDeXuat(DeXuatLopViewModel m)
    {
        if (Request.HasFormContentType && !Request.Form.Keys.Any(key => key.StartsWith("LichRanh[")))
            m.LichRanh = [];
        if (ModelState.IsValid)
            try
            {
                var result = await service.GuiDeXuatAsync(m, await ActorAsync(), HttpContext.RequestAborted);
                if (!result.ThanhCong)
                    throw new LoiNghiepVu(result.ThongBao!, result.MaTrangThai);
                Success("Đã gửi đề xuất để bên học xác nhận.");
                return RedirectToAction(nameof(Index));
            }
            catch (LoiNghiepVu ex) { ModelState.AddModelError("", ex.Message); }
        await ProposalData(m.LienHeId);
        return View(m);
    }
    public async Task<IActionResult> ChiTiet(Guid id)
    {
        var r = await service.ChiTietAsync(id);
        ViewBag.Slots = await service.KhungAsync(id);
        return View(r);
    }
    [HttpPost]
    public async Task<IActionResult> ChapNhan(ChapNhanDeXuatViewModel vm)
    {
        var result = await service.ChapNhanDeXuatAsync(vm.MaYeuCau, vm.RowVersion, await ActorAsync(), HttpContext.RequestAborted);
        if (!result.ThanhCong)
            throw new LoiNghiepVu(result.ThongBao!, result.MaTrangThai);
        var c = result.DuLieu;
        Success("Lớp đã được tạo. Bạn có thể tiếp tục mở hoặc đóng nhu cầu tại mục Nhu cầu.");
        return Redirect($"/HocTap/LopHoc/ChiTiet/{c}");
    }
    [HttpPost]
    public async Task<IActionResult> PhanHoi(Guid id, string action, string rowVersion)
    {
        Rules.Check(action is "TuChoi" or "DaRut", "Phản hồi không hợp lệ.");
        return action == "TuChoi" ? await TuChoi(id, "", Rules.Version(rowVersion)) : await Rut(id, Rules.Version(rowVersion));
    }

    [HttpPost]
    public async Task<IActionResult> TuChoi(Guid maYeuCau, string lyDo, byte[] rowVersion)
    {
        (await service.TuChoiDeXuatAsync(maYeuCau, lyDo, rowVersion, await ActorAsync(), HttpContext.RequestAborted)).BaoLoiNeuThatBai();
        Success();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Rut(Guid maYeuCau, byte[] rowVersion)
    {
        (await service.RutYeuCauAsync(maYeuCau, rowVersion, await ActorAsync(), HttpContext.RequestAborted)).BaoLoiNeuThatBai();
        Success();
        return RedirectToAction(nameof(Index));
    }
    private async Task<Guid> ActorAsync() => (await HttpContext.RequestServices.GetRequiredService<NguoiDungHienTaiService>().HienTaiAsync(HttpContext.RequestAborted)).Id;

    [HttpPost]
    public async Task<IActionResult> HuyKetNoi(Guid maLop, string lyDo, byte[] rowVersion)
    {
        var result = await service.TaoYeuCauHuyAsync(maLop, lyDo, rowVersion, await ActorAsync(), HttpContext.RequestAborted);
        if (!result.ThanhCong)
            throw new LoiNghiepVu(result.ThongBao!, result.MaTrangThai);
        Success("Đã gửi yêu cầu kết thúc lớp.");
        return Redirect($"/HocTap/LopHoc/ChiTiet/{maLop}");
    }

    [HttpPost]
    public async Task<IActionResult> PhanHoiHuy(PhanHoiHuyViewModel vm)
    {
        ValidateInput();
        (await service.PhanHoiHuyAsync(vm.MaYeuCauHuy, vm.DongY, vm.RowVersion, await ActorAsync(), HttpContext.RequestAborted)).BaoLoiNeuThatBai();
        Success();
        return Redirect("/HocTap/LopHoc");
    }
}

