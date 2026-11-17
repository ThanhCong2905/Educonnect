# Kết quả Integration 08 Bai tap Cham

Đã chạy 19 ca: **17 Pass, 2 Fail**, 0 Skip. Đây là kiểm thử tích hợp, có SQL và HTTP thật. Không sửa code nghiệp vụ để làm test đạt.

- Bắt đầu: 2026-11-14T17:37:42.0913290+07:00; kết thúc: 2026-11-14T17:38:14.1583576+07:00.
- Nguồn: Integration Test.xlsx hiện tại, các mã và kỳ vọng giữ theo sheet.
- Database riêng: LAPTOP-3L2V1AFT\SQLEXPRESS / EduConnect_Verification_IT0203. Windows locale và host: vi-VN.
- Mỗi ca có bốn tài khoản thật, fixture mang tag riêng; đăng nhập cookie/phiên SQL, antiforgery hợp lệ trừ ca cố ý thiếu.
- Đối chiếu snapshot trước/sau bằng DbContext mới AsNoTracking: lớp, thành viên, lịch/buổi, notice/outbox, bài tập/nộp/chấm, tệp và SHA256 vật lý.
- Scanner được thay bằng sandbox trả sạch/từ chối hoặc dừng có điều phối. Không dùng mã độc thật, không khẳng định đã kiểm độ chính xác ClamAV. Signature/giới hạn/lưu file/quyền tải vẫn chạy code thật.

## Kết quả từng ca

| Mã | Tình huống | Tiêu chuẩn | Kết quả | Quan sát thực tế |
|---|---|---|---|---|
| IT-087 | Giao bài và tệp | Đầy đủ | Passed | HTTP 200; đúng một BaiTapGiao DaGiao, gắn lớp/buổi. Một PDF sạch thuộc G, gắn đúng bài giao; một thông báo cho H đúng Id bài. |
| IT-088 | Lưu nháp chưa công bố | Xác thực | Passed | HTTP 200, bài BanNhap; không notice mới, không bài nộp. H GET bài nháp và POST nộp đều bị từ chối. |
| IT-089 | Công bố bài nháp | Đúng đắn | Passed | HTTP 200; chuyển chính bài nháp sang DaGiao, không tạo bài giao khác. Một đợt tạo hai notice cho hai thành viên G/H theo code hiện tại. |
| IT-090 | Nộp bài đúng hạn | Đúng đắn | Passed | HTTP 200; một bài nộp LanNop=1, đúng H, assignment và MaThaoTac; PDF sạch gắn đúng BaiTapNop. |
| IT-091 | Nộp muộn theo code hiện tại | Đúng đắn | Passed | HTTP 200, chấp nhận nộp muộn; NopLuc sau HanNopLuc, deadline không đổi; thông báo G đúng “Có bài nộp muộn.”. |
| IT-092 | Nộp lại cùng mã và dữ liệu | Đúng đắn | Passed | Hai POST cùng MaThaoTac, nội dung và checksum PDF đều200; sau request thứ hai toàn bộ snapshot bằng sau request thứ nhất, không tăng lần nộp/tệp/notice. |
| IT-093 | Cùng mã khác nội dung | Đúng đắn | Passed | HTTP 409, SQL51033; cùng mã thao tác nhưng nội dung B không ghi đè nội dung A; toàn bộ nghiệp vụ/tệp không đổi. |
| IT-094 | Nộp lần mới sau đã chấm | Đúng đắn | Passed | HTTP 409, SQL51034; mã mới sau khi đã chấm bị từ chối, không tăng LanNop hoặc thêm tệp/notice. |
| IT-095 | Thành viên chỉ Xem nộp bài | Xác thực | Passed | HTTP 403; membership Xem không nộp được, không thay đổi dữ liệu. |
| IT-096 | ChinhSua nhưng chưa ChoPhepNopBai | Xác thực | Passed | HTTP 403; ChinhSua nhưng ChoPhepNopBai=false không nộp thay được, không thay đổi dữ liệu. |
| IT-097 | Liên kết đã hủy mất quyền nộp | Xác thực | Failed | Kỳ vọng403 nhưng nhận422; H đã mất liên kết/quyền vào lớp. SQL và tệp nguyên trạng, không tạo bài nộp. Fail mã HTTP, chưa thấy vượt quyền. |
| IT-098 | Chấm điểm lần đầu | Đúng đắn | Failed | Kỳ vọng chấm8.25 thành công nhưng nhận422. Quan sát MVC: culture=vi-VN, wire="8.25", bound=825, ModelState invalid. Không có KetQuaChamBai và không có notice kết quả mới. |
| IT-099 | Điểm ngoài miền | Đầy đủ | Passed | Hai dataset độc lập10.01 và−0.01 đều bị validation422, không có kết quả chấm/notice mới. Gửi form theo culture hiện tại:10,01 và−0,01; probe ghi nhận bound−0.01. |
| IT-100 | Điểm nhiều hơn hai số lẻ | Đúng đắn | Passed | Gọi procedure thật trực tiếp với SqlParameter decimal(18,6)=8.251; nhận SQL51036. Không làm tròn rồi lưu, snapshot giữ nguyên. Đây là kiểm tích hợp SQL, không phải HTTP/unit. |
| IT-101 | Chấm lần nộp cũ | Đúng đắn | Passed | HTTP 409, SQL51037; chấm submission lần1 khi đã có lần2 bị từ chối. Điểm đầu vào8 để đi tới kiểm lần nộp, không bị lỗi binding thập phân che nhánh. |
| IT-102 | Sửa điểm RV cũ | Đúng đắn | Passed | HTTP 409, SQL51039; gửi RV1 sau cập nhật RV2 bị từ chối. Giữ điểm9/bản mới và số notice; điểm gửi8 là số nguyên để cô lập nhánh RV. |
| IT-103 | Lớp kết thúc chặn ghi bài | Xác thực | Passed | HTTP 409; lớp DaKetThuc không nhận bài nộp mới, toàn bộ snapshot nguyên trạng. |
| IT-104 | Không đăng nhập gọi thao tác ghi | Xác thực | Passed | Anonymous AJAX có antiforgery hợp lệ nhận401, không Location/redirect HTML; không có thay đổi nghiệp vụ. |
| IT-105 | Thiếu antiforgery token | Xác thực | Passed | Đã đăng nhập nhưng POST thiếu antiforgery nhận400; nghiệp vụ không đổi. |

## Lỗi và khác biệt cần xử lý

### IT-097 — Liên kết đã hủy mất quyền nộp

**Phân loại:** ProductContract.
**Kỳ vọng:** 403; không bài nộp/tệp/notice.
**Thực tế:** Kỳ vọng403 nhưng nhận422; H đã mất liên kết/quyền vào lớp. SQL và tệp nguyên trạng, không tạo bài nộp. Fail mã HTTP, chưa thấy vượt quyền.
**Nguyên nhân:** BaiTapController.Nop bắt lỗi quyền, thêm ModelState rồi gọi lại ChiTiet. ChiTiet ném lỗi quyền tiếp; UiResponseFilter.OnException thấy POST và ModelState invalid nên ghi cứng422, thay mã403 cần giữ.
**Ảnh hưởng và giới hạn:** Sai hợp đồng HTTP, xử lý client có thể nhầm lỗi quyền thành lỗi dữ liệu. Kiểm dữ liệu vẫn cho thấy từ chối ghi đúng.
**Hướng sửa/kiểm lại:** Giữ mã lỗi quyền khi dựng JSON lỗi hoặc tránh gọi lại action ChiTiet không còn quyền.

**Chỗ đối chiếu code** (số dòng tại bản đã chạy):

- `Areas/HocTap/Controllers/BaiTapController.cs:104`.
- `Infrastructure/UiResponseFilter.cs:30`.

Assertion thực tế:

```text
Không quyền nộp phải403, thực tế422.
```

### IT-098 — Chấm điểm lần đầu

**Phân loại:** ProductBinding.
**Kỳ vọng:** 8.25 được lưu, đúng người/bài, có thông báo.
**Thực tế:** Kỳ vọng chấm8.25 thành công nhưng nhận422. Quan sát MVC: culture=vi-VN, wire="8.25", bound=825, ModelState invalid. Không có KetQuaChamBai và không có notice kết quả mới.
**Nguyên nhân:** input type=number gửi value invariant có dấu chấm; máy/host chạy vi-VN, binder form decimal hiểu8.25 thành825. Range0–10 loại trước khi BaiTapService/usp_ChamBaiTap chạy. Probe chỉ đọc ActionArguments/ModelState, không đổi dữ liệu.
**Ảnh hưởng và giới hạn:** Không chấm được điểm thập phân theo chính định dạng biểu mẫu đang xuất trong cấu hình hiện tại; không phải lỗi procedure làm tròn.
**Hướng sửa/kiểm lại:** Thống nhất culture của request/binder với input số invariant; kiểm lại8.25,0.01,10 và ngoài miền. Không đổi ca thành số nguyên để báo đạt.

**Chỗ đối chiếu code** (số dòng tại bản đã chạy):

- `Areas/HocTap/Views/BaiTap/ChiTiet.cshtml:67`.
- `Areas/HocTap/ViewModels/HocTapInputs.cs:85`.
- `Areas/HocTap/Controllers/BaiTapController.cs:107`.

Assertion thực tế:

```text
HTTP kỳ vọng 200, thực tế 422.
Kỳ vọng JSON ok=true.
Thiếu redirectUrl thành công.
Chấm lần đầu sai điểm/người/bài.
Thiếu notice kết quả.
```

## Các bước thực hiện và minh chứng từng ca

Áp dụng cho từng ca: xác nhận DB, tạo dữ liệu nền qua service/EF/SQL thật; chụp baseline; đăng nhập đúng actor; lấy antiforgery cùng client; gửi request; đọc HTTP trước redirect; mở DbContext mới đối chiếu toàn bộ dữ liệu và cleanup chỉ ID/tệp của fixture. Chi tiết ID, input, phiên bản và từng cột nằm trong KetQua.json và evidence JSON trong ZIP.

### IT-087 — Giao bài và tệp

**Endpoint theo bảng:** POST /HocTap/BaiTap/Giao

**Dữ liệu/tiền điều kiện:** Nội dung đủ, hạn tương lai, một PDF sạch.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-087; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Nội dung đủ, hạn tương lai, một PDF sạch. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Giao. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Thành công. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: BaiTapGiao DaGiao đúng lớp; tệp thuộc bài; notice/outbox đúng người có quyền. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_414fb8e05c574b5f81b18e1d601bbdb0`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; HTTP 200; đúng một BaiTapGiao DaGiao, gắn lớp/buổi. Một PDF sạch thuộc G, gắn đúng bài giao; một thông báo cho H đúng Id bài.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":true,\"message\":\"\\u0110\\u00E3 l\\u01B0u thay \\u0111\\u1ED5i.\",\"redirectUrl\":\"/HocTap/LichHoc/Buoi/c9efcc73-17e0-f111-a50f-005056c00003\"}"
  },
  "input": {
    "LopHocId": "6d5c9e50-5cdd-4dbc-9f93-b4948db25e48",
    "BuoiHocId": "1e1d82cb-1193-4bd5-83be-2ab49ebb0547",
    "TieuDe": "Bài tập mới",
    "NoiDung": "Nội dung giao bài",
    "HanNopLuc": "2026-11-23T21:00",
    "LuuNhap": "False"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-088 — Lưu nháp chưa công bố

**Endpoint theo bảng:** POST /HocTap/BaiTap/Giao

**Dữ liệu/tiền điều kiện:** LuuNhap=true; đủ validator.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-088; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: LuuNhap=true; đủ validator. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Giao. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Thành công. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: BanNhap; H không đọc/nộp bài nháp; chưa gửi notice bài mới. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_643c2c2509034f8fb56a5d949a2fce94`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; HTTP 200, bài BanNhap; không notice mới, không bài nộp. H GET bài nháp và POST nộp đều bị từ chối.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":true,\"message\":\"\\u0110\\u00E3 l\\u01B0u thay \\u0111\\u1ED5i.\",\"redirectUrl\":\"/HocTap/LichHoc/Buoi/88d6116d-17e0-f111-a50f-005056c00003\"}"
  },
  "input": {
    "LopHocId": "a688b245-cf81-40f0-9622-82ac7e52a902",
    "BuoiHocId": "7fc5f6a2-ec9d-4cc2-b10a-93fde2d802d4",
    "TieuDe": "Bài tập mới",
    "NoiDung": "Nội dung giao bài",
    "HanNopLuc": "2026-11-23T21:00",
    "LuuNhap": "True"
  },
  "sqlErrorNumbers": [],
  "draftRead": {
    "status": 404,
    "location": null,
    "contentType": "text/html; charset=utf-8",
    "body": "<!doctype html><html lang='vi'><meta charset='utf-8'><meta name='viewport' content='width=device-width'><title>EduConnect</title><link rel='stylesheet' href='/css/app.css'><main class='error-page'><h1>Chưa thể hoàn tất</h1><p role='alert'>B&#224;i tập chưa được giao.</p><a class='btn' href='/'>Về trang chủ</a><button onclick='history.back()'>Quay lại biểu mẫu</button></main></html>"
  },
  "draftSubmit": {
    "status": 422,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"Ki\\u1EC3m tra l\\u1EA1i th\\u00F4ng tin \\u0111\\u00E3 nh\\u1EADp.\",\"errors\":{\"\":[\"B\\u00E0i t\\u1EADp ch\\u01B0a \\u0111\\u01B0\\u1EE3c giao.\"]}}"
  },
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-089 — Công bố bài nháp

**Endpoint theo bảng:** POST /HocTap/BaiTap/CongBo

**Dữ liệu/tiền điều kiện:** G; id=A1 BanNhap; RV mới.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-089; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: G; id=A1 BanNhap; RV mới. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/CongBo. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Thành công. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: DaGiao; một đợt thông báo; không nhân bản A1. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_ed8e7d81a24e409fa5afc67fd86bf4ff`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; HTTP 200; chuyển chính bài nháp sang DaGiao, không tạo bài giao khác. Một đợt tạo hai notice cho hai thành viên G/H theo code hiện tại.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":true,\"message\":\"\\u0110\\u00E3 giao b\\u00E0i t\\u1EADp.\",\"redirectUrl\":\"/HocTap/LichHoc/Buoi/adefcc73-17e0-f111-a50f-005056c00003\"}"
  },
  "input": {
    "id": "39d7e8a1-ed6b-46bf-b3b3-91977e0bff05",
    "rowVersion": "AAAAAAAC18A="
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-090 — Nộp bài đúng hạn

**Endpoint theo bảng:** POST /HocTap/BaiTap/Nop

**Dữ liệu/tiền điều kiện:** H; MaThaoTac=M1; nội dung A; trước HanNopLuc.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-090; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: H; MaThaoTac=M1; nội dung A; trước HanNopLuc. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Nop. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Thành công. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Một BaiTapNop, LanNop=1, đúng H/A1/M1; tệp đúng chủ; thông báo G. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_ab96b32bf7754b66b1cf61e057de9d47`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; HTTP 200; một bài nộp LanNop=1, đúng H, assignment và MaThaoTac; PDF sạch gắn đúng BaiTapNop.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":true,\"message\":\"\\u0110\\u00E3 l\\u01B0u b\\u00E0i n\\u1ED9p.\",\"redirectUrl\":\"/HocTap/BaiTap/ChiTiet/d0618434-d445-4c28-9ec9-9e110780b997\"}"
  },
  "input": {
    "BaiTapGiaoId": "d0618434-d445-4c28-9ec9-9e110780b997",
    "MaThaoTac": "5c5a3cd9-e207-45af-a0ca-2a3ea1727ec2",
    "NoiDung": "Nội dung A"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-091 — Nộp muộn theo code hiện tại

**Endpoint theo bảng:** POST /HocTap/BaiTap/Nop

**Dữ liệu/tiền điều kiện:** HanNopLuc đã qua; A1 chưa chấm; H còn quyền; M1 mới.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-091; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: HanNopLuc đã qua; A1 chưa chấm; H còn quyền; M1 mới. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Nop. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Theo code/SQL hiện tại: tiếp nhận thành công. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: NopLuc>HanNopLuc; notice “Có bài nộp muộn.”; không đổi hạn để che nộp muộn. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_759526e55e6c4c89a212f7c5bb7a327f`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; HTTP 200, chấp nhận nộp muộn; NopLuc sau HanNopLuc, deadline không đổi; thông báo G đúng “Có bài nộp muộn.”.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":true,\"message\":\"\\u0110\\u00E3 l\\u01B0u b\\u00E0i n\\u1ED9p.\",\"redirectUrl\":\"/HocTap/BaiTap/ChiTiet/7bda3932-32f0-41e9-beec-0cf80363f9e9\"}"
  },
  "input": {
    "BaiTapGiaoId": "7bda3932-32f0-41e9-beec-0cf80363f9e9",
    "MaThaoTac": "e6bd71f3-62f7-4006-91e7-d2d3f45e8e81",
    "NoiDung": "Nội dung A"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-092 — Nộp lại cùng mã và dữ liệu

**Endpoint theo bảng:** POST /HocTap/BaiTap/Nop hai lần

**Dữ liệu/tiền điều kiện:** Cùng H/A1/M1/nội dung/tệp không đổi.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-092; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Cùng H/A1/M1/nội dung/tệp không đổi. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Nop hai lần. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Trả bài đã có. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Một BaiTapNop theo M1; không tăng LanNop/notice trùng. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_1130e4c17be44bce9d2c9d84b3fb7d45`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; Hai POST cùng MaThaoTac, nội dung và checksum PDF đều200; sau request thứ hai toàn bộ snapshot bằng sau request thứ nhất, không tăng lần nộp/tệp/notice.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":true,\"message\":\"\\u0110\\u00E3 l\\u01B0u b\\u00E0i n\\u1ED9p.\",\"redirectUrl\":\"/HocTap/BaiTap/ChiTiet/63aaf821-e598-4990-a11b-344edd3d40d8\"}"
  },
  "input": {
    "BaiTapGiaoId": "63aaf821-e598-4990-a11b-344edd3d40d8",
    "MaThaoTac": "4a5b9c13-0f68-4548-8e89-9995f597b264",
    "NoiDung": "Nội dung A"
  },
  "sqlErrorNumbers": [],
  "repeatResponse": {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":true,\"message\":\"\\u0110\\u00E3 l\\u01B0u b\\u00E0i n\\u1ED9p.\",\"redirectUrl\":\"/HocTap/BaiTap/ChiTiet/63aaf821-e598-4990-a11b-344edd3d40d8\"}"
  },
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-093 — Cùng mã khác nội dung

**Endpoint theo bảng:** POST /HocTap/BaiTap/Nop

**Dữ liệu/tiền điều kiện:** M1 đã lưu A; gửi M1 với B.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-093; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: M1 đã lưu A; gửi M1 với B. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Nop. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: 409, SQL 51033. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Giữ A; không bài mới. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_106fbd70efb944f083257e1037375cf0`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; HTTP 409, SQL51033; cùng mã thao tác nhưng nội dung B không ghi đè nội dung A; toàn bộ nghiệp vụ/tệp không đổi.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 409,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"Ki\\u1EC3m tra l\\u1EA1i th\\u00F4ng tin \\u0111\\u00E3 nh\\u1EADp.\",\"errors\":{\"\":[\"M\\u00E3 thao t\\u00E1c \\u0111\\u00E3 d\\u00F9ng v\\u1EDBi d\\u1EEF li\\u1EC7u kh\\u00E1c.\"]}}"
  },
  "input": {
    "BaiTapGiaoId": "52b406e1-58c7-47d4-a93f-c4bf0b7cf502",
    "MaThaoTac": "31f6b550-6484-400c-8b56-95e4f10149bb",
    "NoiDung": "Nội dung B"
  },
  "sqlErrorNumbers": [
    51033
  ],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-094 — Nộp lần mới sau đã chấm

**Endpoint theo bảng:** POST /HocTap/BaiTap/Nop

**Dữ liệu/tiền điều kiện:** A1 có KetQuaChamBai; M2 mới.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-094; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: A1 có KetQuaChamBai; M2 mới. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Nop. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: 409, SQL 51034. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Không tăng LanNop. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_5b0ab072952a4d3c9d2ec7b3a52f76ff`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; HTTP 409, SQL51034; mã mới sau khi đã chấm bị từ chối, không tăng LanNop hoặc thêm tệp/notice.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 409,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"Ki\\u1EC3m tra l\\u1EA1i th\\u00F4ng tin \\u0111\\u00E3 nh\\u1EADp.\",\"errors\":{\"\":[\"B\\u00E0i \\u0111\\u00E3 ch\\u1EA5m; kh\\u00F4ng nh\\u1EADn l\\u1EA7n n\\u1ED9p m\\u1EDBi.\"]}}"
  },
  "input": {
    "BaiTapGiaoId": "4124b8db-9aad-48e4-9460-1d1b6516a3e2",
    "MaThaoTac": "b06747df-ce97-4a9c-ac93-40b045e28ae0",
    "NoiDung": "Nội dung A"
  },
  "sqlErrorNumbers": [
    51034
  ],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-095 — Thành viên chỉ Xem nộp bài

**Endpoint theo bảng:** POST /HocTap/BaiTap/Nop

**Dữ liệu/tiền điều kiện:** X có membership Xem.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-095; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: X có membership Xem. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Nop. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: 403. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Không bài nộp/tệp gắn bài. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_d4ea7e44ce464b21ad658fad8a94e859`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; HTTP 403; membership Xem không nộp được, không thay đổi dữ liệu.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 403,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"Ki\\u1EC3m tra l\\u1EA1i th\\u00F4ng tin \\u0111\\u00E3 nh\\u1EADp.\",\"errors\":{\"\":[\"B\\u1EA1n kh\\u00F4ng c\\u00F3 quy\\u1EC1n th\\u1EF1c hi\\u1EC7n thao t\\u00E1c n\\u00E0y.\"]}}"
  },
  "input": {
    "BaiTapGiaoId": "0810fd07-dddd-4642-bed8-97a25888cd87",
    "MaThaoTac": "aefe2439-43f4-4daf-adc6-98e405a1ea4b",
    "NoiDung": "Nội dung A"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-096 — ChinhSua nhưng chưa ChoPhepNopBai

**Endpoint theo bảng:** POST /HocTap/BaiTap/Nop

**Dữ liệu/tiền điều kiện:** H là thành viên thêm, không bên học; ChinhSua nhưng flag=0.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-096; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: H là thành viên thêm, không bên học; ChinhSua nhưng flag=0. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Nop. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: 403. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Không BaiTapNop. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_4406b5bdcb8048b7861dbe7787704409`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; HTTP 403; ChinhSua nhưng ChoPhepNopBai=false không nộp thay được, không thay đổi dữ liệu.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 403,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"Ki\\u1EC3m tra l\\u1EA1i th\\u00F4ng tin \\u0111\\u00E3 nh\\u1EADp.\",\"errors\":{\"\":[\"B\\u1EA1n ch\\u01B0a \\u0111\\u01B0\\u1EE3c c\\u1EA5p quy\\u1EC1n n\\u1ED9p b\\u00E0i thay.\"]}}"
  },
  "input": {
    "BaiTapGiaoId": "30745125-7a2b-469f-a01c-e524b520be79",
    "MaThaoTac": "64133f59-29a6-423c-83d8-fb37595b0624",
    "NoiDung": "Nội dung A"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-097 — Liên kết đã hủy mất quyền nộp

**Endpoint theo bảng:** POST /HocTap/BaiTap/Nop

**Dữ liệu/tiền điều kiện:** H có flag nộp nhưng LK đã DaHuy.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-097; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: H có flag nộp nhưng LK đã DaHuy. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Nop. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: 403. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Không thêm bài. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_c30cd99b60eb4a7891400692817ac450`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; Kỳ vọng403 nhưng nhận422; H đã mất liên kết/quyền vào lớp. SQL và tệp nguyên trạng, không tạo bài nộp. Fail mã HTTP, chưa thấy vượt quyền.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 422,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"Ki\\u1EC3m tra l\\u1EA1i th\\u00F4ng tin \\u0111\\u00E3 nh\\u1EADp.\",\"errors\":{\"\":[\"B\\u1EA1n ch\\u01B0a c\\u00F3 quy\\u1EC1n ho\\u1EB7c li\\u00EAn k\\u1EBFt ph\\u00F9 h\\u1EE3p \\u0111\\u1EC3 v\\u00E0o l\\u1EDBp n\\u00E0y.\"]}}"
  },
  "input": {
    "BaiTapGiaoId": "c859d90f-bbb4-4bd7-87bf-7a721a90c084",
    "MaThaoTac": "c6fd7f72-e373-45be-9b1c-d50d17372525",
    "NoiDung": "Nội dung A"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [
    "Không quyền nộp phải403, thực tế422."
  ],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-098 — Chấm điểm lần đầu

**Endpoint theo bảng:** POST /HocTap/BaiTap/Cham

**Dữ liệu/tiền điều kiện:** G; bài nộp mới nhất; Diem=8.25; nhận xét đủ.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-098; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: G; bài nộp mới nhất; Diem=8.25; nhận xét đủ. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Cham. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Thành công. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Một KetQuaChamBai đúng BaiTapNopId, điểm 8.25, người G; thông báo đúng. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_7577845479e64ecd8f969e6ff82939fa`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; Kỳ vọng chấm8.25 thành công nhưng nhận422. Quan sát MVC: culture=vi-VN, wire="8.25", bound=825, ModelState invalid. Không có KetQuaChamBai và không có notice kết quả mới.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 422,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"Ki\\u1EC3m tra l\\u1EA1i th\\u00F4ng tin \\u0111\\u00E3 nh\\u1EADp.\",\"errors\":{\"Diem\":[\"\\u0110i\\u1EC3m ph\\u1EA3i t\\u1EEB 0 \\u0111\\u1EBFn 10.\"]}}"
  },
  "input": {
    "BaiTapGiaoId": "7bfdfb1d-f2d4-464e-a395-968a641fba35",
    "BaiTapNopId": "0d59c0ee-da32-4ba6-a29a-af10aa8ef191",
    "Diem": "8.25",
    "NhanXet": "Nhận xét chấm đủ nội dung"
  },
  "sqlErrorNumbers": [],
  "mvcBinding": {
    "culture": "vi-VN",
    "wire": "8.25",
    "bound": 825,
    "valid": false
  },
  "assertionDifferences": [
    "HTTP kỳ vọng 200, thực tế 422.",
    "Kỳ vọng JSON ok=true.",
    "Thiếu redirectUrl thành công.",
    "Chấm lần đầu sai điểm/người/bài.",
    "Thiếu notice kết quả."
  ],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-099 — Điểm ngoài miền

**Endpoint theo bảng:** POST /HocTap/BaiTap/Cham

**Dữ liệu/tiền điều kiện:** Diem=10.01 hoặc -0.01; dataset độc lập.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-099; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Diem=10.01 hoặc -0.01; dataset độc lập. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Cham. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Lỗi dữ liệu; không lưu. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Không kết quả chấm mới/cập nhật. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_6e7db3c8479243cf82f6cb7e51897c57`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; Hai dataset độc lập10.01 và−0.01 đều bị validation422, không có kết quả chấm/notice mới. Gửi form theo culture hiện tại:10,01 và−0,01; probe ghi nhận bound−0.01.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 422,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"Ki\\u1EC3m tra l\\u1EA1i th\\u00F4ng tin \\u0111\\u00E3 nh\\u1EADp.\",\"errors\":{\"Diem\":[\"\\u0110i\\u1EC3m ph\\u1EA3i t\\u1EEB 0 \\u0111\\u1EBFn 10.\"]}}"
  },
  "input": {
    "BaiTapGiaoId": "611ea5d4-bc2d-4869-9b62-479f4bfbe737",
    "BaiTapNopId": "2b185e2b-cf16-4653-b4d6-48aeea2a5715",
    "Diem": "10,01",
    "NhanXet": "Nhận xét chấm đủ nội dung"
  },
  "sqlErrorNumbers": [],
  "mvcBinding": {
    "culture": "vi-VN",
    "wire": "-0,01",
    "bound": -0.01,
    "valid": false
  },
  "secondDataset": {
    "assignmentId": "18fdf260-dd6d-416d-9231-705f8d7e0ef8",
    "submissionId": "732b1583-5ae8-465a-b6c9-1eace5231f11",
    "input": [
      {
        "Key": "BaiTapGiaoId",
        "Value": "18fdf260-dd6d-416d-9231-705f8d7e0ef8"
      },
      {
        "Key": "BaiTapNopId",
        "Value": "732b1583-5ae8-465a-b6c9-1eace5231f11"
      },
      {
        "Key": "Diem",
        "Value": "-0,01"
      },
      {
        "Key": "NhanXet",
        "Value": "Nhận xét chấm đủ nội dung"
      }
    ],
    "http": {
      "status": 422,
      "location": null,
      "contentType": "application/json; charset=utf-8",
      "body": "{\"ok\":false,\"message\":\"Ki\\u1EC3m tra l\\u1EA1i th\\u00F4ng tin \\u0111\\u00E3 nh\\u1EADp.\",\"errors\":{\"Diem\":[\"\\u0110i\\u1EC3m ph\\u1EA3i t\\u1EEB 0 \\u0111\\u1EBFn 10.\"]}}"
    }
  },
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-100 — Điểm nhiều hơn hai số lẻ

**Endpoint theo bảng:** Gọi service chấm thật hoặc EXEC usp_ChamBaiTap

**Dữ liệu/tiền điều kiện:** Diem=8.251; bypass binding làm tròn để kiểm tra procedure.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-100; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Diem=8.251; bypass binding làm tròn để kiểm tra procedure. B4. Gọi service chấm thật hoặc EXEC usp_ChamBaiTap. Dùng connection SQL độc lập; nếu gọi service/worker, lấy implementation thật từ DI scope, không mock AppDbContext hoặc procedure. B5. Thu SqlException.Number/result set hoặc kết quả handler. Đối chiếu: SQL 51036. B6. Đọc lại từ connection mới sau khi transaction hoàn tất bằng SQL cột kiểm tra. Đối chiếu: Không làm tròn ngầm thành 8.25 và ghi. B7. Ca đồng thời: hai scope/connection, điều phối bằng barrier/transaction; không chạy nối tiếp rồi gọi đó là đồng thời. Ca thời gian: kiểm tra giờ SQL sau khóa, không sửa đồng hồ hệ điều hành. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_62d48d64a4d749dca6b0e4562a3db2f8`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; Gọi procedure thật trực tiếp với SqlParameter decimal(18,6)=8.251; nhận SQL51036. Không làm tròn rồi lưu, snapshot giữ nguyên. Đây là kiểm tích hợp SQL, không phải HTTP/unit.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "sqlErrorNumbers": [],
  "directSqlNumber": 51036,
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-101 — Chấm lần nộp cũ

**Endpoint theo bảng:** POST /HocTap/BaiTap/Cham

**Dữ liệu/tiền điều kiện:** Có LanNop=1,2; chấm lần 1.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-101; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Có LanNop=1,2; chấm lần 1. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Cham. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: 409 theo SQL 51037. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Không KetQua cho lần cũ. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_37aec215f93345eea9cc3792c33f7ccf`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; HTTP 409, SQL51037; chấm submission lần1 khi đã có lần2 bị từ chối. Điểm đầu vào8 để đi tới kiểm lần nộp, không bị lỗi binding thập phân che nhánh.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 409,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"Ki\\u1EC3m tra l\\u1EA1i th\\u00F4ng tin \\u0111\\u00E3 nh\\u1EADp.\",\"errors\":{\"\":[\"Ch\\u1EC9 ch\\u1EA5m l\\u1EA7n n\\u1ED9p m\\u1EDBi nh\\u1EA5t.\"]}}"
  },
  "input": {
    "BaiTapGiaoId": "bfe04328-6c90-442f-8ded-1f0c25432f9b",
    "BaiTapNopId": "be390453-a24a-4e41-bc8b-8c9cd0d2aa85",
    "Diem": "8",
    "NhanXet": "Nhận xét chấm đủ nội dung"
  },
  "sqlErrorNumbers": [
    51037
  ],
  "mvcBinding": {
    "culture": "vi-VN",
    "wire": "8",
    "bound": 8,
    "valid": true
  },
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-102 — Sửa điểm RV cũ

**Endpoint theo bảng:** POST /HocTap/BaiTap/Cham

**Dữ liệu/tiền điều kiện:** Đã sửa điểm tạo RV2; input RV1.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-102; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Đã sửa điểm tạo RV2; input RV1. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Cham. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: 409; SQL 51039. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Giữ điểm mới nhất; không thông báo cập nhật sai. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_732fb11c9fd24528bd7ff86115377da1`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; HTTP 409, SQL51039; gửi RV1 sau cập nhật RV2 bị từ chối. Giữ điểm9/bản mới và số notice; điểm gửi8 là số nguyên để cô lập nhánh RV.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 409,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"Ki\\u1EC3m tra l\\u1EA1i th\\u00F4ng tin \\u0111\\u00E3 nh\\u1EADp.\",\"errors\":{\"\":[\"K\\u1EBFt qu\\u1EA3 \\u0111\\u00E3 thay \\u0111\\u1ED5i; t\\u1EA3i l\\u1EA1i tr\\u01B0\\u1EDBc khi s\\u1EEDa.\"]}}"
  },
  "input": {
    "BaiTapGiaoId": "5695ed36-1386-4e9c-b5c9-0ec8c3eb0332",
    "BaiTapNopId": "93d93355-323b-40db-b393-09842e7075a5",
    "Diem": "8",
    "NhanXet": "Nhận xét chấm đủ nội dung",
    "RowVersion": "AAAAAAAC1u4="
  },
  "sqlErrorNumbers": [
    51039
  ],
  "mvcBinding": {
    "culture": "vi-VN",
    "wire": "8",
    "bound": 8,
    "valid": true
  },
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-103 — Lớp kết thúc chặn ghi bài

**Endpoint theo bảng:** POST /HocTap/BaiTap/Nop

**Dữ liệu/tiền điều kiện:** C1 DaKetThuc; H có quyền trước đó.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-103; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: C1 DaKetThuc; H có quyền trước đó. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Nop. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: 409 theo trạng thái lớp. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Không BaiTapNop mới. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_8bf6e7b391604be0b15b2a0c12d0e788`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; HTTP 409; lớp DaKetThuc không nhận bài nộp mới, toàn bộ snapshot nguyên trạng.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 409,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"Ki\\u1EC3m tra l\\u1EA1i th\\u00F4ng tin \\u0111\\u00E3 nh\\u1EADp.\",\"errors\":{\"\":[\"L\\u1EDBp kh\\u00F4ng c\\u00F2n ho\\u1EA1t \\u0111\\u1ED9ng.\"]}}"
  },
  "input": {
    "BaiTapGiaoId": "856ac359-2c61-4870-aa74-d507222d8945",
    "MaThaoTac": "e8a1ed58-cf4b-4acc-8e6e-6bc703070afb",
    "NoiDung": "Nội dung A"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-104 — Không đăng nhập gọi thao tác ghi

**Endpoint theo bảng:** POST /HocTap/BaiTap/Nop

**Dữ liệu/tiền điều kiện:** Không gửi cookie phiên; lấy token antiforgery hợp lệ từ trang công khai nếu cần.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-104; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Không gửi cookie phiên; lấy token antiforgery hợp lệ từ trang công khai nếu cần. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Nop. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: AJAX 401; không chuyển HTML đăng nhập. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Không đổi dữ liệu nghiệp vụ mục tiêu. Nhật ký an toàn từ chối có thể được thêm. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_39590c694ac34654b4f4eb19e8f39de2`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; Anonymous AJAX có antiforgery hợp lệ nhận401, không Location/redirect HTML; không có thay đổi nghiệp vụ.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 401,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"Phiên đã hết hạn. Vui lòng đăng nhập lại.\"}"
  },
  "input": {
    "BaiTapGiaoId": "e42afb15-8cd6-4eb2-82be-d38eb6a63899",
    "MaThaoTac": "7f2673be-b3ee-4a85-b9a1-3539c6c2747d",
    "NoiDung": "Nội dung A"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

### IT-105 — Thiếu antiforgery token

**Endpoint theo bảng:** POST /HocTap/BaiTap/Nop

**Dữ liệu/tiền điều kiện:** Đăng nhập đúng quyền; bỏ __RequestVerificationToken/header token, giữ cookie.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-105; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. C1 hoạt động, G phụ trách, H bên học, X mức Xem; A1 bài DaGiao; input có MaThaoTac riêng; tệp sạch được scanner sandbox xác nhận. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Đăng nhập đúng quyền; bỏ __RequestVerificationToken/header token, giữ cookie. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/BaiTap/Nop. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: HTTP 400 do antiforgery; không yêu cầu JSON theo schema của action. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Không thay đổi dữ liệu nghiệp vụ. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_2fa727db75d94e4abdd7b62bd55c8313`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; Đã đăng nhập nhưng POST thiếu antiforgery nhận400; nghiệp vụ không đổi.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 400,
    "location": null,
    "contentType": null,
    "body": ""
  },
  "input": {
    "BaiTapGiaoId": "e9b59d19-2510-41d1-be2f-b5d294a4b432",
    "MaThaoTac": "ddbaa488-4ef8-4332-844b-cf8a5697cc10",
    "NoiDung": "Nội dung A"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[BaiTapGiao] WHERE Id=@Assignment; SELECT * FROM dbo.[BaiTapNop] WHERE BaiTapGiaoId=@Assignment; SELECT * FROM dbo.[KetQuaChamBai] WHERE BaiTapNopId=@Submission; SELECT * FROM dbo.[TepDinhKem] WHERE BaiTapGiaoId=@Assignment OR BaiTapNopId=@Submission;
```

## Phạm vi, cách chạy lại và file nguồn

- IT-098 giữ giá trị wire8.25 đúng input number hiện tại; probe xác nhận bound825. IT-099 gửi10,01 /−0,01 theo culture để kiểm đúng giá trị ngoài miền. IT-101/102 dùng8 để kiểm được SQL51037/51039, không kết luận định dạng thập phân đã sửa.
- IT-100 gọi SQL trực tiếp theo lựa chọn được phép trong bảng; không mô tả thành test controller.
- IT-106 dùng WebSocket/JSON SignalR thật, cookie phiên thật và group thật. Dừng trước commit, chạy batch worker trước/sau để kiểm không phát sớm và đúng messageId. Hosted timer tự chạy không thuộc bằng chứng ca này.
- IT-107 đối chiếu Id và outbox qua SQL; JSON Gui chỉ trả redirectUrl, không giả rằng response chứa messageId. Không đếm số event nhận qua socket ở ca retry này; kiểm không thêm nguồn phát outbox.
- IT-110 kiểm API before và cơ chế after trả overlap tie; client phải hợp nhất theo Id. Không kết luận DOM frontend không trùng.
- IT-116 từ chối an toàn nhưng thiếu metadata theo thiết kế. IT-118 được đánh Fail điều kiện thứ tự, không coi là lỗ hổng đã xác nhận. SQL1222 là kết quả probe có timeout1 giây; không phải lỗi HTTP giả lập.
- Thời gian/giờ lớp và deadline được điều chỉnh SQL trong bước tạo tiền điều kiện; không dùng SQL để thay kết quả request cần kiểm.

**File thêm:**

- `Cases0809.cs`:32 Fact và Trait tách hai nhóm.
- `AssignmentGradeIntegrationTests.cs`:19 ca nhóm08, form gửi HTTP và kiểm nộp/chấm/RowVersion.
- `FileChatIntegrationTests.cs`:13 ca nhóm09, multipart, phân trang, WebSocket và race khi quét.
- `AssignmentChatFixture.cs`:sandbox scanner/tệp, gate commit, probe binding chỉ đọc, snapshot và cleanup.

**File sửa:** `IntegrationHarness.cs` thêm điểm cấu hình services cho host test và giữ cookie đăng nhập trong bộ nhớ để dùng WebSocket. Cookie không được ghi vào JSON/TRX. ZIP có bản trước và diff. Không sửa code ứng dụng hoặc script schema.

Trong Terminal PowerShell tại repo:

```powershell
$env:EDU_TEST_ROOT='C:\Khóa luận TN\Educonnect'
$env:IT0809_EVIDENCE=Join-Path $env:TEMP 'EduConnect-IT0809-Evidence'
$env:IT0809_SANDBOX=Join-Path $env:TEMP 'EduConnect-IT0809-Files'
dotnet test tests/Integration0203/Integration0203.csproj --filter "Suite=IT08BaiTapCham" --logger "trx;LogFileName=BaiTap-Cham-Integration.trx" --results-directory "TestResults/08-BaiTap-Cham-Integration"
dotnet test tests/Integration0203/Integration0203.csproj --filter "FullyQualifiedName~Integration0203Tests.IT_098"
```

Chạy hai nhóm tuần tự trên cùng DB. Nếu app khóa thư mục build, dừng app hoặc build sang BaseOutputPath riêng với UseAppHost=false rồi chạy dotnet vstest DLL. Test chỉ chấp nhận database kiểm thử nêu trên.

## Xác minh sau chạy

**Hồi quy71 ca nhóm02–07:63 Pass,8 Fail, từng mã không đổi.** Không ghi đè kết quả nhóm cũ; TRX hồi quy đính kèm HoiQuy-02-07.trx. Tám Fail cũ: IT-022, IT-023, IT-025, IT-034, IT-037, IT-057, IT-069, IT-083.

32 ca mới đều đi hết cleanup, không có lỗi khởi tạo/cleanup trong lượt chính. XacMinh-SQL.txt xác nhận còn0 tài khoản/lớp fixture và0 tệp sandbox sau lượt chính và hồi quy. Các nhật ký bảo mật anonymous có thể còn trong database kiểm thử.

Lượt đầu có lỗi phép so chuỗi JSON của test (text Unicode được escape), đã sửa bằng parse JSON. Cũng sửa test khi kết quả chấm vắng để ghi assertion thay vì phát sinh exception; tách input nguyên cho hai ca xung đột. Kết quả chính thức là chạy lại đầy đủ với probe binding. Build cuối0 warning/0 error. Không đổi kỳ vọng để cho Pass, không sửa production.

KetQua.json lưu điều kiện, kỳ vọng và snapshot đầy đủ; TRX lưu lỗi/assertion cùng OBSERVATION_JSON. Ma_nguon_Integration.zip có project test, các nguồn tham chiếu và evidence từng ca. SHA256.json dùng đối chiếu file.
