# Kết quả 08 Kết thúc lớp và worker

**37 ca UT-221–257: 30 Pass, 7 Fail, 0 Skip/Blocked.**

Nguồn kỳ vọng: Unit Test.xlsx, sheet 08. Giữ nguyên tiêu chuẩn xác thực/đúng đắn/đầy đủ của từng ca. Không đổi guard hoặc payload ứng dụng để làm Pass.

## Phạm vi và dữ liệu chung

LopHocService, KetNoiService, ThongBaoService và BaoTriWorker là đối tượng thật. Mock IClassEndingStore, nguồn actor, transaction, IThongBaoData, IMaintenanceCycle và IMaintenanceTicks tùy ca. AppDbContext không provider nên không được phép truy vấn SQL thật. Không chạy MVC pipeline.

H=10000000-0000-0000-0000-000000000001; G=20000000-0000-0000-0000-000000000002; X=30000000-0000-0000-0000-000000000003; lớp C=40000000-0000-0000-0000-000000000004; yêu cầu R=50000000-0000-0000-0000-000000000005. RV1=[1,2,3,4,5,6,7,8]; RV0=[0,0,0,0,0,0,0,1]. T=2026-11-14 10:00:00+07:00. Fixture mới cho từng ca.

Transaction: unit xác nhận không Commit và đã Dispose khi bị từ chối/lưu lỗi; không thực thi rollback CSDL. Những bản ghi/notice/outbox trong RAM đã thêm trước Save lỗi không phải bằng chứng ghi dữ liệu thật. Các ca đọc lại sau khóa mô phỏng trạng thái đã thay đổi bằng dữ liệu trả về từ cổng, không kiểm SQL lock.

UT-221–223 mock IKetNoiService để kiểm riêng wrapper LopHocService. UT-247–250 gọi ExecuteAsync protected thật qua reflection, processor/timer giả để kiểm vòng lặp. Worker hiện có năm processor, được kiểm thứ tự hiện có; vòng đầu chạy ngay rồi chờ tick, không chờ 30 giây thật. UT-251–257 gọi XuLyTuHuyKetNoiAsync private thật với cổng đọc/ghi giả, không mock chính thuật toán xử lý hết hạn.

## Kết quả từng ca

| Mã | Tình huống | Tiêu chuẩn | Kết quả | Quan sát |
| --- | --- | --- | --- | --- |
| UT-221 | Chuyển tiếp thành công | Đúng đắn | Pass | ThanhCong=True; mã=200; thông điệp=null. Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=1; ticks=0; warning=0. |
| UT-222 | Chuyển tiếp lỗi | Đúng đắn | Pass | ThanhCong=False; mã=409; thông điệp="Xung đột mẫu". Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=1; ticks=0; warning=0. |
| UT-223 | Token đã hủy | Đúng đắn | Fail | ThanhCong=True; mã=200; thông điệp=null. Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=1; ticks=0; warning=0. |
| UT-224 | Lý do chỉ khoảng trắng | Đúng đắn | Pass | ThanhCong=False; mã=400; thông điệp="Nhập lý do kết thúc tối đa 2000 ký tự.". Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-225 | Lý do vượt ngưỡng | Đầy đủ | Pass | ThanhCong=False; mã=400; thông điệp="Nhập lý do kết thúc tối đa 2000 ký tự.". Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-226 | Lý do đúng ngưỡng | Đầy đủ | Pass | ThanhCong=True; mã=200; thông điệp=null. Trả mã yêu cầu 650cc558-afac-46d8-9d47-b3e59e6156b4. Begin=1; Save=1; Commit=1; DisposeTx=1; yêu cầu thêm=1; notice/outbox=1/1; forward=0; ticks=0; warning=0. |
| UT-227 | Không có lớp | Đúng đắn | Pass | ThanhCong=False; mã=404; thông điệp="Không tìm thấy lớp.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-228 | Không là thành viên chính | Đúng đắn | Pass | ThanhCong=False; mã=403; thông điệp="Chỉ hai bên chính được yêu cầu kết thúc lớp.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-229 | Lớp không hoạt động | Đúng đắn | Fail | ThanhCong=False; mã=409; thông điệp="Lớp không còn hoạt động.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-230 | Đã có yêu cầu chờ | Đúng đắn | Pass | ThanhCong=False; mã=409; thông điệp="Đang có yêu cầu kết thúc chờ phản hồi.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-231 | Phiên bản lớp cũ | Đúng đắn | Pass | ThanhCong=False; mã=409; thông điệp="Lớp vừa thay đổi. Hãy tải lại.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-232 | Tạo yêu cầu hợp lệ | Đúng đắn | Fail | ThanhCong=True; mã=200; thông điệp=null. Trả mã yêu cầu eef7987d-1914-4188-a058-4d0185697f8c. Begin=1; Save=1; Commit=1; DisposeTx=1; yêu cầu thêm=1; notice/outbox=1/1; forward=0; ticks=0; warning=0. |
| UT-233 | Lỗi concurrency khi lưu | Đúng đắn | Pass | ThanhCong=False; mã=409; thông điệp="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.". Begin=1; Save=1; Commit=0; DisposeTx=1; yêu cầu thêm=1; notice/outbox=1/1; forward=0; ticks=0; warning=0. |
| UT-234 | Lỗi cập nhật dữ liệu | Đúng đắn | Fail | Ném Microsoft.EntityFrameworkCore.DbUpdateException: Lỗi ghi giả lập tại Save. Begin=1; Save=1; Commit=0; DisposeTx=1; yêu cầu thêm=1; notice/outbox=1/1; forward=0; ticks=0; warning=0. |
| UT-235 | Không có yêu cầu | Đúng đắn | Pass | ThanhCong=False; mã=404; thông điệp="Không tìm thấy yêu cầu.". Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-236 | Người gửi tự phản hồi | Đúng đắn | Pass | ThanhCong=False; mã=403; thông điệp="Chỉ bên nhận được phản hồi yêu cầu.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-237 | Người ngoài phản hồi | Đúng đắn | Pass | ThanhCong=False; mã=403; thông điệp="Chỉ bên nhận được phản hồi yêu cầu.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-238 | Yêu cầu đã xử lý | Đúng đắn | Pass | ThanhCong=False; mã=409; thông điệp="Yêu cầu đã xử lý, hết hạn hoặc thay đổi.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-239 | Phiên bản yêu cầu cũ | Đúng đắn | Pass | ThanhCong=False; mã=409; thông điệp="Yêu cầu đã xử lý, hết hạn hoặc thay đổi.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-240 | Dùng phiên bản lớp thay phiên bản yêu cầu | Đúng đắn | Pass | ThanhCong=False; mã=409; thông điệp="Yêu cầu đã xử lý, hết hạn hoặc thay đổi.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-241 | Đúng hạn | Đúng đắn | Pass | ThanhCong=False; mã=409; thông điệp="Yêu cầu đã xử lý, hết hạn hoặc thay đổi.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-242 | Sau hạn | Đúng đắn | Pass | ThanhCong=False; mã=409; thông điệp="Yêu cầu đã xử lý, hết hạn hoặc thay đổi.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-243 | Đồng ý trước hạn | Đúng đắn | Fail | ThanhCong=True; mã=200; thông điệp=null. Begin=1; Save=1; Commit=1; DisposeTx=1; yêu cầu thêm=0; notice/outbox=1/1; forward=0; ticks=0; warning=0. |
| UT-244 | Từ chối trước hạn | Đúng đắn | Fail | ThanhCong=True; mã=200; thông điệp=null. Begin=1; Save=1; Commit=1; DisposeTx=1; yêu cầu thêm=0; notice/outbox=1/1; forward=0; ticks=0; warning=0. |
| UT-245 | Lỗi concurrency | Đúng đắn | Pass | ThanhCong=False; mã=409; thông điệp="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.". Begin=1; Save=1; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=1/1; forward=0; ticks=0; warning=0. |
| UT-246 | Lỗi ghi dữ liệu | Đúng đắn | Fail | Ném Microsoft.EntityFrameworkCore.DbUpdateException: Lỗi ghi giả lập tại Save. Begin=1; Save=1; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=1/1; forward=0; ticks=0; warning=0. |
| UT-247 | Token hủy ngay | Đúng đắn | Pass | ThanhCong=True; mã=0; thông điệp=null. Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-248 | Một vòng bình thường | Đúng đắn | Pass | ThanhCong=True; mã=0; thông điệp=null. Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=1; warning=0. |
| UT-249 | Hủy trong xử lý | Đúng đắn | Pass | ThanhCong=True; mã=0; thông điệp=null. Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-250 | Lỗi ngoài hủy | Đúng đắn | Pass | ThanhCong=True; mã=0; thông điệp=null. Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=2; warning=1. |
| UT-251 | Không có ứng viên | Đúng đắn | Pass | ThanhCong=True; mã=0; thông điệp=null. Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-252 | Chưa đến hạn sau khóa | Đúng đắn | Pass | ThanhCong=True; mã=0; thông điệp=null. Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-253 | Vừa được phản hồi | Đúng đắn | Pass | ThanhCong=True; mã=0; thông điệp=null. Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-254 | Lớp đã kết thúc | Đúng đắn | Pass | ThanhCong=True; mã=0; thông điệp=null. Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0. |
| UT-255 | Đúng hạn đủ điều kiện | Đúng đắn | Pass | ThanhCong=True; mã=0; thông điệp=null. Begin=1; Save=1; Commit=1; DisposeTx=1; yêu cầu thêm=0; notice/outbox=2/2; forward=0; ticks=0; warning=0. |
| UT-256 | Sau hạn đủ điều kiện | Đúng đắn | Pass | ThanhCong=True; mã=0; thông điệp=null. Begin=1; Save=1; Commit=1; DisposeTx=1; yêu cầu thêm=0; notice/outbox=2/2; forward=0; ticks=0; warning=0. |
| UT-257 | Quét lặp | Đúng đắn | Pass | ThanhCong=True; mã=0; thông điệp=null. Begin=2; Save=1; Commit=1; DisposeTx=2; yêu cầu thêm=0; notice/outbox=2/2; forward=0; ticks=0; warning=0. |

## Chi tiết bảy ca Fail

### UT-223 — Token đã hủy

**Đơn vị:** `LopHocService.XacNhanKetThucAsync(...)`.

**Fixture và cách tái hiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. ct đã CancellationRequested trước lời gọi.

**Kỳ vọng:** OperationCanceledException; không gọi PhanHoiHuyAsync.

**Thực tế:** ThanhCong=True; mã=200; thông điệp=null. Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=1; ticks=0; warning=0.

**Các sai lệch và phần đã đạt:**

- Kỳ vọng OperationCanceledException và 0 lời gọi PhanHoiHuyAsync; thực tế không exception, có 1 lời gọi và KetQua thành công.

**Assertion đầu tiên:** `Assert.IsAssignableFrom() Failure: Value is null<br>Expected: typeof(System.OperationCanceledException)<br>Actual:   null`.

**Vị trí code:** `Areas/HocTap/Services/LopHocService.cs:181`.

**Nguyên nhân:** XacNhanKetThucAsync là biểu thức gọi thẳng KetNoi.PhanHoiHuyAsync, không ThrowIfCancellationRequested ở wrapper. Dependency giả lập nhận token đã hủy nhưng trả thành công để kiểm xem wrapper có tự chặn trước lời gọi hay không.

**Ảnh hưởng:** Không đạt yêu cầu chặn sớm của wrapper. Service KetNoi thật vẫn có kiểm token; unit này không kết luận toàn hệ thống bỏ qua hủy.

**Đề xuất sửa:** Kiểm huy.ThrowIfCancellationRequested trước khi gọi dependency, giữ nguyên tham số và kết quả chuyển tiếp.

### UT-229 — Lớp không hoạt động

**Đơn vị:** `KetNoiService.TaoYeuCauHuyAsync(...)`.

**Fixture và cách tái hiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Lớp DaKetThuc; actor đúng thành viên chính.

**Kỳ vọng:** Rollback; KetQua 403.

**Thực tế:** ThanhCong=False; mã=409; thông điệp="Lớp không còn hoạt động.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.

**Các sai lệch và phần đã đạt:**

- Lớp fixture DaKetThuc, đúng thành viên chính: kỳ vọng 403, thực tế 409.
- Không thêm yêu cầu/Save/Commit/notice/outbox; transaction được dispose chưa commit.

**Assertion đầu tiên:** `Assert.Equal() Failure: Values differ<br>Expected: 403<br>Actual:   409`.

**Vị trí code:** `Areas/KetNoi/Services/KetNoiService.KetThuc.cs:28`.

**Nguyên nhân:** Guard trạng thái lớp trong TaoYeuCauHuyAsync đang phát sinh LoiNghiepVu 409 khi lớp không DangHoatDong; bảng yêu cầu 403.

**Ảnh hưởng:** Thao tác vẫn bị chặn; sai lệch ở hợp đồng mã lỗi, không phải đã kết thúc/lưu nhầm lớp.

**Đề xuất sửa:** Chốt mã lỗi 403 hay 409 cho xung đột trạng thái lớp rồi đồng bộ thiết kế và code. Theo bảng hiện tại ca vẫn Fail, không tự đổi kỳ vọng.

### UT-232 — Tạo yêu cầu hợp lệ

**Đơn vị:** `KetNoiService.TaoYeuCauHuyAsync(...)`.

**Fixture và cách tái hiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Lý do=" Xin kết thúc "; lớp hoạt động; thành viên chính đúng; RV1; không yêu cầu chờ; nguồn giờ trả T.

**Kỳ vọng:** Tạo yc có lý do trim, ChoPhanHoi, GuiLuc=T, HetHanLuc=T+72h, người gửi đúng; outbox YeuCauKetThucLop chứa yc.Id; lớp vẫn hoạt động; Save rồi Commit; trả yc.Id.

**Thực tế:** ThanhCong=True; mã=200; thông điệp=null. Trả mã yêu cầu eef7987d-1914-4188-a058-4d0185697f8c. Begin=1; Save=1; Commit=1; DisposeTx=1; yêu cầu thêm=1; notice/outbox=1/1; forward=0; ticks=0; warning=0.

**Các sai lệch và phần đã đạt:**

- Kỳ vọng LoaiSuKien=YeuCauKetThucLop; thực tế KetNoi.
- Payload thực tế chỉ có recipient và target=maLop; không có mã yêu cầu yc.Id.
- Lý do trim, trạng thái ChoPhanHoi, GuiLuc=T, HetHanLuc=T+72h, người gửi, lớp giữ nguyên và Save trước Commit đều đạt các assertion trước lỗi outbox.

**Assertion đầu tiên:** `Assert.Equal() Failure: Strings differ<br>           ↓ (pos 0)<br>Expected: "YeuCauKetThucLop"<br>Actual:   "KetNoi"<br>           ↑ (pos 0)`.

**Vị trí code:** `Areas/KetNoi/Services/KetNoiService.KetThuc.cs:44`.

**Nguyên nhân:** TaoYeuCauHuyAsync gọi notices.Tao(...,"KetNoi",maLop,...). ThongBaoService.Tao dùng cùng chuỗi làm LoaiSuKien và chỉ serialize recipient/target. Vì target là maLop nên payload không chứa yc.Id như kịch bản.

**Ảnh hưởng:** Consumer dựa trên tên sự kiện/mã yêu cầu theo thiết kế không nhận đủ hợp đồng từ outbox hiện tại. Unit không chạy consumer và không chứng minh lỗi gửi thông báo ngoài hệ thống.

**Đề xuất sửa:** Tạo sự kiện YeuCauKetThucLop có mã yêu cầu, mã lớp và người nhận theo hợp đồng; lưu cùng transaction. Không chỉ đổi loại hiển thị của notice nếu nghiệp vụ vẫn cần KetNoi.

### UT-234 — Lỗi cập nhật dữ liệu

**Đơn vị:** `KetNoiService.TaoYeuCauHuyAsync(...)`.

**Fixture và cách tái hiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng Save ném DbUpdateException.

**Kỳ vọng:** Rollback; KetQua 500; không commit.

**Thực tế:** Ném Microsoft.EntityFrameworkCore.DbUpdateException: Lỗi ghi giả lập tại Save. Begin=1; Save=1; Commit=0; DisposeTx=1; yêu cầu thêm=1; notice/outbox=1/1; forward=0; ticks=0; warning=0.

**Các sai lệch và phần đã đạt:**

- Kỳ vọng KetQua lỗi 500; thực tế ném Microsoft.EntityFrameworkCore.DbUpdateException, không trả KetQua.
- Save được thử một lần; không Commit; transaction đã dispose. Yêu cầu và notice/outbox đã được thêm trong bộ nhớ trước Save, không phải dữ liệu đã lưu SQL.

**Assertion đầu tiên:** `Assert.Null() Failure: Value is not null<br>Expected: null<br>Actual:   Microsoft.EntityFrameworkCore.DbUpdateException: Lỗi ghi giả lập tại Save<br>   at EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService.<>c__DisplayClass29_0.<<TaoYeuCauHuyAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Services\KetNoiService.KetThuc.cs:line 45<br>--- End of stack trace from previous location ---<br>   at EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService.<>c__DisplayClass29_0.<<TaoYeuCauHuyAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Services\KetNoiService.KetThuc.cs:line 47<br>--- End of stack trace from previous location ---<br>   at EduConnect.Mvc.Infrastructure.KetQua`1.ThucHienAsync(Func`1 action) in C:\Khóa luận TN\Educonnect\Infrastructure\KetQua.cs:line 24<br>   at EduConnect.KetThucWorker.UnitTests.KetThucWorkerTest.Run(Int32 n) in C:\Khóa luận TN\Educonnect\tests\Unit\KetThucWorkerRunner.cs:line 59`.

**Vị trí code:** `Infrastructure/KetQua.cs:20`.

**Nguyên nhân:** KetQua<T>.ThucHienAsync chỉ bắt LoiNghiepVu, DbUpdateConcurrencyException và một số SqlException. DbUpdateException thường không được bắt nên exception từ Save thoát lên caller.

**Ảnh hưởng:** Caller trông đợi KetQua phải xử lý exception ngoài hợp đồng. Không kết luận rằng SQL đã lưu một phần từ fixture mock này.

**Đề xuất sửa:** Bổ sung xử lý DbUpdateException tại ranh giới nghiệp vụ phù hợp, trả KetQua 500 với thông điệp an toàn; giữ lỗi concurrency 409 riêng.

### UT-243 — Đồng ý trước hạn

**Đơn vị:** `KetNoiService.PhanHoiHuyAsync(...)`.

**Fixture và cách tái hiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. now=HetHanLuc-1 giây; dongY=true; đúng người nhận, RV1.

**Kỳ vọng:** yc=ChapNhan, PhanHoiLuc=now, NguoiPhanHoiId=actor; lớp=DaKetThuc, KetThucLuc=now, CachKetThuc=DongY; outbox PhanHoiKetThucLop; Save rồi Commit.

**Thực tế:** ThanhCong=True; mã=200; thông điệp=null. Begin=1; Save=1; Commit=1; DisposeTx=1; yêu cầu thêm=0; notice/outbox=1/1; forward=0; ticks=0; warning=0.

**Các sai lệch và phần đã đạt:**

- Kỳ vọng LoaiSuKien=PhanHoiKetThucLop; thực tế KetNoi.
- Yêu cầu ChapNhan, người/giờ phản hồi, lớp DaKetThuc/KetThucLuc=T/CachKetThuc=DongY và thứ tự Save/Commit đã đạt.

**Assertion đầu tiên:** `Assert.Equal() Failure: Strings differ<br>           ↓ (pos 0)<br>Expected: "PhanHoiKetThucLop"<br>Actual:   "KetNoi"<br>           ↑ (pos 0)`.

**Vị trí code:** `Areas/KetNoi/Services/KetNoiService.KetThuc.cs:74`.

**Nguyên nhân:** PhanHoiHuyAsync tạo notice/outbox bằng notices.Tao(...,"KetNoi",maLop,...), không tạo sự kiện PhanHoiKetThucLop như thiết kế.

**Ảnh hưởng:** Hợp đồng outbox không đúng dù đối tượng nghiệp vụ được cập nhật đúng trong fixture. Chưa kiểm việc consumer đọc sự kiện thật.

**Đề xuất sửa:** Bổ sung sự kiện PhanHoiKetThucLop với payload đã thống nhất, giữ cùng transaction với phản hồi/lớp.

### UT-244 — Từ chối trước hạn

**Đơn vị:** `KetNoiService.PhanHoiHuyAsync(...)`.

**Fixture và cách tái hiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. now=HetHanLuc-1 giây; dongY=false; điều kiện hợp lệ.

**Kỳ vọng:** yc=TuChoi và ghi người/giờ phản hồi; trạng thái lớp giữ nguyên; outbox đúng; không đặt ngày kết thúc lớp.

**Thực tế:** ThanhCong=True; mã=200; thông điệp=null. Begin=1; Save=1; Commit=1; DisposeTx=1; yêu cầu thêm=0; notice/outbox=1/1; forward=0; ticks=0; warning=0.

**Các sai lệch và phần đã đạt:**

- Kỳ vọng PhanHoiKetThucLop; thực tế KetNoi.
- Yêu cầu TuChoi, người/giờ phản hồi đúng; lớp vẫn hoạt động, không đặt ngày kết thúc; Save rồi Commit đạt.

**Assertion đầu tiên:** `Assert.Equal() Failure: Strings differ<br>           ↓ (pos 0)<br>Expected: "PhanHoiKetThucLop"<br>Actual:   "KetNoi"<br>           ↑ (pos 0)`.

**Vị trí code:** `Areas/KetNoi/Services/KetNoiService.KetThuc.cs:74`.

**Nguyên nhân:** Nhánh từ chối dùng cùng notices.Tao(...,"KetNoi",maLop,...) nên outbox có LoaiSuKien=KetNoi.

**Ảnh hưởng:** Thông tin phản hồi không có đúng tên sự kiện mà hợp đồng thiết kế yêu cầu; không phải lỗi đóng lớp khi bị từ chối.

**Đề xuất sửa:** Đồng bộ hợp đồng sự kiện phản hồi như nhánh đồng ý, bảo toàn trạng thái lớp khi từ chối.

### UT-246 — Lỗi ghi dữ liệu

**Đơn vị:** `KetNoiService.PhanHoiHuyAsync(...)`.

**Fixture và cách tái hiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng lưu ném DbUpdateException.

**Kỳ vọng:** Rollback; KetQua 500; không commit.

**Thực tế:** Ném Microsoft.EntityFrameworkCore.DbUpdateException: Lỗi ghi giả lập tại Save. Begin=1; Save=1; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=1/1; forward=0; ticks=0; warning=0.

**Các sai lệch và phần đã đạt:**

- Kỳ vọng KetQua lỗi 500; thực tế DbUpdateException từ Save thoát ra ngoài, không có KetQua.
- Không Commit; transaction dispose. Đối tượng yêu cầu/lớp và notice/outbox trong RAM đã thay đổi trước Save; không có bằng chứng ghi SQL trong unit.

**Assertion đầu tiên:** `Assert.Null() Failure: Value is not null<br>Expected: null<br>Actual:   Microsoft.EntityFrameworkCore.DbUpdateException: Lỗi ghi giả lập tại Save<br>   at EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService.<>c__DisplayClass30_0.<<PhanHoiHuyAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Services\KetNoiService.KetThuc.cs:line 75<br>--- End of stack trace from previous location ---<br>   at EduConnect.Mvc.Areas.KetNoi.Services.KetNoiService.<>c__DisplayClass30_0.<<PhanHoiHuyAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\Areas\KetNoi\Services\KetNoiService.KetThuc.cs:line 76<br>--- End of stack trace from previous location ---<br>   at EduConnect.Mvc.Infrastructure.KetQua.<>c__DisplayClass12_0.<<ThucHienAsync>b__0>d.MoveNext() in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 22<br>--- End of stack trace from previous location ---<br>   at EduConnect.Mvc.Infrastructure.KetQua`1.ThucHienAsync(Func`1 action) in C:\Khóa luận TN\Educonnect\Infrastructure\KetQua.cs:line 24<br>   at EduConnect.Mvc.Infrastructure.KetQua.ThucHienAsync(Func`1 action) in C:\Khóa luận TN\Educonnect\Infrastructure\KetQuaKhongDuLieu.cs:line 20<br>   at EduConnect.KetThucWorker.UnitTests.KetThucWorkerTest.Run(Int32 n) in C:\Khóa luận TN\Educonnect\tests\Unit\KetThucWorkerRunner.cs:line 60`.

**Vị trí code:** `Infrastructure/KetQua.cs:20`.

**Nguyên nhân:** KetQua<T>.ThucHienAsync chỉ bắt LoiNghiepVu, DbUpdateConcurrencyException và một số SqlException. DbUpdateException thường không được bắt nên exception từ Save thoát lên caller.

**Ảnh hưởng:** Caller trông đợi KetQua phải xử lý exception ngoài hợp đồng. Không kết luận rằng SQL đã lưu một phần từ fixture mock này.

**Đề xuất sửa:** Bổ sung xử lý DbUpdateException tại ranh giới nghiệp vụ phù hợp, trả KetQua 500 với thông điệp an toàn; giữ lỗi concurrency 409 riêng.

## Các bước từng ca

### UT-221 — Chuyển tiếp thành công

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `LopHocService.XacNhanKetThucAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Mock PhanHoiHuyAsync trả thành công.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Forward đúng mã yêu cầu, dongY=true, RV yêu cầu, actor và ct đúng một lần; trả cùng instance KetQua; giữ mã/thông điệp khi lỗi; không ghi độc lập.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Gọi đúng mã yêu cầu, dongY=true, RV yêu cầu, actor, ct một lần; trả nguyên kết quả.
- Thực tế: ThanhCong=True; mã=200; thông điệp=null. Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=1; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_221`.

### UT-222 — Chuyển tiếp lỗi

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `LopHocService.XacNhanKetThucAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Mock PhanHoiHuyAsync trả lỗi 409.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Forward đúng mã yêu cầu, dongY=true, RV yêu cầu, actor và ct đúng một lần; trả cùng instance KetQua; giữ mã/thông điệp khi lỗi; không ghi độc lập.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Giữ nguyên MaLoi/ThongDiep; không ghi độc lập hoặc tạo mốc xác nhận thứ hai.
- Thực tế: ThanhCong=False; mã=409; thông điệp="Xung đột mẫu". Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=1; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_222`.

### UT-223 — Token đã hủy

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `LopHocService.XacNhanKetThucAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. ct đã CancellationRequested trước lời gọi.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Token đã hủy phải ném OperationCanceledException trước lời gọi dependency; Verify PhanHoiHuyAsync Times.Never.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: OperationCanceledException; không gọi PhanHoiHuyAsync.
- Thực tế: ThanhCong=True; mã=200; thông điệp=null. Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=1; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_223`.

### UT-224 — Lý do chỉ khoảng trắng

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.TaoYeuCauHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. lyDo=" ".
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Lỗi 400, Begin=0 và không Add/Save/Commit/notice/outbox.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: KetQua lỗi 400; không mở giao dịch.
- Thực tế: ThanhCong=False; mã=400; thông điệp="Nhập lý do kết thúc tối đa 2000 ký tự.". Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_224`.

### UT-225 — Lý do vượt ngưỡng

- Tiêu chuẩn: Đầy đủ.
- Đích kiểm: `KetNoiService.TaoYeuCauHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. lyDo gồm 2001 ký tự.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Lỗi 400, Begin=0 và không Add/Save/Commit/notice/outbox.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: KetQua lỗi 400; không ghi.
- Thực tế: ThanhCong=False; mã=400; thông điệp="Nhập lý do kết thúc tối đa 2000 ký tự.". Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_225`.

### UT-226 — Lý do đúng ngưỡng

- Tiêu chuẩn: Đầy đủ.
- Đích kiểm: `KetNoiService.TaoYeuCauHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. lyDo gồm 2000 ký tự; lớp/quyền/RV hợp lệ.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Yêu cầu mới có lý do trim/độ dài đúng, đúng lớp/người gửi, ChoPhanHoi, GuiLuc=T và HetHanLuc=T+72h; lớp giữ nguyên; đúng người nhận; Save trước Commit. UT-232 kiểm thêm loại/payload outbox theo bảng.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Qua kiểm tra độ dài; tạo yêu cầu.
- Thực tế: ThanhCong=True; mã=200; thông điệp=null. Trả mã yêu cầu 650cc558-afac-46d8-9d47-b3e59e6156b4. Begin=1; Save=1; Commit=1; DisposeTx=1; yêu cầu thêm=1; notice/outbox=1/1; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_226`.

### UT-227 — Không có lớp

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.TaoYeuCauHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng đọc khóa lớp trả null.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Đúng mã lỗi kỳ vọng; không Save/Commit, không thêm yêu cầu/notice/outbox; trạng thái lớp và yêu cầu không bị sửa; mọi transaction đã mở được dispose.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 404.
- Thực tế: ThanhCong=False; mã=404; thông điệp="Không tìm thấy lớp.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_227`.

### UT-228 — Không là thành viên chính

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.TaoYeuCauHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Actor U3 khác GiaSuId và BenHocId.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Đúng mã lỗi kỳ vọng; không Save/Commit, không thêm yêu cầu/notice/outbox; trạng thái lớp và yêu cầu không bị sửa; mọi transaction đã mở được dispose.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 403.
- Thực tế: ThanhCong=False; mã=403; thông điệp="Chỉ hai bên chính được yêu cầu kết thúc lớp.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_228`.

### UT-229 — Lớp không hoạt động

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.TaoYeuCauHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Lớp DaKetThuc; actor đúng thành viên chính.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Đúng mã lỗi kỳ vọng; không Save/Commit, không thêm yêu cầu/notice/outbox; trạng thái lớp và yêu cầu không bị sửa; mọi transaction đã mở được dispose.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 403.
- Thực tế: ThanhCong=False; mã=409; thông điệp="Lớp không còn hoạt động.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_229`.

### UT-230 — Đã có yêu cầu chờ

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.TaoYeuCauHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Có YeuCauHuyKetNoi ChoPhanHoi của lớp.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Đúng mã lỗi kỳ vọng; không Save/Commit, không thêm yêu cầu/notice/outbox; trạng thái lớp và yêu cầu không bị sửa; mọi transaction đã mở được dispose.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 409; không thêm yêu cầu thứ hai.
- Thực tế: ThanhCong=False; mã=409; thông điệp="Đang có yêu cầu kết thúc chờ phản hồi.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_230`.

### UT-231 — Phiên bản lớp cũ

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.TaoYeuCauHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. lop.RowVersion=RV1; input RV0.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Đúng mã lỗi kỳ vọng; không Save/Commit, không thêm yêu cầu/notice/outbox; trạng thái lớp và yêu cầu không bị sửa; mọi transaction đã mở được dispose.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 409.
- Thực tế: ThanhCong=False; mã=409; thông điệp="Lớp vừa thay đổi. Hãy tải lại.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_231`.

### UT-232 — Tạo yêu cầu hợp lệ

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.TaoYeuCauHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Lý do=" Xin kết thúc "; lớp hoạt động; thành viên chính đúng; RV1; không yêu cầu chờ; nguồn giờ trả T.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Yêu cầu mới có lý do trim/độ dài đúng, đúng lớp/người gửi, ChoPhanHoi, GuiLuc=T và HetHanLuc=T+72h; lớp giữ nguyên; đúng người nhận; Save trước Commit. UT-232 kiểm thêm loại/payload outbox theo bảng.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Tạo yc có lý do trim, ChoPhanHoi, GuiLuc=T, HetHanLuc=T+72h, người gửi đúng; outbox YeuCauKetThucLop chứa yc.Id; lớp vẫn hoạt động; Save rồi Commit; trả yc.Id.
- Thực tế: ThanhCong=True; mã=200; thông điệp=null. Trả mã yêu cầu eef7987d-1914-4188-a058-4d0185697f8c. Begin=1; Save=1; Commit=1; DisposeTx=1; yêu cầu thêm=1; notice/outbox=1/1; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_232`.

### UT-233 — Lỗi concurrency khi lưu

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.TaoYeuCauHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng Save ném DbUpdateConcurrencyException.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Save giả lập ném đúng exception; mong đợi 409 concurrency hoặc 500 DbUpdateException; không Commit và transaction được dispose. Không yêu cầu các đối tượng RAM tự hoàn tác; chưa kiểm rollback SQL.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 409; không commit.
- Thực tế: ThanhCong=False; mã=409; thông điệp="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.". Begin=1; Save=1; Commit=0; DisposeTx=1; yêu cầu thêm=1; notice/outbox=1/1; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_233`.

### UT-234 — Lỗi cập nhật dữ liệu

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.TaoYeuCauHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng Save ném DbUpdateException.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Save giả lập ném đúng exception; mong đợi 409 concurrency hoặc 500 DbUpdateException; không Commit và transaction được dispose. Không yêu cầu các đối tượng RAM tự hoàn tác; chưa kiểm rollback SQL.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 500; không commit.
- Thực tế: Ném Microsoft.EntityFrameworkCore.DbUpdateException: Lỗi ghi giả lập tại Save. Begin=1; Save=1; Commit=0; DisposeTx=1; yêu cầu thêm=1; notice/outbox=1/1; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_234`.

### UT-235 — Không có yêu cầu

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.PhanHoiHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Nguồn đọc ban đầu trả null.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Đúng mã lỗi kỳ vọng; không Save/Commit, không thêm yêu cầu/notice/outbox; trạng thái lớp và yêu cầu không bị sửa; mọi transaction đã mở được dispose.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 404.
- Thực tế: ThanhCong=False; mã=404; thông điệp="Không tìm thấy yêu cầu.". Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_235`.

### UT-236 — Người gửi tự phản hồi

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.PhanHoiHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Actor=yc.NguoiGuiId.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Đúng mã lỗi kỳ vọng; không Save/Commit, không thêm yêu cầu/notice/outbox; trạng thái lớp và yêu cầu không bị sửa; mọi transaction đã mở được dispose.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 403.
- Thực tế: ThanhCong=False; mã=403; thông điệp="Chỉ bên nhận được phản hồi yêu cầu.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_236`.

### UT-237 — Người ngoài phản hồi

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.PhanHoiHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Actor không là người nhận xác định từ lớp.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Đúng mã lỗi kỳ vọng; không Save/Commit, không thêm yêu cầu/notice/outbox; trạng thái lớp và yêu cầu không bị sửa; mọi transaction đã mở được dispose.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 403.
- Thực tế: ThanhCong=False; mã=403; thông điệp="Chỉ bên nhận được phản hồi yêu cầu.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_237`.

### UT-238 — Yêu cầu đã xử lý

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.PhanHoiHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. yc.TrangThai=TuChoi hoặc ChapNhan.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Đúng mã lỗi kỳ vọng; không Save/Commit, không thêm yêu cầu/notice/outbox; trạng thái lớp và yêu cầu không bị sửa; mọi transaction đã mở được dispose.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 409; không ghi đè.
- Thực tế: ThanhCong=False; mã=409; thông điệp="Yêu cầu đã xử lý, hết hạn hoặc thay đổi.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_238`.

### UT-239 — Phiên bản yêu cầu cũ

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.PhanHoiHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. yc.RowVersion=RV1, input RV0; phiên bản lớp không liên quan.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Đúng mã lỗi kỳ vọng; không Save/Commit, không thêm yêu cầu/notice/outbox; trạng thái lớp và yêu cầu không bị sửa; mọi transaction đã mở được dispose.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 409.
- Thực tế: ThanhCong=False; mã=409; thông điệp="Yêu cầu đã xử lý, hết hạn hoặc thay đổi.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_239`.

### UT-240 — Dùng phiên bản lớp thay phiên bản yêu cầu

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.PhanHoiHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. RV lớp khác RV yêu cầu; input bằng RV lớp.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Đúng mã lỗi kỳ vọng; không Save/Commit, không thêm yêu cầu/notice/outbox; trạng thái lớp và yêu cầu không bị sửa; mọi transaction đã mở được dispose.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: 409; không chấp nhận nhầm.
- Thực tế: ThanhCong=False; mã=409; thông điệp="Yêu cầu đã xử lý, hết hạn hoặc thay đổi.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_240`.

### UT-241 — Đúng hạn

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.PhanHoiHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. now=HetHanLuc.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Đúng mã lỗi kỳ vọng; không Save/Commit, không thêm yêu cầu/notice/outbox; trạng thái lớp và yêu cầu không bị sửa; mọi transaction đã mở được dispose.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 409 vì phản hồi yêu cầu now < hạn.
- Thực tế: ThanhCong=False; mã=409; thông điệp="Yêu cầu đã xử lý, hết hạn hoặc thay đổi.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_241`.

### UT-242 — Sau hạn

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.PhanHoiHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. now=HetHanLuc+1 giây.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Đúng mã lỗi kỳ vọng; không Save/Commit, không thêm yêu cầu/notice/outbox; trạng thái lớp và yêu cầu không bị sửa; mọi transaction đã mở được dispose.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 409.
- Thực tế: ThanhCong=False; mã=409; thông điệp="Yêu cầu đã xử lý, hết hạn hoặc thay đổi.". Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_242`.

### UT-243 — Đồng ý trước hạn

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.PhanHoiHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. now=HetHanLuc-1 giây; dongY=true; đúng người nhận, RV1.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Đúng phiên bản yêu cầu; đúng người/giờ phản hồi và trạng thái ChapNhan/TuChoi; đồng ý thì kết thúc lớp với DongY, từ chối giữ nguyên lớp; đúng recipient; Save trước Commit; sự kiện PhanHoiKetThucLop.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: yc=ChapNhan, PhanHoiLuc=now, NguoiPhanHoiId=actor; lớp=DaKetThuc, KetThucLuc=now, CachKetThuc=DongY; outbox PhanHoiKetThucLop; Save rồi Commit.
- Thực tế: ThanhCong=True; mã=200; thông điệp=null. Begin=1; Save=1; Commit=1; DisposeTx=1; yêu cầu thêm=0; notice/outbox=1/1; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_243`.

### UT-244 — Từ chối trước hạn

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.PhanHoiHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. now=HetHanLuc-1 giây; dongY=false; điều kiện hợp lệ.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Đúng phiên bản yêu cầu; đúng người/giờ phản hồi và trạng thái ChapNhan/TuChoi; đồng ý thì kết thúc lớp với DongY, từ chối giữ nguyên lớp; đúng recipient; Save trước Commit; sự kiện PhanHoiKetThucLop.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: yc=TuChoi và ghi người/giờ phản hồi; trạng thái lớp giữ nguyên; outbox đúng; không đặt ngày kết thúc lớp.
- Thực tế: ThanhCong=True; mã=200; thông điệp=null. Begin=1; Save=1; Commit=1; DisposeTx=1; yêu cầu thêm=0; notice/outbox=1/1; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_244`.

### UT-245 — Lỗi concurrency

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.PhanHoiHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng lưu ném DbUpdateConcurrencyException.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Save giả lập ném đúng exception; mong đợi 409 concurrency hoặc 500 DbUpdateException; không Commit và transaction được dispose. Không yêu cầu các đối tượng RAM tự hoàn tác; chưa kiểm rollback SQL.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 409; không commit.
- Thực tế: ThanhCong=False; mã=409; thông điệp="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.". Begin=1; Save=1; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=1/1; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_245`.

### UT-246 — Lỗi ghi dữ liệu

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `KetNoiService.PhanHoiHuyAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng lưu ném DbUpdateException.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Save giả lập ném đúng exception; mong đợi 409 concurrency hoặc 500 DbUpdateException; không Commit và transaction được dispose. Không yêu cầu các đối tượng RAM tự hoàn tác; chưa kiểm rollback SQL.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Rollback; KetQua 500; không commit.
- Thực tế: Ném Microsoft.EntityFrameworkCore.DbUpdateException: Lỗi ghi giả lập tại Save. Begin=1; Save=1; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=1/1; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_246`.

### UT-247 — Token hủy ngay

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `BaoTriWorker.ExecuteAsync(huy)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Token đã hủy trước khi vào vòng xử lý.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Token hủy trước ExecuteAsync: không gọi bất kỳ processor nào và không chờ tick mới.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Kết thúc không chạy vòng quét nghiệp vụ mới.
- Thực tế: ThanhCong=True; mã=0; thông điệp=null. Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_247`.

### UT-248 — Một vòng bình thường

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `BaoTriWorker.ExecuteAsync(huy)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Timer giả cung cấp một tick rồi dừng; hai bộ xử lý thành công.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Một vòng có thứ tự Expire → AutoEnd → Reminders → Needs → Feedback; timer giả dừng ở lần chờ đầu; dispose timer; không DB.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Gọi xử lý hết hạn yêu cầu và tự kết thúc lớp theo thứ tự; không lặp vô hạn.
- Thực tế: ThanhCong=True; mã=0; thông điệp=null. Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=1; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_248`.

### UT-249 — Hủy trong xử lý

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `BaoTriWorker.ExecuteAsync(huy)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Bộ xử lý ném OperationCanceledException khi token dừng.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Processor hủy token và ném OperationCanceledException; vòng lặp kết thúc, không AutoEnd/chờ tick và không LogWarning.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Worker kết thúc; không coi là lỗi nghiệp vụ để tiếp tục vòng vô hạn.
- Thực tế: ThanhCong=True; mã=0; thông điệp=null. Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_249`.

### UT-250 — Lỗi ngoài hủy

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `BaoTriWorker.ExecuteAsync(huy)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Bộ xử lý một vòng ném ngoại lệ; timer cho phép vòng tiếp theo.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Lượt đầu lỗi thường: một LogWarning; tick tiếp theo cho chạy lại; Expire hai lần, AutoEnd một lần; timer kết thúc có giới hạn.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Ghi LogWarning; worker có thể xử lý vòng tiếp theo thay vì chết.
- Thực tế: ThanhCong=True; mã=0; thông điệp=null. Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=2; warning=1.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_250`.

### UT-251 — Không có ứng viên

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `BaoTriWorker.XuLyTuHuyKetNoiAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng đọc danh sách trả rỗng.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Danh sách rỗng không mở transaction; hoặc đọc lại sau khóa thấy chưa đến hạn/đã phản hồi/lớp kết thúc thì không Save/Commit/notice/outbox và giữ nguyên hai đối tượng.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Không bắt đầu xử lý/ghi từng yêu cầu.
- Thực tế: ThanhCong=True; mã=0; thông điệp=null. Begin=0; Save=0; Commit=0; DisposeTx=0; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_251`.

### UT-252 — Chưa đến hạn sau khóa

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `BaoTriWorker.XuLyTuHuyKetNoiAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Danh sách sơ bộ có Id; đọc lại giờ SQL now < hạn.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Danh sách rỗng không mở transaction; hoặc đọc lại sau khóa thấy chưa đến hạn/đã phản hồi/lớp kết thúc thì không Save/Commit/notice/outbox và giữ nguyên hai đối tượng.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Bỏ qua Id; không tự chấp nhận và không thông báo.
- Thực tế: ThanhCong=True; mã=0; thông điệp=null. Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_252`.

### UT-253 — Vừa được phản hồi

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `BaoTriWorker.XuLyTuHuyKetNoiAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Ứng viên ban đầu chờ; sau khóa trạng thái TuChoi.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Danh sách rỗng không mở transaction; hoặc đọc lại sau khóa thấy chưa đến hạn/đã phản hồi/lớp kết thúc thì không Save/Commit/notice/outbox và giữ nguyên hai đối tượng.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Bỏ qua; không ghi đè phản hồi của người dùng.
- Thực tế: ThanhCong=True; mã=0; thông điệp=null. Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_253`.

### UT-254 — Lớp đã kết thúc

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `BaoTriWorker.XuLyTuHuyKetNoiAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Yêu cầu còn chờ nhưng lớp không hoạt động.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Danh sách rỗng không mở transaction; hoặc đọc lại sau khóa thấy chưa đến hạn/đã phản hồi/lớp kết thúc thì không Save/Commit/notice/outbox và giữ nguyên hai đối tượng.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Bỏ qua; không thay đổi cách kết thúc đã có.
- Thực tế: ThanhCong=True; mã=0; thông điệp=null. Begin=1; Save=0; Commit=0; DisposeTx=1; yêu cầu thêm=0; notice/outbox=0/0; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_254`.

### UT-255 — Đúng hạn đủ điều kiện

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `BaoTriWorker.XuLyTuHuyKetNoiAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. now=HetHanLuc; yc còn chờ; lớp đang hoạt động.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Yêu cầu TuDongChapNhan, PhanHoiLuc=T, không người phản hồi; lớp DaKetThuc/KetThucLuc=T/CachKetThuc=HetHan; đúng hai bên nhận và target lớp; Save trước Commit. Quét lần hai UT-257 không tăng Save/Commit/notice/outbox.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: yc=TuDongChapNhan; PhanHoiLuc=now; lớp=DaKetThuc, KetThucLuc=now, CachKetThuc=HetHan; tạo thông báo cho đúng hai bên; Save rồi Commit.
- Thực tế: ThanhCong=True; mã=0; thông điệp=null. Begin=1; Save=1; Commit=1; DisposeTx=1; yêu cầu thêm=0; notice/outbox=2/2; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_255`.

### UT-256 — Sau hạn đủ điều kiện

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `BaoTriWorker.XuLyTuHuyKetNoiAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. now=HetHanLuc+1 giây; các điều kiện hợp lệ.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Yêu cầu TuDongChapNhan, PhanHoiLuc=T, không người phản hồi; lớp DaKetThuc/KetThucLuc=T/CachKetThuc=HetHan; đúng hai bên nhận và target lớp; Save trước Commit. Quét lần hai UT-257 không tăng Save/Commit/notice/outbox.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Tự chấp nhận và kết thúc như ca đúng hạn; không xóa lớp hoặc lịch sử.
- Thực tế: ThanhCong=True; mã=0; thông điệp=null. Begin=1; Save=1; Commit=1; DisposeTx=1; yêu cầu thêm=0; notice/outbox=2/2; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_256`.

### UT-257 — Quét lặp

- Tiêu chuẩn: Đúng đắn.
- Đích kiểm: `BaoTriWorker.XuLyTuHuyKetNoiAsync(...)`.
- Dữ liệu riêng: Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Sau lần đầu yc đã TuDongChapNhan; chạy vòng xử lý lại.
- B1. Tạo KetThucWorkerFixture mới; dùng bộ H/G/X/C/R/RV/T trên; giữ service/worker thật và khởi tạo mock strict.
- B2. Áp dụng điều kiện riêng: null/quyền/trạng thái/phiên bản/thời hạn hoặc Save exception; cổng NowAsync trả đúng T. Với worker vòng lặp, cấu hình processor và tick dừng có giới hạn.
- B3. Await phương thức thật với input/actor/ct; worker private/protected gọi bằng reflection và timeout bảo vệ 3 giây. Không mở SQL hoặc chạy hosted worker vô hạn.
- B4. Thu KetQua/exception, trạng thái lớp/yêu cầu, mã request, lời gọi cổng, notice/outbox và thứ tự transaction. Ghi OBSERVATION_JSON trước assertion để cả ca Fail vẫn có bằng chứng.
- B5. Assert: Yêu cầu TuDongChapNhan, PhanHoiLuc=T, không người phản hồi; lớp DaKetThuc/KetThucLuc=T/CachKetThuc=HetHan; đúng hai bên nhận và target lớp; Save trước Commit. Quét lần hai UT-257 không tăng Save/Commit/notice/outbox.
- B6. Dispose fixture; đọc trạng thái và error/stack trong TRX. Không chia sẻ mutable state giữa các ca.
- Kỳ vọng Excel: Không tạo thông báo hoặc ghi nhận kết thúc trùng.
- Thực tế: ThanhCong=True; mã=0; thông điệp=null. Begin=2; Save=1; Commit=1; DisposeTx=2; yêu cầu thêm=0; notice/outbox=2/2; forward=0; ticks=0; warning=0.
- Chạy riêng: `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~KetThucWorkerTest.UT_257`.

## File thêm và thay đổi

- tests/Unit/KetThucWorkerTest.cs: 37 Fact và Trait Suite=KetThucWorker; KetThucWorkerRunner.cs: Arrange/Act/Assert và ghi quan sát; KetThucWorkerFixture.cs: mock strict, đồng hồ, đối tượng fixture, thông báo, transaction, logger và timer giả.
- Infrastructure/IClassEndingStore.cs và EfClassEndingStore.cs: cổng lưu trữ/giờ SQL cùng adapter mặc định cho kết thúc lớp và worker.
- Infrastructure/IMaintenanceCycle.cs: dependency vòng xử lý và nguồn tick; PeriodicMaintenanceTicks mặc định vẫn dùng PeriodicTimer 30 giây.
- KetNoiService.cs nhận cổng kết thúc tùy chọn; KetNoiService.KetThuc.cs chuyển thao tác EF qua cổng. LopHocService nhận IKetNoiService thay concrete để kiểm wrapper. BaoTriWorker nhận processor/timer/TimeProvider tùy chọn; mặc định vẫn chạy các xử lý và adapter EF hiện có. Các guard, giá trị trạng thái, mã lỗi và lời gọi thông báo nghiệp vụ không được sửa để làm test đạt.

## Chạy lại và đọc kết quả

Mở PowerShell tại C:\Khóa luận TN\Educonnect. Không cần dotnet run hoặc SQL Server. Dùng output build riêng nếu ứng dụng đang giữ DLL.

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --no-restore --filter "Suite=KetThucWorker" -p:UseAppHost=false "-p:BaseOutputPath=$env:TEMP/EduConnect-Group08-Unit/" --logger "trx;LogFileName=KetThuc-Lop-Worker-Unit.trx" --results-directory "TestResults/08-KetThuc-Lop-Worker-Unit"
```

Bảy assertion Fail khiến dotnet test trả exit code 1; không phải lỗi build hoặc không thể chạy. Chạy lại chỉ tự cập nhật TRX; báo cáo MD/JSON cần được tổng hợp lại từ bằng chứng mới.

## Hồi quy và kiểm tra minh chứng

Chạy toàn project unit sau thay đổi dependency: 257 ca = 142 Pass, 112 Fail, 3 Skip. Đối chiếu đủ 220 mã UT-001–220 với các TRX cũ: không ca nào đổi trạng thái. Các báo cáo trước không bị ghi đè; log hồi quy nằm ở work. Chưa chạy lại integration sau refactor này.

Đủ 37 mã liên tiếp, mỗi mã một Fact; JSON và TRX có cùng trạng thái. Markdown được kiểm số cột. ZIP lưu mã test, adapter, bản nguồn liên quan và diff bốn file sửa so với trước nhóm 08; SHA256 dùng đối chiếu kết quả.
