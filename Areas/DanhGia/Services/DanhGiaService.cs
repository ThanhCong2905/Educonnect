using System.Data;
using EduConnect.Mvc.Areas.DanhGia.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.DanhGia.Services;

public sealed class DanhGiaService(AppDbContext db, NguoiDungHienTaiService current,
    QuyenLopHocService rights, ThongBaoService notices)
{
    public async Task<Guid> TaoDotAsync(TaoDotPhanHoiViewModel input)
    {
        Rules.ValidateInput(input);
        var me = await current.HienTaiAsync();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var lop = await GiaoDichNghiepVu.KhoaLopAsync(db, input.MaLop);
        Rules.Check(me.Id == lop.GiaSuId || me.Id == lop.BenHocId, "Chỉ hai bên đứng tên lớp được mở đợt phản hồi.", 403);
        Rules.Check(lop.TrangThai == "DangHoatDong", "Lớp cần đang hoạt động để mở đợt phản hồi.", 409);
        var now = await GiaoDichNghiepVu.GioMayChuAsync(db);
        Rules.Check(input.TuNgay != default && input.DenNgay != default && input.TuNgay < input.DenNgay && input.HanGui >= input.DenNgay && input.HanGui > now,
            "Kiểm tra khoảng đánh giá và hạn phản hồi. Hạn phải sau thời điểm hiện tại.");
        Rules.Check(!await db.Set<DotPhanHoi>().AnyAsync(x => x.LopHocId == lop.Id && x.TuNgay == input.TuNgay && x.DenNgay == input.DenNgay),
            "Khoảng thời gian này đã có đợt phản hồi.", 409);
        var dot = new DotPhanHoi
        {
            LopHocId = lop.Id,
            Ten = input.Ten.Trim(),
            TuNgay = input.TuNgay,
            DenNgay = input.DenNgay,
            HanGui = input.HanGui,
            NguoiTaoId = me.Id
        };
        db.Add(dot);
        await notices.TaoChoLopAsync(lop.Id, "PhanHoi", lop.Id, "Lớp có đợt phản hồi mới.", me.Id);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return dot.Id;
    }

    public Task<KetQua<Guid>> GuiAsync(GuiDanhGiaViewModel duLieu, Guid maNguoiThucHien, CancellationToken huy)
        => KetQua<Guid>.ThucHienAsync(async () =>
        {
            Rules.ValidateInput(duLieu);
            var me = await current.KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
            await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, huy);
            var lop = await GiaoDichNghiepVu.KhoaLopAsync(db, duLieu.MaLop, huy);
            await KiemTraGuiAsync(lop, me.Id);
            var dot = await db.Set<DotPhanHoi>().SingleOrDefaultAsync(x => x.Id == duLieu.DotPhanHoiId && x.LopHocId == lop.Id, huy)
                ?? throw new LoiNghiepVu("Đợt phản hồi không thuộc lớp này.", 404);
            var now = await GiaoDichNghiepVu.GioMayChuAsync(db, huy);
            Rules.Check(dot.TrangThai == "DangMo" && now < dot.HanGui, "Đợt phản hồi đã đóng hoặc hết hạn.", 409);
            var phieu = await db.Set<PhieuDanhGia>().SingleOrDefaultAsync(x => x.DotPhanHoiId == dot.Id && x.NguoiGuiId == me.Id, huy);
            if (phieu == null)
            {
                Rules.Check(string.IsNullOrEmpty(duLieu.RowVersion), "Phiếu đã thay đổi. Hãy mở lại đợt phản hồi.", 409);
                phieu = new PhieuDanhGia { LopHocId = lop.Id, DotPhanHoiId = dot.Id, MocDanhGia = dot.Ten, NguoiGuiId = me.Id };
                db.Add(phieu);
            }
            else
            {
                Rules.Concurrency(db, phieu, duLieu.RowVersion);
                db.Add(new LichSuPhanHoi
                {
                    PhieuDanhGiaId = phieu.Id,
                    NoiDung = phieu.NoiDung,
                    MucDoHaiLong = phieu.MucDoHaiLong,
                    NguoiSuaId = me.Id,
                    ThayDoiLuc = now
                });
            }
            phieu.NoiDung = duLieu.NoiDung.Trim();
            phieu.MucDoHaiLong = duLieu.MucDoHaiLong;
            phieu.GuiLuc = now;
            await notices.TaoChoLopAsync(lop.Id, "PhanHoi", lop.Id, "Có phản hồi mới về quá trình học.", me.Id);
            await db.SaveChangesAsync(huy);
            await tx.CommitAsync(huy);
            return phieu.Id;
        });

    public async Task DongDotAsync(Guid id, string version)
    {
        var me = await current.HienTaiAsync();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var dot = await db.Set<DotPhanHoi>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new LoiNghiepVu("Không tìm thấy đợt phản hồi.", 404);
        var lop = await GiaoDichNghiepVu.KhoaLopAsync(db, dot.LopHocId);
        await GiaoDichNghiepVu.KiemTraTaiKhoanAsync(db, me.Id);
        Rules.Check(me.Id == lop.GiaSuId || me.Id == lop.BenHocId, "Chỉ hai bên chính được đóng đợt.", 403);
        var tracked = await db.Set<DotPhanHoi>().SingleAsync(x => x.Id == id);
        Rules.Concurrency(db, tracked, version);
        tracked.TrangThai = "DaDong";
        await notices.TaoChoLopAsync(lop.Id, "PhanHoi", lop.Id, "Đợt phản hồi đã đóng.", me.Id);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task<Guid> QuyetDinhAsync(QuyetDinhPhanHoiViewModel input)
    {
        Rules.ValidateInput(input);
        var me = await current.HienTaiAsync();
        var dot = await db.Set<DotPhanHoi>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == input.DotPhanHoiId)
            ?? throw new LoiNghiepVu("Không tìm thấy đợt phản hồi.", 404);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var lop = await GiaoDichNghiepVu.KhoaLopAsync(db, dot.LopHocId);
        Rules.Check(lop.TrangThai == "DangHoatDong" && (me.Id == lop.GiaSuId || me.Id == lop.BenHocId), "Chỉ hai bên chính của lớp đang học được chọn hướng xử lý.", 403);
        var decision = await db.Set<QuyetDinhSauPhanHoi>().SingleOrDefaultAsync(x => x.DotPhanHoiId == dot.Id && x.NguoiQuyetDinhId == me.Id);
        if (decision == null)
        {
            Rules.Check(string.IsNullOrEmpty(input.RowVersion), "Quyết định đã thay đổi. Hãy mở lại biểu mẫu.", 409);
            decision = new QuyetDinhSauPhanHoi { DotPhanHoiId = dot.Id, NguoiQuyetDinhId = me.Id };
            db.Add(decision);
        }
        else
            Rules.Concurrency(db, decision, input.RowVersion);
        decision.QuyetDinh = input.QuyetDinh;
        decision.GhiChu = input.GhiChu?.Trim();
        decision.TaoLuc = await GiaoDichNghiepVu.GioMayChuAsync(db);
        await notices.TaoChoLopAsync(lop.Id, "PhanHoi", lop.Id, "Đối tác đã chọn hướng xử lý sau phản hồi.", me.Id);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return lop.Id;
    }

    public Task<KetQua<IReadOnlyList<PhieuDanhGia>>> LayTheoLopAsync(Guid maLop, Guid maNguoiThucHien, CancellationToken huy)
        => KetQua<IReadOnlyList<PhieuDanhGia>>.ThucHienAsync(async () =>
        {
            await current.KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
            await rights.KiemTraQuyenXemAsync(maLop);
            return await db.Set<PhieuDanhGia>().AsNoTracking().Where(x => x.LopHocId == maLop).OrderByDescending(x => x.GuiLuc).ToListAsync(huy);
        });

    public async Task<List<LichSuPhanHoi>> LichSuAsync(Guid id)
    {
        var phieu = await db.Set<PhieuDanhGia>().SingleOrDefaultAsync(x => x.Id == id) ?? throw new LoiNghiepVu("Không tìm thấy phiếu.", 404);
        await rights.KiemTraQuyenXemAsync(phieu.LopHocId);
        return await db.Set<LichSuPhanHoi>().AsNoTracking().Where(x => x.PhieuDanhGiaId == id).OrderByDescending(x => x.ThayDoiLuc).ToListAsync();
    }

    private async Task KiemTraGuiAsync(LopHoc lop, Guid actor)
    {
        await GiaoDichNghiepVu.KiemTraTaiKhoanAsync(db, actor);
        await rights.KiemTraQuyenXemAsync(lop.Id, actor);
        Rules.Check(lop.TrangThai == "DangHoatDong", "Lớp hiện chỉ cho xem lịch sử.", 409);
        Rules.Check(actor == lop.GiaSuId || actor == lop.BenHocId || await db.Set<ThanhVienLopHoc>().AnyAsync(x => x.LopHocId == lop.Id && x.TaiKhoanId == actor && x.TrangThaiDuyet == "DaDuyet" && x.RoiLopLuc == null && x.MucQuyen == "ChinhSua"), "Bạn cần quyền chỉnh sửa để gửi phản hồi.", 403);
    }
}
