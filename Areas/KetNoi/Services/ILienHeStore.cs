using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore.Storage;
namespace EduConnect.Mvc.Areas.KetNoi.Services;

public interface ILienHeStore
{
    Task<NguoiThamGia?> ReadRecipientAsync(Guid id);
    Task<IDbContextTransaction> BeginAsync();
    Task LockProfileAsync(Guid tutor);
    Task<bool> IsHiddenAsync(Guid tutor);
    Task<bool> IsEligibleAsync(Guid tutor);
    Task<bool> ExistsAsync(Guid tutor, Guid learner);
    void AddContact(YeuCauLienHe contact);
    void Notify(Guid recipient, string type, Guid target, string text);
    Task SaveAsync();
}
