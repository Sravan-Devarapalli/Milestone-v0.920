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
	FROM dbo.ChargeCode
	WHERE TimeTypeId = dbo.GetPTOTimeTypeId()

	SELECT @HolidayChargeCodeId = Id
	FROM dbo.ChargeCode
	WHERE TimeTypeId = dbo.GetHolidayTimeTypeId()
	
	SELECT @Hours = SUM(TEH.ActualHours)
	FROM dbo.TimeEntry TE
	INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = te.TimeEntryId 
	WHERE TE.PersonId = @PersonId
	AND TE.ChargeCodeDate = @Date
	AND (@IncludePTOAndHoliday = 1 OR (@IncludePTOAndHoliday = 0 AND TE.ChargeCodeId NOT IN (@PTOChargeCodeId, @HolidayChargeCodeId)))
	GROUP BY TE.PersonId, TE.ChargeCodeDate

	SELECT @Hours
END
