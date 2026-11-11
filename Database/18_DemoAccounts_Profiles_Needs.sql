/* DỮ LIỆU DEMO: 15 gia sư, 15 học sinh, 10 phụ huynh.
   Database EduConnect đã nâng cấp đầy đủ. Chạy toàn bộ file.
   Mật khẩu chung: EduConnect@2026; hash Identity V3 với salt riêng.
   Email: giasu01..15, hocsinh01..15, phuhuynh01..10 @demo.educonnect.test.
   10 hồ sơ đã duyệt/công khai, 5 chờ duyệt/ẩn; 25 nhu cầu đang mở.
   Dừng và rollback nếu email demo đã tồn tại, không ghi đè dữ liệu.
   Đặt @GhiDuLieu=0 để kiểm thử mà không lưu dữ liệu.
*/
SET NOEXEC OFF;
GO
SET PARSEONLY OFF;
GO
USE [EduConnect];
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
DECLARE @GhiDuLieu bit=1;
DECLARE @Now datetimeoffset=SYSDATETIMEOFFSET();
BEGIN TRY
BEGIN TRANSACTION;
DECLARE @Users TABLE(Stt int PRIMARY KEY,VaiTro nvarchar(32),So int,Email nvarchar(256),HoTen nvarchar(256),PasswordHash nvarchar(max),Id uniqueidentifier,HoSoId uniqueidentifier,NhuCauId uniqueidentifier);
INSERT @Users VALUES
(1,N'GiaSu',1,N'giasu01@demo.educonnect.test',N'Nguyễn Minh Anh',N'AQAAAAIAAYagAAAAEFpTO9Gk3MW4GMbOTg2fl2OtyirsTFC+cOcq7VFv2J4QHqJc97o0HQy+6mvad0QVow==',NEWID(),NEWID(),NEWID()),
(2,N'GiaSu',2,N'giasu02@demo.educonnect.test',N'Trần Quốc Bảo',N'AQAAAAIAAYagAAAAEPpu6ct8co3mhGd2Rs4Ak2Giw7C3vtAMobqi0qer1oFAHuv071YYHooBR1PtWicsSA==',NEWID(),NEWID(),NEWID()),
(3,N'GiaSu',3,N'giasu03@demo.educonnect.test',N'Lê Thu Hà',N'AQAAAAIAAYagAAAAEG+YlcDoFLAPeBdr0ULLQWL30VWzDTG/MrsQkUGwYA+OtUcYM68sjFs6UGsDHpE/yw==',NEWID(),NEWID(),NEWID()),
(4,N'GiaSu',4,N'giasu04@demo.educonnect.test',N'Phạm Hoàng Nam',N'AQAAAAIAAYagAAAAELCgAjTYGu+eXDClwM88rYuLcmGez1JR2fRtnWNaYIu4GKq5L5elKDbZod58bXHwzg==',NEWID(),NEWID(),NEWID()),
(5,N'GiaSu',5,N'giasu05@demo.educonnect.test',N'Hoàng Ngọc Mai',N'AQAAAAIAAYagAAAAECmOkzle+aoR5zO2qIQl13UyWM0RtO8mHjKy5g0ddcK4OGfOb1csb+Mg+WKTDgs2tA==',NEWID(),NEWID(),NEWID()),
(6,N'GiaSu',6,N'giasu06@demo.educonnect.test',N'Vũ Đức Huy',N'AQAAAAIAAYagAAAAEM2TMh7gusscdj8E6B+G0xEdzmtK6DiRtvBDh7hHF/3AFeHgR3dbYHxP6heKmufaIA==',NEWID(),NEWID(),NEWID()),
(7,N'GiaSu',7,N'giasu07@demo.educonnect.test',N'Đặng Thanh Trúc',N'AQAAAAIAAYagAAAAECUMAfGsd/Vst4zyFXUKulLc+D50Df4FY3Ne1NL9dpl4z5KzERvxlIFn9BUG6TNwNA==',NEWID(),NEWID(),NEWID()),
(8,N'GiaSu',8,N'giasu08@demo.educonnect.test',N'Bùi Gia Hưng',N'AQAAAAIAAYagAAAAEI4xkjnilUUDU7mQJ3aRnyOa6zok+cMvhUdvEwUKlBSJ+RQK1H6ECudn4KEjdFeWog==',NEWID(),NEWID(),NEWID()),
(9,N'GiaSu',9,N'giasu09@demo.educonnect.test',N'Đỗ Khánh Linh',N'AQAAAAIAAYagAAAAENkEfBRlGJvduEeq8NP1r4HOZACY9aWTrBl0N+0YB1VbuG6+fHgMA1xD1myMaNgllA==',NEWID(),NEWID(),NEWID()),
(10,N'GiaSu',10,N'giasu10@demo.educonnect.test',N'Hồ Tuấn Kiệt',N'AQAAAAIAAYagAAAAEIHkptw7GgRRH2n1tSKHb4ot4p0OEywsRg1D7qhL4MSpf5YgF3o0wjN4uJopx4XiXg==',NEWID(),NEWID(),NEWID()),
(11,N'GiaSu',11,N'giasu11@demo.educonnect.test',N'Ngô Phương Thảo',N'AQAAAAIAAYagAAAAEF4t82dCsmsytziM2b3DehSasxrtnllIkztNEV1UuDd1B5FW7/flFMsBmgLdBfo1ZA==',NEWID(),NEWID(),NEWID()),
(12,N'GiaSu',12,N'giasu12@demo.educonnect.test',N'Dương Hải Đăng',N'AQAAAAIAAYagAAAAENYfavZmWA1QCI5O8LSsiL0jnUBRaFaPWv/wmsUmpWgMk+gOLBXaDmqd/ZN9e1jO3A==',NEWID(),NEWID(),NEWID()),
(13,N'GiaSu',13,N'giasu13@demo.educonnect.test',N'Lý Bảo Ngọc',N'AQAAAAIAAYagAAAAEGJZrOANyCZ7Uf2lui32omwhaoxwiEcMXO75svuRlKWbw2F9mpOoEcnBNPksQCai0g==',NEWID(),NEWID(),NEWID()),
(14,N'GiaSu',14,N'giasu14@demo.educonnect.test',N'Đinh Minh Khang',N'AQAAAAIAAYagAAAAEOk/lUiitkSjzGhEXPIFw+LoQ+s3F7LZu/3PcTr9eIHk14xFwn7dJn0ELxJhgW/S7g==',NEWID(),NEWID(),NEWID()),
(15,N'GiaSu',15,N'giasu15@demo.educonnect.test',N'Trịnh Thùy Dung',N'AQAAAAIAAYagAAAAEMmTnQw+H3OwhSjnmJdYtwQOR6BvRnqws3q9jhUwz5N58SWWbahFYJTS6xvvcFYRlA==',NEWID(),NEWID(),NEWID()),
(16,N'HocSinh',1,N'hocsinh01@demo.educonnect.test',N'Nguyễn Nhật Minh',N'AQAAAAIAAYagAAAAEIWwRIFMowlMKuRIEvnlLnOiBm/UrWSeBiDVIuMWwCUZo0XltTjDjV2E7JPkXj5KrA==',NEWID(),NEWID(),NEWID()),
(17,N'HocSinh',2,N'hocsinh02@demo.educonnect.test',N'Trần Anh Thư',N'AQAAAAIAAYagAAAAEAj6qOeyhI6HG+dWncmiFqU5rg1xWVGe3WyNvI5Q5tUYOtDAbJo6M/4NRq7pOQ9aNg==',NEWID(),NEWID(),NEWID()),
(18,N'HocSinh',3,N'hocsinh03@demo.educonnect.test',N'Lê Quang Huy',N'AQAAAAIAAYagAAAAEH+z1fQBu3ERPbi6ELXQ4CsnVQA7rQc19TxPf0q67o6fCTLaJuY17mpUA2K1IROjDw==',NEWID(),NEWID(),NEWID()),
(19,N'HocSinh',4,N'hocsinh04@demo.educonnect.test',N'Phạm Ngọc Hân',N'AQAAAAIAAYagAAAAEKQjqp53AB4JktYz0po8EKnuOU+kKulAogFjDxZnpD/JMGcLM7jr9j3hJuFLH3C6pw==',NEWID(),NEWID(),NEWID()),
(20,N'HocSinh',5,N'hocsinh05@demo.educonnect.test',N'Hoàng Đức Anh',N'AQAAAAIAAYagAAAAEIql9DXbmG0WIr8uFelKO3bMdL9Fb5lqPJzDDzTiddEOn77aqSDynYnyJuyYK1Eqag==',NEWID(),NEWID(),NEWID()),
(21,N'HocSinh',6,N'hocsinh06@demo.educonnect.test',N'Vũ Minh Châu',N'AQAAAAIAAYagAAAAEA5BFfkWew+qgCqTkcZ+KcY8Yyw9aNpW58QQSyOslHZ21f9B7HnAb9eqRMUNgRCq1A==',NEWID(),NEWID(),NEWID()),
(22,N'HocSinh',7,N'hocsinh07@demo.educonnect.test',N'Đặng Hoàng Phúc',N'AQAAAAIAAYagAAAAEMPO9DsF/wEkuSShzz0W3LVr7Joji9ewJLM7wDxAQC1ACIC4RBTii26FcB0LUzyFtg==',NEWID(),NEWID(),NEWID()),
(23,N'HocSinh',8,N'hocsinh08@demo.educonnect.test',N'Bùi Thanh Tâm',N'AQAAAAIAAYagAAAAEAcVv/+/WxNPZB0zeYoUM1tUvf4EXa+eH5wgS3Oazp1SWf8Tnd0MpQdyjUyLQzo2wQ==',NEWID(),NEWID(),NEWID()),
(24,N'HocSinh',9,N'hocsinh09@demo.educonnect.test',N'Đỗ Gia Bảo',N'AQAAAAIAAYagAAAAECASPQV8ATEJ1VwS1zCHyCqmiHFnsdvqQEGxdWfA/RxOwKUHrag+ZC2INiKPjX4eyw==',NEWID(),NEWID(),NEWID()),
(25,N'HocSinh',10,N'hocsinh10@demo.educonnect.test',N'Hồ Khánh Vy',N'AQAAAAIAAYagAAAAECsNT0ezFVOYuzh5488bFruW+h78XPDVfe0r+1wNTDwJGzxZ81r1VB3S8KdKwpNiCw==',NEWID(),NEWID(),NEWID()),
(26,N'HocSinh',11,N'hocsinh11@demo.educonnect.test',N'Ngô Tuấn Anh',N'AQAAAAIAAYagAAAAEFrJjmBDGfq1QsC+kL4PIgHcVfFGO9/Bvhsrfi4iLUw72tlSfuBOLRnHtgroV10Rbg==',NEWID(),NEWID(),NEWID()),
(27,N'HocSinh',12,N'hocsinh12@demo.educonnect.test',N'Dương Thảo Nguyên',N'AQAAAAIAAYagAAAAEHALMfFhAvkx7/LlfwSvgcq5gjs3I99XOt0uj7NvuXSuvA+Q/SDSYu2wXCi48JWfFQ==',NEWID(),NEWID(),NEWID()),
(28,N'HocSinh',13,N'hocsinh13@demo.educonnect.test',N'Lý Minh Quân',N'AQAAAAIAAYagAAAAEDH2mfhpNGh1Y7qDtNOkp3zMApFMlgVgIux9YmfENvIpBBZXBavpvInwU7B4yG45QQ==',NEWID(),NEWID(),NEWID()),
(29,N'HocSinh',14,N'hocsinh14@demo.educonnect.test',N'Đinh Ngọc Ánh',N'AQAAAAIAAYagAAAAEIuvle41uRAwHAuxKHwZ8WDc3bOL4AHNi/+eLFpjldhI+vrUxYpzcqyAuKTOcrK6Mg==',NEWID(),NEWID(),NEWID()),
(30,N'HocSinh',15,N'hocsinh15@demo.educonnect.test',N'Trịnh Hải Nam',N'AQAAAAIAAYagAAAAEGCaCqnemae05riBKWwYoC9iDz0DBfvRtdeD/hJ4UAvBIxeY9VFS5uH19O3JSDAjBA==',NEWID(),NEWID(),NEWID()),
(31,N'PhuHuynh',1,N'phuhuynh01@demo.educonnect.test',N'Nguyễn Văn Thành',N'AQAAAAIAAYagAAAAENWfpYejudLuayMVGu6/lHRdDy119mXhB1FIE1EwEKnqx1qqG+ffEImyKldesmWtOQ==',NEWID(),NEWID(),NEWID()),
(32,N'PhuHuynh',2,N'phuhuynh02@demo.educonnect.test',N'Trần Thị Hương',N'AQAAAAIAAYagAAAAEMhX/2ZqkNC/k14hGHM1UVIjkibYdxWGyw1UJqMEdK9Iw85nSkeV2lI7J71FjrW9CQ==',NEWID(),NEWID(),NEWID()),
(33,N'PhuHuynh',3,N'phuhuynh03@demo.educonnect.test',N'Lê Quốc Dũng',N'AQAAAAIAAYagAAAAECi4gr7zg/+fNZDb+znbAZXVyNhnbkyKpgpXN2sJRitqax9ukKftInvzrDjJZST6AQ==',NEWID(),NEWID(),NEWID()),
(34,N'PhuHuynh',4,N'phuhuynh04@demo.educonnect.test',N'Phạm Thị Lan',N'AQAAAAIAAYagAAAAEAQPkwpPz2OgbJ265NYcXy/L8u6C3yp1P1k8sd+Sm+n3tJEimde0lCUwvDzv7vRtAA==',NEWID(),NEWID(),NEWID()),
(35,N'PhuHuynh',5,N'phuhuynh05@demo.educonnect.test',N'Hoàng Minh Tuấn',N'AQAAAAIAAYagAAAAEEiwRu9CrevyavP6ehae6cGctXxHGNOoh+GcSRYe3EkASfLDf4jgnsN2a7LfSV56Hw==',NEWID(),NEWID(),NEWID()),
(36,N'PhuHuynh',6,N'phuhuynh06@demo.educonnect.test',N'Vũ Thu Trang',N'AQAAAAIAAYagAAAAEH5+rvHJMXM7inXv8f26BgiVt/+QT/5QBffzG2xym4aqciG77j3qno4yI0Qvwx1eMg==',NEWID(),NEWID(),NEWID()),
(37,N'PhuHuynh',7,N'phuhuynh07@demo.educonnect.test',N'Đặng Văn Hải',N'AQAAAAIAAYagAAAAEBUX1xNylY5MvQz1dwwC7G8h0ASEtcAVJzqsCsya4R4ARj79D5AClARERLXkUraFGQ==',NEWID(),NEWID(),NEWID()),
(38,N'PhuHuynh',8,N'phuhuynh08@demo.educonnect.test',N'Bùi Thị Hoa',N'AQAAAAIAAYagAAAAEM5+o81+FS1k2Sw4sgMQnCUwNUXNM8sHLLGUmICcic724SrRJF7+fRUHncTdQlw9xg==',NEWID(),NEWID(),NEWID()),
(39,N'PhuHuynh',9,N'phuhuynh09@demo.educonnect.test',N'Đỗ Quang Vinh',N'AQAAAAIAAYagAAAAEPDgMi7j7sPj4KffUQcPaheh7TnQtLUpjU4cy2d0gL8gHYhh3GBE+Np1TNthrLJL1w==',NEWID(),NEWID(),NEWID()),
(40,N'PhuHuynh',10,N'phuhuynh10@demo.educonnect.test',N'Hồ Thanh Hằng',N'AQAAAAIAAYagAAAAEMhmauq2G8Fk/FhtwxXE0jCwVsnF52aRt/37KE+riiXISF7m6ysKb9dGnndW4JgVdQ==',NEWID(),NEWID(),NEWID());
IF EXISTS(SELECT 1 FROM @Users s JOIN dbo.AspNetUsers u ON u.NormalizedEmail=UPPER(s.Email))
 THROW 51200,N'Email demo đã tồn tại. Không thêm trùng hoặc ghi đè tài khoản.',1;
IF (SELECT COUNT(*) FROM dbo.AspNetRoles WHERE NormalizedName IN('GIASU','HOCSINH','PHUHUYNH'))<>3
 THROW 51201,N'Thiếu vai trò GiaSu/HocSinh/PhuHuynh. Hãy cài đầy đủ database trước.',1;
DECLARE @Subjects TABLE(Stt int PRIMARY KEY,TenMon nvarchar(256),TenCu nvarchar(256),Id int NULL);
INSERT @Subjects(Stt,TenMon,TenCu) VALUES
(1,N'Tiếng Việt',N'Tiếng Việt'),
(2,N'Ngữ Văn',N'Ngữ Văn'),
(3,N'Tiếng Anh',N'Tiếng Anh'),
(4,N'Toán',N'Toán Học'),
(5,N'Vật lý',N'Vật Lý'),
(6,N'Hóa học',N'Hóa Học'),
(7,N'Sinh học',N'Sinh học'),
(8,N'Lịch sử',N'Lịch Sử'),
(9,N'Địa lý',N'Địa Lý'),
(10,N'Tin học',N'Tin học'),
(11,N'Công nghệ',N'Công nghệ'),
(12,N'Giáo dục công dân',N'GDCD'),
(13,N'Giáo dục kinh tế và pháp luật',N'Giáo dục kinh tế và pháp luật'),
(14,N'Khoa học tự nhiên',N'Khoa học tự nhiên'),
(15,N'Âm nhạc',N'Âm nhạc'),
(16,N'Mỹ thuật',N'Mỹ thuật');
UPDATE s SET Id=(SELECT TOP(1) m.Id FROM dbo.MonHoc m WHERE m.TenMon COLLATE Vietnamese_100_CI_AI IN(s.TenMon,s.TenCu) ORDER BY m.Id) FROM @Subjects s;
INSERT dbo.MonHoc(TenMon,DangSuDung) SELECT TenMon,1 FROM @Subjects WHERE Id IS NULL;
UPDATE s SET Id=(SELECT TOP(1) m.Id FROM dbo.MonHoc m WHERE m.TenMon COLLATE Vietnamese_100_CI_AI IN(s.TenMon,s.TenCu) ORDER BY m.Id) FROM @Subjects s WHERE Id IS NULL;
IF EXISTS(SELECT 1 FROM @Subjects s JOIN dbo.MonHoc m ON m.Id=s.Id WHERE m.DangSuDung=0)
 THROW 51202,N'Môn học cần dùng đang bị tắt. Hãy kiểm tra danh mục môn học.',1;
INSERT dbo.AspNetUsers(Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount,HoTen,TrangThai)
SELECT Id,Email,UPPER(Email),Email,UPPER(Email),1,PasswordHash,CONVERT(nvarchar(36),NEWID()),CONVERT(nvarchar(36),NEWID()),0,0,1,0,HoTen,'HoatDong' FROM @Users;
INSERT dbo.AspNetUserRoles(UserId,RoleId) SELECT u.Id,r.Id FROM @Users u JOIN dbo.AspNetRoles r ON r.NormalizedName=UPPER(u.VaiTro);
INSERT dbo.GiaSu(TaiKhoanId) SELECT Id FROM @Users WHERE VaiTro='GiaSu';
INSERT dbo.HocSinh(TaiKhoanId) SELECT Id FROM @Users WHERE VaiTro='HocSinh';
INSERT dbo.PhuHuynh(TaiKhoanId) SELECT Id FROM @Users WHERE VaiTro='PhuHuynh';
DECLARE @Teaching TABLE(SoGiaSu int,MonStt int,PRIMARY KEY(SoGiaSu,MonStt));
-- Approved tutors cover all subjects; pending tutors also have teaching subjects.
INSERT @Teaching SELECT ((Stt-1)%10)+1,Stt FROM @Subjects;
INSERT @Teaching VALUES(11,1),(12,3),(13,4),(14,10),(15,16);
DECLARE @Reviewer uniqueidentifier=(SELECT TOP(1) u.Id FROM dbo.AspNetUsers u JOIN dbo.AspNetUserRoles ur ON ur.UserId=u.Id JOIN dbo.AspNetRoles r ON r.Id=ur.RoleId WHERE r.NormalizedName='QUANTRI' AND u.TrangThai='HoatDong' ORDER BY u.Id);
INSERT dbo.HoSoGiaSu(Id,GiaSuId,TrinhDo,ChuyenMon,KinhNghiem,MoTa,KhuVuc,TrangThaiDuyet,DangHienThi,DayTrucTuyen,DayTrucTiep,GuiDuyetLuc,DuyetLuc,NguoiDuyetId)
SELECT u.HoSoId,u.Id,N'Cử nhân Sư phạm',d.MonDay,CONCAT(2+u.So%6,N' năm giảng dạy và gia sư'),CONCAT(N'Hồ sơ demo của ',u.HoTen,N'. Chuyên dạy ',d.MonDay,N'; xây dựng lộ trình theo năng lực, củng cố kiến thức và theo dõi tiến bộ mỗi tuần.'),N'Thành phố Hồ Chí Minh',CASE WHEN u.So<=10 THEN 'DaDuyet' ELSE 'ChoDuyet' END,CASE WHEN u.So<=10 THEN 1 ELSE 0 END,1,1,DATEADD(day,-2,@Now),CASE WHEN u.So<=10 THEN DATEADD(day,-1,@Now) END,CASE WHEN u.So<=10 THEN @Reviewer END
FROM @Users u CROSS APPLY(SELECT STRING_AGG(s.TenMon,N', ') AS MonDay FROM @Teaching t JOIN @Subjects s ON s.Stt=t.MonStt WHERE t.SoGiaSu=u.So) d WHERE u.VaiTro='GiaSu';
INSERT dbo.HoSoGiaSuMonDay(HoSoGiaSuId,MonHocId,PhamViDay,HocPhiDeXuat,DonViHocPhi)
SELECT u.HoSoId,s.Id,N'Cơ bản, nâng cao và ôn tập theo chương trình phổ thông',120000+u.So*10000,'VND/Buoi' FROM @Users u JOIN @Teaching t ON t.SoGiaSu=u.So JOIN @Subjects s ON s.Stt=t.MonStt WHERE u.VaiTro='GiaSu';
INSERT dbo.NhuCauHoc(Id,ChuSoHuuId,LoaiNhuCau,MonHocId,KhoiLop,HocLuc,MucTieu,SoBuoiTuan,HinhThucHoc,KhuVuc,DiaChiChiTiet,HocPhiDuKien,TenNguoiHoc,TuoiNguoiHoc,TrangThai,DonViHocPhi,TaoLuc,XacNhanConNhuCauLuc)
SELECT u.NhuCauId,u.Id,'TimGiaSu',s.Id,CASE WHEN s.Stt=1 THEN N'Lớp 5' WHEN s.Stt=13 THEN N'Lớp 11' WHEN s.Stt=12 THEN N'Lớp 9' WHEN s.Stt=14 THEN N'Lớp 8' ELSE N'Lớp 10' END,N'Trung bình',CONCAT(N'Củng cố kiến thức ',s.TenMon,N', luyện bài tập và cải thiện kết quả học tập.'),2,CASE WHEN u.Stt%2=0 THEN 'TrucTuyen' ELSE 'TrucTiep' END,N'Thành phố Hồ Chí Minh',CASE WHEN u.Stt%2=1 THEN N'Quận Bình Thạnh, Thành phố Hồ Chí Minh' END,180000,CASE WHEN u.VaiTro='HocSinh' THEN u.HoTen ELSE CONCAT(N'Con của ',u.HoTen) END,CASE WHEN s.Stt=1 THEN 10 WHEN s.Stt=13 THEN 16 WHEN s.Stt=12 THEN 14 WHEN s.Stt=14 THEN 13 ELSE 15 END,'DangMo','VND/Buoi',@Now,@Now
FROM @Users u JOIN @Subjects s ON s.Stt=((u.Stt-16)%16)+1 WHERE u.VaiTro IN('HocSinh','PhuHuynh');
INSERT dbo.NhuCauMonHoc(NhuCauId,MonHocId) SELECT n.Id,n.MonHocId FROM dbo.NhuCauHoc n JOIN @Users u ON u.NhuCauId=n.Id WHERE u.VaiTro IN('HocSinh','PhuHuynh');
INSERT dbo.KhungGioRanh(Id,NhuCauId,ThuTrongTuan,GioBatDau,GioKetThuc)
SELECT NEWID(),u.NhuCauId,v.Thu,'18:00','19:30' FROM @Users u CROSS JOIN(VALUES(2),(4))v(Thu) WHERE u.VaiTro IN('HocSinh','PhuHuynh');
INSERT dbo.LuotDangNhuCau(Id,NhuCauId,TaiKhoanId,MaThaoTac,HanhDong,DangLuc,NgayDang)
SELECT NEWID(),NhuCauId,Id,NEWID(),'Dang',@Now,CONVERT(date,SWITCHOFFSET(@Now,'+07:00')) FROM @Users WHERE VaiTro IN('HocSinh','PhuHuynh');
IF (SELECT COUNT(*) FROM dbo.AspNetUsers a JOIN @Users u ON u.Id=a.Id)<>40 THROW 51203,N'Sai số lượng tài khoản.',1;
IF (SELECT COUNT(*) FROM dbo.HoSoGiaSu h JOIN @Users u ON u.Id=h.GiaSuId WHERE h.TrangThaiDuyet='DaDuyet')<>10 THROW 51204,N'Sai số lượng hồ sơ đã duyệt.',1;
IF (SELECT COUNT(*) FROM dbo.HoSoGiaSu h JOIN @Users u ON u.Id=h.GiaSuId WHERE h.TrangThaiDuyet='ChoDuyet')<>5 THROW 51205,N'Sai số lượng hồ sơ chờ duyệt.',1;
IF (SELECT COUNT(*) FROM dbo.NhuCauHoc n JOIN @Users u ON u.Id=n.ChuSoHuuId)<>25 THROW 51206,N'Sai số lượng nhu cầu học.',1;
IF EXISTS(SELECT 1 FROM @Subjects s WHERE NOT EXISTS(SELECT 1 FROM dbo.HoSoGiaSuMonDay d JOIN @Users u ON u.HoSoId=d.HoSoGiaSuId WHERE d.MonHocId=s.Id AND u.So<=10)) THROW 51207,N'Chưa phân bổ đủ môn cho hồ sơ công khai.',1;
IF EXISTS(SELECT 1 FROM @Subjects s WHERE NOT EXISTS(SELECT 1 FROM dbo.NhuCauMonHoc m JOIN @Users u ON u.NhuCauId=m.NhuCauId WHERE m.MonHocId=s.Id)) THROW 51208,N'Chưa phân bổ đủ môn cho nhu cầu học.',1;
SELECT VaiTro,COUNT(*) AS SoTaiKhoan FROM @Users GROUP BY VaiTro;
SELECT s.TenMon,COUNT(DISTINCT t.SoGiaSu) AS SoGiaSu FROM @Subjects s JOIN @Teaching t ON t.MonStt=s.Stt GROUP BY s.Stt,s.TenMon ORDER BY s.Stt;
SELECT Email,HoTen,VaiTro FROM @Users ORDER BY Stt;
IF @GhiDuLieu=1 BEGIN COMMIT; PRINT N'Đã lưu 40 tài khoản, 15 hồ sơ và 25 nhu cầu học.'; END
ELSE BEGIN ROLLBACK; PRINT N'Kiểm thử thành công; đã rollback, không lưu dữ liệu.'; END;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK;
 THROW;
END CATCH;
