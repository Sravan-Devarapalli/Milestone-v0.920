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
		  DayOff = 1 
		  AND ActualHours = 8--if a person has added Timeoff  for complete 8 hr then the day is treated as vacation day.
		  AND DATEPART(dw, Date) NOT IN (7, 1) AND
		  DayOff != CompanyDayOff  
	GROUP BY PersonId

