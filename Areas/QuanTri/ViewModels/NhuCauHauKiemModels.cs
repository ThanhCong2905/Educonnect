using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Models;

namespace EduConnect.Mvc.Areas.QuanTri.ViewModels;

public sealed class NhuCauHauKiemRow
{
    public NhuCauHoc NhuCau { get; set; } = null!;
    public string HoTen { get; set; } = "";
    public bool DangAn { get; set; }
    public int BaoCaoCho { get; set; }
}

public sealed class NhuCauHauKiemViewModel
{
    public NhuCauHoc NhuCau { get; set; } = null!;
    public string HoTen { get; set; } = "";
    public string MonHoc { get; set; } = "";
    public List<KhungGioRanh> KhungGio { get; set; } = [];
    public List<BienPhapQuanTri> BienPhap { get; set; } = [];
    public List<BaoCaoViPham> BaoCao { get; set; } = [];
    public List<NhatKyQuanTri> LichSu { get; set; } = [];
}

public sealed class XuLyNhuCauViewModel
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Tải lại nhu cầu trước khi xử lý.")]
    public string RowVersion { get; set; } = "";
    [Required, RegularExpression("YeuCauChinhSua|AnNoiDung")]
    public string QuyetDinh { get; set; } = "YeuCauChinhSua";
    [Required(ErrorMessage = "Nhập lý do và nội dung cần người đăng chỉnh sửa.")]
    [StringLength(2000, ErrorMessage = "Lý do tối đa 2.000 ký tự.")]
    public string LyDo { get; set; } = "";
}
