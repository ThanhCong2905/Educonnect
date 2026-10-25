USE [KLTN_4NghiepVu_V2];
GO
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET XACT_ABORT ON;
BEGIN TRAN;
INSERT dbo.CuocHoiThoai(LoaiHoiThoai,GiaSuId,BenHocId,LienHeId)
SELECT 'TruocKetNoi',h.GiaSuId,h.BenHocId,h.Id FROM dbo.YeuCauLienHe h WHERE h.TrangThai='ChapNhan' AND NOT EXISTS(SELECT 1 FROM dbo.CuocHoiThoai c WHERE c.LienHeId=h.Id AND c.LoaiHoiThoai='TruocKetNoi');
IF NOT EXISTS(SELECT 1 FROM dbo.KhungGioRanh WHERE NhuCauId='30000000-0000-0000-0000-000000000002')
INSERT dbo.KhungGioRanh(NhuCauId,ThuTrongTuan,GioBatDau,GioKetThuc) VALUES('30000000-0000-0000-0000-000000000002',2,'18:00','20:00'),('30000000-0000-0000-0000-000000000002',4,'18:00','20:00');
IF NOT EXISTS(SELECT 1 FROM dbo.NhuCauHoc WHERE Id='30000000-0000-0000-0000-000000000003')
BEGIN
INSERT dbo.NhuCauHoc(Id,ChuSoHuuId,LoaiNhuCau,MonHocId,KhoiLop,MucTieu,SoBuoiTuan,HinhThucHoc,HocPhiDuKien,DonViHocPhi,TenNguoiHoc,TuoiNguoiHoc,TrangThai)
VALUES('30000000-0000-0000-0000-000000000003','10000000-0000-0000-0000-000000000003','TimGiaSu',1,N'Lớp 8',N'Củng cố nền tảng Toán và luyện thói quen tự học',2,'TrucTuyen',150000,'VND/Gio',N'Người học của phụ huynh mẫu',13,'DangMo');
INSERT dbo.KhungGioRanh(NhuCauId,ThuTrongTuan,GioBatDau,GioKetThuc) VALUES('30000000-0000-0000-0000-000000000003',1,'19:00','20:30'),('30000000-0000-0000-0000-000000000003',3,'19:00','20:30');
END;
COMMIT;
GO
