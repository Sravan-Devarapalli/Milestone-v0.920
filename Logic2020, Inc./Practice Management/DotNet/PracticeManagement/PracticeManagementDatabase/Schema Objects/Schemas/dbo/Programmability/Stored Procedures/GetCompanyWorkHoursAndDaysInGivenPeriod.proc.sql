CREATE PROCEDURE [dbo].[GetCompanyWorkHoursAndDaysInGivenPeriod]
(
	@StartDate   DATETIME,
	@EndDate     DATETIME,
	@IncludeCompanyHolidays BIT = 1
)
AS
	SET NOCOUNT ON

	SELECT CONVERT(DECIMAL(10,2),COUNT(*) * CONVERT(DECIMAL(4,2),S.Value)) WorkHours,CONVERT(DECIMAL(10,2),COUNT(*)) AS WorkDays
	FROM dbo.Calendar AS cal
	INNER JOIN dbo.Settings S ON s.SettingsKey = 'DefaultHoursPerDay'
	WHERE cal.Date BETWEEN @StartDate AND @EndDate AND DATEPART(DW,cal.Date ) NOT IN(1,7) AND
 	(@IncludeCompanyHolidays = 0 OR cal.DayOff = 0)
	GROUP BY S.Value
