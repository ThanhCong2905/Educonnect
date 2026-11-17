# Kết quả 13. Quản trị hệ thống

**18 ca: 11 Pass, 7 Fail, 0 Skip/Blocked.**

Trong các Fail có **2 ca kiểm hợp đồng thiết kế** (thiếu lớp/trường input), dừng trước khi chạy nhánh nghiệp vụ; các Fail còn lại có quan sát hành vi thật khác kỳ vọng. Không đồng nhất Fail hợp đồng với lỗi thuật toán đã chạy.

Nguồn: Unit Test.xlsx đã đối chiếu lại trực tiếp với workbook hiện tại, giữ nguyên mã/thứ tự và ba tiêu chuẩn xác thực, đúng đắn, đầy đủ của từng dòng.

## Phạm vi

Real service/filter/validator logic. Persistence, locks, server clock, Identity, notices or scanner replaced at boundaries. DesignContract cases stop at missing input/type; their business branches are not executed.

Riêng UT-304/305 dùng KetThucWorkerFixture cũ: H/G/X/C/R lần lượt prefix 1/2/3/4/5; GUID cụ thể nằm trong quan sát. Các ca khác dùng fixture mới theo từng Fact. Dữ liệu U/G/P/A/C/K/Room/M lần lượt có prefix GUID 1/2/3/4/5/6/7/8; RV1=[1,2,3,4,5,6,7,8]. T cố định cho cổng thời gian là 2026-11-14 10:00:00+07:00. Các chỗ code tự dùng UtcNow được ghi nhận đúng thời gian thực, không giả mạo bằng T.

Không gọi CSDL thật. Fake IQueryable chỉ cung cấp dữ liệu để chạy predicate C# thật; không chứng minh câu SQL sinh ra/constraint/lock/rollback. Mock Identity/files/notice là dependency; không mock service đang kiểm.

## Bảng kết quả

| Mã | Tình huống | Tiêu chuẩn | Kết quả | Quan sát |
| --- | --- | --- | --- | --- |
| UT-331 | Tạo báo cáo | Đúng đắn | Fail hành vi | status=200; success=true; message=null; saves=2; Add=ChoXuLy; noticeRecipients=['40000000-0000-0000-0000-000000000004']; Thiếu xác nhận gửi báo cáo cho người gửi. |
| UT-332 | Báo cáo trùng đang chờ | Đúng đắn | Fail hành vi | status=409; success=false; message="Bạn đã có báo cáo đang chờ xử lý cho nội dung này."; saves=0; Add=; noticeRecipients=[]; Kỳ vọng thông tin/mã báo cáo cũ để bổ sung; hiện trả 409 và chỉ có message. |
| UT-333 | Thiếu lý do | Đầy đủ | Pass | status=400; success=false; message="The LyDo field is required."; saves=0; Add=; noticeRecipients=[] |
| UT-334 | Khóa hợp lệ | Đúng đắn | Fail hành vi | status=200; message=null; state="BiKhoa"; saves=1; commits=1; Add=bản ghi; noticeRecipients=[]; Không thông báo trực tiếp cho người dùng bị khóa (fixture không có lớp/yêu cầu đang chờ). |
| UT-335 | Mở khóa hợp lệ | Đúng đắn | Pass | status=200; message=null; state="HoatDong"; saves=3; commits=1; Add=; noticeRecipients=['10000000-0000-0000-0000-000000000001'] |
| UT-336 | Không đổi trạng thái trùng | Đúng đắn | Fail hành vi | status=200; message=null; state="BiKhoa"; saves=1; commits=1; Add=bản ghi; noticeRecipients=[]; Tài khoản đã BiKhoa phải no-op; hiện thêm biện pháp và Save lại. |
| UT-337 | Thiếu lý do khóa | Đầy đủ | Pass | status=400; message="Lý do cần từ 1 đến 2000 ký tự."; state="HoatDong"; saves=0; commits=0; Add=; noticeRecipients=[] |
| UT-338 | Chấp thuận hồ sơ | Đúng đắn | Pass | status=200; message=null; saves=1; commits=1; Add=bản ghi; noticeRecipients=['20000000-0000-0000-0000-000000000002'] |
| UT-339 | Yêu cầu chỉnh sửa | Đúng đắn | Pass | status=200; message=null; saves=1; commits=1; Add=bản ghi; noticeRecipients=['20000000-0000-0000-0000-000000000002'] |
| UT-340 | Gửi lại sau chỉnh sửa | Đúng đắn | Pass | status=200; message=null; saves=3; commits=2; Add=; noticeRecipients=[] |
| UT-341 | Gán nhiều vai trò | Đúng đắn | Fail hợp đồng | inputProperties=["TaiKhoanId", "ConcurrencyStamp", "Quyen", "LyDo"]; Thiếu hợp đồng đổi vai trò ở phương thức đích; chưa thể truyền một/hai vai trò để kiểm QT-12. |
| UT-342 | Đổi quyền hợp lệ | Xác thực | Fail hợp đồng | inputProperties=["TaiKhoanId", "ConcurrencyStamp", "Quyen", "LyDo"]; Thiếu hợp đồng đổi vai trò ở phương thức đích; chưa thể truyền một/hai vai trò để kiểm QT-12. |
| UT-343 | Bộ lọc hợp lệ | Đúng đắn | Fail hành vi | saves=0; total=2; Thiếu lọc theo người thực hiện: không có tham số actor, kết quả có cả A và B khác người cùng thời gian. |
| UT-344 | Không kết quả | Đầy đủ | Pass | saves=0; total=0 |
| UT-345 | Vai trò không có quyền quản trị | Xác thực | Pass | saves=0; 18 datasets guard đều false/403 |
| UT-346 | Vai trò không có quyền quản trị | Xác thực | Pass | saves=0; 3 datasets guard đều false/403 |
| UT-347 | Vai trò không có quyền quản trị | Xác thực | Pass | saves=0; 3 datasets guard đều false/403 |
| UT-348 | Vai trò không có quyền quản trị | Xác thực | Pass | saves=0; 3 datasets guard đều false/403 |

## Chi tiết từng ca và bước thực hiện

### UT-331 — Tạo báo cáo

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `BaoCaoViPhamService.GuiAsync`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Đối tượng tồn tại; lý do/nội dung hợp lệ; tệp tùy chọn sạch.

**Các bước thực tế:**

1. Khởi tạo BaoCaoViPhamService thật; seed đối tượng tài khoản G tồn tại và admin A hoạt động có quyền XuLyBaoCao.
2. UT-331 input hợp lệ; UT-332 seed báo cáo cũ ChoXuLy cùng người/đối tượng nhưng dùng MaThaoTac mới; UT-333 để LyDo rỗng.
3. Gọi GuiAsync trực tiếp; thu KetQua, bản ghi thêm và mock notice cho từng người nhận.
4. Assert người/đối tượng/trạng thái, không tạo báo cáo trùng, thông tin báo cáo cũ, lỗi LyDo và Save/Commit đúng nhánh.

**Kỳ vọng giữ nguyên:** Lưu ChoXuLy đúng đối tượng/người gửi; thông báo admin và xác nhận người gửi.

**Quan sát:** status=200; success=true; message=null; saves=2; Add=ChoXuLy; noticeRecipients=['40000000-0000-0000-0000-000000000004']; Thiếu xác nhận gửi báo cáo cho người gửi.

**Nguyên nhân:** Sau khi lưu báo cáo, service chỉ query người có quyền XuLyBaoCao và gửi notice cho họ; không có notice xác nhận người gửi. Fixture có admin hoạt động nên admin nhận đúng, người gửi không nhận.

**Ảnh hưởng và giới hạn kết luận:** Báo cáo vẫn được lưu đúng và admin nhận thông báo; thiếu phần xác nhận qua cổng thông báo.

**Hướng xử lý:** Bổ sung xác nhận gửi báo cáo đúng người gửi/mã báo cáo theo hợp đồng; phân biệt notice với giá trị trả về của hàm.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\QuanTri\Services\BaoCaoViPhamService.cs:57` — `foreach (var recipient in recipients)`.

**Assertion không đạt:**

- Thiếu xác nhận gửi báo cáo cho người gửi.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-331",
  "mode": "Behavior",
  "source": "Areas/QuanTri/Services/BaoCaoViPhamService.cs",
  "success": true,
  "status": 200,
  "message": null,
  "returnedId": "dec87d93-c968-410f-9f65-feeebf50b449",
  "added": [
    {
      "Id": "dec87d93-c968-410f-9f65-feeebf50b449",
      "NguoiBiBaoCaoId": "20000000-0000-0000-0000-000000000002",
      "NguoiBaoCaoId": "10000000-0000-0000-0000-000000000001",
      "LoaiDoiTuong": "TaiKhoan",
      "DoiTuongId": "20000000-0000-0000-0000-000000000002",
      "LyDo": "Sai thông tin",
      "MoTa": "Thông tin cần xem xét lại.",
      "TrangThai": "ChoXuLy",
      "NguoiXuLyId": null,
      "KetLuan": null,
      "KetQua": null,
      "XuLyLuc": null,
      "GuiLuc": "2026-11-14T10:00:00+07:00",
      "MaThaoTac": "80000000-0000-0000-0000-000000000008",
      "BanChup": "{\"HoTen\":\"G\"}",
      "RowVersion": ""
    }
  ],
  "saves": 2,
  "notices": [
    {
      "User": "40000000-0000-0000-0000-000000000004",
      "Type": "BaoCao",
      "Target": "dec87d93-c968-410f-9f65-feeebf50b449",
      "Text": "Có báo cáo vi phạm mới cần xem xét."
    }
  ],
  "assertionFailures": [
    "Thiếu xác nhận gửi báo cáo cho người gửi."
  ]
}
```

### UT-332 — Báo cáo trùng đang chờ

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `BaoCaoViPhamService.GuiAsync`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. U1 đã có báo cáo cùng đối tượng/sự việc ChoXuLy.

**Các bước thực tế:**

1. Khởi tạo BaoCaoViPhamService thật; seed đối tượng tài khoản G tồn tại và admin A hoạt động có quyền XuLyBaoCao.
2. UT-331 input hợp lệ; UT-332 seed báo cáo cũ ChoXuLy cùng người/đối tượng nhưng dùng MaThaoTac mới; UT-333 để LyDo rỗng.
3. Gọi GuiAsync trực tiếp; thu KetQua, bản ghi thêm và mock notice cho từng người nhận.
4. Assert người/đối tượng/trạng thái, không tạo báo cáo trùng, thông tin báo cáo cũ, lỗi LyDo và Save/Commit đúng nhánh.

**Kỳ vọng giữ nguyên:** Không tạo báo cáo thứ hai; trả thông tin hỗ trợ bổ sung báo cáo cũ theo quyền.

**Quan sát:** status=409; success=false; message="Bạn đã có báo cáo đang chờ xử lý cho nội dung này."; saves=0; Add=; noticeRecipients=[]; Kỳ vọng thông tin/mã báo cáo cũ để bổ sung; hiện trả 409 và chỉ có message.

**Nguyên nhân:** Guard phát hiện báo cáo cùng người/đối tượng chưa xử lý trả 409 với message; không có mã/thông tin báo cáo cũ để hỗ trợ bổ sung. Input dùng mã thao tác mới để kiểm trùng nghiệp vụ, không phải retry cùng thao tác.

**Ảnh hưởng và giới hạn kết luận:** Không tạo báo cáo trùng đã đạt; thiếu hợp đồng hỗ trợ bổ sung báo cáo cũ.

**Hướng xử lý:** Trả kết quả có mã/thông tin báo cáo cũ theo quyền hoặc bổ sung API điều hướng/bổ sung được thiết kế rõ.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\QuanTri\Services\BaoCaoViPhamService.cs:37` — `Bạn đã có báo cáo đang chờ xử lý`.

**Assertion không đạt:**

- Kỳ vọng thông tin/mã báo cáo cũ để bổ sung; hiện trả 409 và chỉ có message.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-332",
  "mode": "Behavior",
  "source": "Areas/QuanTri/Services/BaoCaoViPhamService.cs",
  "success": false,
  "status": 409,
  "message": "Bạn đã có báo cáo đang chờ xử lý cho nội dung này.",
  "added": [],
  "saves": 0,
  "notices": [],
  "assertionFailures": [
    "Kỳ vọng thông tin/mã báo cáo cũ để bổ sung; hiện trả 409 và chỉ có message."
  ]
}
```

### UT-333 — Thiếu lý do

**Kết quả:** Passed; **Tiêu chuẩn:** Đầy đủ; **Đích:** `BaoCaoViPhamService validation`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Lý do chưa chọn; các trường khác hợp lệ.

**Các bước thực tế:**

1. Khởi tạo BaoCaoViPhamService thật; seed đối tượng tài khoản G tồn tại và admin A hoạt động có quyền XuLyBaoCao.
2. UT-331 input hợp lệ; UT-332 seed báo cáo cũ ChoXuLy cùng người/đối tượng nhưng dùng MaThaoTac mới; UT-333 để LyDo rỗng.
3. Gọi GuiAsync trực tiếp; thu KetQua, bản ghi thêm và mock notice cho từng người nhận.
4. Assert người/đối tượng/trạng thái, không tạo báo cáo trùng, thông tin báo cáo cũ, lỗi LyDo và Save/Commit đúng nhánh.

**Kỳ vọng giữ nguyên:** Lỗi trường lý do; không lưu.

**Quan sát:** status=400; success=false; message="The LyDo field is required."; saves=0; Add=; noticeRecipients=[]

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-333",
  "mode": "Behavior",
  "source": "Areas/QuanTri/Services/BaoCaoViPhamService.cs",
  "success": false,
  "status": 400,
  "message": "The LyDo field is required.",
  "added": [],
  "saves": 0,
  "notices": [],
  "validatorField": "LyDo",
  "assertionFailures": []
}
```

### UT-334 — Khóa hợp lệ

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `QuanTriService khóa tài khoản`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Admin có quyền; tài khoản đang hoạt động; lý do hợp lệ.

**Các bước thực tế:**

1. Tạo QuanTriService thật với current/adminrights/sessions/notice giả và nguồn dữ liệu async. Tài khoản U không có lớp/yêu cầu chờ trong fixture này.
2. UT-334 U đang HoatDong có phiên hoạt động; UT-335 U BiKhoa có biện pháp chưa gỡ và phiên đã thu hồi; UT-336 U đã BiKhoa; UT-337 lý do trắng.
3. Gọi KhoaTaiKhoanAsync hoặc GoBienPhapAsync thật, dùng ConcurrencyStamp hoặc RowVersion phù hợp.
4. Assert trạng thái, biện pháp/lý do, journal callback, notice đúng người, phiên bị thu hồi không phục hồi và số lần Save/Commit; ca no-op không được ghi thêm.

**Kỳ vọng giữ nguyên:** Tài khoản BiKhoa; lưu lý do và nhật ký; thông báo người dùng; yêu cầu thu hồi phiên.

**Quan sát:** status=200; message=null; state="BiKhoa"; saves=1; commits=1; Add=bản ghi; noticeRecipients=[]; Không thông báo trực tiếp cho người dùng bị khóa (fixture không có lớp/yêu cầu đang chờ).

**Nguyên nhân:** ApDungKhoaAsync thêm biện pháp, đổi trạng thái/stamp và thu hồi các phiên; notice hiện chỉ gửi đối tác yêu cầu hoặc lớp. Fixture tài khoản không có lớp/yêu cầu đang chờ, không có notice trực tiếp cho tài khoản bị khóa.

**Ảnh hưởng và giới hạn kết luận:** Khóa/thu hồi phiên/lưu lý do/nhật ký đều đạt; thiếu thông báo người dùng.

**Hướng xử lý:** Gửi notice kết quả khóa cho chính tài khoản bị khóa theo thiết kế, giữ nhật ký và thu hồi phiên hiện có.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\QuanTri\Services\QuanTriService.cs:76` — `internal async Task ApDungKhoaAsync`.

**Assertion không đạt:**

- Không thông báo trực tiếp cho người dùng bị khóa (fixture không có lớp/yêu cầu đang chờ).

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-334",
  "mode": "Behavior",
  "source": "Areas/QuanTri/Services/QuanTriService.cs",
  "status": 200,
  "message": null,
  "state": "BiKhoa",
  "sessionRevoked": "2026-11-14T10:00:00+07:00",
  "measure": null,
  "added": [
    {
      "Id": "29728dc7-5eed-4d3c-8940-cb8cec93f382",
      "NguonBienPhapId": null,
      "LoaiDoiTuong": "TaiKhoan",
      "DoiTuongId": "10000000-0000-0000-0000-000000000001",
      "Loai": "KhoaTaiKhoan",
      "LyDo": "Vi phạm đã xác minh",
      "BaoCaoId": null,
      "NguoiApDungId": "40000000-0000-0000-0000-000000000004",
      "ApDungLuc": "2026-11-14T10:00:00+07:00",
      "NguoiGoId": null,
      "GoLuc": null,
      "LyDoGo": null,
      "RowVersion": ""
    }
  ],
  "audit": [
    "KhoaTaiKhoan"
  ],
  "saves": 1,
  "commits": 1,
  "notices": [],
  "assertionFailures": [
    "Không thông báo trực tiếp cho người dùng bị khóa (fixture không có lớp/yêu cầu đang chờ)."
  ]
}
```

### UT-335 — Mở khóa hợp lệ

**Kết quả:** Passed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `QuanTriService mở khóa`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Admin có quyền; tài khoản BiKhoa.

**Các bước thực tế:**

1. Tạo QuanTriService thật với current/adminrights/sessions/notice giả và nguồn dữ liệu async. Tài khoản U không có lớp/yêu cầu chờ trong fixture này.
2. UT-334 U đang HoatDong có phiên hoạt động; UT-335 U BiKhoa có biện pháp chưa gỡ và phiên đã thu hồi; UT-336 U đã BiKhoa; UT-337 lý do trắng.
3. Gọi KhoaTaiKhoanAsync hoặc GoBienPhapAsync thật, dùng ConcurrencyStamp hoặc RowVersion phù hợp.
4. Assert trạng thái, biện pháp/lý do, journal callback, notice đúng người, phiên bị thu hồi không phục hồi và số lần Save/Commit; ca no-op không được ghi thêm.

**Kỳ vọng giữ nguyên:** Chuyển HoatDong; ghi nhật ký, thông báo; không tự phục hồi phiên đã bị thu hồi.

**Quan sát:** status=200; message=null; state="HoatDong"; saves=3; commits=1; Add=; noticeRecipients=['10000000-0000-0000-0000-000000000001']

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-335",
  "mode": "Behavior",
  "source": "Areas/QuanTri/Services/QuanTriService.cs",
  "status": 200,
  "message": null,
  "state": "HoatDong",
  "sessionRevoked": "2026-11-14T09:58:00+07:00",
  "measure": {
    "Id": "70000000-0000-0000-0000-000000000007",
    "NguonBienPhapId": null,
    "LoaiDoiTuong": "TaiKhoan",
    "DoiTuongId": "10000000-0000-0000-0000-000000000001",
    "Loai": "KhoaTaiKhoan",
    "LyDo": "Lý do cũ",
    "BaoCaoId": null,
    "NguoiApDungId": "40000000-0000-0000-0000-000000000004",
    "ApDungLuc": "2026-11-13T10:00:00+07:00",
    "NguoiGoId": "40000000-0000-0000-0000-000000000004",
    "GoLuc": "2026-11-14T10:00:00+07:00",
    "LyDoGo": "Đã xử lý",
    "RowVersion": "AQIDBAUGBwg="
  },
  "added": [],
  "audit": [
    "GoBienPhap"
  ],
  "saves": 3,
  "commits": 1,
  "notices": [
    {
      "User": "10000000-0000-0000-0000-000000000001",
      "Type": "TaiKhoan",
      "Target": "10000000-0000-0000-0000-000000000001",
      "Text": "Tài khoản đã được mở khóa. Bạn có thể đăng nhập lại."
    }
  ],
  "assertionFailures": []
}
```

### UT-336 — Không đổi trạng thái trùng

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `QuanTriService đổi trạng thái`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Tài khoản đã ở trạng thái được yêu cầu.

**Các bước thực tế:**

1. Tạo QuanTriService thật với current/adminrights/sessions/notice giả và nguồn dữ liệu async. Tài khoản U không có lớp/yêu cầu chờ trong fixture này.
2. UT-334 U đang HoatDong có phiên hoạt động; UT-335 U BiKhoa có biện pháp chưa gỡ và phiên đã thu hồi; UT-336 U đã BiKhoa; UT-337 lý do trắng.
3. Gọi KhoaTaiKhoanAsync hoặc GoBienPhapAsync thật, dùng ConcurrencyStamp hoặc RowVersion phù hợp.
4. Assert trạng thái, biện pháp/lý do, journal callback, notice đúng người, phiên bị thu hồi không phục hồi và số lần Save/Commit; ca no-op không được ghi thêm.

**Kỳ vọng giữ nguyên:** Không cập nhật lại; kết quả không cần thay đổi theo UC.

**Quan sát:** status=200; message=null; state="BiKhoa"; saves=1; commits=1; Add=bản ghi; noticeRecipients=[]; Tài khoản đã BiKhoa phải no-op; hiện thêm biện pháp và Save lại.

**Nguyên nhân:** KhoaTaiKhoanAsync/ApDungKhoaAsync không chặn trạng thái đã BiKhoa trước khi thêm biện pháp và đổi stamp. Lần yêu cầu trùng vẫn Save/Commit.

**Ảnh hưởng và giới hạn kết luận:** Có thể thêm biện pháp khóa dư và đổi dấu phiên bản dù trạng thái không đổi.

**Hướng xử lý:** Thêm quy tắc no-op theo trạng thái hiện tại; tránh thêm biện pháp và nhật ký thay đổi trùng nếu UC yêu cầu không đổi.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\QuanTri\Services\QuanTriService.cs:61` — `public async Task KhoaTaiKhoanAsync`.
- `C:\Khóa luận TN\Educonnect\Areas\QuanTri\Services\QuanTriService.cs:87` — `var measure = new BienPhapQuanTri`.

**Assertion không đạt:**

- Tài khoản đã BiKhoa phải no-op; hiện thêm biện pháp và Save lại.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-336",
  "mode": "Behavior",
  "source": "Areas/QuanTri/Services/QuanTriService.cs",
  "status": 200,
  "message": null,
  "state": "BiKhoa",
  "sessionRevoked": "2026-11-14T10:00:00+07:00",
  "measure": {
    "Id": "70000000-0000-0000-0000-000000000007",
    "NguonBienPhapId": null,
    "LoaiDoiTuong": "TaiKhoan",
    "DoiTuongId": "10000000-0000-0000-0000-000000000001",
    "Loai": "KhoaTaiKhoan",
    "LyDo": "Lý do cũ",
    "BaoCaoId": null,
    "NguoiApDungId": "40000000-0000-0000-0000-000000000004",
    "ApDungLuc": "2026-11-13T10:00:00+07:00",
    "NguoiGoId": null,
    "GoLuc": null,
    "LyDoGo": null,
    "RowVersion": "AQIDBAUGBwg="
  },
  "added": [
    {
      "Id": "4b74abb0-5365-40e1-bd3a-309c0fe73a7f",
      "NguonBienPhapId": null,
      "LoaiDoiTuong": "TaiKhoan",
      "DoiTuongId": "10000000-0000-0000-0000-000000000001",
      "Loai": "KhoaTaiKhoan",
      "LyDo": "Vi phạm đã xác minh",
      "BaoCaoId": null,
      "NguoiApDungId": "40000000-0000-0000-0000-000000000004",
      "ApDungLuc": "2026-11-14T10:00:00+07:00",
      "NguoiGoId": null,
      "GoLuc": null,
      "LyDoGo": null,
      "RowVersion": ""
    }
  ],
  "audit": [
    "KhoaTaiKhoan"
  ],
  "saves": 1,
  "commits": 1,
  "notices": [],
  "assertionFailures": [
    "Tài khoản đã BiKhoa phải no-op; hiện thêm biện pháp và Save lại."
  ]
}
```

### UT-337 — Thiếu lý do khóa

**Kết quả:** Passed; **Tiêu chuẩn:** Đầy đủ; **Đích:** `QuanTriService khóa tài khoản`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Admin hợp lệ; lý do trống.

**Các bước thực tế:**

1. Tạo QuanTriService thật với current/adminrights/sessions/notice giả và nguồn dữ liệu async. Tài khoản U không có lớp/yêu cầu chờ trong fixture này.
2. UT-334 U đang HoatDong có phiên hoạt động; UT-335 U BiKhoa có biện pháp chưa gỡ và phiên đã thu hồi; UT-336 U đã BiKhoa; UT-337 lý do trắng.
3. Gọi KhoaTaiKhoanAsync hoặc GoBienPhapAsync thật, dùng ConcurrencyStamp hoặc RowVersion phù hợp.
4. Assert trạng thái, biện pháp/lý do, journal callback, notice đúng người, phiên bị thu hồi không phục hồi và số lần Save/Commit; ca no-op không được ghi thêm.

**Kỳ vọng giữ nguyên:** Báo lỗi; tài khoản không đổi.

**Quan sát:** status=400; message="Lý do cần từ 1 đến 2000 ký tự."; state="HoatDong"; saves=0; commits=0; Add=; noticeRecipients=[]

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-337",
  "mode": "Behavior",
  "source": "Areas/QuanTri/Services/QuanTriService.cs",
  "status": 400,
  "message": "Lý do cần từ 1 đến 2000 ký tự.",
  "state": "HoatDong",
  "sessionRevoked": null,
  "measure": null,
  "added": [],
  "audit": [],
  "saves": 0,
  "commits": 0,
  "notices": [],
  "assertionFailures": []
}
```

### UT-338 — Chấp thuận hồ sơ

**Kết quả:** Passed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `HoSoGiaSuService.DuyetAsync`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Admin hợp lệ; hồ sơ chờ duyệt đủ thông tin.

**Các bước thực tế:**

1. Tạo hồ sơ đủ môn/phí/giới thiệu/chuyên môn, hidden, chờ duyệt; actor admin có claim DuyetHoSo. UT-340 bắt đầu CanChinhSua và actor chủ hồ sơ.
2. UT-338/339 gọi DuyetAsync thật với quyết định/lý do. UT-340 gọi CapNhatAsync rồi DoiTrangThaiAsync(GuiDuyet) để thực sự gửi lại.
3. Thu trạng thái/lý do, NhatKyQuanTri được Add, notice tới gia sư và số lần Save/Commit.
4. Assert DaDuyet/CanChinhSua/ChoDuyet đúng ca, hồ sơ chưa tự công khai; ca duyệt có nhật ký và thông báo đúng chủ.

**Kỳ vọng giữ nguyên:** DaDuyet; lưu nhật ký; gửi kết quả đúng gia sư.

**Quan sát:** status=200; message=null; saves=1; commits=1; Add=bản ghi; noticeRecipients=['20000000-0000-0000-0000-000000000002']

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-338",
  "mode": "Behavior",
  "source": "Areas/NhuCau/Services/HoSoGiaSuService.cs",
  "status": 200,
  "message": null,
  "profile": {
    "DangHienThi": false,
    "DayTrucTuyen": true,
    "DayTrucTiep": false,
    "GuiDuyetLuc": null,
    "DuyetLuc": "2026-11-14T08:03:05.2970621+00:00",
    "NguoiDuyetId": "40000000-0000-0000-0000-000000000004",
    "Id": "80000000-0000-0000-0000-000000000008",
    "GiaSuId": "20000000-0000-0000-0000-000000000002",
    "TrinhDo": "Dai hoc",
    "ChuyenMon": "Toan",
    "KinhNghiem": "3 nam",
    "MoTa": "Gioi thieu day du",
    "KhuVuc": "Ha Noi",
    "TrangThaiDuyet": "DaDuyet",
    "LyDoTuChoi": null,
    "RowVersion": "AQIDBAUGBwg="
  },
  "added": [
    {
      "Id": "eaf264db-13d0-4519-bbc8-5b11ac641ad1",
      "NguoiThucHienId": "40000000-0000-0000-0000-000000000004",
      "ThoiDiem": "2026-11-14T08:03:05.2972461+00:00",
      "HanhDong": "DuyetHoSo",
      "LoaiDoiTuong": "HoSoGiaSu",
      "DoiTuongId": "80000000-0000-0000-0000-000000000008",
      "KetQua": "DaDuyet",
      "ChiTiet": null,
      "MaLienKetYeuCau": "2f6f10fa-084b-420c-a327-0fa5c76afe31"
    }
  ],
  "notices": [
    {
      "User": "20000000-0000-0000-0000-000000000002",
      "Type": "HoSo",
      "Target": "80000000-0000-0000-0000-000000000008",
      "Text": "Hồ sơ đã được duyệt. Bạn có thể bật hiển thị."
    }
  ],
  "saves": 1,
  "commits": 1,
  "assertionFailures": []
}
```

### UT-339 — Yêu cầu chỉnh sửa

**Kết quả:** Passed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `HoSoGiaSuService.DuyetAsync`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Admin nhập nội dung cần bổ sung.

**Các bước thực tế:**

1. Tạo hồ sơ đủ môn/phí/giới thiệu/chuyên môn, hidden, chờ duyệt; actor admin có claim DuyetHoSo. UT-340 bắt đầu CanChinhSua và actor chủ hồ sơ.
2. UT-338/339 gọi DuyetAsync thật với quyết định/lý do. UT-340 gọi CapNhatAsync rồi DoiTrangThaiAsync(GuiDuyet) để thực sự gửi lại.
3. Thu trạng thái/lý do, NhatKyQuanTri được Add, notice tới gia sư và số lần Save/Commit.
4. Assert DaDuyet/CanChinhSua/ChoDuyet đúng ca, hồ sơ chưa tự công khai; ca duyệt có nhật ký và thông báo đúng chủ.

**Kỳ vọng giữ nguyên:** Hồ sơ sang Yêu cầu chỉnh sửa; lưu nội dung; thông báo đúng gia sư.

**Quan sát:** status=200; message=null; saves=1; commits=1; Add=bản ghi; noticeRecipients=['20000000-0000-0000-0000-000000000002']

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-339",
  "mode": "Behavior",
  "source": "Areas/NhuCau/Services/HoSoGiaSuService.cs",
  "status": 200,
  "message": null,
  "profile": {
    "DangHienThi": false,
    "DayTrucTuyen": true,
    "DayTrucTiep": false,
    "GuiDuyetLuc": null,
    "DuyetLuc": "2026-11-14T08:03:05.331253+00:00",
    "NguoiDuyetId": "40000000-0000-0000-0000-000000000004",
    "Id": "80000000-0000-0000-0000-000000000008",
    "GiaSuId": "20000000-0000-0000-0000-000000000002",
    "TrinhDo": "Dai hoc",
    "ChuyenMon": "Toan",
    "KinhNghiem": "3 nam",
    "MoTa": "Gioi thieu day du",
    "KhuVuc": "Ha Noi",
    "TrangThaiDuyet": "CanChinhSua",
    "LyDoTuChoi": "Bổ sung bằng cấp",
    "RowVersion": "AQIDBAUGBwg="
  },
  "added": [
    {
      "Id": "67e564e0-76cf-41f3-af6d-b14567ec0599",
      "NguoiThucHienId": "40000000-0000-0000-0000-000000000004",
      "ThoiDiem": "2026-11-14T08:03:05.3312596+00:00",
      "HanhDong": "DuyetHoSo",
      "LoaiDoiTuong": "HoSoGiaSu",
      "DoiTuongId": "80000000-0000-0000-0000-000000000008",
      "KetQua": "CanChinhSua",
      "ChiTiet": null,
      "MaLienKetYeuCau": "7cc4732b-a8fa-418d-8f6d-dac20e31ecff"
    }
  ],
  "notices": [
    {
      "User": "20000000-0000-0000-0000-000000000002",
      "Type": "HoSo",
      "Target": "80000000-0000-0000-0000-000000000008",
      "Text": "Hồ sơ cần chỉnh sửa. Xem lý do trong Hồ sơ của tôi."
    }
  ],
  "saves": 1,
  "commits": 1,
  "assertionFailures": []
}
```

### UT-340 — Gửi lại sau chỉnh sửa

**Kết quả:** Passed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `HoSoGiaSuService gửi lại hồ sơ`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Hồ sơ yêu cầu chỉnh sửa; chủ hồ sơ cập nhật hợp lệ.

**Các bước thực tế:**

1. Tạo hồ sơ đủ môn/phí/giới thiệu/chuyên môn, hidden, chờ duyệt; actor admin có claim DuyetHoSo. UT-340 bắt đầu CanChinhSua và actor chủ hồ sơ.
2. UT-338/339 gọi DuyetAsync thật với quyết định/lý do. UT-340 gọi CapNhatAsync rồi DoiTrangThaiAsync(GuiDuyet) để thực sự gửi lại.
3. Thu trạng thái/lý do, NhatKyQuanTri được Add, notice tới gia sư và số lần Save/Commit.
4. Assert DaDuyet/CanChinhSua/ChoDuyet đúng ca, hồ sơ chưa tự công khai; ca duyệt có nhật ký và thông báo đúng chủ.

**Kỳ vọng giữ nguyên:** Trở lại chờ kiểm duyệt; không tự DaDuyet.

**Quan sát:** status=200; message=null; saves=3; commits=2; Add=; noticeRecipients=[]

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-340",
  "mode": "Behavior",
  "source": "Areas/NhuCau/Services/HoSoGiaSuService.cs",
  "status": 200,
  "message": null,
  "profile": {
    "DangHienThi": false,
    "DayTrucTuyen": true,
    "DayTrucTiep": false,
    "GuiDuyetLuc": "2026-11-14T08:03:05.1957281+00:00",
    "DuyetLuc": null,
    "NguoiDuyetId": null,
    "Id": "80000000-0000-0000-0000-000000000008",
    "GiaSuId": "20000000-0000-0000-0000-000000000002",
    "TrinhDo": "Đại học",
    "ChuyenMon": "Toán",
    "KinhNghiem": "3 năm",
    "MoTa": "Giới thiệu đầy đủ cho hồ sơ",
    "KhuVuc": "Hà Nội",
    "TrangThaiDuyet": "ChoDuyet",
    "LyDoTuChoi": null,
    "RowVersion": "AQIDBAUGBwg="
  },
  "added": [],
  "notices": [],
  "saves": 3,
  "commits": 2,
  "workflow": "CapNhatAsync lưu BanNhap; bước gửi lại thật là DoiTrangThaiAsync(GuiDuyet), không kết luận chỉ lưu là đã gửi.",
  "assertionFailures": []
}
```

### UT-341 — Gán nhiều vai trò

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `QuanTriService.PhanQuyenAsync`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Admin chọn hai vai trò cho một tài khoản.

**Các bước thực tế:**

1. Nạp kiểu input thật của phương thức được chỉ định.
2. Dùng reflection liệt kê thuộc tính input, kiểm trường mã thao tác hoặc vai trò mà fixture yêu cầu.
3. Assert có thể biểu diễn input của ca theo hợp đồng; hiện thiếu nên Fail. Không gọi service với input sai ngữ nghĩa để tạo kết quả giả.

**Kỳ vọng giữ nguyên:** Từ chối tổ hợp sai QT-12; không thay quyền.

**Quan sát:** inputProperties=["TaiKhoanId", "ConcurrencyStamp", "Quyen", "LyDo"]; Thiếu hợp đồng đổi vai trò ở phương thức đích; chưa thể truyền một/hai vai trò để kiểm QT-12.

**Nguyên nhân:** PhanQuyenAsync nhận PhanQuyenQuanTriViewModel.Quyen với sáu tên quyền quản trị; input không có vai trò tài khoản. Kịch bản lại yêu cầu chọn một/hai vai trò. Unit kiểm hợp đồng input, không nhét HocSinh/GiaSu vào Quyen để tạo một kết quả từ chối giả.

**Ảnh hưởng và giới hạn kết luận:** Chưa thực thi nhánh đổi vai trò, lưu quyền mới hoặc thu hồi phiên của ca này.

**Hướng xử lý:** Bổ sung/định danh đúng use case đổi vai trò và input có vai trò, áp dụng QT-12; hoặc sửa mục tiêu kịch bản nếu thực sự chỉ cấp admin permissions sau khi thống nhất.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\QuanTri\Services\QuanTriService.CauHinh.cs:11` — `public async Task PhanQuyenAsync`.
- `C:\Khóa luận TN\Educonnect\Areas\QuanTri\ViewModels\QuanTriInputs.cs:34` — `public sealed class PhanQuyenQuanTriViewModel`.

**Assertion không đạt:**

- Thiếu hợp đồng đổi vai trò ở phương thức đích; chưa thể truyền một/hai vai trò để kiểm QT-12.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-341",
  "mode": "DesignContract",
  "source": "Areas/QuanTri/Services/QuanTriService.CauHinh.cs",
  "inputProperties": [
    "TaiKhoanId",
    "ConcurrencyStamp",
    "Quyen",
    "LyDo"
  ],
  "explanation": "PhanQuyenAsync nhận Quyen quản trị (DuyetHoSo, XuLyBaoCao…), không nhận vai trò tài khoản. Không truyền tên vai trò vào Quyen rồi coi bị từ chối là kiểm đúng QT-12.",
  "assertionFailures": [
    "Thiếu hợp đồng đổi vai trò ở phương thức đích; chưa thể truyền một/hai vai trò để kiểm QT-12."
  ]
}
```

### UT-342 — Đổi quyền hợp lệ

**Kết quả:** Failed; **Tiêu chuẩn:** Xác thực; **Đích:** `QuanTriService.PhanQuyenAsync`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Admin chọn một vai trò hợp lệ.

**Các bước thực tế:**

1. Nạp kiểu input thật của phương thức được chỉ định.
2. Dùng reflection liệt kê thuộc tính input, kiểm trường mã thao tác hoặc vai trò mà fixture yêu cầu.
3. Assert có thể biểu diễn input của ca theo hợp đồng; hiện thiếu nên Fail. Không gọi service với input sai ngữ nghĩa để tạo kết quả giả.

**Kỳ vọng giữ nguyên:** Lưu đúng quyền, nhật ký và yêu cầu thu hồi phiên liên quan; quyền mới dùng cho request tiếp theo.

**Quan sát:** inputProperties=["TaiKhoanId", "ConcurrencyStamp", "Quyen", "LyDo"]; Thiếu hợp đồng đổi vai trò ở phương thức đích; chưa thể truyền một/hai vai trò để kiểm QT-12.

**Nguyên nhân:** PhanQuyenAsync nhận PhanQuyenQuanTriViewModel.Quyen với sáu tên quyền quản trị; input không có vai trò tài khoản. Kịch bản lại yêu cầu chọn một/hai vai trò. Unit kiểm hợp đồng input, không nhét HocSinh/GiaSu vào Quyen để tạo một kết quả từ chối giả.

**Ảnh hưởng và giới hạn kết luận:** Chưa thực thi nhánh đổi vai trò, lưu quyền mới hoặc thu hồi phiên của ca này.

**Hướng xử lý:** Bổ sung/định danh đúng use case đổi vai trò và input có vai trò, áp dụng QT-12; hoặc sửa mục tiêu kịch bản nếu thực sự chỉ cấp admin permissions sau khi thống nhất.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\QuanTri\Services\QuanTriService.CauHinh.cs:11` — `public async Task PhanQuyenAsync`.
- `C:\Khóa luận TN\Educonnect\Areas\QuanTri\ViewModels\QuanTriInputs.cs:34` — `public sealed class PhanQuyenQuanTriViewModel`.

**Assertion không đạt:**

- Thiếu hợp đồng đổi vai trò ở phương thức đích; chưa thể truyền một/hai vai trò để kiểm QT-12.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-342",
  "mode": "DesignContract",
  "source": "Areas/QuanTri/Services/QuanTriService.CauHinh.cs",
  "inputProperties": [
    "TaiKhoanId",
    "ConcurrencyStamp",
    "Quyen",
    "LyDo"
  ],
  "explanation": "PhanQuyenAsync nhận Quyen quản trị (DuyetHoSo, XuLyBaoCao…), không nhận vai trò tài khoản. Không truyền tên vai trò vào Quyen rồi coi bị từ chối là kiểm đúng QT-12.",
  "assertionFailures": [
    "Thiếu hợp đồng đổi vai trò ở phương thức đích; chưa thể truyền một/hai vai trò để kiểm QT-12."
  ]
}
```

### UT-343 — Bộ lọc hợp lệ

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `QuanTriService truy vấn nhật ký`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Admin; dữ liệu A khớp thời gian/người, B không khớp.

**Các bước thực tế:**

1. Tạo QuanTriService thật với nguồn dữ liệu async; mock admin cho quyền XemNhatKy.
2. UT-343 seed A đúng thời gian/người, B cùng thời gian khác người, C ngoài thời gian; UT-344 nguồn nhật ký rỗng.
3. Gọi LayNhatKyAsync với tuNgay/denNgay; liệt kê tham số thật để phát hiện thiếu filter người thực hiện.
4. Assert danh sách kỳ vọng và không Add/Remove/Save. UT-343 ghi rõ thời gian đã lọc đúng nhưng không biểu diễn được người trong API.

**Kỳ vọng giữ nguyên:** Trả A; không gọi cổng cập nhật/xóa nhật ký.

**Quan sát:** saves=0; total=2; Thiếu lọc theo người thực hiện: không có tham số actor, kết quả có cả A và B khác người cùng thời gian.

**Nguyên nhân:** LayNhatKyAsync nhận trang/q/tuNgay/denNgay, không có người thực hiện. Hai bản ghi cùng thời gian khác người đều được trả về; bản ghi ngoài thời gian đã bị loại.

**Ảnh hưởng và giới hạn kết luận:** Bộ lọc thời gian/read-only đạt, bộ lọc theo người còn thiếu.

**Hướng xử lý:** Bổ sung filter mã người thực hiện và áp dụng trước phân trang/count; thêm điều kiện khớp cả thời gian và người.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\QuanTri\Services\QuanTriService.CauHinh.cs:36` — `public async Task<Page<NhatKyQuanTriRow>> LayNhatKyAsync`.

**Assertion không đạt:**

- Thiếu lọc theo người thực hiện: không có tham số actor, kết quả có cả A và B khác người cùng thời gian.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-343",
  "mode": "Behavior",
  "source": "Areas/QuanTri/Services/QuanTriService.CauHinh.cs",
  "items": [
    {
      "BanGhi": {
        "Id": "60000000-0000-0000-0000-000000000006",
        "NguoiThucHienId": "40000000-0000-0000-0000-000000000004",
        "ThoiDiem": "2026-11-14T10:00:00+07:00",
        "HanhDong": "Khoa",
        "LoaiDoiTuong": "",
        "DoiTuongId": "",
        "KetQua": "",
        "ChiTiet": null,
        "MaLienKetYeuCau": ""
      },
      "HoTen": "Admin"
    },
    {
      "BanGhi": {
        "Id": "80000000-0000-0000-0000-000000000008",
        "NguoiThucHienId": "10000000-0000-0000-0000-000000000001",
        "ThoiDiem": "2026-11-14T10:00:00+07:00",
        "HanhDong": "Khoa",
        "LoaiDoiTuong": "",
        "DoiTuongId": "",
        "KetQua": "",
        "ChiTiet": null,
        "MaLienKetYeuCau": ""
      },
      "HoTen": "U1"
    }
  ],
  "total": 2,
  "saves": 0,
  "parameters": [
    "trang",
    "q",
    "tuNgay",
    "denNgay"
  ],
  "assertionFailures": [
    "Thiếu lọc theo người thực hiện: không có tham số actor, kết quả có cả A và B khác người cùng thời gian."
  ]
}
```

### UT-344 — Không kết quả

**Kết quả:** Passed; **Tiêu chuẩn:** Đầy đủ; **Đích:** `QuanTriService truy vấn nhật ký`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Nguồn đọc trả rỗng.

**Các bước thực tế:**

1. Tạo QuanTriService thật với nguồn dữ liệu async; mock admin cho quyền XemNhatKy.
2. UT-343 seed A đúng thời gian/người, B cùng thời gian khác người, C ngoài thời gian; UT-344 nguồn nhật ký rỗng.
3. Gọi LayNhatKyAsync với tuNgay/denNgay; liệt kê tham số thật để phát hiện thiếu filter người thực hiện.
4. Assert danh sách kỳ vọng và không Add/Remove/Save. UT-343 ghi rõ thời gian đã lọc đúng nhưng không biểu diễn được người trong API.

**Kỳ vọng giữ nguyên:** Danh sách rỗng, không exception hoặc tạo dữ liệu mới.

**Quan sát:** saves=0; total=0

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-344",
  "mode": "Behavior",
  "source": "Areas/QuanTri/Services/QuanTriService.CauHinh.cs",
  "items": [],
  "total": 0,
  "saves": 0,
  "parameters": [
    "trang",
    "q",
    "tuNgay",
    "denNgay"
  ],
  "assertionFailures": []
}
```

### UT-345 — Vai trò không có quyền quản trị

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `QuyenQuanTri policy`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Lần lượt tạo principal thuộc Học sinh/Phụ huynh/Gia sư, không cấp quyền quản trị.

**Các bước thực tế:**

1. Lần lượt tạo actor HocSinh/PhuHuynh/GiaSu qua mock nguồn người dùng.
2. Khởi tạo QuyenQuanTriService thật, không provider EF: nếu vô tình đọc DB sẽ lỗi test.
3. Gọi CoQuyenAsync và KiemTraAsync cho quyền được ánh xạ từ nhãn policy; UT-345 kiểm cả sáu quyền quản trị, các ca khác kiểm quyền cụ thể.
4. Assert false/403 cho tất cả dataset; không gọi handler nghiệp vụ. Đây là unit guard, không phải kiểm middleware/policy pipeline.

**Kỳ vọng giữ nguyên:** Policy từ chối; handler nghiệp vụ không được phép ghi/đọc dữ liệu quản trị.

**Quan sát:** saves=0; 18 datasets guard đều false/403

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-345",
  "mode": "Behavior",
  "source": "Areas/QuanTri/Services/QuyenQuanTriService.cs",
  "mapping": "Các nhãn policy trong bảng ánh xạ guard CoQuyenAsync/KiemTraAsync của QuyenQuanTriService. Không khẳng định đã chạy ASP.NET authorization pipeline.",
  "datasets": [
    {
      "role": "HocSinh",
      "permission": "DuyetHoSo",
      "allowed": false,
      "status": 403
    },
    {
      "role": "HocSinh",
      "permission": "XuLyBaoCao",
      "allowed": false,
      "status": 403
    },
    {
      "role": "HocSinh",
      "permission": "QuanLyTaiKhoan",
      "allowed": false,
      "status": 403
    },
    {
      "role": "HocSinh",
      "permission": "QuanLyQuyen",
      "allowed": false,
      "status": 403
    },
    {
      "role": "HocSinh",
      "permission": "XemNhatKy",
      "allowed": false,
      "status": 403
    },
    {
      "role": "HocSinh",
      "permission": "QuanLyDanhMuc",
      "allowed": false,
      "status": 403
    },
    {
      "role": "PhuHuynh",
      "permission": "DuyetHoSo",
      "allowed": false,
      "status": 403
    },
    {
      "role": "PhuHuynh",
      "permission": "XuLyBaoCao",
      "allowed": false,
      "status": 403
    },
    {
      "role": "PhuHuynh",
      "permission": "QuanLyTaiKhoan",
      "allowed": false,
      "status": 403
    },
    {
      "role": "PhuHuynh",
      "permission": "QuanLyQuyen",
      "allowed": false,
      "status": 403
    },
    {
      "role": "PhuHuynh",
      "permission": "XemNhatKy",
      "allowed": false,
      "status": 403
    },
    {
      "role": "PhuHuynh",
      "permission": "QuanLyDanhMuc",
      "allowed": false,
      "status": 403
    },
    {
      "role": "GiaSu",
      "permission": "DuyetHoSo",
      "allowed": false,
      "status": 403
    },
    {
      "role": "GiaSu",
      "permission": "XuLyBaoCao",
      "allowed": false,
      "status": 403
    },
    {
      "role": "GiaSu",
      "permission": "QuanLyTaiKhoan",
      "allowed": false,
      "status": 403
    },
    {
      "role": "GiaSu",
      "permission": "QuanLyQuyen",
      "allowed": false,
      "status": 403
    },
    {
      "role": "GiaSu",
      "permission": "XemNhatKy",
      "allowed": false,
      "status": 403
    },
    {
      "role": "GiaSu",
      "permission": "QuanLyDanhMuc",
      "allowed": false,
      "status": 403
    }
  ],
  "saves": 0,
  "assertionFailures": []
}
```

### UT-346 — Vai trò không có quyền quản trị

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `QuyenDuyetHoSo policy`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Lần lượt tạo principal thuộc Học sinh/Phụ huynh/Gia sư, không cấp quyền quản trị.

**Các bước thực tế:**

1. Lần lượt tạo actor HocSinh/PhuHuynh/GiaSu qua mock nguồn người dùng.
2. Khởi tạo QuyenQuanTriService thật, không provider EF: nếu vô tình đọc DB sẽ lỗi test.
3. Gọi CoQuyenAsync và KiemTraAsync cho quyền được ánh xạ từ nhãn policy; UT-345 kiểm cả sáu quyền quản trị, các ca khác kiểm quyền cụ thể.
4. Assert false/403 cho tất cả dataset; không gọi handler nghiệp vụ. Đây là unit guard, không phải kiểm middleware/policy pipeline.

**Kỳ vọng giữ nguyên:** Policy từ chối; handler nghiệp vụ không được phép ghi/đọc dữ liệu quản trị.

**Quan sát:** saves=0; 3 datasets guard đều false/403

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-346",
  "mode": "Behavior",
  "source": "Areas/QuanTri/Services/QuyenQuanTriService.cs",
  "mapping": "Các nhãn policy trong bảng ánh xạ guard CoQuyenAsync/KiemTraAsync của QuyenQuanTriService. Không khẳng định đã chạy ASP.NET authorization pipeline.",
  "datasets": [
    {
      "role": "HocSinh",
      "permission": "DuyetHoSo",
      "allowed": false,
      "status": 403
    },
    {
      "role": "PhuHuynh",
      "permission": "DuyetHoSo",
      "allowed": false,
      "status": 403
    },
    {
      "role": "GiaSu",
      "permission": "DuyetHoSo",
      "allowed": false,
      "status": 403
    }
  ],
  "saves": 0,
  "assertionFailures": []
}
```

### UT-347 — Vai trò không có quyền quản trị

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `QuyenPhanQuyen policy`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Lần lượt tạo principal thuộc Học sinh/Phụ huynh/Gia sư, không cấp quyền quản trị.

**Các bước thực tế:**

1. Lần lượt tạo actor HocSinh/PhuHuynh/GiaSu qua mock nguồn người dùng.
2. Khởi tạo QuyenQuanTriService thật, không provider EF: nếu vô tình đọc DB sẽ lỗi test.
3. Gọi CoQuyenAsync và KiemTraAsync cho quyền được ánh xạ từ nhãn policy; UT-345 kiểm cả sáu quyền quản trị, các ca khác kiểm quyền cụ thể.
4. Assert false/403 cho tất cả dataset; không gọi handler nghiệp vụ. Đây là unit guard, không phải kiểm middleware/policy pipeline.

**Kỳ vọng giữ nguyên:** Policy từ chối; handler nghiệp vụ không được phép ghi/đọc dữ liệu quản trị.

**Quan sát:** saves=0; 3 datasets guard đều false/403

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-347",
  "mode": "Behavior",
  "source": "Areas/QuanTri/Services/QuyenQuanTriService.cs",
  "mapping": "Các nhãn policy trong bảng ánh xạ guard CoQuyenAsync/KiemTraAsync của QuyenQuanTriService. Không khẳng định đã chạy ASP.NET authorization pipeline.",
  "datasets": [
    {
      "role": "HocSinh",
      "permission": "QuanLyQuyen",
      "allowed": false,
      "status": 403
    },
    {
      "role": "PhuHuynh",
      "permission": "QuanLyQuyen",
      "allowed": false,
      "status": 403
    },
    {
      "role": "GiaSu",
      "permission": "QuanLyQuyen",
      "allowed": false,
      "status": 403
    }
  ],
  "saves": 0,
  "assertionFailures": []
}
```

### UT-348 — Vai trò không có quyền quản trị

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `QuyenXemNhatKy policy`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Lần lượt tạo principal thuộc Học sinh/Phụ huynh/Gia sư, không cấp quyền quản trị.

**Các bước thực tế:**

1. Lần lượt tạo actor HocSinh/PhuHuynh/GiaSu qua mock nguồn người dùng.
2. Khởi tạo QuyenQuanTriService thật, không provider EF: nếu vô tình đọc DB sẽ lỗi test.
3. Gọi CoQuyenAsync và KiemTraAsync cho quyền được ánh xạ từ nhãn policy; UT-345 kiểm cả sáu quyền quản trị, các ca khác kiểm quyền cụ thể.
4. Assert false/403 cho tất cả dataset; không gọi handler nghiệp vụ. Đây là unit guard, không phải kiểm middleware/policy pipeline.

**Kỳ vọng giữ nguyên:** Policy từ chối; handler nghiệp vụ không được phép ghi/đọc dữ liệu quản trị.

**Quan sát:** saves=0; 3 datasets guard đều false/403

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-348",
  "mode": "Behavior",
  "source": "Areas/QuanTri/Services/QuyenQuanTriService.cs",
  "mapping": "Các nhãn policy trong bảng ánh xạ guard CoQuyenAsync/KiemTraAsync của QuyenQuanTriService. Không khẳng định đã chạy ASP.NET authorization pipeline.",
  "datasets": [
    {
      "role": "HocSinh",
      "permission": "XemNhatKy",
      "allowed": false,
      "status": 403
    },
    {
      "role": "PhuHuynh",
      "permission": "XemNhatKy",
      "allowed": false,
      "status": 403
    },
    {
      "role": "GiaSu",
      "permission": "XemNhatKy",
      "allowed": false,
      "status": 403
    }
  ],
  "saves": 0,
  "assertionFailures": []
}
```

## Chạy lại trong VS Code

Mở thư mục `C:\Khóa luận TN\Educonnect`, Terminal PowerShell:

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter "Suite=Remaining13" --logger "trx;LogFileName=Group13-Unit.trx" --results-directory "TestResults/13-QuanTri-HeThong-Unit"

dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter "FullyQualifiedName~EduConnect.Remaining.UnitTests.RemainingUnitTest.UT_331"
```

Nếu ứng dụng đang chạy khóa file build, dừng ứng dụng trước khi chạy lại hoặc dùng `-p:UseAppHost=false` và `-p:BaseOutputPath=<thư mục build riêng>` như lần chạy này. Bộ test có Fail nên dotnet test trả exit code 1 là kết quả assertion; không được diễn giải thành lỗi kết nối SQL.

## File thêm/sửa và cách đọc

- `RemainingTest.cs`: 71 Fact với mã UT, Trait Suite=Remaining10…14 để lọc riêng nhóm; không dùng Skip cho ca thiếu thiết kế.
- `RemainingRunner.cs`: Arrange/Act/Assert từng nhánh; gom đầy đủ assertion lệch và ghi OBSERVATION_JSON trước khi test Fail.
- `RemainingFixture.cs`: dữ liệu mới mỗi ca, async query provider chạy LINQ C#, fake store/transaction và mock dependency. Không chứa công thức nghiệp vụ đếm chờ/timeout thay cho app.
- `IServiceDataStore.cs`: interface dữ liệu/clock/lock/Save và EfServiceDataStore mặc định gọi lại EF/static helpers cũ. Dùng cổng này để giữ service thật trong unit.
- `IQuyenQuanTriService` và `IPhienDangNhapService`: tách phụ thuộc phân quyền/thu hồi phiên. Program đăng ký về implementation thật; các constructor cũ vẫn dùng được.
- `ITepService`: thêm property ChoPhepTepChuaQuet với mặc định false; `TepService` nhận scanner tùy chọn, không truyền thì vẫn chạy ClamAV cũ.
- `MatKhauValidator`: nhận blocklist cục bộ tùy chọn; không truyền thì giữ danh sách cấu hình cũ.
- Các service DanhGia/TinNhan/BaoCao/QuanTri/HoSo/TaiKhoan: thay điểm truy cập dữ liệu bằng adapter mặc định và dependency interface; không đổi guard/kết quả nghiệp vụ để làm test Pass.
- `KetQua.md`: bảng và mô tả chi tiết; `KetQua.json`: dữ liệu máy đọc; `GroupNN-Unit.trx`: bằng chứng runner; `Ma_nguon_Unit.zip`: snapshot mã liên quan và diff các service; `SHA256.json`: đối chiếu toàn vẹn.

## Hồi quy và phần chưa thuộc lần chạy

Đã chạy lại toàn project unit: **328 ca = 183 Pass, 142 Fail, 3 Skip**. Đủ **257 ca cũ không đổi trạng thái** so với TRX sau nhóm 08. Ba Skip cũ UT-020/028/064 giữ nguyên trong báo cáo cũ, không tính vào 71 ca mới.

Nhóm 09 là phụ lục SQL/tích hợp (UT-258–300), không được chạy hoặc báo đạt dưới nhãn unit. Các thuật toán SQL tương ứng LoginAttempt/UserSession cũng chưa được thực thi bởi unit. Chưa chạy lại integration sau refactor dependency. Các kết quả nhóm trước không bị ghi đè.
