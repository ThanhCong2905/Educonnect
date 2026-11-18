SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
ALTER TABLE dbo.YeuCauKetNoiLopHoc DROP CONSTRAINT CK_YeuCauKetNoiLopHoc_1;
ALTER TABLE dbo.YeuCauKetNoiLopHoc WITH CHECK ADD CONSTRAINT CK_YeuCauKetNoiLopHoc_1
 CHECK(TrangThai IN('ChoGiaSu','ChoBenHoc','DaChapNhan','TuChoi','DaRut','HetHan','NhuCauDaKetNoi'));
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
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.Id=i.Id WHERE d.TrangThai IN ('DaChapNhan','TuChoi','DaRut','HetHan','NhuCauDaKetNoi'))
 THROW 51116,N'Yêu cầu đã đóng không được sửa hoặc chuyển trạng thái lại.',1;
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
 IF @TrangThai='DaChapNhan'
 BEGIN
  SELECT @LopId=Id FROM dbo.LopHoc WHERE YeuCauKetNoiId=@YeuCauId;
  COMMIT;SELECT * FROM dbo.LopHoc WHERE Id=@LopId;RETURN;
 END;
 IF @TrangThai='NhuCauDaKetNoi' THROW 51152,N'Nhu cầu đã được kết nối. Đề nghị này đã kết thúc.',1;
 IF @NhuCauId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.NhuCauHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@NhuCauId AND ChuSoHuuId=@BenHocId AND TrangThai='DangMo') THROW 51131,N'Nhu cầu không còn mở.',1;
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
 UPDATE dbo.NhuCauHoc SET TrangThai='DaDong',LyDoDong=N'DaKetNoi',NhacXacNhanLuc=NULL
 WHERE Id=@NhuCauId AND ChuSoHuuId=@BenHocId AND TrangThai='DangMo';
 DECLARE @KetThuc TABLE(Id uniqueidentifier,GiaSuId uniqueidentifier,BenHocId uniqueidentifier);
 UPDATE r SET TrangThai='NhuCauDaKetNoi'
 OUTPUT inserted.Id,inserted.GiaSuId,inserted.BenHocId INTO @KetThuc
 FROM dbo.YeuCauKetNoiLopHoc r
 WHERE r.TrangThai IN('ChoGiaSu','ChoBenHoc') AND r.HetHanLuc>@Now AND r.NhuCauId=@NhuCauId AND r.Id<>@YeuCauId;
 DECLARE @ThongBaoKetThuc TABLE(MaSuKien uniqueidentifier,NguoiNhanId uniqueidentifier,YeuCauId uniqueidentifier);
 INSERT @ThongBaoKetThuc
 SELECT NEWID(),p.NguoiNhanId,k.Id FROM @KetThuc k
 CROSS APPLY(VALUES(k.GiaSuId),(k.BenHocId)) p(NguoiNhanId);
 INSERT dbo.SuKienChoXuLy(Id,LoaiSuKien,DuLieuToiThieu,TaoLuc,TrangThai)
 SELECT b.MaSuKien,N'KetNoi',(SELECT b.NguoiNhanId AS recipient,b.YeuCauId AS target FOR JSON PATH,WITHOUT_ARRAY_WRAPPER),@Now,'ChoXuLy'
 FROM @ThongBaoKetThuc b;
 INSERT dbo.ThongBao(Id,NguoiNhanId,LoaiThongBao,DoiTuongId,NoiDungTomTat,TaoLuc,MaSuKien)
 SELECT NEWID(),NguoiNhanId,'KetNoi',YeuCauId,N'Nhu cầu đã được kết nối. Đề nghị này đã kết thúc.',@Now,MaSuKien
 FROM @ThongBaoKetThuc;
 COMMIT;SELECT * FROM dbo.LopHoc WHERE Id=@LopId;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK;THROW;END CATCH;
END;
GO
DECLARE @Now datetimeoffset(0)=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 DECLARE @KetThuc TABLE(Id uniqueidentifier,GiaSuId uniqueidentifier,BenHocId uniqueidentifier);
 UPDATE r SET TrangThai='NhuCauDaKetNoi'
 OUTPUT inserted.Id,inserted.GiaSuId,inserted.BenHocId INTO @KetThuc
 FROM dbo.YeuCauKetNoiLopHoc r
 WHERE r.TrangThai IN('ChoGiaSu','ChoBenHoc') AND r.HetHanLuc>@Now AND EXISTS(SELECT 1 FROM dbo.NhuCauHoc n JOIN dbo.YeuCauKetNoiLopHoc a ON a.NhuCauId=n.Id JOIN dbo.LopHoc c ON c.YeuCauKetNoiId=a.Id WHERE n.Id=r.NhuCauId AND n.TrangThai='DaDong' AND c.TrangThai='DangHoatDong');
 DECLARE @ThongBaoKetThuc TABLE(MaSuKien uniqueidentifier,NguoiNhanId uniqueidentifier,YeuCauId uniqueidentifier);
 INSERT @ThongBaoKetThuc
 SELECT NEWID(),p.NguoiNhanId,k.Id FROM @KetThuc k
 CROSS APPLY(VALUES(k.GiaSuId),(k.BenHocId)) p(NguoiNhanId);
 INSERT dbo.SuKienChoXuLy(Id,LoaiSuKien,DuLieuToiThieu,TaoLuc,TrangThai)
 SELECT b.MaSuKien,N'KetNoi',(SELECT b.NguoiNhanId AS recipient,b.YeuCauId AS target FOR JSON PATH,WITHOUT_ARRAY_WRAPPER),@Now,'ChoXuLy'
 FROM @ThongBaoKetThuc b;
 INSERT dbo.ThongBao(Id,NguoiNhanId,LoaiThongBao,DoiTuongId,NoiDungTomTat,TaoLuc,MaSuKien)
 SELECT NEWID(),NguoiNhanId,'KetNoi',YeuCauId,N'Nhu cầu đã được kết nối. Đề nghị này đã kết thúc.',@Now,MaSuKien
 FROM @ThongBaoKetThuc;
COMMIT;
GO
