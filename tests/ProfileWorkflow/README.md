# Kiểm thử hồ sơ theo môn

Chỉ chạy trên SQL Server kiểm thử đã áp dụng SQL00 mới hoặc script08. Không chạy bộ này vào DB thật. Test từ chối database không có tiền tố `EduConnect_Verification_`, tạo fixture với GUID riêng; các dữ liệu fixture được giữ để kiểm tra trình duyệt.

```powershell
$env:EDU_TEST_CONNECTION='Server=lpc:CONGVJP;Database=EduConnect_Verification_20261025;Integrated Security=True;Encrypt=False'
dotnet run --project tests/ProfileWorkflow/ProfileWorkflow.csproj --no-launch-profile
```

31 ca dịch vụ có trong Program.cs. Kết quả JSON ở bin/Debug/net10.0/profile-results.json. Chạy lại an toàn với các fixture mới; không xóa dữ liệu cũ. Giữ bản kết quả được xác minh tại docs/qa/profile-results.json.

Kiểm tra UI sau khi bộ dịch vụ tạo fixture:

```powershell
$env:ConnectionStrings__EduConnect=$env:EDU_TEST_CONNECTION
dotnet run --project EduConnect.Mvc.csproj --no-launch-profile --urls http://127.0.0.1:5281
# Terminal khác, đã có playwright và Edge:
node tests/profile-ui.cjs
```

Script UI đọc ID fixture trong kết quả dịch vụ, chạy ba session riêng cho gia sư, người học và quản trị; không dùng session của người đang làm việc. Script thay đổi trạng thái fixture, chụp mobile vào TEMP.

Bộ `tests/Integration` cũ đã điều chỉnh lời gọi constructor để build được; các ca tạo nhu cầu nhận lớp của gia sư và hồ sơ không có phí trong đó thuộc quy trình cũ và chưa được chuyển đổi toàn bộ. Không báo kết quả cũ của bộ này là kết quả regression cho luồng mới. Kiểm thử lớp, phụ huynh, quyền và chat bị ảnh hưởng đã được thực hiện trong bộ ProfileWorkflow; các luồng bài tập/lịch khác không được chạy lại toàn bộ trong lần thay đổi này.
