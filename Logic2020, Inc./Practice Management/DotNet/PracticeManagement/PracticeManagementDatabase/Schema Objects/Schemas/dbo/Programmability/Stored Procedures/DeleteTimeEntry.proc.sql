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


		
	
	EXEC dbo.SessionLogUnprepare

 COMMIT TRAN TimeEntryDelete
END

