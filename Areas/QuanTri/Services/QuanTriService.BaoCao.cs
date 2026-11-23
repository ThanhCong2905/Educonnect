using System.Data;
using EduConnect.Mvc.Areas.QuanTri.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.QuanTri.Services;

public sealed partial class QuanTriService
{
    public async Task NhanXuLyAsync(Guid id, string version)
    {
        var actor = await rights.KiemTraAsync("XuLyBaoCao");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await GiaoDichNghiepVu.KiemTraQuyenQuanTriAsync(db, actor, "XuLyBaoCao");
        var report = await db.Set<BaoCaoViPham>().FromSqlInterpolated($"SELECT * FROM dbo.BaoCaoViPham WITH (UPDLOCK,HOLDLOCK) WHERE Id={id}").SingleOrDefaultAsync()
            ?? throw new LoiNghiepVu("Không tìm thấy báo cáo.", 404);
        Rules.Check(report.TrangThai == "ChoXuLy", "Báo cáo đã được tiếp nhận hoặc đã xử lý.", 409);
        Rules.Concurrency(db, report, version);
        report.TrangThai = "DangXuLy";
        report.NguoiXuLyId = actor;
        db.Add(new LichSuXuLyBaoCao { BaoCaoId = id, NguoiThucHienId = actor, HanhDong = "NhanXuLy", NoiDung = "Báo cáo đã được tiếp nhận xử lý.", TaoLuc = await GiaoDichNghiepVu.GioMayChuAsync(db) });
        rights.GhiNhatKy(actor, "NhanXuLyBaoCao", "BaoCaoViPham", id);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task PhanHoiNguoiBaoCaoAsync(Guid id, string? text, CancellationToken token)
    {
        var actor = await rights.KiemTraAsync("XuLyBaoCao");
        var message = text?.Trim();
        Rules.Check(!string.IsNullOrEmpty(message) && message.Length <= 2000, "Phản hồi cần từ 1 đến 2.000 ký tự.");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, token);
        await GiaoDichNghiepVu.KiemTraQuyenQuanTriAsync(db, actor, "XuLyBaoCao");
        var report = await db.Set<BaoCaoViPham>()
            .FromSqlInterpolated($"SELECT * FROM dbo.BaoCaoViPham WITH (UPDLOCK,HOLDLOCK) WHERE Id={id}")
            .SingleOrDefaultAsync(token) ?? throw new LoiNghiepVu("Không tìm thấy báo cáo.", 404);
        Rules.Check(report.NguoiXuLyId == actor && report.TrangThai is ("DangXuLy" or "DaXuLy") &&
            report.NguoiBaoCaoId != actor && report.NguoiBiBaoCaoId != actor, "Bạn không phụ trách báo cáo này.", 403);

        var now = await GiaoDichNghiepVu.GioMayChuAsync(db);
        var recent = now.AddMinutes(-1);
        Rules.Check(!await db.Set<LichSuXuLyBaoCao>().AnyAsync(entry => entry.BaoCaoId == id &&
            entry.HanhDong == "PhanHoiNguoiBaoCao" && entry.NguoiThucHienId == actor && entry.TaoLuc >= recent, token),
            "Bạn vừa gửi phản hồi. Hãy đợi một phút trước khi gửi tiếp.", 429);
        db.Add(new LichSuXuLyBaoCao
        {
            BaoCaoId = id, NguoiThucHienId = actor, HanhDong = "PhanHoiNguoiBaoCao",
            NoiDung = message!, TaoLuc = now
        });
        notices.Tao(report.NguoiBaoCaoId, "BaoCao", id, "Quản trị viên đã phản hồi báo cáo của bạn.");
        rights.GhiNhatKy(actor, "PhanHoiNguoiBaoCao", "BaoCaoViPham", id);
        await db.SaveChangesAsync(token);
        await tx.CommitAsync(token);
    }

    public async Task XuLyBaoCaoAsync(XuLyBaoCaoViewModel input)
    {
        Rules.ValidateInput(input);
        var actor = await rights.KiemTraAsync("XuLyBaoCao");
        Guid? lockedUser = null;
        await using (var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable))
        {
            await GiaoDichNghiepVu.KhoaQuanTriAsync(db, actor, "XuLyBaoCao");
            var report = await db.Set<BaoCaoViPham>().SingleOrDefaultAsync(x => x.Id == input.Id) ?? throw new LoiNghiepVu("Không tìm thấy báo cáo.", 404);
            Rules.Check(report.TrangThai == "DangXuLy" && report.NguoiXuLyId == actor, "Bạn cần nhận xử lý báo cáo trước khi kết luận.", 409);
            Rules.Check(report.NguoiBaoCaoId != actor && report.NguoiBiBaoCaoId != actor, "Quản trị liên quan đến báo cáo cần để quản trị khác xử lý.", 403);
            Rules.Concurrency(db, report, input.RowVersion);
            var now = await GiaoDichNghiepVu.GioMayChuAsync(db);
            if (input.KetQua == "XuLy")
            {
                if (input.BienPhap == "KhoaTaiKhoan")
                {
                    await rights.KiemTraAsync("QuanLyTaiKhoan");
                    await GiaoDichNghiepVu.KiemTraQuyenQuanTriAsync(db, actor, "QuanLyTaiKhoan");
                    Rules.Check(report.NguoiBiBaoCaoId != null, "Báo cáo cũ chưa xác định tài khoản bị báo cáo. Cần bổ sung thông tin trước khi khóa.");
                    var user = await db.Users.SingleAsync(x => x.Id == report.NguoiBiBaoCaoId);
                    await ApDungKhoaAsync(user, actor, input.KetLuan, report.Id);
                    lockedUser = user.Id;
                }
                else if (input.BienPhap == "AnNoiDung")
                {
                    Rules.Check(report.LoaiDoiTuong is "HoSoGiaSu" or "NhuCauHoc" or "TinNhan" or "DanhGiaDoiTac", "Loại đối tượng này cần biện pháp phù hợp khác.");
                    db.Add(new BienPhapQuanTri
                    {
                        LoaiDoiTuong = report.LoaiDoiTuong,
                        DoiTuongId = report.DoiTuongId,
                        Loai = "AnNoiDung",
                        LyDo = input.KetLuan.Trim(),
                        BaoCaoId = report.Id,
                        NguoiApDungId = actor,
                        ApDungLuc = now
                    });
                }
            }
            report.TrangThai = "DaXuLy";
            report.KetQua = input.KetQua;
            report.KetLuan = input.KetLuan.Trim();
            report.XuLyLuc = now;
            db.Add(new LichSuXuLyBaoCao
            {
                BaoCaoId = report.Id,
                NguoiThucHienId = actor,
                HanhDong = input.KetQua,
                NoiDung = report.KetLuan,
                TaoLuc = now
            });
            notices.Tao(report.NguoiBaoCaoId, "BaoCao", report.Id, "Báo cáo của bạn đã có kết quả xử lý.");
            if (report.NguoiBiBaoCaoId is Guid recipient)
                notices.Tao(recipient, "KiemDuyet", report.Id, "Nội dung liên quan đến tài khoản của bạn đã được quản trị xem xét.");
            rights.GhiNhatKy(actor, "XuLyBaoCao", "BaoCaoViPham", report.Id, input.KetQua);
            await db.SaveChangesAsync();
            await tx.CommitAsync();
        }
        if (lockedUser is Guid userId)
            realtime.ThuHoiNguoi(userId);
    }

    public async Task<Page<BienPhapQuanTri>> BienPhapAsync(int trang = 1, string? loai = null, bool conHieuLuc = true, Guid? baoCao = null)
    {
        var accountPermission = await rights.CoQuyenAsync("QuanLyTaiKhoan");
        var reportPermission = await rights.CoQuyenAsync("XuLyBaoCao");
        Rules.Check(accountPermission || reportPermission, "Bạn chưa có quyền xem biện pháp quản trị.", 403);
        Rules.Check(trang is >= 1 and <= 100000 && (loai == null || loai is "TaiKhoan" or "HoSoGiaSu" or "NhuCauHoc" or "TinNhan" or "DanhGiaDoiTac" or "LopHoc"), "Bộ lọc không hợp lệ.");
        var query = db.Set<BienPhapQuanTri>().AsNoTracking().Where(x => (loai == null || x.LoaiDoiTuong == loai) && (!conHieuLuc || x.GoLuc == null)
            && (baoCao == null || x.BaoCaoId == baoCao) && (reportPermission || (accountPermission && (x.Loai == "KhoaTaiKhoan" || x.Loai == "TamDungLop"))));
        return new Page<BienPhapQuanTri>
        {
            Trang = trang,
            KichThuocTrang = 20,
            Total = await query.CountAsync(),
            Items = await query.OrderByDescending(x => x.ApDungLuc).ThenBy(x => x.Id).Skip((trang - 1) * 20).Take(20).ToListAsync()
        };
    }

    public async Task<IReadOnlyDictionary<Guid, string>> TenDoiTuongBienPhapAsync(IEnumerable<BienPhapQuanTri> measures)
    {
        Rules.Check(await rights.CoQuyenAsync("XuLyBaoCao") || await rights.CoQuyenAsync("QuanLyTaiKhoan"),
            "Bạn chưa có quyền xem biện pháp quản trị.", 403);
        var grouped = measures.ToLookup(measure => measure.LoaiDoiTuong, measure => measure.DoiTuongId);
        var accounts = grouped["TaiKhoan"].ToArray();
        var classes = grouped["LopHoc"].ToArray();
        var needs = grouped["NhuCauHoc"].ToArray();
        var profiles = grouped["HoSoGiaSu"].ToArray();
        var names = await db.Users.AsNoTracking().Where(u => accounts.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u.HoTen);
        foreach (var item in await db.Set<LopHoc>().AsNoTracking().Where(x => classes.Contains(x.Id)).Select(x => new { x.Id, Name = x.TenLop }).ToListAsync())
            names[item.Id] = item.Name;
        foreach (var item in await db.Set<NhuCauHoc>().AsNoTracking().Where(x => needs.Contains(x.Id) && x.MucTieu != null).Select(x => new { x.Id, Name = x.MucTieu! }).ToListAsync())
            names[item.Id] = item.Name;
        foreach (var item in await (from profile in db.Set<HoSoGiaSu>().AsNoTracking()
                                   join user in db.Users on profile.GiaSuId equals user.Id
                                   where profiles.Contains(profile.Id)
                                   select new { profile.Id, Name = user.HoTen }).ToListAsync())
            names[item.Id] = item.Name;
        return names;
    }

    public async Task<List<BienPhapQuanTri>> CuaToiAsync()
    {
        var me = await current.HienTaiAsync();
        var query = from b in db.Set<BienPhapQuanTri>().AsNoTracking()
                    where (b.LoaiDoiTuong == "TaiKhoan" && b.DoiTuongId == me.Id)
                      || (b.LoaiDoiTuong == "NhuCauHoc" && db.Set<NhuCauHoc>().Any(n => n.Id == b.DoiTuongId && n.ChuSoHuuId == me.Id))
                      || db.Set<BaoCaoViPham>().Any(r => r.Id == b.BaoCaoId && r.NguoiBiBaoCaoId == me.Id)
                    orderby b.ApDungLuc descending
                    select b;
        return await query.ToListAsync();
    }

    public async Task<KetQuaKiemDuyetViewModel> KetQuaLienQuanAsync(Guid id)
    {
        var me = await current.HienTaiAsync();
        var report = await db.Set<BaoCaoViPham>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.NguoiBiBaoCaoId == me.Id && x.KetLuan != null)
            ?? throw new LoiNghiepVu("Không tìm thấy kết quả xử lý.", 404);
        return new KetQuaKiemDuyetViewModel { BaoCaoId = report.Id, KetLuan = report.KetLuan ?? "", KetQua = report.KetQua, TrangThai = report.TrangThai, RowVersion = Convert.ToBase64String(report.RowVersion) };
    }

    private async Task<Guid> ChuSoHuuAsync(string type, Guid id)
        => type switch
        {
            "TaiKhoan" => id,
            "HoSoGiaSu" => await db.Set<HoSoGiaSu>().Where(x => x.Id == id).Select(x => x.GiaSuId).SingleAsync(),
            "NhuCauHoc" => await db.Set<NhuCauHoc>().Where(x => x.Id == id).Select(x => x.ChuSoHuuId).SingleAsync(),
            "TinNhan" => await db.Set<TinNhan>().IgnoreQueryFilters().Where(x => x.Id == id).Select(x => x.NguoiGuiId).SingleAsync(),
            "DanhGiaDoiTac" => await db.Set<DanhGiaDoiTac>().Where(x => x.Id == id).Select(x => x.NguoiGuiId).SingleAsync(),
            _ => throw new LoiNghiepVu("Loại đối tượng không hợp lệ.")
        };
}
