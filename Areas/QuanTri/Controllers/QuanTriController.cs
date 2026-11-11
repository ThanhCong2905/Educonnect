using EduConnect.Mvc.Areas.QuanTri.Services;
using EduConnect.Mvc.Areas.QuanTri.ViewModels;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect.Mvc.Areas.QuanTri.Controllers;

[Area("QuanTri"), Authorize(Roles = "QuanTri")]
public sealed class QuanTriController(QuanTriService service, QuyenQuanTriService rights) : BaseController
{
    public async Task<IActionResult> NhatKyAnToan([FromServices] NhatKyAnToanQuery query, NhatKyAnToanFilter filter)
    {
        Rules.Check(ModelState.IsValid, "Bộ lọc nhật ký chưa đúng định dạng.");
        return View(await query.LayAsync(filter, HttpContext.RequestAborted));
    }
    public async Task<IActionResult> Index() => View(await service.TongQuanAsync());
    public async Task<IActionResult> TaiKhoan(int trang = 1, string? q = null, string? role = null, string? state = null)
        => View(await service.LayTaiKhoanAsync(trang, q, role, state));
    public async Task<IActionResult> ChiTiet(Guid id) => View(await service.TaiKhoanAsync(id));
    public async Task<IActionResult> XemNhatKy(int trang = 1, string? q = null, DateTimeOffset? tuNgay = null, DateTimeOffset? denNgay = null)
        => View(await service.LayNhatKyAsync(trang, q, tuNgay, denNgay));
    public async Task<IActionResult> NoiDung(int trang = 1, string? loai = null, bool conHieuLuc = true)
        => View(await service.BienPhapAsync(trang, loai, conHieuLuc));

    [HttpPost]
    public async Task<IActionResult> KhoaTaiKhoan(Guid id, string lyDo, string concurrencyStamp)
    {
        await service.KhoaTaiKhoanAsync(id, lyDo, concurrencyStamp);
        Success("Đã khóa tài khoản và thu hồi các phiên đăng nhập.");
        return RedirectToAction(nameof(ChiTiet), new
        {
            id
        });
    }

    [HttpPost]
    public async Task<IActionResult> GoBienPhap(Guid id, string reason, string rowVersion)
    {
        await service.GoBienPhapAsync(id, reason, rowVersion);
        Success("Đã gỡ biện pháp.");
        return RedirectToAction(nameof(NoiDung));
    }

    [HttpGet]
    public async Task<IActionResult> PhanQuyen(Guid id)
    {
        await rights.KiemTraAsync("QuanLyQuyen");
        var user = await service.TaiKhoanAsync(id);
        ViewBag.HoTen = user.TaiKhoan.HoTen;
        return View(new PhanQuyenQuanTriViewModel { TaiKhoanId = id, ConcurrencyStamp = user.TaiKhoan.ConcurrencyStamp!, Quyen = user.Quyen });
    }

    [HttpPost]
    public async Task<IActionResult> PhanQuyen(PhanQuyenQuanTriViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await service.PhanQuyenAsync(model);
                Success("Đã cập nhật quyền. Tài khoản cần đăng nhập lại.");
                return RedirectToAction(nameof(TaiKhoan));
            }
            catch (LoiNghiepVu ex) { ModelState.AddModelError(ex.Truong ?? "", ex.Message); }
        }
        return View(model);
    }

    public async Task<IActionResult> MonHoc(int trang = 1, string? q = null) => View(await service.LayMonAsync(trang, q));

    [HttpPost]
    public async Task<IActionResult> LuuMon(MonHocQuanTriViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await service.LuuMonAsync(model);
                Success("Đã lưu môn học.");
                return RedirectToAction(nameof(MonHoc));
            }
            catch (LoiNghiepVu ex) { ModelState.AddModelError(ex.Truong ?? "", ex.Message); }
        }
        return View("SuaMon", model);
    }
}
