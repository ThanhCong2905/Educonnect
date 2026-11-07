/*
  MT1 INTEGRATION — MỘT FILE CHO CÁC CA SQL 001–013
  Căn cứ: Công nghệ sử dụng (4), thiết kế CSDL; bảng TC Integration MT1.

  CÁCH CHẠY TRONG SSMS
  1. Kết nối LAPTOP-3L2V1AFT\SQLEXPRESS.
  2. Chọn CSDL kiểm thử đã có schema: EduConnect_Verification_MT1.
     File này không tạo CSDL, không cài lại schema và không đổi cấu hình ứng dụng.
  3. Tìm dòng @OnlyTC trong PHẦN CHỌN CA ở cuối file:
       N'MT1-IT-002' = chỉ chạy ca 002 (mặc định);
       NULL         = chạy toàn bộ 001–013.
  4. Bỏ vùng chọn, F5 toàn bộ file. Không chạy một INSERT tách khỏi bộ điều phối.
  5. Kết quả cuối: RunId + MaTC + BienThe + KetQua + MaLoi + ChiTiet.
     Chỉ đối chiếu các dòng có cùng MaTC; không dùng tổng số lỗi ở Messages làm kết quả.
  6. Ca âm chỉ PASS khi đúng loại ràng buộc cần kiểm. Lỗi chuẩn bị fixture là BLOCKED.
     Mọi biến thể có giao dịch riêng và ROLLBACK, kể cả biến thể thành công.
     IDENTITY và bộ đếm rowversion có thể tăng dù rollback — không phải dữ liệu còn sót.

  PHẠM VI
  - 001: metadata; 002–012: thao tác SQL trực tiếp; 013: phần ghi/đọc Unicode tại SQL.
  - 013 còn cần đọc bằng EF/context mới theo Excel để xác nhận ánh xạ ứng dụng.
  - 014–086 kiểm Service/HTTP: chạy bằng bộ kiểm thử tích hợp theo Excel, không thể
    thay lời gọi Service/HTTP bằng INSERT rồi kết luận đã kiểm thử cả luồng.
  - Tài khoản được chèn trong giao dịch chỉ làm fixture FK, không dùng để đăng nhập.

  Fixture SQL dùng GUID cục bộ sau (không phải tài khoản demo):
    HS-A = 10000000-0000-0000-0000-000000000001
    GS-A = 20000000-0000-0000-0000-000000000001
    P1/P2= 30000000-0000-0000-0000-000000000001 / ...0002
    N01  = 40000000-0000-0000-0000-000000000001
    MonHocId lấy OUTPUT INSERTED.Id; không giả định IDENTITY luôn bằng 101.
    Nếu các GUID trên đã tồn tại, biến thể bị BLOCKED; không xóa/ghi đè dữ liệu cũ.
*/
SET NOCOUNT ON;
SET XACT_ABORT OFF; -- cho phép ghi nhận lỗi ràng buộc rồi rollback từng biến thể
GO
IF OBJECT_ID('tempdb..#MT1_Run') IS NOT NULL DROP PROCEDURE #MT1_Run;
IF OBJECT_ID('tempdb..#MT1Results') IS NOT NULL DROP TABLE #MT1Results;
CREATE TABLE #MT1Results
(
    ThuTu int IDENTITY(1,1), RunId uniqueidentifier, DatabaseName sysname,
    MaTC nvarchar(16), BienThe nvarchar(80), KetQua varchar(8),
    MaLoi int NULL, RangBuoc nvarchar(256) NULL,
    MongDoi nvarchar(1500), ChiTiet nvarchar(4000), ThoiDiem datetimeoffset(0)
);
GO
CREATE PROCEDURE #MT1_Run
    @RunId uniqueidentifier,
    @TC nvarchar(16), @Variant nvarchar(80),
    @Prepare nvarchar(max) = N'', @Act nvarchar(max),
    @Expected nvarchar(1500), @Seed bit = 1,
    @ExpectedErrors varchar(100) = NULL,
    @Constraint nvarchar(256) = NULL,
    @Trigger sysname = NULL,
    @AfterError nvarchar(max) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT OFF;
    DECLARE @HS uniqueidentifier='10000000-0000-0000-0000-000000000001',
            @GS uniqueidentifier='20000000-0000-0000-0000-000000000001',
            @P1 uniqueidentifier='30000000-0000-0000-0000-000000000001',
            @P2 uniqueidentifier='30000000-0000-0000-0000-000000000002',
            @N1 uniqueidentifier='40000000-0000-0000-0000-000000000001',
            @Mon int, @Phase nvarchar(20)=N'PREPARE',
            @OwnTran bit=0, @Err int=NULL, @Message nvarchar(4000)=N'', @Status varchar(8)='BLOCKED',
            @Params nvarchar(1000)=N'@HS uniqueidentifier,@GS uniqueidentifier,@P1 uniqueidentifier,@P2 uniqueidentifier,@N1 uniqueidentifier,@Mon int,@TC nvarchar(16),@Variant nvarchar(80)';
    BEGIN TRY
        -- Guard ngay trước mọi thao tác ghi, kể cả khi gọi helper riêng.
        IF DB_NAME() <> N'EduConnect_Verification_MT1'
            THROW 51000,N'Chỉ chạy tại EduConnect_Verification_MT1. Chọn đúng database trong SSMS.',1;
        IF @@TRANCOUNT <> 0
            THROW 51001,N'Cửa sổ đang có giao dịch. Kết thúc giao dịch đó trước khi chạy file.',1;
        IF @Seed=1 AND
          (EXISTS(SELECT 1 FROM dbo.AspNetUsers WHERE Id IN (@HS,@GS)) OR
           EXISTS(SELECT 1 FROM dbo.HoSoGiaSu WHERE Id IN (@P1,@P2)) OR
           EXISTS(SELECT 1 FROM dbo.NhuCauHoc WHERE Id=@N1))
            THROW 51002,N'GUID fixture đã tồn tại; không ghi đè. Dùng bản sao DB kiểm thử sạch.',1;
        IF @Trigger IS NOT NULL AND NOT EXISTS
          (SELECT 1 FROM sys.triggers WHERE name=@Trigger AND parent_id=OBJECT_ID(N'dbo.KhungGioRanh') AND is_disabled=0)
            THROW 51003,N'Thiếu trigger chống chồng lịch theo thiết kế; không thể thực hiện ca trigger.',1;
        BEGIN TRANSACTION;
        SET @OwnTran=1;
        IF @Seed=1
        BEGIN
            INSERT dbo.AspNetUsers
                (Id,UserName,NormalizedUserName,Email,NormalizedEmail,HoTen,TrangThai,
                 EmailConfirmed,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount)
            VALUES
                (@HS,N'mt1-sql-hs',N'MT1-SQL-HS',N'mt1-hs@example.invalid',N'MT1-HS@EXAMPLE.INVALID',N'Học sinh kiểm thử SQL','HoatDong',0,0,0,0,0),
                (@GS,N'mt1-sql-gs',N'MT1-SQL-GS',N'mt1-gs@example.invalid',N'MT1-GS@EXAMPLE.INVALID',N'Gia sư kiểm thử SQL','HoatDong',0,0,0,0,0);
            INSERT dbo.GiaSu(TaiKhoanId) VALUES(@GS);
            INSERT dbo.HocSinh(TaiKhoanId) VALUES(@HS);
            INSERT dbo.MonHoc(TenMon,DangSuDung) VALUES(N'Toán kiểm thử SQL MT1',1);
            SET @Mon=CONVERT(int,SCOPE_IDENTITY());
        END;
        EXEC sys.sp_executesql @Prepare,@Params,@HS,@GS,@P1,@P2,@N1,@Mon,@TC,@Variant;
        SET @Phase=N'ACT';
        EXEC sys.sp_executesql @Act,@Params,@HS,@GS,@P1,@P2,@N1,@Mon,@TC,@Variant;
        IF @ExpectedErrors IS NULL
        BEGIN
            SET @Status='PASS';
            SET @Message=N'Thao tác và các kiểm tra sau ghi thành công.';
        END
        ELSE
        BEGIN
            SET @Status='FAIL';
            SET @Message=N'Không phát sinh lỗi ràng buộc mong đợi; thao tác sai đã được chấp nhận trong giao dịch thử.';
        END;
    END TRY
    BEGIN CATCH
        SELECT @Err=ERROR_NUMBER(),@Message=ERROR_MESSAGE();
        IF @Phase=N'ACT' AND @ExpectedErrors IS NOT NULL
           AND ((@ExpectedErrors='TRIGGER' AND @Err>=50000) OR CHARINDEX(','+CONVERT(varchar(12),@Err)+',',','+@ExpectedErrors+',')>0)
           AND (@Constraint IS NULL OR CHARINDEX(@Constraint,@Message)>0)
           AND (@Trigger IS NULL OR ERROR_PROCEDURE()=@Trigger OR ERROR_PROCEDURE()=N'dbo.'+@Trigger)
            SET @Status='PASS';
        ELSE IF @Phase=N'ACT' SET @Status='FAIL';
        ELSE SET @Status='BLOCKED';
        SET @Message=@Phase+N': '+@Message;
    END CATCH;
    IF @Status='PASS' AND @AfterError IS NOT NULL AND XACT_STATE()<>0
    BEGIN TRY
        EXEC sys.sp_executesql @AfterError,@Params,@HS,@GS,@P1,@P2,@N1,@Mon,@TC,@Variant;
    END TRY
    BEGIN CATCH
        SELECT @Status='FAIL',@Err=ERROR_NUMBER(),@Message=N'Kiểm tra sau lỗi: '+ERROR_MESSAGE();
    END CATCH;
    IF @OwnTran=1 AND XACT_STATE()<>0 ROLLBACK TRANSACTION;
    IF @OwnTran=1 AND @Seed=1 AND
      (EXISTS(SELECT 1 FROM dbo.AspNetUsers WHERE Id IN (@HS,@GS)) OR
       EXISTS(SELECT 1 FROM dbo.HoSoGiaSu WHERE Id IN (@P1,@P2)) OR
       EXISTS(SELECT 1 FROM dbo.NhuCauHoc WHERE Id=@N1))
        SELECT @Status='FAIL',@Message=@Message+N' Dữ liệu fixture chưa được dọn sau rollback.';
    -- Ghi kết quả SAU rollback, nên lỗi hoặc rollback fixture không xóa mất minh chứng.
    INSERT #MT1Results(RunId,DatabaseName,MaTC,BienThe,KetQua,MaLoi,RangBuoc,MongDoi,ChiTiet,ThoiDiem)
    VALUES(@RunId,DB_NAME(),@TC,@Variant,@Status,@Err,@Constraint,@Expected,@Message,SYSDATETIMEOFFSET());
    SELECT @TC AS MaTC,@Variant AS BienThe,@Status AS KetQua,@Err AS MaLoi,@Message AS ChiTiet;
END;
GO
/* ==================== PHẦN CHỌN CA ==================== */
DECLARE @OnlyTC nvarchar(16)=NULL; 
DECLARE @RunId uniqueidentifier=NEWID();
IF DB_NAME()<>N'EduConnect_Verification_MT1'
    THROW 51000,N'Chọn EduConnect_Verification_MT1 trong dropdown SSMS rồi chạy lại toàn bộ file.',1;
IF @@TRANCOUNT<>0
    THROW 51001,N'Đang có giao dịch ngoài bộ chạy; chưa thực hiện bất kỳ TC nào.',1;
IF @OnlyTC IS NOT NULL AND @OnlyTC NOT IN
 (N'MT1-IT-001',N'MT1-IT-002',N'MT1-IT-003',N'MT1-IT-004',N'MT1-IT-005',N'MT1-IT-006',
  N'MT1-IT-007',N'MT1-IT-008',N'MT1-IT-009',N'MT1-IT-010',N'MT1-IT-011',N'MT1-IT-012',N'MT1-IT-013')
    THROW 51004,N'Mã ca không thuộc 001–013. Các ca Service/HTTP thực hiện theo Excel.',1;
SELECT @RunId AS RunId,DB_NAME() AS DatabaseName,@OnlyTC AS ChiChayTC,SYSDATETIMEOFFSET() AS BatDau;
DECLARE @Profile nvarchar(max)=N'
INSERT dbo.HoSoGiaSu(Id,GiaSuId,TrinhDo,ChuyenMon,KinhNghiem,MoTa,KhuVuc,TrangThaiDuyet)
VALUES(@P1,@GS,N''Đại học'',N''Toán'',N''3 năm'',N''Gia sư Toán THCS'',N''Thủ Đức'',''DaDuyet'');';
DECLARE @Need nvarchar(max)=N'
INSERT dbo.NhuCauHoc(Id,ChuSoHuuId,MonHocId,LoaiNhuCau,KhoiLop,HocLuc,MucTieu,SoBuoiTuan,HinhThucHoc,HocPhiDuKien,DonViHocPhi,TrangThai)
VALUES(@N1,@HS,@Mon,''TimGiaSu'',N''Lớp 8'',N''Trung bình'',N''Ôn đại số'',2,''TrucTuyen'',150000,''VND/Gio'',''DangMo'');';
DECLARE @Slot nvarchar(max)=N'
INSERT dbo.KhungGioRanh(Id,NhuCauId,ThuTrongTuan,GioBatDau,GioKetThuc)
VALUES(NEWID(),@N1,1,''18:00'',''19:00'');';
DECLARE @Prep nvarchar(max),@SQL nvarchar(max);

/* ==================== MT1-IT-001 — CHỈ METADATA ==================== */
IF @OnlyTC IS NULL OR @OnlyTC=N'MT1-IT-001'
BEGIN
    EXEC #MT1_Run @RunId,N'MT1-IT-001',N'A — metadata hồ sơ',@Seed=0,
    @Expected=N'PK Id; FK GiaSuId→GiaSu(TaiKhoanId); RowVersion8byte NOT NULL; 6 trường bắt buộc NOT NULL.',
    @Act=N'
SELECT @TC AS MaTC,@Variant AS BienThe,c.name,TYPE_NAME(c.system_type_id) AS Kieu,c.max_length,c.is_nullable
FROM sys.columns c WHERE c.object_id=OBJECT_ID(N''dbo.HoSoGiaSu'') ORDER BY c.column_id;
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID(N''dbo.HoSoGiaSu'') AND name=''RowVersion'' AND system_type_id=189 AND max_length=8 AND is_nullable=0)
 THROW 51010,N''RowVersion không đúng thiết kế.'',1;
IF (SELECT COUNT(*) FROM sys.columns WHERE object_id=OBJECT_ID(N''dbo.HoSoGiaSu'') AND name IN (''TrinhDo'',''ChuyenMon'',''KinhNghiem'',''MoTa'',''KhuVuc'',''TrangThaiDuyet'') AND is_nullable=0)<>6
 THROW 51011,N''Không đủ 6 cột bắt buộc NOT NULL.'',1;
IF NOT EXISTS(SELECT 1 FROM sys.key_constraints k JOIN sys.index_columns ic ON ic.object_id=k.parent_object_id AND ic.index_id=k.unique_index_id JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE k.parent_object_id=OBJECT_ID(N''dbo.HoSoGiaSu'') AND k.type=''PK'' AND c.name=''Id'' AND ic.key_ordinal=1)
 OR (SELECT COUNT(*) FROM sys.key_constraints k JOIN sys.index_columns ic ON ic.object_id=k.parent_object_id AND ic.index_id=k.unique_index_id WHERE k.parent_object_id=OBJECT_ID(N''dbo.HoSoGiaSu'') AND k.type=''PK'' AND ic.key_ordinal>0)<>1
 THROW 51012,N''PK hồ sơ không phải một cột Id.'',1;
IF NOT EXISTS(SELECT 1 FROM sys.foreign_key_columns f JOIN sys.columns a ON a.object_id=f.parent_object_id AND a.column_id=f.parent_column_id JOIN sys.columns b ON b.object_id=f.referenced_object_id AND b.column_id=f.referenced_column_id WHERE f.parent_object_id=OBJECT_ID(N''dbo.HoSoGiaSu'') AND a.name=''GiaSuId'' AND f.referenced_object_id=OBJECT_ID(N''dbo.GiaSu'') AND b.name=''TaiKhoanId'')
 THROW 51013,N''FK hồ sơ không đúng GiaSu(TaiKhoanId).'',1;
SELECT @TC AS MaTC,@Variant AS BienThe,fk.name AS TenFK,OBJECT_NAME(fk.referenced_object_id) AS BangDich FROM sys.foreign_keys fk WHERE fk.parent_object_id=OBJECT_ID(N''dbo.HoSoGiaSu'');';
END;

/* ==================== MT1-IT-002 — UNIQUE GiaSuId ==================== */
IF @OnlyTC IS NULL OR @OnlyTC=N'MT1-IT-002'
BEGIN
    DECLARE @UniqueGS sysname;
    SELECT TOP(1) @UniqueGS=i.name FROM sys.indexes i
    WHERE i.object_id=OBJECT_ID(N'dbo.HoSoGiaSu') AND i.is_unique=1
      AND (SELECT COUNT(*) FROM sys.index_columns k WHERE k.object_id=i.object_id AND k.index_id=i.index_id AND k.key_ordinal>0)=1
      AND EXISTS(SELECT 1 FROM sys.index_columns k JOIN sys.columns c ON c.object_id=k.object_id AND c.column_id=k.column_id WHERE k.object_id=i.object_id AND k.index_id=i.index_id AND k.key_ordinal=1 AND c.name=N'GiaSuId');
    SET @UniqueGS=COALESCE(@UniqueGS,N'UQ_GiaSu_HoSoGiaSu');
    SET @SQL=N'
SELECT @TC AS MaTC,@Variant AS BienThe,Id,GiaSuId FROM dbo.HoSoGiaSu WHERE GiaSuId=@GS;
IF (SELECT COUNT(*) FROM dbo.HoSoGiaSu WHERE GiaSuId=@GS)<>1 THROW 51020,N''Fixture phải có đúng hồ sơ thứ nhất.'',1;
INSERT dbo.HoSoGiaSu(Id,GiaSuId,TrinhDo,ChuyenMon,KinhNghiem,MoTa,KhuVuc,TrangThaiDuyet)
VALUES(@P2,@GS,N''Đại học'',N''Toán'',N''3 năm'',N''Gia sư Toán THCS'',N''Thủ Đức'',''DaDuyet'');';
    EXEC #MT1_Run @RunId,N'MT1-IT-002',N'A — P2 khác Id, cùng GiaSuId',
      @Prepare=@Profile,@Act=@SQL,@ExpectedErrors='2601,2627',@Constraint=@UniqueGS,
      @AfterError=N'SELECT @TC AS MaTC,@Variant AS BienThe,Id,GiaSuId FROM dbo.HoSoGiaSu WHERE GiaSuId=@GS; IF (SELECT COUNT(*) FROM dbo.HoSoGiaSu WHERE GiaSuId=@GS)<>1 OR EXISTS(SELECT 1 FROM dbo.HoSoGiaSu WHERE Id=@P2) THROW 51021,N''Phải chỉ còn P1 trước rollback.'',1;',
      @Expected=N'INSERT P1 thành công; P2 bị UNIQUE GiaSuId chặn (không phải trùng PK Id). Rollback cả fixture sau ghi minh chứng.';
END;

/* ==================== MT1-IT-003 — CRUD VÀ TÊN TRÙNG ==================== */
IF @OnlyTC IS NULL OR @OnlyTC=N'MT1-IT-003'
BEGIN
    EXEC #MT1_Run @RunId,N'MT1-IT-003',N'A — CRUD, hai Id khác/cùng tên',
    @Expected=N'CRUD đúng; hai môn khác Id được phép cùng tên theo bảng TC.',
    @Act=N'
DECLARE @Name nvarchar(256)=N''Toán thử nghiệm MT1_''+CONVERT(nvarchar(36),NEWID()),@Other int;
UPDATE dbo.MonHoc SET TenMon=@Name WHERE Id=@Mon;
IF NOT EXISTS(SELECT 1 FROM dbo.MonHoc WHERE Id=@Mon AND TenMon=@Name) THROW 51030,N''UPDATE tên môn sai.'',1;
INSERT dbo.MonHoc(TenMon,DangSuDung) VALUES(@Name,1);SET @Other=CONVERT(int,SCOPE_IDENTITY());
SELECT @TC AS MaTC,@Variant AS BienThe,Id,TenMon,DangSuDung FROM dbo.MonHoc WHERE Id IN(@Mon,@Other);
IF @Other=@Mon OR (SELECT COUNT(*) FROM dbo.MonHoc WHERE Id IN(@Mon,@Other))<>2 THROW 51031,N''Không có đúng hai môn khác Id.'',1;
DELETE dbo.MonHoc WHERE Id IN(@Mon,@Other);
IF EXISTS(SELECT 1 FROM dbo.MonHoc WHERE Id IN(@Mon,@Other)) THROW 51032,N''DELETE môn chưa hoàn tất.'',1;';
END;

/* ==================== MT1-IT-004 — TÁCH 3 BIẾN THỂ PK/FK ==================== */
IF @OnlyTC IS NULL OR @OnlyTC=N'MT1-IT-004'
BEGIN
    SET @Prep=@Profile+N'INSERT dbo.HoSoGiaSuMonDay(HoSoGiaSuId,MonHocId,PhamViDay,HocPhiDeXuat,DonViHocPhi) VALUES(@P1,@Mon,N''Lớp 8'',150000,''VND/Gio'');';
    EXEC #MT1_Run @RunId,N'MT1-IT-004',N'A — lặp khóa ghép',@Prepare=@Prep,
      @Act=N'INSERT dbo.HoSoGiaSuMonDay(HoSoGiaSuId,MonHocId,PhamViDay,HocPhiDeXuat,DonViHocPhi) VALUES(@P1,@Mon,N''Lớp 8'',150000,''VND/Gio'');',
      @ExpectedErrors='2601,2627',@Constraint=N'PK_HoSoGiaSuMonDay',@Expected=N'Từ chối trùng cặp hồ sơ–môn, không phải lỗi cột phí/FK.';
    EXEC #MT1_Run @RunId,N'MT1-IT-004',N'B — hồ sơ không tồn tại',@Prepare=@Prep,
      @Act=N'INSERT dbo.HoSoGiaSuMonDay(HoSoGiaSuId,MonHocId,PhamViDay,HocPhiDeXuat,DonViHocPhi) VALUES(@P2,@Mon,N''Lớp 8'',150000,''VND/Gio'');',
      @ExpectedErrors='547',@Constraint=N'FK_HoSoGiaSuMonDay_HoSoGiaSuId',@Expected=N'FK hồ sơ chặn liên kết mồ côi.';
    EXEC #MT1_Run @RunId,N'MT1-IT-004',N'C — môn không tồn tại',@Prepare=@Prep,
      @Act=N'INSERT dbo.HoSoGiaSuMonDay(HoSoGiaSuId,MonHocId,PhamViDay,HocPhiDeXuat,DonViHocPhi) VALUES(@P1,-2147483648,N''Lớp 8'',150000,''VND/Gio'');',
      @ExpectedErrors='547',@Constraint=N'FK_HoSoGiaSuMonDay_MonHocId',@Expected=N'FK môn chặn liên kết mồ côi.';
END;

/* ==================== MT1-IT-005 — FK CHỦ SỞ HỮU ==================== */
IF @OnlyTC IS NULL OR @OnlyTC=N'MT1-IT-005'
BEGIN
    SET @SQL=N'IF EXISTS(SELECT 1 FROM dbo.AspNetUsers WHERE Id=''10000000-0000-0000-0000-000000009999'') THROW 51050,N''Id chủ thử phải không tồn tại.'',1;'+
      REPLACE(@Need,N'@N1,@HS,@Mon',N'@N1,''10000000-0000-0000-0000-000000009999'',@Mon');
    EXEC #MT1_Run @RunId,N'MT1-IT-005',N'A — chủ không tồn tại',@Act=@SQL,
      @Prepare=N'IF NOT EXISTS(SELECT 1 FROM sys.foreign_key_columns f JOIN sys.columns c ON c.object_id=f.parent_object_id AND c.column_id=f.parent_column_id WHERE f.parent_object_id=OBJECT_ID(N''dbo.NhuCauHoc'') AND c.name=N''ChuSoHuuId'' AND f.referenced_object_id=OBJECT_ID(N''dbo.AspNetUsers'')) THROW 51051,N''FK chủ không trỏ AspNetUsers theo thiết kế.'',1;',
      @ExpectedErrors='547',@Constraint=N'FK_NhuCauHoc_ChuSoHuuId',@Expected=N'FK ChuSoHuuId→AspNetUsers(Id) từ chối chủ không tồn tại.';
END;

/* ==================== MT1-IT-006 — TÁCH 3 CHECK ==================== */
IF @OnlyTC IS NULL OR @OnlyTC=N'MT1-IT-006'
BEGIN
    SET @SQL=REPLACE(@Need,N'N''Ôn đại số'',2,',N'N''Ôn đại số'',8,');
    EXEC #MT1_Run @RunId,N'MT1-IT-006',N'A — 8 buổi/tuần',@Act=@SQL,
      @ExpectedErrors='547',@Constraint=N'CK_NhuCauHoc_9',@Expected=N'CHECK số buổi1..7 chặn8.';
    SET @SQL=REPLACE(@Need,N'150000,''VND/Gio''',N'-1,''VND/Gio''');
    EXEC #MT1_Run @RunId,N'MT1-IT-006',N'B — học phí âm',@Act=@SQL,
      @ExpectedErrors='547',@Constraint=N'CK_NhuCauHoc_4',@Expected=N'CHECK học phí không âm chặn-1.';
    SET @SQL=REPLACE(@Need,N'''TrucTuyen''',N'''Khac''');
    EXEC #MT1_Run @RunId,N'MT1-IT-006',N'C — hình thức Khac',@Act=@SQL,
      @ExpectedErrors='547',@Constraint=N'CK_NhuCauHoc_3',@Expected=N'CHECK hình thức chặn Khac.';
END;

/* ==================== MT1-IT-007 — TÁCH 3 CHECK LỊCH ==================== */
IF @OnlyTC IS NULL OR @OnlyTC=N'MT1-IT-007'
BEGIN
    EXEC #MT1_Run @RunId,N'MT1-IT-007',N'A — giờ đảo chiều',@Prepare=@Need,
      @Act=N'INSERT dbo.KhungGioRanh(Id,NhuCauId,ThuTrongTuan,GioBatDau,GioKetThuc) VALUES(NEWID(),@N1,1,''20:00'',''19:00'');',
      @ExpectedErrors='547',@Constraint=N'CK_KhungGioRanh_2',@Expected=N'CHECK giờ chặn bắt đầu sau kết thúc.';
    EXEC #MT1_Run @RunId,N'MT1-IT-007',N'B — thứ7 ngoài miền0..6',@Prepare=@Need,
      @Act=N'INSERT dbo.KhungGioRanh(Id,NhuCauId,ThuTrongTuan,GioBatDau,GioKetThuc) VALUES(NEWID(),@N1,7,''19:00'',''20:00'');',
      @ExpectedErrors='547',@Constraint=N'CK_KhungGioRanh_1',@Expected=N'CHECK thứ chặn7.';
    EXEC #MT1_Run @RunId,N'MT1-IT-007',N'C — hai đầu giờ bằng nhau',@Prepare=@Need,
      @Act=N'INSERT dbo.KhungGioRanh(Id,NhuCauId,ThuTrongTuan,GioBatDau,GioKetThuc) VALUES(NEWID(),@N1,1,''19:00'',''19:00'');',
      @ExpectedErrors='547',@Constraint=N'CK_KhungGioRanh_2',@Expected=N'CHECK giờ chặn khoảng rỗng.';
END;

/* ==================== MT1-IT-008 — FK LỊCH ==================== */
IF @OnlyTC IS NULL OR @OnlyTC=N'MT1-IT-008'
BEGIN
    EXEC #MT1_Run @RunId,N'MT1-IT-008',N'A — nhu cầu không tồn tại',
      @Act=N'IF EXISTS(SELECT 1 FROM dbo.NhuCauHoc WHERE Id=''40000000-0000-0000-0000-000000009999'') THROW 51080,N''Id thử đã tồn tại.'',1;
INSERT dbo.KhungGioRanh(Id,NhuCauId,ThuTrongTuan,GioBatDau,GioKetThuc) VALUES(NEWID(),''40000000-0000-0000-0000-000000009999'',1,''19:00'',''20:00'');',
      @ExpectedErrors='547',@Constraint=N'FK_KhungGioRanh_NhuCauId',@Expected=N'FK chặn lịch mồ côi,không nhầm lỗi giờ/NOT NULL.';
END;

/* ==================== MT1-IT-009 — ROWVERSION ==================== */
IF @OnlyTC IS NULL OR @OnlyTC=N'MT1-IT-009'
BEGIN
    EXEC #MT1_Run @RunId,N'MT1-IT-009',N'A — INSERT rồi UPDATE',@Prepare=@Need,
      @Expected=N'v1,v2 đều8byte và khác nhau; không tự gán RowVersion.',
      @Act=N'DECLARE @v1 varbinary(8),@v2 varbinary(8);
SELECT @v1=RowVersion FROM dbo.NhuCauHoc WHERE Id=@N1;
UPDATE dbo.NhuCauHoc SET MucTieu=N''Ôn hình học lớp 8'' WHERE Id=@N1;
SELECT @v2=RowVersion FROM dbo.NhuCauHoc WHERE Id=@N1;
SELECT @TC AS MaTC,@Variant AS BienThe,@v1 AS v1,@v2 AS v2,DATALENGTH(@v1) AS SoByte1,DATALENGTH(@v2) AS SoByte2;
IF @v1 IS NULL OR @v2 IS NULL OR DATALENGTH(@v1)<>8 OR DATALENGTH(@v2)<>8 OR @v1=@v2 THROW 51090,N''RowVersion không tự đổi đúng.'',1;';
END;

/* ==================== MT1-IT-010 — NO ACTION ==================== */
IF @OnlyTC IS NULL OR @OnlyTC=N'MT1-IT-010'
BEGIN
    SET @Prep=@Need+@Slot;
    EXEC #MT1_Run @RunId,N'MT1-IT-010',N'A — DELETE cha còn lịch con',@Prepare=@Prep,
      @Act=N'DELETE dbo.NhuCauHoc WHERE Id=@N1;',@ExpectedErrors='547',@Constraint=N'FK_KhungGioRanh_NhuCauId',
      @Expected=N'FK chặn xóa N01 còn lịch;không xóa slot trước để né kiểm tra.';
END;

/* ==================== MT1-IT-011 — TRIGGER CHỒNG LỊCH ==================== */
IF @OnlyTC IS NULL OR @OnlyTC=N'MT1-IT-011'
BEGIN
    SET @Prep=@Need+@Slot;
    EXEC #MT1_Run @RunId,N'MT1-IT-011',N'A — 18:30–19:30 chồng18–19',@Prepare=@Prep,
      @Trigger=N'tr_KhungGioRanh_KhongChongLan',
      @Act=N'INSERT dbo.KhungGioRanh(Id,NhuCauId,ThuTrongTuan,GioBatDau,GioKetThuc) VALUES(NEWID(),@N1,1,''18:30'',''19:30'');',
      @ExpectedErrors='TRIGGER',@Expected=N'Trigger thiết kế từ chối lịch chồng. Ghi mã lỗi thực tế; cần đúng nguồn trigger,không nhận lỗi FK/CHECK hay lỗi khác làm PASS.';
END;

/* ==================== MT1-IT-012 — NULL VẬT LÝ ==================== */
IF @OnlyTC IS NULL OR @OnlyTC=N'MT1-IT-012'
BEGIN
    EXEC #MT1_Run @RunId,N'MT1-IT-012',N'A — nháp có các cột NULL',
      @Expected=N'NULL được giữ ở nháp; kết quả không chứng minh Service cho đăng công khai thiếu dữ liệu.',
      @Act=N'INSERT dbo.NhuCauHoc(Id,ChuSoHuuId,MonHocId,LoaiNhuCau,HinhThucHoc,TrangThai,MucTieu,SoBuoiTuan,HocPhiDuKien,DonViHocPhi)
VALUES(@N1,@HS,@Mon,''TimGiaSu'',''TrucTuyen'',''BanNhap'',NULL,NULL,NULL,NULL);
SELECT @TC AS MaTC,@Variant AS BienThe,Id,TrangThai,MucTieu,SoBuoiTuan,HocPhiDuKien,DonViHocPhi FROM dbo.NhuCauHoc WHERE Id=@N1;
IF NOT EXISTS(SELECT 1 FROM dbo.NhuCauHoc WHERE Id=@N1 AND TrangThai=''BanNhap'' AND MucTieu IS NULL AND SoBuoiTuan IS NULL AND HocPhiDuKien IS NULL AND DonViHocPhi IS NULL) THROW 51120,N''NULL không được giữ đúng.'',1;';
END;

/* ==================== MT1-IT-013 — UNICODE SQL, CÒN BƯỚC EF TRONG EXCEL ==================== */
IF @OnlyTC IS NULL OR @OnlyTC=N'MT1-IT-013'
BEGIN
    EXEC #MT1_Run @RunId,N'MT1-IT-013',N'A — Unicode tại SQL (chưa xác nhận EF)',@Prepare=@Need,
      @Expected=N'Chuỗi SQL đọc lại đúng từng byte UTF-16; phần EF phải thực hiện theo Excel.',
      @Act=N'DECLARE @Goal nvarchar(256)=N''Ôn tập Hóa học – lớp 8'',@Area nvarchar(256)=N''Thủ Đức'';
UPDATE dbo.NhuCauHoc SET MucTieu=@Goal,KhuVuc=@Area,HinhThucHoc=''TrucTiep'' WHERE Id=@N1;
SELECT @TC AS MaTC,@Variant AS BienThe,MucTieu,KhuVuc,HinhThucHoc FROM dbo.NhuCauHoc WHERE Id=@N1;
IF NOT EXISTS(SELECT 1 FROM dbo.NhuCauHoc WHERE Id=@N1 AND CONVERT(varbinary(max),MucTieu)=CONVERT(varbinary(max),@Goal) AND CONVERT(varbinary(max),KhuVuc)=CONVERT(varbinary(max),@Area)) THROW 51130,N''Unicode không được giữ nguyên.'',1;';
END;

/* ==================== KẾT QUẢ — KHÔNG GỘP BIẾN THỂ KHÁC TC ==================== */
SELECT RunId,DatabaseName,MaTC,BienThe,KetQua,MaLoi,RangBuoc,MongDoi,ChiTiet,ThoiDiem
FROM #MT1Results WHERE RunId=@RunId ORDER BY ThuTu;
SELECT RunId,MaTC,
       CASE WHEN SUM(CASE WHEN KetQua='FAIL' THEN 1 ELSE 0 END)>0 THEN 'FAIL'
            WHEN SUM(CASE WHEN KetQua='BLOCKED' THEN 1 ELSE 0 END)>0 THEN 'BLOCKED'
            ELSE 'PASS' END AS KetQuaSQL,
       COUNT(*) AS SoBienThe,
       CASE WHEN MaTC=N'MT1-IT-013' THEN N'Chỉ phần SQL; cần kết quả đọc EF theo Excel.' ELSE N'Đối chiếu các biến thể cùng mã TC trong bảng chi tiết.' END AS PhamVi
FROM #MT1Results WHERE RunId=@RunId GROUP BY RunId,MaTC ORDER BY MaTC;
SELECT @@TRANCOUNT AS GiaoDichConMo; -- mong đợi 0
DROP PROCEDURE #MT1_Run;
