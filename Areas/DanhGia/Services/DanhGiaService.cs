using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.DanhGia.Services;

public sealed class DanhGiaService(AppDbContext db, NguoiDungHienTaiService current,
    QuyenLopHocService rights)
{
    public Task<KetQua<IReadOnlyList<PhieuDanhGia>>> LayTheoLopAsync(Guid maLop, Guid maNguoiThucHien, CancellationToken huy)
        => KetQua<IReadOnlyList<PhieuDanhGia>>.ThucHienAsync(async () =>
        {
            await current.KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
            await rights.KiemTraQuyenXemAsync(maLop);
            return await db.Set<PhieuDanhGia>().AsNoTracking().Where(x => x.LopHocId == maLop).OrderByDescending(x => x.GuiLuc).ToListAsync(huy);
        });

    public async Task<List<LichSuPhanHoi>> LichSuAsync(Guid id)
    {
        var phieu = await db.Set<PhieuDanhGia>().SingleOrDefaultAsync(x => x.Id == id) ?? throw new LoiNghiepVu("Không tìm thấy phiếu.", 404);
        await rights.KiemTraQuyenXemAsync(phieu.LopHocId);
        return await db.Set<LichSuPhanHoi>().AsNoTracking().Where(x => x.PhieuDanhGiaId == id).OrderByDescending(x => x.ThayDoiLuc).ToListAsync();
    }

}
