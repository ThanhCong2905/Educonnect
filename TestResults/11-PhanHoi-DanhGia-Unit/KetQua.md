# Kết quả 11. Phản hồi và đánh giá

**5 ca: 3 Pass, 2 Fail, 0 Skip/Blocked.**

Trong các Fail có **1 ca kiểm hợp đồng thiết kế** (thiếu lớp/trường input), dừng trước khi chạy nhánh nghiệp vụ; các Fail còn lại có quan sát hành vi thật khác kỳ vọng. Không đồng nhất Fail hợp đồng với lỗi thuật toán đã chạy.

Nguồn: Unit Test.xlsx đã đối chiếu lại trực tiếp với workbook hiện tại, giữ nguyên mã/thứ tự và ba tiêu chuẩn xác thực, đúng đắn, đầy đủ của từng dòng.

## Phạm vi

Real service/filter/validator logic. Persistence, locks, server clock, Identity, notices or scanner replaced at boundaries. DesignContract cases stop at missing input/type; their business branches are not executed.

Riêng UT-304/305 dùng KetThucWorkerFixture cũ: H/G/X/C/R lần lượt prefix 1/2/3/4/5; GUID cụ thể nằm trong quan sát. Các ca khác dùng fixture mới theo từng Fact. Dữ liệu U/G/P/A/C/K/Room/M lần lượt có prefix GUID 1/2/3/4/5/6/7/8; RV1=[1,2,3,4,5,6,7,8]. T cố định cho cổng thời gian là 2026-11-14 10:00:00+07:00. Các chỗ code tự dùng UtcNow được ghi nhận đúng thời gian thực, không giả mạo bằng T.

Không gọi CSDL thật. Fake IQueryable chỉ cung cấp dữ liệu để chạy predicate C# thật; không chứng minh câu SQL sinh ra/constraint/lock/rollback. Mock Identity/files/notice là dependency; không mock service đang kiểm.

## Bảng kết quả

| Mã | Tình huống | Tiêu chuẩn | Kết quả | Quan sát |
| --- | --- | --- | --- | --- |
| UT-318 | Lưu phản hồi hợp lệ | Đúng đắn | Pass | status=200; success=true; message=null; saves=1; commits=1; Add=Phản hồi hợp lệ |
| UT-319 | Thiếu trường bắt buộc | Đầy đủ | Pass | status=400; success=false; message="The NoiDung field is required."; saves=0; commits=0; Add= |
| UT-320 | Đủ phản hồi của các bên | Đầy đủ | Fail hành vi | success=true; saves=0; số phản hồi=4; Kỳ vọng ba nguồn của K1; thực tế truy vấn chỉ lọc lớp nên trộn cả kỳ K2. |
| UT-321 | Gửi lại cùng mã thao tác | Đúng đắn | Fail hợp đồng | operationIdProperty=null; GuiDanhGiaViewModel không có MaThaoTac; không truyền được mã thao tác đã thành công để kiểm retry theo thiết kế. |
| UT-322 | Phiên bản phản hồi cũ | Đúng đắn | Pass | status=409; success=false; message="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu."; saves=1; commits=0; Add=Nội dung đã lưu |

## Chi tiết từng ca và bước thực hiện

### UT-318 — Lưu phản hồi hợp lệ

**Kết quả:** Passed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `DanhGiaService.G​uiAsync`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Vai trò được phép; lớp C1, kỳ K1, nội dung đủ theo vai trò.

**Các bước thực tế:**

1. Tạo service DanhGia thật; fake IQueryable async chỉ thay nguồn dữ liệu, transaction, SQL clock/lock. Giữ nguyên biểu thức lọc/validator trong service.
2. Seed lớp hoạt động, đợt K1 còn hạn và phản hồi của bên khác; UT-319 bỏ NoiDung, UT-320 thêm đủ ba nguồn K1 và một phản hồi K2; UT-322 có phản hồi RV1/input RV0.
3. Gọi GuiAsync hoặc LayTheoLopAsync trực tiếp; UT-322 mock Save ném DbUpdateConcurrencyException để kiểm ánh xạ lỗi và không commit.
4. Kiểm người/lớp/kỳ/nội dung, không đụng phản hồi bên khác, không ghi khi validation lỗi; trường lỗi kiểm lại bằng Rules.ValidateInput thật; đối chiếu 409/RowVersion/Commit cho xung đột.

**Kỳ vọng giữ nguyên:** Lưu đúng người gửi/lớp/kỳ và nội dung; không ghi đè phản hồi bên khác.

**Quan sát:** status=200; success=true; message=null; saves=1; commits=1; Add=Phản hồi hợp lệ

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-318",
  "mode": "Behavior",
  "source": "Areas/DanhGia/Services/DanhGiaService.cs",
  "success": true,
  "status": 200,
  "message": null,
  "added": [
    {
      "Id": "58578afb-01c6-47e4-84d6-5b4f91ccd581",
      "LopHocId": "50000000-0000-0000-0000-000000000005",
      "NguoiGuiId": "10000000-0000-0000-0000-000000000001",
      "DotPhanHoiId": "60000000-0000-0000-0000-000000000006",
      "MocDanhGia": "K1",
      "NoiDung": "Phản hồi hợp lệ",
      "MucDoHaiLong": 4,
      "GuiLuc": "2026-11-14T10:00:00+07:00",
      "QuyetDinhDeXuat": null,
      "RowVersion": ""
    }
  ],
  "saves": 1,
  "commits": 1,
  "version": null,
  "assertionFailures": []
}
```

### UT-319 — Thiếu trường bắt buộc

**Kết quả:** Passed; **Tiêu chuẩn:** Đầy đủ; **Đích:** `DanhGiaService validation`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Bỏ một nội dung bắt buộc của vai trò trong fixture.

**Các bước thực tế:**

1. Tạo service DanhGia thật; fake IQueryable async chỉ thay nguồn dữ liệu, transaction, SQL clock/lock. Giữ nguyên biểu thức lọc/validator trong service.
2. Seed lớp hoạt động, đợt K1 còn hạn và phản hồi của bên khác; UT-319 bỏ NoiDung, UT-320 thêm đủ ba nguồn K1 và một phản hồi K2; UT-322 có phản hồi RV1/input RV0.
3. Gọi GuiAsync hoặc LayTheoLopAsync trực tiếp; UT-322 mock Save ném DbUpdateConcurrencyException để kiểm ánh xạ lỗi và không commit.
4. Kiểm người/lớp/kỳ/nội dung, không đụng phản hồi bên khác, không ghi khi validation lỗi; trường lỗi kiểm lại bằng Rules.ValidateInput thật; đối chiếu 409/RowVersion/Commit cho xung đột.

**Kỳ vọng giữ nguyên:** Trả lỗi trường; không lưu; dùng validator thật.

**Quan sát:** status=400; success=false; message="The NoiDung field is required."; saves=0; commits=0; Add=

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-319",
  "mode": "Behavior",
  "source": "Areas/DanhGia/Services/DanhGiaService.cs",
  "success": false,
  "status": 400,
  "message": "The NoiDung field is required.",
  "added": [],
  "saves": 0,
  "commits": 0,
  "version": null,
  "validatorField": "NoiDung",
  "assertionFailures": []
}
```

### UT-320 — Đủ phản hồi của các bên

**Kết quả:** Failed; **Tiêu chuẩn:** Đầy đủ; **Đích:** `DanhGiaService tổng hợp`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Có phản hồi riêng của gia sư, học sinh và phụ huynh cùng lớp/kỳ.

**Các bước thực tế:**

1. Tạo service DanhGia thật; fake IQueryable async chỉ thay nguồn dữ liệu, transaction, SQL clock/lock. Giữ nguyên biểu thức lọc/validator trong service.
2. Seed lớp hoạt động, đợt K1 còn hạn và phản hồi của bên khác; UT-319 bỏ NoiDung, UT-320 thêm đủ ba nguồn K1 và một phản hồi K2; UT-322 có phản hồi RV1/input RV0.
3. Gọi GuiAsync hoặc LayTheoLopAsync trực tiếp; UT-322 mock Save ném DbUpdateConcurrencyException để kiểm ánh xạ lỗi và không commit.
4. Kiểm người/lớp/kỳ/nội dung, không đụng phản hồi bên khác, không ghi khi validation lỗi; trường lỗi kiểm lại bằng Rules.ValidateInput thật; đối chiếu 409/RowVersion/Commit cho xung đột.

**Kỳ vọng giữ nguyên:** Kết quả tổng hợp giữ đủ 3 nguồn và đúng kỳ; không trộn kỳ khác.

**Quan sát:** success=true; saves=0; số phản hồi=4; Kỳ vọng ba nguồn của K1; thực tế truy vấn chỉ lọc lớp nên trộn cả kỳ K2.

**Nguyên nhân:** LayTheoLopAsync chỉ có mã lớp/người thực hiện/token; Where chỉ so LopHocId. Fixture có ba phản hồi K1 và một phản hồi K2 cùng lớp nên trả cả bốn.

**Ảnh hưởng và giới hạn kết luận:** Tổng hợp theo kỳ không đúng như yêu cầu; không mất ba phản hồi K1.

**Hướng xử lý:** Bổ sung truy vấn theo đợt/kỳ và kiểm kỳ thuộc lớp; hoặc xác định một API tổng hợp theo kỳ khác nếu thiết kế chủ định dùng nó.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\DanhGia\Services\DanhGiaService.cs:128` — `public Task<KetQua<IReadOnlyList<PhieuDanhGia>>> LayTheoLopAsync`.

**Assertion không đạt:**

- Kỳ vọng ba nguồn của K1; thực tế truy vấn chỉ lọc lớp nên trộn cả kỳ K2.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-320",
  "mode": "Behavior",
  "source": "Areas/DanhGia/Services/DanhGiaService.cs",
  "rows": [
    {
      "Id": "4e36b5d0-46ca-474e-886b-b82255a6eba1",
      "LopHocId": "50000000-0000-0000-0000-000000000005",
      "NguoiGuiId": "20000000-0000-0000-0000-000000000002",
      "DotPhanHoiId": "60000000-0000-0000-0000-000000000006",
      "MocDanhGia": "",
      "NoiDung": "Của gia sư",
      "MucDoHaiLong": 5,
      "GuiLuc": "2026-11-14T10:00:00+07:00",
      "QuyetDinhDeXuat": null,
      "RowVersion": ""
    },
    {
      "Id": "1cffc34e-757f-43e5-a87c-2c86c7392be5",
      "LopHocId": "50000000-0000-0000-0000-000000000005",
      "NguoiGuiId": "10000000-0000-0000-0000-000000000001",
      "DotPhanHoiId": "60000000-0000-0000-0000-000000000006",
      "MocDanhGia": "",
      "NoiDung": "Học sinh",
      "MucDoHaiLong": null,
      "GuiLuc": "0001-01-01T00:00:00+00:00",
      "QuyetDinhDeXuat": null,
      "RowVersion": ""
    },
    {
      "Id": "6c216e93-be84-4b44-a4fd-dc4107847617",
      "LopHocId": "50000000-0000-0000-0000-000000000005",
      "NguoiGuiId": "30000000-0000-0000-0000-000000000003",
      "DotPhanHoiId": "60000000-0000-0000-0000-000000000006",
      "MocDanhGia": "",
      "NoiDung": "Phụ huynh",
      "MucDoHaiLong": null,
      "GuiLuc": "0001-01-01T00:00:00+00:00",
      "QuyetDinhDeXuat": null,
      "RowVersion": ""
    },
    {
      "Id": "2ed77554-256f-409d-a53f-3c092690be22",
      "LopHocId": "50000000-0000-0000-0000-000000000005",
      "NguoiGuiId": "10000000-0000-0000-0000-000000000001",
      "DotPhanHoiId": "80000000-0000-0000-0000-000000000008",
      "MocDanhGia": "",
      "NoiDung": "Kỳ khác",
      "MucDoHaiLong": null,
      "GuiLuc": "0001-01-01T00:00:00+00:00",
      "QuyetDinhDeXuat": null,
      "RowVersion": ""
    }
  ],
  "success": true,
  "saves": 0,
  "assertionFailures": [
    "Kỳ vọng ba nguồn của K1; thực tế truy vấn chỉ lọc lớp nên trộn cả kỳ K2."
  ]
}
```

### UT-321 — Gửi lại cùng mã thao tác

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `DanhGiaService gửi phản hồi`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cùng người/lớp/kỳ và mã thao tác đã thành công.

**Các bước thực tế:**

1. Nạp kiểu input thật của phương thức được chỉ định.
2. Dùng reflection liệt kê thuộc tính input, kiểm trường mã thao tác hoặc vai trò mà fixture yêu cầu.
3. Assert có thể biểu diễn input của ca theo hợp đồng; hiện thiếu nên Fail. Không gọi service với input sai ngữ nghĩa để tạo kết quả giả.

**Kỳ vọng giữ nguyên:** Trả kết quả đã có; không tạo thêm phản hồi.

**Quan sát:** operationIdProperty=null; GuiDanhGiaViewModel không có MaThaoTac; không truyền được mã thao tác đã thành công để kiểm retry theo thiết kế.

**Nguyên nhân:** GuiDanhGiaViewModel không có MaThaoTac; không truyền được mã retry theo kịch bản. Unit kiểm hợp đồng input, không giả vờ đã thực thi nhánh idempotency.

**Ảnh hưởng và giới hạn kết luận:** Chưa chứng minh retry idempotent theo mã thao tác; không kết luận hệ thống luôn tạo thêm phiếu.

**Hướng xử lý:** Thống nhất/triển khai mã thao tác và quy tắc retry tại service, rồi kiểm cùng mã/cùng payload/khác payload/quyền bị thu hồi.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\DanhGia\ViewModels\DanhGiaInputs.cs:28` — `public sealed class GuiDanhGiaViewModel`.
- `C:\Khóa luận TN\Educonnect\Areas\DanhGia\Services\DanhGiaService.cs:44` — `public Task<KetQua<Guid>> GuiAsync`.

**Assertion không đạt:**

- GuiDanhGiaViewModel không có MaThaoTac; không truyền được mã thao tác đã thành công để kiểm retry theo thiết kế.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-321",
  "mode": "DesignContract",
  "source": "Areas/DanhGia/Services/DanhGiaService.cs",
  "operationIdProperty": null,
  "assertionFailures": [
    "GuiDanhGiaViewModel không có MaThaoTac; không truyền được mã thao tác đã thành công để kiểm retry theo thiết kế."
  ]
}
```

### UT-322 — Phiên bản phản hồi cũ

**Kết quả:** Passed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `DanhGiaService sửa phản hồi`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Phản hồi RV1; input RV0.

**Các bước thực tế:**

1. Tạo service DanhGia thật; fake IQueryable async chỉ thay nguồn dữ liệu, transaction, SQL clock/lock. Giữ nguyên biểu thức lọc/validator trong service.
2. Seed lớp hoạt động, đợt K1 còn hạn và phản hồi của bên khác; UT-319 bỏ NoiDung, UT-320 thêm đủ ba nguồn K1 và một phản hồi K2; UT-322 có phản hồi RV1/input RV0.
3. Gọi GuiAsync hoặc LayTheoLopAsync trực tiếp; UT-322 mock Save ném DbUpdateConcurrencyException để kiểm ánh xạ lỗi và không commit.
4. Kiểm người/lớp/kỳ/nội dung, không đụng phản hồi bên khác, không ghi khi validation lỗi; trường lỗi kiểm lại bằng Rules.ValidateInput thật; đối chiếu 409/RowVersion/Commit cho xung đột.

**Kỳ vọng giữ nguyên:** Xung đột 409; không ghi đè.

**Quan sát:** status=409; success=false; message="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu."; saves=1; commits=0; Add=Nội dung đã lưu

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-322",
  "mode": "Behavior",
  "source": "Areas/DanhGia/Services/DanhGiaService.cs",
  "success": false,
  "status": 409,
  "message": "Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.",
  "added": [
    {
      "Id": "45fa17c4-f85a-4af9-bcdb-4fbba71e2461",
      "PhieuDanhGiaId": "dfde0beb-eecf-4b37-ab73-a88ac90ec71b",
      "NoiDung": "Nội dung đã lưu",
      "MucDoHaiLong": null,
      "NguoiSuaId": "10000000-0000-0000-0000-000000000001",
      "ThayDoiLuc": "2026-11-14T10:00:00+07:00"
    }
  ],
  "saves": 1,
  "commits": 0,
  "version": "AAAAAAAAAAA=",
  "scope": "Mock Save ném concurrency; không khẳng định RAM được rollback hoặc SQL rowversion đã chạy.",
  "assertionFailures": []
}
```

## Chạy lại trong VS Code

Mở thư mục `C:\Khóa luận TN\Educonnect`, Terminal PowerShell:

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter "Suite=Remaining11" --logger "trx;LogFileName=Group11-Unit.trx" --results-directory "TestResults/11-PhanHoi-DanhGia-Unit"

dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter "FullyQualifiedName~EduConnect.Remaining.UnitTests.RemainingUnitTest.UT_318"
```

Nếu ứng dụng đang chạy khóa file build, dừng ứng dụng trước khi chạy lại hoặc dùng `-p:UseAppHost=false` và `-p:BaseOutputPath=<thư mục build riêng>` như lần chạy này. Bộ test có Fail nên dotnet test trả exit code 1 là kết quả assertion; không được diễn giải thành lỗi kết nối SQL.

## File thêm/sửa và cách đọc

- `RemainingTest.cs`: 71 Fact với mã UT, Trait Suite=Remaining10…14 để lọc riêng nhóm; không dùng Skip cho ca thiếu thiết kế.
- `RemainingRunner.cs`: Arrange/Act/Assert từng nhánh; gom đầy đủ assertion lệch và ghi OBSERVATION_JSON trước khi test Fail.
- `RemainingFixture.cs`: dữ liệu mới mỗi ca, async query provider chạy LINQ C#, fake store/transaction và mock dependency. Không chứa công thức nghiệp vụ đếm chờ/timeout thay cho app.
- `IServiceDataStore.cs`: interface dữ liệu/clock/lock/Save và EfServiceDataStore mặc định gọi lại EF/static helpers cũ. Dùng cổng này để giữ service thật trong unit.
- `IQuyenQuanTriService` và `IPhienDangNhapService`: tách phụ thuộc phân quyền/thu hồi phiên. Program đăng ký về implementation thật; các constructor cũ vẫn dùng được.
- `ITepService`: thêm property ChoPhepTepChuaQuet với mặc định false; `TepService` nhận scanner tùy chọn, không truyền thì vẫn chạy ClamAV cũ.
- `MatKhauValidator`: nhận blocklist cục bộ tùy chọn; không truyền thì giữ danh sách cấu hình cũ.
- Các service DanhGia/TinNhan/BaoCao/QuanTri/HoSo/TaiKhoan: thay điểm truy cập dữ liệu bằng adapter mặc định và dependency interface; không đổi guard/kết quả nghiệp vụ để làm test Pass.
- `KetQua.md`: bảng và mô tả chi tiết; `KetQua.json`: dữ liệu máy đọc; `GroupNN-Unit.trx`: bằng chứng runner; `Ma_nguon_Unit.zip`: snapshot mã liên quan và diff các service; `SHA256.json`: đối chiếu toàn vẹn.

## Hồi quy và phần chưa thuộc lần chạy

Đã chạy lại toàn project unit: **328 ca = 183 Pass, 142 Fail, 3 Skip**. Đủ **257 ca cũ không đổi trạng thái** so với TRX sau nhóm 08. Ba Skip cũ UT-020/028/064 giữ nguyên trong báo cáo cũ, không tính vào 71 ca mới.

Nhóm 09 là phụ lục SQL/tích hợp (UT-258–300), không được chạy hoặc báo đạt dưới nhãn unit. Các thuật toán SQL tương ứng LoginAttempt/UserSession cũng chưa được thực thi bởi unit. Chưa chạy lại integration sau refactor dependency. Các kết quả nhóm trước không bị ghi đè.
