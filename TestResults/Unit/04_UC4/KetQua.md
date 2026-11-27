# Kết quả 04_UC4

3 TC; 3 bộ dữ liệu; {"Passed": 3}.

| TC/DL | Unit | Input | Kết quả kỳ vọng | Thực tế | Script |
|---|---|---|---|---|---|
| UT-066/DL01 | DG-U12 | Seed đánh giá D1 người nhận U1 HoatDong/không bị ẩn;D2 người nhận U2 BiKhoa;D3 người nhận U1/AnNoiDung chưa gỡ;D4 người nhận U1/AnNoiDung đã gỡ. | Query trả đúng D1,D4; loại D2,D3; không thay đổi hoặc lưu đánh giá. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:293 |
| UT-067/DL01 | DG-U12 | Một đánh giá D1 có NguoiNhanId=U1; bảng Users không có U1, biện pháp rỗng. | Query rỗng. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:297 |
| UT-068/DL01 | DG-U16 | value.Id=D1,SoSao=4,NhanXet="Dạy dễ hiểu",PhanHoi="Cảm ơn";actor=U1;now=T=2026-11-24T10:00Z; fakeAdd. | Thêm đúng 1 LichSuDanhGiaDoiTac có DanhGiaId=D1,SoSao=4,NhanXet="Dạy dễ hiểu",PhanHoi="Cảm ơn",NguoiSuaId=U1,ThayDoiLuc=T. Không SaveChanges, không thay value. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:301 |
