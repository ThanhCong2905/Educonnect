SET XACT_ABORT ON;
BEGIN TRAN;
INSERT dbo.MonHoc(TenMon,DangSuDung)
SELECT s.TenMon,1 FROM (VALUES
(N'Sinh học'),
(N'Lịch sử'),
(N'Địa lý'),
(N'Giáo dục công dân'),
(N'Giáo dục kinh tế và pháp luật'),
(N'Khoa học tự nhiên'),
(N'Lịch sử và Địa lý'),
(N'Tin học'),
(N'Công nghệ'),
(N'Tiếng Việt'),
(N'Khoa học'),
(N'Tự nhiên và Xã hội'),
(N'Đạo đức'),
(N'Âm nhạc'),
(N'Mỹ thuật'),
(N'Giáo dục thể chất'),
(N'Tiếng Pháp'),
(N'Tiếng Trung'),
(N'Tiếng Nhật'),
(N'Tiếng Hàn'),
(N'Tiếng Đức'),
(N'Tiếng Tây Ban Nha'),
(N'Lập trình'),
(N'Toán cao cấp'),
(N'Xác suất thống kê')
) s(TenMon) WHERE NOT EXISTS(SELECT 1 FROM dbo.MonHoc m WITH (UPDLOCK,HOLDLOCK) WHERE m.TenMon=s.TenMon);
COMMIT;
