# Database EduConnect

Bản `dev/cong/fullver0` dùng `00_EduConnect_Full.sql` để cài mới database `KLTN_Full_V2` với 49 bảng. `NguoiThamGia` là view đọc tài khoản và vai trò Identity.

## Cài mới

```powershell
sqlcmd -S lpc:CONGVJP -d master -E -C -I -b -f 65001 -i Database\00_EduConnect_Full.sql
```

Hoặc chạy `Database\Setup.ps1 -Server TEN_SERVER`. Script dừng nếu database đã tồn tại. Đổi server trong `ConnectionStrings:EduConnect` cho đúng máy trước khi chạy ứng dụng. SQL tạo danh mục môn và vai trò. Người dùng tạo tài khoản qua trang đăng ký.

Các file `00_Original_DB02.sql` và script đánh số cũ phục vụ phiên bản trước Identity. Bản cài mới tạo database độc lập. Chuyển dữ liệu cũ sang bản mới cần ánh xạ tài khoản riêng.

## Cấu hình tài khoản

- Đăng ký cấp một vai trò Học sinh, Phụ huynh hoặc Gia sư. Quyền Quản trị không được cấp qua đăng ký.
- Development lưu thư trong `.local-mail`, ngoài thư mục web và Git. Liên kết đặt mật khẩu hết hạn sau 30 phút.
- `Authentication:PublicOrigin` phải đúng địa chỉ người dùng truy cập để liên kết đặt lại mật khẩu hoạt động.
- Development bật `Authentication:DemoPublishProfiles` để gia sư công khai hồ sơ đã hoàn thiện. Khi tắt, hệ thống áp dụng điều kiện duyệt hồ sơ.
- Khi triển khai, cấu hình SMTP qua biến môi trường `Smtp__Host`, `Smtp__Port`, `Smtp__From`, `Smtp__Username`, `Smtp__Password` và dùng HTTPS. Giữ thông tin bí mật ngoài Git.
- Có thể bổ sung danh sách mật khẩu bị lộ qua `Authentication:PasswordBlocklistFile`, mỗi dòng một mật khẩu. Danh sách phổ biến đi kèm hiện có phạm vi hạn chế.

Role SQL `KLTN_App` chứa quyền của ứng dụng. Người vận hành gán role này cho database user khi cấu hình tài khoản SQL chạy ứng dụng. Chuỗi kết nối hiện dùng Windows Authentication.

## Tệp tải lên

Development hiện bật `FileScanning:AllowUnscannedUploads` để thử tải tệp khi chưa dùng ClamAV. Tệp được lưu ở trạng thái `ChoQuet`, vẫn kiểm tra định dạng, dung lượng và quyền truy cập. Ngoài Development, ứng dụng yêu cầu ClamAV và dữ liệu nhận diện tại `tools/clamav`, chỉ cho sử dụng tệp đã quét sạch. Nội dung tải lên nằm trong `uploads`, SQL lưu metadata. Sao lưu cả database và thư mục này.

## Lời mời thành viên lớp

Bên học chính chọn tài khoản đủ điều kiện trong danh sách đã liên kết. Người được mời xác nhận trước, sau đó Gia sư duyệt quyền xem, trao đổi và nộp bài thay. Thành viên bổ sung cần có liên kết còn hiệu lực để truy cập lớp. Người học chính và Gia sư tiếp tục dùng quyền của họ trong lớp.

## Nhu cầu nhiều môn

`NhuCauMonHoc` liên kết nhu cầu với các môn bằng khóa chính ghép `(NhuCauId, MonHocId)` và hai khóa ngoại. Một nhu cầu dùng chung mục tiêu, học phí, lịch. Cột `NhuCauHoc.MonHocId` giữ môn đại diện để tương thích các procedure và dữ liệu cũ, danh sách môn nằm trong bảng liên kết.

Script cài mới 00 đã tích hợp phần nhiều môn. `11_MultiSubjectNeeds.sql` là bản nâng cấp cho database trước khi có bảng `NhuCauMonHoc`.

## Nâng cấp database 38 bảng

Sao lưu database, rồi chạy `13_FullVersionWorkflow.sql` trên database đã có `NhuCauMonHoc` và `ThanhVienLopHoc.DongYThamGiaLuc`.

```powershell
sqlcmd -S lpc:CONGVJP -d KLTN_Full_V2 -E -C -I -b -f 65001 -i Database\13_FullVersionWorkflow.sql
```

Script thêm ba bảng `LuotDangNhuCau`, `LienKetPhuHuynhHocSinh` và `LichSuDieuChinhLich`. Lịch cũ chưa ghi nhận có bản ghi buổi tương ứng với tham dự để trống. Bài tập cũ chưa có buổi vẫn hiện ở mục **Bài tập** với trạng thái **Chưa gắn buổi** để Gia sư chọn lại. Bài mới phải có buổi.

## Quản trị và các chức năng bổ sung

SQL 00 đã tích hợp các nâng cấp 13, 14 và 15. Cài mới chỉ chạy `00_EduConnect_Full.sql`. Database 41 bảng đang có dữ liệu chạy lần lượt `14_DanhGiaBaoCaoQuanTri.sql` và `15_NhatKyAnToan.sql` sau khi sao lưu. Không chạy lại SQL 00 lên database đang sử dụng.

Tạo quản trị đầu tiên từ thư mục gốc dự án, với cùng connection string của web.

```powershell
dotnet run --project tools/ProvisionAdmin/ProvisionAdmin.csproj
```

Công cụ hỏi email, họ tên và mật khẩu, tạo tài khoản mới với sáu quyền quản trị. Đăng ký trên web chỉ cấp vai trò Học sinh, Phụ huynh hoặc Gia sư. Trang **Tài khoản** cho quản trị có quyền phân công hoặc thu hồi từng quyền. Thay quyền thu hồi phiên của tài khoản liên quan.

Tiến độ và phản hồi mở từ chi tiết lớp. Đánh giá công khai mở khi lớp đã kết thúc và có buổi thực tế đã ghi nhận. Báo cáo xuất hiện cạnh hồ sơ, nhu cầu, lớp, tin nhắn và đánh giá. Bằng chứng riêng chỉ người gửi và quản trị được cấp quyền xem.

Sơ đồ ERD 49 bảng nằm tại `docs/database/EduConnect-ERD-48-Bang.drawio`. Thiết kế dữ liệu bổ sung nằm tại `docs/database/ThietKe-DanhGia-BaoCao-QuanTri.md`. Sơ đồ lớp tổng thể một trang, tám nhóm nằm tại `docs/diagrams/ClassDesign/FullSystem/EduConnect-ClassDesign-Full.drawio`.

## Nhật ký an toàn AAA

`NhatKyAnToan` lưu kết quả xác thực, từ chối truy cập và thao tác thay đổi dữ liệu. Nhật ký lưu thời điểm, tài khoản, IP, mã yêu cầu và mã đối tượng khi có. Định danh nhập ở lần đăng nhập sai được băm SHA-256. Nội dung biểu mẫu, mật khẩu, cookie và token không được ghi vào bảng này.

Quản trị có quyền `XemNhatKy` xem tại **Hệ thống → Nhật ký hệ thống**. Lịch sử quản trị cũ nằm ở thẻ **Thao tác quản trị**. Phân trang 20 sự kiện, ngày lọc tính theo UTC+7.

Database đã cài bản 48 bảng chạy `15_NhatKyAnToan.sql` một lần để bổ sung bảng. Cài mới chỉ chạy `00_EduConnect_Full.sql`. Role `KLTN_App` chỉ được đọc và thêm nhật ký, bị từ chối sửa hoặc xóa. Cần chạy ứng dụng bằng database user được gán role này để giới hạn có hiệu lực, tài khoản `sysadmin` có quyền cao hơn.

Nhật ký HTTP được ghi bằng kết nối riêng sau xử lý yêu cầu. Nếu ghi database thất bại, ứng dụng ghi lỗi cùng mã yêu cầu vào hệ thống logging của máy chủ để người vận hành kiểm tra. Cần sao lưu và kiểm tra dung lượng nhật ký trong quá trình vận hành.
