using System.Data;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace EduConnect.Mvc.Areas.NhuCau.Services;

public sealed class EfNhuCauStore(AppDbContext db, MonHocCatalog catalog) : INhuCauStore
{
    public Task<IDbContextTransaction> BeginAsync(CancellationToken ct) => db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
    public async Task LockOwnerAsync(Guid owner, CancellationToken ct)
    {
        await db.Database.ExecuteSqlInterpolatedAsync($"SELECT Id FROM dbo.AspNetUsers WITH(UPDLOCK,HOLDLOCK) WHERE Id={owner}", ct);
        var status = await db.Users.Where(x => x.Id == owner).Select(x => x.TrangThai).SingleOrDefaultAsync(ct);
        Rules.Check(status == "HoatDong", "Tài khoản không hoạt động.", 403);
    }
    public Task<LuotDangNhuCau?> FindOperationAsync(Guid owner, Guid operation, CancellationToken ct) => db.Set<LuotDangNhuCau>().AsNoTracking().SingleOrDefaultAsync(x => x.TaiKhoanId == owner && x.MaThaoTac == operation, ct);
    public Task<NhuCauHoc?> FindOwnedAsync(Guid id, Guid owner, bool tracking, CancellationToken ct)
    {
        IQueryable<NhuCauHoc> query = db.Set<NhuCauHoc>();
        if (!tracking) query = query.AsNoTracking().Include(x => x.MonHocs);
        return query.SingleOrDefaultAsync(x => x.Id == id && x.ChuSoHuuId == owner, ct);
    }
    public Task<int> CountNeedsAsync(Guid owner, string state, Guid? excluding, CancellationToken ct) => db.Set<NhuCauHoc>().CountAsync(x => x.ChuSoHuuId == owner && x.TrangThai == state && x.Id != excluding, ct);
    public Task<int> CountPostingsAsync(Guid owner, DateOnly date, CancellationToken ct) => db.Set<LuotDangNhuCau>().CountAsync(x => x.TaiKhoanId == owner && x.NgayDang == date, ct);
    public Task<int> CountActiveSubjectsAsync(List<int> ids, CancellationToken ct) => db.Set<MonHoc>().CountAsync(x => ids.Contains(x.Id) && x.DangSuDung, ct);
    public Task<int> ResolveSubjectAsync(string? name, CancellationToken ct) => catalog.LayHoacThemAsync(name, ct);
    public async Task<List<Guid>> FindDuplicateCandidatesAsync(NhuCauInput input, Guid owner, CancellationToken huy)
    {
        return await db.Set<NhuCauHoc>().Where(x => x.ChuSoHuuId == owner && x.Id != input.Id && x.TrangThai == "DangMo"
            && x.TuoiNguoiHoc == input.TuoiNguoiHoc && x.HinhThucHoc == input.HinhThucHoc
            && x.HocPhiDuKien == input.HocPhiDuKien && x.DonViHocPhi == input.DonViHocPhi
            && x.SoBuoiTuan == input.SoBuoiTuan)
            .Select(x => x.Id).ToListAsync(huy);
    }
    public Task<List<KhungGioRanh>> ReadSlotsAsync(Guid id, CancellationToken ct) => db.Set<KhungGioRanh>().Where(x => x.NhuCauId == id).ToListAsync(ct);
    public Task<MonHoc> ReadSubjectAsync(int id, CancellationToken ct) => db.Set<MonHoc>().SingleAsync(x => x.Id == id, ct);
    public Task<bool> IsCommonSubjectAsync(int id, CancellationToken ct) => db.Set<MonHoc>().AnyAsync(x => x.Id == id && MonHocCatalog.PhoThong.Contains(x.TenMon), ct);
    public void AddNeed(NhuCauHoc need) => db.Add(need);
    public void SetVersion(NhuCauHoc need, byte[] version)
    {
        db.Entry(need).Property(x => x.RowVersion).OriginalValue = version;
        db.Entry(need).Property(x => x.MucTieu).IsModified = true;
    }
    public void AddPosting(LuotDangNhuCau posting) => db.Add(posting);
    public async Task SaveAsync(CancellationToken ct) => await db.SaveChangesAsync(ct);
    public async Task DeleteSlotsAsync(Guid id, CancellationToken ct) => await db.Set<KhungGioRanh>().Where(x => x.NhuCauId == id).ExecuteDeleteAsync(ct);
    public void AddSlot(KhungGioRanh slot) => db.Add(slot);
    public Task<List<NhuCauMonHoc>> ReadLinksAsync(Guid id, CancellationToken ct) => db.Set<NhuCauMonHoc>().Where(x => x.NhuCauId == id).ToListAsync(ct);
    public void RemoveLinks(IEnumerable<NhuCauMonHoc> links) => db.RemoveRange(links);
    public void AddLinks(IEnumerable<NhuCauMonHoc> links) => db.AddRange(links);
}
