CREATE PROCEDURE [dbo].[SaveTimeTrack]
	@TimeEntriesXml		XML,
	@PersonId			INT,
	@StartDate			DATETIME,
	@EndDate			DATETIME,
	@UserLogin			NVARCHAR(255)
AS
BEGIN
	/*
	<Sections>
		<Section Id="">
			<AccountAndProjectSelection AccountId="" AccountName="" ProjectId="" ProjectName="" ProjectNumber="" BusinessUnitId="" BusinessUnitName="">
				<WorkType Id="">
					<CalendarItem Date="" CssClass="">
						<TimeEntryRecord ActualHours="" Note="" IsChargeable="" EntryDate="" IsCorrect="" IsReviewed=""></TimeEntryRecord>
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
	DECLARE @PTOTimeTypeId	INT,
			@CurrentPMTime	DATETIME,
			@ModifiedBy		INT

	SET @PTOTimeTypeId = dbo.GetPTOTimeTypeId()
	SET @CurrentPMTime = dbo.InsertingTime()

	SELECT @ModifiedBy = P.PersonId
	FROM Person P
	WHERE P.Alias = @UserLogin

	BEGIN TRY
		BEGIN TRAN TimeEntry

		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

		--Insert ChargeCode if not exists in ChargeCode Table.
		INSERT INTO dbo.ChargeCode(ClientId, ProjectGroupId, ProjectId, PhaseId, TimeTypeId, TimeEntrySectionId)
		SELECT t.c.value('..[1]/@AccountId', 'INT') ClientId,
				t.c.value('..[1]/@BusinessUnitId', 'INT') ProjectGroupId,
				t.c.value('..[1]/@ProjectId', 'INT') ProjectId,
				01 AS 'PhaseId',
				t.c.value('@Id', 'INT') TimeTypeId,
				t.c.value('..[1]/..[1]/@Id', 'INT') TimeEntrySectionId
		FROM @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType') t(c)
		LEFT JOIN dbo.ChargeCode CC ON CC.ClientId = t.c.value('..[1]/@AccountId', 'INT')
							AND CC.ProjectGroupId = t.c.value('..[1]/@BusinessUnitId', 'INT')
							AND CC.ProjectId = t.c.value('..[1]/@ProjectId', 'INT')
							AND CC.TimeTypeId = t.c.value('@Id', 'INT')
		WHERE CC.Id IS NULL AND t.c.value('@Id', 'INT') > 0

		--Delete timeEntries which are not exists in the xml and timeEntries having ActualHours=0 in xml.
		DELETE TEH
		FROM dbo.TimeEntry TE
		INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId  
		JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND TE.PersonId = @PersonId AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate
		LEFT JOIN @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType/CalendarItem/TimeEntryRecord') NEW(c)
			ON TE.ChargeCodeDate = NEW.c.value('..[1]/@Date', 'DATETIME')
				AND CC.ClientId = NEW.c.value('..[1]/..[1]/..[1]/@AccountId', 'INT')
				AND CC.ProjectGroupId = NEW.c.value('..[1]/..[1]/..[1]/@BusinessUnitId', 'INT')
				AND CC.ProjectId = NEW.c.value('..[1]/..[1]/..[1]/@ProjectId', 'INT')
				AND CC.TimeTypeId = NEW.c.value('..[1]/..[1]/@Id', 'INT')
				AND NEW.c.value('@ActualHours', 'REAL') > 0
				AND NEW.c.value('@IsChargeable', 'BIT') = TEH.IsChargeable
		WHERE NEW.c.value('..[1]/..[1]/..[1]/@AccountId', 'INT') IS NULL


		DELETE TE
		FROM dbo.TimeEntry TE
		LEFT JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
		WHERE TEH.Id IS NULL AND TE.PersonId = @PersonId AND TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate

		--Update TimeEntries which are modified.
		UPDATE TEH
		SET	TEH.ActualHours = NEW.c.value('@ActualHours', 'REAL'),
			TEH.IsChargeable = NEW.c.value('@IsChargeable', 'BIT'),
			TEH.ModifiedDate = @CurrentPMTime,
			TEH.ModifiedBy = @ModifiedBy
		FROM dbo.TimeEntry TE
		INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId   
		JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND TE.PersonId = @PersonId
		JOIN @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType/CalendarItem/TimeEntryRecord') NEW(c)
			ON NEW.c.value('..[1]/@Date', 'DATETIME') = TE.ChargeCodeDate
				AND CC.ClientId = NEW.c.value('..[1]/..[1]/..[1]/@AccountId', 'INT')
				AND CC.ProjectGroupId = NEW.c.value('..[1]/..[1]/..[1]/@BusinessUnitId', 'INT')
				AND CC.ProjectId = NEW.c.value('..[1]/..[1]/..[1]/@ProjectId', 'INT')
				AND CC.TimeTypeId = NEW.c.value('..[1]/..[1]/@Id', 'INT')	
				AND TEH.IsChargeable = NEW.c.value('@IsChargeable', 'BIT')
				AND (
						TEH.ActualHours <> NEW.c.value('@ActualHours', 'REAL')  
						OR TE.Note <> NEW.c.value('@Note', 'NVARCHAR(1000)')
				     )

		UPDATE TE
		SET	TE.Note = NEW.c.value('@Note', 'NVARCHAR(1000)')
		FROM dbo.TimeEntry TE
		INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId   
		INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND TE.PersonId = @PersonId
		INNER JOIN @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType/CalendarItem/TimeEntryRecord') NEW(c)
			ON NEW.c.value('..[1]/@Date', 'DATETIME') = TE.ChargeCodeDate
				AND CC.ClientId = NEW.c.value('..[1]/..[1]/..[1]/@AccountId', 'INT')
				AND CC.ProjectGroupId = NEW.c.value('..[1]/..[1]/..[1]/@BusinessUnitId', 'INT')
				AND CC.ProjectId = NEW.c.value('..[1]/..[1]/..[1]/@ProjectId', 'INT')
				AND CC.TimeTypeId = NEW.c.value('..[1]/..[1]/@Id', 'INT')	
				AND TEH.IsChargeable = NEW.c.value('@IsChargeable', 'BIT')
				AND ( TE.Note <> NEW.c.value('@Note', 'NVARCHAR(1000)'))

		;WITH ForecastedHours AS 
		(
			SELECT MP.PersonId, M.ProjectId, C.Date, SUM(MPE.HoursPerDay) AS 'ForcastedHours'
			FROM @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType/CalendarItem/TimeEntryRecord') NEW(c)
			JOIN dbo.Milestone M ON M.ProjectId = NEW.c.value('..[1]/..[1]/..[1]/@ProjectId', 'INT')
			JOIN dbo.MilestonePerson MP ON MP.PersonId = @PersonId AND M.MilestoneId = MP.MilestoneId
			JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId AND NEW.c.value('..[1]/@Date', 'DATETIME') BETWEEN MPE.StartDate AND MPE.EndDate
			JOIN dbo.Calendar C ON C.Date = NEW.c.value('..[1]/@Date', 'DATETIME')
			GROUP BY MP.PersonId, M.ProjectId, MP.PersonId, C.Date
		)

		--Insert any new entries exists.
		INSERT INTO dbo.TimeEntry(PersonId, 
								ChargeCodeId, 
								ChargeCodeDate,
								ForecastedHours,
								Note,
								IsCorrect,
								IsAutoGenerated)
		SELECT @PersonId,
				CC.Id,
				NEW.c.value('..[1]/@Date', 'DATETIME'),
				ISNULL(FH.ForcastedHours, 0),
				CASE WHEN CC.TimeTypeId = @PTOTimeTypeId THEN 'PTO' ELSE NEW.c.value('@Note', 'NVARCHAR(1000)') END,
				1,
				0
		FROM @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType/CalendarItem/TimeEntryRecord') NEW(c)
		JOIN dbo.ChargeCode CC ON CC.ClientId = NEW.c.value('..[1]/..[1]/..[1]/@AccountId', 'INT')
								AND CC.ProjectGroupId = NEW.c.value('..[1]/..[1]/..[1]/@BusinessUnitId', 'INT')
								AND CC.ProjectId = NEW.c.value('..[1]/..[1]/..[1]/@ProjectId', 'INT')
								AND CC.TimeTypeId = NEW.c.value('..[1]/..[1]/@Id', 'INT')	
		LEFT JOIN dbo.TimeEntry TE ON TE.ChargeCodeId = CC.Id AND TE.PersonId = @PersonId
										AND TE.ChargeCodeDate = NEW.c.value('..[1]/@Date', 'DATETIME')
										AND (TE.Note = NEW.c.value('@Note', 'NVARCHAR(1000)') )
		LEFT JOIN ForecastedHours FH ON FH.ProjectId = NEW.c.value('..[1]/..[1]/..[1]/@ProjectId', 'INT') 
										AND FH.Date = NEW.c.value('..[1]/@Date', 'DATETIME')
		WHERE TE.TimeEntryId IS NULL
			AND NEW.c.value('@ActualHours', 'REAL') > 0


		INSERT INTO dbo.TimeEntryHours(TimeEntryId,
									   ActualHours,
									   IsChargeable,
										CreateDate,
										ModifiedDate,
										ModifiedBy,
										ReviewStatusId)
		SELECT  TE.TimeEntryId,
				NEW.c.value('@ActualHours', 'REAL'),
				NEW.c.value('@IsChargeable', 'BIT'),
				@CurrentPMTime,
				@CurrentPMTime,
				@ModifiedBy,
				1 -- pending status
		FROM @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType/CalendarItem/TimeEntryRecord') NEW(c)
		JOIN dbo.ChargeCode CC ON CC.ClientId = NEW.c.value('..[1]/..[1]/..[1]/@AccountId', 'INT')
								AND CC.ProjectGroupId = NEW.c.value('..[1]/..[1]/..[1]/@BusinessUnitId', 'INT')
								AND CC.ProjectId = NEW.c.value('..[1]/..[1]/..[1]/@ProjectId', 'INT')
								AND CC.TimeTypeId = NEW.c.value('..[1]/..[1]/@Id', 'INT')	
		INNER JOIN dbo.TimeEntry TE ON TE.ChargeCodeId = CC.Id AND TE.PersonId = @PersonId
										AND TE.ChargeCodeDate = NEW.c.value('..[1]/@Date', 'DATETIME')
		LEFT JOIN dbo.TimeEntryHours TEH ON (  TEH.TimeEntryId = TE.TimeEntryId
											   AND TEH.IsChargeable = NEW.c.value('@IsChargeable', 'BIT')
											)

		WHERE TEH.TimeEntryId IS NULL
			AND NEW.c.value('@ActualHours', 'REAL') > 0
			
		
		--Insert PTO.
		INSERT INTO PersonCalendar(Date,
									PersonId,
									DayOff,
									ActualHours,
									IsFloatingHoliday,
									IsFromTimeEntry
									)
		SELECT Dates.c.value('..[1]/@Date', 'DATETIME'),
				@PersonId,
				1,
				Dates.c.value('@ActualHours', 'REAL'),
				0,
				1
		FROM PersonCalendar PC
		RIGHT JOIN @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType/CalendarItem/TimeEntryRecord') Dates(c)
				ON PC.PersonId =  @PersonId
					AND Dates.c.value('..[1]/@Date', 'DATETIME') = PC.Date
		WHERE Dates.c.value('..[1]/..[1]/..[1]/..[1]/@Id', 'INT') = 4
				AND Dates.c.value('..[1]/..[1]/@Id', 'INT') = @PTOTimeTypeId
				AND Dates.c.value('@ActualHours', 'REAL') > 0
				AND PC.Date IS NULL

		DECLARE @DailyTotalHoursWithOutPTOHoliday TABLE (Date DATETIME, TotalHours REAL)
	
		INSERT INTO @DailyTotalHoursWithOutPTOHoliday(Date, TotalHOurs)
		SELECT entryList.DATE, SUM(entryList.ActualHours) totalHours
		FROM (
			SELECT TE.c.value('..[1]/..[1]/..[1]/..[1]/@Id', 'INT') AS 'TimeEntrySectionId',
				TE.c.value('..[1]/..[1]/..[1]/@AccountId', 'INT') AS 'AccountId',
				TE.c.value('..[1]/..[1]/..[1]/@BusinessUnitId', 'INT') AS 'BusinessUnitId',
				TE.c.value('..[1]/..[1]/..[1]/@ProjectId', 'INT') AS 'ProjectId',
				TE.c.value('..[1]/..[1]/@Id', 'INT') AS 'WorkTypeId',
				TE.c.value('..[1]/@Date', 'DATETIME') As 'Date',
				TE.c.value('@ActualHours', 'REAL') AS 'ActualHours'
			FROM @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType/CalendarItem/TimeEntryRecord') TE(C)
			WHERE TE.c.value('..[1]/..[1]/..[1]/..[1]/@Id', 'INT') <> 4
		) AS entryList
		GROUP BY entryList.Date

		--Delete PTO entry from PersonCalendar.
		DELETE PC
		FROM dbo.PersonCalendar PC
		LEFT JOIN @DailyTotalHoursWithOutPTOHoliday DTH ON DTH.Date = PC.Date 
		LEFT JOIN @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType/CalendarItem/TimeEntryRecord') Dates(c)
				ON Dates.c.value('..[1]/..[1]/..[1]/..[1]/@Id', 'INT') = 4 
					AND Dates.c.value('..[1]/@Date', 'DATETIME') = PC.Date
					AND Dates.c.value('..[1]/..[1]/@Id', 'INT') = @PTOTimeTypeId
		WHERE PC.PersonId =  @PersonId AND PC.Date BETWEEN @StartDate AND @EndDate
			AND (DTH.TotalHours >= 8 OR (ISNULL(DTH.TotalHours, 0) < 8 AND Dates.c.value('@ActualHours', 'REAL') = 0))

		--Update PTO.
		UPDATE PC
		SET	ActualHours = ISNULL(Dates.c.value('@ActualHours', 'REAL'), 8 - DTH.TotalHours)
		FROM dbo.PersonCalendar PC
		LEFT JOIN @DailyTotalHoursWithOutPTOHoliday DTH ON DTH.Date = PC.Date  AND DTH.TotalHours < 8
		LEFT JOIN @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType/CalendarItem/TimeEntryRecord') Dates(c)
			ON Dates.c.value('..[1]/..[1]/..[1]/..[1]/@Id', 'INT') = 4
				AND Dates.c.value('..[1]/@Date', 'DATETIME') = PC.Date
				AND Dates.c.value('..[1]/..[1]/@Id', 'INT') = @PTOTimeTypeId
				AND Dates.c.value('@ActualHours', 'REAL') > 0
		WHERE PC.PersonId = @PersonId AND PC.Date BETWEEN @StartDate AND @EndDate AND PC.DayOff = 1
			AND (Dates.c.value('@ActualHours', 'REAL') IS NOT NULL OR DTH.TotalHours IS NOT NULL)
			AND ISNULL(Dates.c.value('@ActualHours', 'REAL'), 8 - DTH.TotalHours) <> PC.ActualHours
		
		--Update PersonCalendarAuto table with PersonCalendar table changes.
		UPDATE ca
		   SET DayOff = pc.DayOff
		FROM dbo.PersonCalendarAuto AS ca
		INNER JOIN dbo.v_PersonCalendar AS pc
			ON ca.date = pc.Date AND ca.PersonId = pc.PersonId AND pc.Date BETWEEN @StartDate AND @EndDate AND ca.PersonId = @PersonId 

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

