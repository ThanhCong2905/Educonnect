using System.Data;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace EduConnect.Mvc.Areas.KetNoi.Services;

public sealed class EfLienHeStore(AppDbContext db, ThongBaoService notices) : ILienHeStore
{
    public Task<NguoiThamGia?> ReadRecipientAsync(Guid id) => db.Set<NguoiThamGia>().SingleOrDefaultAsync(x => x.Id == id);
    public Task<IDbContextTransaction> BeginAsync() => db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
    public Task LockProfileAsync(Guid tutor) => HoSoLock.KhoaAsync(db, tutor);
    public async Task<bool> IsHiddenAsync(Guid tutor) => await db.Set<BienPhapQuanTri>().AnyAsync(b=>b.LoaiDoiTuong=="HoSoGiaSu"&&b.GoLuc==null&&b.Loai=="AnNoiDung"&&db.Set<HoSoGiaSu>().Any(h=>h.Id==b.DoiTuongId&&h.GiaSuId==tutor));
    public async Task<bool> IsEligibleAsync(Guid tutor) => await db.Set<HoSoGiaSu>().AnyAsync(h => h.GiaSuId == tutor && h.DangHienThi && h.TrangThaiDuyet == "DaDuyet" && (h.DayTrucTuyen || h.DayTrucTiep)
            && db.Set<HoSoGiaSuMonDay>().Any(d => d.HoSoGiaSuId == h.Id)
            && !db.Set<HoSoGiaSuMonDay>().Any(d => d.HoSoGiaSuId == h.Id && (d.HocPhiDeXuat == null || d.DonViHocPhi == null || !db.Set<MonHoc>().Any(m => m.Id == d.MonHocId && m.DangSuDung))));
    public Task<bool> ExistsAsync(Guid tutor, Guid learner) => db.Set<YeuCauLienHe>().AnyAsync(x => x.GiaSuId == tutor && x.BenHocId == learner && (x.TrangThai == "ChapNhan" || x.TrangThai == "ChoPhanHoi"));
    public void AddContact(YeuCauLienHe contact) => db.Add(contact);
    public void Notify(Guid recipient, string type, Guid target, string text) => notices.Tao(recipient, type, target, text);
    public async Task SaveAsync() => await db.SaveChangesAsync();
}
