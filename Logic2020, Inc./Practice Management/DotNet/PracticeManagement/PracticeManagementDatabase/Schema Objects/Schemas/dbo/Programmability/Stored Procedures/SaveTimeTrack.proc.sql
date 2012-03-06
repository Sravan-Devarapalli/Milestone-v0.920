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
	DECLARE @PTOTimeTypeId	INT,
			@CurrentPMTime	DATETIME,
			@ModifiedBy		INT,
			@HolidayTimeTypeId INT,
			@ORTTimeTypeId	INT

	SET @PTOTimeTypeId = dbo.GetPTOTimeTypeId()
	SET @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId()
	SET @CurrentPMTime = dbo.InsertingTime()
	SET @ORTTimeTypeId = dbo.GetORTTimeTypeId()

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
				     )

		UPDATE TE
		SET	TE.Note = CASE WHEN TT.IsAdministrative = 1 AND NEW.c.value('@Note', 'NVARCHAR(1000)') = '' THEN TT.Name ELSE NEW.c.value('@Note', 'NVARCHAR(1000)') END
		FROM dbo.TimeEntry TE
		INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId   
		INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND TE.PersonId = @PersonId
		INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId
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
		SELECT DISTINCT @PersonId,
				CC.Id,
				NEW.c.value('..[1]/@Date', 'DATETIME'),
				ISNULL(FH.ForcastedHours, 0),
				CASE WHEN TT.IsAdministrative = 1 AND NEW.c.value('@Note', 'NVARCHAR(1000)') = '' THEN TT.Name + '.' ELSE NEW.c.value('@Note', 'NVARCHAR(1000)') END,
				1,
				0
		FROM @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType/CalendarItem/TimeEntryRecord') NEW(c)
		INNER JOIN dbo.ChargeCode CC ON CC.ClientId = NEW.c.value('..[1]/..[1]/..[1]/@AccountId', 'INT')
								AND CC.ProjectGroupId = NEW.c.value('..[1]/..[1]/..[1]/@BusinessUnitId', 'INT')
								AND CC.ProjectId = NEW.c.value('..[1]/..[1]/..[1]/@ProjectId', 'INT')
								AND CC.TimeTypeId = NEW.c.value('..[1]/..[1]/@Id', 'INT')
		INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId
		LEFT JOIN dbo.TimeEntry TE ON TE.ChargeCodeId = CC.Id AND TE.PersonId = @PersonId
										AND TE.ChargeCodeDate = NEW.c.value('..[1]/@Date', 'DATETIME')
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
				1-- pending status
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

		--Delete PTO entry from PersonCalendar only if the Person has PTO not Floating Holiday.
		DELETE PC
		FROM dbo.PersonCalendar PC
		JOIN Calendar C ON C.Date = PC.Date AND PC.PersonId =  @PersonId AND PC.Date BETWEEN @StartDate AND @EndDate
		LEFT JOIN @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType/CalendarItem/TimeEntryRecord') Dates(c)
				ON Dates.c.value('..[1]/..[1]/..[1]/..[1]/@Id', 'INT') = 4
					AND Dates.c.value('..[1]/@Date', 'DATETIME') = PC.Date
					AND Dates.c.value('..[1]/..[1]/@Id', 'INT') <> @HolidayTimeTypeId
		WHERE PC.TimeTypeId <> @HolidayTimeTypeId AND C.DayOff <> 1 AND PC.IsFromTimeEntry = 1 --can delete only if it is entered for the timeentry page and not floating holiday
			AND ( ISNULL(Dates.c.value('@ActualHours', 'REAL'), 0) = 0)

		--Update PTO actual hours.
		UPDATE PC
		SET	ActualHours = Dates.c.value('@ActualHours', 'REAL'),
			IsSeries = 0,
			IsFromTimeEntry = 1,
			TimeTypeId = Dates.c.value('..[1]/..[1]/@Id', 'INT'),
			Description = CASE WHEN Dates.c.value('@Note', 'NVARCHAR(1000)') = '' THEN tt.Name + '.' ELSE Dates.c.value('@Note', 'NVARCHAR(1000)') END,
			ApprovedBy = CASE Dates.c.value('..[1]/..[1]/@Id', 'INT') WHEN @ORTTimeTypeId THEN Dates.c.value('@ApprovedById', 'INT') ELSE NULL END
		FROM dbo.PersonCalendar PC
		JOIN @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType/CalendarItem/TimeEntryRecord') Dates(c)
			ON Dates.c.value('..[1]/..[1]/..[1]/..[1]/@Id', 'INT') = 4
				AND Dates.c.value('..[1]/@Date', 'DATETIME') = PC.Date
				AND Dates.c.value('..[1]/..[1]/@Id', 'INT') <> @HolidayTimeTypeId
				AND Dates.c.value('@ActualHours', 'REAL') > 0
			INNER JOIN dbo.TimeType tt ON tt.TimeTypeId = PC.TimeTypeId
		WHERE PC.PersonId = @PersonId AND PC.Date BETWEEN @StartDate AND @EndDate AND PC.DayOff = 1
			AND (Dates.c.value('@ActualHours', 'REAL') <> PC.ActualHours 
				OR PC.TimeTypeId <> Dates.c.value('..[1]/..[1]/@Id', 'INT') 
				OR PC.Description <> Dates.c.value('@Note', 'NVARCHAR(1000)')
				OR (PC.TimeTypeId = @ORTTimeTypeId AND PC.ApprovedBy <> Dates.c.value('@ApprovedById', 'INT')))
				
		
		--Insert PTO.
		INSERT INTO PersonCalendar(Date,
									PersonId,
									DayOff,
									ActualHours,
									IsSeries,
									TimeTypeId,
									Description,
									IsFromTimeEntry,
									ApprovedBy
									)
		SELECT Dates.c.value('..[1]/@Date', 'DATETIME'),
				@PersonId,
				1,
				Dates.c.value('@ActualHours', 'REAL'),
				0,
				Dates.c.value('..[1]/..[1]/@Id', 'INT'),
				CASE WHEN Dates.c.value('@Note', 'NVARCHAR(1000)') = '' THEN tt.Name + '.' ELSE Dates.c.value('@Note', 'NVARCHAR(1000)') END,
				1,
				CASE TT.TimeTypeId WHEN @ORTTimeTypeId THEN Dates.c.value('@ApprovedById', 'INT') ELSE NULL END
		FROM PersonCalendar PC
		RIGHT JOIN @TimeEntriesXml.nodes('Sections/Section/AccountAndProjectSelection/WorkType/CalendarItem/TimeEntryRecord') Dates(c)
				ON PC.PersonId =  @PersonId
					AND Dates.c.value('..[1]/@Date', 'DATETIME') = PC.Date
		INNER JOIN TimeType TT ON TT.TimeTypeId = Dates.c.value('..[1]/..[1]/@Id', 'INT')
		WHERE Dates.c.value('..[1]/..[1]/..[1]/..[1]/@Id', 'INT') = 4
				AND Dates.c.value('..[1]/..[1]/@Id', 'INT') <> @HolidayTimeTypeId
				AND Dates.c.value('@ActualHours', 'REAL') > 0
				AND PC.Date IS NULL

		DECLARE @BeforeStartDate DATETIME,
				@AfterEndDate	DATETIME

		SELECT @BeforeStartDate 
		FROM dbo.Calendar  C
		WHERE ( (DATEPART(DW, @BeforeStartDate) = 2 AND C.date = DATEADD(DD, -3, @BeforeStartDate))
					OR  C.date = DATEADD(DD, -1, @BeforeStartDate)
				)

		SELECT @AfterEndDate 
		FROM dbo.Calendar  C
		WHERE ((DATEPART(DW, @AfterEndDate) = 6 AND C.date = DATEADD(DD,3, @AfterEndDate) )
					OR  C.date = DATEADD(DD,1, @AfterEndDate)
				)

		;WITH NeedToModifyDates AS
		(
			SELECT  PC.PersonId, C.Date, CONVERT(BIT, 0) 'IsSeries'
			FROM Calendar C
			JOIN PersonCalendar PC ON C.Date BETWEEN @BeforeStartDate AND @AfterEndDate AND C.Date = PC.Date AND PC.DayOff = 1 AND PC.IsSeries = 1
			LEFT JOIN PersonCalendar APC ON PC.PersonId = APC.PersonId AND APC.DayOff = 1 AND APC.IsSeries = 1 AND APC.TimeTypeId = PC.TimeTypeId AND ISNULL(APC.ApprovedBy, 0) = ISNULL(PC.ApprovedBy, 0)--APC:- AffectedPersonCalendar
						AND ((DATEPART(DW, C.date) = 6 AND APC.date = DATEADD(DD,3, C.date) )
								OR (DATEPART(DW, C.date) = 2 AND APC.date = DATEADD(DD, -3, C.date))
								OR  APC.date = DATEADD(DD,1, C.date)
								OR  APC.date = DATEADD(DD, -1, C.date)
							)
			GROUP BY PC.PersonId, C.date
			Having COUNT(APC.date) < 2
		)

		UPDATE PC
			SET IsSeries = NTMF.IsSeries
		FROM PersonCalendar PC
		JOIN NeedToModifyDates NTMF ON NTMF.PersonId = PC.PersonId AND NTMF.Date = PC.Date
		
		--Update PersonCalendarAuto table with PersonCalendar table changes.
		UPDATE ca
		   SET DayOff = pc.DayOff
		FROM dbo.PersonCalendarAuto AS ca
		INNER JOIN dbo.v_PersonCalendar AS pc
			ON pc.Date BETWEEN @StartDate AND @EndDate AND ca.PersonId = @PersonId AND ca.DayOff <> pc.DayOff AND ca.date = pc.Date AND ca.PersonId = pc.PersonId

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

