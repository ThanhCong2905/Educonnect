# Unit MT1

Các test thực thi không dùng SQL Server, không tạo DbContext và không đọc connection string.

- NhuCauRulesTests.cs: UT-001/002 có sẵn.
- LichRanhTests.cs: UT-003–013.
- HocPhiTests.cs: UT-014–025.
- PhienBanTests.cs: UT-026–030.
- InputValidationTests.cs: UT-031–046 và UT-068–070.
- TimKiemControllerTests.cs: UT-049, nhánh ModelState sai dừng trước phụ thuộc.
- ChuaCoDieuKienCoLapTests.cs: 20 mã còn lại đánh Skip kèm lý do, chưa phải ca có thể thực thi.

Chạy tại gốc repo:
dotnet test tests/Unit/EduConnect.UnitTests.csproj --filter FullyQualifiedName~MT1 --results-directory TestResults/MT1 --logger "trx;LogFileName=MT1-unit.trx"

Các ca skipped cần interface/fake cho service và người dùng hiện tại, hoặc tách logic thuần khỏi truy vấn. Chỉ bỏ Skip sau khi có assertion và điểm gọi cô lập tương ứng.

UT-022 giữ Expected của kịch bản: decimal(18,2) tối đa 9999999999999999.99. Helper hiện chặn ở 9999999999999999 nên test có thể Fail. Cần xác nhận quy tắc với thiết kế.
