using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.QuanTri.Controllers;

[Area("QuanTri")]
public sealed class HoSoController(HoSoGiaSuService service,AppDbContext db):BaseController
{
    public async Task<IActionResult> Index(){var rows=await service.ChoDuyetAsync();ViewBag.People=await db.Set<NguoiThamGia>().AsNoTracking().ToListAsync();return View(rows);}
    public async Task<IActionResult> ChiTiet(Guid id)
    {
        var model=await service.XemDuyetAsync(id);
        ViewBag.Id=id;
        ViewBag.Certificates=await db.Set<TepDinhKem>().AsNoTracking().Where(x=>x.HoSoGiaSuId==id).ToListAsync();
        ViewBag.Subjects=await db.Set<MonHoc>().AsNoTracking().ToListAsync();
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Duyet(Guid id,string rowVersion,bool approve,string? reason)
    {
        await service.DuyetAsync(id,rowVersion,approve,reason);Success();return RedirectToAction(nameof(Index));
    }
}

