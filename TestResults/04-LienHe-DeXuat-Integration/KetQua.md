# Kết quả Integration 04 Liên hệ và đề xuất

**13 ca IT-042–IT-054: 13 Pass, 0 Fail, 0 Skip/Blocked.**

Nguồn: Integration Test.xlsx, sheet 04 Lien he De xuat. Đã đối chiếu lại file hiện tại, giữ nguyên ba tiêu chuẩn và kỳ vọng từng mã. Kết quả áp dụng cho fixture và phiên bản nguồn được lưu kèm; không kết luận toàn bộ hệ thống đã đạt.

## Phạm vi và môi trường

CSDL thực thi: `LAPTOP-3L2V1AFT\SQLEXPRESS / EduConnect_Verification_IT0203`. Không tạo lại CSDL và không sửa code nghiệp vụ/schema để làm Pass. HTTP được gửi qua TestServer; đăng nhập Identity, cookie, phiên SQL, antiforgery, binding, controller, service, EF và procedure đều thật.

Host kiểm thử dùng Development, tắt DemoPublishProfiles. Dừng BaoTriWorker/XuLySuKienWorker để không đổi dữ liệu trong lúc so sánh; kiểm bản ghi notice/outbox, không kiểm phát realtime/email hoặc consumer chạy thành công. Dùng Windows integrated operator cho DB test; không xác nhận quyền tối thiểu của tài khoản DB ứng dụng.

Mỗi Fact có fixture mới, tag/ID riêng. H là học sinh gửi, G là gia sư, H2 là người ngoài; N1 gồm M1/M2 theo ca. Đề xuất nền: hai buổi/tuần, 60 phút/buổi, ngày bắt đầu sau ba ngày, 150000 VND/Gio; thứ 1/3 19:00–20:00. Chuẩn bị fixture có thể gọi service thật trước khi chụp baseline; thao tác được kiểm đều là HTTP.

## Kết quả từng ca

| Mã | Tình huống | Tiêu chuẩn | Kết quả | Quan sát thực tế |
| --- | --- | --- | --- | --- |
| IT-042 | Gửi liên hệ hợp lệ | Đúng đắn | Pass | HTTP 200 JSON ok=true, URL nội bộ /KetNoi/KetNoi/LienHe. SQL có đúng một ChoPhanHoi, lời nhắn Xin học đã trim, notice/outbox đúng gia sư và mã liên hệ; chưa có lớp. |
| IT-043 | Thiếu lời nhắn | Đầy đủ | Pass | HTTP 422 JSON errors có LoiNhan; trước/sau không có liên hệ, yêu cầu, thông báo hoặc sự kiện mới. |
| IT-044 | Liên hệ trùng đồng thời | Đúng đắn | Pass | Hai client, token riêng, cùng bắt đầu qua barrier: nhận 400 và 200. SQL chỉ có một liên hệ hoạt động và một cặp notice/outbox; unique index UQ_DangDung_YeuCauLienHe tồn tại. |
| IT-045 | Tự gửi liên hệ | Xác thực | Pass | HTTP 400; không thêm liên hệ, yêu cầu, hội thoại, lớp, notice/outbox. |
| IT-046 | Chấp nhận liên hệ tạo hội thoại | Đúng đắn | Pass | HTTP 200 JSON thành công. Liên hệ ChapNhan, đúng người phản hồi; có đúng một hội thoại TruocKetNoi gắn liên hệ; chưa tạo yêu cầu/lớp. |
| IT-047 | Sai người phản hồi liên hệ | Xác thực | Pass | HTTP 403; bản ghi liên hệ và RowVersion giữ nguyên; không thêm hội thoại hoặc dữ liệu nghiệp vụ. |
| IT-048 | Bên học gửi yêu cầu kết nối | Đúng đắn | Pass | HTTP 200 JSON thành công. Có đúng một ChoGiaSu, đúng H/LH1/N1; HetHanLuc bằng GuiLuc+72 giờ, chưa tạo lớp. |
| IT-049 | Gia sư tạo đề xuất đầy đủ | Đầy đủ | Pass | HTTP 200 JSON thành công. Một ChoBenHoc, đúng gia sư/môn/phí 150000 VND/Gio; hai khung thứ 1/3, mỗi khung 60 phút; hạn DeXuatLuc+72 giờ. |
| IT-050 | Môn không thuộc nhu cầu nhiều môn | Đúng đắn | Pass | HTTP 422 JSON lỗi môn không thuộc nhu cầu. SQL51131 thật được ghi nhận; không thêm yêu cầu/khung hoặc thay đổi notice/outbox. M3 có trong hồ sơ gia sư để chỉ kiểm điều kiện không thuộc N1. |
| IT-051 | Sửa đề xuất RV cũ | Đúng đắn | Pass | HTTP 422 JSON lỗi phiên bản; SQL51139 thật. Gửi RowVersion từ lần lưu thứ nhất sau khi lần thứ hai đã cập nhật; yêu cầu, RowVersion và hai khung hiện tại giữ nguyên. |
| IT-052 | Bỏ danh sách lịch khỏi form | Đầy đủ | Pass | HTTP 422 JSON lỗi cần 1–35 khung giờ; không dùng lịch mặc định để thành công, không gọi procedure tạo đề xuất, dữ liệu trước/sau giữ nguyên. |
| IT-053 | Không đăng nhập gọi thao tác ghi | Xác thực | Pass | AJAX HTTP 401, không Location và không HTML đăng nhập; dữ liệu nghiệp vụ giữ nguyên. Token lấy từ trang đăng nhập bằng client anonymous. |
| IT-054 | Thiếu antiforgery token | Xác thực | Pass | HTTP 400 do antiforgery; giữ cookie đăng nhập nhưng bỏ token. Không thêm hoặc thay đổi dữ liệu nghiệp vụ; không áp đặt JSON của action. |

## Chi tiết bước thực thi và minh chứng

### IT-042 — Gửi liên hệ hợp lệ

**Tiêu chuẩn:** Đúng đắn. **Đích:** `POST /KetNoi/KetNoi/GuiLienHe`.

**Dữ liệu riêng:** H gửi G; LoiNhan=" Xin học ".

**Các bước đã thực hiện:**

1. Đối chiếu DB_NAME/@@SERVERNAME; tạo fixture có tag và bốn tài khoản hoạt động, môn/hồ sơ đủ điều kiện; service dùng SQL thật để chuẩn bị khi cần.
2. Chụp Contacts/Requests/Slots/Rooms/Classes/Notices/Events qua DbContext mới AsNoTracking trước thao tác.
3. Tạo HttpClient riêng AllowAutoRedirect=false; đăng nhập qua Identity/cookie và phiên SQL thật, trừ IT-053 anonymous. GET form cùng client để lấy antiforgery, trừ IT-054.
4. Gửi DoiTuongId=G và LoiNhan=" Xin học ".
5. Thu status, Content-Type, Location hoặc JSON/body trước redirect; so với phản hồi kỳ vọng.
6. Mở DbContext mới đọc lại dữ liệu nghiệp vụ, so cả bản ghi phải đổi và không được đổi; kiểm số lượng, ID, RowVersion, thông báo/outbox hoặc lỗi SQL theo ca.
7. Dừng host kiểm thử, xóa đúng ID fixture theo thứ tự khóa ngoại; xóa khung trước yêu cầu lớp. Lưu quan sát và TRX, không xóa dữ liệu ngoài fixture.

**Kỳ vọng HTTP:** 200 JSON ok=true, redirectUrl nội bộ.
**Kỳ vọng SQL:** ChoPhanHoi; lời nhắn trim; notice/outbox đúng người nhận; không tạo lớp.

**Thực tế:** HTTP 200 JSON ok=true, URL nội bộ /KetNoi/KetNoi/LienHe. SQL có đúng một ChoPhanHoi, lời nhắn Xin học đã trim, notice/outbox đúng gia sư và mã liên hệ; chưa có lớp.

**Số dòng baseline và sau request** gồm cả dữ liệu chuẩn bị, không đồng nhất tổng notice/outbox với số mới thêm:

```json
{
  "before": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 0,
    "Events": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 1,
    "Events": 1
  },
  "sqlErrorNumbers": []
}
```

**HTTP thu được:**

```json
{
  "status": 200,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": true,
    "message": "Đã gửi yêu cầu liên hệ.",
    "redirectUrl": "/KetNoi/KetNoi/LienHe"
  }
}
```

**Truy vấn đối chiếu theo kịch bản** dùng ID fixture thực trong KetQua.json:

```sql
SELECT * FROM dbo.[YeuCauLienHe] WHERE Id=@Contact; SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE LienHeId=@Contact; SELECT * FROM dbo.[KhungGioDeXuat] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[CuocHoiThoai] WHERE LienHeId=@Contact;
```

### IT-043 — Thiếu lời nhắn

**Tiêu chuẩn:** Đầy đủ. **Đích:** `POST /KetNoi/KetNoi/GuiLienHe`.

**Dữ liệu riêng:** LoiNhan rỗng.

**Các bước đã thực hiện:**

1. Đối chiếu DB_NAME/@@SERVERNAME; tạo fixture có tag và bốn tài khoản hoạt động, môn/hồ sơ đủ điều kiện; service dùng SQL thật để chuẩn bị khi cần.
2. Chụp Contacts/Requests/Slots/Rooms/Classes/Notices/Events qua DbContext mới AsNoTracking trước thao tác.
3. Tạo HttpClient riêng AllowAutoRedirect=false; đăng nhập qua Identity/cookie và phiên SQL thật, trừ IT-053 anonymous. GET form cùng client để lấy antiforgery, trừ IT-054.
4. Gửi cùng form nhưng LoiNhan rỗng.
5. Thu status, Content-Type, Location hoặc JSON/body trước redirect; so với phản hồi kỳ vọng.
6. Mở DbContext mới đọc lại dữ liệu nghiệp vụ, so cả bản ghi phải đổi và không được đổi; kiểm số lượng, ID, RowVersion, thông báo/outbox hoặc lỗi SQL theo ca.
7. Dừng host kiểm thử, xóa đúng ID fixture theo thứ tự khóa ngoại; xóa khung trước yêu cầu lớp. Lưu quan sát và TRX, không xóa dữ liệu ngoài fixture.

**Kỳ vọng HTTP:** 422 JSON errors có LoiNhan.
**Kỳ vọng SQL:** Không thêm yêu cầu hoặc notice nghiệp vụ.

**Thực tế:** HTTP 422 JSON errors có LoiNhan; trước/sau không có liên hệ, yêu cầu, thông báo hoặc sự kiện mới.

**Số dòng baseline và sau request** gồm cả dữ liệu chuẩn bị, không đồng nhất tổng notice/outbox với số mới thêm:

```json
{
  "before": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 0,
    "Events": 0
  },
  "after": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 0,
    "Events": 0
  },
  "sqlErrorNumbers": []
}
```

**HTTP thu được:**

```json
{
  "status": 422,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": false,
    "message": "Kiểm tra lại thông tin đã nhập.",
    "errors": {
      "LoiNhan": [
        "Lời nhắn không được để trống."
      ]
    }
  }
}
```

**Truy vấn đối chiếu theo kịch bản** dùng ID fixture thực trong KetQua.json:

```sql
SELECT * FROM dbo.[YeuCauLienHe] WHERE Id=@Contact; SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE LienHeId=@Contact; SELECT * FROM dbo.[KhungGioDeXuat] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[CuocHoiThoai] WHERE LienHeId=@Contact;
```

### IT-044 — Liên hệ trùng đồng thời

**Tiêu chuẩn:** Đúng đắn. **Đích:** `Hai HTTP POST /KetNoi/KetNoi/GuiLienHe`.

**Dữ liệu riêng:** Hai client cùng H/G gửi cùng lúc; token riêng.

**Các bước đã thực hiện:**

1. Đối chiếu DB_NAME/@@SERVERNAME; tạo fixture có tag và bốn tài khoản hoạt động, môn/hồ sơ đủ điều kiện; service dùng SQL thật để chuẩn bị khi cần.
2. Chụp Contacts/Requests/Slots/Rooms/Classes/Notices/Events qua DbContext mới AsNoTracking trước thao tác.
3. Tạo HttpClient riêng AllowAutoRedirect=false; đăng nhập qua Identity/cookie và phiên SQL thật, trừ IT-053 anonymous. GET form cùng client để lấy antiforgery, trừ IT-054.
4. Tạo hai client đăng nhập H, lấy hai token riêng; dùng TaskCompletionSource làm barrier rồi Task.WhenAll hai POST. Đọc cả hai response trước khi kiểm dữ liệu.
5. Thu status, Content-Type, Location hoặc JSON/body trước redirect; so với phản hồi kỳ vọng.
6. Mở DbContext mới đọc lại dữ liệu nghiệp vụ, so cả bản ghi phải đổi và không được đổi; kiểm số lượng, ID, RowVersion, thông báo/outbox hoặc lỗi SQL theo ca.
7. Dừng host kiểm thử, xóa đúng ID fixture theo thứ tự khóa ngoại; xóa khung trước yêu cầu lớp. Lưu quan sát và TRX, không xóa dữ liệu ngoài fixture.

**Kỳ vọng HTTP:** Một thành công, request còn lại bị từ chối/xung đột.
**Kỳ vọng SQL:** Unique UQ_DangDung_YeuCauLienHe bảo vệ một liên hệ hoạt động.

**Thực tế:** Hai client, token riêng, cùng bắt đầu qua barrier: nhận 400 và 200. SQL chỉ có một liên hệ hoạt động và một cặp notice/outbox; unique index UQ_DangDung_YeuCauLienHe tồn tại.

**Số dòng baseline và sau request** gồm cả dữ liệu chuẩn bị, không đồng nhất tổng notice/outbox với số mới thêm:

```json
{
  "before": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 0,
    "Events": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 1,
    "Events": 1
  },
  "sqlErrorNumbers": []
}
```

**HTTP thu được:**

```json
[
  {
    "status": 400,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":false,\"message\":\"Hai b\\u00EAn \\u0111\\u00E3 li\\u00EAn h\\u1EC7 ho\\u1EB7c \\u0111ang ch\\u1EDD ph\\u1EA3n h\\u1ED3i.\",\"errors\":{\"\":[\"Hai b\\u00EAn \\u0111\\u00E3 li\\u00EAn h\\u1EC7 ho\\u1EB7c \\u0111ang ch\\u1EDD ph\\u1EA3n h\\u1ED3i.\"]}}"
  },
  {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": "{\"ok\":true,\"message\":\"\\u0110\\u00E3 g\\u1EEDi y\\u00EAu c\\u1EA7u li\\u00EAn h\\u1EC7.\",\"redirectUrl\":\"/KetNoi/KetNoi/LienHe\"}"
  }
]
```

**Truy vấn đối chiếu theo kịch bản** dùng ID fixture thực trong KetQua.json:

```sql
SELECT * FROM dbo.[YeuCauLienHe] WHERE Id=@Contact; SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE LienHeId=@Contact; SELECT * FROM dbo.[KhungGioDeXuat] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[CuocHoiThoai] WHERE LienHeId=@Contact;
```

### IT-045 — Tự gửi liên hệ

**Tiêu chuẩn:** Xác thực. **Đích:** `POST /KetNoi/KetNoi/GuiLienHe`.

**Dữ liệu riêng:** DoiTuongId bằng actor.

**Các bước đã thực hiện:**

1. Đối chiếu DB_NAME/@@SERVERNAME; tạo fixture có tag và bốn tài khoản hoạt động, môn/hồ sơ đủ điều kiện; service dùng SQL thật để chuẩn bị khi cần.
2. Chụp Contacts/Requests/Slots/Rooms/Classes/Notices/Events qua DbContext mới AsNoTracking trước thao tác.
3. Tạo HttpClient riêng AllowAutoRedirect=false; đăng nhập qua Identity/cookie và phiên SQL thật, trừ IT-053 anonymous. GET form cùng client để lấy antiforgery, trừ IT-054.
4. Gửi DoiTuongId=H trong phiên H.
5. Thu status, Content-Type, Location hoặc JSON/body trước redirect; so với phản hồi kỳ vọng.
6. Mở DbContext mới đọc lại dữ liệu nghiệp vụ, so cả bản ghi phải đổi và không được đổi; kiểm số lượng, ID, RowVersion, thông báo/outbox hoặc lỗi SQL theo ca.
7. Dừng host kiểm thử, xóa đúng ID fixture theo thứ tự khóa ngoại; xóa khung trước yêu cầu lớp. Lưu quan sát và TRX, không xóa dữ liệu ngoài fixture.

**Kỳ vọng HTTP:** 400.0
**Kỳ vọng SQL:** Không tạo yêu cầu.

**Thực tế:** HTTP 400; không thêm liên hệ, yêu cầu, hội thoại, lớp, notice/outbox.

**Số dòng baseline và sau request** gồm cả dữ liệu chuẩn bị, không đồng nhất tổng notice/outbox với số mới thêm:

```json
{
  "before": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 0,
    "Events": 0
  },
  "after": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 0,
    "Events": 0
  },
  "sqlErrorNumbers": []
}
```

**HTTP thu được:**

```json
{
  "status": 400,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": false,
    "message": "Liên hệ cần một gia sư và một bên học.",
    "errors": {
      "": [
        "Liên hệ cần một gia sư và một bên học."
      ]
    }
  }
}
```

**Truy vấn đối chiếu theo kịch bản** dùng ID fixture thực trong KetQua.json:

```sql
SELECT * FROM dbo.[YeuCauLienHe] WHERE Id=@Contact; SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE LienHeId=@Contact; SELECT * FROM dbo.[KhungGioDeXuat] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[CuocHoiThoai] WHERE LienHeId=@Contact;
```

### IT-046 — Chấp nhận liên hệ tạo hội thoại

**Tiêu chuẩn:** Đúng đắn. **Đích:** `POST /KetNoi/KetNoi/PhanHoiLienHe`.

**Dữ liệu riêng:** Đúng người nhận, action theo hợp đồng chấp nhận, RV hiện tại.

**Các bước đã thực hiện:**

1. Đối chiếu DB_NAME/@@SERVERNAME; tạo fixture có tag và bốn tài khoản hoạt động, môn/hồ sơ đủ điều kiện; service dùng SQL thật để chuẩn bị khi cần.
2. Chụp Contacts/Requests/Slots/Rooms/Classes/Notices/Events qua DbContext mới AsNoTracking trước thao tác.
3. Tạo HttpClient riêng AllowAutoRedirect=false; đăng nhập qua Identity/cookie và phiên SQL thật, trừ IT-053 anonymous. GET form cùng client để lấy antiforgery, trừ IT-054.
4. Fixture tạo liên hệ ChoPhanHoi; G POST action=ChapNhan cùng id và RowVersion hiện tại.
5. Thu status, Content-Type, Location hoặc JSON/body trước redirect; so với phản hồi kỳ vọng.
6. Mở DbContext mới đọc lại dữ liệu nghiệp vụ, so cả bản ghi phải đổi và không được đổi; kiểm số lượng, ID, RowVersion, thông báo/outbox hoặc lỗi SQL theo ca.
7. Dừng host kiểm thử, xóa đúng ID fixture theo thứ tự khóa ngoại; xóa khung trước yêu cầu lớp. Lưu quan sát và TRX, không xóa dữ liệu ngoài fixture.

**Kỳ vọng HTTP:** Thành công.
**Kỳ vọng SQL:** LH=ChapNhan; hội thoại TruocKetNoi chỉ một; chưa tạo LopHoc.

**Thực tế:** HTTP 200 JSON thành công. Liên hệ ChapNhan, đúng người phản hồi; có đúng một hội thoại TruocKetNoi gắn liên hệ; chưa tạo yêu cầu/lớp.

**Số dòng baseline và sau request** gồm cả dữ liệu chuẩn bị, không đồng nhất tổng notice/outbox với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 1,
    "Events": 1
  },
  "after": {
    "Contacts": 1,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 2,
    "Events": 2
  },
  "sqlErrorNumbers": []
}
```

**HTTP thu được:**

```json
{
  "status": 200,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": true,
    "message": "Đã lưu thay đổi.",
    "redirectUrl": "/KetNoi/KetNoi/LienHe"
  }
}
```

**Truy vấn đối chiếu theo kịch bản** dùng ID fixture thực trong KetQua.json:

```sql
SELECT * FROM dbo.[YeuCauLienHe] WHERE Id=@Contact; SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE LienHeId=@Contact; SELECT * FROM dbo.[KhungGioDeXuat] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[CuocHoiThoai] WHERE LienHeId=@Contact;
```

### IT-047 — Sai người phản hồi liên hệ

**Tiêu chuẩn:** Xác thực. **Đích:** `POST /KetNoi/KetNoi/PhanHoiLienHe`.

**Dữ liệu riêng:** X dùng Id LH1/RV hiện tại.

**Các bước đã thực hiện:**

1. Đối chiếu DB_NAME/@@SERVERNAME; tạo fixture có tag và bốn tài khoản hoạt động, môn/hồ sơ đủ điều kiện; service dùng SQL thật để chuẩn bị khi cần.
2. Chụp Contacts/Requests/Slots/Rooms/Classes/Notices/Events qua DbContext mới AsNoTracking trước thao tác.
3. Tạo HttpClient riêng AllowAutoRedirect=false; đăng nhập qua Identity/cookie và phiên SQL thật, trừ IT-053 anonymous. GET form cùng client để lấy antiforgery, trừ IT-054.
4. Fixture tạo liên hệ H/G ChoPhanHoi; tài khoản H2 dùng id và RowVersion thật để phản hồi.
5. Thu status, Content-Type, Location hoặc JSON/body trước redirect; so với phản hồi kỳ vọng.
6. Mở DbContext mới đọc lại dữ liệu nghiệp vụ, so cả bản ghi phải đổi và không được đổi; kiểm số lượng, ID, RowVersion, thông báo/outbox hoặc lỗi SQL theo ca.
7. Dừng host kiểm thử, xóa đúng ID fixture theo thứ tự khóa ngoại; xóa khung trước yêu cầu lớp. Lưu quan sát và TRX, không xóa dữ liệu ngoài fixture.

**Kỳ vọng HTTP:** 403.0
**Kỳ vọng SQL:** LH không đổi; không tạo hội thoại.

**Thực tế:** HTTP 403; bản ghi liên hệ và RowVersion giữ nguyên; không thêm hội thoại hoặc dữ liệu nghiệp vụ.

**Số dòng baseline và sau request** gồm cả dữ liệu chuẩn bị, không đồng nhất tổng notice/outbox với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 1,
    "Events": 1
  },
  "after": {
    "Contacts": 1,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 1,
    "Events": 1
  },
  "sqlErrorNumbers": []
}
```

**HTTP thu được:**

```json
{
  "status": 403,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": false,
    "message": "Không có quyền phản hồi.",
    "errors": {
      "": [
        "Không có quyền phản hồi."
      ]
    }
  }
}
```

**Truy vấn đối chiếu theo kịch bản** dùng ID fixture thực trong KetQua.json:

```sql
SELECT * FROM dbo.[YeuCauLienHe] WHERE Id=@Contact; SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE LienHeId=@Contact; SELECT * FROM dbo.[KhungGioDeXuat] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[CuocHoiThoai] WHERE LienHeId=@Contact;
```

### IT-048 — Bên học gửi yêu cầu kết nối

**Tiêu chuẩn:** Đúng đắn. **Đích:** `POST /KetNoi/KetNoi/GuiYeuCau`.

**Dữ liệu riêng:** LH1 ChapNhan; H gửi lời nhắn hợp lệ.

**Các bước đã thực hiện:**

1. Đối chiếu DB_NAME/@@SERVERNAME; tạo fixture có tag và bốn tài khoản hoạt động, môn/hồ sơ đủ điều kiện; service dùng SQL thật để chuẩn bị khi cần.
2. Chụp Contacts/Requests/Slots/Rooms/Classes/Notices/Events qua DbContext mới AsNoTracking trước thao tác.
3. Tạo HttpClient riêng AllowAutoRedirect=false; đăng nhập qua Identity/cookie và phiên SQL thật, trừ IT-053 anonymous. GET form cùng client để lấy antiforgery, trừ IT-054.
4. Fixture chấp nhận liên hệ H/G và tạo nhu cầu N1; H POST GuiYeuCau với LH1/N1 và lời nhắn hợp lệ.
5. Thu status, Content-Type, Location hoặc JSON/body trước redirect; so với phản hồi kỳ vọng.
6. Mở DbContext mới đọc lại dữ liệu nghiệp vụ, so cả bản ghi phải đổi và không được đổi; kiểm số lượng, ID, RowVersion, thông báo/outbox hoặc lỗi SQL theo ca.
7. Dừng host kiểm thử, xóa đúng ID fixture theo thứ tự khóa ngoại; xóa khung trước yêu cầu lớp. Lưu quan sát và TRX, không xóa dữ liệu ngoài fixture.

**Kỳ vọng HTTP:** Thành công.
**Kỳ vọng SQL:** Yêu cầu ChoGiaSu; hạn=GuiLuc+72h; chưa tạo lớp.

**Thực tế:** HTTP 200 JSON thành công. Có đúng một ChoGiaSu, đúng H/LH1/N1; HetHanLuc bằng GuiLuc+72 giờ, chưa tạo lớp.

**Số dòng baseline và sau request** gồm cả dữ liệu chuẩn bị, không đồng nhất tổng notice/outbox với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 2,
    "Events": 2
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 0,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 3,
    "Events": 3
  },
  "sqlErrorNumbers": []
}
```

**HTTP thu được:**

```json
{
  "status": 200,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": true,
    "message": "Đã lưu thay đổi.",
    "redirectUrl": "/KetNoi/KetNoi"
  }
}
```

**Truy vấn đối chiếu theo kịch bản** dùng ID fixture thực trong KetQua.json:

```sql
SELECT * FROM dbo.[YeuCauLienHe] WHERE Id=@Contact; SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE LienHeId=@Contact; SELECT * FROM dbo.[KhungGioDeXuat] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[CuocHoiThoai] WHERE LienHeId=@Contact;
```

### IT-049 — Gia sư tạo đề xuất đầy đủ

**Tiêu chuẩn:** Đầy đủ. **Đích:** `POST /KetNoi/KetNoi/DeXuat`.

**Dữ liệu riêng:** Đề xuất nền, đúng LH1 và môn trong hồ sơ.

**Các bước đã thực hiện:**

1. Đối chiếu DB_NAME/@@SERVERNAME; tạo fixture có tag và bốn tài khoản hoạt động, môn/hồ sơ đủ điều kiện; service dùng SQL thật để chuẩn bị khi cần.
2. Chụp Contacts/Requests/Slots/Rooms/Classes/Notices/Events qua DbContext mới AsNoTracking trước thao tác.
3. Tạo HttpClient riêng AllowAutoRedirect=false; đăng nhập qua Identity/cookie và phiên SQL thật, trừ IT-053 anonymous. GET form cùng client để lấy antiforgery, trừ IT-054.
4. Fixture LH1 ChapNhan/N1 hai môn; G gửi đủ điều kiện lớp và hai khung 19:00–20:00 qua route DeXuat.
5. Thu status, Content-Type, Location hoặc JSON/body trước redirect; so với phản hồi kỳ vọng.
6. Mở DbContext mới đọc lại dữ liệu nghiệp vụ, so cả bản ghi phải đổi và không được đổi; kiểm số lượng, ID, RowVersion, thông báo/outbox hoặc lỗi SQL theo ca.
7. Dừng host kiểm thử, xóa đúng ID fixture theo thứ tự khóa ngoại; xóa khung trước yêu cầu lớp. Lưu quan sát và TRX, không xóa dữ liệu ngoài fixture.

**Kỳ vọng HTTP:** Thành công; tên route là DeXuat, không GuiDeXuat.
**Kỳ vọng SQL:** ChoBenHoc; 2 KhungGioDeXuat; DeXuatLuc/hạn 72h; đúng phí/đơn vị.

**Thực tế:** HTTP 200 JSON thành công. Một ChoBenHoc, đúng gia sư/môn/phí 150000 VND/Gio; hai khung thứ 1/3, mỗi khung 60 phút; hạn DeXuatLuc+72 giờ.

**Số dòng baseline và sau request** gồm cả dữ liệu chuẩn bị, không đồng nhất tổng notice/outbox với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 2,
    "Events": 2
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 3,
    "Events": 3
  },
  "sqlErrorNumbers": []
}
```

**HTTP thu được:**

```json
{
  "status": 200,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": true,
    "message": "Đã gửi đề xuất để bên học xác nhận.",
    "redirectUrl": "/KetNoi/KetNoi"
  }
}
```

**Truy vấn đối chiếu theo kịch bản** dùng ID fixture thực trong KetQua.json:

```sql
SELECT * FROM dbo.[YeuCauLienHe] WHERE Id=@Contact; SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE LienHeId=@Contact; SELECT * FROM dbo.[KhungGioDeXuat] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[CuocHoiThoai] WHERE LienHeId=@Contact;
```

### IT-050 — Môn không thuộc nhu cầu nhiều môn

**Tiêu chuẩn:** Đúng đắn. **Đích:** `POST /KetNoi/KetNoi/DeXuat`.

**Dữ liệu riêng:** N1 chọn M1/M2; đề xuất M3 không thuộc N1.

**Các bước đã thực hiện:**

1. Đối chiếu DB_NAME/@@SERVERNAME; tạo fixture có tag và bốn tài khoản hoạt động, môn/hồ sơ đủ điều kiện; service dùng SQL thật để chuẩn bị khi cần.
2. Chụp Contacts/Requests/Slots/Rooms/Classes/Notices/Events qua DbContext mới AsNoTracking trước thao tác.
3. Tạo HttpClient riêng AllowAutoRedirect=false; đăng nhập qua Identity/cookie và phiên SQL thật, trừ IT-053 anonymous. GET form cùng client để lấy antiforgery, trừ IT-054.
4. N1 có M1/M2; tạo M3 đang dùng và thêm M3 vào hồ sơ G; G đề xuất M3 gắn N1 để loại trừ nguyên nhân môn không thuộc hồ sơ.
5. Thu status, Content-Type, Location hoặc JSON/body trước redirect; so với phản hồi kỳ vọng.
6. Mở DbContext mới đọc lại dữ liệu nghiệp vụ, so cả bản ghi phải đổi và không được đổi; kiểm số lượng, ID, RowVersion, thông báo/outbox hoặc lỗi SQL theo ca.
7. Dừng host kiểm thử, xóa đúng ID fixture theo thứ tự khóa ngoại; xóa khung trước yêu cầu lớp. Lưu quan sát và TRX, không xóa dữ liệu ngoài fixture.

**Kỳ vọng HTTP:** Từ chối; form AJAX báo lỗi.
**Kỳ vọng SQL:** Không lưu yêu cầu/khung mới.

**Thực tế:** HTTP 422 JSON lỗi môn không thuộc nhu cầu. SQL51131 thật được ghi nhận; không thêm yêu cầu/khung hoặc thay đổi notice/outbox. M3 có trong hồ sơ gia sư để chỉ kiểm điều kiện không thuộc N1.

**Số dòng baseline và sau request** gồm cả dữ liệu chuẩn bị, không đồng nhất tổng notice/outbox với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 2,
    "Events": 2
  },
  "after": {
    "Contacts": 1,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 2,
    "Events": 2
  },
  "sqlErrorNumbers": [
    51131
  ]
}
```

**HTTP thu được:**

```json
{
  "status": 422,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": false,
    "message": "Kiểm tra lại thông tin đã nhập.",
    "errors": {
      "": [
        "Môn đề xuất không thuộc nhu cầu học đang mở."
      ]
    }
  }
}
```

**Truy vấn đối chiếu theo kịch bản** dùng ID fixture thực trong KetQua.json:

```sql
SELECT * FROM dbo.[YeuCauLienHe] WHERE Id=@Contact; SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE LienHeId=@Contact; SELECT * FROM dbo.[KhungGioDeXuat] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[CuocHoiThoai] WHERE LienHeId=@Contact;
```

### IT-051 — Sửa đề xuất RV cũ

**Tiêu chuẩn:** Đúng đắn. **Đích:** `POST /KetNoi/KetNoi/DeXuat`.

**Dữ liệu riêng:** YeuCauId đã có; RowVersion cũ; lịch mới.

**Các bước đã thực hiện:**

1. Đối chiếu DB_NAME/@@SERVERNAME; tạo fixture có tag và bốn tài khoản hoạt động, môn/hồ sơ đủ điều kiện; service dùng SQL thật để chuẩn bị khi cần.
2. Chụp Contacts/Requests/Slots/Rooms/Classes/Notices/Events qua DbContext mới AsNoTracking trước thao tác.
3. Tạo HttpClient riêng AllowAutoRedirect=false; đăng nhập qua Identity/cookie và phiên SQL thật, trừ IT-053 anonymous. GET form cùng client để lấy antiforgery, trừ IT-054.
4. Dùng service/SQL thật tạo đề xuất v1, giữ RV1; cập nhật v2 và hai khung bằng RV1; HTTP gửi lại RV1, tên/lịch khác. Kiểm toàn bộ bản v2 và khung giữ nguyên; interceptor phải thu SqlException 51139.
5. Thu status, Content-Type, Location hoặc JSON/body trước redirect; so với phản hồi kỳ vọng.
6. Mở DbContext mới đọc lại dữ liệu nghiệp vụ, so cả bản ghi phải đổi và không được đổi; kiểm số lượng, ID, RowVersion, thông báo/outbox hoặc lỗi SQL theo ca.
7. Dừng host kiểm thử, xóa đúng ID fixture theo thứ tự khóa ngoại; xóa khung trước yêu cầu lớp. Lưu quan sát và TRX, không xóa dữ liệu ngoài fixture.

**Kỳ vọng HTTP:** Lỗi phiên bản; form AJAX không báo ok=true.
**Kỳ vọng SQL:** Không xóa/ghi đè khung cũ; SQL 51139 được ánh xạ.

**Thực tế:** HTTP 422 JSON lỗi phiên bản; SQL51139 thật. Gửi RowVersion từ lần lưu thứ nhất sau khi lần thứ hai đã cập nhật; yêu cầu, RowVersion và hai khung hiện tại giữ nguyên.

**Số dòng baseline và sau request** gồm cả dữ liệu chuẩn bị, không đồng nhất tổng notice/outbox với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 4,
    "Events": 4
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 4,
    "Events": 4
  },
  "sqlErrorNumbers": [
    51139
  ]
}
```

**HTTP thu được:**

```json
{
  "status": 422,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": false,
    "message": "Kiểm tra lại thông tin đã nhập.",
    "errors": {
      "": [
        "Yêu cầu đã đổi phiên bản, hết hạn hoặc không thuộc gia sư."
      ]
    }
  }
}
```

**Truy vấn đối chiếu theo kịch bản** dùng ID fixture thực trong KetQua.json:

```sql
SELECT * FROM dbo.[YeuCauLienHe] WHERE Id=@Contact; SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE LienHeId=@Contact; SELECT * FROM dbo.[KhungGioDeXuat] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[CuocHoiThoai] WHERE LienHeId=@Contact;
```

### IT-052 — Bỏ danh sách lịch khỏi form

**Tiêu chuẩn:** Đầy đủ. **Đích:** `POST /KetNoi/KetNoi/DeXuat`.

**Dữ liệu riêng:** Không gửi khóa LichRanh[i]; các trường khác đủ.

**Các bước đã thực hiện:**

1. Đối chiếu DB_NAME/@@SERVERNAME; tạo fixture có tag và bốn tài khoản hoạt động, môn/hồ sơ đủ điều kiện; service dùng SQL thật để chuẩn bị khi cần.
2. Chụp Contacts/Requests/Slots/Rooms/Classes/Notices/Events qua DbContext mới AsNoTracking trước thao tác.
3. Tạo HttpClient riêng AllowAutoRedirect=false; đăng nhập qua Identity/cookie và phiên SQL thật, trừ IT-053 anonymous. GET form cùng client để lấy antiforgery, trừ IT-054.
4. Giữ điều kiện lớp hợp lệ nhưng bỏ toàn bộ khóa LichRanh[i] khỏi form POST.
5. Thu status, Content-Type, Location hoặc JSON/body trước redirect; so với phản hồi kỳ vọng.
6. Mở DbContext mới đọc lại dữ liệu nghiệp vụ, so cả bản ghi phải đổi và không được đổi; kiểm số lượng, ID, RowVersion, thông báo/outbox hoặc lỗi SQL theo ca.
7. Dừng host kiểm thử, xóa đúng ID fixture theo thứ tự khóa ngoại; xóa khung trước yêu cầu lớp. Lưu quan sát và TRX, không xóa dữ liệu ngoài fixture.

**Kỳ vọng HTTP:** Lỗi dữ liệu, không dùng lịch mặc định để thành công.
**Kỳ vọng SQL:** Không thêm đề xuất/khung.

**Thực tế:** HTTP 422 JSON lỗi cần 1–35 khung giờ; không dùng lịch mặc định để thành công, không gọi procedure tạo đề xuất, dữ liệu trước/sau giữ nguyên.

**Số dòng baseline và sau request** gồm cả dữ liệu chuẩn bị, không đồng nhất tổng notice/outbox với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 2,
    "Events": 2
  },
  "after": {
    "Contacts": 1,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 2,
    "Events": 2
  },
  "sqlErrorNumbers": []
}
```

**HTTP thu được:**

```json
{
  "status": 422,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": false,
    "message": "Kiểm tra lại thông tin đã nhập.",
    "errors": {
      "": [
        "Cần từ 1 đến 35 khung giờ."
      ]
    }
  }
}
```

**Truy vấn đối chiếu theo kịch bản** dùng ID fixture thực trong KetQua.json:

```sql
SELECT * FROM dbo.[YeuCauLienHe] WHERE Id=@Contact; SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE LienHeId=@Contact; SELECT * FROM dbo.[KhungGioDeXuat] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[CuocHoiThoai] WHERE LienHeId=@Contact;
```

### IT-053 — Không đăng nhập gọi thao tác ghi

**Tiêu chuẩn:** Xác thực. **Đích:** `POST /KetNoi/KetNoi/GuiLienHe`.

**Dữ liệu riêng:** Không gửi cookie phiên; lấy token antiforgery hợp lệ từ trang công khai nếu cần.

**Các bước đã thực hiện:**

1. Đối chiếu DB_NAME/@@SERVERNAME; tạo fixture có tag và bốn tài khoản hoạt động, môn/hồ sơ đủ điều kiện; service dùng SQL thật để chuẩn bị khi cần.
2. Chụp Contacts/Requests/Slots/Rooms/Classes/Notices/Events qua DbContext mới AsNoTracking trước thao tác.
3. Tạo HttpClient riêng AllowAutoRedirect=false; đăng nhập qua Identity/cookie và phiên SQL thật, trừ IT-053 anonymous. GET form cùng client để lấy antiforgery, trừ IT-054.
4. Client anonymous lấy token công khai; POST AJAX không cookie phiên.
5. Thu status, Content-Type, Location hoặc JSON/body trước redirect; so với phản hồi kỳ vọng.
6. Mở DbContext mới đọc lại dữ liệu nghiệp vụ, so cả bản ghi phải đổi và không được đổi; kiểm số lượng, ID, RowVersion, thông báo/outbox hoặc lỗi SQL theo ca.
7. Dừng host kiểm thử, xóa đúng ID fixture theo thứ tự khóa ngoại; xóa khung trước yêu cầu lớp. Lưu quan sát và TRX, không xóa dữ liệu ngoài fixture.

**Kỳ vọng HTTP:** AJAX 401; không chuyển HTML đăng nhập.
**Kỳ vọng SQL:** Không đổi dữ liệu nghiệp vụ mục tiêu. Nhật ký an toàn từ chối có thể được thêm.

**Thực tế:** AJAX HTTP 401, không Location và không HTML đăng nhập; dữ liệu nghiệp vụ giữ nguyên. Token lấy từ trang đăng nhập bằng client anonymous.

**Số dòng baseline và sau request** gồm cả dữ liệu chuẩn bị, không đồng nhất tổng notice/outbox với số mới thêm:

```json
{
  "before": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 0,
    "Events": 0
  },
  "after": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 0,
    "Events": 0
  },
  "sqlErrorNumbers": []
}
```

**HTTP thu được:**

```json
{
  "status": 401,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": false,
    "message": "Phiên đã hết hạn. Vui lòng đăng nhập lại."
  }
}
```

**Truy vấn đối chiếu theo kịch bản** dùng ID fixture thực trong KetQua.json:

```sql
SELECT * FROM dbo.[YeuCauLienHe] WHERE Id=@Contact; SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE LienHeId=@Contact; SELECT * FROM dbo.[KhungGioDeXuat] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[CuocHoiThoai] WHERE LienHeId=@Contact;
```

### IT-054 — Thiếu antiforgery token

**Tiêu chuẩn:** Xác thực. **Đích:** `POST /KetNoi/KetNoi/GuiLienHe`.

**Dữ liệu riêng:** Đăng nhập đúng quyền; bỏ __RequestVerificationToken/header token, giữ cookie.

**Các bước đã thực hiện:**

1. Đối chiếu DB_NAME/@@SERVERNAME; tạo fixture có tag và bốn tài khoản hoạt động, môn/hồ sơ đủ điều kiện; service dùng SQL thật để chuẩn bị khi cần.
2. Chụp Contacts/Requests/Slots/Rooms/Classes/Notices/Events qua DbContext mới AsNoTracking trước thao tác.
3. Tạo HttpClient riêng AllowAutoRedirect=false; đăng nhập qua Identity/cookie và phiên SQL thật, trừ IT-053 anonymous. GET form cùng client để lấy antiforgery, trừ IT-054.
4. Client đăng nhập H thật; POST AJAX không __RequestVerificationToken, giữ cookie.
5. Thu status, Content-Type, Location hoặc JSON/body trước redirect; so với phản hồi kỳ vọng.
6. Mở DbContext mới đọc lại dữ liệu nghiệp vụ, so cả bản ghi phải đổi và không được đổi; kiểm số lượng, ID, RowVersion, thông báo/outbox hoặc lỗi SQL theo ca.
7. Dừng host kiểm thử, xóa đúng ID fixture theo thứ tự khóa ngoại; xóa khung trước yêu cầu lớp. Lưu quan sát và TRX, không xóa dữ liệu ngoài fixture.

**Kỳ vọng HTTP:** HTTP 400 do antiforgery; không yêu cầu JSON theo schema của action.
**Kỳ vọng SQL:** Không thay đổi dữ liệu nghiệp vụ.

**Thực tế:** HTTP 400 do antiforgery; giữ cookie đăng nhập nhưng bỏ token. Không thêm hoặc thay đổi dữ liệu nghiệp vụ; không áp đặt JSON của action.

**Số dòng baseline và sau request** gồm cả dữ liệu chuẩn bị, không đồng nhất tổng notice/outbox với số mới thêm:

```json
{
  "before": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 0,
    "Events": 0
  },
  "after": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 0,
    "Events": 0
  },
  "sqlErrorNumbers": []
}
```

**HTTP thu được:**

```json
{
  "status": 400,
  "location": null,
  "contentType": null,
  "body": ""
}
```

**Truy vấn đối chiếu theo kịch bản** dùng ID fixture thực trong KetQua.json:

```sql
SELECT * FROM dbo.[YeuCauLienHe] WHERE Id=@Contact; SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE LienHeId=@Contact; SELECT * FROM dbo.[KhungGioDeXuat] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[CuocHoiThoai] WHERE LienHeId=@Contact;
```

## Các điểm cần hiểu đúng

- IT-044 dùng hai request HTTP cùng được mở qua barrier; quan sát 400/200, một liên hệ và một notice/outbox. Index unique được đọc từ sys.indexes. Lần chạy không gây lỗi unique 2601/2627: guard/lock đã từ chối request còn lại trước đó; không khẳng định mọi thứ tự tranh chấp đều đã được kiểm.
- IT-050 M3 có trong hồ sơ G nhưng ngoài N1 để tránh từ chối vì sai hồ sơ. SQL51131 thật trả lỗi môn ngoài nhu cầu; không chỉnh dữ liệu nguồn để tạo Pass.
- IT-051 dùng hai phiên bản đã lưu thật, không tạo SqlException giả. HTTP422 phù hợp kịch bản chỉ yêu cầu lỗi phiên bản và không ok=true; kịch bản nhóm này không bắt buộc HTTP409. SQL51139 được thu bởi interceptor quan sát, toàn bộ dữ liệu/khung v2 giữ nguyên.
- IT-052 lỗi lịch là 422, không có lời gọi usp_GuiDeXuatLop trong request; không dùng danh sách lịch mặc định để lưu thành công.
- IT-053/054 kiểm cookie và antiforgery khác nhau; nhật ký an toàn có thể phát sinh dù không thay đổi dữ liệu nghiệp vụ.

## File đã thêm và sửa

- `tests/Integration0203/Cases04.cs`: 13 Fact, một mã một test; Trait Suite=IT04LienHeDeXuat.
- `tests/Integration0203/ContactProposalIntegrationTests.cs`: setup riêng từng ca, HTTP requests, snapshot trước/sau, assertions, barrier cho hai request và interceptor chỉ quan sát lệnh/lỗi SQL. Không sửa kết quả SQL hoặc mock nghiệp vụ.
- `tests/Integration0203/IntegrationHarness.cs`: nhận interceptor tùy chọn cho test host; dọn KhungGioDeXuat theo request ID trước khi xóa yêu cầu để đúng FK. Không đổi cơ chế login thật và giới hạn DB riêng.
- Giữ nguyên các file code ứng dụng và SQL trong đợt Integration04. ZIP chứa snapshot các file cần đọc và diff harness trước/sau.

## Chạy lại trong VS Code

Mở `C:\Khóa luận TN\Educonnect`, Terminal PowerShell:

```powershell
$env:EDU_TEST_ROOT='C:\Khóa luận TN\Educonnect'
dotnet test tests/Integration0203/Integration0203.csproj --filter "Suite=IT04LienHeDeXuat" --logger "trx;LogFileName=LienHe-DeXuat-Integration.trx" --results-directory "TestResults/04-LienHe-DeXuat-Integration"

dotnet test tests/Integration0203/Integration0203.csproj --filter "FullyQualifiedName~Integration0203Tests.IT_051"
```

Nếu ứng dụng đang khóa file build, dừng app hoặc dùng BaseOutputPath riêng và UseAppHost=false. Không đổi EDU_TEST_CONNECTION sang database ứng dụng: harness chỉ chấp nhận EduConnect_Verification_IT0203.

## Hồi quy và vệ sinh dữ liệu

Đã chạy lại 26 ca nhóm02–03: **21 Pass, 5 Fail**, không đổi trạng thái từng mã. Năm Fail cũ: IT-022, IT-023, IT-025, IT-034, IT-037; không tính vào13 ca nhóm04 và không ghi đè báo cáo cũ. TRX hồi quy riêng tại work/it04-regression/Regression02-03-After04.trx.

Sau13 ca nhóm04, truy vấn cho RemainingITFixtureUsers=0; mỗi ca có cleanup thành công. Cleanup chỉ theo ID sở hữu, không xóa toàn bộ DB. SHA-256 file SQL nguồn và hash định nghĩa procedure hiện có được lưu để truy nguyên; không đồng nhất hash file nguồn với hash chuỗi định nghĩa trong sys.sql_modules.

KetQua.json chứa đầy đủ input đã bỏ token, HTTP, snapshot từng cột/RowVersion, số lỗi SQL và assertion. TRX có cùng quan sát OBSERVATION_JSON. Ma_nguon_Integration.zip chứa mã, diff và minh chứng từng ca. SHA256.json dùng đối chiếu tính toàn vẹn.
