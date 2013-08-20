-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 02-17-2012
-- Updated by:	ThulasiRam.P
-- Update date:	04-12-2012
-- Description: Insert/Update Substitute Day for company holiday.
-- =============================================
CREATE PROCEDURE [dbo].[SaveSubstituteDay]
(
	@Date       DATETIME,
	@PersonId   INT,
	@UserLogin	NVARCHAR(255),
	@SubstituteDayDate DATETIME = NULL
)
AS
BEGIN
/*
1.Check weather given @SubstituteDayDate is company holiday or a day -off for the given person.if yes throw exception(Error "The selected date is not a working day.").
2.only w2-salary person can enter substitute day.check weather the person is w2salary on the @SubstituteDayDate.
3.Get the holiday description and format the substitute day description.
4.Check weather the person already entered SubstituteDay for the given @Date.if yes need to update the date or else insert the substitute date.
5.update the substitute date
	a.get the previous substitute date.
	b.update the previous substitute date for the given @Date record for the SUBSTITUTEDATE column.
	c.update the  previous substitute date record with the given @SubstituteDayDate,with the latest description.
	d.update the previous substitute date time entry record  with the given @SubstituteDayDate,with the latest description.
6.Insert the substitute date.
	a.delete the @Date record in the person calendar table.
	b.insert 2 records in to person calendar table 
		(i) Updated @Date record.
		(ii)New @SubstituteDayDate record
	c.Delete holiday time type Entry from TimeEntry tables for given @Date.
	d.Insert holiday time type Entry to TimeEntry tables for the given @SubstituteDayDate
*/

	SET NOCOUNT ON;

		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	DECLARE @CurrentPMTime DATETIME,
			@ModifiedBy INT,
			@HolidayTimeTypeId INT,
			@HolidayChargeCodeId INT,
			@IsW2SalaryPerson	BIT = 0
	
	DECLARE @SubstituteDateLog TABLE(
										PersonId INT,
										HolidayDate DATETIME,
										SubstituteDate DATETIME,
										Notes NVARCHAR(100),
										ActualHours DECIMAL(10,2),
										ApprovedBy	INT,
										IsNewRow	BIT
									)

	BEGIN TRY
	BEGIN TRAN tran_SaveSubstituteDay

	IF (EXISTS(SELECT 1 
			  FROM Calendar AS C 
			  WHERE C.Date = @SubstituteDayDate AND DayOff= 1) OR
		EXISTS(SELECT 1 
			  FROM dbo.PersonCalendar AS PC 
			  WHERE PC.Date = @SubstituteDayDate AND DayOff=1 AND PC.PersonId = @PersonId)
	   )
	BEGIN
		DECLARE @Error NVARCHAR(200)
		SET @Error = 'The selected date is not a working day.'
		RAISERROR(@Error,16,1)
	END

	SELECT @IsW2SalaryPerson = 1
	FROM dbo.Pay pay 
	INNER JOIN dbo.Timescale ts ON pay.Timescale = ts.TimescaleId  
	WHERE	pay.Person = @PersonId AND  ts.Name = 'W2-Salary' AND @SubstituteDayDate BETWEEN pay.StartDate AND pay.EndDate-1

	SELECT  @CurrentPMTime = dbo.InsertingTime(),
			@HolidayTimeTypeId = dbo.GetHolidayTimeTypeId()
				
	SELECT @ModifiedBy = PersonId FROM Person WHERE Alias = @UserLogin
	SELECT @HolidayChargeCodeId = Id FROM ChargeCode WHERE TimeTypeId = @HolidayTimeTypeId


	DECLARE @Note  NVARCHAR(1000),
			 @HolidayDescription NVARCHAR(255) 

	SELECT @HolidayDescription = c.HolidayDescription
	FROM Calendar c WHERE c.[Date] = @Date

	SET @Note = 'Substitute for '+ CONVERT(NVARCHAR(10), @Date, 101) +' - ' +ISNULL(@HolidayDescription,'') +'.'


	IF EXISTS (SELECT 1 
				FROM dbo.PersonCalendar PC 
				WHERE PC.PersonId = @PersonId AND PC.Date = @Date AND PC.DayOff = 0 AND PC.SubstituteDate IS NOT NULL)
	BEGIN
		--Update the old substitute day with new substitute day.
		DECLARE @PreviousSubstituteDate DATETIME

		SELECT @PreviousSubstituteDate = PC.SubstituteDate
		FROM dbo.PersonCalendar PC 
		WHERE PC.PersonId = @PersonId AND PC.Date = @Date AND PC.DayOff = 0 AND PC.SubstituteDate IS NOT NULL

		--To Insert into Activitylog AS PER #3168
		INSERT INTO @SubstituteDateLog
		SELECT PC.PersonId,@Date,PC.SubstituteDate,@Note,8,PC.ApprovedBy,0
		FROM dbo.PersonCalendar PC 
		WHERE PC.PersonId = @PersonId AND PC.Date = @Date AND PC.DayOff = 0 AND PC.SubstituteDate IS NOT NULL

		UPDATE PC
			SET PC.SubstituteDate = @SubstituteDayDate
		FROM dbo.PersonCalendar PC
		WHERE PC.PersonId = @PersonId AND PC.Date = @Date AND PC.DayOff = 0

		UPDATE PC
			SET PC.Date = @SubstituteDayDate,
				Description = @Note,
				ApprovedBy = @ModifiedBy
		FROM dbo.PersonCalendar PC
		WHERE PC.PersonId = @PersonId AND PC.Date = @PreviousSubstituteDate AND PC.DayOff = 1
		
		--To Insert into Activitylog AS PER #3168
		INSERT INTO @SubstituteDateLog
		SELECT @PersonId,@Date,@SubstituteDayDate,@Note,8,PC.ApprovedBy,1
		FROM dbo.PersonCalendar PC
		WHERE PC.PersonId = @PersonId AND  PC.Date = @Date

		UPDATE TE
			SET TE.Note = @Note,
				ChargeCodeDate = @SubstituteDayDate
		FROM dbo.TimeEntry TE
		WHERE TE.PersonId = @PersonId AND TE.ChargeCodeId = @HolidayChargeCodeId AND TE.ChargeCodeDate = @PreviousSubstituteDate

		UPDATE TEH
			SET TEH.ModifiedBy = @ModifiedBy,
				TEH.ModifiedDate = @CurrentPMTime,
				TEH.CreateDate = @CurrentPMTime
		FROM dbo.TimeEntryHours TEH
		INNER JOIN dbo.TimeEntry TE ON  TE.TimeEntryId =TEH.TimeEntryId  AND  TE.PersonId = @PersonId AND TE.ChargeCodeId = @HolidayChargeCodeId AND TE.ChargeCodeDate = @SubstituteDayDate
	END
	ELSE
	BEGIN

		DELETE PC
		FROM dbo.PersonCalendar AS PC
		WHERE PC.Date = @Date AND PC.PersonId =@PersonId

		INSERT INTO dbo.PersonCalendar(ActualHours,Date,DayOff,TimeTypeId,IsFromTimeEntry,PersonId,SubstituteDate,Description, ApprovedBy)
		SELECT NULL,@Date,0,NULL,0,@PersonId,@SubstituteDayDate ,NULL, NULL
		UNION 
		SELECT 8,@SubstituteDayDate,1,@HolidayTimeTypeId ,0,@PersonId,NULL,@Note, @ModifiedBy --for insert substitute day
			
			--To Insert into Activitylog AS PER #3168
		INSERT INTO @SubstituteDateLog	
		SELECT @PersonId,@Date,@SubstituteDayDate,@Note,8,@ModifiedBy,1

		--Delete holiday time type  Entry from TimeEntry table for holiday date.
		--Delete From TimeEntryHours.
		DELETE TEH
		FROM TimeEntry TE 
		JOIN TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
		WHERE  TE.PersonId = @PersonId AND TE.ChargeCodeId = @HolidayChargeCodeId AND TE.ChargeCodeDate = @Date

		--Delete From TimeEntry.
		DELETE TE
		FROM TimeEntry TE 
		WHERE  TE.PersonId = @PersonId AND TE.ChargeCodeId = @HolidayChargeCodeId AND TE.ChargeCodeDate = @Date

		

		IF(@IsW2SalaryPerson = 1)
		BEGIN

		INSERT  INTO [dbo].[TimeEntry]
							(  [PersonId],
							[ChargeCodeId],
							[ChargeCodeDate],
							[Note],
							[ForecastedHours],
							[IsCorrect],
							[IsAutoGenerated]
							)
		VALUES(@PersonId,@HolidayChargeCodeId,@SubstituteDayDate,@Note,0,1,1)

		INSERT INTO [dbo].[TimeEntryHours] 
								(   [TimeEntryId],
									[ActualHours],
									[CreateDate],
									[ModifiedDate],
									[ModifiedBy],
									[IsChargeable],
									[ReviewStatusId]
								)
		SELECT TE.TimeEntryId, 8,@CurrentPMTime,@CurrentPMTime,@ModifiedBy,0,2 /* Approved */
		FROM [dbo].[TimeEntry] AS TE
		WHERE TE.PersonId = @PersonId AND TE.ChargeCodeId =@HolidayChargeCodeId AND TE.ChargeCodeDate = @SubstituteDayDate 

		END

	
	
	END

	EXEC dbo.SessionLogUnprepare
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
    --To log into activitylog as per #3168

	;WITH NEW_VALUES AS
	(
	SELECT	i.PersonId,
			CONVERT(NVARCHAR(10), i.HolidayDate, 101) AS [HolidayDate],
			CONVERT(NVARCHAR(10), i.SubstituteDate, 101) AS SubstituteDate,			
			P.LastName+', '+P.FirstName AS PersonName,
			CAST(i.ActualHours AS DECIMAL(10,2)) AS ActualHours,
			i.Notes,
			i.ApprovedBy AS ApprovedPersonId,
			Per.LastName+', '+Per.FirstName AS ApprovedBy
	FROM @SubstituteDateLog AS i
	JOIN dbo.Person P ON P.PersonId = i.PersonId 
	LEFT JOIN dbo.Person Per ON Per.PersonId = i.ApprovedBy 
	WHERE i.IsNewRow = 1
	),

	OLD_VALUES AS
	(
	SELECT	i.PersonId,
			CONVERT(NVARCHAR(10), i.HolidayDate, 101) AS [HolidayDate],
			CONVERT(NVARCHAR(10), i.SubstituteDate, 101) AS SubstituteDate,			
			P.LastName+', '+P.FirstName AS PersonName,
			CAST(i.ActualHours AS DECIMAL(10,2)) AS ActualHours,
			i.Notes,
			i.ApprovedBy AS ApprovedPersonId,
			Per.LastName+', '+Per.FirstName AS ApprovedBy
	FROM @SubstituteDateLog AS i
	JOIN dbo.Person P ON P.PersonId = i.PersonId 
	LEFT JOIN dbo.Person Per ON Per.PersonId = i.ApprovedBy 
	WHERE i.IsNewRow = 0
	)

	-- Log an activity
	INSERT INTO dbo.UserActivityLog
	            (ActivityTypeID,
	             SessionID,
	             SystemUser,
	             Workstation,
	             ApplicationName,
	             UserLogin,
	             PersonID,
	             LastName,
	             FirstName,
				 Data,
	             LogData,
	             LogDate)
	SELECT  CASE
	           WHEN d.PersonId IS NULL THEN 3
	           WHEN i.PersonId IS NULL THEN 5
	           ELSE 4
	       END as ActivityTypeID,
	       l.SessionID,
	       l.SystemUser,
	       l.Workstation,
	       l.ApplicationName,
	       l.UserLogin,
	       l.PersonID,
	       l.LastName,
	       l.FirstName,
		   Data = CONVERT(NVARCHAR(MAX),(SELECT *
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.PersonId = OLD_VALUES.PersonId 
			           WHERE NEW_VALUES.PersonId = ISNULL(i.PersonId, d.PersonId) OR OLD_VALUES.PersonId = ISNULL(i.PersonId, d.PersonId)
					  FOR XML AUTO, ROOT('SubstituteHoliday'))),
		   LogData = (SELECT 
						 NEW_VALUES.PersonId 
						,NEW_VALUES.[HolidayDate]
						,NEW_VALUES.SubstituteDate
						,NEW_VALUES.PersonName
						,NEW_VALUES.ActualHours
						,NEW_VALUES.Notes
						,NEW_VALUES.ApprovedPersonId
						,NEW_VALUES.ApprovedBy 
						,OLD_VALUES.PersonId 
						,OLD_VALUES.[HolidayDate]
						,OLD_VALUES.SubstituteDate
						,OLD_VALUES.PersonName
						,OLD_VALUES.ActualHours
						,OLD_VALUES.Notes
						,OLD_VALUES.ApprovedPersonId
						,OLD_VALUES.ApprovedBy 
					  FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.PersonId = OLD_VALUES.PersonId
			            WHERE NEW_VALUES.PersonId = ISNULL(i.PersonId , d.PersonId ) OR OLD_VALUES.PersonId = ISNULL(i.PersonId , d.PersonId)
					FOR XML AUTO, ROOT('SubstituteHoliday'), TYPE),
					@CurrentPMTime
	  FROM NEW_VALUES AS i
	       FULL JOIN OLD_VALUES AS d ON i.PersonId = d.PersonId
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID

	COMMIT TRAN tran_SaveSubstituteDay
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN tran_SaveSubstituteDay
		
		DECLARE	 @ERROR_STATE	TINYINT
		,@ERROR_SEVERITY		TINYINT
		,@ERROR_MESSAGE		    NVARCHAR(2000)
		,@InitialTranCount		TINYINT

		SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		SET  @ERROR_SEVERITY	= ERROR_SEVERITY()
		SET  @ERROR_STATE		= ERROR_STATE()
		RAISERROR ('%s', @ERROR_SEVERITY, @ERROR_STATE, @ERROR_MESSAGE)
	END CATCH
		EXEC dbo.SessionLogUnprepare
END

