using System.Data;
using System.Security.Cryptography;
using System.Text;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.NhuCau.Services;

public sealed partial class NhuCauService
{
    private const string DeNghiXemLai = "Chủ bài đề nghị xem lại nhu cầu sau khi chỉnh sửa.";

    public async Task<Dictionary<Guid, NhuCauKiemDuyetViewModel>> KiemDuyetAsync()
    {
        var user = await current.HienTaiAsync();
        var owned = db.Set<NhuCauHoc>().Where(x => x.ChuSoHuuId == user.Id);
        var hidden = await db.Set<BienPhapQuanTri>().AsNoTracking()
            .Where(x => x.LoaiDoiTuong == "NhuCauHoc" && x.Loai == "AnNoiDung" && x.GoLuc == null && owned.Any(n => n.Id == x.DoiTuongId))
            .OrderByDescending(x => x.ApDungLuc).ToListAsync();
        var result = hidden.GroupBy(x => x.DoiTuongId).ToDictionary(g => g.Key, g => new NhuCauKiemDuyetViewModel
        {
            DangAn = true, LyDo = g.First().LyDo, ThoiDiem = g.First().ApDungLuc
        });
        var comments = await db.Set<NhatKyQuanTri>().AsNoTracking()
            .Where(x => x.LoaiDoiTuong == "NhuCauHoc" && x.HanhDong == "YeuCauChinhSuaNhuCau" && owned.Any(n => n.Id.ToString() == x.DoiTuongId)
                && !db.Set<BienPhapQuanTri>().Any(b => b.LoaiDoiTuong == "NhuCauHoc" && b.DoiTuongId.ToString() == x.DoiTuongId && b.Loai == "AnNoiDung" && b.GoLuc >= x.ThoiDiem))
            .OrderByDescending(x => x.ThoiDiem).ToListAsync();
        foreach (var group in comments.GroupBy(x => x.DoiTuongId))
        {
            var id = Guid.Parse(group.Key);
            result.TryAdd(id, new NhuCauKiemDuyetViewModel { LyDo = group.First().ChiTiet ?? "", ThoiDiem = group.First().ThoiDiem });
        }
        return result;
    }

    public async Task<bool> DeNghiXemLaiAsync(Guid id, string version)
    {
        var user = await current.HienTaiAsync();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await KhoaTaiKhoanAsync(user.Id);
        await GiaoDichNghiepVu.KiemTraTaiKhoanAsync(db, user.Id);
        var need = await db.Set<NhuCauHoc>().SingleOrDefaultAsync(x => x.Id == id && x.ChuSoHuuId == user.Id)
            ?? throw new LoiNghiepVu("Không tìm thấy nhu cầu của bạn.", 404);
        Rules.Concurrency(db, need, version);
        Rules.Check(need.TrangThai != "BanNhap", "Hãy hoàn thiện và đăng nhu cầu trước khi đề nghị xem lại.", 409);
        var measure = await db.Set<BienPhapQuanTri>().Where(x => x.LoaiDoiTuong == "NhuCauHoc" && x.DoiTuongId == id && x.Loai == "AnNoiDung" && x.GoLuc == null)
            .OrderByDescending(x => x.ApDungLuc).Select(x => new { x.Id, x.ApDungLuc }).FirstOrDefaultAsync();
        Rules.Check(measure != null, "Nhu cầu hiện không có biện pháp ẩn cần xem lại.", 409);
        var lastComment = await db.Set<NhatKyQuanTri>().Where(x => x.LoaiDoiTuong == "NhuCauHoc" && x.DoiTuongId == id.ToString() && x.HanhDong == "YeuCauChinhSuaNhuCau")
            .OrderByDescending(x => x.ThoiDiem).Select(x => new { x.Id, x.ThoiDiem }).FirstOrDefaultAsync();
        var cycle = lastComment != null && lastComment.ThoiDiem > measure!.ApDungLuc ? lastComment.Id : measure!.Id;
        var reviewers = await (from account in db.Users
                               join claim in db.UserClaims on account.Id equals claim.UserId
                               where account.TrangThai == "HoatDong" && claim.ClaimType == "EduConnect.AdminPermission" && claim.ClaimValue == "XuLyBaoCao"
                                   && db.UserRoles.Any(ur => ur.UserId == account.Id && db.Roles.Any(role => role.Id == ur.RoleId && role.Name == "QuanTri"))
                               select account.Id).Distinct().ToListAsync();
        Rules.Check(reviewers.Count > 0, "Hiện chưa có quản trị tiếp nhận. Vui lòng thử lại sau.", 503);
        var events = reviewers.Select(reviewer => new
        {
            Recipient = reviewer,
            Id = new Guid(SHA256.HashData(Encoding.UTF8.GetBytes($"NhuCauXemLai:{id}:{cycle}:{reviewer}")).AsSpan(0, 16))
        }).ToList();
        var eventIds = events.Select(x => x.Id).ToList();
        if (await db.Set<ThongBao>().AnyAsync(x => eventIds.Contains(x.MaSuKien))) return false;
        var notices = new ThongBaoService(db, current);
        foreach (var item in events) notices.Tao(item.Recipient, "NhuCau", id, DeNghiXemLai, item.Id);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return true;
    }
}
