# Kết quả 03_UC3

18 TC; 18 bộ dữ liệu; {"Passed": 18}.

| TC/DL | Unit | Input | Kết quả kỳ vọng | Thực tế | Script |
|---|---|---|---|---|---|
| UT-048/DL01 | UC3.2-U20 | suKien=[]; gọi XepCot thật trực tiếp. | List vẫn rỗng. Không tạo cột hoặc event. Không phát sinh ngoại lệ. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:221 |
| UT-049/DL01 | UC3.2-U20 | suKien=[E1{Cot=0,PhutDau=1080,PhutCuoi=1140,Lich.Id=N1}]. | E1.CotTrongNgay=0, E1.SoCot=1. Giữ nguyên E1 và số phần tử list=1. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:225 |
| UT-050/DL01 | UC3.2-U20 | Cùng ngày: E1 18:00–19:00, E2 18:30–19:30, Id N1<N2. | E1.CotTrongNgay=0, E2.CotTrongNgay=1; cả hai SoCot=2. List vẫn đúng 2 event. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:229 |
| UT-051/DL01 | UC3.2-U20 | Cùng ngày: E1 18:00–19:00, E2 19:00–20:00. | Hai event đều CotTrongNgay=0, SoCot=1. Nhóm trước được kết thúc trước khi thêm nhóm mới. Không ép foreach nhóm thành 0 khi đang có event. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:233 |
| UT-052/DL01 | UC3.2-U20 | suKien=[E1{Cot=0,PhutDau=100,PhutCuoi=110,Id=N1},E2{Cot=0,PhutDau=124,PhutCuoi=130,Id=N2}]. | E1/E2 lần lượt cột 0/1, cả hai SoCot=2; E1 chiếm hiển thị đến phút 125, không chỉ đến 110. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:237 |
| UT-053/DL01 | UC3.2-U20 | Cùng E1; E2.PhutDau=125,PhutCuoi=150. | E1/E2 đều cột 0,SoCot=1; tại 125 bắt đầu nhóm mới. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:241 |
| UT-054/DL01 | UC3.2-U20 | suKien=[E1{Cot=0,PhutDau=100,PhutCuoi=160,Id=N1},E2{Cot=1,PhutDau=100,PhutCuoi=160,Id=N2}]. | Hai ngày tách biệt: cả hai cột 0,SoCot=1. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:245 |
| UT-055/DL01 | UC3.2-U20 | Cùng ngày: E1 18:00–19:00, E2 18:30–20:00, E3 19:00–19:30; Id N1<N2<N3. | Cột E1=0, E2=1, E3=0. E3 tái dùng cột trống trong khi E2 chưa kết thúc. Cả ba SoCot=2. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:249 |
| UT-056/DL01 | UC3.2-U20 | List đầu vào theo thứ tự[E2,E1], cùng Cot=0,PhutDau=100,PhutCuoi=160; E1.Lich.Id=N1<E2.Lich.Id=N2. | E1.CotTrongNgay=0,E2.CotTrongNgay=1,cả haiSoCot=2; list nguồn không bị sắp xếp lại tại chỗ. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:253 |
| UT-057/DL01 | UC3.2-U21 | lich.TrangThai="DaLenLich",KetThucLuc=T+1h; buoi.TrangThai="DaNghi"; bayGio=T. | Trả "Đã nghỉ". | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:257 |
| UT-058/DL01 | UC3.2-U21 | lich.TrangThai="DaLenLich",KetThucLuc=T+1h; buoi.TrangThai="DaHoc"; bayGio=T. | Trả "Đã học". | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:261 |
| UT-059/DL01 | UC3.2-U21 | lich.TrangThai="DaLenLich",KetThucLuc=T+1h; buoi.TrangThai="DangDienRa"; bayGio=T. | Trả "Đang diễn ra". | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:265 |
| UT-060/DL01 | UC3.2-U21 | lich.TrangThai="DaLenLich",KetThucLuc=T+1h; buoi.TrangThai="DaHuy"; bayGio=T. | Trả "Đã hủy". | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:269 |
| UT-061/DL01 | UC3.2-U21 | lich.TrangThai="DaHuy"; buoi.TrangThai="DaHoc"; bayGio=T. | Trả "Đã hủy"; trạng thái lịch được ưu tiên hơn trạng thái buổi. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:273 |
| UT-062/DL01 | UC3.2-U21 | lich.TrangThai="DaDieuChinh"; buoi.TrangThai="DaHoc". | Trả "Đã đổi lịch". | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:277 |
| UT-063/DL01 | UC3.2-U21 | lich.TrangThai="DaLenLich",KetThucLuc=T;buoi=null;bayGio=T. | Trả "Chưa ghi nhận" vì KetThucLuc<=bayGio. | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:281 |
| UT-064/DL01 | UC3.2-U21 | lich.TrangThai="DaLenLich",KetThucLuc=T+1tick;buoi=null;bayGio=T. | Trả "Dự kiến". | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:285 |
| UT-065/DL01 | UC3.2-U21 | lich.TrangThai="DaLenLich",KetThucLuc=T-1tick;buoi.TrangThai="Khac";bayGio=T. | Trả "Chưa ghi nhận". | xUnit Passed: các assertion trong script đã đạt. | CurrentDesignTests.cs:289 |
