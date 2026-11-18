namespace EduConnect.Mvc.Areas.NhuCau.ViewModels;

public static class HoSoLuaChon
{
    public static readonly string[] TrinhDo = ["THPT", "Cao đẳng", "Đại học", "Giáo viên", "Thạc sĩ", "Tiến sĩ"];
    public static readonly string[] KinhNghiem = ["Chưa có", "Dưới 1 năm", "1–3 năm", "Trên 3–5 năm", "Trên 5 năm"];
    public static readonly string[] HocLuc = ["Yếu", "Trung bình", "Khá", "Giỏi"];
    public static readonly string[] KhuVuc = ["An Giang", "Bắc Ninh", "Cà Mau", "Cần Thơ", "Cao Bằng", "Đà Nẵng", "Đắk Lắk", "Điện Biên", "Đồng Nai", "Đồng Tháp", "Gia Lai", "Hà Nội", "Hà Tĩnh", "Hải Phòng", "Hồ Chí Minh", "Huế", "Hưng Yên", "Khánh Hòa", "Lai Châu", "Lâm Đồng", "Lạng Sơn", "Lào Cai", "Nghệ An", "Ninh Bình", "Phú Thọ", "Quảng Ngãi", "Quảng Ninh", "Quảng Trị", "Sơn La", "Tây Ninh", "Thái Nguyên", "Thanh Hóa", "Tuyên Quang", "Vĩnh Long"];

    public static string[] LopDay(string? value) => (value ?? "").Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    public static bool PhamViHopLe(string? value)
    {
        var grades = LopDay(value);
        return grades.Length is > 0 and <= 12
            && grades.Distinct().Count() == grades.Length
            && grades.All(grade => Enumerable.Range(1, 12).Any(number => grade == $"Lớp {number}"));
    }
}
