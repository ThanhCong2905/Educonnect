SET XACT_ABORT ON;
BEGIN TRAN;
INSERT dbo.MonHoc(TenMon,DangSuDung) SELECT N'Giáo dục quốc phòng và an ninh',1 WHERE NOT EXISTS(SELECT 1 FROM dbo.MonHoc WHERE TenMon=N'Giáo dục quốc phòng và an ninh');
COMMIT;
