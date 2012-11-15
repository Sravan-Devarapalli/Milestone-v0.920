-- =============================================
-- Author:		Srinivas.M
-- Create date: 25-07-2011
-- Updated by:	Sainathc
-- Update date:	31-05-2012
-- =============================================
CREATE PROCEDURE [dbo].[SetRecurringHoliday]
(
	@Id INT = NULL,
	@IsSet	BIT,
	@UserLogin NVARCHAR(255)
)
AS
BEGIN
/*
1.Update the CompanyRecurringHoliday table with the IsSet value for the given @Id.if(@Id is null) update all holidays IsSet to true.
2.Populate the Recurring HolidaysDates in the temp1 table(i.e. @RecurringHolidaysDates table) according to the constraints specified in CompanyRecurringHoliday table.
3.Update Calendar table with the Recurring holidays.
4.update the ISSERIES column in the person calendar table.
5.Delete all administrative WORKTYPE timeEntries on the Recurring HolidaysDates.
6.check weather the Recurring holiday is set or deleted:
	a.If Recurring holiday is set 
		1.Populate PERSONCALENDAR table records which are parent holidays records for the substitute holidays of the recurring holidays with the person is W2SALARY people to the temp2 table(i.e.@SubDatesForPersons table).
		2.Delete all the person calendar table records which are in the temp2 table and similarly substitute day records for the dates in the temp2 table.
		3.Similary delete the time entry records for the substitute dates of the date records in the temp2 table.
		4.Insert company holiday time entry for the W2SALARY people in the TIMEENTRY tables for the records in the temp2 table.
		5.Insert company holiday time entry for the W2SALARY people in the TIMEENTRY tables for the records in the temp1 table.
	b.If Recurring holiday is deleted ( note : we need to delete the substitute dates for that holiday and time entries related to that dates)
		1.Populate PERSONCALENDAR table records which are substitutes dates for the recurring holiday removed dates to the temp3 table (i.e. @SubDates).
		2.Update substitute date to PTO TIMETYPE for the records populated in the temp3 table.
		3.Similarly update the time entry for those records updated in the above step.
		4.Delete all recurring holidays in the PERSONCALENDAR table which has DayOff = 0 i.e. parent holidays for the substitute days .
		5.Insert time entries for the removed recurring holidays dates if any records exists in the person calendar table for the w2salary and w2hourly persons.
*/

	DECLARE @Today DATETIME,
			@ModifiedBy INT,
			@HolidayTimeTypeId INT,
			@CurrentPMTime DATETIME,
			@PTOTimeTypeId INT,
			@HolidayChargeCodeId INT,
			@PTOChargeCodeId INT,
			@FutureDate DATETIME,
			@W2SalaryId			INT,
			@W2HourlyId			INT


	DECLARE @RecurringHolidaysDates TABLE( [Date] DATETIME, [Description] NVARCHAR(255), [Id] INT)

	SELECT @Today = dbo.GettingPMTime(GETUTCDATE())
		, @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId()
		, @CurrentPMTime = dbo.InsertingTime()
		, @PTOTimeTypeId = dbo.GetPTOTimeTypeId()
		, @FutureDate = dbo.GetFutureDate()

	SELECT @ModifiedBy = PersonId FROM dbo.Person WHERE Alias = @UserLogin
	SELECT @PTOChargeCodeId = Id FROM dbo.ChargeCode WHERE TimeTypeId = @PTOTimeTypeId
	SELECT @HolidayChargeCodeId = Id FROM dbo.ChargeCode WHERE TimeTypeId = @HolidayTimeTypeId
	SELECT	@W2SalaryId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Salary'
	SELECT	@W2HourlyId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Hourly'

	BEGIN TRY
	
	BEGIN TRANSACTION Tran_SetRecurringHoliday	
	
	EXEC SessionLogPrepare @UserLogin = @UserLogin

	--Set the value in CompanyRecurringHoliday
	UPDATE dbo.CompanyRecurringHoliday
	SET IsSet = @IsSet
	WHERE Id = @Id OR @Id IS NULL

	INSERT INTO @RecurringHolidaysDates([Date], [Description], [Id])
	SELECT C1.Date, crh.Description, crh.Id
	FROM dbo.Calendar AS C1
	JOIN dbo.CompanyRecurringHoliday crh ON C1.[Date] >= @Today
		AND 
		(
			(crh.[Day] IS NOT NULL --If holiday is on exact Date.
				AND (	--If Holiday comes in 
						DAY(C1.[Date]) = crh.[Day] AND MONTH(C1.[Date]) = crh.[Month] AND DATEPART(DW,C1.[Date]) NOT IN(1,7)
						OR DAY(DATEADD(DD,1,C1.[Date])) = crh.[Day] AND MONTH(DATEADD(DD,1,C1.[Date])) = crh.[Month]  AND DATEPART(DW,C1.[Date]) = 6
						OR DAY(DATEADD(DD,-1,C1.[Date])) = crh.[Day] AND MONTH(DATEADD(DD,-1,C1.[Date])) = crh.[Month] AND DATEPART(DW,C1.[Date]) = 2
					)
				)
				OR
				( crh.[Day] IS NULL AND MONTH(C1.[Date]) = crh.[Month]
				AND (
						DATEPART(DW,C1.[Date]) = crh.DayOfTheWeek
						AND (
								(crh.NumberInMonth IS NOT NULL
									AND  (C1.[Date] - DAY(C1.[Date])+1) -
												CASE WHEN (DATEPART(DW,C1.[Date]-DAY(C1.[Date])+1))%7 <= crh.DayOfTheWeek 
														THEN (DATEPART(DW,C1.[Date]-DAY(C1.[Date])+1))%7
														ELSE (DATEPART(DW,C1.[Date]-DAY(C1.[Date])+1)-7)
														END +(7*(crh.NumberInMonth-1))+crh.DayOfTheWeek = C1.[Date]
									)
									OR( crh.NumberInMonth IS NULL 
										AND (DATEADD(MM,1,C1.[Date] - DAY(C1.[Date])+1)- 1) - 
												(CASE WHEN DATEPART(DW,(DATEADD(MM,1,C1.[Date] - DAY(C1.[Date])+1)- 1)) >= crh.DayOfTheWeek
													THEN (DATEPART(DW,(DATEADD(MM,1,C1.[Date] - DAY(C1.[Date])+1)- 1)))-7
													ELSE (DATEPART(DW,(DATEADD(MM,1,C1.[Date] - DAY(C1.[Date])+1)- 1)))
													END)-(7-crh.DayOfTheWeek)= C1.[Date]
									)
								)
					)
				)
			)	
	WHERE crh.Id = @Id OR @Id IS NULL

	--Update Calendar table.
	UPDATE  C1
	SET DayOff = @IsSet,
		IsRecurring = @IsSet,
		RecurringHolidayId = CASE WHEN @IsSet = 0 THEN null ELSE rhd.Id END,
		HolidayDescription = CASE WHEN @IsSet = 1 THEN rhd.Description
								ELSE NULL END,
		RecurringHolidayDate = NULL
	FROM dbo.Calendar AS C1
	JOIN @RecurringHolidaysDates rhd ON C1.Date = rhd.Date

		
	--Delete all administrative WORKTYPE timeEntries.
	DELETE TEH
	FROM dbo.TimeEntryHours TEH
	INNER JOIN dbo.TimeEntry TE ON TE.TimeEntryId = TEH.TimeEntryId
	INNER JOIN @RecurringHolidaysDates rhd ON rhd.Date = TE.ChargeCodeDate
	INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
	INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId AND TT.IsAdministrative = 1
	
	DELETE TE
	FROM dbo.TimeEntry TE
	INNER JOIN @RecurringHolidaysDates rhd ON rhd.Date = TE.ChargeCodeDate
	INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
	INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId AND TT.IsAdministrative = 1
	
	IF @IsSet = 1
	BEGIN

	    DECLARE @SubDatesForPersons TABLE ([SubstituteDate] DATETIME,PersonId INT,[HolidayDate] DATETIME,IsW2Salaried BIT)

		INSERT INTO @SubDatesForPersons
		SELECT  PC.SubstituteDate,PC.PersonId, PC.Date,
				(CASE WHEN  pay.Person IS NULL THEN 0
					ELSE 1 END) AS IsW2Salaried
		FROM dbo.PersonCalendar AS PC 
		INNER JOIN @RecurringHolidaysDates dates ON PC.SubstituteDate IS NOT NULL AND dates.date = PC.SubstituteDate  
		LEFT JOIN  dbo.Pay pay  ON pay.Timescale = @W2SalaryId AND pay.Person = Pc.PersonId AND  
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

		INSERT  INTO [dbo].[TimeEntry]
		                ( [PersonId],
							[ChargeCodeId],
							[ChargeCodeDate],
							[Note],
							[ForecastedHours],
							[IsCorrect],
							[IsAutoGenerated]
		                )
		SELECT P.PersonId
				,@HolidayChargeCodeId
				,rhd.[Date]
				,rhd.[Description]
				,0 --Forecasted Hours.
				,1
				,1 --Here it is Auto generated.
		FROM dbo.Person P
		INNER JOIN dbo.Pay pay ON pay.Person = P.PersonId  AND pay.Timescale = @W2SalaryId AND p.PersonId = pay.Person AND P.IsStrawman = 0
		INNER JOIN @RecurringHolidaysDates AS rhd ON rhd.Date BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																															ELSE pay.EndDate - 1
																															END)
		LEFT JOIN dbo.TimeEntry TE ON TE.PersonId = P.PersonId AND TE.ChargeCodeId = @HolidayChargeCodeId AND TE.ChargeCodeDate = rhd.Date
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
		SELECT TE.TimeEntryId
				,8--Actual Hours
				,@CurrentPMTime
				,@CurrentPMTime
				,@ModifiedBy
				,0--Non Billable
				,1--Pending ReviewStatusId
		FROM dbo.Person P
		INNER JOIN dbo.Pay pay ON pay.Person = P.PersonId  AND pay.Timescale = @W2SalaryId AND p.PersonId = pay.Person AND P.IsStrawman = 0
		INNER JOIN @RecurringHolidaysDates AS rhd ON rhd.Date BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																															ELSE pay.EndDate - 1
																															END)
		INNER JOIN dbo.TimeEntry TE ON TE.PersonId = P.PersonId AND TE.ChargeCodeId = @HolidayChargeCodeId AND TE.ChargeCodeDate = rhd.Date
		LEFT JOIN TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
		WHERE TEH.TimeEntryId IS NULL
		
	END
	ELSE IF @IsSet = 0
	BEGIN
		
		DECLARE @SubDates TABLE ([date] DATETIME)

		INSERT INTO @SubDates
		SELECT  PC.SubstituteDate
		FROM dbo.PersonCalendar AS PC 
		INNER JOIN @RecurringHolidaysDates dates ON dates.date = PC.Date AND PC.SubstituteDate IS NOT NULL
	    
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
		INNER JOIN @RecurringHolidaysDates rhd ON rhd.Date = Pc.Date AND PC.DayOff = 0


		INSERT  INTO [dbo].[TimeEntry]
		        (	[PersonId],
					[ChargeCodeId],
					[ChargeCodeDate],
					[Note],
					[ForecastedHours],
					[IsCorrect],
					[IsAutoGenerated]
		        )
		SELECT DISTINCT PC.PersonId,
				CC.Id,
				PC.Date,
				PC.Description,
				0,
				1,
				1
		FROM dbo.PersonCalendar PC
		INNER JOIN @RecurringHolidaysDates d ON d.date = PC.Date AND PC.DayOff = 1
		INNER JOIN dbo.Person p ON p.PersonId = PC.PersonId AND P.IsStrawman = 0
		INNER JOIN dbo.Pay pay ON pay.Person = PC.PersonId AND pay.Timescale IN (@W2HourlyId,@W2SalaryId) AND d.date BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																															ELSE pay.EndDate - 1
																															END)
		INNER JOIN dbo.ChargeCode CC ON CC.TimeTypeId = PC.TimeTypeId
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
				CASE PC.TimeTypeId WHEN @HolidayTimeTypeId THEN 8 ELSE ISNULL(PC.ActualHours,8) END,
				@CurrentPMTime,
				@CurrentPMTime,
				@ModifiedBy,
				0,--Non billable
				CASE WHEN PC.IsFromTimeEntry <> 1 AND PC.TimeTypeId <> @HolidayTimeTypeId THEN 2 ELSE 1 END --ReviewStatusId 2 is Approved, 1 is Pending.
		FROM dbo.PersonCalendar PC
		INNER JOIN @RecurringHolidaysDates d ON d.date = PC.Date AND PC.DayOff = 1
		INNER JOIN dbo.Person p ON p.PersonId = PC.PersonId AND P.IsStrawman = 0
		INNER JOIN dbo.Pay pay ON pay.Person = PC.PersonId AND pay.Timescale IN (@W2HourlyId,@W2SalaryId) AND d.date BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																															ELSE pay.EndDate - 1
																															END)
		INNER JOIN dbo.ChargeCode CC ON CC.TimeTypeId = PC.TimeTypeId
		INNER JOIN dbo.TimeEntry TE ON TE.PersonId = p.PersonId AND TE.ChargeCodeId = CC.Id AND TE.ChargeCodeDate = d.Date
		LEFT JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
		WHERE TEH.TimeEntryId IS NULL
	END
	
		COMMIT TRANSACTION Tran_SetRecurringHoliday	
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION Tran_SetRecurringHoliday
		
		DECLARE	 @ERROR_STATE			tinyint
		,@ERROR_SEVERITY		tinyint
		,@ERROR_MESSAGE		    nvarchar(2000)
		,@InitialTranCount		tinyint

		SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		SET  @ERROR_SEVERITY	= ERROR_SEVERITY()
		SET  @ERROR_STATE		= ERROR_STATE()
		RAISERROR ('%s', @ERROR_SEVERITY, @ERROR_STATE, @ERROR_MESSAGE)
	END CATCH

END

GO

