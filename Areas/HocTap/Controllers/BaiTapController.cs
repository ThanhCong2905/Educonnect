using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace EduConnect.Mvc.Areas.HocTap.Controllers;

[Area("HocTap")]
[RequestSizeLimit(52000000)]
public sealed class BaiTapController(BaiTapService service, LopHocService classes, TepService files, QuyenLopHocService rights) : BaseController
{
    public async Task<IActionResult> Giao(Guid id, Guid? buoi)
    {
        await rights.KiemTraQuyenGhiAsync(id, true);
        await NapBuoiAsync(id);
        return View(new GiaoBaiTapViewModel { LopHocId = id, BuoiHocId = buoi });
    }
    [HttpPost]
    public async Task<IActionResult> Giao(GiaoBaiTapViewModel m)
    {
        if (ModelState.IsValid)
            try
            {
                await service.GiaoAsync(m);
                Success();
                if (m.BuoiHocId.HasValue)
                    return RedirectToAction("Buoi", "LichHoc", new { id = await service.LichCuaBuoiAsync(m.BuoiHocId.Value) });
                return RedirectToAction("ChiTiet", "LopHoc", new
                {
                    id = m.LopHocId,
                    tab = "assignments"
                });
            }
            catch (Exception ex) when (Rules.ErrorStatus(ex) < 500) { ModelState.AddModelError((ex as LoiNghiepVu)?.Truong ?? "", Rules.ErrorMessage(ex)); Response.StatusCode = Rules.ErrorStatus(ex); }
        await rights.KiemTraQuyenGhiAsync(m.LopHocId, true);
        await NapBuoiAsync(m.LopHocId);
        return View(m);
    }
    [HttpPost]
    public async Task<IActionResult> CongBo(Guid id, string rowVersion)
    {
        await service.CongBoAsync(id, rowVersion);
        var task = await service.ChiTietAsync(id);
        Success("Đã giao bài tập.");
        return RedirectToAction("Buoi", "LichHoc", new { id = await service.LichCuaBuoiAsync(task.BuoiHocId!.Value) });
    }
    [HttpGet]
    public async Task<IActionResult> GanBuoi(Guid id)
    {
        var task = await service.ChiTietAsync(id);
        await rights.KiemTraQuyenGhiAsync(task.LopHocId, true);
        await NapBuoiAsync(task.LopHocId);
        return View(task);
    }
    private async Task NapBuoiAsync(Guid lop)
    {
        var lessons = (await classes.BuoiAsync(lop)).Where(x => x.TrangThai is "DuKien" or "DangDienRa" or "DaHoc")
            .ToDictionary(x => x.LichHocId);
        ViewBag.Lessons = (await classes.LichAsync(lop))
            .Where(x => lessons.ContainsKey(x.Id) && x.TrangThai == "DaLenLich")
            .Select(x => new SelectListItem($"{x.TieuDe} · {Rules.Date(x.BatDauLuc)}", lessons[x.Id].Id.ToString())).ToList();
    }

    [HttpPost]
    public async Task<IActionResult> GanBuoi(Guid id, Guid buoiHocId, string rowVersion)
    {
        try
        {
            await service.GanBuoiAsync(id, buoiHocId, rowVersion);
            Success("Đã gắn bài tập với buổi học.");
            return RedirectToAction("Buoi", "LichHoc", new { id = await service.LichCuaBuoiAsync(buoiHocId) });
        }
        catch (Exception ex) when (Rules.ErrorStatus(ex) < 500)
        {
            ModelState.AddModelError((ex as LoiNghiepVu)?.Truong ?? "", Rules.ErrorMessage(ex));
            Response.StatusCode = Rules.ErrorStatus(ex);
            return await GanBuoi(id);
        }
    }
    public async Task<IActionResult> ChiTiet(Guid id)
    {
        var a = await service.ChiTietAsync(id);
        ViewBag.Class = await classes.ChiTietAsync(a.LopHocId);
        ViewBag.Submissions = await service.BaiNopAsync(id);
        ViewBag.Results = await service.KetQuaAsync(id);
        ViewBag.Files = await files.DanhSachAsync(id);
        ViewBag.Members = await classes.ThanhVienAsync(a.LopHocId);
        ViewBag.ScheduleId = a.BuoiHocId.HasValue ? await service.LichCuaBuoiAsync(a.BuoiHocId.Value) : (Guid?)null;
        return View("ChiTiet", a);
    }
    [HttpPost]
    public async Task<IActionResult> Nop(NopBaiViewModel m)
    {
        if (ModelState.IsValid)
            try
            {
                await service.NopAsync(m);
                Success("Đã lưu bài nộp.");
                return RedirectToAction(nameof(ChiTiet), new { id = m.BaiTapGiaoId });
            }
            catch (Exception ex) when (Rules.ErrorStatus(ex) < 500) { ModelState.AddModelError((ex as LoiNghiepVu)?.Truong ?? "", Rules.ErrorMessage(ex)); Response.StatusCode = Rules.ErrorStatus(ex); }
        ViewBag.NopInput = m;
        return await ChiTiet(m.BaiTapGiaoId);
    }
    [HttpPost]
    public async Task<IActionResult> Cham(ChamBaiViewModel m)
    {
        if (ModelState.IsValid)
            try
            {
                await service.ChamAsync(m);
                Success();
                return m.BaiTapGiaoId == Guid.Empty ? RedirectToAction("Index", "LopHoc") : RedirectToAction(nameof(ChiTiet), new { id = m.BaiTapGiaoId });
            }
            catch (Exception ex) when (Rules.ErrorStatus(ex) < 500) { ModelState.AddModelError("", Rules.ErrorMessage(ex)); Response.StatusCode = Rules.ErrorStatus(ex); }
        ViewBag.ChamInput = m;
        return m.BaiTapGiaoId == Guid.Empty ? BadRequest(ModelState) : await ChiTiet(m.BaiTapGiaoId);
    }
    public async Task<IActionResult> Tai(Guid id)
    {
        var (f, path) = await files.TaiAsync(id);
        Response.Headers.XContentTypeOptions = "nosniff";
        return PhysicalFile(path, f.LoaiNoiDung, f.TenGoc);
    }
}
