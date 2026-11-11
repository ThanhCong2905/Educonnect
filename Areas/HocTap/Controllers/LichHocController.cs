using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Areas.HocTap.Controllers;

[Area("HocTap")]
public sealed class LichHocController(LichHocService service, LopHocService classes, BaiTapService tasks) : BaseController
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
        ViewBag.Record = (await service.GhiNhanChiTietAsync(id)).Record;
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
        ViewBag.Record = (await service.GhiNhanChiTietAsync(m.Id)).Record;
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
        ViewBag.Record = (await service.GhiNhanChiTietAsync(m.Id)).Record;
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
        ViewBag.Tasks = (await tasks.LayAsync(s.LopHocId)).Where(x => x.BuoiHocId == record?.Id).ToList();
        ViewBag.Makeups = (await classes.LichAsync(s.LopHocId)).Where(x => x.LaBuoiBu && x.LichGocId == (s.LaBuoiBu ? s.LichGocId : s.Id) && x.Id != s.Id).ToList();
        ViewBag.Records = await classes.BuoiAsync(s.LopHocId);
        ViewBag.ScheduleHistory = await service.LichSuDieuChinhAsync(id);
        return View(new GhiNhanBuoiHocViewModel { LichHocId = id, RowVersion = record == null ? null : Convert.ToBase64String(record.RowVersion), CanHocBu = record?.CanHocBu ?? false, KetQuaBuoiHoc = record?.KetQuaBuoiHoc ?? "DaDienRa", NoiDung = record?.NoiDungGiangDay, NhanXet = record?.NhanXet ?? "", TrangThaiGiaSu = record?.TrangThaiThamDuGiaSu ?? "CoMat", TrangThaiBenHoc = record?.TrangThaiThamDuBenHoc ?? "CoMat" });
    }
    [HttpPost]
    public async Task<IActionResult> BatDauBuoi(Guid id, string rowVersion)
    {
        await service.BatDauBuoiAsync(id, rowVersion);
        Success("Buổi học đã bắt đầu.");
        return RedirectToAction(nameof(Buoi), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Nghi(Guid id)
    {
        ViewBag.Schedule = await service.LayAsync(id);
        ViewBag.Record = (await service.GhiNhanChiTietAsync(id)).Record;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ThongBaoNghi(Guid id, string reason, bool canHocBu, string rowVersion)
    {
        try
        {
            await service.ThongBaoNghiAsync(id, reason, canHocBu, rowVersion);
            Success("Đã thông báo nghỉ.");
            return RedirectToAction(nameof(Buoi), new { id });
        }
        catch (Exception ex) when (Rules.ErrorStatus(ex) < 500)
        {
            ThemLoi(ex);
            await Nghi(id);
            ViewBag.Reason = reason;
            ViewBag.CanHocBu = canHocBu;
            return View("Nghi");
        }
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
