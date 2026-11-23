namespace EduConnect.Mvc.Infrastructure;

public static class PhamViMonHoc
{
    public const string KhoaHocTuNhien = "Khoa học tự nhiên";
    public static IReadOnlyList<string> TenPhan { get; } = ["Vật lí", "Hóa học", "Sinh học"];

    public static bool LaKhoaHocTuNhien(string ten) => ten.Equals(KhoaHocTuNhien, StringComparison.OrdinalIgnoreCase);

    public static (int Tu, int Den) Khoi(string ten) => ten.Trim().ToLowerInvariant() switch
    {
        "khoa học tự nhiên" or "lịch sử và địa lí" or "lịch sử và địa lý" or "gdcd" or "giáo dục công dân" => (6, 9),
        "vật lí" or "vật lý" or "lí" or "lý" or "hóa" or "hoá" or "hóa học" or "hoá học" or "sinh" or "sinh học" or "lịch sử" or "địa lí" or "địa lý" => (10, 12),
        "tiếng việt" => (1, 5),
        "ngữ văn" => (6, 12),
        _ => (1, 12)
    };

    public static int DocKhoi(string? value) =>
        value?.StartsWith("Lớp ", StringComparison.Ordinal) == true && int.TryParse(value.AsSpan(4), out var grade) && grade is >= 1 and <= 12 ? grade : 0;

    public static long BitKhoi(int grade) => 1L << (grade - 1);

    // Each grade has its own three bits so a part offered in one grade cannot match another.
    public static long BitPhan(int grade, int part) => 1L << ((grade - 1) * 3 + part);

    public static long BoLocPhan(int grade, int part)
    {
        if (part is < 1 or > 3) return 0;
        return grade > 0 ? BitPhan(grade, part - 1)
            : Enumerable.Range(6, 4).Aggregate(0L, (mask, lop) => mask | BitPhan(lop, part - 1));
    }

    public static string MoTaPhan(int mask) => string.Join(", ", TenPhan.Where((_, index) => (mask & (1 << index)) != 0));

    public static bool PhuHop(int grade, byte requestedParts, long? gradeMask, long offeredParts)
    {
        if (grade == 0) return requestedParts == 0;
        if (gradeMask is long scope && (scope & BitKhoi(grade)) == 0) return false;
        var requested = (long)requestedParts << ((grade - 1) * 3);
        return (offeredParts & requested) == requested;
    }

    public static string MoTaPhamVi(string phamVi, long mask) => mask == 0 ? phamVi : string.Join(" · ",
        Enumerable.Range(6, 4).Select(grade => (grade, parts: (int)(mask >> ((grade - 1) * 3) & 7)))
            .Where(x => x.parts != 0).Select(x => $"Lớp {x.grade}: {(x.parts == 7 ? "Toàn bộ môn" : MoTaPhan(x.parts))}"));
}
