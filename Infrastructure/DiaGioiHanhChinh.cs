using System.Text.Json;

namespace EduConnect.Mvc.Infrastructure;

public static class DiaGioiHanhChinh
{
    private static readonly Dictionary<string, string[]> DanhMuc = DocDanhMuc();

    private static Dictionary<string, string[]> DocDanhMuc()
    {
        using var stream = typeof(DiaGioiHanhChinh).Assembly
            .GetManifestResourceStream("EduConnect.Mvc.wwwroot.data.vietnam-wards.json")
            ?? throw new InvalidOperationException("Thiếu danh mục địa giới hành chính.");
        using var document = JsonDocument.Parse(stream);
        return document.RootElement.GetProperty("provinces").Deserialize<Dictionary<string, string[]>>()!;
    }

    public static IReadOnlyList<string> PhuongXa(string? tinhThanh)
        => tinhThanh != null && DanhMuc.TryGetValue(tinhThanh, out var wards) ? wards : [];

    public static bool HopLe(string? tinhThanh, string? phuongXa)
        => string.IsNullOrWhiteSpace(phuongXa) || PhuongXa(tinhThanh).Contains(phuongXa);
}
