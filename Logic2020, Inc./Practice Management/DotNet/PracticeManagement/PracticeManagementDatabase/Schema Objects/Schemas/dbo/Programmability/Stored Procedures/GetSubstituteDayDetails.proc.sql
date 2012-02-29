CREATE PROCEDURE [dbo].[GetSubstituteDayDetails]
(
	@SubstituteDayDate	DATETIME,
	@PersonId		INT
)
AS
BEGIN
	
	SELECT PC.Date 'HolidayDate', C.HolidayDescription 'HolidayDescription'
	FROM dbo.PersonCalendar PC
	INNER JOIN dbo.Calendar C ON PC.PersonId = @PersonId AND PC.SubstituteDate = @SubstituteDayDate AND C.Date = PC.Date

END
