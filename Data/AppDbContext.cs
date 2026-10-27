using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<MonHoc>(e =>
        {
            e.ToTable("MonHoc", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
            e.Property(x => x.TenMon).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.DangSuDung).HasColumnType("bit").IsRequired();
        });
        b.Entity<HoSoGiaSu>(e =>
        {
            e.ToTable("HoSoGiaSu", t => t.UseSqlOutputClause(false));
            e.Property(x => x.DangHienThi).HasColumnType("bit");
            e.Property(x => x.DayTrucTuyen).HasColumnType("bit");
            e.Property(x => x.DayTrucTiep).HasColumnType("bit");
            e.Property(x => x.GuiDuyetLuc).HasColumnType("datetimeoffset(0)");
            e.Property(x => x.DuyetLuc).HasColumnType("datetimeoffset(0)");
            e.Property(x => x.NguoiDuyetId).HasColumnType("uniqueidentifier");
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.GiaSuId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.TrinhDo).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.ChuyenMon).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.KinhNghiem).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.MoTa).HasColumnType("nvarchar(max)").IsRequired();
            e.Property(x => x.KhuVuc).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.TrangThaiDuyet).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.LyDoTuChoi).HasColumnType("nvarchar(2000)");
            e.Property(x => x.RowVersion).IsRowVersion();
        });
        b.Entity<HoSoGiaSuMonDay>(e =>
        {
            e.ToTable("HoSoGiaSuMonDay", t => t.UseSqlOutputClause(false));
            e.Property(x => x.HocPhiDeXuat).HasColumnType("decimal(18,2)");
            e.Property(x => x.DonViHocPhi).HasColumnType("varchar(16)");
            e.HasKey(x => new { x.HoSoGiaSuId, x.MonHocId });
            e.Property(x => x.HoSoGiaSuId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.MonHocId).HasColumnType("int").IsRequired();
            e.Property(x => x.PhamViDay).HasColumnType("nvarchar(256)").IsRequired();
        });
        b.Entity<NhuCauHoc>(e =>
        {
            e.ToTable("NhuCauHoc", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.ChuSoHuuId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.LoaiNhuCau).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.MonHocId).HasColumnType("int").IsRequired();
            e.Property(x => x.KhoiLop).HasColumnType("nvarchar(256)");
            e.Property(x => x.HocLuc).HasColumnType("nvarchar(256)");
            e.Property(x => x.MucTieu).HasColumnType("nvarchar(256)");
            e.Property(x => x.SoBuoiTuan).HasColumnType("int");
            e.Property(x => x.HinhThucHoc).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.KhuVuc).HasColumnType("nvarchar(256)");
            e.Property(x => x.DiaChiChiTiet).HasColumnType("nvarchar(256)");
            e.Property(x => x.HocPhiDuKien).HasColumnType("decimal(18,2)");
            e.Property(x => x.TenNguoiHoc).HasColumnType("nvarchar(256)");
            e.Property(x => x.TuoiNguoiHoc).HasColumnType("int");
            e.Property(x => x.TrangThai).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.RowVersion).IsRowVersion();
            e.Property(x => x.DonViHocPhi).HasColumnType("varchar(16)");
            e.Property(x => x.TaoLuc).HasColumnType("datetimeoffset(0)").IsRequired();
        });
        b.Entity<KhungGioRanh>(e =>
        {
            e.ToTable("KhungGioRanh", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.NhuCauId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.ThuTrongTuan).HasColumnType("tinyint").IsRequired();
            e.Property(x => x.GioBatDau).HasColumnType("time(0)").IsRequired();
            e.Property(x => x.GioKetThuc).HasColumnType("time(0)").IsRequired();
        });
        b.Entity<YeuCauKetNoiLopHoc>(e =>
        {
            e.ToTable("YeuCauKetNoiLopHoc", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.GiaSuId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.BenHocId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.NguoiGuiId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.NhuCauId).HasColumnType("uniqueidentifier");
            e.Property(x => x.LoiNhan).HasColumnType("nvarchar(256)");
            e.Property(x => x.TrangThai).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.GuiLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.HetHanLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.TenLopDuKien).HasColumnType("nvarchar(256)");
            e.Property(x => x.MonHocId).HasColumnType("int");
            e.Property(x => x.MucTieu).HasColumnType("nvarchar(256)");
            e.Property(x => x.SoBuoiTuan).HasColumnType("int");
            e.Property(x => x.ThoiLuongPhut).HasColumnType("int");
            e.Property(x => x.BatDauHocLuc).HasColumnType("datetimeoffset(0)");
            e.Property(x => x.HinhThucHoc).HasColumnType("varchar(32)");
            e.Property(x => x.DiaDiem).HasColumnType("nvarchar(256)");
            e.Property(x => x.HocPhi).HasColumnType("decimal(18,2)");
            e.Property(x => x.RowVersion).IsRowVersion();
            e.Property(x => x.DonViHocPhi).HasColumnType("varchar(16)");
            e.Property(x => x.LienHeId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.NguoiDeXuatId).HasColumnType("uniqueidentifier");
            e.Property(x => x.DeXuatLuc).HasColumnType("datetimeoffset(0)");
        });
        b.Entity<YeuCauHuyKetNoi>(e =>
        {
            e.ToTable("YeuCauHuyKetNoi", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.LopHocId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.NguoiGuiId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.LyDo).HasColumnType("nvarchar(2000)").IsRequired();
            e.Property(x => x.TrangThai).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.GuiLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.HetHanLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.PhanHoiLuc).HasColumnType("datetimeoffset(0)");
            e.Property(x => x.RowVersion).IsRowVersion();
        });
        b.Entity<CuocHoiThoai>(e =>
        {
            e.ToTable("CuocHoiThoai", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.LoaiHoiThoai).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.GiaSuId).HasColumnType("uniqueidentifier");
            e.Property(x => x.BenHocId).HasColumnType("uniqueidentifier");
            e.Property(x => x.LopHocId).HasColumnType("uniqueidentifier");
            e.Property(x => x.HoiThoaiTruocKetNoiId).HasColumnType("uniqueidentifier");
            e.Property(x => x.TaoLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.LienHeId).HasColumnType("uniqueidentifier");
        });
        b.Entity<TinNhan>(e =>
        {
            e.ToTable("TinNhan", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.CuocHoiThoaiId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.NguoiGuiId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.NoiDung).HasColumnType("nvarchar(max)").IsRequired();
            e.Property(x => x.GuiLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.MaThaoTac).HasColumnType("uniqueidentifier").IsRequired();
        });
        b.Entity<LopHoc>(e =>
        {
            e.ToTable("LopHoc", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.YeuCauKetNoiId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.GiaSuId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.BenHocId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.TenLop).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.MonHocId).HasColumnType("int").IsRequired();
            e.Property(x => x.MucTieu).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.SoBuoiTuan).HasColumnType("int").IsRequired();
            e.Property(x => x.ThoiLuongPhut).HasColumnType("int").IsRequired();
            e.Property(x => x.HinhThucHoc).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.DiaDiem).HasColumnType("nvarchar(256)");
            e.Property(x => x.HocPhi).HasColumnType("decimal(18,2)").IsRequired();
            e.Property(x => x.TrangThai).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.GiaSuXacNhanKetThucLuc).HasColumnType("datetimeoffset(0)");
            e.Property(x => x.BenHocXacNhanKetThucLuc).HasColumnType("datetimeoffset(0)");
            e.Property(x => x.RowVersion).IsRowVersion();
            e.Property(x => x.DonViHocPhi).HasColumnType("varchar(16)").IsRequired();
            e.Property(x => x.TenNguoiHoc).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.TuoiNguoiHoc).HasColumnType("int");
        });
        b.Entity<ThanhVienLopHoc>(e =>
        {
            e.ToTable("ThanhVienLopHoc", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.LopHocId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.TaiKhoanId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.NguoiDeNghiId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.TrangThaiDuyet).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.MucQuyen).HasColumnType("varchar(32)");
            e.Property(x => x.DeNghiLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.DuyetLuc).HasColumnType("datetimeoffset(0)");
            e.Property(x => x.RoiLopLuc).HasColumnType("datetimeoffset(0)");
            e.Property(x => x.RowVersion).IsRowVersion();
        });
        b.Entity<LichHoc>(e =>
        {
            e.ToTable("LichHoc", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.LopHocId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.TieuDe).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.BatDauLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.KetThucLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.LaBuoiBu).HasColumnType("bit").IsRequired();
            e.Property(x => x.TrangThai).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.RowVersion).IsRowVersion();
            e.Property(x => x.LichGocId).HasColumnType("uniqueidentifier");
        });
        b.Entity<BuoiHoc>(e =>
        {
            e.ToTable("BuoiHoc", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.LichHocId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.TieuDe).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.NoiDungGiangDay).HasColumnType("nvarchar(max)");
            e.Property(x => x.NhanXet).HasColumnType("nvarchar(2000)").IsRequired();
            e.Property(x => x.TrangThaiThamDu).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.GhiNhanLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.NguoiGhiNhanId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.RowVersion).IsRowVersion();
            e.Property(x => x.TrangThaiThamDuGiaSu).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.TrangThaiThamDuBenHoc).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.KetQuaBuoiHoc).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.CapNhatLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.PhienBan).HasColumnType("int").IsRequired();
        });
        b.Entity<BaiTapGiao>(e =>
        {
            e.ToTable("BaiTapGiao", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.LopHocId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.BuoiHocId).HasColumnType("uniqueidentifier");
            e.Property(x => x.NguoiGiaoId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.TieuDe).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.NoiDung).HasColumnType("nvarchar(max)").IsRequired();
            e.Property(x => x.HanNopLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.TieuChiCham).HasColumnType("nvarchar(max)");
            e.Property(x => x.RowVersion).IsRowVersion();
        });
        b.Entity<BaiTapNop>(e =>
        {
            e.ToTable("BaiTapNop", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.BaiTapGiaoId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.NguoiNopId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.NoiDung).HasColumnType("nvarchar(max)");
            e.Property(x => x.NopLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.MaThaoTac).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.RowVersion).IsRowVersion();
            e.Property(x => x.LanNop).HasColumnType("int").IsRequired();
        });
        b.Entity<KetQuaChamBai>(e =>
        {
            e.ToTable("KetQuaChamBai", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.BaiTapNopId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.NguoiChamId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.Diem).HasColumnType("decimal(4,2)");
            e.Property(x => x.NhanXet).HasColumnType("nvarchar(2000)").IsRequired();
            e.Property(x => x.ChamLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.RowVersion).IsRowVersion();
        });
        b.Entity<GiaSu>(e =>
        {
            e.ToTable("GiaSu", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.TaiKhoanId });
            e.Property(x => x.TaiKhoanId).HasColumnType("uniqueidentifier").IsRequired();
        });
        b.Entity<TepDinhKem>(e =>
        {
            e.ToTable("TepDinhKem", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.NguoiTaiLenId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.TenGoc).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.TenLuuTru).HasColumnType("nvarchar(512)").IsRequired();
            e.Property(x => x.LoaiNoiDung).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.KichThuoc).HasColumnType("bigint").IsRequired();
            e.Property(x => x.Checksum).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.TrangThaiQuet).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.TaoLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.HoSoGiaSuId).HasColumnType("uniqueidentifier");
            e.Property(x => x.BaiTapGiaoId).HasColumnType("uniqueidentifier");
            e.Property(x => x.BaiTapNopId).HasColumnType("uniqueidentifier");
            e.Property(x => x.TinNhanId).HasColumnType("uniqueidentifier");
        });
        b.Entity<ThongBao>(e =>
        {
            e.ToTable("ThongBao", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.NguoiNhanId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.LoaiThongBao).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.DoiTuongId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.NoiDungTomTat).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.TaoLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.DaDocLuc).HasColumnType("datetimeoffset(0)");
            e.Property(x => x.MaSuKien).HasColumnType("uniqueidentifier").IsRequired();
        });
        b.Entity<SuKienChoXuLy>(e =>
        {
            e.ToTable("SuKienChoXuLy", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.LoaiSuKien).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.DuLieuToiThieu).HasColumnType("nvarchar(max)").IsRequired();
            e.Property(x => x.TaoLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.TrangThai).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.SoLanThu).HasColumnType("int").IsRequired();
            e.Property(x => x.ThuLaiLuc).HasColumnType("datetimeoffset(0)");
            e.Property(x => x.XuLyXongLuc).HasColumnType("datetimeoffset(0)");
            e.Property(x => x.RowVersion).IsRowVersion();
            e.Property(x => x.LeaseId).HasColumnType("uniqueidentifier");
            e.Property(x => x.LeaseHetHanLuc).HasColumnType("datetimeoffset(0)");
        });
        b.Entity<YeuCauLienHe>(e =>
        {
            e.ToTable("YeuCauLienHe", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.GiaSuId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.BenHocId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.NguoiGuiId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.LoiNhan).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.TrangThai).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.GuiLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.NguoiPhanHoiId).HasColumnType("uniqueidentifier");
            e.Property(x => x.PhanHoiLuc).HasColumnType("datetimeoffset(0)");
            e.Property(x => x.RowVersion).IsRowVersion();
        });
        b.Entity<KhungGioDeXuat>(e =>
        {
            e.ToTable("KhungGioDeXuat", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.YeuCauKetNoiId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.ThuTrongTuan).HasColumnType("tinyint").IsRequired();
            e.Property(x => x.GioBatDau).HasColumnType("time(0)").IsRequired();
            e.Property(x => x.GioKetThuc).HasColumnType("time(0)").IsRequired();
        });
        b.Entity<LichSuGhiNhanBuoiHoc>(e =>
        {
            e.ToTable("LichSuGhiNhanBuoiHoc", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.BuoiHocId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.PhienBan).HasColumnType("int").IsRequired();
            e.Property(x => x.TieuDe).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.KetQuaBuoiHoc).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.NoiDungGiangDay).HasColumnType("nvarchar(max)");
            e.Property(x => x.NhanXet).HasColumnType("nvarchar(2000)").IsRequired();
            e.Property(x => x.TrangThaiThamDuGiaSu).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.TrangThaiThamDuBenHoc).HasColumnType("varchar(32)").IsRequired();
            e.Property(x => x.NguoiGhiNhanId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.GhiNhanLuc).HasColumnType("datetimeoffset(0)").IsRequired();
            e.Property(x => x.NguoiSuaId).HasColumnType("uniqueidentifier").IsRequired();
            e.Property(x => x.ThayDoiLuc).HasColumnType("datetimeoffset(0)").IsRequired();
        });
        b.Entity<NguoiThamGia>(e =>
        {
            e.ToTable("NguoiThamGia", t => t.UseSqlOutputClause(false));
            e.HasKey(x => new { x.Id });
            e.Property(x => x.Id).HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
            e.Property(x => x.HoTen).HasColumnType("nvarchar(256)").IsRequired();
            e.Property(x => x.LoaiNguoi).HasColumnType("varchar(16)").IsRequired();
        });
    }
}
