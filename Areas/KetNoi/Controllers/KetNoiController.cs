using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Areas.KetNoi.Controllers;
[Area("KetNoi")]
public sealed class KetNoiController(KetNoiService service,NhuCauService needs):BaseController {
 public async Task<IActionResult> Index(){var (cs,rs)=await service.LayAsync();ViewBag.Contacts=cs;ViewBag.Requests=rs;ViewBag.Needs=await needs.LayAsync();return View();}
 public async Task<IActionResult> LienHe(){var (cs,rs)=await service.LayAsync();ViewBag.Chats=await service.HoiThoaiLienHeAsync();ViewBag.Subjects=await needs.MonTenAsync();ViewBag.Contacts=cs;ViewBag.Requests=rs;ViewBag.Needs=await needs.LayAsync();return View();}
 [HttpPost]public async Task<IActionResult> GuiLienHe(GuiLienHeInput m){ValidateInput();await service.GuiLienHeAsync(m);Success("Đã gửi yêu cầu liên hệ.");return RedirectToAction(nameof(LienHe));}
 [HttpPost]public async Task<IActionResult> PhanHoiLienHe(Guid id,string action,string rowVersion){await service.PhanHoiLienHeAsync(id,action,rowVersion);Success();return RedirectToAction(nameof(LienHe));}
 [HttpPost]public async Task<IActionResult> GuiYeuCau(GuiYeuCauInput m){ValidateInput();await service.GuiYeuCauAsync(m);Success();return RedirectToAction(nameof(Index));}
 private async Task ProposalData(Guid contact){ViewBag.Subjects=await needs.MonDuocChonAsync();var all=await service.LayAsync();var c=all.Contacts.Single(x=>x.Id==contact);ViewBag.Learner=c.BenHocId;}
 public async Task<IActionResult> DeXuat(Guid contact,Guid? id){var m=await service.LayDeXuatAsync(contact,id);await ProposalData(contact);return View(m);}
 [HttpPost]public async Task<IActionResult> DeXuat(DeXuatLopViewModel m){if(Request.HasFormContentType&&!Request.Form.Keys.Any(key=>key.StartsWith("LichRanh[")))m.LichRanh=[];if(ModelState.IsValid)try{await service.GuiDeXuatAsync(m);Success("Đã gửi đề xuất để bên học xác nhận.");return RedirectToAction(nameof(Index));}catch(LoiNghiepVu ex){ModelState.AddModelError("",ex.Message);}await ProposalData(m.LienHeId);return View(m);}
 public async Task<IActionResult> ChiTiet(Guid id){var r=await service.ChiTietAsync(id);ViewBag.Slots=await service.KhungAsync(id);return View(r);}
 [HttpPost]public async Task<IActionResult> ChapNhan(Guid id,string rowVersion){var c=await service.ChapNhanAsync(id,rowVersion);Success("Lớp đã được tạo. Bạn có thể tiếp tục mở hoặc đóng nhu cầu tại mục Nhu cầu.");return Redirect($"/HocTap/LopHoc/ChiTiet/{c}");}
 [HttpPost]public async Task<IActionResult> PhanHoi(Guid id,string action,string rowVersion){await service.PhanHoiAsync(id,action,rowVersion);Success();return RedirectToAction(nameof(Index));}
}

