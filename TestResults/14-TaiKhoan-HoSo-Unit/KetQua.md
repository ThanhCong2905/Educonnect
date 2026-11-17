# Kết quả 14. Tài khoản và hồ sơ

**23 ca: 10 Pass, 13 Fail, 0 Skip/Blocked.**

Trong các Fail có **10 ca kiểm hợp đồng thiết kế** (thiếu lớp/trường input), dừng trước khi chạy nhánh nghiệp vụ; các Fail còn lại có quan sát hành vi thật khác kỳ vọng. Không đồng nhất Fail hợp đồng với lỗi thuật toán đã chạy.

Nguồn: Unit Test.xlsx đã đối chiếu lại trực tiếp với workbook hiện tại, giữ nguyên mã/thứ tự và ba tiêu chuẩn xác thực, đúng đắn, đầy đủ của từng dòng.

## Phạm vi

Real service/filter/validator logic. Persistence, locks, server clock, Identity, notices or scanner replaced at boundaries. DesignContract cases stop at missing input/type; their business branches are not executed.

Riêng UT-304/305 dùng KetThucWorkerFixture cũ: H/G/X/C/R lần lượt prefix 1/2/3/4/5; GUID cụ thể nằm trong quan sát. Các ca khác dùng fixture mới theo từng Fact. Dữ liệu U/G/P/A/C/K/Room/M lần lượt có prefix GUID 1/2/3/4/5/6/7/8; RV1=[1,2,3,4,5,6,7,8]. T cố định cho cổng thời gian là 2026-11-14 10:00:00+07:00. Các chỗ code tự dùng UtcNow được ghi nhận đúng thời gian thực, không giả mạo bằng T.

Không gọi CSDL thật. Fake IQueryable chỉ cung cấp dữ liệu để chạy predicate C# thật; không chứng minh câu SQL sinh ra/constraint/lock/rollback. Mock Identity/files/notice là dependency; không mock service đang kiểm.

## Bảng kết quả

| Mã | Tình huống | Tiêu chuẩn | Kết quả | Quan sát |
| --- | --- | --- | --- | --- |
| UT-349 | Email đã tồn tại | Đúng đắn | Pass | success=false; saves=0; commits=0; Add= |
| UT-350 | Đăng ký vai trò Admin công khai | Xác thực | Pass | success=false; saves=0; commits=0; Add= |
| UT-351 | Gia sư mới cần duyệt | Đúng đắn | Fail hành vi | success=true; saves=1; commits=1; Add=62434d84-285a-4fa5-a2b7-d7197eedf500; Đăng ký chỉ tạo GiaSu, chưa tạo HoSoGiaSu ChoDuyet/ẩn như kỳ vọng. |
| UT-352 | Sai mật khẩu | Xác thực | Pass | message="Email hoặc mật khẩu chưa đúng, hoặc tài khoản hiện không thể đăng nhập."; saves=0; Add= |
| UT-353 | Tài khoản bị khóa | Xác thực | Pass | message="Email hoặc mật khẩu chưa đúng, hoặc tài khoản hiện không thể đăng nhập."; saves=0; Add= |
| UT-354 | Gửi duyệt hồ sơ hoàn chỉnh | Đúng đắn | Fail hành vi | status=200; message=null; saves=3; noticeRecipients=[]; Gửi duyệt không tạo thông báo admin. |
| UT-355 | Thiếu thông tin bắt buộc | Đầy đủ | Fail hành vi | status=400; message="Hoàn thiện thông tin hồ sơ, tối đa 256 ký tự mỗi trường."; saves=0; noticeRecipients=[]; Guard gộp báo thiếu thông tin nhưng không chỉ trường TrinhDo cần bổ sung. |
| UT-356 | Mật khẩu dài 14 | Xác thực | Pass | success=false; runeCount=14; blocklistCalls=0 |
| UT-357 | Mật khẩu dài 15 | Xác thực | Pass | success=true; runeCount=15; blocklistCalls=1 |
| UT-358 | Mật khẩu dài 64 | Xác thực | Pass | success=true; runeCount=64; blocklistCalls=1 |
| UT-359 | Mật khẩu dài 65 | Xác thực | Pass | success=false; runeCount=65; blocklistCalls=0 |
| UT-360 | Mật khẩu có khoảng trắng | Xác thực | Pass | success=true; runeCount=20; blocklistCalls=1 |
| UT-361 | Mật khẩu trong blocklist | Xác thực | Pass | success=false; runeCount=20; blocklistCalls=1 |
| UT-362 | Lần sai 5 chờ 30 giây | Đúng đắn | Fail hợp đồng | requiredType="LoginAttemptService"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL. |
| UT-363 | Lần sai 6 chờ 60 giây | Đúng đắn | Fail hợp đồng | requiredType="LoginAttemptService"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL. |
| UT-364 | Lần sai 7 chờ 120 giây | Đúng đắn | Fail hợp đồng | requiredType="LoginAttemptService"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL. |
| UT-365 | Lần sai 8 chờ 300 giây | Đúng đắn | Fail hợp đồng | requiredType="LoginAttemptService"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL. |
| UT-366 | Lần sai 9 chờ 300 giây | Đúng đắn | Fail hợp đồng | requiredType="LoginAttemptService"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL. |
| UT-367 | Một trong hai bộ đếm còn chờ | Xác thực | Fail hợp đồng | requiredType="LoginAttemptService"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL. |
| UT-368 | Đăng nhập thành công chỉ xóa đếm tài khoản | Đúng đắn | Fail hợp đồng | requiredType="LoginAttemptService"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL. |
| UT-369 | Phiên nhàn rỗi 1799 giây | Xác thực | Fail hợp đồng | requiredType="UserSession"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL. |
| UT-370 | Phiên nhàn rỗi 1800 giây | Xác thực | Fail hợp đồng | requiredType="UserSession"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL. |
| UT-371 | Ping nền không gia hạn | Xác thực | Fail hợp đồng | requiredType="UserSession"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL. |

## Chi tiết từng ca và bước thực hiện

### UT-349 — Email đã tồn tại

**Kết quả:** Passed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `TaiKhoanService đăng ký`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng Identity trả email đã có.

**Các bước thực tế:**

1. Tạo TaiKhoanService thật, mock UserManager của Identity; fake transaction/dữ liệu, NhatKyAnToanService thật với HttpContext riêng.
2. Cấu hình Identity: trùng email, role Admin, đăng ký GiaSu, sai mật khẩu hoặc mật khẩu đúng/tài khoản BiKhoa đúng theo ca.
3. Gọi DangKyAsync hoặc DangNhapAsync trực tiếp. Nhánh đăng nhập bị từ chối không tạo cookie/phiên; thu câu lệnh yêu cầu ghi lượt sai nhưng không chạy SQL.
4. Kiểm Identity Create/AddToRole, dữ liệu con/hồ sơ, Save/Commit, lỗi trả về, log thất bại không chứa mật khẩu và không tiếp tục GetRoles khi bị từ chối.

**Kỳ vọng giữ nguyên:** Lỗi trùng; không tạo tài khoản/vai trò/hồ sơ mới.

**Quan sát:** success=false; saves=0; commits=0; Add=

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-349",
  "mode": "Behavior",
  "source": "Infrastructure/TaiKhoanService.cs",
  "success": false,
  "identityErrors": [
    {
      "Code": "DuplicateEmail",
      "Description": "Email đã tồn tại"
    }
  ],
  "added": [],
  "saves": 0,
  "commits": 0,
  "identityCalls": [
    "set_Logger",
    "CreateAsync"
  ],
  "assertionFailures": []
}
```

### UT-350 — Đăng ký vai trò Admin công khai

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `TaiKhoanService đăng ký`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Input chọn Admin.

**Các bước thực tế:**

1. Tạo TaiKhoanService thật, mock UserManager của Identity; fake transaction/dữ liệu, NhatKyAnToanService thật với HttpContext riêng.
2. Cấu hình Identity: trùng email, role Admin, đăng ký GiaSu, sai mật khẩu hoặc mật khẩu đúng/tài khoản BiKhoa đúng theo ca.
3. Gọi DangKyAsync hoặc DangNhapAsync trực tiếp. Nhánh đăng nhập bị từ chối không tạo cookie/phiên; thu câu lệnh yêu cầu ghi lượt sai nhưng không chạy SQL.
4. Kiểm Identity Create/AddToRole, dữ liệu con/hồ sơ, Save/Commit, lỗi trả về, log thất bại không chứa mật khẩu và không tiếp tục GetRoles khi bị từ chối.

**Kỳ vọng giữ nguyên:** Từ chối; không gọi tạo tài khoản.

**Quan sát:** success=false; saves=0; commits=0; Add=

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-350",
  "mode": "Behavior",
  "source": "Infrastructure/TaiKhoanService.cs",
  "success": false,
  "identityErrors": [
    {
      "Code": null,
      "Description": "Vai trò đăng ký không hợp lệ."
    }
  ],
  "added": [],
  "saves": 0,
  "commits": 0,
  "identityCalls": [
    "set_Logger"
  ],
  "assertionFailures": []
}
```

### UT-351 — Gia sư mới cần duyệt

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `TaiKhoanService đăng ký`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Thông tin hợp lệ, vai trò GiaSu.

**Các bước thực tế:**

1. Tạo TaiKhoanService thật, mock UserManager của Identity; fake transaction/dữ liệu, NhatKyAnToanService thật với HttpContext riêng.
2. Cấu hình Identity: trùng email, role Admin, đăng ký GiaSu, sai mật khẩu hoặc mật khẩu đúng/tài khoản BiKhoa đúng theo ca.
3. Gọi DangKyAsync hoặc DangNhapAsync trực tiếp. Nhánh đăng nhập bị từ chối không tạo cookie/phiên; thu câu lệnh yêu cầu ghi lượt sai nhưng không chạy SQL.
4. Kiểm Identity Create/AddToRole, dữ liệu con/hồ sơ, Save/Commit, lỗi trả về, log thất bại không chứa mật khẩu và không tiếp tục GetRoles khi bị từ chối.

**Kỳ vọng giữ nguyên:** Tài khoản/vai trò/hồ sơ tương ứng được yêu cầu tạo; hồ sơ chờ duyệt, chưa công khai.

**Quan sát:** success=true; saves=1; commits=1; Add=62434d84-285a-4fa5-a2b7-d7197eedf500; Đăng ký chỉ tạo GiaSu, chưa tạo HoSoGiaSu ChoDuyet/ẩn như kỳ vọng.

**Nguyên nhân:** DangKyAsync tạo Identity account, gán role GiaSu và thêm GiaSu; không thêm HoSoGiaSu ở trạng thái ChoDuyet.

**Ảnh hưởng và giới hạn kết luận:** Không phải đăng ký tài khoản thất bại; thiếu trạng thái hồ sơ theo kỳ vọng hiện tại.

**Hướng xử lý:** Thống nhất hồ sơ chờ duyệt phải được tạo ngay lúc đăng ký hay sau khi gia sư hoàn thiện hồ sơ; triển khai/đồng bộ hợp đồng tương ứng.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\TaiKhoanService.cs:49` — `case "GiaSu":`.

**Assertion không đạt:**

- Đăng ký chỉ tạo GiaSu, chưa tạo HoSoGiaSu ChoDuyet/ẩn như kỳ vọng.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-351",
  "mode": "Behavior",
  "source": "Infrastructure/TaiKhoanService.cs",
  "success": true,
  "identityErrors": [],
  "added": [
    {
      "TaiKhoanId": "62434d84-285a-4fa5-a2b7-d7197eedf500"
    }
  ],
  "saves": 1,
  "commits": 1,
  "identityCalls": [
    "set_Logger",
    "CreateAsync",
    "AddToRoleAsync"
  ],
  "assertionFailures": [
    "Đăng ký chỉ tạo GiaSu, chưa tạo HoSoGiaSu ChoDuyet/ẩn như kỳ vọng."
  ]
}
```

### UT-352 — Sai mật khẩu

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `TaiKhoanService đăng nhập`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng Identity trả xác thực thất bại.

**Các bước thực tế:**

1. Tạo TaiKhoanService thật, mock UserManager của Identity; fake transaction/dữ liệu, NhatKyAnToanService thật với HttpContext riêng.
2. Cấu hình Identity: trùng email, role Admin, đăng ký GiaSu, sai mật khẩu hoặc mật khẩu đúng/tài khoản BiKhoa đúng theo ca.
3. Gọi DangKyAsync hoặc DangNhapAsync trực tiếp. Nhánh đăng nhập bị từ chối không tạo cookie/phiên; thu câu lệnh yêu cầu ghi lượt sai nhưng không chạy SQL.
4. Kiểm Identity Create/AddToRole, dữ liệu con/hồ sơ, Save/Commit, lỗi trả về, log thất bại không chứa mật khẩu và không tiếp tục GetRoles khi bị từ chối.

**Kỳ vọng giữ nguyên:** Không tạo phiên; ghi lượt sai theo quy tắc; không ghi mật khẩu vào log.

**Quan sát:** message="Email hoặc mật khẩu chưa đúng, hoặc tài khoản hiện không thể đăng nhập."; saves=0; Add=

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-352",
  "mode": "Behavior",
  "source": "Infrastructure/TaiKhoanService.cs",
  "message": "Email hoặc mật khẩu chưa đúng, hoặc tài khoản hiện không thể đăng nhập.",
  "commands": [
    "EXEC dbo.usp_GhiNhanDangNhapSai @TaiKhoanChuanHoa={0},@DiaChiIp={1}"
  ],
  "logs": [
    {
      "Id": "2f5d1d6e-a3d5-4b96-98b5-617acc19af71",
      "ThoiDiem": "2026-11-14T08:03:03.3222044+00:00",
      "Nhom": "Authentication",
      "SuKien": "TaiKhoan.DangNhap",
      "KetQua": "ThatBai",
      "TaiKhoanId": null,
      "TaiKhoanDichId": "10000000-0000-0000-0000-000000000001",
      "DinhDanhBam": "5017E5CB550A69E382905FFEE337F66D92EAE46BEDB4F7DB7B0A48896E7D8F55",
      "DiaChiIp": "",
      "DuongDan": "",
      "PhuongThuc": "",
      "MaYeuCau": "0HNQ24MQL6EFF",
      "LyDo": "ThongTinDangNhapKhongHopLe",
      "DoiTuongId": null
    }
  ],
  "saves": 0,
  "added": [],
  "scope": "Chỉ xác nhận lời gọi ghi lượt sai; không thực thi quy tắc đếm/khóa SQL hoặc tạo cookie.",
  "assertionFailures": []
}
```

### UT-353 — Tài khoản bị khóa

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `TaiKhoanService đăng nhập`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Mật khẩu đúng; tài khoản BiKhoa.

**Các bước thực tế:**

1. Tạo TaiKhoanService thật, mock UserManager của Identity; fake transaction/dữ liệu, NhatKyAnToanService thật với HttpContext riêng.
2. Cấu hình Identity: trùng email, role Admin, đăng ký GiaSu, sai mật khẩu hoặc mật khẩu đúng/tài khoản BiKhoa đúng theo ca.
3. Gọi DangKyAsync hoặc DangNhapAsync trực tiếp. Nhánh đăng nhập bị từ chối không tạo cookie/phiên; thu câu lệnh yêu cầu ghi lượt sai nhưng không chạy SQL.
4. Kiểm Identity Create/AddToRole, dữ liệu con/hồ sơ, Save/Commit, lỗi trả về, log thất bại không chứa mật khẩu và không tiếp tục GetRoles khi bị từ chối.

**Kỳ vọng giữ nguyên:** Không cho truy cập và không tạo phiên hợp lệ.

**Quan sát:** message="Email hoặc mật khẩu chưa đúng, hoặc tài khoản hiện không thể đăng nhập."; saves=0; Add=

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-353",
  "mode": "Behavior",
  "source": "Infrastructure/TaiKhoanService.cs",
  "message": "Email hoặc mật khẩu chưa đúng, hoặc tài khoản hiện không thể đăng nhập.",
  "commands": [
    "EXEC dbo.usp_GhiNhanDangNhapSai @TaiKhoanChuanHoa={0},@DiaChiIp={1}"
  ],
  "logs": [
    {
      "Id": "7fc429e7-7bb9-47ca-bd4c-921650a17094",
      "ThoiDiem": "2026-11-14T08:03:03.4975946+00:00",
      "Nhom": "Authentication",
      "SuKien": "TaiKhoan.DangNhap",
      "KetQua": "ThatBai",
      "TaiKhoanId": null,
      "TaiKhoanDichId": "10000000-0000-0000-0000-000000000001",
      "DinhDanhBam": "5017E5CB550A69E382905FFEE337F66D92EAE46BEDB4F7DB7B0A48896E7D8F55",
      "DiaChiIp": "",
      "DuongDan": "",
      "PhuongThuc": "",
      "MaYeuCau": "0HNQ24MQL6EFG",
      "LyDo": "ThongTinDangNhapKhongHopLe",
      "DoiTuongId": null
    }
  ],
  "saves": 0,
  "added": [],
  "scope": "Chỉ xác nhận lời gọi ghi lượt sai; không thực thi quy tắc đếm/khóa SQL hoặc tạo cookie.",
  "assertionFailures": []
}
```

### UT-354 — Gửi duyệt hồ sơ hoàn chỉnh

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `HoSoGiaSuService lưu hồ sơ`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Đúng chủ; dữ liệu hợp lệ đầy đủ môn/phí/giới thiệu/chuyên môn.

**Các bước thực tế:**

1. Tạo hồ sơ BanNhap đủ thông tin, đúng chủ GiaSu; input UT-355 bỏ TrinhDo.
2. Gọi CapNhatAsync; riêng UT-354 tiếp tục DoiTrangThaiAsync(GuiDuyet) sau lưu.
3. Thu trạng thái, exception.Truong/message, Saves và các notice.
4. UT-354 yêu cầu ChoDuyet/ẩn và notice admin; UT-355 yêu cầu field TrinhDo, không Save và không DaDuyet.

**Kỳ vọng giữ nguyên:** Hồ sơ chờ duyệt; thông báo admin; không tự công khai.

**Quan sát:** status=200; message=null; saves=3; noticeRecipients=[]; Gửi duyệt không tạo thông báo admin.

**Nguyên nhân:** Đã chạy CapNhatAsync rồi DoiTrangThaiAsync(GuiDuyet). Hồ sơ đúng ChoDuyet và ẩn; nhánh GuiDuyet chỉ đổi trường trạng thái rồi Save/Commit, không gọi notice admin.

**Ảnh hưởng và giới hạn kết luận:** Hồ sơ vẫn được gửi duyệt; thiếu notice admin.

**Hướng xử lý:** Thêm thông báo người có quyền duyệt tại bước gửi duyệt, cùng hợp đồng lưu hồ sơ.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\NhuCau\Services\HoSoGiaSuService.cs:129` — `case "GuiDuyet":`.

**Assertion không đạt:**

- Gửi duyệt không tạo thông báo admin.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-354",
  "mode": "Behavior",
  "source": "Areas/NhuCau/Services/HoSoGiaSuService.cs",
  "status": 200,
  "field": null,
  "message": null,
  "profile": {
    "DangHienThi": false,
    "DayTrucTuyen": true,
    "DayTrucTiep": false,
    "GuiDuyetLuc": "2026-11-14T08:03:03.5756263+00:00",
    "DuyetLuc": null,
    "NguoiDuyetId": null,
    "Id": "80000000-0000-0000-0000-000000000008",
    "GiaSuId": "20000000-0000-0000-0000-000000000002",
    "TrinhDo": "Đại học",
    "ChuyenMon": "Toán",
    "KinhNghiem": "3 năm",
    "MoTa": "Giới thiệu đầy đủ cho hồ sơ",
    "KhuVuc": "Hà Nội",
    "TrangThaiDuyet": "ChoDuyet",
    "LyDoTuChoi": null,
    "RowVersion": "AQIDBAUGBwg="
  },
  "saves": 3,
  "notices": [],
  "assertionFailures": [
    "Gửi duyệt không tạo thông báo admin."
  ]
}
```

### UT-355 — Thiếu thông tin bắt buộc

**Kết quả:** Failed; **Tiêu chuẩn:** Đầy đủ; **Đích:** `HoSoGiaSuService lưu hồ sơ`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Thiếu một trường bắt buộc theo validator thật.

**Các bước thực tế:**

1. Tạo hồ sơ BanNhap đủ thông tin, đúng chủ GiaSu; input UT-355 bỏ TrinhDo.
2. Gọi CapNhatAsync; riêng UT-354 tiếp tục DoiTrangThaiAsync(GuiDuyet) sau lưu.
3. Thu trạng thái, exception.Truong/message, Saves và các notice.
4. UT-354 yêu cầu ChoDuyet/ẩn và notice admin; UT-355 yêu cầu field TrinhDo, không Save và không DaDuyet.

**Kỳ vọng giữ nguyên:** Báo trường cần bổ sung; không gửi hồ sơ thành đã duyệt.

**Quan sát:** status=400; message="Hoàn thiện thông tin hồ sơ, tối đa 256 ký tự mỗi trường."; saves=0; noticeRecipients=[]; Guard gộp báo thiếu thông tin nhưng không chỉ trường TrinhDo cần bổ sung.

**Nguyên nhân:** CapNhatAsync dùng guard gộp HoTen/TrinhDo/ChuyenMon/KinhNghiem/KhuVuc. Khi TrinhDo rỗng, LoiNghiepVu có message chung và Truong=null; không chỉ được trường cần bổ sung.

**Ảnh hưởng và giới hạn kết luận:** Đã từ chối và không Save; thiếu chỉ dẫn lỗi theo trường.

**Hướng xử lý:** Dùng validator thật hoặc các guard có field tương ứng ở tầng service để lỗi nêu TrinhDo; giữ điều kiện không lưu hồ sơ thiếu dữ liệu.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\NhuCau\Services\HoSoGiaSuService.cs:56` — `Rules.Check(new[]{m.HoTen`.

**Assertion không đạt:**

- Guard gộp báo thiếu thông tin nhưng không chỉ trường TrinhDo cần bổ sung.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-355",
  "mode": "Behavior",
  "source": "Areas/NhuCau/Services/HoSoGiaSuService.cs",
  "status": 400,
  "field": null,
  "message": "Hoàn thiện thông tin hồ sơ, tối đa 256 ký tự mỗi trường.",
  "profile": {
    "DangHienThi": false,
    "DayTrucTuyen": true,
    "DayTrucTiep": false,
    "GuiDuyetLuc": null,
    "DuyetLuc": null,
    "NguoiDuyetId": null,
    "Id": "80000000-0000-0000-0000-000000000008",
    "GiaSuId": "20000000-0000-0000-0000-000000000002",
    "TrinhDo": "Dai hoc",
    "ChuyenMon": "Toan",
    "KinhNghiem": "3 nam",
    "MoTa": "Gioi thieu day du",
    "KhuVuc": "Ha Noi",
    "TrangThaiDuyet": "BanNhap",
    "LyDoTuChoi": null,
    "RowVersion": "AQIDBAUGBwg="
  },
  "saves": 0,
  "notices": [],
  "assertionFailures": [
    "Guard gộp báo thiếu thông tin nhưng không chỉ trường TrinhDo cần bổ sung."
  ]
}
```

### UT-356 — Mật khẩu dài 14

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `PasswordValidator ứng dụng`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Mật khẩu không nằm trong blocklist, dài 14; các dữ liệu khác hợp lệ.

**Các bước thực tế:**

1. Tạo MatKhauValidator thật với IConfiguration rỗng và IPasswordBlocklist mock cục bộ.
2. Tạo chuỗi đúng độ dài Unicode, hoặc chứa khoảng trắng; UT-361 nguồn blocklist trả true.
3. Gọi ValidateAsync trực tiếp; không gọi Identity lưu tài khoản hoặc dịch vụ mạng.
4. Assert IdentityResult đúng chấp nhận/từ chối, không cắt chuỗi; UT-361 kiểm gọi blocklist với đúng mật khẩu.

**Kỳ vọng giữ nguyên:** Từ chối về độ dài; không cắt ngầm.

**Quan sát:** success=false; runeCount=14; blocklistCalls=0

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-356",
  "mode": "Behavior",
  "source": "Infrastructure/MatKhauValidator.cs",
  "runeCount": 14,
  "containsWhitespace": false,
  "success": false,
  "errors": [
    {
      "Code": "PasswordPolicy",
      "Description": "Mật khẩu cần từ 15 đến 64 ký tự. Bạn có thể dùng một câu dễ nhớ."
    }
  ],
  "blocklistCalls": 0,
  "externalRequests": 0,
  "assertionFailures": []
}
```

### UT-357 — Mật khẩu dài 15

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `PasswordValidator ứng dụng`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Mật khẩu không nằm trong blocklist, dài 15; các dữ liệu khác hợp lệ.

**Các bước thực tế:**

1. Tạo MatKhauValidator thật với IConfiguration rỗng và IPasswordBlocklist mock cục bộ.
2. Tạo chuỗi đúng độ dài Unicode, hoặc chứa khoảng trắng; UT-361 nguồn blocklist trả true.
3. Gọi ValidateAsync trực tiếp; không gọi Identity lưu tài khoản hoặc dịch vụ mạng.
4. Assert IdentityResult đúng chấp nhận/từ chối, không cắt chuỗi; UT-361 kiểm gọi blocklist với đúng mật khẩu.

**Kỳ vọng giữ nguyên:** Chấp nhận về độ dài.

**Quan sát:** success=true; runeCount=15; blocklistCalls=1

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-357",
  "mode": "Behavior",
  "source": "Infrastructure/MatKhauValidator.cs",
  "runeCount": 15,
  "containsWhitespace": false,
  "success": true,
  "errors": [],
  "blocklistCalls": 1,
  "externalRequests": 0,
  "assertionFailures": []
}
```

### UT-358 — Mật khẩu dài 64

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `PasswordValidator ứng dụng`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Mật khẩu không nằm trong blocklist, dài 64; các dữ liệu khác hợp lệ.

**Các bước thực tế:**

1. Tạo MatKhauValidator thật với IConfiguration rỗng và IPasswordBlocklist mock cục bộ.
2. Tạo chuỗi đúng độ dài Unicode, hoặc chứa khoảng trắng; UT-361 nguồn blocklist trả true.
3. Gọi ValidateAsync trực tiếp; không gọi Identity lưu tài khoản hoặc dịch vụ mạng.
4. Assert IdentityResult đúng chấp nhận/từ chối, không cắt chuỗi; UT-361 kiểm gọi blocklist với đúng mật khẩu.

**Kỳ vọng giữ nguyên:** Chấp nhận về độ dài.

**Quan sát:** success=true; runeCount=64; blocklistCalls=1

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-358",
  "mode": "Behavior",
  "source": "Infrastructure/MatKhauValidator.cs",
  "runeCount": 64,
  "containsWhitespace": false,
  "success": true,
  "errors": [],
  "blocklistCalls": 1,
  "externalRequests": 0,
  "assertionFailures": []
}
```

### UT-359 — Mật khẩu dài 65

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `PasswordValidator ứng dụng`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Mật khẩu không nằm trong blocklist, dài 65; các dữ liệu khác hợp lệ.

**Các bước thực tế:**

1. Tạo MatKhauValidator thật với IConfiguration rỗng và IPasswordBlocklist mock cục bộ.
2. Tạo chuỗi đúng độ dài Unicode, hoặc chứa khoảng trắng; UT-361 nguồn blocklist trả true.
3. Gọi ValidateAsync trực tiếp; không gọi Identity lưu tài khoản hoặc dịch vụ mạng.
4. Assert IdentityResult đúng chấp nhận/từ chối, không cắt chuỗi; UT-361 kiểm gọi blocklist với đúng mật khẩu.

**Kỳ vọng giữ nguyên:** Từ chối về độ dài; không cắt ngầm.

**Quan sát:** success=false; runeCount=65; blocklistCalls=0

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-359",
  "mode": "Behavior",
  "source": "Infrastructure/MatKhauValidator.cs",
  "runeCount": 65,
  "containsWhitespace": false,
  "success": false,
  "errors": [
    {
      "Code": "PasswordPolicy",
      "Description": "Mật khẩu cần từ 15 đến 64 ký tự. Bạn có thể dùng một câu dễ nhớ."
    }
  ],
  "blocklistCalls": 0,
  "externalRequests": 0,
  "assertionFailures": []
}
```

### UT-360 — Mật khẩu có khoảng trắng

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `PasswordValidator ứng dụng`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Chuỗi đủ 15 ký tự gồm khoảng trắng, không phổ biến/đã lộ.

**Các bước thực tế:**

1. Tạo MatKhauValidator thật với IConfiguration rỗng và IPasswordBlocklist mock cục bộ.
2. Tạo chuỗi đúng độ dài Unicode, hoặc chứa khoảng trắng; UT-361 nguồn blocklist trả true.
3. Gọi ValidateAsync trực tiếp; không gọi Identity lưu tài khoản hoặc dịch vụ mạng.
4. Assert IdentityResult đúng chấp nhận/từ chối, không cắt chuỗi; UT-361 kiểm gọi blocklist với đúng mật khẩu.

**Kỳ vọng giữ nguyên:** Không từ chối chỉ do khoảng trắng; không trim hoặc ép loại ký tự.

**Quan sát:** success=true; runeCount=20; blocklistCalls=1

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-360",
  "mode": "Behavior",
  "source": "Infrastructure/MatKhauValidator.cs",
  "runeCount": 20,
  "containsWhitespace": true,
  "success": true,
  "errors": [],
  "blocklistCalls": 1,
  "externalRequests": 0,
  "assertionFailures": []
}
```

### UT-361 — Mật khẩu trong blocklist

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `PasswordValidator ứng dụng`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Chuỗi hợp lệ độ dài nhưng nguồn blocklist cục bộ trả có.

**Các bước thực tế:**

1. Tạo MatKhauValidator thật với IConfiguration rỗng và IPasswordBlocklist mock cục bộ.
2. Tạo chuỗi đúng độ dài Unicode, hoặc chứa khoảng trắng; UT-361 nguồn blocklist trả true.
3. Gọi ValidateAsync trực tiếp; không gọi Identity lưu tài khoản hoặc dịch vụ mạng.
4. Assert IdentityResult đúng chấp nhận/từ chối, không cắt chuỗi; UT-361 kiểm gọi blocklist với đúng mật khẩu.

**Kỳ vọng giữ nguyên:** Từ chối; không gửi mật khẩu ra dịch vụ ngoài.

**Quan sát:** success=false; runeCount=20; blocklistCalls=1

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-361",
  "mode": "Behavior",
  "source": "Infrastructure/MatKhauValidator.cs",
  "runeCount": 20,
  "containsWhitespace": false,
  "success": false,
  "errors": [
    {
      "Code": "PasswordPolicy",
      "Description": "Mật khẩu này quá phổ biến hoặc có trong danh sách đã lộ. Hãy chọn mật khẩu khác."
    }
  ],
  "blocklistCalls": 1,
  "externalRequests": 0,
  "assertionFailures": []
}
```

### UT-362 — Lần sai 5 chờ 30 giây

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `LoginAttemptService policy`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Đồng hồ T; bộ đếm trước tăng=4; thêm một lần sai hợp lệ. Cổng dữ liệu giả trả số lượt mới.

**Các bước thực tế:**

1. Nạp assembly ứng dụng từ project reference của test.
2. Tra đúng tên lớp yêu cầu trong bảng, ghi danh sách lớp khớp (hiện rỗng).
3. Đối chiếu vị trí triển khai liên quan hiện có; không gọi thay một lớp khác để kết luận hành vi.
4. Assert đơn vị thiết kế tồn tại; Fail tại bước này. Nhánh nghiệp vụ riêng chưa được chạy.

**Kỳ vọng giữ nguyên:** Hạn chờ tính đúng T+30 giây.

**Quan sát:** requiredType="LoginAttemptService"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Nguyên nhân:** Không có lớp LoginAttemptService trong assembly. TaiKhoanService gọi usp_GhiNhanDangNhapSai/usp_DangNhapThanhCong; thuật toán đếm/chờ hiện nằm ở SQL. Đây là Fail hợp đồng thiết kế, chưa chạy nhánh thời gian cụ thể của ca.

**Ảnh hưởng và giới hạn kết luận:** Không dùng mock kết quả SQL để khẳng định công thức chờ/timeout SQL đã đúng hay sai.

**Hướng xử lý:** Thống nhất vị trí của quy tắc. Nếu giữ tại SQL, chuyển ca thuật toán sang integration SQL và bổ sung unit cho hợp đồng gọi; nếu yêu cầu unit C#, tách policy thật có clock/store rồi chạy lại.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\TaiKhoanService.cs:64` — `public async Task<string?> DangNhapAsync`.

**Assertion không đạt:**

- Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-362",
  "mode": "DesignContract",
  "requiredType": "LoginAttemptService",
  "found": [],
  "source": "Infrastructure/TaiKhoanService.cs",
  "explanation": "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.",
  "assertionFailures": [
    "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL."
  ]
}
```

### UT-363 — Lần sai 6 chờ 60 giây

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `LoginAttemptService policy`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Đồng hồ T; bộ đếm trước tăng=5; thêm một lần sai hợp lệ. Cổng dữ liệu giả trả số lượt mới.

**Các bước thực tế:**

1. Nạp assembly ứng dụng từ project reference của test.
2. Tra đúng tên lớp yêu cầu trong bảng, ghi danh sách lớp khớp (hiện rỗng).
3. Đối chiếu vị trí triển khai liên quan hiện có; không gọi thay một lớp khác để kết luận hành vi.
4. Assert đơn vị thiết kế tồn tại; Fail tại bước này. Nhánh nghiệp vụ riêng chưa được chạy.

**Kỳ vọng giữ nguyên:** Hạn chờ tính đúng T+60 giây.

**Quan sát:** requiredType="LoginAttemptService"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Nguyên nhân:** Không có lớp LoginAttemptService trong assembly. TaiKhoanService gọi usp_GhiNhanDangNhapSai/usp_DangNhapThanhCong; thuật toán đếm/chờ hiện nằm ở SQL. Đây là Fail hợp đồng thiết kế, chưa chạy nhánh thời gian cụ thể của ca.

**Ảnh hưởng và giới hạn kết luận:** Không dùng mock kết quả SQL để khẳng định công thức chờ/timeout SQL đã đúng hay sai.

**Hướng xử lý:** Thống nhất vị trí của quy tắc. Nếu giữ tại SQL, chuyển ca thuật toán sang integration SQL và bổ sung unit cho hợp đồng gọi; nếu yêu cầu unit C#, tách policy thật có clock/store rồi chạy lại.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\TaiKhoanService.cs:64` — `public async Task<string?> DangNhapAsync`.

**Assertion không đạt:**

- Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-363",
  "mode": "DesignContract",
  "requiredType": "LoginAttemptService",
  "found": [],
  "source": "Infrastructure/TaiKhoanService.cs",
  "explanation": "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.",
  "assertionFailures": [
    "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL."
  ]
}
```

### UT-364 — Lần sai 7 chờ 120 giây

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `LoginAttemptService policy`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Đồng hồ T; bộ đếm trước tăng=6; thêm một lần sai hợp lệ. Cổng dữ liệu giả trả số lượt mới.

**Các bước thực tế:**

1. Nạp assembly ứng dụng từ project reference của test.
2. Tra đúng tên lớp yêu cầu trong bảng, ghi danh sách lớp khớp (hiện rỗng).
3. Đối chiếu vị trí triển khai liên quan hiện có; không gọi thay một lớp khác để kết luận hành vi.
4. Assert đơn vị thiết kế tồn tại; Fail tại bước này. Nhánh nghiệp vụ riêng chưa được chạy.

**Kỳ vọng giữ nguyên:** Hạn chờ tính đúng T+120 giây.

**Quan sát:** requiredType="LoginAttemptService"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Nguyên nhân:** Không có lớp LoginAttemptService trong assembly. TaiKhoanService gọi usp_GhiNhanDangNhapSai/usp_DangNhapThanhCong; thuật toán đếm/chờ hiện nằm ở SQL. Đây là Fail hợp đồng thiết kế, chưa chạy nhánh thời gian cụ thể của ca.

**Ảnh hưởng và giới hạn kết luận:** Không dùng mock kết quả SQL để khẳng định công thức chờ/timeout SQL đã đúng hay sai.

**Hướng xử lý:** Thống nhất vị trí của quy tắc. Nếu giữ tại SQL, chuyển ca thuật toán sang integration SQL và bổ sung unit cho hợp đồng gọi; nếu yêu cầu unit C#, tách policy thật có clock/store rồi chạy lại.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\TaiKhoanService.cs:64` — `public async Task<string?> DangNhapAsync`.

**Assertion không đạt:**

- Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-364",
  "mode": "DesignContract",
  "requiredType": "LoginAttemptService",
  "found": [],
  "source": "Infrastructure/TaiKhoanService.cs",
  "explanation": "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.",
  "assertionFailures": [
    "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL."
  ]
}
```

### UT-365 — Lần sai 8 chờ 300 giây

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `LoginAttemptService policy`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Đồng hồ T; bộ đếm trước tăng=7; thêm một lần sai hợp lệ. Cổng dữ liệu giả trả số lượt mới.

**Các bước thực tế:**

1. Nạp assembly ứng dụng từ project reference của test.
2. Tra đúng tên lớp yêu cầu trong bảng, ghi danh sách lớp khớp (hiện rỗng).
3. Đối chiếu vị trí triển khai liên quan hiện có; không gọi thay một lớp khác để kết luận hành vi.
4. Assert đơn vị thiết kế tồn tại; Fail tại bước này. Nhánh nghiệp vụ riêng chưa được chạy.

**Kỳ vọng giữ nguyên:** Hạn chờ tính đúng T+300 giây.

**Quan sát:** requiredType="LoginAttemptService"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Nguyên nhân:** Không có lớp LoginAttemptService trong assembly. TaiKhoanService gọi usp_GhiNhanDangNhapSai/usp_DangNhapThanhCong; thuật toán đếm/chờ hiện nằm ở SQL. Đây là Fail hợp đồng thiết kế, chưa chạy nhánh thời gian cụ thể của ca.

**Ảnh hưởng và giới hạn kết luận:** Không dùng mock kết quả SQL để khẳng định công thức chờ/timeout SQL đã đúng hay sai.

**Hướng xử lý:** Thống nhất vị trí của quy tắc. Nếu giữ tại SQL, chuyển ca thuật toán sang integration SQL và bổ sung unit cho hợp đồng gọi; nếu yêu cầu unit C#, tách policy thật có clock/store rồi chạy lại.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\TaiKhoanService.cs:64` — `public async Task<string?> DangNhapAsync`.

**Assertion không đạt:**

- Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-365",
  "mode": "DesignContract",
  "requiredType": "LoginAttemptService",
  "found": [],
  "source": "Infrastructure/TaiKhoanService.cs",
  "explanation": "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.",
  "assertionFailures": [
    "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL."
  ]
}
```

### UT-366 — Lần sai 9 chờ 300 giây

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `LoginAttemptService policy`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Đồng hồ T; bộ đếm trước tăng=8; thêm một lần sai hợp lệ. Cổng dữ liệu giả trả số lượt mới.

**Các bước thực tế:**

1. Nạp assembly ứng dụng từ project reference của test.
2. Tra đúng tên lớp yêu cầu trong bảng, ghi danh sách lớp khớp (hiện rỗng).
3. Đối chiếu vị trí triển khai liên quan hiện có; không gọi thay một lớp khác để kết luận hành vi.
4. Assert đơn vị thiết kế tồn tại; Fail tại bước này. Nhánh nghiệp vụ riêng chưa được chạy.

**Kỳ vọng giữ nguyên:** Hạn chờ tính đúng T+300 giây.

**Quan sát:** requiredType="LoginAttemptService"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Nguyên nhân:** Không có lớp LoginAttemptService trong assembly. TaiKhoanService gọi usp_GhiNhanDangNhapSai/usp_DangNhapThanhCong; thuật toán đếm/chờ hiện nằm ở SQL. Đây là Fail hợp đồng thiết kế, chưa chạy nhánh thời gian cụ thể của ca.

**Ảnh hưởng và giới hạn kết luận:** Không dùng mock kết quả SQL để khẳng định công thức chờ/timeout SQL đã đúng hay sai.

**Hướng xử lý:** Thống nhất vị trí của quy tắc. Nếu giữ tại SQL, chuyển ca thuật toán sang integration SQL và bổ sung unit cho hợp đồng gọi; nếu yêu cầu unit C#, tách policy thật có clock/store rồi chạy lại.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\TaiKhoanService.cs:64` — `public async Task<string?> DangNhapAsync`.

**Assertion không đạt:**

- Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-366",
  "mode": "DesignContract",
  "requiredType": "LoginAttemptService",
  "found": [],
  "source": "Infrastructure/TaiKhoanService.cs",
  "explanation": "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.",
  "assertionFailures": [
    "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL."
  ]
}
```

### UT-367 — Một trong hai bộ đếm còn chờ

**Kết quả:** Failed; **Tiêu chuẩn:** Xác thực; **Đích:** `LoginAttemptService policy`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Tài khoản hết chờ, IP còn chờ 20 giây; làm thêm dataset đảo hai bộ đếm.

**Các bước thực tế:**

1. Nạp assembly ứng dụng từ project reference của test.
2. Tra đúng tên lớp yêu cầu trong bảng, ghi danh sách lớp khớp (hiện rỗng).
3. Đối chiếu vị trí triển khai liên quan hiện có; không gọi thay một lớp khác để kết luận hành vi.
4. Assert đơn vị thiết kế tồn tại; Fail tại bước này. Nhánh nghiệp vụ riêng chưa được chạy.

**Kỳ vọng giữ nguyên:** Từ chối thử xác thực; Retry-After phù hợp khoảng còn chờ; không tăng đếm/kéo dài chờ.

**Quan sát:** requiredType="LoginAttemptService"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Nguyên nhân:** Không có lớp LoginAttemptService trong assembly. TaiKhoanService gọi usp_GhiNhanDangNhapSai/usp_DangNhapThanhCong; thuật toán đếm/chờ hiện nằm ở SQL. Đây là Fail hợp đồng thiết kế, chưa chạy nhánh thời gian cụ thể của ca.

**Ảnh hưởng và giới hạn kết luận:** Không dùng mock kết quả SQL để khẳng định công thức chờ/timeout SQL đã đúng hay sai.

**Hướng xử lý:** Thống nhất vị trí của quy tắc. Nếu giữ tại SQL, chuyển ca thuật toán sang integration SQL và bổ sung unit cho hợp đồng gọi; nếu yêu cầu unit C#, tách policy thật có clock/store rồi chạy lại.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\TaiKhoanService.cs:64` — `public async Task<string?> DangNhapAsync`.

**Assertion không đạt:**

- Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-367",
  "mode": "DesignContract",
  "requiredType": "LoginAttemptService",
  "found": [],
  "source": "Infrastructure/TaiKhoanService.cs",
  "explanation": "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.",
  "assertionFailures": [
    "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL."
  ]
}
```

### UT-368 — Đăng nhập thành công chỉ xóa đếm tài khoản

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `LoginAttemptService policy`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Có bộ đếm tài khoản và IP; xác thực thành công.

**Các bước thực tế:**

1. Nạp assembly ứng dụng từ project reference của test.
2. Tra đúng tên lớp yêu cầu trong bảng, ghi danh sách lớp khớp (hiện rỗng).
3. Đối chiếu vị trí triển khai liên quan hiện có; không gọi thay một lớp khác để kết luận hành vi.
4. Assert đơn vị thiết kế tồn tại; Fail tại bước này. Nhánh nghiệp vụ riêng chưa được chạy.

**Kỳ vọng giữ nguyên:** Xóa bộ đếm tài khoản; giữ bộ đếm IP.

**Quan sát:** requiredType="LoginAttemptService"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Nguyên nhân:** Không có lớp LoginAttemptService trong assembly. TaiKhoanService gọi usp_GhiNhanDangNhapSai/usp_DangNhapThanhCong; thuật toán đếm/chờ hiện nằm ở SQL. Đây là Fail hợp đồng thiết kế, chưa chạy nhánh thời gian cụ thể của ca.

**Ảnh hưởng và giới hạn kết luận:** Không dùng mock kết quả SQL để khẳng định công thức chờ/timeout SQL đã đúng hay sai.

**Hướng xử lý:** Thống nhất vị trí của quy tắc. Nếu giữ tại SQL, chuyển ca thuật toán sang integration SQL và bổ sung unit cho hợp đồng gọi; nếu yêu cầu unit C#, tách policy thật có clock/store rồi chạy lại.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\TaiKhoanService.cs:64` — `public async Task<string?> DangNhapAsync`.

**Assertion không đạt:**

- Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-368",
  "mode": "DesignContract",
  "requiredType": "LoginAttemptService",
  "found": [],
  "source": "Infrastructure/TaiKhoanService.cs",
  "explanation": "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.",
  "assertionFailures": [
    "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL."
  ]
}
```

### UT-369 — Phiên nhàn rỗi 1799 giây

**Kết quả:** Failed; **Tiêu chuẩn:** Xác thực; **Đích:** `UserSession policy`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Lần thao tác cuối T; now=T+1799 giây.

**Các bước thực tế:**

1. Nạp assembly ứng dụng từ project reference của test.
2. Tra đúng tên lớp yêu cầu trong bảng, ghi danh sách lớp khớp (hiện rỗng).
3. Đối chiếu vị trí triển khai liên quan hiện có; không gọi thay một lớp khác để kết luận hành vi.
4. Assert đơn vị thiết kế tồn tại; Fail tại bước này. Nhánh nghiệp vụ riêng chưa được chạy.

**Kỳ vọng giữ nguyên:** Phiên chưa hết hạn do nhàn rỗi.

**Quan sát:** requiredType="UserSession"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Nguyên nhân:** Không có lớp UserSession trong assembly. PhienDangNhapService.HopLeAsync gọi usp_KiemTraPhien; không có đối tượng C# UserSession để kiểm nhàn rỗi/LastActivity theo bảng. Đây là Fail hợp đồng thiết kế, chưa chạy nhánh thời gian cụ thể của ca.

**Ảnh hưởng và giới hạn kết luận:** Không dùng mock kết quả SQL để khẳng định công thức chờ/timeout SQL đã đúng hay sai.

**Hướng xử lý:** Thống nhất vị trí của quy tắc. Nếu giữ tại SQL, chuyển ca thuật toán sang integration SQL và bổ sung unit cho hợp đồng gọi; nếu yêu cầu unit C#, tách policy thật có clock/store rồi chạy lại.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\PhienDangNhapService.cs:16` — `public async Task<bool> HopLeAsync`.

**Assertion không đạt:**

- Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-369",
  "mode": "DesignContract",
  "requiredType": "UserSession",
  "found": [],
  "source": "Infrastructure/PhienDangNhapService.cs",
  "explanation": "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.",
  "assertionFailures": [
    "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL."
  ]
}
```

### UT-370 — Phiên nhàn rỗi 1800 giây

**Kết quả:** Failed; **Tiêu chuẩn:** Xác thực; **Đích:** `UserSession policy`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Lần thao tác cuối T; now=T+1800 giây.

**Các bước thực tế:**

1. Nạp assembly ứng dụng từ project reference của test.
2. Tra đúng tên lớp yêu cầu trong bảng, ghi danh sách lớp khớp (hiện rỗng).
3. Đối chiếu vị trí triển khai liên quan hiện có; không gọi thay một lớp khác để kết luận hành vi.
4. Assert đơn vị thiết kế tồn tại; Fail tại bước này. Nhánh nghiệp vụ riêng chưa được chạy.

**Kỳ vọng giữ nguyên:** Phiên hết hạn; không Allow nghiệp vụ.

**Quan sát:** requiredType="UserSession"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Nguyên nhân:** Không có lớp UserSession trong assembly. PhienDangNhapService.HopLeAsync gọi usp_KiemTraPhien; không có đối tượng C# UserSession để kiểm nhàn rỗi/LastActivity theo bảng. Đây là Fail hợp đồng thiết kế, chưa chạy nhánh thời gian cụ thể của ca.

**Ảnh hưởng và giới hạn kết luận:** Không dùng mock kết quả SQL để khẳng định công thức chờ/timeout SQL đã đúng hay sai.

**Hướng xử lý:** Thống nhất vị trí của quy tắc. Nếu giữ tại SQL, chuyển ca thuật toán sang integration SQL và bổ sung unit cho hợp đồng gọi; nếu yêu cầu unit C#, tách policy thật có clock/store rồi chạy lại.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\PhienDangNhapService.cs:16` — `public async Task<bool> HopLeAsync`.

**Assertion không đạt:**

- Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-370",
  "mode": "DesignContract",
  "requiredType": "UserSession",
  "found": [],
  "source": "Infrastructure/PhienDangNhapService.cs",
  "explanation": "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.",
  "assertionFailures": [
    "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL."
  ]
}
```

### UT-371 — Ping nền không gia hạn

**Kết quả:** Failed; **Tiêu chuẩn:** Xác thực; **Đích:** `UserSession policy`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Phiên có LastActivity=T; nhận ping SignalR hoặc tải nền.

**Các bước thực tế:**

1. Nạp assembly ứng dụng từ project reference của test.
2. Tra đúng tên lớp yêu cầu trong bảng, ghi danh sách lớp khớp (hiện rỗng).
3. Đối chiếu vị trí triển khai liên quan hiện có; không gọi thay một lớp khác để kết luận hành vi.
4. Assert đơn vị thiết kế tồn tại; Fail tại bước này. Nhánh nghiệp vụ riêng chưa được chạy.

**Kỳ vọng giữ nguyên:** LastActivity vẫn T; không kéo dài 30 phút.

**Quan sát:** requiredType="UserSession"; found=[]; Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Nguyên nhân:** Không có lớp UserSession trong assembly. PhienDangNhapService.HopLeAsync gọi usp_KiemTraPhien; không có đối tượng C# UserSession để kiểm nhàn rỗi/LastActivity theo bảng. Đây là Fail hợp đồng thiết kế, chưa chạy nhánh thời gian cụ thể của ca.

**Ảnh hưởng và giới hạn kết luận:** Không dùng mock kết quả SQL để khẳng định công thức chờ/timeout SQL đã đúng hay sai.

**Hướng xử lý:** Thống nhất vị trí của quy tắc. Nếu giữ tại SQL, chuyển ca thuật toán sang integration SQL và bổ sung unit cho hợp đồng gọi; nếu yêu cầu unit C#, tách policy thật có clock/store rồi chạy lại.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Infrastructure\PhienDangNhapService.cs:16` — `public async Task<bool> HopLeAsync`.

**Assertion không đạt:**

- Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-371",
  "mode": "DesignContract",
  "requiredType": "UserSession",
  "found": [],
  "source": "Infrastructure/PhienDangNhapService.cs",
  "explanation": "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.",
  "assertionFailures": [
    "Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL."
  ]
}
```

## Chạy lại trong VS Code

Mở thư mục `C:\Khóa luận TN\Educonnect`, Terminal PowerShell:

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter "Suite=Remaining14" --logger "trx;LogFileName=Group14-Unit.trx" --results-directory "TestResults/14-TaiKhoan-HoSo-Unit"

dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter "FullyQualifiedName~EduConnect.Remaining.UnitTests.RemainingUnitTest.UT_349"
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
