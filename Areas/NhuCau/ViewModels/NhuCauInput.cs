using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Infrastructure;
namespace EduConnect.Mvc.Areas.NhuCau.ViewModels;

public sealed class NhuCauInput
{
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
    [Range(1, int.MaxValue)] public int MonHocId { get; set; } = 1;
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
    [StringLength(256)]
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
    [StringLength(256)]
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
    [Required, StringLength(256)] public string HoTen { get; set; } = "";
    [Required, StringLength(256)] public string TrinhDo { get; set; } = "";
    [Required, StringLength(256)] public string ChuyenMon { get; set; } = "";
    [Required, StringLength(256)] public string KinhNghiem { get; set; } = "";
    [Required, StringLength(5000)] public string MoTa { get; set; } = "";
    [Required, StringLength(256)] public string KhuVuc { get; set; } = "";
    public List<int> MonHocIds { get; set; } = [];
    [Required, StringLength(256)] public string PhamViDay { get; set; } = "Lớp 6–12";
}
