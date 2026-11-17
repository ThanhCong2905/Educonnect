# Kết quả Integration 09 Tep Chat

Đã chạy 13 ca: **11 Pass, 2 Fail**, 0 Skip. Đây là kiểm thử tích hợp, có SQL và HTTP thật. Không sửa code nghiệp vụ để làm test đạt.

- Bắt đầu: 2026-11-14T17:38:14.7821322+07:00; kết thúc: 2026-11-14T17:38:39.8423675+07:00.
- Nguồn: Integration Test.xlsx hiện tại, các mã và kỳ vọng giữ theo sheet.
- Database riêng: LAPTOP-3L2V1AFT\SQLEXPRESS / EduConnect_Verification_IT0203. Windows locale và host: vi-VN.
- Mỗi ca có bốn tài khoản thật, fixture mang tag riêng; đăng nhập cookie/phiên SQL, antiforgery hợp lệ trừ ca cố ý thiếu.
- Đối chiếu snapshot trước/sau bằng DbContext mới AsNoTracking: lớp, thành viên, lịch/buổi, notice/outbox, bài tập/nộp/chấm, tệp và SHA256 vật lý.
- Scanner được thay bằng sandbox trả sạch/từ chối hoặc dừng có điều phối. Không dùng mã độc thật, không khẳng định đã kiểm độ chính xác ClamAV. Signature/giới hạn/lưu file/quyền tải vẫn chạy code thật.

## Kết quả từng ca

| Mã | Tình huống | Tiêu chuẩn | Kết quả | Quan sát thực tế |
|---|---|---|---|---|
| IT-106 | Gửi tin qua HTTP và nhận realtime | Đúng đắn | Passed | HTTP 200; WebSocket SignalR thật của G tham gia group hội thoại. Dừng transaction trước commit và gọi worker thật: không nhận TinNhan sớm. Sau commit worker phát CapNhat đúng Id tin/room, SQL có một tin và một chat outbox. |
| IT-107 | Retry tin cùng mã | Đúng đắn | Passed | Retry cùng MaThaoTac/nội dung nhận200; giữ nguyên Id tin đã có, không thêm TinNhan/chat outbox. API không trả Id tin trong JSON; đối chiếu Id bằng SQL. |
| IT-108 | Retry đổi nội dung | Đúng đắn | Passed | Cùng MaThaoTac nhưng nội dung B nhận409; nội dung A, tệp và outbox giữ nguyên. |
| IT-109 | Người ngoài đọc lịch sử | Xác thực | Passed | H2 ngoài hội thoại GET DuLieu bị403; không trả nội dung riêng, không thay đổi SQL. |
| IT-110 | Phân trang tin cùng timestamp | Đầy đủ | Passed | 120 tin cùng GuiLuc được đọc qua before thành50/50/20, đủ120 Id phân biệt. after trả đủ119 tie khác pivot, không lặp trong response. API chủ động trả overlap cùng thời điểm để client hợp nhất theo Id; không kiểm giao diện DOM. |
| IT-111 | Lớp kết thúc chỉ đọc chat | Xác thực | Passed | Lớp kết thúc vẫn GET DuLieu200 và đọc đúng tin cũ; POST gửi tin nhận403, lịch sử và toàn bộ snapshot giữ nguyên. |
| IT-112 | Upload 10 MB đúng biên | Đầy đủ | Passed | PDF đúng10.000.000 byte được chấp nhận200; metadata Sach, đúng chủ/link tin/kích thước. GET Tai200 trả đủ byte, SHA256 khớp metadata. |
| IT-113 | Upload vượt biên | Đầy đủ | Passed | PDF10.000.001 byte bị422; không tăng message/metadata/outbox và không có tệp lưu dở. |
| IT-114 | Sáu tệp một lượt | Đầy đủ | Passed | Gửi6 PDF nhỏ bị422 do tối đa5 tệp; không tạo một phần message/metadata/outbox hoặc tệp vật lý. |
| IT-115 | Đuôi hợp lệ nội dung sai | Xác thực | Passed | Tên.pdf nhưng sai chữ ký bị422 trước scanner; scannerCalls=0, không tệp/metadata/message mới. |
| IT-116 | Scanner báo cách ly | Xác thực | Failed | Scanner sandbox từ chối fixture vô hại; HTTP422, không tệp sạch hay message và không tệp vật lý. Tuy nhiên không có metadata CachLy/TuChoi như kịch bản, nên Fail phần lưu dấu vết từ chối; không có bằng chứng tệp nguy hiểm được phục vụ. |
| IT-117 | Tải tệp không quyền | Xác thực | Passed | H2 ngoài hội thoại GET Tai của tệp sạch có thật bị403; không trả bytes PDF, metadata/tệp/SQL không đổi. |
| IT-118 | Thu hồi quyền trong lúc quét | Xác thực | Failed | Đã dừng request tại scanner. UPDATE thu quyền trên kết nối SQL khác bị lock timeout1222 vì transaction Serializable giữ khóa, nên thu quyền chưa commit trước nhả scanner. Request gửi hoàn tất trước thay đổi quyền. Sau khi thu quyền commit, POST tiếp theo403. Không đạt thứ tự ca yêu cầu, chưa đủ bằng chứng kết luận lỗi vượt quyền. |

## Lỗi và khác biệt cần xử lý

### IT-116 — Scanner báo cách ly

**Phân loại:** DesignImplementationGap.
**Kỳ vọng:** Không phục vụ sạch; có metadata trạng tháiCachLy/TuChoi theo bảng.
**Thực tế:** Scanner sandbox từ chối fixture vô hại; HTTP422, không tệp sạch hay message và không tệp vật lý. Tuy nhiên không có metadata CachLy/TuChoi như kịch bản, nên Fail phần lưu dấu vết từ chối; không có bằng chứng tệp nguy hiểm được phục vụ.
**Nguyên nhân:** TepService.KiemTraAsync gọi scanner trước LuuAsync. Scanner ném lỗi nên không tới đoạn tạo TepDinhKem; đoạn lưu hiện chỉ chọnSach/ChoQuet, không tạo trạng tháiCachLy/TuChoi cho tệp bị từ chối.
**Ảnh hưởng và giới hạn:** Đạt việc từ chối an toàn nhưng thiếu dấu vết cách ly/từ chối theo kịch bản. Test sandbox không xác nhận chất lượng phát hiện của ClamAV thực.
**Hướng sửa/kiểm lại:** Thống nhất chính sách lưu metadata/audit tệp bị từ chối; nếu yêu cầu giữ metadata thì triển khai đường ghi riêng, vẫn không mở quyền tải.

**Chỗ đối chiếu code** (số dòng tại bản đã chạy):

- `Infrastructure/TepService.cs:73`.
- `Infrastructure/TepService.cs:153`.

Assertion thực tế:

```text
Kịch bản yêu cầu metadata CachLy/TuChoi, nhưng scanner ném lỗi trước khi tạo metadata; từ chối an toàn song thiếu bản ghi cách ly/từ chối.
```

### IT-118 — Thu hồi quyền trong lúc quét

**Phân loại:** ExecutionConstraint.
**Kỳ vọng:** Thu quyền commit lúc đang quét; sau quét request bị từ chối và không ghi tệp/tin.
**Thực tế:** Đã dừng request tại scanner. UPDATE thu quyền trên kết nối SQL khác bị lock timeout1222 vì transaction Serializable giữ khóa, nên thu quyền chưa commit trước nhả scanner. Request gửi hoàn tất trước thay đổi quyền. Sau khi thu quyền commit, POST tiếp theo403. Không đạt thứ tự ca yêu cầu, chưa đủ bằng chứng kết luận lỗi vượt quyền.
**Nguyên nhân:** TinNhanService.GuiAsync mở transactionSerializable trước truy vấn quyền và chờ quét. Kết nối thay quyền độc lập không lấy được khóa ghi trong1 giây (SQL1222); không thể có sự kiện thu quyền commit trước khi nhả quét trong lượt này.
**Ảnh hưởng và giới hạn:** Fail điều kiện thực thi/thứ tự của ca, không phải phát hiện xác nhận vượt quyền. Đã kiểm request mới sau thu quyền nhận403. Cần quyết định điểm tuyến tính hóa quyền theo transaction.
**Hướng sửa/kiểm lại:** Đối chiếu yêu cầu với cách khóaSerializable. Nếu bắt buộc quyền mới có hiệu lực trong lúc quét, cần thiết kế bước quét ngoài transaction và kiểm quyền lại trong transaction ghi; sau đó chạy lại đúng thứ tự.

**Chỗ đối chiếu code** (số dòng tại bản đã chạy):

- `Areas/HocTap/Services/TinNhanService.cs:100`.
- `Areas/HocTap/Services/TinNhanService.cs:105`.

Assertion thực tế:

```text
Không đạt thứ tự yêu cầu: thu quyền chưa commit trước nhả scanner (SQL1222); không kết luận bỏ qua quyền. Serializable giữ khóa, request gửi hoàn tất trước thu quyền.
```

## Các bước thực hiện và minh chứng từng ca

Áp dụng cho từng ca: xác nhận DB, tạo dữ liệu nền qua service/EF/SQL thật; chụp baseline; đăng nhập đúng actor; lấy antiforgery cùng client; gửi request; đọc HTTP trước redirect; mở DbContext mới đối chiếu toàn bộ dữ liệu và cleanup chỉ ID/tệp của fixture. Chi tiết ID, input, phiên bản và từng cột nằm trong KetQua.json và evidence JSON trong ZIP.

### IT-106 — Gửi tin qua HTTP và nhận realtime

**Endpoint theo bảng:** POST /HocTap/TinNhan/Gui; SignalR /cap-nhat

**Dữ liệu/tiền điều kiện:** H; ClientMessageId mới; G đăng ký kết nối hợp lệ.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-106; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. Chat CHT1 của C1; G/H được phép; X không quyền. Kho tệp test riêng, scanner sandbox và email sink; không mock service/chat/EF. Không dùng tệp malware thật. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: H; ClientMessageId mới; G đăng ký kết nối hợp lệ. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/TinNhan/Gui; SignalR /cap-nhat. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Gửi thành công sau commit; G nhận event đúng Id. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Một TinNhan; sự kiện không phát trước khi transaction commit. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_401b1fa4d298430189ab6e534c62d8df`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; HTTP 200; WebSocket SignalR thật của G tham gia group hội thoại. Dừng transaction trước commit và gọi worker thật: không nhận TinNhan sớm. Sau commit worker phát CapNhat đúng Id tin/room, SQL có một tin và một chat outbox.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":true,\"message\":null,\"redirectUrl\":\"/HocTap/TinNhan/Lop/32e9e386-17e0-f111-a50f-005056c00003\"}"
  },
  "sqlErrorNumbers": [],
  "signalREvent": {
    "maSuKien": "dc25c5ca-c222-4859-9a6b-3bab1f5da609",
    "loai": "TinNhan",
    "doiTuongId": "0ff1345a-cd37-47c5-be31-aa5119d67218",
    "hoiThoaiId": "32e9e386-17e0-f111-a50f-005056c00003"
  },
  "earlyEventBeforeCommit": false,
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[TinNhan] WHERE CuocHoiThoaiId=@Chat; SELECT * FROM dbo.[TepDinhKem] WHERE TinNhanId IN(SELECT Id FROM dbo.TinNhan WHERE CuocHoiThoaiId=@Chat); SELECT * FROM dbo.[CuocHoiThoai] WHERE Id=@Chat;
```

### IT-107 — Retry tin cùng mã

**Endpoint theo bảng:** POST /HocTap/TinNhan/Gui hai lần

**Dữ liệu/tiền điều kiện:** Cùng sender, ClientMessageId và nội dung.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-107; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. Chat CHT1 của C1; G/H được phép; X không quyền. Kho tệp test riêng, scanner sandbox và email sink; không mock service/chat/EF. Không dùng tệp malware thật. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Cùng sender, ClientMessageId và nội dung. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/TinNhan/Gui hai lần. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Trả cùng tin đã lưu. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: COUNT theo sender/client ID=1; không trùng thông báo. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_4e9a0edb014f49e5bb367ad819ca2e1f`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; Retry cùng MaThaoTac/nội dung nhận200; giữ nguyên Id tin đã có, không thêm TinNhan/chat outbox. API không trả Id tin trong JSON; đối chiếu Id bằng SQL.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":true,\"message\":null,\"redirectUrl\":\"/HocTap/TinNhan/Lop/bd1d9b80-17e0-f111-a50f-005056c00003\"}"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[TinNhan] WHERE CuocHoiThoaiId=@Chat; SELECT * FROM dbo.[TepDinhKem] WHERE TinNhanId IN(SELECT Id FROM dbo.TinNhan WHERE CuocHoiThoaiId=@Chat); SELECT * FROM dbo.[CuocHoiThoai] WHERE Id=@Chat;
```

### IT-108 — Retry đổi nội dung

**Endpoint theo bảng:** POST /HocTap/TinNhan/Gui

**Dữ liệu/tiền điều kiện:** Cùng ClientMessageId, nội dung khác.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-108; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. Chat CHT1 của C1; G/H được phép; X không quyền. Kho tệp test riêng, scanner sandbox và email sink; không mock service/chat/EF. Không dùng tệp malware thật. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Cùng ClientMessageId, nội dung khác. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/TinNhan/Gui. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: 409. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Tin cũ giữ nguyên. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_e90a826184ff4a57bfc562c522c2879f`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; Cùng MaThaoTac nhưng nội dung B nhận409; nội dung A, tệp và outbox giữ nguyên.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 409,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"M\\u00E3 thao t\\u00E1c \\u0111\\u00E3 d\\u00F9ng cho tin nh\\u1EAFn kh\\u00E1c.\",\"errors\":{\"\":[\"M\\u00E3 thao t\\u00E1c \\u0111\\u00E3 d\\u00F9ng cho tin nh\\u1EAFn kh\\u00E1c.\"]}}"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[TinNhan] WHERE CuocHoiThoaiId=@Chat; SELECT * FROM dbo.[TepDinhKem] WHERE TinNhanId IN(SELECT Id FROM dbo.TinNhan WHERE CuocHoiThoaiId=@Chat); SELECT * FROM dbo.[CuocHoiThoai] WHERE Id=@Chat;
```

### IT-109 — Người ngoài đọc lịch sử

**Endpoint theo bảng:** GET /HocTap/TinNhan/DuLieu?id=@Chat

**Dữ liệu/tiền điều kiện:** X có cookie hợp lệ nhưng ngoài hội thoại.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-109; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. Chat CHT1 của C1; G/H được phép; X không quyền. Kho tệp test riêng, scanner sandbox và email sink; không mock service/chat/EF. Không dùng tệp malware thật. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: X có cookie hợp lệ nhưng ngoài hội thoại. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi GET /HocTap/TinNhan/DuLieu?id=@Chat. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: 403/404 theo thiết kế tài nguyên. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Không có nội dung tin/tệp trong response. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_6cea9c8ce19a4293a6207e6633b6ef3c`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; H2 ngoài hội thoại GET DuLieu bị403; không trả nội dung riêng, không thay đổi SQL.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 403,
    "location": null,
    "contentType": "text/html; charset=utf-8",
    "body": "<!doctype html><html lang='vi'><meta charset='utf-8'><meta name='viewport' content='width=device-width'><title>EduConnect</title><link rel='stylesheet' href='/css/app.css'><main class='error-page'><h1>Chưa thể hoàn tất</h1><p role='alert'>Bạn chưa c&#243; quyền hoặc li&#234;n kết ph&#249; hợp để v&#224;o lớp n&#224;y.</p><a class='btn' href='/'>Về trang chủ</a><button onclick='history.back()'>Quay lại biểu mẫu</button></main></html>"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[TinNhan] WHERE CuocHoiThoaiId=@Chat; SELECT * FROM dbo.[TepDinhKem] WHERE TinNhanId IN(SELECT Id FROM dbo.TinNhan WHERE CuocHoiThoaiId=@Chat); SELECT * FROM dbo.[CuocHoiThoai] WHERE Id=@Chat;
```

### IT-110 — Phân trang tin cùng timestamp

**Endpoint theo bảng:** GET /HocTap/TinNhan/DuLieu

**Dữ liệu/tiền điều kiện:** Seed nhiều tin cùng GuiLuc, Id khác; before/after theo contract.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-110; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. Chat CHT1 của C1; G/H được phép; X không quyền. Kho tệp test riêng, scanner sandbox và email sink; không mock service/chat/EF. Không dùng tệp malware thật. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Seed nhiều tin cùng GuiLuc, Id khác; before/after theo contract. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi GET /HocTap/TinNhan/DuLieu. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Không mất/trùng tin khi lấy trang tiếp. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Thứ tự (GuiLuc,Id) ổn định. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_ee497dfe2dd743b88043fd9cc5467015`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; 120 tin cùng GuiLuc được đọc qua before thành50/50/20, đủ120 Id phân biệt. after trả đủ119 tie khác pivot, không lặp trong response. API chủ động trả overlap cùng thời điểm để client hợp nhất theo Id; không kiểm giao diện DOM.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "sqlErrorNumbers": [],
  "beforePages": [
    {
      "cursor": null,
      "ids": [
        "00000000-0000-0000-0000-000000000071",
        "00000000-0000-0000-0000-000000000072",
        "00000000-0000-0000-0000-000000000073",
        "00000000-0000-0000-0000-000000000074",
        "00000000-0000-0000-0000-000000000075",
        "00000000-0000-0000-0000-000000000076",
        "00000000-0000-0000-0000-000000000077",
        "00000000-0000-0000-0000-000000000078",
        "00000000-0000-0000-0000-000000000079",
        "00000000-0000-0000-0000-000000000080",
        "00000000-0000-0000-0000-000000000081",
        "00000000-0000-0000-0000-000000000082",
        "00000000-0000-0000-0000-000000000083",
        "00000000-0000-0000-0000-000000000084",
        "00000000-0000-0000-0000-000000000085",
        "00000000-0000-0000-0000-000000000086",
        "00000000-0000-0000-0000-000000000087",
        "00000000-0000-0000-0000-000000000088",
        "00000000-0000-0000-0000-000000000089",
        "00000000-0000-0000-0000-000000000090",
        "00000000-0000-0000-0000-000000000091",
        "00000000-0000-0000-0000-000000000092",
        "00000000-0000-0000-0000-000000000093",
        "00000000-0000-0000-0000-000000000094",
        "00000000-0000-0000-0000-000000000095",
        "00000000-0000-0000-0000-000000000096",
        "00000000-0000-0000-0000-000000000097",
        "00000000-0000-0000-0000-000000000098",
        "00000000-0000-0000-0000-000000000099",
        "00000000-0000-0000-0000-000000000100",
        "00000000-0000-0000-0000-000000000101",
        "00000000-0000-0000-0000-000000000102",
        "00000000-0000-0000-0000-000000000103",
        "00000000-0000-0000-0000-000000000104",
        "00000000-0000-0000-0000-000000000105",
        "00000000-0000-0000-0000-000000000106",
        "00000000-0000-0000-0000-000000000107",
        "00000000-0000-0000-0000-000000000108",
        "00000000-0000-0000-0000-000000000109",
        "00000000-0000-0000-0000-000000000110",
        "00000000-0000-0000-0000-000000000111",
        "00000000-0000-0000-0000-000000000112",
        "00000000-0000-0000-0000-000000000113",
        "00000000-0000-0000-0000-000000000114",
        "00000000-0000-0000-0000-000000000115",
        "00000000-0000-0000-0000-000000000116",
        "00000000-0000-0000-0000-000000000117",
        "00000000-0000-0000-0000-000000000118",
        "00000000-0000-0000-0000-000000000119",
        "00000000-0000-0000-0000-000000000120"
      ],
      "hasMore": true
    },
    {
      "cursor": "00000000-0000-0000-0000-000000000071",
      "ids": [
        "00000000-0000-0000-0000-000000000021",
        "00000000-0000-0000-0000-000000000022",
        "00000000-0000-0000-0000-000000000023",
        "00000000-0000-0000-0000-000000000024",
        "00000000-0000-0000-0000-000000000025",
        "00000000-0000-0000-0000-000000000026",
        "00000000-0000-0000-0000-000000000027",
        "00000000-0000-0000-0000-000000000028",
        "00000000-0000-0000-0000-000000000029",
        "00000000-0000-0000-0000-000000000030",
        "00000000-0000-0000-0000-000000000031",
        "00000000-0000-0000-0000-000000000032",
        "00000000-0000-0000-0000-000000000033",
        "00000000-0000-0000-0000-000000000034",
        "00000000-0000-0000-0000-000000000035",
        "00000000-0000-0000-0000-000000000036",
        "00000000-0000-0000-0000-000000000037",
        "00000000-0000-0000-0000-000000000038",
        "00000000-0000-0000-0000-000000000039",
        "00000000-0000-0000-0000-000000000040",
        "00000000-0000-0000-0000-000000000041",
        "00000000-0000-0000-0000-000000000042",
        "00000000-0000-0000-0000-000000000043",
        "00000000-0000-0000-0000-000000000044",
        "00000000-0000-0000-0000-000000000045",
        "00000000-0000-0000-0000-000000000046",
        "00000000-0000-0000-0000-000000000047",
        "00000000-0000-0000-0000-000000000048",
        "00000000-0000-0000-0000-000000000049",
        "00000000-0000-0000-0000-000000000050",
        "00000000-0000-0000-0000-000000000051",
        "00000000-0000-0000-0000-000000000052",
        "00000000-0000-0000-0000-000000000053",
        "00000000-0000-0000-0000-000000000054",
        "00000000-0000-0000-0000-000000000055",
        "00000000-0000-0000-0000-000000000056",
        "00000000-0000-0000-0000-000000000057",
        "00000000-0000-0000-0000-000000000058",
        "00000000-0000-0000-0000-000000000059",
        "00000000-0000-0000-0000-000000000060",
        "00000000-0000-0000-0000-000000000061",
        "00000000-0000-0000-0000-000000000062",
        "00000000-0000-0000-0000-000000000063",
        "00000000-0000-0000-0000-000000000064",
        "00000000-0000-0000-0000-000000000065",
        "00000000-0000-0000-0000-000000000066",
        "00000000-0000-0000-0000-000000000067",
        "00000000-0000-0000-0000-000000000068",
        "00000000-0000-0000-0000-000000000069",
        "00000000-0000-0000-0000-000000000070"
      ],
      "hasMore": true
    },
    {
      "cursor": "00000000-0000-0000-0000-000000000021",
      "ids": [
        "00000000-0000-0000-0000-000000000001",
        "00000000-0000-0000-0000-000000000002",
        "00000000-0000-0000-0000-000000000003",
        "00000000-0000-0000-0000-000000000004",
        "00000000-0000-0000-0000-000000000005",
        "00000000-0000-0000-0000-000000000006",
        "00000000-0000-0000-0000-000000000007",
        "00000000-0000-0000-0000-000000000008",
        "00000000-0000-0000-0000-000000000009",
        "00000000-0000-0000-0000-000000000010",
        "00000000-0000-0000-0000-000000000011",
        "00000000-0000-0000-0000-000000000012",
        "00000000-0000-0000-0000-000000000013",
        "00000000-0000-0000-0000-000000000014",
        "00000000-0000-0000-0000-000000000015",
        "00000000-0000-0000-0000-000000000016",
        "00000000-0000-0000-0000-000000000017",
        "00000000-0000-0000-0000-000000000018",
        "00000000-0000-0000-0000-000000000019",
        "00000000-0000-0000-0000-000000000020"
      ],
      "hasMore": false
    }
  ],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[TinNhan] WHERE CuocHoiThoaiId=@Chat; SELECT * FROM dbo.[TepDinhKem] WHERE TinNhanId IN(SELECT Id FROM dbo.TinNhan WHERE CuocHoiThoaiId=@Chat); SELECT * FROM dbo.[CuocHoiThoai] WHERE Id=@Chat;
```

### IT-111 — Lớp kết thúc chỉ đọc chat

**Endpoint theo bảng:** GET dữ liệu rồi POST /HocTap/TinNhan/Gui

**Dữ liệu/tiền điều kiện:** C1 DaKetThuc; H vẫn có quyền xem.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-111; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. Chat CHT1 của C1; G/H được phép; X không quyền. Kho tệp test riêng, scanner sandbox và email sink; không mock service/chat/EF. Không dùng tệp malware thật. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: C1 DaKetThuc; H vẫn có quyền xem. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi GET dữ liệu rồi POST /HocTap/TinNhan/Gui. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: GET đọc được; POST bị từ chối. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Lịch sử còn, không có tin mới. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_bc40f07559ed44a5a836d3ccb7f21bca`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; Lớp kết thúc vẫn GET DuLieu200 và đọc đúng tin cũ; POST gửi tin nhận403, lịch sử và toàn bộ snapshot giữ nguyên.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 403,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"B\\u1EA1n kh\\u00F4ng c\\u00F3 quy\\u1EC1n nh\\u1EAFn tin trong l\\u1EDBp n\\u00E0y.\",\"errors\":{\"\":[\"B\\u1EA1n kh\\u00F4ng c\\u00F3 quy\\u1EC1n nh\\u1EAFn tin trong l\\u1EDBp n\\u00E0y.\"]}}"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[TinNhan] WHERE CuocHoiThoaiId=@Chat; SELECT * FROM dbo.[TepDinhKem] WHERE TinNhanId IN(SELECT Id FROM dbo.TinNhan WHERE CuocHoiThoaiId=@Chat); SELECT * FROM dbo.[CuocHoiThoai] WHERE Id=@Chat;
```

### IT-112 — Upload 10 MB đúng biên

**Endpoint theo bảng:** POST /HocTap/TinNhan/Gui multipart

**Dữ liệu/tiền điều kiện:** Một PDF sạch đúng 10000000 byte; nội dung hợp lệ.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-112; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. Chat CHT1 của C1; G/H được phép; X không quyền. Kho tệp test riêng, scanner sandbox và email sink; không mock service/chat/EF. Không dùng tệp malware thật. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Một PDF sạch đúng 10000000 byte; nội dung hợp lệ. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/TinNhan/Gui multipart. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Qua kiểm tra dung lượng nếu nội dung tệp hợp lệ. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Metadata KichThuoc đúng; đúng liên kết tin; chỉ truy cập sau sạch. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_0253ba9f5ad04d00957b6da3812f3f6c`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; PDF đúng10.000.000 byte được chấp nhận200; metadata Sach, đúng chủ/link tin/kích thước. GET Tai200 trả đủ byte, SHA256 khớp metadata.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":true,\"message\":null,\"redirectUrl\":\"/HocTap/TinNhan/Lop/0fe9e386-17e0-f111-a50f-005056c00003\"}"
  },
  "sqlErrorNumbers": [],
  "download": {
    "status": 200,
    "length": 10000000,
    "sha256": "8FF43BB4E3407B994C70BA50D6AB42C42C97ACBBDBF02F5E029607211781427C"
  },
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[TinNhan] WHERE CuocHoiThoaiId=@Chat; SELECT * FROM dbo.[TepDinhKem] WHERE TinNhanId IN(SELECT Id FROM dbo.TinNhan WHERE CuocHoiThoaiId=@Chat); SELECT * FROM dbo.[CuocHoiThoai] WHERE Id=@Chat;
```

### IT-113 — Upload vượt biên

**Endpoint theo bảng:** POST /HocTap/TinNhan/Gui multipart

**Dữ liệu/tiền điều kiện:** Một tệp 10000001 byte.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-113; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. Chat CHT1 của C1; G/H được phép; X không quyền. Kho tệp test riêng, scanner sandbox và email sink; không mock service/chat/EF. Không dùng tệp malware thật. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Một tệp 10000001 byte. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/TinNhan/Gui multipart. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Lỗi dung lượng; không thành công. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Không tin/tệp gắn một phần; tệp tạm được dọn theo cơ chế. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_be03a0cc9dad4e85b324067053b288d9`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; PDF10.000.001 byte bị422; không tăng message/metadata/outbox và không có tệp lưu dở.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 400,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"M\\u1ED7i t\\u1EC7p ph\\u1EA3i c\\u00F3 d\\u1EEF li\\u1EC7u v\\u00E0 kh\\u00F4ng v\\u01B0\\u1EE3t qu\\u00E1 10 MB.\",\"errors\":{\"Files\":[\"M\\u1ED7i t\\u1EC7p ph\\u1EA3i c\\u00F3 d\\u1EEF li\\u1EC7u v\\u00E0 kh\\u00F4ng v\\u01B0\\u1EE3t qu\\u00E1 10 MB.\"]}}"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[TinNhan] WHERE CuocHoiThoaiId=@Chat; SELECT * FROM dbo.[TepDinhKem] WHERE TinNhanId IN(SELECT Id FROM dbo.TinNhan WHERE CuocHoiThoaiId=@Chat); SELECT * FROM dbo.[CuocHoiThoai] WHERE Id=@Chat;
```

### IT-114 — Sáu tệp một lượt

**Endpoint theo bảng:** POST /HocTap/TinNhan/Gui multipart

**Dữ liệu/tiền điều kiện:** 6 PDF nhỏ hợp lệ.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-114; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. Chat CHT1 của C1; G/H được phép; X không quyền. Kho tệp test riêng, scanner sandbox và email sink; không mock service/chat/EF. Không dùng tệp malware thật. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: 6 PDF nhỏ hợp lệ. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/TinNhan/Gui multipart. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Lỗi vượt 5 tệp. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Không lưu một phần tin. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_950e064a966646db90a91fd7929e2f50`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; Gửi6 PDF nhỏ bị422 do tối đa5 tệp; không tạo một phần message/metadata/outbox hoặc tệp vật lý.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 400,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"Ch\\u1ECDn t\\u1ED1i \\u0111a 5 t\\u1EC7p m\\u1ED7i l\\u1EA7n g\\u1EEDi.\",\"errors\":{\"Files\":[\"Ch\\u1ECDn t\\u1ED1i \\u0111a 5 t\\u1EC7p m\\u1ED7i l\\u1EA7n g\\u1EEDi.\"]}}"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[TinNhan] WHERE CuocHoiThoaiId=@Chat; SELECT * FROM dbo.[TepDinhKem] WHERE TinNhanId IN(SELECT Id FROM dbo.TinNhan WHERE CuocHoiThoaiId=@Chat); SELECT * FROM dbo.[CuocHoiThoai] WHERE Id=@Chat;
```

### IT-115 — Đuôi hợp lệ nội dung sai

**Endpoint theo bảng:** POST /HocTap/TinNhan/Gui multipart

**Dữ liệu/tiền điều kiện:** Đuôi .pdf nhưng bytes không hợp chữ ký PDF; scanner sandbox.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-115; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. Chat CHT1 của C1; G/H được phép; X không quyền. Kho tệp test riêng, scanner sandbox và email sink; không mock service/chat/EF. Không dùng tệp malware thật. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Đuôi .pdf nhưng bytes không hợp chữ ký PDF; scanner sandbox. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/TinNhan/Gui multipart. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Từ chối theo kiểm tra nội dung. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Không phục vụ tệp như PDF sạch. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_ab102e3636274a4f973b488b55d516fc`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; Tên.pdf nhưng sai chữ ký bị422 trước scanner; scannerCalls=0, không tệp/metadata/message mới.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 400,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"N\\u1ED9i dung t\\u1EC7p kh\\u00F4ng kh\\u1EDBp \\u0111\\u1ECBnh d\\u1EA1ng.\",\"errors\":{\"Files\":[\"N\\u1ED9i dung t\\u1EC7p kh\\u00F4ng kh\\u1EDBp \\u0111\\u1ECBnh d\\u1EA1ng.\"]}}"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[TinNhan] WHERE CuocHoiThoaiId=@Chat; SELECT * FROM dbo.[TepDinhKem] WHERE TinNhanId IN(SELECT Id FROM dbo.TinNhan WHERE CuocHoiThoaiId=@Chat); SELECT * FROM dbo.[CuocHoiThoai] WHERE Id=@Chat;
```

### IT-116 — Scanner báo cách ly

**Endpoint theo bảng:** POST /HocTap/TinNhan/Gui multipart

**Dữ liệu/tiền điều kiện:** Scanner sandbox trả kết quả nguy cơ cho fixture vô hại.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-116; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. Chat CHT1 của C1; G/H được phép; X không quyền. Kho tệp test riêng, scanner sandbox và email sink; không mock service/chat/EF. Không dùng tệp malware thật. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Scanner sandbox trả kết quả nguy cơ cho fixture vô hại. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi POST /HocTap/TinNhan/Gui multipart. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Không phát tệp như tệp sạch. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: TrangThaiQuet CachLy/TuChoi theo hợp đồng; không tải nội dung cách ly. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_a684222faa694a0888bf4455eb62eae6`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; Scanner sandbox từ chối fixture vô hại; HTTP422, không tệp sạch hay message và không tệp vật lý. Tuy nhiên không có metadata CachLy/TuChoi như kịch bản, nên Fail phần lưu dấu vết từ chối; không có bằng chứng tệp nguy hiểm được phục vụ.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 400,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"Scanner sandbox t\\u1EEB ch\\u1ED1i fixture v\\u00F4 h\\u1EA1i.\",\"errors\":{\"Files\":[\"Scanner sandbox t\\u1EEB ch\\u1ED1i fixture v\\u00F4 h\\u1EA1i.\"]}}"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [
    "Kịch bản yêu cầu metadata CachLy/TuChoi, nhưng scanner ném lỗi trước khi tạo metadata; từ chối an toàn song thiếu bản ghi cách ly/từ chối."
  ],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[TinNhan] WHERE CuocHoiThoaiId=@Chat; SELECT * FROM dbo.[TepDinhKem] WHERE TinNhanId IN(SELECT Id FROM dbo.TinNhan WHERE CuocHoiThoaiId=@Chat); SELECT * FROM dbo.[CuocHoiThoai] WHERE Id=@Chat;
```

### IT-117 — Tải tệp không quyền

**Endpoint theo bảng:** GET /HocTap/TinNhan/Tai/@File

**Dữ liệu/tiền điều kiện:** X dùng Id tệp của Chat H/G.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-117; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. Chat CHT1 của C1; G/H được phép; X không quyền. Kho tệp test riêng, scanner sandbox và email sink; không mock service/chat/EF. Không dùng tệp malware thật. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: X dùng Id tệp của Chat H/G. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi GET /HocTap/TinNhan/Tai/@File. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: 403/404; không trả bytes tệp. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Không đổi chủ/tình trạng tệp. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_b043e94c3ae140ad9e7ebb88dc297ac4`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; H2 ngoài hội thoại GET Tai của tệp sạch có thật bị403; không trả bytes PDF, metadata/tệp/SQL không đổi.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 403,
    "location": null,
    "contentType": "text/html; charset=utf-8",
    "body": "<!doctype html><html lang='vi'><meta charset='utf-8'><meta name='viewport' content='width=device-width'><title>EduConnect</title><link rel='stylesheet' href='/css/app.css'><main class='error-page'><h1>Chưa thể hoàn tất</h1><p role='alert'>Bạn chưa c&#243; quyền hoặc li&#234;n kết ph&#249; hợp để v&#224;o lớp n&#224;y.</p><a class='btn' href='/'>Về trang chủ</a><button onclick='history.back()'>Quay lại biểu mẫu</button></main></html>"
  },
  "sqlErrorNumbers": [],
  "assertionDifferences": [],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[TinNhan] WHERE CuocHoiThoaiId=@Chat; SELECT * FROM dbo.[TepDinhKem] WHERE TinNhanId IN(SELECT Id FROM dbo.TinNhan WHERE CuocHoiThoaiId=@Chat); SELECT * FROM dbo.[CuocHoiThoai] WHERE Id=@Chat;
```

### IT-118 — Thu hồi quyền trong lúc quét

**Endpoint theo bảng:** Upload bắt đầu rồi admin thu hồi quyền trước gắn tệp

**Dữ liệu/tiền điều kiện:** Điều phối scanner test chờ; commit thu hồi quyền rồi cho scan hoàn tất.

**Bước trong bảng:**

B1. Khởi tạo fixture cho ca IT-118; đối chiếu SELECT DB_NAME(), @@SERVERNAME; ghi phiên bản app và SHA-256 nguồn SQL. B2. Chat CHT1 của C1; G/H được phép; X không quyền. Kho tệp test riêng, scanner sandbox và email sink; không mock service/chat/EF. Không dùng tệp malware thật. Đọc trạng thái ban đầu bằng truy vấn cột SQL; lưu số dòng, RowVersion và các ID liên quan. B3. Thiết lập điều kiện riêng: Điều phối scanner test chờ; commit thu hồi quyền rồi cho scan hoàn tất. B4. Tạo HttpClient riêng cho đúng tài khoản, AllowAutoRedirect=false. Ca đăng ký/đăng nhập/quên hoặc đặt lại mật khẩu và ca kiểm tra chưa đăng nhập dùng client anonymous. Các ca nghiệp vụ còn lại đăng nhập thật bằng Identity/cookie và phiên SQL. GET form cùng client để lấy cookie antiforgery và __RequestVerificationToken. B5. Gửi Upload bắt đầu rồi admin thu hồi quyền trước gắn tệp. Nếu cột phản hồi/ghi chú yêu cầu form thường thì KHÔNG thêm header AJAX; các POST khác thêm X-Requested-With: XMLHttpRequest; dùng form-urlencoded đúng tên tham số, multipart cho tệp. Đưa dữ liệu cột Đầu vào vào request; bỏ token/cookie chỉ khi ca yêu cầu. B6. Thu status, Content-Type, Location hoặc JSON/body trước khi theo redirect. Đối chiếu: Bị từ chối ở kiểm tra quyền cuối. B7. Mở DI scope/DbContext AsNoTracking hoặc connection mới sau request để chạy SQL cột kiểm tra. Đối chiếu: Không gắn tệp/bài/tin sau thu hồi; file tạm xử lý đúng. B8. Đối chiếu cả dữ liệu phải đổi và dữ liệu không được đổi; kiểm tra lỗi không sinh bản ghi mồ côi hoặc duplicate. Với outbox, chờ có giới hạn và đọc lại, không sleep tùy ý rồi kết luận. B9. Ghi actual status/body/SQL trước–sau, thời gian, test name và TRX; chỉ đánh Đạt sau assertions. Reset database fixture/snapshot và tệp test theo thứ tự FK. Không trông chờ rollback transaction của test để hủy HTTP request đã commit.

**Các bước và kết quả đã quan sát:**

1. Tạo fixture có tag `IT0203IT_84368f368b5a46a0936a4419d5d7b9c3`, các actor/Id lớp/bài/tin ghi trong JSON; chụp `before` trước thao tác cần kiểm.
2. Thực hiện đúng nhánh ca; Đã dừng request tại scanner. UPDATE thu quyền trên kết nối SQL khác bị lock timeout1222 vì transaction Serializable giữ khóa, nên thu quyền chưa commit trước nhả scanner. Request gửi hoàn tất trước thay đổi quyền. Sau khi thu quyền commit, POST tiếp theo403. Không đạt thứ tự ca yêu cầu, chưa đủ bằng chứng kết luận lỗi vượt quyền.
3. Đọc `after`, kiểm điều kiện thay đổi/không đổi; giữ `sqlErrorNumbers`, HTTP và các probe phụ để phân biệt lỗi validation, quyền, xung đột và SQL.
4. Dừng host, xóa dữ liệu theo ID sở hữu rồi xóa từng tệp sandbox. Kết quả cleanup nằm trong evidence.

**HTTP / SQL bổ sung:**

```json
{
  "http": {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":true,\"message\":null,\"redirectUrl\":\"/HocTap/TinNhan/Lop/a81d9b80-17e0-f111-a50f-005056c00003\"}"
  },
  "sqlErrorNumbers": [],
  "revocationCommittedBeforeScanRelease": false,
  "revocationSqlNumber": 1222,
  "afterRevocationHttp": 403,
  "assertionDifferences": [
    "Không đạt thứ tự yêu cầu: thu quyền chưa commit trước nhả scanner (SQL1222); không kết luận bỏ qua quyền. Serializable giữ khóa, request gửi hoàn tất trước thu quyền."
  ],
  "cleanup": "Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.",
  "fileCleanup": "Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy."
}
```

**Truy vấn gốc trong bảng** (thay placeholder bằng Id ở JSON, không chạy trên DB ứng dụng):

```sql
SELECT * FROM dbo.[TinNhan] WHERE CuocHoiThoaiId=@Chat; SELECT * FROM dbo.[TepDinhKem] WHERE TinNhanId IN(SELECT Id FROM dbo.TinNhan WHERE CuocHoiThoaiId=@Chat); SELECT * FROM dbo.[CuocHoiThoai] WHERE Id=@Chat;
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
dotnet test tests/Integration0203/Integration0203.csproj --filter "Suite=IT09TepChat" --logger "trx;LogFileName=Tep-Chat-Integration.trx" --results-directory "TestResults/09-Tep-Chat-Integration"
dotnet test tests/Integration0203/Integration0203.csproj --filter "FullyQualifiedName~Integration0203Tests.IT_106"
```

Chạy hai nhóm tuần tự trên cùng DB. Nếu app khóa thư mục build, dừng app hoặc build sang BaseOutputPath riêng với UseAppHost=false rồi chạy dotnet vstest DLL. Test chỉ chấp nhận database kiểm thử nêu trên.

## Xác minh sau chạy

**Hồi quy71 ca nhóm02–07:63 Pass,8 Fail, từng mã không đổi.** Không ghi đè kết quả nhóm cũ; TRX hồi quy đính kèm HoiQuy-02-07.trx. Tám Fail cũ: IT-022, IT-023, IT-025, IT-034, IT-037, IT-057, IT-069, IT-083.

32 ca mới đều đi hết cleanup, không có lỗi khởi tạo/cleanup trong lượt chính. XacMinh-SQL.txt xác nhận còn0 tài khoản/lớp fixture và0 tệp sandbox sau lượt chính và hồi quy. Các nhật ký bảo mật anonymous có thể còn trong database kiểm thử.

Lượt đầu có lỗi phép so chuỗi JSON của test (text Unicode được escape), đã sửa bằng parse JSON. Cũng sửa test khi kết quả chấm vắng để ghi assertion thay vì phát sinh exception; tách input nguyên cho hai ca xung đột. Kết quả chính thức là chạy lại đầy đủ với probe binding. Build cuối0 warning/0 error. Không đổi kỳ vọng để cho Pass, không sửa production.

KetQua.json lưu điều kiện, kỳ vọng và snapshot đầy đủ; TRX lưu lỗi/assertion cùng OBSERVATION_JSON. Ma_nguon_Integration.zip có project test, các nguồn tham chiếu và evidence từng ca. SHA256.json dùng đối chiếu file.
