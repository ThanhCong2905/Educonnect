
using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Infrastructure;
namespace EduConnect.Mvc.Areas.TimKiem.ViewModels;

public sealed class BoLocTimKiemViewModel
{
    [MaxLength(10, ErrorMessage = "Chọn tối đa 10 môn học.")] public List<int> MonHocIds { get; set; } = [];
    [StringLength(256)] public string? TrinhDo { get; set; }
    [StringLength(100, ErrorMessage = "Từ khóa tối đa 100 ký tự.")]
    public string? TuKhoa
    {
        get; set;
    }
    [Range(1, int.MaxValue, ErrorMessage = "Môn học không hợp lệ.")] public int? MonHocId
    {
        get; set;
    }
    [StringLength(256, ErrorMessage = "Khối lớp hoặc phạm vi dạy tối đa 256 ký tự.")] public string? KhoiLop
    {
        get; set;
    }
    [RegularExpression("TrucTuyen|TrucTiep", ErrorMessage = "Chọn hình thức học trong danh sách.")] public string? HinhThucHoc
    {
        get; set;
    }
    [StringLength(256, ErrorMessage = "Khu vực tối đa 256 ký tự.")] public string? KhuVuc
    {
        get; set;
    }
    public decimal? HocPhiToiDa
    {
        get; set;
    }
    public string DonViHocPhi { get; set; } = "VND/Gio"; public byte? ThuTrongTuan
    {
        get; set;
    }
    public TimeOnly? GioBatDau
    {
        get; set;
    }
    public TimeOnly? GioKetThuc
    {
        get; set;
    }
    [Required(ErrorMessage = "Chọn cách sắp xếp."), RegularExpression("MoiNhat|Ten|HocPhiTang|HocPhiGiam", ErrorMessage = "Chọn cách sắp xếp trong danh sách.")]
    public string SapXep { get; set; } = "MoiNhat";
    [Range(1, 100000, ErrorMessage = "Số trang phải từ 1 đến 100000.")] public int Trang { get; set; } = 1;
}
// Không đưa tên, tuổi người học và địa chỉ chi tiết vào dữ liệu công khai.
public class UngVienViewModel
{
    public string? AnhDaiDienUrl { get; set; }
    public List<int> MonHocIds { get; set; } = [];
    public List<MonDayCongKhai> MonTheoHoSo { get; set; } = [];
    public bool DayTrucTuyen { get; set; }
    public bool DayTrucTiep { get; set; }
    public string? ChuyenMon
    {
        get; set;
    }
    public string? KhuVucGiaSu
    {
        get; set;
    }
    public List<string> MonDay { get; set; } = [];
    public Guid Id
    {
        get; set;
    }
    public Guid ChuSoHuuId
    {
        get; set;
    }
    public string HoTen { get; set; } = ""; public string LoaiNguoi { get; set; } = "";
    public string TenMon { get; set; } = ""; public int MonHocId
    {
        get; set;
    }
    public string? KhoiLop
    {
        get; set;
    }
    public string? MucTieu
    {
        get; set;
    }
    public string HinhThucHoc { get; set; } = ""; public string? KhuVuc
    {
        get; set;
    }
    public decimal? HocPhiDuKien
    {
        get; set;
    }
    public string? DonViHocPhi
    {
        get; set;
    }
    public string? TrinhDo
    {
        get; set;
    }
    public string? KinhNghiem
    {
        get; set;
    }
    public string? MoTa
    {
        get; set;
    }
    public DateTimeOffset TaoLuc
    {
        get; set;
    }
    public List<SlotInput> Slots { get; set; } = []; public List<string> LyDo { get; set; } = [];
}
public sealed class MonDayCongKhai
{
    public int MonHocId { get; set; }
    public string TenMon { get; set; } = "";
    public string PhamViDay { get; set; } = "";
    public decimal? HocPhi { get; set; }
    public string? DonVi { get; set; }
}
public sealed class SearchPage
{
    public BoLocTimKiemViewModel Filter { get; set; } = new(); public List<UngVienViewModel> Items { get; set; } = []; public int Total
    {
        get; set;
    }
}
