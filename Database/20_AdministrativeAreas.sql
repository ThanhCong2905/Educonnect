SET XACT_ABORT ON;
BEGIN TRAN;
IF COL_LENGTH('dbo.HoSoGiaSu', 'PhuongXa') IS NULL
    ALTER TABLE dbo.HoSoGiaSu ADD PhuongXa nvarchar(256) NULL;
IF COL_LENGTH('dbo.HoSoGiaSu', 'DiaChiChiTiet') IS NULL
    ALTER TABLE dbo.HoSoGiaSu ADD DiaChiChiTiet nvarchar(500) NULL;
IF COL_LENGTH('dbo.NhuCauHoc', 'PhuongXa') IS NULL
    ALTER TABLE dbo.NhuCauHoc ADD PhuongXa nvarchar(256) NULL;
COMMIT;
GO
