# Tệp và trao đổi

Thời điểm: 27/11/2026 13:01:18 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Fail': 4, 'Chưa đủ phạm vi TC': 1, 'Pass': 8}. Kết quả script xUnit: {'Fail': 4, 'Pass': 9}.

**Script Pass chưa đủ phạm vi TC không được tính là TC Pass hoàn chỉnh. Lượt này không có ca Skip; 8 mã thiếu dispatcher được gọi và Fail trước khi chạy scenario nghiệp vụ.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-124 | TepService ghép scanner process và file sandbox | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-125 | Service gửi tin, tệp và outbox commit đồng bộ | Chưa đủ phạm vi TC | Pass | Đã kiểm tin và outbox; chưa ghép tệp thật trong giao dịch. |
| IT-126 | Mã thao tác tin nhắn chống retry và thay nội dung | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-127 | Đuôi hợp lệ nhưng cấu trúc giả, macro hoặc traversal bị loại | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-128 | Scanner báo nhiễm/lỗi/timeout không được gắn trạng thái sạch | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-129 | Nhánh Development cho tệp chưa quét không áp sang môi trường khác | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-130 | Thu hồi quyền trong lúc quét phải kiểm lại trước gắn tệp | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |
| IT-131 | Lỗi lưu SQL sau ghi blob không để orphan | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-132 | HTTP gửi tin giữ ownership và nội dung | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-133 | Phân trang cùng timestamp và mốc quyền xem | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-134 | Người ngoài, thành viên Xem và lớp kết thúc không gửi được tin | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-135 | Biên số tệp và dung lượng dùng bytes thật | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-136 | Tải tệp kiểm lại quyền, scan state và hash | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |

## IT-124 — TepService ghép scanner process và file sandbox

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Tệp hợp lệ qua scanner, trạng thái Sach; không fake IVirusScanner để khẳng định đã kiểm protocol process.
2. Dữ liệu và side effect: File còn nguyên bytes, tên lưu không theo path người dùng, metadata đúng người/đối tượng khi transaction commit.

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
   at EduConnect.Mvc.Infrastructure.TepService.LuuAsync(List`1 files, Nullable`1 giao, Nullable`1 nop, Nullable`1 hoSo, Nullable`1 tinNhan, Nullable`1 baoCao) in C:\Khóa luận TN\EduConnect1\Infrastructure\TepService.cs:line 198
   at Cases.FileCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FileCases.cs:line 70
   at Cases.FileCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FileCases.cs:line 73
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 282
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_124() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 126
--- End of stack trace from previous location ---
----- Inner Stack Trace -----

```

Bằng chứng: `Evidence\IT-124-ITd342100e5de44ca9971dfb92d530648e.json`

## IT-130 — Thu hồi quyền trong lúc quét phải kiểm lại trước gắn tệp

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Tin nhắn bị từ chối sau kiểm lại quyền; không chấp nhận dựa vào quyền trước lúc quét.
2. Dữ liệu và side effect: Không TinNhan/TepDinhKem/outbox; blob tạm của thao tác thua được dọn; file cũ không bị xóa.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-130
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_130() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 132
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-130-ITf54838e75dce4b13b449b63b079648f2.json`

## IT-132 — HTTP gửi tin giữ ownership và nội dung

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: HTTP200 JSON ok=true và redirectUrl về room hiện tại; nội dung khi render được encode, không thực thi script; sender vẫn H1, không lấy actor giả từ form.
2. Dữ liệu và side effect: Đúng một tin; không lưu actor giả hoặc đưa nội dung chat vào audit nhạy cảm.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   InternalServerError
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Fixture.Token(HttpClient c, String url) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 65
   at Fixture.Post(HttpClient c, String url, Dictionary`2 fields, String form, Boolean csrf, Boolean ajax) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\Harness.cs:line 66
   at Cases.ChatCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ChatCases.cs:line 61
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 246
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_132() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 134
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-132-ITd2809a64e5264e0283446ad926f6a728.json`

## IT-134 — Người ngoài, thành viên Xem và lớp kết thúc không gửi được tin

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: X/sai quyền 403; lớp kết thúc 409; các bên có quyền đọc vẫn đọc được lịch sử.
2. Dữ liệu và side effect: Không tin/file/event mới; không xóa lịch sử khi khóa gửi.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   InternalServerError
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.BusinessCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\BusinessCases.cs:line 61
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 242
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_134() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 136
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-134-ITc4edf369e02848a18d3c5f088a439d75.json`

## Giới hạn của lượt thực thi

Đã chạy toàn bộ danh mục 191 mã: 118 Pass, 73 Fail, 0 Skip. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. 8 ca trước đây pending hiện chạy và Fail do dispatcher chưa có implementation. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
