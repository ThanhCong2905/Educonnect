using System.Linq.Expressions;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace EduConnect.Mvc.Areas.KetNoi.Services;

public sealed class EfKetNoiStore(AppDbContext db):IKetNoiStore
{
    public Task<T?> SingleAsync<T>(Expression<Func<T,bool>> predicate,CancellationToken ct=default,bool tracking=true)where T:class
        =>(tracking?db.Set<T>():db.Set<T>().AsNoTracking()).SingleOrDefaultAsync(predicate,ct);
    public Task<T> RequiredAsync<T>(Expression<Func<T,bool>> predicate,CancellationToken ct=default)where T:class=>db.Set<T>().SingleAsync(predicate,ct);
    public Task<bool> AnyAsync<T>(Expression<Func<T,bool>> predicate,CancellationToken ct=default)where T:class=>db.Set<T>().AnyAsync(predicate,ct);
    public Task<Guid> FindAcceptedContactIdAsync(Guid tutor,Guid learner,CancellationToken ct)=>db.Set<YeuCauLienHe>()
        .Where(x=>x.GiaSuId==tutor&&x.BenHocId==learner&&x.TrangThai=="ChapNhan").Select(x=>x.Id).SingleOrDefaultAsync(ct);
    public Task<Guid> LatestRequestIdAsync(Guid contact,string state,CancellationToken ct)
    {
        var query=db.Set<YeuCauKetNoiLopHoc>().Where(x=>x.LienHeId==contact&&x.TrangThai==state);
        return state=="ChoGiaSu"?query.OrderByDescending(x=>x.GuiLuc).Select(x=>x.Id).FirstAsync(ct):query.OrderByDescending(x=>x.DeXuatLuc).Select(x=>x.Id).FirstAsync(ct);
    }
    public Task<Guid> ClassIdForRequestAsync(Guid request,CancellationToken ct=default)=>db.Set<LopHoc>().Where(x=>x.YeuCauKetNoiId==request).Select(x=>x.Id).SingleAsync(ct);
    public async Task LockContactAsync(Guid contact,CancellationToken ct)=>await db.Set<YeuCauLienHe>()
        .FromSqlInterpolated($"SELECT * FROM dbo.YeuCauLienHe WITH (UPDLOCK,HOLDLOCK) WHERE Id={contact}").AsNoTracking().SingleAsync(ct);
    public Task<bool> HasActiveClassAsync(Guid contact,CancellationToken ct)=>(from lop in db.Set<LopHoc>()
        join request in db.Set<YeuCauKetNoiLopHoc>() on lop.YeuCauKetNoiId equals request.Id
        where request.LienHeId==contact&&lop.TrangThai=="DangHoatDong" select lop.Id).AnyAsync(ct);
    public Task<IDbContextTransaction> BeginAsync(CancellationToken ct)=>db.Database.BeginTransactionAsync(ct);
    public async Task ExecuteAsync(FormattableString command,CancellationToken ct)=>await db.Database.ExecuteSqlInterpolatedAsync(command,ct);
    public void SetVersion(object entity,string version)=>Rules.Concurrency(db,entity,version);
    public async Task SaveAsync(CancellationToken ct)=>await db.SaveChangesAsync(ct);
}
