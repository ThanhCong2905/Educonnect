using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Data;

internal static class NhatKyAnToanMapping
{
    public static void Configure(ModelBuilder builder)
    {
        builder.Entity<NhatKyAnToan>(e =>
        {
            e.ToTable("NhatKyAnToan");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Nhom).HasMaxLength(32).IsUnicode(false);
            e.Property(x => x.SuKien).HasMaxLength(160);
            e.Property(x => x.KetQua).HasMaxLength(16).IsUnicode(false);
            e.Property(x => x.DinhDanhBam).HasMaxLength(64).IsUnicode(false);
            e.Property(x => x.DiaChiIp).HasMaxLength(45).IsUnicode(false);
            e.Property(x => x.DuongDan).HasMaxLength(512);
            e.Property(x => x.PhuongThuc).HasMaxLength(16).IsUnicode(false);
            e.Property(x => x.MaYeuCau).HasMaxLength(128);
            e.Property(x => x.LyDo).HasMaxLength(100);
            e.Property(x => x.DoiTuongId).HasMaxLength(64).IsUnicode(false);
            e.HasOne<TaiKhoan>().WithMany().HasForeignKey(x => x.TaiKhoanId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne<TaiKhoan>().WithMany().HasForeignKey(x => x.TaiKhoanDichId).OnDelete(DeleteBehavior.NoAction);
            e.HasIndex(x => new { x.ThoiDiem, x.Id });
            e.HasIndex(x => new { x.TaiKhoanId, x.ThoiDiem });
            e.HasIndex(x => new { x.Nhom, x.ThoiDiem });
        });
    }
}
