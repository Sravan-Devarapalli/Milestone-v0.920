CREATE PROCEDURE [dbo].[GetSubstituteDate]
(
	@HolidayDate DATETIME,
	@PersonId	INT
)
AS
BEGIN	
	SELECT PC.SubstituteDate 'SubstituteDate'
	FROM dbo.PersonCalendar PC
	WHERE PersonId = @PersonId AND PC.Date = @HolidayDate AND PC.SubstituteDate IS NOT NULL
END
