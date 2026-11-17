using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.TimKiem.Services;

public sealed class TimKiemService(AppDbContext db, INguoiDungHienTaiService current, ITimKiemStore? store = null) : ITimKiemService
{
    private readonly ITimKiemStore persistence = store ?? new EfTimKiemStore(db, current);
    public IQueryable<HoSoGiaSu> HoSoCongKhai() => persistence.HoSoCongKhai();
    public async Task<SearchPage> TimAsync(BoLocTimKiemViewModel f)
    {
        var user = await current.HienTaiAsync();
        var page = (await TimKiemAsync(f, user.Id, default)).DuLieu;
        return new()
        {
            Filter = f,
            Items = page.Items,
            Total = page.Total
        };
    }

    public Task<KetQua<Page<UngVienViewModel>>> TimKiemAsync(BoLocTimKiemViewModel boLoc, Guid? maNguoiXem, CancellationToken huy)
        => KetQua<Page<UngVienViewModel>>.ThucHienAsync(async () =>
        {
            var me = await current.KiemTraNguoiThucHienAsync(maNguoiXem, huy);
            return await TimDuLieuAsync(boLoc, me, huy);
        });

    private async Task<Page<UngVienViewModel>> TimDuLieuAsync(BoLocTimKiemViewModel f, NguoiThamGia me, CancellationToken huy)
    {
        Rules.ValidateInput(f);
        Rules.Check(f.Trang is >= 1 and <= 100000, "Trang không hợp lệ.");
        Rules.Check(f.HocPhiToiDa == null || (f.HocPhiToiDa >= 0 && f.HocPhiToiDa <= 9999999999999999.99m && decimal.Round(f.HocPhiToiDa.Value, 2) == f.HocPhiToiDa), "Học phí tối đa không âm, tối đa hai chữ số thập phân.");
        Rules.Check(f.DonViHocPhi is "VND/Gio" or "VND/Buoi", "Chọn đúng đơn vị học phí.");
        Page<UngVienViewModel> rows;
        if (me.LoaiNguoi != "GiaSu") rows = await persistence.TimHoSoAsync(f, huy);
        else
        {
        Rules.Check(f.ThuTrongTuan == null || f.ThuTrongTuan <= 6, "Thứ không hợp lệ.");
        Rules.Check(f.GioBatDau == null || f.GioKetThuc == null || f.GioBatDau < f.GioKetThuc, "Giờ bắt đầu phải trước giờ kết thúc.");
            rows = await persistence.TimNhuCauAsync(f, me.Id, huy);
        }
        return new() { Trang = f.Trang, Items = rows.Items, Total = rows.Total };
    }
    public async Task<UngVienViewModel> ChiTietAsync(Guid id, CancellationToken huy = default)
    {
        var profile = await persistence.ReadProfileAsync(id, huy);
        if (profile != null) return profile;
        return await persistence.ReadNeedAsync(id, huy) ?? throw new LoiNghiepVu("Hồ sơ hoặc nhu cầu đang ẩn, chưa được duyệt hoặc không còn công khai.", 404);
    }
    public Task<UngVienViewModel> XemTruocAsync() => persistence.XemTruocAsync();
    public Task<List<UngVienViewModel>> LienQuanAsync(Guid owner) => persistence.LienQuanAsync(owner);
    public async Task<List<UngVienViewModel>> SoSanhAsync(Guid[] ids)
    {
        var user = await current.HienTaiAsync();
        return (await SoSanhAsync(ids, user.Id, default)).DuLieu.Items;
    }

    public Task<KetQua<SoSanhViewModel>> SoSanhAsync(IReadOnlyList<Guid> maDoiTuong, Guid? maNguoiXem, CancellationToken huy)
        => KetQua<SoSanhViewModel>.ThucHienAsync(async () =>
        {
            await current.KiemTraNguoiThucHienAsync(maNguoiXem, huy);
            var ids = maDoiTuong.Distinct().ToList();
            Rules.Check(ids.Count is >= 2 and <= 3, "Chọn từ 2 đến 3 đối tượng để so sánh.");
            var rows = new List<UngVienViewModel>();
            foreach (var id in ids)
                rows.Add(await ChiTietAsync(id, huy));
            Rules.Check(rows.All(x => x is HoSoGiaSuCongKhaiViewModel) || rows.All(x => x is NhuCauCongKhaiViewModel),
                "Chỉ so sánh các đối tượng cùng loại.");
            return new SoSanhViewModel
            {
                MaDoiTuong = ids,
                HoSo = rows.OfType<HoSoGiaSuCongKhaiViewModel>().ToList(),
                NhuCau = rows.OfType<NhuCauCongKhaiViewModel>().ToList()
            };
        });
    public Task<(List<UngVienViewModel> Items, bool HasNeed)> GoiYAsync() => persistence.GoiYAsync();
}
