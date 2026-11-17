# Kết quả Integration 06 Thành viên và liên kết

**10 ca: 9 Pass, 1 Fail, 0 Skip/Blocked.**

Kết quả chính thức lấy từ lượt chạy tuần tự, giữ đúng mã, tiêu chuẩn và kỳ vọng của Integration Test.xlsx hiện tại. Không sửa nghiệp vụ hoặc đổi kỳ vọng để làm Pass.

## Phạm vi

CSDL: `LAPTOP-3L2V1AFT\SQLEXPRESS / EduConnect_Verification_IT0203`. HTTP chạy qua TestServer với Identity, cookie, phiên SQL, antiforgery, binding/controller/filter và service/EF/SQL Server thật. Không mock lỗi SQL hoặc service.

Host Development, tắt DemoPublishProfiles, dừng BaoTriWorker/XuLySuKienWorker để so dữ liệu ổn định. Kiểm notice/outbox lưu trong SQL, không kiểm phát thông báo bên ngoài. Dùng Windows integrated operator cho DB test, không kiểm quyền tối thiểu tài khoản DB ứng dụng.

## Kết quả từng ca

| Mã | Tình huống | Tiêu chuẩn | Kết quả | Quan sát |
| --- | --- | --- | --- | --- |
| IT-065 | Phụ huynh gửi liên kết | Đúng đắn | Pass | HTTP 200, một liên kết ChoXacNhan đúng P/H; NguoiGuiId=P và chưa kết thúc. |
| IT-066 | Học sinh đồng ý liên kết | Đúng đắn | Pass | HTTP 200; liên kết DaLienKet, XacNhanLuc không trước GuiLuc, chưa có KetThucLuc. |
| IT-067 | Học sinh từ chối liên kết | Đúng đắn | Pass | HTTP 200; liên kết TuChoi, KetThucLuc hợp lệ và NguoiKetThucId=H. |
| IT-068 | Hủy liên kết | Đúng đắn | Pass | HTTP 200; liên kết DaHuy đúng P. Membership H phụ thuộc liên kết chuyển DaRoiLop và có RoiLopLuc. Giữ cookie H: GET lớp và POST gửi tin tiếp theo đều HTTP 403, không lưu tin mới. |
| IT-069 | Mời thành viên | Đúng đắn | Fail | HTTP 405 Method Not Allowed tại POST MoiThanhVien; không tạo membership/lời mời. Code chỉ đăng ký GET MoiThanhVien, POST dùng DeNghi. Ca Fail, không thay route để đánh Pass. |
| IT-070 | Duyệt quyền xem | Đúng đắn | Pass | HTTP 200; membership của H DaDuyet/Xem, đúng LienKetId, ChoPhepNopBai=false. H dùng bài tập hợp lệ thử POST Nop: HTTP 403; không có bài nộp. |
| IT-071 | Cấp quyền nộp bài | Đúng đắn | Pass | HTTP 200; H DaDuyet/ChinhSua, ChoPhepNopBai=true và gắn đúng liên kết đang hoạt động. |
| IT-072 | Người ngoài truy cập lớp | Xác thực | Pass | HTTP 403 cho H2 ngoài lớp; toàn bộ dữ liệu trước/sau giống nhau. |
| IT-073 | Thu hồi quyền có hiệu lực request sau | Xác thực | Pass | H truy cập lớp HTTP 200 trước đổi quyền. G HTTP Duyet hạ H từ ChinhSua xuống Xem thành công; cùng cookie H gửi tin tiếp theo nhận HTTP 403, không có tin mới. |
| IT-074 | Liên kết gia đình không tự cấp quyền lớp | Xác thực | Pass | P liên kết với H là bên học chính nhưng P không có membership riêng. GET lớp HTTP 403, không tự tạo membership hoặc đổi dữ liệu. |

## Chi tiết ca Fail

### IT-069 — Mời thành viên

**Input:** G đủ quyền mời; email H.
**Kỳ vọng HTTP:** Thành công khi đúng vai trò và trạng thái.
**Kỳ vọng SQL:** Một đề nghị/lời mời, không tự vượt bước xác nhận.

**Thực tế:** HTTP 405 Method Not Allowed tại POST MoiThanhVien; không tạo membership/lời mời. Code chỉ đăng ký GET MoiThanhVien, POST dùng DeNghi. Ca Fail, không thay route để đánh Pass.

**Nguyên nhân:** LopHocController.MoiThanhVien chỉ có HttpGet và trả View cho bước tìm người được mời. POST tạo lời mời hiện nằm ở DeNghi. Vì request của TC dùng POST MoiThanhVien, routing trả405 trước service; không tạo membership.

**Ảnh hưởng và phần vẫn đạt:** Luồng HTTP đúng endpoint của kịch bản chưa thực hiện được. Không kết luận service DeNghi không tạo được lời mời; các ca sau dùng service thật để chuẩn bị fixture riêng.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\HocTap\Controllers\LopHocController.cs:15` — `[HttpGet]`.
- `C:\Khóa luận TN\Educonnect\Areas\HocTap\Controllers\LopHocController.cs:16` — `MoiThanhVien(Guid`.
- `C:\Khóa luận TN\Educonnect\Areas\HocTap\Controllers\LopHocController.cs:57` — `DeNghi(Guid`.
- `C:\Khóa luận TN\Educonnect\Areas\HocTap\Services\LopHocService.cs:62` — `Chỉ bên học chính được mời`.

**Đề xuất xử lý:** Thống nhất endpoint tạo lời mời trong thiết kế. Nếu thiết kế giữ POST MoiThanhVien, bổ sung action/chữ ký đúng để điều phối service. Ngoài ra code hiện chỉ cho bên học chính mời, còn TC ghi G mời: cần chốt chính sách actor; đổi route đơn thuần chưa giải quyết khác biệt đó.

**Assertion không đạt:**

- HTTP kỳ vọng 200, thực tế 405.
- Phản hồi không phải JSON thành công.
- Lời mời phải được tạo và chưa tự duyệt/xác nhận.

## Bước thực thi và bằng chứng từng ca

### IT-065 — Phụ huynh gửi liên kết

**Tiêu chuẩn:** Đúng đắn; **đích:** `POST /HocTap/LienKetTaiKhoan/GuiDeNghi`.
**Input riêng:** P gửi email H.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. P đăng nhập, POST GuiDeNghi với email H đang hoạt động.
5. Đọc liên kết: đúng cặp P/H, người gửi P, ChoXacNhan và đúng một dòng chưa kết thúc.
6. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
7. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Thành công khi đúng vai trò và trạng thái.; SQL ChoXacNhan; người gửi=P; một liên kết hoạt động.

**Quan sát:** HTTP 200, một liên kết ChoXacNhan đúng P/H; NguoiGuiId=P và chưa kết thúc.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 0,
    "Events": 0,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0,
    "Links": 0,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "after": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 1,
    "Events": 1,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0,
    "Links": 1,
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
    "message": "Đã gửi đề nghị liên kết.",
    "redirectUrl": "/HocTap/LienKetTaiKhoan"
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LienKetPhuHuynhHocSinh] WHERE Id=@Link; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[LopHoc] WHERE Id=@Class;
```

### IT-066 — Học sinh đồng ý liên kết

**Tiêu chuẩn:** Đúng đắn; **đích:** `POST /HocTap/LienKetTaiKhoan/PhanHoi`.
**Input riêng:** H, dongY=true, RV mới.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Fixture tạo đề nghị ChoXacNhan thật từ P. H POST PhanHoi dongY=true với RV mới.
5. Đối chiếu DaLienKet, XacNhanLuc hợp lệ và chưa kết thúc.
6. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
7. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Thành công khi đúng vai trò và trạng thái.; SQL DaLienKet; XacNhanLuc hợp lệ.

**Quan sát:** HTTP 200; liên kết DaLienKet, XacNhanLuc không trước GuiLuc, chưa có KetThucLuc.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 1,
    "Events": 1,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0,
    "Links": 1,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "after": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 2,
    "Events": 2,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0,
    "Links": 1,
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
    "message": "Đã liên kết tài khoản.",
    "redirectUrl": "/HocTap/LienKetTaiKhoan"
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LienKetPhuHuynhHocSinh] WHERE Id=@Link; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[LopHoc] WHERE Id=@Class;
```

### IT-067 — Học sinh từ chối liên kết

**Tiêu chuẩn:** Đúng đắn; **đích:** `POST /HocTap/LienKetTaiKhoan/PhanHoi`.
**Input riêng:** H, dongY=false, RV mới.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Fixture tạo đề nghị ChoXacNhan. H POST PhanHoi dongY=false với RV mới.
5. Đối chiếu TuChoi, NguoiKetThucId=H và KetThucLuc hợp lệ.
6. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
7. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Thành công khi đúng vai trò và trạng thái.; SQL TuChoi, KetThucLuc/người kết thúc hợp lệ.

**Quan sát:** HTTP 200; liên kết TuChoi, KetThucLuc hợp lệ và NguoiKetThucId=H.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 1,
    "Events": 1,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0,
    "Links": 1,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 0,
    "Submissions": 0
  },
  "after": {
    "Contacts": 0,
    "Requests": 0,
    "Slots": 0,
    "Rooms": 0,
    "Classes": 0,
    "Notices": 2,
    "Events": 2,
    "Members": 0,
    "Calendar": 0,
    "Lessons": 0,
    "Links": 1,
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
    "message": "Đã từ chối đề nghị.",
    "redirectUrl": "/HocTap/LienKetTaiKhoan"
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LienKetPhuHuynhHocSinh] WHERE Id=@Link; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[LopHoc] WHERE Id=@Class;
```

### IT-068 — Hủy liên kết

**Tiêu chuẩn:** Đúng đắn; **đích:** `POST /HocTap/LienKetTaiKhoan/Huy`.
**Input riêng:** Đúng bên của LK đã liên kết; RV mới.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Fixture liên kết P/H đã đồng ý, lớp do P đứng tên và membership H đã duyệt; giữ client H từ trước hủy và xác nhận GET lớp HTTP 200.
5. P POST Huy liên kết đúng RV. Sau commit dùng lại chính client/cookie H để GET lớp và POST gửi tin.
6. Đối chiếu DaHuy, membership DaRoiLop, hai request tiếp theo bị từ chối và không thêm tin.
7. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
8. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Thành công khi đúng vai trò và trạng thái.; SQL DaHuy; quyền phụ thuộc liên kết không được tiếp tục dùng.

**Quan sát:** HTTP 200; liên kết DaHuy đúng P. Membership H phụ thuộc liên kết chuyển DaRoiLop và có RoiLopLuc. Giữ cookie H: GET lớp và POST gửi tin tiếp theo đều HTTP 403, không lưu tin mới.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 10,
    "Events": 10,
    "Members": 3,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 1,
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
    "Notices": 12,
    "Events": 12,
    "Members": 3,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 1,
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
    "message": "Đã kết thúc liên kết.",
    "redirectUrl": "/HocTap/LienKetTaiKhoan"
  }
}
```

**Request kiểm tra tiếp bằng quyền/cookie hiện tại:**

```json
{
  "followUpAccess": {
    "status": 403,
    "location": null,
    "contentType": "text/html; charset=utf-8",
    "body": "<!doctype html><html lang='vi'><meta charset='utf-8'><meta name='viewport' content='width=device-width'><title>EduConnect</title><link rel='stylesheet' href='/css/app.css'><main class='error-page'><h1>Chưa thể hoàn tất</h1><p role='alert'>Bạn chưa c&#243; quyền hoặc li&#234;n kết ph&#249; hợp để v&#224;o lớp n&#224;y.</p><a class='btn' href='/'>Về trang chủ</a><button onclick='history.back()'>Quay lại biểu mẫu</button></main></html>"
  },
  "followUpWrite": {
    "status": 403,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": {
      "ok": false,
      "message": "Bạn chưa có quyền hoặc liên kết phù hợp để vào lớp này.",
      "errors": {
        "": [
          "Bạn chưa có quyền hoặc liên kết phù hợp để vào lớp này."
        ]
      }
    }
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LienKetPhuHuynhHocSinh] WHERE Id=@Link; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[LopHoc] WHERE Id=@Class;
```

### IT-069 — Mời thành viên

**Tiêu chuẩn:** Đúng đắn; **đích:** `POST /HocTap/LopHoc/MoiThanhVien`.
**Input riêng:** G đủ quyền mời; email H.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Fixture có lớp P đứng tên, H đã liên kết nhưng chưa là thành viên. G POST đúng route MoiThanhVien và email H theo kịch bản.
5. Không chuyển sang route DeNghi để tránh thay nội dung ca. Đối chiếu HTTP và việc tạo lời mời; thực tế dừng405 nên Fail.
6. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
7. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Thành công khi đúng vai trò và trạng thái.; SQL Một đề nghị/lời mời, không tự vượt bước xác nhận.

**Quan sát:** HTTP 405 Method Not Allowed tại POST MoiThanhVien; không tạo membership/lời mời. Code chỉ đăng ký GET MoiThanhVien, POST dùng DeNghi. Ca Fail, không thay route để đánh Pass.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
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
    "Links": 1,
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
    "Links": 1,
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
  "status": 405,
  "location": null,
  "contentType": null,
  "body": ""
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LienKetPhuHuynhHocSinh] WHERE Id=@Link; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[LopHoc] WHERE Id=@Class;
```

### IT-070 — Duyệt quyền xem

**Tiêu chuẩn:** Đúng đắn; **đích:** `POST /HocTap/LopHoc/Duyet`.
**Input riêng:** id=C1, member=H, permission=Xem; RV thật.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. P tạo lời mời bằng service thật và H đồng ý lời mời trong fixture; chưa gia sư duyệt. Tạo một bài tập hợp lệ gắn buổi đang diễn ra để kiểm nộp bài.
5. G POST Duyet với action=DaDuyet, permission=Xem, member=Id bản ghi membership của H và RV hiện tại.
6. Đọc đúng membership/LienKetId/flag, rồi H POST Nop vào bài tập tồn tại; yêu cầu403 và không có bài nộp.
7. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
8. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Thành công khi đúng vai trò và trạng thái.; SQL DaDuyet/Xem đúng người; không có quyền nộp bài.

**Quan sát:** HTTP 200; membership của H DaDuyet/Xem, đúng LienKetId, ChoPhepNopBai=false. H dùng bài tập hợp lệ thử POST Nop: HTTP 403; không có bài nộp.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 10,
    "Events": 10,
    "Members": 3,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 1,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 1,
    "Submissions": 0
  },
  "after": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 11,
    "Events": 11,
    "Members": 3,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 1,
    "ScheduleHistory": 0,
    "LessonHistory": 0,
    "Messages": 0,
    "Assignments": 1,
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
    "redirectUrl": "/HocTap/LopHoc/ThanhVien/6aa09fe9-8b49-4f37-a234-406c8b99b012"
  }
}
```

**Request kiểm tra tiếp bằng quyền/cookie hiện tại:**

```json
{
  "submissionAttempt": {
    "status": 403,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": {
      "ok": false,
      "message": "Kiểm tra lại thông tin đã nhập.",
      "errors": {
        "": [
          "Bạn không có quyền thực hiện thao tác này."
        ]
      }
    }
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LienKetPhuHuynhHocSinh] WHERE Id=@Link; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[LopHoc] WHERE Id=@Class;
```

### IT-071 — Cấp quyền nộp bài

**Tiêu chuẩn:** Đúng đắn; **đích:** `POST /HocTap/LopHoc/Duyet`.
**Input riêng:** H có LK hoạt động; permission=ChinhSua, choPhepNopBai=true.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Fixture lời mời do P tạo, H đã đồng ý và liên kết đang hoạt động.
5. G POST Duyet action=DaDuyet, permission=ChinhSua, choPhepNopBai=true với Id membership/RV thật.
6. Đối chiếu trạng thái, quyền, flag và LienKetId đúng.
7. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
8. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Thành công khi đúng vai trò và trạng thái.; SQL ChoPhepNopBai=1, đúng membership/liên kết.

**Quan sát:** HTTP 200; H DaDuyet/ChinhSua, ChoPhepNopBai=true và gắn đúng liên kết đang hoạt động.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 9,
    "Events": 9,
    "Members": 3,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 1,
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
    "Notices": 10,
    "Events": 10,
    "Members": 3,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 1,
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
    "redirectUrl": "/HocTap/LopHoc/ThanhVien/7775a17c-c51f-445a-a663-1637f96f3445"
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LienKetPhuHuynhHocSinh] WHERE Id=@Link; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[LopHoc] WHERE Id=@Class;
```

### IT-072 — Người ngoài truy cập lớp

**Tiêu chuẩn:** Xác thực; **đích:** `GET /HocTap/LopHoc/ChiTiet/@Class`.
**Input riêng:** X đăng nhập nhưng không membership.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Tạo lớp G/H bằng procedure thật; H2 đăng nhập nhưng không có membership.
5. H2 GET ChiTiet lớp, yêu cầu403/404 và toàn bộ nghiệp vụ giữ nguyên.
6. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
7. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP 403/404 theo hợp đồng che tài nguyên, không 200 dữ liệu lớp.; SQL Không đổi dữ liệu.

**Quan sát:** HTTP 403 cho H2 ngoài lớp; toàn bộ dữ liệu trước/sau giống nhau.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
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
    "Links": 1,
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
    "Links": 1,
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
  "status": 403,
  "location": null,
  "contentType": "text/html; charset=utf-8",
  "body": "<!doctype html><html lang='vi'><meta charset='utf-8'><meta name='viewport' content='width=device-width'><title>EduConnect</title><link rel='stylesheet' href='/css/app.css'><main class='error-page'><h1>Chưa thể hoàn tất</h1><p role='alert'>Bạn chưa c&#243; quyền hoặc li&#234;n kết ph&#249; hợp để v&#224;o lớp n&#224;y.</p><a class='btn' href='/'>Về trang chủ</a><button onclick='history.back()'>Quay lại biểu mẫu</button></main></html>"
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LienKetPhuHuynhHocSinh] WHERE Id=@Link; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[LopHoc] WHERE Id=@Class;
```

### IT-073 — Thu hồi quyền có hiệu lực request sau

**Tiêu chuẩn:** Xác thực; **đích:** `POST /HocTap/LopHoc/Duyet rồi gửi thao tác ghi bằng client H`.
**Input riêng:** Giữ cookie H; đổi membership thành không đủ quyền và commit.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. Fixture H có membership ChinhSua, đang liên kết. Tạo client H trước thay đổi và xác nhận GET lớp HTTP 200.
5. G POST Duyet hạ H xuống Xem, commit thành công.
6. Dùng lại cookie H cũ POST gửi tin vào đúng hội thoại TheoLop; phải403 và không thêm tin.
7. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
8. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Request H tiếp theo bị từ chối.; SQL Không ghi sau khi quyền bị thu hồi.

**Quan sát:** H truy cập lớp HTTP 200 trước đổi quyền. G HTTP Duyet hạ H từ ChinhSua xuống Xem thành công; cùng cookie H gửi tin tiếp theo nhận HTTP 403, không có tin mới.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
    "Contacts": 1,
    "Requests": 1,
    "Slots": 2,
    "Rooms": 2,
    "Classes": 1,
    "Notices": 10,
    "Events": 10,
    "Members": 3,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 1,
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
    "Notices": 11,
    "Events": 11,
    "Members": 3,
    "Calendar": 2,
    "Lessons": 2,
    "Links": 1,
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
    "redirectUrl": "/HocTap/LopHoc/ThanhVien/4d487b7a-7c8a-4819-a810-9ce6a18a4c70"
  }
}
```

**Request kiểm tra tiếp bằng quyền/cookie hiện tại:**

```json
{
  "followUpWrite": {
    "status": 403,
    "location": null,
    "contentType": "application/json; charset=utf-8",
    "body": {
      "ok": false,
      "message": "Bạn không có quyền nhắn tin trong lớp này.",
      "errors": {
        "": [
          "Bạn không có quyền nhắn tin trong lớp này."
        ]
      }
    }
  }
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LienKetPhuHuynhHocSinh] WHERE Id=@Link; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[LopHoc] WHERE Id=@Class;
```

### IT-074 — Liên kết gia đình không tự cấp quyền lớp

**Tiêu chuẩn:** Xác thực; **đích:** `GET /HocTap/LopHoc/ChiTiet/@Class`.
**Input riêng:** P/H liên kết nhưng actor không có quyền cụ thể ở lớp mục tiêu.

**Các bước:**

1. Xác nhận DB_NAME/@@SERVERNAME và tạo fixture có tag/ID sở hữu riêng. Toàn bộ tài khoản, liên kết, lớp và dữ liệu nền dùng service/EF/SQL thật.
2. Chụp baseline qua DbContext mới AsNoTracking, gồm liên kết, lớp, thành viên, lịch/buổi, lịch sử, notice/outbox và dữ liệu ghi liên quan.
3. Client riêng đăng nhập Identity/cookie/phiên SQL thật, AllowAutoRedirect=false; lấy antiforgery cùng client.
4. P/H có liên kết hoạt động; H là bên học chính của lớp nhưng P chưa có membership riêng.
5. P GET ChiTiet; kiểm bị từ chối, không tự tạo quyền/membership từ quan hệ gia đình.
6. Thu status/JSON/URL trước redirect; mở scope mới đọc lại từng cột/RowVersion và so dữ liệu thay đổi/không đổi.
7. Dừng host test và dọn chỉ ID fixture theo khóa ngoại; lưu snapshot, SQL errors và TRX.

**Kỳ vọng:** HTTP Không tự Allow vì quan hệ gia đình.; SQL Membership không tự được tạo từ việc chỉ GET.

**Quan sát:** P liên kết với H là bên học chính nhưng P không có membership riêng. GET lớp HTTP 403, không tự tạo membership hoặc đổi dữ liệu.

**Số dòng trước và sau** bao gồm dữ liệu chuẩn bị fixture, không đồng nhất tổng notice với số mới thêm:

```json
{
  "before": {
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
    "Links": 1,
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
    "Links": 1,
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
  "status": 403,
  "location": null,
  "contentType": "text/html; charset=utf-8",
  "body": "<!doctype html><html lang='vi'><meta charset='utf-8'><meta name='viewport' content='width=device-width'><title>EduConnect</title><link rel='stylesheet' href='/css/app.css'><main class='error-page'><h1>Chưa thể hoàn tất</h1><p role='alert'>Bạn chưa c&#243; quyền hoặc li&#234;n kết ph&#249; hợp để v&#224;o lớp n&#224;y.</p><a class='btn' href='/'>Về trang chủ</a><button onclick='history.back()'>Quay lại biểu mẫu</button></main></html>"
}
```

**Truy vấn trong kịch bản** thay placeholder bằng ID thực ghi trong KetQua.json:

```sql
SELECT * FROM dbo.[LienKetPhuHuynhHocSinh] WHERE Id=@Link; SELECT * FROM dbo.[ThanhVienLopHoc] WHERE LopHocId=@Class; SELECT * FROM dbo.[LopHoc] WHERE Id=@Class;
```

## Phạm vi cần hiểu đúng

- IT-069 chạy đúng POST MoiThanhVien như bảng; không chạy thay DeNghi rồi báo đạt. Quyền mời G so với bên học chính cũng đang khác giữa thiết kế và code; request405 chưa đi vào kiểm quyền này.
- Fixture của IT-068/070/071/073 có lớp do P đứng tên, liên kết P/H đang hoạt động. Lời mời được chuẩn bị bằng service thật do P thực hiện, H đồng ý, gia sư duyệt tùy ca. Chuẩn bị này không chứng minh endpoint IT-069 đã đạt.
- Tham số member trong Duyet là Id của bản ghi ThanhVienLopHoc của H, không phải TaiKhoanId H; mapping được lưu trong minh chứng.
- IT-070 có bài tập thật tồn tại và hợp lệ. POST nộp bài403 là kiểm quyền Xem, không dùng ID ngẫu nhiên để tạo404 giả. IT-071 chỉ xác nhận cấp flag và liên kết đúng, không kết luận mọi luồng nộp bài đã được kiểm.
- IT-068/073 giữ nguyên client và cookie H trước/sau thay đổi. Quyền được đọc lại từ DB cho request sau; không đăng nhập lại để làm sai mục tiêu ca.
- IT-074 P có liên kết với đúng H là bên học chính; chỉ quan hệ gia đình không tạo membership khi GET.

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
dotnet test tests/Integration0203/Integration0203.csproj --filter "Suite=IT06ThanhVienLienKet" --logger "trx;LogFileName=ThanhVien-LienKet-Integration.trx" --results-directory "TestResults/06-ThanhVien-LienKet-Integration"

dotnet test tests/Integration0203/Integration0203.csproj --filter "FullyQualifiedName~Integration0203Tests.IT_069"
```

Chạy tuần tự hai nhóm trên cùng DB. Nếu app đang khóa output build, dừng app hoặc dùng BaseOutputPath riêng và UseAppHost=false. Harness chỉ chấp nhận EduConnect_Verification_IT0203, không dùng DB ứng dụng.

## Lượt chạy thử và hồi quy

Lượt đầu chạy hai tiến trình nhóm06/07 song song làm bốn ca IT-072/077/081/086 deadlock1205 trong khởi tạo fixture, trước request cần kiểm. Các log/TRX thử được giữ tại work/it0607-first. Kết quả chính thức ở đây là chạy lại đầy đủ hai nhóm tuần tự, không đổi assertion hoặc code ứng dụng. Không kết luận SQL không có deadlock trong các tải đồng thời khác.

Hồi quy 49 ca nhóm02–05:43 Pass, 6 Fail; đối chiếu từng mã không đổi. Sáu Fail cũ IT-022/023/025/034/037/057 không tính vào22 ca mới và không ghi đè báo cáo nhóm cũ. TRX hồi quy tại work/it0607-regression/Regression02-05-After0607.trx.

Cả 22 ca chính thức cleanup thành công. Sau lượt chính và hồi quy, còn 0 tài khoản/lớp fixture theo truy vấn xác minh. Dữ liệu SQL trước/sau được lưu trước cleanup. Các nhật ký an toàn anonymous có thể còn trong DB kiểm thử riêng.

File KetQua.json có đầy đủ ID, RowVersion, input đã bỏ token, HTTP, snapshot từng cột, SQL errors và assertion. TRX có OBSERVATION_JSON tương ứng. XacMinh-SQL.txt lưu DB/hash định nghĩa procedure/trigger và kiểm cleanup; SHA256.json dùng đối chiếu tính toàn vẹn.
