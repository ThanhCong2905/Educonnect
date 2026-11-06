# Kiểm thử UC2

Chỉ chạy với database có tên bắt đầu bằng `EduConnect_Verification_`.

```powershell
$env:EDU_TEST_CONNECTION='Server=lpc:CONGVJP;Database=EduConnect_Verification_UC2_20261104;Integrated Security=True;Encrypt=False'
dotnet run --project tests/UC2/UC2.csproj
```

29 kiểm tra tích hợp SQL Server: danh tính, phiên bản lớp/yêu cầu, hạn 72 giờ, quyền phản hồi, đồng ý/từ chối, metadata kết thúc, phản hồi lặp, xử lý quá hạn và thông báo không lặp. Bao gồm gửi yêu cầu, đề xuất, lỗi SQL, chấp nhận đồng thời, duy nhất lớp/lịch/thông báo và giữ đơn vị học phí.

Database mới dùng `Database/00_Original_DB02.sql`. Database cũ cần chạy `Database/09_UC2Workflow.sql` trước khi chạy phiên bản code này. Script chỉ bổ sung cột, không xóa dữ liệu.

Luồng tạo đề xuất và lớp sử dụng stored procedure, bao gồm lịch đầu tiên và thành viên chính. Service kiểm tra người thực hiện và hỗ trợ CancellationToken.

Lớp mới dùng trạng thái `DangHoatDong`; không có bước kích hoạt lớp riêng. Chấp nhận đồng thời được điều phối bằng khóa ứng dụng SQL theo mã yêu cầu trong giao dịch ngoài.

Kiểm tra lỗi lịch trùng: trả 409, rollback lớp và giữ nguyên đề xuất ChoBenHoc. Fixture chọn ngày sau lịch hiện có để có thể chạy nhiều lần trên cùng database kiểm thử.
