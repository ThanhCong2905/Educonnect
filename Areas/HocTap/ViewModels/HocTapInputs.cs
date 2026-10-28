using System.ComponentModel.DataAnnotations;
namespace EduConnect.Mvc.Areas.HocTap.ViewModels;

public sealed class LichInput
{
    [Required, StringLength(256)] public string TieuDe { get; set; } = "Buổi học"; public DateTime BatDauLuc { get; set; } = DateTime.Today.AddDays(1).AddHours(19); public DateTime KetThucLuc { get; set; } = DateTime.Today.AddDays(1).AddHours(20).AddMinutes(30);
}
public sealed class LichHangLoatInput
{
    public Guid LopHocId
    {
        get; set;
    }
    public List<LichInput> Buoi { get; set; } = [];
}
public sealed class SuaLichInput
{
    public Guid Id
    {
        get; set;
    }
    public string RowVersion { get; set; } = ""; public string TrangThai { get; set; } = "DaDieuChinh"; public LichInput Lich { get; set; } = new();
}
public sealed class GhiNhanBuoiHocViewModel
{
    public Guid LichHocId
    {
        get; set;
    }
    public string? RowVersion
    {
        get; set;
    }
    [Required] public string KetQuaBuoiHoc { get; set; } = "DaDienRa"; [StringLength(10000)]
    public string? NoiDung
    {
        get; set;
    }
    [Required, StringLength(2000)] public string NhanXet { get; set; } = ""; public string TrangThaiGiaSu { get; set; } = "CoMat"; public string TrangThaiBenHoc { get; set; } = "CoMat";
}
public sealed class GiaoBaiTapViewModel
{
    public Guid LopHocId
    {
        get; set;
    }
    public Guid? BuoiHocId
    {
        get; set;
    }
    [Required, StringLength(256)] public string TieuDe { get; set; } = ""; [Required, StringLength(10000)] public string NoiDung { get; set; } = ""; public DateTime HanNopLuc { get; set; } = DateTime.Today.AddDays(7).AddHours(21); [StringLength(5000)]
    public string? TieuChiCham
    {
        get; set;
    }
    public List<IFormFile> Files { get; set; } = [];
}
public sealed class NopBaiViewModel
{
    public Guid BaiTapGiaoId
    {
        get; set;
    }
    public Guid MaThaoTac { get; set; } = Guid.NewGuid(); [StringLength(10000)]
    public string? NoiDung
    {
        get; set;
    }
    public List<IFormFile> Files { get; set; } = [];
}
public sealed class ChamBaiViewModel
{
    public Guid BaiTapNopId
    {
        get; set;
    }
    public string? RowVersion
    {
        get; set;
    }
    [Range(0, 10)]
    public decimal? Diem
    {
        get; set;
    }
    [Required, StringLength(2000)] public string NhanXet { get; set; } = "";
}
public sealed class GuiTinNhanViewModel
{
    public Guid MaHoiThoai
    {
        get; set;
    }
    public Guid MaThaoTac { get; set; } = Guid.NewGuid(); [StringLength(10000)] public string? NoiDung { get; set; }
    public List<IFormFile> Files { get; set; } = [];
}
