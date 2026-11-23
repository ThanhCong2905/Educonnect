using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace EduConnect.Mvc.Controllers;

[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class TaiKhoanController(TaiKhoanService accounts) : Controller
{
    [HttpGet]
    public async Task<IActionResult> HoSo(bool chinhSua = false)
    {
        ViewData["ChinhSua"] = chinhSua;
        return View(await accounts.LayHoSoAsync(HttpContext));
    }

    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> Anh(Guid id)
    {
        var image = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var viewerId) && viewerId == id
            ? await accounts.LayAnhCaNhanAsync(id) : await accounts.LayAnhCongKhaiAsync(id);
        Response.Headers.XContentTypeOptions = "nosniff";
        Response.Headers.CacheControl = "private, no-store";
        return PhysicalFile(image.Path, image.ContentType);
    }

    [HttpPost, RequestSizeLimit(11000000)]
    public async Task<IActionResult> HoSo(HoSoTaiKhoanInput model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await accounts.LuuHoSoAsync(model, HttpContext);
                TempData["Success"] = "Đã lưu thông tin cá nhân.";
                return RedirectToAction(nameof(HoSo));
            }
            catch (LoiNghiepVu ex) { ModelState.AddModelError(ex.Truong ?? "", ex.Message); }
        }
        var saved = await accounts.LayHoSoAsync(HttpContext);
        model.Email = saved.Email;
        model.AnhDaiDienUrl = saved.AnhDaiDienUrl;
        ViewData["ChinhSua"] = true;
        return View(model);
    }

    [HttpGet]
    public IActionResult DoiMatKhau() => View(new DoiMatKhauInput());

    [HttpPost, EnableRateLimiting("account")]
    public async Task<IActionResult> DoiMatKhau(DoiMatKhauInput model)
    {
        if (ModelState.IsValid)
        {
            var result = await accounts.DoiMatKhauAsync(model, HttpContext);
            if (result.Succeeded)
            {
                await accounts.DangXuatAsync(HttpContext);
                TempData["AccountMessage"] = "Mật khẩu đã được đổi. Đăng nhập lại để tiếp tục.";
                return RedirectToAction(nameof(DangNhap));
            }
            AddErrors(result);
        }
        model.MatKhauHienTai = model.MatKhau = model.XacNhanMatKhau = "";
        foreach (var key in new[] { "MatKhauHienTai", "MatKhau", "XacNhanMatKhau" })
            if (ModelState.TryGetValue(key, out var state))
                state.RawValue = state.AttemptedValue = null;
        return View(model);
    }
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
            if (error == null)
                return Url.IsLocalUrl(model.ReturnUrl) ? LocalRedirect(model.ReturnUrl!) : RedirectToAction("Index", "Home");
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
        if (!model.DongYChinhSach)
            ModelState.AddModelError(nameof(model.DongYChinhSach), "Hãy đọc và đồng ý với điều khoản và chính sách bảo mật.");
        if (ModelState.IsValid)
        {
            var result = await accounts.DangKyAsync(model);
            if (result.Succeeded)
            {
                if (accounts.YeuCauXacMinhEmailMoi)
                {
                    TempData["PendingEmail"] = model.Email.Trim();
                    TempData["PendingVerificationEmail"] = model.Email.Trim();
                    return RedirectToAction(nameof(KiemTraEmail));
                }
                TempData["AccountMessage"] = "Tạo tài khoản thành công. Đăng nhập để tiếp tục.";
                return RedirectToAction(nameof(DangNhap));
            }
            AddErrors(result);
        }
        ClearPassword();
        return View(model);
    }

    [AllowAnonymous, HttpGet]
    public async Task<IActionResult> KiemTraEmail()
    {
        if (TempData.Peek("PendingVerificationEmail") is string registeredAddress &&
            TempData.Peek("PendingEmail") is string shownAddress &&
            string.Equals(shownAddress, registeredAddress, StringComparison.OrdinalIgnoreCase))
            return View(await accounts.LayTrangThaiXacMinhEmailAsync(registeredAddress));

        var seconds = long.TryParse(TempData.Peek("PendingResendUntil") as string, out var deadline)
            ? (int)Math.Clamp(deadline - DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 0, TaiKhoanService.GiayChoGuiLaiEmail)
            : 0;
        return View(new TrangThaiXacMinhEmailViewModel(TempData.Peek("PendingEmail") as string, false, seconds, false));
    }

    [AllowAnonymous, HttpGet]
    public async Task<IActionResult> TrangThaiXacMinhEmail()
    {
        if (TempData.Peek("PendingVerificationEmail") is not string address)
            return NoContent();

        var status = await accounts.LayTrangThaiXacMinhEmailAsync(address);
        return Json(new { daXacMinh = status.DaXacMinh, giayCho = status.GiayCho });
    }

    [AllowAnonymous, HttpGet]
    public IActionResult GuiLaiXacMinhEmail() => View(new QuenMatKhauInput { Email = TempData.Peek("PendingEmail") as string ?? "" });

    [AllowAnonymous, HttpPost, EnableRateLimiting("account")]
    public async Task<IActionResult> GuiLaiXacMinhEmail(QuenMatKhauInput model)
    {
        if (!ModelState.IsValid) return View(model);
        if (!accounts.CoTheGuiEmailTaiKhoan)
        {
            ModelState.AddModelError("", "Chưa thể gửi email lúc này. Bạn thử lại sau nhé.");
            return View(model);
        }
        await accounts.GuiLaiXacMinhEmailAsync(model.Email);
        TempData["PendingEmail"] = model.Email.Trim();
        TempData["PendingResendUntil"] = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + TaiKhoanService.GiayChoGuiLaiEmail).ToString();
        return RedirectToAction(nameof(KiemTraEmail));
    }

    [AllowAnonymous, HttpGet]
    public IActionResult XacMinhEmail(Guid id, string? ma)
    {
        Response.Headers["Referrer-Policy"] = "no-referrer";
        return View(new XacMinhEmailInput { Id = id, Ma = ma ?? "" });
    }

    [AllowAnonymous, HttpPost, EnableRateLimiting("account")]
    public async Task<IActionResult> XacMinhEmail(XacMinhEmailInput model)
    {
        Response.Headers["Referrer-Policy"] = "no-referrer";
        if (ModelState.IsValid && await accounts.XacMinhEmailAsync(model.Id, model.Ma))
        {
            TempData["AccountMessage"] = "Email đã được xác minh. Bạn có thể đăng nhập.";
            return RedirectToAction(nameof(DangNhap));
        }
        ModelState.AddModelError("", "Liên kết không hợp lệ hoặc đã hết hạn. Bạn có thể gửi lại thư xác minh.");
        return View(model);
    }

    [AllowAnonymous, HttpGet]
    public IActionResult QuenMatKhau() => View(new QuenMatKhauInput());

    [AllowAnonymous, HttpPost, EnableRateLimiting("account")]
    public async Task<IActionResult> QuenMatKhau(QuenMatKhauInput model)
    {
        if (!ModelState.IsValid)
            return View(model);
        if (!accounts.CoTheGuiEmailTaiKhoan)
        {
            ModelState.AddModelError("", "Chưa thể gửi email lúc này. Bạn thử lại sau nhé.");
            return View(model);
        }
        await accounts.QuenMatKhauAsync(model.Email);
        return View("DaGuiLienKet");
    }

    [AllowAnonymous, HttpGet]
    public IActionResult DatLaiMatKhau(string email, string ma)
    {
        Response.Headers["Referrer-Policy"] = "no-referrer";
        return View(new DatLaiMatKhauInput { Email = email ?? "", Ma = ma ?? "" });
    }

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
    public IActionResult KhongCoQuyen() => StatusCode(StatusCodes.Status403Forbidden);

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors.DistinctBy(e => e.Description))
            ModelState.AddModelError("", error.Description);
    }

    private void ClearPassword()
    {
        foreach (var key in new[] { "MatKhau", "XacNhanMatKhau" })
        {
            if (ModelState.TryGetValue(key, out var state))
                state.RawValue = state.AttemptedValue = null;
        }
    }
}
