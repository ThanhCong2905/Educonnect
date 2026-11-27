# Liên kết và thành viên lớp

Thời điểm: 27/11/2026 18:58:00 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Pass': 7, 'Fail': 6}. Kết quả script xUnit: {'Pass': 7, 'Fail': 6}.

**Kết luận Pass chỉ áp dụng cho các assertion đã triển khai; TC thiếu assertion được đánh dấu riêng. Ca xUnit Skip được ghi là chưa thực thi.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-070 | Service đề nghị/xác nhận liên kết không tự cấp quyền lớp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-071 | Service lời mời → đồng ý → duyệt ghi đủ quyền | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-072 | Liên kết phụ huynh–học sinh qua HTTP | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-073 | Không nhận liên kết sai vai trò, sai người hoặc RV cũ | Fail | Fail | Script mới có DL01; chưa có sai người/RV cũ. |
| IT-074 | Đề nghị thành viên tại action DeNghi thực tế | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-075 | Người nhận tự đồng ý hoặc từ chối lời mời | Fail | Fail | Đã kiểm HTTP/state ba DL; chưa assert riêng quyền mở lớp sau từng DL. |
| IT-076 | Gia sư duyệt Xem/Chỉnh sửa và quyền nộp riêng | Fail | Fail | Đã kiểm ba cấu hình quyền lưu SQL; chưa thao tác đọc/chat/nộp cho cả ba cấu hình. |
| IT-077 | Duyệt lại điều kiện liên kết và hạn mức trong transaction | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-078 | Bên học/thành viên không duyệt quyền người khác | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-079 | Đổi/thu hồi quyền có hiệu lực ở request kế tiếp | Fail | Fail | Đã kiểm cookie cũ không chat/nộp; chưa kiểm GET đọc còn được phép. |
| IT-080 | Gỡ/rời thành viên không gỡ hai bên chính | Fail | Fail | Đã có DL gỡ G1/H1/P1 đối với P1; chưa DL bảo vệ hai thành viên chính. |
| IT-081 | Hủy liên kết thu hồi quyền thành viên phụ thuộc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-082 | Biết Id/là người nhà không mở lớp khác hoặc chat riêng | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 271
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
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

Bằng chứng: `Evidence\IT-071-ITcb1b7053248e46c58f1c149ad8518aaa.json`

## IT-073 — Không nhận liên kết sai vai trò, sai người hoặc RV cũ

Phân loại: Kỳ vọng TC cần đối chiếu.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Sai vai trò bị 400; sai người 403; RV cũ 409; ok=false.
2. Dữ liệu và side effect: Không tự chuyển vai trò hoặc tạo liên kết sai; dữ liệu hợp lệ trước đó giữ nguyên.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: BadRequest
Actual:   NotFound
```

**Ý nghĩa:** GuiDeNghiAsync chỉ tìm email tài khoản Học sinh; nhập Gia sư trả404, trong khi Excel yêu cầu400.

Vị trí cần xem trong ứng dụng: `Areas/HocTap/Services/LienKetTaiKhoanService.cs:25`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 24
   at Cases.FamilyCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\FamilyCases.cs:line 24
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 271
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_073() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 75
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-073-ITca8da7c69e5a460e8ec2f24b0c8d89ca.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 271
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_075() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 77
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-075-DL0-ITa65f44b45d7d4cba9a628a04531b7e89.json`, `Evidence\IT-075-DL1-IT08aaa7248130404096cdb2cef38cfddd.json`, `Evidence\IT-075-DL2-IT400eb64f81304170b8db116d4464b101.json`, `Evidence\IT-075-IT92e1e384abef4872a25a7faebe55071d.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 271
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_076() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 78
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-076-DL0-IT7a70d5f30ee24da5b6fed77e9b9f8f99.json`, `Evidence\IT-076-DL1-IT91f0640e42d64ca788b156ae5a243549.json`, `Evidence\IT-076-IT1d4b1b3ba3ed4e639db37630da02031e.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 271
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_079() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 81
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-079-ITa92d70b088cb445db104856819d10d70.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 271
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_080() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 82
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-080-G-IT0fbf3f3026a5426a845d0aebff0904dd.json`, `Evidence\IT-080-H-IT0d33a669b2914afcad7bc0876e6523bc.json`, `Evidence\IT-080-IT7da826c2a272498ba11c746b5643017e.json`

## Giới hạn của lượt thực thi

Lượt này chạy 187 TC; kết quả kết luận {'Pass': 132, 'Fail': 43, 'Chưa đủ phạm vi TC': 12}, kết quả xUnit {'Pass': 144, 'Fail': 43}. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
