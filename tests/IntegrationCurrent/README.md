# Integration — bộ 187 TC ngày 25/11/2026

Nguồn: Kich_ban_kiem_thu_tich_hop_EduConnect.xlsx, Manifest.json là bản đóng băng cùng 187 mã IT. Đây là bộ mới, không dùng mã/kết quả của MT1. Chỉ thêm file test; không sửa code ứng dụng.

Manifest.json là danh mục chuẩn gồm đúng 187 mã IT-001–IT-187; IntegrationTests.cs và implementation.json được sinh từ danh mục này. Report.py yêu cầu số test trong TRX khớp Manifest.json, phân biệt kết quả xUnit với kết luận TC và ghi riêng các ca thiếu phạm vi assertion. Sau khi sửa danh mục, cần chạy lại toàn bộ để sinh TRX/báo cáo mới; không dùng báo cáo cũ của bộ 191 mã để kết luận cho bộ 187 TC.

Chạy từ thư mục EduConnect1 bằng Windows Authentication có quyền truy cập SQL local:

```powershell
.\tests\IntegrationCurrent\Run-Integration.ps1
$runName = 'Integration-' + (Get-Date -Format 'yyyyMMdd-HHmmss')
.\tests\IntegrationCurrent\Run-Integration.ps1 -RunName $runName
.\tests\IntegrationCurrent\Run-Integration.ps1 -Filter 'TestCaseId=IT-017'
```

Database dành riêng: EduConnect_Verification_Integration_20261125_v2. Setup-TestDatabase.sql chỉ tạo mới và từ chối nếu tên đã tồn tại. Không chạy script này lại trên database đang dùng. Không trỏ test vào Educonnect.

Mặc định kết quả vào `TestResults\Integration` và ghi đè lượt cùng tên. Để giữ kết quả cũ, dùng `-RunName` duy nhất; TRX, log, evidence và KetQua.md/json/csv sẽ nằm trong `TestResults\<RunName>`.

Fixture được tạo qua connection quan sát/seed Windows. Khớp nối mục tiêu chạy dưới EXECUTE AS USER IntegrationApp, thuộc role KLTN_App; không dùng quyền dbo để kiểm quyền SQL của ứng dụng. File test kiểm BU1 bằng SQL thật, BU2 bằng Service/EF SqlServer thật, BU3 bằng WebApplicationFactory/HTTP/Identity/cookie/CSRF thật. Các hosted worker tắt ở các TC chưa kiểm worker. Không fake kết quả Service, không EF InMemory. Claims trong driver Service không phải bằng chứng đã kiểm cookie; chỉ ca HTTP chứng minh phần đó.

Thứ tự chạy theo executionOrder topo trong Manifest.json bằng BottomUpOrder. Đây là lượt chẩn đoán: ca trên vẫn có thể chạy dù ca tiên quyết Fail/chưa viết; báo cáo ghi rõ mức sẵn sàng này. Mỗi fixture có actors/môn/Id riêng. Test giữ dữ liệu trong DB riêng để điều tra; không xóa audit hoặc dữ liệu ứng dụng. Bộ đếm IP 'unknown' của TestServer được reset đầu mỗi fixture trong DB này để ca auth trước không khóa ca sau. Các ca reset DB toàn phần/worker độc lập chưa được coi đã phủ.

File:

- IntegrationTests.cs: đúng 187 [Fact] sinh từ Manifest.json, trait mã/nhóm/tầng; ca thiếu handler được đánh Skip để không nhầm là đã thực thi.
- Harness.cs: principal SQL, seed fixture, driver Service, HTTP client, CSRF, test host và bằng chứng lỗi.
- SqlContracts.cs: hợp đồng procedure/constraint/quyền SQL, đọc lại qua connection khác.
- NeedCases.cs: nhu cầu và binding/validation/ownership/RV/cancellation.
- BusinessCases.cs: Service liên hệ/đề xuất/lớp/kết thúc/lịch/nộp/chấm/chat.
- FamilyCases.cs: liên kết tài khoản và quyền thành viên.
- ScheduleCases.cs: lịch/buổi, local time, ghi nhận và concurrency.
- ConnectionCases.cs: HTTP đề xuất, xác nhận lại điều kiện, yêu cầu/phản hồi kết thúc và chặn ghi khi lớp kết thúc.
- LearningCases.cs: bài nháp, buổi không hợp lệ, dữ liệu bài cũ, nộp trước/sau hạn, nộp thay, điểm null/0/10 và RV kết quả.
- SearchCases.cs: tìm kiếm hồ sơ, gợi ý, lọc giá/đơn vị, ẩn/khóa, phân trang và sắp xếp trên SQL thật.
- NoticeCases.cs: ghi notification/outbox cùng transaction và quyền đọc/đánh dấu đã đọc qua HTTP.
- ProgressCases.cs: tổng hợp dữ liệu 1/1/1 buổi, 3/3/2 bài, TB7.00; khoảng ngày, đánh giá và phản hồi.
- HttpCases.cs: HTTP các nhóm nhu cầu tìm kiếm/liên hệ/tài khoản/quyền.
- Manifest.json: đầu vào, nhánh, kỳ vọng, phụ thuộc và căn cứ Excel.
- coverage.json: thiếu sót script đã xác định; không đổi kỳ vọng Excel để làm Pass.
- Run-Integration.ps1 / Report.py: build đúng DLL, chạy, sinh TRX và báo cáo từng nhóm.

Test không gửi thư hay notification tới người thật. Mật khẩu fixture không đưa vào báo cáo; token/cookie cần được che khi trích bằng chứng. Giữ nguyên oracle theo Excel; khi oracle khác code/SRS, phân loại sai lệch hợp đồng, không kết luận mặc định code sai.

IT-105 mô phỏng bản ghi trước chuyển đổi: chỉ trên database test v2, fixture tạm tắt trigger bảo vệ BaiTapGiao trong transaction, đặt liên kết buổi null rồi bật lại trước commit. Endpoint GanBuoi được chạy với trigger đã bật. Không tắt trigger trong lúc Assert.


