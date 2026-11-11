-- Add class ending metadata required by the current EF model.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.LopHoc','KetThucLuc') IS NULL
    ALTER TABLE dbo.LopHoc ADD KetThucLuc datetimeoffset NULL;
IF COL_LENGTH('dbo.LopHoc','CachKetThuc') IS NULL
    ALTER TABLE dbo.LopHoc ADD CachKetThuc nvarchar(32) NULL;
IF COL_LENGTH('dbo.YeuCauHuyKetNoi','NguoiPhanHoiId') IS NULL
    ALTER TABLE dbo.YeuCauHuyKetNoi ADD NguoiPhanHoiId uniqueidentifier NULL;
COMMIT;
GO
