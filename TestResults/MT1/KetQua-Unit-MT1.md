# Kết quả Unit MT1 ngày 06/11/2026

Toàn bộ MT1: **70 ca, 70 Pass, 0 Fail, 0 Skip**. 20 ca trước đây Skip đều đã thực thi và Pass.

## Căn cứ UT-065 và UT-066

- KHTN1262717_BC2_Xaydunghethonghotroketnoigiasuvahocsinh.docx: mục 4.8 UC1.2 mô tả luồng tìm kiếm, mục 5.2–5.4 yêu cầu lấy Expected từ yêu cầu/thiết kế trước khi chạy.
- SRS-Software Requirements Specification (2).docx: P02, FR-06 và FR-07 yêu cầu gợi ý theo tiêu chí có dữ liệu, hiển thị lý do phù hợp; không dùng lịch rảnh gia sư, không tính/hiển thị điểm phần trăm.
- Tài liệu không quy định top 3, thứ tự theo ngày tạo hay phải có đúng lý do Cùng khối lớp. Không dùng những giả định đó làm tiêu chí Pass/Fail.

## Phạm vi đã kiểm

UT-065 kiểm chiều bên học: môn/hình thức và không dùng lịch gia sư. P1 cùng môn/hình thức được gợi ý dù không có lịch; P2 cùng môn khác hình thức không bị gán lý do hình thức; P3 khác môn bị loại. Không có lý do lịch hoặc điểm %.

UT-066 kiểm chiều gia sư: môn đăng ký khớp môn đại diện hoặc môn liên kết của nhu cầu. Môn 101 trả N1/N2, loại N3; môn 103 hoặc không có môn trả rỗng. Bộ lọc cùng môn được tách nguyên trạng từ service để kiểm trong bộ nhớ.

Đây là kiểm thử hai quy tắc cụ thể, không xác nhận đầy đủ FR-06 về phạm vi dạy, khu vực, học phí theo đơn vị, lọc công khai/trạng thái, hay tính đúng của truy vấn SQL. Những nội dung đó cần các case riêng ở cấp phù hợp. UT-067 vẫn kiểm phần tính lý do, không thay IT-060.

## Điều chỉnh kết luận trước

Kết luận UT-065 Fail và UT-066 Pass dựa trên commit demo trước đây không được dùng để đánh giá tuân thủ thiết kế. Đã thay TC và Expected bằng các quy tắc có căn cứ ở hai tài liệu người dùng xác nhận, chạy lại toàn bộ MT1 và ghi kết quả mới. Không sửa chức năng để ép test Pass; phép trích bộ lọc không thay đổi quy tắc chạy.

## Minh chứng và tái hiện

TRX: MT1-unit.trx. Log từng case: Tung-TC/MT1-UT-xxx.txt. CSV/HTML/TXT được cập nhật tại thư mục hiện có. File Excel (3) được cập nhật trực tiếp, gồm TC và sheet Kết quả Unit.

Lệnh:
dotnet test tests/Unit/EduConnect.UnitTests.csproj --no-restore -p:UseAppHost=false -p:OutputPath=bin/MT1-validation/ --filter FullyQualifiedName~MT1 --results-directory TestResults/MT1 --logger "trx;LogFileName=MT1-unit.trx"

Không dùng SQL Server trong lần chạy Unit này.
