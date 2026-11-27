# Kết quả 09_Thanh_phan_khac

1 TC; 1 bộ dữ liệu; {"Passed": 1}.

| TC/DL | Unit | Input | Kết quả kỳ vọng | Thực tế | Script |
|---|---|---|---|---|---|
| UT-360/DL01 | TV-U20 | id=C1; fake relational command ghi nhận lệnh và trả 1. | Gửi đúng SELECT Id FROM dbo.LopHoc WITH (UPDLOCK,HOLDLOCK) WHERE Id={parameter}, parameter=C1; không gọi quyền/thành viên/thông báo. Không kiểm hiệu lực khóa SQL trong Unit. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:1577 |
