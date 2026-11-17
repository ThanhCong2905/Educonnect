using EduConnect.Mvc.Models;
namespace EduConnect.Mvc.Infrastructure;
public interface IThongBaoData
{
    Task<LopHoc> ReadClassAsync(Guid id);
    Task<List<Guid>> ReadEligibleMemberIdsAsync(Guid lop);
    void AddEvent(SuKienChoXuLy ev);
    void AddNotice(ThongBao notice);
}
