using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.ComponentModel.DataAnnotations;
namespace EduConnect.Mvc.Infrastructure;

public sealed class VietnameseValidationMetadataProvider : IValidationMetadataProvider, IDisplayMetadataProvider
{
    static readonly Dictionary<string, string> Labels = new() { ["HoTen"] = "Họ tên", ["TrinhDo"] = "Trình độ", ["ChuyenMon"] = "Chuyên môn", ["KinhNghiem"] = "Kinh nghiệm", ["MoTa"] = "Giới thiệu", ["KhuVuc"] = "Khu vực", ["PhamViDay"] = "Phạm vi dạy", ["MonHocId"] = "Môn học", ["KhoiLop"] = "Khối lớp", ["MucTieu"] = "Mục tiêu", ["SoBuoiTuan"] = "Số buổi mỗi tuần", ["HinhThucHoc"] = "Hình thức học", ["TenNguoiHoc"] = "Tên người học", ["TuoiNguoiHoc"] = "Tuổi người học", ["TenMonKhac"] = "Tên môn học khác", ["TieuDe"] = "Tiêu đề", ["TenLop"] = "Tên lớp", ["NoiDung"] = "Nội dung", ["NhanXet"] = "Nhận xét", ["LoiNhan"] = "Lời nhắn", ["Diem"] = "Điểm", ["ThoiLuongPhut"] = "Thời lượng buổi học", ["HocPhi"] = "Học phí", ["HocPhiDuKien"] = "Học phí dự kiến", ["GioBatDau"] = "Giờ bắt đầu", ["GioKetThuc"] = "Giờ kết thúc" };
    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        if (context.Key.Name is string name && Labels.TryGetValue(name, out var label))
            context.DisplayMetadata.DisplayName = () => label;
    }
    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
        foreach (var attribute in context.Attributes.OfType<ValidationAttribute>())
        {
            if (attribute.ErrorMessage != null || attribute.ErrorMessageResourceType != null)
                continue;
            attribute.ErrorMessage = attribute switch
            {
                RequiredAttribute => "{0} không được để trống.",
                RangeAttribute => "{0} phải từ {1} đến {2}.",
                StringLengthAttribute => "{0} không được vượt quá {1} ký tự.",
                RegularExpressionAttribute => "{0} chưa đúng định dạng.",
                EmailAddressAttribute => "{0} chưa phải địa chỉ email hợp lệ.",
                _ => "{0} chưa hợp lệ."
            };
        }
    }
}
