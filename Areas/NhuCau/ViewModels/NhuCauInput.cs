using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Infrastructure;
namespace EduConnect.Mvc.Areas.NhuCau.ViewModels;

public sealed class NhuCauInput
{
    public Guid MaThaoTac { get; set; } = Guid.NewGuid();
    public List<int> MonHocIds { get; set; } = []; public bool ChonMonKhac
    {
        get; set;
    }
    [StringLength(100)]
    public string? TenMonKhac
    {
        get; set;
    }
    public Guid? Id
    {
        get; set;
    }
    public string? RowVersion
    {
        get; set;
    }
    [Range(0, int.MaxValue)] public int MonHocId { get; set; } = 1;
    [StringLength(256)]
    public string? KhoiLop
    {
        get; set;
    }
    [StringLength(256)]
    public string? HocLuc
    {
        get; set;
    }
    [StringLength(2000)]
    public string? MucTieu
    {
        get; set;
    }
    [Range(1, 7)] public int? SoBuoiTuan { get; set; } = 2;
    [Required, RegularExpression("TrucTuyen|TrucTiep")] public string HinhThucHoc { get; set; } = "TrucTuyen";
    [StringLength(256)]
    public string? KhuVuc
    {
        get; set;
    }
    [StringLength(256, ErrorMessage = "Phường / xã tối đa 256 ký tự.")]
    public string? PhuongXa { get; set; }
    [StringLength(500)]
    public string? DiaChiChiTiet
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
    [StringLength(256)]
    public string? TenNguoiHoc
    {
        get; set;
    }
    [Range(1, 120)]
    public int? TuoiNguoiHoc
    {
        get; set;
    }
    public bool BanNhap
    {
        get; set;
    }
    public List<SlotInput> LichRanh { get; set; } = [new()];
}
public sealed class HoSoInput
{
    public Guid? MinhChungTrinhDoId { get; set; }
    public Guid? MinhChungKinhNghiemId { get; set; }
    public IFormFile? TepTrinhDo { get; set; }
    public IFormFile? TepKinhNghiem { get; set; }
    public bool DangHienThi { get; set; }
    public bool DayTrucTuyen { get; set; }
    public bool DayTrucTiep { get; set; }
    public string TrangThaiDuyet { get; set; } = "BanNhap";
    public string? LyDoTuChoi { get; set; }
    public List<MonDayInput> MonDay { get; set; } = [];
    public List<IFormFile> Files { get; set; } = []; public List<Guid> XoaTepIds { get; set; } = []; public bool ChonMonKhac
    {
        get; set;
    }
    [StringLength(100)]
    public string? TenMonKhac
    {
        get; set;
    }
    public string? RowVersion
    {
        get; set;
    }
    [Required(ErrorMessage = "Nhập họ tên."), StringLength(256, ErrorMessage = "Họ tên không quá 256 ký tự.")] public string HoTen { get; set; } = "";
    [Required(ErrorMessage = "Chọn trình độ."), StringLength(256, ErrorMessage = "Trình độ không quá 256 ký tự.")] public string TrinhDo { get; set; } = "";
    [StringLength(256)] public string ChuyenMon { get; set; } = "";
    [Required(ErrorMessage = "Chọn kinh nghiệm giảng dạy."), StringLength(256, ErrorMessage = "Kinh nghiệm không quá 256 ký tự.")] public string KinhNghiem { get; set; } = "";
    [Required(ErrorMessage = "Nhập giới thiệu bản thân."), StringLength(4000, MinimumLength = 100, ErrorMessage = "Giới thiệu cần từ 100 đến 4.000 ký tự.")] public string MoTa { get; set; } = "";
    [Required(ErrorMessage = "Chọn tỉnh hoặc thành phố."), StringLength(256, ErrorMessage = "Khu vực không quá 256 ký tự.")] public string KhuVuc { get; set; } = "";
    [StringLength(256, ErrorMessage = "Phường / xã tối đa 256 ký tự.")]
    public string? PhuongXa { get; set; }
    [StringLength(500, ErrorMessage = "Địa chỉ chi tiết tối đa 500 ký tự.")]
    public string? DiaChiChiTiet { get; set; }
    public List<int> MonHocIds { get; set; } = [];
    [Required, StringLength(256)] public string PhamViDay { get; set; } = "Lớp 6–12";
}
public sealed class MonDayInput
{
    public Guid? MinhChungId { get; set; }
    public IFormFile? TepMinhChung { get; set; }
    public List<int> LopDay { get; set; } = [];
    public int MonHocId { get; set; }
    [StringLength(100)] public string? TenMonKhac { get; set; }
    [StringLength(256)] public string PhamViDay { get; set; } = "";
    public decimal? HocPhiDeXuat { get; set; }
    public string? DonViHocPhi { get; set; }
}
