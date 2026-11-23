using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.Areas.QuanTri.ViewModels;
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
        ViewBag.Certificates=await db.Set<TepDinhKem>().AsNoTracking().Where(x=>x.HoSoGiaSuId==id).OrderBy(x=>x.TaoLuc).ToListAsync();
        ViewBag.Subjects=await db.Set<MonHoc>().AsNoTracking().ToDictionaryAsync(x=>x.Id, x=>x.TenMon);
        return View(nameof(ChiTiet), model);
    }
    [HttpPost]
    public async Task<IActionResult> Duyet(Guid id, string rowVersion, bool approve, string? reason,
        string? quyetDinh = null, [FromForm(Name = "gopY")] Dictionary<string, string>? gopY = null)
    {
        try
        {
            var reviewReason = reason;
            var tuChoi = quyetDinh == "TuChoi";
            if (!approve)
            {
                Rules.Check(!tuChoi || !string.IsNullOrWhiteSpace(reason), "Nhập lý do từ chối.", field: nameof(reason));
                var notes = gopY ?? [];
                foreach (var (key, content) in notes)
                {
                    Rules.Check(HoSoDuyetFields.Labels.TryGetValue(key, out var label), "Mục góp ý không hợp lệ.");
                    Rules.Check(!string.IsNullOrWhiteSpace(content) && content.Length <= 300,
                        $"Nhập góp ý cho mục {label}, tối đa 300 ký tự.", field: $"gopY[{key}]");
                }
                var lines = HoSoDuyetFields.Labels.Where(item => notes.ContainsKey(item.Key))
                    .Select(item => $"{item.Value}: {notes[item.Key].Trim()}").ToList();
                if (!string.IsNullOrWhiteSpace(reason)) lines.Insert(0, reason.Trim());
                Rules.Check(lines.Count > 0, "Chọn ít nhất một mục và ghi nội dung cần sửa.");
                reviewReason = string.Join("\n\n", lines);
            }
            await service.DuyetAsync(id, rowVersion, approve, reviewReason, tuChoi);
            Success();
            return RedirectToAction(nameof(Index));
        }
        catch(LoiNghiepVu ex)
        {
            ModelState.AddModelError(ex.Truong??"",ex.Message);
            ViewBag.Reason=reason;
            ViewBag.GopY=gopY;
            return await ChiTiet(id);
        }
    }
}

