using System.Data;
using EduConnect.Mvc.Areas.DanhGia.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Areas.DanhGia.Services;

public sealed class DanhGiaService(AppDbContext db, INguoiDungHienTaiService current,
    IQuyenLopHocService rights, IThongBaoService notices, IServiceDataStore? dataStore = null)
{
    private readonly IServiceDataStore data = dataStore ?? new EfServiceDataStore(db);

    public async Task<Guid> TaoDotAsync(TaoDotPhanHoiViewModel input)
    {
        Rules.ValidateInput(input);
        var me = await current.HienTaiAsync();
        await using var tx = await data.BeginAsync(IsolationLevel.Serializable);
        var lop = await data.LockClassAsync( input.MaLop);
        Rules.Check(me.Id == lop.GiaSuId || me.Id == lop.BenHocId, "Chỉ hai bên đứng tên lớp được mở đợt phản hồi.", 403);
        Rules.Check(lop.TrangThai == "DangHoatDong", "Lớp cần đang hoạt động để mở đợt phản hồi.", 409);
        var now = await data.NowAsync();
        Rules.Check(input.TuNgay != default && input.DenNgay != default && input.TuNgay < input.DenNgay && input.HanGui >= input.DenNgay && input.HanGui > now,
            "Kiểm tra khoảng đánh giá và hạn phản hồi. Hạn phải sau thời điểm hiện tại.");
        Rules.Check(!await data.Query<DotPhanHoi>().AnyAsync(x => x.LopHocId == lop.Id && x.TuNgay == input.TuNgay && x.DenNgay == input.DenNgay),
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
        data.Add(dot);
        await notices.TaoChoLopAsync(lop.Id, "PhanHoi", lop.Id, "Lớp có đợt phản hồi mới.", me.Id);
        await data.SaveAsync();
        await tx.CommitAsync();
        return dot.Id;
    }

    public Task<KetQua<Guid>> GuiAsync(GuiDanhGiaViewModel duLieu, Guid maNguoiThucHien, CancellationToken huy)
        => KetQua<Guid>.ThucHienAsync(async () =>
        {
            Rules.ValidateInput(duLieu);
            var me = await current.KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
            await using var tx = await data.BeginAsync(IsolationLevel.Serializable, huy);
            var lop = await data.LockClassAsync( duLieu.MaLop, huy);
            await KiemTraGuiAsync(lop, me.Id);
            var dot = await data.Query<DotPhanHoi>().SingleOrDefaultAsync(x => x.Id == duLieu.DotPhanHoiId && x.LopHocId == lop.Id, huy)
                ?? throw new LoiNghiepVu("Đợt phản hồi không thuộc lớp này.", 404);
            var now = await data.NowAsync(huy);
            Rules.Check(dot.TrangThai == "DangMo" && now < dot.HanGui, "Đợt phản hồi đã đóng hoặc hết hạn.", 409);
            var phieu = await data.Query<PhieuDanhGia>().SingleOrDefaultAsync(x => x.DotPhanHoiId == dot.Id && x.NguoiGuiId == me.Id, huy);
            if (phieu == null)
            {
                Rules.Check(string.IsNullOrEmpty(duLieu.RowVersion), "Phiếu đã thay đổi. Hãy mở lại đợt phản hồi.", 409);
                phieu = new PhieuDanhGia { LopHocId = lop.Id, DotPhanHoiId = dot.Id, MocDanhGia = dot.Ten, NguoiGuiId = me.Id };
                data.Add(phieu);
            }
            else
            {
                data.SetVersion( phieu, duLieu.RowVersion);
                data.Add(new LichSuPhanHoi
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
            await data.SaveAsync(huy);
            await tx.CommitAsync(huy);
            return phieu.Id;
        });

    public async Task DongDotAsync(Guid id, string version)
    {
        var me = await current.HienTaiAsync();
        await using var tx = await data.BeginAsync(IsolationLevel.Serializable);
        var dot = await data.Query<DotPhanHoi>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new LoiNghiepVu("Không tìm thấy đợt phản hồi.", 404);
        var lop = await data.LockClassAsync( dot.LopHocId);
        await data.CheckAccountAsync( me.Id);
        Rules.Check(me.Id == lop.GiaSuId || me.Id == lop.BenHocId, "Chỉ hai bên chính được đóng đợt.", 403);
        var tracked = await data.Query<DotPhanHoi>().SingleAsync(x => x.Id == id);
        data.SetVersion( tracked, version);
        tracked.TrangThai = "DaDong";
        await notices.TaoChoLopAsync(lop.Id, "PhanHoi", lop.Id, "Đợt phản hồi đã đóng.", me.Id);
        await data.SaveAsync();
        await tx.CommitAsync();
    }

    public async Task<Guid> QuyetDinhAsync(QuyetDinhPhanHoiViewModel input)
    {
        Rules.ValidateInput(input);
        var me = await current.HienTaiAsync();
        var dot = await data.Query<DotPhanHoi>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == input.DotPhanHoiId)
            ?? throw new LoiNghiepVu("Không tìm thấy đợt phản hồi.", 404);
        await using var tx = await data.BeginAsync(IsolationLevel.Serializable);
        var lop = await data.LockClassAsync( dot.LopHocId);
        Rules.Check(lop.TrangThai == "DangHoatDong" && (me.Id == lop.GiaSuId || me.Id == lop.BenHocId), "Chỉ hai bên chính của lớp đang học được chọn hướng xử lý.", 403);
        var decision = await data.Query<QuyetDinhSauPhanHoi>().SingleOrDefaultAsync(x => x.DotPhanHoiId == dot.Id && x.NguoiQuyetDinhId == me.Id);
        if (decision == null)
        {
            Rules.Check(string.IsNullOrEmpty(input.RowVersion), "Quyết định đã thay đổi. Hãy mở lại biểu mẫu.", 409);
            decision = new QuyetDinhSauPhanHoi { DotPhanHoiId = dot.Id, NguoiQuyetDinhId = me.Id };
            data.Add(decision);
        }
        else
            data.SetVersion( decision, input.RowVersion);
        decision.QuyetDinh = input.QuyetDinh;
        decision.GhiChu = input.GhiChu?.Trim();
        decision.TaoLuc = await data.NowAsync();
        await notices.TaoChoLopAsync(lop.Id, "PhanHoi", lop.Id, "Đối tác đã chọn hướng xử lý sau phản hồi.", me.Id);
        await data.SaveAsync();
        await tx.CommitAsync();
        return lop.Id;
    }

    public Task<KetQua<IReadOnlyList<PhieuDanhGia>>> LayTheoLopAsync(Guid maLop, Guid maNguoiThucHien, CancellationToken huy)
        => KetQua<IReadOnlyList<PhieuDanhGia>>.ThucHienAsync(async () =>
        {
            await current.KiemTraNguoiThucHienAsync(maNguoiThucHien, huy);
            await rights.KiemTraQuyenXemAsync(maLop);
            return await data.Query<PhieuDanhGia>().AsNoTracking().Where(x => x.LopHocId == maLop).OrderByDescending(x => x.GuiLuc).ToListAsync(huy);
        });

    public async Task<List<LichSuPhanHoi>> LichSuAsync(Guid id)
    {
        var phieu = await data.Query<PhieuDanhGia>().SingleOrDefaultAsync(x => x.Id == id) ?? throw new LoiNghiepVu("Không tìm thấy phiếu.", 404);
        await rights.KiemTraQuyenXemAsync(phieu.LopHocId);
        return await data.Query<LichSuPhanHoi>().AsNoTracking().Where(x => x.PhieuDanhGiaId == id).OrderByDescending(x => x.ThayDoiLuc).ToListAsync();
    }

    private async Task KiemTraGuiAsync(LopHoc lop, Guid actor)
    {
        await data.CheckAccountAsync( actor);
        await rights.KiemTraQuyenXemAsync(lop.Id, actor);
        Rules.Check(lop.TrangThai == "DangHoatDong", "Lớp hiện chỉ cho xem lịch sử.", 409);
        Rules.Check(actor == lop.GiaSuId || actor == lop.BenHocId || await data.Query<ThanhVienLopHoc>().AnyAsync(x => x.LopHocId == lop.Id && x.TaiKhoanId == actor && x.TrangThaiDuyet == "DaDuyet" && x.RoiLopLuc == null && x.MucQuyen == "ChinhSua"), "Bạn cần quyền chỉnh sửa để gửi phản hồi.", 403);
    }
}
