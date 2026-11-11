using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
namespace EduConnect.Mvc.Areas.HocTap.Services;

public sealed partial class BaiTapService(AppDbContext db, NguoiDungHienTaiService current, QuyenLopHocService rights, ThongBaoService notices, TepService files)
{
    public async Task<List<BaiTapGiao>> LayAsync(Guid lop)
    {
        var c = await rights.KiemTraQuyenXemAsync(lop);
        var me = await current.HienTaiAsync();
        return await db.Set<BaiTapGiao>().AsNoTracking().Where(x => x.LopHocId == lop && (x.TrangThai == "DaGiao" || c.GiaSuId == me.Id)).OrderByDescending(x => x.HanNopLuc).ToListAsync();
    }
    public async Task<BaiTapGiao> ChiTietAsync(Guid id)
    {
        var a = await db.Set<BaiTapGiao>().SingleOrDefaultAsync(x => x.Id == id) ?? throw new LoiNghiepVu("Bài tập không tồn tại.", 404);
        var c = await rights.KiemTraQuyenXemAsync(a.LopHocId);
        Rules.Check(a.TrangThai == "DaGiao" || c.GiaSuId == (await current.HienTaiAsync()).Id, "Bài tập chưa được giao.", 404);
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
        Rules.ValidateInput(m);
        var c = await rights.KiemTraQuyenGhiAsync(m.LopHocId, true);
        var u = await current.HienTaiAsync();
        Rules.Check(m.BuoiHocId.HasValue && m.BuoiHocId != Guid.Empty, "Chọn buổi học cho bài tập.", field: "BuoiHocId");
        var lesson = await db.Set<BuoiHoc>().SingleOrDefaultAsync(x => x.Id == m.BuoiHocId);
        Rules.Check(lesson != null && (lesson.TrangThai is "DuKien" or "DangDienRa" or "DaHoc"), "Buổi học không còn cho phép giao bài.", 409);
        Rules.Check(m.LuuNhap || lesson!.TrangThai is "DangDienRa" or "DaHoc", "Buổi chưa diễn ra chỉ được lưu bài nháp.", 409);
        Rules.Check(Rules.Local(m.HanNopLuc) > DateTimeOffset.UtcNow, "Hạn nộp phải ở tương lai.");
        Rules.Check(!string.IsNullOrWhiteSpace(m.TieuDe) && m.TieuDe.Length <= 256 && !string.IsNullOrWhiteSpace(m.NoiDung) && m.NoiDung.Length <= 10000 && (m.TieuChiCham?.Length ?? 0) <= 5000, "Tiêu đề, nội dung hoặc tiêu chí chấm không hợp lệ.");
        if (m.BuoiHocId != null)
            Rules.Check(await (from b in db.Set<BuoiHoc>() join l in db.Set<LichHoc>() on b.LichHocId equals l.Id where b.Id == m.BuoiHocId && l.LopHocId == c.Id select b).AnyAsync(), "Buổi học không thuộc lớp.");
        var checkedFiles = await files.KiemTraAsync(m.Files);
        List<string> written = [];
        await using var tx = await db.Database.BeginTransactionAsync();
        try
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT Id FROM dbo.LopHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id={c.Id}");
            await db.Entry(c).ReloadAsync();
            Rules.Check(c.TrangThai == "DangHoatDong", "Lớp không còn hoạt động.", 409);
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT Id FROM dbo.BuoiHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id={m.BuoiHocId}");
            await db.Entry(lesson!).ReloadAsync();
            Rules.Check(lesson!.TrangThai is "DuKien" or "DangDienRa" or "DaHoc", "Buổi học không còn cho phép giao bài.", 409);
            Rules.Check(m.LuuNhap || lesson.TrangThai is "DangDienRa" or "DaHoc", "Buổi chưa diễn ra chỉ được lưu bài nháp.", 409);
            var id = Guid.NewGuid();
            db.Add(new BaiTapGiao { Id = id, LopHocId = c.Id, BuoiHocId = m.BuoiHocId, TrangThai = m.LuuNhap ? "BanNhap" : "DaGiao", NguoiGiaoId = u.Id, TieuDe = m.TieuDe, NoiDung = m.NoiDung, HanNopLuc = Rules.Local(m.HanNopLuc), TieuChiCham = m.TieuChiCham });
            await db.SaveChangesAsync();
            written = await files.LuuAsync(checkedFiles, giao: id);
            if (!m.LuuNhap) await notices.TaoChoLopAsync(c.Id, "BaiTap", id, "Có bài tập mới.", u.Id);
            await db.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch { TepService.DonDep(written); throw; }
    }
    public async Task NopAsync(NopBaiViewModel m)
    {
        Rules.ValidateInput(m);
        var a = await ChiTietAsync(m.BaiTapGiaoId);
        var c = await rights.KiemTraQuyenGhiAsync(a.LopHocId);
        var u = await current.HienTaiAsync();
        Rules.Check(u.Id != c.GiaSuId, "Gia sư không nộp bài của lớp mình.", 403);
        Rules.Check(a.TrangThai == "DaGiao", "Bài tập chưa được giao.", 409);
        if (u.Id != c.BenHocId)
            Rules.Check(await db.Set<ThanhVienLopHoc>().AnyAsync(x => x.LopHocId == c.Id && x.TaiKhoanId == u.Id && x.TrangThaiDuyet == "DaDuyet" && x.RoiLopLuc == null && x.MucQuyen == "ChinhSua" && x.ChoPhepNopBai), "Bạn chưa được cấp quyền nộp bài thay.", 403);
        Rules.Check(m.MaThaoTac != Guid.Empty, "Mã thao tác không hợp lệ.");
        Rules.Check(!string.IsNullOrWhiteSpace(m.NoiDung) || m.Files.Count > 0, "Cần nội dung hoặc tệp bài làm.");
        Rules.Check((m.NoiDung?.Length ?? 0) <= 10000, "Nội dung bài làm tối đa 10000 ký tự.");
        var checkedFiles = await files.KiemTraAsync(m.Files);
        List<string> written = [];
        await using var tx = await db.Database.BeginTransactionAsync();
        try
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT Id FROM dbo.BaiTapGiao WITH(UPDLOCK,HOLDLOCK) WHERE Id={a.Id}");
            var existing = await db.Set<BaiTapNop>().AsNoTracking().SingleOrDefaultAsync(x => x.NguoiNopId == u.Id && x.MaThaoTac == m.MaThaoTac);
            await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.usp_NopBaiTap @BaiTapGiaoId={m.BaiTapGiaoId},@NguoiNopId={u.Id},@MaThaoTac={m.MaThaoTac},@NoiDung={m.NoiDung}");
            var n = await db.Set<BaiTapNop>().SingleAsync(x => x.NguoiNopId == u.Id && x.MaThaoTac == m.MaThaoTac);
            if (existing != null)
            {
                var storedFiles = await db.Set<TepDinhKem>().AsNoTracking().Where(x => x.BaiTapNopId == existing.Id).ToListAsync();
                var before = storedFiles.Select(x => (x.TenGoc, x.LoaiNoiDung, x.KichThuoc, x.Checksum)).OrderBy(x => x.TenGoc, StringComparer.Ordinal).ThenBy(x => x.Checksum, StringComparer.Ordinal).ToArray();
                var after = checkedFiles.Select(x => (TenGoc: x.Name, LoaiNoiDung: x.Mime, KichThuoc: (long)x.Bytes.Length, Checksum: Convert.ToHexString(SHA256.HashData(x.Bytes)))).OrderBy(x => x.TenGoc, StringComparer.Ordinal).ThenBy(x => x.Checksum, StringComparer.Ordinal).ToArray();
                Rules.Check(before.SequenceEqual(after), "Mã thao tác đã dùng với tệp khác. Hãy tải lại bài tập.", 409);
            }
            else
            {
                written = await files.LuuAsync(checkedFiles, nop: n.Id);
                notices.Tao(c.GiaSuId, "BaiTap", a.Id, n.NopLuc > a.HanNopLuc ? "Có bài nộp muộn." : "Có bài nộp mới.");
                await db.SaveChangesAsync();
            }
            await tx.CommitAsync();
        }
        catch { TepService.DonDep(written); throw; }
    }
    public async Task ChamAsync(ChamBaiViewModel m)
    {
        Rules.ValidateInput(m);
        var n = await db.Set<BaiTapNop>().SingleOrDefaultAsync(x => x.Id == m.BaiTapNopId) ?? throw new LoiNghiepVu("Không có bài nộp.", 404);
        var a = await ChiTietAsync(n.BaiTapGiaoId);
        Rules.Check(m.BaiTapGiaoId == Guid.Empty || m.BaiTapGiaoId == a.Id, "Bài nộp không thuộc bài tập đang mở.");
        var c = await rights.KiemTraQuyenGhiAsync(a.LopHocId, true);
        var u = await current.HienTaiAsync();
        Rules.Check(m.Diem == null || m.Diem is >= 0 and <= 10 && decimal.Round(m.Diem.Value, 2) == m.Diem, "Điểm từ 0 đến 10, tối đa hai chữ số thập phân.");
        Rules.Check(!string.IsNullOrWhiteSpace(m.NhanXet) && m.NhanXet.Length <= 2000, "Cần nhận xét tối đa 2000 ký tự.");
        var rv = Rules.SqlVersion(m.RowVersion);
        await using var tx = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.usp_ChamBaiTap @BaiTapNopId={m.BaiTapNopId},@NguoiChamId={u.Id},@Diem={m.Diem},@NhanXet={m.NhanXet},@RowVersionCu={rv}");
        await notices.TaoChoLopAsync(c.Id, "BaiTap", a.Id, "Bài tập đã có nhận xét và kết quả mới.", u.Id);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}

