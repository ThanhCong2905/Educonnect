using System.Data;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace EduConnect.Mvc.Infrastructure;

public sealed class EfClassEndingStore(AppDbContext db):IClassEndingStore
{
    public Task<IDbContextTransaction> BeginAsync(CancellationToken ct)=>db.Database.BeginTransactionAsync(IsolationLevel.Serializable,ct);
    public Task<LopHoc?> LockClassAsync(Guid id,CancellationToken ct)=>db.Set<LopHoc>().FromSqlInterpolated($"SELECT * FROM dbo.LopHoc WITH (UPDLOCK,HOLDLOCK) WHERE Id={id}").SingleOrDefaultAsync(ct);
    public Task<YeuCauHuyKetNoi> LockRequestAsync(Guid id,CancellationToken ct)=>db.Set<YeuCauHuyKetNoi>().FromSqlInterpolated($"SELECT * FROM dbo.YeuCauHuyKetNoi WITH (UPDLOCK,HOLDLOCK) WHERE Id={id}").SingleAsync(ct);
    public Task<Guid?> RequestClassIdAsync(Guid id,CancellationToken ct)=>db.Set<YeuCauHuyKetNoi>().AsNoTracking().Where(x=>x.Id==id).Select(x=>(Guid?)x.LopHocId).SingleOrDefaultAsync(ct);
    public Task<bool> HasPendingAsync(Guid id,CancellationToken ct)=>db.Set<YeuCauHuyKetNoi>().AnyAsync(x=>x.LopHocId==id&&x.TrangThai=="ChoPhanHoi",ct);
    public Task<DateTimeOffset> NowAsync(CancellationToken ct)=>db.Database.SqlQuery<DateTimeOffset>($"SELECT TODATETIMEOFFSET(SYSUTCDATETIME(), '+00:00') AS Value").SingleAsync(ct);
    public async Task<List<(Guid Id,Guid Lop)>> ExpiredAsync(DateTimeOffset now,CancellationToken ct)
    {
        var rows=await db.Set<YeuCauHuyKetNoi>().AsNoTracking().Where(x=>x.TrangThai=="ChoPhanHoi"&&x.HetHanLuc<=now).Select(x=>new{x.Id,x.LopHocId}).ToListAsync(ct);
        return rows.Select(x=>(x.Id,x.LopHocId)).ToList();
    }
    public void Add(YeuCauHuyKetNoi request)=>db.Add(request);
    public void SetVersion(YeuCauHuyKetNoi request,byte[] version)=>db.Entry(request).Property(x=>x.RowVersion).OriginalValue=version;
    public async Task SaveAsync(CancellationToken ct)=>await db.SaveChangesAsync(ct);
}
