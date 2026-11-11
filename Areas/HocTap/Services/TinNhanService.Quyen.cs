using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.HocTap.Services;

public sealed partial class TinNhanService
{
    private async Task<DateTimeOffset> BatDauXemAsync(Guid room)
    {
        var conversation = await KiemTraAsync(room);
        var me = await current.HienTaiAsync();
        if (conversation.LoaiHoiThoai != "TheoLop") return DateTimeOffset.MinValue;
        var lop = await rights.KiemTraQuyenXemAsync(conversation.LopHocId!.Value);
        if (me.Id == lop.GiaSuId || me.Id == lop.BenHocId) return DateTimeOffset.MinValue;
        return await db.Set<ThanhVienLopHoc>().Where(x => x.LopHocId == lop.Id && x.TaiKhoanId == me.Id && x.TrangThaiDuyet == "DaDuyet" && x.RoiLopLuc == null)
            .Select(x => x.DuyetLuc).SingleAsync() ?? DateTimeOffset.MaxValue;
    }
}
