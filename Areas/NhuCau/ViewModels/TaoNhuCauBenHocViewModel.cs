using System.ComponentModel.DataAnnotations;
using EduConnect.Mvc.Infrastructure;

namespace EduConnect.Mvc.Areas.NhuCau.ViewModels;

public sealed class TaoNhuCauBenHocViewModel
{
    public Guid MaThaoTac { get; set; } = Guid.NewGuid();
    public Guid? MaNhuCau { get; set; }
    public byte[]? RowVersion { get; set; }
    public List<int> MonHocIds { get; set; } = [];
    public int MonHocId { get; set; }
    public bool ChonMonKhac { get; set; }
    [StringLength(100)] public string? TenMonKhac { get; set; }
    [StringLength(256)] public string? KhoiLop { get; set; }
    [StringLength(256)] public string? HocLuc { get; set; }
    [StringLength(256)] public string? MucTieu { get; set; }
    [Range(1, 7)] public int? SoBuoiTuan { get; set; } = 2;
    [Required, RegularExpression("TrucTuyen|TrucTiep")]
    public string HinhThucHoc { get; set; } = "TrucTuyen";
    [StringLength(256)] public string? KhuVuc { get; set; }
    [StringLength(256)] public string? DiaChiChiTiet { get; set; }
    public decimal? HocPhiDuKien { get; set; }
    public string? DonViHocPhi { get; set; }
    [StringLength(256)] public string? TenNguoiHoc { get; set; }
    [Range(1, 120)] public int? TuoiNguoiHoc { get; set; }
    public bool BanNhap { get; set; }
    public List<SlotInput> LichRanh { get; set; } = [new()];

    public NhuCauInput ToInput() => new()
    {
        Id = MaNhuCau, MaThaoTac = MaThaoTac, RowVersion = RowVersion == null ? null : Convert.ToBase64String(RowVersion),
        MonHocIds = MonHocIds.Count > 0 ? [.. MonHocIds] : MonHocId > 0 ? [MonHocId] : [], MonHocId = MonHocId, ChonMonKhac = ChonMonKhac,
        TenMonKhac = TenMonKhac, KhoiLop = KhoiLop, HocLuc = HocLuc, MucTieu = MucTieu,
        SoBuoiTuan = SoBuoiTuan, HinhThucHoc = HinhThucHoc, KhuVuc = KhuVuc,
        DiaChiChiTiet = DiaChiChiTiet, HocPhiDuKien = HocPhiDuKien, DonViHocPhi = DonViHocPhi,
        TenNguoiHoc = TenNguoiHoc, TuoiNguoiHoc = TuoiNguoiHoc, BanNhap = BanNhap, LichRanh = [.. LichRanh]
    };

    public static TaoNhuCauBenHocViewModel FromInput(NhuCauInput m) => new()
    {
        MaNhuCau = m.Id, MaThaoTac = m.MaThaoTac, RowVersion = string.IsNullOrEmpty(m.RowVersion) ? null : Rules.Version(m.RowVersion),
        MonHocIds = [.. m.MonHocIds], MonHocId = m.MonHocIds.FirstOrDefault(), ChonMonKhac = m.ChonMonKhac,
        TenMonKhac = m.TenMonKhac, KhoiLop = m.KhoiLop, HocLuc = m.HocLuc, MucTieu = m.MucTieu,
        SoBuoiTuan = m.SoBuoiTuan, HinhThucHoc = m.HinhThucHoc, KhuVuc = m.KhuVuc,
        DiaChiChiTiet = m.DiaChiChiTiet, HocPhiDuKien = m.HocPhiDuKien, DonViHocPhi = m.DonViHocPhi,
        TenNguoiHoc = m.TenNguoiHoc, TuoiNguoiHoc = m.TuoiNguoiHoc, BanNhap = m.BanNhap, LichRanh = [.. m.LichRanh]
    };
}
