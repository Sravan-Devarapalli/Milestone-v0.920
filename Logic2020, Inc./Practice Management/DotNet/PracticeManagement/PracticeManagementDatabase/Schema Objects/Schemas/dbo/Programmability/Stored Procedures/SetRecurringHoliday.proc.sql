CREATE PROCEDURE [dbo].[SetRecurringHoliday]
(
	@Id INT,
	@IsSet	BIT,
	@UserLogin NVARCHAR(255)
)
AS
BEGIN

DECLARE @Month int,
	@Day int,
	@NumberInMonth int,
	@DayOfTheWeek int,
	@Today DATETIME,
	@ModifiedBy INT

SELECT @Today = dbo.GettingPMTime(GETUTCDATE())
SELECT
		@Month = [Month],
		@Day = [Day],
		@NumberInMonth= [NumberInMonth],
		@DayOfTheWeek = [DayOfTheWeek]
FROM dbo.CompanyRecurringHoliday
WHERE Id = @Id

SELECT @ModifiedBy = PersonId
FROM Person
WHERE Alias = @UserLogin

	BEGIN TRY
	
	BEGIN TRANSACTION Tran_SetRecurringHoliday	
	
	EXEC SessionLogPrepare @UserLogin = @UserLogin

	UPDATE dbo.CompanyRecurringHoliday
	SET IsSet = @IsSet
	WHERE Id = @Id

	UPDATE  C1
	SET DayOff = @IsSet
	FROM dbo.Calendar AS C1
	WHERE [Date] >= @Today
			AND 
			(
					(@Day IS NOT NULL --If holiday is on exact Date.
						AND (	--If Holiday comes in 
								DAY([Date]) = @Day AND MONTH([Date]) = @Month AND DATEPART(DW,[Date]) NOT IN(1,7)
								OR DAY(DATEADD(DD,1,[Date])) = @Day AND MONTH(DATEADD(DD,1,[Date])) = @Month  AND DATEPART(DW,[Date]) = 6
								OR DAY(DATEADD(DD,-1,[Date])) = @Day AND MONTH(DATEADD(DD,-1,[Date])) = @Month AND DATEPART(DW,[Date]) = 2
							)
					 )
					 OR
					 ( @Day IS NULL AND MONTH([Date]) = @Month
						AND (
								DATEPART(DW,[Date]) = @DayOfTheWeek
								AND (
										(@NumberInMonth IS NOT NULL
										 AND  ([Date] - DAY([Date])+1) -
														CASE WHEN (DATEPART(DW,[Date]-DAY([Date])+1))%7 <= @DayOfTheWeek 
															 THEN (DATEPART(DW,[Date]-DAY([Date])+1))%7
															 ELSE (DATEPART(DW,[Date]-DAY([Date])+1)-7)
															 END +(7*(@NumberInMonth-1))+@DayOfTheWeek = [Date]
										 )
										 OR( @NumberInMonth IS NULL 
											 AND (DATEADD(MM,1,[Date] - DAY([Date])+1)- 1) - 
													 (CASE WHEN DATEPART(DW,(DATEADD(MM,1,[Date] - DAY([Date])+1)- 1)) >= @DayOfTheWeek
														   THEN (DATEPART(DW,(DATEADD(MM,1,[Date] - DAY([Date])+1)- 1)))-7
														   ELSE (DATEPART(DW,(DATEADD(MM,1,[Date] - DAY([Date])+1)- 1)))
														   END)-(7-@DayOfTheWeek)= [Date]
										   )
									 )
							)
						
					 )
				 
				)	
	
	UPDATE C1
	SET DayOff = @IsSet
	FROM dbo.PersonCalendarAuto C1
	WHERE [Date] >= @Today
			AND 
			(
					(@Day IS NOT NULL --If holiday is on exact Date.
						AND (	--If Holiday comes in weekends then we need to give prior to weekdays.
								DAY([Date]) = @Day AND MONTH([Date]) = @Month AND DATEPART(DW,[Date]) NOT IN(1,7)
								OR DAY(DATEADD(DD,1,[Date])) = @Day AND MONTH(DATEADD(DD,1,[Date])) = @Month  AND DATEPART(DW,[Date]) = 6
								OR DAY(DATEADD(DD,-1,[Date])) = @Day AND MONTH(DATEADD(DD,-1,[Date])) = @Month AND DATEPART(DW,[Date]) = 2
							)
					 )
					 OR
					 ( @Day IS NULL AND MONTH([Date]) = @Month
						AND (
								DATEPART(DW,[Date]) = @DayOfTheWeek
								AND (
										(@NumberInMonth IS NOT NULL
										 AND  ([Date] - DAY([Date])+1) -
														CASE WHEN (DATEPART(DW,[Date]-DAY([Date])+1))%7 <= @DayOfTheWeek 
															 THEN (DATEPART(DW,[Date]-DAY([Date])+1))%7
															 ELSE (DATEPART(DW,[Date]-DAY([Date])+1)-7)
															 END +(7*(@NumberInMonth-1))+@DayOfTheWeek = [Date]
										 )
										 OR( @NumberInMonth IS NULL 
											 AND (DATEADD(MM,1,[Date] - DAY([Date])+1)- 1) - 
													 (CASE WHEN DATEPART(DW,(DATEADD(MM,1,[Date] - DAY([Date])+1)- 1)) >= @DayOfTheWeek
														   THEN (DATEPART(DW,(DATEADD(MM,1,[Date] - DAY([Date])+1)- 1)))-7
														   ELSE (DATEPART(DW,(DATEADD(MM,1,[Date] - DAY([Date])+1)- 1)))
														   END)-(7-@DayOfTheWeek)= [Date]
										   )
									 )
							)
						
					 )
				 
				)	
		
		
	
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
		JOIN CompanyRecurringHolidaysByPeriod(@Today, null) as rhd ON rhd.Id = @Id AND rhd.Date BETWEEN pay.StartDate AND ISNULL(pay.EndDate, dbo.GetFutureDate())- 1
		WHERE pay.Timescale = 2 
				AND ISNULL(P.TerminationDate, dbo.GetFutureDate()) >= @Today 
				AND  mp.MilestonePersonId IS NULL	
				
		-- if any new MilestonePersons inserted then we need to create MilestonePersonEntry for the new milestonePersonIds
		INSERT INTO MilestonePersonEntry(MilestonePersonId, StartDate, EndDate, Amount, HoursPerDay)
		SELECT Distinct mp.MilestonePersonId, m.StartDate, m.ProjectedDeliveryDate, 0, 8
		FROM MilestonePerson mp
		LEFT JOIN MilestonePersonEntry mpe on mpe.MilestonePersonId = mp.MilestonePersonId
		JOIN Pay p ON p.Person = mp.PersonId
		JOIN CompanyRecurringHolidaysByPeriod(@Today, null) rhd ON rhd.Id = @Id AND rhd.Date BETWEEN p.StartDate AND p.EndDate
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
		JOIN CompanyRecurringHolidaysByPeriod(@Today, null) rhd ON rhd.Id = @Id AND rhd.Date BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																															ELSE pay.EndDate - 1
																															END)
		LEFT JOIN TimeEntries te ON te.MilestonePersonId = mpe.MilestonePersonId AND te.MilestoneDate = rhd.Date
		WHERE  mp.MilestoneId = @DefaultMilestoneId AND te.MilestoneDate IS NULL
		
	END
	ELSE IF @IsSet = 0
	BEGIN
		Delete te
		FROM TimeEntries te
		JOIN CompanyRecurringHolidaysByPeriod(@Today, null) rhd ON rhd.Id = @Id AND rhd.Date = te.MilestoneDate --RecurringHolidays are greater than or equal to TodayDate
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
