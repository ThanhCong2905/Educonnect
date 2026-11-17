using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
namespace EduConnect.Mvc.Infrastructure;
public interface ITepService
{
    bool ChoPhepTepChuaQuet => false;
    string? DuongDanAnhDaiDien(string? key);
    Task<string> LuuAnhDaiDienAsync(IFormFile file);
    void XoaAnhDaiDien(string? key);
    void DonDepDaXoa(IEnumerable<string> keys);
    Task<List<(string Name, string Mime, byte[] Bytes)>> KiemTraAsync(List<IFormFile> files);
    Task<List<string>> LuuAsync(List<(string Name, string Mime, byte[] Bytes)> files, Guid? giao = null, Guid? nop = null, Guid? hoSo = null, Guid? tinNhan = null, Guid? baoCao = null);
    Task<List<TepDinhKem>> DanhSachAsync(Guid giao);
    Task<(TepDinhKem File, string Path)> TaiAsync(Guid id);
    string DuongDan(TepDinhKem file);
}
