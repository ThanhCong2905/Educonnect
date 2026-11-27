# Nhu cầu học

Thời điểm: 27/11/2026 13:01:18 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Fail': 9, 'Pass': 4, 'Chưa đủ phạm vi TC': 1}. Kết quả script xUnit: {'Fail': 9, 'Pass': 5}.

**Script Pass chưa đủ phạm vi TC không được tính là TC Pass hoàn chỉnh. Lượt này không có ca Skip; 8 mã thiếu dispatcher được gọi và Fail trước khi chạy scenario nghiệp vụ.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-014 | Service tạo nhu cầu nhiều môn và đọc lại | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-015 | Lỗi quan hệ môn không được lưu một phần nhu cầu | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-016 | Hủy request khi chờ khóa SQL không để ghi một phần | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-017 | Binding form tạo nhu cầu từ HTTP | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-018 | Lưu nháp thiếu mục tiêu và lịch | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-019 | Phụ huynh đứng tên nhu cầu của người học | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-020 | Khách hoặc gia sư không tạo nhu cầu học | Fail | Fail | Authorize(Roles=HocSinh,PhuHuynh) ở Controller chặn Gia sư bằng403 trước action; Excel lại yêu cầu422. Không có bằng chứng vượt quyền. |
| IT-021 | Hạn mức nhu cầu mở, nháp và lượt đăng/ngày | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-022 | Chuẩn hóa trùng nhu cầu trước khi ghi | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-023 | Cập nhật thay môn/lịch, không đẩy như bài mới | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-024 | Sai chủ hoặc phiên bản cũ không ghi đè | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-025 | Validation form, địa giới và học phí đi qua đúng khớp nối | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-026 | Đóng/mở lại và xác nhận còn nhu cầu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-027 | Form ghi thiếu CSRF không gọi nghiệp vụ | Chưa đủ phạm vi TC | Pass | Đã kiểm CSRF400 và không ghi nhu cầu; chưa assertion audit CSRF. |

## IT-014 — Service tạo nhu cầu nhiều môn và đọc lại

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: KetQua.ThanhCong=true; DuLieu=N1 khác Guid.Empty.
2. Dữ liệu và side effect: Một NhuCauHoc, hai NhuCauMonHoc, hai KhungGioRanh, một LuotDangNhuCau; chủ H1; không nhân bản nhu cầu theo môn.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.InitializeReaderAsync(AsyncEnumerator enumerator, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.MoveNextAsync()
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.HienTaiAsync(CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 27
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.KiemTraNguoiThucHienAsync(Nullable`1 id, CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 33
   at EduConnect.Mvc.Areas.NhuCau.Services.NhuCauService.<>c__DisplayClass7_0.<<TaoNhuCauHocAsync>b__0>d.MoveNext() in C:\Khóa luận TN\EduConnect1\Areas\NhuCau\Services\NhuCauService.cs:line 38
--- End of stack trace from previous location ---
   at EduConnect.Mvc.Infrastructure.KetQua`1.ThucHienAsync(Func`1 action) in C:\Khóa luận TN\EduConnect1\Infrastructure\KetQua.cs:line 30
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 18
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 20
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_014() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 16
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-014-ITed4c32bbb1f74d8bb4545f8b6110adba.json`

## IT-015 — Lỗi quan hệ môn không được lưu một phần nhu cầu

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: KetQua.ThanhCong=false, lỗi môn; không trả Guid nhu cầu được xem như tạo thành công.
2. Dữ liệu và side effect: Không còn dòng cha, link môn, lịch hoặc LuotDang của lần ghi lỗi.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.InitializeReaderAsync(AsyncEnumerator enumerator, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.MoveNextAsync()
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.HienTaiAsync(CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 27
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.KiemTraNguoiThucHienAsync(Nullable`1 id, CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 33
   at EduConnect.Mvc.Areas.NhuCau.Services.NhuCauService.<>c__DisplayClass7_0.<<TaoNhuCauHocAsync>b__0>d.MoveNext() in C:\Khóa luận TN\EduConnect1\Areas\NhuCau\Services\NhuCauService.cs:line 38
--- End of stack trace from previous location ---
   at EduConnect.Mvc.Infrastructure.KetQua`1.ThucHienAsync(Func`1 action) in C:\Khóa luận TN\EduConnect1\Infrastructure\KetQua.cs:line 30
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 18
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 20
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_015() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 17
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-015-IT1906c53efcbe44778ddea9ed83d0b48a.json`

## IT-016 — Hủy request khi chờ khóa SQL không để ghi một phần

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Call bị hủy có kiểm soát trước commit, không trả KetQua thành công hoặc rơi vào tiếp tục ghi sau cancel.
2. Dữ liệu và side effect: Không nhu cầu/lịch/link/lượt đăng của B; khóa của A được giải phóng; token được truyền qua khớp Service→EF→SqlClient.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, SqlCommand command, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteReaderInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteReaderAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 194
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_016() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 18
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-016-ITdb8f3f33d80e4d0689991c986a7f8682.json`

## IT-017 — Binding form tạo nhu cầu từ HTTP

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: HTTP 200 JSON ok=true, redirectUrl về danh sách nhu cầu; thông báo tạo thành công.
2. Dữ liệu và side effect: Dữ liệu chép đúng tất cả môn/lịch/mục tiêu/phí; đúng một nhu cầu và lượt đăng.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   UnprocessableEntity
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 25
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_017() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 19
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-017-ITccaab9f4c9e14c90b1f0b748e40417d5.json`

## IT-018 — Lưu nháp thiếu mục tiêu và lịch

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200 ok=true; được lưu nháp và đọc lại dữ liệu đã nhập.
2. Dữ liệu và side effect: TrangThai=BanNhap; không tự sinh lịch 00:00–00:00; không tăng lượt đăng bài công khai.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   UnprocessableEntity
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 25
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_018() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 20
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-018-ITe6be628237bd4c0d876896b02892a71a.json`

## IT-019 — Phụ huynh đứng tên nhu cầu của người học

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200 ok=true; biểu mẫu hiển thị người học đã mô tả.
2. Dữ liệu và side effect: ChuSoHuuId=P1; giữ tên/tuổi/lớp; không tự tạo tài khoản Học sinh hoặc gộp nhu cầu của H1.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   UnprocessableEntity
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 25
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_019() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 21
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-019-ITd6d41701ba1d40ccb8d5eeef6ad75f2f.json`

## IT-020 — Khách hoặc gia sư không tạo nhu cầu học

Phân loại: Kỳ vọng TC chưa đúng tầng từ chối.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01 HTTP 401; DL02 từ chối quyền (422 qua ModelState của action lưu), ok=false.
2. Dữ liệu và side effect: Không tạo NhuCauHoc/MonHoc/lịch/lượt đăng của bất kỳ DL bị từ chối.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: UnprocessableEntity
Actual:   Forbidden
```

**Ý nghĩa:** Authorize(Roles=HocSinh,PhuHuynh) ở Controller chặn Gia sư bằng403 trước action; Excel lại yêu cầu422. Không có bằng chứng vượt quyền.

Vị trí cần xem trong ứng dụng: `Areas/NhuCau/Controllers/NhuCauController.cs:11`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 27
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_020() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 22
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-020-ITac27b0a62ee24469ad8a34bec690017a.json`

## IT-022 — Chuẩn hóa trùng nhu cầu trước khi ghi

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Cùng chủ: 422 báo cập nhật bài đã có. Khác chủ: 200, không gộp hai nhu cầu độc lập.
2. Dữ liệu và side effect: Cùng chủ vẫn một nhu cầu; khác chủ có hai nhu cầu riêng đúng ownership.

**Quan sát/assertion thực tế:**
```text
Mỗi nhu cầu chỉ chọn một môn trong danh sách.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 39
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 39
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_022() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 24
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-022-ITbd7267801104479fa40b25204d6a633b.json`

## IT-023 — Cập nhật thay môn/lịch, không đẩy như bài mới

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200 ok=true, thông báo cập nhật.
2. Dữ liệu và side effect: Giữ Id/TaoLuc/chủ, RV đổi; link/lịch cũ được thay đồng bộ; không cộng lượt đăng mới.

**Quan sát/assertion thực tế:**
```text
Mỗi nhu cầu chỉ chọn một môn trong danh sách.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 31
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 31
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_023() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 25
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-023-ITfab28a4923b9458284a78c831d6e5d51.json`

## Giới hạn của lượt thực thi

Đã chạy toàn bộ danh mục 191 mã: 118 Pass, 73 Fail, 0 Skip. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. 8 ca trước đây pending hiện chạy và Fail do dispatcher chưa có implementation. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
