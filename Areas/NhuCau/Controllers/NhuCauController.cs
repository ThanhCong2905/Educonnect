using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Areas.NhuCau.Controllers;

[Area("NhuCau")]
[Microsoft.AspNetCore.Authorization.Authorize(Roles = "HocSinh,PhuHuynh")]
public sealed class NhuCauController(NhuCauService service, NguoiDungHienTaiService current) : BaseController
{
    private async Task MonChoBieuMauAsync(NhuCauInput model)
    {
        var me = await current.HienTaiAsync();
        var subjects = await service.MonDuocChonAsync();
        ViewData["AllowOtherSubjects"] = me.LoaiNguoi != "GiaSu";
        if (me.LoaiNguoi != "GiaSu" && model.MonHocIds.Count > 0)
            subjects.AddRange((await service.MonTenAsync()).Where(x => model.MonHocIds.Contains(x.Id)));
        ViewBag.Subjects = subjects.DistinctBy(x => x.Id).ToList();
    }
    public async Task<IActionResult> Index()
    {
        if ((await current.HienTaiAsync()).LoaiNguoi == "GiaSu") return RedirectToAction("Sua", "HoSoGiaSu");
        ViewBag.Subjects = await service.MonTenAsync();
        ViewBag.Linked = await service.DaKetNoiAsync();
        ViewBag.Slots = await service.LichRanhAsync();
        return View(await service.LayAsync());
    }
    [HttpGet]
    public async Task<IActionResult> Tao()
    {
        Rules.Check((await current.HienTaiAsync()).LoaiNguoi is "HocSinh" or "PhuHuynh", "Mở Hồ sơ của tôi để quản lý môn dạy.", 403);
        var model = new NhuCauInput();
        await MonChoBieuMauAsync(model);
        return View("Tao", model);
    }

    [HttpGet]
    public async Task<IActionResult> Sua(Guid? id)
    {
        if (!ModelState.IsValid || id == null || id == Guid.Empty)
            return NotFound();
        var result = await service.LayDeSuaAsync(id.Value);
        if (!result.ThanhCong)
        {
            if (result.MaTrangThai == StatusCodes.Status404NotFound)
                return NotFound();
            return StatusCode(result.MaTrangThai, result.ThongBao);
        }
        var model = result.DuLieu;
        await MonChoBieuMauAsync(model);
        if (model.MonHocIds.Count > 0)
            model.ChonMonKhac = false;
        return View(model);
    }
    [HttpPost]
    public Task<IActionResult> TaoNhuCauHoc(TaoNhuCauBenHocViewModel vm)
        => LuuBieuMauAsync(vm, async () => await service.TaoNhuCauHocAsync(vm,
            (await current.HienTaiAsync()).Id, HttpContext.RequestAborted));

    [HttpPost]
    public Task<IActionResult> CapNhat(Guid maNhuCau, TaoNhuCauBenHocViewModel vm)
    {
        vm.MaNhuCau ??= maNhuCau;
        return LuuBieuMauAsync(vm, async () => await service.CapNhatAsync(maNhuCau, vm,
            vm.RowVersion ?? [], (await current.HienTaiAsync()).Id, HttpContext.RequestAborted));
    }

    private async Task<IActionResult> LuuBieuMauAsync(TaoNhuCauBenHocViewModel vm, Func<Task<KetQua<Guid>>> save)
    {
        if (Request.HasFormContentType && !Request.Form.Keys.Any(key => key.StartsWith("LichRanh[")))
            vm.LichRanh = [];
        if (ModelState.IsValid)
        {
            var result = await save();
            if (result.ThanhCong)
            {
                Success();
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", result.ThongBao!);
        }
        var model = vm.ToInput();
        await MonChoBieuMauAsync(model);
        return View(model.Id == null ? "Tao" : "Sua", model);
    }

    [HttpPost]
    public async Task<IActionResult> Luu(NhuCauInput m)
    {
        if (Request.HasFormContentType && !Request.Form.Keys.Any(key => key.StartsWith("LichRanh[")))
            m.LichRanh = [];
        if (ModelState.IsValid)
            try
            {
                await service.LuuAsync(m);
                Success();
                return RedirectToAction(nameof(Index));
            }
            catch (LoiNghiepVu ex) { ModelState.AddModelError("", ex.Message); }
        await MonChoBieuMauAsync(m);
        return View(m.Id == null ? "Tao" : "Sua", m);
    }
    [HttpPost]
    public async Task<IActionResult> TrangThai(Guid id, string state, string rowVersion)
    {
        await service.TrangThaiAsync(id, state, rowVersion);
        Success();
        return RedirectToAction(nameof(Index));
    }
}
