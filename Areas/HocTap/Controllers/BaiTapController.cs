using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Areas.HocTap.Controllers;
[Area("HocTap")]
[RequestSizeLimit(52000000)]
public sealed class BaiTapController(BaiTapService service,LopHocService classes,TepService files):BaseController {
 public async Task<IActionResult> Giao(Guid id){ViewBag.Class=await classes.ChiTietAsync(id);return View(new GiaoBaiTapViewModel{LopHocId=id});}
 [HttpPost]public async Task<IActionResult> Giao(GiaoBaiTapViewModel m){if(ModelState.IsValid)try{await service.GiaoAsync(m);Success();return RedirectToAction("ChiTiet","LopHoc",new{id=m.LopHocId,tab="assignments"});}catch(LoiNghiepVu ex){ModelState.AddModelError("",ex.Message);}ViewBag.Class=await classes.ChiTietAsync(m.LopHocId);return View(m);}
 public async Task<IActionResult> ChiTiet(Guid id){var a=await service.ChiTietAsync(id);ViewBag.Class=await classes.ChiTietAsync(a.LopHocId);ViewBag.Submissions=await service.BaiNopAsync(id);ViewBag.Results=await service.KetQuaAsync(id);ViewBag.Files=await files.DanhSachAsync(id);return View(a);}
 [HttpPost]public async Task<IActionResult> Nop(NopBaiViewModel m){ValidateInput();await service.NopAsync(m);Success("Đã lưu bài nộp.");return RedirectToAction(nameof(ChiTiet),new{id=m.BaiTapGiaoId});}
 [HttpPost]public async Task<IActionResult> Cham(ChamBaiViewModel m){ValidateInput();await service.ChamAsync(m);Success();return RedirectToAction("Index","LopHoc");}
 public async Task<IActionResult> Tai(Guid id){var (f,path)=await files.TaiAsync(id);Response.Headers.XContentTypeOptions="nosniff";return PhysicalFile(path,f.LoaiNoiDung,f.TenGoc);}
}
