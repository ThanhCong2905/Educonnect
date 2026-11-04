using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Infrastructure;
namespace EduConnect.Mvc.Areas.KetNoi.ViewModels;

public sealed class GuiLienHeInput
{
    public Guid DoiTuongId
    {
        get; set;
    }
    [Required, StringLength(256)] public string LoiNhan { get; set; } = "";
}
public class GuiYeuCauInput
{
    public Guid LienHeId
    {
        get; set;
    }
    public Guid? NhuCauId
    {
        get; set;
    }
    [Required, StringLength(256)] public string LoiNhan { get; set; } = "";
}
public sealed class GuiYeuCauViewModel : GuiYeuCauInput
{
    public Guid? MaNhuCau
    {
        get => NhuCauId; set => NhuCauId = value;
    }
    public Guid MaDoiTuong
    {
        get; set;
    }
}

public sealed class ChapNhanDeXuatViewModel
{
    public Guid MaYeuCau
    {
        get; set;
    }
    public Guid Id
    {
        get => MaYeuCau; set => MaYeuCau = value;
    }
    public byte[] RowVersion { get; set; } = [];
}

public sealed class PhanHoiHuyViewModel
{
    public Guid MaYeuCauHuy
    {
        get; set;
    }
    public bool DongY
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
}
public sealed class DeXuatLopViewModel
{
    public Guid LienHeId
    {
        get; set;
    }
    public Guid? Id
    {
        get; set;
    }
    public Guid? NhuCauId
    {
        get; set;
    }
    public string? RowVersion
    {
        get; set;
    }
    [Required, StringLength(256)] public string TenLop { get; set; } = "";
    [Range(1, int.MaxValue)] public int MonHocId { get; set; } = 1;
    [Required, StringLength(256)] public string MucTieu { get; set; } = "";
    [Range(1, 7)] public int SoBuoiTuan { get; set; } = 1;
    [Range(1, 480)] public int ThoiLuongPhut { get; set; } = 90;
    public DateTime BatDauHocLuc { get; set; } = DateTime.Today.AddDays(2);
    [Required, RegularExpression("TrucTuyen|TrucTiep")] public string HinhThucHoc { get; set; } = "TrucTuyen";
    [StringLength(256)]
    public string? DiaDiem
    {
        get; set;
    }
    public decimal HocPhi { get; set; } = 150000; public string DonViHocPhi { get; set; } = "VND/Gio";
    public List<SlotInput> LichRanh { get; set; } = [new()];
}
