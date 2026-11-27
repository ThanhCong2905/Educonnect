# Tệp và trao đổi

Thời điểm: 27/11/2026 18:58:00 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Pass': 11, 'Fail': 2}. Kết quả script xUnit: {'Pass': 11, 'Fail': 2}.

**Kết luận Pass chỉ áp dụng cho các assertion đã triển khai; TC thiếu assertion được đánh dấu riêng. Ca xUnit Skip được ghi là chưa thực thi.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-124 | TepService ghép scanner process và file sandbox | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-125 | Service gửi tin, tệp và outbox commit đồng bộ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-126 | Mã thao tác tin nhắn chống retry và thay nội dung | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-127 | Đuôi hợp lệ nhưng cấu trúc giả, macro hoặc traversal bị loại | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-128 | Scanner báo nhiễm/lỗi/timeout không được gắn trạng thái sạch | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-129 | Nhánh Development cho tệp chưa quét không áp sang môi trường khác | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-130 | Thu hồi quyền trong lúc quét phải kiểm lại trước gắn tệp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-131 | Lỗi lưu SQL sau ghi blob không để orphan | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-132 | HTTP gửi tin giữ ownership và nội dung | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-133 | Phân trang cùng timestamp và mốc quyền xem | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-134 | Người ngoài, thành viên Xem và lớp kết thúc không gửi được tin | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-135 | Biên số tệp và dung lượng dùng bytes thật | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-136 | Tải tệp kiểm lại quyền, scan state và hash | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 242
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_132() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 134
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-132-IT12066c48904b4cd68d5dcce1a2a1d553.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 238
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_134() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 136
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-134-ITb823a8683c0c472186a8ca6e8a5b1a3c.json`

## Giới hạn của lượt thực thi

Lượt này chạy 187 TC; kết quả kết luận {'Pass': 132, 'Fail': 43, 'Chưa đủ phạm vi TC': 12}, kết quả xUnit {'Pass': 144, 'Fail': 43}. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
