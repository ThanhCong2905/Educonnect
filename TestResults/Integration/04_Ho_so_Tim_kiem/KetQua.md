# Hồ sơ gia sư và tìm kiếm

Thời điểm: 27/11/2026 13:01:18 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Fail': 5, 'Chưa đủ phạm vi TC': 3, 'Pass': 8}. Kết quả script xUnit: {'Fail': 5, 'Pass': 11}.

**Script Pass chưa đủ phạm vi TC không được tính là TC Pass hoàn chỉnh. Lượt này không có ca Skip; 8 mã thiếu dispatcher được gọi và Fail trước khi chạy scenario nghiệp vụ.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-028 | Service lưu hồ sơ, môn và minh chứng trong cùng giao dịch | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-029 | Service tìm kiếm ghép hồ sơ, môn, phí và đánh giá | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-030 | Gợi ý có lý do, không tự tạo kết nối | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-031 | Multipart hồ sơ giữ đúng liên kết minh chứng | Chưa đủ phạm vi TC | Pass | Service/SQL/file path is tested, but POST multipart through HoSoGiaSuController and binding/CSRF/response are not. |
| IT-032 | Thiếu minh chứng không lưu một phần hồ sơ | Chưa đủ phạm vi TC | Pass | Service-level missing-proof rejection and no partial SQL save are tested; HTTP form and ModelState response are not. |
| IT-033 | Hồ sơ phiên bản cũ không xóa minh chứng mới | Chưa đủ phạm vi TC | Pass | Service-level stale RowVersion protection is tested; the HTTP route/status/body are not. |
| IT-034 | Duyệt hồ sơ làm thay đổi điều kiện công khai | Fail | Fail | Admin service review and guest search are tested, but the admin review Controller route/response are not. |
| IT-035 | Sửa nghề nghiệp/họ tên khi công khai phải theo điều kiện ẩn và duyệt lại | Fail | Fail | Both workflow branches are tested at HoSoGiaSuService/SQL level; the TrangThai/Luu HTTP posts are not. |
| IT-036 | Quyền xem trước và tải minh chứng | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-037 | Khách xem hồ sơ và nhu cầu công khai | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-038 | Lọc nhiều tiêu chí với mức phí đúng môn và đơn vị | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-039 | Hồ sơ và nhu cầu bị ẩn/khóa không lọt kết quả | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-040 | Phân trang và sắp xếp ổn định trên dữ liệu thật | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-041 | Bộ lọc sai kiểu không bị bỏ qua | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-042 | So sánh đủ điều kiện và loại Id trùng | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-043 | So sánh sai số lượng, loại hoặc điều kiện công khai | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |

## IT-028 — Service lưu hồ sơ, môn và minh chứng trong cùng giao dịch

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Service hoàn tất; giữ liên kết bằng chứng theo đúng từng mục.
2. Dữ liệu và side effect: Một HoSoGiaSu, hai môn; minh chứng trình độ/môn có FK metadata/blob thật; Chưa có kinh nghiệm không yêu cầu minh chứng kinh nghiệm.

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
   at EduConnect.Mvc.Areas.NhuCau.Services.HoSoGiaSuService.GiaSuAsync() in C:\Khóa luận TN\EduConnect1\Areas\NhuCau\Services\HoSoGiaSuService.cs:line 29
   at EduConnect.Mvc.Areas.NhuCau.Services.HoSoGiaSuService.CapNhatAsync(HoSoInput m) in C:\Khóa luận TN\EduConnect1\Areas\NhuCau\Services\HoSoGiaSuService.cs:line 52
   at Cases.SaveProfileFixture(Fixture f, Boolean submit) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProfileCases.cs:line 61
   at Cases.SaveProfileFixture(Fixture f, Boolean submit) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProfileCases.cs:line 73
   at Cases.ProfileCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProfileCases.cs:line 174
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 339
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_028() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 30
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-028-ITc9dbde2905294dc99ff10fb4060cc729.json`

## IT-029 — Service tìm kiếm ghép hồ sơ, môn, phí và đánh giá

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: KetQua thành công; chỉ G1, mỗi gia sư một mục; Total và Items thống nhất.
2. Dữ liệu và side effect: Không kết quả từ hồ sơ chưa duyệt/ẩn hoặc phí môn/đơn vị không tương ứng; không ghi dữ liệu.

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
   at EduConnect.Mvc.Infrastructure.NguoiDungHienTaiService.NguoiXemAsync(Nullable`1 id, CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Infrastructure\NguoiDungHienTaiService.cs:line 17
   at EduConnect.Mvc.Areas.TimKiem.Services.TimKiemService.<>c__DisplayClass10_0.<<TimKiemAsync>b__0>d.MoveNext() in C:\Khóa luận TN\EduConnect1\Areas\TimKiem\Services\TimKiemService.cs:line 93
--- End of stack trace from previous location ---
   at EduConnect.Mvc.Infrastructure.KetQua`1.ThucHienAsync(Func`1 action) in C:\Khóa luận TN\EduConnect1\Infrastructure\KetQua.cs:line 30
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 20
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 30
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 371
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_029() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 31
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-029-IT8548dfbc65454f6f9b939259f1c0fd37.json`

## IT-030 — Gợi ý có lý do, không tự tạo kết nối

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: H1 ưu tiên đúng mức tiêu chí và có lý do, không phần trăm; G1 chỉ nhu cầu liên quan môn; H2 nhận không có nhu cầu để gợi ý.
2. Dữ liệu và side effect: Không tự tạo YeuCauLienHe/YeuCauKetNoiLopHoc/LopHoc.

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
   at EduConnect.Mvc.Areas.TimKiem.Services.TimKiemService.GoiYAsync() in C:\Khóa luận TN\EduConnect1\Areas\TimKiem\Services\TimKiemService.cs:line 407
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 37
   at Cases.SearchCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SearchCases.cs:line 43
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 371
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_030() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 32
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-030-IT866f0f7d7cb845cb94759a6ad2897742.json`

## IT-034 — Duyệt hồ sơ làm thay đổi điều kiện công khai

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Duyệt 200 ok=true; GET tìm kiếm 200 thấy G1 đúng một lần.
2. Dữ liệu và side effect: Trạng thái DaDuyet, reviewer/thời điểm/lịch sử/thông báo đúng; không công khai chứng minh thư/địa chỉ chi tiết.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   InternalServerError
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.ProfileCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProfileCases.cs:line 214
   at Cases.ProfileCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProfileCases.cs:line 216
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 339
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_034() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 36
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-034-IT33d05d3bd0644c9cb673a8d5dad64f7f.json`

## IT-035 — Sửa nghề nghiệp/họ tên khi công khai phải theo điều kiện ẩn và duyệt lại

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01 bị từ chối theo điều kiện hồ sơ; DL02 lưu được nhưng không công khai trước khi duyệt lại.
2. Dữ liệu và side effect: Không dùng trạng thái cũ để vượt duyệt; lịch sử và minh chứng được giữ; tìm kiếm loại hồ sơ đang ẩn.

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
   at IntegrationTests.IT_035() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 37
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-035-ITf6eb0d62f9e84a27af3203605fb0e107.json`

## Giới hạn của lượt thực thi

Đã chạy toàn bộ danh mục 191 mã: 118 Pass, 73 Fail, 0 Skip. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. 8 ca trước đây pending hiện chạy và Fail do dispatcher chưa có implementation. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
