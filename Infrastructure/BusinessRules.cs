using System.Globalization;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Infrastructure;

public sealed class LoiNghiepVu(string message, int status = 400) : Exception(message)
{
    public int Status { get; } = status;
}
public static class Rules
{
    public static Microsoft.Data.SqlClient.SqlParameter SqlVersion(string? version) => new("RowVersionValue", System.Data.SqlDbType.Binary, 8) { Value = string.IsNullOrEmpty(version) ? DBNull.Value : Version(version) };
    public static void Check(bool condition, string message, int status = 400)
    {
        if (!condition)
            throw new LoiNghiepVu(message, status);
    }
    public static byte[] Version(string? value)
    {
        try
        {
            var b = Convert.FromBase64String(value ?? "");
            Check(b.Length == 8, "Phiên bản không hợp lệ.");
            return b;
        }
        catch (FormatException) { throw new LoiNghiepVu("Phiên bản không hợp lệ."); }
    }
    public static DateTimeOffset Local(DateTime value) => new(DateTime.SpecifyKind(value, DateTimeKind.Unspecified), TimeSpan.FromHours(7));
    public static string Date(DateTimeOffset value) => value.ToOffset(TimeSpan.FromHours(7)).ToString("dd/MM/yyyy HH:mm");
    public static string Fee(decimal? n, string? unit) => n.HasValue ? n.Value.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " đ / " + (unit == "VND/Buoi" ? "buổi" : "giờ") : "Chưa xác định";
    public static string Label(string? x) => x switch { "QuanTri" => "Quản trị", "ChoDuyet" => "Chờ duyệt", "DaDuyet" => "Đã duyệt", "CanChinhSua" => "Cần chỉnh sửa", "CaHai" => "Trực tuyến và trực tiếp", "DangMo" => "Đang mở", "BanNhap" => "Bản nháp", "DaDong" => "Đã đóng", "ChoPhanHoi" => "Chờ phản hồi", "ChapNhan" => "Đã chấp nhận", "TuChoi" => "Từ chối", "DaRut" => "Đã rút", "ChoGiaSu" => "Chờ gia sư", "ChoBenHoc" => "Chờ bên học", "DaChapNhan" => "Đã tạo lớp", "HetHan" => "Hết hạn", "DangHoatDong" => "Đang học", "DaKetThuc" => "Đã kết thúc", "DaHuy" => "Đã hủy", "TrucTuyen" => "Trực tuyến", "TrucTiep" => "Trực tiếp", "GiaSu" => "Gia sư", "HocSinh" => "Học sinh", "PhuHuynh" => "Phụ huynh", "DaLenLich" => "Đã lên lịch", "DaDieuChinh" => "Đã điều chỉnh", _ => x ?? "" };
    public static void Slots(IReadOnlyList<SlotInput> slots)
    {
        Check(slots.Count is > 0 and <= 35, "Cần từ 1 đến 35 khung giờ.");
        foreach (var s in slots)
            Check(s.ThuTrongTuan <= 6 && s.GioBatDau < s.GioKetThuc, "Thứ hoặc giờ của khung giờ không hợp lệ.");
        for (int i = 0; i < slots.Count; i++)
            for (int j = i + 1; j < slots.Count; j++)
                Check(slots[i].ThuTrongTuan != slots[j].ThuTrongTuan || slots[i].GioBatDau >= slots[j].GioKetThuc || slots[j].GioBatDau >= slots[i].GioKetThuc, "Các khung giờ cùng ngày không được chồng lấn.");
    }
    public static void FeeCheck(decimal? value, string? unit)
    {
        Check((value == null) == string.IsNullOrEmpty(unit), "Nhập đồng thời học phí và đơn vị.");
        if (value != null)
            Check(value >= 0 && value <= 9999999999999999m && decimal.Round(value.Value, 2) == value && (unit == "VND/Gio" || unit == "VND/Buoi"), "Học phí không âm, tối đa hai chữ số thập phân; chọn đúng đơn vị.");
    }
    public static void Concurrency(AppDbContext db, object entity, string? version) => db.Entry(entity).Property("RowVersion").OriginalValue = Version(version);
}
public sealed class SlotInput
{
    public byte ThuTrongTuan { get; set; } = 1; public TimeOnly GioBatDau { get; set; } = new(19, 0); public TimeOnly GioKetThuc { get; set; } = new(20, 30);
}
public sealed class NguoiDungMauService(AppDbContext db, IHttpContextAccessor context, PhienCapNhat? realtime = null)
{
    public Task<List<NguoiThamGia>> DanhSachAsync() => db.Set<NguoiThamGia>().AsNoTracking().OrderBy(x => x.LoaiNguoi).ThenBy(x => x.HoTen).ToListAsync();
    public async Task<NguoiThamGia> HienTaiAsync()
    {
        var id = context.HttpContext?.Session.GetString("participant");
        var q = db.Set<NguoiThamGia>().AsNoTracking();
        var u = Guid.TryParse(id, out var g) ? await q.SingleOrDefaultAsync(x => x.Id == g) : null;
        u ??= await q.OrderBy(x => x.LoaiNguoi == "HocSinh" ? 0 : 1).ThenBy(x => x.Id).FirstAsync();
        context.HttpContext!.Session.SetString("participant", u.Id.ToString());
        realtime?.CapNhat(context.HttpContext.Session.Id, u.Id);
        return u;
    }
    public async Task ChonAsync(Guid id)
    {
        Rules.Check(await db.Set<NguoiThamGia>().AnyAsync(x => x.Id == id), "Người tham gia không tồn tại.");
        context.HttpContext!.Session.SetString("participant", id.ToString());
        realtime?.CapNhat(context.HttpContext.Session.Id, id);
    }
}
