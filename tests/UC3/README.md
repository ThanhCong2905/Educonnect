# Kiểm thử UC3

Chạy trên SQL Server thật, chỉ cho phép database có tên bắt đầu bằng `EduConnect_Verification_`. Không trỏ các kiểm thử vào database đang dùng: fixture tạo lớp, lịch, bài tập và thông báo riêng, đồng thời tạo/xóa trigger thử nghiệm để kiểm tra rollback.

```powershell
dotnet build tests/UC3/ScannerStub/ScannerStub.csproj -o .uc3-build/scanner-stub
$env:EDU_TEST_CONNECTION='Server=TEN_SERVER;Database=EduConnect_Verification_UC3;Integrated Security=True;Encrypt=False'
dotnet run --project tests/UC3/UC3.csproj
```

Khởi tạo database kiểm thử từ file 00, đổi tất cả tên database trong bản sao script trước khi chạy. Database cũ cần cập nhật file 10. Không chạy file 00 lại trên database đã tồn tại.

Bộ kiểm thử xác minh quyền lớp, nộp muộn, chống gửi trùng, phiên bản cũ, lịch thay thế/buổi bù, nộp cạnh tranh chấm, thông báo, nhắc 24 giờ, biên tệp và rollback dữ liệu/tệp. Kết quả và dữ liệu cho kiểm thử trình duyệt được ghi tại `.uc3-build`.

`ScannerStub` chỉ giả lập giao thức exit code của tiến trình quét trong thư mục tạm riêng của kiểm thử; không được dùng làm máy quét trong ứng dụng. Các ca này chứng minh ứng dụng cho phép/từ chối theo kết quả máy quét và dọn tệp cách ly, không chứng minh khả năng phát hiện mã độc của ClamAV thật. Trước vận hành phải kiểm tra thêm với ClamAV và cơ sở chữ ký thực tế.

Sau khi chạy bộ C#, mở web với cùng database kiểm thử ở cổng 5297 và chạy:

```powershell
node tests/UC3/browser.cjs
```

Kiểm thử trình duyệt dùng Edge headless để xác minh quyền hiển thị, AJAX giữ dữ liệu, nút tạo buổi bù trong modal và xung đột sửa lịch giữa hai tab.
