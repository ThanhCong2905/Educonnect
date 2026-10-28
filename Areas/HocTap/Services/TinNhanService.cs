using System.Data;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.HocTap.Services;

public sealed class TinNhanService(AppDbContext db, NguoiDungMauService current, QuyenLopHocService rights, TepService files)
{
    public async Task<bool> CoTheGuiAsync(Guid id)
    {
        var c = await KiemTraAsync(id);
        if (c.LoaiHoiThoai != "TheoLop")
            return true;
        var u = await current.HienTaiAsync();
        return await db.Set<LopHoc>().AnyAsync(l => l.Id == c.LopHocId && l.TrangThai == "DangHoatDong" && (l.GiaSuId == u.Id || db.Set<ThanhVienLopHoc>().Any(m => m.LopHocId == l.Id && m.TaiKhoanId == u.Id && m.TrangThaiDuyet == "DaDuyet" && m.RoiLopLuc == null && m.MucQuyen == "ChinhSua")));
    }
    public async Task<List<CuocHoiThoai>> LayAsync(string? q = null)
    {
        var u = await current.HienTaiAsync();
        return await db.Set<CuocHoiThoai>().AsNoTracking().Where(c => c.LoaiHoiThoai == "TruocKetNoi").Where(c => c.LoaiHoiThoai == "TruocKetNoi" ? (c.GiaSuId == u.Id || c.BenHocId == u.Id) && db.Set<YeuCauLienHe>().Any(h => h.Id == c.LienHeId && h.TrangThai == "ChapNhan") : db.Set<ThanhVienLopHoc>().Any(m => m.LopHocId == c.LopHocId && m.TaiKhoanId == u.Id && m.TrangThaiDuyet == "DaDuyet" && m.RoiLopLuc == null)).Where(c => q == null || db.Set<LopHoc>().Any(l => l.Id == c.LopHocId && l.TenLop.Contains(q)) || db.Set<NguoiThamGia>().Any(p => (p.Id == c.GiaSuId || p.Id == c.BenHocId) && p.Id != u.Id && p.HoTen.Contains(q))).OrderByDescending(c => c.TaoLuc).Take(100).ToListAsync();
    }
    public async Task<CuocHoiThoai> KiemTraAsync(Guid id)
    {
        var c = await db.Set<CuocHoiThoai>().SingleOrDefaultAsync(x => x.Id == id) ?? throw new LoiNghiepVu("Không có cuộc hội thoại.", 404);
        var u = await current.HienTaiAsync();
        if (c.LoaiHoiThoai == "TheoLop")
            await rights.KiemTraQuyenXemAsync(c.LopHocId!.Value);
        else
            Rules.Check((c.GiaSuId == u.Id || c.BenHocId == u.Id) && await db.Set<YeuCauLienHe>().AnyAsync(x => x.Id == c.LienHeId && x.TrangThai == "ChapNhan"), "Cần thuộc liên hệ đã được chấp nhận để đọc/gửi tin nhắn.", 403);
        return c;
    }
    public async Task<List<TinNhan>> LichSuAsync(Guid id, string? q = null, int page = 1)
    {
        await KiemTraAsync(id);
        Rules.Check(page >= 1 && page <= 100000, "Trang không hợp lệ.");
        var rows = await db.Set<TinNhan>().AsNoTracking().Where(x => x.CuocHoiThoaiId == id && (q == null || x.NoiDung.Contains(q))).OrderByDescending(x => x.GuiLuc).ThenByDescending(x => x.Id).Skip((page - 1) * 50).Take(50).ToListAsync();
        rows.Reverse();
        return rows;
    }
    public async Task<object> DuLieuAsync(Guid id, Guid? before, Guid? after, string? q)
    {
        await KiemTraAsync(id);
        Rules.Check(before == null || after == null, "Chỉ chọn một hướng tải tin nhắn.");
        var me = await current.HienTaiAsync();
        IQueryable<TinNhan> query = db.Set<TinNhan>();
        var cursor = before ?? after;
        DateTimeOffset? overlapTime = null;
        if (cursor.HasValue)
        {
            var pivot = await db.Set<TinNhan>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == cursor && x.CuocHoiThoaiId == id)
                ?? throw new LoiNghiepVu("Mốc tin nhắn không còn tồn tại.", 404);
            query = after.HasValue
                ? db.Set<TinNhan>().Where(x => x.CuocHoiThoaiId == id && x.GuiLuc > pivot.GuiLuc)
                : db.Set<TinNhan>().FromSqlInterpolated($"SELECT * FROM dbo.TinNhan WHERE CuocHoiThoaiId={id} AND (GuiLuc<{pivot.GuiLuc} OR (GuiLuc={pivot.GuiLuc} AND Id<{pivot.Id}))");
            if (after.HasValue) overlapTime = pivot.GuiLuc;
        }
        query = query.AsNoTracking().Where(x => x.CuocHoiThoaiId == id && (q == null || x.NoiDung.Contains(q)));
        var ordered = after.HasValue ? query.OrderBy(x => x.GuiLuc).ThenBy(x => x.Id)
            : query.OrderByDescending(x => x.GuiLuc).ThenByDescending(x => x.Id);
        var rows = await ordered.Take(51).ToListAsync();
        var hasMore = rows.Count > 50;
        rows = rows.Take(50).ToList();
        if (!after.HasValue) rows.Reverse();
        // GuiLuc has second precision; include ties so a later insert with a smaller Guid is not missed.
        if (overlapTime.HasValue)
        {
            var ties = await db.Set<TinNhan>().AsNoTracking()
                .Where(x => x.CuocHoiThoaiId == id && x.GuiLuc == overlapTime && x.Id != after && (q == null || x.NoiDung.Contains(q)))
                .OrderBy(x => x.Id).ToListAsync();
            rows.InsertRange(0, ties);
        }
        var senders = rows.Select(x => x.NguoiGuiId).Distinct().ToList();
        var names = await db.Set<NguoiThamGia>().Where(x => senders.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x.HoTen);
        var attachments=await TepTheoTinAsync(id,rows.Select(x=>x.Id).ToList());
        return new
        {
            items = rows.Select(x => new { id = x.Id, text = x.NoiDung, sender = names.GetValueOrDefault(x.NguoiGuiId, ""), mine = x.NguoiGuiId == me.Id, time = Rules.Date(x.GuiLuc), attachments=attachments.GetValueOrDefault(x.Id,[]).Select(f=>new{id=f.Id,name=f.TenGoc,size=f.KichThuoc,mime=f.LoaiNoiDung}) }),
            hasMore,
            canSend = await CoTheGuiAsync(id)
        };
    }
    public async Task GuiAsync(GuiTinNhanViewModel m)
    {
        Rules.Check(m.MaThaoTac != Guid.Empty, "Mã thao tác không hợp lệ.");
        var text=m.NoiDung?.Trim()??"";
        Rules.Check(text.Length<=10000&&(text.Length>0||m.Files.Count>0), "Nhập nội dung hoặc chọn ít nhất một tệp, tối đa 10000 ký tự.");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var c = await KiemTraAsync(m.MaHoiThoai);
        if (c.LoaiHoiThoai == "TheoLop")
            Rules.Check(await CoTheGuiAsync(c.Id), "Bạn không có quyền nhắn tin trong lớp này.", 403);
        var u = await current.HienTaiAsync();
        var checkedFiles=await files.KiemTraAsync(m.Files);
        var old = await db.Set<TinNhan>().SingleOrDefaultAsync(x => x.NguoiGuiId == u.Id && x.MaThaoTac == m.MaThaoTac);
        if (old != null)
        {
            var prior=await db.Set<TepDinhKem>().Where(f=>f.TinNhanId==old.Id).ToListAsync();
            var expected=checkedFiles.Select(f=>f.Name+"|"+Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(f.Bytes))).Order().ToList();
            Rules.Check(old.CuocHoiThoaiId == c.Id && old.NoiDung == text && prior.Select(f=>f.TenGoc+"|"+f.Checksum).Order().SequenceEqual(expected), "Mã thao tác đã dùng cho tin nhắn khác.", 409);
            return;
        }
        var messageId=Guid.NewGuid();List<string> written=[];
        try
        {
            db.Add(new TinNhan { Id = messageId, CuocHoiThoaiId = c.Id, NguoiGuiId = u.Id, NoiDung = text, GuiLuc = DateTimeOffset.UtcNow, MaThaoTac = m.MaThaoTac });
            await db.SaveChangesAsync();
            written=await files.LuuAsync(checkedFiles,tinNhan:messageId);
            await db.SaveChangesAsync();await tx.CommitAsync();
        }
        catch{TepService.DonDep(written);throw;}
    }
    public async Task<Dictionary<Guid,List<TepDinhKem>>> TepTheoTinAsync(Guid room,List<Guid> ids)
    {
        await KiemTraAsync(room);
        var rows=await (from f in db.Set<TepDinhKem>().AsNoTracking() join m in db.Set<TinNhan>() on f.TinNhanId equals (Guid?)m.Id where m.CuocHoiThoaiId==room&&ids.Contains(m.Id)&&f.TrangThaiQuet=="Sach" select f).OrderBy(f=>f.TaoLuc).ThenBy(f=>f.Id).ToListAsync();
        return rows.GroupBy(f=>f.TinNhanId!.Value).ToDictionary(g=>g.Key,g=>g.ToList());
    }
    public async Task<(TepDinhKem File,string Path)> TaiTepAsync(Guid id)
    {
        var f=await db.Set<TepDinhKem>().AsNoTracking().SingleOrDefaultAsync(f=>f.Id==id&&f.TinNhanId!=null&&f.TrangThaiQuet=="Sach")??throw new LoiNghiepVu("Không tìm thấy tệp tin nhắn.",404);
        var room=await db.Set<TinNhan>().Where(m=>m.Id==f.TinNhanId).Select(m=>m.CuocHoiThoaiId).SingleAsync();
        await KiemTraAsync(room);return(f,files.DuongDan(f));
    }
    public async Task<Guid?> TheoLopAsync(Guid id)
    {
        await rights.KiemTraQuyenXemAsync(id);
        return await db.Set<CuocHoiThoai>().Where(x => x.LopHocId == id && x.LoaiHoiThoai == "TheoLop").Select(x => (Guid?)x.Id).FirstOrDefaultAsync();
    }
}
