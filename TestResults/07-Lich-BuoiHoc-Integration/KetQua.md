# Kết quả Integration 07 Lịch và buổi học

**12 ca: 11 Pass, 1 Fail, 0 Skip/Blocked.**

Kết quả chính thức lấy từ lượt chạy tuần tự, giữ đúng mã, tiêu chuẩn và kỳ vọng của Integration Test.xlsx hiện tại. Không sửa nghiệp vụ hoặc đổi kỳ vọng để làm Pass.

## Phạm vi

CSDL: `LAPTOP-3L2V1AFT\SQLEXPRESS / EduConnect_Verification_IT0203`. HTTP chạy qua TestServer với Identity, cookie, phiên SQL, antiforgery, binding/controller/filter và service/EF/SQL Server thật. Không mock lỗi SQL hoặc service.

Host Development, tắt DemoPublishProfiles, dừng BaoTriWorker/XuLySuKienWorker để so dữ liệu ổn định. Kiểm notice/outbox lưu trong SQL, không kiểm phát thông báo bên ngoài. Dùng Windows integrated operator cho DB test, không kiểm quyền tối thiểu tài khoản DB ứng dụng.

## Kết quả từng ca

| Mã | Tình huống | Tiêu chuẩn | Kết quả | Quan sát |
| --- | --- | --- | --- | --- |
| IT-075 | Tạo lịch hàng loạt | Đầy đủ | Pass | HTTP 200; thêm đúng hai lịch DaLenLich của C1 và hai BuoiHoc DuKien tương ứng, không thừa. |
| IT-076 | Lịch trùng nội bộ | Đúng đắn | Pass | HTTP 409, SQL 51144 thật do hai buổi trong batch chồng lấn; toàn bộ snapshot không đổi. |
| IT-077 | Trùng lịch gia sư lớp khác | Đúng đắn | Pass | HTTP 409, SQL 51145 thật do G có lịch ở lớp khác. Không thêm một phần batch; lớp/lịch xung đột fixture vẫn giữ nguyên. |
| IT-078 | Đổi lịch buổi chưa bắt đầu | Đầy đủ | Pass | HTTP 200; lịch đổi đúng giờ mới và giữ Id lịch/liên kết buổi. Một LichSuDieuChinhLich ghi đúng giờ cũ/mới, lý do và gia sư sửa. |
| IT-079 | Không đổi giờ buổi đang diễn ra | Đúng đắn | Pass | HTTP 409 khi sửa lịch của BuoiHoc DangDienRa. Probe UPDATE SQL trực tiếp cũng bị trigger chặn bằng 51120; giờ/lịch sử không đổi. |
| IT-080 | Bắt đầu buổi học | Đúng đắn | Pass | HTTP 200; DuKien chuyển DangDienRa, PhienBan tăng đúng1. Bản cũ chưa ghi nhận nên trigger không thêm lịch sử ghi nhận. |
| IT-081 | Ghi nhận buổi hoàn tất | Đầy đủ | Pass | HTTP 200; buổi DaHoc/DaDienRa, đúng G và giờ sau kết thúc. PhienBan tăng 1; đúng một lịch sử chứa phiên bản/nội dung cũ. |
| IT-082 | Ghi nhận trước giờ kết thúc | Đúng đắn | Pass | HTTP 409, không hoàn tất sớm. Probe UPDATE SQL trực tiếp bị51118; trạng thái, phiên bản và lịch sử nguyên trạng. |
| IT-083 | Người không phụ trách ghi nhận | Xác thực | Fail | Kỳ vọng HTTP 403 nhưng HTTP 422 JSON lỗi quyền vào lớp. Buổi/lịch sử và toàn bộ dữ liệu nghiệp vụ không đổi; không gọi procedure ghi nhận. |
| IT-084 | Thông báo nghỉ cần học bù | Đầy đủ | Pass | HTTP 200; buổi DaNghi, CanHocBu=true, lý do đã trim, đúng người và giờ nghỉ; lịch DaHuy. |
| IT-085 | Tạo buổi bù | Đúng đắn | Pass | HTTP 200; thêm một lịch bù LaBuoiBu=true/LichGocId đúng và một BuoiHoc mới; giữ lịch gốc DaHuy, buổi gốc DaNghi/CanHocBu. |
| IT-086 | Hai lần ghi nhận dùng cùng RV | Đúng đắn | Pass | Hai POST A/B cùng RV qua barrier: một HTTP 200, một HTTP 409; quan sát SQL 51150 thật. Nội dung bản thắng được giữ, phiên bản tăng 1, chỉ thêm một lịch sử cũ, không trùng phiên bản. |

## Chi tiết ca Fail

### IT-083 — Người không phụ trách ghi nhận

**Input:** G2 không là GiaSuId lớp.
**Kỳ vọng HTTP:** 403
**Kỳ vọng SQL:** B1 và lịch sử không đổi.

**Thực tế:** Kỳ vọng HTTP 403 nhưng HTTP 422 JSON lỗi quyền vào lớp. Buổi/lịch sử và toàn bộ dữ liệu nghiệp vụ không đổi; không gọi procedure ghi nhận.

**Nguyên nhân:** Đối chiếu code: LayAsync kiểm quyền và ném 403. GhiNhan catch gọi ThemLoi, thêm ModelState và đặt Response.StatusCode=403. Sau đó action gọi Buoi(m.LichHocId) để nạp lại màn hình; Buoi kiểm quyền lại và ném 403. UiResponseFilter.OnException thấy POST cùng ModelState lỗi nên đi nhánh hard-code 422, thay mã 403 ban đầu.

**Ảnh hưởng và phần vẫn đạt:** Quyền ghi nhận vẫn bị chặn: buổi, phiên bản và lịch sử không đổi; procedure ghi nhận không chạy. Sai lệch nằm ở mã giao tiếp HTTP.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\HocTap\Controllers\LichHocController.cs:11` — `private void ThemLoi`.
- `C:\Khóa luận TN\Educonnect\Areas\HocTap\Controllers\LichHocController.cs:15` — `Response.StatusCode = Rules.ErrorStatus`.
- `C:\Khóa luận TN\Educonnect\Areas\HocTap\Controllers\LichHocController.cs:149` — `await Buoi(m.LichHocId);`.
- `C:\Khóa luận TN\Educonnect\Infrastructure\UiResponseFilter.cs:30` — `StatusCode = StatusCodes.Status422UnprocessableEntity`.
- `C:\Khóa luận TN\Educonnect\Infrastructure\QuyenLopHocService.cs:17` — `Bạn chưa có quyền hoặc liên kết phù hợp`.

**Đề xuất xử lý:** Không nạp lại View yêu cầu quyền sau khi đã xác định403, hoặc bảo toàn status lỗi quyền trong exception filter thay vì luôn422 khi ModelState lỗi. Giữ kiểm quyền hiện tại và kiểm lại cả403, 409 cùng lỗi nhập liệu422 sau khi sửa.

**Assertion không đạt:**

- Gia sư không phụ trách phảiHTTP403.

## Bước thực thi và bằng chứng từng ca

### IT-075 — Tạo lịch hàng loạt

**Tiêu chuẩn:** Đầy đủ; **đích:** `POST /HocTap/LichHoc/Them`.
**Input riêng:** LopHocId=C1, Buoi gồm 2 buổi tương lai không trùng.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Tạo lớp đang hoạt động; G POST Them với hai lịch tương lai không trùng, tránh các lịch đầu tiên fixture.
5. Đọc SQL xác nhận đúng hai LichHoc mới và đúng một BuoiHoc DuKien cho mỗi lịch.
6. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
7. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Thành công.; SQL 2 lịch mới và 2 BuoiHoc DuKien tương ứng do tr_LichHoc_TaoBuoi; không tạo thừa.

**Quan sát:** HTTP 200; thêm đúng hai lịch DaLenLich của C1 và hai BuoiHoc DuKien tương ứng, không thừa.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 4,
    "Events": 4,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 5,
    "Events": 5,
    "Members": 2,
    "Calendar": 4,
    "Lessons": 4,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "sqlErrorNumbers": [],
  "directSqlGuard": null
}
```

**HTTP chính:**

```json
{
  "status": 200,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": true,
    "message": "Đã lưu thay đổi.",
    "redirectUrl": "/HocTap/LopHoc/ChiTiet/49d6f32b-1039-4f39-a420-1f30a0c92a31?tab=schedule"
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId=@Schedule; SELECT * FROM dbo.[LichSuGhiNhanBuoiHoc] WHERE BuoiHocId=@Lesson; SELECT * FROM dbo.[LichSuDieuChinhLich] WHERE LichHocId=@Schedule;
```

### IT-076 — Lịch trùng nội bộ

**Tiêu chuẩn:** Đúng đắn; **đích:** `POST /HocTap/LichHoc/Them`.
**Input riêng:** Hai buổi 19:00–20:00 và 19:30–20:30 cùng ngày.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. G POST batch hai buổi cùng ngày 19:00–20:00 và 19:30–20:30 đúng như kịch bản; interceptor thu SQL 51144.
5. Đối chiếu snapshot trước/sau bằng nhau, không ghi một phần batch.
6. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
7. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Lỗi nghiệp vụ; không 500 mất kiểm soát.; SQL Không lưu một phần batch.

**Quan sát:** HTTP 409, SQL 51144 thật do hai buổi trong batch chồng lấn; toàn bộ snapshot không đổi.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 4,
    "Events": 4,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
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
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "sqlErrorNumbers": [
    51144
  ],
  "directSqlGuard": null
}
```

**HTTP chính:**

```json
{
  "status": 409,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": false,
    "message": "Kiểm tra lại thông tin đã nhập.",
    "errors": {
      "": [
        "Danh sách chọn có các buổi trùng giờ."
      ]
    }
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId=@Schedule; SELECT * FROM dbo.[LichSuGhiNhanBuoiHoc] WHERE BuoiHocId=@Lesson; SELECT * FROM dbo.[LichSuDieuChinhLich] WHERE LichHocId=@Schedule;
```

### IT-077 — Trùng lịch gia sư lớp khác

**Tiêu chuẩn:** Đúng đắn; **đích:** `POST /HocTap/LichHoc/Them`.
**Input riêng:** G có lịch chưa hủy ở lớp hoạt động khác trùng thời gian.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Tạo lớp khác G/H2 có lịch hoạt động ở ngày khác lịch ban đầu C1; chọn một giờ của lớp đó làm giờ mới cho C1.
5. G POST Them batch có buổi trùng lịch lớp khác; thu SQL 51145 và so snapshot không đổi.
6. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
7. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Từ chối.; SQL Không tạo lịch mới.

**Quan sát:** HTTP 409, SQL 51145 thật do G có lịch ở lớp khác. Không thêm một phần batch; lớp/lịch xung đột fixture vẫn giữ nguyên.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 2,
    "Requests": 2,
    "Slots": 4,
    "Rooms": 4,
    "Classes": 2,
    "Notices": 9,
    "Events": 9,
    "Members": 4,
    "Calendar": 4,
    "Lessons": 4,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "after": {
    "Contacts": 2,
    "Requests": 2,
    "Slots": 4,
    "Rooms": 4,
    "Classes": 2,
    "Notices": 9,
    "Events": 9,
    "Members": 4,
    "Calendar": 4,
    "Lessons": 4,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "sqlErrorNumbers": [
    51145
  ],
  "directSqlGuard": null
}
```

**HTTP chính:**

```json
{
  "status": 409,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": false,
    "message": "Kiểm tra lại thông tin đã nhập.",
    "errors": {
      "": [
        "Lịch trùng lớp hiện tại hoặc lịch của gia sư/bên học."
      ]
    }
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId=@Schedule; SELECT * FROM dbo.[LichSuGhiNhanBuoiHoc] WHERE BuoiHocId=@Lesson; SELECT * FROM dbo.[LichSuDieuChinhLich] WHERE LichHocId=@Schedule;
```

### IT-078 — Đổi lịch buổi chưa bắt đầu

**Tiêu chuẩn:** Đầy đủ; **đích:** `POST /HocTap/LichHoc/Sua`.
**Input riêng:** L1/B1 DuKien, RV đúng; giờ mới hợp lệ; lý do đổi.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. B1 DuKien, lịch tương lai còn sửa được. G POST Sua với RV lịch, giờ mới và lý do.
5. Kiểm giờ mới, Id lịch/LichHocId của buổi được giữ; một lịch sử điều chỉnh đủ cột cũ/mới/lý do/người.
6. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
7. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Thành công.; SQL Lịch mới theo handler thực tế; LichSuDieuChinhLich giữ giờ cũ/mới/lý do; không mất liên kết buổi.

**Quan sát:** HTTP 200; lịch đổi đúng giờ mới và giữ Id lịch/liên kết buổi. Một LichSuDieuChinhLich ghi đúng giờ cũ/mới, lý do và gia sư sửa.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 4,
    "Events": 4,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 5,
    "Events": 5,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 1,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "sqlErrorNumbers": [],
  "directSqlGuard": null
}
```

**HTTP chính:**

```json
{
  "status": 200,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": true,
    "message": "Đã lưu thay đổi.",
    "redirectUrl": "/HocTap/LopHoc/ChiTiet/0be302ab-46d5-4aae-8390-9c7eed634be3?tab=schedule"
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId=@Schedule; SELECT * FROM dbo.[LichSuGhiNhanBuoiHoc] WHERE BuoiHocId=@Lesson; SELECT * FROM dbo.[LichSuDieuChinhLich] WHERE LichHocId=@Schedule;
```

### IT-079 — Không đổi giờ buổi đang diễn ra

**Tiêu chuẩn:** Đúng đắn; **đích:** `POST /HocTap/LichHoc/Sua`.
**Input riêng:** B1 DangDienRa; đổi giờ L1.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Đặt lịch fixture trong cửa sổ đang diễn ra bằng giờ SQL, rồi gọi service BatDauBuoi thật để B1 DangDienRa trước baseline.
5. G POST Sua, kiểm từ chối. Thực hiện thêm UPDATE SQL trực tiếp đổi giờ để kiểm trigger 51120; probe riêng không đi qua HTTP.
6. Đọc lại xác nhận cả hai thao tác đều không đổi giờ hoặc tạo lịch sử giả.
7. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
8. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Từ chối.; SQL Giờ cũ nguyên trạng; trigger tr_LichHoc_GiuLichSu bảo vệ SQL trực tiếp.

**Quan sát:** HTTP 409 khi sửa lịch của BuoiHoc DangDienRa. Probe UPDATE SQL trực tiếp cũng bị trigger chặn bằng 51120; giờ/lịch sử không đổi.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 4,
    "Events": 4,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
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
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "sqlErrorNumbers": [],
  "directSqlGuard": 51120
}
```

**HTTP chính:**

```json
{
  "status": 409,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": false,
    "message": "Kiểm tra lại thông tin đã nhập.",
    "errors": {
      "": [
        "Không sửa hoặc hủy lịch đã bắt đầu, đã thay thế, đã hủy hay đã ghi nhận."
      ]
    }
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId=@Schedule; SELECT * FROM dbo.[LichSuGhiNhanBuoiHoc] WHERE BuoiHocId=@Lesson; SELECT * FROM dbo.[LichSuDieuChinhLich] WHERE LichHocId=@Schedule;
```

### IT-080 — Bắt đầu buổi học

**Tiêu chuẩn:** Đúng đắn; **đích:** `POST /HocTap/LichHoc/BatDauBuoi`.
**Input riêng:** G; thời gian phù hợp quy tắc; B1 DuKien, RV đúng.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Lịch fixture đang trong khung giờ theo giờ SQL, B1 vẫn DuKien.
5. G POST BatDauBuoi với RV của BuoiHoc; kiểm DangDienRa và PhienBan+1.
6. Đối chiếu quy tắc trigger: chưa có ghi nhận cũ thì không tạo lịch sử ghi nhận.
7. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
8. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Thành công.; SQL B1 DangDienRa, phiên bản tăng đúng; lịch sử đủ theo trigger.

**Quan sát:** HTTP 200; DuKien chuyển DangDienRa, PhienBan tăng đúng1. Bản cũ chưa ghi nhận nên trigger không thêm lịch sử ghi nhận.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 4,
    "Events": 4,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
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
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "sqlErrorNumbers": [],
  "directSqlGuard": null
}
```

**HTTP chính:**

```json
{
  "status": 200,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": true,
    "message": "Buổi học đã bắt đầu.",
    "redirectUrl": "/HocTap/LichHoc/Buoi/096e3f6a-0de0-f111-a50f-005056c00003"
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId=@Schedule; SELECT * FROM dbo.[LichSuGhiNhanBuoiHoc] WHERE BuoiHocId=@Lesson; SELECT * FROM dbo.[LichSuDieuChinhLich] WHERE LichHocId=@Schedule;
```

### IT-081 — Ghi nhận buổi hoàn tất

**Tiêu chuẩn:** Đầy đủ; **đích:** `POST /HocTap/LichHoc/GhiNhan`.
**Input riêng:** G; L1 đã kết thúc; nội dung, nhận xét, điểm danh hợp lệ; RV hiện tại.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Đặt lịch fixture đã kết thúc bằng giờ SQL; tạo một bản ghi nhận hợp lệ bằng service/SQL thật trước baseline để có phiên bản cũ cần lưu lịch sử.
5. G HTTP POST GhiNhan với RV hiện tại, nội dung A/nhận xét/điểm danh hợp lệ.
6. Kiểm DaHoc/DaDienRa, người/giờ, PhienBan+1, đúng một lịch sử có phiên bản/nội dung cũ.
7. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
8. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Thành công.; SQL DaHoc, DaDienRa, đúng người/giờ; PhienBan tăng 1; lịch sử bản cũ đúng một dòng.

**Quan sát:** HTTP 200; buổi DaHoc/DaDienRa, đúng G và giờ sau kết thúc. PhienBan tăng 1; đúng một lịch sử chứa phiên bản/nội dung cũ.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 5,
    "Events": 5,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 6,
    "Events": 6,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 1,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "sqlErrorNumbers": [],
  "directSqlGuard": null
}
```

**HTTP chính:**

```json
{
  "status": 200,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": true,
    "message": "Đã lưu thay đổi.",
    "redirectUrl": "/HocTap/LichHoc/Buoi/f26d3f6a-0de0-f111-a50f-005056c00003"
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId=@Schedule; SELECT * FROM dbo.[LichSuGhiNhanBuoiHoc] WHERE BuoiHocId=@Lesson; SELECT * FROM dbo.[LichSuDieuChinhLich] WHERE LichHocId=@Schedule;
```

### IT-082 — Ghi nhận trước giờ kết thúc

**Tiêu chuẩn:** Đúng đắn; **đích:** `POST /HocTap/LichHoc/GhiNhan`.
**Input riêng:** L1 kết thúc trong tương lai; input DaHoc.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Giữ lịch có giờ kết thúc tương lai, G POST GhiNhan DaDienRa.
5. Kiểm bị từ chối; thêm probe UPDATE SQL trực tiếp đầy đủ các trường DaHoc hợp lệ để buộc kiểm trigger 51118.
6. Đọc lại đảm bảo buổi chưa hoàn tất và lịch sử không tăng.
7. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
8. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Từ chối; trigger SQL 51118 bảo vệ nếu service bỏ sót.; SQL Không hoàn tất buổi; không lưu lịch sử thành công giả.

**Quan sát:** HTTP 409, không hoàn tất sớm. Probe UPDATE SQL trực tiếp bị51118; trạng thái, phiên bản và lịch sử nguyên trạng.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 4,
    "Events": 4,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
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
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "sqlErrorNumbers": [],
  "directSqlGuard": 51118
}
```

**HTTP chính:**

```json
{
  "status": 409,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": false,
    "message": "Kiểm tra lại thông tin đã nhập.",
    "errors": {
      "": [
        "Chỉ ghi nhận sau giờ kết thúc và lịch chưa bị hủy hoặc thay thế."
      ]
    }
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId=@Schedule; SELECT * FROM dbo.[LichSuGhiNhanBuoiHoc] WHERE BuoiHocId=@Lesson; SELECT * FROM dbo.[LichSuDieuChinhLich] WHERE LichHocId=@Schedule;
```

### IT-083 — Người không phụ trách ghi nhận

**Tiêu chuẩn:** Xác thực; **đích:** `POST /HocTap/LichHoc/GhiNhan`.
**Input riêng:** G2 không là GiaSuId lớp.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Tạo G2 thuộc vai trò GiaSu nhưng không phụ trách/không membership lớp; lịch đã kết thúc và form GhiNhan hợp lệ.
5. G2 POST GhiNhan với Id lịch/RV thật, ghi nhận422 thay403 là Fail.
6. So toàn bộ snapshot và kiểm procedure ghi nhận không được gọi.
7. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
8. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP 403.0; SQL B1 và lịch sử không đổi.

**Quan sát:** Kỳ vọng HTTP 403 nhưng HTTP 422 JSON lỗi quyền vào lớp. Buổi/lịch sử và toàn bộ dữ liệu nghiệp vụ không đổi; không gọi procedure ghi nhận.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 4,
    "Events": 4,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
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
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "sqlErrorNumbers": [],
  "directSqlGuard": null
}
```

**HTTP chính:**

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
        "Bạn chưa có quyền hoặc liên kết phù hợp để vào lớp này."
      ]
    }
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId=@Schedule; SELECT * FROM dbo.[LichSuGhiNhanBuoiHoc] WHERE BuoiHocId=@Lesson; SELECT * FROM dbo.[LichSuDieuChinhLich] WHERE LichHocId=@Schedule;
```

### IT-084 — Thông báo nghỉ cần học bù

**Tiêu chuẩn:** Đầy đủ; **đích:** `POST /HocTap/LichHoc/ThongBaoNghi`.
**Input riêng:** B1 hợp lệ; reason có nội dung; canHocBu=true; RV mới.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. G POST ThongBaoNghi với reason có khoảng trắng hai đầu, canHocBu=true, RV của buổi.
5. Đối chiếu DaNghi/CanHocBu, lý do trim, người/giờ nghỉ và lịch chuyển DaHuy.
6. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
7. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Thành công.; SQL DaNghi; CanHocBu=1; lý do/người/giờ nghỉ đầy đủ.

**Quan sát:** HTTP 200; buổi DaNghi, CanHocBu=true, lý do đã trim, đúng người và giờ nghỉ; lịch DaHuy.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 4,
    "Events": 4,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 5,
    "Events": 5,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "sqlErrorNumbers": [],
  "directSqlGuard": null
}
```

**HTTP chính:**

```json
{
  "status": 200,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": true,
    "message": "Đã thông báo nghỉ.",
    "redirectUrl": "/HocTap/LichHoc/Buoi/b6b72364-0de0-f111-a50f-005056c00003"
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId=@Schedule; SELECT * FROM dbo.[LichSuGhiNhanBuoiHoc] WHERE BuoiHocId=@Lesson; SELECT * FROM dbo.[LichSuDieuChinhLich] WHERE LichHocId=@Schedule;
```

### IT-085 — Tạo buổi bù

**Tiêu chuẩn:** Đúng đắn; **đích:** `POST /HocTap/LichHoc/TaoBuoiBu`.
**Input riêng:** B1 nghỉ cần bù; lịch mới không trùng.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Fixture dùng service thật thông báo nghỉ B1, CanHocBu=true; đọc RV lịch mới.
5. G POST TaoBuoiBu bằng SuaLichInput với Id lịch gốc, RV lịch và giờ mới không trùng.
6. Kiểm LaBuoiBu/LichGocId và BuoiHoc mới; giữ nguyên lịch/buổi gốc đã nghỉ.
7. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
8. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Thành công.; SQL LaBuoiBu=1, LichGocId đúng; lịch/buổi ban đầu được giữ.

**Quan sát:** HTTP 200; thêm một lịch bù LaBuoiBu=true/LichGocId đúng và một BuoiHoc mới; giữ lịch gốc DaHuy, buổi gốc DaNghi/CanHocBu.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 5,
    "Events": 5,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 6,
    "Events": 6,
    "Members": 2,
    "Calendar": 3,
    "Lessons": 3,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "sqlErrorNumbers": [],
  "directSqlGuard": null
}
```

**HTTP chính:**

```json
{
  "status": 200,
  "location": null,
  "contentType": "application/json; charset=utf-8",
  "body": {
    "ok": true,
    "message": "Đã thêm buổi học bù.",
    "redirectUrl": "/HocTap/LopHoc/ChiTiet/9d5fe309-97af-4310-a4f1-742a8d744dfe?tab=schedule"
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId=@Schedule; SELECT * FROM dbo.[LichSuGhiNhanBuoiHoc] WHERE BuoiHocId=@Lesson; SELECT * FROM dbo.[LichSuDieuChinhLich] WHERE LichHocId=@Schedule;
```

### IT-086 — Hai lần ghi nhận dùng cùng RV

**Tiêu chuẩn:** Đúng đắn; **đích:** `Hai POST /HocTap/LichHoc/GhiNhan`.
**Input riêng:** Cùng B1/RV nhưng nội dung A/B khác nhau.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Tạo bản ghi nhận hợp lệ trong fixture cho buổi đã kết thúc, giữ cùng một RV trước hai request.
5. Hai client G/token riêng chờ barrier rồi đồng thời POST nội dung A/B.
6. Đối chiếu200/409, SQL 51150, bản thắng/PhienBan+1 và chỉ một lịch sử cũ không trùng.
7. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
8. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Một ghi thành công; lần còn lại 409.; SQL Không mất cập nhật trước; lịch sử không trùng (BuoiHocId, PhienBan).

**Quan sát:** Hai POST A/B cùng RV qua barrier: một HTTP 200, một HTTP 409; quan sát SQL 51150 thật. Nội dung bản thắng được giữ, phiên bản tăng 1, chỉ thêm một lịch sử cũ, không trùng phiên bản.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 5,
    "Events": 5,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 6,
    "Events": 6,
    "Members": 2,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 1,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "sqlErrorNumbers": [
    51150
  ],
  "directSqlGuard": null
}
```

**HTTP chính:**

```json
[
  {
    "status": 200,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": {
      "ok": true,
      "message": "Đã lưu thay đổi.",
      "redirectUrl": "/HocTap/LichHoc/Buoi/fa6d3f6a-0de0-f111-a50f-005056c00003"
    }
  },
  {
    "status": 409,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": {
      "ok": false,
      "message": "Kiểm tra lại thông tin đã nhập.",
      "errors": {
        "": [
          "Ghi nhận đã thay đổi, hãy tải lại trước khi sửa."
        ]
      }
    }
  }
]
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LichHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[BuoiHoc] WHERE LichHocId=@Schedule; SELECT * FROM dbo.[LichSuGhiNhanBuoiHoc] WHERE BuoiHocId=@Lesson; SELECT * FROM dbo.[LichSuDieuChinhLich] WHERE LichHocId=@Schedule;
```

## Phạm vi cần hiểu đúng

- IT-079/082 gồm request HTTP và probe UPDATE SQL trực tiếp tách riêng. Cột directSqlGuard là lỗi thật từ SQL 51120/51118; các probe không bị nhầm thành lỗi SQL của request HTTP. Snapshot sau cả hai vẫn nguyên trạng.
- IT-080 tăng PhienBan đúng1 nhưng không tạo lịch sử ghi nhận vì bản cũ chưa có KetQuaBuoiHoc/GhiNhanLuc. Đây là điều kiện của trigger, không khẳng định lịch sử luôn phải tăng ở mọi thay đổi trạng thái.
- IT-081/086 chuẩn bị một bản ghi nhận hợp lệ bằng service/SQL trước baseline. Thao tác HTTP sửa bản đã có để kiểm chính xác một lịch sử bản cũ và phiên bản tăng 1; không giả rằng buổi chưa từng ghi nhận luôn tạo lịch sử cũ.
- IT-086 gửi hai request A/B cùng RowVersion qua barrier, giữ nội dung đúng request thắng và kiểm SQL 51150 thật. Không thay lỗi concurrency bằng ngoại lệ giả.
- IT-077 baseline có thêm lớp/lịch xung đột G/H2; sau lỗi lớp đó vẫn còn. So snapshot nguyên trạng để kiểm không tạo một phần batch, không yêu cầu tổng lịch của toàn fixture bằng0.
- IT-085 giữ lịch gốc DaHuy và buổi gốc DaNghi/CanHocBu, thêm lịch bù có LichGocId cùng một BuoiHoc mới; không xóa dấu vết lịch sử gốc.

## File thêm và sửa

- `tests/Integration0203/Cases0607.cs`:22 Fact chia Trait Suite=IT06ThanhVienLienKet và IT07LichBuoiHoc.
- `MemberLinkIntegrationTests.cs`:10 ca nhóm06 cùng helper fixture lớp/liên kết và snapshot mở rộng; kiểm request tiếp sau thu hồi quyền.
- `ScheduleLessonIntegrationTests.cs`:12 ca nhóm07; helper form, giờ SQL, probe trigger trực tiếp và hai request cùng RV.
- `IntegrationHarness.cs`:dọn thêm lịch sử điều chỉnh, liên kết, bài tập/nộp/chấm theo ID sở hữu trước khi xóa lớp/người dùng. Không sửa schema hoặc điều kiện nghiệp vụ.
- ZIP chứa các file nguồn liên quan, bản harness trước thay đổi, diff và JSON minh chứng từng ca. Đây là snapshot hỗ trợ đọc code trong project hiện tại.

## Chạy riêng trong VS Code

Mở thư mục `C:\Khóa luận TN\Educonnect`, Terminal PowerShell:

```powershell
$env:EDU_TEST_ROOT='C:\Khóa luận TN\Educonnect'
dotnet test tests/Integration0203/Integration0203.csproj --filter "Suite=IT07LichBuoiHoc" --logger "trx;LogFileName=Lich-BuoiHoc-Integration.trx" --results-directory "TestResults/07-Lich-BuoiHoc-Integration"

dotnet test tests/Integration0203/Integration0203.csproj --filter "FullyQualifiedName~Integration0203Tests.IT_083"
```

Chạy tuần tự hai nhóm trên cùng DB. Nếu app đang khóa output build, dừng app hoặc dùng BaseOutputPath riêng và UseAppHost=false. Harness chỉ chấp nhận EduConnect_Verification_IT0203, không dùng DB ứng dụng.

## Lượt chạy thử và hồi quy

Lượt đầu chạy hai tiến trình nhóm06/07 song song làm bốn ca IT-072/077/081/086 deadlock1205 trong khởi tạo fixture, trước request cần kiểm. Các log/TRX thử được giữ tại work/it0607-first. Kết quả chính thức ở đây là chạy lại đầy đủ hai nhóm tuần tự, không đổi assertion hoặc code ứng dụng. Không kết luận SQL không có deadlock trong các tải đồng thời khác.

Hồi quy 49 ca nhóm02–05:43 Pass, 6 Fail; đối chiếu từng mã không đổi. Sáu Fail cũ IT-022/023/025/034/037/057 không tính vào22 ca mới và không ghi đè báo cáo nhóm cũ. TRX hồi quy tại work/it0607-regression/Regression02-05-After0607.trx.

Cả 22 ca chính thức cleanup thành công. Sau lượt chính và hồi quy, còn 0 tài khoản/lớp fixture theo truy vấn xác minh. Dữ liệu SQL trước/sau được lưu trước cleanup. Các nhật ký an toàn anonymous có thể còn trong DB kiểm thử riêng.

File KetQua.json có đầy đủ ID, RowVersion, input đã bỏ token, HTTP, snapshot từng cột, SQL errors và assertion. TRX có OBSERVATION_JSON tương ứng. XacMinh-SQL.txt lưu DB/hash định nghĩa procedure/trigger và kiểm cleanup; SHA256.json dùng đối chiếu tính toàn vẹn.
