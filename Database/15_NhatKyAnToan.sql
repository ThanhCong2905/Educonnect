SET XACT_ABORT ON;
GO
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.NhatKyAnToan', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NhatKyAnToan
    (
        Id uniqueidentifier NOT NULL CONSTRAINT PK_NhatKyAnToan PRIMARY KEY,
        ThoiDiem datetimeoffset NOT NULL,
        Nhom varchar(32) NOT NULL,
        SuKien nvarchar(160) NOT NULL,
        KetQua varchar(16) NOT NULL,
        TaiKhoanId uniqueidentifier NULL,
        TaiKhoanDichId uniqueidentifier NULL,
        DinhDanhBam varchar(64) NULL,
        DiaChiIp varchar(45) NOT NULL,
        DuongDan nvarchar(512) NOT NULL,
        PhuongThuc varchar(16) NOT NULL,
        MaYeuCau nvarchar(128) NOT NULL,
        LyDo nvarchar(100) NOT NULL,
        DoiTuongId varchar(64) NULL,
        CONSTRAINT FK_NhatKyAnToan_NguoiThucHien FOREIGN KEY(TaiKhoanId) REFERENCES dbo.AspNetUsers(Id),
        CONSTRAINT FK_NhatKyAnToan_TaiKhoanDich FOREIGN KEY(TaiKhoanDichId) REFERENCES dbo.AspNetUsers(Id),
        CONSTRAINT CK_NhatKyAnToan_Nhom CHECK(Nhom IN ('Authentication','Authorization','Accounting')),
        CONSTRAINT CK_NhatKyAnToan_KetQua CHECK(KetQua IN ('ThanhCong','ThatBai','TuChoi'))
    );
    CREATE INDEX IX_NhatKyAnToan_ThoiDiem_Id ON dbo.NhatKyAnToan(ThoiDiem,Id);
    CREATE INDEX IX_NhatKyAnToan_TaiKhoanId_ThoiDiem ON dbo.NhatKyAnToan(TaiKhoanId,ThoiDiem);
    CREATE INDEX IX_NhatKyAnToan_Nhom_ThoiDiem ON dbo.NhatKyAnToan(Nhom,ThoiDiem);
END;
IF DATABASE_PRINCIPAL_ID('KLTN_App') IS NOT NULL
BEGIN
    GRANT SELECT, INSERT ON dbo.NhatKyAnToan TO KLTN_App;
    DENY UPDATE, DELETE ON dbo.NhatKyAnToan TO KLTN_App;
END;
COMMIT;
GO
