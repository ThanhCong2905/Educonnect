using EduConnect.Mvc.Models;
using EduConnect.Mvc.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduConnect.Mvc.Infrastructure;

public sealed partial class TaiKhoanService
{
    private async Task<TaiKhoan> TaiKhoanDangNhapAsync(HttpContext http)
    {
        Rules.Check(Guid.TryParse(http.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id), "Vui lòng đăng nhập.", 401);
        return await db.Users.SingleOrDefaultAsync(x => x.Id == id && x.TrangThai == "HoatDong")
            ?? throw new LoiNghiepVu("Tài khoản hiện không thể truy cập.", 403);
    }

    public async Task<HoSoTaiKhoanInput> LayHoSoAsync(HttpContext http)
    {
        var user = await TaiKhoanDangNhapAsync(http);
        return new HoSoTaiKhoanInput { HoTen = user.HoTen, SoDienThoai = user.PhoneNumber, ConcurrencyStamp = user.ConcurrencyStamp!, Email = user.Email ?? "" };
    }

    public async Task LuuHoSoAsync(HoSoTaiKhoanInput input, HttpContext http)
    {
        Rules.ValidateInput(input);
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var user = await TaiKhoanDangNhapAsync(http);
        Rules.Check(user.ConcurrencyStamp == input.ConcurrencyStamp, "Thông tin đã thay đổi. Hãy tải lại trước khi lưu.", 409);
        var tutor = await db.Set<HoSoGiaSu>().SingleOrDefaultAsync(x => x.GiaSuId == user.Id);
        if (tutor != null && user.HoTen != input.HoTen.Trim())
        {
            Rules.Check(!tutor.DangHienThi && tutor.TrangThaiDuyet != "ChoDuyet", "Ẩn hồ sơ hoặc rút yêu cầu duyệt trước khi đổi họ tên.", 409, nameof(input.HoTen));
            tutor.TrangThaiDuyet = "BanNhap";
            tutor.NguoiDuyetId = null;
            tutor.DuyetLuc = null;
        }
        user.HoTen = input.HoTen.Trim();
        user.PhoneNumber = string.IsNullOrWhiteSpace(input.SoDienThoai) ? null : input.SoDienThoai.Trim();
        var result = await users.UpdateAsync(user);
        Rules.Check(result.Succeeded, "Thông tin tài khoản đã thay đổi. Hãy tải lại trước khi lưu.", 409);
        await tx.CommitAsync();
    }

    public async Task<IdentityResult> DoiMatKhauAsync(DoiMatKhauInput input, HttpContext http)
    {
        Rules.ValidateInput(input);
        var user = await TaiKhoanDangNhapAsync(http);
        var result = await users.ChangePasswordAsync(user, input.MatKhauHienTai, input.MatKhau);
        if (result.Succeeded)
            await sessions.ThuHoiTatCaAsync(user.Id, "Mật khẩu đã thay đổi");
        return result;
    }
}
