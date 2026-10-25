using Microsoft.AspNetCore.Mvc;
namespace EduConnect.Mvc.Controllers;
public abstract class BaseController:Controller {
 protected void ValidateInput(){if(!ModelState.IsValid)throw new EduConnect.Mvc.Infrastructure.LoiNghiepVu(string.Join(" · ",ModelState.Values.SelectMany(x=>x.Errors).Select(x=>string.IsNullOrEmpty(x.ErrorMessage)?"Dữ liệu nhập chưa đúng định dạng.":x.ErrorMessage)));}
 protected void Success(string text="Đã lưu thay đổi.")=>TempData["Success"]=text;
}
