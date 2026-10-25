# Quy ước Unicode

Lỗi ký tự dạng "Lá»›p" là nội dung đã bị giải mã sai, không phải thiếu font. Không sửa bằng đổi font, đổi collation hoặc tự chuyển mã mọi chuỗi khi đọc/ghi.

## Nhập và lưu trong ứng dụng

- Razor trả HTML với charset UTF-8. File nguồn lưu UTF-8 theo .editorconfig.
- Chuỗi .NET giữ Unicode. EF Core dùng tham số; các cột nội dung người dùng dùng nvarchar/nchar.
- varchar chỉ dành cho mã trạng thái kỹ thuật ASCII như GiaSu, DangMo.
- SQL viết trực tiếp phải dùng literal Unicode: N'Tiếng Việt'. Luôn dùng tham số cho dữ liệu người dùng.
- Không dùng Encoding.Default hoặc chuyển UTF-8 qua Windows-1252. Không cần đổi tất cả cột SQL sang UTF-8; nvarchar đã lưu Unicode.

## Nhập file SQL

Chạy qua bộ thực thi thống nhất:

```powershell
.\tools\Invoke-SqlUtf8.ps1 -Script .\Database\05_SubjectNames.sql
```

Lệnh từ chối byte không hợp lệ UTF-8 và file UTF-16, rồi gọi sqlcmd với -f 65001. Database/Setup.ps1 cũng sử dụng lệnh này. Không bỏ tùy chọn encoding khi chạy SQL thủ công. Dữ liệu đã hỏng nhưng vẫn là UTF-8 hợp lệ không thể được phát hiện chỉ bằng kiểm tra encoding file; cần kiểm tra nội dung nguồn.

## Kiểm chứng

```powershell
.\tools\Invoke-SqlUtf8.ps1 -Script .\tests\unicode-roundtrip.sql
```

Kiểm thử đối chiếu chuỗi có dấu và emoji với byte UTF-16 đã biết, nên phát hiện được script bị đọc sai code page. Chỉ sử dụng bảng tạm.

Bộ kiểm thử tích hợp có phép lưu qua EF và đọc bằng DbContext mới, so sánh nguyên văn từng ký tự. Chỉ chạy trên database EduConnect_Verification_*.

Các script sửa dữ liệu cũ chỉ là khôi phục lịch sử. Không chạy chúng mỗi khi người dùng nhập dữ liệu hoặc mỗi khi khởi động ứng dụng.
