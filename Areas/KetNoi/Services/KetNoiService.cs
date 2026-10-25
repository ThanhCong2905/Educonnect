using System.Data;
using System.Text.Json;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.KetNoi.Services;

public sealed class KetNoiService(AppDbContext db, NguoiDungMauService current, ThongBaoService notices)
{
    public async Task<Dictionary<Guid, Guid>> HoiThoaiLienHeAsync()
    {
        var u = await current.HienTaiAsync();
        return await db.Set<CuocHoiThoai>().Where(c => c.LoaiHoiThoai == "TruocKetNoi" && c.LienHeId != null && (c.GiaSuId == u.Id || c.BenHocId == u.Id) && db.Set<YeuCauLienHe>().Any(h => h.Id == c.LienHeId && h.TrangThai == "ChapNhan")).ToDictionaryAsync(c => c.LienHeId!.Value, c => c.Id);
    }
    public async Task<Guid?> HoiThoaiAsync(Guid id)
    {
        var u = await current.HienTaiAsync();
        return await db.Set<CuocHoiThoai>().Where(c => c.LienHeId == id && c.LoaiHoiThoai == "TruocKetNoi" && (c.GiaSuId == u.Id || c.BenHocId == u.Id) && db.Set<YeuCauLienHe>().Any(h => h.Id == id && h.TrangThai == "ChapNhan")).Select(c => (Guid?)c.Id).FirstOrDefaultAsync();
    }
    public async Task<(List<YeuCauLienHe> Contacts, List<YeuCauKetNoiLopHoc> Requests)> LayAsync()
    {
        var u = await current.HienTaiAsync();
        return (await db.Set<YeuCauLienHe>().AsNoTracking().Where(x => x.GiaSuId == u.Id || x.BenHocId == u.Id).OrderByDescending(x => x.GuiLuc).ToListAsync(), await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().Where(x => x.GiaSuId == u.Id || x.BenHocId == u.Id).OrderByDescending(x => x.GuiLuc).ToListAsync());
    }
    public async Task<YeuCauLienHe?> LienHeVoiAsync(Guid other)
    {
        var u = await current.HienTaiAsync();
        return await db.Set<YeuCauLienHe>().AsNoTracking().Where(x => (x.GiaSuId == u.Id && x.BenHocId == other || x.BenHocId == u.Id && x.GiaSuId == other) && (x.TrangThai == "ChapNhan" || x.TrangThai == "ChoPhanHoi")).FirstOrDefaultAsync();
    }
    public async Task GuiLienHeAsync(GuiLienHeInput m)
    {
        var u = await current.HienTaiAsync();
        var other = await db.Set<NguoiThamGia>().SingleOrDefaultAsync(x => x.Id == m.DoiTuongId) ?? throw new LoiNghiepVu("Người nhận không tồn tại.");
        Rules.Check(u.Id != other.Id && (u.LoaiNguoi == "GiaSu") != (other.LoaiNguoi == "GiaSu"), "Liên hệ cần một gia sư và một bên học.");
        var tutor = u.LoaiNguoi == "GiaSu" ? u.Id : other.Id;
        var learner = u.LoaiNguoi == "GiaSu" ? other.Id : u.Id;
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        Rules.Check(!await db.Set<YeuCauLienHe>().AnyAsync(x => x.GiaSuId == tutor && x.BenHocId == learner && (x.TrangThai == "ChapNhan" || x.TrangThai == "ChoPhanHoi")), "Hai bên đã liên hệ hoặc đang chờ phản hồi.");
        var id = Guid.NewGuid();
        db.Add(new YeuCauLienHe { Id = id, GiaSuId = tutor, BenHocId = learner, NguoiGuiId = u.Id, LoiNhan = m.LoiNhan.Trim(), TrangThai = "ChoPhanHoi", GuiLuc = DateTimeOffset.UtcNow });
        notices.Tao(other.Id, "KetNoi", id, u.HoTen + " gửi yêu cầu liên hệ.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
    public async Task PhanHoiLienHeAsync(Guid id, string action, string version)
    {
        Rules.Check(action is "ChapNhan" or "TuChoi" or "DaRut", "Phản hồi không hợp lệ.");
        var u = await current.HienTaiAsync();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var r = await db.Set<YeuCauLienHe>().SingleOrDefaultAsync(x => x.Id == id && (x.GiaSuId == u.Id || x.BenHocId == u.Id)) ?? throw new LoiNghiepVu("Không có quyền phản hồi.", 403);
        Rules.Check(r.TrangThai == "ChoPhanHoi", "Yêu cầu đã được xử lý.", 409);
        Rules.Check(action == "DaRut" ? r.NguoiGuiId == u.Id : r.NguoiGuiId != u.Id, "Không có quyền phản hồi.", 403);
        Rules.Concurrency(db, r, version);
        r.TrangThai = action;
        if (action != "DaRut")
        {
            r.NguoiPhanHoiId = u.Id;
            r.PhanHoiLuc = DateTimeOffset.UtcNow;
        }
        await db.SaveChangesAsync();
        if (action == "ChapNhan" && !await db.Set<CuocHoiThoai>().AnyAsync(c => c.LienHeId == id && c.LoaiHoiThoai == "TruocKetNoi"))
            db.Add(new CuocHoiThoai { Id = Guid.NewGuid(), LoaiHoiThoai = "TruocKetNoi", GiaSuId = r.GiaSuId, BenHocId = r.BenHocId, LienHeId = id, TaoLuc = DateTimeOffset.UtcNow });
        notices.Tao(u.Id == r.GiaSuId ? r.BenHocId : r.GiaSuId, "KetNoi", id, u.HoTen + ": " + Rules.Label(action) + " liên hệ.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
    private async Task<YeuCauLienHe> AcceptedAsync(Guid id, bool tutor)
    {
        var u = await current.HienTaiAsync();
        Rules.Check(tutor ? u.LoaiNguoi == "GiaSu" : u.LoaiNguoi != "GiaSu", "Vai trò không được thực hiện thao tác này.", 403);
        return await db.Set<YeuCauLienHe>().SingleOrDefaultAsync(x => x.Id == id && x.TrangThai == "ChapNhan" && (tutor ? x.GiaSuId == u.Id : x.BenHocId == u.Id)) ?? throw new LoiNghiepVu("Cần liên hệ được chấp nhận trước.", 403);
    }
    public async Task GuiYeuCauAsync(GuiYeuCauInput m)
    {
        var c = await AcceptedAsync(m.LienHeId, false);
        var u = await current.HienTaiAsync();
        await using var tx = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.usp_GuiYeuCauKetNoi @LienHeId={m.LienHeId},@BenHocId={u.Id},@LoiNhan={m.LoiNhan},@NhuCauId={m.NhuCauId}");
        var target = await db.Set<YeuCauKetNoiLopHoc>().Where(x => x.LienHeId == m.LienHeId && x.TrangThai == "ChoGiaSu").OrderByDescending(x => x.GuiLuc).Select(x => x.Id).FirstAsync();
        notices.Tao(c.GiaSuId, "KetNoi", target, "Có yêu cầu kết nối mới.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
    public async Task<DeXuatLopViewModel> LayDeXuatAsync(Guid contact, Guid? id)
    {
        await AcceptedAsync(contact, true);
        if (id == null)
            return new()
            {
                LienHeId = contact
            };
        var u = await current.HienTaiAsync();
        var r = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.LienHeId == contact && x.GiaSuId == u.Id) ?? throw new LoiNghiepVu("Không có yêu cầu này.", 404);
        var slots = await db.Set<KhungGioDeXuat>().Where(x => x.YeuCauKetNoiId == id).Select(x => new SlotInput { ThuTrongTuan = x.ThuTrongTuan, GioBatDau = x.GioBatDau, GioKetThuc = x.GioKetThuc }).ToListAsync();
        return new()
        {
            Id = id,
            LienHeId = contact,
            NhuCauId = r.NhuCauId,
            RowVersion = Convert.ToBase64String(r.RowVersion),
            TenLop = r.TenLopDuKien ?? "",
            MonHocId = r.MonHocId ?? 1,
            MucTieu = r.MucTieu ?? "",
            SoBuoiTuan = r.SoBuoiTuan ?? 1,
            ThoiLuongPhut = r.ThoiLuongPhut ?? 90,
            BatDauHocLuc = r.BatDauHocLuc?.ToOffset(TimeSpan.FromHours(7)).DateTime ?? DateTime.Today.AddDays(2),
            HinhThucHoc = r.HinhThucHoc ?? "TrucTuyen",
            DiaDiem = r.DiaDiem,
            HocPhi = r.HocPhi ?? 150000,
            DonViHocPhi = r.DonViHocPhi ?? "VND/Gio",
            LichRanh = slots.Count == 0 ? [new()] : slots
        };
    }
    public async Task GuiDeXuatAsync(DeXuatLopViewModel m)
    {
        var c = await AcceptedAsync(m.LienHeId, true);
        var u = await current.HienTaiAsync();
        Rules.FeeCheck(m.HocPhi, m.DonViHocPhi);
        Rules.Slots(m.LichRanh);
        Rules.Check(m.LichRanh.Count == m.SoBuoiTuan && m.LichRanh.All(s => (s.GioKetThuc - s.GioBatDau).TotalMinutes == m.ThoiLuongPhut), "Số khung giờ và thời lượng phải khớp điều kiện lớp.");
        Rules.Check(Rules.Local(m.BatDauHocLuc) > DateTimeOffset.UtcNow, "Ngày bắt đầu phải ở tương lai.");
        Rules.Check(m.HinhThucHoc != "TrucTiep" || !string.IsNullOrWhiteSpace(m.DiaDiem), "Nhập địa điểm học trực tiếp.");
        if (m.NhuCauId != null)
            Rules.Check(await db.Set<NhuCauHoc>().AnyAsync(n => n.Id == m.NhuCauId && n.ChuSoHuuId == c.BenHocId && n.LoaiNhuCau == "TimGiaSu" && n.TrangThai == "DangMo"), "Nhu cầu phải thuộc bên học và đang mở.");
        var json = JsonSerializer.Serialize(m.LichRanh);
        var start = Rules.Local(m.BatDauHocLuc);
        var rv = Rules.SqlVersion(m.Id == null ? null : m.RowVersion);
        await using var tx = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.usp_GuiDeXuatLop @LienHeId={m.LienHeId},@GiaSuId={u.Id},@TenLop={m.TenLop},@MonHocId={m.MonHocId},@MucTieu={m.MucTieu},@SoBuoiTuan={m.SoBuoiTuan},@ThoiLuongPhut={m.ThoiLuongPhut},@BatDauHocLuc={start},@HinhThucHoc={m.HinhThucHoc},@HocPhi={m.HocPhi},@DonViHocPhi={m.DonViHocPhi},@KhungGioJson={json},@DiaDiem={m.DiaDiem},@NhuCauId={m.NhuCauId},@YeuCauId={m.Id},@RowVersionCu={rv}");
        var target = await db.Set<YeuCauKetNoiLopHoc>().Where(x => x.LienHeId == m.LienHeId && x.TrangThai == "ChoBenHoc").OrderByDescending(x => x.DeXuatLuc).Select(x => x.Id).FirstAsync();
        notices.Tao(c.BenHocId, "KetNoi", target, "Gia sư gửi đề xuất lớp. Hãy xem lại toàn bộ điều kiện trước khi xác nhận.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
    public async Task<YeuCauKetNoiLopHoc> ChiTietAsync(Guid id)
    {
        var u = await current.HienTaiAsync();
        return await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && (x.GiaSuId == u.Id || x.BenHocId == u.Id)) ?? throw new LoiNghiepVu("Không tìm thấy yêu cầu.", 404);
    }
    public Task<List<KhungGioDeXuat>> KhungAsync(Guid id) => db.Set<KhungGioDeXuat>().Where(x => x.YeuCauKetNoiId == id).ToListAsync();
    public async Task<Guid> ChapNhanAsync(Guid id, string version)
    {
        var u = await current.HienTaiAsync();
        var r = await ChiTietAsync(id);
        Rules.Check(r.BenHocId == u.Id, "Chỉ bên học được chấp nhận đề xuất.", 403);
        if (r.TrangThai == "DaChapNhan")
            return await db.Set<LopHoc>().Where(x => x.YeuCauKetNoiId == id).Select(x => x.Id).SingleAsync();
        await using var tx = await db.Database.BeginTransactionAsync();
        var rv = Rules.Version(version);
        await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.usp_ChapNhanDeXuatTaoLop @YeuCauId={id},@BenHocId={u.Id},@RowVersionCu={rv}");
        var c = await db.Set<LopHoc>().SingleAsync(x => x.YeuCauKetNoiId == id);
        notices.Tao(r.GiaSuId, "KetNoi", c.Id, "Đề xuất được chấp nhận; lớp học đã được tạo.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return c.Id;
    }
    public async Task PhanHoiAsync(Guid id, string action, string version)
    {
        Rules.Check(action is "DaRut" or "TuChoi", "Phản hồi không hợp lệ.");
        var u = await current.HienTaiAsync();
        var r = await db.Set<YeuCauKetNoiLopHoc>().SingleOrDefaultAsync(x => x.Id == id && (x.GiaSuId == u.Id || x.BenHocId == u.Id)) ?? throw new LoiNghiepVu("Không có quyền.", 403);
        Rules.Check(r.TrangThai is "ChoGiaSu" or "ChoBenHoc" && r.HetHanLuc > DateTimeOffset.UtcNow, "Yêu cầu đã hết hiệu lực.", 409);
        var sender = r.TrangThai == "ChoBenHoc" ? r.GiaSuId : r.NguoiGuiId;
        Rules.Check(action == "DaRut" ? sender == u.Id : sender != u.Id, "Không có quyền phản hồi.", 403);
        Rules.Concurrency(db, r, version);
        r.TrangThai = action;
        await using var tx = await db.Database.BeginTransactionAsync();
        notices.Tao(u.Id == r.GiaSuId ? r.BenHocId : r.GiaSuId, "KetNoi", id, "Yêu cầu kết nối: " + Rules.Label(action));
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}


