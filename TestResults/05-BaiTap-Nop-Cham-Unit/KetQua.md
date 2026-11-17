# Kết quả kiểm thử — 05. Bài tập, nộp và chấm bài

Cập nhật ngày 14/11/2026, thay thế báo cáo trước ngay tại cùng vị trí. Chuẩn đối chiếu: các đơn vị và kỳ vọng trong Unit Test.xlsx do người dùng cung cấp.

**26 ca: 2 Pass, 24 Fail, 0 Skip/Blocked.**

Trong đó **24 ca Fail đối chiếu thiết kế** vì thiếu đúng phương thức/overload/model. Đây là các ca bị ảnh hưởng, không phải 24 lỗi độc lập. Các ca cùng thiếu một phương thức có cùng nguyên nhân gốc.

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
| UT-080 | Dữ liệu không hợp lệ | Pass | Unit hành vi | ViewResult giữ đúng input; ModelState còn lỗi; GiaoAsync không được gọi. |
| UT-081 | Dữ liệu không hợp lệ | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapController.Sua(...). Chức năng sửa nội dung/hạn tương ứng cũng chưa có trong code. |
| UT-082 | Dữ liệu không hợp lệ | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapController.NopBai(NopBaiViewModel). Code đang có Nop(NopBaiViewModel); đây không phải tên đơn vị trong thiết kế. |
| UT-083 | Dữ liệu không hợp lệ | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapController.ChamBai(...). Code đang có Cham(ChamBaiViewModel); đây không phải tên đơn vị trong thiết kế. |
| UT-084 | Giao bài hợp lệ | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.GiaoBaiTapAsync(...). Code đang có GiaoAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-085 | Tệp sai định dạng | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.GiaoBaiTapAsync(...). Code đang có GiaoAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-086 | Tệp quá dung lượng | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.GiaoBaiTapAsync(...). Code đang có GiaoAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-087 | Cập nhật nội dung và hạn | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.CapNhatBaiTapAsync(...). Chức năng sửa nội dung/hạn tương ứng cũng chưa có trong code. |
| UT-088 | Xung đột phiên bản bài tập | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.CapNhatBaiTapAsync(...). Chức năng sửa nội dung/hạn tương ứng cũng chưa có trong code. |
| UT-089 | Nộp trước hạn | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.NopBaiAsync(...). Code đang có NopAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-090 | Nộp sau hạn | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.NopBaiAsync(...). Code đang có NopAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-091 | Đúng thời điểm hạn | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.NopBaiAsync(...). Code đang có NopAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-092 | Tệp không hỗ trợ | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.NopBaiAsync(...). Code đang có NopAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-093 | Chấm lần đầu thành công | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.ChamBaiAsync(...). Code đang có ChamAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-094 | Chấm lần đầu sai quyền | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.ChamBaiAsync(...). Code đang có ChamAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-095 | Sửa kết quả chấm thành công | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.ChamBaiAsync(...). Code đang có ChamAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-096 | Sửa kết quả chấm sai quyền | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.ChamBaiAsync(...). Code đang có ChamAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-097 | Điểm ngoài thang | Fail | Đối chiếu thiết kế | Không tìm thấy ChamDiemViewModel. Code đang có ChamBaiViewModel; đây không phải tên đơn vị trong thiết kế. |
| UT-098 | Thiếu nhận xét bắt buộc | Fail | Đối chiếu thiết kế | Không tìm thấy ChamDiemViewModel. Code đang có ChamBaiViewModel; đây không phải tên đơn vị trong thiết kế. |
| UT-099 | Sửa điểm với RV cũ | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.ChamBaiAsync(...). Code đang có ChamAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-100 | Lỗi lưu bài tập | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.GiaoBaiTapAsync(...). Code đang có GiaoAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-101 | Lỗi phát thông báo giữ bài đã lưu | Pass | Unit hành vi | Lần phát lỗi gọi retry một lần, không complete; lần thử lại dùng cùng event ID và complete sau khi publish thành công. |
| UT-102 | Đủ người nhận có quyền | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.GiaoBaiTapAsync(...). Code đang có GiaoAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-103 | Chưa có bài nộp | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.ChamBaiAsync(...). Code đang có ChamAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-104 | Đủ cập nhật tiến độ và người nhận | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.ChamBaiAsync(...). Code đang có ChamAsync; đây không phải tên đơn vị trong thiết kế. |
| UT-105 | Quyền bị thu hồi trong lúc quét | Fail | Đối chiếu thiết kế | Không tìm thấy BaiTapService.NopBaiAsync(...). Code đang có NopAsync; đây không phải tên đơn vị trong thiết kế. |

## Mô tả chi tiết từng ca

### UT-080 — Dữ liệu không hợp lệ

**Kết quả: Pass.** Loại kiểm tra: unit hành vi.

**Mong đợi:** ModelState giao bài lỗi phải giữ model/lỗi và không gọi service giao bài.

**Thực tế:** ViewResult giữ đúng input; ModelState còn lỗi; GiaoAsync không được gọi.

**Các bước kiểm thử đã thực hiện:**

1. Tạo controller thật; thêm lỗi TieuDe vào ModelState, giữ tiêu đề/nội dung đã nhập.
2. Mock các dữ liệu đọc cần để nạp lại form; gọi Giao(m).
3. Assert ViewResult giữ đúng model và lỗi; Verify service ghi Times.Never().

### UT-081 — Dữ liệu không hợp lệ

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapController.Sua(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapController.Sua(...). Chức năng sửa nội dung/hạn tương ứng cũng chưa có trong code.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapController.Sua(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapController.Sua(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi Không có action sửa nội dung và hạn bài tập thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Controllers/BaiTapController.cs — không có khai báo phương thức yêu cầu trong lớp này.

**Nguyên nhân:** Thiếu triển khai chức năng theo thiết kế.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Dữ liệu không hợp lệ”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapController.Sua(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-081 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapController.Sua(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: Không có action sửa nội dung và hạn bài tập.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.HocTapControllerTest.UT_081_ThietKe_Sua_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapControllerTest.cs:line 79

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-082 — Dữ liệu không hợp lệ

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapController.NopBai(NopBaiViewModel) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapController.NopBai(NopBaiViewModel). Code đang có Nop(NopBaiViewModel); đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapController.NopBai(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapController.NopBai(NopBaiViewModel). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi Nop(NopBaiViewModel) thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Controllers/BaiTapController.cs:93 — `public async Task<IActionResult> Nop(NopBaiViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Dữ liệu không hợp lệ”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapController.NopBai(NopBaiViewModel) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-082 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapController.NopBai(NopBaiViewModel).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: Nop(NopBaiViewModel).
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.HocTapControllerTest.UT_082_ThietKe_NopBai_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapControllerTest.cs:line 84

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-083 — Dữ liệu không hợp lệ

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapController.ChamBai(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapController.ChamBai(...). Code đang có Cham(ChamBaiViewModel); đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapController.ChamBai(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapController.ChamBai(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi Cham(ChamBaiViewModel) thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Controllers/BaiTapController.cs:107 — `public async Task<IActionResult> Cham(ChamBaiViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Dữ liệu không hợp lệ”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapController.ChamBai(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-083 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapController.ChamBai(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: Cham(ChamBaiViewModel).
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.HocTapControllerTest.UT_083_ThietKe_ChamBai_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapControllerTest.cs:line 89

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-084 — Giao bài hợp lệ

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.GiaoBaiTapAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.GiaoBaiTapAsync(...). Code đang có GiaoAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.GiaoBaiTapAsync(m) / validator tệp.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.GiaoBaiTapAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi GiaoAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:36 — `public async Task GiaoAsync(GiaoBaiTapViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Giao bài hợp lệ”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.GiaoBaiTapAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-084 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.GiaoBaiTapAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: GiaoAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_084_ThietKe_GiaoBaiTapAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 13

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-085 — Tệp sai định dạng

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.GiaoBaiTapAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.GiaoBaiTapAsync(...). Code đang có GiaoAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.GiaoBaiTapAsync(m) / validator tệp.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.GiaoBaiTapAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi GiaoAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:36 — `public async Task GiaoAsync(GiaoBaiTapViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Tệp sai định dạng”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.GiaoBaiTapAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-085 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.GiaoBaiTapAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: GiaoAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_085_ThietKe_GiaoBaiTapAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 18

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-086 — Tệp quá dung lượng

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.GiaoBaiTapAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.GiaoBaiTapAsync(...). Code đang có GiaoAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.GiaoBaiTapAsync(m) / validator tệp.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.GiaoBaiTapAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi GiaoAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:36 — `public async Task GiaoAsync(GiaoBaiTapViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Tệp quá dung lượng”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.GiaoBaiTapAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-086 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.GiaoBaiTapAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: GiaoAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_086_ThietKe_GiaoBaiTapAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 23

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-087 — Cập nhật nội dung và hạn

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.CapNhatBaiTapAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.CapNhatBaiTapAsync(...). Chức năng sửa nội dung/hạn tương ứng cũng chưa có trong code.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.CapNhatBaiTapAsync(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.CapNhatBaiTapAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi Không có phương thức sửa nội dung và hạn bài tập thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs — không có khai báo phương thức yêu cầu trong lớp này.

**Nguyên nhân:** Thiếu triển khai chức năng theo thiết kế.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Cập nhật nội dung và hạn”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.CapNhatBaiTapAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-087 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.CapNhatBaiTapAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: Không có phương thức sửa nội dung và hạn bài tập.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_087_ThietKe_CapNhatBaiTapAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 28

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-088 — Xung đột phiên bản bài tập

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.CapNhatBaiTapAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.CapNhatBaiTapAsync(...). Chức năng sửa nội dung/hạn tương ứng cũng chưa có trong code.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.CapNhatBaiTapAsync(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.CapNhatBaiTapAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi Không có phương thức sửa nội dung và hạn bài tập thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs — không có khai báo phương thức yêu cầu trong lớp này.

**Nguyên nhân:** Thiếu triển khai chức năng theo thiết kế.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Xung đột phiên bản bài tập”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.CapNhatBaiTapAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-088 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.CapNhatBaiTapAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: Không có phương thức sửa nội dung và hạn bài tập.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_088_ThietKe_CapNhatBaiTapAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 33

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-089 — Nộp trước hạn

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.NopBaiAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.NopBaiAsync(...). Code đang có NopAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.NopBaiAsync(m) / validator.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.NopBaiAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi NopAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:71 — `public async Task NopAsync(NopBaiViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Nộp trước hạn”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.NopBaiAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-089 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.NopBaiAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: NopAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_089_ThietKe_NopBaiAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 38

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-090 — Nộp sau hạn

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.NopBaiAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.NopBaiAsync(...). Code đang có NopAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.NopBaiAsync(m) / validator.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.NopBaiAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi NopAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:71 — `public async Task NopAsync(NopBaiViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Nộp sau hạn”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.NopBaiAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-090 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.NopBaiAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: NopAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_090_ThietKe_NopBaiAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 43

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-091 — Đúng thời điểm hạn

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.NopBaiAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.NopBaiAsync(...). Code đang có NopAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.NopBaiAsync(m) / validator.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.NopBaiAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi NopAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:71 — `public async Task NopAsync(NopBaiViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Đúng thời điểm hạn”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.NopBaiAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Điểm cần làm rõ sau khi sửa tên hàm:** bảng gốc chưa chốt < hay <= tại đúng hạn. Kết luận Fail hiện tại là do thiếu NopBaiAsync, không phải tự chọn một quy tắc so sánh thời gian chưa được đặc tả.

**Minh chứng test:**

```text
UT-091 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.NopBaiAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: NopAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_091_ThietKe_NopBaiAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 48

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-092 — Tệp không hỗ trợ

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.NopBaiAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.NopBaiAsync(...). Code đang có NopAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.NopBaiAsync(m) / validator.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.NopBaiAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi NopAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:71 — `public async Task NopAsync(NopBaiViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Tệp không hỗ trợ”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.NopBaiAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-092 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.NopBaiAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: NopAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_092_ThietKe_NopBaiAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 53

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-093 — Chấm lần đầu thành công

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.ChamBaiAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.ChamBaiAsync(...). Code đang có ChamAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.ChamBaiAsync(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.ChamBaiAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi ChamAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:110 — `public async Task ChamAsync(ChamBaiViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Chấm lần đầu thành công”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.ChamBaiAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-093 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.ChamBaiAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: ChamAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_093_ThietKe_ChamBaiAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 58

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-094 — Chấm lần đầu sai quyền

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.ChamBaiAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.ChamBaiAsync(...). Code đang có ChamAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.ChamBaiAsync(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.ChamBaiAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi ChamAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:110 — `public async Task ChamAsync(ChamBaiViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Chấm lần đầu sai quyền”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.ChamBaiAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-094 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.ChamBaiAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: ChamAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_094_ThietKe_ChamBaiAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 63

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-095 — Sửa kết quả chấm thành công

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.ChamBaiAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.ChamBaiAsync(...). Code đang có ChamAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.ChamBaiAsync(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.ChamBaiAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi ChamAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:110 — `public async Task ChamAsync(ChamBaiViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Sửa kết quả chấm thành công”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.ChamBaiAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-095 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.ChamBaiAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: ChamAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_095_ThietKe_ChamBaiAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 68

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-096 — Sửa kết quả chấm sai quyền

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.ChamBaiAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.ChamBaiAsync(...). Code đang có ChamAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.ChamBaiAsync(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.ChamBaiAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi ChamAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:110 — `public async Task ChamAsync(ChamBaiViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Sửa kết quả chấm sai quyền”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.ChamBaiAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-096 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.ChamBaiAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: ChamAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_096_ThietKe_ChamBaiAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 73

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-097 — Điểm ngoài thang

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có ChamDiemViewModel đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy ChamDiemViewModel. Code đang có ChamBaiViewModel; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: ChamDiemViewModel validation.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên ChamDiemViewModel. Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi ChamBaiViewModel thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/ViewModels/HocTapInputs.cs:74 — `public sealed class ChamBaiViewModel`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Điểm ngoài thang”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ ChamDiemViewModel và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-097 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: có model public ChamDiemViewModel để kiểm validation.
Thực tế: không tìm thấy model ChamDiemViewModel; code hiện có ChamBaiViewModel.
Không thay model khác tên để báo Pass cho model trong thiết kế.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireModel(String id, Assembly assembly, String name, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 19

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_097_ThietKe_ChamDiemViewModel_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 78

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-098 — Thiếu nhận xét bắt buộc

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có ChamDiemViewModel đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy ChamDiemViewModel. Code đang có ChamBaiViewModel; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: ChamDiemViewModel validation.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên ChamDiemViewModel. Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi ChamBaiViewModel thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/ViewModels/HocTapInputs.cs:74 — `public sealed class ChamBaiViewModel`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Thiếu nhận xét bắt buộc”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ ChamDiemViewModel và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-098 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: có model public ChamDiemViewModel để kiểm validation.
Thực tế: không tìm thấy model ChamDiemViewModel; code hiện có ChamBaiViewModel.
Không thay model khác tên để báo Pass cho model trong thiết kế.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireModel(String id, Assembly assembly, String name, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 19

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_098_ThietKe_ChamDiemViewModel_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 83

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-099 — Sửa điểm với RV cũ

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.ChamBaiAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.ChamBaiAsync(...). Code đang có ChamAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.ChamBaiAsync(m).

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.ChamBaiAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi ChamAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:110 — `public async Task ChamAsync(ChamBaiViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Sửa điểm với RV cũ”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.ChamBaiAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-099 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.ChamBaiAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: ChamAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_099_ThietKe_ChamBaiAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 88

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-100 — Lỗi lưu bài tập

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.GiaoBaiTapAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.GiaoBaiTapAsync(...). Code đang có GiaoAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.GiaoBaiTapAsync.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.GiaoBaiTapAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi GiaoAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:36 — `public async Task GiaoAsync(GiaoBaiTapViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Lỗi lưu bài tập”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.GiaoBaiTapAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-100 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.GiaoBaiTapAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: GiaoAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_100_ThietKe_GiaoBaiTapAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 93

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-101 — Lỗi phát thông báo giữ bài đã lưu

**Kết quả: Pass.** Loại kiểm tra: unit hành vi.

**Mong đợi:** Khi phát một thông báo thất bại, handler phải yêu cầu thử lại cùng sự kiện, không đánh dấu hoàn tất trước khi phát thành công.

**Thực tế:** Lần phát lỗi gọi retry một lần, không complete; lần thử lại dùng cùng event ID và complete sau khi publish thành công.

**Các bước kiểm thử đã thực hiện:**

1. Tạo sự kiện đã sẵn sàng xử lý; mock callback publish ném IOException, callback retry/complete ghi nhận số lần gọi.
2. Gọi OutboxDelivery.RunAsync thật. Assert retry=1, complete=0, đúng lỗi được ghi nhận.
3. Đổi publish thành công và chạy lại cùng sự kiện; assert cùng ID, publish=1, complete=1. Không giả lập một bài tập trong bộ nhớ rồi coi là đã kiểm chứng lưu CSDL.

### UT-102 — Đủ người nhận có quyền

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.GiaoBaiTapAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.GiaoBaiTapAsync(...). Code đang có GiaoAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Ca này thuộc luồng giao bài ở UT-084; kiểm phương thức thiết kế của chính luồng đó trước khi kiểm tác dụng phụ.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.GiaoBaiTapAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi GiaoAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:36 — `public async Task GiaoAsync(GiaoBaiTapViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Đủ người nhận có quyền”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.GiaoBaiTapAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-102 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.GiaoBaiTapAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: GiaoAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_102_ThietKe_GiaoBaiTapAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 98

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-103 — Chưa có bài nộp

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.ChamBaiAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.ChamBaiAsync(...). Code đang có ChamAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Tên đơn vị ghi trong Unit Test.xlsx: BaiTapService.ChamBaiAsync.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.ChamBaiAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi ChamAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:110 — `public async Task ChamAsync(ChamBaiViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Chưa có bài nộp”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.ChamBaiAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-103 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.ChamBaiAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: ChamAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_103_ThietKe_ChamBaiAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 103

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-104 — Đủ cập nhật tiến độ và người nhận

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.ChamBaiAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.ChamBaiAsync(...). Code đang có ChamAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Ca này thuộc luồng chấm bài ở UT-093/103; kiểm phương thức thiết kế của chính luồng đó trước khi kiểm tác dụng phụ.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.ChamBaiAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi ChamAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:110 — `public async Task ChamAsync(ChamBaiViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Đủ cập nhật tiến độ và người nhận”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.ChamBaiAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-104 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.ChamBaiAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: ChamAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_104_ThietKe_ChamBaiAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 108

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

### UT-105 — Quyền bị thu hồi trong lúc quét

**Kết quả: Fail.** Loại kiểm tra: đối chiếu thiết kế.

**Mong đợi:** Code phải có BaiTapService.NopBaiAsync(...) đúng tên/overload/model trước khi chạy nhánh hành vi của ca này.

**Thực tế:** Không tìm thấy BaiTapService.NopBaiAsync(...). Code đang có NopAsync; đây không phải tên đơn vị trong thiết kế.

**Căn cứ xác định đơn vị:** Ca này thuộc luồng nộp bài ở UT-089; kiểm phương thức thiết kế của chính luồng đó trước khi kiểm tác dụng phụ.

**Các bước kiểm thử đã thực hiện:**

1. Nạp assembly ứng dụng đã build và xác định lớp/model cần kiểm theo bảng thiết kế.
2. Liệt kê các khai báo public, tìm đúng tên BaiTapService.NopBaiAsync(...). Nếu là overload, kiểm đúng kiểu tham số đầu vào đã xác định.
3. Assert đơn vị được tìm thấy. Thực tế không tìm thấy nên test Fail tại bước này; không gọi NopAsync thay thế.

**Vị trí code cần kiểm tra:** Areas/HocTap/Services/BaiTapService.cs:71 — `public async Task NopAsync(NopBaiViewModel m)`

**Nguyên nhân:** Khai báo trong code không khớp tên/overload/model được yêu cầu; test cũ đã tự thay tên nên bỏ qua sai lệch này.

**Hành vi/thiết kế bị ảnh hưởng:** Không có đúng đơn vị để thực hiện tình huống “Quyền bị thu hồi trong lúc quét”. Đây là Fail đối chiếu thiết kế; không phải sự cố môi trường test và không dùng Blocked.

**Yêu cầu sửa:** Triển khai/đồng bộ BaiTapService.NopBaiAsync(...) và các lời gọi liên quan đúng thiết kế; sau đó viết/chạy phần kiểm hành vi của ca. Không thêm hàm rỗng hoặc đổi kỳ vọng sang tên hiện có chỉ để test Pass.

**Phạm vi kết luận:** Fail vì không có đúng đơn vị thiết kế. Chưa kết luận nhánh dữ liệu bên trong hàm có lỗi gì, vì chưa có hàm đúng tên/overload để chạy nhánh đó.

**Minh chứng test:**

```text
UT-105 — FAIL ĐỐI CHIẾU THIẾT KẾ.
Mong đợi: code khai báo public BaiTapService.NopBaiAsync(...).
Thực tế: không tìm thấy phương thức/overload này.
Code hiện có: NopAsync.
Không tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.
   at EduConnect.HocTap.UnitTests.HocTapDesignContract.RequireMethod(String id, Type type, String name, Type firstParameter, String actualCounterpart) in C:\Khóa luận TN\Educonnect\tests\Unit\HocTapDesignContract.cs:line 14

   at EduConnect.HocTap.UnitTests.BaiTapServiceTest.UT_105_ThietKe_NopBaiAsync_PhaiCoTrongCode() in C:\Khóa luận TN\Educonnect\tests\Unit\BaiTapServiceTest.cs:line 113

   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)

   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
```

## Những kết luận trước đã được sửa

- Đã bỏ các Skip của UT-081/087/088/091: thiếu phương thức thiết kế được kiểm và ghi Fail.
- Đã thu hồi những Pass dựa trên việc gọi phương thức khác tên/model khác tên. Không dùng kết quả của GiaoAsync để xác nhận GiaoBaiTapAsync hoặc ChamBaiViewModel để xác nhận ChamDiemViewModel.
- UT-068 đã thêm assertion actor/token mà test trước bỏ sót; vì vậy ca này chuyển từ Pass sang Fail.
- Đã bỏ việc tạo một object bài tập giả trong test sự kiện rồi mô tả đó là kiểm chứng bài đã lưu; UT-101 hiện chỉ kiểm handler phát/thử lại đúng phạm vi unit.
- Báo cáo trước nêu SQL lẫn trong phần kết quả TC đã gây hiểu nhầm. Các báo cáo hiện tại mô tả code C#, mock và assertion; không lấy việc mock báo thành công làm bằng chứng dữ liệu đã lưu thật.

## Chạy lại

Tại thư mục Educonnect trong VS Code:

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --no-restore -p:UseAppHost=false "-p:BaseOutputPath=C:/Users/NGUYEN THI YEN/Documents/Codex/2026-11-13/un/work/hoctap-bin/" --filter "Suite=BaiTap" --results-directory "C:/Users/NGUYEN THI YEN/Documents/Codex/2026-11-13/un/outputs/05-BaiTap-Nop-Cham-Unit" --logger "trx;LogFileName=BaiTap-Nop-Cham-Unit.trx"
```

TestKind=DesignContract là kiểm đúng đơn vị khai báo theo thiết kế. Bộ này chưa tuyên bố rà mọi hàm phụ trợ trong toàn hệ thống; phạm vi là 41 ca được yêu cầu.

## Minh chứng hiện hành

- [TRX đã thay thế](BaiTap-Nop-Cham-Unit.trx)
- [Kết quả máy đọc được](KetQua.json)
- [Mã nguồn test và các thay đổi hỗ trợ](Ma_nguon_Unit.zip)
