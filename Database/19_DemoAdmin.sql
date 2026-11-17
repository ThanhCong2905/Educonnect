/* Chỉ thêm 1 quản trị viên demo, không thêm hồ sơ hoặc nhu cầu học.
   Chạy toàn bộ file trên database EduConnect đã cài đặt đầy đủ.
   Email: admin@demo.educonnect.test
   Mật khẩu: EduConnect@2026 (ASP.NET Core Identity V3).
   Nếu email đã tồn tại: dừng, không ghi đè tài khoản hoặc mật khẩu.
*/
SET NOEXEC OFF;
GO
SET PARSEONLY OFF;
GO
USE [EduConnect];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @Id uniqueidentifier = NEWID();
DECLARE @Email nvarchar(256) = N'admin@demo.educonnect.test';
DECLARE @RoleId uniqueidentifier;

BEGIN TRY
    BEGIN TRANSACTION;

    IF EXISTS (SELECT 1 FROM dbo.AspNetUsers WITH (UPDLOCK, HOLDLOCK)
               WHERE NormalizedEmail = UPPER(@Email) OR NormalizedUserName = UPPER(@Email))
        THROW 51210, N'Email hoặc tên đăng nhập admin đã tồn tại. Không ghi đè tài khoản.', 1;

    SELECT @RoleId = Id FROM dbo.AspNetRoles WHERE NormalizedName = N'QUANTRI';
    IF @RoleId IS NULL
        THROW 51211, N'Thiếu vai trò QuanTri. Hãy cài đặt đầy đủ database trước.', 1;

    INSERT dbo.AspNetUsers
        (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
         PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed,
         TwoFactorEnabled, LockoutEnabled, AccessFailedCount, HoTen, TrangThai)
    VALUES
        (@Id, @Email, UPPER(@Email), @Email, UPPER(@Email), 1,
         N'AQAAAAIAAYagAAAAEIaC3zX7Z688oIYGprA2P/s2Dh/To7pUGKMruTpiponaVIQ3Mg4M/QQ5AslQFBgYjw==',
         CONVERT(nvarchar(36), NEWID()), CONVERT(nvarchar(36), NEWID()),
         0, 0, 1, 0, N'Quản trị viên Demo', N'HoatDong');

    INSERT dbo.AspNetUserRoles (UserId, RoleId) VALUES (@Id, @RoleId);

    INSERT dbo.AspNetUserClaims (UserId, ClaimType, ClaimValue)
    SELECT @Id, N'EduConnect.AdminPermission', p.Quyen
    FROM (VALUES (N'DuyetHoSo'), (N'XuLyBaoCao'), (N'QuanLyTaiKhoan'),
                 (N'QuanLyQuyen'), (N'XemNhatKy'), (N'QuanLyDanhMuc')) p(Quyen);

    INSERT dbo.AspNetUserClaims (UserId, ClaimType, ClaimValue)
    VALUES (@Id, N'EduConnect.AdminConfigured', N'true');

    COMMIT TRANSACTION;
    SELECT @Email AS Email, N'QuanTri' AS VaiTro, N'Đã tạo tài khoản với đủ 6 quyền quản trị.' AS KetQua;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
