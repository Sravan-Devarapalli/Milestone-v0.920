-- =============================================
-- Author:		Srinivas.M
-- Create date: 
-- Updated by:	ThulasiRam.P
-- Update date:	12-06-2012
-- =============================================
CREATE PROCEDURE [dbo].[SaveTimeTrack]
(
	@TimeEntriesXml		XML,
	@PersonId			INT,
	@StartDate			DATETIME,
	@EndDate			DATETIME,
	@UserLogin			NVARCHAR(255)
)
AS
BEGIN
	/*
	<Sections>
		<Section Id="">
			<AccountAndProjectSelection AccountId="" AccountName="" ProjectId="" ProjectName="" ProjectNumber="" BusinessUnitId="" BusinessUnitName="">
				<WorkType Id="">
					<CalendarItem Date="" CssClass="">
						<TimeEntryRecord ActualHours="" Note="" IsChargeable="" EntryDate="" IsCorrect="" IsReviewed="" ApprovedById=""></TimeEntryRecord>
							.
							.
					</CalendarItem>
						.
						.
				</WorkType>
					.
					.
			</AccountAndProjectSelection>
				.
				.
		</Section>
			.
			.
	</Sections>
	*/

	SET NOCOUNT ON;

	DECLARE @CurrentPMTime		DATETIME,
			@ModifiedBy			INT,
			@HolidayTimeTypeId	INT,
			@ORTTimeTypeId		INT,
			@UnpaidTimeTypeId	INT,
			@W2SalaryId			INT,
			@W2HourlyId			INT

	DECLARE @PersonCalendarLog TABLE(
										Id					INT,
										StartDate			DATETIME,
										EndDate			DATETIME,
										[PersonId]			INT,
										[DayOff]			BIT,
										[ActualHours]		REAL,
										[IsSeries]			BIT,
										[TimeTypeId]		INT,
										[SubstituteDate]	DATETIME ,
										[Description]		NVARCHAR(500) ,
										[IsFromTimeEntry]	BIT ,
										[ApprovedBy]		INT ,
										[SeriesId]          BIGINT,
										IsUpdate			INT, --P.IsUpdate = 0 FOR INSERT ,1 FOR UPDATE,2 FOR DELETE
										IsNewRow			BIT   --IsNewRow = 1 for insert into table,0 for others
										)

	SELECT @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId(),
		   @CurrentPMTime	  = dbo.InsertingTime(), 
		   @ORTTimeTypeId	  = dbo.GetORTTimeTypeId(), 
		   @UnpaidTimeTypeId  = dbo.GetUnpaidTimeTypeId()

	SELECT @W2SalaryId = TimescaleId FROM Timescale WHERE Name = 'W2-Salary'
	SELECT @W2HourlyId = TimescaleId FROM Timescale WHERE Name = 'W2-Hourly'

	SELECT @ModifiedBy = P.PersonId
	FROM Person P
	WHERE P.Alias = @UserLogin

	BEGIN TRY
		BEGIN TRAN TimeEntry

		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

		DECLARE @ThisWeekTimeEntries TABLE (ClientId            INT	NOT NULL,
											ProjectGroupId      INT	NOT NULL,
											ProjectId           INT	NOT NULL,
											TimeTypeId          INT	NOT NULL,
											TimeEntrySectionId	INT	NOT NULL,
											ChargeCodeDate      DATETIME	NOT NULL,
											ActualHours         REAL		NOT NULL,
											IsChargeable        BIT			NOT NULL,
											[Note]              VARCHAR (1000)	NOT NULL,
											OldTimeTypeId       INT	NULL,
											ApprovedById        INT NULL
										   )

		--- Execute a SELECT stmt using OPENXML row set provider.
		INSERT INTO @ThisWeekTimeEntries
		SELECT NEW.c.value('..[1]/..[1]/..[1]/@AccountId', 'INT'),
			   NEW.c.value('..[1]/..[1]/..[1]/@BusinessUnitId', 'INT'),
			   NEW.c.value('..[1]/..[1]/..[1]/@ProjectId', 'INT'),
			   NEW.c.value('..[1]/..[1]/@Id', 'INT'),
			   NEW.c.value('..[1]/..[1]/..[1]/..[1]/@Id', 'INT'),
			   NEW.c.value('..[1]/@Date', 'DATETIME'),
			   NEW.c.value('@ActualHours', 'REAL'),
			   NEW.c.value('@IsChargeable', 'BIT'),
			   NEW.c.value('@Note', 'NVARCHAR(1000)'),
			   NEW.c.value('..[1]/..[1]/@OldId', 'INT'),
			   NEW.c.value('@ApprovedById', 'INT')
		FROM @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType/CalendarItem/TimeEntryRecord') NEW(c)
		

		--Insert ChargeCode if not exists in ChargeCode Table.
		INSERT INTO dbo.ChargeCode(ClientId, ProjectGroupId, ProjectId, PhaseId, TimeTypeId, TimeEntrySectionId)
		SELECT DISTINCT  TWTE.ClientId,
				TWTE.ProjectGroupId,
				TWTE.ProjectId,
				01,
				TWTE.TimeTypeId,
				TWTE.TimeEntrySectionId
		FROM @ThisWeekTimeEntries AS TWTE
		LEFT JOIN dbo.ChargeCode CC ON CC.ClientId = TWTE.ClientId
							AND CC.ProjectGroupId = TWTE.ProjectGroupId
							AND CC.ProjectId = TWTE.ProjectId
							AND CC.TimeTypeId = TWTE.TimeTypeId
		WHERE CC.Id IS NULL AND TWTE.TimeTypeId > 0


		--Delete timeEntries which are not exists in the xml and timeEntries having ActualHours=0 in xml.
		DELETE TEH
		FROM dbo.TimeEntry TE
		INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId  
		INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND TE.PersonId = @PersonId AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate
		LEFT  JOIN @ThisWeekTimeEntries AS TWTE ON TE.ChargeCodeDate = TWTE.ChargeCodeDate
				AND CC.ClientId = TWTE.ClientId
				AND CC.ProjectGroupId = TWTE.ProjectGroupId
				AND CC.ProjectId = TWTE.ProjectId
				AND CC.TimeTypeId = TWTE.TimeTypeId
				AND TWTE.ActualHours > 0
				AND TWTE.IsChargeable = TEH.IsChargeable
		WHERE TWTE.ClientId IS NULL


		DELETE TE
		FROM dbo.TimeEntry TE
		LEFT JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
		WHERE TEH.Id IS NULL AND TE.PersonId = @PersonId AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate

		--Update TimeEntries which are modified.
		UPDATE TEH
		SET	TEH.ActualHours = TWTE.ActualHours,
			TEH.ModifiedDate = @CurrentPMTime,
			TEH.ModifiedBy = @ModifiedBy
		FROM dbo.TimeEntry TE
		INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId   
		INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND TE.PersonId = @PersonId
		INNER JOIN @ThisWeekTimeEntries AS TWTE
			ON TWTE.ChargeCodeDate = TE.ChargeCodeDate
				AND CC.ClientId = TWTE.ClientId
				AND CC.ProjectGroupId = TWTE.ProjectGroupId
				AND CC.ProjectId = TWTE.ProjectId
				AND CC.TimeTypeId = TWTE.TimeTypeId
				AND TEH.IsChargeable = TWTE.IsChargeable
				AND (
						TEH.ActualHours <> TWTE.ActualHours OR
						TE.Note <> TWTE.Note  --Added to fire the trigger on table 'TimeEntryHours' When note Changed.
				     )

		UPDATE TE
		SET	TE.Note = CASE WHEN TT.IsAdministrative = 1 AND TWTE.Note = '' THEN TT.Name + '.' ELSE TWTE.Note END
		FROM dbo.TimeEntry TE
		INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId   
		INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND TE.PersonId = @PersonId
		INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId AND @HolidayTimeTypeId <> TT.TimeTypeId AND @UnpaidTimeTypeId <> TT.TimeTypeId
		INNER JOIN @ThisWeekTimeEntries AS TWTE
			ON TWTE.ChargeCodeDate = TE.ChargeCodeDate
				AND CC.ClientId = TWTE.ClientId
				AND CC.ProjectGroupId = TWTE.ProjectGroupId
				AND CC.ProjectId = TWTE.ProjectId
				AND CC.TimeTypeId = TWTE.TimeTypeId	
				AND TEH.IsChargeable = TWTE.IsChargeable
				AND ( TE.Note <> TWTE.Note )

	

		--Insert any new entries exists.
		INSERT INTO dbo.TimeEntry(PersonId, 
								ChargeCodeId, 
								ChargeCodeDate,
								ForecastedHours,
								Note,
								IsCorrect,
								IsAutoGenerated)
		SELECT DISTINCT @PersonId,
				CC.Id,
				TWTE.ChargeCodeDate,
				0,
				CASE WHEN TT.IsAdministrative = 1 AND ( TWTE.Note = '' OR ISNULL(OldTT.Name,'') + '.' = TWTE.Note ) THEN TT.Name + '.' ELSE TWTE.Note END,
				1,
				0
		FROM @ThisWeekTimeEntries AS TWTE
		INNER JOIN dbo.ChargeCode CC ON CC.ClientId = TWTE.ClientId
								AND CC.ProjectGroupId = TWTE.ProjectGroupId
								AND CC.ProjectId = TWTE.ProjectId
								AND CC.TimeTypeId = TWTE.TimeTypeId	
		INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId AND TT.TimeTypeId <> @HolidayTimeTypeId AND TT.TimeTypeId <> @UnpaidTimeTypeId
		LEFT JOIN dbo.TimeEntry TE ON TE.ChargeCodeId = CC.Id AND TE.PersonId = @PersonId
										AND TE.ChargeCodeDate = TWTE.ChargeCodeDate
		LEFT JOIN dbo.TimeType OldTT ON OldTT.TimeTypeId = TWTE.OldTimeTypeId
		WHERE TE.TimeEntryId IS NULL
			AND TWTE.ActualHours > 0


		INSERT INTO dbo.TimeEntryHours(TimeEntryId,
									   ActualHours,
									   IsChargeable,
										CreateDate,
										ModifiedDate,
										ModifiedBy,
										ReviewStatusId)
		SELECT  TE.TimeEntryId,
				TWTE.ActualHours,
				TWTE.IsChargeable,
				@CurrentPMTime,
				@CurrentPMTime,
				@ModifiedBy,
				1-- pending status
		FROM  @ThisWeekTimeEntries AS TWTE
		INNER JOIN dbo.ChargeCode AS CC ON CC.ClientId = TWTE.ClientId
								AND CC.ProjectGroupId = TWTE.ProjectGroupId
								AND CC.ProjectId = TWTE.ProjectId
								AND CC.TimeTypeId = TWTE.TimeTypeId		
		INNER JOIN dbo.TimeEntry TE ON TE.ChargeCodeId = CC.Id AND TE.PersonId = @PersonId
										AND TE.ChargeCodeDate = TWTE.ChargeCodeDate
		LEFT JOIN dbo.TimeEntryHours TEH ON (  TEH.TimeEntryId = TE.TimeEntryId
											   AND TEH.IsChargeable = TWTE.IsChargeable
											)
		WHERE TEH.TimeEntryId IS NULL
			AND TWTE.ActualHours > 0

		/*
			Delete PersonCalendar Entry Only,
			1. if Time Off entered from Calendar page and (w2salaried/w2Hourly) person on that date and there is no entry in XML(note:Excluding holiday and unpaid time types).
			2. if Time Off entered from any where / updated time Off from time Entry page and now there is no entry in XML.
		*/
		--Delete PTO entry from PersonCalendar only if the Person has PTO not Floating Holiday.
		
		-- To insert into activitylog from timeentry page,we are inserting records into @Personcalendarlog when update/delete/insert operations happens on Timeentry table.
		DECLARE @TimeOffDates TABLE(Date DATETIME)
		DECLARE @SeriesIds TABLE(Id INT)

		INSERT INTO @TimeOffDates
		SELECT PC.Date
		FROM dbo.PersonCalendar PC
		INNER JOIN dbo.Calendar C ON C.Date = PC.Date AND PC.PersonId =  @PersonId AND PC.Date BETWEEN @StartDate AND @EndDate
		LEFT  JOIN @ThisWeekTimeEntries AS TWTE
				 ON TWTE.TimeEntrySectionId = 4
					AND TWTE.ChargeCodeDate = PC.Date
					AND TWTE.TimeTypeId <> @HolidayTimeTypeId
					AND TWTE.TimeTypeId <> @UnpaidTimeTypeId
		INNER JOIN dbo.Pay AS pay ON pay.Person = PC.PersonId AND PC.Date BETWEEN pay.StartDate AND (pay.EndDate - 1) AND pay.Timescale IN (@W2SalaryId,@W2HourlyId)
								  AND PC.TimeTypeId <> @HolidayTimeTypeId AND PC.TimeTypeId <> @UnpaidTimeTypeId AND C.DayOff <> 1 
		WHERE ISNULL(TWTE.ActualHours, 0) = 0 --can delete only if it is entered for the time entry page and not floating holiday
		
		INSERT INTO @PersonCalendarLog(Id,PersonId,StartDate,EndDate,SeriesId)
		SELECT	ROW_NUMBER() OVER(ORDER BY P.Date),
				@PersonId,
				P.Date,
				P.Date ,
				P.SeriesId
		FROM dbo.PersonCalendar P
		INNER JOIN @TimeOffDates T ON T.Date = P.Date
		AND P.PersonId = @PersonId 

		UPDATE  P
		SET P.DayOff = PC.DayOff,
			P.ActualHours = PC.ActualHours,
			P.IsSeries = PC.IsSeries,
			P.TimeTypeId = PC.TimeTypeId,
			P.SubstituteDate = PC.SubstituteDate,
			P.Description = PC.Description,
			P.IsFromTimeEntry =1,
			P.ApprovedBy = PC.ApprovedBy,
			P.IsUpdate = 2,
			P.IsNewRow = 0
		FROM @PersonCalendarLog P
		JOIN dbo.PersonCalendar PC ON PC.PersonId = P.PersonId
		JOIN @TimeOffDates T ON T.Date = PC.Date AND P.IsUpdate IS NULL
					
		DELETE PC
		FROM dbo.PersonCalendar PC
		INNER JOIN dbo.Calendar C ON C.Date = PC.Date AND PC.PersonId =  @PersonId AND PC.Date BETWEEN @StartDate AND @EndDate
		LEFT  JOIN @ThisWeekTimeEntries AS TWTE
				 ON TWTE.TimeEntrySectionId = 4
					AND TWTE.ChargeCodeDate = PC.Date
					AND TWTE.TimeTypeId <> @HolidayTimeTypeId
					AND TWTE.TimeTypeId <> @UnpaidTimeTypeId
		INNER JOIN dbo.Pay AS pay ON pay.Person = PC.PersonId AND PC.Date BETWEEN pay.StartDate AND (pay.EndDate - 1) AND pay.Timescale IN (@W2SalaryId,@W2HourlyId)
								  AND PC.TimeTypeId <> @HolidayTimeTypeId AND PC.TimeTypeId <> @UnpaidTimeTypeId AND C.DayOff <> 1 
		WHERE ISNULL(TWTE.ActualHours, 0) = 0 --can delete only if it is entered for the time entry page and not floating holiday

		--Update administrative time type ACTUAL HOURS in person calendar table(note:Excluding HOLIDAY and UNPAID time types) and APPROVED BY for the ORT.
		DELETE @TimeOffDates
		INSERT INTO @TimeOffDates(Date)
		SELECT PC.Date
		FROM dbo.PersonCalendar PC
		INNER JOIN @ThisWeekTimeEntries AS TWTE
			ON TWTE.TimeEntrySectionId = 4
				AND TWTE.ChargeCodeDate = PC.Date
				AND TWTE.TimeTypeId <> @HolidayTimeTypeId
				AND TWTE.TimeTypeId <> @UnpaidTimeTypeId
				AND TWTE.ActualHours > 0
			INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = TWTE.TimeTypeId
			LEFT  JOIN dbo.TimeType OldTT ON OldTT.TimeTypeId = TWTE.OldTimeTypeId
		WHERE PC.PersonId = @PersonId AND PC.Date BETWEEN @StartDate AND @EndDate AND PC.DayOff = 1
			AND (TWTE.ActualHours <> PC.ActualHours 
				OR PC.TimeTypeId <> TWTE.TimeTypeId 
				OR PC.Description <> TWTE.Note
				OR PC.ApprovedBy <> CASE WHEN TWTE.TimeTypeId = @ORTTimeTypeId THEN TWTE.ApprovedById  ELSE PC.ApprovedBy END)

		INSERT INTO @PersonCalendarLog(Id,PersonId,StartDate,EndDate,SeriesId)
		SELECT	ROW_NUMBER() OVER(ORDER BY P.Date),
				@PersonId,
				P.Date,
				P.Date,
				P.SeriesId
		FROM dbo.PersonCalendar P
		WHERE P.Date IN (SELECT Date FROM @TimeOffDates)
		AND P.PersonId = @PersonId 

		UPDATE  P
		SET P.DayOff = PC.DayOff,
			P.ActualHours = PC.ActualHours,
			P.IsSeries = PC.IsSeries,
			P.TimeTypeId = PC.TimeTypeId,
			P.SubstituteDate = PC.SubstituteDate,
			P.Description = PC.Description,
			P.IsFromTimeEntry =PC.IsFromTimeEntry,
			P.ApprovedBy = PC.ApprovedBy,
			P.IsUpdate = 1,
			P.IsNewRow = 0
		FROM @PersonCalendarLog P
		JOIN dbo.PersonCalendar PC ON PC.PersonId = P.PersonId
		JOIN @TimeOffDates T ON T.Date = PC.Date AND P.IsUpdate IS NULL

		UPDATE PC
		SET	PC.ActualHours = TWTE.ActualHours,
			PC.IsFromTimeEntry = 1,
			PC.TimeTypeId = TWTE.TimeTypeId,
			PC.Description = CASE WHEN TT.IsAdministrative = 1 AND ( TWTE.Note = '' OR ISNULL(OldTT.Name,'') + '.' = TWTE.Note ) THEN TT.Name + '.' ELSE TWTE.Note END,
			PC.ApprovedBy = CASE WHEN TWTE.TimeTypeId = @ORTTimeTypeId THEN TWTE.ApprovedById  ELSE @ModifiedBy END
		FROM dbo.PersonCalendar PC
		INNER JOIN @ThisWeekTimeEntries AS TWTE
			ON TWTE.TimeEntrySectionId = 4
				AND TWTE.ChargeCodeDate = PC.Date
				AND TWTE.TimeTypeId <> @HolidayTimeTypeId
				AND TWTE.TimeTypeId <> @UnpaidTimeTypeId
				AND TWTE.ActualHours > 0
			INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = TWTE.TimeTypeId
			LEFT  JOIN dbo.TimeType OldTT ON OldTT.TimeTypeId = TWTE.OldTimeTypeId
		WHERE PC.PersonId = @PersonId AND PC.Date BETWEEN @StartDate AND @EndDate AND PC.DayOff = 1
			AND (TWTE.ActualHours <> PC.ActualHours 
				OR PC.TimeTypeId <> TWTE.TimeTypeId 
				OR PC.Description <> TWTE.Note
				OR PC.ApprovedBy <> CASE WHEN TWTE.TimeTypeId = @ORTTimeTypeId THEN TWTE.ApprovedById  ELSE PC.ApprovedBy END)
		
		
		INSERT INTO @PersonCalendarLog(Id,PersonId,StartDate,EndDate,SeriesId)
		SELECT	ROW_NUMBER() OVER(ORDER BY P.Date),
				@PersonId,
				P.Date,
				P.Date,
				P.SeriesId
		FROM dbo.PersonCalendar P
		WHERE P.Date IN (SELECT Date FROM @TimeOffDates)
		AND P.PersonId = @PersonId 

		UPDATE  P
		SET P.DayOff = PC.DayOff,
			P.ActualHours = PC.ActualHours,
			P.IsSeries = PC.IsSeries,
			P.TimeTypeId = PC.TimeTypeId,
			P.SubstituteDate = PC.SubstituteDate,
			P.Description = PC.Description,
			P.IsFromTimeEntry =1,
			P.ApprovedBy = PC.ApprovedBy,
			P.IsUpdate = 1,
			P.IsNewRow = 1
		FROM @PersonCalendarLog P
		JOIN dbo.PersonCalendar PC ON PC.PersonId = P.PersonId
		JOIN @TimeOffDates T ON T.Date = PC.Date AND P.IsUpdate IS NULL
		--Insert administrative TIME TYPE time entries in person calendar table(note:Excluding HOLIDAY and UNPAID time types).
		
		DELETE @TimeOffDates

		INSERT INTO @TimeOffDates(Date)
		SELECT TWTE.ChargeCodeDate
		FROM dbo.PersonCalendar PC
			RIGHT JOIN @ThisWeekTimeEntries AS TWTE
				ON PC.PersonId =  @PersonId
					AND TWTE.ChargeCodeDate = PC.Date
		INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = TWTE.TimeTypeId
		LEFT  JOIN dbo.TimeType OldTT ON OldTT.TimeTypeId = TWTE.OldTimeTypeId
		WHERE TWTE.TimeEntrySectionId = 4
				AND TWTE.TimeTypeId <> @HolidayTimeTypeId
				AND TWTE.TimeTypeId <> @UnpaidTimeTypeId
				AND TWTE.ActualHours > 0
				AND PC.Date IS NULL
		
		INSERT INTO PersonCalendar(Date,
									PersonId,
									DayOff,
									ActualHours,
									TimeTypeId,
									Description,
									IsFromTimeEntry,
									ApprovedBy
									)
		SELECT TWTE.ChargeCodeDate,
				@PersonId,
				1,
				TWTE.ActualHours,
				TWTE.TimeTypeId,
				CASE WHEN TT.IsAdministrative = 1 AND ( TWTE.Note = '' OR ISNULL(OldTT.Name,'') + '.' = TWTE.Note ) THEN TT.Name + '.' ELSE TWTE.Note END,
				1,
				CASE WHEN TWTE.TimeTypeId = @ORTTimeTypeId THEN TWTE.ApprovedById  ELSE @ModifiedBy END  
		FROM dbo.PersonCalendar PC
		RIGHT JOIN @ThisWeekTimeEntries AS TWTE
				ON PC.PersonId =  @PersonId
					AND TWTE.ChargeCodeDate = PC.Date
		INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = TWTE.TimeTypeId
		LEFT  JOIN dbo.TimeType OldTT ON OldTT.TimeTypeId = TWTE.OldTimeTypeId
		WHERE TWTE.TimeEntrySectionId = 4
				AND TWTE.TimeTypeId <> @HolidayTimeTypeId
				AND TWTE.TimeTypeId <> @UnpaidTimeTypeId
				AND TWTE.ActualHours > 0
				AND PC.Date IS NULL

		DELETE @SeriesIds

		INSERT INTO @SeriesIds
		SELECT DISTINCT PC.SeriesId
		FROM dbo.PersonCalendar PC
		WHERE PC.Date IN (SELECT Date FROM @TimeOffDates)
		AND PC.PersonId = @PersonId
		
		INSERT INTO @PersonCalendarLog(Id,StartDate,EndDate,SeriesId)
		SELECT	ROW_NUMBER() OVER(ORDER BY MIN(PC.Date)),
				MIN(PC.Date),
				MAX(PC.Date),
				PC.SeriesId
		FROM dbo.PersonCalendar PC
		WHERE PC.SeriesId IN (SELECT Id FROM @SeriesIds)
		AND PC.PersonId = @PersonId 
		GROUP BY PC.SeriesId
	UPDATE  P
		SET P.DayOff = PC.DayOff,
			P.PersonId = PC.PersonId,
			P.ActualHours = PC.ActualHours,
			P.IsSeries = PC.IsSeries,
			P.TimeTypeId = PC.TimeTypeId,
			P.SubstituteDate = PC.SubstituteDate,
			P.Description = PC.Description,
			P.IsFromTimeEntry =1,
			P.ApprovedBy = PC.ApprovedBy,
			P.IsUpdate = 0,
			P.IsNewRow = 1
		FROM @PersonCalendarLog P
		JOIN dbo.PersonCalendar PC ON PC.SeriesId = P.SeriesId
		WHERE P.SeriesId IN (SELECT Id FROM @SeriesIds) AND P.IsUpdate IS NULL 
		--To insert to Activitylog 
		EXEC dbo.SessionLogUnprepare
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	;WITH NEW_VALUES AS
	(
		SELECT	P.Id,
				CONVERT(NVARCHAR(10), P.StartDate, 101) AS [StartDate],
				CONVERT(NVARCHAR(10), P.EndDate, 101) AS [EndDate],	
				P.PersonId,
				per1.LastName+', '+per1.FirstName AS PersonName,
				CAST(P.ActualHours as decimal(20,2)) AS ActualHours,
				CASE WHEN P.IsFromTimeEntry = 1 THEN 'YES' ELSE 'NO' END IsFromTimeEntry,
				P.TimeTypeId,
				TT.Name AS TimeTypeName,
				P.Description AS Notes,
				P.ApprovedBy AS ApprovedPersonId,
				ISNULL(per2.LastName+', '+per2.FirstName,'') AS ApprovedBy,
				P.SeriesId
		FROM @PersonCalendarLog P
		LEFT JOIN dbo.Person per1 ON per1.PersonId = P.PersonId
		LEFT JOIN dbo.TimeType TT ON TT.TimeTypeId = P.TimeTypeId
		LEFT JOIN dbo.Person per2 ON per2.PersonId = P.ApprovedBy
		WHERE P.IsNewRow = 1 			
	),

	OLD_VALUES AS
	(
		SELECT	P.Id,
				CONVERT(NVARCHAR(10), P.StartDate, 101) AS [StartDate],
				CONVERT(NVARCHAR(10), P.EndDate, 101) AS [EndDate],	
				P.PersonId,
				per1.LastName+', '+per1.FirstName AS PersonName,
				CAST(P.ActualHours as decimal(20,2)) AS ActualHours,
				CASE WHEN P.IsFromTimeEntry = 1 THEN 'YES' ELSE 'NO' END IsFromTimeEntry,
				P.TimeTypeId,
				TT.Name AS TimeTypeName,
				P.Description AS Notes,
				P.ApprovedBy AS ApprovedPersonId,
				ISNULL(per2.LastName+', '+per2.FirstName,'') AS ApprovedBy,
				P.SeriesId
		FROM @PersonCalendarLog P
		LEFT JOIN dbo.Person per1 ON per1.PersonId = P.PersonId
		LEFT JOIN dbo.TimeType TT ON TT.TimeTypeId = P.TimeTypeId
		LEFT JOIN dbo.Person per2 ON per2.PersonId = P.ApprovedBy
		WHERE P.IsNewRow = 0 			
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
					         FULL JOIN OLD_VALUES ON NEW_VALUES.PersonId = OLD_VALUES.PersonId AND NEW_VALUES.Id = OLD_VALUES.Id AND NEW_VALUES.StartDate = OLD_VALUES.StartDate
							 WHERE (NEW_VALUES.PersonId = ISNULL(i.PersonId, d.PersonId) OR OLD_VALUES.PersonId = ISNULL(i.PersonId, d.PersonId)) AND (NEW_VALUES.Id = ISNULL(i.Id, d.Id) OR OLD_VALUES.Id = ISNULL(i.Id, d.Id)) AND (NEW_VALUES.StartDate = ISNULL(i.StartDate, d.StartDate) OR OLD_VALUES.StartDate = ISNULL(i.StartDate, d.StartDate))
					  FOR XML AUTO, ROOT('PersonCalendar'))),
		   LogData = (SELECT 
						 NEW_VALUES.[StartDate] 
						,NEW_VALUES.[EndDate]
						,NEW_VALUES.PersonId
						,NEW_VALUES.PersonName
						,NEW_VALUES.ActualHours
						,NEW_VALUES.IsFromTimeEntry
						,NEW_VALUES.TimeTypeId 
						,NEW_VALUES.TimeTypeName
						,NEW_VALUES.Notes 
						,NEW_VALUES.ApprovedPersonId 
						,NEW_VALUES.ApprovedBy
						,NEW_VALUES.SeriesId
						,OLD_VALUES.[StartDate] 
						,OLD_VALUES.[EndDate]
						,OLD_VALUES.PersonId
						,OLD_VALUES.PersonName
						,OLD_VALUES.ActualHours
						,OLD_VALUES.IsFromTimeEntry
						,OLD_VALUES.TimeTypeId 
						,OLD_VALUES.TimeTypeName
						,OLD_VALUES.Notes 
						,OLD_VALUES.ApprovedPersonId 
						,OLD_VALUES.ApprovedBy
						,OLD_VALUES.SeriesId
					  FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.PersonId = OLD_VALUES.PersonId AND NEW_VALUES.Id = OLD_VALUES.Id AND NEW_VALUES.StartDate = OLD_VALUES.StartDate
			            WHERE (NEW_VALUES.PersonId = ISNULL(i.PersonId , d.PersonId ) OR OLD_VALUES.PersonId = ISNULL(i.PersonId , d.PersonId)) AND (NEW_VALUES.Id = ISNULL(i.Id , d.Id ) OR OLD_VALUES.Id = ISNULL(i.Id , d.Id)) AND (NEW_VALUES.StartDate = ISNULL(i.StartDate , d.StartDate) OR OLD_VALUES.StartDate= ISNULL(i.StartDate, d.StartDate))
					FOR XML AUTO, ROOT('PersonCalendar'), TYPE),
					@CurrentPMTime
	  FROM NEW_VALUES AS i
	       FULL JOIN OLD_VALUES AS d ON i.PersonId = d.PersonId AND i.Id = d.Id 
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	  WHERE  i.StartDate <> d.StartDate
	  OR i.EndDate <> d.EndDate
	  OR i.ActualHours <> d.ActualHours
	  OR i.TimeTypeId <> i.TimeTypeId
	  OR ISNULL(i.Notes, '') <> ISNULL(d.Notes, '')

	  EXEC dbo.SessionLogUnprepare

		 COMMIT TRAN TimeEntry
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN TimeEntry

		DECLARE @ErrorMessage NVARCHAR(2000)
		SET @ErrorMessage = ERROR_MESSAGE()

		RAISERROR(@ErrorMessage, 16, 1)
	END CATCH
END


