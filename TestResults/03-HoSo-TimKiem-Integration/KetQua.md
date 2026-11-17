# Kết quả tích hợp 03 Ho so Tim kiem

**12 ca: 10 Pass, 2 Fail, 0 Skip/Blocked.**

Đối chiếu đúng mã/thứ tự và kỳ vọng sheet `03 Ho so Tim kiem` trong `Integration Test.xlsx`. Không đổi kỳ vọng để phù hợp với code hiện tại.

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
| IT-030 | Lưu hồ sơ nhiều môn | Đầy đủ | Pass | HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/HoSoGiaSu/Sua"} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 2 dòng môn dạy của fixture. |
| IT-031 | Duyệt và công khai liên thông tìm kiếm | Đúng đắn | Pass | HTTP 200, text/html; charset=utf-8 Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. Trang 1/GET có 1 thẻ ứng viên. |
| IT-032 | Loại hồ sơ ẩn/chưa duyệt | Xác thực | Pass | HTTP 200, text/html; charset=utf-8 Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 3 hồ sơ, 3 dòng môn dạy của fixture. Trang 1/GET có 1 thẻ ứng viên. |
| IT-033 | Lọc phí đúng từng đơn vị | Đúng đắn | Pass | HTTP 200, text/html; charset=utf-8 Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 3 hồ sơ, 3 dòng môn dạy của fixture. Trang 1/GET có 1 thẻ ứng viên. |
| IT-034 | Phân trang ổn định | Đầy đủ | Fail | HTTP 200, text/html; charset=utf-8 Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 23 hồ sơ, 23 dòng môn dạy của fixture. Trang 1/GET có 9 thẻ ứng viên. Trang 2 có 9 thẻ; ID trang 1=9, trang 2=9, giao nhau=0. |
| IT-035 | Tham số Trang sai kiểu | Đầy đủ | Pass | HTTP 400, application/json; charset=utf-8 {"ok": false, "message": "Trang chưa đúng định dạng.", "errors": {"": ["Trang chưa đúng định dạng."]}} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-036 | Lọc lịch rảnh hồ sơ theo procedure cũ | Đúng đắn | Pass | SQL 51202: Hồ sơ gia sư không dùng bộ lọc lịch rảnh. Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-037 | So sánh 1 ID phân biệt | Đầy đủ | Fail | HTTP 400, application/json; charset=utf-8 {"ok": false, "message": "Chọn từ 2 đến 3 đối tượng để so sánh.", "errors": {"": ["Chọn từ 2 đến 3 đối tượng để so sánh."]}} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |
| IT-038 | So sánh 3 ID phân biệt | Đầy đủ | Pass | HTTP 200, text/html; charset=utf-8 Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 4 hồ sơ, 4 dòng môn dạy của fixture. |
| IT-039 | So sánh 4 ID phân biệt | Đầy đủ | Pass | HTTP 400, application/json; charset=utf-8 {"ok": false, "message": "Chọn từ 2 đến 3 đối tượng để so sánh.", "errors": {"": ["Chọn từ 2 đến 3 đối tượng để so sánh."]}} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 4 hồ sơ, 4 dòng môn dạy của fixture. |
| IT-040 | So sánh loại ID trùng | Đúng đắn | Pass | HTTP 200, text/html; charset=utf-8 Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 4 hồ sơ, 4 dòng môn dạy của fixture. |
| IT-041 | Chi tiết đối tượng hết công khai | Xác thực | Pass | HTTP 404, application/json; charset=utf-8 {"ok": false, "message": "Hồ sơ hoặc nhu cầu đang ẩn, chưa được duyệt hoặc không còn công khai.", "errors": {"": ["Hồ sơ hoặc nhu cầu đang ẩn, chưa được duyệt hoặc không còn công khai."]}} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. |

## Chi tiết ca thất bại

### IT-034 — Phân trang ổn định

**Điểm vào:** `GET /TimKiem/TimKiem/Tim`.

**Dữ liệu:** 23 hồ sơ cùng điều kiện; Trang=1 rồi 2; KichThuocTrang=20. Tag=IT0203IT_bca1919228814849ad7efe1883c7a924; actor/ID cụ thể xem observation trong JSON.

**Kỳ vọng HTTP:** 20 và 3 ứng viên; tổng 23; không trùng ID giữa trang.

**Kỳ vọng dữ liệu:** Không có ghi DB nghiệp vụ.

**Thực tế:** HTTP 200, text/html; charset=utf-8 Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 23 hồ sơ, 23 dòng môn dạy của fixture. Trang 1/GET có 9 thẻ ứng viên. Trang 2 có 9 thẻ; ID trang 1=9, trang 2=9, giao nhau=0.

**Các sai lệch đã assert:**

- Trang 1 phải có 20 ứng viên, thực tế 9.
- Trang 2 phải có 3 ứng viên, thực tế 9.
- Phân trang trùng/thiếu ID hoặc tổng không bằng 23.

**Nguyên nhân trong code:** BoLocTimKiemViewModel không khai báo KichThuocTrang nên tham số query KichThuocTrang=20 không điều khiển được truy vấn. EfTimKiemStore.TimHoSoAsync dùng cố định Skip((Trang-1)*9).Take(9). Vì vậy 23 hồ sơ được chia 9/9/5 thay vì 20/3 theo TC.

**Vị trí:** `Areas/TimKiem/Services/EfTimKiemStore.cs:58`; `Areas/TimKiem/ViewModels/SearchModels.cs:6`.

**Ảnh hưởng/phạm vi kết luận:** Hai trang trả tổng 18/23 hồ sơ thay vì đủ 23 trên hai trang mong đợi. Không quan sát trùng ID giữa hai trang; không nên mô tả lỗi này thành nhân đôi dữ liệu hoặc mất hồ sơ trong DB.

**Đề xuất sửa:** Đồng bộ kích thước trang với thiết kế: khai báo/validate KichThuocTrang, dùng cùng giá trị trong Skip/Take và UI/Page metadata; nếu thiết kế cố định 20 thì dùng thống nhất 20 thay vì 9. Không đổi TC thành 9 chỉ để Pass.

**Body phản hồi liên quan:**

```text
HTTP 200; page1Cards=9; page2Cards=9; page1Ids=['19d1f00a-5a2a-40f4-9491-0c922a942899', '9a2c2d1f-fe22-459f-81b6-2897cfb658b8', '8a7c1b09-647f-4e62-9ae7-1b442b88a5b8', '93a6f618-5103-48c9-a45a-8451c9b128b0', 'a4e197f3-1b0c-415d-9af7-e60fc3001815', '30cfdf05-0468-43ce-a8a3-978210a09cbd', 'bf7225c5-cf3a-4d93-8e14-1594faa47ff7', '8e74a76f-ecde-40b6-a631-4f74957b4f8d', '0c7827e3-d389-49a5-8ed6-fa2eefa51511']; page2Ids=['ded89d54-aa03-4d2e-b34a-a57519a1e7f5', 'b4f2b426-ab67-4dad-85f8-a6096a88b256', '7262f71e-d546-4567-9b84-3dab4e9849b6', 'ad919e87-6ff8-4420-b104-61bd65567110', '2fa6bc5b-fda7-471f-bbfb-0fc4a4c5c21c', 'bb712ab5-4bbe-4b60-8efd-5e53a7e96135', '51839706-ed8f-4f5c-ada3-849b83c78522', '7f704e83-cfd7-4fa2-8440-aa467b81bf23', 'cf2f7f33-760a-41fe-9de1-40a1f893439f']
```

### IT-037 — So sánh 1 ID phân biệt

**Điểm vào:** `GET /TimKiem/TimKiem/SoSanh`.

**Dữ liệu:** Query lặp ids=<Guid> với 1 ID; X-Edu-Modal=true. Tag=IT0203IT_6be361f6cbfc446999b3e734011b1520; actor/ID cụ thể xem observation trong JSON.

**Kỳ vọng HTTP:** 200 HTML partial, không layout đầy đủ.

**Kỳ vọng dữ liệu:** Không ghi dữ liệu; đủ đối tượng còn công khai.

**Thực tế:** HTTP 400, application/json; charset=utf-8 {"ok": false, "message": "Chọn từ 2 đến 3 đối tượng để so sánh.", "errors": {"": ["Chọn từ 2 đến 3 đối tượng để so sánh."]}} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Các sai lệch đã assert:**

- HTTP kỳ vọng 200, thực tế 400.
- So sánh phải trả HTML.
- Thiếu đối tượng A trong bảng so sánh.

**Nguyên nhân trong code:** TimKiemService.SoSanhAsync Distinct các ID rồi yêu cầu ids.Count từ 2 đến 3. Một ID hợp lệ bị chặn ngay ở guard với lỗi 400, trước khi đọc hồ sơ và render partial HTML.

**Vị trí:** `Areas/TimKiem/Services/TimKiemService.cs:68`.

**Ảnh hưởng/phạm vi kết luận:** Luồng so sánh một hồ sơ trong kịch bản không mở được; trả JSON lỗi thay cho HTML modal. Dữ liệu không đổi và không có bằng chứng lộ hồ sơ riêng tư từ ca này.

**Đề xuất sửa:** Đồng bộ guard với thiết kế cho phép 1–3 ID phân biệt, đồng thời giữ giới hạn 4 ID và bước Distinct. Nếu sản phẩm muốn tối thiểu 2 thì cần sửa yêu cầu có phê duyệt; kết quả hiện tại vẫn Fail theo Excel.

**Body phản hồi liên quan:**

```json
{
  "ok": false,
  "message": "Chọn từ 2 đến 3 đối tượng để so sánh.",
  "errors": {
    "": [
      "Chọn từ 2 đến 3 đối tượng để so sánh."
    ]
  }
}
```

## Các bước và bằng chứng từng ca

### IT-030 — Lưu hồ sơ nhiều môn (Pass)

**Tiêu chuẩn:** Đầy đủ.

**Điều kiện riêng:** G; môn/phí/hình thức đầy đủ; dùng ViewModel thật.

**Test:** `Integration0203Tests.IT_030`; thời gian 00:00:00.9990681.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /NhuCau/HoSoGiaSu/Luu` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: POST HoSoInput thật với hai MonDay/phí; BanNhap/ẩn, đủ hai môn/phí, không tự duyệt.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** Lưu thành công.

**Kỳ vọng DB gốc:** Đúng môn/phí, workflow trạng thái theo thao tác; không tự duyệt.

**Quan sát:** HTTP 200, application/json; charset=utf-8 {"ok": true, "message": "Đã lưu thay đổi.", "redirectUrl": "/NhuCau/HoSoGiaSu/Sua"} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 2 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[HoSoGiaSu] WHERE Id=@Profile; SELECT * FROM dbo.[HoSoGiaSuMonDay] WHERE HoSoGiaSuId=@Profile; SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_c64b34d3174247beaf8de4d0c66b0830`.

### IT-031 — Duyệt và công khai liên thông tìm kiếm (Pass)

**Tiêu chuẩn:** Đúng đắn.

**Điều kiện riêng:** Admin DuyetHoSo; approve=true, RV hiện tại; chủ bật hiển thị hợp lệ.

**Test:** `Integration0203Tests.IT_031`; thời gian 00:00:01.8891269.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `POST /QuanTri/HoSo/Duyet rồi GET /TimKiem/TimKiem/Tim` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: Admin có claim DuyetHoSo duyệt qua HTTP; G bật Hien bằng RV mới; H tìm thấy đúng một hồ sơ DaDuyet/công khai, không có nhu cầu NhanLop cũ.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** Kết quả tìm kiếm chỉ trả hồ sơ đã đủ điều kiện công khai.

**Kỳ vọng DB gốc:** DaDuyet; DangHienThi đúng; không cần nhu cầu NhanLop kiểu cũ.

**Quan sát:** HTTP 200, text/html; charset=utf-8 Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture. Trang 1/GET có 1 thẻ ứng viên.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[HoSoGiaSu] WHERE Id=@Profile; SELECT * FROM dbo.[HoSoGiaSuMonDay] WHERE HoSoGiaSuId=@Profile; SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_75cffe72cb1d478b9b4f09cb118c8bf9`.

### IT-032 — Loại hồ sơ ẩn/chưa duyệt (Pass)

**Tiêu chuẩn:** Xác thực.

**Điều kiện riêng:** H tìm M1; H1 công khai, H2 chờ duyệt, H3 ẩn.

**Test:** `Integration0203Tests.IT_032`; thời gian 00:00:01.2630287.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `GET /TimKiem/TimKiem/Tim` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: H tìm M1; chỉ hồ sơ công khai xuất hiện, hồ sơ ChoDuyet/ẩn không xuất hiện; GET không đổi dữ liệu nghiệp vụ.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** 200; chỉ H1 trong ứng viên phù hợp.

**Kỳ vọng DB gốc:** Không thay đổi hồ sơ; không lộ H2/H3.

**Quan sát:** HTTP 200, text/html; charset=utf-8 Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 3 hồ sơ, 3 dòng môn dạy của fixture. Trang 1/GET có 1 thẻ ứng viên.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[HoSoGiaSu] WHERE Id=@Profile; SELECT * FROM dbo.[HoSoGiaSuMonDay] WHERE HoSoGiaSuId=@Profile; SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_ff42b32dc9a54e7d96ba162af23d8220`.

### IT-033 — Lọc phí đúng từng đơn vị (Pass)

**Tiêu chuẩn:** Đúng đắn.

**Điều kiện riêng:** M1; HocPhiToiDa=200000; DonViHocPhi=VND/Gio.

**Test:** `Integration0203Tests.IT_033`; thời gian 00:00:01.0706110.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `GET /TimKiem/TimKiem/Tim` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: Ba hồ sơ 150000/Gio, 150000/Buoi, 250000/Gio; lọc 200000/Gio chỉ trả hồ sơ đầu; dữ liệu không đổi.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** Chỉ ứng viên có môn/phí phù hợp cùng đơn vị.

**Kỳ vọng DB gốc:** Không so sánh VND/Buoi trực tiếp với VND/Gio.

**Quan sát:** HTTP 200, text/html; charset=utf-8 Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 3 hồ sơ, 3 dòng môn dạy của fixture. Trang 1/GET có 1 thẻ ứng viên.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[HoSoGiaSu] WHERE Id=@Profile; SELECT * FROM dbo.[HoSoGiaSuMonDay] WHERE HoSoGiaSuId=@Profile; SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_4ffc706879f04076b997a6693548b350`.

### IT-034 — Phân trang ổn định (Fail)

**Tiêu chuẩn:** Đầy đủ.

**Điều kiện riêng:** 23 hồ sơ cùng điều kiện; Trang=1 rồi 2; KichThuocTrang=20.

**Test:** `Integration0203Tests.IT_034`; thời gian 00:00:02.9716683.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `GET /TimKiem/TimKiem/Tim` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: 23 hồ sơ hợp lệ; GET Trang=1/2 với KichThuocTrang=20; cần 20/3, đủ 23 ID không trùng, tổng 23.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** 20 và 3 ứng viên; tổng 23; không trùng ID giữa trang.

**Kỳ vọng DB gốc:** Không có ghi DB nghiệp vụ.

**Quan sát:** HTTP 200, text/html; charset=utf-8 Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 23 hồ sơ, 23 dòng môn dạy của fixture. Trang 1/GET có 9 thẻ ứng viên. Trang 2 có 9 thẻ; ID trang 1=9, trang 2=9, giao nhau=0.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[HoSoGiaSu] WHERE Id=@Profile; SELECT * FROM dbo.[HoSoGiaSuMonDay] WHERE HoSoGiaSuId=@Profile; SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_bca1919228814849ad7efe1883c7a924`.

KichThuocTrang=20 được gửi đúng như Excel dù ViewModel hiện chưa khai báo thuộc tính này. Bộ test giữ kỳ vọng 20/3 và ghi Fail; không tự đổi thành 9/9.

### IT-035 — Tham số Trang sai kiểu (Pass)

**Tiêu chuẩn:** Đầy đủ.

**Điều kiện riêng:** AJAX; input sai kiểu binding.

**Test:** `Integration0203Tests.IT_035`; thời gian 00:00:00.7400973.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `GET /TimKiem/TimKiem/Tim?Trang=abc` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: AJAX GET Trang=abc; HTTP 400 và DB nghiệp vụ không đổi.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** 400 theo nhánh GET; không 500 hoặc bỏ lọc im lặng.

**Kỳ vọng DB gốc:** Không đổi DB.

**Quan sát:** HTTP 400, application/json; charset=utf-8 {"ok": false, "message": "Trang chưa đúng định dạng.", "errors": {"": ["Trang chưa đúng định dạng."]}} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[HoSoGiaSu] WHERE Id=@Profile; SELECT * FROM dbo.[HoSoGiaSuMonDay] WHERE HoSoGiaSuId=@Profile; SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_712c38e9f5864bb8a39559fc77f07532`.

### IT-036 — Lọc lịch rảnh hồ sơ theo procedure cũ (Pass)

**Tiêu chuẩn:** Đúng đắn.

**Điều kiện riêng:** Procedure cuối không hỗ trợ lịch rảnh gia sư.

**Test:** `Integration0203Tests.IT_036`; thời gian 00:00:00.4617306.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `EXEC dbo.usp_TimKiemGiaSu @ThuTrongTuan=1` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: SQL thật EXEC usp_TimKiemGiaSu @ThuTrongTuan=1 phát sinh SqlException.Number=51202; không ghi DB.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** SQL 51202.

**Kỳ vọng DB gốc:** Không ghi dữ liệu.

**Quan sát:** SQL 51202: Hồ sơ gia sư không dùng bộ lọc lịch rảnh. Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[HoSoGiaSu] WHERE Id=@Profile; SELECT * FROM dbo.[HoSoGiaSuMonDay] WHERE HoSoGiaSuId=@Profile; SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_11f800acbf5b424099d52367cc35c81b`.

### IT-037 — So sánh 1 ID phân biệt (Fail)

**Tiêu chuẩn:** Đầy đủ.

**Điều kiện riêng:** Query lặp ids=<Guid> với 1 ID; X-Edu-Modal=true.

**Test:** `Integration0203Tests.IT_037`; thời gian 00:00:00.6978063.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `GET /TimKiem/TimKiem/SoSanh` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: Một ID công khai + X-Edu-Modal=true; cần 200 text/html partial chứa đối tượng, không layout, không ghi DB.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** 200 HTML partial, không layout đầy đủ.

**Kỳ vọng DB gốc:** Không ghi dữ liệu; đủ đối tượng còn công khai.

**Quan sát:** HTTP 400, application/json; charset=utf-8 {"ok": false, "message": "Chọn từ 2 đến 3 đối tượng để so sánh.", "errors": {"": ["Chọn từ 2 đến 3 đối tượng để so sánh."]}} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[HoSoGiaSu] WHERE Id=@Profile; SELECT * FROM dbo.[HoSoGiaSuMonDay] WHERE HoSoGiaSuId=@Profile; SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_6be361f6cbfc446999b3e734011b1520`.

### IT-038 — So sánh 3 ID phân biệt (Pass)

**Tiêu chuẩn:** Đầy đủ.

**Điều kiện riêng:** Query lặp ids=<Guid> với 3 ID; X-Edu-Modal=true.

**Test:** `Integration0203Tests.IT_038`; thời gian 00:00:03.7168185.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `GET /TimKiem/TimKiem/SoSanh` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: Ba ID công khai + X-Edu-Modal=true; 200 HTML partial có đủ A/B/C, không layout, không ghi DB.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** 200 HTML partial, không layout đầy đủ.

**Kỳ vọng DB gốc:** Không ghi dữ liệu; đủ đối tượng còn công khai.

**Quan sát:** HTTP 200, text/html; charset=utf-8 Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 4 hồ sơ, 4 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[HoSoGiaSu] WHERE Id=@Profile; SELECT * FROM dbo.[HoSoGiaSuMonDay] WHERE HoSoGiaSuId=@Profile; SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_a4251c70b47640c2bffe4abaa80da213`.

### IT-039 — So sánh 4 ID phân biệt (Pass)

**Tiêu chuẩn:** Đầy đủ.

**Điều kiện riêng:** Query lặp ids=<Guid> với 4 ID; X-Edu-Modal=true.

**Test:** `Integration0203Tests.IT_039`; thời gian 00:00:01.1689725.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `GET /TimKiem/TimKiem/SoSanh` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: Bốn ID phân biệt; 400; không trả bảng bốn đối tượng, không ghi DB.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** 400 lỗi số lượng.

**Kỳ vọng DB gốc:** Không ghi dữ liệu; không trả bảng 4 cột.

**Quan sát:** HTTP 400, application/json; charset=utf-8 {"ok": false, "message": "Chọn từ 2 đến 3 đối tượng để so sánh.", "errors": {"": ["Chọn từ 2 đến 3 đối tượng để so sánh."]}} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 4 hồ sơ, 4 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[HoSoGiaSu] WHERE Id=@Profile; SELECT * FROM dbo.[HoSoGiaSuMonDay] WHERE HoSoGiaSuId=@Profile; SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_961c9950305d491c8499e1ad20d1ff40`.

### IT-040 — So sánh loại ID trùng (Pass)

**Tiêu chuẩn:** Đúng đắn.

**Điều kiện riêng:** ids=A,A,B,C.

**Test:** `Integration0203Tests.IT_040`; thời gian 00:00:01.1514852.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `GET /TimKiem/TimKiem/SoSanh` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: ids=A,A,B,C; 200 partial chứa A/B/C sau Distinct, không ghi DB.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** 200; ba đối tượng phân biệt.

**Kỳ vọng DB gốc:** Không ghi; không nhân đôi ứng viên.

**Quan sát:** HTTP 200, text/html; charset=utf-8 Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 4 hồ sơ, 4 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[HoSoGiaSu] WHERE Id=@Profile; SELECT * FROM dbo.[HoSoGiaSuMonDay] WHERE HoSoGiaSuId=@Profile; SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_9f3b377009c24b5a9cafed54fc2ffe03`.

### IT-041 — Chi tiết đối tượng hết công khai (Pass)

**Tiêu chuẩn:** Xác thực.

**Điều kiện riêng:** Hồ sơ đã bị ẩn sau lần tìm kiếm.

**Test:** `Integration0203Tests.IT_041`; thời gian 00:00:00.8540336.

1. Khởi tạo fixture mới trong database kiểm thử; tạo Identity user/role, hai môn đang dùng, hồ sơ cơ sở. Ghi server/database và tag; cố định danh tính bằng tài khoản riêng, không dùng account của người dùng.
2. Thiết lập điều kiện riêng ở trên. Nếu sửa/đóng/mở nhu cầu, tạo N1 bằng service thật rồi đọc ID/RowVersion từ SQL. Nếu kiểm concurrency, lưu A thật trước khi gửi B với RV1 cũ. Nếu tìm kiếm, seed đủ hồ sơ/phí/trạng thái được yêu cầu.
3. Đăng nhập thật và lấy cookie/phiên SQL. GET trang có quyền truy cập để lấy token antiforgery cùng client. Ca gia sư bị cấm tạo nhu cầu lấy token từ trang hồ sơ; anonymous lấy từ trang đăng nhập. IT-029 cố ý bỏ token.
4. Gửi `GET /TimKiem/TimKiem/ChiTiet/@Profile` với dữ liệu ca và header AJAX/modal nếu cần; không tự theo redirect. IT-036 chạy procedure SQL thật, không gửi HTTP.
5. Lưu status, Content-Type, Location, body đã che token; đọc lại nhu cầu/môn/lịch/lượt đăng/hồ sơ từ DbContext mới. Với ca read-only hoặc bị từ chối, so sánh snapshot nghiệp vụ trước/sau; nhật ký an toàn không thuộc tập cấm ghi.
6. Assert cụ thể: Ẩn hồ sơ sau fixture; GET ChiTiet trả 404 và không đổi dữ liệu nghiệp vụ.
7. Ghi OBSERVATION_JSON và lỗi assertion vào TRX, rồi dừng test host và xóa đúng dữ liệu sở hữu theo thứ tự FK. Không dùng transaction của test để giả định HTTP tự rollback.

**Kỳ vọng HTTP gốc:** 404, không trả nội dung riêng tư.

**Kỳ vọng DB gốc:** Không đổi DB.

**Quan sát:** HTTP 404, application/json; charset=utf-8 {"ok": false, "message": "Hồ sơ hoặc nhu cầu đang ẩn, chưa được duyệt hoặc không còn công khai.", "errors": {"": ["Hồ sơ hoặc nhu cầu đang ẩn, chưa được duyệt hoặc không còn công khai."]}} Sau thao tác: 0 nhu cầu, 0 liên kết môn, 0 lịch, 0 lượt đăng, 1 hồ sơ, 1 dòng môn dạy của fixture.

**Dọn fixture:** Đã dọn fixture theo đúng Id sở hữu; không xóa dữ liệu ngoài ca.

**Truy vấn kiểm tra theo kịch bản (placeholder được đối chiếu bằng ID fixture):**

```sql
SELECT * FROM dbo.[HoSoGiaSu] WHERE Id=@Profile; SELECT * FROM dbo.[HoSoGiaSuMonDay] WHERE HoSoGiaSuId=@Profile; SELECT * FROM dbo.[NhuCauHoc] WHERE Id=@Need;
```

ID trong JSON: `observation.actors`, `profiles`, `target`, `input`; dữ liệu trước/sau ở `beforeOperation`, `targetBefore`, `savedA`, `after`. Trường không áp dụng cho ca sẽ không xuất hiện. Tag ca: `IT0203IT_3d06271879e24bf0a4c27fbf4cf45c62`.

## File thêm và hướng dẫn chạy lại

Thêm project `tests/Integration0203/Integration0203.csproj`, `IntegrationHarness.cs`, `IntegrationTests.cs`, `Cases.cs`. Harness dùng lại cơ chế hỗ trợ từ MT1 dưới tên độc lập, bổ sung guard tên DB, root cấu hình, cleanup lượt đăng/nhật ký/claims và fixture quản trị. Không sửa controller/service/SQL nguồn để làm Pass.

Mở Terminal trong thư mục dự án. CSDL kiểm thử đã được tạo và dữ liệu fixture nghiệp vụ đã dọn; không cần chạy SQL 00 lại trên DB đang có bảng. Với máy khác phải chuẩn bị database trống riêng và đổi connection đúng tên database được guard.

```powershell
$env:EDU_TEST_ROOT = "C:/Khóa luận TN/Educonnect"
$env:EDU_TEST_CONNECTION = "Server=LAPTOP-3L2V1AFT\SQLEXPRESS;Database=EduConnect_Verification_IT0203;Integrated Security=True;Encrypt=False;TrustServerCertificate=True"
dotnet restore tests/Integration0203/Integration0203.csproj --ignore-failed-sources
dotnet test tests/Integration0203/Integration0203.csproj --no-restore --filter "Suite=IT03HoSoTimKiem" -p:UseAppHost=false "-p:BaseOutputPath=$env:TEMP/EduConnect-IT0203/" --logger "trx;LogFileName=03-HoSo-TimKiem-Integration.trx" --results-directory "TestResults/03-HoSo-TimKiem-Integration"
```

Chạy một ca bằng filter `FullyQualifiedName~Integration0203Tests.IT_022` (thay đúng mã). Cả hai suite có ca Fail nên dotnet test trả exit code 1; không có lỗi build hoặc fixture trong TRX cuối. Chạy lại cập nhật TRX; MD/JSON là báo cáo của lần chạy ghi thời gian trong JSON, cần tạo lại khi kết quả đổi.

## Ghi nhận cài đặt SQL và tính nhất quán

Script 00 nguồn đã tạo 49 bảng, view và procedure cần dùng, nhưng đoạn bổ sung ở cuối còn `DROP CONSTRAINT CK_HoSoGiaSu_1` không có guard; constraint này đã được đổi thành `CK_HoSo_Workflow` ở phần trước nên đoạn cuối báo 3728/3727. Kiểm tra thấy workflow/visibility constraints vẫn tồn tại, đăng nhập và nghiệp vụ chạy được. Chỉ áp dụng script 17 bổ sung metadata lên database kiểm thử; không sửa file SQL nguồn và không sửa database ứng dụng. Đây là vấn đề của cài đặt schema, được ghi riêng, không gán thành Fail cho 26 ca nghiệp vụ đã thực thi.

Lần thử ban đầu có lỗi cleanup thiếu xóa LuotDangNhuCau trước NhuCauHoc, đã sửa trong harness và dọn chính xác các ID thuộc lần thử đó. Các báo cáo/TRX này chỉ chứa lần chạy cuối theo từng suite, không lấy lỗi harness làm lỗi sản phẩm.

Mỗi ca có đủ snapshot trước/sau, HTTP hoặc SqlException thật và cleanup thành công. Các bảng Markdown được kiểm số cột. SHA256 đối chiếu bản MD/JSON/TRX/ZIP; ZIP chứa mã test, SQL manifest và các đoạn nguồn liên quan tại thời điểm chạy. Các kết quả unit/MT trước không bị ghi đè.
