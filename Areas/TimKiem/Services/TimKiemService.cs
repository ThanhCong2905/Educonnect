using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;

namespace EduConnect.Mvc.Areas.TimKiem.Services;

public sealed class TimKiemService(AppDbContext db, INguoiDungHienTaiService current, ITimKiemStore? store = null) : ITimKiemService
{
    private readonly ITimKiemStore persistence = store ?? new EfTimKiemStore(db, current);

    public IQueryable<HoSoGiaSu> HoSoCongKhai() => persistence.HoSoCongKhai();

    public async Task<SearchPage> TimAsync(BoLocTimKiemViewModel filter)
    {
        var user = await current.HienTaiAsync();
        var page = (await TimKiemAsync(filter, user.Id, default)).DuLieu;
        return new SearchPage
        {
            Filter = filter,
            Items = page.Items,
            Total = page.Total
        };
    }

    public Task<KetQua<Page<UngVienViewModel>>> TimKiemAsync(
        BoLocTimKiemViewModel filter,
        Guid? viewerId,
        CancellationToken cancellationToken)
        => KetQua<Page<UngVienViewModel>>.ThucHienAsync(async () =>
        {
            var user = await current.KiemTraNguoiThucHienAsync(viewerId, cancellationToken);
            return await TimDuLieuAsync(filter, user, cancellationToken);
        });

    private async Task<Page<UngVienViewModel>> TimDuLieuAsync(
        BoLocTimKiemViewModel filter,
        NguoiThamGia user,
        CancellationToken cancellationToken)
    {
        Rules.ValidateInput(filter);
        Rules.Check(DiaGioiHanhChinh.HopLe(filter.KhuVuc, filter.PhuongXa),
            "Chọn phường / xã thuộc tỉnh / thành phố đã chọn.", field: nameof(filter.PhuongXa));
        Rules.Check(filter.Trang is >= 1 and <= 100000, "Trang không hợp lệ.");
        Rules.Check(filter.HocPhiToiDa == null
            || (filter.HocPhiToiDa >= 0 && filter.HocPhiToiDa <= 9999999999999999m
                && decimal.Truncate(filter.HocPhiToiDa.Value) == filter.HocPhiToiDa),
            "Học phí tối đa phải là số VND nguyên không âm.", field: nameof(filter.HocPhiToiDa));
        Rules.Check(filter.DonViHocPhi is "VND/Gio" or "VND/Buoi", "Chọn đúng đơn vị học phí.");
        Rules.Check(user.LoaiNguoi != "GiaSu" || filter.SapXep is not ("DanhGia" or "HocVien"),
            "Sắp xếp theo đánh giá và số bên học chỉ áp dụng danh sách gia sư.", field: nameof(filter.SapXep));

        Page<UngVienViewModel> result;
        if (user.LoaiNguoi != "GiaSu")
        {
            result = await persistence.TimHoSoAsync(filter, cancellationToken);
        }
        else
        {
            Rules.Check(filter.ThuTrongTuan == null || filter.ThuTrongTuan <= 6, "Thứ không hợp lệ.");
            Rules.Check(filter.GioBatDau == null || filter.GioKetThuc == null || filter.GioBatDau < filter.GioKetThuc,
                "Giờ bắt đầu phải trước giờ kết thúc.");
            result = await persistence.TimNhuCauAsync(filter, user.Id, cancellationToken);
        }

        return new Page<UngVienViewModel>
        {
            Trang = filter.Trang,
            Items = result.Items,
            Total = result.Total
        };
    }

    public async Task<UngVienViewModel> ChiTietAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profile = await persistence.ReadProfileAsync(id, cancellationToken);
        if (profile != null)
            return profile;

        return await persistence.ReadNeedAsync(id, cancellationToken)
            ?? throw new LoiNghiepVu("Hồ sơ hoặc nhu cầu đang ẩn, chưa được duyệt hoặc không còn công khai.", 404);
    }

    public Task<UngVienViewModel> XemTruocAsync() => persistence.XemTruocAsync();

    public async Task<List<UngVienViewModel>> LienQuanAsync(Guid owner, Guid? boQuaId = null)
    {
        var rows = await persistence.LienQuanAsync(owner);
        return boQuaId is null ? rows : rows.Where(row => row.Id != boQuaId).ToList();
    }

    public async Task<List<UngVienViewModel>> SoSanhAsync(Guid[] ids)
    {
        var user = await current.HienTaiAsync();
        return (await SoSanhAsync(ids, user.Id, default)).DuLieu.Items;
    }

    public Task<KetQua<SoSanhViewModel>> SoSanhAsync(
        IReadOnlyList<Guid> objectIds,
        Guid? viewerId,
        CancellationToken cancellationToken)
        => KetQua<SoSanhViewModel>.ThucHienAsync(async () =>
        {
            await current.KiemTraNguoiThucHienAsync(viewerId, cancellationToken);
            var ids = objectIds.Distinct().ToList();
            Rules.Check(ids.Count is >= 2 and <= 3, "Chọn từ 2 đến 3 đối tượng để so sánh.");
            var rows = new List<UngVienViewModel>();
            foreach (var id in ids)
                rows.Add(await ChiTietAsync(id, cancellationToken));
            Rules.Check(rows.All(row => row is HoSoGiaSuCongKhaiViewModel)
                || rows.All(row => row is NhuCauCongKhaiViewModel),
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
