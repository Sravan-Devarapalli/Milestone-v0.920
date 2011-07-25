CREATE PROCEDURE [dbo].[GetRecurringHolidaysList]
AS
BEGIN
	SELECT Id
			,Description
			,IsSet
	FROM dbo.CompanyRecurringHoliday
	
END
GO
