SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO
BEGIN TRANSACTION;
GO
IF OBJECT_ID(N'dbo.NhuCauMonHoc',N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NhuCauMonHoc (
        NhuCauId uniqueidentifier NOT NULL,
        MonHocId int NOT NULL,
        CONSTRAINT PK_NhuCauMonHoc PRIMARY KEY (NhuCauId,MonHocId),
        CONSTRAINT FK_NhuCauMonHoc_NhuCau FOREIGN KEY (NhuCauId) REFERENCES dbo.NhuCauHoc(Id) ON DELETE CASCADE,
        CONSTRAINT FK_NhuCauMonHoc_MonHoc FOREIGN KEY (MonHocId) REFERENCES dbo.MonHoc(Id)
    );
    CREATE INDEX IX_NhuCauMonHoc_MonHoc ON dbo.NhuCauMonHoc(MonHocId,NhuCauId);
END;
GO
CREATE TABLE #MonCanChuyen(NhuCauId uniqueidentifier NOT NULL,MonHocId int NOT NULL,PRIMARY KEY(NhuCauId,MonHocId));
INSERT #MonCanChuyen SELECT Id,MonHocId FROM dbo.NhuCauHoc WITH(TABLOCKX,HOLDLOCK);
IF COL_LENGTH('dbo.NhuCauHoc','MonHocIds') IS NOT NULL
BEGIN
    EXEC(N'
        IF EXISTS(SELECT 1 FROM dbo.NhuCauHoc WHERE ISJSON(MonHocIds)<>1 OR LEFT(LTRIM(MonHocIds),1)<>N''['')
            THROW 51152,N''Dữ liệu JSON chưa hợp lệ, dừng chuyển môn.'',1;
        IF EXISTS(SELECT 1 FROM dbo.NhuCauHoc n CROSS APPLY OPENJSON(n.MonHocIds) j
                  WHERE j.type<>2 OR TRY_CONVERT(int,j.value) IS NULL
                     OR NOT EXISTS(SELECT 1 FROM dbo.MonHoc m WHERE m.Id=TRY_CONVERT(int,j.value)))
            THROW 51152,N''Dữ liệu chứa môn không hợp lệ, dừng chuyển môn.'',1;
        INSERT #MonCanChuyen(NhuCauId,MonHocId)
        SELECT DISTINCT n.Id,CONVERT(int,j.value) FROM dbo.NhuCauHoc n CROSS APPLY OPENJSON(n.MonHocIds) j
        WHERE NOT EXISTS(SELECT 1 FROM #MonCanChuyen t WHERE t.NhuCauId=n.Id AND t.MonHocId=CONVERT(int,j.value));
    ');
END;
INSERT dbo.NhuCauMonHoc(NhuCauId,MonHocId)
SELECT t.NhuCauId,t.MonHocId FROM #MonCanChuyen t
WHERE NOT EXISTS(SELECT 1 FROM dbo.NhuCauMonHoc m WHERE m.NhuCauId=t.NhuCauId AND m.MonHocId=t.MonHocId);
IF EXISTS(SELECT NhuCauId,MonHocId FROM #MonCanChuyen EXCEPT SELECT NhuCauId,MonHocId FROM dbo.NhuCauMonHoc)
    THROW 51153,N'Chưa chuyển đủ môn học, hủy nâng cấp.',1;
DROP TABLE #MonCanChuyen;
DROP TRIGGER IF EXISTS dbo.trg_NhuCauHoc_DanhSachMon;
DROP TRIGGER IF EXISTS dbo.trg_MonHoc_KiemTraNhuCau;
IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_NhuCauHoc_MonHocIds')
    ALTER TABLE dbo.NhuCauHoc DROP CONSTRAINT CK_NhuCauHoc_MonHocIds;
IF COL_LENGTH('dbo.NhuCauHoc','MonHocIds') IS NOT NULL
BEGIN
    DECLARE @DefaultName sysname;
    SELECT @DefaultName=d.name FROM sys.default_constraints d JOIN sys.columns c ON c.object_id=d.parent_object_id AND c.column_id=d.parent_column_id
    WHERE d.parent_object_id=OBJECT_ID('dbo.NhuCauHoc') AND c.name='MonHocIds';
    IF @DefaultName IS NOT NULL
    BEGIN
        DECLARE @DropDefault nvarchar(4000)=N'ALTER TABLE dbo.NhuCauHoc DROP CONSTRAINT '+QUOTENAME(@DefaultName);
        EXEC(@DropDefault);
    END;
    ALTER TABLE dbo.NhuCauHoc DROP COLUMN MonHocIds;
END;
IF DATABASE_PRINCIPAL_ID(N'KLTN_App') IS NOT NULL
    GRANT SELECT,INSERT,UPDATE,DELETE ON dbo.NhuCauMonHoc TO KLTN_App;
GO
CREATE OR ALTER PROCEDURE dbo.usp_GuiDeXuatLop
 @LienHeId uniqueidentifier,@GiaSuId uniqueidentifier,@TenLop nvarchar(256),@MonHocId int,@MucTieu nvarchar(256),
 @SoBuoiTuan int,@ThoiLuongPhut int,@BatDauHocLuc datetimeoffset(0),@HinhThucHoc varchar(32),
 @HocPhi decimal(18,2),@DonViHocPhi varchar(16),@KhungGioJson nvarchar(max),
 @DiaDiem nvarchar(256)=NULL,@NhuCauId uniqueidentifier=NULL,@YeuCauId uniqueidentifier=NULL,@RowVersionCu binary(8)=NULL
AS
BEGIN
 SET NOCOUNT ON;SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRAN;
 DECLARE @BenHocId uniqueidentifier,@Now datetimeoffset(0)=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 SELECT @BenHocId=BenHocId FROM dbo.YeuCauLienHe WITH(UPDLOCK,HOLDLOCK) WHERE Id=@LienHeId AND GiaSuId=@GiaSuId AND TrangThai='ChapNhan';
 SET @Now=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 IF @BenHocId IS NULL THROW 51133,N'Chỉ gia sư của liên hệ đã chấp nhận được đề xuất lớp.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.HoSoGiaSu h JOIN dbo.HoSoGiaSuMonDay m ON m.HoSoGiaSuId=h.Id WHERE h.GiaSuId=@GiaSuId AND m.MonHocId=@MonHocId) THROW 51134,N'Môn học chưa đăng ký trong hồ sơ gia sư.',1;
 IF @NhuCauId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.NhuCauHoc n WITH(UPDLOCK,HOLDLOCK) WHERE n.Id=@NhuCauId AND n.ChuSoHuuId=@BenHocId AND n.TrangThai='DangMo' AND (n.MonHocId=@MonHocId OR EXISTS(SELECT 1 FROM dbo.NhuCauMonHoc m WHERE m.NhuCauId=n.Id AND m.MonHocId=@MonHocId))) THROW 51131,N'Môn đề xuất không thuộc nhu cầu học đang mở.',1;
 IF ISJSON(@KhungGioJson)<>1 THROW 51135,N'Lịch đề xuất phải là danh sách khung giờ hợp lệ.',1;
 DECLARE @Slots TABLE(Thu tinyint NOT NULL,BatDau time(0) NOT NULL,KetThuc time(0) NOT NULL);
 INSERT @Slots SELECT ThuTrongTuan,GioBatDau,GioKetThuc FROM OPENJSON(@KhungGioJson) WITH(ThuTrongTuan tinyint,GioBatDau time(0),GioKetThuc time(0));
 IF NOT EXISTS(SELECT 1 FROM @Slots) OR EXISTS(SELECT 1 FROM @Slots WHERE Thu>6 OR BatDau>=KetThuc OR DATEDIFF(second,BatDau,KetThuc)<>@ThoiLuongPhut*60) OR (SELECT COUNT(*) FROM @Slots)<>@SoBuoiTuan THROW 51136,N'Số khung giờ, thời lượng hoặc thứ chưa hợp lệ.',1;
 IF @HocPhi IS NULL OR @HocPhi<0 OR @DonViHocPhi IS NULL OR @DonViHocPhi NOT IN ('VND/Gio','VND/Buoi') OR @SoBuoiTuan NOT BETWEEN 1 AND 7 OR @ThoiLuongPhut NOT BETWEEN 1 AND 480 OR @TenLop IS NULL OR LEN(LTRIM(RTRIM(@TenLop)))=0 THROW 51137,N'Điều kiện lớp hoặc học phí chưa hợp lệ.',1;
 IF @YeuCauId IS NULL
 BEGIN
  IF EXISTS(SELECT 1 FROM dbo.YeuCauKetNoiLopHoc WHERE LienHeId=@LienHeId AND TrangThai IN ('ChoGiaSu','ChoBenHoc') AND HetHanLuc>@Now) THROW 51138,N'Đang có yêu cầu chờ; phản hồi trên yêu cầu đó.',1;
  SET @YeuCauId=NEWID();
  INSERT dbo.YeuCauKetNoiLopHoc(Id,GiaSuId,BenHocId,NguoiGuiId,NhuCauId,TrangThai,GuiLuc,HetHanLuc,TenLopDuKien,MonHocId,MucTieu,SoBuoiTuan,ThoiLuongPhut,BatDauHocLuc,HinhThucHoc,DiaDiem,HocPhi,DonViHocPhi,LienHeId,NguoiDeXuatId,DeXuatLuc)
  VALUES(@YeuCauId,@GiaSuId,@BenHocId,@GiaSuId,@NhuCauId,'ChoBenHoc',@Now,DATEADD(hour,72,@Now),@TenLop,@MonHocId,@MucTieu,@SoBuoiTuan,@ThoiLuongPhut,@BatDauHocLuc,@HinhThucHoc,@DiaDiem,@HocPhi,@DonViHocPhi,@LienHeId,@GiaSuId,@Now);
 END
 ELSE
 BEGIN
  DECLARE @LockedId uniqueidentifier;
  SELECT @LockedId=Id FROM dbo.YeuCauKetNoiLopHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@YeuCauId;
  SET @Now=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
  UPDATE dbo.YeuCauKetNoiLopHoc SET TrangThai='ChoBenHoc',TenLopDuKien=@TenLop,MonHocId=@MonHocId,MucTieu=@MucTieu,SoBuoiTuan=@SoBuoiTuan,ThoiLuongPhut=@ThoiLuongPhut,BatDauHocLuc=@BatDauHocLuc,HinhThucHoc=@HinhThucHoc,DiaDiem=@DiaDiem,HocPhi=@HocPhi,DonViHocPhi=@DonViHocPhi,NguoiDeXuatId=@GiaSuId,DeXuatLuc=@Now,HetHanLuc=DATEADD(hour,72,@Now)
  WHERE Id=@YeuCauId AND LienHeId=@LienHeId AND GiaSuId=@GiaSuId AND TrangThai IN ('ChoGiaSu','ChoBenHoc') AND HetHanLuc>@Now AND RowVersion=@RowVersionCu;
  IF @@ROWCOUNT<>1 THROW 51139,N'Yêu cầu đã đổi phiên bản, hết hạn hoặc không thuộc gia sư.',1;
 END;
 DELETE dbo.KhungGioDeXuat WHERE YeuCauKetNoiId=@YeuCauId;
 INSERT dbo.KhungGioDeXuat(YeuCauKetNoiId,ThuTrongTuan,GioBatDau,GioKetThuc) SELECT @YeuCauId,Thu,BatDau,KetThuc FROM @Slots;
 COMMIT;SELECT * FROM dbo.YeuCauKetNoiLopHoc WHERE Id=@YeuCauId;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK;THROW;END CATCH;
END;
GO
COMMIT;
GO
