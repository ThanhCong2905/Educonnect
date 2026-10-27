using System.ComponentModel.DataAnnotations;
namespace EduConnect.Mvc.Models;

public sealed class MonHoc
{
    public int Id
    {
        get; set;
    }
    public string TenMon { get; set; } = "";
    public bool DangSuDung
    {
        get; set;
    }
}
public sealed class HoSoGiaSu
{
    public bool DangHienThi { get; set; }
    public bool DayTrucTuyen { get; set; }
    public bool DayTrucTiep { get; set; }
    public DateTimeOffset? GuiDuyetLuc { get; set; }
    public DateTimeOffset? DuyetLuc { get; set; }
    public Guid? NguoiDuyetId { get; set; }
    public Guid Id
    {
        get; set;
    }
    public Guid GiaSuId
    {
        get; set;
    }
    public string TrinhDo { get; set; } = "";
    public string ChuyenMon { get; set; } = "";
    public string KinhNghiem { get; set; } = "";
    public string MoTa { get; set; } = "";
    public string KhuVuc { get; set; } = "";
    public string TrangThaiDuyet { get; set; } = "";
    public string? LyDoTuChoi
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
}
public sealed class HoSoGiaSuMonDay
{
    public decimal? HocPhiDeXuat { get; set; }
    public string? DonViHocPhi { get; set; }
    public Guid HoSoGiaSuId
    {
        get; set;
    }
    public int MonHocId
    {
        get; set;
    }
    public string PhamViDay { get; set; } = "";
}
public sealed class NhuCauHoc
{
    public Guid Id
    {
        get; set;
    }
    public Guid ChuSoHuuId
    {
        get; set;
    }
    public string LoaiNhuCau { get; set; } = "";
    public int MonHocId
    {
        get; set;
    }
    public string? KhoiLop
    {
        get; set;
    }
    public string? HocLuc
    {
        get; set;
    }
    public string? MucTieu
    {
        get; set;
    }
    public int? SoBuoiTuan
    {
        get; set;
    }
    public string HinhThucHoc { get; set; } = "";
    public string? KhuVuc
    {
        get; set;
    }
    public string? DiaChiChiTiet
    {
        get; set;
    }
    public decimal? HocPhiDuKien
    {
        get; set;
    }
    public string? TenNguoiHoc
    {
        get; set;
    }
    public int? TuoiNguoiHoc
    {
        get; set;
    }
    public string TrangThai { get; set; } = "";
    public byte[] RowVersion { get; set; } = [];
    public string? DonViHocPhi
    {
        get; set;
    }
    public DateTimeOffset TaoLuc
    {
        get; set;
    }
}
public sealed class KhungGioRanh
{
    public Guid Id
    {
        get; set;
    }
    public Guid NhuCauId
    {
        get; set;
    }
    public byte ThuTrongTuan
    {
        get; set;
    }
    public TimeOnly GioBatDau
    {
        get; set;
    }
    public TimeOnly GioKetThuc
    {
        get; set;
    }
}
public sealed class YeuCauKetNoiLopHoc
{
    public Guid Id
    {
        get; set;
    }
    public Guid GiaSuId
    {
        get; set;
    }
    public Guid BenHocId
    {
        get; set;
    }
    public Guid NguoiGuiId
    {
        get; set;
    }
    public Guid? NhuCauId
    {
        get; set;
    }
    public string? LoiNhan
    {
        get; set;
    }
    public string TrangThai { get; set; } = "";
    public DateTimeOffset GuiLuc
    {
        get; set;
    }
    public DateTimeOffset HetHanLuc
    {
        get; set;
    }
    public string? TenLopDuKien
    {
        get; set;
    }
    public int? MonHocId
    {
        get; set;
    }
    public string? MucTieu
    {
        get; set;
    }
    public int? SoBuoiTuan
    {
        get; set;
    }
    public int? ThoiLuongPhut
    {
        get; set;
    }
    public DateTimeOffset? BatDauHocLuc
    {
        get; set;
    }
    public string? HinhThucHoc
    {
        get; set;
    }
    public string? DiaDiem
    {
        get; set;
    }
    public decimal? HocPhi
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
    public string? DonViHocPhi
    {
        get; set;
    }
    public Guid LienHeId
    {
        get; set;
    }
    public Guid? NguoiDeXuatId
    {
        get; set;
    }
    public DateTimeOffset? DeXuatLuc
    {
        get; set;
    }
}
public sealed class YeuCauHuyKetNoi
{
    public Guid Id
    {
        get; set;
    }
    public Guid LopHocId
    {
        get; set;
    }
    public Guid NguoiGuiId
    {
        get; set;
    }
    public string LyDo { get; set; } = "";
    public string TrangThai { get; set; } = "";
    public DateTimeOffset GuiLuc
    {
        get; set;
    }
    public DateTimeOffset HetHanLuc
    {
        get; set;
    }
    public DateTimeOffset? PhanHoiLuc
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
}
public sealed class CuocHoiThoai
{
    public Guid Id
    {
        get; set;
    }
    public string LoaiHoiThoai { get; set; } = "";
    public Guid? GiaSuId
    {
        get; set;
    }
    public Guid? BenHocId
    {
        get; set;
    }
    public Guid? LopHocId
    {
        get; set;
    }
    public Guid? HoiThoaiTruocKetNoiId
    {
        get; set;
    }
    public DateTimeOffset TaoLuc
    {
        get; set;
    }
    public Guid? LienHeId
    {
        get; set;
    }
}
public sealed class TinNhan
{
    public Guid Id
    {
        get; set;
    }
    public Guid CuocHoiThoaiId
    {
        get; set;
    }
    public Guid NguoiGuiId
    {
        get; set;
    }
    public string NoiDung { get; set; } = "";
    public DateTimeOffset GuiLuc
    {
        get; set;
    }
    public Guid MaThaoTac
    {
        get; set;
    }
}
public sealed class LopHoc
{
    public Guid Id
    {
        get; set;
    }
    public Guid YeuCauKetNoiId
    {
        get; set;
    }
    public Guid GiaSuId
    {
        get; set;
    }
    public Guid BenHocId
    {
        get; set;
    }
    public string TenLop { get; set; } = "";
    public int MonHocId
    {
        get; set;
    }
    public string MucTieu { get; set; } = "";
    public int SoBuoiTuan
    {
        get; set;
    }
    public int ThoiLuongPhut
    {
        get; set;
    }
    public string HinhThucHoc { get; set; } = "";
    public string? DiaDiem
    {
        get; set;
    }
    public decimal HocPhi
    {
        get; set;
    }
    public string TrangThai { get; set; } = "";
    public DateTimeOffset? GiaSuXacNhanKetThucLuc
    {
        get; set;
    }
    public DateTimeOffset? BenHocXacNhanKetThucLuc
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
    public string DonViHocPhi { get; set; } = "";
    public string TenNguoiHoc { get; set; } = "";
    public int? TuoiNguoiHoc
    {
        get; set;
    }
}
public sealed class ThanhVienLopHoc
{
    public Guid Id
    {
        get; set;
    }
    public Guid LopHocId
    {
        get; set;
    }
    public Guid TaiKhoanId
    {
        get; set;
    }
    public Guid NguoiDeNghiId
    {
        get; set;
    }
    public string TrangThaiDuyet { get; set; } = "";
    public string? MucQuyen
    {
        get; set;
    }
    public DateTimeOffset DeNghiLuc
    {
        get; set;
    }
    public DateTimeOffset? DuyetLuc
    {
        get; set;
    }
    public DateTimeOffset? RoiLopLuc
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
}
public sealed class LichHoc
{
    public Guid Id
    {
        get; set;
    }
    public Guid LopHocId
    {
        get; set;
    }
    public string TieuDe { get; set; } = "";
    public DateTimeOffset BatDauLuc
    {
        get; set;
    }
    public DateTimeOffset KetThucLuc
    {
        get; set;
    }
    public bool LaBuoiBu
    {
        get; set;
    }
    public string TrangThai { get; set; } = "";
    public byte[] RowVersion { get; set; } = [];
    public Guid? LichGocId
    {
        get; set;
    }
}
public sealed class BuoiHoc
{
    public Guid Id
    {
        get; set;
    }
    public Guid LichHocId
    {
        get; set;
    }
    public string TieuDe { get; set; } = "";
    public string? NoiDungGiangDay
    {
        get; set;
    }
    public string NhanXet { get; set; } = "";
    public string TrangThaiThamDu { get; set; } = "";
    public DateTimeOffset GhiNhanLuc
    {
        get; set;
    }
    public Guid NguoiGhiNhanId
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
    public string TrangThaiThamDuGiaSu { get; set; } = "";
    public string TrangThaiThamDuBenHoc { get; set; } = "";
    public string KetQuaBuoiHoc { get; set; } = "";
    public DateTimeOffset CapNhatLuc
    {
        get; set;
    }
    public int PhienBan
    {
        get; set;
    }
}
public sealed class BaiTapGiao
{
    public Guid Id
    {
        get; set;
    }
    public Guid LopHocId
    {
        get; set;
    }
    public Guid? BuoiHocId
    {
        get; set;
    }
    public Guid NguoiGiaoId
    {
        get; set;
    }
    public string TieuDe { get; set; } = "";
    public string NoiDung { get; set; } = "";
    public DateTimeOffset HanNopLuc
    {
        get; set;
    }
    public string? TieuChiCham
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
}
public sealed class BaiTapNop
{
    public Guid Id
    {
        get; set;
    }
    public Guid BaiTapGiaoId
    {
        get; set;
    }
    public Guid NguoiNopId
    {
        get; set;
    }
    public string? NoiDung
    {
        get; set;
    }
    public DateTimeOffset NopLuc
    {
        get; set;
    }
    public Guid MaThaoTac
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
    public int LanNop
    {
        get; set;
    }
}
public sealed class KetQuaChamBai
{
    public Guid Id
    {
        get; set;
    }
    public Guid BaiTapNopId
    {
        get; set;
    }
    public Guid NguoiChamId
    {
        get; set;
    }
    public decimal? Diem
    {
        get; set;
    }
    public string NhanXet { get; set; } = "";
    public DateTimeOffset ChamLuc
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
}
public sealed class GiaSu
{
    public Guid TaiKhoanId
    {
        get; set;
    }
}
public sealed class TepDinhKem
{
    public Guid Id
    {
        get; set;
    }
    public Guid NguoiTaiLenId
    {
        get; set;
    }
    public string TenGoc { get; set; } = "";
    public string TenLuuTru { get; set; } = "";
    public string LoaiNoiDung { get; set; } = "";
    public long KichThuoc
    {
        get; set;
    }
    public string Checksum { get; set; } = "";
    public string TrangThaiQuet { get; set; } = "";
    public DateTimeOffset TaoLuc
    {
        get; set;
    }
    public Guid? HoSoGiaSuId
    {
        get; set;
    }
    public Guid? BaiTapGiaoId
    {
        get; set;
    }
    public Guid? BaiTapNopId
    {
        get; set;
    }
    public Guid? TinNhanId
    {
        get; set;
    }
}
public sealed class ThongBao
{
    public Guid Id
    {
        get; set;
    }
    public Guid NguoiNhanId
    {
        get; set;
    }
    public string LoaiThongBao { get; set; } = "";
    public Guid DoiTuongId
    {
        get; set;
    }
    public string NoiDungTomTat { get; set; } = "";
    public DateTimeOffset TaoLuc
    {
        get; set;
    }
    public DateTimeOffset? DaDocLuc
    {
        get; set;
    }
    public Guid MaSuKien
    {
        get; set;
    }
}
public sealed class SuKienChoXuLy
{
    public Guid Id
    {
        get; set;
    }
    public string LoaiSuKien { get; set; } = "";
    public string DuLieuToiThieu { get; set; } = "";
    public DateTimeOffset TaoLuc
    {
        get; set;
    }
    public string TrangThai { get; set; } = "";
    public int SoLanThu
    {
        get; set;
    }
    public DateTimeOffset? ThuLaiLuc
    {
        get; set;
    }
    public DateTimeOffset? XuLyXongLuc
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
    public Guid? LeaseId
    {
        get; set;
    }
    public DateTimeOffset? LeaseHetHanLuc
    {
        get; set;
    }
}
public sealed class YeuCauLienHe
{
    public Guid Id
    {
        get; set;
    }
    public Guid GiaSuId
    {
        get; set;
    }
    public Guid BenHocId
    {
        get; set;
    }
    public Guid NguoiGuiId
    {
        get; set;
    }
    public string LoiNhan { get; set; } = "";
    public string TrangThai { get; set; } = "";
    public DateTimeOffset GuiLuc
    {
        get; set;
    }
    public Guid? NguoiPhanHoiId
    {
        get; set;
    }
    public DateTimeOffset? PhanHoiLuc
    {
        get; set;
    }
    public byte[] RowVersion { get; set; } = [];
}
public sealed class KhungGioDeXuat
{
    public Guid Id
    {
        get; set;
    }
    public Guid YeuCauKetNoiId
    {
        get; set;
    }
    public byte ThuTrongTuan
    {
        get; set;
    }
    public TimeOnly GioBatDau
    {
        get; set;
    }
    public TimeOnly GioKetThuc
    {
        get; set;
    }
}
public sealed class LichSuGhiNhanBuoiHoc
{
    public Guid Id
    {
        get; set;
    }
    public Guid BuoiHocId
    {
        get; set;
    }
    public int PhienBan
    {
        get; set;
    }
    public string TieuDe { get; set; } = "";
    public string KetQuaBuoiHoc { get; set; } = "";
    public string? NoiDungGiangDay
    {
        get; set;
    }
    public string NhanXet { get; set; } = "";
    public string TrangThaiThamDuGiaSu { get; set; } = "";
    public string TrangThaiThamDuBenHoc { get; set; } = "";
    public Guid NguoiGhiNhanId
    {
        get; set;
    }
    public DateTimeOffset GhiNhanLuc
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
public sealed class NguoiThamGia
{
    public Guid Id
    {
        get; set;
    }
    public string HoTen { get; set; } = "";
    public string LoaiNguoi { get; set; } = "";
}
