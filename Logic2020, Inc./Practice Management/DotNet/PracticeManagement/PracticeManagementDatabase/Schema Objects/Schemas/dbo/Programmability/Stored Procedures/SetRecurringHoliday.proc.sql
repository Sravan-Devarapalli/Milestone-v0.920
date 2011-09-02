﻿CREATE PROCEDURE [dbo].[SetRecurringHoliday]
(
	@Id INT = NULL,
	@IsSet	BIT,
	@UserLogin NVARCHAR(255)
)
AS
BEGIN

DECLARE @Today DATETIME,
	@ModifiedBy INT,
	@HolidayTimeTypeId INT

DECLARE @RecurringHolidaysDates TABLE( [Date] DATETIME, [Description] NVARCHAR(255), [Id] INT)

SELECT @Today = dbo.GettingPMTime(GETUTCDATE()), @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId()

SELECT @ModifiedBy = PersonId
FROM Person
WHERE Alias = @UserLogin

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
	

	--Update PersonCalendarAuto table
	UPDATE C1
	SET DayOff = @IsSet
	FROM dbo.PersonCalendarAuto C1
	JOIN @RecurringHolidaysDates rhd ON C1.Date = rhd.Date
		
		
	
	DECLARE @CurrentPMTime DATETIME,
		 @DefaultMilestoneId INT,
		 @PTOTimeTypeId INT
	
	SET @CurrentPMTime = dbo.InsertingTime()
	SELECT @DefaultMilestoneId = MilestoneId FROM DefaultMilestoneSetting
	SELECT @PTOTimeTypeId = TimeTypeId FROM TimeType WHERE Name = 'PTO'

	DElETE TE
	FROM TimeEntries TE
	JOIN @RecurringHolidaysDates rhd ON rhd.Date = TE.MilestoneDate
	WHERE TE.TimeTypeId IN (@PTOTimeTypeId, @HolidayTimeTypeId)
			
	IF @IsSet = 1
	BEGIN
		
		--If MilestonePersonId is not there for the Person in DefaultMilestone
		INSERT INTO MilestonePerson(PersonId,MilestoneId)
		SELECT Distinct P.PersonId, @DefaultMilestoneId AS [MilestoneId]
		FROM Person P
		LEFT JOIN MilestonePerson mp ON mp.MilestoneId = @DefaultMilestoneId AND P.PersonId = mp.PersonId
		JOIN Pay pay ON pay.Person = p.PersonId
		JOIN @RecurringHolidaysDates as rhd ON rhd.Date BETWEEN pay.StartDate AND ISNULL(pay.EndDate, dbo.GetFutureDate())- 1
		WHERE pay.Timescale = 2 
				AND ISNULL(P.TerminationDate, dbo.GetFutureDate()) >= @Today 
				AND  mp.MilestonePersonId IS NULL	
				
		-- if any new MilestonePersons inserted then we need to create MilestonePersonEntry for the new milestonePersonIds
		INSERT INTO MilestonePersonEntry(MilestonePersonId, StartDate, EndDate, Amount, HoursPerDay)
		SELECT Distinct mp.MilestonePersonId, m.StartDate, m.ProjectedDeliveryDate, 0, 8
		FROM MilestonePerson mp
		LEFT JOIN MilestonePersonEntry mpe on mpe.MilestonePersonId = mp.MilestonePersonId
		JOIN Pay p ON p.Person = mp.PersonId
		JOIN @RecurringHolidaysDates rhd ON rhd.Date BETWEEN p.StartDate AND p.EndDate
		JOIN Milestone m ON m.MilestoneId = mp.MilestoneId
		WHERE mp.MilestoneId = @DefaultMilestoneId AND p.Timescale = 2 AND mpe.MilestonePersonId IS NULL
		
		INSERT  INTO [dbo].[TimeEntries]
		                ( [EntryDate] ,
		                  [MilestoneDate] ,
		                  [ModifiedDate] ,
		                  [MilestonePersonId] ,
		                  [ActualHours] ,
		                  [ForecastedHours] ,
		                  [TimeTypeId] ,
		                  [ModifiedBy] ,
		                  [Note] ,
		                  [IsChargeable] ,
		                  [IsCorrect],
						  [IsAutoGenerated]
		                )
		SELECT @CurrentPMTime
				,rhd.[Date]
				,@CurrentPMTime
				,mp.MilestonePersonId
				,8
				,mpe.HoursPerDay
				,@HolidayTimeTypeId
				,@ModifiedBy
				,rhd.[Description]
				,m.IsChargeable
				,1
				,1 --Here it is Auto generated.
		FROM MilestonePerson mp 
		JOIN MilestonePersonEntry mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
		JOIN Milestone m ON m.MilestoneId = mp.MilestoneId
		JOIN Pay pay ON pay.Person = mp.PersonId  AND pay.Timescale = 2
		JOIN Person p ON p.PersonId = pay.Person
		JOIN @RecurringHolidaysDates AS rhd ON rhd.Date BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																															ELSE pay.EndDate - 1
																															END)
		LEFT JOIN TimeEntries te ON te.MilestonePersonId = mpe.MilestonePersonId AND te.MilestoneDate = rhd.Date
		WHERE  mp.MilestoneId = @DefaultMilestoneId AND te.MilestoneDate IS NULL


		UPDATE TE
			SET TE.Note = rhd.[Description],
				TE.TimeTypeId = @HolidayTimeTypeId,
				TE.ActualHours = 8
		FROM TimeEntries AS TE
		JOIN Calendar AS C ON C.Date = TE.MilestoneDate
		JOIN @RecurringHolidaysDates AS rhd ON rhd.Date = TE.MilestoneDate
		WHERE C.IsRecurring = 1 AND TE.Note <> rhd.Description
		
	END
	ELSE IF @IsSet = 0
	BEGIN
		Delete te
		FROM TimeEntries te
		JOIN @RecurringHolidaysDates AS rhd ON rhd.Date = te.MilestoneDate --RecurringHolidays are greater than or equal to TodayDate
		WHERE te.IsAutoGenerated = 1
				
		INSERT  INTO [dbo].[TimeEntries]
						( [EntryDate] ,
							[MilestoneDate] ,
							[ModifiedDate] ,
							[MilestonePersonId] ,
							[ActualHours] ,
							[ForecastedHours] ,
							[TimeTypeId] ,
							[ModifiedBy] ,
							[Note] ,
							[IsChargeable] ,
							[IsCorrect],
							[IsAutoGenerated]
						)
		SELECT @CurrentPMTime
				,d.[date]
				,@CurrentPMTime
				,mp.MilestonePersonId
				,CASE WHEN PC.ActualHours IS NOT NULL AND ISNULL(PC.IsFloatingHoliday,0) = 0 THEN PC.ActualHours ELSE 8 END
				,mpe.HoursPerDay
				,CASE WHEN PC.IsFloatingHoliday = 1 THEN @HolidayTimeTypeId  ELSE @PTOTimeTypeId END
				,@ModifiedBy
				,CASE WHEN PC.IsFloatingHoliday = 1 THEN 'Floating Holiday' ELSE 'PTO' END
				,m.IsChargeable
				,1
				,1 --Here it is Auto generated.
		FROM PersonCalendar PC
		JOIN @RecurringHolidaysDates d ON d.date = PC.Date AND PC.DayOff = 1
		JOIN MilestonePerson MP ON MP.PersonId = PC.PersonId AND MP.MilestoneId = @DefaultMilestoneId
		JOIN Person p ON p.PersonId = PC.PersonId
		JOIN Pay pay ON pay.Person = PC.PersonId AND pay.Timescale = 2 AND d.date BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																															ELSE pay.EndDate - 1
																															END)
		JOIN Milestone m ON m.MilestoneId = MP.MilestoneId
		JOIN MilestonePersonEntry mpe ON mpe.MilestonePersonId = MP.MilestonePersonId
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
