SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH(N'dbo.NhatKyQuanTri',N'ChiTiet') IS NULL
    ALTER TABLE dbo.NhatKyQuanTri ADD ChiTiet nvarchar(4000) NULL;
IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_ThongBao_FullVersion')
 ALTER TABLE dbo.ThongBao DROP CONSTRAINT CK_ThongBao_FullVersion;
ALTER TABLE dbo.ThongBao WITH CHECK ADD CONSTRAINT CK_ThongBao_FullVersion CHECK(LoaiThongBao IN('KetNoi','LichHoc','BaiTap','ThanhVien','NhacViec','QuanTri','HoSo','LoiMoiLop','LienKetTaiKhoan','NhuCau','PhanHoi','DanhGiaDoiTac','BaoCao','KiemDuyet','TaiKhoan'));
IF COL_LENGTH('dbo.MonHoc','RowVersion') IS NULL
 ALTER TABLE dbo.MonHoc ADD RowVersion rowversion NOT NULL;

IF OBJECT_ID('dbo.DotPhanHoi','U') IS NULL
CREATE TABLE dbo.DotPhanHoi (
 Id uniqueidentifier NOT NULL PRIMARY KEY,
 LopHocId uniqueidentifier NOT NULL REFERENCES dbo.LopHoc(Id),
 Ten nvarchar(100) NOT NULL,
 TuNgay datetimeoffset NOT NULL,
 DenNgay datetimeoffset NOT NULL,
 HanGui datetimeoffset NOT NULL,
 NguoiTaoId uniqueidentifier NOT NULL REFERENCES dbo.AspNetUsers(Id),
 TrangThai nvarchar(32) NOT NULL,
 RowVersion rowversion NOT NULL,
 CONSTRAINT CK_DotPhanHoi_ThoiGian CHECK(TuNgay<DenNgay AND HanGui>=DenNgay),
 CONSTRAINT CK_DotPhanHoi_TrangThai CHECK(TrangThai IN ('DangMo','DaDong')),
 CONSTRAINT UQ_DotPhanHoi_Khoang UNIQUE(LopHocId,TuNgay,DenNgay)
);
IF COL_LENGTH('dbo.PhieuDanhGia','DotPhanHoiId') IS NULL
 ALTER TABLE dbo.PhieuDanhGia ADD DotPhanHoiId uniqueidentifier NULL REFERENCES dbo.DotPhanHoi(Id);
GO
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.PhieuDanhGia') AND name='UX_Phieu_Dot_Nguoi')
 CREATE UNIQUE INDEX UX_Phieu_Dot_Nguoi ON dbo.PhieuDanhGia(DotPhanHoiId,NguoiGuiId) WHERE DotPhanHoiId IS NOT NULL;

IF OBJECT_ID('dbo.LichSuPhanHoi','U') IS NULL
CREATE TABLE dbo.LichSuPhanHoi (
 Id uniqueidentifier NOT NULL PRIMARY KEY,
 PhieuDanhGiaId uniqueidentifier NOT NULL REFERENCES dbo.PhieuDanhGia(Id),
 NoiDung nvarchar(max) NOT NULL, MucDoHaiLong int NULL,
 NguoiSuaId uniqueidentifier NOT NULL REFERENCES dbo.AspNetUsers(Id),
 ThayDoiLuc datetimeoffset NOT NULL,
 CONSTRAINT CK_LichSuPhanHoi_HaiLong CHECK(MucDoHaiLong BETWEEN 1 AND 5)
);
IF OBJECT_ID('dbo.QuyetDinhSauPhanHoi','U') IS NULL
CREATE TABLE dbo.QuyetDinhSauPhanHoi (
 Id uniqueidentifier NOT NULL PRIMARY KEY,
 DotPhanHoiId uniqueidentifier NOT NULL REFERENCES dbo.DotPhanHoi(Id),
 NguoiQuyetDinhId uniqueidentifier NOT NULL REFERENCES dbo.AspNetUsers(Id),
 QuyetDinh nvarchar(16) NOT NULL,
 GhiChu nvarchar(2000) NULL, TaoLuc datetimeoffset NOT NULL, RowVersion rowversion NOT NULL,
 CONSTRAINT UQ_QuyetDinh_Dot_Nguoi UNIQUE(DotPhanHoiId,NguoiQuyetDinhId),
 CONSTRAINT CK_QuyetDinh_GiaTri CHECK(QuyetDinh IN ('TiepTuc','DieuChinh','Dung'))
);
IF OBJECT_ID('dbo.DanhGiaDoiTac','U') IS NULL
CREATE TABLE dbo.DanhGiaDoiTac (
 Id uniqueidentifier NOT NULL PRIMARY KEY,
 LopHocId uniqueidentifier NOT NULL REFERENCES dbo.LopHoc(Id),
 NguoiGuiId uniqueidentifier NOT NULL REFERENCES dbo.AspNetUsers(Id),
 NguoiNhanId uniqueidentifier NOT NULL REFERENCES dbo.AspNetUsers(Id),
 SoSao int NOT NULL, NhanXet nvarchar(2000) NOT NULL, GuiLuc datetimeoffset NOT NULL,
 PhanHoi nvarchar(2000) NULL, PhanHoiLuc datetimeoffset NULL, RowVersion rowversion NOT NULL,
 CONSTRAINT UQ_DanhGia_Lop_Nguoi UNIQUE(LopHocId,NguoiGuiId,NguoiNhanId),
 CONSTRAINT CK_DanhGia_Sao CHECK(SoSao BETWEEN 1 AND 5),
 CONSTRAINT CK_DanhGia_DoiTac CHECK(NguoiGuiId<>NguoiNhanId AND LEN(LTRIM(RTRIM(NhanXet))) BETWEEN 10 AND 2000)
);
IF OBJECT_ID('dbo.LichSuDanhGiaDoiTac','U') IS NULL
CREATE TABLE dbo.LichSuDanhGiaDoiTac (
 Id uniqueidentifier NOT NULL PRIMARY KEY,
 DanhGiaId uniqueidentifier NOT NULL REFERENCES dbo.DanhGiaDoiTac(Id),
 SoSao int NOT NULL, NhanXet nvarchar(2000) NOT NULL, PhanHoi nvarchar(2000) NULL,
 NguoiSuaId uniqueidentifier NOT NULL REFERENCES dbo.AspNetUsers(Id), ThayDoiLuc datetimeoffset NOT NULL
);

IF COL_LENGTH('dbo.BaoCaoViPham','GuiLuc') IS NULL
 ALTER TABLE dbo.BaoCaoViPham ADD GuiLuc datetimeoffset NOT NULL CONSTRAINT DF_BaoCao_GuiLuc DEFAULT(SYSDATETIMEOFFSET());
IF COL_LENGTH('dbo.BaoCaoViPham','NguoiBiBaoCaoId') IS NULL
 ALTER TABLE dbo.BaoCaoViPham ADD NguoiBiBaoCaoId uniqueidentifier NULL REFERENCES dbo.AspNetUsers(Id);
IF COL_LENGTH('dbo.BaoCaoViPham','KetQua') IS NULL
 ALTER TABLE dbo.BaoCaoViPham ADD KetQua varchar(16) NULL;
IF COL_LENGTH('dbo.BaoCaoViPham','MaThaoTac') IS NULL
 ALTER TABLE dbo.BaoCaoViPham ADD MaThaoTac uniqueidentifier NOT NULL CONSTRAINT DF_BaoCao_MaThaoTac DEFAULT(NEWID());
IF COL_LENGTH('dbo.BaoCaoViPham','BanChup') IS NULL
 ALTER TABLE dbo.BaoCaoViPham ADD BanChup nvarchar(max) NOT NULL CONSTRAINT DF_BaoCao_BanChup DEFAULT(N'');
GO
IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_BaoCaoViPham_1')
 ALTER TABLE dbo.BaoCaoViPham DROP CONSTRAINT CK_BaoCaoViPham_1;
ALTER TABLE dbo.BaoCaoViPham ADD CONSTRAINT CK_BaoCaoViPham_1 CHECK(LoaiDoiTuong IN ('TaiKhoan','HoSoGiaSu','NhuCauHoc','LopHoc','TinNhan','DanhGiaDoiTac'));
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.BaoCaoViPham') AND name='UX_BaoCao_MaThaoTac')
 CREATE UNIQUE INDEX UX_BaoCao_MaThaoTac ON dbo.BaoCaoViPham(NguoiBaoCaoId,MaThaoTac);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.BaoCaoViPham') AND name='UX_BaoCao_DangXuLy')
 CREATE UNIQUE INDEX UX_BaoCao_DangXuLy ON dbo.BaoCaoViPham(NguoiBaoCaoId,LoaiDoiTuong,DoiTuongId) WHERE TrangThai<>'DaXuLy';
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_BaoCao_KetQua')
 ALTER TABLE dbo.BaoCaoViPham ADD CONSTRAINT CK_BaoCao_KetQua CHECK(KetQua IS NULL OR KetQua IN ('XuLy','BacBo'));

IF OBJECT_ID('dbo.LichSuXuLyBaoCao','U') IS NULL
CREATE TABLE dbo.LichSuXuLyBaoCao (
 Id uniqueidentifier NOT NULL PRIMARY KEY,
 BaoCaoId uniqueidentifier NOT NULL REFERENCES dbo.BaoCaoViPham(Id),
 NguoiThucHienId uniqueidentifier NOT NULL REFERENCES dbo.AspNetUsers(Id),
 HanhDong nvarchar(32) NOT NULL, NoiDung nvarchar(2000) NOT NULL, TaoLuc datetimeoffset NOT NULL
);
IF OBJECT_ID('dbo.BienPhapQuanTri','U') IS NULL
CREATE TABLE dbo.BienPhapQuanTri (
 Id uniqueidentifier NOT NULL PRIMARY KEY,
 NguonBienPhapId uniqueidentifier NULL REFERENCES dbo.BienPhapQuanTri(Id),
 LoaiDoiTuong nvarchar(32) NOT NULL, DoiTuongId uniqueidentifier NOT NULL,
 Loai nvarchar(32) NOT NULL, LyDo nvarchar(2000) NOT NULL,
 BaoCaoId uniqueidentifier NULL REFERENCES dbo.BaoCaoViPham(Id),
 NguoiApDungId uniqueidentifier NOT NULL REFERENCES dbo.AspNetUsers(Id), ApDungLuc datetimeoffset NOT NULL,
 NguoiGoId uniqueidentifier NULL REFERENCES dbo.AspNetUsers(Id), GoLuc datetimeoffset NULL,
 LyDoGo nvarchar(2000) NULL, RowVersion rowversion NOT NULL,
 CONSTRAINT CK_BienPhap_Loai CHECK(Loai IN ('KhoaTaiKhoan','AnNoiDung','TamDungLop')),
 CONSTRAINT CK_BienPhap_Go CHECK((GoLuc IS NULL AND NguoiGoId IS NULL AND LyDoGo IS NULL) OR (GoLuc IS NOT NULL AND NguoiGoId IS NOT NULL AND LEN(LTRIM(RTRIM(LyDoGo)))>0))
);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.BienPhapQuanTri') AND name='IX_BienPhap_HieuLuc')
 CREATE INDEX IX_BienPhap_HieuLuc ON dbo.BienPhapQuanTri(LoaiDoiTuong,DoiTuongId,GoLuc);

IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_LopHoc_5')
 ALTER TABLE dbo.LopHoc DROP CONSTRAINT CK_LopHoc_5;
ALTER TABLE dbo.LopHoc ADD CONSTRAINT CK_LopHoc_5 CHECK(TrangThai IN ('DangHoatDong','DaHuy','DaKetThuc','TamDung'));

INSERT dbo.AspNetUserClaims(UserId,ClaimType,ClaimValue)
SELECT ur.UserId,'EduConnect.AdminPermission',p.Quyen
FROM dbo.AspNetUserRoles ur JOIN dbo.AspNetRoles r ON r.Id=ur.RoleId
CROSS JOIN (VALUES('DuyetHoSo'),('XuLyBaoCao'),('QuanLyTaiKhoan'),('QuanLyQuyen'),('XemNhatKy'),('QuanLyDanhMuc')) p(Quyen)
WHERE r.Name='QuanTri' AND NOT EXISTS(SELECT 1 FROM dbo.AspNetUserClaims c WHERE c.UserId=ur.UserId AND c.ClaimType='EduConnect.AdminConfigured');
INSERT dbo.AspNetUserClaims(UserId,ClaimType,ClaimValue)
SELECT ur.UserId,'EduConnect.AdminConfigured','true'
FROM dbo.AspNetUserRoles ur JOIN dbo.AspNetRoles r ON r.Id=ur.RoleId
WHERE r.Name='QuanTri' AND NOT EXISTS(SELECT 1 FROM dbo.AspNetUserClaims c WHERE c.UserId=ur.UserId AND c.ClaimType='EduConnect.AdminConfigured');

IF DATABASE_PRINCIPAL_ID('KLTN_App') IS NOT NULL
BEGIN
 GRANT SELECT,INSERT,UPDATE ON dbo.DotPhanHoi TO KLTN_App;
 GRANT SELECT,INSERT,UPDATE ON dbo.PhieuDanhGia TO KLTN_App;
 GRANT SELECT,INSERT ON dbo.LichSuPhanHoi TO KLTN_App;
 GRANT SELECT,INSERT,UPDATE ON dbo.QuyetDinhSauPhanHoi TO KLTN_App;
 GRANT SELECT,INSERT,UPDATE ON dbo.DanhGiaDoiTac TO KLTN_App;
 GRANT SELECT,INSERT ON dbo.LichSuDanhGiaDoiTac TO KLTN_App;
 GRANT SELECT,INSERT,UPDATE ON dbo.BaoCaoViPham TO KLTN_App;
 GRANT SELECT,INSERT ON dbo.LichSuXuLyBaoCao TO KLTN_App;
 GRANT SELECT,INSERT,UPDATE ON dbo.BienPhapQuanTri TO KLTN_App;
 GRANT SELECT,INSERT ON dbo.NhatKyQuanTri TO KLTN_App;
 DENY UPDATE,DELETE ON dbo.NhatKyQuanTri TO KLTN_App;
 DENY UPDATE,DELETE ON dbo.LichSuPhanHoi TO KLTN_App;
 DENY UPDATE,DELETE ON dbo.LichSuDanhGiaDoiTac TO KLTN_App;
 DENY UPDATE,DELETE ON dbo.LichSuXuLyBaoCao TO KLTN_App;
END;
COMMIT;
GO
CREATE OR ALTER TRIGGER dbo.tr_KetNoi_TaiKhoanHoatDong ON dbo.YeuCauKetNoiLopHoc AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i WHERE i.TrangThai IN ('ChoGiaSu','ChoBenHoc','DaChapNhan')
   AND EXISTS(SELECT 1 FROM dbo.AspNetUsers u WHERE u.Id IN(i.GiaSuId,i.BenHocId) AND u.TrangThai<>'HoatDong'))
   THROW 51150,N'Tài khoản đối tác đã bị khóa. Yêu cầu không thể tiếp tục.',1;
END;
GO
