using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Models;
namespace EduConnect.Mvc.Areas.NhuCau.Services;

public static class HoSoCongKhaiMapper
{
    public static HoSoGiaSuCongKhaiViewModel Map(
        HoSoGiaSu h,
        string hoTen,
        string? anhDaiDien,
        List<MonDayCongKhai> monDay,
        decimal? rating = null,
        int reviewCount = 0,
        int learnerCount = 0)
    {
            return new HoSoGiaSuCongKhaiViewModel
            {
                Id = h.Id,
                ChuSoHuuId = h.GiaSuId,
                LoaiNguoi = "GiaSu",
                HoTen = hoTen,
                AnhDaiDienUrl = anhDaiDien == null ? null : $"/TaiKhoan/Anh/{h.GiaSuId}",
                DiemDanhGia = rating,
                SoDanhGia = reviewCount,
                SoBenHoc = learnerCount,
                TrinhDo = h.TrinhDo,
                ChuyenMon = h.ChuyenMon,
                KinhNghiem = h.KinhNghiem,
                MoTa = h.MoTa,
                KhuVuc = h.KhuVuc,
                KhuVucGiaSu = h.KhuVuc,
                DayTrucTuyen = h.DayTrucTuyen,
                DayTrucTiep = h.DayTrucTiep,
                HinhThucHoc = h.DayTrucTuyen && h.DayTrucTiep ? "CaHai" : h.DayTrucTuyen ? "TrucTuyen" : "TrucTiep",
                TaoLuc = h.DuyetLuc ?? DateTimeOffset.MinValue,
                MonTheoHoSo = monDay,
                MonDay = monDay.Select(m => m.TenMon).ToList()
            };
    }
}
