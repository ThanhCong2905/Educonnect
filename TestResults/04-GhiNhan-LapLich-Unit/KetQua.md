# Kết quả kiểm thử — 04. Ghi nhận và lập lịch học

Cập nhật ngày 14/11/2026, thay thế báo cáo trước ngay tại cùng vị trí. Chuẩn đối chiếu: các đơn vị và kỳ vọng trong Unit Test.xlsx do người dùng cung cấp.

**15 ca: 3 Pass, 12 Fail, 0 Skip/Blocked.**

Trong đó **8 ca Fail đối chiếu thiết kế** vì thiếu đúng phương thức/overload/model. Đây là các ca bị ảnh hưởng, không phải 8 lỗi độc lập. Các ca cùng thiếu một phương thức có cùng nguyên nhân gốc.

## Quy tắc kết luận

- Phương thức/model thiết kế yêu cầu nhưng code chưa có, hoặc code dùng tên/overload khác: **Fail — không đáp ứng thiết kế**.
- Không tự coi Buoi(m) là GhiNhan(m), NopBai là Nop, hoặc GiaoBaiTapAsync là GiaoAsync để báo Pass.
- Với ca Fail đối chiếu thiết kế: kiểm tra sự tồn tại đã chạy và thất bại; nhánh hành vi phía sau chưa được chạy. Không đổi kết luận thành Blocked.
- Với đơn vị có đúng hợp đồng đã xác định: gọi code thật, dùng mock cho dependency, rồi assert đầu ra/tác dụng phụ. Không chạy database trong bộ unit này.
- Mock cổng lưu là kỹ thuật cô lập dependency. Không đặt việc mở SQL Server, chạy procedure hoặc truy vấn bảng dữ liệu vào bước thực thi unit.
- Không sửa code nghiệp vụ trong lần cập nhật này. Đã sửa test, chạy lại và thay báo cáo/TRX cũ.

## Tổng hợp từng ca

| Mã | Tình huống | Kết quả | Kiểm tra đã chạy | Sai lệch cụ thể / kết quả quan sát |
|---|---|---|---|---|
| UT-065 | Id rỗng | Fail | Unit hành vi | Controller vẫn gọi service. Trong unit test, service giả lập trả dữ liệu; action đi tiếp và trả ViewResult. Assert.IsType<BadRequestResult> thất bại. |
| UT-066 | Không tìm thấy | Fail | Unit hành vi | Mock service ném LoiNghiepVu có Status=404. Exception thoát khỏi Buoi; action không trả NotFoundResult. |
| UT-067 | Không có quyền | Fail | Unit hành vi | Mock service ném LoiNghiepVu có Status=403; exception thoát khỏi action, không có ForbidResult. |
| UT-068 | Nạp biểu mẫu | Fail | Unit hành vi | Id, RowVersion, nội dung và nhận xét đã đúng. Tuy nhiên các lời gọi service chỉ có Guid id; không có đối số CancellationToken từ RequestAborted và không có mã actor của phiên test. |
| UT-069 | ModelState ghi nhận lỗi | Fail | Đối chiếu thiết kế | Không tìm thấy LichHocController.Buoi(GhiNhanBuoiHocViewModel). Code đang có GhiNhan(GhiNhanBuoiHocViewModel); đây không phải tên đơn vị trong thiết kế. |
| UT-070 | Ghi nhận hợp lệ | Fail | Đối chiếu thiết kế | Không tìm thấy LichHocService.GhiNhanBuoiHocAsync(...). Code đang có GhiNhanAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-071 | Sai quyền gia sư | Fail | Đối chiếu thiết kế | Không tìm thấy LichHocService.GhiNhanBuoiHocAsync(...). Code đang có GhiNhanAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-072 | Trạng thái không cho ghi nhận | Fail | Đối chiếu thiết kế | Không tìm thấy LichHocService.GhiNhanBuoiHocAsync(...). Code đang có GhiNhanAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-073 | ModelState lịch học lỗi | Pass | Unit hành vi | Model được giữ nguyên, lỗi còn trong ModelState, ThemHangLoatAsync không được gọi. |
| UT-074 | Tạo nhiều buổi hợp lệ | Pass | Unit hành vi | Đối số cổng lưu chứa đúng lớp, gia sư và hai buổi với tên/giờ đúng; gọi thông báo LichHoc, lưu và commit đúng một lần. |
| UT-075 | ModelState đổi lịch lỗi | Fail | Đối chiếu thiết kế | Không tìm thấy LichHocController.DoiLich(SuaLichInput). Code đang có Sua(SuaLichInput); đây không phải tên đơn vị trong thiết kế. |
| UT-076 | Đổi lịch thành công | Fail | Đối chiếu thiết kế | Không tìm thấy LichHocService.DoiLichAsync(...). Code đang có SuaAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-077 | Lớp không hoạt động | Fail | Đối chiếu thiết kế | Không tìm thấy LichHocService.GhiNhanBuoiHocAsync(...). Code đang có GhiNhanAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-078 | Buổi chưa kết thúc | Fail | Đối chiếu thiết kế | Không tìm thấy LichHocService.GhiNhanBuoiHocAsync(...). Code đang có GhiNhanAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-079 | Thêm buổi giữ buổi ban đầu | Pass | Unit hành vi | Buổi cũ giữ nguyên thời điểm/trạng thái; cổng lưu nhận một yêu cầu thêm buổi mới, đúng lớp. |

## Mô tả chi tiết từng ca

### UT-065 — Id rỗng

**Kết quả: Fail.** Loại kiểm tra: unit hành vi.

**Mong đợi:** Gọi Buoi(Guid.Empty) phải trả BadRequestResult ngay; không gọi service lấy lịch.

**Thực tế:** Controller vẫn gọi service. Trong unit test, service giả lập trả dữ liệu; action đi tiếp và trả ViewResult. Assert.IsType<BadRequestResult> thất bại.

**Các bước kiểm thử đã thực hiện:**

1. Tạo controller thật, HttpContext/TempData giả và các service mock. Cấu hình service trả dữ liệu để quan sát controller có chặn ID rỗng hay không.
2. Gọi trực tiếp Buoi(Guid.Empty).
3. Assert BadRequestResult; Verify service không được gọi. Thực tế nhận ViewResult nên Fail.

**Vị trí code cần kiểm tra:** Areas/HocTap/Controllers/LichHocController.cs:89–91

**Nguyên nhân:** Hàm Buoi(Guid id) gọi await service.LayAsync(id) ngay ở đầu hàm; không có điều kiện if (id == Guid.Empty) trước đó.

**Hành vi/thiết kế bị ảnh hưởng:** Không đáp ứng nhánh từ chối ID rỗng tại controller theo thiết kế. Không có nghĩa database thật đã lưu lịch có ID rỗng.

**Yêu cầu sửa:** Bổ sung kiểm tra Guid.Empty trước lời gọi service, trả BadRequestResult; test phải Verify service lấy lịch Times.Never().

**Minh chứng test:**

```text
Assert.IsType() Failure: Value is not the exact type
Expected: typeof(Microsoft.AspNetCore.Mvc.BadRequestResult)
Actual:   typeof(Microsoft.AspNetCore.Mvc.ViewResult)
   at EduConnect.HocTap.UnitTests.HocTapControllerTest.UT_065_IdRong_BadRequest_KhongDocService() in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapControllerTest.cs:line 21
--- End of stack trace from previous location ---
```

### UT-066 — Không tìm thấy

**Kết quả: Fail.** Loại kiểm tra: unit hành vi.

**Mong đợi:** Khi service báo lịch không tồn tại, action trả NotFoundResult theo bảng TC.

**Thực tế:** Mock service ném LoiNghiepVu có Status=404. Exception thoát khỏi Buoi; action không trả NotFoundResult.

**Các bước kiểm thử đã thực hiện:**

1. Mock LayAsync(id) ném LoiNghiepVu("Không có lịch học.", 404).
2. Gọi Buoi(id) trên controller thật.
3. Assert kết quả là NotFoundResult. Thực tế có exception thoát ra nên Fail.

**Vị trí code cần kiểm tra:** Areas/HocTap/Controllers/LichHocController.cs:91

**Nguyên nhân:** GET Buoi chưa có nhánh bắt kết quả/lỗi không tìm thấy và chuyển thành NotFoundResult.

**Hành vi/thiết kế bị ảnh hưởng:** Sai kiểu kết quả và nhánh xử lý lỗi của action so với thiết kế được dùng làm chuẩn. Unit này không kiểm mã HTTP của middleware.

**Yêu cầu sửa:** Xử lý nhánh không tìm thấy tại action đúng hợp đồng NotFoundResult; giữ assert theo thiết kế, không đổi thành chấp nhận exception để làm test Pass.

**Minh chứng test:**

```text
EduConnect.Mvc.Infrastructure.LoiNghiepVu : Không có lịch học.
   at EduConnect.Mvc.Areas.HocTap.Controllers.LichHocController.Buoi(Guid id) in C:\Khóa luận TN\Educonnect\Areas\HocTap\Controllers\LichHocController.cs:line 91
   at EduConnect.HocTap.UnitTests.HocTapControllerTest.UT_066_KhongTimThay_TraNotFound() in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapControllerTest.cs:line 27
--- End of stack trace from previous location ---
```

### UT-067 — Không có quyền

**Kết quả: Fail.** Loại kiểm tra: unit hành vi.

**Mong đợi:** Khi người dùng không có quyền xem buổi học, action trả ForbidResult.

**Thực tế:** Mock service ném LoiNghiepVu có Status=403; exception thoát khỏi action, không có ForbidResult.

**Các bước kiểm thử đã thực hiện:**

1. Mock service lấy lịch ném LoiNghiepVu("Không có quyền.", 403).
2. Gọi Buoi(id).
3. Assert ForbidResult. Thực tế exception thoát ra nên Fail.

**Vị trí code cần kiểm tra:** Areas/HocTap/Controllers/LichHocController.cs:91

**Nguyên nhân:** Action gọi service trực tiếp, chưa chuyển nhánh từ chối quyền thành ForbidResult theo bảng TC.

**Hành vi/thiết kế bị ảnh hưởng:** Sai nhánh phản hồi của controller. Không đánh đồng exception 403 với việc đã trả ForbidResult.

**Yêu cầu sửa:** Xử lý nhánh từ chối quyền theo hợp đồng action; kiểm thêm service không bị gọi ghi dữ liệu.

**Minh chứng test:**

```text
EduConnect.Mvc.Infrastructure.LoiNghiepVu : Không có quyền.
   at EduConnect.Mvc.Areas.HocTap.Controllers.LichHocController.Buoi(Guid id) in C:\Khóa luận TN\Educonnect\Areas\HocTap\Controllers\LichHocController.cs:line 91
   at EduConnect.HocTap.UnitTests.HocTapControllerTest.UT_067_KhongCoQuyen_TraForbid() in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapControllerTest.cs:line 33
--- End of stack trace from previous location ---
```

### UT-068 — Nạp biểu mẫu

**Kết quả: Fail.** Loại kiểm tra: unit hành vi.

**Mong đợi:** Nạp đúng model Buoi và truyền đúng mã người thực hiện/cancellation token RequestAborted xuống service theo cột kết quả mong đợi.

**Thực tế:** Id, RowVersion, nội dung và nhận xét đã đúng. Tuy nhiên các lời gọi service chỉ có Guid id; không có đối số CancellationToken từ RequestAborted và không có mã actor của phiên test.

**Các bước kiểm thử đã thực hiện:**

1. Tạo HttpContext với RequestAborted là token riêng và principal chứa ActorId riêng; mock trả lịch/bản ghi có RV1.
2. Gọi Buoi(id), assert model và các trường (đã đạt).
3. Đọc các invocation thực trên mock, assert có token bằng RequestAborted và ActorId. Không có token trong đối số nên Fail.

**Vị trí code cần kiểm tra:** Areas/HocTap/Controllers/LichHocController.cs:91–102; Areas/HocTap/Services/ILichHocService.cs:LayAsync/GhiNhanChiTietAsync

**Nguyên nhân:** Hợp đồng đọc lịch/chi tiết hiện tại chỉ nhận ID; controller không chuyển actor/token như bảng TC yêu cầu. Test trước chỉ assert model nên đã bỏ sót phần này.

**Hành vi/thiết kế bị ảnh hưởng:** Model đúng nhưng chưa đáp ứng đầy đủ dữ liệu ngữ cảnh mà thiết kế yêu cầu truyền. Vì vậy không được báo Pass toàn bộ UT-068.

**Yêu cầu sửa:** Đồng bộ chữ ký service đọc buổi và lời gọi controller theo thiết kế để nhận actor/ct; bổ sung Verify đủ tham số rồi chạy lại.

**Minh chứng test:**

```text
UT-068: model đã nạp đúng nhưng controller không truyền RequestAborted xuống service như kỳ vọng thiết kế; các lời gọi hiện chỉ nhận Guid id.
   at EduConnect.HocTap.UnitTests.HocTapControllerTest.UT_068_NapBieuMau_DungIdVaRowVersion() in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapControllerTest.cs:line 45
--- End of stack trace from previous location ---
```

### UT-069 — ModelState ghi nhận lỗi

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có LichHocController.Buoi(GhiNhanBuoiHocViewModel) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy LichHocController.Buoi(GhiNhanBuoiHocViewModel). Code đang có GhiNhan(GhiNhanBuoiHocViewModel); đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: LichHocController.Buoi(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên LichHocController.Buoi(GhiNhanBuoiHocViewModel). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi GhiNhan(GhiNhanBuoiHocViewModel) thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Controllers/LichHocController.cs:139 — `public async Task<IActionResult> GhiNhan(GhiNhanBuoiHocViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “ModelState ghi nhận lỗi”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ LichHocController.Buoi(GhiNhanBuoiHocViewModel) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-069 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public LichHocController.Buoi(GhiNhanBuoiHocViewModel).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: GhiNhan(GhiNhanBuoiHocViewModel).
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14
   at EduConnect.HocTap.UnitTests.HocTapControllerTest.UT_069_ThietKe_Buoi_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapControllerTest.cs:line 53
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-070 — Ghi nhận hợp lệ

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có LichHocService.GhiNhanBuoiHocAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy LichHocService.GhiNhanBuoiHocAsync(...). Code đang có GhiNhanAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: LichHocService.GhiNhanBuoiHocAsync(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên LichHocService.GhiNhanBuoiHocAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi GhiNhanAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/LichHocService.cs:124 — `public async Task GhiNhanAsync(GhiNhanBuoiHocViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Ghi nhận hợp lệ”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ LichHocService.GhiNhanBuoiHocAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-070 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public LichHocService.GhiNhanBuoiHocAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: GhiNhanAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14
   at EduConnect.HocTap.UnitTests.LichHocServiceTest.UT_070_ThietKe_GhiNhanBuoiHocAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\LichHocServiceTest.cs:line 17
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-071 — Sai quyền gia sư

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có LichHocService.GhiNhanBuoiHocAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy LichHocService.GhiNhanBuoiHocAsync(...). Code đang có GhiNhanAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: LichHocService.GhiNhanBuoiHocAsync(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên LichHocService.GhiNhanBuoiHocAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi GhiNhanAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/LichHocService.cs:124 — `public async Task GhiNhanAsync(GhiNhanBuoiHocViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Sai quyền gia sư”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ LichHocService.GhiNhanBuoiHocAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-071 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public LichHocService.GhiNhanBuoiHocAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: GhiNhanAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14
   at EduConnect.HocTap.UnitTests.LichHocServiceTest.UT_071_ThietKe_GhiNhanBuoiHocAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\LichHocServiceTest.cs:line 22
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-072 — Trạng thái không cho ghi nhận

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có LichHocService.GhiNhanBuoiHocAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy LichHocService.GhiNhanBuoiHocAsync(...). Code đang có GhiNhanAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: LichHocService.GhiNhanBuoiHocAsync(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên LichHocService.GhiNhanBuoiHocAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi GhiNhanAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/LichHocService.cs:124 — `public async Task GhiNhanAsync(GhiNhanBuoiHocViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Trạng thái không cho ghi nhận”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ LichHocService.GhiNhanBuoiHocAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-072 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public LichHocService.GhiNhanBuoiHocAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: GhiNhanAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14
   at EduConnect.HocTap.UnitTests.LichHocServiceTest.UT_072_ThietKe_GhiNhanBuoiHocAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\LichHocServiceTest.cs:line 27
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-073 — ModelState lịch học lỗi

**Kết quả: Pass.** Loại kiểm tra: unit hành vi.

**Mong đợi:** Controller nhận ModelState lỗi phải giữ đúng biểu mẫu và không gọi hàm thêm lịch.

**Thực tế:** Model được giữ nguyên, lỗi còn trong ModelState, ThemHangLoatAsync không được gọi.

**Các bước kiểm thử đã thực hiện:**

1. Tạo LichHangLoatInput có LopHocId và một buổi; thêm lỗi Buoi[0].TieuDe vào ModelState.
2. Mock nguồn dữ liệu lớp chỉ để trả lại form; gọi Them(m) thật.
3. Assert ViewResult giữ cùng model, ModelState không hợp lệ; Verify ThemHangLoatAsync Times.Never().

### UT-074 — Tạo nhiều buổi hợp lệ

**Kết quả: Pass.** Loại kiểm tra: unit hành vi.

**Mong đợi:** Service phải chuyển đúng hai buổi hợp lệ tới cổng lưu và yêu cầu thông báo lịch học.

**Thực tế:** Đối số cổng lưu chứa đúng lớp, gia sư và hai buổi với tên/giờ đúng; gọi thông báo LichHoc, lưu và commit đúng một lần.

**Các bước kiểm thử đã thực hiện:**

1. Cố định clock; mock quyền gia sư hợp lệ, cổng lưu và thông báo. Chuẩn bị hai buổi tương lai không giao nhau.
2. Gọi ThemHangLoatAsync(m) thật.
3. Đọc các đối số đã bắt trên mock: kiểm đúng hai phần tử, LopHocId, GiaSuId, thời gian; Verify thông báo, Save/Commit. Không kiểm dữ liệu trong CSDL.

### UT-075 — ModelState đổi lịch lỗi

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có LichHocController.DoiLich(SuaLichInput) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy LichHocController.DoiLich(SuaLichInput). Code đang có Sua(SuaLichInput); đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: LichHocController.DoiLich(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên LichHocController.DoiLich(SuaLichInput). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi Sua(SuaLichInput) thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Controllers/LichHocController.cs:40 — `public async Task<IActionResult> Sua(Guid id)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “ModelState đổi lịch lỗi”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ LichHocController.DoiLich(SuaLichInput) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-075 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public LichHocController.DoiLich(SuaLichInput).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: Sua(SuaLichInput).
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14
   at EduConnect.HocTap.UnitTests.HocTapControllerTest.UT_075_ThietKe_DoiLich_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapControllerTest.cs:line 66
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-076 — Đổi lịch thành công

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có LichHocService.DoiLichAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy LichHocService.DoiLichAsync(...). Code đang có SuaAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: LichHocService.DoiLichAsync(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên LichHocService.DoiLichAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi SuaAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/LichHocService.cs:43 — `public async Task SuaAsync(SuaLichInput m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Đổi lịch thành công”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ LichHocService.DoiLichAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-076 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public LichHocService.DoiLichAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: SuaAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14
   at EduConnect.HocTap.UnitTests.LichHocServiceTest.UT_076_ThietKe_DoiLichAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\LichHocServiceTest.cs:line 41
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-077 — Lớp không hoạt động

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có LichHocService.GhiNhanBuoiHocAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy LichHocService.GhiNhanBuoiHocAsync(...). Code đang có GhiNhanAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: LichHocService.GhiNhanBuoiHocAsync.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên LichHocService.GhiNhanBuoiHocAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi GhiNhanAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/LichHocService.cs:124 — `public async Task GhiNhanAsync(GhiNhanBuoiHocViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Lớp không hoạt động”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ LichHocService.GhiNhanBuoiHocAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-077 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public LichHocService.GhiNhanBuoiHocAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: GhiNhanAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14
   at EduConnect.HocTap.UnitTests.LichHocServiceTest.UT_077_ThietKe_GhiNhanBuoiHocAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\LichHocServiceTest.cs:line 46
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-078 — Buổi chưa kết thúc

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có LichHocService.GhiNhanBuoiHocAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy LichHocService.GhiNhanBuoiHocAsync(...). Code đang có GhiNhanAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: LichHocService.GhiNhanBuoiHocAsync.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên LichHocService.GhiNhanBuoiHocAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi GhiNhanAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/LichHocService.cs:124 — `public async Task GhiNhanAsync(GhiNhanBuoiHocViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Buổi chưa kết thúc”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ LichHocService.GhiNhanBuoiHocAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-078 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public LichHocService.GhiNhanBuoiHocAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: GhiNhanAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14
   at EduConnect.HocTap.UnitTests.LichHocServiceTest.UT_078_ThietKe_GhiNhanBuoiHocAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\LichHocServiceTest.cs:line 51
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-079 — Thêm buổi giữ buổi ban đầu

**Kết quả: Pass.** Loại kiểm tra: unit hành vi.

**Mong đợi:** Yêu cầu thêm buổi mới không được làm thay đổi đối tượng buổi cũ trong đơn vị đang kiểm tra.

**Thực tế:** Buổi cũ giữ nguyên thời điểm/trạng thái; cổng lưu nhận một yêu cầu thêm buổi mới, đúng lớp.

**Các bước kiểm thử đã thực hiện:**

1. Chuẩn bị B1 trong fixture và chụp thời điểm/trạng thái; chuẩn bị dữ liệu B2 khác giờ.
2. Gọi ThemHangLoatAsync với chỉ B2.
3. Assert B1 không bị sửa trong bộ nhớ; Verify yêu cầu gửi cổng lưu có đúng một buổi B2 và đúng lớp.

## Những kết luận trước đã được sửa

- Đã bỏ các Skip của UT-081/087/088/091: thiếu phương thức thiết kế được kiểm và ghi Fail.
- Đã thu hồi những Pass dựa trên việc gọi phương thức khác tên/model khác tên. Không dùng kết quả của GiaoAsync để xác nhận GiaoBaiTapAsync hoặc ChamBaiViewModel để xác nhận ChamDiemViewModel.
- UT-068 đã thêm assertion actor/token mà test trước bỏ sót; vì vậy ca này chuyển từ Pass sang Fail.
- Đã bỏ việc tạo một object bài tập giả trong test sự kiện rồi mô tả đó là kiểm chứng bài đã lưu; UT-101 hiện chỉ kiểm handler phát/thử lại đúng phạm vi unit.
- Báo cáo trước nêu SQL lẫn trong phần kết quả TC đã gây hiểu nhầm. Các báo cáo hiện tại mô tả code C#, mock và assertion; không lấy việc mock báo thành công làm bằng chứng dữ liệu đã lưu thật.

## Chạy lại

Tại thư mục Educonnect trong VS Code:

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --no-restore -p:UseAppHost=false "-p:BaseOutputPath=C:/Users/NGUYEN THI YEN/Documents/Codex/2026-11-13/un/work/hoctap-bin/" --filter "Suite=LichHoc" --results-directory "C:/Users/NGUYEN THI YEN/Documents/Codex/2026-11-13/un/outputs/04-GhiNhan-LapLich-Unit" --logger "trx;LogFileName=GhiNhan-LapLich-Unit.trx"
```

TestKind=DesignContract là kiểm đúng đơn vị khai báo theo thiết kế. Bộ này chưa tuyên bố rà mọi hàm phụ trợ trong toàn hệ thống; phạm vi là 41 ca được yêu cầu.

## Minh chứng hiện hành

- [TRX đã thay thế](GhiNhan-LapLich-Unit.trx)
- [Kết quả máy đọc được](KetQua.json)
- [Mã nguồn test và các thay đổi hỗ trợ](Ma_nguon_Unit.zip)
