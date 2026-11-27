# Lịch và buổi học

Thời điểm: 27/11/2026 18:58:00 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Pass': 10, 'Fail': 3}. Kết quả script xUnit: {'Pass': 10, 'Fail': 3}.

**Kết luận Pass chỉ áp dụng cho các assertion đã triển khai; TC thiếu assertion được đánh dấu riêng. Ca xUnit Skip được ghi là chưa thực thi.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-083 | Service thêm batch ghép LichHoc–BuoiHoc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-084 | Service bắt đầu và ghi nhận buổi giữ Id | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-085 | Hai ghi nhận cùng RV không đè mất dữ liệu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-086 | Không nhân đôi buổi bù còn hiệu lực | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-087 | Form lịch hàng loạt và ngày/giờ local | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-088 | Batch trùng nội bộ hoặc đụng lịch người tham gia phải atomic | Fail | Fail | DL02 dùng lịch hiện hữu cùng lớp; chưa lớp C2 dùng chung G1. |
| IT-089 | Đổi lịch buổi chưa diễn ra giữ buổi và bài tập | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-090 | Không đổi buổi có hoạt động thực tế về tương lai | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-091 | Đến giờ mới bắt đầu được buổi | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-092 | Ghi nhận nội dung, kết quả và tham dự | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-093 | Qua giờ nhưng chưa ghi nhận không tính đã học | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-094 | Nghỉ cần bù chưa biết ngày vẫn được lưu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-095 | Tạo buổi bù giữ liên kết buổi gốc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |

## IT-088 — Batch trùng nội bộ hoặc đụng lịch người tham gia phải atomic

Phân loại: Kỳ vọng TC chưa khớp ánh xạ lỗi lịch.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01/02 HTTP422 JSON ok=false qua ModelState của form Them; có lỗi trùng lịch. DL03 HTTP200 ok=true vì hai khoảng liền kề không chồng lấn.
2. Dữ liệu và side effect: DL01/02 không lưu một phần batch/notice; DL03 đúng số buổi.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: 422
Actual:   409
```

**Ý nghĩa:** Batch trùng bị409, Excel yêu cầu422. LichHocController.ThemLoi giữ Rules.ErrorStatus vào Response.StatusCode; UiResponseFilter giữ status>=400. Không coi chênh mã này là bằng chứng batch đã commit một phần.

Vị trí cần xem trong ứng dụng: `Areas/HocTap/Controllers/LichHocController.cs:12`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.ScheduleCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ScheduleCases.cs:line 29
   at Cases.ScheduleCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ScheduleCases.cs:line 29
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 370
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_088() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 90
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-088-DL0-IT51896ab101784ad2a88b1150fae6a1ef.json`, `Evidence\IT-088-ITc78ae8bf6c004352b52cdf16eb3d6545.json`

## IT-090 — Không đổi buổi có hoạt động thực tế về tương lai

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Các lỗi nghiệp vụ ở form Sua được chuyển ModelState: HTTP422 ok=false, phân biệt buổi có hoạt động/sai quyền/phiên bản; không thay giờ để che lịch sử.
2. Dữ liệu và side effect: Giữ thời gian/ghi nhận/tham dự/bài tập, không che lịch sử hoặc sinh lịch sử sửa giả.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: 422
Actual:   400
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.ScheduleCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ScheduleCases.cs:line 59
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 370
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_090() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 92
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-090-IT8b4b88604b86402583dd1020f8c63ba3.json`

## IT-093 — Qua giờ nhưng chưa ghi nhận không tính đã học

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200; B1 là Chưa ghi nhận; số buổi đã học không tăng vì chỉ qua giờ.
2. Dữ liệu và side effect: Không có ghi nhận/điểm danh giả; dữ liệu chờ nhắc vẫn được giữ.

**Quan sát/assertion thực tế:**
```text
Assert.Contains() Failure: Sub-string not found
String:    "<!doctype html><html lang="vi"><head><met"···
Not found: "ChÆ°a ghi nháº­n"
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.ScheduleCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ScheduleCases.cs:line 37
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 370
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_093() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 95
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-093-ITace85c97cb5942578520660aa14b7013.json`

## Giới hạn của lượt thực thi

Lượt này chạy 187 TC; kết quả kết luận {'Pass': 132, 'Fail': 43, 'Chưa đủ phạm vi TC': 12}, kết quả xUnit {'Pass': 144, 'Fail': 43}. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
