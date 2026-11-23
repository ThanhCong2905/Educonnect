namespace EduConnect.Mvc.ViewModels;

public sealed record HuongDanMuc(string Ma, string Ten, string MoTa, string BieuTuong);

public sealed record MucPhiCongKhai(decimal ThapNhat, decimal CaoNhat, int SoMucPhi);
public sealed record HocPhiTheoLop(int Lop, MucPhiCongKhai? Chung, IReadOnlyList<MucPhiCongKhai?> TheoTrinhDo);
public sealed record BangHocPhi(string DonVi, int? MonHocId, IReadOnlyList<(int Id, string Ten)> MonHoc,
    IReadOnlyList<string> TrinhDo, IReadOnlyList<HocPhiTheoLop> CacLop,
    IReadOnlyDictionary<string, MucPhiCongKhai> MucPhiHienHanh, DateTimeOffset XemLuc);

public sealed record HuongDanViewModel(HuongDanMuc Muc, string? EmailHoTro, int? NgayLamViecDuyetHoSo)
{
    public static IReadOnlyList<HuongDanMuc> DanhMuc { get; } =
    [
        new("cach-hoat-dong", "Cách hoạt động", "Tìm người học hoặc gia sư, rồi trao đổi để thống nhất lịch học.", "people"),
        new("hoc-phi", "Học phí gia sư", "Xem khoảng giá và cách so sánh học phí theo giờ hoặc theo buổi.", "collection"),
        new("duyet-ho-so", "Duyệt hồ sơ gia sư", "Xem cần chuẩn bị giấy tờ gì và tìm kết quả duyệt ở đâu.", "person-badge"),
        new("bao-cao", "Hướng dẫn báo cáo", "Bạn gặp nội dung hoặc cách trao đổi thiếu phù hợp? Đây là cách báo cho EduConnect.", "flag"),
        new("bao-mat", "Chính sách bảo mật", "Bạn có thể xem dữ liệu nào được lưu, dùng vào việc gì và ai được xem.", "shield-check"),
        new("dieu-khoan", "Điều khoản sử dụng", "Quy tắc ứng xử và cách xử lý khi có vi phạm.", "journal-plus")
    ];
}
