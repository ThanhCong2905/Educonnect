using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Areas.HocTap.Controllers;

[Area("HocTap")]
public sealed class LichHocController(LichHocService service, LopHocService classes) : BaseController
{
    private void ThemLoi(Exception ex)
    {
        var field = (ex as LoiNghiepVu)?.Truong ?? "";
        ModelState.AddModelError(field, Rules.ErrorMessage(ex));
        Response.StatusCode = Rules.ErrorStatus(ex);
    }
    public async Task<IActionResult> Them(Guid id)
    {
        ViewBag.Class = await classes.ChiTietAsync(id);
        return View(new LichHangLoatInput { LopHocId = id });
    }
    [HttpPost]
    public async Task<IActionResult> Them(LichHangLoatInput m)
    {
        if (ModelState.IsValid)
            try
            {
                await service.ThemHangLoatAsync(m);
                Success();
                return RedirectToAction("ChiTiet", "LopHoc", new
                {
                    id = m.LopHocId,
                    tab = "schedule"
                });
            }
            catch (Exception ex) when (Rules.ErrorStatus(ex) < 500) { ThemLoi(ex); }
        ViewBag.Class = await classes.ChiTietAsync(m.LopHocId);
        return View(m);
    }
    public async Task<IActionResult> Sua(Guid id)
    {
        var s = await service.LayAsync(id);
        ViewBag.Schedule = s;
        return View(new SuaLichInput { Id = id, RowVersion = Convert.ToBase64String(s.RowVersion), Lich = new() { TieuDe = s.TieuDe, BatDauLuc = s.BatDauLuc.ToOffset(TimeSpan.FromHours(7)).DateTime, KetThucLuc = s.KetThucLuc.ToOffset(TimeSpan.FromHours(7)).DateTime } });
    }
    [HttpPost]
    public async Task<IActionResult> Sua(SuaLichInput m)
    {
        if (m.TrangThai == "DaHuy")
        {
            ModelState.Remove("Lich.TieuDe");
            ModelState.Remove("Lich.BatDauLuc");
            ModelState.Remove("Lich.KetThucLuc");
        }
        if (ModelState.IsValid)
            try
            {
                var s = await service.LayAsync(m.Id);
                await service.SuaAsync(m);
                Success();
                return RedirectToAction("ChiTiet", "LopHoc", new
                {
                    id = s.LopHocId,
                    tab = "schedule"
                });
            }
            catch (Exception ex) when (Rules.ErrorStatus(ex) < 500) { ThemLoi(ex); }
        ViewBag.Schedule = await service.LayAsync(m.Id);
        return View(m);
    }
    [HttpPost]
    public async Task<IActionResult> TaoBuoiBu(SuaLichInput m)
    {
        if (ModelState.IsValid)
            try
            {
                var s = await service.LayAsync(m.Id);
                await service.TaoBuoiBuAsync(m);
                Success("Đã thêm buổi học bù.");
                return RedirectToAction("ChiTiet", "LopHoc", new { id = s.LopHocId, tab = "schedule" });
            }
            catch (Exception ex) when (Rules.ErrorStatus(ex) < 500) { ThemLoi(ex); }
        ViewBag.Schedule = await service.LayAsync(m.Id);
        return View("Sua", m);
    }
    public async Task<IActionResult> Buoi(Guid id)
    {
        var s = await service.LayAsync(id);
        var (record, history) = await service.GhiNhanChiTietAsync(id);
        ViewBag.Schedule = s;
        ViewBag.Class = await classes.ChiTietAsync(s.LopHocId);
        ViewBag.Record = record;
        ViewBag.History = history;
        return View(new GhiNhanBuoiHocViewModel { LichHocId = id, RowVersion = record == null ? null : Convert.ToBase64String(record.RowVersion), KetQuaBuoiHoc = record?.KetQuaBuoiHoc ?? "DaDienRa", NoiDung = record?.NoiDungGiangDay, NhanXet = record?.NhanXet ?? "", TrangThaiGiaSu = record?.TrangThaiThamDuGiaSu ?? "CoMat", TrangThaiBenHoc = record?.TrangThaiThamDuBenHoc ?? "CoMat" });
    }
    [HttpPost]
    public async Task<IActionResult> GhiNhan(GhiNhanBuoiHocViewModel m)
    {
        if (ModelState.IsValid)
            try
            {
                await service.GhiNhanAsync(m);
                Success();
                return RedirectToAction(nameof(Buoi), new { id = m.LichHocId });
            }
            catch (Exception ex) when (Rules.ErrorStatus(ex) < 500) { ThemLoi(ex); }
        await Buoi(m.LichHocId);
        return View("Buoi", m);
    }
}
