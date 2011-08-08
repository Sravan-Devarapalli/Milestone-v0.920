-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-5-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	8-12-2008
-- Description:	Updates a calendar item
-- =============================================
CREATE PROCEDURE [dbo].[CalendarUpdate]
(
	@Date       DATETIME,
	@DayOff     BIT,
	@PersonId   INT,
	@UserLogin	NVARCHAR(255),
	@IsRecurringHoliday BIT,
	@RecurringHolidayId INT,
	@HolidayDescription	NVARCHAR(255),
	@RecurringHolidayDate DATETIME
)
AS
	SET NOCOUNT ON
	
	DECLARE @Today DATETIME,
			@DefaultMilestoneId INT,
			@PTOTimeTypeId INT

	SELECT @Today = dbo.GettingPMTime(GETDATE())
	SELECT @DefaultMilestoneId = MilestoneId FROM DefaultMilestoneSetting
	SELECT @PTOTimeTypeId = TimeTypeId FROM TimeType WHERE Name = 'PTO'

	DECLARE @Dates TABLE ([date] DATETIME)

	--In case of  special recurring holidays we have choice of removing only.
	--We need to update companyRecurringHoliday table if @RecurringHolidayId has value.
	IF @RecurringHolidayId IS NOT NULL AND @DayOff = 0 AND @IsRecurringHoliday = 1 --For special Recurring holidays dates.
	BEGIN
		
		UPDATE CompanyRecurringHoliday
		SET IsSet = @DayOff
		WHERE Id = @RecurringHolidayId

		INSERT INTO @Dates([Date])
		SELECT C1.Date
		FROM dbo.Calendar AS C1
		JOIN dbo.CompanyRecurringHoliday crh ON C1.[Date] >= @Today
				AND 
				(
						(crh.[Day] IS NOT NULL --If holiday is on exact Date.
							AND (	--If Holiday comes in weekends then we need to give prior to weekdays.
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
		WHERE crh.Id = @RecurringHolidayId
	END
	ELSE  -- For Recurring Holidays setting manually.
	BEGIN
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
	END
	
	IF @PersonId IS NULL
	BEGIN
		-- Update the company calendar
		UPDATE C1
		   SET DayOff = @DayOff,
				HolidayDescription = CASE WHEN @DayOff = 0 THEN '' ELSE @HolidayDescription END,
				IsRecurring = CASE WHEN @DayOff = 0 THEN NULL ELSE @IsRecurringHoliday END, --whether it should be applied to future dates or not either dayoff 0 or 1.
				RecurringHolidayId = CASE WHEN @DayOff = 0 THEN NULL ELSE @RecurringHolidayId END, --we need to set holidayId to null while removing..
				RecurringHolidayDate = CASE WHEN @DayOff = 0 THEN NULL ELSE @RecurringHolidayDate END-- if date is not recurring OR recurring holiday Id is there then Recurringholidaydate should be null.
		FROM dbo.Calendar C1
		JOIN @Dates d ON d.[date] = C1.Date
	END
	ELSE IF EXISTS (SELECT 1
	                  FROM dbo.Calendar AS cal
	                 WHERE cal.Date = @Date AND cal.DayOff = @DayOff)
	BEGIN
		-- Clear the person record
		DELETE
		  FROM dbo.PersonCalendar
		 WHERE Date = @Date AND PersonId = @PersonId
	END
	ELSE IF EXISTS (SELECT 1
	                  FROM dbo.PersonCalendar AS pcal
	                 WHERE pcal.Date = @Date AND pcal.PersonId = @PersonId)
	BEGIN
		-- Update an existing person record
		UPDATE dbo.PersonCalendar
		   SET DayOff = @DayOff
		 WHERE Date = @Date AND PersonId = @PersonId
	END
	ELSE
	BEGIN
		-- A person record does not exist - create it
		INSERT INTO dbo.PersonCalendar
		            (Date, PersonId, DayOff)
		     VALUES (@Date, @PersonId, @DayOff)
	END

	UPDATE ca
	   SET DayOff = pc.DayOff
	  FROM dbo.PersonCalendarAuto AS ca
	       INNER JOIN dbo.v_PersonCalendar AS pc ON ca.date = pc.Date AND ca.PersonId = pc.PersonId
		   JOIN @Dates AS d ON ca.Date = d.[date]
	 WHERE (@PersonId IS NULL OR ca.PersonId = @PersonId)
	 

	IF @DayOff = 0
	BEGIN
		DELETE TE
		FROM TimeEntries TE
		JOIN @Dates crh ON crh.[date] = TE.MilestoneDate
		JOIN MilestonePerson MP ON MP.MilestonePersonId = TE.MilestonePersonId AND MP.MilestoneId = @DefaultMilestoneId
		WHERE TE.IsAutoGenerated = 1
			AND (MP.PersonId = @PersonId OR @PersonId IS NULL)
	END
	ELSE
	BEGIN
		DECLARE @CurrentPMTime DATETIME,
					@ModifiedBy INT,
					@HolidayTimeTypeId INT
				
		SET @CurrentPMTime = dbo.InsertingTime()
		SELECT @ModifiedBy = PersonId FROM Person WHERE Alias = @UserLogin
		SET @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId()
				
		--If MilestonePersonId is not there for the Person in DefaultMilestone
		INSERT INTO MilestonePerson(PersonId,MilestoneId)
		SELECT Distinct P.PersonId, @DefaultMilestoneId AS [MilestoneId]
		FROM Person P
		LEFT JOIN MilestonePerson mp ON mp.MilestoneId = @DefaultMilestoneId AND P.PersonId = mp.PersonId
		JOIN Pay pay ON pay.Person = p.PersonId
		JOIN @Dates as rhd ON rhd.[date] BETWEEN pay.StartDate AND ISNULL(pay.EndDate, dbo.GetFutureDate())- 1
		WHERE pay.Timescale = 2 
				AND ISNULL(P.TerminationDate, dbo.GetFutureDate()) >= @Today 
				AND  mp.MilestonePersonId IS NULL
				AND (P.PersonId = @PersonId OR @PersonId IS NULL)	
						
		-- if any new MilestonePersons inserted then we need to create MilestonePersonEntry for the new milestonePersonIds
		INSERT INTO MilestonePersonEntry(MilestonePersonId, StartDate, EndDate, Amount, HoursPerDay)
		SELECT Distinct mp.MilestonePersonId, m.StartDate, m.ProjectedDeliveryDate, 0, 8
		FROM MilestonePerson mp
		LEFT JOIN MilestonePersonEntry mpe on mpe.MilestonePersonId = mp.MilestonePersonId
		JOIN Pay p ON p.Person = mp.PersonId
		JOIN @Dates rhd ON rhd.[date] BETWEEN p.StartDate AND p.EndDate
		JOIN Milestone m ON m.MilestoneId = mp.MilestoneId
		WHERE mp.MilestoneId = @DefaultMilestoneId 
				AND p.Timescale = 2 
				AND mpe.MilestonePersonId IS NULL
				AND (mp.PersonId = @PersonId OR @PersonId IS NULL)
				
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
				,CASE WHEN @PersonId IS NOT NULL AND c.DayOff <> @DayOff THEN @PTOTimeTypeId ELSE @HolidayTimeTypeId END 
				,@ModifiedBy
				,CASE WHEN @PersonId IS NOT NULL AND c.DayOff <> @DayOff THEN 'PTO' ELSE C.HolidayDescription END
				,m.IsChargeable
				,0
				,1 --Here it is Auto generated.
		FROM MilestonePerson mp 
		JOIN MilestonePersonEntry mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
		JOIN Milestone m ON m.MilestoneId = mp.MilestoneId
		JOIN Pay pay ON pay.Person = mp.PersonId  AND pay.Timescale = 2
		JOIN Person p ON p.PersonId = pay.Person
		JOIN @Dates rhd ON rhd.[date] BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																															ELSE pay.EndDate - 1
																															END)
		LEFT JOIN TimeEntries te ON te.MilestonePersonId = mpe.MilestonePersonId AND te.MilestoneDate = rhd.Date
		JOIN Calendar c ON c.Date = rhd.date AND DATEPART(DW, c.date) NOT IN (1,7)
		WHERE  mp.MilestoneId = @DefaultMilestoneId 
				AND te.MilestoneDate IS NULL
				AND (mp.PersonId = @PersonId OR @PersonId IS NULL)
	END
