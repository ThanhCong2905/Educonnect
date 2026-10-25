SET XACT_ABORT ON;
BEGIN TRAN;
UPDATE dbo.NhuCauHoc SET KhoiLop=N'Lớp 8' WHERE KhoiLop=N'Lá»›p 8';
UPDATE dbo.NhuCauHoc SET MucTieu=N'Củng cố nền tảng Toán và luyện thói quen tự học' WHERE MucTieu=N'Cá»§ng cá»‘ ná»n táº£ng ToÃ¡n vÃ  luyá»‡n thÃ³i quen tá»± há»c';
UPDATE dbo.NhuCauHoc SET TenNguoiHoc=N'Người học của phụ huynh mẫu' WHERE TenNguoiHoc=N'NgÆ°á»i há»c cá»§a phá»¥ huynh máº«u';
COMMIT;
