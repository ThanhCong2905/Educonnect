SET NOCOUNT ON;
SELECT DB_NAME() DatabaseName,COUNT(*) TableCount FROM sys.tables;
SELECT h.Id,h.GiaSuId,d.MonHocId,m.TenMon,COUNT(DISTINCT CASE WHEN n.Id IS NOT NULL THEN CONCAT(n.HocPhiDuKien,'|',n.DonViHocPhi) END) FeeVariants
FROM dbo.HoSoGiaSu h JOIN dbo.HoSoGiaSuMonDay d ON d.HoSoGiaSuId=h.Id JOIN dbo.MonHoc m ON m.Id=d.MonHocId
LEFT JOIN dbo.NhuCauHoc n ON n.ChuSoHuuId=h.GiaSuId AND n.MonHocId=d.MonHocId AND n.LoaiNhuCau='NhanLop'
GROUP BY h.Id,h.GiaSuId,d.MonHocId,m.TenMon;
SELECT COUNT(*) LegacyTutorNeeds FROM dbo.NhuCauHoc WHERE LoaiNhuCau='NhanLop';
SELECT COUNT(*) ReferencedLegacyNeeds FROM dbo.YeuCauKetNoiLopHoc r JOIN dbo.NhuCauHoc n ON n.Id=r.NhuCauId WHERE n.LoaiNhuCau='NhanLop';
SELECT COUNT(*) Classes FROM dbo.LopHoc;

