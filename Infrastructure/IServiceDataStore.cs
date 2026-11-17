using System.Data;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace EduConnect.Mvc.Infrastructure;

// Replaces persistence, database locks and the server clock in isolated service tests.
// Business conditions remain in the service. The production adapter uses the same EF operations.
public interface IServiceDataStore
{
    IQueryable<T> Query<T>() where T : class;
    Task<IDbContextTransaction> BeginAsync(IsolationLevel? isolation = null, CancellationToken token = default);
    void Add(object entity);
    void RemoveRange(IEnumerable<object> entities);
    Task<int> SaveAsync(CancellationToken token = default);
    void SetVersion(object entity, string? version);
    void MarkModified(object entity, string property);
    Task<DateTimeOffset> NowAsync(CancellationToken token = default);
    Task<LopHoc> LockClassAsync(Guid id, CancellationToken token = default);
    Task LockProfileAsync(Guid id);
    Task LockAdminAsync(Guid? actor = null, string? permission = null);
    Task CheckAccountAsync(Guid actor);
    Task CheckAdminAsync(Guid actor, string permission);
    Task<int> ExecuteAsync(FormattableString command, CancellationToken token = default);
}
public sealed class EfServiceDataStore(AppDbContext db) : IServiceDataStore
{
    public IQueryable<T> Query<T>() where T : class => db.Set<T>();
    public Task<IDbContextTransaction> BeginAsync(IsolationLevel? isolation = null, CancellationToken token = default)
        => isolation.HasValue ? db.Database.BeginTransactionAsync(isolation.Value, token) : db.Database.BeginTransactionAsync(token);
    public void Add(object entity) => db.Add(entity);
    public void RemoveRange(IEnumerable<object> entities) => db.RemoveRange(entities);
    public Task<int> SaveAsync(CancellationToken token = default) => db.SaveChangesAsync(token);
    public void SetVersion(object entity, string? version) => Rules.Concurrency(db, entity, version);
    public void MarkModified(object entity, string property) => db.Entry(entity).Property(property).IsModified = true;
    public Task<DateTimeOffset> NowAsync(CancellationToken token = default) => GiaoDichNghiepVu.GioMayChuAsync(db, token);
    public Task<LopHoc> LockClassAsync(Guid id, CancellationToken token = default) => GiaoDichNghiepVu.KhoaLopAsync(db, id, token);
    public Task LockProfileAsync(Guid id) => HoSoLock.KhoaAsync(db, id);
    public Task LockAdminAsync(Guid? actor = null, string? permission = null) => GiaoDichNghiepVu.KhoaQuanTriAsync(db, actor, permission);
    public Task CheckAccountAsync(Guid actor) => GiaoDichNghiepVu.KiemTraTaiKhoanAsync(db, actor);
    public Task CheckAdminAsync(Guid actor, string permission) => GiaoDichNghiepVu.KiemTraQuyenQuanTriAsync(db, actor, permission);
    public Task<int> ExecuteAsync(FormattableString command, CancellationToken token = default) => db.Database.ExecuteSqlInterpolatedAsync(command, token);
}
public interface IPhienDangNhapService
{
    Task<bool> HopLeAsync(Guid session, Guid user, bool active, CancellationToken token = default);
    Task ThuHoiAsync(Guid session, string reason);
    Task ThuHoiTatCaAsync(Guid user, string reason);
}
