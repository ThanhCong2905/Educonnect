
using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Infrastructure;
namespace EduConnect.Mvc.Areas.TimKiem.ViewModels;

public sealed class BoLocTimKiemViewModel
{
    public List<int> MonHocIds { get; set; } = [];
    [StringLength(256)] public string? TrinhDo { get; set; }
    [StringLength(100)]
    public string? TuKhoa
    {
        get; set;
    }
    public int? MonHocId
    {
        get; set;
    }
    public string? KhoiLop
    {
        get; set;
    }
    public string? HinhThucHoc
    {
        get; set;
    }
    public string? KhuVuc
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
    public string SapXep { get; set; } = "MoiNhat"; public int Trang { get; set; } = 1;
}
// Không đưa tên, tuổi người học và địa chỉ chi tiết vào dữ liệu công khai.
public sealed class UngVienViewModel
{
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
