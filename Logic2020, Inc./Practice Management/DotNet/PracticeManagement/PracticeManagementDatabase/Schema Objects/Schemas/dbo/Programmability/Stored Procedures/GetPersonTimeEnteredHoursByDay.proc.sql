CREATE PROCEDURE [dbo].[GetPersonTimeEnteredHoursByDay]
	@PersonId INT, 
	@Date		DATETIME,
	@IncludePTOAndHoliday	BIT
AS
BEGIN
	DECLARE @Hours REAL,
		@PTOChargeCodeId INT,
		@HolidayChargeCodeId INT

	SELECT @PTOChargeCodeId = Id
	FROM ChargeCode
	WHERE TimeTypeId = dbo.GetPTOTimeTypeId()

	SELECT @HolidayChargeCodeId = Id
	FROM ChargeCode
	WHERE TimeTypeId = dbo.GetHolidayTimeTypeId()
	
	SELECT @Hours = SUM(ActualHours)
	FROM TimeTrack
	WHERE PersonId = @PersonId
	AND ChargeCodeDate = @Date
	AND (@IncludePTOAndHoliday = 1 OR (@IncludePTOAndHoliday = 0 AND ChargeCodeId NOT IN (@PTOChargeCodeId, @HolidayChargeCodeId)))
	GROUP BY PersonId, ChargeCodeDate

	SELECT @Hours
END
