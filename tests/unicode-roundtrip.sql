SET NOCOUNT ON;
SET XACT_ABORT ON;
DECLARE @expected nvarchar(1000)=CONVERT(nvarchar(1000),0x54006900bf1e6e006700200056006900c71e74003a0020004e00670075007900c51e6e00200054006800cb1e2000c1006e00680020001420200054006f00e1006e002c0020005600ad1e740020006c00fd002c0020004800f300610020006800cd1e63002c0020001001cb1e610020006c00fd002c00200003012000e2002000ea002000f4002000a1012000b001200011012000ae1e2000c41e2000e21e20003dd84ddc);
DECLARE @fromFile nvarchar(1000)=N'Tiếng Việt: Nguyễn Thị Ánh — Toán, Vật lý, Hóa học, Địa lý, ă â ê ô ơ ư đ Ắ Ễ Ợ 👍';
IF CONVERT(varbinary(max),@fromFile)<>CONVERT(varbinary(max),@expected) THROW 51000,'SQL input was not decoded as UTF-8.',1;
CREATE TABLE #UnicodeCheck(Value nvarchar(1000));
INSERT #UnicodeCheck(Value) VALUES(@fromFile);
IF NOT EXISTS(SELECT 1 FROM #UnicodeCheck WHERE CONVERT(varbinary(max),Value)=CONVERT(varbinary(max),@expected)) THROW 51001,'Unicode SQL round trip failed.',1;
SELECT N'PASS: SQL file and Unicode storage preserve every character.' AS Result;
