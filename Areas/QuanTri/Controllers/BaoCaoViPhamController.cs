using EduConnect.Mvc.Areas.QuanTri.Services;
using EduConnect.Mvc.Areas.QuanTri.ViewModels;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect.Mvc.Areas.QuanTri.Controllers;

[Area("QuanTri")]
public sealed class BaoCaoViPhamController(BaoCaoViPhamService service, QuanTriService admin, NguoiDungHienTaiService current) : BaseController
{
    public async Task<IActionResult> Index(int trang = 1, string? state = null, string? q = null, bool quanTri = false)
    {
        ViewBag.QuanTri = quanTri;
        return View(await service.LayAsync(trang, state, q, quanTri));
    }

    public async Task<IActionResult> ChiTiet(Guid maBaoCao) => View(await service.ChiTietAsync(maBaoCao));

    [HttpGet]
    public async Task<IActionResult> Gui(string loaiDoiTuong, Guid doiTuongId)
    {
        _ = await service.DoiTuongAsync(loaiDoiTuong, doiTuongId);
        return View(new TaoBaoCaoViewModel { LoaiDoiTuong = loaiDoiTuong, DoiTuongId = doiTuongId });
    }

    [HttpPost]
    [RequestSizeLimit(51000000)]
    public async Task<IActionResult> Gui(TaoBaoCaoViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await service.GuiAsync(model, (await current.HienTaiAsync()).Id, HttpContext.RequestAborted);
            if (result.ThanhCong)
            {
                Success("Đã tiếp nhận báo cáo.");
                return RedirectToAction(nameof(ChiTiet), new
                {
                    maBaoCao = result.DuLieu
                });
            }
            ModelState.AddModelError("", result.ThongBao!);
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> BoSung(Guid id, string text, string rowVersion, bool xemLai = false)
    {
        var reporter = await service.BoSungAsync(id, text, rowVersion, xemLai);
        Success("Đã gửi thông tin bổ sung.");
        return reporter ? RedirectToAction(nameof(ChiTiet), new
        {
            maBaoCao = id
        }) : RedirectToAction(nameof(KetQua), new
        {
            id
        });
    }

    [HttpPost]
    public async Task<IActionResult> NhanXuLy(Guid id, string rowVersion)
    {
        await admin.NhanXuLyAsync(id, rowVersion);
        Success("Bạn đã nhận xử lý báo cáo.");
        return RedirectToAction(nameof(ChiTiet), new
        {
            maBaoCao = id
        });
    }

    [HttpPost]
    public async Task<IActionResult> XuLy(XuLyBaoCaoViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await admin.XuLyBaoCaoAsync(model);
                Success("Đã lưu kết quả xử lý.");
                return RedirectToAction(nameof(ChiTiet), new
                {
                    maBaoCao = model.Id
                });
            }
            catch (LoiNghiepVu ex) { ModelState.AddModelError(ex.Truong ?? "", ex.Message); }
        }
        ViewBag.XuLy = model;
        return View("ChiTiet", await service.ChiTietAsync(model.Id));
    }

    public async Task<IActionResult> Tep(Guid id)
    {
        var result = await service.TaiTepAsync(id);
        return PhysicalFile(result.Path, result.File.LoaiNoiDung, result.File.TenGoc);
    }

    public async Task<IActionResult> KetQua(Guid id) => View(model: await admin.KetQuaLienQuanAsync(id));
    public async Task<IActionResult> KiemDuyet() => View(await admin.CuaToiAsync());
}
