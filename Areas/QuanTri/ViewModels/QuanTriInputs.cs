using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;

namespace EduConnect.Mvc.Areas.QuanTri.ViewModels;

public sealed class TaoBaoCaoViewModel
{
    public Guid MaThaoTac { get; set; } = Guid.NewGuid();
    [Required, RegularExpression("TaiKhoan|HoSoGiaSu|NhuCauHoc|LopHoc|TinNhan|DanhGiaDoiTac")]
    public string LoaiDoiTuong { get; set; } = "TaiKhoan";
    public Guid DoiTuongId
    {
        get; set;
    }
    [Required, StringLength(2000)] public string LyDo { get; set; } = "";
    [Required, StringLength(4000, MinimumLength = 10)] public string MoTa { get; set; } = "";
    public List<IFormFile> Files { get; set; } = [];
}

public sealed class XuLyBaoCaoViewModel
{
    public Guid Id
    {
        get; set;
    }
    [Required] public string RowVersion { get; set; } = "";
    [Required, RegularExpression("XuLy|BacBo")] public string KetQua { get; set; } = "XuLy";
    [Required, StringLength(2000)] public string KetLuan { get; set; } = "";
    [Required, RegularExpression("NhacNho|AnNoiDung|KhoaTaiKhoan")]
    public string BienPhap { get; set; } = "NhacNho";
}

public sealed class PhanQuyenQuanTriViewModel
{
    public Guid TaiKhoanId
    {
        get; set;
    }
    [Required] public string ConcurrencyStamp { get; set; } = "";
    public List<string> Quyen { get; set; } = [];
    [Required, StringLength(2000)] public string LyDo { get; set; } = "";
}

public sealed class BaoCaoChiTietViewModel
{
    public BaoCaoViPham BaoCao { get; set; } = null!;
    public List<LichSuXuLyBaoCao> LichSu { get; set; } = [];
    public List<TepDinhKem> Tep { get; set; } = [];
    public bool CoTheXuLy
    {
        get; set;
    }
}

public sealed class QuanTriDashboard
{
    public int HoSoChoDuyet
    {
        get; set;
    }
    public int BaoCaoChoXuLy
    {
        get; set;
    }
    public int TaiKhoanBiKhoa
    {
        get; set;
    }
    public int LopTamDung
    {
        get; set;
    }
    public List<NhatKyQuanTriRow> GanDay { get; set; } = [];
}

public sealed class NhatKyQuanTriRow
{
    public NhatKyQuanTri BanGhi { get; set; } = null!;
    public string HoTen { get; set; } = "";
}

public sealed class TaiKhoanQuanTriPage
{
    public Page<TaiKhoan> DanhSach { get; set; } = new();
    public Dictionary<Guid, string> VaiTro { get; set; } = [];
}

public sealed class TaiKhoanQuanTriChiTiet
{
    public TaiKhoan TaiKhoan { get; set; } = null!;
    public string VaiTro { get; set; } = "";
    public List<string> Quyen { get; set; } = [];
    public List<BienPhapQuanTri> BienPhap { get; set; } = [];
    public int YeuCauCho
    {
        get; set;
    }
    public int LopDangHoc
    {
        get; set;
    }
}

public sealed class MonHocQuanTriViewModel
{
    public int Id
    {
        get; set;
    }
    [Required, StringLength(100)] public string TenMon { get; set; } = "";
    public bool DangSuDung { get; set; } = true;
    public string? RowVersion
    {
        get; set;
    }
}
