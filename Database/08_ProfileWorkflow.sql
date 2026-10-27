/* Nâng cấp hồ sơ. Chạy 08_ProfilePreflight.sql, sao lưu DB và uploads trước.
   Không xóa nhu cầu cũ, không sửa yêu cầu/lớp. Phí mâu thuẫn để NULL.
   Script dùng database đang kết nối; không có USE hoặc CREATE DATABASE. */
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET XACT_ABORT ON;
IF COL_LENGTH('dbo.HoSoGiaSu','DangHienThi') IS NULL
BEGIN
 ALTER TABLE dbo.HoSoGiaSu ADD DangHienThi bit NOT NULL CONSTRAINT DF_HoSo_HienThi DEFAULT(0),
 DayTrucTuyen bit NOT NULL CONSTRAINT DF_HoSo_Online DEFAULT(0),DayTrucTiep bit NOT NULL CONSTRAINT DF_HoSo_Offline DEFAULT(0),
 GuiDuyetLuc datetimeoffset(0) NULL,DuyetLuc datetimeoffset(0) NULL,NguoiDuyetId uniqueidentifier NULL;
END;
IF COL_LENGTH('dbo.HoSoGiaSuMonDay','HocPhiDeXuat') IS NULL
 ALTER TABLE dbo.HoSoGiaSuMonDay ADD HocPhiDeXuat decimal(18,2) NULL,DonViHocPhi varchar(16) NULL;
GO
SET XACT_ABORT ON;
BEGIN TRAN;
IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_ThongBao_1') ALTER TABLE dbo.ThongBao DROP CONSTRAINT CK_ThongBao_1;
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_ThongBao_Profile') ALTER TABLE dbo.ThongBao WITH CHECK ADD CONSTRAINT CK_ThongBao_Profile CHECK(LoaiThongBao IN('KetNoi','LichHoc','BaiTap','ThanhVien','NhacViec','QuanTri','HoSo'));
IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_HoSoGiaSu_1') ALTER TABLE dbo.HoSoGiaSu DROP CONSTRAINT CK_HoSoGiaSu_1;
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_HoSo_Workflow')
 ALTER TABLE dbo.HoSoGiaSu WITH CHECK ADD CONSTRAINT CK_HoSo_Workflow CHECK(TrangThaiDuyet IN ('BanNhap','ChoDuyet','DaDuyet','CanChinhSua','TuChoi'));
IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_NguoiThamGia_1') ALTER TABLE dbo.NguoiThamGia DROP CONSTRAINT CK_NguoiThamGia_1;
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_NguoiThamGia_Role')
 ALTER TABLE dbo.NguoiThamGia WITH CHECK ADD CONSTRAINT CK_NguoiThamGia_Role CHECK(LoaiNguoi IN('GiaSu','HocSinh','PhuHuynh','QuanTri'));
IF NOT EXISTS(SELECT 1 FROM sys.extended_properties WHERE major_id=OBJECT_ID('dbo.HoSoGiaSu') AND name='ProfileWorkflowV1')
BEGIN
 ;WITH Prices AS(
 SELECT h.Id,d.MonHocId,MIN(n.HocPhiDuKien) Fee,MIN(n.DonViHocPhi) Unit,
 COUNT(DISTINCT CONCAT(COALESCE(CONVERT(varchar(40),n.HocPhiDuKien),'NULL'),'|',COALESCE(n.DonViHocPhi,'NULL'))) Variants
 FROM dbo.HoSoGiaSu h JOIN dbo.HoSoGiaSuMonDay d ON d.HoSoGiaSuId=h.Id
 JOIN dbo.NhuCauHoc n ON n.ChuSoHuuId=h.GiaSuId AND n.MonHocId=d.MonHocId AND n.LoaiNhuCau='NhanLop'
 GROUP BY h.Id,d.MonHocId)
 UPDATE d SET HocPhiDeXuat=p.Fee,DonViHocPhi=p.Unit FROM dbo.HoSoGiaSuMonDay d JOIN Prices p ON p.Id=d.HoSoGiaSuId AND p.MonHocId=d.MonHocId
 WHERE p.Variants=1 AND p.Fee IS NOT NULL AND p.Unit IN('VND/Gio','VND/Buoi') AND d.HocPhiDeXuat IS NULL;
 UPDATE h SET DayTrucTuyen=CASE WHEN EXISTS(SELECT 1 FROM dbo.NhuCauHoc n WHERE n.ChuSoHuuId=h.GiaSuId AND n.LoaiNhuCau='NhanLop' AND n.HinhThucHoc='TrucTuyen') THEN 1 ELSE 0 END,
 DayTrucTiep=CASE WHEN EXISTS(SELECT 1 FROM dbo.NhuCauHoc n WHERE n.ChuSoHuuId=h.GiaSuId AND n.LoaiNhuCau='NhanLop' AND n.HinhThucHoc='TrucTiep') THEN 1 ELSE 0 END,
 DangHienThi=0,TrangThaiDuyet='BanNhap' FROM dbo.HoSoGiaSu h;
 EXEC sys.sp_addextendedproperty @name='ProfileWorkflowV1',@value=N'2026-10-27; legacy preserved; conflicting fees NULL',@level0type='SCHEMA',@level0name='dbo',@level1type='TABLE',@level1name='HoSoGiaSu';
END;
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_MonDay_Fee')
 ALTER TABLE dbo.HoSoGiaSuMonDay WITH CHECK ADD CONSTRAINT CK_MonDay_Fee CHECK((HocPhiDeXuat IS NULL AND DonViHocPhi IS NULL) OR (HocPhiDeXuat IS NOT NULL AND HocPhiDeXuat>=0 AND DonViHocPhi IS NOT NULL AND DonViHocPhi IN('VND/Gio','VND/Buoi')));
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_HoSo_Visibility')
 ALTER TABLE dbo.HoSoGiaSu WITH CHECK ADD CONSTRAINT CK_HoSo_Visibility CHECK(DangHienThi=0 OR (TrangThaiDuyet='DaDuyet' AND (DayTrucTuyen=1 OR DayTrucTiep=1)));
IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name='FK_HoSo_Reviewer') ALTER TABLE dbo.HoSoGiaSu ADD CONSTRAINT FK_HoSo_Reviewer FOREIGN KEY(NguoiDuyetId) REFERENCES dbo.NguoiThamGia(Id);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='IX_HoSo_Public' AND object_id=OBJECT_ID('dbo.HoSoGiaSu')) CREATE INDEX IX_HoSo_Public ON dbo.HoSoGiaSu(TrangThaiDuyet,DangHienThi) INCLUDE(GiaSuId,DayTrucTuyen,DayTrucTiep);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='IX_MonDay_Search' AND object_id=OBJECT_ID('dbo.HoSoGiaSuMonDay')) CREATE INDEX IX_MonDay_Search ON dbo.HoSoGiaSuMonDay(MonHocId,DonViHocPhi,HocPhiDeXuat) INCLUDE(HoSoGiaSuId,PhamViDay);
IF NOT EXISTS(SELECT 1 FROM dbo.NguoiThamGia WHERE LoaiNguoi='QuanTri') INSERT dbo.NguoiThamGia(Id,HoTen,LoaiNguoi) VALUES(NEWID(),N'Quản trị mẫu','QuanTri');
COMMIT;
GO
/* Bỏ ràng buộc cũ buộc giữ môn chỉ vì có nhu cầu nhận dạy cũ. Lịch sử vẫn giữ nguyên. */
DROP TRIGGER IF EXISTS dbo.tr_MonDay_GiuNhuCauMo;
GO
CREATE OR ALTER TRIGGER dbo.tr_LienHe_HoSoCongKhai ON dbo.YeuCauLienHe AFTER INSERT AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i WHERE NOT EXISTS(SELECT 1 FROM dbo.HoSoGiaSu h WITH(UPDLOCK,HOLDLOCK) WHERE h.GiaSuId=i.GiaSuId AND h.DangHienThi=1 AND h.TrangThaiDuyet='DaDuyet'))
 THROW 51201,N'Hồ sơ gia sư đang ẩn hoặc chưa được duyệt. Không thể gửi liên hệ mới.',1;
END;
GO
CREATE OR ALTER VIEW dbo.vw_GiaSuCongKhai AS
SELECT h.Id,h.GiaSuId,n.HoTen,h.TrinhDo,h.ChuyenMon,h.KinhNghiem,h.MoTa,h.KhuVuc,h.RowVersion
FROM dbo.HoSoGiaSu h JOIN dbo.GiaSu g ON g.TaiKhoanId=h.GiaSuId JOIN dbo.NguoiThamGia n ON n.Id=g.TaiKhoanId
WHERE h.TrangThaiDuyet='DaDuyet' AND h.DangHienThi=1 AND n.LoaiNguoi='GiaSu'
AND(h.DayTrucTuyen=1 OR h.DayTrucTiep=1)
AND EXISTS(SELECT 1 FROM dbo.HoSoGiaSuMonDay d WHERE d.HoSoGiaSuId=h.Id)
AND NOT EXISTS(SELECT 1 FROM dbo.HoSoGiaSuMonDay d JOIN dbo.MonHoc m ON m.Id=d.MonHocId WHERE d.HoSoGiaSuId=h.Id AND(d.HocPhiDeXuat IS NULL OR d.DonViHocPhi IS NULL OR m.DangSuDung=0));
GO
SELECT h.GiaSuId,d.MonHocId,m.TenMon,d.HocPhiDeXuat,d.DonViHocPhi FROM dbo.HoSoGiaSu h JOIN dbo.HoSoGiaSuMonDay d ON d.HoSoGiaSuId=h.Id JOIN dbo.MonHoc m ON m.Id=d.MonHocId WHERE d.HocPhiDeXuat IS NULL;
GO
CREATE OR ALTER VIEW dbo.vw_NhuCauCongKhai AS
SELECT Id,ChuSoHuuId,LoaiNhuCau,MonHocId,KhoiLop,HocLuc,MucTieu,SoBuoiTuan,HinhThucHoc,KhuVuc,HocPhiDuKien,DonViHocPhi,RowVersion
FROM dbo.NhuCauHoc WHERE TrangThai='DangMo' AND LoaiNhuCau='TimGiaSu';
GO
/* Giữ chữ ký để phát hiện caller cũ; lịch rảnh gia sư không còn là bộ lọc. MVC dùng LINQ. */
CREATE OR ALTER PROCEDURE dbo.usp_TimKiemGiaSu
 @TuKhoa nvarchar(256)=NULL,@MonHocId int=NULL,@KhuVuc nvarchar(256)=NULL,@HocPhiToiDa decimal(18,2)=NULL,@DonViHocPhi varchar(16)=NULL,
 @KhoiLop nvarchar(256)=NULL,@HinhThucHoc varchar(32)=NULL,@ThuTrongTuan tinyint=NULL,@GioBatDau time(0)=NULL,@GioKetThuc time(0)=NULL,
 @SapXep varchar(16)='MoiNhat',@Trang int=1,@KichThuocTrang int=20
AS
BEGIN
 SET NOCOUNT ON;
 IF @Trang<1 OR @KichThuocTrang NOT BETWEEN 1 AND 100 THROW 51100,N'Phân trang không hợp lệ.',1;
 IF @SapXep NOT IN('MoiNhat','Ten','HocPhiTang','HocPhiGiam') THROW 51101,N'Sắp xếp không hợp lệ.',1;
 IF @ThuTrongTuan IS NOT NULL OR @GioBatDau IS NOT NULL OR @GioKetThuc IS NOT NULL THROW 51202,N'Hồ sơ gia sư không dùng bộ lọc lịch rảnh.',1;
 IF (@HocPhiToiDa IS NOT NULL OR @SapXep IN('HocPhiTang','HocPhiGiam')) AND ISNULL(@DonViHocPhi,'') NOT IN('VND/Gio','VND/Buoi') THROW 51102,N'Chọn đúng đơn vị học phí.',1;
 ;WITH Results AS(
 SELECT h.Id,h.GiaSuId,u.HoTen,h.TrinhDo,h.ChuyenMon,h.KinhNghiem,h.MoTa,h.KhuVuc,h.DayTrucTuyen,h.DayTrucTiep,h.DuyetLuc,p.Fee
 FROM dbo.HoSoGiaSu h JOIN dbo.NguoiThamGia u ON u.Id=h.GiaSuId AND u.LoaiNguoi='GiaSu'
 CROSS APPLY(SELECT MIN(d.HocPhiDeXuat) Fee,COUNT(*) Matches FROM dbo.HoSoGiaSuMonDay d JOIN dbo.MonHoc m ON m.Id=d.MonHocId AND m.DangSuDung=1
 WHERE d.HoSoGiaSuId=h.Id AND(@MonHocId IS NULL OR d.MonHocId=@MonHocId) AND(@KhoiLop IS NULL OR d.PhamViDay COLLATE Vietnamese_100_CI_AS LIKE N'%'+@KhoiLop+N'%')
 AND(@HocPhiToiDa IS NULL OR(d.HocPhiDeXuat<=@HocPhiToiDa AND d.DonViHocPhi=@DonViHocPhi))
 AND(@SapXep NOT IN('HocPhiTang','HocPhiGiam') OR d.DonViHocPhi=@DonViHocPhi))p
 WHERE h.DangHienThi=1 AND h.TrangThaiDuyet='DaDuyet' AND(h.DayTrucTuyen=1 OR h.DayTrucTiep=1) AND p.Matches>0
 AND NOT EXISTS(SELECT 1 FROM dbo.HoSoGiaSuMonDay bad LEFT JOIN dbo.MonHoc m ON m.Id=bad.MonHocId WHERE bad.HoSoGiaSuId=h.Id AND(bad.HocPhiDeXuat IS NULL OR bad.DonViHocPhi IS NULL OR m.DangSuDung<>1))
 AND EXISTS(SELECT 1 FROM dbo.GiaSu g WHERE g.TaiKhoanId=h.GiaSuId)
 AND(@KhuVuc IS NULL OR h.KhuVuc COLLATE Vietnamese_100_CI_AS LIKE N'%'+LTRIM(RTRIM(@KhuVuc))+N'%')
 AND(@HinhThucHoc IS NULL OR(@HinhThucHoc='TrucTuyen' AND h.DayTrucTuyen=1) OR(@HinhThucHoc='TrucTiep' AND h.DayTrucTiep=1))
 AND(@TuKhoa IS NULL OR u.HoTen COLLATE Vietnamese_100_CI_AS LIKE N'%'+@TuKhoa+N'%' OR h.ChuyenMon COLLATE Vietnamese_100_CI_AS LIKE N'%'+@TuKhoa+N'%'
 OR EXISTS(SELECT 1 FROM dbo.HoSoGiaSuMonDay d JOIN dbo.MonHoc m ON m.Id=d.MonHocId WHERE d.HoSoGiaSuId=h.Id AND m.TenMon COLLATE Vietnamese_100_CI_AS LIKE N'%'+@TuKhoa+N'%')))
 SELECT *,COUNT_BIG(*) OVER() TongKetQua FROM Results ORDER BY CASE WHEN @SapXep='Ten' THEN HoTen END,CASE WHEN @SapXep='HocPhiTang' THEN Fee END,CASE WHEN @SapXep='HocPhiGiam' THEN Fee END DESC,DuyetLuc DESC,Id
 OFFSET (CONVERT(bigint,@Trang)-1)*@KichThuocTrang ROWS FETCH NEXT @KichThuocTrang ROWS ONLY;
END;
GO

