SET XACT_ABORT ON;
BEGIN TRANSACTION;
ALTER TABLE dbo.BuoiHoc DROP CONSTRAINT CK_Buoi_ThamDu;
ALTER TABLE dbo.BuoiHoc ADD CONSTRAINT CK_Buoi_ThamDu CHECK(
    (TrangThaiThamDu IS NULL OR TrangThaiThamDu IN('CoMat','VangCoBaoTruoc','VangKhongBaoTruoc'))
    AND (TrangThaiThamDuGiaSu IS NULL OR TrangThaiThamDuGiaSu IN('CoMat','VangCoBaoTruoc','VangKhongBaoTruoc'))
    AND (TrangThaiThamDuBenHoc IS NULL OR TrangThaiThamDuBenHoc IN('CoMat','VangCoBaoTruoc','VangKhongBaoTruoc'))
    AND (KetQuaBuoiHoc IS NULL OR KetQuaBuoiHoc IN('DaDienRa','KhongDienRa'))
    AND (KetQuaBuoiHoc<>'KhongDienRa' OR TrangThaiThamDuGiaSu<>'CoMat' OR TrangThaiThamDuBenHoc<>'CoMat')
    AND (GhiNhanLuc IS NULL OR (NguoiGhiNhanId IS NOT NULL AND KetQuaBuoiHoc IS NOT NULL AND TrangThaiThamDuGiaSu IS NOT NULL AND TrangThaiThamDuBenHoc IS NOT NULL AND (KetQuaBuoiHoc='DaDienRa' OR LEN(LTRIM(RTRIM(ISNULL(NhanXet,N''))))>0))));
COMMIT;
GO
CREATE OR ALTER PROCEDURE dbo.usp_GhiNhanBuoiHoc
 @LichHocId uniqueidentifier,@GiaSuId uniqueidentifier,@KetQuaBuoiHoc varchar(32),@NhanXet nvarchar(max),
 @TrangThaiGiaSu varchar(32),@TrangThaiBenHoc varchar(32),@NoiDung nvarchar(max)=NULL,@RowVersionCu binary(8)=NULL,@CanHocBu bit=0
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
 IF (@KetQuaBuoiHoc='KhongDienRa' AND LEN(LTRIM(RTRIM(ISNULL(@NhanXet,N''))))=0) OR LEN(ISNULL(@NhanXet,N''))>2000 OR LEN(ISNULL(@NoiDung,N''))>10000 THROW 51148,N'Cần nhận xét hoặc lý do không diễn ra.',1;
 IF @KetQuaBuoiHoc NOT IN('DaDienRa','KhongDienRa') OR @TrangThaiGiaSu NOT IN('CoMat','VangCoBaoTruoc','VangKhongBaoTruoc') OR @TrangThaiBenHoc NOT IN('CoMat','VangCoBaoTruoc','VangKhongBaoTruoc')
 OR (@KetQuaBuoiHoc='DaDienRa' AND LEN(LTRIM(RTRIM(ISNULL(@NoiDung,N''))))=0)
 OR (@KetQuaBuoiHoc='KhongDienRa' AND @TrangThaiGiaSu='CoMat' AND @TrangThaiBenHoc='CoMat') THROW 51164,N'Kết quả, nội dung hoặc trạng thái tham dự không hợp lệ.',1;
 SELECT @Id=Id FROM dbo.BuoiHoc WITH(UPDLOCK,HOLDLOCK) WHERE LichHocId=@LichHocId;
 IF @Id IS NULL THROW 51149,N'Không có buổi học; hãy tải lại.',1;
 IF @KetQuaBuoiHoc='KhongDienRa' AND EXISTS(SELECT 1 FROM dbo.BaiTapGiao WITH(HOLDLOCK) WHERE BuoiHocId=@Id AND TrangThai='DaGiao')
    THROW 51164,N'Buổi đã có bài tập công bố. Hãy kiểm tra lại trước khi ghi nhận nghỉ.',1;
 BEGIN
  UPDATE dbo.BuoiHoc SET NguoiGhiNhanId=@GiaSuId,TieuDe=@Title,CanHocBu=CASE WHEN @KetQuaBuoiHoc='KhongDienRa' THEN @CanHocBu ELSE 0 END, TrangThai=CASE WHEN @KetQuaBuoiHoc='DaDienRa' THEN 'DaHoc' ELSE 'DaNghi' END,GhiNhanLuc=@Now,LyDoNghi=CASE WHEN @KetQuaBuoiHoc='KhongDienRa' THEN @NhanXet ELSE NULL END,NguoiThongBaoNghiId=CASE WHEN @KetQuaBuoiHoc='KhongDienRa' THEN @GiaSuId ELSE NULL END,ThongBaoNghiLuc=CASE WHEN @KetQuaBuoiHoc='KhongDienRa' THEN @Now ELSE NULL END, NoiDungGiangDay=@NoiDung,NhanXet=@NhanXet,TrangThaiThamDu=@TrangThaiBenHoc,TrangThaiThamDuGiaSu=@TrangThaiGiaSu,TrangThaiThamDuBenHoc=@TrangThaiBenHoc,KetQuaBuoiHoc=@KetQuaBuoiHoc,CapNhatLuc=@Now,PhienBan=PhienBan+1 WHERE Id=@Id AND RowVersion=@RowVersionCu;
  IF @@ROWCOUNT<>1 THROW 51150,N'Ghi nhận đã thay đổi, hãy tải lại trước khi sửa.',1;
 END;
 COMMIT;SELECT * FROM dbo.BuoiHoc WHERE Id=@Id;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK;THROW;END CATCH;
END;
GO
