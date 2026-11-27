# Thông báo, realtime, worker và nhật ký

Thời điểm: 27/11/2026 13:01:18 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Pass': 10, 'Fail': 8}. Kết quả script xUnit: {'Pass': 10, 'Fail': 8}.

**Script Pass chưa đủ phạm vi TC không được tính là TC Pass hoàn chỉnh. Lượt này không có ca Skip; 8 mã thiếu dispatcher được gọi và Fail trước khi chạy scenario nghiệp vụ.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-170 | Producer lưu notification/outbox trong cùng transaction | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-171 | Audit service/middleware ghép SQL bằng scope độc lập | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-172 | Đọc một/đọc tất cả thông báo đúng người nhận | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-173 | Audit liên thông request thành công/deny/CSRF/tải tệp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-174 | Lỗi lưu audit không hủy giao dịch nghiệp vụ đã commit | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-175 | MVC/filter ánh xạ response đúng kiểu request | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-176 | Hub chỉ nhận phiên thật hợp lệ và kiểm quyền room | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |
| IT-177 | Worker không phát trước commit rồi phát đúng EventId | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |
| IT-178 | Transport lỗi giữ nghiệp vụ và retry có backoff | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |
| IT-179 | Hai worker và lease hết hạn không làm mất sự kiện | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |
| IT-180 | Thu hồi quyền rời group và không nhận tin mới | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |
| IT-181 | Ping/notification nền không gia hạn phiên | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |
| IT-182 | Bảo trì hết hạn đề xuất không tạo lớp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-183 | Hết72 giờ tự kết thúc đúng yêu cầu còn chờ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-184 | Worker và phản hồi đúng hạn cạnh tranh có một kết quả hợp lệ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-185 | Nhắc30 ngày và đóng sau7 ngày không xác nhận | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-186 | Nhắc buổi/bài từ dữ liệu thật không tự chấm hoặc ghi nhận | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-187 | Dừng host không làm mất sự kiện chưa hoàn tất | Fail | Fail | Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng. |

## IT-175 — MVC/filter ánh xạ response đúng kiểu request

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Form thường 302 Location nội bộ; AJAX thành công 200 JSON ok=true/redirectUrl; AJAX lỗi 422 JSON ok=false/errors.MucTieu.
2. Dữ liệu và side effect: Hai DL thành công có đúng dữ liệu; DL lỗi không ghi; TempData thành công không rò sang request lỗi kế tiếp.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: Found
Actual:   OK
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 43
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 43
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 43
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 326
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_175() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 177
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-175-Form-IT6ac2039ce0204910bf405d2bdef33759.json`, `Evidence\IT-175-IT57a74c33e2c240ee900eb59013f51651.json`

## IT-176 — Hub chỉ nhận phiên thật hợp lệ và kiểm quyền room

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: H1 được tham gia đúng room; anonymous/thu hồi không kết nối hợp lệ; X nhận HubException về quyền.
2. Dữ liệu và side effect: Không add X vào group C1; audit truy cập hub ghi đúng kết quả, không lộ tin hoặc cookie.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-176
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_176() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 178
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-176-IT17c399d7133d42cb93f863fc7a211d8d.json`

## IT-177 — Worker không phát trước commit rồi phát đúng EventId

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Chỉ sau commit mới nhận event CapNhat với maSuKien=E1,loai/doiTuongId/hoiThoaiId đúng contract.
2. Dữ liệu và side effect: E1=HoanTat, có XuLyXongLuc; release lease; notification không nhân đôi. Không dùng SELECT bị khóa để suy ra đã phát.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-177
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_177() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 179
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-177-ITa257c64bd25d4d4dbc9afbebb078ff3f.json`

## IT-178 — Transport lỗi giữ nghiệp vụ và retry có backoff

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Lượt lỗi không đánh HoanTat; sau phục hồi phát đúng E1 rồi hoàn tất.
2. Dữ liệu và side effect: Tin/notice gốc không mất hoặc nhân bản; SoLanThu tăng; lease được trả; client dùng EventId để chống hiển thị lặp.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-178
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_178() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 180
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-178-ITc2cdc7f1503c4666ab3a5943334a0493.json`

## IT-179 — Hai worker và lease hết hạn không làm mất sự kiện

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Event được claim hợp lệ rồi hoàn tất; lease hết hạn được phục hồi; lease còn hạn của worker khác không bị chiếm.
2. Dữ liệu và side effect: Không mất event hoặc ghi HoanTat từ lease không còn sở hữu; không kết luận exactly-once mạng, mọi replay mang cùng EventId.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-179
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_179() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 181
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-179-IT291fd5e06f264834958f965097c0fb56.json`

## IT-180 — Thu hồi quyền rời group và không nhận tin mới

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: P1 bị bỏ group/đóng kết nối theo điều kiện; không nhận payload tin sau thu hồi; HTTP quyền cũ bị từ chối.
2. Dữ liệu và side effect: Không bỏ H1/G1 khỏi lớp; lịch sử cũ giữ theo chính sách quyền; có audit denial.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-180
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_180() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 182
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-180-IT984123fc5bbf445e954282fba4ccf8cf.json`

## IT-181 — Ping/notification nền không gia hạn phiên

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Mốc hoạt động không được kéo dài bởi nền; khi quá30 phút phiên bị từ chối/đóng.
2. Dữ liệu và side effect: Không tạo phiên mới hoặc cập nhật thời điểm như hoạt động chủ động; audit đúng loại.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-181
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_181() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 183
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-181-ITd1b4c60ee5dc42f998f5f30c93abc7af.json`

## IT-187 — Dừng host không làm mất sự kiện chưa hoàn tất

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Worker dừng có kiểm soát; event chưa ack không bị tự đánh HoanTat.
2. Dữ liệu và side effect: E1 còn để retry/lease recovery và xử lý được ở lượt mới; không mất tin/notice đã commit.

**Quan sát/assertion thực tế:**
```text
System.InvalidOperationException : No implementation: IT-187
```

**Ý nghĩa:** Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.

Script/stack thực thi:
```text
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_187() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 189
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-187-IT1fb131c6580f4f05a890732b28b8d2f5.json`

## Giới hạn của lượt thực thi

Đã chạy toàn bộ danh mục 191 mã: 118 Pass, 73 Fail, 0 Skip. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. 8 ca trước đây pending hiện chạy và Fail do dispatcher chưa có implementation. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
