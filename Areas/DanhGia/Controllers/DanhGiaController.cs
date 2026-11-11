using EduConnect.Mvc.Areas.DanhGia.Services;
using EduConnect.Mvc.Areas.DanhGia.ViewModels;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect.Mvc.Areas.DanhGia.Controllers;

[Area("DanhGia")]
public sealed class DanhGiaController(DanhGiaService service, TienDoService progress, NguoiDungHienTaiService current) : BaseController
{
    public async Task<IActionResult> DanhSach(Guid maLop, DateTimeOffset? tuNgay = null, DateTimeOffset? denNgay = null, int trang = 1)
        => View(await progress.LayAsync(maLop, tuNgay, denNgay, trang));

    [HttpGet]
    public async Task<IActionResult> TaoDot(Guid maLop)
    {
        _ = await progress.LayAsync(maLop);
        return View(new TaoDotPhanHoiViewModel { MaLop = maLop, TuNgay = DateTimeOffset.Now.AddDays(-30), DenNgay = DateTimeOffset.Now, HanGui = DateTimeOffset.Now.AddDays(7) });
    }

    [HttpPost]
    public async Task<IActionResult> TaoDot(TaoDotPhanHoiViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await service.TaoDotAsync(model);
                Success("Đã mở đợt phản hồi.");
                return RedirectToAction(nameof(DanhSach), new
                {
                    maLop = model.MaLop
                });
            }
            catch (LoiNghiepVu ex) { ModelState.AddModelError(ex.Truong ?? "", ex.Message); }
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Gui(Guid maLop, Guid dotPhanHoiId)
    {
        var me = await current.HienTaiAsync();
        var page = await progress.LayAsync(maLop, dot: dotPhanHoiId);
        Rules.Check(page.CoThePhanHoi, "Bạn chưa có quyền gửi phản hồi trong lớp.", 403);
        Rules.Check(page.Dot.Any(x => x.Id == dotPhanHoiId), "Đợt phản hồi không thuộc lớp này.", 404);
        var old = page.Phieu.SingleOrDefault(x => x.DotPhanHoiId == dotPhanHoiId && x.NguoiGuiId == me.Id);
        return View(new GuiDanhGiaViewModel { MaLop = maLop, DotPhanHoiId = dotPhanHoiId, NoiDung = old?.NoiDung ?? "", MucDoHaiLong = old?.MucDoHaiLong, RowVersion = old == null ? null : Convert.ToBase64String(old.RowVersion) });
    }

    [HttpPost]
    public async Task<IActionResult> Gui(GuiDanhGiaViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await service.GuiAsync(model, (await current.HienTaiAsync()).Id, HttpContext.RequestAborted);
            if (result.ThanhCong)
            {
                Success("Đã lưu phản hồi.");
                return RedirectToAction(nameof(DanhSach), new
                {
                    maLop = model.MaLop
                });
            }
            ModelState.AddModelError("", result.ThongBao!);
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DongDot(Guid id, Guid maLop, string rowVersion)
    {
        await service.DongDotAsync(id, rowVersion);
        Success("Đã đóng đợt phản hồi.");
        return RedirectToAction(nameof(DanhSach), new
        {
            maLop
        });
    }

    [HttpGet]
    public async Task<IActionResult> QuyetDinh(Guid maLop, Guid dotPhanHoiId)
    {
        var page = await progress.LayAsync(maLop, dot: dotPhanHoiId);
        Rules.Check(page.Dot.Any(x => x.Id == dotPhanHoiId), "Đợt phản hồi không thuộc lớp.", 404);
        var me = await current.HienTaiAsync();
        var value = page.QuyetDinh.SingleOrDefault(x => x.DotPhanHoiId == dotPhanHoiId && x.NguoiQuyetDinhId == me.Id);
        return View(new QuyetDinhPhanHoiViewModel { DotPhanHoiId = dotPhanHoiId, QuyetDinh = value?.QuyetDinh ?? "TiepTuc", GhiChu = value?.GhiChu, RowVersion = value == null ? null : Convert.ToBase64String(value.RowVersion) });
    }

    [HttpPost]
    public async Task<IActionResult> QuyetDinh(QuyetDinhPhanHoiViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var lop = await service.QuyetDinhAsync(model);
                Success("Đã ghi nhận hướng xử lý.");
                if (model.QuyetDinh == "Dung")
                    return RedirectToAction("HuyKetNoi", "LopHoc", new
                    {
                        area = "HocTap",
                        id = lop
                    });
                if (model.QuyetDinh == "DieuChinh")
                    return RedirectToAction("ChiTiet", "LopHoc", new
                    {
                        area = "HocTap",
                        id = lop,
                        tab = "chat"
                    });
                return RedirectToAction(nameof(DanhSach), new
                {
                    maLop = lop
                });
            }
            catch (LoiNghiepVu ex) { ModelState.AddModelError(ex.Truong ?? "", ex.Message); }
        }
        return View(model);
    }

    public async Task<IActionResult> LichSu(Guid id) => View(await service.LichSuAsync(id));
}
