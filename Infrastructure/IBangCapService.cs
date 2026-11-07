using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Infrastructure;

public interface IBangCapService
{
    Task<List<TepDinhKem>> CuaToiAsync();
    Task TaiLenAsync(List<IFormFile> uploads);
    Task<(TepDinhKem File, string Path)> TaiAsync(Guid id);
    Task XoaAsync(Guid id);
}
