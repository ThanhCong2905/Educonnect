# Kiểm thử đính kèm tin nhắn

Chạy từ thư mục gốc, chỉ với database kiểm thử có tiền tố `EduConnect_Verification_` đã cài schema hiện tại:

```powershell
dotnet run --project tests/ProfileWorkflow/ProfileWorkflow.csproj --no-launch-profile
dotnet run --project tests/ChatAttachments/ChatAttachments.csproj --no-launch-profile
```

Bộ ProfileWorkflow tạo fixture, bộ ChatAttachments đọc đúng database và ID fixture đó. Không dùng DB chính; mã test kiểm tra tên DB và từ chối cấu hình không khớp. Dữ liệu/tệp test được giữ để kiểm tra trình duyệt, không xóa dữ liệu cũ. Tệp nằm `%TEMP%/EduConnectChatAttachments/uploads`.

19 nhóm kiểm thử gồm file-only, text + file, PDF, đúng/quá 10.000.000 byte, số lượng, nội dung giả định dạng, tải theo quyền, chống gửi lặp, thành viên chỉ xem/rời lớp, lớp kết thúc và tách riêng quyền chat riêng/chat lớp. Kết quả ở `bin/Debug/net10.0/results.json`.

`tests/chat-attachments.cjs` chạy với bản sao ứng dụng đã tắt worker trên cổng 5281, kết nối đúng DB trong results.json và contentRoot là uploadRoot của fixture. Không chạy vào app thông thường hoặc DB chính. Dùng Playwright/Edge có sẵn; không thêm package cho ứng dụng. Browser test gửi tệp thật nhưng chỉ vào fixture kiểm thử; kiểm tra cả client, HTTP, polling, giữ tệp khi đổi hội thoại/gửi lỗi, Enter, 390/768px và ảnh hiển thị. Kết quả `browser-results.json` cùng thư mục test output.
