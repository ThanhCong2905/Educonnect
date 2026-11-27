# Kết quả thực thi Unit Test — EduConnect1

Chạy trên `C:\Khóa luận TN\EduConnect1`, thời điểm lập báo cáo `2026-11-25T13:10:30+07:00` (UTC+7).

Đã thực thi **360/360 TC, 389/389 bộ dữ liệu, 79/79 Unit** theo danh mục hiện tại. Kết quả: **359 TC Pass, 1 TC Fail**; theo lượt xUnit: **388 Pass, 1 Fail, 0 Skip, 0 chưa thực thi**. Fail duy nhất thuộc kỳ vọng TC không khớp thiết kế; không ghi nhận nó thành lỗi code ứng dụng.

Không sửa code ứng dụng. Chỉ thêm/sửa project test, cấu hình chạy và kết quả trong `tests/Unit`, `TestResults/Unit`; `bin/obj` được cập nhật khi build.

## Kết quả theo nhóm

| Nhóm | Unit | TC | TC Pass | TC Fail | DL Pass | DL Fail | Chưa thực thi |
|---|---:|---:|---:|---:|---:|---:|---:|
| 01_UC1 | 8 | 42 | 42 | 0 | 44 | 0 | 0 |
| 02_UC2 | 4 | 5 | 5 | 0 | 5 | 0 | 0 |
| 03_UC3 | 2 | 18 | 18 | 0 | 18 | 0 | 0 |
| 04_UC4 | 2 | 3 | 3 | 0 | 3 | 0 | 0 |
| 05_UC5 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
| 06_UC6 | 4 | 14 | 14 | 0 | 15 | 0 | 0 |
| 07_UC7 | 24 | 72 | 72 | 0 | 73 | 0 | 0 |
| 08_Dung_chung | 34 | 205 | 204 | 1 | 229 | 1 | 0 |
| 09_Thanh_phan_khac | 1 | 1 | 1 | 0 | 1 | 0 | 0 |

UC5 không có Unit C# được chọn trong danh mục hiện tại; không tự thêm test workflow hoặc tính một lượt Pass giả cho nhóm rỗng.

## Phân tích ca Fail

UT-354/DL01 — `NhatKyAnToanMapping.Configure` (HT-U55).

- Kỳ vọng đang ghi trong Excel: `MaYeuCau.IsUnicode == false`.
- Quan sát khi chạy: `FindProperty("MaYeuCau").IsUnicode()` trả `null`; assertion thất bại tại chính trường `MaYeuCau`.
- Code ứng dụng: `Data/NhatKyAnToanMapping.cs:22` chỉ có `e.Property(x => x.MaYeuCau).HasMaxLength(128);`.
- Căn cứ: Process HT-U55, bảng 509 trong “Thiết kế xử lý chi tiết (1).docx” cũng chỉ đặt `HasMaxLength(128)`, không có `.IsUnicode(false)` cho trường này.
- Phân loại: lỗi kỳ vọng của kịch bản TC, **chưa có căn cứ kết luận code sai**. Giá trị `null` ở metadata có nghĩa là chưa cấu hình Unicode tường minh, không phải giá trị dữ liệu của mã yêu cầu.
- Cách sửa TC khi hiệu chỉnh bộ kịch bản: bỏ `MaYeuCau` khỏi danh sách phải có `IsUnicode=false`, vẫn kiểm độ dài 128. Không cần sửa code ứng dụng để làm test xanh.

Kết quả vẫn ghi Fail theo nguyên kỳ vọng Excel đã nhận; không sửa oracle để che kết quả. Chi tiết assertion/stack trace nằm trong TRX của nhóm `08_Dung_chung`.

## Cách thực thi và phạm vi chứng minh

- xUnit gọi method thật theo từng `UT-xxx/DLxx`; các helper C# trong project test chỉ dựng fixture, gọi reflection vào method private và assert.
- Truy vấn LINQ và theo dõi entity dùng EF InMemory làm test double kỹ thuật. Model được khởi tạo trong Arrange; Act chạy method được chọn. Không suy ra khả năng dịch SQL, ràng buộc SQL hoặc transaction từ kết quả của fake này.
- Các wrapper tạo lệnh quan hệ dùng interceptor chặn mở kết nối và chặn thực thi command, ghi lại SQL/parameter/token rồi trả dữ liệu fixture. Không kết nối SQL Server hoặc kiểm hiệu lực khóa thật.
- Các ca cần giờ cố định/resource/File.Delete dùng Lib.Harmony 2.4.2, chỉ shim API framework trong testhost. Không patch method ứng dụng. Tắt inlining/ReadyToRun/tiered compilation trong tiến trình test để lời gọi framework đi qua shim. Shim được tháo trong Dispose, test không chạy song song.
- `UT-008` kiểm ba mốc quanh 17:00 UTC; `UT-121..132` kiểm bảng quyết định khóa đăng nhập và biên ±1 tick; `UT-280/281` kiểm null/JSON stream và dispose; `UT-330/331` kiểm thứ tự Delete và cùng tham chiếu IOException, không I/O thật.
- TRX là bằng chứng thực thi. Số ca chạy đủ không đồng nghĩa đã đo 100% line/branch coverage; lượt này không xuất báo cáo coverage.

## Chạy lại

Tại thư mục EduConnect1:

```powershell
.\tests\Unit\Run-UnitTests.ps1
```

Script build ứng dụng rồi project test, chạy theo từng nhóm, ghi đè TRX/log cùng đường dẫn. Exit 1 là kết quả có test Fail; xem `08_Dung_chung` để phân biệt với lỗi build. Build log nằm ở `Build-App.log`, `Build.log`.

Chạy riêng một TC cần shim, ví dụ:

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --no-build --no-restore --filter "Suite=CurrentDesign&TestCaseId=UT-128" --environment COMPlus_JitNoInline=1 --environment DOTNET_ReadyToRun=0 --environment DOTNET_TieredCompilation=0
```

`KetQua.json` và `Doi_chieu_TC.csv` chứa đủ từng TC/DL, input, expected, outcome, vị trí script và đường dẫn TRX. Mỗi thư mục nhóm có `KetQua.md`, `KetQua.json`, `Run.log`, TRX riêng. `Script-SHA256.json` lưu hash script và assembly thực tế được chạy.
