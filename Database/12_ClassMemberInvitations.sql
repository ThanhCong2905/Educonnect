SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.ThanhVienLopHoc', 'DongYThamGiaLuc') IS NULL
    ALTER TABLE dbo.ThanhVienLopHoc ADD DongYThamGiaLuc datetimeoffset(0) NULL;
IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_ThongBao_Profile' AND parent_object_id = OBJECT_ID('dbo.ThongBao'))
    ALTER TABLE dbo.ThongBao DROP CONSTRAINT CK_ThongBao_Profile;
ALTER TABLE dbo.ThongBao WITH CHECK ADD CONSTRAINT CK_ThongBao_Profile
    CHECK (LoaiThongBao IN ('KetNoi','LichHoc','BaiTap','ThanhVien','NhacViec','QuanTri','HoSo','LoiMoiLop'));
COMMIT;
GO
