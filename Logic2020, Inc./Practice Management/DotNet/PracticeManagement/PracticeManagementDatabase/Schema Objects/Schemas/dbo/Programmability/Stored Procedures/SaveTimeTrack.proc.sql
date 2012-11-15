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
		UPDATE PC
		SET	PC.ActualHours = TWTE.ActualHours,
			PC.IsFromTimeEntry = 1,
			PC.TimeTypeId = TWTE.TimeTypeId,
			PC.Description = CASE WHEN TT.IsAdministrative = 1 AND ( TWTE.Note = '' OR ISNULL(OldTT.Name,'') + '.' = TWTE.Note ) THEN TT.Name + '.' ELSE TWTE.Note END,
			PC.ApprovedBy = CASE TWTE.TimeTypeId WHEN @ORTTimeTypeId THEN TWTE.ApprovedById ELSE NULL END
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
				OR (PC.TimeTypeId = @ORTTimeTypeId AND PC.ApprovedBy <> TWTE.ApprovedById))
				
		
		--Insert administrative TIME TYPE time entries in person calendar table(note:Excluding HOLIDAY and UNPAID time types).
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
				CASE TT.TimeTypeId WHEN @ORTTimeTypeId THEN TWTE.ApprovedById ELSE NULL END
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


