using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Infrastructure;

public static class GiaoDichNghiepVu
{
    public static Task<DateTimeOffset> GioMayChuAsync(AppDbContext db, CancellationToken huy = default)
        => db.Database.SqlQuery<DateTimeOffset>($"SELECT SYSDATETIMEOFFSET() AS Value").SingleAsync(huy);

    public static async Task<LopHoc> KhoaLopAsync(AppDbContext db, Guid id, CancellationToken huy = default)
        => await db.Set<LopHoc>().FromSqlInterpolated($"SELECT * FROM dbo.LopHoc WITH (UPDLOCK,HOLDLOCK) WHERE Id={id}")
            .SingleOrDefaultAsync(huy) ?? throw new LoiNghiepVu("Không tìm thấy lớp học.", 404);

    public static async Task KhoaQuanTriAsync(AppDbContext db, Guid? actor = null, string? quyen = null)
    {
        await db.Database.ExecuteSqlRawAsync("DECLARE @r int; EXEC @r=sys.sp_getapplock @Resource=N'EduConnect:QuanTri', @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=10000; IF @r<0 THROW 51163,N'Có thao tác quản trị đang xử lý. Hãy thử lại.',1;");
        if (actor.HasValue && quyen != null)
            await KiemTraQuyenQuanTriAsync(db, actor.Value, quyen);
    }

    public static async Task KiemTraQuyenQuanTriAsync(AppDbContext db, Guid actor, string quyen)
    {
        Rules.Check(await db.Users.AnyAsync(u => u.Id == actor && u.TrangThai == "HoatDong"
            && db.UserClaims.Any(c => c.UserId == u.Id && c.ClaimType == "EduConnect.AdminPermission" && c.ClaimValue == quyen)
            && db.UserRoles.Any(ur => ur.UserId == u.Id && db.Roles.Any(r => r.Id == ur.RoleId && r.Name == "QuanTri"))),
            "Tài khoản hoặc quyền quản trị đã thay đổi. Hãy đăng nhập lại.", 403);
    }
    public static async Task KiemTraTaiKhoanAsync(AppDbContext db, Guid actor)
        => Rules.Check(await db.Users.AnyAsync(x => x.Id == actor && x.TrangThai == "HoatDong"), "Tài khoản hiện không thể thực hiện thao tác này.", 403);
}
