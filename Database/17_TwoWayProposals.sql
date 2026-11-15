SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO
SET XACT_ABORT ON;
BEGIN TRAN;
ALTER TABLE dbo.YeuCauKetNoiLopHoc ALTER COLUMN LoiNhan nvarchar(2000) NULL;
ALTER TABLE dbo.YeuCauKetNoiLopHoc DROP CONSTRAINT [CK_YeuCauKetNoiLopHoc_13];
ALTER TABLE dbo.YeuCauKetNoiLopHoc WITH CHECK ADD CONSTRAINT [CK_YeuCauKetNoiLopHoc_13] CHECK ([NguoiDeXuatId] IS NULL OR [NguoiDeXuatId] IN ([GiaSuId],[BenHocId]));
ALTER TABLE dbo.YeuCauKetNoiLopHoc DROP CONSTRAINT [CK_YeuCauKetNoiLopHoc_15];
ALTER TABLE dbo.YeuCauKetNoiLopHoc WITH CHECK ADD CONSTRAINT [CK_YeuCauKetNoiLopHoc_15] CHECK ([TrangThai] NOT IN ('ChoBenHoc','DaChapNhan') OR ([NguoiDeXuatId] IN ([GiaSuId],[BenHocId]) AND [NguoiDeXuatId] IS NOT NULL AND [TenLopDuKien] IS NOT NULL AND LEN(LTRIM(RTRIM([TenLopDuKien])))>0 AND [MonHocId] IS NOT NULL AND [MucTieu] IS NOT NULL AND [SoBuoiTuan] IS NOT NULL AND [ThoiLuongPhut] IS NOT NULL AND [BatDauHocLuc] IS NOT NULL AND [HinhThucHoc] IS NOT NULL AND [HocPhi] IS NOT NULL AND [DonViHocPhi] IS NOT NULL));
ALTER TABLE dbo.YeuCauKetNoiLopHoc DROP CONSTRAINT [CK_YeuCauKetNoiLopHoc_16];
ALTER TABLE dbo.YeuCauKetNoiLopHoc WITH CHECK ADD CONSTRAINT [CK_YeuCauKetNoiLopHoc_16] CHECK ([NguoiGuiId]<>[BenHocId] OR [NguoiDeXuatId]=[BenHocId] OR ([LoiNhan] IS NOT NULL AND LEN(LTRIM(RTRIM([LoiNhan])))>0));
COMMIT;
GO
CREATE OR ALTER TRIGGER dbo.tr_KetNoi_LienHeChapNhan ON dbo.YeuCauKetNoiLopHoc AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.YeuCauLienHe h ON h.Id=i.LienHeId WHERE h.TrangThai<>'ChapNhan' OR h.GiaSuId<>i.GiaSuId OR h.BenHocId<>i.BenHocId)
 THROW 51113,N'Kết nối phải có liên hệ đã chấp nhận của đúng hai bên.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.NhuCauHoc n ON n.Id=i.NhuCauId WHERE n.ChuSoHuuId<>i.BenHocId OR n.LoaiNhuCau<>'TimGiaSu')
 THROW 51114,N'Nhu cầu gắn yêu cầu kết nối phải thuộc bên học.',1;
 IF EXISTS(SELECT 1 FROM inserted i WHERE i.TrangThai='ChoGiaSu' AND (i.NguoiGuiId<>i.BenHocId OR (i.NguoiDeXuatId IS NOT NULL AND i.NguoiDeXuatId<>i.BenHocId)))
 THROW 51115,N'Chờ gia sư là yêu cầu ban đầu của bên học, chưa có đề xuất.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.Id=i.Id WHERE d.TrangThai IN ('DaChapNhan','TuChoi','DaRut','HetHan'))
 THROW 51116,N'Yêu cầu đã đóng không được sửa hoặc chuyển trạng thái lại.',1;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_GuiDeXuatLop
 @LienHeId uniqueidentifier,@GiaSuId uniqueidentifier,@TenLop nvarchar(256),@MonHocId int,@MucTieu nvarchar(256),
 @SoBuoiTuan int,@ThoiLuongPhut int,@BatDauHocLuc datetimeoffset(0),@HinhThucHoc varchar(32),
 @HocPhi decimal(18,2),@DonViHocPhi varchar(16),@KhungGioJson nvarchar(max),
 @DiaDiem nvarchar(256)=NULL,@NhuCauId uniqueidentifier=NULL,@YeuCauId uniqueidentifier=NULL,@RowVersionCu binary(8)=NULL,@NguoiThucHienId uniqueidentifier=NULL,@LoiNhan nvarchar(2000)=NULL
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
 IF @NguoiThucHienId NOT IN(@GiaSuId,@BenHocId) THROW 51140,N'Không có quyền gửi đề xuất.',1;
 DECLARE @Cho varchar(32)=CASE WHEN @NguoiThucHienId=@GiaSuId THEN 'ChoBenHoc' ELSE 'ChoGiaSu' END;
 IF EXISTS(SELECT 1 FROM dbo.AspNetUsers WITH(UPDLOCK,HOLDLOCK) WHERE Id IN(@GiaSuId,@BenHocId) AND TrangThai<>'HoatDong') THROW 51140,N'Tài khoản hiện không hoạt động.',1;
 IF EXISTS(SELECT 1 FROM dbo.LopHoc WITH(UPDLOCK,HOLDLOCK) WHERE GiaSuId=@GiaSuId AND BenHocId=@BenHocId AND TrangThai='DangHoatDong') THROW 51138,N'Hai bên đã có lớp đang hoạt động.',1;
 IF @BatDauHocLuc<=@Now THROW 51137,N'Ngày bắt đầu phải ở tương lai.',1;
 IF @HocPhi<>ROUND(@HocPhi,0) THROW 51137,N'Học phí phải là số VND nguyên không âm.',1;
 IF @HinhThucHoc NOT IN('TrucTuyen','TrucTiep') OR (@HinhThucHoc='TrucTiep' AND NULLIF(LTRIM(RTRIM(@DiaDiem)),N'') IS NULL) THROW 51137,N'Chọn hình thức và địa điểm học hợp lệ.',1;
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
  VALUES(@YeuCauId,@GiaSuId,@BenHocId,@NguoiThucHienId,@NhuCauId,@Cho,@Now,DATEADD(hour,72,@Now),@TenLop,@MonHocId,@MucTieu,@SoBuoiTuan,@ThoiLuongPhut,@BatDauHocLuc,@HinhThucHoc,@DiaDiem,@HocPhi,@DonViHocPhi,@LienHeId,@NguoiThucHienId,@Now);
 END
 ELSE
 BEGIN
  DECLARE @LockedId uniqueidentifier;
  SELECT @LockedId=Id FROM dbo.YeuCauKetNoiLopHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@YeuCauId;
  SET @Now=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
  UPDATE dbo.YeuCauKetNoiLopHoc SET TrangThai=@Cho,NhuCauId=@NhuCauId,TenLopDuKien=@TenLop,MonHocId=@MonHocId,MucTieu=@MucTieu,SoBuoiTuan=@SoBuoiTuan,ThoiLuongPhut=@ThoiLuongPhut,BatDauHocLuc=@BatDauHocLuc,HinhThucHoc=@HinhThucHoc,DiaDiem=@DiaDiem,HocPhi=@HocPhi,DonViHocPhi=@DonViHocPhi,NguoiDeXuatId=@NguoiThucHienId,DeXuatLuc=@Now,HetHanLuc=DATEADD(hour,72,@Now)
  WHERE Id=@YeuCauId AND LienHeId=@LienHeId AND GiaSuId=@GiaSuId AND (NguoiDeXuatId=@NguoiThucHienId OR NguoiDeXuatId IS NULL) AND TrangThai IN ('ChoGiaSu','ChoBenHoc') AND HetHanLuc>@Now AND RowVersion=@RowVersionCu;
  IF @@ROWCOUNT<>1 THROW 51139,N'Yêu cầu đã đổi phiên bản, hết hạn hoặc không thuộc gia sư.',1;
 END;
 UPDATE dbo.YeuCauKetNoiLopHoc SET LoiNhan=@LoiNhan WHERE Id=@YeuCauId;
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
 IF @NguoiThucHienId NOT IN(@GiaSuId,@BenHocId) THROW 51140,N'Không có quyền xác nhận đề xuất.',1;
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
 IF NOT EXISTS(SELECT 1 FROM dbo.YeuCauKetNoiLopHoc WHERE Id=@YeuCauId AND ((TrangThai='ChoBenHoc' AND @NguoiThucHienId=@BenHocId AND NguoiDeXuatId=@GiaSuId) OR (TrangThai='ChoGiaSu' AND @NguoiThucHienId=@GiaSuId AND NguoiDeXuatId=@BenHocId)) AND RowVersion=@RowVersionCu AND HetHanLuc>@Now) THROW 51152,N'Đề xuất không còn hiệu lực hoặc đã đổi phiên bản.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.KhungGioDeXuat WHERE YeuCauKetNoiId=@YeuCauId) THROW 51153,N'Đề xuất chưa có lịch dự kiến.',1;
 DECLARE @Ten nvarchar(256),@Tuoi int,@TenTaiKhoan nvarchar(256);
 SELECT @TenTaiKhoan=HoTen FROM dbo.NguoiThamGia WHERE Id=@BenHocId;
 SELECT @Ten=TenNguoiHoc,@Tuoi=TuoiNguoiHoc FROM dbo.NhuCauHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@NhuCauId AND ChuSoHuuId=@BenHocId;
 IF EXISTS(SELECT 1 FROM dbo.NguoiThamGia WHERE Id=@BenHocId AND LoaiNguoi='PhuHuynh')
 BEGIN
  IF @Ten IS NULL OR LEN(LTRIM(RTRIM(@Ten)))=0 OR @Tuoi IS NULL THROW 51154,N'Phụ huynh cần nhu cầu có tên và tuổi người học trước khi tạo lớp.',1;
 END
 ELSE SET @Ten=@TenTaiKhoan;
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
