using System.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.KetNoi.Services;

public sealed partial class KetNoiService
{
    private async Task KiemTraNguoiThucHienAsync(Guid actor, CancellationToken huy)
    {
        huy.ThrowIfCancellationRequested();
        Rules.Check((await current.HienTaiAsync(huy)).Id == actor, "Không có quyền thực hiện thao tác.", 403);
    }

    private Task<DateTimeOffset> GioMayChuAsync(CancellationToken huy) => db.Database
        .SqlQuery<DateTimeOffset>($"SELECT TODATETIMEOFFSET(SYSUTCDATETIME(), '+00:00') AS Value")
        .SingleAsync(huy);

    public Task<KetQua<Guid>> TaoYeuCauHuyAsync(Guid maLop, string lyDo, byte[] rowVersion,
        Guid maNguoiThucHien, CancellationToken huy) => KetQua<Guid>.ThucHienAsync(async () =>
    {
        await KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
        lyDo = lyDo?.Trim() ?? "";
        Rules.Check(lyDo.Length is > 0 and <= 2000, "Nhập lý do kết thúc tối đa 2000 ký tự.");
        Rules.Check(rowVersion is { Length: 8 }, "Phiên bản không hợp lệ.");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, huy);
        var lop = await db.Set<LopHoc>().FromSqlInterpolated($"SELECT * FROM dbo.LopHoc WITH (UPDLOCK,HOLDLOCK) WHERE Id={maLop}")
            .SingleOrDefaultAsync(huy) ?? throw new LoiNghiepVu("Không tìm thấy lớp.", 404);
        Rules.Check(lop.GiaSuId == maNguoiThucHien || lop.BenHocId == maNguoiThucHien, "Chỉ hai bên chính được yêu cầu kết thúc lớp.", 403);
        Rules.Check(lop.TrangThai == "DangHoatDong", "Lớp không còn hoạt động.", 409);
        Rules.Check(lop.RowVersion.SequenceEqual(rowVersion), "Lớp vừa thay đổi. Hãy tải lại.", 409);
        Rules.Check(!await db.Set<YeuCauHuyKetNoi>().AnyAsync(x => x.LopHocId == maLop && x.TrangThai == "ChoPhanHoi", huy), "Đang có yêu cầu kết thúc chờ phản hồi.", 409);
        var now = await GioMayChuAsync(huy);
        var yc = new YeuCauHuyKetNoi
        {
            Id = Guid.NewGuid(),
            LopHocId = maLop,
            NguoiGuiId = maNguoiThucHien,
            LyDo = lyDo,
            TrangThai = "ChoPhanHoi",
            GuiLuc = now,
            HetHanLuc = now.AddHours(72)
        };
        db.Add(yc);
        notices.Tao(lop.GiaSuId == maNguoiThucHien ? lop.BenHocId : lop.GiaSuId, "KetNoi", maLop,
            "Có yêu cầu kết thúc lớp, cần phản hồi trong 72 giờ.");
        await db.SaveChangesAsync(huy);
        await tx.CommitAsync(huy);
        return yc.Id;
    });

    public Task<KetQua> PhanHoiHuyAsync(Guid maYeuCau, bool dongY, byte[] rowVersion,
        Guid maNguoiThucHien, CancellationToken huy) => KetQua.ThucHienAsync(async () =>
    {
        await KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
        Rules.Check(rowVersion is { Length: 8 }, "Phiên bản không hợp lệ.");
        var maLop = await db.Set<YeuCauHuyKetNoi>().AsNoTracking().Where(x => x.Id == maYeuCau)
            .Select(x => (Guid?)x.LopHocId).SingleOrDefaultAsync(huy) ?? throw new LoiNghiepVu("Không tìm thấy yêu cầu.", 404);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, huy);
        var lop = await db.Set<LopHoc>().FromSqlInterpolated($"SELECT * FROM dbo.LopHoc WITH (UPDLOCK,HOLDLOCK) WHERE Id={maLop}").SingleAsync(huy);
        var yc = await db.Set<YeuCauHuyKetNoi>().FromSqlInterpolated($"SELECT * FROM dbo.YeuCauHuyKetNoi WITH (UPDLOCK,HOLDLOCK) WHERE Id={maYeuCau}").SingleAsync(huy);
        var recipient = yc.NguoiGuiId == lop.GiaSuId ? lop.BenHocId : lop.GiaSuId;
        Rules.Check(maNguoiThucHien == recipient, "Chỉ bên nhận được phản hồi yêu cầu.", 403);
        var now = await GioMayChuAsync(huy);
        Rules.Check(yc.TrangThai == "ChoPhanHoi" && now < yc.HetHanLuc && yc.RowVersion.SequenceEqual(rowVersion), "Yêu cầu đã xử lý, hết hạn hoặc thay đổi.", 409);
        Rules.Check(lop.TrangThai == "DangHoatDong", "Lớp không còn hoạt động.", 409);
        db.Entry(yc).Property(x => x.RowVersion).OriginalValue = rowVersion;
        yc.TrangThai = dongY ? "ChapNhan" : "TuChoi";
        yc.PhanHoiLuc = now;
        yc.NguoiPhanHoiId = maNguoiThucHien;
        if (dongY)
        {
            lop.TrangThai = "DaKetThuc";
            lop.KetThucLuc = now;
            lop.CachKetThuc = "DongY";
        }
        notices.Tao(yc.NguoiGuiId, "KetNoi", maLop, dongY ? "Yêu cầu được chấp nhận. Lớp đã kết thúc." : "Yêu cầu kết thúc bị từ chối. Lớp tiếp tục hoạt động.");
        await db.SaveChangesAsync(huy);
        await tx.CommitAsync(huy);
    });
}
