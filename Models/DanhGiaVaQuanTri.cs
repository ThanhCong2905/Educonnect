using System.ComponentModel.DataAnnotations;

namespace EduConnect.Mvc.Models;

public sealed class DotPhanHoi
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LopHocId
    {
        get; set;
    }
    [MaxLength(100)] public string Ten { get; set; } = "";
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
    public Guid NguoiTaoId
    {
        get; set;
    }
    [MaxLength(32)] public string TrangThai { get; set; } = "DangMo";
    public byte[] RowVersion { get; set; } = [];
}

public sealed class PhieuDanhGia
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LopHocId
    {
        get; set;
    }
    public Guid NguoiGuiId
    {
        get; set;
    }
    public Guid? DotPhanHoiId
    {
        get; set;
    }
    [MaxLength(256)] public string MocDanhGia { get; set; } = "";
    public string NoiDung { get; set; } = "";
    public int? MucDoHaiLong
    {
        get; set;
    }
    public DateTimeOffset GuiLuc
    {
        get; set;
    }
    [MaxLength(16)]
    public string? QuyetDinhDeXuat
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
}

public sealed class LichSuPhanHoi
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PhieuDanhGiaId
    {
        get; set;
    }
    public string NoiDung { get; set; } = "";
    public int? MucDoHaiLong
    {
        get; set;
    }
    public Guid NguoiSuaId
    {
        get; set;
    }
    public DateTimeOffset ThayDoiLuc
    {
        get; set;
    }
}

public sealed class QuyetDinhSauPhanHoi
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DotPhanHoiId
    {
        get; set;
    }
    public Guid NguoiQuyetDinhId
    {
        get; set;
    }
    [MaxLength(16)] public string QuyetDinh { get; set; } = "";
    [MaxLength(2000)]
    public string? GhiChu
    {
        get; set;
    }
    public DateTimeOffset TaoLuc
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
}

public sealed class DanhGiaDoiTac
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LopHocId
    {
        get; set;
    }
    public Guid NguoiGuiId
    {
        get; set;
    }
    public Guid NguoiNhanId
    {
        get; set;
    }
    public int SoSao
    {
        get; set;
    }
    [MaxLength(2000)] public string NhanXet { get; set; } = "";
    public DateTimeOffset GuiLuc
    {
        get; set;
    }
    [MaxLength(2000)]
    public string? PhanHoi
    {
        get; set;
    }
    public DateTimeOffset? PhanHoiLuc
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
}

public sealed class LichSuDanhGiaDoiTac
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DanhGiaId
    {
        get; set;
    }
    public int SoSao
    {
        get; set;
    }
    [MaxLength(2000)] public string NhanXet { get; set; } = "";
    [MaxLength(2000)]
    public string? PhanHoi
    {
        get; set;
    }
    public Guid NguoiSuaId
    {
        get; set;
    }
    public DateTimeOffset ThayDoiLuc
    {
        get; set;
    }
}

public sealed class BaoCaoViPham
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? NguoiBiBaoCaoId
    {
        get; set;
    }
    public Guid NguoiBaoCaoId
    {
        get; set;
    }
    [MaxLength(32)] public string LoaiDoiTuong { get; set; } = "";
    public Guid DoiTuongId
    {
        get; set;
    }
    [MaxLength(2000)] public string LyDo { get; set; } = "";
    public string MoTa { get; set; } = "";
    [MaxLength(32)] public string TrangThai { get; set; } = "ChoXuLy";
    public Guid? NguoiXuLyId
    {
        get; set;
    }
    [MaxLength(2000)]
    public string? KetLuan
    {
        get; set;
    }
    [MaxLength(16)]
    public string? KetQua
    {
        get; set;
    }
    public DateTimeOffset? XuLyLuc
    {
        get; set;
    }
    public DateTimeOffset GuiLuc
    {
        get; set;
    }
    public Guid MaThaoTac
    {
        get; set;
    }
    public string BanChup { get; set; } = "";
    public byte[] RowVersion { get; set; } = [];
}

public sealed class LichSuXuLyBaoCao
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BaoCaoId
    {
        get; set;
    }
    public Guid NguoiThucHienId
    {
        get; set;
    }
    [MaxLength(32)] public string HanhDong { get; set; } = "";
    [MaxLength(2000)] public string NoiDung { get; set; } = "";
    public DateTimeOffset TaoLuc
    {
        get; set;
    }
}

public sealed class BienPhapQuanTri
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? NguonBienPhapId
    {
        get; set;
    }
    [MaxLength(32)] public string LoaiDoiTuong { get; set; } = "";
    public Guid DoiTuongId
    {
        get; set;
    }
    [MaxLength(32)] public string Loai { get; set; } = "";
    [MaxLength(2000)] public string LyDo { get; set; } = "";
    public Guid? BaoCaoId
    {
        get; set;
    }
    public Guid NguoiApDungId
    {
        get; set;
    }
    public DateTimeOffset ApDungLuc
    {
        get; set;
    }
    public Guid? NguoiGoId
    {
        get; set;
    }
    public DateTimeOffset? GoLuc
    {
        get; set;
    }
    [MaxLength(2000)]
    public string? LyDoGo
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
}

public sealed class NhatKyQuanTri
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid NguoiThucHienId
    {
        get; set;
    }
    public DateTimeOffset ThoiDiem
    {
        get; set;
    }
    [MaxLength(256)] public string HanhDong { get; set; } = "";
    [MaxLength(256)] public string LoaiDoiTuong { get; set; } = "";
    [MaxLength(256)] public string DoiTuongId { get; set; } = "";
    [MaxLength(256)] public string KetQua { get; set; } = "";
    [MaxLength(4000)]
    public string? ChiTiet
    {
        get; set;
    }
    [MaxLength(256)] public string MaLienKetYeuCau { get; set; } = "";
}
