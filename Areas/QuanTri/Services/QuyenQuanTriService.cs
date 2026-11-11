using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.QuanTri.Services;

public sealed class QuyenQuanTriService(AppDbContext db, NguoiDungHienTaiService current, IHttpContextAccessor http)
{
    private HashSet<string>? granted;
    public const string ClaimType = "EduConnect.AdminPermission";
    public static readonly string[] DanhSach = ["DuyetHoSo", "XuLyBaoCao", "QuanLyTaiKhoan", "QuanLyQuyen", "XemNhatKy", "QuanLyDanhMuc"];

    public async Task<bool> CoQuyenAsync(string quyen)
    {
        var me = await current.HienTaiAsync();
        if (me.LoaiNguoi != "QuanTri")
            return false;
        granted ??= (await db.UserClaims.Where(x => x.UserId == me.Id && x.ClaimType == ClaimType).Select(x => x.ClaimValue!).ToListAsync()).ToHashSet();
        return granted.Contains(quyen);
    }

    public async Task<Guid> KiemTraAsync(string quyen)
    {
        Rules.Check(await CoQuyenAsync(quyen), "Bạn chưa được cấp quyền quản trị cho thao tác này.", 403);
        return (await current.HienTaiAsync()).Id;
    }

    public void GhiNhatKy(Guid actor, string hanhDong, string loai, object id, string ketQua = "ThanhCong", string? chiTiet = null)
        => db.Add(new NhatKyQuanTri
        {
            NguoiThucHienId = actor,
            HanhDong = hanhDong,
            LoaiDoiTuong = loai,
            DoiTuongId = id.ToString() ?? "",
            KetQua = ketQua,
            ChiTiet = chiTiet,
            ThoiDiem = DateTimeOffset.UtcNow,
            MaLienKetYeuCau = http.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString()
        });
}
