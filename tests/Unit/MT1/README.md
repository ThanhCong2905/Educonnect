# Unit MT1

Các test thực thi không dùng SQL Server, không tạo DbContext và không đọc connection string.

- NhuCauRulesTests.cs: UT-001/002 có sẵn.
- LichRanhTests.cs: UT-003–013.
- HocPhiTests.cs: UT-014–025.
- PhienBanTests.cs: UT-026–030.
- InputValidationTests.cs: UT-031–046 và UT-068–070.
- TimKiemControllerTests.cs: UT-049, nhánh ModelState sai dừng trước phụ thuộc.
- ControllerIsolationTests.cs: UT-050–064, dùng interface và strict fake. Lời gọi không cấu hình làm test thất bại.
- TimKiemServiceIsolationTests.cs: UT-047/048 và UT-065–067. Không có DbContext; phần tính gợi ý dùng chính GoiYRules được gọi từ service.

Chạy tại gốc repo:
dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~MT1 --results-directory TestResults/MT1 --logger "trx;LogFileName=MT1-unit.trx"

Đã thay toàn bộ 20 test giữ chỗ bằng test thực thi có assertion. Controller được gọi trực tiếp, không kiểm middleware, antiforgery, truy vấn SQL hoặc quy tắc nghiệp vụ bên trong service đã fake.

Căn cứ UT-065/066: `KHTN1262717_BC2_Xaydunghethonghotroketnoigiasuvahocsinh.docx`, mục 4.8 UC1.2 và 5.2–5.4; `SRS-Software Requirements Specification (2).docx`, P02, FR-06/07. Expected phải lấy từ yêu cầu và thiết kế, không dùng commit demo hoặc hành vi code làm chuẩn. Bản SRS này yêu cầu lý do phù hợp, không tính/hiển thị điểm phần trăm và không dùng lịch rảnh gia sư.

UT-065 kiểm chiều bên học: gợi ý theo môn và hình thức; không cần lịch rảnh gia sư, không sinh lý do lịch hoặc điểm %. UT-066 kiểm chiều gia sư: môn đã đăng ký khớp môn đại diện hoặc môn liên kết của nhu cầu; không khớp/không có môn trả rỗng. Không áp đặt thứ tự hoặc giới hạn 3 vì hai tài liệu chưa đặc tả những Expected đó. Đây là phạm vi kiểm hai quy tắc cụ thể, không xác nhận đầy đủ mọi tiêu chí FR-06 (phạm vi dạy, khu vực, học phí theo đơn vị).

UT-057 dùng API TimKiemAsync hiện tại và xác nhận filter, Items, Total thay vì yêu cầu cùng instance SearchPage của API cũ. UT-067 chỉ kiểm phần tính lý do trong bộ nhớ, không thay cho IT-060. Các kiểm tra công khai, trạng thái, SQL và tác động nghiệp vụ cần Integration/System riêng.

Chạy khi website đang khóa thư mục build:
dotnet test tests/Unit/EduConnect.UnitTests.csproj --no-restore -p:UseAppHost=false -p:OutputPath=bin/MT1-validation/ --filter FullyQualifiedName~MT1 --results-directory TestResults/MT1 --logger "trx;LogFileName=MT1-unit.trx"
