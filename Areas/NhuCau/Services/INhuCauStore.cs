using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace EduConnect.Mvc.Areas.NhuCau.Services;

public interface INhuCauStore
{
    Task<IDbContextTransaction> BeginAsync(CancellationToken ct);
    Task LockOwnerAsync(Guid owner, CancellationToken ct);
    Task<LuotDangNhuCau?> FindOperationAsync(Guid owner, Guid operation, CancellationToken ct);
    Task<NhuCauHoc?> FindOwnedAsync(Guid id, Guid owner, bool tracking, CancellationToken ct);
    Task<int> CountNeedsAsync(Guid owner, string state, Guid? excluding, CancellationToken ct);
    Task<int> CountPostingsAsync(Guid owner, DateOnly date, CancellationToken ct);
    Task<int> CountActiveSubjectsAsync(List<int> ids, CancellationToken ct);
    Task<int> ResolveSubjectAsync(string? name, CancellationToken ct);
    Task<List<Guid>> FindDuplicateCandidatesAsync(NhuCauInput input, Guid owner, CancellationToken ct);
    Task<List<KhungGioRanh>> ReadSlotsAsync(Guid id, CancellationToken ct);
    Task<MonHoc> ReadSubjectAsync(int id, CancellationToken ct);
    Task<bool> IsCommonSubjectAsync(int id, CancellationToken ct);
    void AddNeed(NhuCauHoc need);
    void SetVersion(NhuCauHoc need, byte[] version);
    void AddPosting(LuotDangNhuCau posting);
    Task SaveAsync(CancellationToken ct);
    Task DeleteSlotsAsync(Guid id, CancellationToken ct);
    void AddSlot(KhungGioRanh slot);
    Task<List<NhuCauMonHoc>> ReadLinksAsync(Guid id, CancellationToken ct);
    void RemoveLinks(IEnumerable<NhuCauMonHoc> links);
    void AddLinks(IEnumerable<NhuCauMonHoc> links);
}
