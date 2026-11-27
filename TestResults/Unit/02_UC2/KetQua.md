# Kết quả 02_UC2

5 TC; 5 bộ dữ liệu; {"Passed": 5}.

| TC/DL | Unit | Input | Kết quả kỳ vọng | Thực tế | Script |
|---|---|---|---|---|---|
| UT-043/DL01 | UC2.1-U21 | id=R1; KhungGioDeXuat fakeK1,K2 thuộc R1 và K3 thuộc R2. | Trả đúng K1,K2; không K3; giữ nguyên thứ/giờ. Không yêu cầu thứ tự vìthiếulệnhsort. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:201 |
| UT-044/DL01 | UC2.1-U21 | id=R3; không có khung giờ thuộc R3. | Danh sách rỗng. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:205 |
| UT-045/DL01 | UC2.1-U23 | huy=ct; fake scalar reader trả duy nhất T=2026-11-24T10:00:00+00:00. | Trả chính T, giữ offset+00:00; command chứa TODATETIMEOFFSET(SYSUTCDATETIME(), '+00:00') AS Value; nhận ct. Không dùng giờ máy chạy test. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:209 |
| UT-046/DL01 | UC2.3-U14 | id=C1,version="AAAAAAAAAAE=". | Awaitném LoiNghiepVu Status409,Message="Hãy phản hồi yêu cầu kết thúc bằng phiên bản yêu cầu đang hiển thị."; không đọc ghi dữ liệu. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:213 |
| UT-047/DL01 | UC2.3-U08 | Scope factory fake trả service provider chứa context với fake command; huy=ct; command fake trả 2. | Gửi đúng một lệnh UPDATE dbo.YeuCauKetNoiLopHoc SET TrangThai='HetHan' WHERE TrangThai IN ('ChoGiaSu','ChoBenHoc') AND HetHanLuc<=SYSDATETIMEOFFSET(), token=ct; scope được dispose; không tự gọi helper bảo trì khác. Không suy diễn đã cập nhật 2 hàng CSDL thật. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:217 |
