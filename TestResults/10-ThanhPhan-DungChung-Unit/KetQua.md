# Kết quả 10. Thành phần dùng chung

**17 ca: 10 Pass, 7 Fail, 0 Skip/Blocked.**

Trong các Fail có **5 ca kiểm hợp đồng thiết kế** (thiếu lớp/trường input), dừng trước khi chạy nhánh nghiệp vụ; các Fail còn lại có quan sát hành vi thật khác kỳ vọng. Không đồng nhất Fail hợp đồng với lỗi thuật toán đã chạy.

Nguồn: Unit Test.xlsx đã đối chiếu lại trực tiếp với workbook hiện tại, giữ nguyên mã/thứ tự và ba tiêu chuẩn xác thực, đúng đắn, đầy đủ của từng dòng.

## Phạm vi

Real service/filter/validator logic. Persistence, locks, server clock, Identity, notices or scanner replaced at boundaries. DesignContract cases stop at missing input/type; their business branches are not executed.

Riêng UT-304/305 dùng KetThucWorkerFixture cũ: H/G/X/C/R lần lượt prefix 1/2/3/4/5; GUID cụ thể nằm trong quan sát. Các ca khác dùng fixture mới theo từng Fact. Dữ liệu U/G/P/A/C/K/Room/M lần lượt có prefix GUID 1/2/3/4/5/6/7/8; RV1=[1,2,3,4,5,6,7,8]. T cố định cho cổng thời gian là 2026-11-14 10:00:00+07:00. Các chỗ code tự dùng UtcNow được ghi nhận đúng thời gian thực, không giả mạo bằng T.

Không gọi CSDL thật. Fake IQueryable chỉ cung cấp dữ liệu để chạy predicate C# thật; không chứng minh câu SQL sinh ra/constraint/lock/rollback. Mock Identity/files/notice là dependency; không mock service đang kiểm.

## Bảng kết quả

| Mã | Tình huống | Tiêu chuẩn | Kết quả | Quan sát |
| --- | --- | --- | --- | --- |
| UT-301 | Chuyển lỗi nghiệp vụ AJAX | Đúng đắn | Pass | status=409; handled=true; JSON={"ok": false, "message": "Lỗi nghiệp vụ mẫu", "errors": {"": ["Lỗi nghiệp vụ mẫu"]}} |
| UT-302 | Chuyển success redirect thành JSON | Đúng đắn | Pass | JSON={"ok": true, "message": "Đã gửi liên hệ.", "redirectUrl": "/KetNoi/LienHe"} |
| UT-303 | Chuyển lỗi không tìm thấy | Đúng đắn | Pass | status=404 |
| UT-304 | Payload tối thiểu đúng yêu cầu | Đúng đắn | Fail hành vi | commits=1; outbox=[{"type": "KetNoi", "payload": "{\"recipient\":\"20000000-0000-0000-0000-000000000002\",\"target\":\"40000000-0000-0000-0000-000000000004\"}", "time": "2026-11-14T08:03:02.9337561+00:00"}]; Sai LoaiSuKien: kỳ vọng YeuCauKetThucLop; Payload thiếu mã yêu cầu; không dùng mã lớp thay thế.; TaoLuc phải bằng đồng hồ T. |
| UT-305 | Payload phản hồi đúng yêu cầu | Đúng đắn | Fail hành vi | commits=1; outbox=[{"type": "KetNoi", "payload": "{\"recipient\":\"20000000-0000-0000-0000-000000000002\",\"target\":\"40000000-0000-0000-0000-000000000004\"}", "time": "2026-11-14T08:03:02.7209335+00:00"}]; Sai LoaiSuKien: kỳ vọng PhanHoiKetThucLop; Payload thiếu mã yêu cầu; không dùng mã lớp thay thế. |
| UT-306 | Thành viên chỉ Xem không được nhắn tin | Xác thực | Fail hợp đồng | requiredType="ClassAccessService"; found=[]; Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass. |
| UT-307 | Thành viên chỉ Xem không được nộp bài | Xác thực | Fail hợp đồng | requiredType="ClassAccessService"; found=[]; Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass. |
| UT-308 | Thành viên chỉ Xem không được gửi phản hồi | Xác thực | Fail hợp đồng | requiredType="ClassAccessService"; found=[]; Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass. |
| UT-309 | Quan hệ gia đình không tự cấp quyền | Xác thực | Fail hợp đồng | requiredType="ClassAccessService"; found=[]; Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass. |
| UT-310 | Lỗi đọc quyền không thành cho phép | Xác thực | Fail hợp đồng | requiredType="ClassAccessService"; found=[]; Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass. |
| UT-311 | Kích thước tệp 9999999 byte | Đầy đủ | Pass | status=200; message=null; fileCount=1; eachLength=9999999; returnedLengths=[9999999]; scannerCalls=1 |
| UT-312 | Kích thước tệp 10000000 byte | Đầy đủ | Pass | status=200; message=null; fileCount=1; eachLength=10000000; returnedLengths=[10000000]; scannerCalls=1 |
| UT-313 | Kích thước tệp 10000001 byte | Đầy đủ | Pass | status=400; message="Mỗi tệp phải có dữ liệu và không vượt quá 10 MB."; fileCount=1; eachLength=10000001; returnedLengths=null; scannerCalls=0 |
| UT-314 | Số lượng 5 tệp | Đầy đủ | Pass | status=200; message=null; fileCount=5; eachLength=1000000; returnedLengths=[1000000, 1000000, 1000000, 1000000, 1000000]; scannerCalls=5 |
| UT-315 | Số lượng 6 tệp | Đầy đủ | Pass | status=400; message="Chọn tối đa 5 tệp mỗi lần gửi."; fileCount=6; eachLength=1000000; returnedLengths=null; scannerCalls=0 |
| UT-316 | Tổng dung lượng tối đa | Đầy đủ | Pass | status=200; message=null; fileCount=5; eachLength=10000000; returnedLengths=[10000000, 10000000, 10000000, 10000000, 10000000]; scannerCalls=5 |
| UT-317 | Từ chối định dạng không hỗ trợ | Xác thực | Pass | status=400; message="Chỉ nhận PDF, JPG, PNG, DOCX."; fileCount=1; eachLength=1000000; returnedLengths=null; scannerCalls=0 |

## Chi tiết từng ca và bước thực hiện

### UT-301 — Chuyển lỗi nghiệp vụ AJAX

**Kết quả:** Passed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `UIResponseFilter.OnException(context)`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Context AJAX chứa LoiNghiepVu với message và status xác định.

**Các bước thực tế:**

1. Tạo DefaultHttpContext mới; UT-301/302 gán X-Requested-With=XMLHttpRequest; UT-303 response có MemoryStream và chưa bắt đầu.
2. Khởi tạo UiResponseFilter thật với TempData factory giả; riêng UT-303 dùng middleware thật và delegate ném LoiNghiepVu 404.
3. UT-301 gọi OnException với lỗi 409/message; UT-302 đặt TempData Success, RedirectResult tới /KetNoi/LienHe rồi gọi OnResultExecutionAsync; đếm next.
4. Đọc JsonResult/ExceptionHandled hoặc status/body; assert từng giá trị, URL và encoding nội dung lỗi. Không chạy MVC pipeline.

**Kỳ vọng giữ nguyên:** Response giữ status/message theo hợp đồng và xử lý exception; không trả success.

**Quan sát:** status=409; handled=true; JSON={"ok": false, "message": "Lỗi nghiệp vụ mẫu", "errors": {"": ["Lỗi nghiệp vụ mẫu"]}}

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-301",
  "mode": "Behavior",
  "source": "Infrastructure/UiResponseFilter.cs",
  "mapping": "UIResponseFilter trong bảng là UiResponseFilter trong code (khác viết hoa).",
  "status": 409,
  "json": {
    "ok": false,
    "message": "Lỗi nghiệp vụ mẫu",
    "errors": {
      "": [
        "Lỗi nghiệp vụ mẫu"
      ]
    }
  },
  "handled": true,
  "assertionFailures": []
}
```

### UT-302 — Chuyển success redirect thành JSON

**Kết quả:** Passed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `UIResponseFilter.OnResultExecutionAsync(...)`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Action result Success(text)+Redirect LienHe; request AJAX; next delegate giả.

**Các bước thực tế:**

1. Tạo DefaultHttpContext mới; UT-301/302 gán X-Requested-With=XMLHttpRequest; UT-303 response có MemoryStream và chưa bắt đầu.
2. Khởi tạo UiResponseFilter thật với TempData factory giả; riêng UT-303 dùng middleware thật và delegate ném LoiNghiepVu 404.
3. UT-301 gọi OnException với lỗi 409/message; UT-302 đặt TempData Success, RedirectResult tới /KetNoi/LienHe rồi gọi OnResultExecutionAsync; đếm next.
4. Đọc JsonResult/ExceptionHandled hoặc status/body; assert từng giá trị, URL và encoding nội dung lỗi. Không chạy MVC pipeline.

**Kỳ vọng giữ nguyên:** Theo sơ đồ: JSON có ok, message, URL đúng; không mất URL chuyển hướng.

**Quan sát:** JSON={"ok": true, "message": "Đã gửi liên hệ.", "redirectUrl": "/KetNoi/LienHe"}

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-302",
  "mode": "Behavior",
  "source": "Infrastructure/UiResponseFilter.cs",
  "mapping": "UIResponseFilter trong bảng là UiResponseFilter trong code (khác viết hoa).",
  "json": {
    "ok": true,
    "message": "Đã gửi liên hệ.",
    "redirectUrl": "/KetNoi/LienHe"
  },
  "nextCalls": 1,
  "assertionFailures": []
}
```

### UT-303 — Chuyển lỗi không tìm thấy

**Kết quả:** Passed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `LoiNghiepVuMiddleware.InvokeAsync(context)`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Next delegate ném LoiNghiepVu(message,404); response chưa bắt đầu.

**Các bước thực tế:**

1. Tạo DefaultHttpContext mới; UT-301/302 gán X-Requested-With=XMLHttpRequest; UT-303 response có MemoryStream và chưa bắt đầu.
2. Khởi tạo UiResponseFilter thật với TempData factory giả; riêng UT-303 dùng middleware thật và delegate ném LoiNghiepVu 404.
3. UT-301 gọi OnException với lỗi 409/message; UT-302 đặt TempData Success, RedirectResult tới /KetNoi/LienHe rồi gọi OnResultExecutionAsync; đếm next.
4. Đọc JsonResult/ExceptionHandled hoặc status/body; assert từng giá trị, URL và encoding nội dung lỗi. Không chạy MVC pipeline.

**Kỳ vọng giữ nguyên:** HTTP status 404; phản hồi lỗi phù hợp; không đổi thành 200.

**Quan sát:** status=404

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-303",
  "mode": "Behavior",
  "source": "Infrastructure/LoiNghiepVuMiddleware.cs",
  "mapping": "UIResponseFilter trong bảng là UiResponseFilter trong code (khác viết hoa).",
  "status": 404,
  "body": "<!doctype html><html lang='vi'><meta charset='utf-8'><meta name='viewport' content='width=device-width'><title>EduConnect</title><link rel='stylesheet' href='/css/app.css'><main class='error-page'><h1>Chưa thể hoàn tất</h1><p role='alert'>Kh&#244;ng thấy &lt;mẫu&gt;</p><a class='btn' href='/'>Về trang chủ</a><button onclick='history.back()'>Quay lại biểu mẫu</button></main></html>",
  "assertionFailures": []
}
```

### UT-304 — Payload tối thiểu đúng yêu cầu

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `Thông điệp outbox YeuCauKetThucLop`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Qua thao tác tạo yêu cầu thành công với yc.Id=Y1; thu sự kiện gửi lưu.

**Các bước thực tế:**

1. Tạo KetThucWorkerFixture mới: service KetNoi, notice service thật; current user, transaction, store và nguồn thời gian giả.
2. UT-304 gọi TaoYeuCauHuyAsync; UT-305 khởi tạo LopHocService với KetNoiService thật và gọi XacNhanKetThucAsync để đi qua wrapper.
3. Xác nhận thao tác chính thành công/commit và thu đúng một SuKienChoXuLy từ IThongBaoData.
4. Đối chiếu LoaiSuKien, mã yêu cầu trong payload, ChoXuLy; UT-304 kiểm TaoLuc=T. Ghi đồng thời mọi sai lệch.

**Kỳ vọng giữ nguyên:** LoaiSuKien=YeuCauKetThucLop, payload có Id=Y1, trạng thái ChoXuLy, TaoLuc=T; không dùng mã lớp thay mã yêu cầu.

**Quan sát:** commits=1; outbox=[{"type": "KetNoi", "payload": "{\"recipient\":\"20000000-0000-0000-0000-000000000002\",\"target\":\"40000000-0000-0000-0000-000000000004\"}", "time": "2026-11-14T08:03:02.9337561+00:00"}]; Sai LoaiSuKien: kỳ vọng YeuCauKetThucLop; Payload thiếu mã yêu cầu; không dùng mã lớp thay thế.; TaoLuc phải bằng đồng hồ T.

**Nguyên nhân:** Service kết thúc lớp gọi notice loại KetNoi với target là mã lớp; ThongBaoService.Tao dùng type/target đó để tạo outbox chỉ gồm recipient và target. TaoLuc dùng DateTimeOffset.UtcNow, không phải đồng hồ T của cổng kết thúc.

**Ảnh hưởng và giới hạn kết luận:** Sai hợp đồng consumer theo thiết kế; unit không chạy consumer hoặc gửi thông báo bên ngoài.

**Hướng xử lý:** Bổ sung hợp đồng outbox riêng cho tạo yêu cầu/phản hồi, có mã yêu cầu và nguồn thời gian thống nhất; giữ thông báo hiển thị nếu vẫn cần loại KetNoi.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\KetNoi\Services\KetNoiService.KetThuc.cs:44` — `Có yêu cầu kết thúc lớp`.
- `C:\Khóa luận TN\Educonnect\Infrastructure\ThongBaoService.cs:25` — `persistence.AddEvent(new SuKienChoXuLy { Id = ev`.

**Assertion không đạt:**

- Sai LoaiSuKien: kỳ vọng YeuCauKetThucLop
- Payload thiếu mã yêu cầu; không dùng mã lớp thay thế.
- TaoLuc phải bằng đồng hồ T.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-304",
  "mode": "Behavior",
  "source": "Areas/KetNoi/Services/KetNoiService.KetThuc.cs",
  "resultSuccess": true,
  "requestId": "8913ee75-9964-4c3a-84a1-06c424bddabd",
  "classId": "40000000-0000-0000-0000-000000000004",
  "events": [
    {
      "Id": "085c0e66-63a1-4cb0-9f35-0d81cbcabdb8",
      "LoaiSuKien": "KetNoi",
      "DuLieuToiThieu": "{\"recipient\":\"20000000-0000-0000-0000-000000000002\",\"target\":\"40000000-0000-0000-0000-000000000004\"}",
      "TaoLuc": "2026-11-14T08:03:02.9337561+00:00",
      "TrangThai": "ChoXuLy",
      "SoLanThu": 0,
      "ThuLaiLuc": null,
      "XuLyXongLuc": null,
      "RowVersion": "",
      "LeaseId": null,
      "LeaseHetHanLuc": null
    }
  ],
  "commits": 1,
  "assertionFailures": [
    "Sai LoaiSuKien: kỳ vọng YeuCauKetThucLop",
    "Payload thiếu mã yêu cầu; không dùng mã lớp thay thế.",
    "TaoLuc phải bằng đồng hồ T."
  ]
}
```

### UT-305 — Payload phản hồi đúng yêu cầu

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `Thông điệp outbox PhanHoiKetThucLop`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Qua PhanHoiHuyAsync thành công Y1; thu sự kiện gửi lưu.

**Các bước thực tế:**

1. Tạo KetThucWorkerFixture mới: service KetNoi, notice service thật; current user, transaction, store và nguồn thời gian giả.
2. UT-304 gọi TaoYeuCauHuyAsync; UT-305 khởi tạo LopHocService với KetNoiService thật và gọi XacNhanKetThucAsync để đi qua wrapper.
3. Xác nhận thao tác chính thành công/commit và thu đúng một SuKienChoXuLy từ IThongBaoData.
4. Đối chiếu LoaiSuKien, mã yêu cầu trong payload, ChoXuLy; UT-304 kiểm TaoLuc=T. Ghi đồng thời mọi sai lệch.

**Kỳ vọng giữ nguyên:** LoaiSuKien=PhanHoiKetThucLop, payload có Id=Y1, trạng thái ChoXuLy; không tạo hai sự kiện do đi qua wrapper.

**Quan sát:** commits=1; outbox=[{"type": "KetNoi", "payload": "{\"recipient\":\"20000000-0000-0000-0000-000000000002\",\"target\":\"40000000-0000-0000-0000-000000000004\"}", "time": "2026-11-14T08:03:02.7209335+00:00"}]; Sai LoaiSuKien: kỳ vọng PhanHoiKetThucLop; Payload thiếu mã yêu cầu; không dùng mã lớp thay thế.

**Nguyên nhân:** Service kết thúc lớp gọi notice loại KetNoi với target là mã lớp; ThongBaoService.Tao dùng type/target đó để tạo outbox chỉ gồm recipient và target. Qua wrapper thật chỉ có một sự kiện; phần chống tạo hai sự kiện đạt.

**Ảnh hưởng và giới hạn kết luận:** Sai hợp đồng consumer theo thiết kế; unit không chạy consumer hoặc gửi thông báo bên ngoài.

**Hướng xử lý:** Bổ sung hợp đồng outbox riêng cho tạo yêu cầu/phản hồi, có mã yêu cầu và nguồn thời gian thống nhất; giữ thông báo hiển thị nếu vẫn cần loại KetNoi.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\KetNoi\Services\KetNoiService.KetThuc.cs:50` — `PhanHoiHuyAsync`.
- `C:\Khóa luận TN\Educonnect\Infrastructure\ThongBaoService.cs:25` — `persistence.AddEvent(new SuKienChoXuLy { Id = ev`.

**Assertion không đạt:**

- Sai LoaiSuKien: kỳ vọng PhanHoiKetThucLop
- Payload thiếu mã yêu cầu; không dùng mã lớp thay thế.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-305",
  "mode": "Behavior",
  "source": "Areas/KetNoi/Services/KetNoiService.KetThuc.cs",
  "resultSuccess": true,
  "requestId": "50000000-0000-0000-0000-000000000005",
  "classId": "40000000-0000-0000-0000-000000000004",
  "events": [
    {
      "Id": "706a58c3-eba9-4247-8029-760d455980c4",
      "LoaiSuKien": "KetNoi",
      "DuLieuToiThieu": "{\"recipient\":\"20000000-0000-0000-0000-000000000002\",\"target\":\"40000000-0000-0000-0000-000000000004\"}",
      "TaoLuc": "2026-11-14T08:03:02.7209335+00:00",
      "TrangThai": "ChoXuLy",
      "SoLanThu": 0,
      "ThuLaiLuc": null,
      "XuLyXongLuc": null,
      "RowVersion": "",
      "LeaseId": null,
      "LeaseHetHanLuc": null
    }
  ],
  "commits": 1,
  "assertionFailures": [
    "Sai LoaiSuKien: kỳ vọng PhanHoiKetThucLop",
    "Payload thiếu mã yêu cầu; không dùng mã lớp thay thế."
  ]
}
```

### UT-306 — Thành viên chỉ Xem không được nhắn tin

**Kết quả:** Failed; **Tiêu chuẩn:** Xác thực; **Đích:** `ClassAccessService`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Thành viên đã duyệt nhưng mức Xem; lớp hoạt động; thao tác=nhắn tin.

**Các bước thực tế:**

1. Nạp assembly ứng dụng từ project reference của test.
2. Tra đúng tên lớp yêu cầu trong bảng, ghi danh sách lớp khớp (hiện rỗng).
3. Đối chiếu vị trí triển khai liên quan hiện có; không gọi thay một lớp khác để kết luận hành vi.
4. Assert đơn vị thiết kế tồn tại; Fail tại bước này. Nhánh nghiệp vụ riêng chưa được chạy.

**Kỳ vọng giữ nguyên:** Deny; không gọi cổng ghi dữ liệu của thao tác.

**Quan sát:** requiredType="ClassAccessService"; found=[]; Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.

**Nguyên nhân:** Assembly ứng dụng không có lớp ClassAccessService được chỉ định trong bảng. Code có QuyenLopHocService nhưng chưa có ánh xạ thiết kế được xác nhận. Đây là Fail hợp đồng thiết kế; nhánh phân quyền cụ thể của ca chưa được chạy.

**Ảnh hưởng và giới hạn kết luận:** Không được suy ra hệ thống đang cho phép sai quyền chỉ từ thiếu tên lớp.

**Hướng xử lý:** Thống nhất ánh xạ với QuyenLopHocService hoặc triển khai lớp/adapter trong thiết kế, sau đó chạy lại nhánh đúng dữ liệu của từng ca. Không tự thêm lớp giả trong project test để báo Pass.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\QuyenLopHocService.cs:6` — `public sealed class`.

**Assertion không đạt:**

- Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-306",
  "mode": "DesignContract",
  "requiredType": "ClassAccessService",
  "found": [],
  "source": "Infrastructure/QuyenLopHocService.cs",
  "explanation": "Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.",
  "assertionFailures": [
    "Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass."
  ]
}
```

### UT-307 — Thành viên chỉ Xem không được nộp bài

**Kết quả:** Failed; **Tiêu chuẩn:** Xác thực; **Đích:** `ClassAccessService`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Thành viên đã duyệt nhưng mức Xem; lớp hoạt động; thao tác=nộp bài.

**Các bước thực tế:**

1. Nạp assembly ứng dụng từ project reference của test.
2. Tra đúng tên lớp yêu cầu trong bảng, ghi danh sách lớp khớp (hiện rỗng).
3. Đối chiếu vị trí triển khai liên quan hiện có; không gọi thay một lớp khác để kết luận hành vi.
4. Assert đơn vị thiết kế tồn tại; Fail tại bước này. Nhánh nghiệp vụ riêng chưa được chạy.

**Kỳ vọng giữ nguyên:** Deny; không gọi cổng ghi dữ liệu của thao tác.

**Quan sát:** requiredType="ClassAccessService"; found=[]; Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.

**Nguyên nhân:** Assembly ứng dụng không có lớp ClassAccessService được chỉ định trong bảng. Code có QuyenLopHocService nhưng chưa có ánh xạ thiết kế được xác nhận. Đây là Fail hợp đồng thiết kế; nhánh phân quyền cụ thể của ca chưa được chạy.

**Ảnh hưởng và giới hạn kết luận:** Không được suy ra hệ thống đang cho phép sai quyền chỉ từ thiếu tên lớp.

**Hướng xử lý:** Thống nhất ánh xạ với QuyenLopHocService hoặc triển khai lớp/adapter trong thiết kế, sau đó chạy lại nhánh đúng dữ liệu của từng ca. Không tự thêm lớp giả trong project test để báo Pass.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\QuyenLopHocService.cs:6` — `public sealed class`.

**Assertion không đạt:**

- Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-307",
  "mode": "DesignContract",
  "requiredType": "ClassAccessService",
  "found": [],
  "source": "Infrastructure/QuyenLopHocService.cs",
  "explanation": "Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.",
  "assertionFailures": [
    "Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass."
  ]
}
```

### UT-308 — Thành viên chỉ Xem không được gửi phản hồi

**Kết quả:** Failed; **Tiêu chuẩn:** Xác thực; **Đích:** `ClassAccessService`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Thành viên đã duyệt nhưng mức Xem; lớp hoạt động; thao tác=gửi phản hồi.

**Các bước thực tế:**

1. Nạp assembly ứng dụng từ project reference của test.
2. Tra đúng tên lớp yêu cầu trong bảng, ghi danh sách lớp khớp (hiện rỗng).
3. Đối chiếu vị trí triển khai liên quan hiện có; không gọi thay một lớp khác để kết luận hành vi.
4. Assert đơn vị thiết kế tồn tại; Fail tại bước này. Nhánh nghiệp vụ riêng chưa được chạy.

**Kỳ vọng giữ nguyên:** Deny; không gọi cổng ghi dữ liệu của thao tác.

**Quan sát:** requiredType="ClassAccessService"; found=[]; Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.

**Nguyên nhân:** Assembly ứng dụng không có lớp ClassAccessService được chỉ định trong bảng. Code có QuyenLopHocService nhưng chưa có ánh xạ thiết kế được xác nhận. Đây là Fail hợp đồng thiết kế; nhánh phân quyền cụ thể của ca chưa được chạy.

**Ảnh hưởng và giới hạn kết luận:** Không được suy ra hệ thống đang cho phép sai quyền chỉ từ thiếu tên lớp.

**Hướng xử lý:** Thống nhất ánh xạ với QuyenLopHocService hoặc triển khai lớp/adapter trong thiết kế, sau đó chạy lại nhánh đúng dữ liệu của từng ca. Không tự thêm lớp giả trong project test để báo Pass.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\QuyenLopHocService.cs:6` — `public sealed class`.

**Assertion không đạt:**

- Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-308",
  "mode": "DesignContract",
  "requiredType": "ClassAccessService",
  "found": [],
  "source": "Infrastructure/QuyenLopHocService.cs",
  "explanation": "Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.",
  "assertionFailures": [
    "Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass."
  ]
}
```

### UT-309 — Quan hệ gia đình không tự cấp quyền

**Kết quả:** Failed; **Tiêu chuẩn:** Xác thực; **Đích:** `ClassAccessService`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. P là phụ huynh của H nhưng P chưa được cấp quyền lớp C1.

**Các bước thực tế:**

1. Nạp assembly ứng dụng từ project reference của test.
2. Tra đúng tên lớp yêu cầu trong bảng, ghi danh sách lớp khớp (hiện rỗng).
3. Đối chiếu vị trí triển khai liên quan hiện có; không gọi thay một lớp khác để kết luận hành vi.
4. Assert đơn vị thiết kế tồn tại; Fail tại bước này. Nhánh nghiệp vụ riêng chưa được chạy.

**Kỳ vọng giữ nguyên:** Từ chối quyền lớp nếu không có quan hệ tài nguyên cho phép.

**Quan sát:** requiredType="ClassAccessService"; found=[]; Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.

**Nguyên nhân:** Assembly ứng dụng không có lớp ClassAccessService được chỉ định trong bảng. Code có QuyenLopHocService nhưng chưa có ánh xạ thiết kế được xác nhận. Đây là Fail hợp đồng thiết kế; nhánh phân quyền cụ thể của ca chưa được chạy.

**Ảnh hưởng và giới hạn kết luận:** Không được suy ra hệ thống đang cho phép sai quyền chỉ từ thiếu tên lớp.

**Hướng xử lý:** Thống nhất ánh xạ với QuyenLopHocService hoặc triển khai lớp/adapter trong thiết kế, sau đó chạy lại nhánh đúng dữ liệu của từng ca. Không tự thêm lớp giả trong project test để báo Pass.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\QuyenLopHocService.cs:6` — `public sealed class`.

**Assertion không đạt:**

- Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-309",
  "mode": "DesignContract",
  "requiredType": "ClassAccessService",
  "found": [],
  "source": "Infrastructure/QuyenLopHocService.cs",
  "explanation": "Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.",
  "assertionFailures": [
    "Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass."
  ]
}
```

### UT-310 — Lỗi đọc quyền không thành cho phép

**Kết quả:** Failed; **Tiêu chuẩn:** Xác thực; **Đích:** `ClassAccessService`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Nguồn đọc quyền báo lỗi/không xác định.

**Các bước thực tế:**

1. Nạp assembly ứng dụng từ project reference của test.
2. Tra đúng tên lớp yêu cầu trong bảng, ghi danh sách lớp khớp (hiện rỗng).
3. Đối chiếu vị trí triển khai liên quan hiện có; không gọi thay một lớp khác để kết luận hành vi.
4. Assert đơn vị thiết kế tồn tại; Fail tại bước này. Nhánh nghiệp vụ riêng chưa được chạy.

**Kỳ vọng giữ nguyên:** Không trả Allow; truyền lỗi hoặc Deny theo hợp đồng.

**Quan sát:** requiredType="ClassAccessService"; found=[]; Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.

**Nguyên nhân:** Assembly ứng dụng không có lớp ClassAccessService được chỉ định trong bảng. Code có QuyenLopHocService nhưng chưa có ánh xạ thiết kế được xác nhận. Đây là Fail hợp đồng thiết kế; nhánh phân quyền cụ thể của ca chưa được chạy.

**Ảnh hưởng và giới hạn kết luận:** Không được suy ra hệ thống đang cho phép sai quyền chỉ từ thiếu tên lớp.

**Hướng xử lý:** Thống nhất ánh xạ với QuyenLopHocService hoặc triển khai lớp/adapter trong thiết kế, sau đó chạy lại nhánh đúng dữ liệu của từng ca. Không tự thêm lớp giả trong project test để báo Pass.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\QuyenLopHocService.cs:6` — `public sealed class`.

**Assertion không đạt:**

- Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-310",
  "mode": "DesignContract",
  "requiredType": "ClassAccessService",
  "found": [],
  "source": "Infrastructure/QuyenLopHocService.cs",
  "explanation": "Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.",
  "assertionFailures": [
    "Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass."
  ]
}
```

### UT-311 — Kích thước tệp 9999999 byte

**Kết quả:** Passed; **Tiêu chuẩn:** Đầy đủ; **Đích:** `TepService validator`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. PDF sạch có Length=9999999, 1 tệp.

**Các bước thực tế:**

1. Tạo TepService thật ở môi trường Production, IFileContentScanner mock trả hoàn tất; không bật bỏ quét.
2. Cấp MemoryStream có số byte thực đúng cột dữ liệu; nội dung bắt đầu %PDF-. UT-317 giữ nội dung PDF nhưng tên .exe để kiểm whitelist.
3. Gọi KiemTraAsync thật và bắt LoiNghiepVu nếu có; không gọi hàm LuuAsync.
4. Kiểm số phần tử/kích thước kết quả và số lần quét. Ca bị loại phải chưa quét; ca chấp nhận phải quét đủ từng tệp.

**Kỳ vọng giữ nguyên:** Qua ngưỡng dung lượng.

**Quan sát:** status=200; message=null; fileCount=1; eachLength=9999999; returnedLengths=[9999999]; scannerCalls=1

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-311",
  "mode": "Behavior",
  "source": "Infrastructure/TepService.cs",
  "fileCount": 1,
  "eachLength": 9999999,
  "status": 200,
  "message": null,
  "field": null,
  "returnedLengths": [
    9999999
  ],
  "scannerCalls": 1,
  "assertionFailures": []
}
```

### UT-312 — Kích thước tệp 10000000 byte

**Kết quả:** Passed; **Tiêu chuẩn:** Đầy đủ; **Đích:** `TepService validator`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. PDF sạch có Length=10000000, 1 tệp.

**Các bước thực tế:**

1. Tạo TepService thật ở môi trường Production, IFileContentScanner mock trả hoàn tất; không bật bỏ quét.
2. Cấp MemoryStream có số byte thực đúng cột dữ liệu; nội dung bắt đầu %PDF-. UT-317 giữ nội dung PDF nhưng tên .exe để kiểm whitelist.
3. Gọi KiemTraAsync thật và bắt LoiNghiepVu nếu có; không gọi hàm LuuAsync.
4. Kiểm số phần tử/kích thước kết quả và số lần quét. Ca bị loại phải chưa quét; ca chấp nhận phải quét đủ từng tệp.

**Kỳ vọng giữ nguyên:** Qua ngưỡng dung lượng.

**Quan sát:** status=200; message=null; fileCount=1; eachLength=10000000; returnedLengths=[10000000]; scannerCalls=1

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-312",
  "mode": "Behavior",
  "source": "Infrastructure/TepService.cs",
  "fileCount": 1,
  "eachLength": 10000000,
  "status": 200,
  "message": null,
  "field": null,
  "returnedLengths": [
    10000000
  ],
  "scannerCalls": 1,
  "assertionFailures": []
}
```

### UT-313 — Kích thước tệp 10000001 byte

**Kết quả:** Passed; **Tiêu chuẩn:** Đầy đủ; **Đích:** `TepService validator`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. PDF sạch có Length=10000001, 1 tệp.

**Các bước thực tế:**

1. Tạo TepService thật ở môi trường Production, IFileContentScanner mock trả hoàn tất; không bật bỏ quét.
2. Cấp MemoryStream có số byte thực đúng cột dữ liệu; nội dung bắt đầu %PDF-. UT-317 giữ nội dung PDF nhưng tên .exe để kiểm whitelist.
3. Gọi KiemTraAsync thật và bắt LoiNghiepVu nếu có; không gọi hàm LuuAsync.
4. Kiểm số phần tử/kích thước kết quả và số lần quét. Ca bị loại phải chưa quét; ca chấp nhận phải quét đủ từng tệp.

**Kỳ vọng giữ nguyên:** Từ chối; không gọi lưu tệp.

**Quan sát:** status=400; message="Mỗi tệp phải có dữ liệu và không vượt quá 10 MB."; fileCount=1; eachLength=10000001; returnedLengths=null; scannerCalls=0

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-313",
  "mode": "Behavior",
  "source": "Infrastructure/TepService.cs",
  "fileCount": 1,
  "eachLength": 10000001,
  "status": 400,
  "message": "Mỗi tệp phải có dữ liệu và không vượt quá 10 MB.",
  "field": "Files",
  "returnedLengths": null,
  "scannerCalls": 0,
  "assertionFailures": []
}
```

### UT-314 — Số lượng 5 tệp

**Kết quả:** Passed; **Tiêu chuẩn:** Đầy đủ; **Đích:** `TepService validator`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. 5 tệp sạch, mỗi tệp 1.000.000 byte.

**Các bước thực tế:**

1. Tạo TepService thật ở môi trường Production, IFileContentScanner mock trả hoàn tất; không bật bỏ quét.
2. Cấp MemoryStream có số byte thực đúng cột dữ liệu; nội dung bắt đầu %PDF-. UT-317 giữ nội dung PDF nhưng tên .exe để kiểm whitelist.
3. Gọi KiemTraAsync thật và bắt LoiNghiepVu nếu có; không gọi hàm LuuAsync.
4. Kiểm số phần tử/kích thước kết quả và số lần quét. Ca bị loại phải chưa quét; ca chấp nhận phải quét đủ từng tệp.

**Kỳ vọng giữ nguyên:** Qua ngưỡng số lượng.

**Quan sát:** status=200; message=null; fileCount=5; eachLength=1000000; returnedLengths=[1000000, 1000000, 1000000, 1000000, 1000000]; scannerCalls=5

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-314",
  "mode": "Behavior",
  "source": "Infrastructure/TepService.cs",
  "fileCount": 5,
  "eachLength": 1000000,
  "status": 200,
  "message": null,
  "field": null,
  "returnedLengths": [
    1000000,
    1000000,
    1000000,
    1000000,
    1000000
  ],
  "scannerCalls": 5,
  "assertionFailures": []
}
```

### UT-315 — Số lượng 6 tệp

**Kết quả:** Passed; **Tiêu chuẩn:** Đầy đủ; **Đích:** `TepService validator`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. 6 tệp sạch, mỗi tệp 1.000.000 byte.

**Các bước thực tế:**

1. Tạo TepService thật ở môi trường Production, IFileContentScanner mock trả hoàn tất; không bật bỏ quét.
2. Cấp MemoryStream có số byte thực đúng cột dữ liệu; nội dung bắt đầu %PDF-. UT-317 giữ nội dung PDF nhưng tên .exe để kiểm whitelist.
3. Gọi KiemTraAsync thật và bắt LoiNghiepVu nếu có; không gọi hàm LuuAsync.
4. Kiểm số phần tử/kích thước kết quả và số lần quét. Ca bị loại phải chưa quét; ca chấp nhận phải quét đủ từng tệp.

**Kỳ vọng giữ nguyên:** Từ chối do quá 5 tệp; không lưu một phần.

**Quan sát:** status=400; message="Chọn tối đa 5 tệp mỗi lần gửi."; fileCount=6; eachLength=1000000; returnedLengths=null; scannerCalls=0

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-315",
  "mode": "Behavior",
  "source": "Infrastructure/TepService.cs",
  "fileCount": 6,
  "eachLength": 1000000,
  "status": 400,
  "message": "Chọn tối đa 5 tệp mỗi lần gửi.",
  "field": "Files",
  "returnedLengths": null,
  "scannerCalls": 0,
  "assertionFailures": []
}
```

### UT-316 — Tổng dung lượng tối đa

**Kết quả:** Passed; **Tiêu chuẩn:** Đầy đủ; **Đích:** `TepService validator`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. 5 tệp, mỗi tệp đúng 10.000.000 byte.

**Các bước thực tế:**

1. Tạo TepService thật ở môi trường Production, IFileContentScanner mock trả hoàn tất; không bật bỏ quét.
2. Cấp MemoryStream có số byte thực đúng cột dữ liệu; nội dung bắt đầu %PDF-. UT-317 giữ nội dung PDF nhưng tên .exe để kiểm whitelist.
3. Gọi KiemTraAsync thật và bắt LoiNghiepVu nếu có; không gọi hàm LuuAsync.
4. Kiểm số phần tử/kích thước kết quả và số lần quét. Ca bị loại phải chưa quét; ca chấp nhận phải quét đủ từng tệp.

**Kỳ vọng giữ nguyên:** Qua giới hạn mỗi tệp/số lượng/tổng 50.000.000 byte; còn phải qua kiểm tra nội dung/quét.

**Quan sát:** status=200; message=null; fileCount=5; eachLength=10000000; returnedLengths=[10000000, 10000000, 10000000, 10000000, 10000000]; scannerCalls=5

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-316",
  "mode": "Behavior",
  "source": "Infrastructure/TepService.cs",
  "fileCount": 5,
  "eachLength": 10000000,
  "status": 200,
  "message": null,
  "field": null,
  "returnedLengths": [
    10000000,
    10000000,
    10000000,
    10000000,
    10000000
  ],
  "scannerCalls": 5,
  "assertionFailures": []
}
```

### UT-317 — Từ chối định dạng không hỗ trợ

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `TepService validator`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Tệp .exe hoặc định dạng ngoài PDF/JPG/JPEG/PNG/DOCX.

**Các bước thực tế:**

1. Tạo TepService thật ở môi trường Production, IFileContentScanner mock trả hoàn tất; không bật bỏ quét.
2. Cấp MemoryStream có số byte thực đúng cột dữ liệu; nội dung bắt đầu %PDF-. UT-317 giữ nội dung PDF nhưng tên .exe để kiểm whitelist.
3. Gọi KiemTraAsync thật và bắt LoiNghiepVu nếu có; không gọi hàm LuuAsync.
4. Kiểm số phần tử/kích thước kết quả và số lần quét. Ca bị loại phải chưa quét; ca chấp nhận phải quét đủ từng tệp.

**Kỳ vọng giữ nguyên:** Từ chối; không gọi lưu.

**Quan sát:** status=400; message="Chỉ nhận PDF, JPG, PNG, DOCX."; fileCount=1; eachLength=1000000; returnedLengths=null; scannerCalls=0

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-317",
  "mode": "Behavior",
  "source": "Infrastructure/TepService.cs",
  "fileCount": 1,
  "eachLength": 1000000,
  "status": 400,
  "message": "Chỉ nhận PDF, JPG, PNG, DOCX.",
  "field": "Files",
  "returnedLengths": null,
  "scannerCalls": 0,
  "assertionFailures": []
}
```

## Chạy lại trong VS Code

Mở thư mục `C:\Khóa luận TN\Educonnect`, Terminal PowerShell:

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter "Suite=Remaining10" --logger "trx;LogFileName=Group10-Unit.trx" --results-directory "TestResults/10-ThanhPhan-DungChung-Unit"

dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter "FullyQualifiedName~EduConnect.Remaining.UnitTests.RemainingUnitTest.UT_301"
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
