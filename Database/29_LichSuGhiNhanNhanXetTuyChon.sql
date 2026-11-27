SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.LichSuGhiNhanBuoiHoc') AND name=N'CK_LichSuGhiNhanBuoiHoc_3')
    ALTER TABLE dbo.LichSuGhiNhanBuoiHoc DROP CONSTRAINT CK_LichSuGhiNhanBuoiHoc_3;
ALTER TABLE dbo.LichSuGhiNhanBuoiHoc WITH CHECK ADD CONSTRAINT CK_LichSuGhiNhanBuoiHoc_3
    CHECK (KetQuaBuoiHoc='DaDienRa' OR LEN(LTRIM(RTRIM(NhanXet)))>0);
EXEC(N'
CREATE OR ALTER TRIGGER dbo.tr_BuoiHoc_LuuPhienBan ON dbo.BuoiHoc AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    INSERT dbo.LichSuGhiNhanBuoiHoc
        (BuoiHocId,PhienBan,TieuDe,KetQuaBuoiHoc,NoiDungGiangDay,NhanXet,TrangThaiThamDuGiaSu,TrangThaiThamDuBenHoc,NguoiGhiNhanId,GhiNhanLuc,NguoiSuaId,ThayDoiLuc)
    SELECT d.Id,d.PhienBan,d.TieuDe,d.KetQuaBuoiHoc,d.NoiDungGiangDay,COALESCE(d.NhanXet,N''''),
        d.TrangThaiThamDuGiaSu,d.TrangThaiThamDuBenHoc,d.NguoiGhiNhanId,d.GhiNhanLuc,
        COALESCE(i.NguoiGhiNhanId,i.NguoiThongBaoNghiId),i.CapNhatLuc
    FROM deleted d JOIN inserted i ON i.Id=d.Id
    WHERE d.KetQuaBuoiHoc IS NOT NULL AND d.GhiNhanLuc IS NOT NULL;
END;');
COMMIT;
GO
