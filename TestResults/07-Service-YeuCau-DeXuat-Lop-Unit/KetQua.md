# Kết quả 07. Service yêu cầu và đề xuất lớp

**51 ca UT-170–220: 49 Pass, 2 Fail, 0 Skip/Blocked.** Thực thi 123 lượt fixture/service: 48 ca chạy một lượt, ba ca ánh xạ SQL chạy 25 mã mỗi ca.

Nguồn kỳ vọng: `Unit Test.xlsx`, sheet `07. Service yêu cầu và đề xuất `. Giữ đúng mã và thứ tự trong file cập nhật. Ba tiêu chuẩn trong bảng gốc được giữ ở từng ca; các ca kiểm phiên/quyền thuộc xác thực, điều kiện/tham số/kết quả thuộc đúng đắn, nhánh lỗi và chống thiếu dữ liệu theo tiêu chuẩn đầy đủ của từng dòng. Báo cáo này không phải kết luận hệ thống đã đầy đủ mọi nhánh chỉ vì chạy hết 51 dòng.

## Phạm vi và giới hạn của kết quả

Service `KetNoiService` là đối tượng thật. Tầng đọc/ghi `IKetNoiStore`, nguồn người dùng `INguoiDungHienTaiService`, transaction và `IThongBaoData` dùng Moq strict. `ThongBaoService` thật tạo notice/outbox vào mock nhận dữ liệu. AppDbContext không cấu hình provider, không mở kết nối CSDL. Đồng hồ phục vụ điều kiện đề xuất/hết hạn cố định T=2026-11-14 03:00 UTC (10:00 UTC+7). Mỗi lượt có fixture và dữ liệu riêng.

- UT-175–180 kiểm overload private `AcceptedAsync(Guid,bool,Guid,CancellationToken)` bằng reflection vì đó là helper đang có trong code và là đích của TC. Không tạo một bản helper khác trong test; không coi đây là kiểm endpoint public. Các ca khác gọi trực tiếp overload public tương ứng.
- UT-195/198/201 tạo SqlException thật về kiểu dữ liệu nhưng chỉ là exception giả lập trong bộ nhớ, lần lượt 51130–51154. Không gọi procedure.
- **UT-203:** Pass cho phần service nhận lỗi khóa 51152, trả 409 và dừng trước lệnh tạo lớp. Điều kiện “cổng khóa trả số âm” nằm trong chuỗi SQL thực thi; unit này không kiểm được việc SQL Server trả số âm rồi phát sinh 51152. Cần integration test cho phần đó.
- **UT-211/217:** Pass cho truyền RV0, xử lý exception concurrency tại Save và không Commit. Đối tượng theo dõi trong RAM đã đổi TuChoi/DaRut; không có dữ liệu được ghi xuống CSDL trong unit. Việc SQL phát hiện phiên bản cũ và rollback cần integration test.
- Ca tạo yêu cầu/đề xuất/lớp thành công chỉ xác nhận service phát đúng lệnh, đọc đúng kết quả giả lập, thêm notice/outbox và gọi Save trước Commit. Không xác nhận procedure đã tạo bản ghi lớp hoặc lịch học thực tế.

Cách cô lập dependency và Arrange–Act–Assert dựa trên [Microsoft: unit testing best practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices) và mẫu mock dependency của [Microsoft: testing controller logic in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/testing?view=aspnetcore-10.0). Việc gọi helper private bằng reflection là lựa chọn cụ thể cho TC này, không phải yêu cầu của Microsoft.

## Hai ca thất bại

### UT-175 — Liên hệ không tồn tại

**Loại lỗi:** Sai mã lỗi đối với liên hệ không tồn tại.

**Dữ liệu và tái hiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng đọc liên hệ trả null.

Tạo fixture mới, giữ actor H là bên học, gọi helper thật qua overload có kiểm actor. Cổng đọc liên hệ không có LH1 (Contact=null).

**Kỳ vọng:** KetQua lỗi 404.

**Thực tế:** KetQua.ThanhCong=false, mã=403, thông điệp="Cần liên hệ được chấp nhận trước.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.

**Sai lệch quan sát:**

- Kỳ vọng KetQua lỗi 404, thực tế lỗi 403.
- Thông điệp thực tế “Cần liên hệ được chấp nhận trước.” gộp tình huống không tồn tại với chưa được chấp nhận hoặc sai phía.
- Không ghi dữ liệu/commit: đạt; sai lệch ở mã phản hồi, không phải đã ghi nhầm dữ liệu.

**Assertion thất bại đầu tiên:** `Assert.Equal() Failure: Values differ / Expected: 404 / Actual:   403`.

**Code liên quan:** `Areas/KetNoi/Services/KetNoiService.cs:101`.

**Nguyên nhân:** AcceptedAsync dùng một truy vấn kết hợp Id + TrangThai==ChapNhan + bên tham gia. Khi kết quả null, mọi nguyên nhân đều đi vào cùng LoiNghiepVu(...,403). Nhánh không tồn tại không được phân biệt thành 404 như TC.

**Ảnh hưởng:** Caller không phân biệt được liên hệ không tồn tại với liên hệ không đủ quyền. Đây là sai lệch hợp đồng lỗi của TC; không phải lỗi crash hoặc thao tác ghi.

**Đề xuất sửa:** Đọc theo Id trước; nếu null trả 404; sau đó kiểm bên tham gia/trạng thái và trả 403. Chỉ đổi sau khi chốt yêu cầu 404 trong thiết kế, vì lựa chọn gộp 403 có thể là chủ ý hạn chế lộ thông tin.

### UT-179 — Một tài khoản bị khóa

**Loại lỗi:** Thiếu kiểm trạng thái khóa tài khoản ở helper AcceptedAsync.

**Dữ liệu và tái hiện:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Liên hệ đã chấp nhận, đúng bên; có tài khoản BiKhoa.

Tạo fixture mới, giữ actor H là bên học, gọi helper thật qua overload có kiểm actor. Contact LH1 có GiaSuId=G, BenHocId=H, TrangThai=ChapNhan; nguồn tài khoản giả lập có G.TrangThai=BiKhoa và H.HoatDong.

**Kỳ vọng:** KetQua lỗi 403.

**Thực tế:** KetQua.ThanhCong=true, mã=200, thông điệp=null. Liên hệ trả về=40000000-0000-0000-0000-000000000004. Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.

**Sai lệch quan sát:**

- Kỳ vọng KetQua lỗi 403, thực tế ThanhCong=true, mã 200, DuLieu=LH1.
- Fixture có tài khoản gia sư G.TrangThai=BiKhoa, liên hệ LH1 vẫn ChapNhan và actor H đúng bên học.
- AccountReads=0: helper không hỏi trạng thái tài khoản của hai phía.
- Helper này không ghi dữ liệu: Save/Commit/notice/outbox đều 0. Không kết luận từ unit này rằng toàn bộ hệ thống hoặc procedure cũng bỏ qua trạng thái khóa.

**Assertion thất bại đầu tiên:** `Phải trả KetQua lỗi; thực tế thành công.`.

**Code liên quan:** `Areas/KetNoi/Services/KetNoiService.cs:97`.

**Nguyên nhân:** AcceptedAsync chỉ kiểm vai trò actor, liên hệ đã ChapNhan và Id bên tham gia. NguoiThamGia không chứa TrangThai tài khoản; helper không đọc TaiKhoan hay gọi cổng kiểm trạng thái hai phía. Trạng thái BiKhoa trong nguồn giả lập không được code sử dụng.

**Ảnh hưởng:** Điều kiện “không tài khoản bị khóa” của bộ TC chưa được thực thi tại helper service này. Cần kiểm riêng các lớp bảo vệ khác trước khi kết luận mức ảnh hưởng khi chạy HTTP/SQL thật.

**Đề xuất sửa:** Bổ sung dependency đọc trạng thái tài khoản hai phía hoặc policy chuyên trách; từ chối 403 nếu một bên bị khóa, trước khi trả liên hệ hoặc thực hiện ghi. Giữ unit kiểm cả trường hợp gia sư và bên học bị khóa khi sửa.

## Kết quả từng ca

| Mã | Kịch bản | Tiêu chuẩn | Kết quả | Quan sát thực tế |
| --- | --- | --- | --- | --- |
| UT-170 | Người thực hiện không khớp phiên | Xác thực | Pass | KetQua.ThanhCong=false, mã=403, thông điệp="Không có quyền thực hiện thao tác.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-171 | Người thực hiện không khớp phiên | Xác thực | Pass | KetQua.ThanhCong=false, mã=403, thông điệp="Không có quyền thực hiện thao tác.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-172 | Người thực hiện không khớp phiên | Xác thực | Pass | KetQua.ThanhCong=false, mã=403, thông điệp="Không có quyền thực hiện thao tác.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-173 | Người thực hiện không khớp phiên | Xác thực | Pass | KetQua.ThanhCong=false, mã=403, thông điệp="Không có quyền thực hiện thao tác.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-174 | Người thực hiện không khớp phiên | Xác thực | Pass | KetQua.ThanhCong=false, mã=403, thông điệp="Không có quyền thực hiện thao tác.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-175 | Liên hệ không tồn tại | Đúng đắn | Fail | KetQua.ThanhCong=false, mã=403, thông điệp="Cần liên hệ được chấp nhận trước.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-176 | Sai bên gia sư | Xác thực | Pass | KetQua.ThanhCong=false, mã=403, thông điệp="Cần liên hệ được chấp nhận trước.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-177 | Sai bên học | Xác thực | Pass | KetQua.ThanhCong=false, mã=403, thông điệp="Cần liên hệ được chấp nhận trước.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-178 | Chưa được chấp nhận | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=403, thông điệp="Cần liên hệ được chấp nhận trước.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-179 | Một tài khoản bị khóa | Đúng đắn | Fail | KetQua.ThanhCong=true, mã=200, thông điệp=null. Liên hệ trả về=40000000-0000-0000-0000-000000000004. Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-180 | Liên hệ hợp lệ | Đúng đắn | Pass | KetQua.ThanhCong=true, mã=200, thông điệp=null. Liên hệ trả về=40000000-0000-0000-0000-000000000004. Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-181 | Phân giải liên hệ từ đối tượng | Đúng đắn | Pass | KetQua.ThanhCong=true, mã=200, thông điệp=null. DuLieu=50000000-0000-0000-0000-000000000005. Lệnh nghiệp vụ=1; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-182 | Gửi thành công | Đúng đắn | Pass | KetQua.ThanhCong=true, mã=200, thông điệp=null. DuLieu=50000000-0000-0000-0000-000000000005. Lệnh nghiệp vụ=1; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-183 | Phí không hợp lệ | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=400, thông điệp="Học phí không âm, tối đa hai chữ số thập phân; chọn đúng đơn vị.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-184 | Khung giờ không hợp lệ | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=400, thông điệp="Thứ hoặc giờ của khung giờ không hợp lệ.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-185 | Số khung không bằng số buổi | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=400, thông điệp="Số khung giờ và thời lượng phải khớp điều kiện lớp.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-186 | Thời lượng không khớp | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=400, thông điệp="Số khung giờ và thời lượng phải khớp điều kiện lớp.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-187 | Ngày bắt đầu không ở tương lai | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=400, thông điệp="Ngày bắt đầu phải ở tương lai.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-188 | Trực tiếp thiếu địa điểm | Đầy đủ | Pass | KetQua.ThanhCong=false, mã=400, thông điệp="Nhập địa điểm học trực tiếp.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-189 | Nhu cầu sai chủ | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=400, thông điệp="Nhu cầu phải thuộc bên học và đang mở.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-190 | Nhu cầu không mở | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=400, thông điệp="Nhu cầu phải thuộc bên học và đang mở.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-191 | Tạo đề xuất mới | Đúng đắn | Pass | KetQua.ThanhCong=true, mã=200, thông điệp=null. DuLieu=50000000-0000-0000-0000-000000000005. Lệnh nghiệp vụ=1; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-192 | Cập nhật đề xuất | Đúng đắn | Pass | KetQua.ThanhCong=true, mã=200, thông điệp=null. DuLieu=50000000-0000-0000-0000-000000000005. Lệnh nghiệp vụ=1; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-193 | Ánh xạ lỗi concurrency | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=409, thông điệp="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.". Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-194 | Lỗi ngoài dự kiến | Đúng đắn | Pass | Ngoại lệ thoát ra: System.InvalidOperationException: Hạ tầng lỗi giả lập. Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-195 | Ánh xạ mã lỗi SQL đã biết | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=400, thông điệp="SQL giả lập 51130". Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-196 | Ánh xạ lỗi concurrency | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=409, thông điệp="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.". Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-197 | Lỗi ngoài dự kiến | Đúng đắn | Pass | Ngoại lệ thoát ra: System.InvalidOperationException: Hạ tầng lỗi giả lập. Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-198 | Ánh xạ mã lỗi SQL đã biết | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=400, thông điệp="SQL giả lập 51130". Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-199 | Ánh xạ lỗi concurrency | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=409, thông điệp="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.". Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-200 | Lỗi ngoài dự kiến | Đúng đắn | Pass | Ngoại lệ thoát ra: System.InvalidOperationException: Hạ tầng lỗi giả lập. Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-201 | Ánh xạ mã lỗi SQL đã biết | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=400, thông điệp="SQL giả lập 51130". Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-202 | RV không đủ 8 byte | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=400, thông điệp="Phiên bản không hợp lệ.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-203 | Khóa ứng dụng thất bại | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=409, thông điệp="SQL giả lập 51152". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-204 | Sai bên học | Xác thực | Pass | KetQua.ThanhCong=false, mã=403, thông điệp="Chỉ bên học được chấp nhận đề xuất.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-205 | Đã chấp nhận trước đó | Đúng đắn | Pass | KetQua.ThanhCong=true, mã=200, thông điệp=null. DuLieu=60000000-0000-0000-0000-000000000006. Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=DaChapNhan. |
| UT-206 | Chấp nhận mới hợp lệ | Đúng đắn | Pass | KetQua.ThanhCong=true, mã=200, thông điệp=null. DuLieu=60000000-0000-0000-0000-000000000006. Lệnh nghiệp vụ=1; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-207 | Phiên bản sai độ dài | Đầy đủ | Pass | KetQua.ThanhCong=false, mã=400, thông điệp="Phiên bản không hợp lệ.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-208 | Yêu cầu đã xử lý | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=409, thông điệp="Yêu cầu đã hết hiệu lực.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=TuChoi. |
| UT-209 | Yêu cầu hết hạn | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=409, thông điệp="Yêu cầu đã hết hiệu lực.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-210 | Sai phía thao tác | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=403, thông điệp="Không có quyền phản hồi.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-211 | Phiên bản cũ | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=409, thông điệp="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.". Lệnh nghiệp vụ=0; Save=1; Commit=0; notice=1; outbox=1; trạng thái đối tượng theo dõi=TuChoi. |
| UT-212 | Phản hồi hợp lệ | Đúng đắn | Pass | KetQua.ThanhCong=true, mã=200, thông điệp=null. Lệnh nghiệp vụ=0; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=TuChoi. |
| UT-213 | Phiên bản sai độ dài | Đầy đủ | Pass | KetQua.ThanhCong=false, mã=400, thông điệp="Phiên bản không hợp lệ.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-214 | Yêu cầu đã xử lý | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=409, thông điệp="Yêu cầu đã hết hiệu lực.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=TuChoi. |
| UT-215 | Yêu cầu hết hạn | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=409, thông điệp="Yêu cầu đã hết hiệu lực.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-216 | Sai phía thao tác | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=403, thông điệp="Không có quyền phản hồi.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |
| UT-217 | Phiên bản cũ | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=409, thông điệp="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.". Lệnh nghiệp vụ=0; Save=1; Commit=0; notice=1; outbox=1; trạng thái đối tượng theo dõi=DaRut. |
| UT-218 | Phản hồi hợp lệ | Đúng đắn | Pass | KetQua.ThanhCong=true, mã=200, thông điệp=null. Lệnh nghiệp vụ=0; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=DaRut. |
| UT-219 | Lý do dài 2000 | Đầy đủ | Pass | KetQua.ThanhCong=true, mã=200, thông điệp=null. Lệnh nghiệp vụ=0; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=TuChoi. |
| UT-220 | Lý do dài 2001 | Đúng đắn | Pass | KetQua.ThanhCong=false, mã=400, thông điệp="Lý do tối đa 2000 ký tự.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc. |

## Dữ liệu chung để tái hiện

H/U1 (bên học)=`10000000-0000-0000-0000-000000000001`; G/U2 (gia sư)=`20000000-0000-0000-0000-000000000002`; người khác U3=`30000000-0000-0000-0000-000000000003`. LH1=`40000000-0000-0000-0000-000000000004`; YC1=`50000000-0000-0000-0000-000000000005`; C1=`60000000-0000-0000-0000-000000000006`.

RV1=`[1,2,3,4,5,6,7,8]`, Base64=`AQIDBAUGBwg=`; RV0=`[0,0,0,0,0,0,0,1]`. Đề xuất mặc định: Toán lớp 9, MonHocId=1, mục tiêu Ôn tập, 1 buổi/tuần, 60 phút; thứ 2 từ 18:00–19:00; học phí 150000 VND/Gio; trực tuyến; bắt đầu 2026-11-15 10:00 UTC+7. Yêu cầu mặc định ChoBenHoc, gia sư là người gửi, hết hạn T+2 ngày.

## Các bước thực thi và assertion theo từng ca

### UT-170 — Người thực hiện không khớp phiên (Pass)

- **Đích thiết kế:** `KetNoiService.GuiYeuCauAsync(...)`.
- **Đích thực thi:** `GuiYeuCauAsync(GuiYeuCauViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Xác thực.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Phiên hiện tại U1; maNguoiThucHien=U2.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiYeuCauAsync(GuiYeuCauViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** KetQua lỗi 403; không lệnh ghi, Save, Commit, notice hoặc outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi 403; không thực hiện thay đổi dữ liệu hoặc commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=403, thông điệp="Không có quyền thực hiện thao tác.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.KetThuc.cs:10`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_170`.

### UT-171 — Người thực hiện không khớp phiên (Pass)

- **Đích thiết kế:** `KetNoiService.GuiDeXuatAsync(...)`.
- **Đích thực thi:** `GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Xác thực.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Phiên hiện tại U1; maNguoiThucHien=U2.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** KetQua lỗi 403; không lệnh ghi, Save, Commit, notice hoặc outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi 403; không thực hiện thay đổi dữ liệu hoặc commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=403, thông điệp="Không có quyền thực hiện thao tác.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.KetThuc.cs:10`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_171`.

### UT-172 — Người thực hiện không khớp phiên (Pass)

- **Đích thiết kế:** `KetNoiService.ChapNhanDeXuatAsync(...)`.
- **Đích thực thi:** `ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Xác thực.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Phiên hiện tại U1; maNguoiThucHien=U2.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** KetQua lỗi 403; không lệnh ghi, Save, Commit, notice hoặc outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi 403; không thực hiện thay đổi dữ liệu hoặc commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=403, thông điệp="Không có quyền thực hiện thao tác.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.KetThuc.cs:10`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_172`.

### UT-173 — Người thực hiện không khớp phiên (Pass)

- **Đích thiết kế:** `KetNoiService.TuChoiDeXuatAsync(...)`.
- **Đích thực thi:** `TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Xác thực.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Phiên hiện tại U1; maNguoiThucHien=U2.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** KetQua lỗi 403; không lệnh ghi, Save, Commit, notice hoặc outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi 403; không thực hiện thay đổi dữ liệu hoặc commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=403, thông điệp="Không có quyền thực hiện thao tác.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.KetThuc.cs:10`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_173`.

### UT-174 — Người thực hiện không khớp phiên (Pass)

- **Đích thiết kế:** `KetNoiService.RutYeuCauAsync(...)`.
- **Đích thực thi:** `RutYeuCauAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Xác thực.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Phiên hiện tại U1; maNguoiThucHien=U2.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await RutYeuCauAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** KetQua lỗi 403; không lệnh ghi, Save, Commit, notice hoặc outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi 403; không thực hiện thay đổi dữ liệu hoặc commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=403, thông điệp="Không có quyền thực hiện thao tác.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.KetThuc.cs:10`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_174`.

### UT-175 — Liên hệ không tồn tại (Fail)

- **Đích thiết kế:** `KetNoiService.AcceptedAsync(...) qua phương thức public`.
- **Đích thực thi:** `AcceptedAsync(Guid, bool, Guid, CancellationToken) — helper private thực tế trong code`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng đọc liên hệ trả null.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await AcceptedAsync(Guid, bool, Guid, CancellationToken) — helper private thực tế trong code` với actor/token của fixture. Helper private được gọi qua reflection, await Task<KetQua<YeuCauLienHe>> thật.
- **B4 – Assert:** Mã lỗi đúng bảng; không lệnh ghi, Save, Commit, notice/outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi 404.
- **Thực tế:** KetQua.ThanhCong=false, mã=403, thông điệp="Cần liên hệ được chấp nhận trước.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:97`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_175`.

### UT-176 — Sai bên gia sư (Pass)

- **Đích thiết kế:** `KetNoiService.AcceptedAsync(...) qua phương thức public`.
- **Đích thực thi:** `AcceptedAsync(Guid, bool, Guid, CancellationToken) — helper private thực tế trong code`.
- **Tiêu chuẩn:** Xác thực.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. tutor=true nhưng GiaSuId không phải actor.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await AcceptedAsync(Guid, bool, Guid, CancellationToken) — helper private thực tế trong code` với actor/token của fixture. Helper private được gọi qua reflection, await Task<KetQua<YeuCauLienHe>> thật.
- **B4 – Assert:** Mã lỗi đúng bảng; không lệnh ghi, Save, Commit, notice/outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi 403.
- **Thực tế:** KetQua.ThanhCong=false, mã=403, thông điệp="Cần liên hệ được chấp nhận trước.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:97`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_176`.

### UT-177 — Sai bên học (Pass)

- **Đích thiết kế:** `KetNoiService.AcceptedAsync(...) qua phương thức public`.
- **Đích thực thi:** `AcceptedAsync(Guid, bool, Guid, CancellationToken) — helper private thực tế trong code`.
- **Tiêu chuẩn:** Xác thực.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. tutor=false nhưng BenHocId không phải actor.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await AcceptedAsync(Guid, bool, Guid, CancellationToken) — helper private thực tế trong code` với actor/token của fixture. Helper private được gọi qua reflection, await Task<KetQua<YeuCauLienHe>> thật.
- **B4 – Assert:** Mã lỗi đúng bảng; không lệnh ghi, Save, Commit, notice/outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi 403.
- **Thực tế:** KetQua.ThanhCong=false, mã=403, thông điệp="Cần liên hệ được chấp nhận trước.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:97`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_177`.

### UT-178 — Chưa được chấp nhận (Pass)

- **Đích thiết kế:** `KetNoiService.AcceptedAsync(...) qua phương thức public`.
- **Đích thực thi:** `AcceptedAsync(Guid, bool, Guid, CancellationToken) — helper private thực tế trong code`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Đúng bên nhưng trạng thái không phải ChapNhan.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await AcceptedAsync(Guid, bool, Guid, CancellationToken) — helper private thực tế trong code` với actor/token của fixture. Helper private được gọi qua reflection, await Task<KetQua<YeuCauLienHe>> thật.
- **B4 – Assert:** Mã lỗi đúng bảng; không lệnh ghi, Save, Commit, notice/outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi 403.
- **Thực tế:** KetQua.ThanhCong=false, mã=403, thông điệp="Cần liên hệ được chấp nhận trước.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:97`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_178`.

### UT-179 — Một tài khoản bị khóa (Fail)

- **Đích thiết kế:** `KetNoiService.AcceptedAsync(...) qua phương thức public`.
- **Đích thực thi:** `AcceptedAsync(Guid, bool, Guid, CancellationToken) — helper private thực tế trong code`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Liên hệ đã chấp nhận, đúng bên; có tài khoản BiKhoa.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await AcceptedAsync(Guid, bool, Guid, CancellationToken) — helper private thực tế trong code` với actor/token của fixture. Helper private được gọi qua reflection, await Task<KetQua<YeuCauLienHe>> thật.
- **B4 – Assert:** Mã lỗi đúng bảng; không lệnh ghi, Save, Commit, notice/outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi 403.
- **Thực tế:** KetQua.ThanhCong=true, mã=200, thông điệp=null. Liên hệ trả về=40000000-0000-0000-0000-000000000004. Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:97`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_179`.

### UT-180 — Liên hệ hợp lệ (Pass)

- **Đích thiết kế:** `KetNoiService.AcceptedAsync(...) qua phương thức public`.
- **Đích thực thi:** `AcceptedAsync(Guid, bool, Guid, CancellationToken) — helper private thực tế trong code`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Đúng bên; ChapNhan; không tài khoản bị khóa.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await AcceptedAsync(Guid, bool, Guid, CancellationToken) — helper private thực tế trong code` với actor/token của fixture. Helper private được gọi qua reflection, await Task<KetQua<YeuCauLienHe>> thật.
- **B4 – Assert:** Thành công; DuLieu là chính liên hệ LH1 đã đọc; không có thao tác ghi.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua thành công chứa đúng liên hệ.
- **Thực tế:** KetQua.ThanhCong=true, mã=200, thông điệp=null. Liên hệ trả về=40000000-0000-0000-0000-000000000004. Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:97`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_180`.

### UT-181 — Phân giải liên hệ từ đối tượng (Pass)

- **Đích thiết kế:** `KetNoiService.GuiYeuCauAsync(...)`.
- **Đích thực thi:** `GuiYeuCauAsync(GuiYeuCauViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. MaDoiTuong là gia sư U2; liên hệ ChapNhan giữa U1/U2 có Id LH1.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiYeuCauAsync(GuiYeuCauViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Phân giải đúng cặp G/H và token một lần; LienHeId của input, khóa liên hệ và đối số lệnh đều là LH1; trả YC1; thông báo đúng gia sư; Save trước Commit.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Dùng LH1 trong kiểm tra và lệnh gửi; không dùng nhầm liên hệ người khác.
- **Thực tế:** KetQua.ThanhCong=true, mã=200, thông điệp=null. DuLieu=50000000-0000-0000-0000-000000000005. Lệnh nghiệp vụ=1; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:103`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_181`.

### UT-182 — Gửi thành công (Pass)

- **Đích thiết kế:** `KetNoiService.GuiYeuCauAsync(...)`.
- **Đích thực thi:** `GuiYeuCauAsync(GuiYeuCauViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Phiên đúng; liên hệ hợp lệ; cổng procedure thành công; truy vấn lại trả yêu cầu mới YC1.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiYeuCauAsync(GuiYeuCauViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Trả YC1; đúng mã liên hệ/người gửi/lời nhắn/nhu cầu; một lệnh nghiệp vụ; một notice/outbox liên kết cùng MaSuKien; nhận G, target YC1; Event/Notice trước Save trước Commit.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Trả YC1; tạo notice KetNoi đúng recipient/target; lưu thông báo/outbox trước commit; không trả Id yêu cầu cũ.
- **Thực tế:** KetQua.ThanhCong=true, mã=200, thông điệp=null. DuLieu=50000000-0000-0000-0000-000000000005. Lệnh nghiệp vụ=1; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:103`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_182`.

### UT-183 — Phí không hợp lệ (Pass)

- **Đích thiết kế:** `KetNoiService.GuiDeXuatAsync(...)`.
- **Đích thực thi:** `GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. HocPhi=-1; các trường khác hợp lệ.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** KetQua lỗi 400; không lệnh procedure, Save, Commit, notice/outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi dữ liệu; không gọi procedure, lưu hoặc commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=400, thông điệp="Học phí không âm, tối đa hai chữ số thập phân; chọn đúng đơn vị.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:146`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_183`.

### UT-184 — Khung giờ không hợp lệ (Pass)

- **Đích thiết kế:** `KetNoiService.GuiDeXuatAsync(...)`.
- **Đích thực thi:** `GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Một khung có bắt đầu >= kết thúc.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** KetQua lỗi 400; không lệnh procedure, Save, Commit, notice/outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi dữ liệu; không gọi procedure, lưu hoặc commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=400, thông điệp="Thứ hoặc giờ của khung giờ không hợp lệ.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:146`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_184`.

### UT-185 — Số khung không bằng số buổi (Pass)

- **Đích thiết kế:** `KetNoiService.GuiDeXuatAsync(...)`.
- **Đích thực thi:** `GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. SoBuoiTuan=2, chỉ 1 khung.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** KetQua lỗi 400; không lệnh procedure, Save, Commit, notice/outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi dữ liệu; không gọi procedure, lưu hoặc commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=400, thông điệp="Số khung giờ và thời lượng phải khớp điều kiện lớp.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:146`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_185`.

### UT-186 — Thời lượng không khớp (Pass)

- **Đích thiết kế:** `KetNoiService.GuiDeXuatAsync(...)`.
- **Đích thực thi:** `GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. ThoiLuongPhut=60, một khung dài 45 phút.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** KetQua lỗi 400; không lệnh procedure, Save, Commit, notice/outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi dữ liệu; không gọi procedure, lưu hoặc commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=400, thông điệp="Số khung giờ và thời lượng phải khớp điều kiện lớp.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:146`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_186`.

### UT-187 — Ngày bắt đầu không ở tương lai (Pass)

- **Đích thiết kế:** `KetNoiService.GuiDeXuatAsync(...)`.
- **Đích thực thi:** `GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. BatDauHocLuc=T-1 phút; đồng hồ T.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** KetQua lỗi 400; không lệnh procedure, Save, Commit, notice/outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi dữ liệu; không gọi procedure, lưu hoặc commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=400, thông điệp="Ngày bắt đầu phải ở tương lai.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:146`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_187`.

### UT-188 — Trực tiếp thiếu địa điểm (Pass)

- **Đích thiết kế:** `KetNoiService.GuiDeXuatAsync(...)`.
- **Đích thực thi:** `GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đầy đủ.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. HinhThuc=TrucTiep; DiaDiem trống.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** KetQua lỗi 400; không lệnh procedure, Save, Commit, notice/outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi dữ liệu; không gọi procedure, lưu hoặc commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=400, thông điệp="Nhập địa điểm học trực tiếp.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:146`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_188`.

### UT-189 — Nhu cầu sai chủ (Pass)

- **Đích thiết kế:** `KetNoiService.GuiDeXuatAsync(...)`.
- **Đích thực thi:** `GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. NhuCauId thuộc U3 thay vì bên học của liên hệ.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** KetQua lỗi 400; không lệnh procedure, Save, Commit, notice/outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi dữ liệu; không gọi procedure, lưu hoặc commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=400, thông điệp="Nhu cầu phải thuộc bên học và đang mở.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:146`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_189`.

### UT-190 — Nhu cầu không mở (Pass)

- **Đích thiết kế:** `KetNoiService.GuiDeXuatAsync(...)`.
- **Đích thực thi:** `GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. NhuCauId đúng chủ nhưng trạng thái không DangMo.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** KetQua lỗi 400; không lệnh procedure, Save, Commit, notice/outbox; trạng thái yêu cầu giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi dữ liệu; không gọi procedure, lưu hoặc commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=400, thông điệp="Nhu cầu phải thuộc bên học và đang mở.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:146`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_190`.

### UT-191 — Tạo đề xuất mới (Pass)

- **Đích thiết kế:** `KetNoiService.GuiDeXuatAsync(...)`.
- **Đích thực thi:** `GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Id=null; phí/lịch/buổi/thời lượng/ngày/địa điểm hợp lệ; cổng SQL trả thành công YC1.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Một lệnh đề xuất; JSON bằng dữ liệu lịch đầu vào; đúng LH1/G; @YeuCauId null và SQL parameter RV=DBNull khi tạo, hoặc YC1/RV1 đúng 8 byte khi cập nhật; trả YC1; thông báo H/YC1; Save trước Commit.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Gửi JSON đúng các khung; @YeuCauId null và phiên bản cũ null; trả YC1; tạo thông báo và commit sau lưu.
- **Thực tế:** KetQua.ThanhCong=true, mã=200, thông điệp=null. DuLieu=50000000-0000-0000-0000-000000000005. Lệnh nghiệp vụ=1; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:146`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_191`.

### UT-192 — Cập nhật đề xuất (Pass)

- **Đích thiết kế:** `KetNoiService.GuiDeXuatAsync(...)`.
- **Đích thực thi:** `GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Id=YC1; RowVersion=RV1; dữ liệu hợp lệ.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Một lệnh đề xuất; JSON bằng dữ liệu lịch đầu vào; đúng LH1/G; @YeuCauId null và SQL parameter RV=DBNull khi tạo, hoặc YC1/RV1 đúng 8 byte khi cập nhật; trả YC1; thông báo H/YC1; Save trước Commit.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Cổng SQL nhận Id YC1 và đúng 8 byte RV1 sau chuyển đổi; JSON đúng; không biến cập nhật thành tạo mới.
- **Thực tế:** KetQua.ThanhCong=true, mã=200, thông điệp=null. DuLieu=50000000-0000-0000-0000-000000000005. Lệnh nghiệp vụ=1; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:146`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_192`.

### UT-193 — Ánh xạ lỗi concurrency (Pass)

- **Đích thiết kế:** `KetNoiService.GuiYeuCauAsync(...)`.
- **Đích thực thi:** `GuiYeuCauAsync(GuiYeuCauViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng ghi ném DbUpdateConcurrencyException.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiYeuCauAsync(GuiYeuCauViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** DbUpdateConcurrencyException được chuyển thành lỗi 409; một lệnh nghiệp vụ đã được thử; không Save, Commit hoặc notice; không báo thành công.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi 409; không commit; không báo thành công.
- **Thực tế:** KetQua.ThanhCong=false, mã=409, thông điệp="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.". Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Infrastructure/KetQua.cs:20`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_193`.

### UT-194 — Lỗi ngoài dự kiến (Pass)

- **Đích thiết kế:** `KetNoiService.GuiYeuCauAsync(...)`.
- **Đích thực thi:** `GuiYeuCauAsync(GuiYeuCauViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng hạ tầng ném ngoại lệ không nằm trong bảng ánh xạ.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiYeuCauAsync(GuiYeuCauViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Đúng instance InvalidOperationException thoát lên caller; không Commit, không notice.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Ngoại lệ được truyền tới tầng xử lý lỗi; không trả thành công và không commit.
- **Thực tế:** Ngoại lệ thoát ra: System.InvalidOperationException: Hạ tầng lỗi giả lập. Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Infrastructure/KetQua.cs:20`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_194`.

### UT-195 — Ánh xạ mã lỗi SQL đã biết (Pass)

- **Đích thiết kế:** `KetNoiService.GuiYeuCauAsync(...)`.
- **Đích thực thi:** `GuiYeuCauAsync(GuiYeuCauViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng SQL ném lỗi thuộc 51130–51154; chọn từng mã có trong bảng ánh xạ thật.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiYeuCauAsync(GuiYeuCauViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** 25 mã 51130–51154 đều có status theo bảng kỳ vọng độc lập: 51133/51140/51151→403; 51132/51138/51139/51141/51144/51145/51152→409; còn lại→400. Mỗi mã fixture riêng; không Commit/Save/notice.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Trả MaLoi 400/403/409 đúng bảng ánh xạ; không commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=400, thông điệp="SQL giả lập 51130". Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Infrastructure/KetQua.cs:20`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_195`.

Đã kiểm đủ 25 mã SQL: 51130→400, 51131→400, 51132→409, 51133→403, 51134→400, 51135→400, 51136→400, 51137→400, 51138→409, 51139→409, 51140→403, 51141→409, 51142→400, 51143→400, 51144→409, 51145→409, 51146→400, 51147→400, 51148→400, 51149→400, 51150→400, 51151→403, 51152→409, 51153→400, 51154→400.

### UT-196 — Ánh xạ lỗi concurrency (Pass)

- **Đích thiết kế:** `KetNoiService.GuiDeXuatAsync(...)`.
- **Đích thực thi:** `GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng ghi ném DbUpdateConcurrencyException.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** DbUpdateConcurrencyException được chuyển thành lỗi 409; một lệnh nghiệp vụ đã được thử; không Save, Commit hoặc notice; không báo thành công.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi 409; không commit; không báo thành công.
- **Thực tế:** KetQua.ThanhCong=false, mã=409, thông điệp="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.". Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Infrastructure/KetQua.cs:20`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_196`.

### UT-197 — Lỗi ngoài dự kiến (Pass)

- **Đích thiết kế:** `KetNoiService.GuiDeXuatAsync(...)`.
- **Đích thực thi:** `GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng hạ tầng ném ngoại lệ không nằm trong bảng ánh xạ.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Đúng instance InvalidOperationException thoát lên caller; không Commit, không notice.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Ngoại lệ được truyền tới tầng xử lý lỗi; không trả thành công và không commit.
- **Thực tế:** Ngoại lệ thoát ra: System.InvalidOperationException: Hạ tầng lỗi giả lập. Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Infrastructure/KetQua.cs:20`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_197`.

### UT-198 — Ánh xạ mã lỗi SQL đã biết (Pass)

- **Đích thiết kế:** `KetNoiService.GuiDeXuatAsync(...)`.
- **Đích thực thi:** `GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng SQL ném lỗi thuộc 51130–51154; chọn từng mã có trong bảng ánh xạ thật.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await GuiDeXuatAsync(DeXuatLopViewModel, Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** 25 mã 51130–51154 đều có status theo bảng kỳ vọng độc lập: 51133/51140/51151→403; 51132/51138/51139/51141/51144/51145/51152→409; còn lại→400. Mỗi mã fixture riêng; không Commit/Save/notice.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Trả MaLoi 400/403/409 đúng bảng ánh xạ; không commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=400, thông điệp="SQL giả lập 51130". Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Infrastructure/KetQua.cs:20`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_198`.

Đã kiểm đủ 25 mã SQL: 51130→400, 51131→400, 51132→409, 51133→403, 51134→400, 51135→400, 51136→400, 51137→400, 51138→409, 51139→409, 51140→403, 51141→409, 51142→400, 51143→400, 51144→409, 51145→409, 51146→400, 51147→400, 51148→400, 51149→400, 51150→400, 51151→403, 51152→409, 51153→400, 51154→400.

### UT-199 — Ánh xạ lỗi concurrency (Pass)

- **Đích thiết kế:** `KetNoiService.ChapNhanDeXuatAsync(...)`.
- **Đích thực thi:** `ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng ghi ném DbUpdateConcurrencyException.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** DbUpdateConcurrencyException được chuyển thành lỗi 409; một lệnh nghiệp vụ đã được thử; không Save, Commit hoặc notice; không báo thành công.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua lỗi 409; không commit; không báo thành công.
- **Thực tế:** KetQua.ThanhCong=false, mã=409, thông điệp="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.". Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Infrastructure/KetQua.cs:20`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_199`.

### UT-200 — Lỗi ngoài dự kiến (Pass)

- **Đích thiết kế:** `KetNoiService.ChapNhanDeXuatAsync(...)`.
- **Đích thực thi:** `ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng hạ tầng ném ngoại lệ không nằm trong bảng ánh xạ.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Đúng instance InvalidOperationException thoát lên caller; không Commit, không notice.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Ngoại lệ được truyền tới tầng xử lý lỗi; không trả thành công và không commit.
- **Thực tế:** Ngoại lệ thoát ra: System.InvalidOperationException: Hạ tầng lỗi giả lập. Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Infrastructure/KetQua.cs:20`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_200`.

### UT-201 — Ánh xạ mã lỗi SQL đã biết (Pass)

- **Đích thiết kế:** `KetNoiService.ChapNhanDeXuatAsync(...)`.
- **Đích thực thi:** `ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng SQL ném lỗi thuộc 51130–51154; chọn từng mã có trong bảng ánh xạ thật.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** 25 mã 51130–51154 đều có status theo bảng kỳ vọng độc lập: 51133/51140/51151→403; 51132/51138/51139/51141/51144/51145/51152→409; còn lại→400. Mỗi mã fixture riêng; không Commit/Save/notice.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Trả MaLoi 400/403/409 đúng bảng ánh xạ; không commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=400, thông điệp="SQL giả lập 51130". Lệnh nghiệp vụ=1; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Infrastructure/KetQua.cs:20`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_201`.

Đã kiểm đủ 25 mã SQL: 51130→400, 51131→400, 51132→409, 51133→403, 51134→400, 51135→400, 51136→400, 51137→400, 51138→409, 51139→409, 51140→403, 51141→409, 51142→400, 51143→400, 51144→409, 51145→409, 51146→400, 51147→400, 51148→400, 51149→400, 51150→400, 51151→403, 51152→409, 51153→400, 51154→400.

### UT-202 — RV không đủ 8 byte (Pass)

- **Đích thiết kế:** `KetNoiService.ChapNhanDeXuatAsync(...)`.
- **Đích thực thi:** `ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Truyền byte[7].
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Byte[7] bị từ chối với mã 400; không lệnh nghiệp vụ/Save/Commit/notice/outbox; trạng thái giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Bị từ chối trước thực thi procedure tạo lớp; mã lỗi cần code.
- **Thực tế:** KetQua.ThanhCong=false, mã=400, thông điệp="Phiên bản không hợp lệ.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:179`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_202`.

### UT-203 — Khóa ứng dụng thất bại (Pass)

- **Đích thiết kế:** `KetNoiService.ChapNhanDeXuatAsync(...)`.
- **Đích thực thi:** `ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Cổng khóa trả số âm.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Nhận SqlException 51152 từ cổng thực thi khóa → KetQua 409; không lệnh tạo lớp, Save, Commit hoặc notice/outbox. Kiểm phần ánh xạ ở service, không kiểm kết quả âm thực tế của sp_getapplock.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Ánh xạ 51152 thành 409; không tạo lớp/lịch/thông báo; không commit.
- **Thực tế:** KetQua.ThanhCong=false, mã=409, thông điệp="SQL giả lập 51152". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:179`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_203`.

### UT-204 — Sai bên học (Pass)

- **Đích thiết kế:** `KetNoiService.ChapNhanDeXuatAsync(...)`.
- **Đích thực thi:** `ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Xác thực.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Đọc lại sau khóa thấy BenHocId=U2, actor U1.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Request được đọc sau bước khóa; actor là gia sư G, BenHocId=H khác actor; lỗi 403, không gọi lệnh tạo lớp/Save/Commit/notice.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua 403; không gọi procedure tạo lớp.
- **Thực tế:** KetQua.ThanhCong=false, mã=403, thông điệp="Chỉ bên học được chấp nhận đề xuất.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:179`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_204`.

### UT-205 — Đã chấp nhận trước đó (Pass)

- **Đích thiết kế:** `KetNoiService.ChapNhanDeXuatAsync(...)`.
- **Đích thực thi:** `ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Sau khóa, yêu cầu DaChapNhan và lớp C1 đã tồn tại.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Khóa và đọc lại yêu cầu DaChapNhan; trả lớp C1 có sẵn; không gọi lệnh tạo mới, Save, Commit hoặc notice/outbox.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Trả C1; không tạo thêm lớp/lịch/thông báo; không gọi procedure tạo mới.
- **Thực tế:** KetQua.ThanhCong=true, mã=200, thông điệp=null. DuLieu=60000000-0000-0000-0000-000000000006. Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=DaChapNhan.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:179`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_205`.

### UT-206 — Chấp nhận mới hợp lệ (Pass)

- **Đích thiết kế:** `KetNoiService.ChapNhanDeXuatAsync(...)`.
- **Đích thực thi:** `ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Sau khóa, đúng bên học, yêu cầu hợp lệ; procedure trả lớp C1.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await ChapNhanDeXuatAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Hai lệnh gồm khóa ứng dụng rồi đúng một lệnh tạo lớp; đối số YC1 và RV1 đúng; đọc lại C1; trả C1; notice G/target C1; outbox liên kết notice; Save trước Commit.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Trả C1; chỉ một chuỗi tạo lớp; thông báo/outbox lưu trước commit.
- **Thực tế:** KetQua.ThanhCong=true, mã=200, thông điệp=null. DuLieu=60000000-0000-0000-0000-000000000006. Lệnh nghiệp vụ=1; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:179`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_206`.

### UT-207 — Phiên bản sai độ dài (Pass)

- **Đích thiết kế:** `KetNoiService.TuChoiDeXuatAsync(...)`.
- **Đích thực thi:** `TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đầy đủ.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. rowVersion=byte[7].
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Byte[7] bị từ chối với mã 400; không lệnh nghiệp vụ/Save/Commit/notice/outbox; trạng thái giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Từ chối; không thay đổi yêu cầu.
- **Thực tế:** KetQua.ThanhCong=false, mã=400, thông điệp="Phiên bản không hợp lệ.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:197`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_207`.

### UT-208 — Yêu cầu đã xử lý (Pass)

- **Đích thiết kế:** `KetNoiService.TuChoiDeXuatAsync(...)`.
- **Đích thực thi:** `TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Yêu cầu không còn chờ.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Lỗi 409, không ghi đè trạng thái, không lệnh nghiệp vụ, Save/Commit/notice/outbox.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Lỗi 403/409 theo hợp đồng; không ghi đè.
- **Thực tế:** KetQua.ThanhCong=false, mã=409, thông điệp="Yêu cầu đã hết hiệu lực.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=TuChoi.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:197`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_208`.

### UT-209 — Yêu cầu hết hạn (Pass)

- **Đích thiết kế:** `KetNoiService.TuChoiDeXuatAsync(...)`.
- **Đích thực thi:** `TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Yêu cầu chờ nhưng đã quá hạn.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Lỗi 409, không ghi đè trạng thái, không lệnh nghiệp vụ, Save/Commit/notice/outbox.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Lỗi theo hợp đồng; không thay đổi trạng thái.
- **Thực tế:** KetQua.ThanhCong=false, mã=409, thông điệp="Yêu cầu đã hết hiệu lực.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:197`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_209`.

### UT-210 — Sai phía thao tác (Pass)

- **Đích thiết kế:** `KetNoiService.TuChoiDeXuatAsync(...)`.
- **Đích thực thi:** `TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Từ chối bởi bên gửi hoặc rút bởi bên nhận.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Sai phía thao tác trả 403; không sửa trạng thái, không Save/Commit/notice/outbox.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua 403; không lưu.
- **Thực tế:** KetQua.ThanhCong=false, mã=403, thông điệp="Không có quyền phản hồi.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:197`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_210`.

### UT-211 — Phiên bản cũ (Pass)

- **Đích thiết kế:** `KetNoiService.TuChoiDeXuatAsync(...)`.
- **Đích thực thi:** `TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Input RV0, bản hiện tại RV1.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** RV0 được chuyển Base64 và đặt làm original version; Save giả lập ném concurrency; KetQua 409; không Commit, không gọi lệnh tạo lớp. Đối tượng trong RAM có thể đã đổi trạng thái; unit không chứng minh rollback CSDL.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Lỗi xung đột; không cập nhật.
- **Thực tế:** KetQua.ThanhCong=false, mã=409, thông điệp="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.". Lệnh nghiệp vụ=0; Save=1; Commit=0; notice=1; outbox=1; trạng thái đối tượng theo dõi=TuChoi.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:197`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_211`.

### UT-212 — Phản hồi hợp lệ (Pass)

- **Đích thiết kế:** `KetNoiService.TuChoiDeXuatAsync(...)`.
- **Đích thực thi:** `TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Đúng bên nhận khi từ chối/đúng bên gửi khi rút; đang chờ, chưa hết hạn, RV1.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Trạng thái TuChoi/DaRut đúng thao tác; version RV1; thông báo tới đối tác, target YC1; đúng một Save và Commit; outbox/notice được thêm trước Save; không lệnh tạo lớp.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Trạng thái=TuChoi; thông báo đúng đối tác; lưu và commit; không tạo lớp.
- **Thực tế:** KetQua.ThanhCong=true, mã=200, thông điệp=null. Lệnh nghiệp vụ=0; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=TuChoi.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:197`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_212`.

### UT-213 — Phiên bản sai độ dài (Pass)

- **Đích thiết kế:** `KetNoiService.RutYeuCauAsync(...)`.
- **Đích thực thi:** `RutYeuCauAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đầy đủ.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. rowVersion=byte[7].
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await RutYeuCauAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Byte[7] bị từ chối với mã 400; không lệnh nghiệp vụ/Save/Commit/notice/outbox; trạng thái giữ nguyên.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Từ chối; không thay đổi yêu cầu.
- **Thực tế:** KetQua.ThanhCong=false, mã=400, thông điệp="Phiên bản không hợp lệ.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:197`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_213`.

### UT-214 — Yêu cầu đã xử lý (Pass)

- **Đích thiết kế:** `KetNoiService.RutYeuCauAsync(...)`.
- **Đích thực thi:** `RutYeuCauAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Yêu cầu không còn chờ.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await RutYeuCauAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Lỗi 409, không ghi đè trạng thái, không lệnh nghiệp vụ, Save/Commit/notice/outbox.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Lỗi 403/409 theo hợp đồng; không ghi đè.
- **Thực tế:** KetQua.ThanhCong=false, mã=409, thông điệp="Yêu cầu đã hết hiệu lực.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=TuChoi.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:197`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_214`.

### UT-215 — Yêu cầu hết hạn (Pass)

- **Đích thiết kế:** `KetNoiService.RutYeuCauAsync(...)`.
- **Đích thực thi:** `RutYeuCauAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Yêu cầu chờ nhưng đã quá hạn.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await RutYeuCauAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Lỗi 409, không ghi đè trạng thái, không lệnh nghiệp vụ, Save/Commit/notice/outbox.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Lỗi theo hợp đồng; không thay đổi trạng thái.
- **Thực tế:** KetQua.ThanhCong=false, mã=409, thông điệp="Yêu cầu đã hết hiệu lực.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:197`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_215`.

### UT-216 — Sai phía thao tác (Pass)

- **Đích thiết kế:** `KetNoiService.RutYeuCauAsync(...)`.
- **Đích thực thi:** `RutYeuCauAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Từ chối bởi bên gửi hoặc rút bởi bên nhận.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await RutYeuCauAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Sai phía thao tác trả 403; không sửa trạng thái, không Save/Commit/notice/outbox.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** KetQua 403; không lưu.
- **Thực tế:** KetQua.ThanhCong=false, mã=403, thông điệp="Không có quyền phản hồi.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:197`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_216`.

### UT-217 — Phiên bản cũ (Pass)

- **Đích thiết kế:** `KetNoiService.RutYeuCauAsync(...)`.
- **Đích thực thi:** `RutYeuCauAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Input RV0, bản hiện tại RV1.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await RutYeuCauAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** RV0 được chuyển Base64 và đặt làm original version; Save giả lập ném concurrency; KetQua 409; không Commit, không gọi lệnh tạo lớp. Đối tượng trong RAM có thể đã đổi trạng thái; unit không chứng minh rollback CSDL.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Lỗi xung đột; không cập nhật.
- **Thực tế:** KetQua.ThanhCong=false, mã=409, thông điệp="Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.". Lệnh nghiệp vụ=0; Save=1; Commit=0; notice=1; outbox=1; trạng thái đối tượng theo dõi=DaRut.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:197`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_217`.

### UT-218 — Phản hồi hợp lệ (Pass)

- **Đích thiết kế:** `KetNoiService.RutYeuCauAsync(...)`.
- **Đích thực thi:** `RutYeuCauAsync(Guid, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Đúng bên nhận khi từ chối/đúng bên gửi khi rút; đang chờ, chưa hết hạn, RV1.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await RutYeuCauAsync(Guid, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Trạng thái TuChoi/DaRut đúng thao tác; version RV1; thông báo tới đối tác, target YC1; đúng một Save và Commit; outbox/notice được thêm trước Save; không lệnh tạo lớp.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Trạng thái=DaRut; thông báo đúng đối tác; lưu và commit; không tạo lớp.
- **Thực tế:** KetQua.ThanhCong=true, mã=200, thông điệp=null. Lệnh nghiệp vụ=0; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=DaRut.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.cs:197`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_218`.

### UT-219 — Lý do dài 2000 (Pass)

- **Đích thiết kế:** `KetNoiService.TuChoiDeXuatAsync(...)`.
- **Đích thực thi:** `TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đầy đủ.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Lý do 2000 ký tự; mọi điều kiện khác hợp lệ.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Trạng thái TuChoi/DaRut đúng thao tác; version RV1; thông báo tới đối tác, target YC1; đúng một Save và Commit; outbox/notice được thêm trước Save; không lệnh tạo lớp.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Không bị từ chối bởi độ dài; xử lý từ chối bình thường.
- **Thực tế:** KetQua.ThanhCong=true, mã=200, thông điệp=null. Lệnh nghiệp vụ=0; Save=1; Commit=1; notice=1; outbox=1; trạng thái đối tượng theo dõi=TuChoi.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.YeuCau.cs:34`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_219`.

### UT-220 — Lý do dài 2001 (Pass)

- **Đích thiết kế:** `KetNoiService.TuChoiDeXuatAsync(...)`.
- **Đích thực thi:** `TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)`.
- **Tiêu chuẩn:** Đúng đắn.
- **Dữ liệu riêng:** Tạo đối tượng thật cần kiểm tra, fixture mới cho ca này. Dùng U1/U2 là hai người khác nhau, RV1 là byte[8] hiện tại, RV0 là byte[8] khác RV1; cố định đồng hồ nếu cần. Lý do 2001 ký tự; mọi điều kiện khác hợp lệ.
- **B1 – Arrange:** Khởi tạo `ServiceYeuCauDeXuatFixture` riêng. Dùng bộ ID/RV/đồng hồ ở trên; giữ service thật và mock cổng lưu trữ, người dùng, transaction, dữ liệu thông báo. Cấu hình predicate đọc trên bản ghi giả lập, không mock chính service đang kiểm.
- **B2 – Cấu hình:** Áp dụng dữ liệu riêng của dòng này vào input/contact/request/tài khoản. Với lỗi SQL/concurrency, cổng giả lập ném đúng exception khi lệnh nghiệp vụ hoặc Save được gọi; không mở database.
- **B3 – Act:** `await TuChoiDeXuatAsync(Guid, string, byte[], Guid, CancellationToken)` với actor/token của fixture. Ghi lại KetQua hoặc exception thoát ra, không mock kết quả này.
- **B4 – Assert:** Lý do 2001 ký tự trả 400; trạng thái giữ nguyên; không Save, Commit, lệnh nghiệp vụ, notice/outbox.
- **B5 – Bằng chứng:** Ghi `OBSERVATION_JSON` vào output của test trước assertion; TRX giữ trạng thái/stack, JSON giữ toàn bộ quan sát. Dispose fixture và transaction sau mỗi lượt.
- **Kỳ vọng từ Excel:** Lỗi dữ liệu; không lưu trạng thái từ chối.
- **Thực tế:** KetQua.ThanhCong=false, mã=400, thông điệp="Lý do tối đa 2000 ký tự.". Lệnh nghiệp vụ=0; Save=0; Commit=0; notice=0; outbox=0; trạng thái đối tượng theo dõi=ChoBenHoc.
- **Nguồn ứng dụng:** `Areas/KetNoi/Services/KetNoiService.YeuCau.cs:34`.
- **Chạy riêng:** `dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_220`.

## File thêm và thay đổi

- `tests/Unit/ServiceYeuCauDeXuatTest.cs`: 51 `[Fact]` có mã UT-170–220 và Trait Suite=ServiceYeuCauDeXuat.
- `tests/Unit/ServiceYeuCauDeXuatRunner.cs`: Arrange/Act/Assert riêng theo mã; ghi quan sát; ba ca SQL lặp đủ 25 mã.
- `tests/Unit/ServiceYeuCauDeXuatFixture.cs`: mock strict, ID/dữ liệu riêng, đồng hồ cố định, bắt lệnh/thông báo/outbox, tạo exception giả lập.
- `Areas/KetNoi/Services/IKetNoiStore.cs`: cổng đọc/ghi để thay dependency hạ tầng khi unit test.
- `Areas/KetNoi/Services/EfKetNoiStore.cs`: adapter EF mặc định giữ truy vấn/transaction/lệnh SQL trước đó.
- `KetNoiService.cs` và `KetNoiService.YeuCau.cs`: nhận store/TimeProvider tùy chọn; chuyển thao tác đọc/ghi qua adapter và dùng đồng hồ cho điều kiện thời gian. Không đổi guard nghiệp vụ, mã lỗi hay thuật toán để làm test Pass. Diff so với trước nhóm 07 nằm trong ZIP.

## Chạy lại trong VS Code

Mở Terminal ở `C:/Khóa luận TN/Educonnect`. Bộ test đã được build và chạy với .NET SDK 10.0.403, target net10.0. Nếu có ứng dụng đang chạy và giữ DLL, dừng tiến trình đó hoặc dùng BaseOutputPath riêng như lệnh bên dưới. Không cần khởi động ứng dụng hay SQL Server để chạy nhóm này.

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --no-restore --filter "Suite=ServiceYeuCauDeXuat" -p:UseAppHost=false "-p:BaseOutputPath=$env:TEMP/EduConnect-Group07-Unit/" --logger "trx;LogFileName=Service-YeuCau-DeXuat-Lop-Unit.trx" --results-directory "TestResults/07-Service-YeuCau-DeXuat-Lop-Unit"
```

Muốn chạy một ca, đổi filter thành `FullyQualifiedName~ServiceYeuCauDeXuatTest.UT_175`. Trong Test Explorer, chọn đúng lớp ServiceYeuCauDeXuatTest. Hai ca Fail hiện tại khiến `dotnet test` trả exit code 1; đây là thất bại assertion đã mô tả, không phải build bị lỗi.

TRX là bằng chứng từ test runner; `KetQua.json` là bản đối chiếu chi tiết theo Excel; `KetQua.md` là bản đọc được có bảng và giải thích. Chạy lại lệnh chỉ cập nhật TRX; muốn đồng bộ MD/JSON cần chạy lại script báo cáo kèm trong ZIP với bản Excel/đường dẫn fixture tương ứng.

## Kiểm tra hồi quy và tính nhất quán

Chạy lại toàn bộ project: 220 ca, 112 Pass, 105 Fail, 3 Skip. Đối chiếu 169 ca UT-001–169 với các TRX đã có: **không ca nào đổi trạng thái**. Các Fail/Skip của nhóm cũ vẫn giữ nguyên; không tính chúng vào kết quả 51 ca nhóm 07. Log hồi quy chỉ lưu ở work, không ghi đè báo cáo cũ.

Đã xác nhận đủ 51 mã liên tiếp, mỗi mã một Fact; tổng dự án 220 mã duy nhất. Ba ca SQL có đủ 25 quan sát/mã. Bảng Markdown được kiểm số cột ở header, delimiter và từng dòng. SHA256 đi kèm để đối chiếu file kết quả và gói mã nguồn.
