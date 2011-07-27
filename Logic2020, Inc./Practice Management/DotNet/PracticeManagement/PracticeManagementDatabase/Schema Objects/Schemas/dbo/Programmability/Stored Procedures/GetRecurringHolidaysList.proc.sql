CREATE PROCEDURE [dbo].[GetRecurringHolidaysList]
AS
BEGIN
	SELECT Id
			,Description + '<br />&nbsp;' + DateDescription AS 'Description'
			,IsSet
	FROM dbo.CompanyRecurringHoliday
	
END
GO
