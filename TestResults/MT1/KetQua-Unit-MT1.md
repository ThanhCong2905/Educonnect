# Kết quả Unit MT1

Đã chạy xUnit trên project tests/Unit/EduConnect.UnitTests.csproj, không SQL Server và không connection string.

70 mã TC được nhận diện. 50 ca có mã thực thi: 49 Pass, 1 Fail. 20 mã còn lại là Skip có lý do, chưa có triển khai cô lập.

Fail MT1-UT-022: kịch bản dùng decimal(18,2) tối đa 9999999999999999.99, nhưng Rules.FeeCheck ở Infrastructure/BusinessRules.cs dòng 63 so với 9999999999999999. Test giữ Expected và ghi lỗi thực tế.

Các mã chưa có triển khai cô lập: UT-047, UT-048, UT-050–067. Service và người dùng hiện tại dùng lớp sealed và truy vấn AppDbContext trực tiếp. Các ca Controller cần điểm thay thế/fake; UT-065–067 cần logic gợi ý có thể gọi độc lập và dữ liệu/Expected cụ thể. Các test Skip hiện chỉ dùng để báo danh sách thiếu trong runner.

Tổ chức
- NhuCauRulesTests: hai ca sẵn có.
- LichRanhTests: UT-003–013.
- HocPhiTests: UT-014–025.
- PhienBanTests: UT-026–030.
- InputValidationTests: UT-031–046, UT-068–070.
- TimKiemControllerTests: UT-049.
- ChuaCoDieuKienCoLapTests: danh sách Skip.
- README: hướng dẫn chạy tại repo.

Tệp minh chứng: MT1-Unit-results/MT1-unit-verified.trx.

Lệnh chạy tại gốc repo:
dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~MT1 --results-directory TestResults/MT1 --logger "trx;LogFileName=MT1-unit.trx"
