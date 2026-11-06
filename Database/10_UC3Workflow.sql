-- Nâng cấp UC3: không tạo lại database, không xóa dữ liệu, không thêm cột.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET XACT_ABORT ON;
BEGIN TRAN;
GO
BEGIN TRAN;
IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.LichHoc') AND name=N'CK_LichHoc_4')
 ALTER TABLE dbo.LichHoc DROP CONSTRAINT CK_LichHoc_4;
ALTER TABLE dbo.LichHoc WITH CHECK ADD CONSTRAINT CK_LichHoc_4 CHECK(LaBuoiBu=0 OR LichGocId IS NOT NULL);
COMMIT;
GO
CREATE OR ALTER PROCEDURE dbo.usp_NopBaiTap
 @BaiTapGiaoId uniqueidentifier,@NguoiNopId uniqueidentifier,@MaThaoTac uniqueidentifier,@NoiDung nvarchar(max)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
  BEGIN TRANSACTION;
  DECLARE @LopId uniqueidentifier,@Lan int,@Id uniqueidentifier;
  SELECT @LopId=LopHocId FROM dbo.BaiTapGiao WITH(UPDLOCK,HOLDLOCK) WHERE Id=@BaiTapGiaoId;
  IF @LopId IS NULL THROW 51032,N'Bài giao không tồn tại.',1;
  IF NOT EXISTS(SELECT 1 FROM dbo.LopHoc c WITH(UPDLOCK,HOLDLOCK)
   WHERE c.Id=@LopId AND c.GiaSuId<>@NguoiNopId AND (c.BenHocId=@NguoiNopId OR EXISTS(
    SELECT 1 FROM dbo.ThanhVienLopHoc m WHERE m.LopHocId=c.Id AND m.TaiKhoanId=@NguoiNopId
    AND m.TrangThaiDuyet='DaDuyet' AND m.RoiLopLuc IS NULL AND m.MucQuyen='ChinhSua')))
   THROW 51160,N'Không có quyền nộp bài trong lớp.',1;
  IF NOT EXISTS(SELECT 1 FROM dbo.LopHoc WHERE Id=@LopId AND TrangThai='DangHoatDong') THROW 51162,N'Lớp không còn hoạt động.',1;
  IF @MaThaoTac IS NULL OR @MaThaoTac='00000000-0000-0000-0000-000000000000' OR LEN(ISNULL(@NoiDung,N''))>10000 THROW 51164,N'Mã thao tác hoặc nội dung bài làm không hợp lệ.',1;
  SELECT @Id=Id FROM dbo.BaiTapNop WHERE NguoiNopId=@NguoiNopId AND MaThaoTac=@MaThaoTac;
  IF @Id IS NOT NULL
  BEGIN
   IF NOT EXISTS(SELECT 1 FROM dbo.BaiTapNop WHERE Id=@Id AND BaiTapGiaoId=@BaiTapGiaoId AND ISNULL(NoiDung,N'')=ISNULL(@NoiDung,N'')) THROW 51033,N'Mã thao tác đã dùng với dữ liệu khác.',1;
   COMMIT; SELECT * FROM dbo.BaiTapNop WHERE Id=@Id; RETURN;
  END;
  IF EXISTS(SELECT 1 FROM dbo.BaiTapNop n JOIN dbo.KetQuaChamBai k ON k.BaiTapNopId=n.Id WHERE n.BaiTapGiaoId=@BaiTapGiaoId)
    THROW 51034,N'Bài đã chấm; không nhận lần nộp mới.',1;
  SELECT @Lan=ISNULL(MAX(LanNop),0)+1 FROM dbo.BaiTapNop WHERE BaiTapGiaoId=@BaiTapGiaoId;
  SET @Id=NEWID();
  INSERT dbo.BaiTapNop(Id,BaiTapGiaoId,NguoiNopId,NoiDung,MaThaoTac,LanNop) VALUES(@Id,@BaiTapGiaoId,@NguoiNopId,@NoiDung,@MaThaoTac,@Lan);
  COMMIT; SELECT * FROM dbo.BaiTapNop WHERE Id=@Id;
 END TRY
 BEGIN CATCH
  IF XACT_STATE()<>0 ROLLBACK; THROW;
 END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_ChamBaiTap
 @BaiTapNopId uniqueidentifier,@NguoiChamId uniqueidentifier,@Diem decimal(18,6)=NULL,
 @NhanXet nvarchar(max)=N'',@RowVersionCu binary(8)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
  BEGIN TRANSACTION;
  DECLARE @Giao uniqueidentifier,@LopId uniqueidentifier,@Id uniqueidentifier;
  SELECT @Giao=BaiTapGiaoId FROM dbo.BaiTapNop WHERE Id=@BaiTapNopId;
  SELECT @LopId=LopHocId FROM dbo.BaiTapGiao WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Giao;
  IF @LopId IS NULL THROW 51035,N'Bài nộp không tồn tại.',1;
  IF NOT EXISTS(SELECT 1 FROM dbo.LopHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@LopId AND GiaSuId=@NguoiChamId) THROW 51161,N'Chỉ gia sư phụ trách được chấm bài.',1;
  IF NOT EXISTS(SELECT 1 FROM dbo.LopHoc WHERE Id=@LopId AND TrangThai='DangHoatDong') THROW 51162,N'Lớp không còn hoạt động.',1;
  IF @NhanXet IS NULL OR LEN(LTRIM(RTRIM(@NhanXet)))=0 OR LEN(@NhanXet)>2000 THROW 51164,N'Cần nhận xét tối đa 2000 ký tự.',1;
  IF @Diem IS NOT NULL AND (@Diem NOT BETWEEN 0 AND 10 OR @Diem<>ROUND(@Diem,2)) THROW 51036,N'Điểm bài tập từ 0 đến 10.',1;
  IF EXISTS(SELECT 1 FROM dbo.BaiTapNop n JOIN dbo.BaiTapNop c ON c.Id=@BaiTapNopId WHERE n.BaiTapGiaoId=@Giao AND n.LanNop>c.LanNop)
    THROW 51037,N'Chỉ chấm lần nộp mới nhất.',1;
  SELECT @Id=Id FROM dbo.KetQuaChamBai WHERE BaiTapNopId=@BaiTapNopId;
  IF @Id IS NULL
  BEGIN
   IF @RowVersionCu IS NOT NULL THROW 51038,N'Kết quả không còn tồn tại.',1;
   SET @Id=NEWID(); INSERT dbo.KetQuaChamBai(Id,BaiTapNopId,NguoiChamId,Diem,NhanXet) VALUES(@Id,@BaiTapNopId,@NguoiChamId,@Diem,@NhanXet);
  END
  ELSE
  BEGIN
   UPDATE dbo.KetQuaChamBai SET NguoiChamId=@NguoiChamId,Diem=@Diem,NhanXet=@NhanXet,ChamLuc=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00') WHERE Id=@Id AND RowVersion=@RowVersionCu;
   IF @@ROWCOUNT<>1 THROW 51039,N'Kết quả đã thay đổi; tải lại trước khi sửa.',1;
  END;
  COMMIT; SELECT * FROM dbo.KetQuaChamBai WHERE Id=@Id;
 END TRY
 BEGIN CATCH
  IF XACT_STATE()<>0 ROLLBACK; THROW;
 END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_ThemLichHocHangLoat
 @LopHocId uniqueidentifier,@GiaSuId uniqueidentifier,@DanhSachBuoiJson nvarchar(max)
AS
BEGIN
 SET NOCOUNT ON;SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRAN;
 DECLARE @BenHocId uniqueidentifier,@Lock uniqueidentifier;
 SELECT @BenHocId=BenHocId FROM dbo.LopHoc WHERE Id=@LopHocId AND GiaSuId=@GiaSuId;
 IF @BenHocId IS NULL THROW 51140,N'Chỉ gia sư phụ trách được thêm lịch.',1;
 DECLARE @First uniqueidentifier=CASE WHEN @GiaSuId<@BenHocId THEN @GiaSuId ELSE @BenHocId END,@Second uniqueidentifier=CASE WHEN @GiaSuId<@BenHocId THEN @BenHocId ELSE @GiaSuId END;
 SELECT @Lock=Id FROM dbo.NguoiThamGia WITH(UPDLOCK,HOLDLOCK) WHERE Id=@First;
 SELECT @Lock=Id FROM dbo.NguoiThamGia WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Second;
 IF NOT EXISTS(SELECT 1 FROM dbo.LopHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@LopHocId AND GiaSuId=@GiaSuId AND TrangThai='DangHoatDong') THROW 51141,N'Lớp không còn hoạt động.',1;
 IF ISJSON(@DanhSachBuoiJson)<>1 THROW 51142,N'Danh sách buổi học không hợp lệ.',1;
 DECLARE @Rows TABLE(Stt int IDENTITY PRIMARY KEY,TieuDe nvarchar(256) NOT NULL,BatDau datetimeoffset(0) NOT NULL,KetThuc datetimeoffset(0) NOT NULL);
 INSERT @Rows(TieuDe,BatDau,KetThuc) SELECT TieuDe,BatDauLuc,KetThucLuc FROM OPENJSON(@DanhSachBuoiJson) WITH(TieuDe nvarchar(256),BatDauLuc datetimeoffset(0),KetThucLuc datetimeoffset(0));
 IF (SELECT COUNT(*) FROM @Rows) NOT BETWEEN 1 AND 100 OR EXISTS(SELECT 1 FROM @Rows WHERE BatDau>=KetThuc OR BatDau<=SYSDATETIMEOFFSET() OR LEN(LTRIM(RTRIM(TieuDe)))=0) THROW 51143,N'Ngày giờ đã qua hoặc khoảng giờ không hợp lệ.',1;
 IF EXISTS(SELECT 1 FROM @Rows a JOIN @Rows b ON a.Stt<b.Stt AND a.BatDau<b.KetThuc AND b.BatDau<a.KetThuc) THROW 51144,N'Danh sách chọn có các buổi trùng giờ.',1;
 IF EXISTS(SELECT 1 FROM @Rows r JOIN dbo.LichHoc l ON l.BatDauLuc<r.KetThuc AND r.BatDau<l.KetThucLuc JOIN dbo.LopHoc c ON c.Id=l.LopHocId WHERE l.TrangThai='DaLenLich' AND c.TrangThai='DangHoatDong' AND (c.Id=@LopHocId OR c.GiaSuId=@GiaSuId OR c.BenHocId=@BenHocId)) THROW 51145,N'Lịch trùng lớp hiện tại hoặc lịch của gia sư/bên học.',1;
 INSERT dbo.LichHoc(LopHocId,TieuDe,BatDauLuc,KetThucLuc,LaBuoiBu,TrangThai) SELECT @LopHocId,TieuDe,BatDau,KetThuc,0,'DaLenLich' FROM @Rows;
 COMMIT;SELECT COUNT(*) AS SoBuoiDaTao FROM @Rows;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK;THROW;END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_GhiNhanBuoiHoc
 @LichHocId uniqueidentifier,@GiaSuId uniqueidentifier,@KetQuaBuoiHoc varchar(32),@NhanXet nvarchar(max),
 @TrangThaiGiaSu varchar(32),@TrangThaiBenHoc varchar(32),@NoiDung nvarchar(max)=NULL,@RowVersionCu binary(8)=NULL
AS
BEGIN
 SET NOCOUNT ON;SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRAN;
 DECLARE @LopId uniqueidentifier,@Id uniqueidentifier,@Now datetimeoffset(0)=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00'),@Title nvarchar(256);
 SELECT @LopId=LopHocId FROM dbo.LichHoc WHERE Id=@LichHocId;
 IF NOT EXISTS(SELECT 1 FROM dbo.LopHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@LopId AND GiaSuId=@GiaSuId) THROW 51146,N'Chỉ gia sư phụ trách được ghi nhận.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.LopHoc WHERE Id=@LopId AND TrangThai='DangHoatDong') THROW 51162,N'Lớp không còn hoạt động.',1;
 SET @Now=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 SELECT @Title=TieuDe FROM dbo.LichHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@LichHocId AND TrangThai='DaLenLich' AND KetThucLuc<=@Now;
 IF @Title IS NULL THROW 51147,N'Buổi đã hủy hoặc chưa qua giờ kết thúc.',1;
 IF @NhanXet IS NULL OR LEN(LTRIM(RTRIM(@NhanXet)))=0 OR LEN(@NhanXet)>2000 OR LEN(ISNULL(@NoiDung,N''))>10000 THROW 51148,N'Cần nhận xét hoặc lý do không diễn ra.',1;
 IF @KetQuaBuoiHoc NOT IN('DaDienRa','KhongDienRa') OR @TrangThaiGiaSu NOT IN('CoMat','VangCoBaoTruoc','VangKhongBaoTruoc') OR @TrangThaiBenHoc NOT IN('CoMat','VangCoBaoTruoc','VangKhongBaoTruoc')
 OR (@KetQuaBuoiHoc='DaDienRa' AND LEN(LTRIM(RTRIM(ISNULL(@NoiDung,N''))))=0)
 OR (@KetQuaBuoiHoc='KhongDienRa' AND @TrangThaiGiaSu='CoMat' AND @TrangThaiBenHoc='CoMat') THROW 51164,N'Kết quả, nội dung hoặc trạng thái tham dự không hợp lệ.',1;
 SELECT @Id=Id FROM dbo.BuoiHoc WITH(UPDLOCK,HOLDLOCK) WHERE LichHocId=@LichHocId;
 IF @Id IS NULL
 BEGIN
  IF @RowVersionCu IS NOT NULL THROW 51149,N'Ghi nhận không còn tồn tại.',1;
  SET @Id=NEWID();INSERT dbo.BuoiHoc(Id,LichHocId,TieuDe,NoiDungGiangDay,NhanXet,TrangThaiThamDu,NguoiGhiNhanId,TrangThaiThamDuGiaSu,TrangThaiThamDuBenHoc,KetQuaBuoiHoc,CapNhatLuc)
  VALUES(@Id,@LichHocId,@Title,@NoiDung,@NhanXet,@TrangThaiBenHoc,@GiaSuId,@TrangThaiGiaSu,@TrangThaiBenHoc,@KetQuaBuoiHoc,@Now);
 END
 ELSE
 BEGIN
  UPDATE dbo.BuoiHoc SET NoiDungGiangDay=@NoiDung,NhanXet=@NhanXet,TrangThaiThamDu=@TrangThaiBenHoc,TrangThaiThamDuGiaSu=@TrangThaiGiaSu,TrangThaiThamDuBenHoc=@TrangThaiBenHoc,KetQuaBuoiHoc=@KetQuaBuoiHoc,CapNhatLuc=@Now,PhienBan=PhienBan+1 WHERE Id=@Id AND RowVersion=@RowVersionCu;
  IF @@ROWCOUNT<>1 THROW 51150,N'Ghi nhận đã thay đổi, hãy tải lại trước khi sửa.',1;
 END;
 COMMIT;SELECT * FROM dbo.BuoiHoc WHERE Id=@Id;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK;THROW;END CATCH;
END;
GO

CREATE OR ALTER TRIGGER dbo.tr_LichHoc_BuoiBu ON dbo.LichHoc AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.LichHoc g ON g.Id=i.LichGocId WHERE i.LopHocId<>g.LopHocId OR i.Id=g.Id)
   THROW 51021,N'Lịch gốc phải thuộc cùng lớp và không được trỏ chính nó.',1;
END;
GO
COMMIT;
GO
