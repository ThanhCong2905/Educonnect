using System.Data;
using EduConnect.Mvc.Areas.QuanTri.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.QuanTri.Services;

public sealed partial class QuanTriService(AppDbContext db, NguoiDungHienTaiService current, QuyenQuanTriService rights,
    PhienDangNhapService sessions, ThongBaoService notices, PhienCapNhat realtime)
{
    public async Task<QuanTriDashboard> TongQuanAsync()
    {
        Rules.Check((await current.HienTaiAsync()).LoaiNguoi == "QuanTri", "Chỉ quản trị được xem màn hình này.", 403);
        return new QuanTriDashboard
        {
            HoSoChoDuyet = await rights.CoQuyenAsync("DuyetHoSo") ? await db.Set<HoSoGiaSu>().CountAsync(x => x.TrangThaiDuyet == "ChoDuyet") : 0,
            BaoCaoChoXuLy = await rights.CoQuyenAsync("XuLyBaoCao") ? await db.Set<BaoCaoViPham>().CountAsync(x => x.TrangThai != "DaXuLy") : 0,
            TaiKhoanBiKhoa = await rights.CoQuyenAsync("QuanLyTaiKhoan") ? await db.Users.CountAsync(x => x.TrangThai == "BiKhoa") : 0,
            LopTamDung = await rights.CoQuyenAsync("QuanLyTaiKhoan") ? await db.Set<LopHoc>().CountAsync(x => x.TrangThai == "TamDung") : 0,
            GanDay = await rights.CoQuyenAsync("XemNhatKy") ? await (from n in db.Set<NhatKyQuanTri>().AsNoTracking() join u in db.Users on n.NguoiThucHienId equals u.Id orderby n.ThoiDiem descending, n.Id select new NhatKyQuanTriRow { BanGhi = n, HoTen = u.HoTen }).Take(10).ToListAsync() : []
        };
    }

    public async Task<TaiKhoanQuanTriPage> LayTaiKhoanAsync(int trang = 1, string? q = null, string? role = null, string? state = null)
    {
        Rules.Check(await rights.CoQuyenAsync("QuanLyTaiKhoan") || await rights.CoQuyenAsync("QuanLyQuyen"), "Bạn chưa có quyền xem danh sách tài khoản.", 403);
        Rules.Check(trang is >= 1 and <= 100000 && (q?.Length ?? 0) <= 100, "Trang hoặc từ khóa không hợp lệ.");
        Rules.Check(role == null || role is "HocSinh" or "PhuHuynh" or "GiaSu" or "QuanTri", "Vai trò không hợp lệ.");
        Rules.Check(state == null || state is "HoatDong" or "BiKhoa", "Trạng thái tài khoản không hợp lệ.");
        var query = db.Users.AsNoTracking().Where(x => (q == null || x.HoTen.Contains(q) || x.Email!.Contains(q)) && (state == null || x.TrangThai == state));
        if (role != null)
            query = query.Where(x => db.UserRoles.Any(ur => ur.UserId == x.Id && db.Roles.Any(r => r.Id == ur.RoleId && r.Name == role)));
        var rows = await query.OrderBy(x => x.HoTen).ThenBy(x => x.Id).Skip((trang - 1) * 20).Take(20).ToListAsync();
        var ids = rows.Select(x => x.Id).ToList();
        return new TaiKhoanQuanTriPage
        {
            DanhSach = new Page<TaiKhoan> { Items = rows, Trang = trang, KichThuocTrang = 20, Total = await query.CountAsync() },
            VaiTro = await (from ur in db.UserRoles join r in db.Roles on ur.RoleId equals r.Id where ids.Contains(ur.UserId) select new { ur.UserId, r.Name }).ToDictionaryAsync(x => x.UserId, x => x.Name ?? "")
        };
    }

    public async Task<TaiKhoanQuanTriChiTiet> TaiKhoanAsync(Guid id)
    {
        Rules.Check(await rights.CoQuyenAsync("QuanLyTaiKhoan") || await rights.CoQuyenAsync("QuanLyQuyen"), "Bạn chưa có quyền xem tài khoản quản trị.", 403);
        var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id) ?? throw new LoiNghiepVu("Không tìm thấy tài khoản.", 404);
        return new TaiKhoanQuanTriChiTiet
        {
            TaiKhoan = user,
            VaiTro = await VaiTroAsync(id),
            Quyen = await db.UserClaims.Where(x => x.UserId == id && x.ClaimType == QuyenQuanTriService.ClaimType).Select(x => x.ClaimValue!).ToListAsync(),
            BienPhap = await db.Set<BienPhapQuanTri>().AsNoTracking().Where(x => x.LoaiDoiTuong == "TaiKhoan" && x.DoiTuongId == id).OrderByDescending(x => x.ApDungLuc).ToListAsync(),
            YeuCauCho = await db.Set<YeuCauKetNoiLopHoc>().CountAsync(x => (x.GiaSuId == id || x.BenHocId == id) && (x.TrangThai == "ChoGiaSu" || x.TrangThai == "ChoBenHoc")),
            LopDangHoc = await db.Set<LopHoc>().CountAsync(x => (x.GiaSuId == id || x.BenHocId == id) && x.TrangThai == "DangHoatDong")
        };
    }

    public async Task KhoaTaiKhoanAsync(Guid id, string lyDo, string concurrencyStamp)
    {
        var actor = await rights.KiemTraAsync("QuanLyTaiKhoan");
        KiemTraLyDo(lyDo);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await GiaoDichNghiepVu.KhoaQuanTriAsync(db, actor, "QuanLyTaiKhoan");
        var user = await db.Users.SingleOrDefaultAsync(x => x.Id == id) ?? throw new LoiNghiepVu("Không tìm thấy tài khoản.", 404);
        Rules.Check(user.ConcurrencyStamp == concurrencyStamp, "Tài khoản đã thay đổi. Hãy tải lại.", 409);
        await ApDungKhoaAsync(user, actor, lyDo, null);
        rights.GhiNhatKy(actor, "KhoaTaiKhoan", "TaiKhoan", id);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        realtime.ThuHoiNguoi(id);
    }

    internal async Task ApDungKhoaAsync(TaiKhoan user, Guid actor, string reason, Guid? report)
    {
        Rules.Check(user.Id != actor, "Bạn không thể tự khóa tài khoản đang thao tác.");
        if (await VaiTroAsync(user.Id) == "QuanTri")
        {
            Rules.Check(await db.Users.CountAsync(x => x.Id != user.Id && x.TrangThai == "HoatDong" && db.UserRoles.Any(ur => ur.UserId == x.Id && db.Roles.Any(r => r.Id == ur.RoleId && r.Name == "QuanTri"))) > 0, "Cần giữ ít nhất một tài khoản quản trị đang hoạt động.");
            var critical = await db.UserClaims.Where(x => x.UserId == user.Id && x.ClaimType == QuyenQuanTriService.ClaimType && (x.ClaimValue == "QuanLyQuyen" || x.ClaimValue == "QuanLyTaiKhoan")).Select(x => x.ClaimValue).ToListAsync();
            foreach (var permission in critical)
                Rules.Check(await db.UserClaims.AnyAsync(x => x.UserId != user.Id && x.ClaimType == QuyenQuanTriService.ClaimType && x.ClaimValue == permission && db.Users.Any(u => u.Id == x.UserId && u.TrangThai == "HoatDong")), "Tài khoản này đang giữ quyền quản trị cần thiết cuối cùng.");
        }
        var now = await GiaoDichNghiepVu.GioMayChuAsync(db);
        var measure = new BienPhapQuanTri { LoaiDoiTuong = "TaiKhoan", DoiTuongId = user.Id, Loai = "KhoaTaiKhoan", LyDo = reason.Trim(), BaoCaoId = report, NguoiApDungId = actor, ApDungLuc = now };
        db.Add(measure);
        user.TrangThai = "BiKhoa";
        user.SecurityStamp = Guid.NewGuid().ToString();
        user.ConcurrencyStamp = Guid.NewGuid().ToString();
        var pending = await db.Set<YeuCauKetNoiLopHoc>().Where(x => (x.GiaSuId == user.Id || x.BenHocId == user.Id) && (x.TrangThai == "ChoGiaSu" || x.TrangThai == "ChoBenHoc")).ToListAsync();
        foreach (var request in pending)
        {
            request.TrangThai = "DaRut";
            notices.Tao(request.GiaSuId == user.Id ? request.BenHocId : request.GiaSuId, "KetNoi", request.Id, "Yêu cầu kết nối đã hủy do tài khoản đối tác bị khóa.");
        }
        var classes = await db.Set<LopHoc>().Where(x => (x.GiaSuId == user.Id || x.BenHocId == user.Id) && (x.TrangThai == "DangHoatDong" || x.TrangThai == "TamDung")).ToListAsync();
        foreach (var lop in classes)
        {
            lop.TrangThai = "TamDung";
            db.Add(new BienPhapQuanTri { NguonBienPhapId = measure.Id, LoaiDoiTuong = "LopHoc", DoiTuongId = lop.Id, Loai = "TamDungLop", LyDo = "Tạm dừng do tài khoản của một bên bị khóa.", BaoCaoId = report, NguoiApDungId = actor, ApDungLuc = now });
            await notices.TaoChoLopAsync(lop.Id, "LichHoc", lop.Id, "Lớp tạm dừng do tài khoản của một bên bị khóa.");
        }
        var activeSessions = await db.Set<PhienDangNhap>().Where(x => x.TaiKhoanId == user.Id && x.ThuHoiLuc == null).ToListAsync();
        foreach (var session in activeSessions)
        {
            session.ThuHoiLuc = now;
            session.LyDoThuHoi = "Tài khoản bị quản trị khóa";
        }
    }

    public async Task GoBienPhapAsync(Guid id, string reason, string version)
    {
        KiemTraLyDo(reason);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await GiaoDichNghiepVu.KhoaQuanTriAsync(db);
        var measure = await db.Set<BienPhapQuanTri>().SingleOrDefaultAsync(x => x.Id == id) ?? throw new LoiNghiepVu("Không tìm thấy biện pháp.", 404);
        var actor = await rights.KiemTraAsync(measure.Loai == "KhoaTaiKhoan" ? "QuanLyTaiKhoan" : "XuLyBaoCao");
        Rules.Check(measure.GoLuc == null, "Biện pháp đã được gỡ.", 409);
        Rules.Check(measure.NguonBienPhapId == null, "Biện pháp này được gỡ cùng nguyên nhân khóa tài khoản.", 409);
        Rules.Concurrency(db, measure, version);
        var now = await GiaoDichNghiepVu.GioMayChuAsync(db);
        Go(measure, actor, now, reason);
        var children = await db.Set<BienPhapQuanTri>().Where(x => x.NguonBienPhapId == id && x.GoLuc == null).ToListAsync();
        foreach (var child in children)
            Go(child, actor, now, reason);
        await db.SaveChangesAsync();
        if (measure.Loai == "KhoaTaiKhoan")
        {
            var user = await db.Users.SingleAsync(x => x.Id == measure.DoiTuongId);
            if (!await db.Set<BienPhapQuanTri>().AnyAsync(x => x.DoiTuongId == user.Id && x.Loai == "KhoaTaiKhoan" && x.GoLuc == null))
            {
                user.TrangThai = "HoatDong";
                user.ConcurrencyStamp = Guid.NewGuid().ToString();
                notices.Tao(user.Id, "TaiKhoan", user.Id, "Tài khoản đã được mở khóa. Bạn có thể đăng nhập lại.");
            }
            var classIds = children.Select(x => x.DoiTuongId).ToList();
            await db.SaveChangesAsync();
            var classes = await db.Set<LopHoc>().Where(x => classIds.Contains(x.Id) && x.TrangThai == "TamDung").ToListAsync();
            foreach (var lop in classes)
            {
                var blocked = await db.Set<BienPhapQuanTri>().AnyAsync(x => x.DoiTuongId == lop.Id && x.Loai == "TamDungLop" && x.GoLuc == null);
                var disabled = await db.Users.AnyAsync(x => (x.Id == lop.GiaSuId || x.Id == lop.BenHocId) && x.TrangThai != "HoatDong");
                if (!blocked && !disabled)
                {
                    lop.TrangThai = "DangHoatDong";
                    await notices.TaoChoLopAsync(lop.Id, "LichHoc", lop.Id, "Lớp được tiếp tục. Kiểm tra lại lịch học sắp tới.");
                }
            }
        }
        if (measure.Loai == "AnNoiDung")
            notices.Tao(await ChuSoHuuAsync(measure.LoaiDoiTuong, measure.DoiTuongId), "KiemDuyet", measure.BaoCaoId ?? measure.Id, "Biện pháp ẩn nội dung đã được gỡ.");
        rights.GhiNhatKy(actor, "GoBienPhap", measure.LoaiDoiTuong, measure.DoiTuongId);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    private static void Go(BienPhapQuanTri measure, Guid actor, DateTimeOffset now, string reason)
    {
        measure.GoLuc = now;
        measure.NguoiGoId = actor;
        measure.LyDoGo = reason.Trim();
    }

    private async Task<string> VaiTroAsync(Guid id)
        => await (from ur in db.UserRoles join r in db.Roles on ur.RoleId equals r.Id where ur.UserId == id select r.Name!).SingleOrDefaultAsync()
            ?? throw new LoiNghiepVu("Không tìm thấy tài khoản và vai trò hợp lệ.", 404);

    private static void KiemTraLyDo(string reason)
        => Rules.Check(!string.IsNullOrWhiteSpace(reason) && reason.Length <= 2000, "Lý do cần từ 1 đến 2000 ký tự.");
}
