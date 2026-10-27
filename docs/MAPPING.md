# Cấu trúc ứng dụng

## Kiến trúc

Thay đổi 27/10/2026: [mapping và luồng hồ sơ theo môn](profile-workflow.md), [sơ đồ lớp/dữ liệu và trạng thái](diagrams/ProfileWorkflow.drawio). Thêm Area `QuanTri`, giữ MVC + Service + EF. Các mô tả hồ sơ không có học phí/nhu cầu nhận lớp gia sư phía dưới là lịch sử trước thay đổi; quy tắc hiện tại nằm trong tài liệu mới.

Ứng dụng gồm bốn Area nghiệp vụ và Area kiểm duyệt, lớp Service và EF Core truy cập SQL Server. Controller xử lý yêu cầu HTTP, Service kiểm tra quyền và điều phối giao dịch, Razor View hiển thị dữ liệu.

## Package

| Package trong sơ đồ | Mã mới |
| --- | --- |
| 1 Xác định nhu cầu học | `Areas/NhuCau`: `NhuCauController`, `NhuCauService`, `HoSoGiaSuController`, `HoSoGiaSuService`, ViewModel riêng và Razor Views |
| 2 Tìm kiếm và lựa chọn đối tượng | `Areas/TimKiem`: `TimKiemController`, `TimKiemService`, `BoLocTimKiemViewModel`, `UngVienViewModel`, tìm kiếm/chi tiết/so sánh |
| 3 Tiếp cận và thỏa thuận | `Areas/KetNoi`: `KetNoiController`, `KetNoiService`, biểu mẫu liên hệ/yêu cầu/đề xuất và Views |
| 4 Thực hiện và theo dõi quá trình học | `Areas/HocTap`: `LopHocController/Service`, `LichHocController/Service`, `BaiTapController/Service`, `TinNhanController/Service` |
| Thành phần dùng chung | `Infrastructure`: người dùng mẫu, quyền lớp, thông báo, lỗi, kiểm tra nghiệp vụ, kho tệp, worker bảo trì |
| Entity và DbContext | `Models/Entities.cs`, `Data/AppDbContext.cs`: một định nghĩa cho mỗi bảng, không tạo Entity trùng ở từng Area |

Route chuẩn: `/{area}/{controller}/{action}/{id?}`; Razor tìm tại `Areas/{area}/Views/{controller}`. `_ViewImports` và `_ViewStart` đặt ở `Areas` để cả bốn Area dùng chung layout. Trang chủ ở `Controllers/HomeController`.

Giữ tên lớp/phương thức tiếng Việt khi phù hợp: `LayDeSuaAsync`, `CapNhatAsync`, `GuiYeuCauAsync`, `GuiDeXuatAsync`, `ChapNhanAsync`, `KiemTraQuyenXemAsync`, `KiemTraQuyenGhiAsync`, `GhiNhanAsync`, `GiaoAsync`, `NopAsync`, `ChamAsync`, `GuiAsync`.

## Quy ước triển khai

| Trong sơ đồ | Bản triển khai và lý do |
| --- | --- |
| `TaiKhoan`, Identity và các role kế thừa | `NguoiThamGia` của DB02; `NguoiDungMauService` thay nguồn danh tính. Không triển khai xác thực ngoài phạm vi |
| `LichDuKien: string` | `List<SlotInput>` và bảng `KhungGioDeXuat`, JSON có cấu trúc khi gọi procedure |
| Lọc `HocPhiTu/HocPhiDen` | Một mức phí và đơn vị trên mỗi nhu cầu; lọc mức tối đa theo đúng đơn vị, không thêm khoảng giá vào Entity |
| Quyền thành viên | `Xem` hoặc `ChinhSua`. Bên học chính có `ChinhSua`; thành viên bổ sung do gia sư duyệt |
| `LaBuoiBu`/`LichGocId` | Schema có các cột buổi bù; luồng hiện tại tạo lịch thường `0/NULL`, chưa có luồng học bù riêng |
| `ThongBaoController` | Các action `ThongBao`, `Doc`, `ChuaDoc` trên `HomeController`; trách nhiệm lưu và lọc theo chủ sở hữu ở `ThongBaoService` |
| `TepController` | Action tải tệp trong `BaiTapController`; kiểm tra nội dung, kho tệp và quyền ở `TepService` |
| `XuLySuKienWorker`/Hub | Chưa triển khai gửi ngoài ứng dụng; sự kiện và thông báo nội bộ lưu trong cùng transaction, event được đánh dấu hoàn tất. Không có tuyên bố email/push đã gửi |

## Transaction và phiên bản

EF Core ánh xạ đủ 24 bảng, đúng kiểu SQL và khóa kép. Tắt SQL OUTPUT trực tiếp trên bảng vì DB có trigger. `rowversion` là concurrency token; biểu mẫu mang phiên bản gốc, Service kiểm tra trước khi lưu; xung đột trả 409 và yêu cầu tải lại.

Procedure gốc dùng cho gửi yêu cầu/đề xuất, chấp nhận tạo lớp, thêm lịch hàng loạt, ghi nhận, nộp và chấm bài. Service kiểm tra vai trò, quyền đối tượng trước khi gọi procedure. SQL tự khóa, kiểm tra ràng buộc và rollback khi thất bại. Không có Generic Repository. `UiResponseFilter` chỉ chuyển cách trình bày phản hồi AJAX (partial, lỗi theo trường, xác nhận); không thay đổi quyền hoặc quy tắc nghiệp vụ.

Các kiểu công khai chỉ chứa trường cho phép; không trả nguyên Entity nhu cầu ra tìm kiếm. View không truy vấn SQL. Nguồn danh tính duy nhất là `NguoiDungMauService`; không nhận sender/owner từ biểu mẫu để quyết định người thực hiện.

`Database/00_Original_DB02.sql` là bộ cài mới đã hợp nhất các cập nhật cho ứng dụng hiện tại. Entity và mapping được duy trì trong `Models/Entities.cs` và `Data/AppDbContext.cs`.
