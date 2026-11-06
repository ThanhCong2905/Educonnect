using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EduConnect.Mvc.Controllers;

[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class TaiKhoanController(TaiKhoanService accounts) : Controller
{
    [AllowAnonymous, HttpGet]
    public IActionResult DangNhap(string? returnUrl)
        => User.Identity?.IsAuthenticated == true ? RedirectToAction("Index", "Home")
            : View(new DangNhapInput { ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : null });

    [AllowAnonymous, HttpPost]
    public async Task<IActionResult> DangNhap(DangNhapInput model)
    {
        if (ModelState.IsValid)
        {
            var error = await accounts.DangNhapAsync(model, HttpContext);
            if (error == null) return Url.IsLocalUrl(model.ReturnUrl) ? LocalRedirect(model.ReturnUrl!) : RedirectToAction("Index", "Home");
            ModelState.AddModelError("", error);
        }
        ClearPassword();
        return View(model);
    }

    [AllowAnonymous, HttpGet]
    public IActionResult DangKy() => View(new DangKyInput());

    [AllowAnonymous, HttpPost, EnableRateLimiting("account")]
    public async Task<IActionResult> DangKy(DangKyInput model)
    {
        if (ModelState.IsValid)
        {
            var result = await accounts.DangKyAsync(model);
            if (result.Succeeded)
            {
                TempData["AccountMessage"] = "Tạo tài khoản thành công. Đăng nhập để tiếp tục.";
                return RedirectToAction(nameof(DangNhap));
            }
            AddErrors(result);
        }
        ClearPassword();
        return View(model);
    }

    [AllowAnonymous, HttpGet]
    public IActionResult QuenMatKhau() => View(new QuenMatKhauInput());

    [AllowAnonymous, HttpPost, EnableRateLimiting("account")]
    public async Task<IActionResult> QuenMatKhau(QuenMatKhauInput model)
    {
        if (!ModelState.IsValid) return View(model);
        await accounts.QuenMatKhauAsync(model.Email);
        return View("DaGuiLienKet");
    }

    [AllowAnonymous, HttpGet]
    public IActionResult DatLaiMatKhau(string email, string ma) => View(new DatLaiMatKhauInput { Email = email ?? "", Ma = ma ?? "" });

    [AllowAnonymous, HttpPost, EnableRateLimiting("account")]
    public async Task<IActionResult> DatLaiMatKhau(DatLaiMatKhauInput model)
    {
        if (ModelState.IsValid)
        {
            var result = await accounts.DatLaiMatKhauAsync(model);
            if (result.Succeeded)
            {
                await accounts.DangXuatAsync(HttpContext);
                TempData["AccountMessage"] = "Mật khẩu đã được cập nhật. Vui lòng đăng nhập lại.";
                return RedirectToAction(nameof(DangNhap));
            }
            AddErrors(result);
        }
        ClearPassword();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DangXuat()
    {
        await accounts.DangXuatAsync(HttpContext);
        return RedirectToAction(nameof(DangNhap));
    }

    [AllowAnonymous, HttpGet]
    public IActionResult KhongCoQuyen()
    {
        Response.StatusCode = 403;
        return View();
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors.DistinctBy(e => e.Description)) ModelState.AddModelError("", error.Description);
    }

    private void ClearPassword()
    {
        foreach (var key in new[] { "MatKhau", "XacNhanMatKhau" })
        {
            if (ModelState.TryGetValue(key, out var state)) state.RawValue = state.AttemptedValue = null;
        }
    }
}
