# Kết quả unit test — 06. Controller kết nối và kết thúc

Ngày thực hiện: 14/11/2026 (Asia/Saigon). Đối chiếu Unit Test.xlsx: UT-106–169.

**64 ca đã chạy: 8 Pass, 56 Fail, 0 Skip/Blocked.** Các action và overload yêu cầu đều có trong code; đợt này kiểm hành vi controller thật, không thay tên action để chạy.

**56 ca Fail không đồng nghĩa 56 lỗi độc lập.** Nhiều mã lỗi service cùng đi qua một nhánh xử lý thiếu/sai, nên cùng nguyên nhân ảnh hưởng nhiều TC.

## Tổng hợp theo action

| Action | Mã TC | Pass | Fail |
|---|---|---:|---:|
| GuiYeuCau | UT-106–113 | 0 | 8 |
| GuiDeXuat | UT-114–121 | 1 | 7 |
| ChapNhan | UT-122–129 | 2 | 6 |
| TuChoi | UT-130–137 | 1 | 7 |
| Rut | UT-138–145 | 1 | 7 |
| XacNhanKetThuc | UT-146–153 | 1 | 7 |
| HuyKetNoi | UT-154–161 | 1 | 7 |
| PhanHoiHuy | UT-162–169 | 1 | 7 |

Các ca Pass: **UT-121, UT-123, UT-129, UT-137, UT-145, UT-153, UT-161, UT-169**.

## Các vấn đề nổi bật

1. **ModelState lỗi nhưng vẫn gọi service và báo thành công:** ChapNhan, TuChoi, Rut, XacNhanKetThuc, HuyKetNoi. Ví dụ UT-122 quan sát được ChapNhanDeXuatAsync gọi một lần dù ModelState đang lỗi, trái kỳ vọng không gọi service.
2. **ModelState lỗi bị ném exception thay BadRequestObjectResult:** GuiYeuCau và PhanHoiHuy. GuiDeXuat giữ lỗi nhưng trả ViewResult.
3. **Lỗi service không được ánh xạ sang kiểu kết quả theo thiết kế:** 35 ca ném exception ra khỏi action; 5 ca GuiDeXuat trả ViewResult. Không ca lỗi service nào đáp ứng đủ Forbid/NotFound/Conflict/BadRequest và ModelState theo bảng.
4. **Thông báo/đích điều hướng sau thành công khác TC:** bảy ca thành công Fail. HuyKetNoi và PhanHoiHuy còn trả RedirectResult tới URL lớp thay RedirectToAction(Index).
5. **UT-113 yêu cầu RV1 nhưng DTO GuiYeuCauViewModel không có RowVersion:** các trường khác và actor/token đã đúng; không bỏ qua phần thiếu này để báo Pass.

## Kết quả từng ca

| Mã | Action / tình huống | Kết quả | Quan sát thực tế |
|---|---|---|---|
| UT-106 | GuiYeuCau — ModelState lỗi | Fail | Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi GuiYeuCauAsync 0 lần. |
| UT-107 | GuiYeuCau — Thành công | Fail | RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi GuiYeuCauAsync 1 lần. |
| UT-108 | GuiYeuCau — Service trả mã 403 | Fail | Ném LoiNghiepVu(Status=403), message="Lỗi mẫu"; không trả IActionResult. Gọi GuiYeuCauAsync 1 lần. |
| UT-109 | GuiYeuCau — Service trả mã 404 | Fail | Ném LoiNghiepVu(Status=404), message="Lỗi mẫu"; không trả IActionResult. Gọi GuiYeuCauAsync 1 lần. |
| UT-110 | GuiYeuCau — Service trả mã 409 | Fail | Ném LoiNghiepVu(Status=409), message="Lỗi mẫu"; không trả IActionResult. Gọi GuiYeuCauAsync 1 lần. |
| UT-111 | GuiYeuCau — Service trả mã 400 | Fail | Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi GuiYeuCauAsync 1 lần. |
| UT-112 | GuiYeuCau — Service trả mã 500 | Fail | Ném LoiNghiepVu(Status=500), message="Lỗi mẫu"; không trả IActionResult. Gọi GuiYeuCauAsync 1 lần. |
| UT-113 | GuiYeuCau — Truyền đúng tham số và token | Fail | RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi GuiYeuCauAsync 1 lần. |
| UT-114 | GuiDeXuat — ModelState lỗi | Fail | ViewResult, view=mặc định của action; lỗi ModelState={"field": ["Lỗi mẫu"]}; Success=null. Gọi GuiDeXuatAsync 0 lần. |
| UT-115 | GuiDeXuat — Thành công | Fail | RedirectToActionResult tới Index; Success="Đã gửi đề xuất để bên học xác nhận.". Gọi GuiDeXuatAsync 1 lần. |
| UT-116 | GuiDeXuat — Service trả mã 403 | Fail | ViewResult, view=mặc định của action; lỗi ModelState={"": ["Lỗi mẫu"]}; Success=null. Gọi GuiDeXuatAsync 1 lần. |
| UT-117 | GuiDeXuat — Service trả mã 404 | Fail | ViewResult, view=mặc định của action; lỗi ModelState={"": ["Lỗi mẫu"]}; Success=null. Gọi GuiDeXuatAsync 1 lần. |
| UT-118 | GuiDeXuat — Service trả mã 409 | Fail | ViewResult, view=mặc định của action; lỗi ModelState={"": ["Lỗi mẫu"]}; Success=null. Gọi GuiDeXuatAsync 1 lần. |
| UT-119 | GuiDeXuat — Service trả mã 400 | Fail | ViewResult, view=mặc định của action; lỗi ModelState={"": ["Lỗi mẫu"]}; Success=null. Gọi GuiDeXuatAsync 1 lần. |
| UT-120 | GuiDeXuat — Service trả mã 500 | Fail | ViewResult, view=mặc định của action; lỗi ModelState={"": ["Lỗi mẫu"]}; Success=null. Gọi GuiDeXuatAsync 1 lần. |
| UT-121 | GuiDeXuat — Truyền đúng tham số và token | Pass | RedirectToActionResult tới Index; Success="Đã gửi đề xuất để bên học xác nhận.". Gọi GuiDeXuatAsync 1 lần. |
| UT-122 | ChapNhan — ModelState lỗi | Fail | RedirectResult.Url=/HocTap/LopHoc/ChiTiet/f5b9ee7d-c19c-4aa7-8682-235b54e4fa82; Success="Lớp đã được tạo. Bạn có thể tiếp tục mở hoặc đóng nhu cầu tại mục Nhu cầu.". Gọi ChapNhanDeXuatAsync 1 lần. |
| UT-123 | ChapNhan — Thành công | Pass | RedirectResult.Url=/HocTap/LopHoc/ChiTiet/ce3f1532-4568-410c-87a6-3633766d5769; Success="Lớp đã được tạo. Bạn có thể tiếp tục mở hoặc đóng nhu cầu tại mục Nhu cầu.". Gọi ChapNhanDeXuatAsync 1 lần. |
| UT-124 | ChapNhan — Service trả mã 403 | Fail | Ném LoiNghiepVu(Status=403), message="Lỗi mẫu"; không trả IActionResult. Gọi ChapNhanDeXuatAsync 1 lần. |
| UT-125 | ChapNhan — Service trả mã 404 | Fail | Ném LoiNghiepVu(Status=404), message="Lỗi mẫu"; không trả IActionResult. Gọi ChapNhanDeXuatAsync 1 lần. |
| UT-126 | ChapNhan — Service trả mã 409 | Fail | Ném LoiNghiepVu(Status=409), message="Lỗi mẫu"; không trả IActionResult. Gọi ChapNhanDeXuatAsync 1 lần. |
| UT-127 | ChapNhan — Service trả mã 400 | Fail | Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi ChapNhanDeXuatAsync 1 lần. |
| UT-128 | ChapNhan — Service trả mã 500 | Fail | Ném LoiNghiepVu(Status=500), message="Lỗi mẫu"; không trả IActionResult. Gọi ChapNhanDeXuatAsync 1 lần. |
| UT-129 | ChapNhan — Truyền đúng tham số và token | Pass | RedirectResult.Url=/HocTap/LopHoc/ChiTiet/b67b83f3-9b74-473a-8446-eb5d8cd5cad3; Success="Lớp đã được tạo. Bạn có thể tiếp tục mở hoặc đóng nhu cầu tại mục Nhu cầu.". Gọi ChapNhanDeXuatAsync 1 lần. |
| UT-130 | TuChoi — ModelState lỗi | Fail | RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi TuChoiDeXuatAsync 1 lần. |
| UT-131 | TuChoi — Thành công | Fail | RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi TuChoiDeXuatAsync 1 lần. |
| UT-132 | TuChoi — Service trả mã 403 | Fail | Ném LoiNghiepVu(Status=403), message="Lỗi mẫu"; không trả IActionResult. Gọi TuChoiDeXuatAsync 1 lần. |
| UT-133 | TuChoi — Service trả mã 404 | Fail | Ném LoiNghiepVu(Status=404), message="Lỗi mẫu"; không trả IActionResult. Gọi TuChoiDeXuatAsync 1 lần. |
| UT-134 | TuChoi — Service trả mã 409 | Fail | Ném LoiNghiepVu(Status=409), message="Lỗi mẫu"; không trả IActionResult. Gọi TuChoiDeXuatAsync 1 lần. |
| UT-135 | TuChoi — Service trả mã 400 | Fail | Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi TuChoiDeXuatAsync 1 lần. |
| UT-136 | TuChoi — Service trả mã 500 | Fail | Ném LoiNghiepVu(Status=500), message="Lỗi mẫu"; không trả IActionResult. Gọi TuChoiDeXuatAsync 1 lần. |
| UT-137 | TuChoi — Truyền đúng tham số và token | Pass | RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi TuChoiDeXuatAsync 1 lần. |
| UT-138 | Rut — ModelState lỗi | Fail | RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi RutYeuCauAsync 1 lần. |
| UT-139 | Rut — Thành công | Fail | RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi RutYeuCauAsync 1 lần. |
| UT-140 | Rut — Service trả mã 403 | Fail | Ném LoiNghiepVu(Status=403), message="Lỗi mẫu"; không trả IActionResult. Gọi RutYeuCauAsync 1 lần. |
| UT-141 | Rut — Service trả mã 404 | Fail | Ném LoiNghiepVu(Status=404), message="Lỗi mẫu"; không trả IActionResult. Gọi RutYeuCauAsync 1 lần. |
| UT-142 | Rut — Service trả mã 409 | Fail | Ném LoiNghiepVu(Status=409), message="Lỗi mẫu"; không trả IActionResult. Gọi RutYeuCauAsync 1 lần. |
| UT-143 | Rut — Service trả mã 400 | Fail | Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi RutYeuCauAsync 1 lần. |
| UT-144 | Rut — Service trả mã 500 | Fail | Ném LoiNghiepVu(Status=500), message="Lỗi mẫu"; không trả IActionResult. Gọi RutYeuCauAsync 1 lần. |
| UT-145 | Rut — Truyền đúng tham số và token | Pass | RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi RutYeuCauAsync 1 lần. |
| UT-146 | XacNhanKetThuc — ModelState lỗi | Fail | RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi XacNhanKetThucAsync 1 lần. |
| UT-147 | XacNhanKetThuc — Thành công | Fail | RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi XacNhanKetThucAsync 1 lần. |
| UT-148 | XacNhanKetThuc — Service trả mã 403 | Fail | Ném LoiNghiepVu(Status=403), message="Lỗi mẫu"; không trả IActionResult. Gọi XacNhanKetThucAsync 1 lần. |
| UT-149 | XacNhanKetThuc — Service trả mã 404 | Fail | Ném LoiNghiepVu(Status=404), message="Lỗi mẫu"; không trả IActionResult. Gọi XacNhanKetThucAsync 1 lần. |
| UT-150 | XacNhanKetThuc — Service trả mã 409 | Fail | Ném LoiNghiepVu(Status=409), message="Lỗi mẫu"; không trả IActionResult. Gọi XacNhanKetThucAsync 1 lần. |
| UT-151 | XacNhanKetThuc — Service trả mã 400 | Fail | Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi XacNhanKetThucAsync 1 lần. |
| UT-152 | XacNhanKetThuc — Service trả mã 500 | Fail | Ném LoiNghiepVu(Status=500), message="Lỗi mẫu"; không trả IActionResult. Gọi XacNhanKetThucAsync 1 lần. |
| UT-153 | XacNhanKetThuc — Truyền đúng tham số và token | Pass | RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi XacNhanKetThucAsync 1 lần. |
| UT-154 | HuyKetNoi — ModelState lỗi | Fail | RedirectResult.Url=/HocTap/LopHoc/ChiTiet/256a4c5a-69c7-409e-923c-2eee7cfd129c; Success="Đã gửi yêu cầu kết thúc lớp.". Gọi TaoYeuCauHuyAsync 1 lần. |
| UT-155 | HuyKetNoi — Thành công | Fail | RedirectResult.Url=/HocTap/LopHoc/ChiTiet/5cb4e17a-3693-45ad-92ca-045d8bcc0303; Success="Đã gửi yêu cầu kết thúc lớp.". Gọi TaoYeuCauHuyAsync 1 lần. |
| UT-156 | HuyKetNoi — Service trả mã 403 | Fail | Ném LoiNghiepVu(Status=403), message="Lỗi mẫu"; không trả IActionResult. Gọi TaoYeuCauHuyAsync 1 lần. |
| UT-157 | HuyKetNoi — Service trả mã 404 | Fail | Ném LoiNghiepVu(Status=404), message="Lỗi mẫu"; không trả IActionResult. Gọi TaoYeuCauHuyAsync 1 lần. |
| UT-158 | HuyKetNoi — Service trả mã 409 | Fail | Ném LoiNghiepVu(Status=409), message="Lỗi mẫu"; không trả IActionResult. Gọi TaoYeuCauHuyAsync 1 lần. |
| UT-159 | HuyKetNoi — Service trả mã 400 | Fail | Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi TaoYeuCauHuyAsync 1 lần. |
| UT-160 | HuyKetNoi — Service trả mã 500 | Fail | Ném LoiNghiepVu(Status=500), message="Lỗi mẫu"; không trả IActionResult. Gọi TaoYeuCauHuyAsync 1 lần. |
| UT-161 | HuyKetNoi — Truyền đúng tham số và token | Pass | RedirectResult.Url=/HocTap/LopHoc/ChiTiet/58ce9a2d-3f2b-416b-b7a4-43106785f6fe; Success="Đã gửi yêu cầu kết thúc lớp.". Gọi TaoYeuCauHuyAsync 1 lần. |
| UT-162 | PhanHoiHuy — ModelState lỗi | Fail | Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi PhanHoiHuyAsync 0 lần. |
| UT-163 | PhanHoiHuy — Thành công | Fail | RedirectResult.Url=/HocTap/LopHoc; Success="Đã lưu thay đổi.". Gọi PhanHoiHuyAsync 1 lần. |
| UT-164 | PhanHoiHuy — Service trả mã 403 | Fail | Ném LoiNghiepVu(Status=403), message="Lỗi mẫu"; không trả IActionResult. Gọi PhanHoiHuyAsync 1 lần. |
| UT-165 | PhanHoiHuy — Service trả mã 404 | Fail | Ném LoiNghiepVu(Status=404), message="Lỗi mẫu"; không trả IActionResult. Gọi PhanHoiHuyAsync 1 lần. |
| UT-166 | PhanHoiHuy — Service trả mã 409 | Fail | Ném LoiNghiepVu(Status=409), message="Lỗi mẫu"; không trả IActionResult. Gọi PhanHoiHuyAsync 1 lần. |
| UT-167 | PhanHoiHuy — Service trả mã 400 | Fail | Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi PhanHoiHuyAsync 1 lần. |
| UT-168 | PhanHoiHuy — Service trả mã 500 | Fail | Ném LoiNghiepVu(Status=500), message="Lỗi mẫu"; không trả IActionResult. Gọi PhanHoiHuyAsync 1 lần. |
| UT-169 | PhanHoiHuy — Truyền đúng tham số và token | Pass | RedirectResult.Url=/HocTap/LopHoc; Success="Đã lưu thay đổi.". Gọi PhanHoiHuyAsync 1 lần. |

## Chi tiết từng TC

### UT-106 — GuiYeuCau: ModelState lỗi

**Kết quả: Fail.** Xử lý ModelState không đúng hợp đồng controller

**Đơn vị thiết kế:** `KetNoiController.GuiYeuCau(m)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaNhuCau": "9e00fa30-9967-4543-801a-7ee7296e9be1",
  "MaDoiTuong": "3e8c0999-b542-416d-8bf8-b531119a2060",
  "LienHeId": "daa397bc-83be-417e-b4eb-86f144335bf9",
  "NhuCauId": "9e00fa30-9967-4543-801a-7ee7296e9be1",
  "LoiNhan": "Lời nhắn gốc"
}
```

Người hiện tại U1: `d0f4b4ea-254d-4071-bbf0-37606d829f47`. Trường actor giả mạo đặt trong request: `5ccc4180-a080-4bf0-990d-71e7fa0c6149`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- Thêm ModelState.AddModelError("field", "Lỗi mẫu").
- Service được cấu hình trả thành công để phát hiện việc controller vẫn gọi service khi đầu vào đang lỗi; kỳ vọng số lần gọi là 0.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** BadRequestObjectResult chứa lỗi ModelState; không gọi GuiYeuCauAsync; không thông báo thành công.

**Thực tế:** Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi GuiYeuCauAsync 0 lần.

**Lỗi ModelState quan sát được:** `{"field": ["Lỗi mẫu"]}`.

**Đối chiếu kỳ vọng:**

- Thiết kế yêu cầu BadRequestObjectResult chứa lỗi; thực tế exception thoát khỏi action.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:50; Controllers/BaseController.cs:9

**Nguyên nhân:** Action gọi ValidateInput(); hàm này ném LoiNghiepVu khi ModelState lỗi, thay vì action trả BadRequestObjectResult.

**Ảnh hưởng:** Nhánh từ chối dữ liệu của controller không đáp ứng thiết kế. Không trả đúng loại kết quả mà bên gọi/action contract yêu cầu.

**Yêu cầu sửa/đối chiếu:** Kiểm ModelState và trả BadRequest(ModelState) tại action trước lời gọi ghi nghiệp vụ; không gán Success.

**Lời gọi service bắt được trên mock:**

```json
[]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Controllers.BaseController.ValidateInput() in C:\Khóa luận TN\Educonnect\Controllers\BaseController.cs:line 9

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.GuiYeuCau(GuiYeuCauViewModel m) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 50

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 22

--- End of stack trace from previous location ---
```

### UT-107 — GuiYeuCau: Thành công

**Kết quả: Fail.** Phản hồi thành công khác thiết kế

**Đơn vị thiết kế:** `KetNoiController.GuiYeuCau(m)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaNhuCau": "429216cf-8eee-477a-8eac-76a3a76b1aca",
  "MaDoiTuong": "34c3ac3e-73a0-4976-ab33-b96a10733a3e",
  "LienHeId": "669c8e7a-8c6c-40d5-8a81-71fe3796b4bd",
  "NhuCauId": "429216cf-8eee-477a-8eac-76a3a76b1aca",
  "LoiNhan": "Lời nhắn gốc"
}
```

Người hiện tại U1: `315ce525-bbc3-4da3-9d02-47c7e961651d`. Trường actor giả mạo đặt trong request: `462a6103-f0be-4d5d-a427-0a625b2c33f8`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- GuiYeuCauAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** RedirectToActionResult.ActionName=Index; TempData["Success"]="Đã xử lý yêu cầu."; chỉ gọi service một lần.

**Thực tế:** RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi GuiYeuCauAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Mong đợi Success="Đã xử lý yêu cầu."; thực tế Success="Đã lưu thay đổi.".

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:54; Areas/KetNoi/Controllers/KetNoiController.cs:55; Controllers/BaseController.cs:11

**Nguyên nhân:** Action gọi Success() mặc định “Đã lưu thay đổi.” hoặc truyền một nội dung khác với TC.

**Ảnh hưởng:** Phản hồi sau xử lý thành công không khớp đích điều hướng/nội dung thông báo được thiết kế. Không phải bằng chứng service đã lưu dữ liệu sai.

**Yêu cầu sửa/đối chiếu:** Đồng bộ nội dung Success và kiểu/đích chuyển hướng theo từng kỳ vọng TC; giữ nguyên ID trả về ở ca chấp nhận.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "GuiYeuCauAsync",
    "arguments": [
      {
        "MaNhuCau": "429216cf-8eee-477a-8eac-76a3a76b1aca",
        "MaDoiTuong": "34c3ac3e-73a0-4976-ab33-b96a10733a3e",
        "LienHeId": "669c8e7a-8c6c-40d5-8a81-71fe3796b4bd",
        "NhuCauId": "429216cf-8eee-477a-8eac-76a3a76b1aca",
        "LoiNhan": "Lời nhắn gốc"
      },
      "315ce525-bbc3-4da3-9d02-47c7e961651d",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Equal() Failure: Values differ

Expected: Đã xử lý yêu cầu.

Actual:   Đã lưu thay đổi.
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 33

--- End of stack trace from previous location ---
```

### UT-108 — GuiYeuCau: Service trả mã 403

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.GuiYeuCau(m)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaNhuCau": "c5bcbca8-565a-4fc8-82b0-29576298a110",
  "MaDoiTuong": "a20ecfee-69f4-4967-bf88-caf65ca5dc83",
  "LienHeId": "32acb43a-08a8-4779-8742-ace67a49d191",
  "NhuCauId": "c5bcbca8-565a-4fc8-82b0-29576298a110",
  "LoiNhan": "Lời nhắn gốc"
}
```

Người hiện tại U1: `f728c567-df49-4f82-b0aa-f78942bc9823`. Trường actor giả mạo đặt trong request: `4f0e972b-c71d-4aea-9a46-e66a9c04dc81`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- GuiYeuCauAsync trả KetQua thất bại: MaLoi=403, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ForbidResult; ModelState có lỗi chung “Lỗi mẫu”; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=403), message="Lỗi mẫu"; không trả IActionResult. Gọi GuiYeuCauAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 403 nhưng action ném LoiNghiepVu thay vì trả ForbidResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:53

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả ForbidResult theo mã 403; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "GuiYeuCauAsync",
    "arguments": [
      {
        "MaNhuCau": "c5bcbca8-565a-4fc8-82b0-29576298a110",
        "MaDoiTuong": "a20ecfee-69f4-4967-bf88-caf65ca5dc83",
        "LienHeId": "32acb43a-08a8-4779-8742-ace67a49d191",
        "NhuCauId": "c5bcbca8-565a-4fc8-82b0-29576298a110",
        "LoiNhan": "Lời nhắn gốc"
      },
      "f728c567-df49-4f82-b0aa-f78942bc9823",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.GuiYeuCau(GuiYeuCauViewModel m) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 53

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-109 — GuiYeuCau: Service trả mã 404

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.GuiYeuCau(m)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaNhuCau": "925ff262-79d2-434c-840c-d7fd70a56abd",
  "MaDoiTuong": "9529cff4-02e1-4d46-b646-4fe58149344b",
  "LienHeId": "eb154117-78d5-45a6-a5ae-b2f64956d142",
  "NhuCauId": "925ff262-79d2-434c-840c-d7fd70a56abd",
  "LoiNhan": "Lời nhắn gốc"
}
```

Người hiện tại U1: `5f1350f0-24ea-4650-b718-28ea8d7b1dee`. Trường actor giả mạo đặt trong request: `c0504407-d75c-41a5-a946-a0106a5e9781`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- GuiYeuCauAsync trả KetQua thất bại: MaLoi=404, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả NotFoundObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=404), message="Lỗi mẫu"; không trả IActionResult. Gọi GuiYeuCauAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 404 nhưng action ném LoiNghiepVu thay vì trả NotFoundObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:53

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả NotFoundObjectResult theo mã 404; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "GuiYeuCauAsync",
    "arguments": [
      {
        "MaNhuCau": "925ff262-79d2-434c-840c-d7fd70a56abd",
        "MaDoiTuong": "9529cff4-02e1-4d46-b646-4fe58149344b",
        "LienHeId": "eb154117-78d5-45a6-a5ae-b2f64956d142",
        "NhuCauId": "925ff262-79d2-434c-840c-d7fd70a56abd",
        "LoiNhan": "Lời nhắn gốc"
      },
      "5f1350f0-24ea-4650-b718-28ea8d7b1dee",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.GuiYeuCau(GuiYeuCauViewModel m) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 53

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-110 — GuiYeuCau: Service trả mã 409

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.GuiYeuCau(m)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaNhuCau": "96c9bee5-2f23-47ea-9284-f3984bf62721",
  "MaDoiTuong": "918ddb5a-6f1b-467c-9310-09856138f559",
  "LienHeId": "37bd119e-2503-4ad9-8202-b733372e9e62",
  "NhuCauId": "96c9bee5-2f23-47ea-9284-f3984bf62721",
  "LoiNhan": "Lời nhắn gốc"
}
```

Người hiện tại U1: `287a4d6d-e59d-44a5-8abf-f87b99e6d9f8`. Trường actor giả mạo đặt trong request: `0b7376c2-6c50-463b-bd34-41865aa7b94b`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- GuiYeuCauAsync trả KetQua thất bại: MaLoi=409, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ConflictObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=409), message="Lỗi mẫu"; không trả IActionResult. Gọi GuiYeuCauAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 409 nhưng action ném LoiNghiepVu thay vì trả ConflictObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:53

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả ConflictObjectResult theo mã 409; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "GuiYeuCauAsync",
    "arguments": [
      {
        "MaNhuCau": "96c9bee5-2f23-47ea-9284-f3984bf62721",
        "MaDoiTuong": "918ddb5a-6f1b-467c-9310-09856138f559",
        "LienHeId": "37bd119e-2503-4ad9-8202-b733372e9e62",
        "NhuCauId": "96c9bee5-2f23-47ea-9284-f3984bf62721",
        "LoiNhan": "Lời nhắn gốc"
      },
      "287a4d6d-e59d-44a5-8abf-f87b99e6d9f8",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.GuiYeuCau(GuiYeuCauViewModel m) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 53

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-111 — GuiYeuCau: Service trả mã 400

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.GuiYeuCau(m)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaNhuCau": "04b2bb2f-ff79-41a5-87e4-2f433f9d7928",
  "MaDoiTuong": "52decd29-d688-4c2b-87f3-f6125225150c",
  "LienHeId": "19964649-d87c-4ce7-a9d1-299923b9b0f0",
  "NhuCauId": "04b2bb2f-ff79-41a5-87e4-2f433f9d7928",
  "LoiNhan": "Lời nhắn gốc"
}
```

Người hiện tại U1: `9b0639c9-4af9-47d7-acfe-149f3ec147be`. Trường actor giả mạo đặt trong request: `f0a1604a-f0bf-41ea-9065-347eb0856a02`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- GuiYeuCauAsync trả KetQua thất bại: MaLoi=400, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi GuiYeuCauAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 400 nhưng action ném LoiNghiepVu thay vì trả BadRequestObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:53

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả BadRequestObjectResult theo mã 400; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "GuiYeuCauAsync",
    "arguments": [
      {
        "MaNhuCau": "04b2bb2f-ff79-41a5-87e4-2f433f9d7928",
        "MaDoiTuong": "52decd29-d688-4c2b-87f3-f6125225150c",
        "LienHeId": "19964649-d87c-4ce7-a9d1-299923b9b0f0",
        "NhuCauId": "04b2bb2f-ff79-41a5-87e4-2f433f9d7928",
        "LoiNhan": "Lời nhắn gốc"
      },
      "9b0639c9-4af9-47d7-acfe-149f3ec147be",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.GuiYeuCau(GuiYeuCauViewModel m) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 53

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-112 — GuiYeuCau: Service trả mã 500

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.GuiYeuCau(m)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaNhuCau": "e2037b8b-1984-42c4-bab7-50b264fb041d",
  "MaDoiTuong": "e842180f-e955-461c-9eea-c99632e485d7",
  "LienHeId": "9e5f244b-a9f1-4e5a-85ab-8cc778f41f5f",
  "NhuCauId": "e2037b8b-1984-42c4-bab7-50b264fb041d",
  "LoiNhan": "Lời nhắn gốc"
}
```

Người hiện tại U1: `64f7b46b-b2be-4bb3-8a36-056e96cd1f3c`. Trường actor giả mạo đặt trong request: `a56b2459-236e-480b-be02-ce53efd078f6`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- GuiYeuCauAsync trả KetQua thất bại: MaLoi=500, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=500), message="Lỗi mẫu"; không trả IActionResult. Gọi GuiYeuCauAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 500 nhưng action ném LoiNghiepVu thay vì trả BadRequestObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:53

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả BadRequestObjectResult theo mã 500; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "GuiYeuCauAsync",
    "arguments": [
      {
        "MaNhuCau": "e2037b8b-1984-42c4-bab7-50b264fb041d",
        "MaDoiTuong": "e842180f-e955-461c-9eea-c99632e485d7",
        "LienHeId": "9e5f244b-a9f1-4e5a-85ab-8cc778f41f5f",
        "NhuCauId": "e2037b8b-1984-42c4-bab7-50b264fb041d",
        "LoiNhan": "Lời nhắn gốc"
      },
      "64f7b46b-b2be-4bb3-8a36-056e96cd1f3c",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.GuiYeuCau(GuiYeuCauViewModel m) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 53

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-113 — GuiYeuCau: Truyền đúng tham số và token

**Kết quả: Fail.** Thiếu trường phiên bản đầu vào theo TC

**Đơn vị thiết kế:** `KetNoiController.GuiYeuCau(m)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaNhuCau": "96cc7259-3c95-4c2f-ad47-cfcf5fee65cc",
  "MaDoiTuong": "acb12b74-1abf-4e57-8642-2032864717c5",
  "LienHeId": "f4ea640e-f272-4c43-809b-522ef72111fc",
  "NhuCauId": "96cc7259-3c95-4c2f-ad47-cfcf5fee65cc",
  "LoiNhan": "Lời nhắn gốc"
}
```

Người hiện tại U1: `8d595958-5fdf-4775-96c8-e05f2c427a71`. Trường actor giả mạo đặt trong request: `7eabc16a-9d3a-48c4-ac6d-8d31f3c5252c`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- GuiYeuCauAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Verify GuiYeuCauAsync nhận nguyên mã, RV1, các trường input, actor=U1 và ct đúng một lần; không dùng actor từ dữ liệu người dùng gửi.

**Thực tế:** RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi GuiYeuCauAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Các trường có thật trong GuiYeuCauViewModel, actor U1 và RequestAborted đã truyền đúng.
- TC còn yêu cầu RV1, nhưng GuiYeuCauViewModel không có thuộc tính RowVersion nên không thể truyền nguyên RV1 như bảng yêu cầu.

**Vị trí code:** Areas/KetNoi/ViewModels/KetNoiInput.cs:GuiYeuCauInput/GuiYeuCauViewModel; Areas/KetNoi/Controllers/KetNoiController.cs:51

**Nguyên nhân:** Kịch bản UT-113 yêu cầu RV1; hợp đồng DTO hiện tại chỉ có LienHeId/NhuCauId/LoiNhan/MaNhuCau/MaDoiTuong. Test trước không được bỏ qua phần RV1 để báo Pass.

**Ảnh hưởng:** Chưa đáp ứng đủ hợp đồng dữ liệu nêu trong TC-113. Đây là sai lệch giữa yêu cầu của bảng và DTO, không phải lỗi token/actor.

**Yêu cầu sửa/đối chiếu:** Đối chiếu DTO trong thiết kế gốc để giải quyết yêu cầu RV1. Nếu thiết kế có trường phiên bản thì triển khai đúng; nếu thiết kế không có, sửa chính phần RV1 của TC một cách có căn cứ, không âm thầm bỏ assertion.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "GuiYeuCauAsync",
    "arguments": [
      {
        "MaNhuCau": "96cc7259-3c95-4c2f-ad47-cfcf5fee65cc",
        "MaDoiTuong": "acb12b74-1abf-4e57-8642-2032864717c5",
        "LienHeId": "f4ea640e-f272-4c43-809b-522ef72111fc",
        "NhuCauId": "96cc7259-3c95-4c2f-ad47-cfcf5fee65cc",
        "LoiNhan": "Lời nhắn gốc"
      },
      "8d595958-5fdf-4775-96c8-e05f2c427a71",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
UT-113: bảng TC yêu cầu truyền RV1 nhưng GuiYeuCauViewModel hiện không khai báo RowVersion. Các trường hiện có, actor và token đã truyền đúng; chưa đáp ứng đầy đủ yêu cầu RV1 của TC.
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 57

--- End of stack trace from previous location ---
```

### UT-114 — GuiDeXuat: ModelState lỗi

**Kết quả: Fail.** Xử lý ModelState không đúng hợp đồng controller

**Đơn vị thiết kế:** `KetNoiController.GuiDeXuat(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "LienHeId": "403549cf-cef2-4274-bca2-669563f97ea1",
  "Id": "9c123c51-e8ca-473e-a94f-9cb92328f32d",
  "NhuCauId": "165ea32c-29fb-4ce4-81ca-698458d09023",
  "RowVersion": "AQIDBAUGBwg=",
  "TenLop": "Lớp kiểm thử",
  "MonHocId": 7,
  "MucTieu": "Mục tiêu gốc",
  "SoBuoiTuan": 2,
  "ThoiLuongPhut": 60,
  "BatDauHocLuc": "2030-01-02T18:00:00",
  "HinhThucHoc": "TrucTiep",
  "DiaDiem": "Địa điểm gốc",
  "HocPhi": 125000,
  "DonViHocPhi": "VND/Buoi",
  "LichRanh": [
    {
      "ThuTrongTuan": 1,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    },
    {
      "ThuTrongTuan": 3,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    }
  ]
}
```

Người hiện tại U1: `c6baa669-cfe6-409f-b986-7356f310f4b4`. Trường actor giả mạo đặt trong request: `f49d58ca-7e59-46f3-b0bb-999a1fd84cc7`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- Thêm ModelState.AddModelError("field", "Lỗi mẫu").
- Service được cấu hình trả thành công để phát hiện việc controller vẫn gọi service khi đầu vào đang lỗi; kỳ vọng số lần gọi là 0.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** BadRequestObjectResult chứa lỗi ModelState; không gọi GuiDeXuatAsync; không thông báo thành công.

**Thực tế:** ViewResult, view=mặc định của action; lỗi ModelState={"field": ["Lỗi mẫu"]}; Success=null. Gọi GuiDeXuatAsync 0 lần.

**Lỗi ModelState quan sát được:** `{"field": ["Lỗi mẫu"]}`.

**Đối chiếu kỳ vọng:**

- Giữ được lỗi ModelState và không ghi nghiệp vụ, nhưng trả ViewResult thay vì BadRequestObjectResult theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:87

**Nguyên nhân:** Nhánh ModelState không hợp lệ nạp dữ liệu biểu mẫu và return View(m), không return BadRequest(ModelState).

**Ảnh hưởng:** Nhánh từ chối dữ liệu của controller không đáp ứng thiết kế. Không trả đúng loại kết quả mà bên gọi/action contract yêu cầu.

**Yêu cầu sửa/đối chiếu:** Đồng bộ nhánh ModelState lỗi với BadRequestObjectResult như thiết kế; vẫn giữ lỗi và không gọi service ghi.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "LayAsync",
    "arguments": []
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.IsType() Failure: Value is not the exact type

Expected: typeof(Microsoft.AspNetCore.Mvc.BadRequestObjectResult)

Actual:   typeof(Microsoft.AspNetCore.Mvc.ViewResult)
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 22

--- End of stack trace from previous location ---
```

### UT-115 — GuiDeXuat: Thành công

**Kết quả: Fail.** Phản hồi thành công khác thiết kế

**Đơn vị thiết kế:** `KetNoiController.GuiDeXuat(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "LienHeId": "4a189645-bf01-496c-a0aa-1a4ab036af0b",
  "Id": "c9fc0e8b-3170-43ad-9210-ec1d12e94d35",
  "NhuCauId": "c94065db-770d-4d3e-bf3d-524c9ab380bb",
  "RowVersion": "AQIDBAUGBwg=",
  "TenLop": "Lớp kiểm thử",
  "MonHocId": 7,
  "MucTieu": "Mục tiêu gốc",
  "SoBuoiTuan": 2,
  "ThoiLuongPhut": 60,
  "BatDauHocLuc": "2030-01-02T18:00:00",
  "HinhThucHoc": "TrucTiep",
  "DiaDiem": "Địa điểm gốc",
  "HocPhi": 125000,
  "DonViHocPhi": "VND/Buoi",
  "LichRanh": [
    {
      "ThuTrongTuan": 1,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    },
    {
      "ThuTrongTuan": 3,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    }
  ]
}
```

Người hiện tại U1: `05d2e6cd-dd79-4f96-a790-e4e08266cd35`. Trường actor giả mạo đặt trong request: `1056468e-9c7f-4fa2-ba8f-8a5abb93f96c`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- GuiDeXuatAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** RedirectToActionResult.ActionName=Index; TempData["Success"]="Đã xử lý yêu cầu."; chỉ gọi service một lần.

**Thực tế:** RedirectToActionResult tới Index; Success="Đã gửi đề xuất để bên học xác nhận.". Gọi GuiDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Mong đợi Success="Đã xử lý yêu cầu."; thực tế Success="Đã gửi đề xuất để bên học xác nhận.".

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:82; Areas/KetNoi/Controllers/KetNoiController.cs:83

**Nguyên nhân:** Action gọi Success() mặc định “Đã lưu thay đổi.” hoặc truyền một nội dung khác với TC.

**Ảnh hưởng:** Phản hồi sau xử lý thành công không khớp đích điều hướng/nội dung thông báo được thiết kế. Không phải bằng chứng service đã lưu dữ liệu sai.

**Yêu cầu sửa/đối chiếu:** Đồng bộ nội dung Success và kiểu/đích chuyển hướng theo từng kỳ vọng TC; giữ nguyên ID trả về ở ca chấp nhận.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "GuiDeXuatAsync",
    "arguments": [
      {
        "LienHeId": "4a189645-bf01-496c-a0aa-1a4ab036af0b",
        "Id": "c9fc0e8b-3170-43ad-9210-ec1d12e94d35",
        "NhuCauId": "c94065db-770d-4d3e-bf3d-524c9ab380bb",
        "RowVersion": "AQIDBAUGBwg=",
        "TenLop": "Lớp kiểm thử",
        "MonHocId": 7,
        "MucTieu": "Mục tiêu gốc",
        "SoBuoiTuan": 2,
        "ThoiLuongPhut": 60,
        "BatDauHocLuc": "2030-01-02T18:00:00",
        "HinhThucHoc": "TrucTiep",
        "DiaDiem": "Địa điểm gốc",
        "HocPhi": 125000,
        "DonViHocPhi": "VND/Buoi",
        "LichRanh": [
          {
            "ThuTrongTuan": 1,
            "GioBatDau": "18:00:00",
            "GioKetThuc": "19:00:00"
          },
          {
            "ThuTrongTuan": 3,
            "GioBatDau": "18:00:00",
            "GioKetThuc": "19:00:00"
          }
        ]
      },
      "05d2e6cd-dd79-4f96-a790-e4e08266cd35",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Equal() Failure: Values differ

Expected: Đã xử lý yêu cầu.

Actual:   Đã gửi đề xuất để bên học xác nhận.
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 33

--- End of stack trace from previous location ---
```

### UT-116 — GuiDeXuat: Service trả mã 403

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.GuiDeXuat(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "LienHeId": "9f59e63d-4552-49ec-aade-022f5b6a501c",
  "Id": "2d4212c3-fadb-40fb-8395-3a785f72c7ba",
  "NhuCauId": "af5c1db3-eecf-4135-8e02-7b48a330fe1c",
  "RowVersion": "AQIDBAUGBwg=",
  "TenLop": "Lớp kiểm thử",
  "MonHocId": 7,
  "MucTieu": "Mục tiêu gốc",
  "SoBuoiTuan": 2,
  "ThoiLuongPhut": 60,
  "BatDauHocLuc": "2030-01-02T18:00:00",
  "HinhThucHoc": "TrucTiep",
  "DiaDiem": "Địa điểm gốc",
  "HocPhi": 125000,
  "DonViHocPhi": "VND/Buoi",
  "LichRanh": [
    {
      "ThuTrongTuan": 1,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    },
    {
      "ThuTrongTuan": 3,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    }
  ]
}
```

Người hiện tại U1: `3e6fba15-dc08-4a9c-a802-8797040ff4dc`. Trường actor giả mạo đặt trong request: `dbf800d2-29db-45b0-b9c0-01376aef19c4`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- GuiDeXuatAsync trả KetQua thất bại: MaLoi=403, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ForbidResult; ModelState có lỗi chung “Lỗi mẫu”; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** ViewResult, view=mặc định của action; lỗi ModelState={"": ["Lỗi mẫu"]}; Success=null. Gọi GuiDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{"": ["Lỗi mẫu"]}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 403; action trả ViewResult thay vì ForbidResult.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:85; Areas/KetNoi/Controllers/KetNoiController.cs:87

**Nguyên nhân:** GuiDeXuat bắt LoiNghiepVu rồi thêm ModelState, nhưng cuối cùng trả View(m), không ánh xạ mã lỗi sang loại IActionResult thiết kế.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Giữ lỗi chung đã thêm nhưng trả ForbidResult theo mã 403; với kết quả có body, trả đúng đối tượng KetQua lỗi.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "GuiDeXuatAsync",
    "arguments": [
      {
        "LienHeId": "9f59e63d-4552-49ec-aade-022f5b6a501c",
        "Id": "2d4212c3-fadb-40fb-8395-3a785f72c7ba",
        "NhuCauId": "af5c1db3-eecf-4135-8e02-7b48a330fe1c",
        "RowVersion": "AQIDBAUGBwg=",
        "TenLop": "Lớp kiểm thử",
        "MonHocId": 7,
        "MucTieu": "Mục tiêu gốc",
        "SoBuoiTuan": 2,
        "ThoiLuongPhut": 60,
        "BatDauHocLuc": "2030-01-02T18:00:00",
        "HinhThucHoc": "TrucTiep",
        "DiaDiem": "Địa điểm gốc",
        "HocPhi": 125000,
        "DonViHocPhi": "VND/Buoi",
        "LichRanh": [
          {
            "ThuTrongTuan": 1,
            "GioBatDau": "18:00:00",
            "GioKetThuc": "19:00:00"
          },
          {
            "ThuTrongTuan": 3,
            "GioBatDau": "18:00:00",
            "GioKetThuc": "19:00:00"
          }
        ]
      },
      "3e6fba15-dc08-4a9c-a802-8797040ff4dc",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  },
  {
    "method": "LayAsync",
    "arguments": []
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.IsType() Failure: Value is not the exact type

Expected: typeof(Microsoft.AspNetCore.Mvc.ForbidResult)

Actual:   typeof(Microsoft.AspNetCore.Mvc.ViewResult)
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 43

--- End of stack trace from previous location ---
```

### UT-117 — GuiDeXuat: Service trả mã 404

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.GuiDeXuat(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "LienHeId": "a576ff50-dfb1-496a-8f29-49b4d3cc041b",
  "Id": "0e014684-20a1-4bf3-a7a5-4dc945b4313e",
  "NhuCauId": "51d5b793-0da1-4a99-9d41-d90a23d1911e",
  "RowVersion": "AQIDBAUGBwg=",
  "TenLop": "Lớp kiểm thử",
  "MonHocId": 7,
  "MucTieu": "Mục tiêu gốc",
  "SoBuoiTuan": 2,
  "ThoiLuongPhut": 60,
  "BatDauHocLuc": "2030-01-02T18:00:00",
  "HinhThucHoc": "TrucTiep",
  "DiaDiem": "Địa điểm gốc",
  "HocPhi": 125000,
  "DonViHocPhi": "VND/Buoi",
  "LichRanh": [
    {
      "ThuTrongTuan": 1,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    },
    {
      "ThuTrongTuan": 3,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    }
  ]
}
```

Người hiện tại U1: `14a28752-8510-4c7d-8e75-bdf19052168b`. Trường actor giả mạo đặt trong request: `e498ab4e-f3e2-4e2c-bd09-b9678cd7cc3a`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- GuiDeXuatAsync trả KetQua thất bại: MaLoi=404, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả NotFoundObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** ViewResult, view=mặc định của action; lỗi ModelState={"": ["Lỗi mẫu"]}; Success=null. Gọi GuiDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{"": ["Lỗi mẫu"]}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 404; action trả ViewResult thay vì NotFoundObjectResult.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:85; Areas/KetNoi/Controllers/KetNoiController.cs:87

**Nguyên nhân:** GuiDeXuat bắt LoiNghiepVu rồi thêm ModelState, nhưng cuối cùng trả View(m), không ánh xạ mã lỗi sang loại IActionResult thiết kế.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Giữ lỗi chung đã thêm nhưng trả NotFoundObjectResult theo mã 404; với kết quả có body, trả đúng đối tượng KetQua lỗi.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "GuiDeXuatAsync",
    "arguments": [
      {
        "LienHeId": "a576ff50-dfb1-496a-8f29-49b4d3cc041b",
        "Id": "0e014684-20a1-4bf3-a7a5-4dc945b4313e",
        "NhuCauId": "51d5b793-0da1-4a99-9d41-d90a23d1911e",
        "RowVersion": "AQIDBAUGBwg=",
        "TenLop": "Lớp kiểm thử",
        "MonHocId": 7,
        "MucTieu": "Mục tiêu gốc",
        "SoBuoiTuan": 2,
        "ThoiLuongPhut": 60,
        "BatDauHocLuc": "2030-01-02T18:00:00",
        "HinhThucHoc": "TrucTiep",
        "DiaDiem": "Địa điểm gốc",
        "HocPhi": 125000,
        "DonViHocPhi": "VND/Buoi",
        "LichRanh": [
          {
            "ThuTrongTuan": 1,
            "GioBatDau": "18:00:00",
            "GioKetThuc": "19:00:00"
          },
          {
            "ThuTrongTuan": 3,
            "GioBatDau": "18:00:00",
            "GioKetThuc": "19:00:00"
          }
        ]
      },
      "14a28752-8510-4c7d-8e75-bdf19052168b",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  },
  {
    "method": "LayAsync",
    "arguments": []
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.IsType() Failure: Value is not the exact type

Expected: typeof(Microsoft.AspNetCore.Mvc.NotFoundObjectResult)

Actual:   typeof(Microsoft.AspNetCore.Mvc.ViewResult)
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 44

--- End of stack trace from previous location ---
```

### UT-118 — GuiDeXuat: Service trả mã 409

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.GuiDeXuat(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "LienHeId": "3dc40fbd-c822-41d1-a940-2fae12d6b093",
  "Id": "0fe4533e-b675-47fc-abdd-efe99a703b75",
  "NhuCauId": "f789991f-b8ba-4265-aa8b-173ce075a6f5",
  "RowVersion": "AQIDBAUGBwg=",
  "TenLop": "Lớp kiểm thử",
  "MonHocId": 7,
  "MucTieu": "Mục tiêu gốc",
  "SoBuoiTuan": 2,
  "ThoiLuongPhut": 60,
  "BatDauHocLuc": "2030-01-02T18:00:00",
  "HinhThucHoc": "TrucTiep",
  "DiaDiem": "Địa điểm gốc",
  "HocPhi": 125000,
  "DonViHocPhi": "VND/Buoi",
  "LichRanh": [
    {
      "ThuTrongTuan": 1,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    },
    {
      "ThuTrongTuan": 3,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    }
  ]
}
```

Người hiện tại U1: `1e90de4e-468d-4d4d-b9c9-b51085297c6e`. Trường actor giả mạo đặt trong request: `7c16b226-f931-41c9-9dbb-55fdb00ab23d`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- GuiDeXuatAsync trả KetQua thất bại: MaLoi=409, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ConflictObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** ViewResult, view=mặc định của action; lỗi ModelState={"": ["Lỗi mẫu"]}; Success=null. Gọi GuiDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{"": ["Lỗi mẫu"]}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 409; action trả ViewResult thay vì ConflictObjectResult.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:85; Areas/KetNoi/Controllers/KetNoiController.cs:87

**Nguyên nhân:** GuiDeXuat bắt LoiNghiepVu rồi thêm ModelState, nhưng cuối cùng trả View(m), không ánh xạ mã lỗi sang loại IActionResult thiết kế.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Giữ lỗi chung đã thêm nhưng trả ConflictObjectResult theo mã 409; với kết quả có body, trả đúng đối tượng KetQua lỗi.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "GuiDeXuatAsync",
    "arguments": [
      {
        "LienHeId": "3dc40fbd-c822-41d1-a940-2fae12d6b093",
        "Id": "0fe4533e-b675-47fc-abdd-efe99a703b75",
        "NhuCauId": "f789991f-b8ba-4265-aa8b-173ce075a6f5",
        "RowVersion": "AQIDBAUGBwg=",
        "TenLop": "Lớp kiểm thử",
        "MonHocId": 7,
        "MucTieu": "Mục tiêu gốc",
        "SoBuoiTuan": 2,
        "ThoiLuongPhut": 60,
        "BatDauHocLuc": "2030-01-02T18:00:00",
        "HinhThucHoc": "TrucTiep",
        "DiaDiem": "Địa điểm gốc",
        "HocPhi": 125000,
        "DonViHocPhi": "VND/Buoi",
        "LichRanh": [
          {
            "ThuTrongTuan": 1,
            "GioBatDau": "18:00:00",
            "GioKetThuc": "19:00:00"
          },
          {
            "ThuTrongTuan": 3,
            "GioBatDau": "18:00:00",
            "GioKetThuc": "19:00:00"
          }
        ]
      },
      "1e90de4e-468d-4d4d-b9c9-b51085297c6e",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  },
  {
    "method": "LayAsync",
    "arguments": []
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.IsType() Failure: Value is not the exact type

Expected: typeof(Microsoft.AspNetCore.Mvc.ConflictObjectResult)

Actual:   typeof(Microsoft.AspNetCore.Mvc.ViewResult)
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 45

--- End of stack trace from previous location ---
```

### UT-119 — GuiDeXuat: Service trả mã 400

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.GuiDeXuat(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "LienHeId": "d8a04292-6031-4d4e-9b0f-84785315139d",
  "Id": "a44d17ed-0518-473d-a580-48a40af5616d",
  "NhuCauId": "de4b87b4-7f55-4bfd-aa8b-10a26e8f0041",
  "RowVersion": "AQIDBAUGBwg=",
  "TenLop": "Lớp kiểm thử",
  "MonHocId": 7,
  "MucTieu": "Mục tiêu gốc",
  "SoBuoiTuan": 2,
  "ThoiLuongPhut": 60,
  "BatDauHocLuc": "2030-01-02T18:00:00",
  "HinhThucHoc": "TrucTiep",
  "DiaDiem": "Địa điểm gốc",
  "HocPhi": 125000,
  "DonViHocPhi": "VND/Buoi",
  "LichRanh": [
    {
      "ThuTrongTuan": 1,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    },
    {
      "ThuTrongTuan": 3,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    }
  ]
}
```

Người hiện tại U1: `05c5c8da-6a77-455a-944d-c05f9887b966`. Trường actor giả mạo đặt trong request: `e92e3219-f6d2-4c66-924d-2403c37f39ae`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- GuiDeXuatAsync trả KetQua thất bại: MaLoi=400, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** ViewResult, view=mặc định của action; lỗi ModelState={"": ["Lỗi mẫu"]}; Success=null. Gọi GuiDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{"": ["Lỗi mẫu"]}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 400; action trả ViewResult thay vì BadRequestObjectResult.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:85; Areas/KetNoi/Controllers/KetNoiController.cs:87

**Nguyên nhân:** GuiDeXuat bắt LoiNghiepVu rồi thêm ModelState, nhưng cuối cùng trả View(m), không ánh xạ mã lỗi sang loại IActionResult thiết kế.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Giữ lỗi chung đã thêm nhưng trả BadRequestObjectResult theo mã 400; với kết quả có body, trả đúng đối tượng KetQua lỗi.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "GuiDeXuatAsync",
    "arguments": [
      {
        "LienHeId": "d8a04292-6031-4d4e-9b0f-84785315139d",
        "Id": "a44d17ed-0518-473d-a580-48a40af5616d",
        "NhuCauId": "de4b87b4-7f55-4bfd-aa8b-10a26e8f0041",
        "RowVersion": "AQIDBAUGBwg=",
        "TenLop": "Lớp kiểm thử",
        "MonHocId": 7,
        "MucTieu": "Mục tiêu gốc",
        "SoBuoiTuan": 2,
        "ThoiLuongPhut": 60,
        "BatDauHocLuc": "2030-01-02T18:00:00",
        "HinhThucHoc": "TrucTiep",
        "DiaDiem": "Địa điểm gốc",
        "HocPhi": 125000,
        "DonViHocPhi": "VND/Buoi",
        "LichRanh": [
          {
            "ThuTrongTuan": 1,
            "GioBatDau": "18:00:00",
            "GioKetThuc": "19:00:00"
          },
          {
            "ThuTrongTuan": 3,
            "GioBatDau": "18:00:00",
            "GioKetThuc": "19:00:00"
          }
        ]
      },
      "05c5c8da-6a77-455a-944d-c05f9887b966",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  },
  {
    "method": "LayAsync",
    "arguments": []
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.IsType() Failure: Value is not the exact type

Expected: typeof(Microsoft.AspNetCore.Mvc.BadRequestObjectResult)

Actual:   typeof(Microsoft.AspNetCore.Mvc.ViewResult)
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 46

--- End of stack trace from previous location ---
```

### UT-120 — GuiDeXuat: Service trả mã 500

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.GuiDeXuat(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "LienHeId": "8881c9ca-12f4-47b4-96a8-96737d7fa534",
  "Id": "43cce84b-dc6d-412d-a6f6-09fc1b2a2836",
  "NhuCauId": "84d73447-2270-42aa-a6d5-27a3cf64bf34",
  "RowVersion": "AQIDBAUGBwg=",
  "TenLop": "Lớp kiểm thử",
  "MonHocId": 7,
  "MucTieu": "Mục tiêu gốc",
  "SoBuoiTuan": 2,
  "ThoiLuongPhut": 60,
  "BatDauHocLuc": "2030-01-02T18:00:00",
  "HinhThucHoc": "TrucTiep",
  "DiaDiem": "Địa điểm gốc",
  "HocPhi": 125000,
  "DonViHocPhi": "VND/Buoi",
  "LichRanh": [
    {
      "ThuTrongTuan": 1,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    },
    {
      "ThuTrongTuan": 3,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    }
  ]
}
```

Người hiện tại U1: `0357a8a9-3088-478f-b337-f61137c91922`. Trường actor giả mạo đặt trong request: `53e6678f-9a20-48eb-a1c8-bb05b458d9b2`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- GuiDeXuatAsync trả KetQua thất bại: MaLoi=500, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** ViewResult, view=mặc định của action; lỗi ModelState={"": ["Lỗi mẫu"]}; Success=null. Gọi GuiDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{"": ["Lỗi mẫu"]}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 500; action trả ViewResult thay vì BadRequestObjectResult.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:85; Areas/KetNoi/Controllers/KetNoiController.cs:87

**Nguyên nhân:** GuiDeXuat bắt LoiNghiepVu rồi thêm ModelState, nhưng cuối cùng trả View(m), không ánh xạ mã lỗi sang loại IActionResult thiết kế.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Giữ lỗi chung đã thêm nhưng trả BadRequestObjectResult theo mã 500; với kết quả có body, trả đúng đối tượng KetQua lỗi.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "GuiDeXuatAsync",
    "arguments": [
      {
        "LienHeId": "8881c9ca-12f4-47b4-96a8-96737d7fa534",
        "Id": "43cce84b-dc6d-412d-a6f6-09fc1b2a2836",
        "NhuCauId": "84d73447-2270-42aa-a6d5-27a3cf64bf34",
        "RowVersion": "AQIDBAUGBwg=",
        "TenLop": "Lớp kiểm thử",
        "MonHocId": 7,
        "MucTieu": "Mục tiêu gốc",
        "SoBuoiTuan": 2,
        "ThoiLuongPhut": 60,
        "BatDauHocLuc": "2030-01-02T18:00:00",
        "HinhThucHoc": "TrucTiep",
        "DiaDiem": "Địa điểm gốc",
        "HocPhi": 125000,
        "DonViHocPhi": "VND/Buoi",
        "LichRanh": [
          {
            "ThuTrongTuan": 1,
            "GioBatDau": "18:00:00",
            "GioKetThuc": "19:00:00"
          },
          {
            "ThuTrongTuan": 3,
            "GioBatDau": "18:00:00",
            "GioKetThuc": "19:00:00"
          }
        ]
      },
      "0357a8a9-3088-478f-b337-f61137c91922",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  },
  {
    "method": "LayAsync",
    "arguments": []
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.IsType() Failure: Value is not the exact type

Expected: typeof(Microsoft.AspNetCore.Mvc.BadRequestObjectResult)

Actual:   typeof(Microsoft.AspNetCore.Mvc.ViewResult)
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 46

--- End of stack trace from previous location ---
```

### UT-121 — GuiDeXuat: Truyền đúng tham số và token

**Kết quả: Pass.** Đạt truyền dữ liệu/người thực hiện/token

**Đơn vị thiết kế:** `KetNoiController.GuiDeXuat(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "LienHeId": "1e8faf7c-00e3-4450-a8a2-926160e98b7e",
  "Id": "05eee346-6704-462a-8891-1e88f8871e1a",
  "NhuCauId": "90b6da65-a9f9-4966-a7da-725e979c2787",
  "RowVersion": "AQIDBAUGBwg=",
  "TenLop": "Lớp kiểm thử",
  "MonHocId": 7,
  "MucTieu": "Mục tiêu gốc",
  "SoBuoiTuan": 2,
  "ThoiLuongPhut": 60,
  "BatDauHocLuc": "2030-01-02T18:00:00",
  "HinhThucHoc": "TrucTiep",
  "DiaDiem": "Địa điểm gốc",
  "HocPhi": 125000,
  "DonViHocPhi": "VND/Buoi",
  "LichRanh": [
    {
      "ThuTrongTuan": 1,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    },
    {
      "ThuTrongTuan": 3,
      "GioBatDau": "18:00:00",
      "GioKetThuc": "19:00:00"
    }
  ]
}
```

Người hiện tại U1: `9abb03ef-388d-43dc-b3fd-73a9da486692`. Trường actor giả mạo đặt trong request: `fc9247a4-9281-471c-a8ad-1fc5c4cc7ff7`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- GuiDeXuatAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Verify GuiDeXuatAsync nhận nguyên mã, RV1, các trường input, actor=U1 và ct đúng một lần; không dùng actor từ dữ liệu người dùng gửi.

**Thực tế:** RedirectToActionResult tới Index; Success="Đã gửi đề xuất để bên học xác nhận.". Gọi GuiDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Các assertion của ca này đạt; không lấy kết quả của ca này để suy ra các nhánh lỗi/thành công khác đều đúng.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:79

**Nguyên nhân:** Giữ nguyên input; service nhận đúng các ID, RV1/lý do/đồng ý có trong chữ ký, actor từ nguồn hiện tại và token RequestAborted, đúng một lần.

**Yêu cầu sửa/đối chiếu:** Không có sai lệch trong phạm vi ca truyền tham số.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "GuiDeXuatAsync",
    "arguments": [
      {
        "LienHeId": "1e8faf7c-00e3-4450-a8a2-926160e98b7e",
        "Id": "05eee346-6704-462a-8891-1e88f8871e1a",
        "NhuCauId": "90b6da65-a9f9-4966-a7da-725e979c2787",
        "RowVersion": "AQIDBAUGBwg=",
        "TenLop": "Lớp kiểm thử",
        "MonHocId": 7,
        "MucTieu": "Mục tiêu gốc",
        "SoBuoiTuan": 2,
        "ThoiLuongPhut": 60,
        "BatDauHocLuc": "2030-01-02T18:00:00",
        "HinhThucHoc": "TrucTiep",
        "DiaDiem": "Địa điểm gốc",
        "HocPhi": 125000,
        "DonViHocPhi": "VND/Buoi",
        "LichRanh": [
          {
            "ThuTrongTuan": 1,
            "GioBatDau": "18:00:00",
            "GioKetThuc": "19:00:00"
          },
          {
            "ThuTrongTuan": 3,
            "GioBatDau": "18:00:00",
            "GioKetThuc": "19:00:00"
          }
        ]
      },
      "9abb03ef-388d-43dc-b3fd-73a9da486692",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

### UT-122 — ChapNhan: ModelState lỗi

**Kết quả: Fail.** Xử lý ModelState không đúng hợp đồng controller

**Đơn vị thiết kế:** `KetNoiController.ChapNhan(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCau": "9824d5b4-5a76-4d42-86be-d9cec3fa425a",
  "Id": "9824d5b4-5a76-4d42-86be-d9cec3fa425a",
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `b29fb3b6-6b72-415e-aa7f-f4f6356cabda`. Trường actor giả mạo đặt trong request: `0db9e0fc-ea63-4671-9256-bfe5a543e079`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- Thêm ModelState.AddModelError("field", "Lỗi mẫu").
- Service được cấu hình trả thành công để phát hiện việc controller vẫn gọi service khi đầu vào đang lỗi; kỳ vọng số lần gọi là 0.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** BadRequestObjectResult chứa lỗi ModelState; không gọi ChapNhanDeXuatAsync; không thông báo thành công.

**Thực tế:** RedirectResult.Url=/HocTap/LopHoc/ChiTiet/f5b9ee7d-c19c-4aa7-8682-235b54e4fa82; Success="Lớp đã được tạo. Bạn có thể tiếp tục mở hoặc đóng nhu cầu tại mục Nhu cầu.". Gọi ChapNhanDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{"field": ["Lỗi mẫu"]}`.

**Đối chiếu kỳ vọng:**

- Thiết kế yêu cầu không gọi service, thực tế gọi ChapNhanDeXuatAsync 1 lần.
- ModelState đang lỗi nhưng vẫn gán thông báo thành công "Lớp đã được tạo. Bạn có thể tiếp tục mở hoặc đóng nhu cầu tại mục Nhu cầu.".
- Thiết kế yêu cầu BadRequestObjectResult; thực tế trả RedirectResult.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:98

**Nguyên nhân:** Action chưa chặn ModelState không hợp lệ trước khi gọi service. Mock service trả thành công nên code tiếp tục gán Success và chuyển hướng.

**Ảnh hưởng:** Nhánh từ chối dữ liệu của controller không đáp ứng thiết kế. Controller đã dispatch thao tác ghi dù ModelState lỗi; unit không thực hiện thao tác ghi thật.

**Yêu cầu sửa/đối chiếu:** Thêm guard ModelState đầu action, trả BadRequest(ModelState); service phải có số lần gọi bằng 0 khi dữ liệu đầu vào lỗi.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "ChapNhanDeXuatAsync",
    "arguments": [
      "9824d5b4-5a76-4d42-86be-d9cec3fa425a",
      "AQIDBAUGBwg=",
      "b29fb3b6-6b72-415e-aa7f-f4f6356cabda",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.False() Failure

Expected: False

Actual:   True
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 20

--- End of stack trace from previous location ---
```

### UT-123 — ChapNhan: Thành công

**Kết quả: Pass.** Đạt chuyển hướng thành công

**Đơn vị thiết kế:** `KetNoiController.ChapNhan(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCau": "f1ec705e-64a6-46bf-b663-5c4336e50054",
  "Id": "f1ec705e-64a6-46bf-b663-5c4336e50054",
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `6cdb03f3-e108-4662-bf0b-cd709e56403b`. Trường actor giả mạo đặt trong request: `bdebb79c-0143-4a93-9672-b8269cb81e8a`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- ChapNhanDeXuatAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** RedirectResult.Url=/HocTap/LopHoc/ChiTiet/C1 (thay C1 bằng Guid thật).

**Thực tế:** RedirectResult.Url=/HocTap/LopHoc/ChiTiet/ce3f1532-4568-410c-87a6-3633766d5769; Success="Lớp đã được tạo. Bạn có thể tiếp tục mở hoặc đóng nhu cầu tại mục Nhu cầu.". Gọi ChapNhanDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Các assertion của ca này đạt; không lấy kết quả của ca này để suy ra các nhánh lỗi/thành công khác đều đúng.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:103

**Nguyên nhân:** Đúng RedirectResult.Url có C1 do service trả về.

**Yêu cầu sửa/đối chiếu:** Không có sai lệch trong kỳ vọng của ca này.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "ChapNhanDeXuatAsync",
    "arguments": [
      "f1ec705e-64a6-46bf-b663-5c4336e50054",
      "AQIDBAUGBwg=",
      "6cdb03f3-e108-4662-bf0b-cd709e56403b",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

### UT-124 — ChapNhan: Service trả mã 403

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.ChapNhan(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCau": "f6a583cb-225c-4bca-9459-693d5e33cbd1",
  "Id": "f6a583cb-225c-4bca-9459-693d5e33cbd1",
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `5ef648c2-be9c-429a-87ab-29fd3e4feb27`. Trường actor giả mạo đặt trong request: `b4824bf9-3c7c-475d-822e-5adc492d8a3c`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- ChapNhanDeXuatAsync trả KetQua thất bại: MaLoi=403, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ForbidResult; ModelState có lỗi chung “Lỗi mẫu”; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=403), message="Lỗi mẫu"; không trả IActionResult. Gọi ChapNhanDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 403 nhưng action ném LoiNghiepVu thay vì trả ForbidResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:100

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả ForbidResult theo mã 403; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "ChapNhanDeXuatAsync",
    "arguments": [
      "f6a583cb-225c-4bca-9459-693d5e33cbd1",
      "AQIDBAUGBwg=",
      "5ef648c2-be9c-429a-87ab-29fd3e4feb27",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.ChapNhan(ChapNhanDeXuatViewModel vm) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 100

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-125 — ChapNhan: Service trả mã 404

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.ChapNhan(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCau": "fc5e5ddd-93c4-4ba2-94c8-c4ecb0b2487d",
  "Id": "fc5e5ddd-93c4-4ba2-94c8-c4ecb0b2487d",
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `01dbd742-c651-4a45-9d99-e6b62a890e2b`. Trường actor giả mạo đặt trong request: `8866812d-1598-4091-85d3-fca726aa50c7`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- ChapNhanDeXuatAsync trả KetQua thất bại: MaLoi=404, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả NotFoundObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=404), message="Lỗi mẫu"; không trả IActionResult. Gọi ChapNhanDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 404 nhưng action ném LoiNghiepVu thay vì trả NotFoundObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:100

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả NotFoundObjectResult theo mã 404; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "ChapNhanDeXuatAsync",
    "arguments": [
      "fc5e5ddd-93c4-4ba2-94c8-c4ecb0b2487d",
      "AQIDBAUGBwg=",
      "01dbd742-c651-4a45-9d99-e6b62a890e2b",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.ChapNhan(ChapNhanDeXuatViewModel vm) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 100

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-126 — ChapNhan: Service trả mã 409

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.ChapNhan(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCau": "eeac674d-db08-4e13-aeca-4082ed701290",
  "Id": "eeac674d-db08-4e13-aeca-4082ed701290",
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `c78075e8-ecbe-4c9e-814d-66601b62aec4`. Trường actor giả mạo đặt trong request: `cab71496-74ca-4058-9522-e607574ace2f`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- ChapNhanDeXuatAsync trả KetQua thất bại: MaLoi=409, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ConflictObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=409), message="Lỗi mẫu"; không trả IActionResult. Gọi ChapNhanDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 409 nhưng action ném LoiNghiepVu thay vì trả ConflictObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:100

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả ConflictObjectResult theo mã 409; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "ChapNhanDeXuatAsync",
    "arguments": [
      "eeac674d-db08-4e13-aeca-4082ed701290",
      "AQIDBAUGBwg=",
      "c78075e8-ecbe-4c9e-814d-66601b62aec4",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.ChapNhan(ChapNhanDeXuatViewModel vm) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 100

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-127 — ChapNhan: Service trả mã 400

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.ChapNhan(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCau": "909c93f4-612b-40f6-bf0a-8fe76523fb9c",
  "Id": "909c93f4-612b-40f6-bf0a-8fe76523fb9c",
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `3f95968d-24af-4f73-b878-e59248561ad3`. Trường actor giả mạo đặt trong request: `b4ac4087-6574-4be2-aee8-fd68602d6285`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- ChapNhanDeXuatAsync trả KetQua thất bại: MaLoi=400, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi ChapNhanDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 400 nhưng action ném LoiNghiepVu thay vì trả BadRequestObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:100

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả BadRequestObjectResult theo mã 400; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "ChapNhanDeXuatAsync",
    "arguments": [
      "909c93f4-612b-40f6-bf0a-8fe76523fb9c",
      "AQIDBAUGBwg=",
      "3f95968d-24af-4f73-b878-e59248561ad3",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.ChapNhan(ChapNhanDeXuatViewModel vm) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 100

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-128 — ChapNhan: Service trả mã 500

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.ChapNhan(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCau": "a80c119e-1a60-4aca-9954-4354bdb011f1",
  "Id": "a80c119e-1a60-4aca-9954-4354bdb011f1",
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `671c71a4-fd38-4bb0-a6d8-411615668364`. Trường actor giả mạo đặt trong request: `09ac51ac-ea8d-4904-b7dd-026dff4da4a4`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- ChapNhanDeXuatAsync trả KetQua thất bại: MaLoi=500, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=500), message="Lỗi mẫu"; không trả IActionResult. Gọi ChapNhanDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 500 nhưng action ném LoiNghiepVu thay vì trả BadRequestObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:100

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả BadRequestObjectResult theo mã 500; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "ChapNhanDeXuatAsync",
    "arguments": [
      "a80c119e-1a60-4aca-9954-4354bdb011f1",
      "AQIDBAUGBwg=",
      "671c71a4-fd38-4bb0-a6d8-411615668364",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.ChapNhan(ChapNhanDeXuatViewModel vm) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 100

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-129 — ChapNhan: Truyền đúng tham số và token

**Kết quả: Pass.** Đạt truyền dữ liệu/người thực hiện/token

**Đơn vị thiết kế:** `KetNoiController.ChapNhan(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCau": "e1288c73-55dd-467b-bd9d-4a308db3f0b6",
  "Id": "e1288c73-55dd-467b-bd9d-4a308db3f0b6",
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `d326cdd8-84ad-475e-8d9f-9ef05c30001a`. Trường actor giả mạo đặt trong request: `39d6fb6e-12cb-47a4-947d-0fecec93586e`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- ChapNhanDeXuatAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Verify ChapNhanDeXuatAsync nhận nguyên mã, RV1, các trường input, actor=U1 và ct đúng một lần; không dùng actor từ dữ liệu người dùng gửi.

**Thực tế:** RedirectResult.Url=/HocTap/LopHoc/ChiTiet/b67b83f3-9b74-473a-8446-eb5d8cd5cad3; Success="Lớp đã được tạo. Bạn có thể tiếp tục mở hoặc đóng nhu cầu tại mục Nhu cầu.". Gọi ChapNhanDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Các assertion của ca này đạt; không lấy kết quả của ca này để suy ra các nhánh lỗi/thành công khác đều đúng.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:98

**Nguyên nhân:** Giữ nguyên input; service nhận đúng các ID, RV1/lý do/đồng ý có trong chữ ký, actor từ nguồn hiện tại và token RequestAborted, đúng một lần.

**Yêu cầu sửa/đối chiếu:** Không có sai lệch trong phạm vi ca truyền tham số.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "ChapNhanDeXuatAsync",
    "arguments": [
      "e1288c73-55dd-467b-bd9d-4a308db3f0b6",
      "AQIDBAUGBwg=",
      "d326cdd8-84ad-475e-8d9f-9ef05c30001a",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

### UT-130 — TuChoi: ModelState lỗi

**Kết quả: Fail.** Xử lý ModelState không đúng hợp đồng controller

**Đơn vị thiết kế:** `KetNoiController.TuChoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "01a6d571-76fd-4f62-889c-3c73ee12f564",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `cecdfc33-3436-471f-88a4-22d9fd7629f8`. Trường actor giả mạo đặt trong request: `74183dff-b6b2-48c7-80bf-4bbc11b144a5`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- Thêm ModelState.AddModelError("field", "Lỗi mẫu").
- Service được cấu hình trả thành công để phát hiện việc controller vẫn gọi service khi đầu vào đang lỗi; kỳ vọng số lần gọi là 0.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** BadRequestObjectResult chứa lỗi ModelState; không gọi TuChoiDeXuatAsync; không thông báo thành công.

**Thực tế:** RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi TuChoiDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{"field": ["Lỗi mẫu"]}`.

**Đối chiếu kỳ vọng:**

- Thiết kế yêu cầu không gọi service, thực tế gọi TuChoiDeXuatAsync 1 lần.
- ModelState đang lỗi nhưng vẫn gán thông báo thành công "Đã lưu thay đổi.".
- Thiết kế yêu cầu BadRequestObjectResult; thực tế trả RedirectToActionResult.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:115

**Nguyên nhân:** Action chưa chặn ModelState không hợp lệ trước khi gọi service. Mock service trả thành công nên code tiếp tục gán Success và chuyển hướng.

**Ảnh hưởng:** Nhánh từ chối dữ liệu của controller không đáp ứng thiết kế. Controller đã dispatch thao tác ghi dù ModelState lỗi; unit không thực hiện thao tác ghi thật.

**Yêu cầu sửa/đối chiếu:** Thêm guard ModelState đầu action, trả BadRequest(ModelState); service phải có số lần gọi bằng 0 khi dữ liệu đầu vào lỗi.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TuChoiDeXuatAsync",
    "arguments": [
      "01a6d571-76fd-4f62-889c-3c73ee12f564",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "cecdfc33-3436-471f-88a4-22d9fd7629f8",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.False() Failure

Expected: False

Actual:   True
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 20

--- End of stack trace from previous location ---
```

### UT-131 — TuChoi: Thành công

**Kết quả: Fail.** Phản hồi thành công khác thiết kế

**Đơn vị thiết kế:** `KetNoiController.TuChoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "932037f9-b055-4c06-a55f-a042cf933500",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `70bb0430-ea21-4046-bddf-ec6c65ffbe4a`. Trường actor giả mạo đặt trong request: `39eb6b5e-4296-4c18-b438-456cb75d04b8`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- TuChoiDeXuatAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** RedirectToActionResult.ActionName=Index; TempData["Success"]="Đã xử lý yêu cầu."; chỉ gọi service một lần.

**Thực tế:** RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi TuChoiDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Mong đợi Success="Đã xử lý yêu cầu."; thực tế Success="Đã lưu thay đổi.".

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:116; Areas/KetNoi/Controllers/KetNoiController.cs:117; Controllers/BaseController.cs:11

**Nguyên nhân:** Action gọi Success() mặc định “Đã lưu thay đổi.” hoặc truyền một nội dung khác với TC.

**Ảnh hưởng:** Phản hồi sau xử lý thành công không khớp đích điều hướng/nội dung thông báo được thiết kế. Không phải bằng chứng service đã lưu dữ liệu sai.

**Yêu cầu sửa/đối chiếu:** Đồng bộ nội dung Success và kiểu/đích chuyển hướng theo từng kỳ vọng TC; giữ nguyên ID trả về ở ca chấp nhận.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TuChoiDeXuatAsync",
    "arguments": [
      "932037f9-b055-4c06-a55f-a042cf933500",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "70bb0430-ea21-4046-bddf-ec6c65ffbe4a",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Equal() Failure: Values differ

Expected: Đã xử lý yêu cầu.

Actual:   Đã lưu thay đổi.
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 33

--- End of stack trace from previous location ---
```

### UT-132 — TuChoi: Service trả mã 403

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.TuChoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "2f2639a7-c0cb-432f-800c-9050ba16b5ec",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `7b4a1ecd-ed74-4810-88e2-4b7ca73a32b3`. Trường actor giả mạo đặt trong request: `d37613e0-f7f9-4ae2-a634-730ed47ca59d`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- TuChoiDeXuatAsync trả KetQua thất bại: MaLoi=403, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ForbidResult; ModelState có lỗi chung “Lỗi mẫu”; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=403), message="Lỗi mẫu"; không trả IActionResult. Gọi TuChoiDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 403 nhưng action ném LoiNghiepVu thay vì trả ForbidResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:115; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả ForbidResult theo mã 403; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TuChoiDeXuatAsync",
    "arguments": [
      "2f2639a7-c0cb-432f-800c-9050ba16b5ec",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "7b4a1ecd-ed74-4810-88e2-4b7ca73a32b3",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.TuChoi(Guid maYeuCau, String lyDo, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 115

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-133 — TuChoi: Service trả mã 404

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.TuChoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "626b5524-efb6-4a2c-9ff6-cee1563dde36",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `3b5cf6e5-acbd-41b2-a869-72fd86fad7e2`. Trường actor giả mạo đặt trong request: `448ed458-c62b-46d2-9f4d-1dcc5c0d0017`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- TuChoiDeXuatAsync trả KetQua thất bại: MaLoi=404, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả NotFoundObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=404), message="Lỗi mẫu"; không trả IActionResult. Gọi TuChoiDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 404 nhưng action ném LoiNghiepVu thay vì trả NotFoundObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:115; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả NotFoundObjectResult theo mã 404; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TuChoiDeXuatAsync",
    "arguments": [
      "626b5524-efb6-4a2c-9ff6-cee1563dde36",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "3b5cf6e5-acbd-41b2-a869-72fd86fad7e2",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.TuChoi(Guid maYeuCau, String lyDo, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 115

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-134 — TuChoi: Service trả mã 409

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.TuChoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "ad18c6ac-e738-45c6-9421-4436d07f6edc",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `e4bdcfa3-db36-4633-837f-98a2760dedea`. Trường actor giả mạo đặt trong request: `fbe2e327-032d-41c3-b300-cbaf45bcc30a`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- TuChoiDeXuatAsync trả KetQua thất bại: MaLoi=409, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ConflictObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=409), message="Lỗi mẫu"; không trả IActionResult. Gọi TuChoiDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 409 nhưng action ném LoiNghiepVu thay vì trả ConflictObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:115; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả ConflictObjectResult theo mã 409; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TuChoiDeXuatAsync",
    "arguments": [
      "ad18c6ac-e738-45c6-9421-4436d07f6edc",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "e4bdcfa3-db36-4633-837f-98a2760dedea",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.TuChoi(Guid maYeuCau, String lyDo, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 115

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-135 — TuChoi: Service trả mã 400

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.TuChoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "db63b82f-2f19-4a48-bd71-a0f818e16dbe",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `57f553b4-e114-4283-8942-b4677db855cb`. Trường actor giả mạo đặt trong request: `464e38ac-e8ed-4a78-ba33-994b3fd4ca7a`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- TuChoiDeXuatAsync trả KetQua thất bại: MaLoi=400, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi TuChoiDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 400 nhưng action ném LoiNghiepVu thay vì trả BadRequestObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:115; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả BadRequestObjectResult theo mã 400; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TuChoiDeXuatAsync",
    "arguments": [
      "db63b82f-2f19-4a48-bd71-a0f818e16dbe",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "57f553b4-e114-4283-8942-b4677db855cb",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.TuChoi(Guid maYeuCau, String lyDo, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 115

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-136 — TuChoi: Service trả mã 500

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.TuChoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "2486a832-8beb-416e-a603-2b763945ff0b",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `f21def39-66f3-4668-9b86-47be3a7cd541`. Trường actor giả mạo đặt trong request: `e08e925e-2512-499c-8968-2680c500c511`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- TuChoiDeXuatAsync trả KetQua thất bại: MaLoi=500, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=500), message="Lỗi mẫu"; không trả IActionResult. Gọi TuChoiDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 500 nhưng action ném LoiNghiepVu thay vì trả BadRequestObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:115; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả BadRequestObjectResult theo mã 500; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TuChoiDeXuatAsync",
    "arguments": [
      "2486a832-8beb-416e-a603-2b763945ff0b",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "f21def39-66f3-4668-9b86-47be3a7cd541",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.TuChoi(Guid maYeuCau, String lyDo, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 115

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-137 — TuChoi: Truyền đúng tham số và token

**Kết quả: Pass.** Đạt truyền dữ liệu/người thực hiện/token

**Đơn vị thiết kế:** `KetNoiController.TuChoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "714b6f6f-1488-4e24-b157-bb40fbd3eb3c",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `224a3eb1-0151-4aab-a814-9173f58a0a95`. Trường actor giả mạo đặt trong request: `cf0a6392-436a-4060-934b-d1ad4b94474c`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- TuChoiDeXuatAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Verify TuChoiDeXuatAsync nhận nguyên mã, RV1, các trường input, actor=U1 và ct đúng một lần; không dùng actor từ dữ liệu người dùng gửi.

**Thực tế:** RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi TuChoiDeXuatAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Các assertion của ca này đạt; không lấy kết quả của ca này để suy ra các nhánh lỗi/thành công khác đều đúng.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:115

**Nguyên nhân:** Giữ nguyên input; service nhận đúng các ID, RV1/lý do/đồng ý có trong chữ ký, actor từ nguồn hiện tại và token RequestAborted, đúng một lần.

**Yêu cầu sửa/đối chiếu:** Không có sai lệch trong phạm vi ca truyền tham số.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TuChoiDeXuatAsync",
    "arguments": [
      "714b6f6f-1488-4e24-b157-bb40fbd3eb3c",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "224a3eb1-0151-4aab-a814-9173f58a0a95",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

### UT-138 — Rut: ModelState lỗi

**Kết quả: Fail.** Xử lý ModelState không đúng hợp đồng controller

**Đơn vị thiết kế:** `KetNoiController.Rut(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "e022acee-1578-4519-8c9d-7469a8de3f64",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `d239aa2e-9517-4f5a-b691-35eb457455a3`. Trường actor giả mạo đặt trong request: `a11c2627-467b-4a09-9dfd-b36c973ee5e2`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- Thêm ModelState.AddModelError("field", "Lỗi mẫu").
- Service được cấu hình trả thành công để phát hiện việc controller vẫn gọi service khi đầu vào đang lỗi; kỳ vọng số lần gọi là 0.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** BadRequestObjectResult chứa lỗi ModelState; không gọi RutYeuCauAsync; không thông báo thành công.

**Thực tế:** RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi RutYeuCauAsync 1 lần.

**Lỗi ModelState quan sát được:** `{"field": ["Lỗi mẫu"]}`.

**Đối chiếu kỳ vọng:**

- Thiết kế yêu cầu không gọi service, thực tế gọi RutYeuCauAsync 1 lần.
- ModelState đang lỗi nhưng vẫn gán thông báo thành công "Đã lưu thay đổi.".
- Thiết kế yêu cầu BadRequestObjectResult; thực tế trả RedirectToActionResult.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:123

**Nguyên nhân:** Action chưa chặn ModelState không hợp lệ trước khi gọi service. Mock service trả thành công nên code tiếp tục gán Success và chuyển hướng.

**Ảnh hưởng:** Nhánh từ chối dữ liệu của controller không đáp ứng thiết kế. Controller đã dispatch thao tác ghi dù ModelState lỗi; unit không thực hiện thao tác ghi thật.

**Yêu cầu sửa/đối chiếu:** Thêm guard ModelState đầu action, trả BadRequest(ModelState); service phải có số lần gọi bằng 0 khi dữ liệu đầu vào lỗi.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "RutYeuCauAsync",
    "arguments": [
      "e022acee-1578-4519-8c9d-7469a8de3f64",
      "AQIDBAUGBwg=",
      "d239aa2e-9517-4f5a-b691-35eb457455a3",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.False() Failure

Expected: False

Actual:   True
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 20

--- End of stack trace from previous location ---
```

### UT-139 — Rut: Thành công

**Kết quả: Fail.** Phản hồi thành công khác thiết kế

**Đơn vị thiết kế:** `KetNoiController.Rut(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "1f2b5214-fd34-40ec-8b43-943a437fcedc",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `ac3b1b91-c911-4041-9faf-e1a2f5465c0d`. Trường actor giả mạo đặt trong request: `9c60f659-1375-4b68-be31-cb4e5203e5ce`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- RutYeuCauAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** RedirectToActionResult.ActionName=Index; TempData["Success"]="Đã xử lý yêu cầu."; chỉ gọi service một lần.

**Thực tế:** RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi RutYeuCauAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Mong đợi Success="Đã xử lý yêu cầu."; thực tế Success="Đã lưu thay đổi.".

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:124; Areas/KetNoi/Controllers/KetNoiController.cs:125; Controllers/BaseController.cs:11

**Nguyên nhân:** Action gọi Success() mặc định “Đã lưu thay đổi.” hoặc truyền một nội dung khác với TC.

**Ảnh hưởng:** Phản hồi sau xử lý thành công không khớp đích điều hướng/nội dung thông báo được thiết kế. Không phải bằng chứng service đã lưu dữ liệu sai.

**Yêu cầu sửa/đối chiếu:** Đồng bộ nội dung Success và kiểu/đích chuyển hướng theo từng kỳ vọng TC; giữ nguyên ID trả về ở ca chấp nhận.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "RutYeuCauAsync",
    "arguments": [
      "1f2b5214-fd34-40ec-8b43-943a437fcedc",
      "AQIDBAUGBwg=",
      "ac3b1b91-c911-4041-9faf-e1a2f5465c0d",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Equal() Failure: Values differ

Expected: Đã xử lý yêu cầu.

Actual:   Đã lưu thay đổi.
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 33

--- End of stack trace from previous location ---
```

### UT-140 — Rut: Service trả mã 403

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.Rut(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "0fde284a-c053-41f1-8c60-5cf5116af68f",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `052a3b1c-07e0-47b3-a286-a742f06de0cd`. Trường actor giả mạo đặt trong request: `4a7455ff-0960-4e0a-b65c-0a82358b0f37`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- RutYeuCauAsync trả KetQua thất bại: MaLoi=403, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ForbidResult; ModelState có lỗi chung “Lỗi mẫu”; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=403), message="Lỗi mẫu"; không trả IActionResult. Gọi RutYeuCauAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 403 nhưng action ném LoiNghiepVu thay vì trả ForbidResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:123; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả ForbidResult theo mã 403; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "RutYeuCauAsync",
    "arguments": [
      "0fde284a-c053-41f1-8c60-5cf5116af68f",
      "AQIDBAUGBwg=",
      "052a3b1c-07e0-47b3-a286-a742f06de0cd",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.Rut(Guid maYeuCau, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 123

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-141 — Rut: Service trả mã 404

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.Rut(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "1775c833-14cc-467b-b8ed-89a6a07b05af",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `6f3b8630-733f-45c1-b425-69e3f5643b20`. Trường actor giả mạo đặt trong request: `97e3b7a9-a6bc-414e-9561-b3ea42ad1f50`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- RutYeuCauAsync trả KetQua thất bại: MaLoi=404, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả NotFoundObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=404), message="Lỗi mẫu"; không trả IActionResult. Gọi RutYeuCauAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 404 nhưng action ném LoiNghiepVu thay vì trả NotFoundObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:123; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả NotFoundObjectResult theo mã 404; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "RutYeuCauAsync",
    "arguments": [
      "1775c833-14cc-467b-b8ed-89a6a07b05af",
      "AQIDBAUGBwg=",
      "6f3b8630-733f-45c1-b425-69e3f5643b20",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.Rut(Guid maYeuCau, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 123

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-142 — Rut: Service trả mã 409

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.Rut(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "4e8c91f1-81ce-41b3-8756-c53aece147ef",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `2b75cfb0-601a-46c6-8d37-df6819ae8bcf`. Trường actor giả mạo đặt trong request: `cc85b29c-ed5f-4166-b83e-655286efe1ba`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- RutYeuCauAsync trả KetQua thất bại: MaLoi=409, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ConflictObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=409), message="Lỗi mẫu"; không trả IActionResult. Gọi RutYeuCauAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 409 nhưng action ném LoiNghiepVu thay vì trả ConflictObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:123; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả ConflictObjectResult theo mã 409; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "RutYeuCauAsync",
    "arguments": [
      "4e8c91f1-81ce-41b3-8756-c53aece147ef",
      "AQIDBAUGBwg=",
      "2b75cfb0-601a-46c6-8d37-df6819ae8bcf",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.Rut(Guid maYeuCau, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 123

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-143 — Rut: Service trả mã 400

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.Rut(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "39279896-0cc3-4d41-83a3-16a66e13f367",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `465a6bcc-9760-4342-8fdb-43b54bed9ed9`. Trường actor giả mạo đặt trong request: `7a738046-0e29-4206-985b-5ecf0cb55974`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- RutYeuCauAsync trả KetQua thất bại: MaLoi=400, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi RutYeuCauAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 400 nhưng action ném LoiNghiepVu thay vì trả BadRequestObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:123; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả BadRequestObjectResult theo mã 400; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "RutYeuCauAsync",
    "arguments": [
      "39279896-0cc3-4d41-83a3-16a66e13f367",
      "AQIDBAUGBwg=",
      "465a6bcc-9760-4342-8fdb-43b54bed9ed9",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.Rut(Guid maYeuCau, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 123

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-144 — Rut: Service trả mã 500

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.Rut(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "d70881e2-fa08-4405-a779-5f60cd4be54d",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `68061063-ef91-48d3-8b0d-aac40ce20a6d`. Trường actor giả mạo đặt trong request: `ba9d7004-d7a3-4124-94da-87501b10c9f7`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- RutYeuCauAsync trả KetQua thất bại: MaLoi=500, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=500), message="Lỗi mẫu"; không trả IActionResult. Gọi RutYeuCauAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 500 nhưng action ném LoiNghiepVu thay vì trả BadRequestObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:123; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả BadRequestObjectResult theo mã 500; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "RutYeuCauAsync",
    "arguments": [
      "d70881e2-fa08-4405-a779-5f60cd4be54d",
      "AQIDBAUGBwg=",
      "68061063-ef91-48d3-8b0d-aac40ce20a6d",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.Rut(Guid maYeuCau, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 123

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-145 — Rut: Truyền đúng tham số và token

**Kết quả: Pass.** Đạt truyền dữ liệu/người thực hiện/token

**Đơn vị thiết kế:** `KetNoiController.Rut(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "5325d625-c061-4ef6-904a-85954603757c",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `498b8fa7-5af9-485b-a517-ec161d034cff`. Trường actor giả mạo đặt trong request: `bde3c1c2-b08d-469e-9708-3d0122227a27`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- RutYeuCauAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Verify RutYeuCauAsync nhận nguyên mã, RV1, các trường input, actor=U1 và ct đúng một lần; không dùng actor từ dữ liệu người dùng gửi.

**Thực tế:** RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi RutYeuCauAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Các assertion của ca này đạt; không lấy kết quả của ca này để suy ra các nhánh lỗi/thành công khác đều đúng.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:123

**Nguyên nhân:** Giữ nguyên input; service nhận đúng các ID, RV1/lý do/đồng ý có trong chữ ký, actor từ nguồn hiện tại và token RequestAborted, đúng một lần.

**Yêu cầu sửa/đối chiếu:** Không có sai lệch trong phạm vi ca truyền tham số.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "RutYeuCauAsync",
    "arguments": [
      "5325d625-c061-4ef6-904a-85954603757c",
      "AQIDBAUGBwg=",
      "498b8fa7-5af9-485b-a517-ec161d034cff",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

### UT-146 — XacNhanKetThuc: ModelState lỗi

**Kết quả: Fail.** Xử lý ModelState không đúng hợp đồng controller

**Đơn vị thiết kế:** `LopHocController.XacNhanKetThuc(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "b1ad6bf5-fa3c-4c50-8020-124edc0de8f6",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `92277bc7-13fc-4db4-8369-e0fb1b3c4724`. Trường actor giả mạo đặt trong request: `d396083f-adc6-4472-9a5b-25c98ed0c52d`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- Thêm ModelState.AddModelError("field", "Lỗi mẫu").
- Service được cấu hình trả thành công để phát hiện việc controller vẫn gọi service khi đầu vào đang lỗi; kỳ vọng số lần gọi là 0.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** BadRequestObjectResult chứa lỗi ModelState; không gọi XacNhanKetThucAsync; không thông báo thành công.

**Thực tế:** RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi XacNhanKetThucAsync 1 lần.

**Lỗi ModelState quan sát được:** `{"field": ["Lỗi mẫu"]}`.

**Đối chiếu kỳ vọng:**

- Thiết kế yêu cầu không gọi service, thực tế gọi XacNhanKetThucAsync 1 lần.
- ModelState đang lỗi nhưng vẫn gán thông báo thành công "Đã lưu thay đổi.".
- Thiết kế yêu cầu BadRequestObjectResult; thực tế trả RedirectToActionResult.

**Vị trí code:** Areas/HocTap/Controllers/LopHocController.cs:110

**Nguyên nhân:** Action chưa chặn ModelState không hợp lệ trước khi gọi service. Mock service trả thành công nên code tiếp tục gán Success và chuyển hướng.

**Ảnh hưởng:** Nhánh từ chối dữ liệu của controller không đáp ứng thiết kế. Controller đã dispatch thao tác ghi dù ModelState lỗi; unit không thực hiện thao tác ghi thật.

**Yêu cầu sửa/đối chiếu:** Thêm guard ModelState đầu action, trả BadRequest(ModelState); service phải có số lần gọi bằng 0 khi dữ liệu đầu vào lỗi.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "XacNhanKetThucAsync",
    "arguments": [
      "b1ad6bf5-fa3c-4c50-8020-124edc0de8f6",
      "AQIDBAUGBwg=",
      "92277bc7-13fc-4db4-8369-e0fb1b3c4724",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.False() Failure

Expected: False

Actual:   True
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 20

--- End of stack trace from previous location ---
```

### UT-147 — XacNhanKetThuc: Thành công

**Kết quả: Fail.** Phản hồi thành công khác thiết kế

**Đơn vị thiết kế:** `LopHocController.XacNhanKetThuc(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "e05ec08e-e386-4a9e-8da7-625bf612a331",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `d71aee51-d5b6-41e6-b032-153e1788d583`. Trường actor giả mạo đặt trong request: `d30e4f50-908b-4c9a-91dc-e111c61d97c4`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- XacNhanKetThucAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** RedirectToActionResult.ActionName=Index; TempData["Success"]="Đã xử lý yêu cầu."; chỉ gọi service một lần.

**Thực tế:** RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi XacNhanKetThucAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Mong đợi Success="Đã xử lý yêu cầu."; thực tế Success="Đã lưu thay đổi.".

**Vị trí code:** Areas/HocTap/Controllers/LopHocController.cs:111; Areas/HocTap/Controllers/LopHocController.cs:112; Controllers/BaseController.cs:11

**Nguyên nhân:** Action gọi Success() mặc định “Đã lưu thay đổi.” hoặc truyền một nội dung khác với TC.

**Ảnh hưởng:** Phản hồi sau xử lý thành công không khớp đích điều hướng/nội dung thông báo được thiết kế. Không phải bằng chứng service đã lưu dữ liệu sai.

**Yêu cầu sửa/đối chiếu:** Đồng bộ nội dung Success và kiểu/đích chuyển hướng theo từng kỳ vọng TC; giữ nguyên ID trả về ở ca chấp nhận.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "XacNhanKetThucAsync",
    "arguments": [
      "e05ec08e-e386-4a9e-8da7-625bf612a331",
      "AQIDBAUGBwg=",
      "d71aee51-d5b6-41e6-b032-153e1788d583",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Equal() Failure: Values differ

Expected: Đã xử lý yêu cầu.

Actual:   Đã lưu thay đổi.
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 33

--- End of stack trace from previous location ---
```

### UT-148 — XacNhanKetThuc: Service trả mã 403

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `LopHocController.XacNhanKetThuc(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "ece4891c-3fbc-4740-bbb7-45ba74bb2afb",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `f35d645b-2d60-4b06-b5b7-8182309fd65d`. Trường actor giả mạo đặt trong request: `6fd55dd3-f8e4-47a1-9751-d921918d9065`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- XacNhanKetThucAsync trả KetQua thất bại: MaLoi=403, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ForbidResult; ModelState có lỗi chung “Lỗi mẫu”; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=403), message="Lỗi mẫu"; không trả IActionResult. Gọi XacNhanKetThucAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 403 nhưng action ném LoiNghiepVu thay vì trả ForbidResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.

**Vị trí code:** Areas/HocTap/Controllers/LopHocController.cs:110; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả ForbidResult theo mã 403; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "XacNhanKetThucAsync",
    "arguments": [
      "ece4891c-3fbc-4740-bbb7-45ba74bb2afb",
      "AQIDBAUGBwg=",
      "f35d645b-2d60-4b06-b5b7-8182309fd65d",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.HocTap.Controllers.LopHocController.XacNhanKetThuc(Guid maYeuCau, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\HocTap\Controllers\LopHocController.cs:line 110

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-149 — XacNhanKetThuc: Service trả mã 404

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `LopHocController.XacNhanKetThuc(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "29f640c2-5764-48aa-9aa8-f8433b93976e",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `fcc01983-3940-4f19-a659-561c1d4d5826`. Trường actor giả mạo đặt trong request: `c4217644-8e1c-456c-a8a5-b5af8a46b693`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- XacNhanKetThucAsync trả KetQua thất bại: MaLoi=404, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả NotFoundObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=404), message="Lỗi mẫu"; không trả IActionResult. Gọi XacNhanKetThucAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 404 nhưng action ném LoiNghiepVu thay vì trả NotFoundObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/HocTap/Controllers/LopHocController.cs:110; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả NotFoundObjectResult theo mã 404; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "XacNhanKetThucAsync",
    "arguments": [
      "29f640c2-5764-48aa-9aa8-f8433b93976e",
      "AQIDBAUGBwg=",
      "fcc01983-3940-4f19-a659-561c1d4d5826",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.HocTap.Controllers.LopHocController.XacNhanKetThuc(Guid maYeuCau, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\HocTap\Controllers\LopHocController.cs:line 110

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-150 — XacNhanKetThuc: Service trả mã 409

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `LopHocController.XacNhanKetThuc(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "333a9c8c-2c12-4d0b-8271-df3b584a2cc2",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `0b271257-3e42-4b1f-9262-77543dcbf6d9`. Trường actor giả mạo đặt trong request: `f63b0f10-3390-4f93-baa6-50ddf71b0863`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- XacNhanKetThucAsync trả KetQua thất bại: MaLoi=409, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ConflictObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=409), message="Lỗi mẫu"; không trả IActionResult. Gọi XacNhanKetThucAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 409 nhưng action ném LoiNghiepVu thay vì trả ConflictObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/HocTap/Controllers/LopHocController.cs:110; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả ConflictObjectResult theo mã 409; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "XacNhanKetThucAsync",
    "arguments": [
      "333a9c8c-2c12-4d0b-8271-df3b584a2cc2",
      "AQIDBAUGBwg=",
      "0b271257-3e42-4b1f-9262-77543dcbf6d9",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.HocTap.Controllers.LopHocController.XacNhanKetThuc(Guid maYeuCau, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\HocTap\Controllers\LopHocController.cs:line 110

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-151 — XacNhanKetThuc: Service trả mã 400

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `LopHocController.XacNhanKetThuc(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "28e137f8-3520-4700-a7f3-f8a058422582",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `2fd7910c-6075-46e3-a9da-c6abcfcbc126`. Trường actor giả mạo đặt trong request: `64c18a63-0d2a-475c-820c-5285ee2a9886`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- XacNhanKetThucAsync trả KetQua thất bại: MaLoi=400, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi XacNhanKetThucAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 400 nhưng action ném LoiNghiepVu thay vì trả BadRequestObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/HocTap/Controllers/LopHocController.cs:110; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả BadRequestObjectResult theo mã 400; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "XacNhanKetThucAsync",
    "arguments": [
      "28e137f8-3520-4700-a7f3-f8a058422582",
      "AQIDBAUGBwg=",
      "2fd7910c-6075-46e3-a9da-c6abcfcbc126",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.HocTap.Controllers.LopHocController.XacNhanKetThuc(Guid maYeuCau, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\HocTap\Controllers\LopHocController.cs:line 110

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-152 — XacNhanKetThuc: Service trả mã 500

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `LopHocController.XacNhanKetThuc(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "25102b73-6dbe-4ad1-b3b8-db0abc017955",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `1e37ba9f-b154-48f1-ba73-d22203ecbbbc`. Trường actor giả mạo đặt trong request: `9ca2933e-92ea-461a-b293-38302ef03404`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- XacNhanKetThucAsync trả KetQua thất bại: MaLoi=500, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=500), message="Lỗi mẫu"; không trả IActionResult. Gọi XacNhanKetThucAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 500 nhưng action ném LoiNghiepVu thay vì trả BadRequestObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/HocTap/Controllers/LopHocController.cs:110; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả BadRequestObjectResult theo mã 500; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "XacNhanKetThucAsync",
    "arguments": [
      "25102b73-6dbe-4ad1-b3b8-db0abc017955",
      "AQIDBAUGBwg=",
      "1e37ba9f-b154-48f1-ba73-d22203ecbbbc",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.HocTap.Controllers.LopHocController.XacNhanKetThuc(Guid maYeuCau, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\HocTap\Controllers\LopHocController.cs:line 110

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-153 — XacNhanKetThuc: Truyền đúng tham số và token

**Kết quả: Pass.** Đạt truyền dữ liệu/người thực hiện/token

**Đơn vị thiết kế:** `LopHocController.XacNhanKetThuc(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maYeuCau": "02fac8ad-d543-4a2e-9609-b6cd4a37b3ff",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `1244381d-073a-41d6-bd0e-9586fc29f01e`. Trường actor giả mạo đặt trong request: `45a74616-eda9-4f72-be4f-c98e33c73584`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- XacNhanKetThucAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Verify XacNhanKetThucAsync nhận nguyên mã, RV1, các trường input, actor=U1 và ct đúng một lần; không dùng actor từ dữ liệu người dùng gửi.

**Thực tế:** RedirectToActionResult tới Index; Success="Đã lưu thay đổi.". Gọi XacNhanKetThucAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Các assertion của ca này đạt; không lấy kết quả của ca này để suy ra các nhánh lỗi/thành công khác đều đúng.

**Vị trí code:** Areas/HocTap/Controllers/LopHocController.cs:110

**Nguyên nhân:** Giữ nguyên input; service nhận đúng các ID, RV1/lý do/đồng ý có trong chữ ký, actor từ nguồn hiện tại và token RequestAborted, đúng một lần.

**Yêu cầu sửa/đối chiếu:** Không có sai lệch trong phạm vi ca truyền tham số.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "XacNhanKetThucAsync",
    "arguments": [
      "02fac8ad-d543-4a2e-9609-b6cd4a37b3ff",
      "AQIDBAUGBwg=",
      "1244381d-073a-41d6-bd0e-9586fc29f01e",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

### UT-154 — HuyKetNoi: ModelState lỗi

**Kết quả: Fail.** Xử lý ModelState không đúng hợp đồng controller

**Đơn vị thiết kế:** `KetNoiController.HuyKetNoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maLop": "256a4c5a-69c7-409e-923c-2eee7cfd129c",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `e6615290-1adc-41e2-a1c5-494a567e40df`. Trường actor giả mạo đặt trong request: `079e5244-8134-4fc3-aeb6-69673c968871`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- Thêm ModelState.AddModelError("field", "Lỗi mẫu").
- Service được cấu hình trả thành công để phát hiện việc controller vẫn gọi service khi đầu vào đang lỗi; kỳ vọng số lần gọi là 0.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** BadRequestObjectResult chứa lỗi ModelState; không gọi TaoYeuCauHuyAsync; không thông báo thành công.

**Thực tế:** RedirectResult.Url=/HocTap/LopHoc/ChiTiet/256a4c5a-69c7-409e-923c-2eee7cfd129c; Success="Đã gửi yêu cầu kết thúc lớp.". Gọi TaoYeuCauHuyAsync 1 lần.

**Lỗi ModelState quan sát được:** `{"field": ["Lỗi mẫu"]}`.

**Đối chiếu kỳ vọng:**

- Thiết kế yêu cầu không gọi service, thực tế gọi TaoYeuCauHuyAsync 1 lần.
- ModelState đang lỗi nhưng vẫn gán thông báo thành công "Đã gửi yêu cầu kết thúc lớp.".
- Thiết kế yêu cầu BadRequestObjectResult; thực tế trả RedirectResult.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:132

**Nguyên nhân:** Action chưa chặn ModelState không hợp lệ trước khi gọi service. Mock service trả thành công nên code tiếp tục gán Success và chuyển hướng.

**Ảnh hưởng:** Nhánh từ chối dữ liệu của controller không đáp ứng thiết kế. Controller đã dispatch thao tác ghi dù ModelState lỗi; unit không thực hiện thao tác ghi thật.

**Yêu cầu sửa/đối chiếu:** Thêm guard ModelState đầu action, trả BadRequest(ModelState); service phải có số lần gọi bằng 0 khi dữ liệu đầu vào lỗi.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TaoYeuCauHuyAsync",
    "arguments": [
      "256a4c5a-69c7-409e-923c-2eee7cfd129c",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "e6615290-1adc-41e2-a1c5-494a567e40df",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.False() Failure

Expected: False

Actual:   True
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 20

--- End of stack trace from previous location ---
```

### UT-155 — HuyKetNoi: Thành công

**Kết quả: Fail.** Phản hồi thành công khác thiết kế

**Đơn vị thiết kế:** `KetNoiController.HuyKetNoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maLop": "5cb4e17a-3693-45ad-92ca-045d8bcc0303",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `26d56571-c446-4a0a-8721-f4ea5bba0ccf`. Trường actor giả mạo đặt trong request: `4ec5b334-78e5-4979-aa97-6a51d4672ef9`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- TaoYeuCauHuyAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** RedirectToActionResult.ActionName=Index; TempData["Success"]="Đã xử lý yêu cầu."; chỉ gọi service một lần.

**Thực tế:** RedirectResult.Url=/HocTap/LopHoc/ChiTiet/5cb4e17a-3693-45ad-92ca-045d8bcc0303; Success="Đã gửi yêu cầu kết thúc lớp.". Gọi TaoYeuCauHuyAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Mong đợi RedirectToActionResult.ActionName=Index; thực tế RedirectResult, Url=/HocTap/LopHoc/ChiTiet/5cb4e17a-3693-45ad-92ca-045d8bcc0303.
- Mong đợi Success="Đã xử lý yêu cầu."; thực tế Success="Đã gửi yêu cầu kết thúc lớp.".

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:135; Areas/KetNoi/Controllers/KetNoiController.cs:136

**Nguyên nhân:** Action gọi Success() mặc định “Đã lưu thay đổi.” hoặc truyền một nội dung khác với TC. Đồng thời action dùng Redirect(url) tới trang lớp thay vì RedirectToAction(Index).

**Ảnh hưởng:** Phản hồi sau xử lý thành công không khớp đích điều hướng/nội dung thông báo được thiết kế. Không phải bằng chứng service đã lưu dữ liệu sai.

**Yêu cầu sửa/đối chiếu:** Đồng bộ nội dung Success và kiểu/đích chuyển hướng theo từng kỳ vọng TC; giữ nguyên ID trả về ở ca chấp nhận.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TaoYeuCauHuyAsync",
    "arguments": [
      "5cb4e17a-3693-45ad-92ca-045d8bcc0303",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "26d56571-c446-4a0a-8721-f4ea5bba0ccf",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.IsType() Failure: Value is not the exact type

Expected: typeof(Microsoft.AspNetCore.Mvc.RedirectToActionResult)

Actual:   typeof(Microsoft.AspNetCore.Mvc.RedirectResult)
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 32

--- End of stack trace from previous location ---
```

### UT-156 — HuyKetNoi: Service trả mã 403

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.HuyKetNoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maLop": "ccd90373-3fc9-442e-952e-2493cc9102d5",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `17a6dae2-c056-4d75-b258-a877f4729073`. Trường actor giả mạo đặt trong request: `6b34e5d3-86de-4ad1-97bb-55ebb4b8702a`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- TaoYeuCauHuyAsync trả KetQua thất bại: MaLoi=403, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ForbidResult; ModelState có lỗi chung “Lỗi mẫu”; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=403), message="Lỗi mẫu"; không trả IActionResult. Gọi TaoYeuCauHuyAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 403 nhưng action ném LoiNghiepVu thay vì trả ForbidResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:134

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả ForbidResult theo mã 403; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TaoYeuCauHuyAsync",
    "arguments": [
      "ccd90373-3fc9-442e-952e-2493cc9102d5",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "17a6dae2-c056-4d75-b258-a877f4729073",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.HuyKetNoi(Guid maLop, String lyDo, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 134

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-157 — HuyKetNoi: Service trả mã 404

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.HuyKetNoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maLop": "9d32ddc0-2daf-438a-b4cc-bb36f53cd64d",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `aacce3ce-b4b4-4a0b-81a3-1e007f97f219`. Trường actor giả mạo đặt trong request: `4bc07480-1d4b-4718-ad26-f1d8bd9bd955`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- TaoYeuCauHuyAsync trả KetQua thất bại: MaLoi=404, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả NotFoundObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=404), message="Lỗi mẫu"; không trả IActionResult. Gọi TaoYeuCauHuyAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 404 nhưng action ném LoiNghiepVu thay vì trả NotFoundObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:134

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả NotFoundObjectResult theo mã 404; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TaoYeuCauHuyAsync",
    "arguments": [
      "9d32ddc0-2daf-438a-b4cc-bb36f53cd64d",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "aacce3ce-b4b4-4a0b-81a3-1e007f97f219",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.HuyKetNoi(Guid maLop, String lyDo, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 134

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-158 — HuyKetNoi: Service trả mã 409

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.HuyKetNoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maLop": "e7dd20fb-1a08-4a87-9053-42a32ce5bc57",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `659dd91b-4f76-4bb6-80e1-141dea932e28`. Trường actor giả mạo đặt trong request: `7fe16737-2613-44f7-af10-c772bf460b39`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- TaoYeuCauHuyAsync trả KetQua thất bại: MaLoi=409, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ConflictObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=409), message="Lỗi mẫu"; không trả IActionResult. Gọi TaoYeuCauHuyAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 409 nhưng action ném LoiNghiepVu thay vì trả ConflictObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:134

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả ConflictObjectResult theo mã 409; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TaoYeuCauHuyAsync",
    "arguments": [
      "e7dd20fb-1a08-4a87-9053-42a32ce5bc57",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "659dd91b-4f76-4bb6-80e1-141dea932e28",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.HuyKetNoi(Guid maLop, String lyDo, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 134

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-159 — HuyKetNoi: Service trả mã 400

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.HuyKetNoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maLop": "60350739-4c92-4ca2-9ea2-839fb76cf6f3",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `08be07fa-6cbf-4919-9b41-f2e9daa3a51c`. Trường actor giả mạo đặt trong request: `64a839b1-32dd-4b5a-9462-3956b5380819`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- TaoYeuCauHuyAsync trả KetQua thất bại: MaLoi=400, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi TaoYeuCauHuyAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 400 nhưng action ném LoiNghiepVu thay vì trả BadRequestObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:134

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả BadRequestObjectResult theo mã 400; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TaoYeuCauHuyAsync",
    "arguments": [
      "60350739-4c92-4ca2-9ea2-839fb76cf6f3",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "08be07fa-6cbf-4919-9b41-f2e9daa3a51c",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.HuyKetNoi(Guid maLop, String lyDo, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 134

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-160 — HuyKetNoi: Service trả mã 500

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.HuyKetNoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maLop": "7166ca0e-28da-4486-a131-a930bebf9318",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `2f80bc69-c102-4bd2-91ad-9c8d1db23e20`. Trường actor giả mạo đặt trong request: `d3c46bef-c3a1-4ee8-a44e-16eeff612897`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- TaoYeuCauHuyAsync trả KetQua thất bại: MaLoi=500, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=500), message="Lỗi mẫu"; không trả IActionResult. Gọi TaoYeuCauHuyAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 500 nhưng action ném LoiNghiepVu thay vì trả BadRequestObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:134

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả BadRequestObjectResult theo mã 500; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TaoYeuCauHuyAsync",
    "arguments": [
      "7166ca0e-28da-4486-a131-a930bebf9318",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "2f80bc69-c102-4bd2-91ad-9c8d1db23e20",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.HuyKetNoi(Guid maLop, String lyDo, Byte[] rowVersion) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 134

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-161 — HuyKetNoi: Truyền đúng tham số và token

**Kết quả: Pass.** Đạt truyền dữ liệu/người thực hiện/token

**Đơn vị thiết kế:** `KetNoiController.HuyKetNoi(...)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "maLop": "58ce9a2d-3f2b-416b-b7a4-43106785f6fe",
  "lyDo": "Lý do kiểm thử giữ nguyên",
  "rowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `69674f84-32d9-4dd9-96c8-45c33074dfdf`. Trường actor giả mạo đặt trong request: `b05e0f14-a6bd-4c0a-846c-393606f6106c`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- TaoYeuCauHuyAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Verify TaoYeuCauHuyAsync nhận nguyên mã, RV1, các trường input, actor=U1 và ct đúng một lần; không dùng actor từ dữ liệu người dùng gửi.

**Thực tế:** RedirectResult.Url=/HocTap/LopHoc/ChiTiet/58ce9a2d-3f2b-416b-b7a4-43106785f6fe; Success="Đã gửi yêu cầu kết thúc lớp.". Gọi TaoYeuCauHuyAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Các assertion của ca này đạt; không lấy kết quả của ca này để suy ra các nhánh lỗi/thành công khác đều đúng.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:132

**Nguyên nhân:** Giữ nguyên input; service nhận đúng các ID, RV1/lý do/đồng ý có trong chữ ký, actor từ nguồn hiện tại và token RequestAborted, đúng một lần.

**Yêu cầu sửa/đối chiếu:** Không có sai lệch trong phạm vi ca truyền tham số.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "TaoYeuCauHuyAsync",
    "arguments": [
      "58ce9a2d-3f2b-416b-b7a4-43106785f6fe",
      "Lý do kiểm thử giữ nguyên",
      "AQIDBAUGBwg=",
      "69674f84-32d9-4dd9-96c8-45c33074dfdf",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

### UT-162 — PhanHoiHuy: ModelState lỗi

**Kết quả: Fail.** Xử lý ModelState không đúng hợp đồng controller

**Đơn vị thiết kế:** `KetNoiController.PhanHoiHuy(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCauHuy": "e593ef60-00c4-4648-8084-56d5c7c1b0b0",
  "DongY": true,
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `8cadfa13-bc46-45be-94f9-0bc2a191daaf`. Trường actor giả mạo đặt trong request: `ad0a1084-230d-4860-9874-e04f0c77266a`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- Thêm ModelState.AddModelError("field", "Lỗi mẫu").
- Service được cấu hình trả thành công để phát hiện việc controller vẫn gọi service khi đầu vào đang lỗi; kỳ vọng số lần gọi là 0.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** BadRequestObjectResult chứa lỗi ModelState; không gọi PhanHoiHuyAsync; không thông báo thành công.

**Thực tế:** Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi PhanHoiHuyAsync 0 lần.

**Lỗi ModelState quan sát được:** `{"field": ["Lỗi mẫu"]}`.

**Đối chiếu kỳ vọng:**

- Thiết kế yêu cầu BadRequestObjectResult chứa lỗi; thực tế exception thoát khỏi action.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:142; Controllers/BaseController.cs:9

**Nguyên nhân:** Action gọi ValidateInput(); hàm này ném LoiNghiepVu khi ModelState lỗi, thay vì action trả BadRequestObjectResult.

**Ảnh hưởng:** Nhánh từ chối dữ liệu của controller không đáp ứng thiết kế. Không trả đúng loại kết quả mà bên gọi/action contract yêu cầu.

**Yêu cầu sửa/đối chiếu:** Kiểm ModelState và trả BadRequest(ModelState) tại action trước lời gọi ghi nghiệp vụ; không gán Success.

**Lời gọi service bắt được trên mock:**

```json
[]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Controllers.BaseController.ValidateInput() in C:\Khóa luận TN\Educonnect\Controllers\BaseController.cs:line 9

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.PhanHoiHuy(PhanHoiHuyViewModel vm) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 142

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 22

--- End of stack trace from previous location ---
```

### UT-163 — PhanHoiHuy: Thành công

**Kết quả: Fail.** Phản hồi thành công khác thiết kế

**Đơn vị thiết kế:** `KetNoiController.PhanHoiHuy(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCauHuy": "3ca87c44-6f00-4036-bfe3-6787c530062f",
  "DongY": true,
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `dc2d616d-fde4-4e73-9de4-067107af505e`. Trường actor giả mạo đặt trong request: `dc6974b0-8f77-4c72-95e5-4fcd19a6a311`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- PhanHoiHuyAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** RedirectToActionResult.ActionName=Index; TempData["Success"]="Đã xử lý yêu cầu."; chỉ gọi service một lần.

**Thực tế:** RedirectResult.Url=/HocTap/LopHoc; Success="Đã lưu thay đổi.". Gọi PhanHoiHuyAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Mong đợi RedirectToActionResult.ActionName=Index; thực tế RedirectResult, Url=/HocTap/LopHoc.
- Mong đợi Success="Đã xử lý yêu cầu."; thực tế Success="Đã lưu thay đổi.".

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:144; Areas/KetNoi/Controllers/KetNoiController.cs:145; Controllers/BaseController.cs:11

**Nguyên nhân:** Action gọi Success() mặc định “Đã lưu thay đổi.” hoặc truyền một nội dung khác với TC. Đồng thời action dùng Redirect(url) tới trang lớp thay vì RedirectToAction(Index).

**Ảnh hưởng:** Phản hồi sau xử lý thành công không khớp đích điều hướng/nội dung thông báo được thiết kế. Không phải bằng chứng service đã lưu dữ liệu sai.

**Yêu cầu sửa/đối chiếu:** Đồng bộ nội dung Success và kiểu/đích chuyển hướng theo từng kỳ vọng TC; giữ nguyên ID trả về ở ca chấp nhận.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "PhanHoiHuyAsync",
    "arguments": [
      "3ca87c44-6f00-4036-bfe3-6787c530062f",
      true,
      "AQIDBAUGBwg=",
      "dc2d616d-fde4-4e73-9de4-067107af505e",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.IsType() Failure: Value is not the exact type

Expected: typeof(Microsoft.AspNetCore.Mvc.RedirectToActionResult)

Actual:   typeof(Microsoft.AspNetCore.Mvc.RedirectResult)
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 32

--- End of stack trace from previous location ---
```

### UT-164 — PhanHoiHuy: Service trả mã 403

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.PhanHoiHuy(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCauHuy": "59533749-6725-4c28-9179-eba2db115ab7",
  "DongY": true,
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `e51b06ad-c6fd-44f3-b60a-6afa2c49a89c`. Trường actor giả mạo đặt trong request: `0a148de4-749f-47a0-9a10-29ec136d09a1`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- PhanHoiHuyAsync trả KetQua thất bại: MaLoi=403, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ForbidResult; ModelState có lỗi chung “Lỗi mẫu”; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=403), message="Lỗi mẫu"; không trả IActionResult. Gọi PhanHoiHuyAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 403 nhưng action ném LoiNghiepVu thay vì trả ForbidResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:143; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả ForbidResult theo mã 403; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "PhanHoiHuyAsync",
    "arguments": [
      "59533749-6725-4c28-9179-eba2db115ab7",
      true,
      "AQIDBAUGBwg=",
      "e51b06ad-c6fd-44f3-b60a-6afa2c49a89c",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.PhanHoiHuy(PhanHoiHuyViewModel vm) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 143

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-165 — PhanHoiHuy: Service trả mã 404

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.PhanHoiHuy(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCauHuy": "cb4aa974-925a-43b3-a6cb-a5179ad39cba",
  "DongY": true,
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `f0ed640a-fdd5-4119-bed4-aea1cdd6533e`. Trường actor giả mạo đặt trong request: `f51e4716-e6e1-4a69-b49e-ba2d00d3aad3`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- PhanHoiHuyAsync trả KetQua thất bại: MaLoi=404, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả NotFoundObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=404), message="Lỗi mẫu"; không trả IActionResult. Gọi PhanHoiHuyAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 404 nhưng action ném LoiNghiepVu thay vì trả NotFoundObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:143; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả NotFoundObjectResult theo mã 404; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "PhanHoiHuyAsync",
    "arguments": [
      "cb4aa974-925a-43b3-a6cb-a5179ad39cba",
      true,
      "AQIDBAUGBwg=",
      "f0ed640a-fdd5-4119-bed4-aea1cdd6533e",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.PhanHoiHuy(PhanHoiHuyViewModel vm) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 143

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-166 — PhanHoiHuy: Service trả mã 409

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.PhanHoiHuy(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCauHuy": "faf86340-39f4-4cdb-97f7-8c2be480cc44",
  "DongY": true,
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `b933b2b2-8d97-4646-bcdd-78e9868318c8`. Trường actor giả mạo đặt trong request: `61769fed-f9e7-441b-8d91-7521b39c66fd`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- PhanHoiHuyAsync trả KetQua thất bại: MaLoi=409, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả ConflictObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=409), message="Lỗi mẫu"; không trả IActionResult. Gọi PhanHoiHuyAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 409 nhưng action ném LoiNghiepVu thay vì trả ConflictObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:143; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả ConflictObjectResult theo mã 409; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "PhanHoiHuyAsync",
    "arguments": [
      "faf86340-39f4-4cdb-97f7-8c2be480cc44",
      true,
      "AQIDBAUGBwg=",
      "b933b2b2-8d97-4646-bcdd-78e9868318c8",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.PhanHoiHuy(PhanHoiHuyViewModel vm) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 143

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-167 — PhanHoiHuy: Service trả mã 400

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.PhanHoiHuy(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCauHuy": "81bccc24-5f5b-4e76-a00f-d8c92d429bf1",
  "DongY": true,
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `11160d65-7eb0-48f3-bbcc-86ce5cad2eee`. Trường actor giả mạo đặt trong request: `a96d0052-caac-4e45-aa08-9b98c50bbc08`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- PhanHoiHuyAsync trả KetQua thất bại: MaLoi=400, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=400), message="Lỗi mẫu"; không trả IActionResult. Gọi PhanHoiHuyAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 400 nhưng action ném LoiNghiepVu thay vì trả BadRequestObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:143; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả BadRequestObjectResult theo mã 400; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "PhanHoiHuyAsync",
    "arguments": [
      "81bccc24-5f5b-4e76-a00f-d8c92d429bf1",
      true,
      "AQIDBAUGBwg=",
      "11160d65-7eb0-48f3-bbcc-86ce5cad2eee",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.PhanHoiHuy(PhanHoiHuyViewModel vm) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 143

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-168 — PhanHoiHuy: Service trả mã 500

**Kết quả: Fail.** Chuyển lỗi service sang IActionResult không đúng thiết kế

**Đơn vị thiết kế:** `KetNoiController.PhanHoiHuy(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCauHuy": "21b298ea-aa3b-4106-a3d0-6fbf8aa970b5",
  "DongY": true,
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `2b3d5aaa-58f3-4956-a394-b20a3a663d42`. Trường actor giả mạo đặt trong request: `79a6a937-7bcf-4c03-b9f3-05b3a41141d8`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- PhanHoiHuyAsync trả KetQua thất bại: MaLoi=500, thông điệp “Lỗi mẫu”.
- Dùng đối tượng KetQua thật chứa lỗi do fixture tạo; chỉ mock service nghiệp vụ.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Kết quả BadRequestObjectResult; ModelState có lỗi chung “Lỗi mẫu”; body chứa kq lỗi; không ghi Success hoặc chuyển hướng thành công.

**Thực tế:** Ném LoiNghiepVu(Status=500), message="Lỗi mẫu"; không trả IActionResult. Gọi PhanHoiHuyAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Service trả lỗi 500 nhưng action ném LoiNghiepVu thay vì trả BadRequestObjectResult.
- Không thêm “Lỗi mẫu” vào ModelState khóa chung như thiết kế yêu cầu.
- Không có đúng ObjectResult chứa đối tượng KetQua lỗi làm body theo TC.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:143; Infrastructure/KetQuaKhongDuLieu.cs:39

**Nguyên nhân:** Action chuyển KetQua lỗi thành throw LoiNghiepVu hoặc gọi BaoLoiNeuThatBai(), nên không tạo IActionResult theo mã lỗi.

**Ảnh hưởng:** Hợp đồng phản hồi của action bị sai. Đây là kết quả gọi action trực tiếp; không lấy exception.Status làm bằng chứng controller đã trả đúng HTTP/IActionResult.

**Yêu cầu sửa/đối chiếu:** Xử lý KetQua thất bại trong action: thêm ModelState lỗi chung và trả BadRequestObjectResult theo mã 500; không ghi Success hoặc chuyển hướng thành công.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "PhanHoiHuyAsync",
    "arguments": [
      "21b298ea-aa3b-4106-a3d0-6fbf8aa970b5",
      true,
      "AQIDBAUGBwg=",
      "2b3d5aaa-58f3-4956-a394-b20a3a663d42",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

**Assertion và stack trace gốc:**

```text
Assert.Null() Failure: Value is not null

Expected: null

Actual:   EduConnect.Mvc.Infrastructure.LoiNghiepVu: Lỗi mẫu

   at EduConnect.Mvc.Infrastructure.KetQua.BaoLoiNeuThatBai() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 39

   at EduConnect.Mvc.Areas.KetNoi.Controllers.KetNoiController.PhanHoiHuy(PhanHoiHuyViewModel vm) in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Controllers\KetNoiController.cs:line 143

   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.<>c__DisplayClass0_0.<<RunAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 16

--- End of stack trace from previous location ---

   at Xunit.Record.ExceptionAsync(Func`1 testCode) in /_/src/xunit.core/Record.cs:line 76
   at EduConnect.KetNoiKetThuc.UnitTests.ControllerKetNoiKetThucRunner.RunAsync(Int32 id, ITestOutputHelper output) in C:\Khóa luận TN\Educonnect\tests\Unit\ControllerKetNoiKetThucRunner.cs:line 39

--- End of stack trace from previous location ---
```

### UT-169 — PhanHoiHuy: Truyền đúng tham số và token

**Kết quả: Pass.** Đạt truyền dữ liệu/người thực hiện/token

**Đơn vị thiết kế:** `KetNoiController.PhanHoiHuy(vm)`. Kiểm tên, kiểu đầu vào, Task<IActionResult> và HttpPost trước khi gọi.

**Đầu vào cụ thể của lần chạy:**

```json
{
  "MaYeuCauHuy": "6351dd55-6ed3-4101-a0fb-d2366362f5ff",
  "DongY": true,
  "RowVersion": "AQIDBAUGBwg="
}
```

Người hiện tại U1: `8fa66df9-81d2-4571-8d22-727b8c579026`. Trường actor giả mạo đặt trong request: `e49e2a70-b680-49e0-a868-efb68bc11111`. Token RequestAborted riêng cho mỗi test.

**Cấu hình mock:**

- PhanHoiHuyAsync trả thành công; C1 dùng cho những hợp đồng có dữ liệu trả về.
- Các action dùng KetQua không dữ liệu không bị giả tạo thuộc tính DuLieu.

**Các bước unit đã thực hiện:**

1. Tạo controller thật; gắn HttpContext, TempData, URL helper giả và nguồn người dùng hiện tại qua interface.
2. Kiểm đúng action/overload và kiểu trả về; chuẩn bị input/mock/ModelState như trên.
3. Gọi trực tiếp action, bắt lại kết quả hoặc exception để ghi minh chứng trước khi assert.
4. Assert theo kỳ vọng bên dưới; kiểm số lần gọi service. Ca truyền tham số kiểm nguyên input, RV/lý do/đồng ý có trong chữ ký, actor và token.

**Mong đợi theo bảng TC:** Verify PhanHoiHuyAsync nhận nguyên mã, RV1, các trường input, actor=U1 và ct đúng một lần; không dùng actor từ dữ liệu người dùng gửi.

**Thực tế:** RedirectResult.Url=/HocTap/LopHoc; Success="Đã lưu thay đổi.". Gọi PhanHoiHuyAsync 1 lần.

**Lỗi ModelState quan sát được:** `{}`.

**Đối chiếu kỳ vọng:**

- Các assertion của ca này đạt; không lấy kết quả của ca này để suy ra các nhánh lỗi/thành công khác đều đúng.

**Vị trí code:** Areas/KetNoi/Controllers/KetNoiController.cs:143

**Nguyên nhân:** Giữ nguyên input; service nhận đúng các ID, RV1/lý do/đồng ý có trong chữ ký, actor từ nguồn hiện tại và token RequestAborted, đúng một lần.

**Yêu cầu sửa/đối chiếu:** Không có sai lệch trong phạm vi ca truyền tham số.

**Lời gọi service bắt được trên mock:**

```json
[
  {
    "method": "PhanHoiHuyAsync",
    "arguments": [
      "6351dd55-6ed3-4101-a0fb-d2366362f5ff",
      true,
      "AQIDBAUGBwg=",
      "8fa66df9-81d2-4571-8d22-727b8c579026",
      {
        "type": "CancellationToken",
        "matchesRequestAborted": true
      }
    ]
  }
]
```

## Phạm vi và thay đổi để chạy unit

- Chỉ kiểm code controller và hợp đồng gọi service. Không khởi động ứng dụng, không gọi database, không chạy procedure hoặc middleware/filter.
- Mỗi ca có fixture riêng, mock strict, CancellationTokenSource riêng và input riêng. Không mock action đang kiểm thử.
- Current user được mock qua INguoiDungHienTaiService. Kiểm actor U1 truyền từ nguồn hiện tại, không dùng trường actor giả mạo trong request. Đây là unit gọi trực tiếp, không phải kiểm model binding/authorization pipeline.
- Bổ sung URL helper giả cho kết quả RedirectToAction, tránh ghi nhầm lỗi thiếu fixture thành lỗi ứng dụng. Lần chạy cuối không có lỗi fixture/thiếu dependency.
- Đổi phần lấy current user trong hai controller từ concrete sang interface đã đăng ký DI. LopHocController nhận ILopHocService/IBaiTapService đã có. Không sửa tên action, nhánh ModelState, xử lý KetQua, thông báo hoặc đích chuyển hướng để làm test Pass.
- Thêm ControllerKetNoiKetThucTest.cs (64 Fact), ControllerKetNoiKetThucRunner.cs và ControllerKetNoiKetThucFixture.cs.
- Build cuối thành công. Project có 169 mã TC duy nhất UT-001–169. Không ghi đè kết quả các nhóm trước.

## Chạy lại trong VS Code

Tại `C:\Khóa luận TN\Educonnect`:

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --no-restore -p:UseAppHost=false "-p:BaseOutputPath=C:/Users/NGUYEN THI YEN/Documents/Codex/2026-11-13/un/work/group06-bin/" --filter "Suite=ControllerKetNoiKetThuc" --results-directory "C:/Users/NGUYEN THI YEN/Documents/Codex/2026-11-13/un/outputs/06-Controller-KetNoi-KetThuc-Unit" --logger "trx;LogFileName=Controller-KetNoi-KetThuc-Unit.trx"
```

Chạy riêng ví dụ: `--filter "Suite=ControllerKetNoiKetThuc&TestCaseId=UT-122"`.

## Minh chứng

- [TRX cuối](Controller-KetNoi-KetThuc-Unit.trx)
- [Kết quả JSON kèm observation từng ca](KetQua.json)
- [Mã nguồn và thay đổi để chạy test](Ma_nguon_Unit.zip)
