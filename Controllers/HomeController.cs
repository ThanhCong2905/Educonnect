using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Controllers;

public sealed class HomeController(TimKiemService search, ThongBaoService notices, LopHocService classes, EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService connections) : BaseController
{
    public async Task<IActionResult> Index()
    {
        if(User.IsInRole("QuanTri")) return Redirect("/QuanTri/QuanTri/Index");
        var (items, has) = await search.GoiYAsync();
        ViewBag.HasNeed = has;
        ViewBag.Classes = await classes.LayAsync();
        return View(items);
    }
    public async Task<IActionResult> ThongBao(int trang=1,string? state=null,string? q=null,string? loai=null)
    {
        ViewBag.Unread=await notices.ChuaDocAsync();
        return View(await notices.PhanTrangAsync(trang,state,q,loai));
    }
    [HttpPost]
    public async Task<IActionResult> DocTatCa()
    {
        await notices.DocTatCaAsync();
        Success("Đã đánh dấu tất cả thông báo là đã đọc.");
        return RedirectToAction(nameof(ThongBao));
    }
    public async Task<IActionResult> ChuaDoc() => Json(new { count = await notices.ChuaDocAsync() });
    [HttpPost]
    public async Task<IActionResult> Doc(Guid id)
    {
        var n = await notices.DocAsync(id);
        if(n.LoaiThongBao=="NhacViec") return Redirect($"/HocTap/LichHoc/Buoi/{n.DoiTuongId}");
        if(n.LoaiThongBao=="PhanHoi") return Redirect($"/DanhGia/DanhGia/DanhSach?maLop={n.DoiTuongId}");
        if(n.LoaiThongBao=="BaoCao") return Redirect($"/QuanTri/BaoCaoViPham/ChiTiet?maBaoCao={n.DoiTuongId}");
        if(n.LoaiThongBao=="KiemDuyet") return Redirect($"/QuanTri/BaoCaoViPham/KetQua?id={n.DoiTuongId}");
        if(n.LoaiThongBao=="TaiKhoan") return Redirect("/TaiKhoan/HoSo");
        if(n.LoaiThongBao=="DanhGiaDoiTac") return Redirect($"/DanhGia/DoiTac/Xem/{n.DoiTuongId}");
        if (n.LoaiThongBao == "LienKetTaiKhoan") return Redirect("/HocTap/LienKetTaiKhoan/Index");
        if (n.LoaiThongBao == "NhuCau") return Redirect("/NhuCau/NhuCau/Index");
        if (n.LoaiThongBao == "HoSo") return Redirect("/NhuCau/HoSoGiaSu/Sua");
        if (n.LoaiThongBao == "BaiTap")
            return Redirect($"/HocTap/BaiTap/ChiTiet/{n.DoiTuongId}");
        if (n.LoaiThongBao == "LoiMoiLop")
            return Redirect($"/HocTap/LopHoc/LoiMoi/{n.DoiTuongId}");
        if (n.LoaiThongBao == "ThanhVien")
            return Redirect($"/HocTap/LopHoc/ThanhVien/{n.DoiTuongId}");
        if (n.LoaiThongBao == "LichHoc")
            return Redirect($"/HocTap/LopHoc/ChiTiet/{n.DoiTuongId}");
        var cs = await classes.LayAsync();
        if (cs.Any(x => x.Id == n.DoiTuongId))
            return Redirect($"/HocTap/LopHoc/ChiTiet/{n.DoiTuongId}");
        var links = await connections.LayAsync();
        return Redirect("/KetNoi/KetNoi/" + (links.Contacts.Any(x => x.Id == n.DoiTuongId) ? "LienHe" : "Index") + "#" + n.DoiTuongId);
    }
}
