# Database EduConnect

Ứng dụng dùng database chính `KLTN_Full_V2`, cấu hình tại `ConnectionStrings:EduConnect` trong `appsettings.json`. Bộ cài mới thống nhất là `00_EduConnect_Full.sql`, gồm 50 bảng và các nâng cấp đến script 29. `NguoiThamGia` là view đọc tài khoản và vai trò Identity.

Database đã đến script 21 chạy `22_OptionalLessonComment.sql` để cho phép bỏ trống nhận xét của buổi đã diễn ra. Buổi không diễn ra vẫn phải có lý do. Bản cài mới đã có thay đổi này trong file 00.

Database đã đến script 22 chạy `23_LienKetTaiKhoanHaiChieu.sql` để cho phép Học sinh hoặc Phụ huynh gửi đề nghị liên kết. Chỉ bên nhận được đồng ý hoặc từ chối; mỗi cặp chỉ có một đề nghị hoặc liên kết còn hiệu lực. File 00 đã có ràng buộc mới, không thêm bảng hay cột.

Database đã đến script 23 chạy `24_DongNhuCauDaKetNoi.sql`. Khi tạo lớp thành công, procedure đóng nhu cầu gắn với đề xuất trong cùng giao dịch, ghi `LyDoDong=DaKetNoi` và bỏ nhắc xác nhận. Nếu tạo lớp thất bại, nhu cầu vẫn mở. Script cũng đóng các nhu cầu đang mở đã có lớp đang hoạt động. File 00 đã cập nhật procedure cho cài mới; không thêm bảng hay cột.

Database đã đến script 24 chạy `25_KetThucDeXuatCungNhuCau.sql`. Khi tạo lớp, các yêu cầu/đề xuất còn chờ và chưa hết hạn của cùng nhu cầu chuyển sang `NhuCauDaKetNoi` (Nhu cầu đã được kết nối). Mỗi đề nghị kết thúc có thông báo và sự kiện chờ phát cho hai tài khoản liên quan, lưu cùng giao dịch. Đề nghị đã từ chối/rút/hết hạn và các nhu cầu khác giữ nguyên. Script cũng xử lý các đề nghị còn chờ gắn nhu cầu đã đóng có lớp đang hoạt động; chạy lại không gửi thông báo lặp.

Database đã đến script 25 chạy `26_HoanThienLuongNghiepVu.sql`. Script thêm `LichSuQuyenThanhVien` để lưu người đổi và quyền trước/sau; dữ liệu cũ không được tự tạo lịch sử giả. Procedure lập đề xuất bỏ tham số lời nhắn, giữ nội dung yêu cầu ban đầu. Dùng cùng bản code đã cập nhật và khởi động lại ứng dụng sau nâng cấp.

Máy khác cài mới chỉ chạy file 00 rồi đổi tên SQL Server trong chuỗi kết nối. Database đang sử dụng không chạy lại file 00; chỉ nâng cấp nếu thiếu cấu trúc. Các file nâng cấp riêng được giữ để hỗ trợ database của phiên bản cũ.

Database đã đến script 26 chạy `27_PhamViMonHoc_ChinhSach.sql` trước khi dùng code mới. Script thêm cột cho phạm vi môn dạy, phần Khoa học tự nhiên của nhu cầu và thời điểm, phiên bản chính sách khi đăng ký. Bản cũ giữ nguyên tên môn, lịch sử và minh chứng đã lưu. Tài khoản cũ để trống thông tin đồng ý. Script không thêm bảng.

Database đã đến script 27 chạy `28_HinhThucNhuCau.sql` trước khi dùng code mới. Nhu cầu có thể chọn `TrucTuyen`, `TrucTiep` hoặc `CaHai`. Đề xuất và lớp học vẫn lưu một hình thức đã thống nhất. Bản nâng cấp giữ nguyên dữ liệu hiện có và số bảng.

## Cài mới

Database đã đến script 28 chạy `29_LichSuGhiNhanNhanXetTuyChon.sql`. Lịch sử của buổi đã diễn ra cho phép nhận xét trống; buổi không diễn ra vẫn cần lý do. Trigger lưu phiên bản chuyển nhận xét NULL của bản cũ thành chuỗi rỗng, tránh lỗi khi sửa ghi nhận lần tiếp theo. Script không thêm bảng hoặc cột, không xóa lịch sử. File 00 đã gồm bản sửa này.

```powershell
sqlcmd -S lpc:CONGVJP -d master -E -C -I -b -f 65001 -i Database\00_EduConnect_Full.sql
```

Hoặc chạy `Database\Setup.ps1 -Server TEN_SERVER`. Script dừng nếu database đã tồn tại. Đổi server trong `ConnectionStrings:EduConnect` cho đúng máy trước khi chạy ứng dụng. SQL tạo danh mục môn và vai trò. Người dùng tạo tài khoản qua trang đăng ký.

Các file `00_Original_DB02.sql` và script đánh số cũ phục vụ phiên bản trước Identity. Bản cài mới tạo database độc lập. Chuyển dữ liệu cũ sang bản mới cần ánh xạ tài khoản riêng.

## Cấu hình tài khoản

- Đăng ký cấp một vai trò Học sinh, Phụ huynh hoặc Gia sư. Quyền Quản trị không được cấp qua đăng ký.
- Development lưu thư trong `.local-mail`, ngoài thư mục web và Git. Liên kết đặt mật khẩu hết hạn sau 30 phút.
- `Authentication:PublicOrigin` phải đúng địa chỉ người dùng truy cập để liên kết đặt lại mật khẩu hoạt động.
- Hồ sơ gia sư cần được quản trị duyệt cùng minh chứng trước khi công khai. `Authentication:DemoPublishProfiles` đã tắt trong Development.
- Khi triển khai, cấu hình SMTP qua biến môi trường `Smtp__Host`, `Smtp__Port`, `Smtp__From`, `Smtp__Username`, `Smtp__Password` và dùng HTTPS. Giữ thông tin bí mật ngoài Git.
- Có thể bổ sung danh sách mật khẩu bị lộ qua `Authentication:PasswordBlocklistFile`, mỗi dòng một mật khẩu. Danh sách phổ biến đi kèm hiện có phạm vi hạn chế.

Role SQL `KLTN_App` chứa quyền của ứng dụng. Người vận hành gán role này cho database user khi cấu hình tài khoản SQL chạy ứng dụng. Chuỗi kết nối hiện dùng Windows Authentication.

## Tệp tải lên

### Tệp đính kèm và ClamAV

Ứng dụng gọi `tools/clamav/clamscan.exe` trên Windows hoặc `tools/clamav/clamscan` trên Linux. Đặt bộ nhận diện trong `tools/clamav/database`. Các tệp đã kiểm tra sạch mang trạng thái `Sach`. Tệp cũ còn `ChoQuet` cần được quét và ghi kết quả trước khi sử dụng lại. Dữ liệu tệp nằm trong `uploads`; sao lưu thư mục này cùng CSDL.

Để tải tệp nhanh hơn trên Windows, chạy `.\tools\Start-ClamAv.ps1` với bản ClamAV đã giải nén đầy đủ. Sau đó đặt `$env:FileScanning__ClamdPort = '3310'` trong PowerShell trước khi chạy web. ClamAV giữ bộ nhận diện trong bộ nhớ và chỉ nhận kết nối từ `127.0.0.1`. Khi dịch vụ chưa sẵn sàng, ứng dụng quay về cách quét từng tệp bằng `clamscan`.

Trên máy Windows đã chuẩn bị ClamAV, chạy lệnh sau từ thư mục dự án để cập nhật bộ nhận diện. `freshclam.conf` cần trỏ `DatabaseDirectory` đến thư mục bộ nhận diện của dự án.

```powershell
& .\tools\clamav\freshclam.exe --config-file=.\tools\clamav\freshclam.conf
```

Tệp chương trình và bộ nhận diện cài riêng trên máy chạy web. Môi trường kiểm thử cô lập có thể chủ động đặt `FileScanning__AllowUnscannedUploads=true` nếu cần chạy một ca kiểm thử không dùng máy quét. Ngoài Development, ứng dụng luôn yêu cầu quét thành công.

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

SQL 00 đã tích hợp các nâng cấp đến 21. Cài mới chỉ chạy `00_EduConnect_Full.sql`. Database 41 bảng đang có dữ liệu chạy lần lượt các nâng cấp từ 14 đến 21 sau khi sao lưu. Bản đã đến 20 chỉ chạy `21_SingleSubjectProposalFlow.sql`. Script 21 chuyển nhu cầu cũ về môn đại diện, giới hạn một môn và bổ sung tên người học trong đề xuất lớp. Không chạy lại SQL 00 lên database đang sử dụng.

Tạo quản trị đầu tiên từ thư mục gốc dự án, với cùng connection string của web.

```powershell
dotnet run --project tools/ProvisionAdmin/ProvisionAdmin.csproj
```

Công cụ hỏi email, họ tên và mật khẩu, tạo tài khoản mới với sáu quyền quản trị. Đăng ký trên web chỉ cấp vai trò Học sinh, Phụ huynh hoặc Gia sư. Trang **Tài khoản** cho quản trị có quyền phân công hoặc thu hồi từng quyền. Thay quyền thu hồi phiên của tài khoản liên quan.

Tiến độ nằm trong Tổng quan lớp. Gia sư và Bên học đứng tên lớp mỗi bên được gửi một đánh giá gồm 1–5 sao và nhận xét sau khi lớp kết thúc, có buổi thực tế đã ghi nhận đã học. Phản hồi theo đợt cũ được giữ để xem lịch sử. Báo cáo xuất hiện cạnh hồ sơ, nhu cầu, lớp, tin nhắn và đánh giá. Bằng chứng riêng chỉ người gửi và quản trị được cấp quyền xem.

Sơ đồ ERD 49 bảng nằm tại `docs/database/EduConnect-ERD-49-Bang.drawio`. Thiết kế dữ liệu bổ sung nằm tại `docs/database/ThietKe-DanhGia-BaoCao-QuanTri.md`. Sơ đồ lớp tổng thể một trang, tám nhóm nằm tại `docs/diagrams/ClassDesign/FullSystem/EduConnect-ClassDesign-Full.drawio`.

## Tiền kiểm và hậu kiểm

Nhu cầu hợp lệ được đăng ngay sau khi kiểm tra dữ liệu, hạn mức và trùng bài. Quản trị có quyền `XuLyBaoCao` mở **Kiểm duyệt → Hậu kiểm nhu cầu** để yêu cầu chỉnh sửa hoặc ẩn bài kèm lý do. Chủ bài xem lý do, sửa và đề nghị xem lại. Biện pháp ẩn được giữ đến khi quản trị gỡ.

`19_NeedModeration.sql` cập nhật ba procedure gửi/chấp nhận đề xuất để kiểm tra nhu cầu bị ẩn trong giao dịch. Các bảng `BienPhapQuanTri`, `NhatKyQuanTri`, `ThongBao` và `SuKienChoXuLy` lưu xử lý và thông báo. Database giữ 49 bảng. SQL 00 đã tích hợp phần này để cài mới.

## Nhật ký an toàn AAA

`NhatKyAnToan` lưu kết quả xác thực, từ chối truy cập và thao tác thay đổi dữ liệu. Nhật ký lưu thời điểm, tài khoản, IP, mã yêu cầu và mã đối tượng khi có. Định danh nhập ở lần đăng nhập sai được băm SHA-256. Nội dung biểu mẫu, mật khẩu, cookie và token không được ghi vào bảng này.

Quản trị có quyền `XemNhatKy` xem tại **Hệ thống → Nhật ký hệ thống**. Lịch sử quản trị cũ nằm ở thẻ **Thao tác quản trị**. Phân trang 20 sự kiện, ngày lọc tính theo UTC+7.

Database đã cài bản 48 bảng chạy `15_NhatKyAnToan.sql` một lần để bổ sung bảng. Cài mới chỉ chạy `00_EduConnect_Full.sql`. Role `KLTN_App` chỉ được đọc và thêm nhật ký, bị từ chối sửa hoặc xóa. Cần chạy ứng dụng bằng database user được gán role này để giới hạn có hiệu lực, tài khoản `sysadmin` có quyền cao hơn.

Nhật ký HTTP được ghi bằng kết nối riêng sau xử lý yêu cầu. Nếu ghi database thất bại, ứng dụng ghi lỗi cùng mã yêu cầu vào hệ thống logging của máy chủ để người vận hành kiểm tra. Cần sao lưu và kiểm tra dung lượng nhật ký trong quá trình vận hành.
