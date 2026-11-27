# Nhu cầu học

Thời điểm: 27/11/2026 18:58:00 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Fail': 8, 'Pass': 6}. Kết quả script xUnit: {'Fail': 8, 'Pass': 6}.

**Kết luận Pass chỉ áp dụng cho các assertion đã triển khai; TC thiếu assertion được đánh dấu riêng. Ca xUnit Skip được ghi là chưa thực thi.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-014 | Service tạo nhu cầu nhiều môn và đọc lại | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-015 | Lỗi quan hệ môn không được lưu một phần nhu cầu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-016 | Hủy request khi chờ khóa SQL không để ghi một phần | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-017 | Binding form tạo nhu cầu từ HTTP | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-018 | Lưu nháp thiếu mục tiêu và lịch | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-019 | Phụ huynh đứng tên nhu cầu của người học | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-020 | Khách hoặc gia sư không tạo nhu cầu học | Fail | Fail | Authorize(Roles=HocSinh,PhuHuynh) ở Controller chặn Gia sư bằng403 trước action; Excel lại yêu cầu422. Không có bằng chứng vượt quyền. |
| IT-021 | Hạn mức nhu cầu mở, nháp và lượt đăng/ngày | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-022 | Chuẩn hóa trùng nhu cầu trước khi ghi | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-023 | Cập nhật thay môn/lịch, không đẩy như bài mới | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-024 | Sai chủ hoặc phiên bản cũ không ghi đè | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-025 | Validation form, địa giới và học phí đi qua đúng khớp nối | Fail | Fail | Một DL Excel yêu cầu422 nhưng HTTP trả200. Xem fieldUnderTest và http trong Evidence để xác định trường; chưa mặc định mọi DL validation đều sai. |
| IT-026 | Đóng/mở lại và xác nhận còn nhu cầu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-027 | Form ghi thiếu CSRF không gọi nghiệp vụ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |

## IT-014 — Service tạo nhu cầu nhiều môn và đọc lại

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: KetQua.ThanhCong=true; DuLieu=N1 khác Guid.Empty.
2. Dữ liệu và side effect: Một NhuCauHoc, hai NhuCauMonHoc, hai KhungGioRanh, một LuotDangNhuCau; chủ H1; không nhân bản nhu cầu theo môn.

**Quan sát/assertion thực tế:**
```text
Mỗi nhu cầu chỉ chọn một môn trong danh sách.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 18
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 20
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_014() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 16
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-014-IT1f7d706042b04cdfa2ae677f9c45760a.json`

## IT-017 — Binding form tạo nhu cầu từ HTTP

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: HTTP 200 JSON ok=true, redirectUrl về danh sách nhu cầu; thông báo tạo thành công.
2. Dữ liệu và side effect: Dữ liệu chép đúng tất cả môn/lịch/mục tiêu/phí; đúng một nhu cầu và lượt đăng.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   UnprocessableEntity
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 25
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_017() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 19
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-017-IT4d9852b152b943d7bc5e1eaed1d41761.json`

## IT-018 — Lưu nháp thiếu mục tiêu và lịch

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200 ok=true; được lưu nháp và đọc lại dữ liệu đã nhập.
2. Dữ liệu và side effect: TrangThai=BanNhap; không tự sinh lịch 00:00–00:00; không tăng lượt đăng bài công khai.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   UnprocessableEntity
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 25
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_018() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 20
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-018-IT59cd355e0d06410eb572a409bde50cc6.json`

## IT-019 — Phụ huynh đứng tên nhu cầu của người học

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200 ok=true; biểu mẫu hiển thị người học đã mô tả.
2. Dữ liệu và side effect: ChuSoHuuId=P1; giữ tên/tuổi/lớp; không tự tạo tài khoản Học sinh hoặc gộp nhu cầu của H1.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: OK
Actual:   UnprocessableEntity
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 25
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_019() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 21
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-019-IT2d2966c946fb4801b0998ab0667eea62.json`

## IT-020 — Khách hoặc gia sư không tạo nhu cầu học

Phân loại: Kỳ vọng TC chưa đúng tầng từ chối.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: DL01 HTTP 401; DL02 từ chối quyền (422 qua ModelState của action lưu), ok=false.
2. Dữ liệu và side effect: Không tạo NhuCauHoc/MonHoc/lịch/lượt đăng của bất kỳ DL bị từ chối.

**Quan sát/assertion thực tế:**
```text
Assert.Equal() Failure: Values differ
Expected: UnprocessableEntity
Actual:   Forbidden
```

**Ý nghĩa:** Authorize(Roles=HocSinh,PhuHuynh) ở Controller chặn Gia sư bằng403 trước action; Excel lại yêu cầu422. Không có bằng chứng vượt quyền.

Vị trí cần xem trong ứng dụng: `Areas/NhuCau/Controllers/NhuCauController.cs:11`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 27
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_020() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 22
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-020-ITf66330a181114c29b8e481a82c1c7d2b.json`

## IT-022 — Chuẩn hóa trùng nhu cầu trước khi ghi

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Cùng chủ: 422 báo cập nhật bài đã có. Khác chủ: 200, không gộp hai nhu cầu độc lập.
2. Dữ liệu và side effect: Cùng chủ vẫn một nhu cầu; khác chủ có hai nhu cầu riêng đúng ownership.

**Quan sát/assertion thực tế:**
```text
Mỗi nhu cầu chỉ chọn một môn trong danh sách.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 39
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 39
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_022() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 24
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-022-IT5d16066780024838919ae556827528bf.json`

## IT-023 — Cập nhật thay môn/lịch, không đẩy như bài mới

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: 200 ok=true, thông báo cập nhật.
2. Dữ liệu và side effect: Giữ Id/TaoLuc/chủ, RV đổi; link/lịch cũ được thay đồng bộ; không cộng lượt đăng mới.

**Quan sát/assertion thực tế:**
```text
Mỗi nhu cầu chỉ chọn một môn trong danh sách.
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 31
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 31
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_023() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 25
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-023-IT3d15b1a5713c4d8086ad91c3b07bfd33.json`

## IT-025 — Validation form, địa giới và học phí đi qua đúng khớp nối

Phân loại: Cần xác minh nhánh validation cho phép lưu.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Mỗi DL 422 ok=false; chỉ rõ trường nhập sai; dữ liệu biểu mẫu không bị thay bằng thành công.
2. Dữ liệu và side effect: Không có dòng cha/con/lượt đăng; không biến số sai kiểu thành một giá trị hợp lệ ngầm.

**Quan sát/assertion thực tế:**
```text
Request thiếu CSRF phải được audit là Authorization/TuChoi với lý do CSRFKhongHopLe.
```

**Ý nghĩa:** Một DL Excel yêu cầu422 nhưng HTTP trả200. Xem fieldUnderTest và http trong Evidence để xác định trường; chưa mặc định mọi DL validation đều sai.

Vị trí cần xem trong ứng dụng: `Areas/NhuCau/Services/NhuCauService.cs:66`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.

Script/stack thực thi:
```text
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 35
   at Cases.NeedCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\NeedCases.cs:line 35
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 322
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 397
   at IntegrationTests.IT_025() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 27
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-025-IT6eafb35c74e040efbe6e1e294ec403df.json`

## Giới hạn của lượt thực thi

Lượt này chạy 187 TC; kết quả kết luận {'Pass': 132, 'Fail': 43, 'Chưa đủ phạm vi TC': 12}, kết quả xUnit {'Pass': 144, 'Fail': 43}. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
