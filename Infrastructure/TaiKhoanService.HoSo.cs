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
        return new HoSoTaiKhoanInput
        {
            HoTen = user.HoTen,
            SoDienThoai = user.PhoneNumber,
            ConcurrencyStamp = user.ConcurrencyStamp!,
            Email = user.Email ?? "",
            AnhDaiDienUrl = files.DuongDanAnhDaiDien(user.AnhDaiDien) == null ? null : $"/TaiKhoan/Anh/{user.Id}"
        };
    }

    public async Task<(string Path, string ContentType)> LayAnhAsync(Guid id)
    {
        var key = await db.Users.AsNoTracking().Where(x => x.Id == id && x.TrangThai == "HoatDong")
            .Select(x => x.AnhDaiDien).SingleOrDefaultAsync();
        var path = files.DuongDanAnhDaiDien(key);
        Rules.Check(path != null && File.Exists(path), "Ảnh đại diện chưa có hoặc đã được gỡ.", 404);
        return (path!, Path.GetExtension(path) == ".png" ? "image/png" : "image/jpeg");
    }

    public async Task LuuHoSoAsync(HoSoTaiKhoanInput input, HttpContext http)
    {
        Rules.ValidateInput(input);
        string? uploaded = null;
        string? retired = null;
        try
        {
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
            Rules.Check(input.AnhDaiDienFile == null || !input.XoaAnhDaiDien, "Chọn thay ảnh hoặc xóa ảnh hiện tại.", field: nameof(input.AnhDaiDienFile));
            if (input.AnhDaiDienFile != null)
            {
                uploaded = await files.LuuAnhDaiDienAsync(input.AnhDaiDienFile);
                retired = user.AnhDaiDien;
                user.AnhDaiDien = uploaded;
            }
            else if (input.XoaAnhDaiDien)
            {
                retired = user.AnhDaiDien;
                user.AnhDaiDien = null;
            }
            var result = await users.UpdateAsync(user);
            Rules.Check(result.Succeeded, "Thông tin tài khoản đã thay đổi. Hãy tải lại trước khi lưu.", 409);
            await tx.CommitAsync();
        }
        catch (LoiNghiepVu ex)
        {
            files.XoaAnhDaiDien(uploaded);
            if (ex.Truong == "Files")
                throw new LoiNghiepVu(ex.Message, ex.Status) { Truong = nameof(input.AnhDaiDienFile) };
            throw;
        }
        catch
        {
            files.XoaAnhDaiDien(uploaded);
            throw;
        }
        files.XoaAnhDaiDien(retired);
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
