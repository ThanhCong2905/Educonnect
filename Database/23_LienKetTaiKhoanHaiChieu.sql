SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.LienKetPhuHuynhHocSinh') AND name=N'CK_LienKet_Cap')
    ALTER TABLE dbo.LienKetPhuHuynhHocSinh DROP CONSTRAINT CK_LienKet_Cap;
ALTER TABLE dbo.LienKetPhuHuynhHocSinh WITH CHECK ADD CONSTRAINT CK_LienKet_Cap
    CHECK (PhuHuynhId<>HocSinhId AND NguoiGuiId IN(PhuHuynhId,HocSinhId));
COMMIT;
GO
