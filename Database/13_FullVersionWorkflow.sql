SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_ThongBao_Profile')
    ALTER TABLE dbo.ThongBao DROP CONSTRAINT CK_ThongBao_Profile;
IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE name='CK_ThongBao_FullVersion')
    ALTER TABLE dbo.ThongBao DROP CONSTRAINT CK_ThongBao_FullVersion;
ALTER TABLE dbo.ThongBao WITH CHECK ADD CONSTRAINT CK_ThongBao_FullVersion CHECK(LoaiThongBao IN('KetNoi','LichHoc','BaiTap','ThanhVien','NhacViec','QuanTri','HoSo','LoiMoiLop','LienKetTaiKhoan','NhuCau'));

IF OBJECT_ID('dbo.LuotDangNhuCau','U') IS NULL
BEGIN
    CREATE TABLE dbo.LuotDangNhuCau (
        Id uniqueidentifier NOT NULL PRIMARY KEY,
        NhuCauId uniqueidentifier NOT NULL REFERENCES dbo.NhuCauHoc(Id),
        TaiKhoanId uniqueidentifier NOT NULL REFERENCES dbo.AspNetUsers(Id),
        MaThaoTac uniqueidentifier NOT NULL,
        HanhDong varchar(16) NOT NULL CHECK(HanhDong IN('Dang','MoLai')),
        DangLuc datetimeoffset NOT NULL,
        NgayDang date NOT NULL,
        CONSTRAINT UQ_LuotDang_ThaoTac UNIQUE(TaiKhoanId,MaThaoTac)
    );
    CREATE INDEX IX_LuotDang_Ngay ON dbo.LuotDangNhuCau(TaiKhoanId,NgayDang);
END;
IF COL_LENGTH('dbo.NhuCauHoc','XacNhanConNhuCauLuc') IS NULL
    ALTER TABLE dbo.NhuCauHoc ADD XacNhanConNhuCauLuc datetimeoffset NULL, NhacXacNhanLuc datetimeoffset NULL, LyDoDong nvarchar(256) NULL;

IF OBJECT_ID('dbo.LienKetPhuHuynhHocSinh','U') IS NULL
BEGIN
    CREATE TABLE dbo.LienKetPhuHuynhHocSinh (
        Id uniqueidentifier NOT NULL PRIMARY KEY,
        PhuHuynhId uniqueidentifier NOT NULL REFERENCES dbo.PhuHuynh(TaiKhoanId),
        HocSinhId uniqueidentifier NOT NULL REFERENCES dbo.HocSinh(TaiKhoanId),
        NguoiGuiId uniqueidentifier NOT NULL REFERENCES dbo.AspNetUsers(Id),
        TrangThai varchar(32) NOT NULL,
        GuiLuc datetimeoffset NOT NULL,
        XacNhanLuc datetimeoffset NULL,
        KetThucLuc datetimeoffset NULL,
        NguoiKetThucId uniqueidentifier NULL REFERENCES dbo.AspNetUsers(Id),
        RowVersion rowversion NOT NULL,
        CONSTRAINT CK_LienKet_Cap CHECK (PhuHuynhId<>HocSinhId AND NguoiGuiId=PhuHuynhId),
        CONSTRAINT CK_LienKet_TrangThai CHECK (TrangThai IN('ChoXacNhan','DaLienKet','TuChoi','DaHuy')),
        CONSTRAINT CK_LienKet_ThoiGian CHECK ((XacNhanLuc IS NULL OR XacNhanLuc>=GuiLuc) AND (KetThucLuc IS NULL OR KetThucLuc>=GuiLuc)),
        CONSTRAINT CK_LienKet_HieuLuc CHECK (
            (TrangThai='ChoXacNhan' AND XacNhanLuc IS NULL AND KetThucLuc IS NULL AND NguoiKetThucId IS NULL)
            OR (TrangThai='DaLienKet' AND XacNhanLuc IS NOT NULL AND KetThucLuc IS NULL AND NguoiKetThucId IS NULL)
            OR (TrangThai IN('TuChoi','DaHuy') AND KetThucLuc IS NOT NULL AND NguoiKetThucId IN(PhuHuynhId,HocSinhId)))
    );
    CREATE UNIQUE INDEX UQ_LienKet_DangHoatDong ON dbo.LienKetPhuHuynhHocSinh(PhuHuynhId,HocSinhId) WHERE KetThucLuc IS NULL;
END;
IF COL_LENGTH('dbo.ThanhVienLopHoc','LienKetId') IS NULL
    ALTER TABLE dbo.ThanhVienLopHoc ADD LienKetId uniqueidentifier NULL REFERENCES dbo.LienKetPhuHuynhHocSinh(Id);
IF COL_LENGTH('dbo.ThanhVienLopHoc','ChoPhepNopBai') IS NULL
    ALTER TABLE dbo.ThanhVienLopHoc ADD ChoPhepNopBai bit NOT NULL CONSTRAINT DF_ThanhVien_ChoPhepNopBai DEFAULT(0);

IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID('dbo.TinNhan') AND name='GuiLuc' AND scale=0)
BEGIN
    DROP INDEX IX_LichSu_TinNhan ON dbo.TinNhan;
    ALTER TABLE dbo.TinNhan ALTER COLUMN GuiLuc datetimeoffset(7) NOT NULL;
    CREATE INDEX IX_LichSu_TinNhan ON dbo.TinNhan(CuocHoiThoaiId,GuiLuc,Id);
END;
IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID('dbo.ThanhVienLopHoc') AND name='DuyetLuc' AND scale=0)
BEGIN
    DROP INDEX UQ_ChuaRoi_ThanhVienLopHoc ON dbo.ThanhVienLopHoc;
    ALTER TABLE dbo.ThanhVienLopHoc DROP CONSTRAINT CK_ThanhVienLopHoc_3,CK_ThanhVienLopHoc_4,CK_ThanhVienLopHoc_5;
    ALTER TABLE dbo.ThanhVienLopHoc ALTER COLUMN DeNghiLuc datetimeoffset(7) NOT NULL;
    ALTER TABLE dbo.ThanhVienLopHoc ALTER COLUMN DongYThamGiaLuc datetimeoffset(7) NULL;
    ALTER TABLE dbo.ThanhVienLopHoc ALTER COLUMN DuyetLuc datetimeoffset(7) NULL;
    ALTER TABLE dbo.ThanhVienLopHoc ALTER COLUMN RoiLopLuc datetimeoffset(7) NULL;
    ALTER TABLE dbo.ThanhVienLopHoc WITH CHECK ADD
        CONSTRAINT CK_ThanhVienLopHoc_3 CHECK(TrangThaiDuyet<>'DaDuyet' OR (MucQuyen IS NOT NULL AND DuyetLuc IS NOT NULL)),
        CONSTRAINT CK_ThanhVienLopHoc_4 CHECK(DuyetLuc IS NULL OR DuyetLuc>=DeNghiLuc),
        CONSTRAINT CK_ThanhVienLopHoc_5 CHECK(RoiLopLuc IS NULL OR RoiLopLuc>=DeNghiLuc);
    CREATE UNIQUE INDEX UQ_ChuaRoi_ThanhVienLopHoc ON dbo.ThanhVienLopHoc(LopHocId,TaiKhoanId)
        WHERE RoiLopLuc IS NULL AND TrangThaiDuyet<>'TuChoi';
END;

COMMIT;
IF COL_LENGTH('dbo.LopHoc','HocSinhLienKetId') IS NULL
    ALTER TABLE dbo.LopHoc ADD HocSinhLienKetId uniqueidentifier NULL REFERENCES dbo.HocSinh(TaiKhoanId);

GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;
DROP TRIGGER IF EXISTS dbo.tr_BuoiHoc_SauGioKetThuc;
DROP TRIGGER IF EXISTS dbo.tr_BuoiHoc_LuuPhienBan;
DROP TRIGGER IF EXISTS dbo.tr_LichHoc_GiuLichSu;
DECLARE @sql nvarchar(max)=N'';
SELECT @sql=@sql+N'ALTER TABLE dbo.BuoiHoc DROP CONSTRAINT '+QUOTENAME(name)+N';'
FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('dbo.BuoiHoc')
    AND name IN('CK_BuoiHoc_1','CK_BuoiHoc_2','CK_BuoiHoc_3','CK_BuoiHoc_4','CK_BuoiHoc_6','CK_BuoiHoc_7','CK_BuoiHoc_8','CK_Buoi_Workflow','CK_Buoi_ThamDu');
EXEC sys.sp_executesql @sql;
ALTER TABLE dbo.BuoiHoc ALTER COLUMN NhanXet nvarchar(2000) NULL;
ALTER TABLE dbo.BuoiHoc ALTER COLUMN TrangThaiThamDu varchar(32) NULL;
ALTER TABLE dbo.BuoiHoc ALTER COLUMN TrangThaiThamDuGiaSu varchar(32) NULL;
ALTER TABLE dbo.BuoiHoc ALTER COLUMN TrangThaiThamDuBenHoc varchar(32) NULL;
ALTER TABLE dbo.BuoiHoc ALTER COLUMN GhiNhanLuc datetimeoffset(0) NULL;
ALTER TABLE dbo.BuoiHoc ALTER COLUMN NguoiGhiNhanId uniqueidentifier NULL;
ALTER TABLE dbo.BuoiHoc ALTER COLUMN KetQuaBuoiHoc varchar(32) NULL;
IF COL_LENGTH('dbo.BuoiHoc','TrangThai') IS NULL
 ALTER TABLE dbo.BuoiHoc ADD TrangThai varchar(32) NOT NULL DEFAULT('DuKien'), CanHocBu bit NOT NULL DEFAULT(0), LyDoNghi nvarchar(2000) NULL, NguoiThongBaoNghiId uniqueidentifier NULL REFERENCES dbo.AspNetUsers(Id), ThongBaoNghiLuc datetimeoffset NULL;
IF COL_LENGTH('dbo.BaiTapGiao','TrangThai') IS NULL
 ALTER TABLE dbo.BaiTapGiao ADD TrangThai varchar(16) NOT NULL DEFAULT('DaGiao');
IF OBJECT_ID('dbo.LichSuDieuChinhLich','U') IS NULL
 CREATE TABLE dbo.LichSuDieuChinhLich(Id uniqueidentifier NOT NULL PRIMARY KEY,LichHocId uniqueidentifier NOT NULL REFERENCES dbo.LichHoc(Id),BatDauCu datetimeoffset NOT NULL,KetThucCu datetimeoffset NOT NULL,BatDauMoi datetimeoffset NOT NULL,KetThucMoi datetimeoffset NOT NULL,LyDo nvarchar(2000) NOT NULL,NguoiSuaId uniqueidentifier NOT NULL REFERENCES dbo.AspNetUsers(Id),ThayDoiLuc datetimeoffset NOT NULL,CHECK(BatDauCu<KetThucCu AND BatDauMoi<KetThucMoi));
GO
UPDATE dbo.BuoiHoc SET TrangThai=CASE WHEN KetQuaBuoiHoc='DaDienRa' THEN 'DaHoc' ELSE 'DaNghi' END,
 LyDoNghi=CASE WHEN KetQuaBuoiHoc='KhongDienRa' THEN NhanXet ELSE NULL END,
 NguoiThongBaoNghiId=CASE WHEN KetQuaBuoiHoc='KhongDienRa' THEN NguoiGhiNhanId ELSE NULL END,
 ThongBaoNghiLuc=CASE WHEN KetQuaBuoiHoc='KhongDienRa' THEN GhiNhanLuc ELSE NULL END
WHERE TrangThai='DuKien' AND KetQuaBuoiHoc IS NOT NULL;
INSERT dbo.BuoiHoc(Id,LichHocId,TieuDe,NhanXet,TrangThaiThamDu,GhiNhanLuc,NguoiGhiNhanId,TrangThaiThamDuGiaSu,TrangThaiThamDuBenHoc,KetQuaBuoiHoc,TrangThai,CapNhatLuc)
SELECT NEWID(),l.Id,l.TieuDe,NULL,NULL,NULL,NULL,NULL,NULL,NULL,CASE WHEN l.TrangThai='DaHuy' THEN 'DaHuy' ELSE 'DuKien' END,SYSDATETIMEOFFSET()
FROM dbo.LichHoc l WHERE NOT EXISTS(SELECT 1 FROM dbo.BuoiHoc b WHERE b.LichHocId=l.Id);
ALTER TABLE dbo.BuoiHoc ADD CONSTRAINT CK_Buoi_Workflow CHECK(TrangThai IN('DuKien','DangDienRa','DaHoc','DaNghi','DaHuy') AND (TrangThai<>'DaHoc' OR (KetQuaBuoiHoc='DaDienRa' AND NguoiGhiNhanId IS NOT NULL AND GhiNhanLuc IS NOT NULL AND LEN(LTRIM(RTRIM(NoiDungGiangDay)))>0)) AND (TrangThai<>'DaNghi' OR (LEN(LTRIM(RTRIM(LyDoNghi)))>0 AND NguoiThongBaoNghiId IS NOT NULL AND ThongBaoNghiLuc IS NOT NULL)) AND (CanHocBu=0 OR TrangThai='DaNghi'));
ALTER TABLE dbo.BuoiHoc ADD CONSTRAINT CK_Buoi_ThamDu CHECK(
    (TrangThaiThamDu IS NULL OR TrangThaiThamDu IN('CoMat','VangCoBaoTruoc','VangKhongBaoTruoc'))
    AND (TrangThaiThamDuGiaSu IS NULL OR TrangThaiThamDuGiaSu IN('CoMat','VangCoBaoTruoc','VangKhongBaoTruoc'))
    AND (TrangThaiThamDuBenHoc IS NULL OR TrangThaiThamDuBenHoc IN('CoMat','VangCoBaoTruoc','VangKhongBaoTruoc'))
    AND (KetQuaBuoiHoc IS NULL OR KetQuaBuoiHoc IN('DaDienRa','KhongDienRa'))
    AND (KetQuaBuoiHoc<>'KhongDienRa' OR TrangThaiThamDuGiaSu<>'CoMat' OR TrangThaiThamDuBenHoc<>'CoMat')
    AND (GhiNhanLuc IS NULL OR (NguoiGhiNhanId IS NOT NULL AND KetQuaBuoiHoc IS NOT NULL AND TrangThaiThamDuGiaSu IS NOT NULL AND TrangThaiThamDuBenHoc IS NOT NULL AND LEN(LTRIM(RTRIM(ISNULL(NhanXet,N''))))>0)));
COMMIT;
GO
CREATE OR ALTER TRIGGER dbo.tr_LichHoc_TaoBuoi ON dbo.LichHoc AFTER INSERT AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.BuoiHoc(Id,LichHocId,TieuDe,NhanXet,TrangThaiThamDu,GhiNhanLuc,NguoiGhiNhanId,TrangThaiThamDuGiaSu,TrangThaiThamDuBenHoc,KetQuaBuoiHoc,TrangThai,CapNhatLuc)
 SELECT NEWID(),Id,TieuDe,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'DuKien',SYSDATETIMEOFFSET() FROM inserted;
END;
GO
CREATE OR ALTER TRIGGER dbo.tr_BuoiHoc_SauGioKetThuc ON dbo.BuoiHoc AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.LichHoc l ON l.Id=i.LichHocId JOIN dbo.LopHoc c ON c.Id=l.LopHocId WHERE i.TrangThai='DaHoc' AND (l.KetThucLuc>SYSDATETIMEOFFSET() OR i.NguoiGhiNhanId<>c.GiaSuId)) THROW 51118,N'Chỉ gia sư ghi hoàn tất sau giờ học.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.Id=i.Id WHERE i.LichHocId<>d.LichHocId OR i.PhienBan<>d.PhienBan+1) THROW 51119,N'Buổi học đã thay đổi hoặc sai phiên bản.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.tr_LichHoc_GiuLichSu ON dbo.LichHoc AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.Id=i.Id JOIN dbo.BuoiHoc b ON b.LichHocId=d.Id WHERE b.TrangThai IN('DangDienRa','DaHoc') AND (i.BatDauLuc<>d.BatDauLuc OR i.KetThucLuc<>d.KetThucLuc)) THROW 51120,N'Không đổi giờ buổi đã bắt đầu học.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.tr_BuoiHoc_LuuPhienBan ON dbo.BuoiHoc AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.LichSuGhiNhanBuoiHoc(BuoiHocId,PhienBan,TieuDe,KetQuaBuoiHoc,NoiDungGiangDay,NhanXet,TrangThaiThamDuGiaSu,TrangThaiThamDuBenHoc,NguoiGhiNhanId,GhiNhanLuc,NguoiSuaId,ThayDoiLuc)
 SELECT d.Id,d.PhienBan,d.TieuDe,d.KetQuaBuoiHoc,d.NoiDungGiangDay,d.NhanXet,d.TrangThaiThamDuGiaSu,d.TrangThaiThamDuBenHoc,d.NguoiGhiNhanId,d.GhiNhanLuc,COALESCE(i.NguoiGhiNhanId,i.NguoiThongBaoNghiId),i.CapNhatLuc FROM deleted d JOIN inserted i ON i.Id=d.Id WHERE d.KetQuaBuoiHoc IS NOT NULL AND d.GhiNhanLuc IS NOT NULL;
END;
GO
CREATE OR ALTER TRIGGER dbo.tr_BaiTap_Buoi ON dbo.BaiTapGiao AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i LEFT JOIN deleted d ON d.Id=i.Id WHERE i.BuoiHocId IS NULL AND (d.Id IS NULL OR d.BuoiHocId IS NOT NULL)) THROW 51164,N'Bài mới phải gắn buổi học.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.BuoiHoc b ON b.Id=i.BuoiHocId JOIN dbo.LichHoc l ON l.Id=b.LichHocId WHERE l.LopHocId<>i.LopHocId) THROW 51165,N'Buổi học không thuộc lớp.',1;
END;
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
 IF @NhanXet IS NULL OR LEN(LTRIM(RTRIM(@NhanXet)))=0 OR LEN(@NhanXet)>2000 OR LEN(ISNULL(@NoiDung,N''))>10000 THROW 51148,N'Cần nhận xét hoặc lý do không diễn ra.',1;
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
    AND m.TrangThaiDuyet='DaDuyet' AND m.RoiLopLuc IS NULL AND m.MucQuyen='ChinhSua' AND m.ChoPhepNopBai=1 AND EXISTS(SELECT 1 FROM dbo.LienKetPhuHuynhHocSinh lk WITH(HOLDLOCK) WHERE lk.Id=m.LienKetId AND lk.TrangThai='DaLienKet' AND lk.KetThucLuc IS NULL))))
   THROW 51160,N'Không có quyền nộp bài trong lớp.',1;
  IF NOT EXISTS(SELECT 1 FROM dbo.LopHoc WHERE Id=@LopId AND TrangThai='DangHoatDong') THROW 51162,N'Lớp không còn hoạt động.',1;
  IF NOT EXISTS(SELECT 1 FROM dbo.BaiTapGiao WHERE Id=@BaiTapGiaoId AND TrangThai='DaGiao') THROW 51164,N'Bài tập chưa được giao.',1;
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
IF DATABASE_PRINCIPAL_ID('KLTN_App') IS NOT NULL
BEGIN
 GRANT SELECT,INSERT,UPDATE ON dbo.LienKetPhuHuynhHocSinh TO KLTN_App;
 GRANT SELECT,INSERT ON dbo.LuotDangNhuCau TO KLTN_App;
 GRANT SELECT,INSERT ON dbo.LichSuDieuChinhLich TO KLTN_App;
END;
GO
