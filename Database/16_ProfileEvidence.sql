SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.HoSoGiaSu','MinhChungTrinhDoId') IS NULL
    ALTER TABLE dbo.HoSoGiaSu ADD MinhChungTrinhDoId uniqueidentifier NULL, MinhChungKinhNghiemId uniqueidentifier NULL;
IF COL_LENGTH('dbo.HoSoGiaSuMonDay','MinhChungId') IS NULL
    ALTER TABLE dbo.HoSoGiaSuMonDay ADD MinhChungId uniqueidentifier NULL;
GO
IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name='FK_HoSo_MinhChungTrinhDo')
    ALTER TABLE dbo.HoSoGiaSu WITH CHECK ADD CONSTRAINT FK_HoSo_MinhChungTrinhDo FOREIGN KEY(MinhChungTrinhDoId) REFERENCES dbo.TepDinhKem(Id);
IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name='FK_HoSo_MinhChungKinhNghiem')
    ALTER TABLE dbo.HoSoGiaSu WITH CHECK ADD CONSTRAINT FK_HoSo_MinhChungKinhNghiem FOREIGN KEY(MinhChungKinhNghiemId) REFERENCES dbo.TepDinhKem(Id);
IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name='FK_MonDay_MinhChung')
    ALTER TABLE dbo.HoSoGiaSuMonDay WITH CHECK ADD CONSTRAINT FK_MonDay_MinhChung FOREIGN KEY(MinhChungId) REFERENCES dbo.TepDinhKem(Id);
COMMIT;
GO
