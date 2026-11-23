SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.LichSuQuyenThanhVien',N'U') IS NULL
BEGIN
 CREATE TABLE dbo.LichSuQuyenThanhVien(
  Id uniqueidentifier NOT NULL CONSTRAINT PK_LichSuQuyenThanhVien PRIMARY KEY,
  ThanhVienId uniqueidentifier NOT NULL,
  NguoiThucHienId uniqueidentifier NOT NULL,
  MucQuyenCu varchar(32) NULL,
  MucQuyenMoi varchar(32) NOT NULL,
  ChoPhepNopBaiCu bit NOT NULL,
  ChoPhepNopBaiMoi bit NOT NULL,
  ThayDoiLuc datetimeoffset NOT NULL,
  CONSTRAINT FK_LichSuQuyen_ThanhVien FOREIGN KEY(ThanhVienId) REFERENCES dbo.ThanhVienLopHoc(Id),
  CONSTRAINT FK_LichSuQuyen_NguoiThucHien FOREIGN KEY(NguoiThucHienId) REFERENCES dbo.AspNetUsers(Id),
  CONSTRAINT CK_LichSuQuyen_Muc CHECK((MucQuyenCu IS NULL OR MucQuyenCu IN('Xem','ChinhSua')) AND MucQuyenMoi IN('Xem','ChinhSua')),
  CONSTRAINT CK_LichSuQuyen_NopBai CHECK(MucQuyenMoi='ChinhSua' OR ChoPhepNopBaiMoi=0)
 );
 CREATE INDEX IX_LichSuQuyen_ThanhVien_ThoiDiem ON dbo.LichSuQuyenThanhVien(ThanhVienId,ThayDoiLuc DESC);
END;
IF DATABASE_PRINCIPAL_ID(N'KLTN_App') IS NOT NULL
BEGIN
 GRANT SELECT,INSERT ON dbo.LichSuQuyenThanhVien TO KLTN_App;
 DENY UPDATE,DELETE ON dbo.LichSuQuyenThanhVien TO KLTN_App;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_GuiDeXuatLop
 @LienHeId uniqueidentifier,@GiaSuId uniqueidentifier,@TenLop nvarchar(256),@MonHocId int,@MucTieu nvarchar(2000),
 @SoBuoiTuan int,@ThoiLuongPhut int,@BatDauHocLuc datetimeoffset(0),@HinhThucHoc varchar(32),
 @HocPhi decimal(18,2),@DonViHocPhi varchar(16),@KhungGioJson nvarchar(max),
 @DiaDiem nvarchar(500)=NULL,@NhuCauId uniqueidentifier=NULL,@YeuCauId uniqueidentifier=NULL,@RowVersionCu binary(8)=NULL,@NguoiThucHienId uniqueidentifier=NULL,@TenNguoiHoc nvarchar(256)=NULL
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
 UPDATE dbo.YeuCauKetNoiLopHoc SET TenNguoiHoc=LTRIM(RTRIM(@TenNguoiHoc)) WHERE Id=@YeuCauId;
 DELETE dbo.KhungGioDeXuat WHERE YeuCauKetNoiId=@YeuCauId;
 INSERT dbo.KhungGioDeXuat(YeuCauKetNoiId,ThuTrongTuan,GioBatDau,GioKetThuc) SELECT @YeuCauId,Thu,BatDau,KetThuc FROM @Slots;
 COMMIT;SELECT * FROM dbo.YeuCauKetNoiLopHoc WHERE Id=@YeuCauId;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK;THROW;END CATCH;
END;
GO
COMMIT;
GO
