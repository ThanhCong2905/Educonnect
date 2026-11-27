# Hợp đồng SQL và ràng buộc nền

Thời điểm: 27/11/2026 13:01:18 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Pass': 3, 'Fail': 9, 'Chưa đủ phạm vi TC': 1}. Kết quả script xUnit: {'Pass': 4, 'Fail': 9}.

**Script Pass chưa đủ phạm vi TC không được tính là TC Pass hoàn chỉnh. Lượt này không có ca Skip; 8 mã thiếu dispatcher được gọi và Fail trước khi chạy scenario nghiệp vụ.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-001 | Một tài khoản chỉ có một vai trò | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-002 | Khóa ghép nhu cầu–môn và khóa ngoại | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-003 | Đề xuất SQL kiểm tra liên hệ và môn | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-004 | Chấp nhận SQL tạo đúng một lớp và dữ liệu con | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-005 | Chấp nhận SQL thất bại phải rollback | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-006 | Procedure lịch và trigger tạo BuoiHoc | Fail | Fail | Chưa có DL02 batch lỗi để chứng minh rollback. |
| IT-007 | Procedure ghi nhận buổi kiểm người thực hiện | Fail | Fail | Đã kiểm actor H1 sai quyền; chưa thay bằng Gia sư G2 như DL02 trong Excel. |
| IT-008 | Nộp bài SQL giữ lịch sử và chống lặp mã | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-009 | Chấm SQL kiểm điểm, quyền và lần mới nhất | Fail | Fail | Đã kiểm H1 sai quyền; chưa có Gia sư G2 riêng như Excel. |
| IT-010 | Procedure lỗi đăng nhập và thời gian chờ | Fail | Fail | TC yêu cầu chờ 0/1/5/15 phút, procedure hiện tại quy định 30/60/120/300 giây. Không tự kết luận SQL sai trước khi đối chiếu thiết kế/SRS. |
| IT-011 | Procedure phiên kiểm chủ, thu hồi và hết hạn | Fail | Fail | Chưa có DL tài khoản BiKhoa và đối chiếu hoạt động nền/chủ động. |
| IT-012 | Principal ứng dụng bị chặn sửa/xóa nhật ký | Chưa đủ phạm vi TC | Pass | Đã kiểm quyền SQL UPDATE/DELETE; chưa INSERT/SELECT một audit fixture thực. |
| IT-013 | EF–SQL round-trip trạng thái, khóa phiên bản và mốc kết thúc | Fail | Fail | Đã round-trip LopHoc và RV; chưa đủ YeuCauHuyKetNoi/thời điểm UTC+07/TamDung. |

## IT-002 — Khóa ghép nhu cầu–môn và khóa ngoại

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01 lỗi unique/PK; DL02 lỗi FK. Không coi kiểm metadata là thực thi hành vi tích hợp.
2. Dữ liệu và side effect: Mỗi DL chỉ giữ link ban đầu; nhu cầu N1 không bị xóa hoặc đổi môn đại diện.

**Quan sát/assertion thực tế:**
```text
Assert.Contains() Failure: Item not found in collection
Collection: [547]
Not found:  2601
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 16
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_002() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 4
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-002-IT35bef99a2d894d61a902e5f28b9d71d2.json`

## IT-005 — Chấp nhận SQL thất bại phải rollback

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Từng DL bị từ chối vì sai quyền/thiếu lịch/trùng lịch; không chỉ kiểm exception rồi bỏ qua dữ liệu đã ghi.
2. Dữ liệu và side effect: R1 không sang DaChapNhan; không LopHoc/LichHoc/BuoiHoc/hội thoại lớp/thông báo mồ côi.

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
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 23
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 28
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_005() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 7
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

Bằng chứng: `Evidence\IT-005-DL0-ITbba97a269b9341dd8bb7555a1fdc943f.json`, `Evidence\IT-005-DL1-IT5803fb4f9ca54151881f08140ab2cd99.json`, `Evidence\IT-005-ITb6157b3928c04b6dac4edd9f363e5e3d.json`

## IT-006 — Procedure lịch và trigger tạo BuoiHoc

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01 commit hai lịch, mỗi lịch có đúng một BuoiHoc. DL02 bị từ chối toàn batch.
2. Dữ liệu và side effect: Không lịch tồn tại mà thiếu BuoiHoc; DL02 không lưu một phần batch.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : Invalid operation. The connection is closed.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.ValidateConnectionForExecute(SqlCommand command)
   at Microsoft.Data.SqlClient.SqlInternalTransaction.Rollback()
   at Microsoft.Data.SqlClient.SqlInternalTransaction.Dispose(Boolean disposing)
   at Microsoft.Data.SqlClient.SqlTransaction.Dispose(Boolean disposing)
   at Microsoft.EntityFrameworkCore.Storage.RelationalTransaction.DisposeAsync()
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
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 31
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_006() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 8
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-006-IT835c3378773a487f9a386a2f72bdc364.json`

## IT-007 — Procedure ghi nhận buổi kiểm người thực hiện

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: G1 được ghi; G2 bị từ chối tại DB, không thể vượt quyền bằng cách bỏ HTTP.
2. Dữ liệu và side effect: Một kết quả hiện hành; không đổi LichHocId hoặc ghi dữ liệu cho sai lớp.

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
   at Fixture.SeedLesson(Boolean past) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 59
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 33
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_007() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 9
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

Bằng chứng: `Evidence\IT-007-IT067b9d1d10904ea49848e267bb302a63.json`

## IT-008 — Nộp bài SQL giữ lịch sử và chống lặp mã

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: O1 chỉ tạo một lần nộp; O2 tạo lần mới; số lần nộp tăng trong đúng A1.
2. Dữ liệu và side effect: Giữ cả hai lần; không lẫn bài giao khác; mã thao tác không nhân bản metadata tệp/thông báo.

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
   at Fixture.SeedLesson(Boolean past) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 59
   at Fixture.SeedAssignment() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 60
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 37
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_008() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 10
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

Bằng chứng: `Evidence\IT-008-IT568f2aaa9d8a4e68901589a84bcf05e3.json`

## IT-009 — Chấm SQL kiểm điểm, quyền và lần mới nhất

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01 thành công; các DL sai quyền/lần cũ/điểm ngoài miền bị từ chối.
2. Dữ liệu và side effect: Không kết quả chấm cho DL02..04; điểm DL01=8.25, không làm tròn tùy tiện thành 8 hoặc 8.3.

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
   at Fixture.SeedLesson(Boolean past) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 59
   at Fixture.SeedAssignment() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 60
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 39
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_009() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 11
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

Bằng chứng: `Evidence\IT-009-IT8bb3ac9bbbd942839c3c64d610a0c8a5.json`

## IT-010 — Procedure lỗi đăng nhập và thời gian chờ

Phân loại: Sai lệch kỳ vọng TC/SQL cần thống nhất.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Số lỗi sau là 5/6/7/8; thời gian chờ tương ứng 0/1/5/15 phút theo thiết kế SQL.
2. Dữ liệu và side effect: Hai loại khóa được ghi đúng giá trị; trong thời gian đang chờ không cộng thêm lỗi; account/IP khác không bị khóa nhầm.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: 0
Actual:   30
```

**Ý nghĩa:** TC yêu cầu chờ 0/1/5/15 phút, procedure hiện tại quy định 30/60/120/300 giây. Không tự kết luận SQL sai trước khi đối chiếu thiết kế/SRS.

Vị trí cần xem trong ứng dụng: `Database/00_EduConnect_Full.sql:1189`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 46
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 46
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_010() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 12
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-010-IT17c3145c7ac543d9bd4ea6b6ebd9b3ea.json`

## IT-011 — Procedure phiên kiểm chủ, thu hồi và hết hạn

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Chỉ phiên đúng chủ/hoạt động/chưa thu hồi/còn hạn trả HopLe=1. Request nền không gia hạn; hoạt động chủ động hợp lệ mới gia hạn.
2. Dữ liệu và side effect: Không chuyển chủ phiên hoặc hồi sinh phiên bị thu hồi/hết hạn/tài khoản khóa.

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
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 49
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 51
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_011() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 13
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-011-IT85df7ab40b1b44818ba9379bb464910b.json`

## IT-013 — EF–SQL round-trip trạng thái, khóa phiên bản và mốc kết thúc

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Giá trị sau round-trip đúng kiểu/độ chính xác schema; trạng thái hợp lệ không lỗi CHECK; context cũ phát hiện concurrency.
2. Dữ liệu và side effect: Không mất thời điểm/người/lý do/cách kết thúc; RowVersion mới có 8 byte và đổi sau update; SQL/EF không ánh xạ lệch cột.

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
   at Cases.SqlContracts(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\SqlContracts.cs:line 55
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 388
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_013() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 15
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

Bằng chứng: `Evidence\IT-013-ITe540ab7f50c24350b8a475a5dedfbba4.json`

## Giới hạn của lượt thực thi

Đã chạy toàn bộ danh mục 191 mã: 118 Pass, 73 Fail, 0 Skip. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. 8 ca trước đây pending hiện chạy và Fail do dispatcher chưa có implementation. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
