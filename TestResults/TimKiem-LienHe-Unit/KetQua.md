# Kết quả unit test — Tìm kiếm và lời mời liên hệ

Ngày thực hiện: 14/11/2026 (Asia/Saigon).

**44 ca được phát hiện: 34 Pass, 8 Fail, 2 Skip. Có 42 ca đã thực thi.**

| Phạm vi | Tổng | Pass | Fail | Skip |
|---|---:|---:|---:|---:|
| Tìm kiếm và lựa chọn: UT-021–046, UT-047–049 | 29 | 21 | 7 | 1 |
| Lời mời liên hệ: UT-050–063, UT-064 | 15 | 13 | 1 | 1 |

8 Fail là kết quả assertion so với bảng Unit cũ; KHÔNG đồng nghĩa 8 lỗi ứng dụng đã được xác nhận. Một số ca có yêu cầu không tồn tại trong DTO hoặc mâu thuẫn với MT1. Xem phân loại ở dưới.

## Phạm vi và cách thực hiện

- Mã hiện hành theo Unit Test.xlsx cập nhật: Tìm kiếm UT-021–049; Lời mời liên hệ UT-050–064. Các mã là duy nhất trong project test. Phần kết quả Nhu cầu được giữ nguyên.
- Tạo service/controller thật; mock strict ITimKiemStore, ILienHeStore, các service phụ thuộc và nguồn người dùng. Không mock phương thức đang kiểm thử.
- Mỗi test có fixture riêng; dùng token/context/TempData khi cần. DbContext không có provider trong các fixture service; không kết nối CSDL, không chạy app/server, không sử dụng dữ liệu production.
- Controller unit gọi action trực tiếp, không chạy routing, binding, authorization, antiforgery, filter hoặc middleware.
- Không thay giới hạn so sánh, cách xử lý lỗi hoặc thông điệp sản phẩm để làm test Pass.

## Kết quả từng ca

| Mã | Tình huống | Kết quả | Diễn giải |
|---|---|---|---|
| UT-021 | Học sinh tìm đúng loại đối tượng | Pass | Học sinh dùng nhánh đọc hồ sơ, đúng dữ liệu, không gọi nhánh nhu cầu. |
| UT-022 | Phụ huynh tìm đúng loại đối tượng | Pass | Phụ huynh dùng nhánh hồ sơ, đúng dữ liệu, không gọi nhánh nhu cầu. |
| UT-023 | Gia sư tìm đúng loại đối tượng | Pass | Gia sư dùng nhánh nhu cầu, truyền đúng viewer/token, không gọi nhánh hồ sơ. |
| UT-024 | Phí tối đa âm | Pass | Phí âm: lỗi 400, không truy vấn store. |
| UT-025 | Đơn vị phí không hợp lệ | Pass | Đơn vị sai: lỗi 400, không truy vấn store. |
| UT-026 | Số trang không hợp lệ | Pass | Trang 0: lỗi 400, không truy vấn store. |
| UT-027 | Ánh xạ phân trang | Fail | Hai dòng, tổng 12, Trang=2 đều đạt; KichThuocTrang thực tế là 9. |
| UT-028 | Ánh xạ điểm phù hợp | Skip / Blocked | Chưa có DiemPhuHop hoặc bộ tính điểm theo bảng gốc; MT1 lại quy định lý do gợi ý, không phần trăm. Cần chốt lại yêu cầu. |
| UT-029 | Có kết quả | Pass | View Index giữ 2 ứng viên, Total=12, Trang=2 và bộ lọc. |
| UT-030 | Danh sách rỗng | Pass | Model rỗng, Total=0, không exception; thông điệp Razor cần kiểm ở UI/integration. |
| UT-031 | Tiêu chí lỗi | Fail | Ném LoiNghiepVu “Tiêu chí không hợp lệ.” khi đọc DuLieu; không trả ViewResult. |
| UT-032 | Truyền bộ lọc mới | Fail | Các trường đang có và actor/token truyền đúng; reflection không tìm thấy thuộc tính KichThuocTrang. |
| UT-033 | Hồ sơ công khai tồn tại | Pass | Đọc đúng hồ sơ công khai, không đọc fallback nhu cầu. |
| UT-034 | Nhu cầu công khai tồn tại | Pass | Không có hồ sơ: đọc đúng nhu cầu công khai. |
| UT-035 | Không có đối tượng công khai | Pass | Không có cả hai loại: lỗi 404, không trả model riêng tư. |
| UT-036 | Chi tiết khi chưa có liên hệ | Pass | Chưa có liên hệ: Contact/Chat null, Related đúng; không gọi HoiThoaiAsync. |
| UT-037 | Chi tiết khi đã chấp nhận | Pass | Liên hệ ChapNhan: model/contact/related và ID hội thoại đúng. |
| UT-038 | Chấp nhận 1 ID phân biệt | Fail | KetQua lỗi 400, “Chọn từ 2 đến 3 đối tượng để so sánh.”; không đọc đối tượng. |
| UT-039 | Chấp nhận 2 ID phân biệt | Pass | Hai ID hợp lệ: đúng hai model, mỗi ID đọc một lần. |
| UT-040 | Chấp nhận 3 ID phân biệt | Pass | Ba ID hợp lệ: đúng ba model, mỗi ID đọc một lần. |
| UT-041 | Từ chối 0 ID phân biệt | Pass | Không ID: lỗi số lượng, không đọc store. |
| UT-042 | Từ chối 4 ID phân biệt | Pass | Bốn ID phân biệt: lỗi số lượng, không đọc store. |
| UT-043 | Loại trùng trước đếm | Pass | A/A/B/C: đếm sau Distinct, đọc A/B/C mỗi ID một lần. |
| UT-044 | Loại đối tượng hết công khai | Fail | KetQua lỗi 404 khi đọc B, dừng cả phép so sánh. |
| UT-045 | Kết quả so sánh hợp lệ | Pass | Trả view so sánh mặc định với danh sách A/B. Không ép model phải là SoSanhViewModel vì kịch bản không quy định kiểu cụ thể. |
| UT-046 | Kết quả so sánh lỗi | Fail | Ném LoiNghiepVu “Số lượng không hợp lệ.”; không trả Index. |
| UT-047 | Loại nhu cầu đang xem | Fail | Service trả cả N1 và N2 theo danh sách public cùng owner do store cung cấp. |
| UT-048 | Đủ thông tin hồ sơ chi tiết | Pass | Mapper dùng trong production giữ đủ thông tin hồ sơ và hai môn/học phí/đơn vị riêng biệt. Không xác nhận join SQL trong unit. |
| UT-049 | Xem không ghi dữ liệu | Pass | Chỉ đọc chi tiết A/B đúng một lần, không lời gọi store khác; DbContext không có provider nên đường test không thể truy vấn DB. |
| UT-050 | Lỗi dữ liệu tại controller | Fail | Không gọi service (đạt); ValidateInput ném LoiNghiepVu với Status=400, không trả ObjectResult 422. |
| UT-051 | Lời nhắn dài 1 | Pass | Một ký tự: lưu đúng lời nhắn, chủ thể và trạng thái; Notify đúng recipient/target. |
| UT-052 | Lời nhắn dài 256 | Pass | 256 ký tự: lưu nguyên nội dung, đúng recipient/target. |
| UT-053 | Lời nhắn rỗng | Pass | Khoảng trắng: lỗi 400 trước đọc người dùng/store; không ghi hoặc commit. |
| UT-054 | Vượt 256 ký tự | Pass | 257 ký tự: lỗi 400 trước đọc người dùng/store; không ghi hoặc commit. |
| UT-055 | Tự gửi liên hệ | Pass | Tự gửi: lỗi 400, không mở transaction hoặc ghi. |
| UT-056 | Không bên nào là gia sư | Pass | Học sinh và phụ huynh: lỗi 400, không ghi. |
| UT-057 | Người nhận không tồn tại | Pass | Recipient null: lỗi 400, không ghi. |
| UT-058 | Vai trò không được phép | Pass | QuanTri gửi GiaSu: lỗi vai trò 403 trước khóa hồ sơ, không ghi. |
| UT-059 | Hồ sơ không đủ điều kiện | Pass | Hồ sơ không đủ điều kiện: lỗi 409, không ghi. |
| UT-060 | Liên hệ trùng | Pass | Liên hệ trùng: lỗi 400, không ghi. |
| UT-061 | Khóa hồ sơ thất bại | Pass | Khóa hồ sơ lỗi: truyền đúng lỗi, không ghi/commit; dispose transaction. |
| UT-062 | Lưu lời mời và thông báo | Pass | Trim “  Xin liên hệ  ”, trạng thái ChoPhanHoi; gọi AddContact → Notify → Save → Commit đúng thứ tự. Kiểm lời gọi thông báo, không xác nhận bản ghi outbox thật. |
| UT-063 | Lưu lỗi không commit | Pass | Save ném lỗi: truyền lỗi lên, không commit; dispose transaction. Không xác nhận rollback SQL thực tế. |
| UT-064 | Chưa có nhu cầu đang mở | Skip / Blocked | Chưa chốt điều kiện cần nhu cầu đang mở áp dụng cho vai trò nào; chưa có hợp đồng policy tương ứng trong GuiLienHeAsync. Không bổ sung nghiệp vụ mới chỉ để test. |

## Chi tiết tám ca Fail

### UT-027 — Ánh xạ phân trang

**Phân loại:** Kịch bản/hợp đồng phân trang chưa thống nhất.

**Mong đợi:** Hai dòng, tổng 12, Trang=2, KichThuocTrang=2.

**Thực tế:** Hai dòng, tổng 12, Trang=2 đều đạt; KichThuocTrang thực tế là 9.

**Vị trí liên quan:** `Areas/TimKiem/Services/TimKiemService.cs:47; Infrastructure/KetQua.cs:55; adapter vẫn Skip/Take cố định 9 như mã gốc.`

**Nguyên nhân:** BoLocTimKiemViewModel không có KichThuocTrang. Store giả lập trả Page kích thước 2 theo bảng cũ, nhưng service tạo Page với mặc định 9. Đây là sai lệch hợp đồng trong fixture/kịch bản, chưa đủ để kết luận ứng dụng phải hỗ trợ kích thước 2.

**Hướng xử lý:** Chốt yêu cầu kích thước cố định hay cấu hình được. Nếu cố định 9, sửa fixture/kỳ vọng; nếu cấu hình được, bổ sung hợp đồng và truyền kích thước xuyên suốt.

Thông báo gốc trong TRX:

```text
Assert.Equal() Failure: Values differ

Expected: 2

Actual:   9
```

### UT-031 — Tiêu chí lỗi

**Phân loại:** Nhánh lỗi controller khác kịch bản.

**Mong đợi:** Service trả KetQua lỗi tiêu chí; action trả View Index kèm thông báo lỗi.

**Thực tế:** Ném LoiNghiepVu “Tiêu chí không hợp lệ.” khi đọc DuLieu; không trả ViewResult.

**Vị trí liên quan:** `Areas/TimKiem/Controllers/TimKiemController.cs:20; Infrastructure/KetQua.cs:18.`

**Nguyên nhân:** Action chưa xét ThanhCong trước khi truy cập result.DuLieu. Getter DuLieu chủ động ném lại lỗi nếu KetQua thất bại.

**Hướng xử lý:** Nếu UC yêu cầu trả lại form Index, xử lý KetQua lỗi trước DuLieu, giữ bộ lọc và thêm lỗi. Nếu thiết kế dùng middleware/filter cho lỗi này, phải cập nhật kỳ vọng của unit và kiểm response bằng integration.

Thông báo gốc trong TRX:

```text
EduConnect.Mvc.Infrastructure.LoiNghiepVu : Tiêu chí không hợp lệ.
```

### UT-032 — Truyền bộ lọc mới

**Phân loại:** Bảng test yêu cầu trường không tồn tại.

**Mong đợi:** Chuyển toàn bộ bộ lọc mới, gồm KichThuocTrang=10.

**Thực tế:** Các trường đang có và actor/token truyền đúng; reflection không tìm thấy thuộc tính KichThuocTrang.

**Vị trí liên quan:** `Areas/TimKiem/ViewModels/SearchModels.cs:BoLocTimKiemViewModel; tests/Unit/TimKiemControllerTest.cs:55.`

**Nguyên nhân:** DTO bộ lọc hiện dùng Trang và các tiêu chí tìm kiếm; không có trường kích thước trang. Test không tự thêm trường vào DTO để làm Pass.

**Hướng xử lý:** Chốt hợp đồng phân trang cùng UT-027; không coi toàn bộ việc truyền bộ lọc hiện có là sai.

Thông báo gốc trong TRX:

```text
Assert.NotNull() Failure: Value is null
```

### UT-038 — Chấp nhận 1 ID phân biệt

**Phân loại:** Mâu thuẫn giới hạn 1–3 và 2–3 đã nhận diện khi rà MT.

**Mong đợi:** Một ID công khai vẫn so sánh thành công theo bảng gốc.

**Thực tế:** KetQua lỗi 400, “Chọn từ 2 đến 3 đối tượng để so sánh.”; không đọc đối tượng.

**Vị trí liên quan:** `Areas/TimKiem/Services/TimKiemService.cs:68.`

**Nguyên nhân:** Code kiểm số ID phân biệt trong miền 2–3. Bảng Unit cũ cho phép 1–3.

**Hướng xử lý:** Chốt FR-10/SRS hiện hành. Nếu yêu cầu là 1–3, sửa guard; nếu là 2–3, sửa kỳ vọng UT-038.

Thông báo gốc trong TRX:

```text
Chọn từ 2 đến 3 đối tượng để so sánh.
```

### UT-044 — Loại đối tượng hết công khai

**Phân loại:** Bảng Unit cũ mâu thuẫn MT1-IT-058.

**Mong đợi:** A/B/C, B không còn công khai: trả A/C và bỏ B.

**Thực tế:** KetQua lỗi 404 khi đọc B, dừng cả phép so sánh.

**Vị trí liên quan:** `Areas/TimKiem/Services/TimKiemService.cs:53 và vòng lặp ChiTietAsync trong SoSanhAsync.`

**Nguyên nhân:** ChiTietAsync ném lỗi khi đối tượng không còn công khai. Vòng so sánh không bắt lỗi 404 để tiếp tục. MT1-IT-058 lại yêu cầu lỗi cả yêu cầu, phù hợp code hiện tại.

**Hướng xử lý:** Chốt dùng kết quả một phần hay từ chối cả yêu cầu; không đổi code chỉ để làm xanh UT-044 khi chưa thống nhất với MT1.

Thông báo gốc trong TRX:

```text
Hồ sơ hoặc nhu cầu đang ẩn, chưa được duyệt hoặc không còn công khai.
```

### UT-046 — Kết quả so sánh lỗi

**Phân loại:** Nhánh lỗi controller khác kịch bản.

**Mong đợi:** Service so sánh trả KetQua lỗi: controller trả Index cùng lỗi.

**Thực tế:** Ném LoiNghiepVu “Số lượng không hợp lệ.”; không trả Index.

**Vị trí liên quan:** `Areas/TimKiem/Controllers/TimKiemController.cs:38; Infrastructure/KetQua.cs:18.`

**Nguyên nhân:** Controller đọc result.DuLieu.Items ngay mà không xử lý ThanhCong=false.

**Hướng xử lý:** Chốt nhánh trả Index hay để middleware/filter xử lý. Nếu trả Index là yêu cầu, xử lý lỗi trước truy cập DuLieu.

Thông báo gốc trong TRX:

```text
EduConnect.Mvc.Infrastructure.LoiNghiepVu : Số lượng không hợp lệ.
```

### UT-047 — Loại nhu cầu đang xem

**Phân loại:** Thiếu thông tin ID đang xem trong hợp đồng phương thức.

**Mong đợi:** Trong danh sách liên quan, loại N1 đang xem, chỉ giữ N2 cùng chủ công khai.

**Thực tế:** Service trả cả N1 và N2 theo danh sách public cùng owner do store cung cấp.

**Vị trí liên quan:** `Areas/TimKiem/Services/TimKiemService.cs:56; Areas/TimKiem/Controllers/TimKiemController.cs:31.`

**Nguyên nhân:** LienQuanAsync chỉ nhận owner, không nhận ID cần loại trừ; controller cũng chỉ truyền ChuSoHuuId. Test dùng store đúng hợp đồng trả toàn bộ nhu cầu công khai cùng chủ, nên N1 còn trong kết quả. Không xác nhận SQL lọc quyền ở unit này.

**Hướng xử lý:** Nếu UC yêu cầu loại nhu cầu đang xem, bổ sung ID loại trừ vào hợp đồng hoặc lọc ở tầng có biết ID. Nếu danh sách được phép bao gồm N1, sửa kỳ vọng.

Thông báo gốc trong TRX:

```text
Assert.DoesNotContain() Failure: Filter matched in collection

             ↓ (pos 0)

Collection: [NhuCauCongKhaiViewModel { AnhDaiDienUrl = null, ChuSoHuuId = ab8b6b38-d590-4011-bf4a-53410f88b4a6, ChuyenMon = null, DayTrucTiep = False, DayTrucTuyen = False, ··· }, NhuCauCongKhaiViewModel { AnhDaiDienUrl = null, ChuSoHuuId = ab8b6b38-d590-4011-bf4a-53410f88b4a6, ChuyenMon = null, DayTrucTiep = False, DayTrucTuyen = False, ··· }]
```

### UT-050 — Lỗi dữ liệu tại controller

**Phân loại:** Kịch bản trộn mức unit controller với HTTP/filter integration.

**Mong đợi:** ModelState lỗi LoiNhan; action trực tiếp trả kết quả HTTP 422 và không gọi service.

**Thực tế:** Không gọi service (đạt); ValidateInput ném LoiNghiepVu với Status=400, không trả ObjectResult 422.

**Vị trí liên quan:** `Areas/KetNoi/Controllers/KetNoiController.cs:35; Controllers/BaseController.cs:9; LoiNghiepVu mặc định Status=400.`

**Nguyên nhân:** Gọi trực tiếp action không chạy MVC pipeline hoặc filter. Không có response HTTP 422 để assert ở đây. Không mock/override ValidateInput vì cần kiểm controller thật.

**Hướng xử lý:** Tách unit kiểm exception và service Times.Never; kiểm HTTP 422/400 qua integration có pipeline, theo hợp đồng đã được chốt. Không kết luận response HTTP thực tế chỉ từ unit này.

Thông báo gốc trong TRX:

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lời nhắn không hợp lệ.

   at EduConnect.Mvc.Controllers.BaseController.ValidateInput() in C:\Khóa luận TN\Educonnect\Controllers\BaseController.cs:line 9

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.GuiLienHe(GuiLienHeInput m) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 35

   at EduConnect.SearchContact.UnitTests.LienHeTest.<>c__DisplayClass0_0.<<UT_046_ModelStateLoi_KhongGoiService_Tra422TheoBangGoc>b__1>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\LienHeTest.cs:line 28

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
```

## Hai ca Skip

- **UT-028:** Chưa có DiemPhuHop hoặc bộ tính điểm theo bảng gốc; MT1 lại quy định lý do gợi ý, không phần trăm. Cần chốt lại yêu cầu.
- **UT-064:** Chưa chốt điều kiện cần nhu cầu đang mở áp dụng cho vai trò nào; chưa có hợp đồng policy tương ứng trong GuiLienHeAsync. Không bổ sung nghiệp vụ mới chỉ để test.

## Các file mã nguồn đã thêm/sửa

**Thêm:**

- Areas/TimKiem/Services/ITimKiemStore.cs: hợp đồng đọc dữ liệu cho service tìm kiếm.
- Areas/TimKiem/Services/EfTimKiemStore.cs: giữ truy vấn EF/SQL, lọc/sắp xếp/phân trang, đọc chi tiết và bổ sung môn/lịch.
- Areas/KetNoi/Services/ILienHeStore.cs: hợp đồng đọc người nhận, transaction/khóa hồ sơ, kiểm điều kiện, lưu lời mời và yêu cầu tạo thông báo.
- Areas/KetNoi/Services/EfLienHeStore.cs: adapter EF và ThongBaoService thực cho hợp đồng trên.
- Areas/NhuCau/Services/HoSoCongKhaiMapper.cs: tách projection hồ sơ thành hàm thuần, được truy vấn thực sử dụng và UT-048 kiểm tra.
- tests/Unit/TimKiemServiceTest.cs, TimKiemControllerTest.cs, LienHeTest.cs: bộ 44 mã TC, có Trait Suite=TimKiemLienHe.

**Sửa:**

- TimKiemService.cs: giữ validation, chọn nhánh theo vai trò, dựng Page và so sánh trong service; nhận store tùy chọn, mặc định dùng EF adapter.
- KetNoiService.cs: nhận người dùng qua interface; đường gửi lời mời gọi adapter, giữ nguyên thứ tự kiểm và lưu.
- KetNoiController.cs: nhận IKetNoiService/INhuCauService để mock được; giữ thân action.
- IKetNoiService.cs: công khai các phương thức đã tồn tại trên implementation mà controller đang gọi; không thêm hành vi nghiệp vụ.
- HoSoCongKhaiQuery.cs: sử dụng mapper thuần thay cho projection inline, giữ dữ liệu đầu vào từ query.

## Kiểm tra và giới hạn

- Build cuối thành công; dotnet test exit code 1 do 8 ca Fail.
- Đối chiếu xác nhận truy vấn lọc hồ sơ và truy vấn bổ sung lịch/môn được giữ nguyên; các luồng KetNoiService từ PhanHoiLienHeAsync trở đi giữ nguyên thân phương thức.
- Kiểm tra Program.cs đã đăng ký cả concrete và interface cho current/search/connection; adapter mặc định được tạo khi không cung cấp mock.
- Unit không kiểm SQL collation, điều kiện WHERE thực, trigger, FK, khóa đồng thời, commit/rollback thật, outbox được ghi thật hoặc phản hồi HTTP qua pipeline. Cần Integration để xác nhận các phần này sau khi đổi adapter.
- UT-051/052/062 kiểm service yêu cầu tạo thông báo đúng recipient/type/target và thứ tự lưu; adapter Notify ủy quyền cho ThongBaoService. Không đánh đồng lời gọi Notify với việc đã kiểm hai bảng ThongBao/SuKienChoXuLy.
- UT-063 xác nhận không gọi Commit và dispose transaction sau lỗi lưu, không chứng minh rollback của SQL.
- UT-030 chỉ kiểm model rỗng; câu hiển thị “không có đối tượng” thuộc Razor/UI.
- UT-048 kiểm mapper, không kiểm truy vấn join lấy người và môn.
- Chưa sửa các sai lệch nghiệp vụ sau khi chạy. Không ghi đè minh chứng đợt Nhu cầu trước.

## Chạy lại trong VS Code

Mở Terminal tại `C:\Khóa luận TN\Educonnect`:

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --no-restore -p:UseAppHost=false "-p:BaseOutputPath=C:/Users/NGUYEN THI YEN/Documents/Codex/2026-11-13/un/work/search-contact-bin/" --filter "Suite=TimKiemLienHe" --results-directory "C:/Users/NGUYEN THI YEN/Documents/Codex/2026-11-13/un/outputs/TimKiem-LienHe-Unit" --logger "trx;LogFileName=TimKiem-LienHe-Unit.trx"
```

Chạy riêng ca: dùng `--filter "Suite=TimKiemLienHe&TestCaseId=UT-038"` và đặt tên TRX riêng để giữ kết quả toàn bộ bộ test.

## Minh chứng hiện hành

- [TRX gốc](TimKiem-LienHe-Unit.trx)
- [Kết quả JSON](KetQua.json)
- [Mã nguồn và bản thay đổi](Ma_nguon_TimKiem_LienHe.zip)
- [Mã kiểm tra nguồn](SHA256.json)
