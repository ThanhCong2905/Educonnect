using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Areas.HocTap.Controllers;

[Area("HocTap")]
public sealed class LichHocController(LichHocService service, LopHocService classes) : BaseController
{
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
            catch (LoiNghiepVu ex) { ModelState.AddModelError("", ex.Message); }
        ViewBag.Class = await classes.ChiTietAsync(m.LopHocId);
        return View(m);
    }
    public async Task<IActionResult> Sua(Guid id)
    {
        var s = await service.LayAsync(id);
        return View(new SuaLichInput { Id = id, RowVersion = Convert.ToBase64String(s.RowVersion), Lich = new() { TieuDe = s.TieuDe, BatDauLuc = s.BatDauLuc.ToOffset(TimeSpan.FromHours(7)).DateTime, KetThucLuc = s.KetThucLuc.ToOffset(TimeSpan.FromHours(7)).DateTime } });
    }
    [HttpPost]
    public async Task<IActionResult> Sua(SuaLichInput m)
    {
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
            catch (LoiNghiepVu ex) { ModelState.AddModelError("", ex.Message); }
        return View(m);
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
        ValidateInput();
        await service.GhiNhanAsync(m);
        Success();
        return RedirectToAction(nameof(Buoi), new
        {
            id = m.LichHocId
        });
    }
}
