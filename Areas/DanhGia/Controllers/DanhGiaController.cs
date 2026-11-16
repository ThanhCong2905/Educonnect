using EduConnect.Mvc.Areas.DanhGia.Services;
using EduConnect.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect.Mvc.Areas.DanhGia.Controllers;

[Area("DanhGia")]
public sealed class DanhGiaController(DanhGiaService service, TienDoService progress) : BaseController
{
    public async Task<IActionResult> DanhSach(Guid maLop, DateTimeOffset? tuNgay = null, DateTimeOffset? denNgay = null, int trang = 1)
    {
        await progress.LayAsync(maLop, tuNgay, denNgay, trang);
        return RedirectToAction("ChiTiet", "LopHoc", new { area = "HocTap", id = maLop, tuNgay, denNgay, trang });
    }

    public async Task<IActionResult> LichSu(Guid id) => View(await service.LichSuAsync(id));
}
