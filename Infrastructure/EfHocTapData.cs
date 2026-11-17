using System.Data;
using System.Linq.Expressions;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace EduConnect.Mvc.Infrastructure;

public sealed class EfHocTapData(AppDbContext db):IHocTapData
{
    public Task<T?> SingleAsync<T>(Expression<Func<T,bool>> predicate,bool tracking=true) where T:class=>(tracking?db.Set<T>():db.Set<T>().AsNoTracking()).SingleOrDefaultAsync(predicate);
    public Task<T> RequiredAsync<T>(Expression<Func<T,bool>> predicate) where T:class=>db.Set<T>().SingleAsync(predicate);
    public Task<List<T>> ListAsync<T>(Expression<Func<T,bool>> predicate) where T:class=>db.Set<T>().Where(predicate).ToListAsync();
    public Task<bool> AnyAsync<T>(Expression<Func<T,bool>> predicate) where T:class=>db.Set<T>().AnyAsync(predicate);
    public Task<IDbContextTransaction> BeginAsync(IsolationLevel? level=null)=>level.HasValue?db.Database.BeginTransactionAsync(level.Value):db.Database.BeginTransactionAsync();
    public async Task ExecuteAsync(FormattableString command)=>await db.Database.ExecuteSqlInterpolatedAsync(command);
    public Task ReloadAsync(object entity)=>db.Entry(entity).ReloadAsync();
    public void SetVersion(object entity,string? version)=>Rules.Concurrency(db,entity,version);
    public void Add<T>(T entity) where T:class=>db.Add(entity);
    public async Task SaveAsync()=>await db.SaveChangesAsync();
    public Task<bool> OverlapAsync(LopHoc c,Guid excluding,DateTimeOffset start,DateTimeOffset end)
        =>(from l in db.Set<LichHoc>() join cl in db.Set<LopHoc>() on l.LopHocId equals cl.Id where l.Id!=excluding&&l.TrangThai=="DaLenLich"&&cl.TrangThai=="DangHoatDong"&&(cl.GiaSuId==c.GiaSuId||cl.BenHocId==c.BenHocId)&&l.BatDauLuc<end&&start<l.KetThucLuc select l).AnyAsync();
    public Task<bool> LessonInClassAsync(Guid lesson,Guid lop)
        =>(from b in db.Set<BuoiHoc>() join l in db.Set<LichHoc>() on b.LichHocId equals l.Id where b.Id==lesson&&l.LopHocId==lop select b).AnyAsync();
}
