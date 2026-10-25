using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Infrastructure;

public sealed class MonHocCatalog(AppDbContext db)
{
    public static readonly string[] PhoThong = ["Toán Học", "Ngữ Văn", "Tiếng Anh", "Lịch Sử", "Địa Lý", "Sinh học", "Hóa Học", "Vật Lý", "GDCD"];
    public async Task<int> LayHoacThemAsync(string? ten)
    {
        ten = ten?.Trim();
        Rules.Check(!string.IsNullOrWhiteSpace(ten) && ten.Length <= 100, "Nhập tên môn khác (tối đa 100 ký tự).");
        var existing = await db.Set<MonHoc>().OrderBy(x => x.Id).FirstOrDefaultAsync(x => x.TenMon == ten);
        if (existing != null)
        {
            Rules.Check(existing.DangSuDung, "Môn học này đang ngừng sử dụng.");
            return existing.Id;
        }
        var subject = new MonHoc { TenMon = ten!, DangSuDung = true };
        db.Add(subject);
        await db.SaveChangesAsync();
        return subject.Id;
    }
}
