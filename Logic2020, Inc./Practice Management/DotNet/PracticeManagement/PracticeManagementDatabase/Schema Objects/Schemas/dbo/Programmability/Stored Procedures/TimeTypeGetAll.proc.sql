-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-11-30
-- Description:	Gets all avaliable time types
-- =============================================
CREATE PROCEDURE dbo.TimeTypeGetAll
AS
BEGIN

	WITH UsageInfo AS (
	SELECT 
		tt.TimeTypeId, 
		tt.[Name], 
		'True' AS InUse,
		tt.IsDefault
	FROM TimeType AS tt
	WHERE tt.TimeTypeId IN (SELECT DISTINCT(TimeTypeId) FROM TimeEntries)
	UNION 
	SELECT 
		tt.TimeTypeId, 
		tt.[Name], 
		'False' AS InUse,
		tt.IsDefault
	FROM TimeType AS tt
	WHERE tt.TimeTypeId NOT IN (SELECT DISTINCT(TimeTypeId) FROM TimeEntries)
	)
	SELECT * 
	FROM UsageInfo
	ORDER BY Name
END

