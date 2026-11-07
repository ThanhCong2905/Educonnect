-- Run in SSMS against the existing Educonnect database.
-- Preserve existing role IDs and user assignments; add missing roles only.
USE [Educonnect];
GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @Roles TABLE (Name nvarchar(256), NormalizedName nvarchar(256));
INSERT @Roles VALUES
    (N'HocSinh', N'HOCSINH'),
    (N'PhuHuynh', N'PHUHUYNH'),
    (N'GiaSu', N'GIASU'),
    (N'QuanTri', N'QUANTRI');

-- Repair normalization for roles that already have the expected name.
UPDATE existing
SET NormalizedName = expected.NormalizedName,
    ConcurrencyStamp = CONVERT(nvarchar(36), NEWID())
FROM dbo.AspNetRoles existing WITH (UPDLOCK, HOLDLOCK)
JOIN @Roles expected ON existing.Name = expected.Name
WHERE (existing.NormalizedName IS NULL OR existing.NormalizedName <> expected.NormalizedName)
  AND NOT EXISTS (
      SELECT 1 FROM dbo.AspNetRoles other
      WHERE other.NormalizedName = expected.NormalizedName AND other.Id <> existing.Id
  );

INSERT dbo.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
SELECT NEWID(), expected.Name, expected.NormalizedName, CONVERT(nvarchar(36), NEWID())
FROM @Roles expected
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.AspNetRoles existing WITH (UPDLOCK, HOLDLOCK)
    WHERE existing.NormalizedName = expected.NormalizedName
);

COMMIT;
SELECT Id, Name, NormalizedName FROM dbo.AspNetRoles ORDER BY Name;
GO
