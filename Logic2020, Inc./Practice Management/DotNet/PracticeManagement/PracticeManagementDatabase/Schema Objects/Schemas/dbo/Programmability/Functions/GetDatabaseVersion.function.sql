
-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-11-05
-- Description:	Returns database version
-- =============================================
CREATE FUNCTION dbo.GetDatabaseVersion()
RETURNS VARCHAR(100)
AS
BEGIN
	DECLARE @result VARCHAR(100);
	
	WITH LastUpdates AS (
		SELECT TOP 1 [name],
					 modify_date,
					 'Proc' AS 'Type'
		FROM sys.procedures
		ORDER BY modify_date DESC 
		UNION
		SELECT TOP 1 [name],
					 modify_date,
					 'Table' AS 'Type'
		FROM sys.tables
		ORDER BY modify_date DESC 
		UNION
		SELECT TOP 1 [name],
					 modify_date,
					 'View' AS 'Type'
		FROM sys.[views]
		ORDER BY modify_date DESC 
		UNION
		SELECT TOP 1 obj.[name],
					 obj.modify_date,
					 'Func' AS 'Type'
		FROM sys.objects AS obj
		WHERE obj.[type] IN (N'FN', N'IF', N'TF', N'FS', N'FT')
		ORDER BY obj.modify_date DESC 
	)
	SELECT TOP 1  @result = 'Last updated: ' + CONVERT(VARCHAR(10), modify_date, 101) FROM LastUpdates
	ORDER BY modify_date DESC 
	
	RETURN @result
	-- RETURN 'v0.675 - 2009/11/27 - 01'
END

