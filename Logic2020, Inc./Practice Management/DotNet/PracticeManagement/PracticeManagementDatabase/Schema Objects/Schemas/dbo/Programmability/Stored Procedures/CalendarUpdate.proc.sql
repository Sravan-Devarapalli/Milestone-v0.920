-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-5-2008
-- Updated by:	ThulasiRam.P
-- Update date:	30-05-2012
-- Description:	Updates a calendar item
-- =============================================
CREATE PROCEDURE [dbo].[CalendarUpdate]
(
	@Date       DATETIME,
	@DayOff     BIT,
	@UserLogin	NVARCHAR(255),
	@IsRecurringHoliday BIT,
	@RecurringHolidayId INT,
	@HolidayDescription	NVARCHAR(255),
	@RecurringHolidayDate DATETIME,
	@ActualHours REAL
)
AS
	SET NOCOUNT ON
		
	/*
		From CalendarPage:

		For W2Salaried Person:-
		1. If Company Holiday then insert Holiday Time Entry.
		2. If Person PTO then insert PTO Time Entry(Update/Delete also).

		For Non W2Salaried person:-
		1. If person kept PTO then not inserting Time Entry.
		1. If updating PTO actual hours then Updating the actual hours.
		2. If person kept FloatingHoliday then, Deleting the time Entry(No Time Entry for floating Holiday).
		3. If Person kept FloatingHoliday, later kept PTO then, not inserting PTO time entry.
	*/
	
	DECLARE @Today DATETIME,
			@PTOTimeTypeId INT,
			@CurrentPMTime DATETIME,
			@ModifiedBy INT,
			@HolidayTimeTypeId INT,
			@PTOChargeCodeId INT,
			@HolidayChargeCodeId INT,
			@FutureDate DATETIME

	SELECT @Today = dbo.GettingPMTime(GETUTCDATE()),
			@CurrentPMTime = dbo.InsertingTime(),
			@PTOTimeTypeId = dbo.GetPTOTimeTypeId(),
			@HolidayTimeTypeId = dbo.GetHolidayTimeTypeId(),
			@FutureDate = dbo.GetFutureDate()
				
	SELECT @ModifiedBy = PersonId FROM Person WHERE Alias = @UserLogin
	SELECT @PTOChargeCodeId = Id FROM ChargeCode WHERE TimeTypeId = @PTOTimeTypeId
	SELECT @HolidayChargeCodeId = Id FROM ChargeCode WHERE TimeTypeId = @HolidayTimeTypeId

	DECLARE @Dates TABLE ([date] DATETIME)

	BEGIN TRY
	BEGIN TRAN tran_CalendarUpdate

	INSERT INTO @Dates
	SELECT [Date]
	FROM Calendar
	WHERE Date >= @Date
	AND ( (@IsRecurringHoliday = 0 AND Date = @Date)
		OR (@IsRecurringHoliday = 1 AND @RecurringHolidayDate IS NOT NULL  AND RecurringHolidayId IS NULL AND
				(
					(DAY([Date]) = DAY(@RecurringHolidayDate) AND MONTH([Date]) = MONTH(@RecurringHolidayDate) AND DATEPART(DW,[Date]) NOT IN(1,7))
							OR ( DAY(DATEADD(DD,1,[Date])) = DAY(@RecurringHolidayDate) AND MONTH(DATEADD(DD,1,[Date])) = MONTH(@RecurringHolidayDate)  AND DATEPART(DW,[Date]) = 6
								OR DAY(DATEADD(DD,-1,[Date])) = DAY(@RecurringHolidayDate) AND MONTH(DATEADD(DD,-1,[Date])) = MONTH(@RecurringHolidayDate) AND DATEPART(DW,[Date]) = 2
							)
				)
		   )
		)
	
	-- Update the company calendar
	UPDATE C1
		SET DayOff = @DayOff,
			HolidayDescription = CASE WHEN @DayOff = 0 THEN NULL ELSE @HolidayDescription END,
			IsRecurring = CASE WHEN @DayOff = 0 THEN NULL ELSE @IsRecurringHoliday END, --whether it should be applied to future dates or not either dayoff 0 or 1.
			RecurringHolidayId = CASE WHEN @DayOff = 0 THEN NULL ELSE @RecurringHolidayId END, --we need to set holidayId to null while removing..
			RecurringHolidayDate = CASE WHEN @DayOff = 0 THEN NULL ELSE @RecurringHolidayDate END-- if date is not recurring OR recurring holiday Id is there then Recurringholidaydate should be null.
	FROM dbo.Calendar C1
	JOIN @Dates d ON d.[date] = C1.Date

	IF(@DayOff = 0)/* 
							Remove holiday from Company holiday page then 
						•	Update Entry For substitute day as PTO time type in time entry & Person calendar. 
		  
		            */
	BEGIN

		DECLARE @SubDates TABLE ([date] DATETIME)

		INSERT INTO @SubDates
		SELECT  PC.SubstituteDate
		FROM dbo.PersonCalendar AS PC 
		INNER JOIN @Dates dates ON dates.date = PC.Date AND PC.SubstituteDate IS NOT NULL
	    
		UPDATE PC
		SET PC.TimeTypeId = @PTOTimeTypeId,
		PC.Description = 'PTO'
		FROM dbo.PersonCalendar AS PC 
		INNER JOIN @SubDates AS SUBDATES ON PC.Date = SUBDATES.date

		UPDATE TEH 
		SET TEH.ModifiedBy = @ModifiedBy,
			TEH.ModifiedDate = @CurrentPMTime
		FROM dbo.TimeEntryHours AS TEH 
		INNER JOIN dbo.TimeEntry AS TE ON TE.TimeEntryId = TEH.TimeEntryId
		INNER JOIN @SubDates AS SUBDATES ON SUBDATES.date = TE.ChargeCodeDate AND TE.ChargeCodeId = @HolidayChargeCodeId 

		UPDATE TE 
		SET TE.Note = 'PTO',
			TE.ChargeCodeId = @PTOChargeCodeId
		FROM dbo.TimeEntry AS TE 
		INNER JOIN @SubDates AS SUBDATES ON SUBDATES.date = TE.ChargeCodeDate AND TE.ChargeCodeId = @HolidayChargeCodeId 

		DELETE PC
		FROM dbo.PersonCalendar AS PC 
		WHERE PC.Date = @Date AND PC.DayOff = 0

	END
	ELSE 
	BEGIN 

		
		
		/* Insert  Company holiday date as Any person substitute day date */
		DECLARE @SubDatesForPersons TABLE ([SubstituteDate] DATETIME,PersonId INT,[HolidayDate] DATETIME,IsW2Salaried BIT)

		INSERT INTO @SubDatesForPersons
		SELECT  PC.SubstituteDate,PC.PersonId, PC.Date,
				(CASE WHEN  pay.Person IS NULL THEN 0
					ELSE 1 END) AS IsW2Salaried
		FROM dbo.PersonCalendar AS PC 
		INNER JOIN @Dates dates ON PC.SubstituteDate IS NOT NULL AND dates.date = PC.SubstituteDate  
		LEFT JOIN  dbo.Pay pay  ON pay.Timescale = 2 /* 'W2-Salary' */ AND pay.Person = Pc.PersonId AND  
									PC.Date BETWEEN pay.StartDate AND ISNULL(pay.EndDate,@FutureDate)

			
		DELETE pc
		FROM dbo.PersonCalendar pc 
		INNER JOIN @SubDatesForPersons AS SDP ON (pc.SubstituteDate = SDP.[SubstituteDate] AND pc.PersonId = SDP.PersonId) OR
													(pc.Date =SDP.[SubstituteDate] AND pc.PersonId = SDP.PersonId)

		--Delete holiday timetype  Entry from TimeEntry table for substitute date.
		--Delete From TimeEntryHours.
		DELETE TEH
		FROM dbo.TimeEntry TE 
		INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
		INNER JOIN @SubDatesForPersons AS SDP ON TE.PersonId = SDP.PersonId AND TE.ChargeCodeDate = SDP.[SubstituteDate]
		WHERE TE.ChargeCodeId = @HolidayChargeCodeId 

		--Delete From TimeEntry.
		DELETE TE
		FROM dbo.TimeEntry TE
		INNER JOIN @SubDatesForPersons AS SDP ON TE.PersonId = SDP.PersonId AND TE.ChargeCodeDate = SDP.[SubstituteDate]
		WHERE  TE.ChargeCodeId = @HolidayChargeCodeId

		INSERT  INTO [dbo].[TimeEntry]
							([PersonId],
							[ChargeCodeId],
							[ChargeCodeDate],
							[Note],
							[ForecastedHours],
							[IsCorrect],
							[IsAutoGenerated]
							)
		SELECT SDP.PersonId,@HolidayChargeCodeId,SDP.HolidayDate,c.HolidayDescription,0,1,1
		FROM dbo.Calendar c 
		INNER JOIN @SubDatesForPersons AS SDP ON SDP.HolidayDate =  c.Date  AND  SDP.IsW2Salaried = 1
				
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
		INNER JOIN @SubDatesForPersons AS SDP ON SDP.HolidayDate =  TE.ChargeCodeDate 
													AND SDP.IsW2Salaried = 1  
													AND TE.PersonId = SDP.PersonId  
													AND TE.ChargeCodeId = @HolidayChargeCodeId

	END
	
	 
	--Delete All Holiday/PTO timeEntries.
	--Delete From TimeEntryHours.
	DELETE TEH
	FROM dbo.Person P
	JOIN dbo.Pay pay ON pay.Person = P.PersonId  AND pay.Timescale = 2 AND p.IsStrawman = 0 
	JOIN @Dates rhd ON rhd.[date] BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																	ELSE pay.EndDate - 1
																	END)
	JOIN dbo.TimeEntry TE ON TE.PersonId = P.PersonId AND TE.ChargeCodeDate = rhd.date
	JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
	JOIN dbo.TimeType TT ON TT.IsAdministrative = 1 AND TT.TimeTypeId = CC.TimeTypeId 
	JOIN dbo.Calendar c ON c.Date = rhd.date AND DATEPART(DW, c.date) NOT IN (1,7)
	JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId

	--Delete From TimeEntry.
	DELETE TE
	FROM dbo.Person P
	JOIN dbo.Pay pay ON pay.Person = P.PersonId  AND pay.Timescale = 2 AND p.IsStrawman = 0 
	JOIN @Dates rhd ON rhd.[date] BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																	ELSE pay.EndDate - 1
																	END)
	JOIN dbo.TimeEntry TE ON TE.PersonId = P.PersonId AND TE.ChargeCodeDate = rhd.date
	JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
	JOIN dbo.TimeType TT ON TT.IsAdministrative = 1 AND TT.TimeTypeId = CC.TimeTypeId 
	JOIN dbo.Calendar c ON c.Date = rhd.date AND DATEPART(DW, c.date) NOT IN (1,7)

	IF @DayOff = 0
	BEGIN

		INSERT  INTO [dbo].[TimeEntry]
					( [PersonId],
						[ChargeCodeId],
						[ChargeCodeDate],
						[Note],
						[ForecastedHours],
						[IsCorrect],
						[IsAutoGenerated]
					)
		SELECT DISTINCT PC.PersonId,
				CC.Id,
				PC.date,
				PC.Description,
				0,--Forecasted Hours.
				1,--IsCorrect.
				1--IsAutogenerated.
		FROM dbo.PersonCalendar PC
		INNER JOIN @Dates d ON d.date = PC.Date AND PC.DayOff = 1
		INNER JOIN dbo.Person p ON p.PersonId = PC.PersonId AND P.IsStrawman = 0
		INNER JOIN dbo.Pay pay ON pay.Person = PC.PersonId AND pay.Timescale = 2 AND d.date BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																															ELSE pay.EndDate - 1
																															END)
		INNER JOIN ChargeCode CC ON CC.TimeTypeId = PC.TimeTypeId
		LEFT JOIN dbo.TimeEntry TE ON TE.PersonId = PC.PersonId AND TE.ChargeCodeId = CC.Id AND TE.ChargeCodeDate = PC.Date
		WHERE TE.TimeEntryId IS NULL
			
		INSERT INTO [dbo].[TimeEntryHours] 
								( [TimeEntryId],
									[ActualHours],
									[CreateDate],
									[ModifiedDate],
									[ModifiedBy],
									[IsChargeable],
									[ReviewStatusId]
								)
		SELECT TE.TimeEntryId,
				CASE WHEN PC.ActualHours IS NOT NULL AND ISNULL(PC.TimeTypeId,0) != @HolidayTimeTypeId  THEN PC.ActualHours ELSE 8 END,
				@CurrentPMTime,
				@CurrentPMTime,
				@ModifiedBy,
				0,--Non Billable
				CASE WHEN PC.IsFromTimeEntry <> 1 AND PC.TimeTypeId <> @HolidayTimeTypeId THEN 2 ELSE 1 END--Inserting PTO timeEntries with Approved Status.
		FROM dbo.PersonCalendar PC
		JOIN @Dates d ON d.date = PC.Date AND PC.DayOff = 1
		JOIN dbo.Person p ON p.PersonId = PC.PersonId AND P.IsStrawman = 0
		JOIN dbo.Pay pay ON pay.Person = PC.PersonId AND pay.Timescale = 2 AND d.date BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																															ELSE pay.EndDate - 1
																															END)
		INNER JOIN dbo.ChargeCode CC ON CC.TimeTypeId = PC.TimeTypeId
		JOIN dbo.TimeEntry TE ON TE.PersonId = PC.PersonId AND TE.ChargeCodeId = CC.Id AND TE.ChargeCodeDate = PC.Date
		LEFT JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
		WHERE TEH.TimeEntryId IS NULL
		
	END
	ELSE
	BEGIN

		INSERT  INTO [dbo].[TimeEntry]
		                ( [PersonId],
							[ChargeCodeId],
							[ChargeCodeDate],
							[Note],
							[ForecastedHours],
							[IsCorrect],
							[IsAutoGenerated]
		                )
		SELECT DISTINCT P.PersonId
				, @HolidayChargeCodeId  
				,rhd.[Date]
				,ISNULL(C.HolidayDescription, '')
				,0
				,1
				,1 --Here it is Auto generated.
		FROM dbo.Person P
		JOIN dbo.Pay pay ON p.PersonId = pay.Person  AND pay.Timescale = 2 AND P.IsStrawman = 0 
		JOIN @Dates rhd ON rhd.[date] BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																															ELSE pay.EndDate - 1
																															END)
		LEFT JOIN dbo.TimeEntry TE ON TE.PersonId = P.PersonId
									AND (TE.ChargeCodeId = @HolidayChargeCodeId)
									AND TE.ChargeCodeDate = rhd.Date
		JOIN dbo.Calendar c ON c.Date = rhd.date AND DATEPART(DW, c.date) NOT IN (1,7)
		LEFT JOIN dbo.PersonCalendar PC ON PC.PersonId = P.PersonId AND PC.Date = c.Date
		WHERE  TE.TimeEntryId IS NULL

		INSERT INTO [dbo].[TimeEntryHours] 
								( [TimeEntryId],
									[ActualHours],
									[CreateDate],
									[ModifiedDate],
									[ModifiedBy],
									[IsChargeable],
									[ReviewStatusId]
								)
		SELECT TE.TimeEntryId
				,8 
				,@CurrentPMTime
				,@CurrentPMTime
				,@ModifiedBy
				,0--Non Billable
				,1--Setting Approved Status flag for PTO entries from Calendar page.
		FROM dbo.Person P
		JOIN dbo.Pay pay ON p.PersonId = pay.Person  AND pay.Timescale = 2 AND P.IsStrawman = 0
		JOIN @Dates rhd ON rhd.[date] BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																															ELSE pay.EndDate - 1
																															END)
		JOIN dbo.TimeEntry TE ON TE.PersonId = P.PersonId
									AND (TE.ChargeCodeId = @HolidayChargeCodeId)
									AND TE.ChargeCodeDate = rhd.Date
		JOIN dbo.Calendar c ON c.Date = rhd.date AND DATEPART(DW, c.date) NOT IN (1,7)
		LEFT JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
		LEFT JOIN dbo.PersonCalendar PC ON PC.PersonId = P.PersonId AND PC.Date = c.Date
		WHERE  TEH.TimeEntryId IS NULL
	END

	COMMIT TRAN tran_CalendarUpdate
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN tran_CalendarUpdate
		
		DECLARE	 @ERROR_STATE	tinyint
		,@ERROR_SEVERITY		tinyint
		,@ERROR_MESSAGE		    nvarchar(2000)
		,@InitialTranCount		tinyint

		SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		SET  @ERROR_SEVERITY	= ERROR_SEVERITY()
		SET  @ERROR_STATE		= ERROR_STATE()
		RAISERROR ('%s', @ERROR_SEVERITY, @ERROR_STATE, @ERROR_MESSAGE)
	END CATCH


GO

