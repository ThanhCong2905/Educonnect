-- Restore authentication procedures on an existing Identity database.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO
CREATE OR ALTER PROCEDURE dbo.usp_GhiNhanDangNhapSai
 @TaiKhoanChuanHoa nvarchar(256),@DiaChiIp nvarchar(256)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
  BEGIN TRANSACTION;
  DECLARE @Now datetimeoffset(0)=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
  -- Giữ khóa hai dải PK theo thứ tự cố định để tránh mất lượt sai và giảm deadlock.
  DECLARE @A int,@B int;
  SELECT @A=SoLanSai FROM dbo.TrangThaiThuDangNhap WITH(UPDLOCK,HOLDLOCK) WHERE LoaiKhoa='TaiKhoan' AND GiaTriKhoa=@TaiKhoanChuanHoa;
  SELECT @B=SoLanSai FROM dbo.TrangThaiThuDangNhap WITH(UPDLOCK,HOLDLOCK) WHERE LoaiKhoa='DiaChiIp' AND GiaTriKhoa=@DiaChiIp;
  IF EXISTS(SELECT 1 FROM dbo.TrangThaiThuDangNhap WHERE ((LoaiKhoa='TaiKhoan' AND GiaTriKhoa=@TaiKhoanChuanHoa) OR (LoaiKhoa='DiaChiIp' AND GiaTriKhoa=@DiaChiIp)) AND LanSaiCuoiLuc>DATEADD(minute,-15,@Now) AND ChoDenLuc>@Now)
  BEGIN
   COMMIT;
   SELECT CAST(0 AS bit) AS DuocGhiNhan,MAX(ChoDenLuc) AS ChoDenLuc FROM dbo.TrangThaiThuDangNhap WHERE (LoaiKhoa='TaiKhoan' AND GiaTriKhoa=@TaiKhoanChuanHoa) OR (LoaiKhoa='DiaChiIp' AND GiaTriKhoa=@DiaChiIp);
   RETURN;
  END;
  DELETE dbo.TrangThaiThuDangNhap WHERE ((LoaiKhoa='TaiKhoan' AND GiaTriKhoa=@TaiKhoanChuanHoa) OR (LoaiKhoa='DiaChiIp' AND GiaTriKhoa=@DiaChiIp)) AND LanSaiCuoiLuc<=DATEADD(minute,-15,@Now);
  UPDATE dbo.TrangThaiThuDangNhap SET SoLanSai=SoLanSai+1,LanSaiCuoiLuc=@Now,
   ChoDenLuc=CASE WHEN SoLanSai+1<5 THEN NULL ELSE DATEADD(second,CASE SoLanSai+1 WHEN 5 THEN 30 WHEN 6 THEN 60 WHEN 7 THEN 120 ELSE 300 END,@Now) END
  WHERE (LoaiKhoa='TaiKhoan' AND GiaTriKhoa=@TaiKhoanChuanHoa) OR (LoaiKhoa='DiaChiIp' AND GiaTriKhoa=@DiaChiIp);
  IF NOT EXISTS(SELECT 1 FROM dbo.TrangThaiThuDangNhap WHERE LoaiKhoa='TaiKhoan' AND GiaTriKhoa=@TaiKhoanChuanHoa)
   INSERT dbo.TrangThaiThuDangNhap(LoaiKhoa,GiaTriKhoa,SoLanSai,LanSaiCuoiLuc) VALUES('TaiKhoan',@TaiKhoanChuanHoa,1,@Now);
  IF NOT EXISTS(SELECT 1 FROM dbo.TrangThaiThuDangNhap WHERE LoaiKhoa='DiaChiIp' AND GiaTriKhoa=@DiaChiIp)
   INSERT dbo.TrangThaiThuDangNhap(LoaiKhoa,GiaTriKhoa,SoLanSai,LanSaiCuoiLuc) VALUES('DiaChiIp',@DiaChiIp,1,@Now);
  COMMIT;
  SELECT CAST(1 AS bit) AS DuocGhiNhan,MAX(ChoDenLuc) AS ChoDenLuc FROM dbo.TrangThaiThuDangNhap WHERE (LoaiKhoa='TaiKhoan' AND GiaTriKhoa=@TaiKhoanChuanHoa) OR (LoaiKhoa='DiaChiIp' AND GiaTriKhoa=@DiaChiIp);
 END TRY
 BEGIN CATCH
  IF XACT_STATE()<>0 ROLLBACK; THROW;
 END CATCH;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_DangNhapThanhCong
 @TaiKhoanId uniqueidentifier,@TaiKhoanChuanHoa nvarchar(256),@DiaChiIp nvarchar(256)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 -- Chỉ gọi từ máy chủ SAU khi UserManager/SignInManager xác minh mật khẩu.
 -- Không cấp trực tiếp cho trình duyệt hoặc người dùng cuối.
 BEGIN TRY
  BEGIN TRANSACTION;
  DECLARE @Now datetimeoffset(0)=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00'),@A int,@B int;
  SELECT @A=SoLanSai FROM dbo.TrangThaiThuDangNhap WITH(UPDLOCK,HOLDLOCK) WHERE LoaiKhoa='TaiKhoan' AND GiaTriKhoa=@TaiKhoanChuanHoa;
  SELECT @B=SoLanSai FROM dbo.TrangThaiThuDangNhap WITH(UPDLOCK,HOLDLOCK) WHERE LoaiKhoa='DiaChiIp' AND GiaTriKhoa=@DiaChiIp;
  IF EXISTS(SELECT 1 FROM dbo.TrangThaiThuDangNhap WHERE ((LoaiKhoa='TaiKhoan' AND GiaTriKhoa=@TaiKhoanChuanHoa) OR (LoaiKhoa='DiaChiIp' AND GiaTriKhoa=@DiaChiIp)) AND LanSaiCuoiLuc>DATEADD(minute,-15,@Now) AND ChoDenLuc>@Now)
   THROW 51040,N'Chưa hết thời gian chờ đăng nhập.',1;
  IF NOT EXISTS(SELECT 1 FROM dbo.AspNetUsers WITH(UPDLOCK,HOLDLOCK) WHERE Id=@TaiKhoanId AND NormalizedEmail=@TaiKhoanChuanHoa AND TrangThai='HoatDong')
   THROW 51041,N'Tài khoản không hợp lệ hoặc bị khóa.',1;
  DELETE dbo.TrangThaiThuDangNhap WHERE LoaiKhoa='TaiKhoan' AND GiaTriKhoa=@TaiKhoanChuanHoa;
  DECLARE @Id uniqueidentifier=NEWID();
  INSERT dbo.PhienDangNhap(Id,TaiKhoanId,TaoLuc,HoatDongCuoiLuc) VALUES(@Id,@TaiKhoanId,@Now,@Now);
  COMMIT; SELECT @Id AS PhienDangNhapId;
 END TRY
 BEGIN CATCH
  IF XACT_STATE()<>0 ROLLBACK; THROW;
 END CATCH;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_KiemTraPhien
 @PhienId uniqueidentifier,@TaiKhoanId uniqueidentifier,@LaHoatDongChuDong bit=0
AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @Now datetimeoffset(0)=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 IF @LaHoatDongChuDong=1
  UPDATE p SET HoatDongCuoiLuc=@Now FROM dbo.PhienDangNhap p JOIN dbo.AspNetUsers u ON u.Id=p.TaiKhoanId
  WHERE p.Id=@PhienId AND p.TaiKhoanId=@TaiKhoanId AND p.ThuHoiLuc IS NULL AND p.HoatDongCuoiLuc>DATEADD(minute,-30,@Now) AND u.TrangThai='HoatDong';
 SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM dbo.PhienDangNhap p JOIN dbo.AspNetUsers u ON u.Id=p.TaiKhoanId WHERE p.Id=@PhienId AND p.TaiKhoanId=@TaiKhoanId AND p.ThuHoiLuc IS NULL AND p.HoatDongCuoiLuc>DATEADD(minute,-30,@Now) AND u.TrangThai='HoatDong') THEN 1 ELSE 0 END AS bit) AS HopLe;
END;
GO
IF DATABASE_PRINCIPAL_ID(N'KLTN_App') IS NOT NULL
BEGIN
    GRANT EXECUTE ON OBJECT::dbo.[usp_GhiNhanDangNhapSai] TO [KLTN_App];
    GRANT EXECUTE ON OBJECT::dbo.[usp_DangNhapThanhCong] TO [KLTN_App];
    GRANT EXECUTE ON OBJECT::dbo.[usp_KiemTraPhien] TO [KLTN_App];
END;
GO