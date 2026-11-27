# Liên hệ và đề xuất hai chiều

Thời điểm: 27/11/2026 18:58:00 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Chưa đủ phạm vi TC': 1, 'Fail': 5, 'Pass': 7}. Kết quả script xUnit: {'Pass': 8, 'Fail': 5}.

**Kết luận Pass chỉ áp dụng cho các assertion đã triển khai; TC thiếu assertion được đánh dấu riêng. Ca xUnit Skip được ghi là chưa thực thi.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-044 | Service gửi/chấp nhận liên hệ ghép hội thoại và thông báo | Chưa đủ phạm vi TC | Pass | Đã kiểm liên hệ, hội thoại, notification; chưa kiểm đầy đủ payload/outbox chống nhân bản. |
| IT-045 | Service gửi đề xuất hai chiều và kiểm vai trò bên nhận | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-046 | Hai gửi liên hệ đồng thời không nhân bản quan hệ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-047 | Gửi liên hệ qua HTTP giữ danh tính từ phiên | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-048 | Chặn liên hệ không hợp lệ trước và trong giao dịch | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-049 | Phản hồi liên hệ đúng bên và phiên bản | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-050 | Chưa chấp nhận liên hệ không nhắn tin hoặc đề xuất | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-051 | Form đề xuất đầy đủ từ cả hai phía | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-052 | Chỉ bên gửi được sửa đề xuất đang chờ | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-053 | Thiếu dữ liệu lớp hoặc mảng lịch bị loại | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-054 | Gửi/chấp nhận kiểm lại môn và nhu cầu bị ẩn | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-055 | Từ chối và rút theo đúng bên tham gia | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-056 | Điểm vào chuyển tiếp không bỏ kiểm tra actor và liên hệ | Fail | Fail | Người ngoài nhận404, Excel yêu cầu403; không chứng minh người ngoài tạo được yêu cầu. Đối chiếu quy tắc che sự tồn tại của liên hệ trước khi kết luận lỗi nghiệp vụ. |

## IT-045 — Service gửi đề xuất hai chiều và kiểm vai trò bên nhận

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: KetQua thành công; G1 gửi thì chờ Bên học, H1 gửi thì chờ Gia sư; nhận đúng bên còn lại.
2. Dữ liệu và side effect: Một đề xuất/DL, đầy đủ KhungGioDeXuat; mốc hết hạn 72 giờ; phí/mục tiêu/địa điểm khớp đầu vào; chưa tạo lớp.

**Quan sát/assertion thực tế:**
```text
EduConnect.Mvc.Infrastructure.LoiNghiepVu : Chỉ gia sư được gửi biểu mẫu đề xuất lớp.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at EduConnect.Mvc.Infrastructure.Rules.Check(Boolean condition, String message, Int32 status, String field) in C:\Khóa luận TN\EduConnect1\Infrastructure\BusinessRules.cs:line 47
   at EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService.GuiDeXuatAsync(DeXuatLopViewModel m, CancellationToken huy) in C:\Khóa luận TN\EduConnect1\Areas\KetNoi\Services\KetNoiService.cs:line 185
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 18
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 18
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 18
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 238
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_045() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 47
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-045-DL0-IT1a4cf5ec9286448d99eb7dbddfe41e15.json`, `Evidence\IT-045-DL1-ITfe38bd4d19a94f88a0221aa6783923e8.json`, `Evidence\IT-045-ITcab0578e436842d7a688efeb2373553c.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 257
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_051() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 53
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-051-G-IT73defaf63ce04707b5dfa7bdf29ccfd8.json`, `Evidence\IT-051-IT7fcc1965ce3f4f25891477027d60bb78.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 257
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_052() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 54
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-052-DL0-ITeb5a52ba60a9427e9f4437f69ac7278b.json`, `Evidence\IT-052-IT33478547c4e149b291fe7e37cebf3822.json`

## IT-055 — Từ chối và rút theo đúng bên tham gia

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01/02 200; sai bên/người ngoài 403; giữ lịch sử kết quả thay vì xóa yêu cầu.
2. Dữ liệu và side effect: R1 lần lượt TuChoi/DaRut; không lớp mới; notification/outbox gửi bên còn lại đúng một lần.

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
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 35
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 35
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 257
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_055() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 57
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

Bằng chứng: `Evidence\IT-055-DL0-IT791ea75c82cc480bbfd985d704243b8c.json`, `Evidence\IT-055-DL1-IT53f6c9cdedf64142822bcdd9ef6be973.json`, `Evidence\IT-055-ITbd418816eb544efd8c957ce8a09632dd.json`

## IT-056 — Điểm vào chuyển tiếp không bỏ kiểm tra actor và liên hệ

Phân loại: Sai lệch mã lỗi quyền cần thống nhất.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: H1 hợp lệ: HTTP200 ok=true, tạo yêu cầu ChoGiaSu để Gia sư bổ sung đề xuất; X bị403; không tạo lớp ngay từ yêu cầu sơ bộ.
2. Dữ liệu và side effect: Không bỏ qua liên hệ/ownership, không tạo lớp hoặc cấp quyền chat cho X.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: Found
Actual:   InternalServerError
```

**Ý nghĩa:** Người ngoài nhận404, Excel yêu cầu403; không chứng minh người ngoài tạo được yêu cầu. Đối chiếu quy tắc che sự tồn tại của liên hệ trước khi kết luận lỗi nghiệp vụ.

Vị trí cần xem trong ứng dụng: `Areas/KetNoi/Services/KetNoiService.cs:113`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Fixture.Client(Nullable`1 actor) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 64
   at Fixture.Client(Nullable`1 actor) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 64
   at Cases.ConnectionCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ConnectionCases.cs:line 37
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 257
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_056() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 58
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-056-ITe727042da8a040179ec9758155dead9f.json`

## Giới hạn của lượt thực thi

Lượt này chạy 187 TC; kết quả kết luận {'Pass': 132, 'Fail': 43, 'Chưa đủ phạm vi TC': 12}, kết quả xUnit {'Pass': 144, 'Fail': 43}. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
