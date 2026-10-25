SET NOCOUNT ON;
IF DB_NAME() NOT LIKE 'EduConnect_Verification[_]%' THROW 51000,'Test database required.',1;
INSERT dbo.CuocHoiThoai(LoaiHoiThoai,GiaSuId,BenHocId,LienHeId)
SELECT 'TruocKetNoi',h.GiaSuId,h.BenHocId,h.Id FROM dbo.YeuCauLienHe h
WHERE h.TrangThai='ChapNhan' AND NOT EXISTS(SELECT 1 FROM dbo.CuocHoiThoai c WHERE c.LienHeId=h.Id AND c.LoaiHoiThoai='TruocKetNoi');
