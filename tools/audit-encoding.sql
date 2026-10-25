SET NOCOUNT ON;
DECLARE @sql nvarchar(max);
SELECT @sql=STRING_AGG(CONVERT(nvarchar(max),N'SELECT N'''+t.name+N''' AS Bang,N'''+c.name+N''' AS Cot,CONVERT(nvarchar(max),'+QUOTENAME(c.name)+N') AS GiaTri FROM '+QUOTENAME(s.name)+N'.'+QUOTENAME(t.name)+N' WHERE '+QUOTENAME(c.name)+N' COLLATE Latin1_General_100_BIN2 LIKE N''%''+NCHAR(195)+N''%'' OR '+QUOTENAME(c.name)+N' COLLATE Latin1_General_100_BIN2 LIKE N''%''+NCHAR(225)+NCHAR(187)+N''%'' OR '+QUOTENAME(c.name)+N' COLLATE Latin1_General_100_BIN2 LIKE N''%''+NCHAR(225)+NCHAR(186)+N''%'' OR '+QUOTENAME(c.name)+N' COLLATE Latin1_General_100_BIN2 LIKE N''%''+NCHAR(65533)+N''%'''),N' UNION ALL ')
FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id JOIN sys.columns c ON c.object_id=t.object_id JOIN sys.types ty ON ty.user_type_id=c.user_type_id WHERE ty.name IN ('nvarchar','nchar','varchar','char') AND t.is_ms_shipped=0;
EXEC sp_executesql @sql;
