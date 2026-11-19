# Biên bản thực thi Unit Test — EduConnect1

Ngày chạy: 18/11/2026. Project riêng: `tests/Unit/EduConnect.UnitTests.csproj`, .NET 10 / xUnit. Test được chạy trực tiếp trên EduConnect1; không thực thi project MT1 trong lần chạy được báo cáo này.

**Kết quả: 135 lượt dữ liệu xUnit Pass, 0 Fail, 0 Skip.**

| Nhóm | Số lượt | Pass | Fail |
|---|---:|---:|---:|
| [Quy tắc chung](01_Quy_tac_chung/KetQua.md) | 49 | 49 | 0 |
| [Dữ liệu đầu vào và kết quả nghiệp vụ](02_Du_lieu_dau_vao/KetQua.md) | 19 | 19 | 0 |
| [Controller và middleware](03_Controller_Middleware/KetQua.md) | 14 | 14 | 0 |
| [Lịch học](04_Lich_hoc/KetQua.md) | 12 | 12 | 0 |
| [Địa giới và danh mục](05_Dia_gioi_Danh_muc/KetQua.md) | 7 | 7 | 0 |
| [Khách và xác thực](06_Khach_Xac_thuc/KetQua.md) | 4 | 4 | 0 |
| [Biên đầu vào và mật khẩu](07_Bien_dau_vao_Mat_khau/KetQua.md) | 30 | 30 | 0 |

Các nhóm 01–06 có 105 lượt từ các kiểm thử độc lập đã có; nhóm 07 chuyển 30 kiểm tra biên đầu vào/mật khẩu của chương trình console thành xUnit để từng lượt có kết quả TRX. Tổng 135 là số lượt dữ liệu xUnit, không phải 135 Unit hoặc 135 mã UT trong Excel.

## Phạm vi đã kiểm

- Rules: khung giờ, học phí, rowVersion, một số ràng buộc đầu vào. Giữ nguyên assertion theo script; không thay kỳ vọng để làm Pass.
- Dữ liệu hồ sơ, danh mục lớp và kiểm tra thuộc tính; KetQua giữ nguyên ngoại lệ, trường lỗi và status.
- Controller: ModelState không hợp lệ dừng trước dependency; token đã hủy dừng sớm. Middleware được gọi với RequestDelegate giả trong bộ nhớ để kiểm status, mã hóa lỗi và che lỗi nội bộ.
- Lịch tuần: múi giờ, ngày đầu tuần, giao tháng/năm, qua đêm, chồng lịch, trạng thái và biên ngày.
- Danh mục địa giới/môn: tính đầy đủ theo assertion, không trùng, phường thuộc tỉnh, thứ tự môn.
- Người dùng khách: nhánh không đăng nhập dừng trước DB; kiểm số lượng so sánh không hợp lệ; metadata AllowAnonymous của action. Kiểm attribute không chứng minh toàn bộ HTTP pipeline bảo mật.
- Nhóm 07: các biên bộ lọc, nhu cầu, liên hệ, bài tập/chấm bài, lý do sửa lịch; độ dài mật khẩu tính bằng Unicode scalar, kể cả emoji.

Các test chạy trong bộ nhớ. Không dùng SQL Server, không khởi động ứng dụng web, không gửi email và không chạy worker thật. Những test truyền dependency null chỉ chứng minh nhánh dừng sớm, không chứng minh nhánh thành công sử dụng dependency.

## Đối chiếu bộ 719 TC trong Excel

**Chưa thể kết luận đã chạy 719 TC / 1.342 bộ dữ liệu của Excel.** Bộ 135 lượt này chưa có ánh xạ được xác minh đầy đủ tới từng mã mới, từng DL, nhánh và kỳ vọng. File `Doi_chieu_719_TC.csv` ghi trạng thái riêng cho 719 mã; không tự ghép verdict do mã trùng bộ cũ. Workbook kịch bản và kỳ vọng không bị sửa trong lần thực thi này.

Ví dụ, UT-001 yêu cầu controller thật và service/người hiện tại fake cho đường mở biểu mẫu; các lớp NhuCauService và NguoiDungHienTaiService ở nhánh hiện tại là sealed và controller nhận concrete class. Chưa có script cho đúng cách cô lập của ca này trong project mới. Đây là phần hiện thực test còn thiếu, không phải một assertion đã chạy và Fail. Nếu code thiếu một method đã chốt trong thiết kế thì cần ghi lỗi đối chiếu thiết kế riêng; không đồng nhất với việc chưa viết script.

## Cách chạy lại

Tại thư mục EduConnect1:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tests/Unit/Run-UnitTests.ps1
```

Chạy riêng một nhóm:

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter "FullyQualifiedName~ControllerGuardTests" --logger "trx;LogFileName=Controller.trx" --results-directory TestResults/Unit/03_Controller_Middleware
```

Mỗi thư mục nhóm có TRX gốc, `Run.log`, `KetQua.md` và `KetQua.json`. TRX cung cấp verdict và tên từng lượt; script tại `tests/Unit/*.cs` chứa assertion cụ thể. Build.log ghi lần build của chính project mới. Lần chạy thử trước trong môi trường giới hạn từng bị timeout kết nối VSTest/testhost; không tính timeout hạ tầng đó thành lỗi nghiệp vụ. Lần chạy chính có đầy đủ TRX và kết thúc thành công.
