using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;

namespace EduConnect.Mvc.Areas.HocTap.ViewModels;

public sealed class LichTuanViewModel
{
    public LopHoc Lop { get; init; } = null!;
    public DateOnly TuNgay { get; init; }
    public DateOnly HomNay { get; init; }
    public int GioDau { get; init; }
    public int GioCuoi { get; init; }
    public List<SuKienLichViewModel> SuKien { get; init; } = [];

    public static LichTuanViewModel Tao(LopHoc lop, IEnumerable<LichHoc> lich,
        IEnumerable<BuoiHoc> buoi, DateOnly? tuan, DateTimeOffset bayGio)
    {
        var homNay = DateOnly.FromDateTime(bayGio.ToOffset(TimeSpan.FromHours(7)).DateTime);
        var ngay = tuan ?? homNay;
        var dauTuan = ngay.AddDays(-((int)ngay.DayOfWeek + 6) % 7);
        Rules.Check(dauTuan.DayNumber >= 7 && dauTuan.DayNumber <= DateOnly.MaxValue.DayNumber - 14,
            "Tuần nằm ngoài khoảng ngày được hỗ trợ.", field: "week");
        var ghiNhan = buoi.ToDictionary(x => x.LichHocId);
        var suKien = new List<SuKienLichViewModel>();
        for (var cot = 0; cot < 7; cot++)
        {
            var dauNgay = new DateTimeOffset(dauTuan.AddDays(cot).ToDateTime(TimeOnly.MinValue), TimeSpan.FromHours(7));
            var cuoiNgay = dauNgay.AddDays(1);
            foreach (var item in lich.Where(x => x.BatDauLuc < cuoiNgay && x.KetThucLuc > dauNgay))
            {
                var batDau = item.BatDauLuc > dauNgay ? item.BatDauLuc : dauNgay;
                var ketThuc = item.KetThucLuc < cuoiNgay ? item.KetThucLuc : cuoiNgay;
                ghiNhan.TryGetValue(item.Id, out var record);
                suKien.Add(new SuKienLichViewModel
                {
                    Lich = item, Buoi = record, Cot = cot,
                    PhutDau = (batDau - dauNgay).TotalMinutes,
                    PhutCuoi = (ketThuc - dauNgay).TotalMinutes,
                    TrangThai = SuKienLichViewModel.TrangThaiBuoi(item, record, bayGio)
                });
            }
        }
        XepCot(suKien);
        var gioDau = suKien.Count == 0 ? 8 : Math.Clamp((int)(suKien.Min(x => x.PhutDau) / 60) - 1, 0, 16);
        var gioCuoi = suKien.Count == 0 ? 22 : Math.Clamp((int)Math.Ceiling(suKien.Max(x => x.PhutCuoi) / 60) + 1, gioDau + 4, 24);
        return new LichTuanViewModel { Lop = lop, TuNgay = dauTuan, HomNay = homNay, GioDau = gioDau, GioCuoi = gioCuoi, SuKien = suKien };
    }

    private static void XepCot(List<SuKienLichViewModel> suKien)
    {
        foreach (var ngay in suKien.GroupBy(x => x.Cot))
        {
            var nhom = new List<SuKienLichViewModel>();
            var hetHan = new List<double>();
            foreach (var item in ngay.OrderBy(x => x.PhutDau).ThenBy(x => x.PhutCuoi).ThenBy(x => x.Lich.Id))
            {
                var phutCuoiHienThi = Math.Max(item.PhutCuoi, item.PhutDau + 25);
                if (hetHan.Count > 0 && item.PhutDau >= hetHan.Max())
                {
                    foreach (var old in nhom) old.SoCot = hetHan.Count;
                    nhom.Clear();
                    hetHan.Clear();
                }
                var cot = hetHan.FindIndex(x => x <= item.PhutDau);
                if (cot < 0) { cot = hetHan.Count; hetHan.Add(phutCuoiHienThi); }
                else hetHan[cot] = phutCuoiHienThi;
                item.CotTrongNgay = cot;
                nhom.Add(item);
            }
            foreach (var item in nhom) item.SoCot = hetHan.Count;
        }
    }
}

public sealed class SuKienLichViewModel
{
    public LichHoc Lich { get; init; } = null!;
    public BuoiHoc? Buoi { get; init; }
    public int Cot { get; init; }
    public int CotTrongNgay { get; set; }
    public int SoCot { get; set; } = 1;
    public double PhutDau { get; init; }
    public double PhutCuoi { get; init; }
    public string TrangThai { get; init; } = "Dự kiến";
    public string Kieu => TrangThai switch
    {
        "Đã học" => "completed",
        "Đã nghỉ" => "absent",
        "Đã hủy" or "Đã đổi lịch" => "cancelled",
        "Đang diễn ra" => "ongoing",
        "Chưa ghi nhận" => "unrecorded",
        _ => Lich.LaBuoiBu ? "makeup" : "planned"
    };

    public static string TrangThaiBuoi(LichHoc lich, BuoiHoc? buoi, DateTimeOffset bayGio)
    {
        if (lich.TrangThai == "DaHuy") return "Đã hủy";
        if (lich.TrangThai == "DaDieuChinh") return "Đã đổi lịch";
        return buoi?.TrangThai switch
        {
            "DaNghi" => "Đã nghỉ",
            "DaHoc" => "Đã học",
            "DangDienRa" => "Đang diễn ra",
            "DaHuy" => "Đã hủy",
            _ => lich.KetThucLuc <= bayGio ? "Chưa ghi nhận" : "Dự kiến"
        };
    }
}
