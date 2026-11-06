using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.KetNoi.Services;

public sealed partial class KetNoiService
{
    public Task<KetQua<Guid>> GuiYeuCauAsync(GuiYeuCauViewModel duLieu, Guid maNguoiThucHien, CancellationToken huy) =>
        KetQua<Guid>.ThucHienAsync(async () =>
        {
            await KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
            if (duLieu.MaDoiTuong != Guid.Empty)
                duLieu.LienHeId = await db.Set<YeuCauLienHe>().Where(x => x.GiaSuId == duLieu.MaDoiTuong && x.BenHocId == maNguoiThucHien && x.TrangThai == "ChapNhan")
                    .Select(x => x.Id).SingleOrDefaultAsync(huy);
            return await GuiYeuCauAsync((GuiYeuCauInput)duLieu, huy);
        });

    public Task<KetQua<Guid>> GuiDeXuatAsync(DeXuatLopViewModel duLieu, Guid maNguoiThucHien, CancellationToken huy) =>
        KetQua<Guid>.ThucHienAsync(async () =>
        {
            await KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
            return await GuiDeXuatAsync(duLieu, huy);
        });

    public Task<KetQua<Guid>> ChapNhanDeXuatAsync(Guid maYeuCau, byte[] rowVersion, Guid maNguoiThucHien, CancellationToken huy) =>
        KetQua<Guid>.ThucHienAsync(async () =>
        {
            await KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
            Rules.Check(rowVersion is { Length: 8 }, "Phiên bản không hợp lệ.");
            return await ChapNhanDeXuatAsync(maYeuCau, Convert.ToBase64String(rowVersion), huy);
        });

    public Task<KetQua> TuChoiDeXuatAsync(Guid maYeuCau, string lyDo, byte[] rowVersion, Guid maNguoiThucHien, CancellationToken huy) =>
        KetQua.ThucHienAsync(async () =>
        {
            await KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
            Rules.Check((lyDo?.Length ?? 0) <= 2000, "Lý do tối đa 2000 ký tự.");
            Rules.Check(rowVersion is { Length: 8 }, "Phiên bản không hợp lệ.");
            await PhanHoiAsync(maYeuCau, "TuChoi", Convert.ToBase64String(rowVersion), huy);
        });

    public Task<KetQua> RutYeuCauAsync(Guid maYeuCau, byte[] rowVersion, Guid maNguoiThucHien, CancellationToken huy) =>
        KetQua.ThucHienAsync(async () =>
        {
            await KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
            Rules.Check(rowVersion is { Length: 8 }, "Phiên bản không hợp lệ.");
            await PhanHoiAsync(maYeuCau, "DaRut", Convert.ToBase64String(rowVersion), huy);
        });

    private Task<KetQua<YeuCauLienHe>> AcceptedAsync(Guid id, bool tutor, Guid maNguoiThucHien, CancellationToken huy) =>
        KetQua<YeuCauLienHe>.ThucHienAsync(async () =>
        {
            await KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
            return await AcceptedAsync(id, tutor);
        });
}
