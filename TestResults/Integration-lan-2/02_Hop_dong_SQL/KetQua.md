# Hợp đồng SQL và ràng buộc nền

Thời điểm: 27/11/2026 18:58:00 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Pass': 6, 'Fail': 2, 'Chưa đủ phạm vi TC': 5}. Kết quả script xUnit: {'Pass': 11, 'Fail': 2}.

**Kết luận Pass chỉ áp dụng cho các assertion đã triển khai; TC thiếu assertion được đánh dấu riêng. Ca xUnit Skip được ghi là chưa thực thi.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-001 | Một tài khoản chỉ có một vai trò | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-002 | Khóa ghép nhu cầu–môn và khóa ngoại | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-003 | Đề xuất SQL kiểm tra liên hệ và môn | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-004 | Chấp nhận SQL tạo đúng một lớp và dữ liệu con | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-005 | Chấp nhận SQL thất bại phải rollback | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-006 | Procedure lịch và trigger tạo BuoiHoc | Chưa đủ phạm vi TC | Pass | Chưa có DL02 batch lỗi để chứng minh rollback. |
| IT-007 | Procedure ghi nhận buổi kiểm người thực hiện | Chưa đủ phạm vi TC | Pass | Đã kiểm actor H1 sai quyền; chưa thay bằng Gia sư G2 như DL02 trong Excel. |
| IT-008 | Nộp bài SQL giữ lịch sử và chống lặp mã | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-009 | Chấm SQL kiểm điểm, quyền và lần mới nhất | Chưa đủ phạm vi TC | Pass | Đã kiểm H1 sai quyền; chưa có Gia sư G2 riêng như Excel. |
| IT-010 | Procedure lỗi đăng nhập và thời gian chờ | Fail | Fail | TC yêu cầu chờ 0/1/5/15 phút, procedure hiện tại quy định 30/60/120/300 giây. Không tự kết luận SQL sai trước khi đối chiếu thiết kế/SRS. |
| IT-011 | Procedure phiên kiểm chủ, thu hồi và hết hạn | Chưa đủ phạm vi TC | Pass | Chưa có DL tài khoản BiKhoa và đối chiếu hoạt động nền/chủ động. |
| IT-012 | Principal ứng dụng bị chặn sửa/xóa nhật ký | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-013 | EF–SQL round-trip trạng thái, khóa phiên bản và mốc kết thúc | Chưa đủ phạm vi TC | Pass | Đã round-trip LopHoc và RV; chưa đủ YeuCauHuyKetNoi/thời điểm UTC+07/TamDung. |

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 390
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_002() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 4
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-002-IT8e8799308ec64b728c7501e996a4ecf3.json`

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
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 390
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_010() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 12
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-010-ITe84395f0efce4add96c51891aae46323.json`

## Giới hạn của lượt thực thi

Lượt này chạy 187 TC; kết quả kết luận {'Pass': 132, 'Fail': 43, 'Chưa đủ phạm vi TC': 12}, kết quả xUnit {'Pass': 144, 'Fail': 43}. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
