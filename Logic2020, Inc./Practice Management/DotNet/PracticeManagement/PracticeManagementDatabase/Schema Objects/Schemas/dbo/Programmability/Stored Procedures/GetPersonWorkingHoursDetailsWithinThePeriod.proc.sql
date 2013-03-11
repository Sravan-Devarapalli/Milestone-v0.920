CREATE PROCEDURE [dbo].[GetPersonWorkingHoursDetailsWithinThePeriod]
(
	@PersonId    INT,
	@StartDate   DATETIME,
	@EndDate     DATETIME
)
AS
	SET NOCOUNT ON

	SELECT CONVERT(DECIMAL(16,2),ISNULL(SUM(8 - ISNULL(ActualHours,0)),0)) TotalWorkHoursExcludingVacationHours,
		  COUNT(cal.Date) AS TotalWorkDaysIncludingVacationDays,
		  ISNULL(SUM(CASE WHEN cal.DayOff = 1 THEN 1 ELSE 0 END),0) AS VacationDays
	  FROM dbo.v_PersonCalendar AS cal
	 WHERE cal.Date BETWEEN @StartDate AND @EndDate
	   AND (cal.DayOff = 0 OR (cal.DayOff = 1 AND cal.CompanyDayOff = 0 AND cal.IsFloatingHoliday = 0))
	   AND cal.PersonId = @PersonId

