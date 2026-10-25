using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Controllers;
public sealed class HomeController(TimKiemService search,NguoiDungMauService current,ThongBaoService notices,LopHocService classes,EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService connections):BaseController {
 public async Task<IActionResult> Index(){var (items,has)=await search.GoiYAsync();ViewBag.HasNeed=has;ViewBag.Classes=await classes.LayAsync();return View(items);}
 [HttpPost]public async Task<IActionResult> Chon(Guid id){await current.ChonAsync(id);return RedirectToAction(nameof(Index));}
 public async Task<IActionResult> ThongBao()=>View(await notices.LayAsync());
 public async Task<IActionResult> ChuaDoc()=>Json(new{count=await notices.ChuaDocAsync()});
 [HttpPost]public async Task<IActionResult> Doc(Guid id){var n=await notices.DocAsync(id);if(n.LoaiThongBao=="BaiTap")return Redirect($"/HocTap/BaiTap/ChiTiet/{n.DoiTuongId}");if(n.LoaiThongBao is "LichHoc" or "ThanhVien")return Redirect($"/HocTap/LopHoc/ChiTiet/{n.DoiTuongId}");var cs=await classes.LayAsync();if(cs.Any(x=>x.Id==n.DoiTuongId))return Redirect($"/HocTap/LopHoc/ChiTiet/{n.DoiTuongId}");var links=await connections.LayAsync();return Redirect("/KetNoi/KetNoi/"+(links.Contacts.Any(x=>x.Id==n.DoiTuongId)?"LienHe":"Index")+"#"+n.DoiTuongId);}
}
