﻿-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 02-17-2012
-- Updated by:	Srinivas.M
-- Update date:	02-28-2012
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
	SET NOCOUNT ON;
	
	DECLARE @Today DATETIME,
			@PTOTimeTypeId INT,
			@CurrentPMTime DATETIME,
			@ModifiedBy INT,
			@HolidayTimeTypeId INT,
			@PTOChargeCodeId INT,
			@HolidayChargeCodeId INT,
			@IsW2SalaryPerson	BIT = 0

	

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
		SET @Error = 'Selected date is not a Working day.Please select any Working day.'
		RAISERROR(@Error,16,1)
	END


	SELECT @IsW2SalaryPerson = 1
	FROM dbo.Pay pay 
	INNER JOIN dbo.Timescale ts ON pay.Timescale = ts.TimescaleId  
	WHERE	pay.Person = @PersonId AND  ts.Name = 'W2-Salary' AND @SubstituteDayDate BETWEEN pay.StartDate AND ISNULL(pay.EndDate,dbo.GetFutureDate())

	SELECT @Today = dbo.GettingPMTime(GETUTCDATE()),
			@CurrentPMTime = dbo.InsertingTime(),
			@PTOTimeTypeId = dbo.GetPTOTimeTypeId(),
			@HolidayTimeTypeId = dbo.GetHolidayTimeTypeId()
				
	SELECT @ModifiedBy = PersonId FROM Person WHERE Alias = @UserLogin
	SELECT @PTOChargeCodeId = Id FROM ChargeCode WHERE TimeTypeId = @PTOTimeTypeId
	SELECT @HolidayChargeCodeId = Id FROM ChargeCode WHERE TimeTypeId = @HolidayTimeTypeId


	DECLARE @Note  NVARCHAR(1000),
			 @HolidayDescription NVARCHAR(255) 

	SELECT @HolidayDescription = c.HolidayDescription
	FROM Calendar c WHERE c.[Date] = @Date

	DECLARE @UserName NVARCHAR(MAX)
	SELECT @UserName =  (FirstName +' '+ LastName) FROM [dbo].[Person] WHERE [Alias] = @UserLogin

	SET @Note = 'Substitute for '+ CONVERT(NVARCHAR(10), @Date, 101) +' - ' +@HolidayDescription +'. Approved by '+ @UserName + '.'


	IF EXISTS (SELECT 1 
				FROM dbo.PersonCalendar PC 
				WHERE PC.PersonId = @PersonId AND PC.Date = @Date AND PC.DayOff = 0 AND PC.SubstituteDate IS NOT NULL)
	BEGIN
		--Update the old substitute day with new substitute day.
		DECLARE @PreviousSubstituteDate DATETIME

		SELECT @PreviousSubstituteDate = PC.SubstituteDate
		FROM dbo.PersonCalendar PC 
		WHERE PC.PersonId = @PersonId AND PC.Date = @Date AND PC.DayOff = 0 AND PC.SubstituteDate IS NOT NULL

		UPDATE PC
			SET PC.SubstituteDate = @SubstituteDayDate
		FROM dbo.PersonCalendar PC
		WHERE PC.PersonId = @PersonId AND PC.Date = @Date AND PC.DayOff = 0

		UPDATE PC
			SET PC.Date = @SubstituteDayDate,
				Description = @Note
		FROM dbo.PersonCalendar PC
		WHERE PC.PersonId = @PersonId AND PC.Date = @PreviousSubstituteDate AND PC.DayOff = 1
		
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
		INNER JOIN dbo.TimeEntry TE ON TE.PersonId = @PersonId AND TE.ChargeCodeId = @HolidayChargeCodeId AND TE.ChargeCodeDate = @SubstituteDayDate
	END
	ELSE
	BEGIN

		INSERT INTO dbo.PersonCalendar(ActualHours,Date,DayOff,TimeTypeId,IsFromTimeEntry,PersonId,SubstituteDate,Description)
		SELECT NULL,@Date,0,NULL,0,@PersonId,@SubstituteDayDate ,NULL
		UNION 
		SELECT 8,@SubstituteDayDate,1,@HolidayTimeTypeId ,0,@PersonId,NULL,@Note
				

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

END
