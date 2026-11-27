# Liên kết và thành viên lớp

Thời điểm: 27/11/2026 13:01:18 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Fail': 8, 'Pass': 4, 'Chưa đủ phạm vi TC': 1}. Kết quả script xUnit: {'Fail': 8, 'Pass': 5}.

**Script Pass chưa đủ phạm vi TC không được tính là TC Pass hoàn chỉnh. Lượt này không có ca Skip; 8 mã thiếu dispatcher được gọi và Fail trước khi chạy scenario nghiệp vụ.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-070 | Service đề nghị/xác nhận liên kết không tự cấp quyền lớp | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-071 | Service lời mời → đồng ý → duyệt ghi đủ quyền | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-072 | Liên kết phụ huynh–học sinh qua HTTP | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-073 | Không nhận liên kết sai vai trò, sai người hoặc RV cũ | Fail | Fail | Script mới có DL01; chưa có sai người/RV cũ. |
| IT-074 | Đề nghị thành viên tại action DeNghi thực tế | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-075 | Người nhận tự đồng ý hoặc từ chối lời mời | Fail | Fail | Đã kiểm HTTP/state ba DL; chưa assert riêng quyền mở lớp sau từng DL. |
| IT-076 | Gia sư duyệt Xem/Chỉnh sửa và quyền nộp riêng | Fail | Fail | Đã kiểm ba cấu hình quyền lưu SQL; chưa thao tác đọc/chat/nộp cho cả ba cấu hình. |
| IT-077 | Duyệt lại điều kiện liên kết và hạn mức trong transaction | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-078 | Bên học/thành viên không duyệt quyền người khác | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-079 | Đổi/thu hồi quyền có hiệu lực ở request kế tiếp | Fail | Fail | Đã kiểm cookie cũ không chat/nộp; chưa kiểm GET đọc còn được phép. |
| IT-080 | Gỡ/rời thành viên không gỡ hai bên chính | Fail | Fail | Đã có DL gỡ G1/H1/P1 đối với P1; chưa DL bảo vệ hai thành viên chính. |
| IT-081 | Hủy liên kết thu hồi quyền thành viên phụ thuộc | Chưa đủ phạm vi TC | Pass | Service cancellation and rights revocation are tested; HTTP route/cookie/CSRF are not. |
| IT-082 | Biết Id/là người nhà không mở lớp khác hoặc chat riêng | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |

## IT-070 — Service đề nghị/xác nhận liên kết không tự cấp quyền lớp

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Liên kết từ chờ sang DaLienKet, đúng P1/H1; không gọi hoặc giả kết quả Service.
2. Dữ liệu và side effect: Chưa có ThanhVienLopHoc hoặc quyền chat lớp chỉ vì liên kết tài khoản đã xác nhận.

**Quan sát/assertion thực tế:**
```text
Microsoft.EntityFrameworkCore.DbUpdateException : An error occurred while saving the entity changes. See the inner exception for details.
---- Microsoft.Data.SqlClient.SqlException : Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
-------- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Update.Internal.SqlServerModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.SaveChangesAsync(IList`1 entries, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(IList`1 entriesToSave, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(StateManager stateManager, Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Fixture.SeedProposal() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 56
   at Fixture.SeedProposal() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 56
   at Fixture.SeedClass() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 58
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 18
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 275
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_070() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 72
--- End of stack trace from previous location ---
----- Inner Stack Trace -----
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-070-IT78e7280ccfa34ab39a540d5231f21076.json`

## IT-071 — Service lời mời → đồng ý → duyệt ghi đủ quyền

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Trước đồng ý/duyệt chưa có quyền; sau duyệt đủ quyền Xem đúng C1.
2. Dữ liệu và side effect: Một thành viên; thời điểm đồng ý/duyệt và quyền được lưu, notice đúng người; không cấp chat riêng H1/G1.

**Quan sát/assertion thực tế:**
```text
Microsoft.EntityFrameworkCore.DbUpdateException : An error occurred while saving the entity changes. See the inner exception for details.
---- Microsoft.Data.SqlClient.SqlException : Invalid object name 'LichSuQuyenThanhVien'.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Update.Internal.SqlServerModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.SaveChangesAsync(IList`1 entries, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(IList`1 entriesToSave, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(StateManager stateManager, Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
   at EduConnect.Mvc.Areas.HocTap.Services.LopHocService.DuyetAsync(Guid member, String action, String permission, String version, Boolean choPhepNopBai) in C:\Khóa luận TN\EduConnect1\Areas\HocTap\Services\LopHocService.cs:line 195
   at EduConnect.Mvc.Areas.HocTap.Services.LopHocService.DuyetAsync(Guid member, String action, String permission, String version, Boolean choPhepNopBai) in C:\Khóa luận TN\EduConnect1\Areas\HocTap\Services\LopHocService.cs:line 196
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 20
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 20
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 20
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 275
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_071() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 73
--- End of stack trace from previous location ---
----- Inner Stack Trace -----
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
```

Bằng chứng: `Evidence\IT-071-ITa24e9eb510424287a4103e86c02b5ac5.json`

## IT-072 — Liên kết phụ huynh–học sinh qua HTTP

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200 ok=true; đồng ý=DaLienKet; từ chối không thành liên kết có hiệu lực.
2. Dữ liệu và side effect: Đúng hai tài khoản, không tạo lớp/thành viên; lưu thời điểm và thông báo phản hồi.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: Found
Actual:   InternalServerError
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Fixture.Client(Nullable`1 actor) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 64
   at Fixture.Client(Nullable`1 actor) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 64
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 22
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 22
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 22
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 275
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_072() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 74
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-072-False-IT550c991f28024b6cb94f5d7aa08b440e.json`, `Evidence\IT-072-ITa4aa466ba96b42538ba8006e16bf940f.json`, `Evidence\IT-072-True-ITba35f9c4bcdb47a891bc88dc05285d06.json`

## IT-073 — Không nhận liên kết sai vai trò, sai người hoặc RV cũ

Phân loại: Kỳ vọng TC cần đối chiếu.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Sai vai trò bị 400; sai người 403; RV cũ 409; ok=false.
2. Dữ liệu và side effect: Không tự chuyển vai trò hoặc tạo liên kết sai; dữ liệu hợp lệ trước đó giữ nguyên.

**Quan sát/assertion thực tế:**
```text
Microsoft.Data.SqlClient.SqlException : Connection Timeout Expired.  The timeout period elapsed during the post-login phase.  The connection could have timed out while waiting for server to complete the login process and respond; Or it could have timed out while attempting to create multiple active connections.  The duration spent while attempting to connect to this server was - [Pre-Login] initialization=0; handshake=0; [Login] initialization=0; authentication=0; [Post-Login] complete=14014; 
---- System.ComponentModel.Win32Exception : The wait operation timed out.
```

**Ý nghĩa:** GuiDeNghiAsync chỉ tìm email tài khoản Học sinh; nhập Gia sư trả404, trong khi Excel yêu cầu400.

Vị trí cần xem trong ứng dụng: `Areas/HocTap/Services/LienKetTaiKhoanService.cs:25`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, SqlCommand command, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ThrowExceptionAndWarning(Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniError(TdsParserStateObject stateObj, UInt32 error)
   at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniSyncOverAsync()
   at Microsoft.Data.SqlClient.TdsParserStateObject.TryPrepareBuffer()
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.CompleteLogin(Boolean enlistOK)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.LoginNoFailover(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.OpenLoginEnlist(TimeoutTimer timeout, SqlConnectionString connectionOptions, SqlCredential credential, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds..ctor(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, Object providerInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString userConnectionOptions, SessionData reconnectSessionData, Boolean applyTransientFaultHandling, String accessToken, IDbConnectionPool pool, Func`3 accessTokenCallback)
   at Microsoft.Data.ProviderBase.DbConnectionFactory.<>c__DisplayClass28_0.<CreateReplaceConnectionContinuation>b__0(Task`1 _)
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Sql(String sql, Dictionary`2 pars, Boolean app) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 61
   at Fixture.Init() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 49
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 194
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_073() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 75
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-073-IT27ce04cfe2864019b97a57097e011520.json`

## IT-075 — Người nhận tự đồng ý hoặc từ chối lời mời

Phân loại: Sai lệch mã lỗi/quyền cần đối chiếu.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01/02 200; DL03 403. Đồng ý chỉ chuyển chờ Gia sư duyệt, chưa mở quyền lớp.
2. Dữ liệu và side effect: Lưu dấu đồng ý/từ chối; không tự duyệt hoặc cấp quyền cho X.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: Forbidden
Actual:   NotFound
```

**Ý nghĩa:** PhanHoiLoiMoiAsync tra lời mời thuộc người hiện tại; người ngoài nhận404, Excel yêu cầu403.

Vị trí cần xem trong ứng dụng: `Areas/HocTap/Services/LopHocService.cs:121`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 28
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 28
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 275
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_075() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 77
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-075-DL0-IT86d62053cd65471fbd6e8440ac1b35f9.json`, `Evidence\IT-075-DL1-IT3e7f6b4d2d464e92a4c97ac03f7f52bb.json`, `Evidence\IT-075-DL2-IT97cba1c0cd2449feb71a0a0a7e2223c4.json`, `Evidence\IT-075-IT387ddb6d1dcc4641af23289ecf57b283.json`

## IT-076 — Gia sư duyệt Xem/Chỉnh sửa và quyền nộp riêng

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01 chỉ đọc; DL02 đọc+chat nhưng không nộp; DL03 đọc+chat+nộp khi bài cho phép.
2. Dữ liệu và side effect: Quyền theo đúng ba tổ hợp; thông báo và lịch sử cập nhật; không suy ra quyền nộp từ ChinhSua.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   InternalServerError
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 30
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 30
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 275
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_076() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 78
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-076-DL0-IT5833b0332d4b4716bbf9bfb73e890c17.json`, `Evidence\IT-076-DL1-ITad2221479be2477080a70e229ab0551a.json`, `Evidence\IT-076-ITca259979550345238d0656fa5a7a8467.json`

## IT-079 — Đổi/thu hồi quyền có hiệu lực ở request kế tiếp

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Sau commit P1 không chat/nộp nữa; chỉ còn quyền đọc được cấp; cookie cũ không giữ quyền cũ.
2. Dữ liệu và side effect: Không tin/bài nộp mới; SQL và quyền realtime phải nhất quán; có notice/lịch sử thay quyền.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   InternalServerError
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 36
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 275
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_079() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 81
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-079-ITc492ea48fdbe493bbfc9bb81156e0391.json`

## IT-080 — Gỡ/rời thành viên không gỡ hai bên chính

Phân loại: Ứng viên lỗi thiếu luồng nghiệp vụ theo SRS.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01..03 thu hồi quyền thành viên bổ sung; DL04 bị từ chối.
2. Dữ liệu và side effect: Giữ lịch sử/thông tin người rời; không xóa G1/H1 hoặc xóa bài/tin cũ do P1 gửi.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   Forbidden
```

**Ý nghĩa:** DuyetAsync luôn kiểm quyền ghi với giaSuOnly=true; Bên học gỡ thành viên hoặc thành viên tự rời bị403 dù FR-38/QT-08 cho phép. Cần xem cả SRS và thiết kế trước sửa code.

Vị trí cần xem trong ứng dụng: `Areas/HocTap/Services/LopHocService.cs:145`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 34
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 34
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 275
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_080() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 82
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-080-G-ITc21b11f43b8847d6ae23bef4de75c7f7.json`, `Evidence\IT-080-H-ITdab70007aaf34c729b4d52e44d5cd24c.json`, `Evidence\IT-080-IT247a18c104884ef49ea2269a69aadad5.json`

## Giới hạn của lượt thực thi

Đã chạy toàn bộ danh mục 191 mã: 118 Pass, 73 Fail, 0 Skip. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. 8 ca trước đây pending hiện chạy và Fail do dispatcher chưa có implementation. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
