
USE [EduConnect];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Now datetimeoffset(0) = SYSDATETIMEOFFSET();
DECLARE @StartDate date = DATEADD(DAY, 7, CONVERT(date, @Now));

BEGIN TRY
    BEGIN TRANSACTION;

    ------------------------------------------------------------
    -- 1. CHUẨN BỊ 10 CẶP GIA SƯ - BÊN HỌC
    ------------------------------------------------------------

    IF OBJECT_ID('tempdb..#Seed') IS NOT NULL DROP TABLE #Seed;
    IF OBJECT_ID('tempdb..#Slots') IS NOT NULL DROP TABLE #Slots;

    ;WITH Candidate AS
    (
        SELECT
            N.Id AS NhuCauId,
            N.ChuSoHuuId AS BenHocId,
            HS.GiaSuId,
            N.MonHocId,
            MH.TenMon,
            N.MucTieu,
            N.SoBuoiTuan,
            N.HinhThucHoc,
            N.KhuVuc,
            N.DiaChiChiTiet,
            N.HocPhiDuKien,
            N.DonViHocPhi,
            N.TenNguoiHoc,
            N.TuoiNguoiHoc,
            N.TaoLuc,

            ROW_NUMBER() OVER
            (
                PARTITION BY N.ChuSoHuuId
                ORDER BY N.TaoLuc DESC, N.Id
            ) AS RN

        FROM dbo.NhuCauHoc N

        JOIN dbo.MonHoc MH
            ON MH.Id = N.MonHocId

        CROSS APPLY
        (
            SELECT TOP (1)
                H.GiaSuId
            FROM dbo.HoSoGiaSu H
            JOIN dbo.GiaSu G
                ON G.TaiKhoanId = H.GiaSuId
            JOIN dbo.HoSoGiaSuMonDay HD
                ON HD.HoSoGiaSuId = H.Id
            WHERE H.TrangThaiDuyet = 'DaDuyet'
              AND HD.MonHocId = N.MonHocId
            ORDER BY CHECKSUM(N.Id, H.GiaSuId)
        ) HS

        WHERE N.LoaiNhuCau = 'TimGiaSu'
          AND N.TrangThai = 'DangMo'

          AND (
              EXISTS (
                  SELECT 1 FROM dbo.HocSinh HSIN
                  WHERE HSIN.TaiKhoanId = N.ChuSoHuuId
              )
              OR EXISTS (
                  SELECT 1 FROM dbo.PhuHuynh PH
                  WHERE PH.TaiKhoanId = N.ChuSoHuuId
              )
          )

          AND N.TenNguoiHoc IS NOT NULL
          AND LEN(LTRIM(RTRIM(N.TenNguoiHoc))) > 0
          AND N.TuoiNguoiHoc BETWEEN 1 AND 120
          AND N.SoBuoiTuan BETWEEN 1 AND 7
          AND N.HinhThucHoc IN ('TrucTiep', 'TrucTuyen')
          AND N.HocPhiDuKien IS NOT NULL
          AND N.HocPhiDuKien >= 0
          AND (
              N.DonViHocPhi IS NULL
              OR N.DonViHocPhi IN ('VND/Gio', 'VND/Buoi')
          )

          -- Không tạo lại cặp liên hệ đã tồn tại
          AND NOT EXISTS
          (
              SELECT 1
              FROM dbo.YeuCauLienHe YC
              WHERE YC.GiaSuId = HS.GiaSuId
                AND YC.BenHocId = N.ChuSoHuuId
          )

          -- Không tạo lại cặp kết nối đã tồn tại
          AND NOT EXISTS
          (
              SELECT 1
              FROM dbo.YeuCauKetNoiLopHoc YC
              WHERE YC.GiaSuId = HS.GiaSuId
                AND YC.BenHocId = N.ChuSoHuuId
          )
    )
    SELECT TOP (10)
        ROW_NUMBER() OVER (ORDER BY C.TaoLuc, C.NhuCauId) AS Stt,

        NEWID() AS LienHeId,
        NEWID() AS YeuCauId,
        NEWID() AS LopHocId,

        C.NhuCauId,
        C.GiaSuId,
        C.BenHocId,
        C.MonHocId,
        C.TenMon,

        COALESCE(
            NULLIF(C.MucTieu, N''),
            N'Củng cố kiến thức và nâng cao kết quả học tập'
        ) AS MucTieu,

        C.SoBuoiTuan,
        C.HinhThucHoc,

        CASE
            WHEN C.HinhThucHoc = 'TrucTiep'
            THEN LEFT(
                COALESCE(
                    NULLIF(C.DiaChiChiTiet, N''),
                    NULLIF(C.KhuVuc, N''),
                    N'Theo thỏa thuận'
                ), 256
            )
            ELSE NULL
        END AS DiaDiem,

        C.HocPhiDuKien AS HocPhi,
        COALESCE(C.DonViHocPhi, 'VND/Buoi') AS DonViHocPhi,
        C.TenNguoiHoc,
        C.TuoiNguoiHoc,

        LEFT(
            N'Lớp ' + C.TenMon + N' - ' + C.TenNguoiHoc,
            256
        ) AS TenLop

    INTO #Seed
    FROM Candidate C
    WHERE C.RN = 1
    ORDER BY C.TaoLuc, C.NhuCauId;

    IF (SELECT COUNT(*) FROM #Seed) < 10
        THROW 52001,
            N'Không tìm đủ 10 cặp gia sư và nhu cầu học phù hợp. Không có dữ liệu nào được lưu.',
            1;


    ------------------------------------------------------------
    -- 2. INSERT 10 YÊU CẦU LIÊN HỆ ĐÃ CHẤP NHẬN
    ------------------------------------------------------------

    INSERT INTO dbo.YeuCauLienHe
    (
        Id,
        GiaSuId,
        BenHocId,
        NguoiGuiId,
        LoiNhan,
        TrangThai,
        GuiLuc,
        NguoiPhanHoiId,
        PhanHoiLuc
    )
    SELECT
        LienHeId,
        GiaSuId,
        BenHocId,
        BenHocId,
        N'Em muốn được tư vấn và học môn ' + TenMon
            + N' theo nhu cầu học đã đăng.',
        'ChapNhan',
        DATEADD(MINUTE, -10, @Now),
        GiaSuId,
        @Now
    FROM #Seed;


    ------------------------------------------------------------
    -- 3. INSERT 8 YÊU CẦU KẾT NỐI ĐÃ CHẤP NHẬN
    ------------------------------------------------------------

    INSERT INTO dbo.YeuCauKetNoiLopHoc
    (
        Id,
        GiaSuId,
        BenHocId,
        NguoiGuiId,
        NhuCauId,
        LoiNhan,
        TrangThai,
        GuiLuc,
        HetHanLuc,
        TenLopDuKien,
        MonHocId,
        MucTieu,
        SoBuoiTuan,
        ThoiLuongPhut,
        BatDauHocLuc,
        HinhThucHoc,
        DiaDiem,
        HocPhi,
        DonViHocPhi,
        LienHeId,
        NguoiDeXuatId,
        DeXuatLuc
    )
    SELECT
        YeuCauId,
        GiaSuId,
        BenHocId,
        GiaSuId,
        NhuCauId,
        N'Gia sư đề xuất tạo lớp học môn ' + TenMon,
        'DaChapNhan',
        @Now,
        DATEADD(HOUR, 72, @Now),
        TenLop,
        MonHocId,
        MucTieu,
        SoBuoiTuan,
        60,
        DATEADD(DAY, 7, @Now),
        HinhThucHoc,
        DiaDiem,
        HocPhi,
        DonViHocPhi,
        LienHeId,
        GiaSuId,
        @Now
    FROM #Seed
    WHERE Stt <= 8;


    ------------------------------------------------------------
    -- 4. INSERT 8 LỚP HỌC
    ------------------------------------------------------------

    INSERT INTO dbo.LopHoc
    (
        Id,
        YeuCauKetNoiId,
        GiaSuId,
        BenHocId,
        TenLop,
        MonHocId,
        MucTieu,
        SoBuoiTuan,
        ThoiLuongPhut,
        HinhThucHoc,
        DiaDiem,
        HocPhi,
        TrangThai,
        GiaSuXacNhanKetThucLuc,
        BenHocXacNhanKetThucLuc,
        DonViHocPhi,
        TenNguoiHoc,
        TuoiNguoiHoc,
        KetThucLuc,
        CachKetThuc
    )
    SELECT
        LopHocId,
        YeuCauId,
        GiaSuId,
        BenHocId,
        TenLop,
        MonHocId,
        MucTieu,
        SoBuoiTuan,
        60,
        HinhThucHoc,
        DiaDiem,
        HocPhi,
        'DangHoatDong',
        NULL,
        NULL,
        DonViHocPhi,
        TenNguoiHoc,
        TuoiNguoiHoc,
        NULL,
        NULL
    FROM #Seed
    WHERE Stt <= 8;


    ------------------------------------------------------------
    -- 5. INSERT THÀNH VIÊN CHO 8 LỚP
    -- Mỗi lớp gồm 1 gia sư và 1 bên học
    ------------------------------------------------------------

    INSERT INTO dbo.ThanhVienLopHoc
    (
        Id,
        LopHocId,
        TaiKhoanId,
        NguoiDeNghiId,
        TrangThaiDuyet,
        MucQuyen,
        DeNghiLuc,
        DuyetLuc,
        DongYThamGiaLuc,
        RoiLopLuc
    )
    SELECT
        NEWID(),
        P.LopHocId,
        P.GiaSuId,
        P.GiaSuId,
        'DaDuyet',
        'ChinhSua',
        @Now,
        @Now,
        @Now,
        NULL
    FROM #Seed P
    WHERE P.Stt <= 8

    UNION ALL

    SELECT
        NEWID(),
        P.LopHocId,
        P.BenHocId,
        P.GiaSuId,
        'DaDuyet',
        'Xem',
        @Now,
        @Now,
        @Now,
        NULL
    FROM #Seed P
    WHERE P.Stt <= 8;


    ------------------------------------------------------------
    -- 6. TẠO CÁC KHUNG GIỜ ĐỀ XUẤT
    ------------------------------------------------------------

    SELECT
        P.Stt,
        P.YeuCauId,
        P.LopHocId,
        CONVERT(tinyint, (2 + (V.N - 1) * 2) % 7) AS ThuTrongTuan,

        CONVERT(
            time(0),
            DATEADD(
                HOUR,
                17 + (P.Stt % 4),
                CONVERT(datetime2, '19000101', 112)
            )
        ) AS GioBatDau,

        CONVERT(
            time(0),
            DATEADD(
                HOUR,
                18 + (P.Stt % 4),
                CONVERT(datetime2, '19000101', 112)
            )
        ) AS GioKetThuc,

        P.TenLop

    INTO #Slots

    FROM #Seed P

    CROSS APPLY
    (
        SELECT TOP (P.SoBuoiTuan)
            ROW_NUMBER() OVER (ORDER BY object_id) AS N
        FROM sys.all_objects
    ) V

    WHERE P.Stt <= 8;


    INSERT INTO dbo.KhungGioDeXuat
    (
        Id,
        YeuCauKetNoiId,
        ThuTrongTuan,
        GioBatDau,
        GioKetThuc
    )
    SELECT
        NEWID(),
        YeuCauId,
        ThuTrongTuan,
        GioBatDau,
        GioKetThuc
    FROM #Slots;


    ------------------------------------------------------------
    -- 7. INSERT LỊCH HỌC CHO 8 LỚP
    ------------------------------------------------------------

    INSERT INTO dbo.LichHoc
    (
        Id,
        LopHocId,
        TieuDe,
        BatDauLuc,
        KetThucLuc,
        LaBuoiBu,
        TrangThai,
        LichGocId
    )
    SELECT
        NEWID(),
        S.LopHocId,
        S.TenLop,

        TODATETIMEOFFSET(
            DATEADD(
                SECOND,
                DATEDIFF(SECOND, CONVERT(time(0), '00:00'), S.GioBatDau),
                CONVERT(datetime2,
                    DATEADD(
                        DAY,
                        (
                            S.ThuTrongTuan
                            - (
                                (DATEDIFF(DAY, '19000107', @StartDate) % 7 + 7) % 7
                            )
                            + 7
                        ) % 7,
                        @StartDate
                    )
                )
            ),
            '+07:00'
        ),

        TODATETIMEOFFSET(
            DATEADD(
                SECOND,
                DATEDIFF(SECOND, CONVERT(time(0), '00:00'), S.GioKetThuc),
                CONVERT(datetime2,
                    DATEADD(
                        DAY,
                        (
                            S.ThuTrongTuan
                            - (
                                (DATEDIFF(DAY, '19000107', @StartDate) % 7 + 7) % 7
                            )
                            + 7
                        ) % 7,
                        @StartDate
                    )
                )
            ),
            '+07:00'
        ),

        0,
        'DaLenLich',
        NULL

    FROM #Slots S;


    ------------------------------------------------------------
    -- 8. INSERT 8 PHIẾU ĐÁNH GIÁ
    ------------------------------------------------------------

    INSERT INTO dbo.PhieuDanhGia
    (
        Id,
        LopHocId,
        NguoiGuiId,
        MocDanhGia,
        NoiDung,
        MucDoHaiLong,
        GuiLuc,
        QuyetDinhDeXuat
    )
    SELECT
        NEWID(),
        P.LopHocId,
        P.BenHocId,
        N'Đánh giá sau khi kết nối lớp',
        CASE
            WHEN P.Stt % 3 = 0
                THEN N'Gia sư giảng dạy nhiệt tình, nội dung dễ hiểu và có sự theo sát tiến độ học tập.'
            WHEN P.Stt % 3 = 1
                THEN N'Gia sư có chuyên môn tốt, giải thích rõ ràng và hỗ trợ người học trong quá trình học.'
            ELSE
                N'Quá trình học diễn ra thuận lợi, gia sư đúng giờ và nội dung giảng dạy phù hợp mục tiêu.'
        END,
        CASE WHEN P.Stt % 3 = 0 THEN 4 ELSE 5 END,
        @Now,
        'TiepTuc'
    FROM #Seed P
    WHERE P.Stt <= 8;


    ------------------------------------------------------------
    -- 9. KIỂM TRA SỐ LƯỢNG DỮ LIỆU ĐÃ INSERT
    ------------------------------------------------------------

    SELECT
        (SELECT COUNT(*)
         FROM dbo.YeuCauLienHe
         WHERE Id IN (SELECT LienHeId FROM #Seed)
           AND TrangThai = 'ChapNhan') AS SoYeuCauLienHe,

        (SELECT COUNT(*)
         FROM dbo.YeuCauKetNoiLopHoc
         WHERE Id IN (SELECT YeuCauId FROM #Seed WHERE Stt <= 8)
           AND TrangThai = 'DaChapNhan') AS SoYeuCauKetNoi,

        (SELECT COUNT(*)
         FROM dbo.LopHoc
         WHERE Id IN (SELECT LopHocId FROM #Seed WHERE Stt <= 8))
            AS SoLopHoc,

        (SELECT COUNT(*)
         FROM dbo.ThanhVienLopHoc
         WHERE LopHocId IN (SELECT LopHocId FROM #Seed WHERE Stt <= 8))
            AS SoThanhVien,

        (SELECT COUNT(*)
         FROM dbo.KhungGioDeXuat
         WHERE YeuCauKetNoiId IN (SELECT YeuCauId FROM #Seed WHERE Stt <= 8))
            AS SoKhungGio,

        (SELECT COUNT(*)
         FROM dbo.LichHoc
         WHERE LopHocId IN (SELECT LopHocId FROM #Seed WHERE Stt <= 8))
            AS SoLichHoc,

        (SELECT COUNT(*)
         FROM dbo.PhieuDanhGia
         WHERE LopHocId IN (SELECT LopHocId FROM #Seed WHERE Stt <= 8))
            AS SoPhieuDanhGia;


    -- Hiển thị chi tiết các dữ liệu vừa tạo
    SELECT
        P.Stt,
        GS.HoTen AS GiaSu,
        BH.HoTen AS BenHoc,
        P.TenMon,
        P.LienHeId,
        P.YeuCauId,
        P.LopHocId,
        P.TenLop
    FROM #Seed P
    JOIN dbo.AspNetUsers GS ON GS.Id = P.GiaSuId
    JOIN dbo.AspNetUsers BH ON BH.Id = P.BenHocId
    ORDER BY P.Stt;


    COMMIT TRANSACTION;
    PRINT N'Đã INSERT dữ liệu thành công.';

END TRY
BEGIN CATCH

    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;

END CATCH;
GO

INSERT INTO dbo.DanhGiaDoiTac
(
    Id,
    LopHocId,
    NguoiGuiId,
    NguoiNhanId,
    SoSao,
    NhanXet,
    GuiLuc,
    PhanHoi,
    PhanHoiLuc
)
SELECT
    NEWID(),
    lh.Id,
    lh.BenHocId,
    lh.GiaSuId,
    dg.SoSao,
    dg.NhanXet,
    DATEADD(DAY, -dg.STT, SYSDATETIMEOFFSET()),
    NULL,
    NULL
FROM
(
    SELECT
        Id,
        BenHocId,
        GiaSuId,
        ROW_NUMBER() OVER (ORDER BY Id) AS STT
    FROM dbo.LopHoc
) AS lh
CROSS APPLY
(
    SELECT
        CASE lh.STT
            WHEN 1 THEN 5
            WHEN 2 THEN 5
            WHEN 3 THEN 4
            WHEN 4 THEN 5
            WHEN 5 THEN 4
            WHEN 6 THEN 5
            WHEN 7 THEN 4
            ELSE 5
        END AS SoSao,
        CASE lh.STT
            WHEN 1 THEN N'Gia sư giảng dạy nhiệt tình, dễ hiểu và đúng giờ.'
            WHEN 2 THEN N'Gia sư có chuyên môn tốt, hướng dẫn rất tận tâm.'
            WHEN 3 THEN N'Phương pháp giảng dạy phù hợp, cần cải thiện thêm tốc độ.'
            WHEN 4 THEN N'Gia sư chuẩn bị bài đầy đủ và hỗ trợ học sinh tốt.'
            WHEN 5 THEN N'Nội dung giảng dạy rõ ràng, giúp học sinh tiến bộ.'
            WHEN 6 THEN N'Gia sư nhiệt tình, giải đáp thắc mắc chi tiết.'
            WHEN 7 THEN N'Buổi học hiệu quả, gia sư có trách nhiệm.'
            ELSE N'Gia sư dạy tốt, thái độ thân thiện và chuyên nghiệp.'
        END AS NhanXet,
        lh.STT
) AS dg
WHERE lh.STT <= 8
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.DanhGiaDoiTac d
      WHERE d.LopHocId = lh.Id
        AND d.NguoiGuiId = lh.BenHocId
        AND d.NguoiNhanId = lh.GiaSuId
  );


  SELECT
    d.Id,
    d.LopHocId,
    d.NguoiGuiId,
    d.NguoiNhanId,
    d.SoSao,
    d.NhanXet,
    d.GuiLuc,
    d.PhanHoi,
    d.PhanHoiLuc
FROM dbo.DanhGiaDoiTac d
ORDER BY d.GuiLuc DESC;
