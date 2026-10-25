using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.HocTap.Services;

public sealed class BaiTapService(AppDbContext db, NguoiDungMauService current, QuyenLopHocService rights, ThongBaoService notices, TepService files)
{
    public async Task<List<BaiTapGiao>> LayAsync(Guid lop)
    {
        await rights.KiemTraQuyenXemAsync(lop);
        return await db.Set<BaiTapGiao>().AsNoTracking().Where(x => x.LopHocId == lop).OrderByDescending(x => x.HanNopLuc).ToListAsync();
    }
    public async Task<BaiTapGiao> ChiTietAsync(Guid id)
    {
        var a = await db.Set<BaiTapGiao>().SingleOrDefaultAsync(x => x.Id == id) ?? throw new LoiNghiepVu("Bài tập không tồn tại.", 404);
        await rights.KiemTraQuyenXemAsync(a.LopHocId);
        return a;
    }
    public async Task<List<BaiTapNop>> BaiNopAsync(Guid id)
    {
        await ChiTietAsync(id);
        return await db.Set<BaiTapNop>().AsNoTracking().Where(x => x.BaiTapGiaoId == id).OrderByDescending(x => x.LanNop).ToListAsync();
    }
    public async Task<List<KetQuaChamBai>> KetQuaAsync(Guid id)
    {
        await ChiTietAsync(id);
        return await (from k in db.Set<KetQuaChamBai>() join n in db.Set<BaiTapNop>() on k.BaiTapNopId equals n.Id where n.BaiTapGiaoId == id select k).AsNoTracking().ToListAsync();
    }
    public async Task GiaoAsync(GiaoBaiTapViewModel m)
    {
        var c = await rights.KiemTraQuyenGhiAsync(m.LopHocId, true);
        var u = await current.HienTaiAsync();
        Rules.Check(Rules.Local(m.HanNopLuc) > DateTimeOffset.UtcNow, "Hạn nộp phải ở tương lai.");
        if (m.BuoiHocId != null)
            Rules.Check(await (from b in db.Set<BuoiHoc>() join l in db.Set<LichHoc>() on b.LichHocId equals l.Id where b.Id == m.BuoiHocId && l.LopHocId == c.Id select b).AnyAsync(), "Buổi học không thuộc lớp.");
        var checkedFiles = await files.KiemTraAsync(m.Files);
        List<string> written = [];
        await using var tx = await db.Database.BeginTransactionAsync();
        try
        {
            var id = Guid.NewGuid();
            db.Add(new BaiTapGiao { Id = id, LopHocId = c.Id, BuoiHocId = m.BuoiHocId, NguoiGiaoId = u.Id, TieuDe = m.TieuDe, NoiDung = m.NoiDung, HanNopLuc = Rules.Local(m.HanNopLuc), TieuChiCham = m.TieuChiCham });
            await db.SaveChangesAsync();
            written = await files.LuuAsync(checkedFiles, giao: id);
            notices.Tao(c.BenHocId, "BaiTap", id, "Có bài tập mới: " + m.TieuDe);
            await db.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch { TepService.DonDep(written); throw; }
    }
    public async Task NopAsync(NopBaiViewModel m)
    {
        var a = await ChiTietAsync(m.BaiTapGiaoId);
        var c = await rights.KiemTraQuyenGhiAsync(a.LopHocId);
        var u = await current.HienTaiAsync();
        Rules.Check(u.Id != c.GiaSuId, "Gia sư không nộp bài của lớp mình.", 403);
        Rules.Check(m.MaThaoTac != Guid.Empty, "Mã thao tác không hợp lệ.");
        Rules.Check(!string.IsNullOrWhiteSpace(m.NoiDung) || m.Files.Count > 0, "Cần nội dung hoặc tệp bài làm.");
        var checkedFiles = await files.KiemTraAsync(m.Files);
        List<string> written = [];
        await using var tx = await db.Database.BeginTransactionAsync();
        try
        {
            var existing = await db.Set<BaiTapNop>().AsNoTracking().SingleOrDefaultAsync(x => x.NguoiNopId == u.Id && x.MaThaoTac == m.MaThaoTac);
            await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.usp_NopBaiTap @BaiTapGiaoId={m.BaiTapGiaoId},@NguoiNopId={u.Id},@MaThaoTac={m.MaThaoTac},@NoiDung={m.NoiDung}");
            var n = await db.Set<BaiTapNop>().SingleAsync(x => x.NguoiNopId == u.Id && x.MaThaoTac == m.MaThaoTac);
            if (existing == null)
            {
                written = await files.LuuAsync(checkedFiles, nop: n.Id);
                notices.Tao(c.GiaSuId, "BaiTap", a.Id, "Có bài nộp mới: " + a.TieuDe);
                await db.SaveChangesAsync();
            }
            await tx.CommitAsync();
        }
        catch { TepService.DonDep(written); throw; }
    }
    public async Task ChamAsync(ChamBaiViewModel m)
    {
        var n = await db.Set<BaiTapNop>().SingleOrDefaultAsync(x => x.Id == m.BaiTapNopId) ?? throw new LoiNghiepVu("Không có bài nộp.");
        var a = await ChiTietAsync(n.BaiTapGiaoId);
        var c = await rights.KiemTraQuyenGhiAsync(a.LopHocId, true);
        var u = await current.HienTaiAsync();
        Rules.Check(m.Diem == null || decimal.Round(m.Diem.Value, 2) == m.Diem, "Điểm tối đa hai chữ số thập phân.");
        var rv = Rules.SqlVersion(m.RowVersion);
        await using var tx = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.usp_ChamBaiTap @BaiTapNopId={m.BaiTapNopId},@NguoiChamId={u.Id},@Diem={m.Diem},@NhanXet={m.NhanXet},@RowVersionCu={rv}");
        notices.Tao(c.BenHocId, "BaiTap", a.Id, "Bài tập đã có nhận xét: " + a.TieuDe);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}

