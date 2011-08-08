﻿CREATE PROCEDURE [dbo].[SetRecurringHoliday]
(
	@Id INT = NULL,
	@IsSet	BIT,
	@UserLogin NVARCHAR(255)
)
AS
BEGIN

DECLARE @Today DATETIME,
	@ModifiedBy INT

DECLARE @RecurringHolidaysDates TABLE( [Date] DATETIME, [Description] NVARCHAR(255))

SELECT @Today = dbo.GettingPMTime(GETUTCDATE())

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

	INSERT INTO @RecurringHolidaysDates([Date], [Description])
	SELECT C1.Date, crh.Description
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
		RecurringHolidayId = CASE WHEN @IsSet = 0 THEN null ELSE @Id END,
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
		
		
	
	DECLARE @CurrentPMTime DATETIME 
	DECLARE @DefaultMilestoneId INT
	
	SET @CurrentPMTime = dbo.InsertingTime()
	SELECT @DefaultMilestoneId = MilestoneId FROM DefaultMilestoneSetting	
			
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
				,dbo.GetHolidayTimeTypeId()
				,@ModifiedBy
				,rhd.[Description]
				,m.IsChargeable
				,0
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
			SET TE.Note = rhd.[Description]
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
	 
	END
	
		COMMIT TRANSACTION Tran_SetRecurringHoliday	
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION Tran_SetRecurringHoliday
		
		DECLARE @ErrorMessage NVARCHAR(2048)
		SELECT @ErrorMessage = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 16, 1)
	END CATCH

END
