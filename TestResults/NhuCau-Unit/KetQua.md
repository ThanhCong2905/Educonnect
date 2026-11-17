# Kết quả unit test — Xác định nhu cầu học

Ngày thực hiện: 14/11/2026 (Asia/Saigon).

**Tổng 20 ca: 16 Pass, 3 Fail, 1 Skip/Blocked. Có 19 ca thực thi; UT-020 chưa thực thi.**

## Phạm vi và đổi mã

- Giữ UT-001 đến UT-019; loại UT-292 khỏi đợt chạy; đổi UT-293 thành UT-020.
- UT-020 cũ của bộ tổng thể thuộc tìm kiếm không nằm trong đợt này. Không đổi mã toàn bộ workbook để tránh trùng mã; danh sách 20 ca hiện hành được lưu riêng trong báo cáo này.
- Giữ nguyên 7 test controller đã có; bổ sung 12 test service và một test khai báo Skip có lý do. Không sửa thông báo ứng dụng hoặc nới assertion để làm test Pass.
- Nguồn chạy: `C:\Khóa luận TN\Educonnect\tests\Unit\EduConnect.UnitTests.csproj`.

## Kết quả từng ca

| Mã | Tình huống | Kết quả | Bằng chứng/diễn giải |
|---|---|---|---|
| UT-001 | Mở biểu mẫu tạo | Pass | View Tao, model mới và danh mục môn đúng; không gọi tạo/lưu. |
| UT-002 | ModelState không hợp lệ | Pass | ModelState lỗi: giữ dữ liệu, không gọi service tạo. |
| UT-003 | Tạo nhu cầu — đăng công khai | Pass | Service thật tạo đúng chủ/môn/lịch/DangMo; thêm một nhu cầu, commit một giao dịch. |
| UT-004 | Tạo nhu cầu — lưu nháp | Pass | Service thật tạo BanNhap đúng dữ liệu; không ghi lượt đăng. |
| UT-005 | Môn không tồn tại | Pass | Môn không tồn tại: lỗi 400, không thêm hoặc lưu nhu cầu/lịch. |
| UT-006 | Môn ngừng sử dụng | Pass | Môn ngừng sử dụng: lỗi 400, không thêm hoặc lưu. |
| UT-007 | Lịch rảnh sai khoảng giờ | Pass | Lịch 19:00–18:00: lỗi 400, không ghi. |
| UT-008 | Tạo công khai thành công | Fail | Redirect và tham số service đúng; thông báo mong đợi “Tạo nhu cầu thành công”, thực tế “Đã lưu thay đổi.”. |
| UT-009 | Tạo nháp thành công | Fail | Redirect và tham số service đúng; mong đợi “Đã lưu nháp”, thực tế “Đã lưu thay đổi.”. |
| UT-010 | Service từ chối nghiệp vụ | Pass | Trả View Tao, giữ dữ liệu, có lỗi ModelState và không có thông báo thành công. Chỉ xác nhận thông điệp tổng quát, không khẳng định gắn lỗi vào từng trường. |
| UT-011 | Nạp dữ liệu của chủ sở hữu | Pass | Nạp đúng Id, RowVersion, hai môn, dữ liệu chính và hai lịch; không ghi. |
| UT-012 | Không đọc nhu cầu người khác | Pass | Repository nhận đúng owner U1, không trả dữ liệu nhu cầu U2; lỗi 404; không đọc lịch hoặc ghi. |
| UT-013 | ModelState sửa không hợp lệ | Pass | ModelState sửa lỗi: giữ Id/RV/dữ liệu, không gọi cập nhật. |
| UT-014 | Thay lịch và mở bản nháp | Pass | BanNhap → DangMo; truyền đúng RV1; gọi xóa lịch cũ trước khi thêm đúng hai lịch mới; commit một lần. |
| UT-015 | Sai chủ sở hữu | Pass | Sai chủ: lỗi 403; không Save hoặc thay lịch; snapshot ban đầu giữ nguyên. |
| UT-016 | Phiên bản cũ | Pass | Truyền đúng RV0 xuống store; khi store mô phỏng DbUpdateConcurrencyException, service trả 409; không xóa lịch/commit. Không kiểm chứng concurrency thật của SQL. |
| UT-017 | Môn không hợp lệ | Pass | Môn ngừng sử dụng khi sửa: lỗi 400; không ghi đè. Biến thể được chọn theo “không tồn tại hoặc ngừng sử dụng” trong bảng gốc. |
| UT-018 | Lịch rảnh không hợp lệ | Pass | Giờ bắt đầu bằng kết thúc: lỗi 400; không ghi đè. |
| UT-019 | Cập nhật thành công | Fail | Redirect và tham số Id/vm/RV/U1/ct đúng; mong đợi “Cập nhật nhu cầu thành công”, thực tế “Đã lưu thay đổi.”. |
| UT-020 | Không bỏ sót gợi ý ban đầu | Skip / Blocked | UT-293 đổi thành UT-020 trong phạm vi bộ 20 ca này. Skip/Blocked: chưa có luồng điều phối sau tạo nhận N1 và gọi nguồn gợi ý trả H1/H2. Không thay bằng test GoiYRules. |

## Thay đổi mã nguồn để chạy unit cô lập

- Thêm INhuCauStore và EfNhuCauStore: adapter chứa truy vấn, transaction, khóa tài khoản, SaveChanges, xóa lịch và ghi dữ liệu.
- NhuCauService nhận nguồn người dùng qua INguoiDungHienTaiService và store tùy chọn. Khi ứng dụng chạy bình thường, mặc định dùng EfNhuCauStore với DbContext/catalog hiện có.
- Giữ kiểm tra, ánh xạ và điều phối tạo/cập nhật/nạp sửa trong NhuCauService thật. Test không mock phương thức đang kiểm thử.
- Không thay truy vấn kiểm môn phổ thông bằng so sánh chuỗi trong bộ nhớ, để giữ ngữ nghĩa collation SQL của adapter.
- Mỗi ca tạo fixture riêng: mock strict của store/current, transaction mock, token riêng. DbContext không cấu hình provider; truy cập EF ngoài adapter sẽ không thể vô tình kết nối SQL.
- Các ca tạo/cập nhật vẫn có ba lần Save trong một transaction như mã gốc. “Lưu một lần” được xác nhận là thêm một nhu cầu và commit một giao dịch, KHÔNG phải SaveChanges đúng một lần. Nếu yêu cầu chính thức bắt buộc đúng một SaveChanges thì cần sửa tiêu chí/thiết kế trước khi dùng kết quả này cho yêu cầu đó.

## Ba ca Fail

UT-008, UT-009 và UT-019 thất bại tại assertion nội dung TempData["Success"]. NhuCauController.LuuBieuMauAsync vẫn gọi Success() mặc định, nên ba nhánh đều trả “Đã lưu thay đổi.”. Các assertion về redirect, tham số và số lần gọi service trước đó đều đạt.

## UT-020 chưa thực thi

Bảng gốc yêu cầu đơn vị điều phối sau khi tạo N1 gọi nguồn gợi ý và trả đủ H1/H2. Hiện action tạo chỉ redirect Index; INhuCauService chưa có hợp đồng gợi ý sau tạo. GoiYAsync ở phần tìm kiếm xử lý một luồng khác. Test được khai báo Skip kèm lý do rõ ràng, không đánh Pass và không kết luận Fail nghiệp vụ từ việc chưa có đơn vị kiểm tra phù hợp.

## Giới hạn của kết quả

- Pass của service xác nhận xử lý với hợp đồng store đã giả lập; không xác nhận SQL, trigger, transaction rollback hoặc khóa đồng thời thực sự.
- UT-016 kiểm tra service chuyển đúng RV và xử lý lỗi concurrency từ adapter; kiểm chứng EF/SQL phát hiện RV cũ phải thực hiện Integration.
- UT-012/015 kiểm tham số owner và phản ứng khi store không tìm thấy nhu cầu thuộc owner đó; điều kiện WHERE thực tế của adapter cần Integration riêng.
- Kết quả dùng mã nguồn SAU khi tách adapter; chưa thực hiện lại Integration với adapter mới.
- Lần build tại bin mặc định bị DLL ứng dụng đang chạy khóa. Lần chạy tạo TRX thành công dùng thư mục build riêng; không dừng ứng dụng đang mở.
- dotnet test trả exit code 1 vì ba test Fail; không phải build thất bại của lần chạy cuối.

## Chạy lại trong VS Code

Mở thư mục Educonnect, mở Terminal PowerShell rồi chạy:

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --no-restore -p:UseAppHost=false "-p:BaseOutputPath=C:\Users\NGUYEN THI YEN\Documents\Codex\2026-11-13\un\work\uc1-bin\" --filter FullyQualifiedName~EduConnect.UnitTests --results-directory "C:\Users\NGUYEN THI YEN\Documents\Codex\2026-11-13\un\outputs\UC1-Unit-20" --logger "trx;LogFileName=UC1-Unit-20.trx"
```

Chạy riêng UT-016: thay bộ lọc bằng `--filter TestCaseId=UT-016` và đổi tên TRX để giữ minh chứng của cả bộ.

## Minh chứng

- [TRX gốc](UC1-Unit-20.trx)
- [Mã nguồn và cấu hình test](Ma_nguon_UC1_Unit_20.zip)
- [Danh sách và kết quả máy đọc được](KetQua.json)

Các test không truy cập database ứng dụng. Không có UT-292 hay UT-293 trong danh sách test được phát hiện; mã hiện hành kết thúc ở UT-020.
