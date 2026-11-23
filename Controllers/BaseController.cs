using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Controllers;

public abstract class BaseController : Controller
{
    protected void ValidateInput()
    {
        if (!ModelState.IsValid)
            throw new EduConnect.Mvc.Infrastructure.LoiNghiepVu(string.Join(" · ", ModelState.Values.SelectMany(x => x.Errors).Select(x => string.IsNullOrEmpty(x.ErrorMessage) ? "Dữ liệu nhập chưa đúng định dạng." : x.ErrorMessage)));
    }
    protected void Success(string text = "Đã lưu thay đổi.") => TempData["Success"] = text;

    protected IActionResult TraTep(EduConnect.Mvc.Models.TepDinhKem file, string path, bool xem)
    {
        Response.Headers.XContentTypeOptions = "nosniff";
        Response.Headers.CacheControl = "private, no-store";
        if (xem && EduConnect.Mvc.Infrastructure.TepService.XemTrucTiepDuoc(file.LoaiNoiDung))
        {
            Response.Headers.ContentDisposition = "inline";
            return PhysicalFile(path, file.LoaiNoiDung, enableRangeProcessing: true);
        }
        return PhysicalFile(path, file.LoaiNoiDung, file.TenGoc);
    }
}
