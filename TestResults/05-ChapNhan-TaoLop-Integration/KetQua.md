# Kết quả Integration05 Chấp nhận và tạo lớp

**10 ca IT-055–IT-064: 9 Pass, 1 Fail, 0 Skip/Blocked.**

**IT-057 Fail:** cần403 nhưng trả404 cho người ngoài. Không tạo lớp; không thay đổi yêu cầu hoặc dữ liệu liên quan. Không sửa kỳ vọng hay code nghiệp vụ để làm Pass.

Nguồn Integration Test.xlsx, sheet05 Chap nhan Tao lop; đã đối chiếu đúng file hiện tại theo từng mã, đầu vào, ba tiêu chuẩn và kỳ vọng.

## Phạm vi

Database thực thi: `LAPTOP-3L2V1AFT\SQLEXPRESS / EduConnect_Verification_IT0203`. HTTP TestServer đi qua đăng nhập Identity,cookie,phiên SQL,antiforgery,binding,controller,service,EF và procedure thật. Trigger LichHoc tạo BuoiHoc cũng được kiểm bằng dữ liệu đọc lại. Không mock nghiệp vụ hoặc lỗiSQL.

Host Development,tắt DemoPublishProfiles,dừng BaoTriWorker/XuLySuKienWorker. Kiểm notice/outbox đã lưu, không kiểm phát realtime/email bên ngoài. SQL dùng tài khoản Windows tích hợp của người vận hành DB test; không kết luận về quyền tối thiểu của role ứng dụng. Không tạo lại DB hoặc sửa schema.

## Kết quả từng ca

| Mã | Tình huống | Tiêu chuẩn | Kết quả | Thực tế |
| --- | --- | --- | --- | --- |
| IT-055 | Chấp nhận tạo đầy đủ lớp | Đầy đủ | Pass | HTTP200 JSON ok=true, redirectUrl trỏ đúng lớp SQL tạo. Có một LopHoc DangHoatDong, hai thành viên DaDuyet/ChinhSua, một hội thoại TheoLop, hai lịch đầu tiên đúng ngày/giờ và hai BuoiHoc DuKien do trigger. Yêu cầu DaChapNhan; đúng phí 150000 VND/Gio và điều kiện đề xuất. |
| IT-056 | Chấp nhận lại không nhân bản | Đúng đắn | Pass | Hai POST đều HTTP200, cùng mã lớp trong redirectUrl. Snapshot sau lần đầu và lần hai giống hệt: một lớp, hai thành viên, hai lịch, hai buổi và số notice/outbox không tăng thêm. |
| IT-057 | Chấp nhận sai bên học | Xác thực | Fail | Kỳ vọng403 nhưng thực tế HTTP 404 JSON ok=false, thông báo Không tìm thấy yêu cầu. Actor X là tài khoản PhuHuynh hoạt động không thuộc yêu cầu. Toàn bộ dữ liệu nghiệp vụ trước/sau giữ nguyên; không tạo lớp hoặc dữ liệu liên quan. |
| IT-058 | RV cũ khi chấp nhận | Đúng đắn | Pass | HTTP 409 JSON lỗi phiên bản; interceptor quan sát SQL 51152 thật. RV1 được đọc trước cập nhật v2; gửi RV1 không tạo lớp/thành viên/lịch/buổi và giữ nguyên v2. |
| IT-059 | Chấp nhận đã đến hạn | Đúng đắn | Pass | HTTP 409 với SQL 51152 thật; đề xuất vẫn ChoBenHoc nhưng HetHanLuc đã qua. Không tạo lớp, không gia hạn; worker bị dừng trong host. |
| IT-060 | Lịch đầu tiên trùng gây rollback | Đúng đắn | Pass | HTTP 409 với SQL 51145 trùng lịch thật. Lớp xung đột fixture vẫn còn; không có lớp của YC1 hoặc thành viên/hội thoại/lịch/buổi mới. Snapshot trước/sau giống hệt, YC1 chưa DaChapNhan. |
| IT-061 | Hai client chấp nhận đồng thời | Đúng đắn | Pass | Hai POST qua barrier đều HTTP200, cùng một mã lớp. Có đúng một lớp, hai thành viên, một hội thoại TheoLop, hai lịch/hai buổi, chỉ tăng một notice/outbox. UQ_YeuCau_LopHoc tồn tại. |
| IT-062 | Từ chối yêu cầu | Đúng đắn | Pass | HTTP200; YC1 chuyển TuChoi, không tạo lớp. Thông báo mới gửi đúng G và mã YC1. |
| IT-063 | Rút bởi người gửi | Đúng đắn | Pass | HTTP200; G gửi rút, YC1 chuyển DaRut; không tạo lớp. Thông báo mới đúng H/mã yêu cầu. |
| IT-064 | Người nhận cố rút | Xác thực | Pass | HTTP 403; H là người nhận cố rút đề xuất của G. Snapshot nghiệp vụ không đổi; không có lớp. |

## Chi tiết IT-057 Fail

**Cách tái hiện:** Đăng nhập X là tài khoản PhuHuynh hoạt động, không phải H/G của YC1. POST đúng MaYeuCau, RowVersion hiện tại và antiforgery hợp lệ tới ChapNhan.

**Kỳ vọng:** HTTP 403; request giữ nguyên, không tạo lớp.
**Thực tế:** HTTP 404, ok=false; request giữ nguyên, không tạo lớp; SQL procedure chấp nhận chưa được gọi.

**Nguyên nhân:** ChapNhanDeXuatAsync gọi ChiTietAsync trước guard r.BenHocId==u.Id. ChiTietAsync lọc Id cùng GiaSuId/BenHocId của actor; X không thuộc hai bên nên truy vấn trả null và ném LoiNghiepVu404. Guard403 dành cho bên học chưa được chạy. KetQua giữ404; controller ném lại đúng status và UiResponseFilter trả JSON404.

**Ảnh hưởng:** Quyền vẫn được bảo vệ và toàn bộ dữ liệu giữ nguyên. Sai lệch là hợp đồng mã HTTP trong IT-057, không phải người ngoài tạo được lớp.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\KetNoi\Services\KetNoiService.cs:186` — `var r = await ChiTietAsync(id);`.
- `C:\Khóa luận TN\Educonnect\Areas\KetNoi\Services\KetNoiService.cs:175` — `Không tìm thấy yêu cầu.`.
- `C:\Khóa luận TN\Educonnect\Areas\KetNoi\Services\KetNoiService.cs:187` — `Rules.Check(r.BenHocId == u.Id`.
- `C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:100` — `ChapNhan chuyển KetQua lỗi thành LoiNghiepVu cùng status`.
- `C:\Khóa luận TN\Educonnect\Infrastructure\UiResponseFilter.cs:35` — `context.Result = new JsonResult(new { ok = false`.

**Hướng xử lý:** Thống nhất hợp đồng403 hay404 cho tài nguyên của người khác. Nếu giữ kỳ vọng403, phân biệt yêu cầu tồn tại nhưng actor không có quyền trong luồng chấp nhận; không bỏ kiểm quyền để sửa status. Nếu thiết kế chủ định che sự tồn tại bằng404, cần cập nhật thiết kế sau khi được thống nhất; lần chạy này vẫn Fail theo bảng.

## Các bước và minh chứng từng ca

### IT-055 — Chấp nhận tạo đầy đủ lớp

**Đích:** `POST /KetNoi/KetNoi/ChapNhan`; **Tiêu chuẩn:** Đầy đủ.
**Input riêng:** MaYeuCau=YC1; RowVersion=base64 RV thật.

**Các bước thực hiện:**

1. Đối chiếu DB_NAME và @@SERVERNAME; tạo fixture mới có tag/ID riêng, H/G/X hoạt động, nhu cầu học và hồ sơ gia sư hợp lệ.
2. Dùng service/EF/SQL thật tạo liên hệ ChapNhan và đề xuất YC1 ChoBenHoc có hai khung, 2 buổi/tuần,60phút, phí150000 VND/Gio và bắt đầu tương lai. Đây là chuẩn bị fixture, chưa phải thao tác đang kiểm.
3. Chụp baseline bằng DbContext mới AsNoTracking: liên hệ, yêu cầu, khung đề xuất, lớp, thành viên, hội thoại, lịch, buổi học, notice/outbox.
4. Tạo client đúng actor, đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; GET form để lấy antiforgery cùng client.
5. H gửi MaYeuCau=YC1 và base64 RowVersion thật đến ChapNhan.
6. Gửi POST AJAX qua pipeline thật; thu status,JSON,redirectUrl; interceptor chỉ quan sát lệnh và mã lỗi SQL, không thay kết quả.
7. Đọc lại SQL bằng scope mới, đối chiếu cả dữ liệu phải thay đổi và dữ liệu không được đổi. So lịch đầu tiên với khung đề xuất và kiểm một BuoiHoc cho mỗi LichHoc; ca thất bại phải không có bản ghi tạo dở.
8. Dừng host, xóa đúng dữ liệu fixture theo khóa ngoại: hội thoại và buổi/lịch/thành viên trước lớp; khung trước yêu cầu. Lưu JSON/TRX và kết quả cleanup.

**Kỳ vọng HTTP:** 200 ok=true; redirectUrl tới lớp mới.
**Kỳ vọng SQL:** Một LopHoc DangHoatDong; 2 thành viên chính; 1 hội thoại TheoLop; đủ lịch đầu tiên và BuoiHoc do trigger; YC1 DaChapNhan; phí/đơn vị đúng.

**Quan sát:** HTTP200 JSON ok=true, redirectUrl trỏ đúng lớp SQL tạo. Có một LopHoc DangHoatDong, hai thành viên DaDuyet/ChinhSua, một hội thoại TheoLop, hai lịch đầu tiên đúng ngày/giờ và hai BuoiHoc DuKien do trigger. Yêu cầu DaChapNhan; đúng phí 150000 VND/Gio và điều kiện đề xuất.

**Baseline và sau request** bao gồm dữ liệu fixture, không chỉ bản ghi phát sinh:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 3,
    "Events": 3,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 4,
    "Events": 4,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2
  },
  "sqlErrorNumbers": [],
  "classIdsFromResponses": null
}
```

**HTTP:**

```json
{
  "status": 200,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": true,
    "message": "Lớp đã được tạo. Bạn có thể tiếp tục mở hoặc đóng nhu cầu tại mục Nhu cầu.",
    "redirectUrl": "/HocTap/LopHoc/ChiTiet/0837b1af-aa34-47d9-8ce2-24ad028f349b"
  }
}
```

**Truy vấn của kịch bản:**

```sql
SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE Id=@Request; SELECT * FROM dbo.[LopHoc] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[CuocHoiThoai] WHERE LopHocId=@Class; SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId IN(SELECT Id FROM dbo.LichHoc WHERE LopHocId=@Class);
```

### IT-056 — Chấp nhận lại không nhân bản

**Đích:** `POST /KetNoi/KetNoi/ChapNhan hai lần`; **Tiêu chuẩn:** Đúng đắn.
**Input riêng:** Cùng YC1 đã chấp nhận; đúng bên học.

**Các bước thực hiện:**

1. Đối chiếu DB_NAME và @@SERVERNAME; tạo fixture mới có tag/ID riêng, H/G/X hoạt động, nhu cầu học và hồ sơ gia sư hợp lệ.
2. Dùng service/EF/SQL thật tạo liên hệ ChapNhan và đề xuất YC1 ChoBenHoc có hai khung, 2 buổi/tuần,60phút, phí150000 VND/Gio và bắt đầu tương lai. Đây là chuẩn bị fixture, chưa phải thao tác đang kiểm.
3. Chụp baseline bằng DbContext mới AsNoTracking: liên hệ, yêu cầu, khung đề xuất, lớp, thành viên, hội thoại, lịch, buổi học, notice/outbox.
4. Tạo client đúng actor, đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; GET form để lấy antiforgery cùng client.
5. H POST ChapNhan lần đầu; chụp đầy đủ dữ liệu rồi gửi lại cùng YC1/RV, so mã lớp và toàn bộ snapshot sau hai lần.
6. Gửi POST AJAX qua pipeline thật; thu status,JSON,redirectUrl; interceptor chỉ quan sát lệnh và mã lỗi SQL, không thay kết quả.
7. Đọc lại SQL bằng scope mới, đối chiếu cả dữ liệu phải thay đổi và dữ liệu không được đổi. So lịch đầu tiên với khung đề xuất và kiểm một BuoiHoc cho mỗi LichHoc; ca thất bại phải không có bản ghi tạo dở.
8. Dừng host, xóa đúng dữ liệu fixture theo khóa ngoại: hội thoại và buổi/lịch/thành viên trước lớp; khung trước yêu cầu. Lưu JSON/TRX và kết quả cleanup.

**Kỳ vọng HTTP:** Trả cùng Id lớp; không tạo lớp mới.
**Kỳ vọng SQL:** COUNT LopHoc theo YC1=1; lịch/thành viên/notice không nhân đôi.

**Quan sát:** Hai POST đều HTTP200, cùng mã lớp trong redirectUrl. Snapshot sau lần đầu và lần hai giống hệt: một lớp, hai thành viên, hai lịch, hai buổi và số notice/outbox không tăng thêm.

**Baseline và sau request** bao gồm dữ liệu fixture, không chỉ bản ghi phát sinh:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 3,
    "Events": 3,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 4,
    "Events": 4,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2
  },
  "sqlErrorNumbers": [],
  "classIdsFromResponses": [
    "ed49d4ca-ea1d-499d-93cf-40c3c1065b5b",
    "ed49d4ca-ea1d-499d-93cf-40c3c1065b5b"
  ]
}
```

**HTTP:**

```json
[
  {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": {
      "ok": true,
      "message": "Lớp đã được tạo. Bạn có thể tiếp tục mở hoặc đóng nhu cầu tại mục Nhu cầu.",
      "redirectUrl": "/HocTap/LopHoc/ChiTiet/ed49d4ca-ea1d-499d-93cf-40c3c1065b5b"
    }
  },
  {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": {
      "ok": true,
      "message": "Lớp đã được tạo. Bạn có thể tiếp tục mở hoặc đóng nhu cầu tại mục Nhu cầu.",
      "redirectUrl": "/HocTap/LopHoc/ChiTiet/ed49d4ca-ea1d-499d-93cf-40c3c1065b5b"
    }
  }
]
```

**Truy vấn của kịch bản:**

```sql
SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE Id=@Request; SELECT * FROM dbo.[LopHoc] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[CuocHoiThoai] WHERE LopHocId=@Class; SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId IN(SELECT Id FROM dbo.LichHoc WHERE LopHocId=@Class);
```

### IT-057 — Chấp nhận sai bên học

**Đích:** `POST /KetNoi/KetNoi/ChapNhan`; **Tiêu chuẩn:** Xác thực.
**Input riêng:** Actor X thay H.

**Các bước thực hiện:**

1. Đối chiếu DB_NAME và @@SERVERNAME; tạo fixture mới có tag/ID riêng, H/G/X hoạt động, nhu cầu học và hồ sơ gia sư hợp lệ.
2. Dùng service/EF/SQL thật tạo liên hệ ChapNhan và đề xuất YC1 ChoBenHoc có hai khung, 2 buổi/tuần,60phút, phí150000 VND/Gio và bắt đầu tương lai. Đây là chuẩn bị fixture, chưa phải thao tác đang kiểm.
3. Chụp baseline bằng DbContext mới AsNoTracking: liên hệ, yêu cầu, khung đề xuất, lớp, thành viên, hội thoại, lịch, buổi học, notice/outbox.
4. Tạo client đúng actor, đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; GET form để lấy antiforgery cùng client.
5. Đăng nhập X bằng tài khoản PhuHuynh không thuộc YC1; gửi đúng mã YC1 và RowVersion hiện tại. Không thay X thành gia sư của yêu cầu để làm đạt403.
6. Gửi POST AJAX qua pipeline thật; thu status,JSON,redirectUrl; interceptor chỉ quan sát lệnh và mã lỗi SQL, không thay kết quả.
7. Đọc lại SQL bằng scope mới, đối chiếu cả dữ liệu phải thay đổi và dữ liệu không được đổi. So lịch đầu tiên với khung đề xuất và kiểm một BuoiHoc cho mỗi LichHoc; ca thất bại phải không có bản ghi tạo dở.
8. Dừng host, xóa đúng dữ liệu fixture theo khóa ngoại: hội thoại và buổi/lịch/thành viên trước lớp; khung trước yêu cầu. Lưu JSON/TRX và kết quả cleanup.

**Kỳ vọng HTTP:** 403.0
**Kỳ vọng SQL:** Không lớp; yêu cầu giữ nguyên.

**Quan sát:** Kỳ vọng403 nhưng thực tế HTTP 404 JSON ok=false, thông báo Không tìm thấy yêu cầu. Actor X là tài khoản PhuHuynh hoạt động không thuộc yêu cầu. Toàn bộ dữ liệu nghiệp vụ trước/sau giữ nguyên; không tạo lớp hoặc dữ liệu liên quan.

**Baseline và sau request** bao gồm dữ liệu fixture, không chỉ bản ghi phát sinh:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 3,
    "Events": 3,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 3,
    "Events": 3,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0
  },
  "sqlErrorNumbers": [],
  "classIdsFromResponses": null
}
```

**HTTP:**

```json
{
  "status": 404,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": false,
    "message": "Không tìm thấy yêu cầu.",
    "errors": {
      "": [
        "Không tìm thấy yêu cầu."
      ]
    }
  }
}
```

**Truy vấn của kịch bản:**

```sql
SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE Id=@Request; SELECT * FROM dbo.[LopHoc] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[CuocHoiThoai] WHERE LopHocId=@Class; SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId IN(SELECT Id FROM dbo.LichHoc WHERE LopHocId=@Class);
```

### IT-058 — RV cũ khi chấp nhận

**Đích:** `POST /KetNoi/KetNoi/ChapNhan`; **Tiêu chuẩn:** Đúng đắn.
**Input riêng:** Đọc RV1 rồi sửa YC1 tạo RV2; H gửi RV1.

**Các bước thực hiện:**

1. Đối chiếu DB_NAME và @@SERVERNAME; tạo fixture mới có tag/ID riêng, H/G/X hoạt động, nhu cầu học và hồ sơ gia sư hợp lệ.
2. Dùng service/EF/SQL thật tạo liên hệ ChapNhan và đề xuất YC1 ChoBenHoc có hai khung, 2 buổi/tuần,60phút, phí150000 VND/Gio và bắt đầu tương lai. Đây là chuẩn bị fixture, chưa phải thao tác đang kiểm.
3. Chụp baseline bằng DbContext mới AsNoTracking: liên hệ, yêu cầu, khung đề xuất, lớp, thành viên, hội thoại, lịch, buổi học, notice/outbox.
4. Tạo client đúng actor, đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; GET form để lấy antiforgery cùng client.
5. Đọc RV1; G cập nhật đề xuất bằng service/SQL thật tạo RV2; H gửi RV1 qua HTTP.
6. Gửi POST AJAX qua pipeline thật; thu status,JSON,redirectUrl; interceptor chỉ quan sát lệnh và mã lỗi SQL, không thay kết quả.
7. Đọc lại SQL bằng scope mới, đối chiếu cả dữ liệu phải thay đổi và dữ liệu không được đổi. So lịch đầu tiên với khung đề xuất và kiểm một BuoiHoc cho mỗi LichHoc; ca thất bại phải không có bản ghi tạo dở.
8. Dừng host, xóa đúng dữ liệu fixture theo khóa ngoại: hội thoại và buổi/lịch/thành viên trước lớp; khung trước yêu cầu. Lưu JSON/TRX và kết quả cleanup.

**Kỳ vọng HTTP:** 409.0
**Kỳ vọng SQL:** Không tạo lớp hoặc lịch một phần.

**Quan sát:** HTTP 409 JSON lỗi phiên bản; interceptor quan sát SQL 51152 thật. RV1 được đọc trước cập nhật v2; gửi RV1 không tạo lớp/thành viên/lịch/buổi và giữ nguyên v2.

**Baseline và sau request** bao gồm dữ liệu fixture, không chỉ bản ghi phát sinh:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 4,
    "Events": 4,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 4,
    "Events": 4,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0
  },
  "sqlErrorNumbers": [
    51152
  ],
  "classIdsFromResponses": null
}
```

**HTTP:**

```json
{
  "status": 409,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": false,
    "message": "Đề xuất không còn hiệu lực hoặc đã đổi phiên bản.",
    "errors": {
      "": [
        "Đề xuất không còn hiệu lực hoặc đã đổi phiên bản."
      ]
    }
  }
}
```

**Truy vấn của kịch bản:**

```sql
SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE Id=@Request; SELECT * FROM dbo.[LopHoc] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[CuocHoiThoai] WHERE LopHocId=@Class; SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId IN(SELECT Id FROM dbo.LichHoc WHERE LopHocId=@Class);
```

### IT-059 — Chấp nhận đã đến hạn

**Đích:** `POST /KetNoi/KetNoi/ChapNhan`; **Tiêu chuẩn:** Đúng đắn.
**Input riêng:** Seed GuiLuc/DeXuatLuc và HetHanLuc hợp CHECK 72h; hạn <= giờ SQL.

**Các bước thực hiện:**

1. Đối chiếu DB_NAME và @@SERVERNAME; tạo fixture mới có tag/ID riêng, H/G/X hoạt động, nhu cầu học và hồ sơ gia sư hợp lệ.
2. Dùng service/EF/SQL thật tạo liên hệ ChapNhan và đề xuất YC1 ChoBenHoc có hai khung, 2 buổi/tuần,60phút, phí150000 VND/Gio và bắt đầu tương lai. Đây là chuẩn bị fixture, chưa phải thao tác đang kiểm.
3. Chụp baseline bằng DbContext mới AsNoTracking: liên hệ, yêu cầu, khung đề xuất, lớp, thành viên, hội thoại, lịch, buổi học, notice/outbox.
4. Tạo client đúng actor, đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; GET form để lấy antiforgery cùng client.
5. Dùng giờ SQL đặt hạn một giờ trước hiện tại, đồng thời đặt GuiLuc/DeXuatLuc bằng hạn trừ 72h để thỏa CHECK. Đọc RowVersion mới rồi POST; worker chưa đổi trạng thái hết hạn.
6. Gửi POST AJAX qua pipeline thật; thu status,JSON,redirectUrl; interceptor chỉ quan sát lệnh và mã lỗi SQL, không thay kết quả.
7. Đọc lại SQL bằng scope mới, đối chiếu cả dữ liệu phải thay đổi và dữ liệu không được đổi. So lịch đầu tiên với khung đề xuất và kiểm một BuoiHoc cho mỗi LichHoc; ca thất bại phải không có bản ghi tạo dở.
8. Dừng host, xóa đúng dữ liệu fixture theo khóa ngoại: hội thoại và buổi/lịch/thành viên trước lớp; khung trước yêu cầu. Lưu JSON/TRX và kết quả cleanup.

**Kỳ vọng HTTP:** 409 dù worker chưa đánh HetHan.
**Kỳ vọng SQL:** Không lớp; request handler không tự kéo dài hạn.

**Quan sát:** HTTP 409 với SQL 51152 thật; đề xuất vẫn ChoBenHoc nhưng HetHanLuc đã qua. Không tạo lớp, không gia hạn; worker bị dừng trong host.

**Baseline và sau request** bao gồm dữ liệu fixture, không chỉ bản ghi phát sinh:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 3,
    "Events": 3,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 3,
    "Events": 3,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0
  },
  "sqlErrorNumbers": [
    51152
  ],
  "classIdsFromResponses": null
}
```

**HTTP:**

```json
{
  "status": 409,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": false,
    "message": "Đề xuất không còn hiệu lực hoặc đã đổi phiên bản.",
    "errors": {
      "": [
        "Đề xuất không còn hiệu lực hoặc đã đổi phiên bản."
      ]
    }
  }
}
```

**Truy vấn của kịch bản:**

```sql
SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE Id=@Request; SELECT * FROM dbo.[LopHoc] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[CuocHoiThoai] WHERE LopHocId=@Class; SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId IN(SELECT Id FROM dbo.LichHoc WHERE LopHocId=@Class);
```

### IT-060 — Lịch đầu tiên trùng gây rollback

**Đích:** `POST /KetNoi/KetNoi/ChapNhan`; **Tiêu chuẩn:** Đúng đắn.
**Input riêng:** G hoặc H đã có lịch lớp hoạt động khác trùng giờ đề xuất.

**Các bước thực hiện:**

1. Đối chiếu DB_NAME và @@SERVERNAME; tạo fixture mới có tag/ID riêng, H/G/X hoạt động, nhu cầu học và hồ sơ gia sư hợp lệ.
2. Dùng service/EF/SQL thật tạo liên hệ ChapNhan và đề xuất YC1 ChoBenHoc có hai khung, 2 buổi/tuần,60phút, phí150000 VND/Gio và bắt đầu tương lai. Đây là chuẩn bị fixture, chưa phải thao tác đang kiểm.
3. Chụp baseline bằng DbContext mới AsNoTracking: liên hệ, yêu cầu, khung đề xuất, lớp, thành viên, hội thoại, lịch, buổi học, notice/outbox.
4. Tạo client đúng actor, đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; GET form để lấy antiforgery cùng client.
5. Tạo liên hệ G/H2, đề xuất và chấp nhận bằng service/SQL thật, hình thành lớp hoạt động có lịch cùng giờ với YC1. Chụp baseline gồm lớp xung đột này rồi H POST chấp nhận YC1.
6. Gửi POST AJAX qua pipeline thật; thu status,JSON,redirectUrl; interceptor chỉ quan sát lệnh và mã lỗi SQL, không thay kết quả.
7. Đọc lại SQL bằng scope mới, đối chiếu cả dữ liệu phải thay đổi và dữ liệu không được đổi. So lịch đầu tiên với khung đề xuất và kiểm một BuoiHoc cho mỗi LichHoc; ca thất bại phải không có bản ghi tạo dở.
8. Dừng host, xóa đúng dữ liệu fixture theo khóa ngoại: hội thoại và buổi/lịch/thành viên trước lớp; khung trước yêu cầu. Lưu JSON/TRX và kết quả cleanup.

**Kỳ vọng HTTP:** 409/lỗi nghiệp vụ theo Rules ánh xạ SQL.
**Kỳ vọng SQL:** Không tồn tại lớp, thành viên, hội thoại, lịch mới; YC chưa DaChapNhan.

**Quan sát:** HTTP 409 với SQL 51145 trùng lịch thật. Lớp xung đột fixture vẫn còn; không có lớp của YC1 hoặc thành viên/hội thoại/lịch/buổi mới. Snapshot trước/sau giống hệt, YC1 chưa DaChapNhan.

**Baseline và sau request** bao gồm dữ liệu fixture, không chỉ bản ghi phát sinh:

```json
{
  "before": {
    "Contacts": 2,
    "Requests": 2,
    "Slots": 4,
    "Rooms": 3,
    "Classes": 1,
    "Notices": 7,
    "Events": 7,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2
  },
  "after": {
    "Contacts": 2,
    "Requests": 2,
    "Slots": 4,
    "Rooms": 3,
    "Classes": 1,
    "Notices": 7,
    "Events": 7,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2
  },
  "sqlErrorNumbers": [
    51145
  ],
  "classIdsFromResponses": null
}
```

**HTTP:**

```json
{
  "status": 409,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": false,
    "message": "Lịch trùng lớp hiện tại hoặc lịch của gia sư/bên học.",
    "errors": {
      "": [
        "Lịch trùng lớp hiện tại hoặc lịch của gia sư/bên học."
      ]
    }
  }
}
```

**Truy vấn của kịch bản:**

```sql
SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE Id=@Request; SELECT * FROM dbo.[LopHoc] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[CuocHoiThoai] WHERE LopHocId=@Class; SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId IN(SELECT Id FROM dbo.LichHoc WHERE LopHocId=@Class);
```

### IT-061 — Hai client chấp nhận đồng thời

**Đích:** `Hai POST /KetNoi/KetNoi/ChapNhan qua barrier`; **Tiêu chuẩn:** Đúng đắn.
**Input riêng:** Hai phiên H; cùng YC1/RV.

**Các bước thực hiện:**

1. Đối chiếu DB_NAME và @@SERVERNAME; tạo fixture mới có tag/ID riêng, H/G/X hoạt động, nhu cầu học và hồ sơ gia sư hợp lệ.
2. Dùng service/EF/SQL thật tạo liên hệ ChapNhan và đề xuất YC1 ChoBenHoc có hai khung, 2 buổi/tuần,60phút, phí150000 VND/Gio và bắt đầu tương lai. Đây là chuẩn bị fixture, chưa phải thao tác đang kiểm.
3. Chụp baseline bằng DbContext mới AsNoTracking: liên hệ, yêu cầu, khung đề xuất, lớp, thành viên, hội thoại, lịch, buổi học, notice/outbox.
4. Tạo client đúng actor, đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; GET form để lấy antiforgery cùng client.
5. Tạo hai HttpClient đăng nhập H riêng, token riêng; cả hai chờ barrier rồi cùng POST YC1/RV. Thu cả hai response và kiểm một bộ dữ liệu lớp.
6. Gửi POST AJAX qua pipeline thật; thu status,JSON,redirectUrl; interceptor chỉ quan sát lệnh và mã lỗi SQL, không thay kết quả.
7. Đọc lại SQL bằng scope mới, đối chiếu cả dữ liệu phải thay đổi và dữ liệu không được đổi. So lịch đầu tiên với khung đề xuất và kiểm một BuoiHoc cho mỗi LichHoc; ca thất bại phải không có bản ghi tạo dở.
8. Dừng host, xóa đúng dữ liệu fixture theo khóa ngoại: hội thoại và buổi/lịch/thành viên trước lớp; khung trước yêu cầu. Lưu JSON/TRX và kết quả cleanup.

**Kỳ vọng HTTP:** Không có hai kết quả trỏ hai lớp khác nhau.
**Kỳ vọng SQL:** Unique UQ_YeuCau_LopHoc; một lớp và một bộ dữ liệu liên quan.

**Quan sát:** Hai POST qua barrier đều HTTP200, cùng một mã lớp. Có đúng một lớp, hai thành viên, một hội thoại TheoLop, hai lịch/hai buổi, chỉ tăng một notice/outbox. UQ_YeuCau_LopHoc tồn tại.

**Baseline và sau request** bao gồm dữ liệu fixture, không chỉ bản ghi phát sinh:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 3,
    "Events": 3,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 4,
    "Events": 4,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2
  },
  "sqlErrorNumbers": [],
  "classIdsFromResponses": [
    "7c4b5a10-9570-4696-bea8-d74125024800",
    "7c4b5a10-9570-4696-bea8-d74125024800"
  ]
}
```

**HTTP:**

```json
[
  {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": {
      "ok": true,
      "message": "Lớp đã được tạo. Bạn có thể tiếp tục mở hoặc đóng nhu cầu tại mục Nhu cầu.",
      "redirectUrl": "/HocTap/LopHoc/ChiTiet/7c4b5a10-9570-4696-bea8-d74125024800"
    }
  },
  {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": {
      "ok": true,
      "message": "Lớp đã được tạo. Bạn có thể tiếp tục mở hoặc đóng nhu cầu tại mục Nhu cầu.",
      "redirectUrl": "/HocTap/LopHoc/ChiTiet/7c4b5a10-9570-4696-bea8-d74125024800"
    }
  }
]
```

**Truy vấn của kịch bản:**

```sql
SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE Id=@Request; SELECT * FROM dbo.[LopHoc] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[CuocHoiThoai] WHERE LopHocId=@Class; SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId IN(SELECT Id FROM dbo.LichHoc WHERE LopHocId=@Class);
```

### IT-062 — Từ chối yêu cầu

**Đích:** `POST /KetNoi/KetNoi/TuChoi`; **Tiêu chuẩn:** Đúng đắn.
**Input riêng:** H đúng người nhận; lý do <=2000, RV hiện tại.

**Các bước thực hiện:**

1. Đối chiếu DB_NAME và @@SERVERNAME; tạo fixture mới có tag/ID riêng, H/G/X hoạt động, nhu cầu học và hồ sơ gia sư hợp lệ.
2. Dùng service/EF/SQL thật tạo liên hệ ChapNhan và đề xuất YC1 ChoBenHoc có hai khung, 2 buổi/tuần,60phút, phí150000 VND/Gio và bắt đầu tương lai. Đây là chuẩn bị fixture, chưa phải thao tác đang kiểm.
3. Chụp baseline bằng DbContext mới AsNoTracking: liên hệ, yêu cầu, khung đề xuất, lớp, thành viên, hội thoại, lịch, buổi học, notice/outbox.
4. Tạo client đúng actor, đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; GET form để lấy antiforgery cùng client.
5. H POST TuChoi với lyDo hợp lệ và RowVersion hiện tại.
6. Gửi POST AJAX qua pipeline thật; thu status,JSON,redirectUrl; interceptor chỉ quan sát lệnh và mã lỗi SQL, không thay kết quả.
7. Đọc lại SQL bằng scope mới, đối chiếu cả dữ liệu phải thay đổi và dữ liệu không được đổi. So lịch đầu tiên với khung đề xuất và kiểm một BuoiHoc cho mỗi LichHoc; ca thất bại phải không có bản ghi tạo dở.
8. Dừng host, xóa đúng dữ liệu fixture theo khóa ngoại: hội thoại và buổi/lịch/thành viên trước lớp; khung trước yêu cầu. Lưu JSON/TRX và kết quả cleanup.

**Kỳ vọng HTTP:** Thành công.
**Kỳ vọng SQL:** TuChoi; không tạo lớp; thông báo đối tác.

**Quan sát:** HTTP200; YC1 chuyển TuChoi, không tạo lớp. Thông báo mới gửi đúng G và mã YC1.

**Baseline và sau request** bao gồm dữ liệu fixture, không chỉ bản ghi phát sinh:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 3,
    "Events": 3,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 4,
    "Events": 4,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0
  },
  "sqlErrorNumbers": [],
  "classIdsFromResponses": null
}
```

**HTTP:**

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

**Truy vấn của kịch bản:**

```sql
SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE Id=@Request; SELECT * FROM dbo.[LopHoc] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[CuocHoiThoai] WHERE LopHocId=@Class; SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId IN(SELECT Id FROM dbo.LichHoc WHERE LopHocId=@Class);
```

### IT-063 — Rút bởi người gửi

**Đích:** `POST /KetNoi/KetNoi/Rut`; **Tiêu chuẩn:** Đúng đắn.
**Input riêng:** G là bên gửi đề xuất; chưa hạn, RV hiện tại.

**Các bước thực hiện:**

1. Đối chiếu DB_NAME và @@SERVERNAME; tạo fixture mới có tag/ID riêng, H/G/X hoạt động, nhu cầu học và hồ sơ gia sư hợp lệ.
2. Dùng service/EF/SQL thật tạo liên hệ ChapNhan và đề xuất YC1 ChoBenHoc có hai khung, 2 buổi/tuần,60phút, phí150000 VND/Gio và bắt đầu tương lai. Đây là chuẩn bị fixture, chưa phải thao tác đang kiểm.
3. Chụp baseline bằng DbContext mới AsNoTracking: liên hệ, yêu cầu, khung đề xuất, lớp, thành viên, hội thoại, lịch, buổi học, notice/outbox.
4. Tạo client đúng actor, đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; GET form để lấy antiforgery cùng client.
5. G POST Rut với YC1/RowVersion hiện tại khi chưa hết hạn.
6. Gửi POST AJAX qua pipeline thật; thu status,JSON,redirectUrl; interceptor chỉ quan sát lệnh và mã lỗi SQL, không thay kết quả.
7. Đọc lại SQL bằng scope mới, đối chiếu cả dữ liệu phải thay đổi và dữ liệu không được đổi. So lịch đầu tiên với khung đề xuất và kiểm một BuoiHoc cho mỗi LichHoc; ca thất bại phải không có bản ghi tạo dở.
8. Dừng host, xóa đúng dữ liệu fixture theo khóa ngoại: hội thoại và buổi/lịch/thành viên trước lớp; khung trước yêu cầu. Lưu JSON/TRX và kết quả cleanup.

**Kỳ vọng HTTP:** Thành công.
**Kỳ vọng SQL:** DaRut; không lớp.

**Quan sát:** HTTP200; G gửi rút, YC1 chuyển DaRut; không tạo lớp. Thông báo mới đúng H/mã yêu cầu.

**Baseline và sau request** bao gồm dữ liệu fixture, không chỉ bản ghi phát sinh:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 3,
    "Events": 3,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 4,
    "Events": 4,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0
  },
  "sqlErrorNumbers": [],
  "classIdsFromResponses": null
}
```

**HTTP:**

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

**Truy vấn của kịch bản:**

```sql
SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE Id=@Request; SELECT * FROM dbo.[LopHoc] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[CuocHoiThoai] WHERE LopHocId=@Class; SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId IN(SELECT Id FROM dbo.LichHoc WHERE LopHocId=@Class);
```

### IT-064 — Người nhận cố rút

**Đích:** `POST /KetNoi/KetNoi/Rut`; **Tiêu chuẩn:** Xác thực.
**Input riêng:** H là người nhận đề xuất.

**Các bước thực hiện:**

1. Đối chiếu DB_NAME và @@SERVERNAME; tạo fixture mới có tag/ID riêng, H/G/X hoạt động, nhu cầu học và hồ sơ gia sư hợp lệ.
2. Dùng service/EF/SQL thật tạo liên hệ ChapNhan và đề xuất YC1 ChoBenHoc có hai khung, 2 buổi/tuần,60phút, phí150000 VND/Gio và bắt đầu tương lai. Đây là chuẩn bị fixture, chưa phải thao tác đang kiểm.
3. Chụp baseline bằng DbContext mới AsNoTracking: liên hệ, yêu cầu, khung đề xuất, lớp, thành viên, hội thoại, lịch, buổi học, notice/outbox.
4. Tạo client đúng actor, đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; GET form để lấy antiforgery cùng client.
5. H POST Rut với YC1/RowVersion thật dù H không phải người gửi.
6. Gửi POST AJAX qua pipeline thật; thu status,JSON,redirectUrl; interceptor chỉ quan sát lệnh và mã lỗi SQL, không thay kết quả.
7. Đọc lại SQL bằng scope mới, đối chiếu cả dữ liệu phải thay đổi và dữ liệu không được đổi. So lịch đầu tiên với khung đề xuất và kiểm một BuoiHoc cho mỗi LichHoc; ca thất bại phải không có bản ghi tạo dở.
8. Dừng host, xóa đúng dữ liệu fixture theo khóa ngoại: hội thoại và buổi/lịch/thành viên trước lớp; khung trước yêu cầu. Lưu JSON/TRX và kết quả cleanup.

**Kỳ vọng HTTP:** 403.0
**Kỳ vọng SQL:** Không đổi trạng thái.

**Quan sát:** HTTP 403; H là người nhận cố rút đề xuất của G. Snapshot nghiệp vụ không đổi; không có lớp.

**Baseline và sau request** bao gồm dữ liệu fixture, không chỉ bản ghi phát sinh:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 3,
    "Events": 3,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 1,
    "Classes": 0,
    "Notices": 3,
    "Events": 3,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0
  },
  "sqlErrorNumbers": [],
  "classIdsFromResponses": null
}
```

**HTTP:**

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

**Truy vấn của kịch bản:**

```sql
SELECT * FROM dbo.[YeuCauKetNoiLopHoc] WHERE Id=@Request; SELECT * FROM dbo.[LopHoc] WHERE YeuCauKetNoiId=@Request; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[CuocHoiThoai] WHERE LopHocId=@Class; SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId IN(SELECT Id FROM dbo.LichHoc WHERE LopHocId=@Class);
```

## Cách hiểu các ca quan trọng

- IT-055 đối chiếu từng trường điều kiện lớp, đúng hai thành viên DaDuyet/ChinhSua, một hội thoại TheoLop, đủ hai lịch đầu tiên đúng ngày/giờ từ khung đề xuất và một BuoiHoc DuKien cho mỗi lịch.
- IT-056 giữ nguyên RV đầu cho lần retry sau khi đã chấp nhận; hai URL trả cùng mã lớp và toàn bộ snapshot sau hai lần giống nhau, gồm cả notice/outbox.
- IT-058 dùng RV1 thật trước cập nhật v2; IT-059 seed cả GuiLuc/DeXuatLuc/HetHanLuc thỏa CHECK 72h rồi lấy RV mới, vì vậy lỗi phải do hết hạn thay vì vô tình dùng RV cũ. Cả hai quan sát SQL 51152 thật.
- IT-060 baseline đã có một lớp xung đột của G/H2 cùng giờ. Sau request lỗi, lớp đó vẫn còn; lớp của YC1 không tồn tại. Không diễn giải tổng số lớp=1 thành lớp tạo dở. SQL 51145 và toàn bộ snapshot không đổi là minh chứng rollback của thao tác YC1.
- IT-061 hai client/token riêng cùng khởi chạy qua barrier; quan sát hai HTTP 200 cùng mã lớp, một bộ dữ liệu và một notice/outbox mới. Đọc unique index từ sys.indexes; không cố ép lỗi unique hoặc khẳng định đã bao phủ mọi thứ tự tranh chấp.
- IT-062 kiểm hành vi từ chối và thông báo; kịch bản không yêu cầu lý do phải được lưu ở một cột riêng nên kết quả này không chứng minh có lưu lịch sử lý do.

## File thêm và sửa

- `tests/Integration0203/Cases05.cs`:10Fact, Trait Suite=IT05ChapNhanTaoLop.
- `tests/Integration0203/AcceptClassIntegrationTests.cs`: fixture đề xuất thật, các request HTTP, dữ liệu lớp/thành viên/lịch/buổi, retry, barrier và assertion rollback; ghi OBSERVATION_JSON trước khi assertion Fail.
- `tests/Integration0203/IntegrationHarness.cs`: mở rộng cleanup theo ID sở hữu, xóa lịch sử/buổi/lịch/thành viên trước lớp, rồi mới xóa khung/yêu cầu. Không xóa toàn bộ DB.
- Tái sử dụng helper login,antiforgery,form đề xuất,snapshot và interceptor chỉ quan sát từ nhóm04. Không thay code nghiệp vụ/SQL so với snapshot nhóm04; hash các file ứng dụng đối chiếu không đổi.

## Chạy riêng trong VS Code

Mở thư mục `C:\Khóa luận TN\Educonnect`, Terminal PowerShell:

```powershell
$env:EDU_TEST_ROOT='C:\Khóa luận TN\Educonnect'
dotnet test tests/Integration0203/Integration0203.csproj --filter "Suite=IT05ChapNhanTaoLop" --logger "trx;LogFileName=ChapNhan-TaoLop-Integration.trx" --results-directory "TestResults/05-ChapNhan-TaoLop-Integration"

dotnet test tests/Integration0203/Integration0203.csproj --filter "FullyQualifiedName~Integration0203Tests.IT_057"
```

Nếu ứng dụng đang khóa output build, dừng app hoặc dùng BaseOutputPath riêng và UseAppHost=false như lần chạy này. Harness chỉ chấp nhận EduConnect_Verification_IT0203; không đổi sang DB ứng dụng. Exitcode1 của lượt này do assertion IT-057, không phải không kết nối được SQL.

## Hồi quy và cleanup

Đã chạy lại39 ca Integration02–04:34 Pass, 5 Fail; trạng thái từng mã không đổi. Năm Fail cũ IT-022/023/025/034/037 không tính vào10 ca mới, không ghi đè kết quả nhóm trước. TRX hồi quy tại work/it05-regression/Regression02-04-After05.trx.

Cả10 ca cleanup thành công; sau lượt chính RemainingITFixtureUsers=0. Snapshot được chụp trước cleanup và nằm đầy đủ trong KetQua.json/TRX. Hash file SQL nguồn và định nghĩa procedure/trigger trong DB được lưu để truy nguyên; hai loại hash không đại diện cùng một chuỗi dữ liệu.

Bộ kết quả gồm KetQua.md/json,TRX thật,XacMinh-SQL.txt,ZIP mã nguồn/minh chứng và SHA256.json. ZIP có bản harness trước sửa cùng diff.
