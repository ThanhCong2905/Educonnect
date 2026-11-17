# Tổng hợp các nhóm unit còn lại

**UT-301–371: 71 ca = 41 Pass, 30 Fail, 0 Skip/Blocked.**

30 Fail gồm **18 ca thiếu/sai hợp đồng thiết kế** và **12 ca khác kỳ vọng sau chạy hành vi thật**. Không kết luận 30 lỗi độc lập; nhiều ca cùng nguyên nhân.

| Nhóm | Số ca | Pass | Fail | Fail hợp đồng | Kết quả |
| --- | --- | --- | --- | --- | --- |
| 10. Thành phần dùng chung | 17 | 10 | 7 | 5 | [Đọc chi tiết](10-ThanhPhan-DungChung-Unit/KetQua.md) |
| 11. Phản hồi và đánh giá | 5 | 3 | 2 | 1 | [Đọc chi tiết](11-PhanHoi-DanhGia-Unit/KetQua.md) |
| 12. Trao đổi và nhắn tin | 8 | 7 | 1 | 0 | [Đọc chi tiết](12-TraoDoi-TinNhan-Unit/KetQua.md) |
| 13. Quản trị hệ thống | 18 | 11 | 7 | 2 | [Đọc chi tiết](13-QuanTri-HeThong-Unit/KetQua.md) |
| 14. Tài khoản và hồ sơ | 23 | 10 | 13 | 10 | [Đọc chi tiết](14-TaiKhoan-HoSo-Unit/KetQua.md) |

Mỗi nhóm có KetQua.md/json, TRX thật, snapshot mã nguồn và SHA256. Kết quả cũng được lưu vào thư mục TestResults tương ứng trong project; không thay kết quả nhóm cũ.

**Hồi quy:** 328 ca = 183 Pass, 142 Fail, 3 Skip; 257 ca cũ không đổi trạng thái. Ba Skip cũ UT-020/028/064 vẫn chưa được thực thi.

**Giới hạn:** các ca thiếu ClassAccessService, LoginAttemptService, UserSession, MaThaoTac phản hồi hoặc input vai trò dừng ở kiểm hợp đồng. Không gọi DB để giả thành unit và không kết luận thuật toán stored procedure sai. Nhóm 09 SQL/tích hợp không nằm trong lượt unit này.

Đã đối chiếu lại đúng workbook Unit Test.xlsx hiện tại. Chưa chạy lại integration sau thay đổi dependency.