CREATE FUNCTION [dbo].[GetPersonVacationDays]
(
	@PersonId INT,
	@startDate DATETIME,
	@DaysForward INT
)
RETURNS INT
AS
BEGIN
	DECLARE @result INT
	
	DECLARE @EndRange DATETIME	
	SET @EndRange = DATEADD(dd , @DaysForward, @StartDate) - 1

	SELECT @result = COUNT(*) 
	FROM dbo.v_PersonCalendar
	WHERE Date BETWEEN @startDate AND @EndRange AND 
		  @PersonId = PersonId AND 
		  DayOff = 1 AND
		  DATEPART(dw, Date) != 7 AND DATEPART(dw, Date) != 1 AND
		  DayOff != CompanyDayOff  

	RETURN @result
END
