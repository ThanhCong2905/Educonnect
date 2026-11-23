SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.HoSoGiaSuMonDay','MatNaKhoi') IS NULL
    ALTER TABLE dbo.HoSoGiaSuMonDay ADD MatNaKhoi bigint NULL;
IF COL_LENGTH('dbo.HoSoGiaSuMonDay','PhanKhoaHoc') IS NULL
    ALTER TABLE dbo.HoSoGiaSuMonDay ADD PhanKhoaHoc bigint NOT NULL CONSTRAINT DF_MonDay_PhanKhoaHoc DEFAULT (0);
IF COL_LENGTH('dbo.NhuCauHoc','PhanKhoaHoc') IS NULL
    ALTER TABLE dbo.NhuCauHoc ADD PhanKhoaHoc tinyint NOT NULL CONSTRAINT DF_NhuCau_PhanKhoaHoc DEFAULT (0),
        CONSTRAINT CK_NhuCau_PhanKhoaHoc CHECK (PhanKhoaHoc BETWEEN 0 AND 7);
IF COL_LENGTH('dbo.AspNetUsers','PhienBanChinhSach') IS NULL
    ALTER TABLE dbo.AspNetUsers ADD PhienBanChinhSach nvarchar(20) NULL, DongYChinhSachLuc datetimeoffset(7) NULL;
IF NOT EXISTS (SELECT 1 FROM dbo.MonHoc WHERE TenMon = N'Khoa học tự nhiên')
    INSERT dbo.MonHoc (TenMon, DangSuDung) VALUES (N'Khoa học tự nhiên', 1);
IF NOT EXISTS (SELECT 1 FROM dbo.MonHoc WHERE TenMon = N'Tiếng Việt')
    INSERT dbo.MonHoc (TenMon, DangSuDung) VALUES (N'Tiếng Việt', 1);
IF NOT EXISTS (SELECT 1 FROM dbo.MonHoc WHERE TenMon = N'Lịch sử và Địa lí')
    INSERT dbo.MonHoc (TenMon, DangSuDung) VALUES (N'Lịch sử và Địa lí', 1);
EXEC sp_executesql N'
    UPDATE day SET MatNaKhoi = scope.Mask
    FROM dbo.HoSoGiaSuMonDay AS day
    CROSS APPLY (
        SELECT SUM(DISTINCT CONVERT(bigint, POWER(2, grade.Lop - 1))) AS Mask
        FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12)) AS grade(Lop)
        WHERE EXISTS (
            SELECT 1 FROM STRING_SPLIT(REPLACE(REPLACE(REPLACE(REPLACE(LOWER(day.PhamViDay), N''lớp'', N''''), N'' '', N''''), N''–'', N''-''), N''—'', N''-''), '','') AS part
            CROSS APPLY (VALUES(CHARINDEX(''-'', part.value))) AS dash(Pos)
            WHERE TRY_CONVERT(int, part.value) = grade.Lop
               OR dash.Pos > 0 AND grade.Lop BETWEEN TRY_CONVERT(int, LEFT(part.value, NULLIF(dash.Pos, 0) - 1))
                   AND TRY_CONVERT(int, SUBSTRING(part.value, dash.Pos + 1, 20))
        )
    ) AS scope
    WHERE day.MatNaKhoi IS NULL AND scope.Mask IS NOT NULL;';
COMMIT;
GO
