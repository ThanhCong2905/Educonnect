using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore.Storage;
namespace EduConnect.Mvc.Infrastructure;

public interface IClassEndingStore
{
    Task<IDbContextTransaction> BeginAsync(CancellationToken ct);
    Task<LopHoc?> LockClassAsync(Guid id,CancellationToken ct);
    Task<YeuCauHuyKetNoi> LockRequestAsync(Guid id,CancellationToken ct);
    Task<Guid?> RequestClassIdAsync(Guid id,CancellationToken ct);
    Task<bool> HasPendingAsync(Guid id,CancellationToken ct);
    Task<DateTimeOffset> NowAsync(CancellationToken ct);
    Task<List<(Guid Id,Guid Lop)>> ExpiredAsync(DateTimeOffset now,CancellationToken ct);
    void Add(YeuCauHuyKetNoi request);
    void SetVersion(YeuCauHuyKetNoi request,byte[] version);
    Task SaveAsync(CancellationToken ct);
}
