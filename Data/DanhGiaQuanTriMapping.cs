using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Data;

internal static class DanhGiaQuanTriMapping
{
    public static void Configure(ModelBuilder b)
    {
        b.Entity<MonHoc>().Property(x => x.RowVersion).IsRowVersion();
        Map<DotPhanHoi>(b);
        Map<PhieuDanhGia>(b);
        Map<LichSuPhanHoi>(b, version: false);
        Map<QuyetDinhSauPhanHoi>(b);
        Map<DanhGiaDoiTac>(b);
        Map<LichSuDanhGiaDoiTac>(b, version: false);
        Map<BaoCaoViPham>(b);
        Map<LichSuXuLyBaoCao>(b, version: false);
        Map<BienPhapQuanTri>(b);
        b.Entity<BienPhapQuanTri>().HasOne<BienPhapQuanTri>().WithMany().HasForeignKey(x => x.NguonBienPhapId).OnDelete(DeleteBehavior.Restrict);
        Map<NhatKyQuanTri>(b, version: false);
        b.Entity<DotPhanHoi>().HasIndex(x => new { x.LopHocId, x.TuNgay, x.DenNgay }).IsUnique();
        b.Entity<PhieuDanhGia>().HasIndex(x => new { x.DotPhanHoiId, x.NguoiGuiId }).IsUnique().HasFilter("[DotPhanHoiId] IS NOT NULL");
        b.Entity<QuyetDinhSauPhanHoi>().HasIndex(x => new { x.DotPhanHoiId, x.NguoiQuyetDinhId }).IsUnique();
        b.Entity<DanhGiaDoiTac>().HasIndex(x => new { x.LopHocId, x.NguoiGuiId, x.NguoiNhanId }).IsUnique();
        b.Entity<BaoCaoViPham>().HasIndex(x => new { x.NguoiBaoCaoId, x.MaThaoTac }).IsUnique();
        b.Entity<PhieuDanhGia>().Property(x => x.QuyetDinhDeXuat).IsUnicode(false);
        b.Entity<BaoCaoViPham>().Property(x => x.KetQua).IsUnicode(false);
        b.Entity<BaoCaoViPham>().Property(x => x.TrangThai).IsUnicode(false);
        b.Entity<BaoCaoViPham>().Property(x => x.LoaiDoiTuong).IsUnicode(false);
    }

    private static void Map<T>(ModelBuilder b, bool version = true) where T : class
    {
        var entity = b.Entity<T>();
        entity.ToTable(typeof(T).Name);
        entity.HasKey("Id");
        entity.Property<Guid>("Id").ValueGeneratedNever();
        if (version)
            entity.Property<byte[]>("RowVersion").IsRowVersion();
    }
}
