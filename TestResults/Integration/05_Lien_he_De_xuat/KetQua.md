# Liên hệ và đề xuất hai chiều

Thời điểm: 27/11/2026 13:01:18 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Fail': 6, 'Pass': 7}. Kết quả script xUnit: {'Fail': 6, 'Pass': 7}.

**Script Pass chưa đủ phạm vi TC không được tính là TC Pass hoàn chỉnh. Lượt này không có ca Skip; 8 mã thiếu dispatcher được gọi và Fail trước khi chạy scenario nghiệp vụ.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-044 | Service gửi/chấp nhận liên hệ ghép hội thoại và thông báo | Fail | Fail | Đã kiểm liên hệ, hội thoại, notification; chưa kiểm đầy đủ payload/outbox chống nhân bản. |
| IT-045 | Service gửi đề xuất hai chiều và kiểm vai trò bên nhận | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-046 | Hai gửi liên hệ đồng thời không nhân bản quan hệ | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-047 | Gửi liên hệ qua HTTP giữ danh tính từ phiên | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-048 | Chặn liên hệ không hợp lệ trước và trong giao dịch | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-049 | Phản hồi liên hệ đúng bên và phiên bản | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-050 | Chưa chấp nhận liên hệ không nhắn tin hoặc đề xuất | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-051 | Form đề xuất đầy đủ từ cả hai phía | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-052 | Chỉ bên gửi được sửa đề xuất đang chờ | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-053 | Thiếu dữ liệu lớp hoặc mảng lịch bị loại | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-054 | Gửi/chấp nhận kiểm lại môn và nhu cầu bị ẩn | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-055 | Từ chối và rút theo đúng bên tham gia | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-056 | Điểm vào chuyển tiếp không bỏ kiểm tra actor và liên hệ | Fail | Fail | Người ngoài nhận404, Excel yêu cầu403; không chứng minh người ngoài tạo được yêu cầu. Đối chiếu quy tắc che sự tồn tại của liên hệ trước khi kết luận lỗi nghiệp vụ. |

## IT-044 — Service gửi/chấp nhận liên hệ ghép hội thoại và thông báo

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Service hoàn tất; liên hệ từ ChoPhanHoi sang ChapNhan.
2. Dữ liệu và side effect: Đúng một liên hệ/hội thoại riêng cho cặp H1/G1; thành viên hội thoại đúng hai người; thông báo/outbox đúng người nhận; chưa có lớp.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteReaderAsync(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 194
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_044() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 46
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-044-IT012add0eab2a42128880cb616d888681.json`

## IT-045 — Service gửi đề xuất hai chiều và kiểm vai trò bên nhận

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: KetQua thành công; G1 gửi thì chờ Bên học, H1 gửi thì chờ Gia sư; nhận đúng bên còn lại.
2. Dữ liệu và side effect: Một đề xuất/DL, đầy đủ KhungGioDeXuat; mốc hết hạn 72 giờ; phí/mục tiêu/địa điểm khớp đầu vào; chưa tạo lớp.

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
   at EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService.GuiDeXuatAsync(DeXuatLopViewModel m, CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Areas\KetNoi\Services\KetNoiService.cs:line 184
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 18
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 18
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 18
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 242
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_045() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 47
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-045-DL0-IT61d7dea4ba434f84b0a785f33f2f3162.json`, `Evidence\IT-045-ITfc60cb8d6ba0480da1781443953bf4e0.json`

## IT-046 — Hai gửi liên hệ đồng thời không nhân bản quan hệ

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Một lần tạo được; lần còn lại nhận từ chối trùng quan hệ có kiểm soát; không exception infrastructure/500.
2. Dữ liệu và side effect: Chỉ một liên hệ đang chờ của cặp, không hai hội thoại/thông báo trùng.

**Quan sát/assertion thực tế:**
```text
Assert.Contains() Failure: Filter not matched in collection
Collection: [Microsoft.Data.SqlClient.SqlException (0x80131904): Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
 ---> System.ComponentModel.Win32Exception (258): The wait operation timed out.
   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, SqlCommand command, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.SqlCommand.CheckThrowSNIException()
   at Microsoft.Data.SqlClient.SqlCommand.InternalEndExecuteNonQuery(IAsyncResult asyncResult, Boolean isInternal, String endMethod)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryInternal(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.EndExecuteNonQueryAsync(IAsyncResult asyncResult)
   at Microsoft.Data.SqlClient.SqlCommand.<>c.<InternalExecuteNonQueryAsync>b__193_1(IAsyncResult asyncResult)
   at System.Threading.Tasks.TaskFactory`1.FromAsyncCoreLogic(IAsyncResult iar, Func`2 endFunction, Action`1 endAction, Task`1 promise, Boolean requiresSynchronization)
--- End of stack trace from previous location ---
   at AppPrincipal.ConnectionOpenedAsync(DbConnection c, ConnectionEndEventData e, CancellationToken ct) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 36
   at AppPrincipal.ConnectionOpenedAsync(DbConnection c, ConnectionEndEventData e, CancellationToken ct) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 36
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenInternalAsync(Boolean errorsExpected, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenInternalAsync(Boolean errorsExpected, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenAsync(CancellationToken cancellationToken, Boolean errorsExpected)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.InitializeReaderAsync(AsyncEnumerator enumerator, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.MoveNextAsync()
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.ShapedQueryCompilingExpressionVisitor.SingleOrDefaultAsync[TSource](IAsyncEnumerable`1 asyncEnumerable, CancellationToken cancellationToken)
   at EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService.GuiLienHeAsync(GuiLienHeInput m) in C:\Khóa luận TN\EduConnect1\Areas\KetNoi\Services\KetNoiService.cs:line 60
   at Cases.Capture(Func`1 f) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 67
ClientConnectionId:e2dc02d3-4d4c-4bad-924e-a3ccc33fb754
Error Number:-2,State:0,Class:11, Microsoft.Data.SqlClient.SqlException (0x80131904): Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
 ---> System.ComponentModel.Win32Exception (258): The wait operation timed out.
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
   at EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService.GuiLienHeAsync(GuiLienHeInput m) in C:\Khóa luận TN\EduConnect1\Areas\KetNoi\Services\KetNoiService.cs:line 59
   at Cases.Capture(Func`1 f) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 67
ClientConnectionId:d33a37cc-829a-4302-b2b9-a3e16388b9ea
Error Number:-2,State:0,Class:11]
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 20
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 20
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 20
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 242
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_046() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 48
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-046-IT02e657b67dee4bc59212fdf6038cc6ff.json`

## IT-051 — Form đề xuất đầy đủ từ cả hai phía

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200 ok=true; redirect về danh sách kết nối; điều kiện hiển thị đúng phía nhận.
2. Dữ liệu và side effect: Đầy đủ đề xuất/lịch/hạn72h; lịch gửi không bị binder thay bằng slot mặc định; không tự chấp nhận.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   UnprocessableEntity
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 29
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 29
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 261
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_051() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 53
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-051-G-IT64b1220b2bfd48808b97cedcc00bde6d.json`, `Evidence\IT-051-ITe71dc49ba2164fe4aee7f3d70ee5b5d6.json`

## IT-052 — Chỉ bên gửi được sửa đề xuất đang chờ

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01 200; DL02/03 422 ok=false, lý do quyền/phiên bản.
2. Dữ liệu và side effect: DL01 thay lịch đồng bộ và mốc chờ mới theo thiết kế; DL02/03 giữ phiên bản mới, không xóa lịch.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: 200
Actual:   422
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 31
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 31
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 261
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_052() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 54
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-052-DL0-IT7eae429386ab4de4ab3c838e3989546c.json`, `Evidence\IT-052-IT2b09094ae04d4813af83261175292c99.json`

## IT-056 — Điểm vào chuyển tiếp không bỏ kiểm tra actor và liên hệ

Phân loại: Sai lệch mã lỗi quyền cần thống nhất.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: H1 hợp lệ: HTTP200 ok=true, tạo yêu cầu ChoGiaSu để Gia sư bổ sung đề xuất; X bị403; không tạo lớp ngay từ yêu cầu sơ bộ.
2. Dữ liệu và side effect: Không bỏ qua liên hệ/ownership, không tạo lớp hoặc cấp quyền chat cho X.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: 403
Actual:   404
```

**Ý nghĩa:** Người ngoài nhận404, Excel yêu cầu403; không chứng minh người ngoài tạo được yêu cầu. Đối chiếu quy tắc che sự tồn tại của liên hệ trước khi kết luận lỗi nghiệp vụ.

Vị trí cần xem trong ứng dụng: `Areas/KetNoi/Services/KetNoiService.cs:113`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 37
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 261
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_056() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 58
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-056-ITb5f0829b43024c0ba63574eec704bb61.json`

## Giới hạn của lượt thực thi

Đã chạy toàn bộ danh mục 191 mã: 118 Pass, 73 Fail, 0 Skip. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. 8 ca trước đây pending hiện chạy và Fail do dispatcher chưa có implementation. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
