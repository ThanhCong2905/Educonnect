namespace EduConnect.Mvc.Areas.QuanTri.ViewModels;

public static class HoSoDuyetFields
{
    public static IReadOnlyDictionary<string, string> Labels { get; } = new Dictionary<string, string>
    {
        ["HoTen"] = "Họ tên",
        ["TrinhDo"] = "Trình độ và minh chứng",
        ["KinhNghiem"] = "Kinh nghiệm và minh chứng",
        ["KhuVuc"] = "Khu vực và địa chỉ",
        ["HinhThuc"] = "Hình thức dạy",
        ["MoTa"] = "Giới thiệu",
        ["MonDay"] = "Môn dạy và học phí",
        ["TepKhac"] = "Minh chứng bổ sung"
    };
}
