CREATE PROCEDURE [dbo].[GetWorkingDaysForTheGivenYear]
(
	@Year    INT
)
AS
	SET NOCOUNT ON
	DECLARE @DaysInYear INT = 260
	SELECT @DaysInYear = C.DaysInYear FROM dbo.Calendar C WHERE C.Year = @Year

	SELECT @DaysInYear AS DaysInYear
