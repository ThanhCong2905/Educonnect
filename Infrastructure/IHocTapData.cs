using System.Data;
using System.Linq.Expressions;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore.Storage;
namespace EduConnect.Mvc.Infrastructure;

// Persistence boundary. It deliberately does not emulate stored procedures.
public interface IHocTapData
{
    Task<T?> SingleAsync<T>(Expression<Func<T,bool>> predicate,bool tracking=true) where T:class;
    Task<T> RequiredAsync<T>(Expression<Func<T,bool>> predicate) where T:class;
    Task<List<T>> ListAsync<T>(Expression<Func<T,bool>> predicate) where T:class;
    Task<bool> AnyAsync<T>(Expression<Func<T,bool>> predicate) where T:class;
    Task<IDbContextTransaction> BeginAsync(IsolationLevel? level=null);
    Task ExecuteAsync(FormattableString command);
    Task ReloadAsync(object entity);
    void SetVersion(object entity,string? version);
    void Add<T>(T entity) where T:class;
    Task SaveAsync();
    Task<bool> OverlapAsync(LopHoc lop,Guid excluding,DateTimeOffset start,DateTimeOffset end);
    Task<bool> LessonInClassAsync(Guid lesson,Guid lop);
}
