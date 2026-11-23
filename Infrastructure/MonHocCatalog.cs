using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Infrastructure;

public sealed class MonHocCatalog(AppDbContext db)
{
    public static readonly string[] PhoThong = ["Toán Học", "Ngữ Văn", "Tiếng Anh", "Tiếng Việt", PhamViMonHoc.KhoaHocTuNhien,
        "Lịch Sử", "Địa Lý", "Lịch sử và Địa lí", "Sinh học", "Hóa Học", "Vật Lý", "GDCD"];
    public static int ThuTuHienThi(string tenMon)
    {
        if (tenMon.Equals("Tiếng Việt", StringComparison.OrdinalIgnoreCase)) return 1;
        var index = Array.FindIndex(PhoThong, name => name.Equals(tenMon, StringComparison.OrdinalIgnoreCase));
        return index >= 0 ? index : 99;
    }
    public async Task<int> LayHoacThemAsync(string? ten, CancellationToken huy = default)
    {
        ten = ten?.Trim();
        Rules.Check(!string.IsNullOrWhiteSpace(ten) && ten.Length <= 100, "Nhập tên môn khác (tối đa 100 ký tự).");
        var existing = await db.Set<MonHoc>().OrderBy(x => x.Id).FirstOrDefaultAsync(x => x.TenMon == ten, huy);
        if (existing != null)
        {
            Rules.Check(existing.DangSuDung, "Môn học này đang ngừng sử dụng.");
            return existing.Id;
        }
        var subject = new MonHoc { TenMon = ten!, DangSuDung = true };
        db.Add(subject);
        await db.SaveChangesAsync(huy);
        return subject.Id;
    }
}
