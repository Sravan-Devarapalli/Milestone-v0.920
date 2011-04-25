-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-10-29
-- Description:	Gets number of holiday days for the person
-- =============================================
CREATE FUNCTION GetNumberHolidayDays
(
	@PersonId INT,
	@startDate DATETIME,
	@endDate DATETIME
)
RETURNS INT
AS
BEGIN
	DECLARE @res INT

	SELECT @res = COUNT(*) 
	FROM dbo.v_PersonCalendar
	WHERE Date BETWEEN @startDate AND @endDate AND 
		  @PersonId = PersonId AND 
		  DayOff = 1 AND
		  DATEPART(dw, Date) != 7 AND DATEPART(dw, Date) != 1

	RETURN @res
END

