CREATE PROCEDURE [dbo].[GetRecurringHolidaysList]
AS
BEGIN
	SELECT Id
			,Description + '<br />&nbsp;' + DateDescription AS 'Description'
			,IsSet
	FROM dbo.CompanyRecurringHoliday
	ORDER BY Month, Day, CASE WHEN NumberInMonth IS NULL THEN 6 ELSE NumberInMonth END , DayOfTheWeek
	
END
GO
