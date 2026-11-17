using System.Linq.Expressions;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore.Storage;
namespace EduConnect.Mvc.Areas.KetNoi.Services;

public interface IKetNoiStore
{
    Task<T?> SingleAsync<T>(Expression<Func<T,bool>> predicate,CancellationToken ct=default,bool tracking=true) where T:class;
    Task<T> RequiredAsync<T>(Expression<Func<T,bool>> predicate,CancellationToken ct=default) where T:class;
    Task<bool> AnyAsync<T>(Expression<Func<T,bool>> predicate,CancellationToken ct=default) where T:class;
    Task<Guid> FindAcceptedContactIdAsync(Guid tutor,Guid learner,CancellationToken ct);
    Task<Guid> LatestRequestIdAsync(Guid contact,string state,CancellationToken ct);
    Task<Guid> ClassIdForRequestAsync(Guid request,CancellationToken ct=default);
    Task LockContactAsync(Guid contact,CancellationToken ct);
    Task<bool> HasActiveClassAsync(Guid contact,CancellationToken ct);
    Task<IDbContextTransaction> BeginAsync(CancellationToken ct);
    Task ExecuteAsync(FormattableString command,CancellationToken ct);
    void SetVersion(object entity,string version);
    Task SaveAsync(CancellationToken ct);
}
