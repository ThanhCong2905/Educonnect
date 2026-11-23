using EduConnect.Mvc.Areas.QuanTri.Services;
using EduConnect.Mvc.Areas.QuanTri.ViewModels;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect.Mvc.Areas.QuanTri.Controllers;

[Area("QuanTri")]
public sealed class BaoCaoViPhamController(BaoCaoViPhamService service, QuanTriService admin, NguoiDungHienTaiService current) : BaseController
{
    public async Task<IActionResult> Index(int trang = 1, string? state = null, string? q = null, bool quanTri = false, Guid? nguoiGui = null)
    {
        ViewBag.QuanTri = quanTri;
        var page = await service.LayAsync(trang, state, q, quanTri, nguoiGui);
        if (quanTri)
        {
            ViewData["ReporterFilter"] = nguoiGui;
            ViewData["ReportAccounts"] = await service.LayTaiKhoanBaoCaoAsync(page.Items, nguoiGui);
        }
        return View(page);
    }

    public async Task<IActionResult> ChiTiet(Guid maBaoCao, string? quayLai = null)
    {
        ViewData["ReportListUrl"] = ReportListUrl(quayLai);
        return View(await service.ChiTietAsync(maBaoCao));
    }

    private string? ReportListUrl(string? url)
    {
        if (string.IsNullOrEmpty(url) || url.Length > 1000 || !Url.IsLocalUrl(url))
            return null;
        var path = url.Split('?', 2)[0].TrimEnd('/');
        return path.Equals("/QuanTri/BaoCaoViPham", StringComparison.OrdinalIgnoreCase) ||
            path.Equals("/QuanTri/BaoCaoViPham/Index", StringComparison.OrdinalIgnoreCase) ? url : null;
    }

    [HttpGet]
    public async Task<IActionResult> Gui(string loaiDoiTuong, Guid doiTuongId)
    {
        var model = new TaoBaoCaoViewModel { LoaiDoiTuong = loaiDoiTuong, DoiTuongId = doiTuongId };
        model.NoiDungGanKem = await service.XemTruocAsync(loaiDoiTuong, doiTuongId);
        return View(model);
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
            ModelState.AddModelError(result.Truong ?? "", result.ThongBao!);
        }
        try { model.NoiDungGanKem = await service.XemTruocAsync(model.LoaiDoiTuong, model.DoiTuongId); }
        catch (LoiNghiepVu) { }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> TepTinNhan(Guid maBaoCao, Guid id, bool xem = false)
    {
        var result = await service.TaiTepTinNhanAsync(maBaoCao, id);
        return TraTep(result.File, result.Path, xem);
    }

    [HttpPost]
    public async Task<IActionResult> BoSung(Guid id, string text, string rowVersion, bool xemLai = false, string? quayLai = null)
    {
        var reporter = await service.BoSungAsync(id, text, rowVersion, xemLai);
        Success("Đã gửi thông tin bổ sung.");
        return reporter ? RedirectToAction(nameof(ChiTiet), new
        {
            maBaoCao = id, quayLai = ReportListUrl(quayLai)
        }) : RedirectToAction(nameof(KetQua), new
        {
            id
        });
    }

    [HttpPost]
    public async Task<IActionResult> NhanXuLy(Guid id, string rowVersion, string? quayLai = null)
    {
        await admin.NhanXuLyAsync(id, rowVersion);
        Success("Bạn đã nhận xử lý báo cáo.");
        return RedirectToAction(nameof(ChiTiet), new
        {
            maBaoCao = id, quayLai = ReportListUrl(quayLai)
        });
    }

    [HttpPost]
    [Authorize(Roles = "QuanTri")]
    public async Task<IActionResult> PhanHoi(Guid id, string? text, string? quayLai = null)
    {
        var back = ReportListUrl(quayLai);
        try
        {
            await admin.PhanHoiNguoiBaoCaoAsync(id, text, HttpContext.RequestAborted);
            Success("Đã gửi phản hồi cho người báo cáo.");
            return RedirectToAction(nameof(ChiTiet), new { maBaoCao = id, quayLai = back });
        }
        catch (LoiNghiepVu error) when ((error.Status is 400 or 429) && !UiResponseFilter.IsAjax(Request))
        {
            ViewData["ReportListUrl"] = back;
            ViewData["ReplyError"] = error.Message;
            ViewData["ReplyText"] = text;
            return View("ChiTiet", await service.ChiTietAsync(id));
        }
    }

    [HttpPost]
    public async Task<IActionResult> XuLy(XuLyBaoCaoViewModel model, string? quayLai = null)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await admin.XuLyBaoCaoAsync(model);
                Success("Đã lưu kết quả xử lý.");
                return RedirectToAction(nameof(ChiTiet), new
                {
                    maBaoCao = model.Id, quayLai = ReportListUrl(quayLai)
                });
            }
            catch (LoiNghiepVu ex) { ModelState.AddModelError(ex.Truong ?? "", ex.Message); }
        }
        ViewData["ReportListUrl"] = ReportListUrl(quayLai);
        ViewBag.XuLy = model;
        return View("ChiTiet", await service.ChiTietAsync(model.Id));
    }

    public async Task<IActionResult> Tep(Guid id, bool xem = false)
    {
        var result = await service.TaiTepAsync(id);
        return TraTep(result.File, result.Path, xem);
    }

    public async Task<IActionResult> KetQua(Guid id) => View(model: await admin.KetQuaLienQuanAsync(id));
    public async Task<IActionResult> KiemDuyet() => View(await admin.CuaToiAsync());
}
