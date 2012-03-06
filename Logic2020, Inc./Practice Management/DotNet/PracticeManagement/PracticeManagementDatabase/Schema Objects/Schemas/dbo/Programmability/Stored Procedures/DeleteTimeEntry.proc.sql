CREATE PROCEDURE dbo.DeleteTimeEntry
(
	@ClientId   INT,
	@ProjectId  INT,
	@TimeTypeId INT,
	@StartDate  DATETIME,
	@EndDate    DATETIME,
	@personId   INT,
	@UserLogin  NVARCHAR(255)
)
AS
BEGIN
 
 BEGIN TRAN TimeEntryDelete
 
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

		DELETE TTH
	    FROM dbo.TimeEntry TE 
		INNER JOIN dbo.TimeEntryHours AS TTH  ON TE.TimeEntryId = TTH.TimeEntryId
		INNER JOIN dbo.ChargeCode cc ON TE.ChargeCodeId = cc.Id AND cc.ClientId = @ClientId 
										AND cc.ProjectId =  @ProjectId AND cc.TimeTypeId = @TimeTypeId 
		WHERE TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate AND TE.PersonId = @personId
	
		DELETE TE
		FROM dbo.TimeEntry TE 
		INNER JOIN dbo.ChargeCode cc ON TE.ChargeCodeId = cc.Id AND cc.ClientId = @ClientId 
										AND cc.ProjectId =  @ProjectId AND cc.TimeTypeId = @TimeTypeId 
		WHERE TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate AND TE.PersonId = @personId

		--Delete PersonCalendar entry.
		DELETE PC
		FROM dbo.PersonCalendar PC
		WHERE PC.PersonId = @personId AND PC.Date BETWEEN @StartDate AND @EndDate AND PC.TimeTypeId = @TimeTypeId

		SET @StartDate = @StartDate - 2
		SET @EndDate = @EndDate + 2

		--Update the series.
		;WITH NeedToModifyDates AS
		(
			SELECT  PC.PersonId, C.Date, CONVERT(BIT, 0) 'IsSeries'
			FROM Calendar C
			JOIN PersonCalendar PC ON C.Date IN (@StartDate, @EndDate ) AND C.Date = PC.Date AND PC.PersonId = @personId AND PC.DayOff = 1 AND PC.IsSeries = 1
			LEFT JOIN PersonCalendar APC ON PC.PersonId = APC.PersonId AND APC.DayOff = 1 AND APC.IsSeries = 1 AND APC.ActualHours = PC.ActualHours AND APC.TimeTypeId = PC.TimeTypeId AND ISNULL(APC.ApprovedBy, 0) = ISNULL(PC.ApprovedBy, 0)--APC:- AffectedPersonCalendar
						AND (APC.date = DATEADD(DD,1, C.date)
								OR  APC.date = DATEADD(DD, -1, C.date)
							)
			GROUP BY PC.PersonId, C.date
			Having COUNT(APC.date) < 1
		)

		UPDATE PC
			SET IsSeries = NTMF.IsSeries
		FROM PersonCalendar PC
		JOIN NeedToModifyDates NTMF ON NTMF.PersonId = PC.PersonId AND NTMF.Date = PC.Date
	
	EXEC dbo.SessionLogUnprepare

 COMMIT TRAN TimeEntryDelete
END
