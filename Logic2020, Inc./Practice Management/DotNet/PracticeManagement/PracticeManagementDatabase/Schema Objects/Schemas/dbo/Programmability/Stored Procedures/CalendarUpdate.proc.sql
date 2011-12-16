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
	@RecurringHolidayDate DATETIME,
	@ActualHours REAL,
	@IsFloatingHoliday BIT
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
			@DefaultMilestoneId INT,
			@PTOTimeTypeId INT

	SELECT @Today = dbo.GettingPMTime(GETUTCDATE())
	SELECT @DefaultMilestoneId = MilestoneId FROM DefaultMilestoneSetting
	SELECT @PTOTimeTypeId = TimeTypeId FROM TimeType WHERE Name = 'PTO'

	DECLARE @Dates TABLE ([date] DATETIME)

	BEGIN TRY
	BEGIN TRAN tran_CalendarUpdate

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
				HolidayDescription = CASE WHEN @DayOff = 0 THEN NULL ELSE @HolidayDescription END,
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
		/*
			1. If we remove PTO and company don't have holiday then we need to delete.
			2. Person kept PTO on a date, 
				company kept holiday on the same date, 
				later person removed PTO on the day from person calendar page,
				then we need to remove record in PersonCalendar table.
			3. Company has holiday,
				person wants to work on that date and later dont want to work.
		*/
		-- Clear the person record
		DELETE
		  FROM dbo.PersonCalendar
		 WHERE Date = @Date AND PersonId = @PersonId
	END
	ELSE IF EXISTS (SELECT 1
	                  FROM dbo.PersonCalendar AS pcal
	                 WHERE pcal.Date = @Date AND pcal.PersonId = @PersonId)
	BEGIN
		/*
			1. We have an updating option to update PTO/IsFloatingHoliday and actualhours in Person Calendar page, then we will update.
			2. Person kept PTO on a date, company kept holiday on the same date then we will update person dayOff=0.
		*/
		-- Update an existing person record
		UPDATE dbo.PersonCalendar
		   SET DayOff = @DayOff,
				ActualHours = @ActualHours,
				IsFloatingHoliday = @IsFloatingHoliday
		 WHERE Date = @Date AND PersonId = @PersonId
	END
	ELSE
	BEGIN
		/*
			1. To keep PTO then we need to insert entry with DayOff=1 into PersonCalendar.
			2. If company has holiday on a date and person wants to work then we need to insert an entry with dayOff = 0.
		*/
		-- A person record does not exist - create it
		INSERT INTO dbo.PersonCalendar
		            (Date, PersonId, DayOff, ActualHours, IsFloatingHoliday, IsFromTimeEntry)
		     VALUES (@Date, @PersonId, @DayOff, @ActualHours, @IsFloatingHoliday, 0)
	END

	UPDATE ca
	   SET DayOff = pc.DayOff
	  FROM dbo.PersonCalendarAuto AS ca
	       INNER JOIN dbo.v_PersonCalendar AS pc ON ca.date = pc.Date AND ca.PersonId = pc.PersonId
		   JOIN @Dates AS d ON ca.Date = d.[date]
	 WHERE (@PersonId IS NULL OR ca.PersonId = @PersonId)
	 
	
	DECLARE @CurrentPMTime DATETIME,
				@ModifiedBy INT,
				@HolidayTimeTypeId INT
				
	SET @CurrentPMTime = dbo.InsertingTime()
	SELECT @ModifiedBy = PersonId FROM Person WHERE Alias = @UserLogin
	SET @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId()

		DELETE TE 
		FROM MilestonePerson mp 
		JOIN Pay pay ON pay.Person = mp.PersonId  AND pay.Timescale = 2
		JOIN Person p ON p.PersonId = pay.Person AND p.IsStrawman = 0
		JOIN @Dates rhd ON rhd.[date] BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																															ELSE pay.EndDate - 1
																															END)
		JOIN TimeEntries TE
		ON te.MilestonePersonId = mp.MilestonePersonId AND te.MilestoneDate = rhd.Date
		AND te.TimeTypeId  IN(@HolidayTimeTypeId , @PTOTimeTypeId)
		JOIN Calendar c ON c.Date = rhd.date AND DATEPART(DW, c.date) NOT IN (1,7)
		WHERE  mp.MilestoneId = @DefaultMilestoneId 
				AND (mp.PersonId = @PersonId OR @PersonId IS NULL)

	IF @DayOff = 0
	BEGIN
		IF(@PersonId IS NULL)
		BEGIN
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
								[IsAutoGenerated],
								[IsReviewed]
							)
			SELECT @CurrentPMTime
					,d.[date]
					,@CurrentPMTime
					,mp.MilestonePersonId
					,CASE WHEN PC.ActualHours IS NOT NULL AND ISNULL(PC.IsFloatingHoliday,0) = 0 THEN PC.ActualHours ELSE 8 END
					,mpe.HoursPerDay
					,CASE WHEN PC.IsFloatingHoliday = 1 THEN @HolidayTimeTypeId ELSE @PTOTimeTypeId END
					,@ModifiedBy
					,CASE WHEN PC.IsFloatingHoliday = 1 THEN 'Floating Holiday' ELSE 'PTO' END
					,m.IsChargeable
					,1
					,1 --Here it is Auto generated.
					,CASE WHEN PC.IsFromTimeEntry <> 1 AND PC.IsFloatingHoliday <> 1 THEN 1 ELSE NULL END--Inserting PTO timeEntries with Approved Status.
			FROM PersonCalendar PC
			JOIN @Dates d ON d.date = PC.Date AND PC.DayOff = 1
			JOIN MilestonePerson MP ON MP.PersonId = PC.PersonId AND MP.MilestoneId = @DefaultMilestoneId
			JOIN Person p ON p.PersonId = PC.PersonId
			JOIN Pay pay ON pay.Person = PC.PersonId AND pay.Timescale = 2 AND d.date BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																																ELSE pay.EndDate - 1
																																END)
			JOIN Milestone m ON m.MilestoneId = MP.MilestoneId
			JOIN MilestonePersonEntry mpe ON mpe.MilestonePersonId = MP.MilestonePersonId
			WHERE P.IsStrawman = 0
		END
		ELSE
		BEGIN
			--For Non W2Salaried person.
			DELETE TE 
			FROM TimeEntries TE
			JOIN MilestonePerson MP ON MP.MilestonePersonId = TE.MilestonePersonId AND mp.MilestoneId = @DefaultMilestoneId AND MP.PersonId = @PersonId
			JOIN Person P ON P.PersonId = MP.PersonId
			LEFT JOIN Pay pay ON pay.Person = mp.PersonId  AND pay.Timescale <> 2 AND @Date BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																															ELSE pay.EndDate - 1
																															END)
			WHERE DATEPART(DW, @Date) NOT IN (1,7) AND TE.MilestoneDate = @Date AND P.IsStrawman = 0 AND TE.TimeTypeId = @PTOTimeTypeId
		END
	END
	ELSE
	BEGIN
		--If MilestonePersonId is not there for the Person in DefaultMilestone
		INSERT INTO MilestonePerson(PersonId,MilestoneId)
		SELECT Distinct P.PersonId, @DefaultMilestoneId AS [MilestoneId]
		FROM Person P
		LEFT JOIN MilestonePerson mp ON mp.MilestoneId = @DefaultMilestoneId AND P.PersonId = mp.PersonId
		JOIN Pay pay ON pay.Person = p.PersonId
		JOIN @Dates as rhd ON rhd.[date] BETWEEN pay.StartDate AND ISNULL(pay.EndDate, dbo.GetFutureDate())- 1
		WHERE pay.Timescale = 2 AND P.IsStrawman = 0
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
		JOIN Person per ON per.PersonId = p.Person
		WHERE mp.MilestoneId = @DefaultMilestoneId 
				AND p.Timescale = 2 
				AND mpe.MilestonePersonId IS NULL
				AND (mp.PersonId = @PersonId OR @PersonId IS NULL)
				AND per.IsStrawman = 0
				
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
							[IsAutoGenerated],
							[IsReviewed]
						)
		SELECT @CurrentPMTime
				,rhd.[Date]
				,@CurrentPMTime
				,mp.MilestonePersonId
				,CASE WHEN @PersonId IS NOT NULL AND c.DayOff <> @DayOff AND @ActualHours IS NOT NULL AND @IsFloatingHoliday = 0 THEN @ActualHours ELSE 8 END
				,mpe.HoursPerDay
				,CASE WHEN @PersonId IS NOT NULL AND c.DayOff <> @DayOff AND @IsFloatingHoliday = 0 THEN @PTOTimeTypeId ELSE @HolidayTimeTypeId END 
				,@ModifiedBy
				,CASE WHEN @PersonId IS NOT NULL AND c.DayOff <> @DayOff AND @IsFloatingHoliday = 0 THEN 'PTO'
						WHEN @IsFloatingHoliday = 1 THEN 'Floating Holiday'
						ELSE ISNULL(C.HolidayDescription, '') END
				,m.IsChargeable
				,1
				,1 --Here it is Auto generated.
				,CASE WHEN @PersonId IS NOT NULL AND c.DayOff <> @DayOff AND @IsFloatingHoliday = 0 AND ISNULL(PC.IsFromTimeEntry, 1) = 0 THEN 1 ELSE NULL END--Setting Approved Status flag for PTO entries from Calendar page.
		FROM MilestonePerson mp 
		JOIN MilestonePersonEntry mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
		JOIN Milestone m ON m.MilestoneId = mp.MilestoneId
		JOIN Pay pay ON pay.Person = mp.PersonId  AND pay.Timescale = 2
		JOIN Person p ON p.PersonId = pay.Person
		JOIN @Dates rhd ON rhd.[date] BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																															ELSE pay.EndDate - 1
																															END)
		LEFT JOIN TimeEntries te 
		ON te.MilestonePersonId = mpe.MilestonePersonId AND te.MilestoneDate = rhd.Date
		AND (@PersonId IS NULL AND te.TimeTypeId = @HolidayTimeTypeId OR @PersonId IS NOT NULL AND te.TimeTypeId = @PTOTimeTypeId)
		JOIN Calendar c ON c.Date = rhd.date AND DATEPART(DW, c.date) NOT IN (1,7)
		LEFT JOIN PersonCalendar PC ON PC.PersonId = P.PersonId AND PC.Date = c.Date
		WHERE  mp.MilestoneId = @DefaultMilestoneId AND P.IsStrawman = 0
				AND (te.MilestoneDate IS NULL) 
				AND (mp.PersonId = @PersonId OR @PersonId IS NULL)

		-- For non-W2Salaried person.
		IF @PersonId IS NOT NULL
		BEGIN
			IF @IsFloatingHoliday = 1
			BEGIN
				--For Non W2Salaryed person.
				DELETE TE 
				FROM TimeEntries TE
				JOIN MilestonePerson MP ON MP.MilestonePersonId = TE.MilestonePersonId AND mp.MilestoneId = @DefaultMilestoneId AND MP.PersonId = @PersonId
				JOIN Person P ON P.PersonId = MP.PersonId
				LEFT JOIN Pay pay ON pay.Person = mp.PersonId  AND pay.Timescale <> 2 AND @Date BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																																ELSE pay.EndDate - 1
																																END)
				WHERE DATEPART(DW, @Date) NOT IN (1,7) AND TE.MilestoneDate = @Date AND P.IsStrawman = 0 AND TE.TimeTypeId = @PTOTimeTypeId

			END
			ELSE
			BEGIN

				UPDATE TE
				SET TE.ActualHours = @ActualHours,
					IsAutoGenerated = 1
				FROM TimeEntries TE
				JOIN MilestonePerson MP ON MP.MilestonePersonId = TE.MilestonePersonId AND mp.MilestoneId = @DefaultMilestoneId AND MP.PersonId = @PersonId
				JOIN Person P ON P.PersonId = MP.PersonId
				LEFT JOIN Pay pay ON pay.Person = mp.PersonId  AND pay.Timescale <> 2 AND @Date BETWEEN pay.StartDate AND (CASE WHEN p.TerminationDate IS NOT NULL AND pay.EndDate - 1 > p.TerminationDate THEN p.TerminationDate
																																ELSE pay.EndDate - 1
																																END)
				WHERE DATEPART(DW, @Date) NOT IN (1,7) AND TE.MilestoneDate = @Date AND P.IsStrawman = 0 AND TE.TimeTypeId = @PTOTimeTypeId
			END

		END
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

