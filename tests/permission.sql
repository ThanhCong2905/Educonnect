USE [EduConnect_Verification_20261025];
GO
CREATE OR ALTER PROCEDURE dbo.usp_ChapNhanDeXuatTaoLop
 @YeuCauId uniqueidentifier,@BenHocId uniqueidentifier,@RowVersionCu binary(8)
AS
BEGIN
 SET NOCOUNT ON;SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRAN;
 DECLARE @GiaSuId uniqueidentifier,@LienHeId uniqueidentifier,@Lock uniqueidentifier,@TrangThai varchar(32),@NhuCauId uniqueidentifier,@LopId uniqueidentifier,@Now datetimeoffset(0)=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 SELECT @GiaSuId=GiaSuId,@LienHeId=LienHeId FROM dbo.YeuCauKetNoiLopHoc WHERE Id=@YeuCauId AND BenHocId=@BenHocId;
 IF @GiaSuId IS NULL THROW 51151,N'Yêu cầu không thuộc bên học.',1;
 DECLARE @First uniqueidentifier=CASE WHEN @GiaSuId<@BenHocId THEN @GiaSuId ELSE @BenHocId END,@Second uniqueidentifier=CASE WHEN @GiaSuId<@BenHocId THEN @BenHocId ELSE @GiaSuId END;
 SELECT @Lock=Id FROM dbo.NguoiThamGia WITH(UPDLOCK,HOLDLOCK) WHERE Id=@First;
 SELECT @Lock=Id FROM dbo.NguoiThamGia WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Second;
 SELECT @Lock=Id FROM dbo.YeuCauLienHe WITH(UPDLOCK,HOLDLOCK) WHERE Id=@LienHeId AND TrangThai='ChapNhan';
 SELECT @TrangThai=TrangThai,@NhuCauId=NhuCauId FROM dbo.YeuCauKetNoiLopHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@YeuCauId;
 SET @Now=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 IF @TrangThai='DaChapNhan'
 BEGIN
  SELECT @LopId=Id FROM dbo.LopHoc WHERE YeuCauKetNoiId=@YeuCauId;
  COMMIT;SELECT * FROM dbo.LopHoc WHERE Id=@LopId;RETURN;
 END;
 IF NOT EXISTS(SELECT 1 FROM dbo.YeuCauKetNoiLopHoc WHERE Id=@YeuCauId AND TrangThai='ChoBenHoc' AND NguoiDeXuatId=@GiaSuId AND RowVersion=@RowVersionCu AND HetHanLuc>@Now) THROW 51152,N'Đề xuất không còn hiệu lực hoặc đã đổi phiên bản.',1;
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

-- Approved change only for the principal learner, not additional members.
UPDATE m SET MucQuyen='ChinhSua' FROM dbo.ThanhVienLopHoc m JOIN dbo.LopHoc c ON c.Id=m.LopHocId AND c.BenHocId=m.TaiKhoanId WHERE m.TrangThaiDuyet='DaDuyet' AND m.RoiLopLuc IS NULL AND m.MucQuyen='Xem';
-- Original seed has no membership rows; repair only absent principal memberships.
INSERT dbo.ThanhVienLopHoc(LopHocId,TaiKhoanId,NguoiDeNghiId,TrangThaiDuyet,MucQuyen,DuyetLuc) SELECT c.Id,p.Id,c.GiaSuId,'DaDuyet','ChinhSua',SYSDATETIMEOFFSET() FROM dbo.LopHoc c CROSS APPLY(VALUES(c.GiaSuId),(c.BenHocId)) p(Id) WHERE NOT EXISTS(SELECT 1 FROM dbo.ThanhVienLopHoc m WHERE m.LopHocId=c.Id AND m.TaiKhoanId=p.Id AND m.RoiLopLuc IS NULL AND m.TrangThaiDuyet<>'TuChoi');

