CREATE FUNCTION [dbo].[GetPersonVacationDaysTable]
(
	@StartDate DATETIME,
	@EndDate DATETIME
)
RETURNS TABLE
AS
	RETURN
	SELECT PersonId,COUNT(*) AS VacationDays
	FROM dbo.v_PersonCalendar
	WHERE Date BETWEEN @StartDate AND @EndDate AND 
		  DayOff = 1 AND
		  DATEPART(dw, Date) NOT IN (7, 1) AND
		  DayOff != CompanyDayOff  
	GROUP BY PersonId
