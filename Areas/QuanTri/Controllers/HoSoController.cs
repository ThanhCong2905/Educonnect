using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.QuanTri.Controllers;

[Area("QuanTri")]
[Microsoft.AspNetCore.Authorization.Authorize(Roles = "QuanTri")]
public sealed class HoSoController(HoSoGiaSuService service,AppDbContext db):BaseController
{
    public async Task<IActionResult> Index(int trang=1,string state="ChoDuyet",string? q=null)
    {
        var page=await service.LayDanhSachDuyetAsync(trang,state,q);
        var owners=page.Items.Select(x=>x.GiaSuId).ToList();
        ViewBag.People=await db.Users.Where(x=>owners.Contains(x.Id)).ToDictionaryAsync(x=>x.Id,x=>x.HoTen);
        return View(page);
    }
    public async Task<IActionResult> ChiTiet(Guid id)
    {
        var model=await service.XemDuyetAsync(id);
        ViewBag.Id=id;
        ViewBag.Certificates=await db.Set<TepDinhKem>().AsNoTracking().Where(x=>x.HoSoGiaSuId==id).ToListAsync();
        ViewBag.Subjects=await db.Set<MonHoc>().AsNoTracking().ToListAsync();
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Duyet(Guid id,string rowVersion,bool approve,string? reason,string? quyetDinh=null)
    {
        try
        {
            await service.DuyetAsync(id,rowVersion,approve,reason,quyetDinh=="TuChoi");
            Success();
            return RedirectToAction(nameof(Index));
        }
        catch(LoiNghiepVu ex)
        {
            ModelState.AddModelError(ex.Truong??"",ex.Message);
            ViewBag.Reason=reason;
            return await ChiTiet(id);
        }
    }
}

