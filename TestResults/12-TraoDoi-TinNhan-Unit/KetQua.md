# Kết quả 12. Trao đổi và nhắn tin

**8 ca: 7 Pass, 1 Fail, 0 Skip/Blocked.**

Trong các Fail có **0 ca kiểm hợp đồng thiết kế** (thiếu lớp/trường input), dừng trước khi chạy nhánh nghiệp vụ; các Fail còn lại có quan sát hành vi thật khác kỳ vọng. Không đồng nhất Fail hợp đồng với lỗi thuật toán đã chạy.

Nguồn: Unit Test.xlsx đã đối chiếu lại trực tiếp với workbook hiện tại, giữ nguyên mã/thứ tự và ba tiêu chuẩn xác thực, đúng đắn, đầy đủ của từng dòng.

## Phạm vi

Real service/filter/validator logic. Persistence, locks, server clock, Identity, notices or scanner replaced at boundaries. DesignContract cases stop at missing input/type; their business branches are not executed.

Riêng UT-304/305 dùng KetThucWorkerFixture cũ: H/G/X/C/R lần lượt prefix 1/2/3/4/5; GUID cụ thể nằm trong quan sát. Các ca khác dùng fixture mới theo từng Fact. Dữ liệu U/G/P/A/C/K/Room/M lần lượt có prefix GUID 1/2/3/4/5/6/7/8; RV1=[1,2,3,4,5,6,7,8]. T cố định cho cổng thời gian là 2026-11-14 10:00:00+07:00. Các chỗ code tự dùng UtcNow được ghi nhận đúng thời gian thực, không giả mạo bằng T.

Không gọi CSDL thật. Fake IQueryable chỉ cung cấp dữ liệu để chạy predicate C# thật; không chứng minh câu SQL sinh ra/constraint/lock/rollback. Mock Identity/files/notice là dependency; không mock service đang kiểm.

## Bảng kết quả

| Mã | Tình huống | Tiêu chuẩn | Kết quả | Quan sát |
| --- | --- | --- | --- | --- |
| UT-323 | Chưa xác nhận kết nối | Xác thực | Pass | status=403; message="Cần thuộc liên hệ đã được chấp nhận để đọc/gửi tin nhắn."; saves=0; commits=0; Add= |
| UT-324 | Lớp đã kết thúc | Xác thực | Pass | status=403; message="Bạn không có quyền nhắn tin trong lớp này."; saves=0; commits=0; Add= |
| UT-325 | Lịch sử lớp kết thúc được giữ | Đầy đủ | Pass | status=200; message=null; saves=0; commits=0; Add= |
| UT-326 | Tin rỗng | Đầy đủ | Pass | status=400; message="Nhập nội dung hoặc chọn ít nhất một tệp, tối đa 10000 ký tự."; saves=0; commits=0; Add= |
| UT-327 | Gửi tin hợp lệ | Đúng đắn | Pass | status=200; message=null; saves=2; commits=1; Add=A,ChoXuLy |
| UT-328 | Gửi lại cùng nội dung | Đúng đắn | Fail hành vi | status=200; message=null; saves=0; commits=0; returnType="Task"; Add=; Không thêm trùng đã đạt; nhưng GuiAsync trả Task không mang tin đã lưu như hợp đồng kịch bản. |
| UT-329 | Cùng mã khác nội dung | Đúng đắn | Pass | status=409; message="Mã thao tác đã dùng cho tin nhắn khác."; saves=0; commits=0; Add= |
| UT-330 | Gửi lại sau thu hồi quyền | Xác thực | Pass | status=403; message="Quyền đã thu hồi"; saves=0; commits=0; Add= |

## Chi tiết từng ca và bước thực hiện

### UT-323 — Chưa xác nhận kết nối

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `TinNhanService quyền hội thoại`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Chưa có kết nối hoặc đang chờ xác nhận.

**Các bước thực tế:**

1. Tạo TinNhanService thật; nguồn dữ liệu async/filerights/files/transaction giả, fixture mới cho từng ca.
2. Seed hội thoại và lớp; đặt trạng thái liên hệ/lớp theo ca. Mã ClientMessageId trong bảng ánh xạ MaThaoTac thật; retry seed tin M1/A đã lưu trước đó.
3. UT-323/325 gọi LichSuAsync; các ca còn lại gọi GuiAsync. UT-330 nguồn quyền ném 403 để mô phỏng quyền đã bị thu hồi ngay trước retry.
4. Thu exception/status, lịch sử, tin và outbox đã Add, thứ tự Save/Commit; assert không ghi khi từ chối, giữ A khi xung đột, không tạo trùng khi retry. UT-328 kiểm thêm hợp đồng trả tin đã lưu.

**Kỳ vọng giữ nguyên:** Từ chối mở kênh theo UC5.1; không trả lịch sử riêng tư.

**Quan sát:** status=403; message="Cần thuộc liên hệ đã được chấp nhận để đọc/gửi tin nhắn."; saves=0; commits=0; Add=

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-323",
  "mode": "Behavior",
  "source": "Areas/HocTap/Services/TinNhanService.cs",
  "status": 403,
  "message": "Cần thuộc liên hệ đã được chấp nhận để đọc/gửi tin nhắn.",
  "history": null,
  "added": [],
  "saves": 0,
  "commits": 0,
  "order": [],
  "oldText": null,
  "mapping": "ClientMessageId trong kịch bản được ánh xạ MaThaoTac của GuiTinNhanViewModel.",
  "assertionFailures": []
}
```

### UT-324 — Lớp đã kết thúc

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `TinNhanService gửi tin`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Người dùng còn quyền đọc nhưng lớp DaKetThuc.

**Các bước thực tế:**

1. Tạo TinNhanService thật; nguồn dữ liệu async/filerights/files/transaction giả, fixture mới cho từng ca.
2. Seed hội thoại và lớp; đặt trạng thái liên hệ/lớp theo ca. Mã ClientMessageId trong bảng ánh xạ MaThaoTac thật; retry seed tin M1/A đã lưu trước đó.
3. UT-323/325 gọi LichSuAsync; các ca còn lại gọi GuiAsync. UT-330 nguồn quyền ném 403 để mô phỏng quyền đã bị thu hồi ngay trước retry.
4. Thu exception/status, lịch sử, tin và outbox đã Add, thứ tự Save/Commit; assert không ghi khi từ chối, giữ A khi xung đột, không tạo trùng khi retry. UT-328 kiểm thêm hợp đồng trả tin đã lưu.

**Kỳ vọng giữ nguyên:** Từ chối gửi mới; không thêm tin.

**Quan sát:** status=403; message="Bạn không có quyền nhắn tin trong lớp này."; saves=0; commits=0; Add=

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-324",
  "mode": "Behavior",
  "source": "Areas/HocTap/Services/TinNhanService.cs",
  "status": 403,
  "message": "Bạn không có quyền nhắn tin trong lớp này.",
  "history": null,
  "added": [],
  "saves": 0,
  "commits": 0,
  "order": [
    "Begin"
  ],
  "oldText": null,
  "mapping": "ClientMessageId trong kịch bản được ánh xạ MaThaoTac của GuiTinNhanViewModel.",
  "assertionFailures": []
}
```

### UT-325 — Lịch sử lớp kết thúc được giữ

**Kết quả:** Passed; **Tiêu chuẩn:** Đầy đủ; **Đích:** `TinNhanService đọc lịch sử`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Lớp kết thúc; người đọc vẫn có quyền xem.

**Các bước thực tế:**

1. Tạo TinNhanService thật; nguồn dữ liệu async/filerights/files/transaction giả, fixture mới cho từng ca.
2. Seed hội thoại và lớp; đặt trạng thái liên hệ/lớp theo ca. Mã ClientMessageId trong bảng ánh xạ MaThaoTac thật; retry seed tin M1/A đã lưu trước đó.
3. UT-323/325 gọi LichSuAsync; các ca còn lại gọi GuiAsync. UT-330 nguồn quyền ném 403 để mô phỏng quyền đã bị thu hồi ngay trước retry.
4. Thu exception/status, lịch sử, tin và outbox đã Add, thứ tự Save/Commit; assert không ghi khi từ chối, giữ A khi xung đột, không tạo trùng khi retry. UT-328 kiểm thêm hợp đồng trả tin đã lưu.

**Kỳ vọng giữ nguyên:** Trả lịch sử cũ theo quyền; không xóa hoặc sửa tin.

**Quan sát:** status=200; message=null; saves=0; commits=0; Add=

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-325",
  "mode": "Behavior",
  "source": "Areas/HocTap/Services/TinNhanService.cs",
  "status": 200,
  "message": null,
  "history": [
    {
      "Id": "60000000-0000-0000-0000-000000000006",
      "CuocHoiThoaiId": "70000000-0000-0000-0000-000000000007",
      "NguoiGuiId": "10000000-0000-0000-0000-000000000001",
      "NoiDung": "A",
      "GuiLuc": "2026-11-14T10:00:00+07:00",
      "MaThaoTac": "80000000-0000-0000-0000-000000000008"
    }
  ],
  "added": [],
  "saves": 0,
  "commits": 0,
  "order": [],
  "oldText": "A",
  "mapping": "ClientMessageId trong kịch bản được ánh xạ MaThaoTac của GuiTinNhanViewModel.",
  "assertionFailures": []
}
```

### UT-326 — Tin rỗng

**Kết quả:** Passed; **Tiêu chuẩn:** Đầy đủ; **Đích:** `TinNhanService.GuiAsync`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Nội dung rỗng và không tệp.

**Các bước thực tế:**

1. Tạo TinNhanService thật; nguồn dữ liệu async/filerights/files/transaction giả, fixture mới cho từng ca.
2. Seed hội thoại và lớp; đặt trạng thái liên hệ/lớp theo ca. Mã ClientMessageId trong bảng ánh xạ MaThaoTac thật; retry seed tin M1/A đã lưu trước đó.
3. UT-323/325 gọi LichSuAsync; các ca còn lại gọi GuiAsync. UT-330 nguồn quyền ném 403 để mô phỏng quyền đã bị thu hồi ngay trước retry.
4. Thu exception/status, lịch sử, tin và outbox đã Add, thứ tự Save/Commit; assert không ghi khi từ chối, giữ A khi xung đột, không tạo trùng khi retry. UT-328 kiểm thêm hợp đồng trả tin đã lưu.

**Kỳ vọng giữ nguyên:** Trả lỗi; không lưu và không báo gửi thành công.

**Quan sát:** status=400; message="Nhập nội dung hoặc chọn ít nhất một tệp, tối đa 10000 ký tự."; saves=0; commits=0; Add=

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-326",
  "mode": "Behavior",
  "source": "Areas/HocTap/Services/TinNhanService.cs",
  "status": 400,
  "message": "Nhập nội dung hoặc chọn ít nhất một tệp, tối đa 10000 ký tự.",
  "history": null,
  "added": [],
  "saves": 0,
  "commits": 0,
  "order": [],
  "oldText": null,
  "mapping": "ClientMessageId trong kịch bản được ánh xạ MaThaoTac của GuiTinNhanViewModel.",
  "assertionFailures": []
}
```

### UT-327 — Gửi tin hợp lệ

**Kết quả:** Passed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `TinNhanService.GuiAsync`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Đúng quyền; ClientMessageId=M1, nội dung A.

**Các bước thực tế:**

1. Tạo TinNhanService thật; nguồn dữ liệu async/filerights/files/transaction giả, fixture mới cho từng ca.
2. Seed hội thoại và lớp; đặt trạng thái liên hệ/lớp theo ca. Mã ClientMessageId trong bảng ánh xạ MaThaoTac thật; retry seed tin M1/A đã lưu trước đó.
3. UT-323/325 gọi LichSuAsync; các ca còn lại gọi GuiAsync. UT-330 nguồn quyền ném 403 để mô phỏng quyền đã bị thu hồi ngay trước retry.
4. Thu exception/status, lịch sử, tin và outbox đã Add, thứ tự Save/Commit; assert không ghi khi từ chối, giữ A khi xung đột, không tạo trùng khi retry. UT-328 kiểm thêm hợp đồng trả tin đã lưu.

**Kỳ vọng giữ nguyên:** Lưu đúng người gửi/hội thoại/nội dung; chỉ báo thành công sau lưu.

**Quan sát:** status=200; message=null; saves=2; commits=1; Add=A,ChoXuLy

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-327",
  "mode": "Behavior",
  "source": "Areas/HocTap/Services/TinNhanService.cs",
  "status": 200,
  "message": null,
  "history": null,
  "added": [
    {
      "Id": "fc18e90b-4ea5-49ef-ad76-cba0c294ab3f",
      "CuocHoiThoaiId": "70000000-0000-0000-0000-000000000007",
      "NguoiGuiId": "10000000-0000-0000-0000-000000000001",
      "NoiDung": "A",
      "GuiLuc": "2026-11-14T08:03:02.7969835+00:00",
      "MaThaoTac": "80000000-0000-0000-0000-000000000008"
    },
    {
      "Id": "5ed11c85-9d88-435b-ac1d-47e6e4630149",
      "LoaiSuKien": "TinNhan",
      "DuLieuToiThieu": "{\"room\":\"70000000-0000-0000-0000-000000000007\",\"target\":\"fc18e90b-4ea5-49ef-ad76-cba0c294ab3f\"}",
      "TaoLuc": "2026-11-14T08:03:02.8037613+00:00",
      "TrangThai": "ChoXuLy",
      "SoLanThu": 0,
      "ThuLaiLuc": null,
      "XuLyXongLuc": null,
      "RowVersion": "",
      "LeaseId": null,
      "LeaseHetHanLuc": null
    }
  ],
  "saves": 2,
  "commits": 1,
  "order": [
    "Begin",
    "Add:TinNhan",
    "Save",
    "Add:SuKienChoXuLy",
    "Save",
    "Commit"
  ],
  "oldText": null,
  "mapping": "ClientMessageId trong kịch bản được ánh xạ MaThaoTac của GuiTinNhanViewModel.",
  "assertionFailures": []
}
```

### UT-328 — Gửi lại cùng nội dung

**Kết quả:** Failed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `TinNhanService.GuiAsync`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. M1 của U1 đã lưu A; U1 còn quyền, gửi lại M1/A.

**Các bước thực tế:**

1. Tạo TinNhanService thật; nguồn dữ liệu async/filerights/files/transaction giả, fixture mới cho từng ca.
2. Seed hội thoại và lớp; đặt trạng thái liên hệ/lớp theo ca. Mã ClientMessageId trong bảng ánh xạ MaThaoTac thật; retry seed tin M1/A đã lưu trước đó.
3. UT-323/325 gọi LichSuAsync; các ca còn lại gọi GuiAsync. UT-330 nguồn quyền ném 403 để mô phỏng quyền đã bị thu hồi ngay trước retry.
4. Thu exception/status, lịch sử, tin và outbox đã Add, thứ tự Save/Commit; assert không ghi khi từ chối, giữ A khi xung đột, không tạo trùng khi retry. UT-328 kiểm thêm hợp đồng trả tin đã lưu.

**Kỳ vọng giữ nguyên:** Trả tin đã lưu; không thêm tin hoặc sự kiện trùng.

**Quan sát:** status=200; message=null; saves=0; commits=0; returnType="Task"; Add=; Không thêm trùng đã đạt; nhưng GuiAsync trả Task không mang tin đã lưu như hợp đồng kịch bản.

**Nguyên nhân:** Nhánh old != null kiểm hội thoại/nội dung/tệp rồi return; GuiAsync trả Task. Không thêm tin/outbox đã đạt, nhưng không trả tin đã lưu như cột kỳ vọng.

**Ảnh hưởng và giới hạn kết luận:** Sai hợp đồng giá trị trả về; không phải lỗi tạo tin trùng.

**Hướng xử lý:** Chốt hợp đồng retry: nếu cần trả tin đã lưu thì dùng kết quả có dữ liệu/mã tin; nếu chỉ cần hoàn tất không thêm trùng, sửa thiết kế sau khi thống nhất. Không tự đổi kỳ vọng của lần chạy này.

**Vị trí code cần trỏ:**

- `C:\Khóa luận TN\Educonnect\Areas\HocTap\Services\TinNhanService.cs:95` — `public async Task GuiAsync`.
- `C:\Khóa luận TN\Educonnect\Areas\HocTap\Services\TinNhanService.cs:107` — `if (old != null)`.

**Assertion không đạt:**

- Không thêm trùng đã đạt; nhưng GuiAsync trả Task không mang tin đã lưu như hợp đồng kịch bản.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-328",
  "mode": "Behavior",
  "source": "Areas/HocTap/Services/TinNhanService.cs",
  "status": 200,
  "message": null,
  "history": null,
  "added": [],
  "saves": 0,
  "commits": 0,
  "order": [
    "Begin"
  ],
  "oldText": "A",
  "mapping": "ClientMessageId trong kịch bản được ánh xạ MaThaoTac của GuiTinNhanViewModel.",
  "returnType": "Task",
  "assertionFailures": [
    "Không thêm trùng đã đạt; nhưng GuiAsync trả Task không mang tin đã lưu như hợp đồng kịch bản."
  ]
}
```

### UT-329 — Cùng mã khác nội dung

**Kết quả:** Passed; **Tiêu chuẩn:** Đúng đắn; **Đích:** `TinNhanService.GuiAsync`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. M1 của U1 đã lưu A; gửi lại M1/B.

**Các bước thực tế:**

1. Tạo TinNhanService thật; nguồn dữ liệu async/filerights/files/transaction giả, fixture mới cho từng ca.
2. Seed hội thoại và lớp; đặt trạng thái liên hệ/lớp theo ca. Mã ClientMessageId trong bảng ánh xạ MaThaoTac thật; retry seed tin M1/A đã lưu trước đó.
3. UT-323/325 gọi LichSuAsync; các ca còn lại gọi GuiAsync. UT-330 nguồn quyền ném 403 để mô phỏng quyền đã bị thu hồi ngay trước retry.
4. Thu exception/status, lịch sử, tin và outbox đã Add, thứ tự Save/Commit; assert không ghi khi từ chối, giữ A khi xung đột, không tạo trùng khi retry. UT-328 kiểm thêm hợp đồng trả tin đã lưu.

**Kỳ vọng giữ nguyên:** 409; không đổi A sang B.

**Quan sát:** status=409; message="Mã thao tác đã dùng cho tin nhắn khác."; saves=0; commits=0; Add=

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-329",
  "mode": "Behavior",
  "source": "Areas/HocTap/Services/TinNhanService.cs",
  "status": 409,
  "message": "Mã thao tác đã dùng cho tin nhắn khác.",
  "history": null,
  "added": [],
  "saves": 0,
  "commits": 0,
  "order": [
    "Begin"
  ],
  "oldText": "A",
  "mapping": "ClientMessageId trong kịch bản được ánh xạ MaThaoTac của GuiTinNhanViewModel.",
  "assertionFailures": []
}
```

### UT-330 — Gửi lại sau thu hồi quyền

**Kết quả:** Passed; **Tiêu chuẩn:** Xác thực; **Đích:** `TinNhanService.GuiAsync`.

**Dữ liệu/tiền điều kiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. M1 từng thành công; quyền U1 vừa bị thu hồi; gửi lại M1/A.

**Các bước thực tế:**

1. Tạo TinNhanService thật; nguồn dữ liệu async/filerights/files/transaction giả, fixture mới cho từng ca.
2. Seed hội thoại và lớp; đặt trạng thái liên hệ/lớp theo ca. Mã ClientMessageId trong bảng ánh xạ MaThaoTac thật; retry seed tin M1/A đã lưu trước đó.
3. UT-323/325 gọi LichSuAsync; các ca còn lại gọi GuiAsync. UT-330 nguồn quyền ném 403 để mô phỏng quyền đã bị thu hồi ngay trước retry.
4. Thu exception/status, lịch sử, tin và outbox đã Add, thứ tự Save/Commit; assert không ghi khi từ chối, giữ A khi xung đột, không tạo trùng khi retry. UT-328 kiểm thêm hợp đồng trả tin đã lưu.

**Kỳ vọng giữ nguyên:** Từ chối do quyền hiện tại; không dùng idempotency để bỏ qua phân quyền.

**Quan sát:** status=403; message="Quyền đã thu hồi"; saves=0; commits=0; Add=

**Assertion:** các kiểm tra trong nhánh ca này đều đạt; giá trị và số lần gọi được lưu trong quan sát dưới đây.

**Bằng chứng thu từ lần chạy:**

```json
{
  "id": "UT-330",
  "mode": "Behavior",
  "source": "Areas/HocTap/Services/TinNhanService.cs",
  "status": 403,
  "message": "Quyền đã thu hồi",
  "history": null,
  "added": [],
  "saves": 0,
  "commits": 0,
  "order": [
    "Begin"
  ],
  "oldText": "A",
  "mapping": "ClientMessageId trong kịch bản được ánh xạ MaThaoTac của GuiTinNhanViewModel.",
  "assertionFailures": []
}
```

## Chạy lại trong VS Code

Mở thư mục `C:\Khóa luận TN\Educonnect`, Terminal PowerShell:

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter "Suite=Remaining12" --logger "trx;LogFileName=Group12-Unit.trx" --results-directory "TestResults/12-TraoDoi-TinNhan-Unit"

dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter "FullyQualifiedName~EduConnect.Remaining.UnitTests.RemainingUnitTest.UT_323"
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
