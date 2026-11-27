# Bài tập, nộp và chấm bài

Thời điểm: 27/11/2026 13:01:18 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Pass': 12, 'Chưa đủ phạm vi TC': 2, 'Fail': 1}. Kết quả script xUnit: {'Pass': 14, 'Fail': 1}.

**Script Pass chưa đủ phạm vi TC không được tính là TC Pass hoàn chỉnh. Lượt này không có ca Skip; 8 mã thiếu dispatcher được gọi và Fail trước khi chạy scenario nghiệp vụ.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-096 | Service giao bài và tệp gắn đúng BuoiHoc | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-097 | Service nộp/chấm ghép khóa mới nhất và lưu precision | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-098 | Retry cùng mã không tạo lần nộp mới | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-099 | Lưu lần nộp trước, chỉ chấm lần mới nhất | Chưa đủ phạm vi TC | Pass | Chưa phân loại chặt exception của lần chấm bản cũ; chưa đủ bằng chứng để kết luận TC hoàn chỉnh. |
| IT-100 | Chặn sai gia sư, điểm ngoài miền và precision vượt hai số lẻ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-101 | Nộp và chấm đồng thời kiểm lại trạng thái trong transaction | Chưa đủ phạm vi TC | Pass | Đã dùng hai scope/Task.WhenAll và kiểm invariant; chưa có barrier tại thời điểm tranh khóa. |
| IT-102 | Multipart giao bài qua Controller | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-103 | Buổi tương lai chỉ lưu nháp, công bố khi đủ điều kiện | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-104 | Bài bắt buộc gắn buổi đúng lớp, không giao vào buổi nghỉ/hủy | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-105 | Gắn buổi cho bài cũ chưa có BuoiHocId | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-106 | Nộp trước và sau hạn đều được ghi đúng thời điểm | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-107 | Đã có kết quả chấm không nhận lần nộp mới | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-108 | Quyền nộp thay không suy ra từ quyền chat | Fail | Fail | Một DL bị422 thay vì403 như Excel; cần xem ModelState và nhánh fallback ChiTiet trong BaiTapController.Nop/UiResponseFilter. Không tự kết luận có nộp bài trái quyền; query dữ liệu và giữ oracle theo Excel. |
| IT-109 | Chấm 0/10 hoặc chỉ nhận xét không quy đổi null thành 0 | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-110 | Sửa điểm dùng RV của kết quả, chống ghi đè | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |

## IT-108 — Quyền nộp thay không suy ra từ quyền chat

Phân loại: Sai lệch mã từ chối của form nộp bài.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Chỉ DL03 thành công; còn lại 403; hai bên chính vẫn dùng quyền vốn có.
2. Dữ liệu và side effect: Bài nộp thành công ghi đúng người nộp thay/người học; không nộp bài lớp khác.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: 403
Actual:   422
```

**Ý nghĩa:** Một DL bị422 thay vì403 như Excel; cần xem ModelState và nhánh fallback ChiTiet trong BaiTapController.Nop/UiResponseFilter. Không tự kết luận có nộp bài trái quyền; query dữ liệu và giữ oracle theo Excel.

Vị trí cần xem trong ứng dụng: `Areas/HocTap/Controllers/BaiTapController.cs:104`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.LearningCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\LearningCases.cs:line 36
   at Cases.LearningCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\LearningCases.cs:line 36
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 310
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_108() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 110
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-108-DL0-ITf008e6d7eb274213bff84bc2976304b9.json`, `Evidence\IT-108-DL1-IT522061270c7c44a69002576f884db79a.json`, `Evidence\IT-108-DL2-IT75167ca2fb234aceb560cf65a4ff40f5.json`, `Evidence\IT-108-DL3-IT1da9d39025c44bc6960961c2302d0d93.json`, `Evidence\IT-108-IT7223e0bdc49e4921af6e85aaf49b377c.json`

## Giới hạn của lượt thực thi

Đã chạy toàn bộ danh mục 191 mã: 118 Pass, 73 Fail, 0 Skip. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. 8 ca trước đây pending hiện chạy và Fail do dispatcher chưa có implementation. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
