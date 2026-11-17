using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
namespace EduConnect.Mvc.Infrastructure;
public interface IThongBaoService
{
    Task TaoChoLopAsync(Guid lopHocId, string type, Guid target, string text, Guid? loaiTru = null);
    void Tao(Guid recipient, string type, Guid target, string text);
    Task<List<ThongBao>> LayAsync();
    Task<int> ChuaDocAsync();
    Task<Page<ThongBao>> PhanTrangAsync(int trang=1,string? state=null,string? q=null,string? loai=null);
    Task DocTatCaAsync();
    Task<ThongBao> DocAsync(Guid id);
}
