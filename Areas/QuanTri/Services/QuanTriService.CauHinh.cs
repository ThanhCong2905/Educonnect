using System.Data;
using EduConnect.Mvc.Areas.QuanTri.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.QuanTri.Services;

public sealed partial class QuanTriService
{
    public async Task PhanQuyenAsync(PhanQuyenQuanTriViewModel input)
    {
        Rules.ValidateInput(input);
        var actor = await rights.KiemTraAsync("QuanLyQuyen");
        Rules.Check(input.Quyen.All(QuyenQuanTriService.DanhSach.Contains), "Quyền quản trị không hợp lệ.");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await GiaoDichNghiepVu.KhoaQuanTriAsync(db, actor, "QuanLyQuyen");
        Rules.Check(await VaiTroAsync(input.TaiKhoanId) == "QuanTri", "Chỉ cấp quyền này cho tài khoản quản trị.");
        var user = await db.Users.SingleAsync(x => x.Id == input.TaiKhoanId);
        Rules.Check(user.ConcurrencyStamp == input.ConcurrencyStamp, "Quyền tài khoản đã thay đổi. Hãy tải lại.", 409);
        foreach (var critical in new[] { "QuanLyQuyen", "QuanLyTaiKhoan" }.Except(input.Quyen))
            Rules.Check(await db.UserClaims.AnyAsync(x => x.UserId != user.Id && x.ClaimType == QuyenQuanTriService.ClaimType && x.ClaimValue == critical && db.Users.Any(u => u.Id == x.UserId && u.TrangThai == "HoatDong")), "Cần giữ quản trị đang hoạt động có quyền cấp quyền và quản lý tài khoản.");
        var old = await db.UserClaims.Where(x => x.UserId == user.Id && x.ClaimType == QuyenQuanTriService.ClaimType).ToListAsync();
        db.RemoveRange(old);
        foreach (var permission in input.Quyen.Distinct())
            db.UserClaims.Add(new Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid> { UserId = user.Id, ClaimType = QuyenQuanTriService.ClaimType, ClaimValue = permission });
        user.ConcurrencyStamp = Guid.NewGuid().ToString();
        user.SecurityStamp = Guid.NewGuid().ToString();
        var detail = $"Lý do: {input.LyDo.Trim()}\nQuyền trước: {string.Join(", ", old.Select(x => x.ClaimValue).Order())}\nQuyền sau: {string.Join(", ", input.Quyen.Distinct().Order())}";
        rights.GhiNhatKy(actor, "PhanQuyenQuanTri", "TaiKhoan", user.Id, chiTiet: detail);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        await sessions.ThuHoiTatCaAsync(user.Id, "Quyền quản trị đã thay đổi");
    }

    public async Task<Page<NhatKyQuanTriRow>> LayNhatKyAsync(int trang = 1, string? q = null, DateTimeOffset? tuNgay = null, DateTimeOffset? denNgay = null)
    {
        await rights.KiemTraAsync("XemNhatKy");
        Rules.Check(trang is >= 1 and <= 100000 && (q?.Length ?? 0) <= 100 && (tuNgay == null || denNgay == null || tuNgay <= denNgay), "Bộ lọc nhật ký không hợp lệ.");
        var query = db.Set<NhatKyQuanTri>().AsNoTracking().Where(x => (q == null || x.HanhDong.Contains(q) || x.LoaiDoiTuong.Contains(q) || x.DoiTuongId.Contains(q))
            && (tuNgay == null || x.ThoiDiem >= tuNgay) && (denNgay == null || x.ThoiDiem < denNgay.Value.AddDays(1)));
        var rows = from n in query
                   join u in db.Users on n.NguoiThucHienId equals u.Id
                   orderby n.ThoiDiem descending, n.Id
                   select new NhatKyQuanTriRow { BanGhi = n, HoTen = u.HoTen };
        return new Page<NhatKyQuanTriRow>
        {
            Trang = trang,
            KichThuocTrang = 20,
            Total = await query.CountAsync(),
            Items = await rows.Skip((trang - 1) * 20).Take(20).ToListAsync()
        };
    }

    public async Task<Page<MonHoc>> LayMonAsync(int trang = 1, string? q = null)
    {
        await rights.KiemTraAsync("QuanLyDanhMuc");
        Rules.Check(trang is >= 1 and <= 100000 && (q?.Length ?? 0) <= 100, "Trang hoặc từ khóa không hợp lệ.");
        var query = db.Set<MonHoc>().AsNoTracking().Where(x => q == null || x.TenMon.Contains(q));
        return new Page<MonHoc> { Trang = trang, KichThuocTrang = 20, Total = await query.CountAsync(), Items = await query.OrderBy(x => x.TenMon).ThenBy(x => x.Id).Skip((trang - 1) * 20).Take(20).ToListAsync() };
    }

    public async Task LuuMonAsync(MonHocQuanTriViewModel input)
    {
        Rules.ValidateInput(input);
        var actor = await rights.KiemTraAsync("QuanLyDanhMuc");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await GiaoDichNghiepVu.KhoaQuanTriAsync(db, actor, "QuanLyDanhMuc");
        var name = input.TenMon.Trim().Normalize();
        Rules.Check(!await db.Set<MonHoc>().AnyAsync(x => x.Id != input.Id && x.TenMon == name), "Tên môn học đã có trong danh mục.", 409);
        var subject = input.Id == 0 ? new MonHoc() : await db.Set<MonHoc>().SingleOrDefaultAsync(x => x.Id == input.Id) ?? throw new LoiNghiepVu("Không tìm thấy môn học.", 404);
        if (input.Id != 0)
            Rules.Concurrency(db, subject, input.RowVersion);
        subject.TenMon = name;
        subject.DangSuDung = input.DangSuDung;
        if (input.Id == 0)
            db.Add(subject);
        await db.SaveChangesAsync();
        rights.GhiNhatKy(actor, "CapNhatMonHoc", "MonHoc", subject.Id);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
