using System.Globalization;

namespace EduConnect.Mvc.ViewModels;

public sealed record AnhDaiDienViewModel(string HoTen, string? AnhUrl, bool Lazy = true)
{
    public string ChuCai
    {
        get
        {
            var parts = HoTen.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 0) return "?";
            var last = Array.FindLast(parts, part => char.IsLetter(part, 0)) ?? parts[^1];
            var first = StringInfo.GetNextTextElement(parts[0]);
            return (last == parts[0] ? first : first + StringInfo.GetNextTextElement(last)).ToUpperInvariant();
        }
    }

    public int Mau => (int)(ChuCai.Aggregate(0L, (sum, character) => sum + character) % 4);
}
