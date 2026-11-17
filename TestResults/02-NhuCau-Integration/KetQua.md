# Kết quả tích hợp 02 Nhu cau

**14 ca: 11 Pass, 3 Fail, 0 Skip/Blocked.**

Đối chiếu đúng mã/thứ tự và kỳ vọng sheet `02 Nhu cau` trong `Integration Test.xlsx`. Không đổi kỳ vọng để phù hợp với code hiện tại.

## Môi trường và cách thực thi

- Chuỗi thật: HttpClient → TestServer/WebApplicationFactory → middleware, Identity/cookie, phân quyền, antiforgery, model binding, filter → controller/service → EF hoặc procedure → SQL Server. Không mock service, nguồn người dùng hoặc kho dữ liệu.
- CSDL: `LAPTOP-3L2V1AFT\SQLEXPRESS`, database riêng `EduConnect_Verification_IT0203`. Mỗi ca ghi DB_NAME()/@@SERVERNAME và dùng tài khoản, môn, hồ sơ riêng có tag; không ghi CSDL ứng dụng `Educonnect`.
- Đăng nhập bằng mật khẩu fixture qua action đăng nhập thật và phiên SQL; AllowAutoRedirect=false. POST AJAX gửi X-Requested-With và antiforgery thật, trừ đúng ca kiểm thiếu token/anonymous.
- Development, DemoPublishProfiles=false để duyệt hồ sơ theo workflow; ngừng hai worker trong test host để không có tác động nền ngoài ca. Đây không phải kiểm worker hoặc browser/JavaScript.
- Sau HTTP, đọc snapshot bằng DbContext mới/AsNoTracking. Thu dữ liệu trước cleanup; xóa theo đúng ID và thứ tự FK. Tất cả ca cuối đều cleanup thành công.
- Quyền kết nối SQL dùng Windows của người chạy; chưa kiểm quyền giới hạn của role KLTN_App. Không coi kết quả này là kiểm mọi cấu hình triển khai.

## Kết quả từng ca

| Mã | Kịch bản | Tiêu chuẩn | Kết quả | Thực tế |
| --- | --- | --- | --- | --- |
| IT-016 | Tạo nhu cầu hai môn | Đầy đủ | Pass | HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/NhuCau"} Sau thao tác: 1 nhu cầu, 2 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-017 | Lưu bản nháp thiếu lịch | Đúng đắn | Pass | HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/NhuCau"} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-018 | Phụ huynh tạo cho người học | Đầy đủ | Pass | HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/NhuCau"} Sau thao tác: 1 nhu cầu, 2 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-019 | Gia sư cố tạo nhu cầu | Xác thực | Pass | HTTP 403, application/json; charset=utf-8 {"ok": false, "message": "Bạn không có quyền thực hiện thao tác này."} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-020 | Môn ngừng sử dụng | Đúng đắn | Pass | HTTP 422, application/json; charset=utf-8 {"ok": false, "message": "Kiểm tra lại thông tin đã nhập.", "errors": {"": ["Môn học không hợp lệ."]}} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-021 | Update thay môn và lịch | Đầy đủ | Pass | HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/NhuCau"} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-022 | Update sai chủ | Xác thực | Fail | HTTP 422, application/json; charset=utf-8 {"ok": false, "message": "Kiểm tra lại thông tin đã nhập.", "errors": {"": ["Nhu cầu không thuộc về bạn."]}} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-023 | Update RV cũ | Đúng đắn | Fail | HTTP 422, application/json; charset=utf-8 {"ok": false, "message": "Kiểm tra lại thông tin đã nhập.", "errors": {"": ["Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu."]}} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-024 | Gửi lại cùng mã thao tác | Đúng đắn | Pass | HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/NhuCau"} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-025 | Mã thao tác đã dùng cho nhu cầu khác | Đúng đắn | Fail | HTTP 422, application/json; charset=utf-8 {"ok": false, "message": "Kiểm tra lại thông tin đã nhập.", "errors": {"": ["Mã thao tác đã dùng cho nhu cầu khác."]}} Sau thao tác: 2 nhu cầu, 2 liên kết môn, 4 lịch, 2 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-026 | Đóng nhu cầu | Đúng đắn | Pass | HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/NhuCau"} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-027 | Mở lại nhu cầu đã đóng | Đúng đắn | Pass | HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/NhuCau"} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 2 lịch, 2 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-028 | Không đăng nhập gọi thao tác ghi | Xác thực | Pass | HTTP 401, application/json; charset=utf-8 {"ok": false, "message": "Phiên đã hết hạn. Vui lòng đăng nhập lại."} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-029 | Thiếu antiforgery token | Xác thực | Pass | HTTP 400, None Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |

## Chi tiết ca thất bại

### IT-022 — Update sai chủ

**Điểm vào:** `POST /NhuCau/NhuCau/Luu`.

**Dữ liệu:** Actor X; Id=N1 của H; lấy RV hợp lệ từ fixture. Tag=IT0203IT_3186e850be6c43bd9a8562298f1e2cfb; actor/ID cụ thể xem observation trong JSON.

**Kỳ vọng HTTP:** 403; không vì có RV mà có quyền.

**Kỳ vọng dữ liệu:** N1, môn và lịch nguyên trạng.

**Thực tế:** HTTP 422, application/json; charset=utf-8 {"ok": false, "message": "Kiểm tra lại thông tin đã nhập.", "errors": {"": ["Nhu cầu không thuộc về bạn."]}} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Các sai lệch đã assert:**

- HTTP kỳ vọng 403, thực tế 422.

**Nguyên nhân trong code:** Service phát sinh LoiNghiepVu 403 vì N1 không thuộc actor X. Action Luu bắt exception nhưng chỉ chép ex.Message vào ModelState, không giữ ex.Status. UiResponseFilter thấy ModelState lỗi và HTTP chưa có mã >=400 nên đổi thành JSON 422.

**Vị trí:** `Areas/NhuCau/Controllers/NhuCauController.cs:100`; `Infrastructure/UiResponseFilter.cs:46`; `Areas/NhuCau/Services/NhuCauService.cs:164`.

**Ảnh hưởng/phạm vi kết luận:** Quyền sở hữu vẫn được bảo vệ: N1/môn/lịch/lượt đăng không đổi. Sai lệch là mã giao tiếp HTTP 422 thay cho 403, khiến client xử lý nhầm lỗi quyền thành lỗi dữ liệu.

**Đề xuất sửa:** Giữ mã 403 của lỗi quyền khi chuyển LoiNghiepVu sang ActionResult/JSON. Không gộp lỗi quyền với ModelState validation 422.

**Body phản hồi liên quan:**

```json
{
  "ok": false,
  "message": "Kiểm tra lại thông tin đã nhập.",
  "errors": {
    "": [
      "Nhu cầu không thuộc về bạn."
    ]
  }
}
```

### IT-023 — Update RV cũ

**Điểm vào:** `POST /NhuCau/NhuCau/Luu`.

**Dữ liệu:** Đọc RV1; lưu A tạo RV2; gửi B với RV1. Tag=IT0203IT_e1f71b56776f471187866ebd982ef1d7; actor/ID cụ thể xem observation trong JSON.

**Kỳ vọng HTTP:** B trả 409.

**Kỳ vọng dữ liệu:** Dữ liệu A giữ nguyên; không xóa lịch/môn của A.

**Thực tế:** HTTP 422, application/json; charset=utf-8 {"ok": false, "message": "Kiểm tra lại thông tin đã nhập.", "errors": {"": ["Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu."]}} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Các sai lệch đã assert:**

- HTTP kỳ vọng 409, thực tế 422.

**Nguyên nhân trong code:** RV1 được đọc trước lần lưu A. A tạo RV2; B gửi RV1 bị cơ chế concurrency phát hiện và KetQua đổi thành lỗi 409. Qua LuuAsync, lỗi được ném lại thành LoiNghiepVu; controller bắt và thêm ModelState, làm mất status 409. Filter trả 422.

**Vị trí:** `Areas/NhuCau/Controllers/NhuCauController.cs:100`; `Infrastructure/UiResponseFilter.cs:46`; `Infrastructure/KetQua.cs:27`; `Areas/NhuCau/Services/EfNhuCauStore.cs:43`.

**Ảnh hưởng/phạm vi kết luận:** Dữ liệu A, môn và lịch của A được giữ nguyên sau B; không thấy ghi đè bằng RV cũ. Client không nhận đúng 409 để báo xung đột và yêu cầu tải lại.

**Đề xuất sửa:** Bảo toàn status 409 đến HTTP response; giữ nguyên kiểm RowVersion và transaction đang bảo vệ dữ liệu. Sau sửa chạy lại hai client/lần lưu A–B.

**Body phản hồi liên quan:**

```json
{
  "ok": false,
  "message": "Kiểm tra lại thông tin đã nhập.",
  "errors": {
    "": [
      "Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu."
    ]
  }
}
```

### IT-025 — Mã thao tác đã dùng cho nhu cầu khác

**Điểm vào:** `POST /NhuCau/NhuCau/Luu`.

**Dữ liệu:** MaThaoTac đã gắn N1 nhưng payload trỏ N2. Tag=IT0203IT_5b04c6e3f6a74f0093058041b68072c4; actor/ID cụ thể xem observation trong JSON.

**Kỳ vọng HTTP:** 409.0

**Kỳ vọng dữ liệu:** N1/N2 không bị ghi nhầm.

**Thực tế:** HTTP 422, application/json; charset=utf-8 {"ok": false, "message": "Kiểm tra lại thông tin đã nhập.", "errors": {"": ["Mã thao tác đã dùng cho nhu cầu khác."]}} Sau thao tác: 2 nhu cầu, 2 liên kết môn, 4 lịch, 2 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Các sai lệch đã assert:**

- HTTP kỳ vọng 409, thực tế 422.

**Nguyên nhân trong code:** Service nhận ra MaThaoTac đã gắn N1 nhưng payload đang sửa N2 và phát sinh 409 đúng rule. Controller Luu chuyển lỗi thành ModelState, rồi UiResponseFilter trả 422 vì response chưa được đặt mã lỗi gốc.

**Vị trí:** `Areas/NhuCau/Controllers/NhuCauController.cs:100`; `Infrastructure/UiResponseFilter.cs:46`; `Areas/NhuCau/Services/NhuCauService.cs:69`.

**Ảnh hưởng/phạm vi kết luận:** N1/N2 không bị ghi nhầm và số lượt đăng không đổi; hợp đồng HTTP xung đột mã thao tác chưa đúng 409.

**Đề xuất sửa:** Bảo toàn status 409 của lỗi nghiệp vụ; phân biệt lỗi xung đột thao tác với lỗi nhập liệu 422. Giữ rule chống tái sử dụng mã của nhu cầu khác.

**Body phản hồi liên quan:**

```json
{
  "ok": false,
  "message": "Kiểm tra lại thông tin đã nhập.",
  "errors": {
    "": [
      "Mã thao tác đã dùng cho nhu cầu khác."
    ]
  }
}
```

## Các bước và bằng chứng từng ca

### IT-016 — Tạo nhu cầu hai môn (Pass)

**Tiêu chuẩn:** Đầy đủ.

**Điều kiện riêng:** Actor H; MonHocIds=[M1,M2], BanNhap=false.

**Test:** `Integration0203Tests.IT_016`; thời gian 00:00:00.9133522.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /NhuCau/NhuCau/Luu` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: HTTP 200 JSON ok=true/redirectUrl; đúng một nhu cầu DangMo chủ H, hai môn, hai lịch và một lượt đúng MaThaoTac.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** AJAX 200 ok=true; đọc redirectUrl.

**Kỳ vọng DB gốc:** Một DangMo, chủ H; 2 NhuCauMonHoc, 2 lịch; một lượt đăng đúng MaThaoTac.

**Quan sát:** HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/NhuCau"} Sau thao tác: 1 nhu cầu, 2 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need; SELECT * FROM dbo.[NhuCauMonHoc] WHERE NhuCauId=@Need; SELECT * FROM dbo.[KhungGioRanh] WHERE NhuCauId=@Need; SELECT * FROM dbo.[LuotDangNhuCau] WHERE TaiKhoanId=@Actor;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_6dcbe105301c4bb1809cd21192b83cc5`.

### IT-017 — Lưu bản nháp thiếu lịch (Pass)

**Tiêu chuẩn:** Đúng đắn.

**Điều kiện riêng:** BanNhap=true; lịch rỗng; các trường validator bắt buộc vẫn hợp lệ.

**Test:** `Integration0203Tests.IT_017`; thời gian 00:00:00.9268224.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /NhuCau/NhuCau/Luu` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: HTTP thành công; BanNhap, không lịch và không lượt đăng công khai.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** Thành công nếu dữ liệu đáp ứng validator bản nháp.

**Kỳ vọng DB gốc:** BanNhap; không tạo lượt đăng công khai.

**Quan sát:** HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/NhuCau"} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need; SELECT * FROM dbo.[NhuCauMonHoc] WHERE NhuCauId=@Need; SELECT * FROM dbo.[KhungGioRanh] WHERE NhuCauId=@Need; SELECT * FROM dbo.[LuotDangNhuCau] WHERE TaiKhoanId=@Actor;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_9dc9276ef0044d8ca443ba17a90ef8c8`.

### IT-018 — Phụ huynh tạo cho người học (Pass)

**Tiêu chuẩn:** Đầy đủ.

**Điều kiện riêng:** Actor P; tên người học An; tuổi 12; hai môn.

**Test:** `Integration0203Tests.IT_018`; thời gian 00:00:00.8706975.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /NhuCau/NhuCau/Luu` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: HTTP thành công; nhu cầu thuộc P, tên An/tuổi 12, đủ hai môn và hai lịch.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** Thành công.

**Kỳ vọng DB gốc:** Chủ=P; TenNguoiHoc/TuoiNguoiHoc đúng; đủ liên kết môn/lịch.

**Quan sát:** HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/NhuCau"} Sau thao tác: 1 nhu cầu, 2 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need; SELECT * FROM dbo.[NhuCauMonHoc] WHERE NhuCauId=@Need; SELECT * FROM dbo.[KhungGioRanh] WHERE NhuCauId=@Need; SELECT * FROM dbo.[LuotDangNhuCau] WHERE TaiKhoanId=@Actor;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_d6fbfa56963f433ba94c28918c17af57`.

### IT-019 — Gia sư cố tạo nhu cầu (Pass)

**Tiêu chuẩn:** Xác thực.

**Điều kiện riêng:** Actor G; input nền hợp lệ.

**Test:** `Integration0203Tests.IT_019`; thời gian 00:00:00.9265415.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /NhuCau/NhuCau/Luu` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: Gia sư đăng nhập thật; token hợp lệ lấy từ trang hồ sơ có quyền truy cập; POST trả 403 và toàn bộ dữ liệu nghiệp vụ không đổi.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** 403 theo service.

**Kỳ vọng DB gốc:** Không tạo nhu cầu/lượt đăng.

**Quan sát:** HTTP 403, application/json; charset=utf-8 {"ok": false, "message": "Bạn không có quyền thực hiện thao tác này."} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need; SELECT * FROM dbo.[NhuCauMonHoc] WHERE NhuCauId=@Need; SELECT * FROM dbo.[KhungGioRanh] WHERE NhuCauId=@Need; SELECT * FROM dbo.[LuotDangNhuCau] WHERE TaiKhoanId=@Actor;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_73de9ec3fd064f328cbb5bf590a3511a`.

### IT-020 — Môn ngừng sử dụng (Pass)

**Tiêu chuẩn:** Đúng đắn.

**Điều kiện riêng:** M2.DangSuDung=0; input chọn M1/M2.

**Test:** `Integration0203Tests.IT_020`; thời gian 00:00:01.0361712.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /NhuCau/NhuCau/Luu` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: M2 ngừng sử dụng; POST bị từ chối 4xx có body lỗi; nhu cầu/môn/lịch/lượt đăng giữ nguyên.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** Từ chối dữ liệu; body có lý do.

**Kỳ vọng DB gốc:** Không lưu một phần N1 hoặc môn M1; transaction rollback.

**Quan sát:** HTTP 422, application/json; charset=utf-8 {"ok": false, "message": "Kiểm tra lại thông tin đã nhập.", "errors": {"": ["Môn học không hợp lệ."]}} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need; SELECT * FROM dbo.[NhuCauMonHoc] WHERE NhuCauId=@Need; SELECT * FROM dbo.[KhungGioRanh] WHERE NhuCauId=@Need; SELECT * FROM dbo.[LuotDangNhuCau] WHERE TaiKhoanId=@Actor;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_961ee4694b854d3999c416acf3eada36`.

### IT-021 — Update thay môn và lịch (Pass)

**Tiêu chuẩn:** Đầy đủ.

**Điều kiện riêng:** Id=N1, RV hiện tại; thay M1 bằng M2 và lịch mới.

**Test:** `Integration0203Tests.IT_021`; thời gian 00:00:01.2039120.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /NhuCau/NhuCau/Luu` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: Cập nhật N1 bằng RV hiện tại; chỉ còn M2, lịch thứ 2/4, RowVersion đổi; không cộng dồn liên kết/lịch cũ.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** Thành công.

**Kỳ vọng DB gốc:** Môn/lịch cũ được thay, không cộng dồn; RowVersion mới khác RV cũ.

**Quan sát:** HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/NhuCau"} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need; SELECT * FROM dbo.[NhuCauMonHoc] WHERE NhuCauId=@Need; SELECT * FROM dbo.[KhungGioRanh] WHERE NhuCauId=@Need; SELECT * FROM dbo.[LuotDangNhuCau] WHERE TaiKhoanId=@Actor;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_6ecf9728c35f4d05817a51377b288cbd`.

### IT-022 — Update sai chủ (Fail)

**Tiêu chuẩn:** Xác thực.

**Điều kiện riêng:** Actor X; Id=N1 của H; lấy RV hợp lệ từ fixture.

**Test:** `Integration0203Tests.IT_022`; thời gian 00:00:04.2051188.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /NhuCau/NhuCau/Luu` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: X gửi N1 của H cùng RV hợp lệ; phải 403, không đổi N1/môn/lịch/lượt đăng.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** 403; không vì có RV mà có quyền.

**Kỳ vọng DB gốc:** N1, môn và lịch nguyên trạng.

**Quan sát:** HTTP 422, application/json; charset=utf-8 {"ok": false, "message": "Kiểm tra lại thông tin đã nhập.", "errors": {"": ["Nhu cầu không thuộc về bạn."]}} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need; SELECT * FROM dbo.[NhuCauMonHoc] WHERE NhuCauId=@Need; SELECT * FROM dbo.[KhungGioRanh] WHERE NhuCauId=@Need; SELECT * FROM dbo.[LuotDangNhuCau] WHERE TaiKhoanId=@Actor;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_3186e850be6c43bd9a8562298f1e2cfb`.

### IT-023 — Update RV cũ (Fail)

**Tiêu chuẩn:** Đúng đắn.

**Điều kiện riêng:** Đọc RV1; lưu A tạo RV2; gửi B với RV1.

**Test:** `Integration0203Tests.IT_023`; thời gian 00:00:01.1587680.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /NhuCau/NhuCau/Luu` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: Lưu A tạo RV2; B giữ RV1; phải 409 và giữ nguyên bản A cùng môn/lịch sau A.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** B trả 409.

**Kỳ vọng DB gốc:** Dữ liệu A giữ nguyên; không xóa lịch/môn của A.

**Quan sát:** HTTP 422, application/json; charset=utf-8 {"ok": false, "message": "Kiểm tra lại thông tin đã nhập.", "errors": {"": ["Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu."]}} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need; SELECT * FROM dbo.[NhuCauMonHoc] WHERE NhuCauId=@Need; SELECT * FROM dbo.[KhungGioRanh] WHERE NhuCauId=@Need; SELECT * FROM dbo.[LuotDangNhuCau] WHERE TaiKhoanId=@Actor;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_e1f71b56776f471187866ebd982ef1d7`.

### IT-024 — Gửi lại cùng mã thao tác (Pass)

**Tiêu chuẩn:** Đúng đắn.

**Điều kiện riêng:** Cùng H, MaThaoTac và nhu cầu.

**Test:** `Integration0203Tests.IT_024`; thời gian 00:00:01.1005845.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /NhuCau/NhuCau/Luu hai lần` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: POST cùng actor/MaThaoTac hai lần; vẫn đúng một nhu cầu và một lượt đăng. Lần hai đọc lại cùng tập ID từ CSDL, HTTP thành công; JSON hiện chỉ chứa redirectUrl, không có trường ID riêng.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** Không tạo nhu cầu thứ hai; trả Id đã có.

**Kỳ vọng DB gốc:** COUNT lượt theo user/mã=1; không tăng hạn mức hai lần.

**Quan sát:** HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/NhuCau"} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need; SELECT * FROM dbo.[NhuCauMonHoc] WHERE NhuCauId=@Need; SELECT * FROM dbo.[KhungGioRanh] WHERE NhuCauId=@Need; SELECT * FROM dbo.[LuotDangNhuCau] WHERE TaiKhoanId=@Actor;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_13589cea7a824b7f9eec9b7011266635`.

Lưu ý: API hiện trả redirectUrl, không có trường ID riêng trong JSON; assertion ID ở ca này dựa trên ID duy nhất trong CSDL sau hai request. Ca không xác nhận một trường response ID chưa được định nghĩa trong schema phản hồi.

### IT-025 — Mã thao tác đã dùng cho nhu cầu khác (Fail)

**Tiêu chuẩn:** Đúng đắn.

**Điều kiện riêng:** MaThaoTac đã gắn N1 nhưng payload trỏ N2.

**Test:** `Integration0203Tests.IT_025`; thời gian 00:00:01.0387134.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /NhuCau/NhuCau/Luu` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: MaThaoTac của N1 dùng cập nhật N2; phải 409; N1/N2/lịch/môn/lượt đăng không đổi.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** 409.0

**Kỳ vọng DB gốc:** N1/N2 không bị ghi nhầm.

**Quan sát:** HTTP 422, application/json; charset=utf-8 {"ok": false, "message": "Kiểm tra lại thông tin đã nhập.", "errors": {"": ["Mã thao tác đã dùng cho nhu cầu khác."]}} Sau thao tác: 2 nhu cầu, 2 liên kết môn, 4 lịch, 2 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need; SELECT * FROM dbo.[NhuCauMonHoc] WHERE NhuCauId=@Need; SELECT * FROM dbo.[KhungGioRanh] WHERE NhuCauId=@Need; SELECT * FROM dbo.[LuotDangNhuCau] WHERE TaiKhoanId=@Actor;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_5b04c6e3f6a74f0093058041b68072c4`.

### IT-026 — Đóng nhu cầu (Pass)

**Tiêu chuẩn:** Đúng đắn.

**Điều kiện riêng:** id=N1; state=DaDong; RV hiện tại.

**Test:** `Integration0203Tests.IT_026`; thời gian 00:00:01.1106909.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /NhuCau/NhuCau/TrangThai` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: Đóng N1 bằng HTTP; DaDong; đủ lịch; truy vấn công khai service thật không còn đọc được N1.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** Thành công.

**Kỳ vọng DB gốc:** N1=DaDong; không còn trong truy vấn công khai; lịch sử/lịch không bị xóa.

**Quan sát:** HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/NhuCau"} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 2 lịch, 1 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need; SELECT * FROM dbo.[NhuCauMonHoc] WHERE NhuCauId=@Need; SELECT * FROM dbo.[KhungGioRanh] WHERE NhuCauId=@Need; SELECT * FROM dbo.[LuotDangNhuCau] WHERE TaiKhoanId=@Actor;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_902e1eac58924f2398b414940a0b36d5`.

### IT-027 — Mở lại nhu cầu đã đóng (Pass)

**Tiêu chuẩn:** Đúng đắn.

**Điều kiện riêng:** state=DangMo; N1 DaDong và đủ điều kiện/hạn mức.

**Test:** `Integration0203Tests.IT_027`; thời gian 00:00:01.1517100.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /NhuCau/NhuCau/TrangThai` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: Đóng N1 trước, lấy RV mới rồi mở; DangMo và có LuotDang.HanhDong=MoLai đúng H.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** Thành công.

**Kỳ vọng DB gốc:** DangMo, mốc xác nhận mới; LuotDang ghi MoLai đúng user.

**Quan sát:** HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/NhuCau"} Sau thao tác: 1 nhu cầu, 1 liên kết môn, 2 lịch, 2 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need; SELECT * FROM dbo.[NhuCauMonHoc] WHERE NhuCauId=@Need; SELECT * FROM dbo.[KhungGioRanh] WHERE NhuCauId=@Need; SELECT * FROM dbo.[LuotDangNhuCau] WHERE TaiKhoanId=@Actor;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_64bcda963d434bda8558a745d7911d9b`.

### IT-028 — Không đăng nhập gọi thao tác ghi (Pass)

**Tiêu chuẩn:** Xác thực.

**Điều kiện riêng:** Không gửi cookie phiên; lấy token antiforgery hợp lệ từ trang công khai nếu cần.

**Test:** `Integration0203Tests.IT_028`; thời gian 00:00:00.7879881.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /NhuCau/NhuCau/Luu` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: Client anonymous dùng token từ trang đăng nhập; AJAX POST trả 401; dữ liệu nghiệp vụ không đổi.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** AJAX 401; không chuyển HTML đăng nhập.

**Kỳ vọng DB gốc:** Không đổi dữ liệu nghiệp vụ mục tiêu. Nhật ký an toàn từ chối có thể được thêm.

**Quan sát:** HTTP 401, application/json; charset=utf-8 {"ok": false, "message": "Phiên đã hết hạn. Vui lòng đăng nhập lại."} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need; SELECT * FROM dbo.[NhuCauMonHoc] WHERE NhuCauId=@Need; SELECT * FROM dbo.[KhungGioRanh] WHERE NhuCauId=@Need; SELECT * FROM dbo.[LuotDangNhuCau] WHERE TaiKhoanId=@Actor;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_f432dea7b8c94aaeb0661f816f4f44dc`.

### IT-029 — Thiếu antiforgery token (Pass)

**Tiêu chuẩn:** Xác thực.

**Điều kiện riêng:** Đăng nhập đúng quyền; bỏ __RequestVerificationToken/header token, giữ cookie.

**Test:** `Integration0203Tests.IT_029`; thời gian 00:00:00.8036641.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /NhuCau/NhuCau/Luu` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: Client có cookie hợp lệ nhưng bỏ token; POST trả 400 từ antiforgery, không thay dữ liệu.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** HTTP 400 do antiforgery; không yêu cầu JSON theo schema của action.

**Kỳ vọng DB gốc:** Không thay đổi dữ liệu nghiệp vụ.

**Quan sát:** HTTP 400, None Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need; SELECT * FROM dbo.[NhuCauMonHoc] WHERE NhuCauId=@Need; SELECT * FROM dbo.[KhungGioRanh] WHERE NhuCauId=@Need; SELECT * FROM dbo.[LuotDangNhuCau] WHERE TaiKhoanId=@Actor;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_e60e8cada9984545971e74933297ed4b`.

## File thêm và hướng dẫn chạy lại

Thêm project `tests/Integration0203/Integration0203.csproj`, `IntegrationHarness.cs`, `IntegrationTests.cs`, `Cases.cs`. Harness dùng lại cơ chế hỗ trợ từ MT1 dưới tên độc lập, bổ sung guard tên DB, root cấu hình, cleanup lượt đăng/nhật ký/claims và fixture quản trị. Không sửa controller/service/SQL nguồn để làm Pass.

Mở Terminal trong thư mục dự án. CSDL kiểm thử đã được tạo và dữ liệu fixture nghiệp vụ đã dọn; không cần chạy SQL 00 lại trên DB đang có bảng. Với máy khác phải chuẩn bị database trống riêng và đổi connection đúng tên database được guard.

```powershell
$env:EDU_TEST_ROOT = "C:/Khóa luận TN/Educonnect"
$env:EDU_TEST_CONNECTION = "Server=LAPTOP-3L2V1AFT\SQLEXPRESS;Database=EduConnect_Verification_IT0203;Integrated Security=True;Encrypt=False;TrustServerCertificate=True"
dotnet restore tests/Integration0203/Integration0203.csproj --ignore-failed-sources
dotnet test tests/Integration0203/Integration0203.csproj --no-restore --filter "Suite=IT02NhuCau" -p:UseAppHost=false "-p:BaseOutputPath=$env:TEMP/EduConnect-IT0203/" --logger "trx;LogFileName=02-NhuCau-Integration.trx" --results-directory "TestResults/02-NhuCau-Integration"
```

Chạy một ca bằng filter `FullyQualifiedName~Integration0203Tests.IT_022` (thay đúng mã). Cả hai suite có ca Fail nên dotnet test trả exit code 1; không có lỗi build hoặc fixture trong TRX cuối. Chạy lại cập nhật TRX; MD/JSON là báo cáo của lần chạy ghi thời gian trong JSON, cần tạo lại khi kết quả đổi.

## Ghi nhận cài đặt SQL và tính nhất quán

Script 00 nguồn đã tạo 49 bảng, view và procedure cần dùng, nhưng đoạn bổ sung ở cuối còn `DROP CONSTRAINT CK_HoSoGiaSu_1` không có guard; constraint này đã được đổi thành `CK_HoSo_Workflow` ở phần trước nên đoạn cuối báo 3728/3727. Kiểm tra thấy workflow/visibility constraints vẫn tồn tại, đăng nhập và nghiệp vụ chạy được. Chỉ áp dụng script 17 bổ sung metadata lên database kiểm thử; không sửa file SQL nguồn và không sửa database ứng dụng. Đây là vấn đề của cài đặt schema, được ghi riêng, không gán thành Fail cho 26 ca nghiệp vụ đã thực thi.

Lần thử ban đầu có lỗi cleanup thiếu xóa LuotDangNhuCau trước NhuCauHoc, đã sửa trong harness và dọn chính xác các ID thuộc lần thử đó. Các báo cáo/TRX này chỉ chứa lần chạy cuối theo từng suite, không lấy lỗi harness làm lỗi sản phẩm.

Mỗi ca có đủ snapshot trước/sau, HTTP hoặc SqlException thật và cleanup thành công. Các bảng Markdown được kiểm số cột. SHA256 đối chiếu bản MD/JSON/TRX/ZIP; ZIP chứa mã test, SQL manifest và các đoạn nguồn liên quan tại thời điểm chạy. Các kết quả unit/MT trước không bị ghi đè.
