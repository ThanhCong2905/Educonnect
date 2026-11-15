using System.Data;
using EduConnect.Mvc.Areas.QuanTri.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.QuanTri.Services;

public sealed partial class QuanTriService
{
    public async Task<Page<NhuCauHauKiemRow>> LayNhuCauHauKiemAsync(int trang = 1, string? q = null, string? state = null, bool? dangAn = null)
    {
        await rights.KiemTraAsync("XuLyBaoCao");
        Rules.Check(trang is >= 1 and <= 100000 && (q?.Length ?? 0) <= 100, "Trang hoặc từ khóa không hợp lệ.");
        Rules.Check(state == null || state is "DangMo" or "DaDong", "Chọn trạng thái đang mở hoặc đã đóng.");
        q = q?.Trim();
        var query = from need in db.Set<NhuCauHoc>().AsNoTracking()
                    join owner in db.Users on need.ChuSoHuuId equals owner.Id
                    where need.LoaiNhuCau == "TimGiaSu" && need.TrangThai != "BanNhap"
                        && (state == null || need.TrangThai == state)
                        && (q == null || owner.HoTen.Contains(q) || (need.MucTieu != null && need.MucTieu.Contains(q)))
                    select new NhuCauHauKiemRow
                    {
                        NhuCau = need,
                        HoTen = owner.HoTen,
                        DangAn = db.Set<BienPhapQuanTri>().Any(b => b.LoaiDoiTuong == "NhuCauHoc" && b.DoiTuongId == need.Id && b.Loai == "AnNoiDung" && b.GoLuc == null),
                        BaoCaoCho = db.Set<BaoCaoViPham>().Count(b => b.LoaiDoiTuong == "NhuCauHoc" && b.DoiTuongId == need.Id && b.TrangThai != "DaXuLy")
                    };
        if (dangAn.HasValue) query = query.Where(row => row.DangAn == dangAn.Value);
        return new Page<NhuCauHauKiemRow>
        {
            Trang = trang, KichThuocTrang = 20, Total = await query.CountAsync(),
            Items = await query.OrderByDescending(row => row.NhuCau.TaoLuc).ThenBy(row => row.NhuCau.Id)
                .Skip((trang - 1) * 20).Take(20).ToListAsync()
        };
    }

    public async Task<NhuCauHauKiemViewModel> XemNhuCauHauKiemAsync(Guid id)
    {
        await rights.KiemTraAsync("XuLyBaoCao");
        var need = await db.Set<NhuCauHoc>().AsNoTracking().Include(x => x.MonHocs)
            .SingleOrDefaultAsync(x => x.Id == id && x.LoaiNhuCau == "TimGiaSu" && x.TrangThai != "BanNhap")
            ?? throw new LoiNghiepVu("Không tìm thấy nhu cầu đã đăng.", 404);
        var subjects = need.CacMonHocId.ToList();
        return new NhuCauHauKiemViewModel
        {
            NhuCau = need,
            HoTen = await db.Users.Where(x => x.Id == need.ChuSoHuuId).Select(x => x.HoTen).SingleAsync(),
            MonHoc = string.Join(", ", await db.Set<MonHoc>().Where(x => subjects.Contains(x.Id)).OrderBy(x => x.Id).Select(x => x.TenMon).ToListAsync()),
            KhungGio = await db.Set<KhungGioRanh>().AsNoTracking().Where(x => x.NhuCauId == id).OrderBy(x => x.ThuTrongTuan).ThenBy(x => x.GioBatDau).ToListAsync(),
            BienPhap = await db.Set<BienPhapQuanTri>().AsNoTracking().Where(x => x.LoaiDoiTuong == "NhuCauHoc" && x.DoiTuongId == id).OrderByDescending(x => x.ApDungLuc).ToListAsync(),
            BaoCao = await db.Set<BaoCaoViPham>().AsNoTracking().Where(x => x.LoaiDoiTuong == "NhuCauHoc" && x.DoiTuongId == id).OrderByDescending(x => x.GuiLuc).ToListAsync(),
            LichSu = await db.Set<NhatKyQuanTri>().AsNoTracking().Where(x => x.LoaiDoiTuong == "NhuCauHoc" && x.DoiTuongId == id.ToString()).OrderByDescending(x => x.ThoiDiem).Take(20).ToListAsync()
        };
    }

    public async Task XuLyNhuCauAsync(XuLyNhuCauViewModel input)
    {
        Rules.ValidateInput(input);
        var actor = await rights.KiemTraAsync("XuLyBaoCao");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await GiaoDichNghiepVu.KhoaQuanTriAsync(db, actor, "XuLyBaoCao");
        var need = await db.Set<NhuCauHoc>().FromSqlInterpolated($"SELECT * FROM dbo.NhuCauHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id={input.Id}").SingleOrDefaultAsync()
            ?? throw new LoiNghiepVu("Không tìm thấy nhu cầu.", 404);
        Rules.Check(need.TrangThai != "BanNhap" && need.LoaiNhuCau == "TimGiaSu", "Chỉ hậu kiểm nhu cầu đã đăng.", 409);
        Rules.Check(need.ChuSoHuuId != actor, "Nội dung liên quan đến bạn cần quản trị khác xem xét.", 403);
        Rules.Concurrency(db, need, input.RowVersion);
        var now = await GiaoDichNghiepVu.GioMayChuAsync(db);
        if (input.QuyetDinh == "AnNoiDung")
        {
            Rules.Check(!await db.Set<BienPhapQuanTri>().AnyAsync(x => x.LoaiDoiTuong == "NhuCauHoc" && x.DoiTuongId == need.Id && x.Loai == "AnNoiDung" && x.GoLuc == null),
                "Nhu cầu đã được ẩn. Xem lại biện pháp đang áp dụng.", 409);
            db.Add(new BienPhapQuanTri
            {
                LoaiDoiTuong = "NhuCauHoc", DoiTuongId = need.Id, Loai = "AnNoiDung",
                LyDo = input.LyDo.Trim(), NguoiApDungId = actor, ApDungLuc = now
            });
        }
        db.Entry(need).Property(x => x.MucTieu).IsModified = true;
        rights.GhiNhatKy(actor, input.QuyetDinh == "AnNoiDung" ? "AnNhuCau" : "YeuCauChinhSuaNhuCau", "NhuCauHoc", need.Id, chiTiet: input.LyDo.Trim());
        notices.Tao(need.ChuSoHuuId, "NhuCau", need.Id, input.QuyetDinh == "AnNoiDung"
            ? "Nhu cầu đã được ẩn khỏi tìm kiếm. Mở Nhu cầu của tôi để xem lý do và chỉnh sửa."
            : "Quản trị đề nghị bạn chỉnh sửa nhu cầu. Mở Nhu cầu của tôi để xem nội dung cần bổ sung.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
