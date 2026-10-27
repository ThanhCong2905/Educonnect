/* EduConnect - cài mới database KLTN_4NghiepVu_V2.
Chạy toàn file với UTF-8. Dừng nếu database đã tồn tại.
Dữ liệu mẫu dùng để thử nghiệp vụ, chưa có xác thực đăng nhập. */
USE [master];
GO
IF DB_ID(N'KLTN_4NghiepVu_V2') IS NOT NULL
BEGIN
    RAISERROR(N'Database KLTN_4NghiepVu_V2 đã tồn tại. Hãy dùng database mới hoặc đổi tên trong file; không ghi đè.',16,1);
    SET NOEXEC ON;
END;
GO
CREATE DATABASE [KLTN_4NghiepVu_V2];
GO
USE [KLTN_4NghiepVu_V2];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;
SET XACT_ABORT ON;
GO
BEGIN TRY
BEGIN TRANSACTION;
CREATE TABLE dbo.[MonHoc] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TenMon] nvarchar(256) NOT NULL,
    [DangSuDung] bit NOT NULL CONSTRAINT [DF_MonHoc_DangSuDung] DEFAULT (1),
    CONSTRAINT [PK_MonHoc] PRIMARY KEY ([Id])
);
CREATE TABLE dbo.[HoSoGiaSu] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_HoSoGiaSu_Id] DEFAULT (NEWSEQUENTIALID()),
    [GiaSuId] uniqueidentifier NOT NULL,
    [TrinhDo] nvarchar(256) NOT NULL,
    [ChuyenMon] nvarchar(256) NOT NULL,
    [KinhNghiem] nvarchar(256) NOT NULL,
    [MoTa] nvarchar(max) NOT NULL,
    [KhuVuc] nvarchar(256) NOT NULL,
    [TrangThaiDuyet] varchar(32) NOT NULL,
    [LyDoTuChoi] nvarchar(2000) NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_HoSoGiaSu] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_HoSoGiaSu_1] CHECK ([TrangThaiDuyet] IN (N'ChoDuyet', N'DaDuyet', N'TuChoi'))
);
CREATE TABLE dbo.[HoSoGiaSuMonDay] (
    [HoSoGiaSuId] uniqueidentifier NOT NULL,
    [MonHocId] int NOT NULL,
    [PhamViDay] nvarchar(256) NOT NULL,
    CONSTRAINT [PK_HoSoGiaSuMonDay] PRIMARY KEY ([HoSoGiaSuId], [MonHocId])
);
CREATE TABLE dbo.[NhuCauHoc] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_NhuCauHoc_Id] DEFAULT (NEWSEQUENTIALID()),
    [ChuSoHuuId] uniqueidentifier NOT NULL,
    [LoaiNhuCau] varchar(32) NOT NULL,
    [MonHocId] int NOT NULL,
    [KhoiLop] nvarchar(256) NULL,
    [HocLuc] nvarchar(256) NULL,
    [MucTieu] nvarchar(256) NULL,
    [SoBuoiTuan] int NULL,
    [HinhThucHoc] varchar(32) NOT NULL,
    [KhuVuc] nvarchar(256) NULL,
    [DiaChiChiTiet] nvarchar(256) NULL,
    [HocPhiDuKien] decimal(18,2) NULL,
    [TenNguoiHoc] nvarchar(256) NULL,
    [TuoiNguoiHoc] int NULL,
    [TrangThai] varchar(32) NOT NULL,
    [RowVersion] rowversion NOT NULL,
    [DonViHocPhi] varchar(16) NULL,
    [TaoLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_NhuCauHoc_TaoLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    CONSTRAINT [PK_NhuCauHoc] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_NhuCauHoc_1] CHECK ([LoaiNhuCau] IN (N'TimGiaSu', N'NhanLop')),
    CONSTRAINT [CK_NhuCauHoc_2] CHECK ([SoBuoiTuan] > 0),
    CONSTRAINT [CK_NhuCauHoc_3] CHECK ([HinhThucHoc] IN (N'TrucTuyen', N'TrucTiep')),
    CONSTRAINT [CK_NhuCauHoc_4] CHECK ([HocPhiDuKien] >= 0),
    CONSTRAINT [CK_NhuCauHoc_5] CHECK ([TuoiNguoiHoc] > 0),
    CONSTRAINT [CK_NhuCauHoc_6] CHECK ([TrangThai] IN (N'BanNhap', N'DangMo', N'DaDong')),
    CONSTRAINT [CK_NhuCauHoc_7] CHECK ([DonViHocPhi] IN ('VND/Gio','VND/Buoi')),
    CONSTRAINT [CK_NhuCauHoc_8] CHECK ((([HocPhiDuKien] IS NULL AND [DonViHocPhi] IS NULL) OR ([HocPhiDuKien] IS NOT NULL AND [DonViHocPhi] IS NOT NULL))),
    CONSTRAINT [CK_NhuCauHoc_9] CHECK ([SoBuoiTuan] BETWEEN 1 AND 7),
    CONSTRAINT [CK_NhuCauHoc_10] CHECK ([TuoiNguoiHoc] BETWEEN 1 AND 120)
);
CREATE TABLE dbo.[KhungGioRanh] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_KhungGioRanh_Id] DEFAULT (NEWSEQUENTIALID()),
    [NhuCauId] uniqueidentifier NOT NULL,
    [ThuTrongTuan] tinyint NOT NULL,
    [GioBatDau] time(0) NOT NULL,
    [GioKetThuc] time(0) NOT NULL,
    CONSTRAINT [PK_KhungGioRanh] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_KhungGioRanh_1] CHECK ([ThuTrongTuan] BETWEEN 0 AND 6),
    CONSTRAINT [CK_KhungGioRanh_2] CHECK ([GioBatDau] < [GioKetThuc])
);
CREATE TABLE dbo.[YeuCauKetNoiLopHoc] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_YeuCauKetNoiLopHoc_Id] DEFAULT (NEWSEQUENTIALID()),
    [GiaSuId] uniqueidentifier NOT NULL,
    [BenHocId] uniqueidentifier NOT NULL,
    [NguoiGuiId] uniqueidentifier NOT NULL,
    [NhuCauId] uniqueidentifier NULL,
    [LoiNhan] nvarchar(256) NULL,
    [TrangThai] varchar(32) NOT NULL,
    [GuiLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_YeuCauKetNoiLopHoc_GuiLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    [HetHanLuc] datetimeoffset(0) NOT NULL,
    [TenLopDuKien] nvarchar(256) NULL,
    [MonHocId] int NULL,
    [MucTieu] nvarchar(256) NULL,
    [SoBuoiTuan] int NULL,
    [ThoiLuongPhut] int NULL,
    [BatDauHocLuc] datetimeoffset(0) NULL,
    [HinhThucHoc] varchar(32) NULL,
    [DiaDiem] nvarchar(256) NULL,
    [HocPhi] decimal(18,2) NULL,
    [RowVersion] rowversion NOT NULL,
    [DonViHocPhi] varchar(16) NULL,
    [LienHeId] uniqueidentifier NOT NULL,
    [NguoiDeXuatId] uniqueidentifier NULL,
    [DeXuatLuc] datetimeoffset(0) NULL,
    CONSTRAINT [PK_YeuCauKetNoiLopHoc] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_1] CHECK ([TrangThai] IN (N'ChoGiaSu', N'ChoBenHoc', N'DaChapNhan', N'TuChoi', N'DaRut', N'HetHan')),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_2] CHECK ([SoBuoiTuan] > 0),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_3] CHECK ([ThoiLuongPhut] > 0),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_4] CHECK ([HinhThucHoc] IN (N'TrucTuyen', N'TrucTiep')),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_5] CHECK ([HocPhi] >= 0),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_6] CHECK ([DonViHocPhi] IN ('VND/Gio','VND/Buoi')),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_7] CHECK ((([HocPhi] IS NULL AND [DonViHocPhi] IS NULL) OR ([HocPhi] IS NOT NULL AND [DonViHocPhi] IS NOT NULL))),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_8] CHECK ([GiaSuId] <> [BenHocId]),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_9] CHECK ([NguoiGuiId] IN ([GiaSuId],[BenHocId])),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_10] CHECK ([SoBuoiTuan] BETWEEN 1 AND 7),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_11] CHECK ([ThoiLuongPhut] BETWEEN 1 AND 480),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_12] CHECK ([HetHanLuc] = DATEADD(hour,72,COALESCE([DeXuatLuc],[GuiLuc]))),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_13] CHECK ([NguoiDeXuatId] IS NULL OR [NguoiDeXuatId]=[GiaSuId]),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_14] CHECK (([DeXuatLuc] IS NULL AND [NguoiDeXuatId] IS NULL) OR ([DeXuatLuc] IS NOT NULL AND [NguoiDeXuatId] IS NOT NULL AND [DeXuatLuc]>=[GuiLuc])),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_15] CHECK ([TrangThai] NOT IN ('ChoBenHoc','DaChapNhan') OR ([NguoiDeXuatId]=[GiaSuId] AND [NguoiDeXuatId] IS NOT NULL AND [TenLopDuKien] IS NOT NULL AND LEN(LTRIM(RTRIM([TenLopDuKien])))>0 AND [MonHocId] IS NOT NULL AND [MucTieu] IS NOT NULL AND [SoBuoiTuan] IS NOT NULL AND [ThoiLuongPhut] IS NOT NULL AND [BatDauHocLuc] IS NOT NULL AND [HinhThucHoc] IS NOT NULL AND [HocPhi] IS NOT NULL AND [DonViHocPhi] IS NOT NULL)),
    CONSTRAINT [CK_YeuCauKetNoiLopHoc_16] CHECK ([NguoiGuiId]<>[BenHocId] OR ([LoiNhan] IS NOT NULL AND LEN(LTRIM(RTRIM([LoiNhan])))>0))
);
CREATE TABLE dbo.[YeuCauHuyKetNoi] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_YeuCauHuyKetNoi_Id] DEFAULT (NEWSEQUENTIALID()),
    [LopHocId] uniqueidentifier NOT NULL,
    [NguoiGuiId] uniqueidentifier NOT NULL,
    [LyDo] nvarchar(2000) NOT NULL,
    [TrangThai] varchar(32) NOT NULL,
    [GuiLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_YeuCauHuyKetNoi_GuiLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    [HetHanLuc] datetimeoffset(0) NOT NULL,
    [PhanHoiLuc] datetimeoffset(0) NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_YeuCauHuyKetNoi] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_YeuCauHuyKetNoi_1] CHECK ([TrangThai] IN (N'ChoPhanHoi', N'ChapNhan', N'TuChoi', N'TuDongChapNhan')),
    CONSTRAINT [CK_YeuCauHuyKetNoi_2] CHECK ([HetHanLuc] = DATEADD(hour,72,[GuiLuc])),
    CONSTRAINT [CK_YeuCauHuyKetNoi_3] CHECK ([PhanHoiLuc] IS NULL OR [PhanHoiLuc] >= [GuiLuc])
);
CREATE TABLE dbo.[CuocHoiThoai] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_CuocHoiThoai_Id] DEFAULT (NEWSEQUENTIALID()),
    [LoaiHoiThoai] varchar(32) NOT NULL,
    [GiaSuId] uniqueidentifier NULL,
    [BenHocId] uniqueidentifier NULL,
    [LopHocId] uniqueidentifier NULL,
    [HoiThoaiTruocKetNoiId] uniqueidentifier NULL,
    [TaoLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_CuocHoiThoai_TaoLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    [LienHeId] uniqueidentifier NULL,
    CONSTRAINT [PK_CuocHoiThoai] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_CuocHoiThoai_1] CHECK ([LoaiHoiThoai] IN (N'TruocKetNoi', N'TheoLop')),
    CONSTRAINT [CK_CuocHoiThoai_2] CHECK ([HoiThoaiTruocKetNoiId] IS NULL OR [HoiThoaiTruocKetNoiId] <> [Id]),
    CONSTRAINT [CK_CuocHoiThoai_3] CHECK (([LoaiHoiThoai]='TruocKetNoi' AND [GiaSuId] IS NOT NULL AND [BenHocId] IS NOT NULL AND [GiaSuId]<>[BenHocId] AND [LopHocId] IS NULL AND [HoiThoaiTruocKetNoiId] IS NULL) OR ([LoaiHoiThoai]='TheoLop' AND [LopHocId] IS NOT NULL)),
    CONSTRAINT [CK_CuocHoiThoai_4] CHECK ([LoaiHoiThoai]<>'TruocKetNoi' OR [LienHeId] IS NOT NULL)
);
CREATE TABLE dbo.[TinNhan] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_TinNhan_Id] DEFAULT (NEWSEQUENTIALID()),
    [CuocHoiThoaiId] uniqueidentifier NOT NULL,
    [NguoiGuiId] uniqueidentifier NOT NULL,
    [NoiDung] nvarchar(max) NOT NULL,
    [GuiLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_TinNhan_GuiLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    [MaThaoTac] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_TinNhan] PRIMARY KEY ([Id])
);
CREATE TABLE dbo.[LopHoc] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_LopHoc_Id] DEFAULT (NEWSEQUENTIALID()),
    [YeuCauKetNoiId] uniqueidentifier NOT NULL,
    [GiaSuId] uniqueidentifier NOT NULL,
    [BenHocId] uniqueidentifier NOT NULL,
    [TenLop] nvarchar(256) NOT NULL,
    [MonHocId] int NOT NULL,
    [MucTieu] nvarchar(256) NOT NULL,
    [SoBuoiTuan] int NOT NULL,
    [ThoiLuongPhut] int NOT NULL,
    [HinhThucHoc] varchar(32) NOT NULL,
    [DiaDiem] nvarchar(256) NULL,
    [HocPhi] decimal(18,2) NOT NULL,
    [TrangThai] varchar(32) NOT NULL,
    [GiaSuXacNhanKetThucLuc] datetimeoffset(0) NULL,
    [BenHocXacNhanKetThucLuc] datetimeoffset(0) NULL,
    [RowVersion] rowversion NOT NULL,
    [DonViHocPhi] varchar(16) NOT NULL,
    [TenNguoiHoc] nvarchar(256) NOT NULL,
    [TuoiNguoiHoc] int NULL,
    CONSTRAINT [PK_LopHoc] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_LopHoc_1] CHECK ([SoBuoiTuan] > 0),
    CONSTRAINT [CK_LopHoc_2] CHECK ([ThoiLuongPhut] > 0),
    CONSTRAINT [CK_LopHoc_3] CHECK ([HinhThucHoc] IN (N'TrucTuyen', N'TrucTiep')),
    CONSTRAINT [CK_LopHoc_4] CHECK ([HocPhi] >= 0),
    CONSTRAINT [CK_LopHoc_5] CHECK ([TrangThai] IN (N'DangHoatDong', N'DaHuy', N'DaKetThuc')),
    CONSTRAINT [CK_LopHoc_6] CHECK ([TuoiNguoiHoc] > 0),
    CONSTRAINT [CK_LopHoc_7] CHECK ([DonViHocPhi] IN ('VND/Gio','VND/Buoi')),
    CONSTRAINT [CK_LopHoc_8] CHECK ((([HocPhi] IS NULL AND [DonViHocPhi] IS NULL) OR ([HocPhi] IS NOT NULL AND [DonViHocPhi] IS NOT NULL))),
    CONSTRAINT [CK_LopHoc_9] CHECK ([GiaSuId] <> [BenHocId]),
    CONSTRAINT [CK_LopHoc_10] CHECK ([SoBuoiTuan] BETWEEN 1 AND 7),
    CONSTRAINT [CK_LopHoc_11] CHECK ([ThoiLuongPhut] BETWEEN 1 AND 480),
    CONSTRAINT [CK_LopHoc_12] CHECK ([TuoiNguoiHoc] BETWEEN 1 AND 120),
    CONSTRAINT [CK_LopHoc_13] CHECK (LEN(LTRIM(RTRIM([TenNguoiHoc])))>0)
);
CREATE TABLE dbo.[ThanhVienLopHoc] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_ThanhVienLopHoc_Id] DEFAULT (NEWSEQUENTIALID()),
    [LopHocId] uniqueidentifier NOT NULL,
    [TaiKhoanId] uniqueidentifier NOT NULL,
    [NguoiDeNghiId] uniqueidentifier NOT NULL,
    [TrangThaiDuyet] varchar(32) NOT NULL,
    [MucQuyen] varchar(32) NULL,
    [DeNghiLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_ThanhVienLopHoc_DeNghiLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    [DuyetLuc] datetimeoffset(0) NULL,
    [RoiLopLuc] datetimeoffset(0) NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_ThanhVienLopHoc] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_ThanhVienLopHoc_1] CHECK ([TrangThaiDuyet] IN (N'ChoDuyet', N'DaDuyet', N'TuChoi', N'DaRoiLop')),
    CONSTRAINT [CK_ThanhVienLopHoc_2] CHECK ([MucQuyen] IN (N'Xem', N'ChinhSua')),
    CONSTRAINT [CK_ThanhVienLopHoc_3] CHECK ([TrangThaiDuyet] <> 'DaDuyet' OR ([MucQuyen] IS NOT NULL AND [DuyetLuc] IS NOT NULL)),
    CONSTRAINT [CK_ThanhVienLopHoc_4] CHECK ([DuyetLuc] IS NULL OR [DuyetLuc] >= [DeNghiLuc]),
    CONSTRAINT [CK_ThanhVienLopHoc_5] CHECK ([RoiLopLuc] IS NULL OR [RoiLopLuc] >= [DeNghiLuc])
);
CREATE TABLE dbo.[LichHoc] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_LichHoc_Id] DEFAULT (NEWSEQUENTIALID()),
    [LopHocId] uniqueidentifier NOT NULL,
    [TieuDe] nvarchar(256) NOT NULL,
    [BatDauLuc] datetimeoffset(0) NOT NULL,
    [KetThucLuc] datetimeoffset(0) NOT NULL,
    [LaBuoiBu] bit NOT NULL CONSTRAINT [DF_LichHoc_LaBuoiBu] DEFAULT (0),
    [TrangThai] varchar(32) NOT NULL,
    [RowVersion] rowversion NOT NULL,
    [LichGocId] uniqueidentifier NULL,
    CONSTRAINT [PK_LichHoc] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_LichHoc_1] CHECK ([TrangThai] IN (N'DaLenLich', N'DaDieuChinh', N'DaHuy')),
    CONSTRAINT [CK_LichHoc_2] CHECK ([BatDauLuc] < [KetThucLuc]),
    CONSTRAINT [CK_LichHoc_3] CHECK ([LichGocId] IS NULL OR [LichGocId] <> [Id]),
    CONSTRAINT [CK_LichHoc_4] CHECK (([LaBuoiBu] = 1 AND [LichGocId] IS NOT NULL) OR ([LaBuoiBu] = 0 AND [LichGocId] IS NULL))
);
CREATE TABLE dbo.[BuoiHoc] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_BuoiHoc_Id] DEFAULT (NEWSEQUENTIALID()),
    [LichHocId] uniqueidentifier NOT NULL,
    [TieuDe] nvarchar(256) NOT NULL,
    [NoiDungGiangDay] nvarchar(max) NULL,
    [NhanXet] nvarchar(2000) NOT NULL,
    [TrangThaiThamDu] varchar(32) NOT NULL,
    [GhiNhanLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_BuoiHoc_GhiNhanLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    [NguoiGhiNhanId] uniqueidentifier NOT NULL,
    [RowVersion] rowversion NOT NULL,
    [TrangThaiThamDuGiaSu] varchar(32) NOT NULL CONSTRAINT [DF_BuoiHoc_TrangThaiThamDuGiaSu] DEFAULT ('CoMat'),
    [TrangThaiThamDuBenHoc] varchar(32) NOT NULL CONSTRAINT [DF_BuoiHoc_TrangThaiThamDuBenHoc] DEFAULT ('CoMat'),
    [KetQuaBuoiHoc] varchar(32) NOT NULL,
    [CapNhatLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_BuoiHoc_CapNhatLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    [PhienBan] int NOT NULL CONSTRAINT [DF_BuoiHoc_PhienBan] DEFAULT (1),
    CONSTRAINT [PK_BuoiHoc] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_BuoiHoc_1] CHECK ([TrangThaiThamDu] IN (N'CoMat', N'VangCoBaoTruoc', N'VangKhongBaoTruoc')),
    CONSTRAINT [CK_BuoiHoc_2] CHECK ([TrangThaiThamDuGiaSu] IN (N'CoMat', N'VangCoBaoTruoc', N'VangKhongBaoTruoc')),
    CONSTRAINT [CK_BuoiHoc_3] CHECK ([TrangThaiThamDuBenHoc] IN (N'CoMat', N'VangCoBaoTruoc', N'VangKhongBaoTruoc')),
    CONSTRAINT [CK_BuoiHoc_4] CHECK ([KetQuaBuoiHoc] IN ('DaDienRa','KhongDienRa')),
    CONSTRAINT [CK_BuoiHoc_5] CHECK ([PhienBan]>0),
    CONSTRAINT [CK_BuoiHoc_6] CHECK (LEN(LTRIM(RTRIM([NhanXet])))>0),
    CONSTRAINT [CK_BuoiHoc_7] CHECK ([KetQuaBuoiHoc]<>'DaDienRa' OR ([NoiDungGiangDay] IS NOT NULL AND LEN(LTRIM(RTRIM([NoiDungGiangDay])))>0)),
    CONSTRAINT [CK_BuoiHoc_8] CHECK ([KetQuaBuoiHoc]<>'KhongDienRa' OR [TrangThaiThamDuGiaSu]<>'CoMat' OR [TrangThaiThamDuBenHoc]<>'CoMat')
);
CREATE TABLE dbo.[BaiTapGiao] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_BaiTapGiao_Id] DEFAULT (NEWSEQUENTIALID()),
    [LopHocId] uniqueidentifier NOT NULL,
    [BuoiHocId] uniqueidentifier NULL,
    [NguoiGiaoId] uniqueidentifier NOT NULL,
    [TieuDe] nvarchar(256) NOT NULL,
    [NoiDung] nvarchar(max) NOT NULL,
    [HanNopLuc] datetimeoffset(0) NOT NULL,
    [TieuChiCham] nvarchar(max) NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_BaiTapGiao] PRIMARY KEY ([Id])
);
CREATE TABLE dbo.[BaiTapNop] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_BaiTapNop_Id] DEFAULT (NEWSEQUENTIALID()),
    [BaiTapGiaoId] uniqueidentifier NOT NULL,
    [NguoiNopId] uniqueidentifier NOT NULL,
    [NoiDung] nvarchar(max) NULL,
    [NopLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_BaiTapNop_NopLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    [MaThaoTac] uniqueidentifier NOT NULL,
    [RowVersion] rowversion NOT NULL,
    [LanNop] int NOT NULL,
    CONSTRAINT [PK_BaiTapNop] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_BaiTapNop_1] CHECK ([LanNop] > 0)
);
CREATE TABLE dbo.[KetQuaChamBai] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_KetQuaChamBai_Id] DEFAULT (NEWSEQUENTIALID()),
    [BaiTapNopId] uniqueidentifier NOT NULL,
    [NguoiChamId] uniqueidentifier NOT NULL,
    [Diem] decimal(4,2) NULL,
    [NhanXet] nvarchar(2000) NOT NULL,
    [ChamLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_KetQuaChamBai_ChamLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_KetQuaChamBai] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_KetQuaChamBai_1] CHECK ([Diem] IS NULL OR [Diem] BETWEEN 0 AND 10)
);
CREATE TABLE dbo.[GiaSu] (
    [TaiKhoanId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_GiaSu] PRIMARY KEY ([TaiKhoanId])
);
CREATE TABLE dbo.[TepDinhKem] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_TepDinhKem_Id] DEFAULT (NEWSEQUENTIALID()),
    [NguoiTaiLenId] uniqueidentifier NOT NULL,
    [TenGoc] nvarchar(256) NOT NULL,
    [TenLuuTru] nvarchar(512) NOT NULL,
    [LoaiNoiDung] nvarchar(256) NOT NULL,
    [KichThuoc] bigint NOT NULL,
    [Checksum] nvarchar(256) NOT NULL,
    [TrangThaiQuet] varchar(32) NOT NULL,
    [TaoLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_TepDinhKem_TaoLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    [HoSoGiaSuId] uniqueidentifier NULL,
    [BaiTapGiaoId] uniqueidentifier NULL,
    [BaiTapNopId] uniqueidentifier NULL,
    [TinNhanId] uniqueidentifier NULL,
    CONSTRAINT [PK_TepDinhKem] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_TepDinhKem_1] CHECK ([TrangThaiQuet] IN (N'ChoQuet', N'CachLy', N'Sach', N'TuChoi')),
    CONSTRAINT [CK_TepDinhKem_2] CHECK ([KichThuoc] BETWEEN 1 AND 10000000),
    CONSTRAINT [CK_TepDinhKem_3] CHECK (CASE WHEN [HoSoGiaSuId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [BaiTapGiaoId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [BaiTapNopId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [TinNhanId] IS NULL THEN 0 ELSE 1 END <= 1)
);
CREATE TABLE dbo.[ThongBao] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_ThongBao_Id] DEFAULT (NEWSEQUENTIALID()),
    [NguoiNhanId] uniqueidentifier NOT NULL,
    [LoaiThongBao] varchar(32) NOT NULL,
    [DoiTuongId] uniqueidentifier NOT NULL,
    [NoiDungTomTat] nvarchar(256) NOT NULL,
    [TaoLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_ThongBao_TaoLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    [DaDocLuc] datetimeoffset(0) NULL,
    [MaSuKien] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_ThongBao] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_ThongBao_1] CHECK ([LoaiThongBao] IN (N'KetNoi', N'LichHoc', N'BaiTap', N'ThanhVien', N'NhacViec', N'QuanTri'))
);
CREATE TABLE dbo.[SuKienChoXuLy] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_SuKienChoXuLy_Id] DEFAULT (NEWSEQUENTIALID()),
    [LoaiSuKien] nvarchar(256) NOT NULL,
    [DuLieuToiThieu] nvarchar(max) NOT NULL,
    [TaoLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_SuKienChoXuLy_TaoLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    [TrangThai] varchar(32) NOT NULL,
    [SoLanThu] int NOT NULL CONSTRAINT [DF_SuKienChoXuLy_SoLanThu] DEFAULT (0),
    [ThuLaiLuc] datetimeoffset(0) NULL,
    [XuLyXongLuc] datetimeoffset(0) NULL,
    [RowVersion] rowversion NOT NULL,
    [LeaseId] uniqueidentifier NULL,
    [LeaseHetHanLuc] datetimeoffset(0) NULL,
    CONSTRAINT [PK_SuKienChoXuLy] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_SuKienChoXuLy_1] CHECK ([TrangThai] IN (N'ChoXuLy', N'DangXuLy', N'ChoThuLai', N'HoanTat')),
    CONSTRAINT [CK_SuKienChoXuLy_2] CHECK ([SoLanThu] >= 0),
    CONSTRAINT [CK_SuKienChoXuLy_3] CHECK (([LeaseId] IS NULL AND [LeaseHetHanLuc] IS NULL) OR ([LeaseId] IS NOT NULL AND [LeaseHetHanLuc] IS NOT NULL)),
    CONSTRAINT [CK_SuKienChoXuLy_4] CHECK (ISJSON([DuLieuToiThieu])=1)
);
CREATE TABLE dbo.[YeuCauLienHe] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_YeuCauLienHe_Id] DEFAULT (NEWSEQUENTIALID()),
    [GiaSuId] uniqueidentifier NOT NULL,
    [BenHocId] uniqueidentifier NOT NULL,
    [NguoiGuiId] uniqueidentifier NOT NULL,
    [LoiNhan] nvarchar(256) NOT NULL,
    [TrangThai] varchar(32) NOT NULL,
    [GuiLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_YeuCauLienHe_GuiLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    [NguoiPhanHoiId] uniqueidentifier NULL,
    [PhanHoiLuc] datetimeoffset(0) NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_YeuCauLienHe] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_YeuCauLienHe_1] CHECK ([GiaSuId]<>[BenHocId]),
    CONSTRAINT [CK_YeuCauLienHe_2] CHECK ([NguoiGuiId] IN ([GiaSuId],[BenHocId])),
    CONSTRAINT [CK_YeuCauLienHe_3] CHECK ([TrangThai] IN ('ChoPhanHoi','ChapNhan','TuChoi','DaRut')),
    CONSTRAINT [CK_YeuCauLienHe_4] CHECK (LEN(LTRIM(RTRIM([LoiNhan])))>0),
    CONSTRAINT [CK_YeuCauLienHe_5] CHECK ([PhanHoiLuc] IS NULL OR [PhanHoiLuc]>=[GuiLuc]),
    CONSTRAINT [CK_YeuCauLienHe_6] CHECK ([NguoiPhanHoiId] IS NULL OR ([NguoiPhanHoiId] IN ([GiaSuId],[BenHocId]) AND [NguoiPhanHoiId]<>[NguoiGuiId])),
    CONSTRAINT [CK_YeuCauLienHe_7] CHECK ([TrangThai] NOT IN ('ChapNhan','TuChoi') OR ([PhanHoiLuc] IS NOT NULL AND [NguoiPhanHoiId] IS NOT NULL))
);
CREATE TABLE dbo.[KhungGioDeXuat] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_KhungGioDeXuat_Id] DEFAULT (NEWSEQUENTIALID()),
    [YeuCauKetNoiId] uniqueidentifier NOT NULL,
    [ThuTrongTuan] tinyint NOT NULL,
    [GioBatDau] time(0) NOT NULL,
    [GioKetThuc] time(0) NOT NULL,
    CONSTRAINT [PK_KhungGioDeXuat] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_KhungGioDeXuat_1] CHECK ([ThuTrongTuan] BETWEEN 0 AND 6),
    CONSTRAINT [CK_KhungGioDeXuat_2] CHECK ([GioBatDau]<[GioKetThuc])
);
CREATE TABLE dbo.[LichSuGhiNhanBuoiHoc] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_LichSuGhiNhanBuoiHoc_Id] DEFAULT (NEWSEQUENTIALID()),
    [BuoiHocId] uniqueidentifier NOT NULL,
    [PhienBan] int NOT NULL,
    [TieuDe] nvarchar(256) NOT NULL,
    [KetQuaBuoiHoc] varchar(32) NOT NULL,
    [NoiDungGiangDay] nvarchar(max) NULL,
    [NhanXet] nvarchar(2000) NOT NULL,
    [TrangThaiThamDuGiaSu] varchar(32) NOT NULL CONSTRAINT [DF_LichSuGhiNhanBuoiHoc_TrangThaiThamDuGiaSu] DEFAULT ('CoMat'),
    [TrangThaiThamDuBenHoc] varchar(32) NOT NULL CONSTRAINT [DF_LichSuGhiNhanBuoiHoc_TrangThaiThamDuBenHoc] DEFAULT ('CoMat'),
    [NguoiGhiNhanId] uniqueidentifier NOT NULL,
    [GhiNhanLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_LichSuGhiNhanBuoiHoc_GhiNhanLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    [NguoiSuaId] uniqueidentifier NOT NULL,
    [ThayDoiLuc] datetimeoffset(0) NOT NULL CONSTRAINT [DF_LichSuGhiNhanBuoiHoc_ThayDoiLuc] DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00')),
    CONSTRAINT [PK_LichSuGhiNhanBuoiHoc] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_LichSuGhiNhanBuoiHoc_1] CHECK ([KetQuaBuoiHoc] IN ('DaDienRa','KhongDienRa')),
    CONSTRAINT [CK_LichSuGhiNhanBuoiHoc_2] CHECK ([PhienBan]>0),
    CONSTRAINT [CK_LichSuGhiNhanBuoiHoc_3] CHECK (LEN(LTRIM(RTRIM([NhanXet])))>0),
    CONSTRAINT [CK_LichSuGhiNhanBuoiHoc_4] CHECK ([KetQuaBuoiHoc]<>'DaDienRa' OR ([NoiDungGiangDay] IS NOT NULL AND LEN(LTRIM(RTRIM([NoiDungGiangDay])))>0)),
    CONSTRAINT [CK_LichSuGhiNhanBuoiHoc_5] CHECK ([KetQuaBuoiHoc]<>'KhongDienRa' OR [TrangThaiThamDuGiaSu]<>'CoMat' OR [TrangThaiThamDuBenHoc]<>'CoMat')
);

-- Dữ liệu định danh để thử nghiệm nghiệp vụ, không là tài khoản.
CREATE TABLE dbo.[NguoiThamGia] (
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_NguoiThamGia_Id] DEFAULT (NEWSEQUENTIALID()),
    [HoTen] nvarchar(256) NOT NULL,
    [LoaiNguoi] varchar(16) NOT NULL,
    CONSTRAINT [PK_NguoiThamGia] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_NguoiThamGia_1] CHECK ([LoaiNguoi] IN ('GiaSu','HocSinh','PhuHuynh'))
);
ALTER TABLE dbo.[HoSoGiaSu] WITH CHECK ADD CONSTRAINT [FK_HoSoGiaSu_GiaSuId] FOREIGN KEY ([GiaSuId]) REFERENCES dbo.[GiaSu]([TaiKhoanId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE UNIQUE INDEX [UQ_GiaSu_HoSoGiaSu] ON dbo.[HoSoGiaSu] ([GiaSuId]);
ALTER TABLE dbo.[HoSoGiaSuMonDay] WITH CHECK ADD CONSTRAINT [FK_HoSoGiaSuMonDay_HoSoGiaSuId] FOREIGN KEY ([HoSoGiaSuId]) REFERENCES dbo.[HoSoGiaSu]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[HoSoGiaSuMonDay] WITH CHECK ADD CONSTRAINT [FK_HoSoGiaSuMonDay_MonHocId] FOREIGN KEY ([MonHocId]) REFERENCES dbo.[MonHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE INDEX [IX_Mon_HoSoGiaSuMonDay] ON dbo.[HoSoGiaSuMonDay] ([MonHocId], [HoSoGiaSuId]);
ALTER TABLE dbo.[NhuCauHoc] WITH CHECK ADD CONSTRAINT [FK_NhuCauHoc_ChuSoHuuId] FOREIGN KEY ([ChuSoHuuId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[NhuCauHoc] WITH CHECK ADD CONSTRAINT [FK_NhuCauHoc_MonHocId] FOREIGN KEY ([MonHocId]) REFERENCES dbo.[MonHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE INDEX [IX_TimKiem_NhuCauHoc] ON dbo.[NhuCauHoc] ([TrangThai], [MonHocId], [LoaiNhuCau]);
ALTER TABLE dbo.[KhungGioRanh] WITH CHECK ADD CONSTRAINT [FK_KhungGioRanh_NhuCauId] FOREIGN KEY ([NhuCauId]) REFERENCES dbo.[NhuCauHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[YeuCauKetNoiLopHoc] WITH CHECK ADD CONSTRAINT [FK_YeuCauKetNoiLopHoc_GiaSuId] FOREIGN KEY ([GiaSuId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[YeuCauKetNoiLopHoc] WITH CHECK ADD CONSTRAINT [FK_YeuCauKetNoiLopHoc_BenHocId] FOREIGN KEY ([BenHocId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[YeuCauKetNoiLopHoc] WITH CHECK ADD CONSTRAINT [FK_YeuCauKetNoiLopHoc_NguoiGuiId] FOREIGN KEY ([NguoiGuiId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[YeuCauKetNoiLopHoc] WITH CHECK ADD CONSTRAINT [FK_YeuCauKetNoiLopHoc_NhuCauId] FOREIGN KEY ([NhuCauId]) REFERENCES dbo.[NhuCauHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[YeuCauKetNoiLopHoc] WITH CHECK ADD CONSTRAINT [FK_YeuCauKetNoiLopHoc_MonHocId] FOREIGN KEY ([MonHocId]) REFERENCES dbo.[MonHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[YeuCauKetNoiLopHoc] WITH CHECK ADD CONSTRAINT [FK_YeuCauKetNoiLopHoc_LienHeId] FOREIGN KEY ([LienHeId]) REFERENCES dbo.[YeuCauLienHe]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[YeuCauKetNoiLopHoc] WITH CHECK ADD CONSTRAINT [FK_YeuCauKetNoiLopHoc_NguoiDeXuatId] FOREIGN KEY ([NguoiDeXuatId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE INDEX [IX_HetHan_YeuCauKetNoiLopHoc] ON dbo.[YeuCauKetNoiLopHoc] ([TrangThai], [HetHanLuc]);
ALTER TABLE dbo.[YeuCauHuyKetNoi] WITH CHECK ADD CONSTRAINT [FK_YeuCauHuyKetNoi_LopHocId] FOREIGN KEY ([LopHocId]) REFERENCES dbo.[LopHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[YeuCauHuyKetNoi] WITH CHECK ADD CONSTRAINT [FK_YeuCauHuyKetNoi_NguoiGuiId] FOREIGN KEY ([NguoiGuiId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE INDEX [IX_HetHan_YeuCauHuyKetNoi] ON dbo.[YeuCauHuyKetNoi] ([TrangThai], [HetHanLuc]);
ALTER TABLE dbo.[CuocHoiThoai] WITH CHECK ADD CONSTRAINT [FK_CuocHoiThoai_GiaSuId] FOREIGN KEY ([GiaSuId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[CuocHoiThoai] WITH CHECK ADD CONSTRAINT [FK_CuocHoiThoai_BenHocId] FOREIGN KEY ([BenHocId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[CuocHoiThoai] WITH CHECK ADD CONSTRAINT [FK_CuocHoiThoai_LopHocId] FOREIGN KEY ([LopHocId]) REFERENCES dbo.[LopHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[CuocHoiThoai] WITH CHECK ADD CONSTRAINT [FK_CuocHoiThoai_HoiThoaiTruocKetNoiId] FOREIGN KEY ([HoiThoaiTruocKetNoiId]) REFERENCES dbo.[CuocHoiThoai]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[CuocHoiThoai] WITH CHECK ADD CONSTRAINT [FK_CuocHoiThoai_LienHeId] FOREIGN KEY ([LienHeId]) REFERENCES dbo.[YeuCauLienHe]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE UNIQUE INDEX [UQ_TruocKetNoi_CuocHoiThoai] ON dbo.[CuocHoiThoai] ([LienHeId]) WHERE [LoaiHoiThoai]='TruocKetNoi';
ALTER TABLE dbo.[TinNhan] WITH CHECK ADD CONSTRAINT [FK_TinNhan_CuocHoiThoaiId] FOREIGN KEY ([CuocHoiThoaiId]) REFERENCES dbo.[CuocHoiThoai]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[TinNhan] WITH CHECK ADD CONSTRAINT [FK_TinNhan_NguoiGuiId] FOREIGN KEY ([NguoiGuiId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE UNIQUE INDEX [UQ_Retry_TinNhan] ON dbo.[TinNhan] ([NguoiGuiId], [MaThaoTac]);
CREATE INDEX [IX_LichSu_TinNhan] ON dbo.[TinNhan] ([CuocHoiThoaiId], [GuiLuc], [Id]);
ALTER TABLE dbo.[LopHoc] WITH CHECK ADD CONSTRAINT [FK_LopHoc_YeuCauKetNoiId] FOREIGN KEY ([YeuCauKetNoiId]) REFERENCES dbo.[YeuCauKetNoiLopHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[LopHoc] WITH CHECK ADD CONSTRAINT [FK_LopHoc_GiaSuId] FOREIGN KEY ([GiaSuId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[LopHoc] WITH CHECK ADD CONSTRAINT [FK_LopHoc_BenHocId] FOREIGN KEY ([BenHocId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[LopHoc] WITH CHECK ADD CONSTRAINT [FK_LopHoc_MonHocId] FOREIGN KEY ([MonHocId]) REFERENCES dbo.[MonHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE UNIQUE INDEX [UQ_YeuCau_LopHoc] ON dbo.[LopHoc] ([YeuCauKetNoiId]);
ALTER TABLE dbo.[ThanhVienLopHoc] WITH CHECK ADD CONSTRAINT [FK_ThanhVienLopHoc_LopHocId] FOREIGN KEY ([LopHocId]) REFERENCES dbo.[LopHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[ThanhVienLopHoc] WITH CHECK ADD CONSTRAINT [FK_ThanhVienLopHoc_TaiKhoanId] FOREIGN KEY ([TaiKhoanId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[ThanhVienLopHoc] WITH CHECK ADD CONSTRAINT [FK_ThanhVienLopHoc_NguoiDeNghiId] FOREIGN KEY ([NguoiDeNghiId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE UNIQUE INDEX [UQ_ChuaRoi_ThanhVienLopHoc] ON dbo.[ThanhVienLopHoc] ([LopHocId], [TaiKhoanId]) WHERE [RoiLopLuc] IS NULL AND [TrangThaiDuyet] <> 'TuChoi';
ALTER TABLE dbo.[LichHoc] WITH CHECK ADD CONSTRAINT [FK_LichHoc_LopHocId] FOREIGN KEY ([LopHocId]) REFERENCES dbo.[LopHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[LichHoc] WITH CHECK ADD CONSTRAINT [FK_LichHoc_LichGocId] FOREIGN KEY ([LichGocId]) REFERENCES dbo.[LichHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE INDEX [IX_Lop_LichHoc] ON dbo.[LichHoc] ([LopHocId], [BatDauLuc]);
ALTER TABLE dbo.[BuoiHoc] WITH CHECK ADD CONSTRAINT [FK_BuoiHoc_LichHocId] FOREIGN KEY ([LichHocId]) REFERENCES dbo.[LichHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[BuoiHoc] WITH CHECK ADD CONSTRAINT [FK_BuoiHoc_NguoiGhiNhanId] FOREIGN KEY ([NguoiGhiNhanId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE UNIQUE INDEX [UQ_Lich_BuoiHoc] ON dbo.[BuoiHoc] ([LichHocId]);
ALTER TABLE dbo.[BaiTapGiao] WITH CHECK ADD CONSTRAINT [FK_BaiTapGiao_LopHocId] FOREIGN KEY ([LopHocId]) REFERENCES dbo.[LopHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[BaiTapGiao] WITH CHECK ADD CONSTRAINT [FK_BaiTapGiao_BuoiHocId] FOREIGN KEY ([BuoiHocId]) REFERENCES dbo.[BuoiHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[BaiTapGiao] WITH CHECK ADD CONSTRAINT [FK_BaiTapGiao_NguoiGiaoId] FOREIGN KEY ([NguoiGiaoId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[BaiTapNop] WITH CHECK ADD CONSTRAINT [FK_BaiTapNop_BaiTapGiaoId] FOREIGN KEY ([BaiTapGiaoId]) REFERENCES dbo.[BaiTapGiao]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[BaiTapNop] WITH CHECK ADD CONSTRAINT [FK_BaiTapNop_NguoiNopId] FOREIGN KEY ([NguoiNopId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE UNIQUE INDEX [UQ_Retry_BaiTapNop] ON dbo.[BaiTapNop] ([NguoiNopId], [MaThaoTac]);
CREATE UNIQUE INDEX [UQ_Lan_BaiTapNop] ON dbo.[BaiTapNop] ([BaiTapGiaoId], [LanNop]);
ALTER TABLE dbo.[KetQuaChamBai] WITH CHECK ADD CONSTRAINT [FK_KetQuaChamBai_BaiTapNopId] FOREIGN KEY ([BaiTapNopId]) REFERENCES dbo.[BaiTapNop]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[KetQuaChamBai] WITH CHECK ADD CONSTRAINT [FK_KetQuaChamBai_NguoiChamId] FOREIGN KEY ([NguoiChamId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE UNIQUE INDEX [UQ_BaiNop_KetQuaChamBai] ON dbo.[KetQuaChamBai] ([BaiTapNopId]);
ALTER TABLE dbo.[GiaSu] WITH CHECK ADD CONSTRAINT [FK_GiaSu_TaiKhoanId] FOREIGN KEY ([TaiKhoanId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[TepDinhKem] WITH CHECK ADD CONSTRAINT [FK_TepDinhKem_NguoiTaiLenId] FOREIGN KEY ([NguoiTaiLenId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[TepDinhKem] WITH CHECK ADD CONSTRAINT [FK_TepDinhKem_HoSoGiaSuId] FOREIGN KEY ([HoSoGiaSuId]) REFERENCES dbo.[HoSoGiaSu]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[TepDinhKem] WITH CHECK ADD CONSTRAINT [FK_TepDinhKem_BaiTapGiaoId] FOREIGN KEY ([BaiTapGiaoId]) REFERENCES dbo.[BaiTapGiao]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[TepDinhKem] WITH CHECK ADD CONSTRAINT [FK_TepDinhKem_BaiTapNopId] FOREIGN KEY ([BaiTapNopId]) REFERENCES dbo.[BaiTapNop]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[TepDinhKem] WITH CHECK ADD CONSTRAINT [FK_TepDinhKem_TinNhanId] FOREIGN KEY ([TinNhanId]) REFERENCES dbo.[TinNhan]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE UNIQUE INDEX [UQ_KhoaLuuTru_TepDinhKem] ON dbo.[TepDinhKem] ([TenLuuTru]);
ALTER TABLE dbo.[ThongBao] WITH CHECK ADD CONSTRAINT [FK_ThongBao_NguoiNhanId] FOREIGN KEY ([NguoiNhanId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[ThongBao] WITH CHECK ADD CONSTRAINT [FK_ThongBao_MaSuKien] FOREIGN KEY ([MaSuKien]) REFERENCES dbo.[SuKienChoXuLy]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE UNIQUE INDEX [UQ_SuKien_ThongBao] ON dbo.[ThongBao] ([MaSuKien], [NguoiNhanId]);
CREATE INDEX [IX_HopTin_ThongBao] ON dbo.[ThongBao] ([NguoiNhanId], [DaDocLuc], [TaoLuc]);
CREATE INDEX [IX_HangDoi_SuKienChoXuLy] ON dbo.[SuKienChoXuLy] ([TrangThai], [ThuLaiLuc], [LeaseHetHanLuc]);
ALTER TABLE dbo.[YeuCauLienHe] WITH CHECK ADD CONSTRAINT [FK_YeuCauLienHe_GiaSuId] FOREIGN KEY ([GiaSuId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[YeuCauLienHe] WITH CHECK ADD CONSTRAINT [FK_YeuCauLienHe_BenHocId] FOREIGN KEY ([BenHocId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[YeuCauLienHe] WITH CHECK ADD CONSTRAINT [FK_YeuCauLienHe_NguoiGuiId] FOREIGN KEY ([NguoiGuiId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[YeuCauLienHe] WITH CHECK ADD CONSTRAINT [FK_YeuCauLienHe_NguoiPhanHoiId] FOREIGN KEY ([NguoiPhanHoiId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE UNIQUE INDEX [UQ_DangDung_YeuCauLienHe] ON dbo.[YeuCauLienHe] ([GiaSuId], [BenHocId]) WHERE [TrangThai] IN ('ChoPhanHoi','ChapNhan');
CREATE INDEX [IX_NguoiNhan_YeuCauLienHe] ON dbo.[YeuCauLienHe] ([GiaSuId], [BenHocId], [TrangThai]);
ALTER TABLE dbo.[KhungGioDeXuat] WITH CHECK ADD CONSTRAINT [FK_KhungGioDeXuat_YeuCauKetNoiId] FOREIGN KEY ([YeuCauKetNoiId]) REFERENCES dbo.[YeuCauKetNoiLopHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE UNIQUE INDEX [UQ_Khung_KhungGioDeXuat] ON dbo.[KhungGioDeXuat] ([YeuCauKetNoiId], [ThuTrongTuan], [GioBatDau]);
ALTER TABLE dbo.[LichSuGhiNhanBuoiHoc] WITH CHECK ADD CONSTRAINT [FK_LichSuGhiNhanBuoiHoc_BuoiHocId] FOREIGN KEY ([BuoiHocId]) REFERENCES dbo.[BuoiHoc]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[LichSuGhiNhanBuoiHoc] WITH CHECK ADD CONSTRAINT [FK_LichSuGhiNhanBuoiHoc_NguoiGhiNhanId] FOREIGN KEY ([NguoiGhiNhanId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE dbo.[LichSuGhiNhanBuoiHoc] WITH CHECK ADD CONSTRAINT [FK_LichSuGhiNhanBuoiHoc_NguoiSuaId] FOREIGN KEY ([NguoiSuaId]) REFERENCES dbo.[NguoiThamGia]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
CREATE UNIQUE INDEX [UQ_PhienBan_LichSuGhiNhanBuoiHoc] ON dbo.[LichSuGhiNhanBuoiHoc] ([BuoiHocId], [PhienBan]);
INSERT dbo.MonHoc(TenMon,DangSuDung) VALUES(N'Toán Học',1),(N'Ngữ Văn',1),(N'Tiếng Anh',1),(N'Vật Lý',1),(N'Hóa Học',1),(N'Lịch Sử',1),(N'Địa Lý',1),(N'Sinh học',1),(N'GDCD',1);
INSERT dbo.NguoiThamGia(Id,HoTen,LoaiNguoi) VALUES
('10000000-0000-0000-0000-000000000001',N'Gia sư mẫu','GiaSu'),
('10000000-0000-0000-0000-000000000002',N'Học sinh mẫu','HocSinh'),
('10000000-0000-0000-0000-000000000003',N'Phụ huynh mẫu','PhuHuynh');
INSERT dbo.GiaSu(TaiKhoanId) VALUES('10000000-0000-0000-0000-000000000001');
INSERT dbo.HoSoGiaSu(Id,GiaSuId,TrinhDo,ChuyenMon,KinhNghiem,MoTa,KhuVuc,TrangThaiDuyet) VALUES('20000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000001',N'Đại học',N'Toán',N'2 năm',N'Hồ sơ mẫu để thử tìm kiếm',N'Hà Nội','DaDuyet');
INSERT dbo.HoSoGiaSuMonDay(HoSoGiaSuId,MonHocId,PhamViDay) VALUES('20000000-0000-0000-0000-000000000001',1,N'Lớp 6–12');
INSERT dbo.NhuCauHoc(Id,ChuSoHuuId,LoaiNhuCau,MonHocId,KhoiLop,MucTieu,SoBuoiTuan,HinhThucHoc,KhuVuc,HocPhiDuKien,TenNguoiHoc,TuoiNguoiHoc,TrangThai,DonViHocPhi) VALUES('30000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000002','TimGiaSu',1,N'Lớp 9',N'Củng cố kiến thức',2,'TrucTuyen',N'Hà Nội',150000,N'Người học mẫu',15,'DangMo','VND/Gio');
INSERT dbo.KhungGioRanh(NhuCauId,ThuTrongTuan,GioBatDau,GioKetThuc) VALUES('30000000-0000-0000-0000-000000000001',2,'18:00','20:00'),('30000000-0000-0000-0000-000000000001',4,'18:00','20:00');
INSERT dbo.YeuCauLienHe(Id,GiaSuId,BenHocId,NguoiGuiId,LoiNhan,TrangThai,NguoiPhanHoiId,PhanHoiLuc) VALUES('41000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000002','10000000-0000-0000-0000-000000000002',N'Mong muốn trao đổi việc học','ChapNhan','10000000-0000-0000-0000-000000000001',TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00'));
DECLARE @GuiLuc datetimeoffset(0)=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
INSERT dbo.YeuCauKetNoiLopHoc(LienHeId,NguoiDeXuatId,DeXuatLuc,BatDauHocLuc,Id,GiaSuId,BenHocId,NguoiGuiId,NhuCauId,TrangThai,GuiLuc,HetHanLuc,TenLopDuKien,MonHocId,MucTieu,SoBuoiTuan,ThoiLuongPhut,HinhThucHoc,HocPhi,DonViHocPhi) VALUES('41000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000001',@GuiLuc,'2026-10-27T18:00:00+07:00','40000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000002','10000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000001','DaChapNhan',@GuiLuc,DATEADD(hour,72,@GuiLuc),N'Lớp Toán mẫu',1,N'Củng cố kiến thức',2,90,'TrucTuyen',150000,'VND/Gio');
INSERT dbo.LopHoc(TenNguoiHoc,TuoiNguoiHoc,Id,YeuCauKetNoiId,GiaSuId,BenHocId,TenLop,MonHocId,MucTieu,SoBuoiTuan,ThoiLuongPhut,HinhThucHoc,HocPhi,TrangThai,DonViHocPhi) VALUES(N'Người học mẫu',15,'50000000-0000-0000-0000-000000000001','40000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000002',N'Lớp Toán mẫu',1,N'Củng cố kiến thức',2,90,'TrucTuyen',150000,'DangHoatDong','VND/Gio');
INSERT dbo.CuocHoiThoai(Id,LoaiHoiThoai,LopHocId) VALUES('60000000-0000-0000-0000-000000000001','TheoLop','50000000-0000-0000-0000-000000000001');
INSERT dbo.TinNhan(CuocHoiThoaiId,NguoiGuiId,NoiDung,MaThaoTac) VALUES('60000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000002',N'Thầy/cô cho em xác nhận lịch học ạ.',NEWID());
INSERT dbo.LichHoc(Id,LopHocId,TieuDe,BatDauLuc,KetThucLuc,LaBuoiBu,TrangThai) VALUES('70000000-0000-0000-0000-000000000001','50000000-0000-0000-0000-000000000001',N'Buổi Toán mẫu','2026-10-27T18:00:00+07:00','2026-10-27T19:30:00+07:00',0,'DaLenLich');
INSERT dbo.BaiTapGiao(Id,LopHocId,NguoiGiaoId,TieuDe,NoiDung,HanNopLuc) VALUES('80000000-0000-0000-0000-000000000001','50000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000001',N'Bài tập mẫu',N'Giải phương trình x + 2 = 5.','2026-10-30T23:59:00+07:00');
INSERT dbo.KhungGioDeXuat(YeuCauKetNoiId,ThuTrongTuan,GioBatDau,GioKetThuc) VALUES('40000000-0000-0000-0000-000000000001',2,'18:00','19:30');
INSERT dbo.NhuCauHoc(Id,ChuSoHuuId,LoaiNhuCau,MonHocId,KhoiLop,MucTieu,SoBuoiTuan,HinhThucHoc,KhuVuc,HocPhiDuKien,TrangThai,DonViHocPhi) VALUES('30000000-0000-0000-0000-000000000002','10000000-0000-0000-0000-000000000001','NhanLop',1,N'Lớp 9',N'Nhận kèm Toán THCS',2,'TrucTuyen',N'Hà Nội',150000,'DangMo','VND/Gio');
COMMIT TRANSACTION;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;
GO
CREATE OR ALTER VIEW dbo.vw_GiaSuCongKhai AS
SELECT h.Id, h.GiaSuId, n.HoTen, h.TrinhDo, h.ChuyenMon, h.KinhNghiem, h.MoTa,
       h.KhuVuc, h.RowVersion
FROM dbo.HoSoGiaSu h JOIN dbo.GiaSu g ON g.TaiKhoanId=h.GiaSuId
JOIN dbo.NguoiThamGia n ON n.Id=g.TaiKhoanId
WHERE h.TrangThaiDuyet='DaDuyet';
GO
CREATE OR ALTER VIEW dbo.vw_NhuCauCongKhai AS
SELECT Id, ChuSoHuuId, LoaiNhuCau, MonHocId, KhoiLop, HocLuc, MucTieu, SoBuoiTuan,
       HinhThucHoc, KhuVuc, HocPhiDuKien, DonViHocPhi, RowVersion
FROM dbo.NhuCauHoc WHERE TrangThai='DangMo';
GO
CREATE OR ALTER VIEW dbo.vw_TienDoBaiTap AS
SELECT g.Id AS BaiTapGiaoId, g.LopHocId, g.TieuDe, g.HanNopLuc,
       n.Id AS BaiTapNopMoiNhatId, n.LanNop, n.NopLuc, k.Diem, k.NhanXet, k.ChamLuc
FROM dbo.BaiTapGiao g
OUTER APPLY (SELECT TOP(1) * FROM dbo.BaiTapNop x WHERE x.BaiTapGiaoId=g.Id ORDER BY x.LanNop DESC) n
LEFT JOIN dbo.KetQuaChamBai k ON k.BaiTapNopId=n.Id;
GO
CREATE OR ALTER VIEW dbo.vw_DanhSachBuoiHoc AS
SELECT l.Id AS LichHocId,l.LopHocId,l.TieuDe,l.BatDauLuc,l.KetThucLuc,b.Id AS BuoiHocId,b.KetQuaBuoiHoc,b.NhanXet,b.PhienBan,b.CapNhatLuc,
 CASE WHEN l.TrangThai='DaHuy' THEN 'DaHuy' WHEN b.Id IS NOT NULL THEN 'DaGhiNhan' WHEN l.KetThucLuc<=SYSDATETIMEOFFSET() THEN 'DaQuaGio' WHEN l.BatDauLuc<=SYSDATETIMEOFFSET() THEN 'DangDienRa' ELSE 'SapDienRa' END AS TrangThaiHienThi
FROM dbo.LichHoc l LEFT JOIN dbo.BuoiHoc b ON b.LichHocId=l.Id;
GO
CREATE OR ALTER TRIGGER dbo.tr_BaiTapGiao_CungLop ON dbo.BaiTapGiao AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.BuoiHoc b ON b.Id=i.BuoiHocId JOIN dbo.LichHoc l ON l.Id=b.LichHocId WHERE l.LopHocId<>i.LopHocId)
   THROW 51020,N'Buổi học của bài tập phải thuộc đúng lớp.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.tr_LichHoc_BuoiBu ON dbo.LichHoc AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.LichHoc g ON g.Id=i.LichGocId WHERE i.LopHocId<>g.LopHocId OR g.LaBuoiBu=1)
   THROW 51021,N'Buổi bù phải trỏ buổi gốc cùng lớp; không trỏ buổi bù khác.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.tr_NhuCau_MonDangKy ON dbo.NhuCauHoc AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i WHERE i.LoaiNhuCau='NhanLop' AND NOT EXISTS(SELECT 1 FROM dbo.HoSoGiaSu h JOIN dbo.HoSoGiaSuMonDay m ON m.HoSoGiaSuId=h.Id WHERE h.GiaSuId=i.ChuSoHuuId AND m.MonHocId=i.MonHocId))
 THROW 51110,N'Nhu cầu nhận lớp chỉ dùng môn đã đăng ký trong hồ sơ.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.tr_MonDay_GiuNhuCauMo ON dbo.HoSoGiaSuMonDay AFTER DELETE,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM deleted d JOIN dbo.HoSoGiaSu h ON h.Id=d.HoSoGiaSuId JOIN dbo.NhuCauHoc n ON n.ChuSoHuuId=h.GiaSuId AND n.MonHocId=d.MonHocId AND n.LoaiNhuCau='NhanLop' AND n.TrangThai='DangMo' WHERE NOT EXISTS(SELECT 1 FROM dbo.HoSoGiaSuMonDay m WHERE m.HoSoGiaSuId=d.HoSoGiaSuId AND m.MonHocId=d.MonHocId))
 THROW 51111,N'Đóng nhu cầu đang mở trước khi bỏ môn khỏi hồ sơ.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.tr_KhungGioRanh_KhongChongLan ON dbo.KhungGioRanh AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.KhungGioRanh a ON a.NhuCauId=i.NhuCauId AND a.ThuTrongTuan=i.ThuTrongTuan AND a.Id<>i.Id AND a.GioBatDau<i.GioKetThuc AND i.GioBatDau<a.GioKetThuc)
 THROW 51112,N'Khung giờ trong cùng ngày không được chồng lấn.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.tr_KhungGioDeXuat_KhongChongLan ON dbo.KhungGioDeXuat AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.KhungGioDeXuat a ON a.YeuCauKetNoiId=i.YeuCauKetNoiId AND a.ThuTrongTuan=i.ThuTrongTuan AND a.Id<>i.Id AND a.GioBatDau<i.GioKetThuc AND i.GioBatDau<a.GioKetThuc)
 THROW 51112,N'Khung giờ trong cùng ngày không được chồng lấn.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.tr_KetNoi_LienHeChapNhan ON dbo.YeuCauKetNoiLopHoc AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.YeuCauLienHe h ON h.Id=i.LienHeId WHERE h.TrangThai<>'ChapNhan' OR h.GiaSuId<>i.GiaSuId OR h.BenHocId<>i.BenHocId)
 THROW 51113,N'Kết nối phải có liên hệ đã chấp nhận của đúng hai bên.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.NhuCauHoc n ON n.Id=i.NhuCauId WHERE n.ChuSoHuuId<>i.BenHocId OR n.LoaiNhuCau<>'TimGiaSu')
 THROW 51114,N'Nhu cầu gắn yêu cầu kết nối phải thuộc bên học.',1;
 IF EXISTS(SELECT 1 FROM inserted i WHERE i.TrangThai='ChoGiaSu' AND (i.NguoiGuiId<>i.BenHocId OR i.NguoiDeXuatId IS NOT NULL))
 THROW 51115,N'Chờ gia sư là yêu cầu ban đầu của bên học, chưa có đề xuất.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.Id=i.Id WHERE d.TrangThai IN ('DaChapNhan','TuChoi','DaRut','HetHan'))
 THROW 51116,N'Yêu cầu đã đóng không được sửa hoặc chuyển trạng thái lại.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.tr_HoiThoai_LienHe ON dbo.CuocHoiThoai AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.YeuCauLienHe h ON h.Id=i.LienHeId WHERE i.LoaiHoiThoai='TruocKetNoi' AND (h.TrangThai<>'ChapNhan' OR h.GiaSuId<>i.GiaSuId OR h.BenHocId<>i.BenHocId))
 THROW 51117,N'Hội thoại trước kết nối cần liên hệ đã chấp nhận đúng hai bên.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.tr_BuoiHoc_SauGioKetThuc ON dbo.BuoiHoc AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.LichHoc l ON l.Id=i.LichHocId JOIN dbo.LopHoc c ON c.Id=l.LopHocId WHERE l.TrangThai='DaHuy' OR l.KetThucLuc>SYSDATETIMEOFFSET() OR i.GhiNhanLuc<l.KetThucLuc OR i.NguoiGhiNhanId<>c.GiaSuId)
 THROW 51118,N'Chỉ gia sư của lớp ghi nhận sau giờ kết thúc, không ghi nhận buổi đã hủy.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.Id=i.Id WHERE i.LichHocId<>d.LichHocId OR i.PhienBan<>d.PhienBan+1 OR i.CapNhatLuc<d.CapNhatLuc)
 THROW 51119,N'Không đổi lịch của ghi nhận; cập nhật phải tăng đúng một phiên bản.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.tr_LichHoc_GiuLichSu ON dbo.LichHoc AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.Id=i.Id WHERE d.BatDauLuc<=SYSDATETIMEOFFSET() OR EXISTS(SELECT 1 FROM dbo.BuoiHoc b WHERE b.LichHocId=d.Id))
 THROW 51120,N'Không điều chỉnh hoặc hủy lịch đã bắt đầu/đã ghi nhận.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.tr_BuoiHoc_LuuPhienBan ON dbo.BuoiHoc AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.LichSuGhiNhanBuoiHoc(BuoiHocId,PhienBan,TieuDe,KetQuaBuoiHoc,NoiDungGiangDay,NhanXet,TrangThaiThamDuGiaSu,TrangThaiThamDuBenHoc,NguoiGhiNhanId,GhiNhanLuc,NguoiSuaId,ThayDoiLuc)
 SELECT d.Id,d.PhienBan,d.TieuDe,d.KetQuaBuoiHoc,d.NoiDungGiangDay,d.NhanXet,d.TrangThaiThamDuGiaSu,d.TrangThaiThamDuBenHoc,d.NguoiGhiNhanId,d.GhiNhanLuc,i.NguoiGhiNhanId,i.CapNhatLuc FROM deleted d JOIN inserted i ON i.Id=d.Id;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_TimKiemGiaSu
 @TuKhoa nvarchar(256)=NULL,@MonHocId int=NULL,@KhuVuc nvarchar(256)=NULL,
 @HocPhiToiDa decimal(18,2)=NULL,@DonViHocPhi varchar(16)=NULL,
 @KhoiLop nvarchar(256)=NULL,@HinhThucHoc varchar(32)=NULL,
 @ThuTrongTuan tinyint=NULL,@GioBatDau time(0)=NULL,@GioKetThuc time(0)=NULL,
 @SapXep varchar(16)='MoiNhat',@Trang int=1,@KichThuocTrang int=20
AS
BEGIN
 SET NOCOUNT ON;
 IF @Trang<1 OR @KichThuocTrang NOT BETWEEN 1 AND 100 THROW 51100,N'Phân trang không hợp lệ.',1;
 IF @SapXep NOT IN ('MoiNhat','HocPhiTang','HocPhiGiam','Ten') THROW 51101,N'Kiểu sắp xếp không hợp lệ.',1;
 IF (@HocPhiToiDa IS NOT NULL OR @SapXep IN ('HocPhiTang','HocPhiGiam')) AND (@DonViHocPhi IS NULL OR @DonViHocPhi NOT IN ('VND/Gio','VND/Buoi')) THROW 51102,N'Lọc/sắp xếp học phí phải cùng đơn vị.',1;
 IF @HocPhiToiDa<0 OR @ThuTrongTuan>6 OR (@GioBatDau IS NOT NULL AND @GioKetThuc IS NOT NULL AND @GioBatDau>=@GioKetThuc) THROW 51103,N'Bộ lọc không hợp lệ.',1;
 ;WITH KetQua AS (
 SELECT v.*,n.Id AS NhuCauId,n.MonHocId,n.KhoiLop,n.MucTieu,n.HinhThucHoc,n.KhuVuc AS KhuVucNhanLop,n.HocPhiDuKien,n.DonViHocPhi,n.TaoLuc AS TaoNhuCauLuc,
 ROW_NUMBER() OVER(PARTITION BY v.Id ORDER BY CASE WHEN @SapXep='HocPhiTang' THEN n.HocPhiDuKien END ASC,CASE WHEN @SapXep='HocPhiGiam' THEN n.HocPhiDuKien END DESC,n.TaoLuc DESC,n.Id DESC) AS ViTri
 FROM dbo.vw_GiaSuCongKhai v JOIN dbo.NhuCauHoc n ON n.ChuSoHuuId=v.GiaSuId AND n.LoaiNhuCau='NhanLop' AND n.TrangThai='DangMo'
 JOIN dbo.HoSoGiaSuMonDay m ON m.HoSoGiaSuId=v.Id AND m.MonHocId=n.MonHocId JOIN dbo.MonHoc mh ON mh.Id=n.MonHocId
 WHERE (@TuKhoa IS NULL OR v.HoTen LIKE N'%'+@TuKhoa+N'%' OR v.ChuyenMon LIKE N'%'+@TuKhoa+N'%' OR mh.TenMon LIKE N'%'+@TuKhoa+N'%' OR n.MucTieu LIKE N'%'+@TuKhoa+N'%')
 AND (@MonHocId IS NULL OR n.MonHocId=@MonHocId) AND (@KhoiLop IS NULL OR n.KhoiLop=@KhoiLop)
 AND (@HinhThucHoc IS NULL OR n.HinhThucHoc=@HinhThucHoc) AND (@KhuVuc IS NULL OR n.KhuVuc LIKE N'%'+@KhuVuc+N'%')
 AND (@HocPhiToiDa IS NULL OR (n.HocPhiDuKien<=@HocPhiToiDa AND n.DonViHocPhi=@DonViHocPhi))
 AND (@SapXep NOT IN ('HocPhiTang','HocPhiGiam') OR (n.HocPhiDuKien IS NOT NULL AND n.DonViHocPhi=@DonViHocPhi))
 AND ((@ThuTrongTuan IS NULL AND @GioBatDau IS NULL AND @GioKetThuc IS NULL) OR EXISTS(SELECT 1 FROM dbo.KhungGioRanh k WHERE k.NhuCauId=n.Id AND (@ThuTrongTuan IS NULL OR k.ThuTrongTuan=@ThuTrongTuan) AND (@GioBatDau IS NULL OR k.GioBatDau<=@GioBatDau) AND (@GioKetThuc IS NULL OR k.GioKetThuc>=@GioKetThuc)))
 ) SELECT *,COUNT_BIG(*) OVER() AS TongKetQua FROM KetQua WHERE ViTri=1
 ORDER BY CASE WHEN @SapXep='Ten' THEN HoTen END,CASE WHEN @SapXep='HocPhiTang' THEN HocPhiDuKien END ASC,CASE WHEN @SapXep='HocPhiGiam' THEN HocPhiDuKien END DESC,TaoNhuCauLuc DESC,Id DESC
 OFFSET (CONVERT(bigint,@Trang)-1)*@KichThuocTrang ROWS FETCH NEXT @KichThuocTrang ROWS ONLY;
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
  -- Bản nghiệp vụ: chưa kiểm tra danh tính/quyền đăng nhập.
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
 @BaiTapNopId uniqueidentifier,@NguoiChamId uniqueidentifier,@Diem decimal(4,2)=NULL,
 @NhanXet nvarchar(2000)=N'',@RowVersionCu binary(8)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
  BEGIN TRANSACTION;
  DECLARE @Giao uniqueidentifier,@LopId uniqueidentifier,@Id uniqueidentifier;
  SELECT @Giao=BaiTapGiaoId FROM dbo.BaiTapNop WHERE Id=@BaiTapNopId;
  SELECT @LopId=LopHocId FROM dbo.BaiTapGiao WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Giao;
  IF @LopId IS NULL THROW 51035,N'Bài nộp không tồn tại.',1;
  -- Bản nghiệp vụ: danh tính người chấm do dữ liệu thử nghiệm cung cấp.
  IF @Diem IS NOT NULL AND @Diem NOT BETWEEN 0 AND 10 THROW 51036,N'Điểm bài tập từ 0 đến 10.',1;
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
CREATE OR ALTER PROCEDURE dbo.usp_GuiYeuCauKetNoi
 @LienHeId uniqueidentifier,@BenHocId uniqueidentifier,@LoiNhan nvarchar(256),@NhuCauId uniqueidentifier=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRAN;
 DECLARE @GiaSuId uniqueidentifier,@Id uniqueidentifier=NEWID(),@Now datetimeoffset(0)=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 SELECT @GiaSuId=GiaSuId FROM dbo.YeuCauLienHe WITH(UPDLOCK,HOLDLOCK) WHERE Id=@LienHeId AND BenHocId=@BenHocId AND TrangThai='ChapNhan';
 SET @Now=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 IF @GiaSuId IS NULL OR @LoiNhan IS NULL OR LEN(LTRIM(RTRIM(@LoiNhan)))=0 THROW 51130,N'Liên hệ chưa được chấp nhận hoặc lời nhắn trống.',1;
 IF @NhuCauId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.NhuCauHoc WHERE Id=@NhuCauId AND ChuSoHuuId=@BenHocId AND LoaiNhuCau='TimGiaSu' AND TrangThai='DangMo') THROW 51131,N'Nhu cầu học không hợp lệ.',1;
 IF EXISTS(SELECT 1 FROM dbo.YeuCauKetNoiLopHoc WHERE LienHeId=@LienHeId AND TrangThai IN ('ChoGiaSu','ChoBenHoc') AND HetHanLuc>@Now) THROW 51132,N'Đang có yêu cầu kết nối chờ phản hồi.',1;
 INSERT dbo.YeuCauKetNoiLopHoc(Id,GiaSuId,BenHocId,NguoiGuiId,NhuCauId,LoiNhan,TrangThai,GuiLuc,HetHanLuc,LienHeId)
 VALUES(@Id,@GiaSuId,@BenHocId,@BenHocId,@NhuCauId,@LoiNhan,'ChoGiaSu',@Now,DATEADD(hour,72,@Now),@LienHeId);
 COMMIT;SELECT * FROM dbo.YeuCauKetNoiLopHoc WHERE Id=@Id;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK;THROW;END CATCH;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_GuiDeXuatLop
 @LienHeId uniqueidentifier,@GiaSuId uniqueidentifier,@TenLop nvarchar(256),@MonHocId int,@MucTieu nvarchar(256),
 @SoBuoiTuan int,@ThoiLuongPhut int,@BatDauHocLuc datetimeoffset(0),@HinhThucHoc varchar(32),
 @HocPhi decimal(18,2),@DonViHocPhi varchar(16),@KhungGioJson nvarchar(max),
 @DiaDiem nvarchar(256)=NULL,@NhuCauId uniqueidentifier=NULL,@YeuCauId uniqueidentifier=NULL,@RowVersionCu binary(8)=NULL
AS
BEGIN
 SET NOCOUNT ON;SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRAN;
 DECLARE @BenHocId uniqueidentifier,@Now datetimeoffset(0)=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 SELECT @BenHocId=BenHocId FROM dbo.YeuCauLienHe WITH(UPDLOCK,HOLDLOCK) WHERE Id=@LienHeId AND GiaSuId=@GiaSuId AND TrangThai='ChapNhan';
 SET @Now=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
 IF @BenHocId IS NULL THROW 51133,N'Chỉ gia sư của liên hệ đã chấp nhận được đề xuất lớp.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.HoSoGiaSu h JOIN dbo.HoSoGiaSuMonDay m ON m.HoSoGiaSuId=h.Id WHERE h.GiaSuId=@GiaSuId AND m.MonHocId=@MonHocId) THROW 51134,N'Môn học chưa đăng ký trong hồ sơ gia sư.',1;
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
  VALUES(@YeuCauId,@GiaSuId,@BenHocId,@GiaSuId,@NhuCauId,'ChoBenHoc',@Now,DATEADD(hour,72,@Now),@TenLop,@MonHocId,@MucTieu,@SoBuoiTuan,@ThoiLuongPhut,@BatDauHocLuc,@HinhThucHoc,@DiaDiem,@HocPhi,@DonViHocPhi,@LienHeId,@GiaSuId,@Now);
 END
 ELSE
 BEGIN
  DECLARE @LockedId uniqueidentifier;
  SELECT @LockedId=Id FROM dbo.YeuCauKetNoiLopHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@YeuCauId;
  SET @Now=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00');
  UPDATE dbo.YeuCauKetNoiLopHoc SET TrangThai='ChoBenHoc',TenLopDuKien=@TenLop,MonHocId=@MonHocId,MucTieu=@MucTieu,SoBuoiTuan=@SoBuoiTuan,ThoiLuongPhut=@ThoiLuongPhut,BatDauHocLuc=@BatDauHocLuc,HinhThucHoc=@HinhThucHoc,DiaDiem=@DiaDiem,HocPhi=@HocPhi,DonViHocPhi=@DonViHocPhi,NguoiDeXuatId=@GiaSuId,DeXuatLuc=@Now,HetHanLuc=DATEADD(hour,72,@Now)
  WHERE Id=@YeuCauId AND LienHeId=@LienHeId AND GiaSuId=@GiaSuId AND TrangThai IN ('ChoGiaSu','ChoBenHoc') AND HetHanLuc>@Now AND RowVersion=@RowVersionCu;
  IF @@ROWCOUNT<>1 THROW 51139,N'Yêu cầu đã đổi phiên bản, hết hạn hoặc không thuộc gia sư.',1;
 END;
 DELETE dbo.KhungGioDeXuat WHERE YeuCauKetNoiId=@YeuCauId;
 INSERT dbo.KhungGioDeXuat(YeuCauKetNoiId,ThuTrongTuan,GioBatDau,GioKetThuc) SELECT @YeuCauId,Thu,BatDau,KetThuc FROM @Slots;
 COMMIT;SELECT * FROM dbo.YeuCauKetNoiLopHoc WHERE Id=@YeuCauId;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK;THROW;END CATCH;
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
 IF NOT EXISTS(SELECT 1 FROM @Rows) OR EXISTS(SELECT 1 FROM @Rows WHERE BatDau>=KetThuc OR BatDau<=SYSDATETIMEOFFSET() OR LEN(LTRIM(RTRIM(TieuDe)))=0) THROW 51143,N'Ngày giờ đã qua hoặc khoảng giờ không hợp lệ.',1;
 IF EXISTS(SELECT 1 FROM @Rows a JOIN @Rows b ON a.Stt<b.Stt AND a.BatDau<b.KetThuc AND b.BatDau<a.KetThuc) THROW 51144,N'Danh sách chọn có các buổi trùng giờ.',1;
 IF EXISTS(SELECT 1 FROM @Rows r JOIN dbo.LichHoc l ON l.BatDauLuc<r.KetThuc AND r.BatDau<l.KetThucLuc JOIN dbo.LopHoc c ON c.Id=l.LopHocId WHERE l.TrangThai<>'DaHuy' AND c.TrangThai='DangHoatDong' AND (c.Id=@LopHocId OR c.GiaSuId=@GiaSuId OR c.BenHocId=@BenHocId)) THROW 51145,N'Lịch trùng lớp hiện tại hoặc lịch của gia sư/bên học.',1;
 INSERT dbo.LichHoc(LopHocId,TieuDe,BatDauLuc,KetThucLuc,LaBuoiBu,TrangThai) SELECT @LopHocId,TieuDe,BatDau,KetThuc,0,'DaLenLich' FROM @Rows;
 COMMIT;SELECT COUNT(*) AS SoBuoiDaTao FROM @Rows;
 END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK;THROW;END CATCH;
END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_GhiNhanBuoiHoc
 @LichHocId uniqueidentifier,@GiaSuId uniqueidentifier,@KetQuaBuoiHoc varchar(32),@NhanXet nvarchar(2000),
 @TrangThaiGiaSu varchar(32),@TrangThaiBenHoc varchar(32),@NoiDung nvarchar(max)=NULL,@RowVersionCu binary(8)=NULL
AS
BEGIN
 SET NOCOUNT ON;SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRAN;
 DECLARE @LopId uniqueidentifier,@Id uniqueidentifier,@Now datetimeoffset(0)=TODATETIMEOFFSET(SYSUTCDATETIME(),'+00:00'),@Title nvarchar(256);
 SELECT @LopId=LopHocId FROM dbo.LichHoc WHERE Id=@LichHocId;
 IF NOT EXISTS(SELECT 1 FROM dbo.LopHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@LopId AND GiaSuId=@GiaSuId) THROW 51146,N'Chỉ gia sư phụ trách được ghi nhận.',1;
 SELECT @Title=TieuDe FROM dbo.LichHoc WITH(UPDLOCK,HOLDLOCK) WHERE Id=@LichHocId AND TrangThai<>'DaHuy' AND KetThucLuc<=@Now;
 IF @Title IS NULL THROW 51147,N'Buổi đã hủy hoặc chưa qua giờ kết thúc.',1;
 IF @NhanXet IS NULL OR LEN(LTRIM(RTRIM(@NhanXet)))=0 THROW 51148,N'Cần nhận xét hoặc lý do không diễn ra.',1;
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
UPDATE m SET MucQuyen='ChinhSua' FROM dbo.ThanhVienLopHoc m JOIN dbo.LopHoc c ON c.Id=m.LopHocId AND c.BenHocId=m.TaiKhoanId WHERE m.TrangThaiDuyet='DaDuyet' AND m.RoiLopLuc IS NULL AND m.MucQuyen='Xem';
INSERT dbo.ThanhVienLopHoc(LopHocId,TaiKhoanId,NguoiDeNghiId,TrangThaiDuyet,MucQuyen,DuyetLuc) SELECT c.Id,p.Id,c.GiaSuId,'DaDuyet','ChinhSua',SYSDATETIMEOFFSET() FROM dbo.LopHoc c CROSS APPLY(VALUES(c.GiaSuId),(c.BenHocId)) p(Id) WHERE NOT EXISTS(SELECT 1 FROM dbo.ThanhVienLopHoc m WHERE m.LopHocId=c.Id AND m.TaiKhoanId=p.Id AND m.RoiLopLuc IS NULL AND m.TrangThaiDuyet<>'TuChoi');


GO
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET XACT_ABORT ON;
BEGIN TRAN;
INSERT dbo.CuocHoiThoai(LoaiHoiThoai,GiaSuId,BenHocId,LienHeId)
SELECT 'TruocKetNoi',h.GiaSuId,h.BenHocId,h.Id FROM dbo.YeuCauLienHe h WHERE h.TrangThai='ChapNhan' AND NOT EXISTS(SELECT 1 FROM dbo.CuocHoiThoai c WHERE c.LienHeId=h.Id AND c.LoaiHoiThoai='TruocKetNoi');
IF NOT EXISTS(SELECT 1 FROM dbo.KhungGioRanh WHERE NhuCauId='30000000-0000-0000-0000-000000000002')
INSERT dbo.KhungGioRanh(NhuCauId,ThuTrongTuan,GioBatDau,GioKetThuc) VALUES('30000000-0000-0000-0000-000000000002',2,'18:00','20:00'),('30000000-0000-0000-0000-000000000002',4,'18:00','20:00');
IF NOT EXISTS(SELECT 1 FROM dbo.NhuCauHoc WHERE Id='30000000-0000-0000-0000-000000000003')
BEGIN
INSERT dbo.NhuCauHoc(Id,ChuSoHuuId,LoaiNhuCau,MonHocId,KhoiLop,MucTieu,SoBuoiTuan,HinhThucHoc,HocPhiDuKien,DonViHocPhi,TenNguoiHoc,TuoiNguoiHoc,TrangThai)
VALUES('30000000-0000-0000-0000-000000000003','10000000-0000-0000-0000-000000000003','TimGiaSu',1,N'Lớp 8',N'Củng cố nền tảng Toán và luyện thói quen tự học',2,'TrucTuyen',150000,'VND/Gio',N'Người học của phụ huynh mẫu',13,'DangMo');
INSERT dbo.KhungGioRanh(NhuCauId,ThuTrongTuan,GioBatDau,GioKetThuc) VALUES('30000000-0000-0000-0000-000000000003',1,'19:00','20:30'),('30000000-0000-0000-0000-000000000003',3,'19:00','20:30');
END;
COMMIT;
GO

INSERT dbo.CuocHoiThoai(LoaiHoiThoai,GiaSuId,BenHocId,LopHocId,LienHeId,HoiThoaiTruocKetNoiId)
SELECT 'TheoLop',c.GiaSuId,c.BenHocId,c.Id,r.LienHeId,p.Id
FROM dbo.LopHoc c JOIN dbo.YeuCauKetNoiLopHoc r ON r.Id=c.YeuCauKetNoiId
LEFT JOIN dbo.CuocHoiThoai p ON p.LienHeId=r.LienHeId AND p.LoaiHoiThoai='TruocKetNoi'
WHERE NOT EXISTS(SELECT 1 FROM dbo.CuocHoiThoai x WHERE x.LopHocId=c.Id AND x.LoaiHoiThoai='TheoLop');
GO
SELECT DB_NAME() AS DatabaseDaTao,COUNT(*) AS SoBang FROM sys.tables WHERE is_ms_shipped=0;
GO
SET NOEXEC OFF;
GO
/* PROFILE_WORKFLOW_V1 */
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
