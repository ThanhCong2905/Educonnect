using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;

namespace EduConnect.Mvc.Areas.DanhGia.ViewModels;

public sealed class TaoDotPhanHoiViewModel
{
    public Guid MaLop
    {
        get; set;
    }
    [Required, StringLength(100)] public string Ten { get; set; } = "";
    public DateTimeOffset TuNgay
    {
        get; set;
    }
    public DateTimeOffset DenNgay
    {
        get; set;
    }
    public DateTimeOffset HanGui
    {
        get; set;
    }
}

public sealed class GuiDanhGiaViewModel
{
    public Guid MaLop
    {
        get; set;
    }
    public Guid DotPhanHoiId
    {
        get; set;
    }
    [Required, StringLength(4000)] public string NoiDung { get; set; } = "";
    [Range(1, 5)]
    public int? MucDoHaiLong
    {
        get; set;
    }
    public string? RowVersion
    {
        get; set;
    }
}

public sealed class QuyetDinhPhanHoiViewModel
{
    public Guid DotPhanHoiId
    {
        get; set;
    }
    [Required, RegularExpression("TiepTuc|DieuChinh|Dung")]
    public string QuyetDinh { get; set; } = "TiepTuc";
    [StringLength(2000)]
    public string? GhiChu
    {
        get; set;
    }
    public string? RowVersion
    {
        get; set;
    }
}

public sealed class DanhGiaDoiTacViewModel
{
    public Guid MaLop
    {
        get; set;
    }
    [Range(1, 5)] public int SoSao { get; set; } = 5;
    [Required, StringLength(2000, MinimumLength = 10)] public string NhanXet { get; set; } = "";
    public bool XacNhanRiengTu
    {
        get; set;
    }
    public string? RowVersion
    {
        get; set;
    }
}

public sealed class PhanHoiDoiTacViewModel
{
    public Guid DanhGiaId
    {
        get; set;
    }
    [Required, StringLength(2000)] public string NoiDung { get; set; } = "";
    [Required] public string RowVersion { get; set; } = "";
}

public sealed class TienDoViewModel
{
    public LopHoc Lop { get; set; } = null!;
    public int SoBuoiDaHoc
    {
        get; set;
    }
    public int SoBuoiNghi
    {
        get; set;
    }
    public int SoBuoiChuaGhiNhan
    {
        get; set;
    }
    public int GiaSuCoMat
    {
        get; set;
    }
    public int BenHocCoMat
    {
        get; set;
    }
    public int SoBaiDaGiao
    {
        get; set;
    }
    public int SoBaiDaNop
    {
        get; set;
    }
    public int SoBaiNopMuon
    {
        get; set;
    }
    public int SoBaiDaCham
    {
        get; set;
    }
    public decimal? DiemTrungBinh
    {
        get; set;
    }
    public List<DiemTienDoRow> Diem { get; set; } = [];
    public List<BuoiHoc> Buoi { get; set; } = [];
    public List<BaiTapGiao> Bai { get; set; } = [];
    public List<DotPhanHoi> Dot { get; set; } = [];
    public int TongDot
    {
        get; set;
    }
    public int Trang { get; set; } = 1;
    public List<PhieuDanhGia> Phieu { get; set; } = [];
    public List<QuyetDinhSauPhanHoi> QuyetDinh { get; set; } = [];
    public bool CoThePhanHoi
    {
        get; set;
    }
    public Dictionary<Guid, string> HoTen { get; set; } = [];
    public DateTimeOffset? TuNgay
    {
        get; set;
    }
    public DateTimeOffset? DenNgay
    {
        get; set;
    }
}

public sealed class DiemTienDoRow
{
    public Guid BaiTapId
    {
        get; set;
    }
    public string TieuDe { get; set; } = "";
    public decimal? Diem
    {
        get; set;
    }
    public DateTimeOffset ChamLuc
    {
        get; set;
    }
}

public sealed class DanhGiaCongKhaiPage
{
    public Guid TaiKhoanId
    {
        get; set;
    }
    public string HoTen { get; set; } = "";
    public decimal? TrungBinh
    {
        get; set;
    }
    public Page<DanhGiaDoiTac> DanhGia { get; set; } = new();
}
