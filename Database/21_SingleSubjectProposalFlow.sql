SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.YeuCauKetNoiLopHoc','TenNguoiHoc') IS NULL
    ALTER TABLE dbo.YeuCauKetNoiLopHoc ADD TenNguoiHoc nvarchar(256) NULL;
DELETE link FROM dbo.NhuCauMonHoc link JOIN dbo.NhuCauHoc need ON need.Id=link.NhuCauId WHERE link.MonHocId<>need.MonHocId;
INSERT dbo.NhuCauMonHoc(NhuCauId,MonHocId)
SELECT n.Id,n.MonHocId FROM dbo.NhuCauHoc n WHERE NOT EXISTS(SELECT 1 FROM dbo.NhuCauMonHoc m WHERE m.NhuCauId=n.Id);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.NhuCauMonHoc') AND name='UQ_NhuCauMonHoc_MotMon')
    CREATE UNIQUE INDEX UQ_NhuCauMonHoc_MotMon ON dbo.NhuCauMonHoc(NhuCauId);
COMMIT;
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO
CREATE OR ALTER PROCEDURE dbo.usp_GuiDeXuatLop
 @LienHeId uniqueidentifier,@GiaSuId uniqueidentifier,@TenLop nvarchar(256),@MonHocId int,@MucTieu nvarchar(2000),
 @SoBuoiTuan int,@ThoiLuongPhut int,@BatDauHocLuc datetimeoffset(0),@HinhThucHoc varchar(32),
 @HocPhi decimal(18,2),@DonViHocPhi varchar(16),@KhungGioJson nvarchar(max),
 @DiaDiem nvarchar(500)=NULL,@NhuCauId uniqueidentifier=NULL,@YeuCauId uniqueidentifier=NULL,@RowVersionCu binary(8)=NULL,@NguoiThucHienId uniqueidentifier=NULL,@LoiNhan nvarchar(2000)=NULL,@TenNguoiHoc nvarchar(256)=NULL
AS
BEGIN
 SET NOCOUNT ON;SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRAN;
 DECLARE @BenHocId uniqueidentifier,@Now datetimeoffset(0)=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 SELECT @BenHocId=BenHocId FROM dbo.YeuCauLienHe WITH(UPDLOCK,HOLDLOCK) WHERE Id=@LienHeId AND GiaSuId=@GiaSuId AND TrangThai='ChapNhan';
 SET @Now=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 IF @BenHocId IS NULL THROW 51133,N'Liên hệ chưa được chấp nhận.',1;
 SET @NguoiThucHienId=COALESCE(@NguoiThucHienId,@GiaSuId);
 IF @NguoiThucHienId<>@GiaSuId THROW 51140,N'Chỉ gia sư được gửi đề xuất lớp.',1;
 IF NULLIF(LTRIM(RTRIM(@TenNguoiHoc)),N'') IS NULL THROW 51137,N'Nhập tên người học.',1;
 DECLARE @Cho varchar(32)=CASE WHEN @NguoiThucHienId=@GiaSuId THEN 'ChoBenHoc' ELSE 'ChoGiaSu' END;
 IF EXISTS(SELECT 1 FROM dbo.AspNetUsers WITH(UPDLOCK,HOLDLOCK) WHERE Id IN(@GiaSuId,@BenHocId) AND TrangThai<>'HoatDong') THROW 51140,N'Tài khoản hiện không hoạt động.',1;
 IF EXISTS(SELECT 1 FROM dbo.LopHoc WITH(UPDLOCK,HOLDLOCK) WHERE GiaSuId=@GiaSuId AND BenHocId=@BenHocId AND TrangThai='DangHoatDong') THROW 51138,N'Hai bên đã có lớp đang hoạt động.',1;
 IF @BatDauHocLuc<=@Now THROW 51137,N'Ngày bắt đầu phải ở tương lai.',1;
 IF @HocPhi<>ROUND(@HocPhi,0) THROW 51137,N'Học phí phải là số VND nguyên không âm.',1;
 IF @HinhThucHoc NOT IN('TrucTuyen','TrucTiep') OR (@HinhThucHoc='TrucTiep' AND NULLIF(LTRIM(RTRIM(@DiaDiem)),N'') IS NULL) THROW 51137,N'Chọn hình thức và địa điểm học hợp lệ.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.HoSoGiaSu h JOIN dbo.HoSoGiaSuMonDay m ON m.HoSoGiaSuId=h.Id WHERE h.GiaSuId=@GiaSuId AND m.MonHocId=@MonHocId) THROW 51134,N'Môn học chưa đăng ký trong hồ sơ gia sư.',1;
 IF @NhuCauId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.NhuCauHoc n WITH(UPDLOCK,HOLDLOCK) WHERE n.Id=@NhuCauId AND n.ChuSoHuuId=@BenHocId AND n.TrangThai='DangMo' AND (n.MonHocId=@MonHocId OR EXISTS(SELECT 1 FROM dbo.NhuCauMonHoc m WHERE m.NhuCauId=n.Id AND m.MonHocId=@MonHocId))) THROW 51131,N'Môn đề xuất không thuộc nhu cầu học đang mở.',1;
 IF @NhuCauId IS NOT NULL AND EXISTS(SELECT 1 FROM dbo.BienPhapQuanTri WITH(UPDLOCK,HOLDLOCK) WHERE LoaiDoiTuong='NhuCauHoc' AND DoiTuongId=@NhuCauId AND Loai='AnNoiDung' AND GoLuc IS NULL) THROW 51138,N'Nhu cầu đang bị ẩn để quản trị xem xét.',1;
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
  VALUES(@YeuCauId,@GiaSuId,@BenHocId,@NguoiThucHienId,@NhuCauId,@Cho,@Now,DATEADD(hour,72,@Now),@TenLop,@MonHocId,@MucTieu,@SoBuoiTuan,@ThoiLuongPhut,@BatDauHocLuc,@HinhThucHoc,@DiaDiem,@HocPhi,@DonViHocPhi,@LienHeId,@NguoiThucHienId,@Now);
 END
 ELSE
 BEGIN
  DECLARE @LockedId uniqueidentifier;
  SELECT @LockedId=Id FROM dbo.YeuCauKetNoiLopHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@YeuCauId;
  SET @Now=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
  IF EXISTS(SELECT 1 FROM dbo.YeuCauKetNoiLopHoc WHERE Id=@YeuCauId AND NguoiGuiId=@BenHocId AND (NhuCauId<>@NhuCauId OR @NhuCauId IS NULL)) THROW 51139,N'Giữ nhu cầu đã được bên học gửi.',1;
  UPDATE dbo.YeuCauKetNoiLopHoc SET TrangThai=@Cho,NhuCauId=@NhuCauId,TenLopDuKien=@TenLop,MonHocId=@MonHocId,MucTieu=@MucTieu,SoBuoiTuan=@SoBuoiTuan,ThoiLuongPhut=@ThoiLuongPhut,BatDauHocLuc=@BatDauHocLuc,HinhThucHoc=@HinhThucHoc,DiaDiem=@DiaDiem,HocPhi=@HocPhi,DonViHocPhi=@DonViHocPhi,NguoiDeXuatId=@NguoiThucHienId,DeXuatLuc=@Now,HetHanLuc=DATEADD(hour,72,@Now)
  WHERE Id=@YeuCauId AND LienHeId=@LienHeId AND GiaSuId=@GiaSuId AND (NguoiDeXuatId=@NguoiThucHienId OR TrangThai='ChoGiaSu') AND TrangThai IN ('ChoGiaSu','ChoBenHoc') AND HetHanLuc>@Now AND RowVersion=@RowVersionCu;
  IF @@ROWCOUNT<>1 THROW 51139,N'Yêu cầu đã đổi phiên bản, hết hạn hoặc không thuộc gia sư.',1;
 END;
 UPDATE dbo.YeuCauKetNoiLopHoc SET LoiNhan=@LoiNhan,TenNguoiHoc=LTRIM(RTRIM(@TenNguoiHoc)) WHERE Id=@YeuCauId;
 DELETE dbo.KhungGioDeXuat WHERE YeuCauKetNoiId=@YeuCauId;
 INSERT dbo.KhungGioDeXuat(YeuCauKetNoiId,ThuTrongTuan,GioBatDau,GioKetThuc) SELECT @YeuCauId,Thu,BatDau,KetThuc FROM @Slots;
 COMMIT;SELECT * FROM dbo.YeuCauKetNoiLopHoc WHERE Id=@YeuCauId;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK;THROW;END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_ChapNhanDeXuatTaoLop
 @YeuCauId uniqueidentifier,@BenHocId uniqueidentifier,@RowVersionCu binary(8),@NguoiThucHienId uniqueidentifier=NULL
AS
BEGIN
 SET NOCOUNT ON;SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRAN;
 DECLARE @GiaSuId uniqueidentifier,@LienHeId uniqueidentifier,@Lock uniqueidentifier,@TrangThai varchar(32),@NhuCauId uniqueidentifier,@LopId uniqueidentifier,@Now datetimeoffset(0)=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 SELECT @GiaSuId=GiaSuId,@LienHeId=LienHeId FROM dbo.YeuCauKetNoiLopHoc WHERE Id=@YeuCauId AND BenHocId=@BenHocId;
 IF @GiaSuId IS NULL THROW 51151,N'Yêu cầu không thuộc bên học.',1;
 SET @NguoiThucHienId=COALESCE(@NguoiThucHienId,@BenHocId);
 IF @NguoiThucHienId<>@BenHocId THROW 51140,N'Chỉ bên học được xác nhận tạo lớp.',1;
 DECLARE @First uniqueidentifier=CASE WHEN @GiaSuId<@BenHocId THEN @GiaSuId ELSE @BenHocId END,@Second uniqueidentifier=CASE WHEN @GiaSuId<@BenHocId THEN @BenHocId ELSE @GiaSuId END;
 SELECT @Lock=Id FROM dbo.NguoiThamGia WITH(UPDLOCK,HOLDLOCK) WHERE Id=@First;
 SELECT @Lock=Id FROM dbo.NguoiThamGia WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Second;
 SELECT @Lock=Id FROM dbo.YeuCauLienHe WITH(UPDLOCK,HOLDLOCK) WHERE Id=@LienHeId AND TrangThai='ChapNhan';
 SELECT @TrangThai=TrangThai,@NhuCauId=NhuCauId FROM dbo.YeuCauKetNoiLopHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@YeuCauId;
 SET @Now=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 IF EXISTS(SELECT 1 FROM dbo.AspNetUsers WHERE Id IN(@GiaSuId,@BenHocId) AND TrangThai<>'HoatDong') THROW 51140,N'Tài khoản hiện không hoạt động.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.YeuCauLienHe WHERE Id=@LienHeId AND TrangThai='ChapNhan') THROW 51140,N'Liên hệ không còn hiệu lực.',1;
 IF @NhuCauId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.NhuCauHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@NhuCauId AND ChuSoHuuId=@BenHocId AND TrangThai='DangMo') THROW 51131,N'Nhu cầu không còn mở.',1;
 IF @TrangThai='DaChapNhan'
 BEGIN
  SELECT @LopId=Id FROM dbo.LopHoc WHERE YeuCauKetNoiId=@YeuCauId;
  COMMIT;SELECT * FROM dbo.LopHoc WHERE Id=@LopId;RETURN;
 END;
 IF @NhuCauId IS NOT NULL AND EXISTS(SELECT 1 FROM dbo.BienPhapQuanTri WITH(UPDLOCK,HOLDLOCK) WHERE LoaiDoiTuong='NhuCauHoc' AND DoiTuongId=@NhuCauId AND Loai='AnNoiDung' AND GoLuc IS NULL) THROW 51152,N'Nhu cầu đang bị ẩn để quản trị xem xét.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.YeuCauKetNoiLopHoc WHERE Id=@YeuCauId AND (TrangThai='ChoBenHoc' AND @NguoiThucHienId=@BenHocId AND NguoiDeXuatId=@GiaSuId) AND RowVersion=@RowVersionCu AND HetHanLuc>@Now) THROW 51152,N'Đề xuất không còn hiệu lực hoặc đã đổi phiên bản.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.KhungGioDeXuat WHERE YeuCauKetNoiId=@YeuCauId) THROW 51153,N'Đề xuất chưa có lịch dự kiến.',1;
 DECLARE @Ten nvarchar(256),@Tuoi int,@TenTaiKhoan nvarchar(256);
 SELECT @TenTaiKhoan=HoTen FROM dbo.NguoiThamGia WHERE Id=@BenHocId;
 SELECT @Ten=TenNguoiHoc,@Tuoi=TuoiNguoiHoc FROM dbo.NhuCauHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@NhuCauId AND ChuSoHuuId=@BenHocId;
 SELECT @Ten=COALESCE(NULLIF(LTRIM(RTRIM(TenNguoiHoc)),N''),@Ten,@TenTaiKhoan) FROM dbo.YeuCauKetNoiLopHoc WHERE Id=@YeuCauId;
 IF NULLIF(LTRIM(RTRIM(@Ten)),N'') IS NULL THROW 51154,N'Đề xuất chưa có tên người học.',1;
 SET @LopId=NEWID();
 INSERT dbo.LopHoc(Id,YeuCauKetNoiId,GiaSuId,BenHocId,TenLop,MonHocId,MucTieu,SoBuoiTuan,ThoiLuongPhut,HinhThucHoc,DiaDiem,HocPhi,TrangThai,DonViHocPhi,TenNguoiHoc,TuoiNguoiHoc)
 SELECT @LopId,Id,GiaSuId,BenHocId,TenLopDuKien,MonHocId,MucTieu,SoBuoiTuan,ThoiLuongPhut,HinhThucHoc,DiaDiem,HocPhi,'DangHoatDong',DonViHocPhi,@Ten,@Tuoi FROM dbo.YeuCauKetNoiLopHoc WHERE Id=@YeuCauId;
 INSERT dbo.ThanhVienLopHoc(LopHocId,TaiKhoanId,NguoiDeNghiId,TrangThaiDuyet,MucQuyen,DeNghiLuc,DuyetLuc) VALUES(@LopId,@GiaSuId,@GiaSuId,'DaDuyet','ChinhSua',@Now,@Now),(@LopId,@BenHocId,@GiaSuId,'DaDuyet','ChinhSua',@Now,@Now);
 INSERT dbo.CuocHoiThoai(LoaiHoiThoai,GiaSuId,BenHocId,LopHocId,LienHeId) VALUES('TheoLop',@GiaSuId,@BenHocId,@LopId,@LienHeId);
 DECLARE @Start datetimeoffset(0),@StartDate date,@Offset int,@Json nvarchar(max);
 SELECT @Start=BatDauHocLuc FROM dbo.YeuCauKetNoiLopHoc WHERE Id=@YeuCauId;
 SET @StartDate=CONVERT(date,@Start);SET @Offset=DATEPART(TZOFFSET,@Start);
 SELECT @Json=(SELECT c.TenLop AS TieuDe,
 TODATETIMEOFFSET(DATEADD(second,DATEDIFF(second,CONVERT(time(0),'00:00'),s.GioBatDau),DATEADD(day,(s.ThuTrongTuan-((DATEDIFF(day,'19000107',@StartDate)%7+7)%7)+7)%7,CONVERT(datetime2(0),@StartDate))),@Offset) AS BatDauLuc,
 TODATETIMEOFFSET(DATEADD(second,DATEDIFF(second,CONVERT(time(0),'00:00'),s.GioKetThuc),DATEADD(day,(s.ThuTrongTuan-((DATEDIFF(day,'19000107',@StartDate)%7+7)%7)+7)%7,CONVERT(datetime2(0),@StartDate))),@Offset) AS KetThucLuc
 FROM dbo.KhungGioDeXuat s CROSS JOIN dbo.LopHoc c WHERE s.YeuCauKetNoiId=@YeuCauId AND c.Id=@LopId FOR JSON PATH);
 EXEC dbo.usp_ThemLichHocHangLoat @LopHocId=@LopId,@GiaSuId=@GiaSuId,@DanhSachBuoiJson=@Json;
 UPDATE dbo.YeuCauKetNoiLopHoc SET TrangThai='DaChapNhan' WHERE Id=@YeuCauId;
 COMMIT;SELECT * FROM dbo.LopHoc WHERE Id=@LopId;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK;THROW;END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_GuiYeuCauKetNoi
 @LienHeId uniqueidentifier,@BenHocId uniqueidentifier,@LoiNhan nvarchar(256),@NhuCauId uniqueidentifier=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRAN;
 DECLARE @GiaSuId uniqueidentifier,@Id uniqueidentifier=NEWID(),@Now datetimeoffset(0)=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 SELECT @GiaSuId=GiaSuId FROM dbo.YeuCauLienHe WITH(UPDLOCK,HOLDLOCK) WHERE Id=@LienHeId AND BenHocId=@BenHocId AND TrangThai='ChapNhan';
 SET @Now=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 IF @GiaSuId IS NULL OR @LoiNhan IS NULL OR LEN(LTRIM(RTRIM(@LoiNhan)))=0 THROW 51130,N'Liên hệ chưa được chấp nhận hoặc lời nhắn trống.',1;
 IF @NhuCauId IS NULL THROW 51131,N'Chọn nhu cầu học để gửi yêu cầu.',1;
 IF @NhuCauId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.NhuCauHoc WHERE Id=@NhuCauId AND ChuSoHuuId=@BenHocId AND LoaiNhuCau='TimGiaSu' AND TrangThai='DangMo') THROW 51131,N'Nhu cầu học không hợp lệ.',1;
 IF @NhuCauId IS NOT NULL AND EXISTS(SELECT 1 FROM dbo.BienPhapQuanTri WITH(UPDLOCK,HOLDLOCK) WHERE LoaiDoiTuong='NhuCauHoc' AND DoiTuongId=@NhuCauId AND Loai='AnNoiDung' AND GoLuc IS NULL) THROW 51138,N'Nhu cầu đang bị ẩn để quản trị xem xét.',1;
 IF EXISTS(SELECT 1 FROM dbo.YeuCauKetNoiLopHoc WHERE LienHeId=@LienHeId AND TrangThai IN ('ChoGiaSu','ChoBenHoc') AND HetHanLuc>@Now) THROW 51132,N'Đang có yêu cầu kết nối chờ phản hồi.',1;
 INSERT dbo.YeuCauKetNoiLopHoc(Id,GiaSuId,BenHocId,NguoiGuiId,NhuCauId,LoiNhan,TrangThai,GuiLuc,HetHanLuc,LienHeId)
 VALUES(@Id,@GiaSuId,@BenHocId,@BenHocId,@NhuCauId,@LoiNhan,'ChoGiaSu',@Now,DATEADD(hour,72,@Now),@LienHeId);
 COMMIT;SELECT * FROM dbo.YeuCauKetNoiLopHoc WHERE Id=@Id;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK;THROW;END CATCH;
END;
GO
