CREATE PROCEDURE [dbo].[DatabaseCleanUp]
AS
BEGIN
BEGIN TRY
	BEGIN TRAN tran_DatabaseCleanUp
	DECLARE @CurrentTime DATETIME
			
	SELECT @CurrentTime = dbo.GettingPMTime(GETUTCDATE())

	IF (Day(@CurrentTime) =1 AND MONTH(@CurrentTime) = 1)
	BEGIN 
	DECLARE @UserActivitylogDataBackUpyears INT = -1,
			@PersonCalendarAutoFutureyears INT = 5,
			@UserActivitylogDataLastDate DATETIME,
			@PersonCalendarAutoFutureDate DATETIME
	
	SELECT @UserActivitylogDataLastDate = DATEADD(YY,@UserActivitylogDataBackUpyears,@CurrentTime),
			@PersonCalendarAutoFutureDate= DATEADD(YY,@PersonCalendarAutoFutureyears,@CurrentTime)

	DELETE u
	FROM useractivitylog u 
	WHERE logdate < @UserActivitylogDataLastDate

	INSERT INTO dbo.PersonCalendarAuto (Date, PersonId, DayOff,CompanyDayOff,TimeOffHours)
	SELECT c.Date, c.PersonId, c.DayOff,c.CompanyDayOff,c.ActualHours
	FROM dbo.v_PersonCalendar AS C 
	LEFT JOIN dbo.PersonCalendarAuto AS PCA ON C.PersonId = PCA.PersonId AND C.Date = PCA.Date
	WHERE PCA.Date IS NULL AND PCA.PersonId IS NULL AND C.Date < @PersonCalendarAutoFutureDate

	END

	COMMIT TRAN tran_DatabaseCleanUp
END TRY
BEGIN CATCH
	ROLLBACK TRAN tran_DatabaseCleanUp
		
	DECLARE	 @ERROR_STATE	tinyint
	,@ERROR_SEVERITY		tinyint
	,@ERROR_MESSAGE		    nvarchar(2000)
	,@InitialTranCount		tinyint

	SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
	SET  @ERROR_SEVERITY	= ERROR_SEVERITY()
	SET  @ERROR_STATE		= ERROR_STATE()
	RAISERROR ('%s', @ERROR_SEVERITY, @ERROR_STATE, @ERROR_MESSAGE)
END CATCH
END
