using EduConnect.Mvc.Data;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Infrastructure;

public static class HoSoLock
{
    public static Task KhoaAsync(AppDbContext db, Guid tutor)
    {
        var resource = "EduConnect:profile:" + tutor.ToString("D").ToLowerInvariant();
        return db.Database.ExecuteSqlInterpolatedAsync($"DECLARE @r int; EXEC @r=sys.sp_getapplock @Resource={resource},@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=15000; IF @r<0 THROW 51200,N'Đang có thao tác trên hồ sơ. Hãy thử lại.',1;");
    }
}
