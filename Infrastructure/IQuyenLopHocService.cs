using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
namespace EduConnect.Mvc.Infrastructure;
public interface IQuyenLopHocService
{
    Task<LopHoc> KiemTraQuyenXemAsync(Guid id);
    Task<LopHoc> KiemTraQuyenXemAsync(Guid id, Guid nguoiXem);
    Task<LopHoc> KiemTraQuyenGhiAsync(Guid id, bool tutor = false);
}
