CREATE PROCEDURE [dbo].[OnPersonHireDateChange]
(
	@PersonId        INT,
	@NewHireDate	 DATETIME,
	@ModifiedBy		 INT
)
AS
/*
A.Update The person Compensation 
1.Get the previous termination date of the person
2.Delete all the compensation records that are before the new hire date and greater than the previous termination date
3.Get the latest start date of the compensation record after the previous termination date
4.Update the latest compensation start date with the hire date.

B.Need to adjust  person status history record if the person is hired for the first time

*/
BEGIN
--a.Update The person Compensation 
	DECLARE @PreviousTerminationDate DATETIME ,@LatestCompensationStartDate DATETIME, @LatestTimeScaleId INT, @W2SalaryId INT, @W2HourlyId INT, @HolidayTimeTypeId INT, @CurrentPMTime DATETIME, @PTOTimeTypeId INT, @PTOChargeCodeId INT

	SELECT @W2SalaryId = TimescaleId FROM Timescale WHERE Name = 'W2-Salary'
	SELECT @W2HourlyId  = TimescaleId FROM Timescale WHERE Name = 'W2-Hourly'
	SELECT @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId(), @PTOTimeTypeId = dbo.GetPTOTimeTypeId()
	SELECT  @PTOChargeCodeId = ID
	FROM dbo.ChargeCode
	WHERE TimeTypeId = @PTOTimeTypeId
	SELECT @CurrentPMTime = dbo.InsertingTime()

	--1.Get the previous termination date of the person
	SELECT Top 1 @PreviousTerminationDate = VP.TerminationDate 
	FROM dbo.v_PersonHistory VP
	WHERE VP.TerminationDate < @NewHireDate
			AND VP.PersonId = @PersonId 
	ORDER BY VP.HireDate DESC

	--2.Delete all the compensation records that are before the new hire date and greater than the previous termination date
	DELETE pay
	FROM dbo.pay pay 
	WHERE Pay.Person = @PersonId 
			AND ( @PreviousTerminationDate  IS NULL OR pay.StartDate > @PreviousTerminationDate) -- Compensation After the previous termination date
			AND Pay.EndDate-1 < @NewHireDate -- Compensation before new hire date


	--3.Get the latest start date of the compensation record after the previous termination date
	SELECT TOP 1 @LatestCompensationStartDate = p.StartDate, @LatestTimeScaleId = p.Timescale
	FROM dbo.pay p
	WHERE p.Person = @PersonId 
				--AND @NewHireDate < p.StartDate 
				AND (@PreviousTerminationDate  IS NULL OR p.StartDate > @PreviousTerminationDate) -- Compensation After the previous termination date
				--AND	@NewHireDate BETWEEN p.StartDate AND p.EndDate-1
	ORDER BY p.StartDate

	--4.Update the latest compensation start date with the hire date.
	UPDATE pay
	SET Pay.StartDate = @NewHireDate
	FROM dbo.pay pay
	WHERE Pay.Person = @PersonId 
			AND Pay.StartDate = @LatestCompensationStartDate

		--Delete all Administrative time entries from previous terminationDate to new hire Date.

		--If the company holiday but person work day on no compensation day, then we need to delete work day and keep the floating holiday as PTO if the floating holiday is valid.
		--If we are deleting floating holiday as there is no compensation day, then we need to delete floating day and remove the work day on the company holiday.
		--Insert holiday time entry if the parent date present in w2salary period.

		DECLARE @DateDetails TABLE (AffectDate DATETIME, Parent DATETIME, SubstituteDate DATETIME, ParentTimeScale INT, SubstituteTimeScale INT)

		INSERT INTO @DateDetails(AffectDate, Parent, SubstituteDate, ParentTimeScale, SubstituteTimeScale)
		SELECT PC.Date AS 'AffectDate', PCD.Date 'Parent', PC.SubstituteDate 'Substitute', PP.Timescale, PS.Timescale
		FROM dbo.PersonCalendar PC
		LEFT JOIN dbo.PersonCalendar PCD ON PC.PersonId = PCD.PersonId AND PC.Date = PCD.SubstituteDate
		LEFT JOIN dbo.Pay PP ON PP.Person = PC.PersonId AND PCD.Date BETWEEN PP.StartDate AND PP.EndDate
		LEFT JOIN dbo.Pay PS ON PS.Person = PC.PersonId AND PC.SubstituteDate BETWEEN PS.StartDate AND PS.EndDate
		WHERE PC.PersonId = @PersonId AND PC.Date BETWEEN ISNULL(@PreviousTerminationDate, PC.Date) AND @NewHireDate
		AND (PCD.Date IS NOT NULL OR PC.SubstituteDate IS NOT NULL)
		AND (PCD.Date IS NULL OR (PCD.Date IS NOT NULL AND PCD.Date NOT BETWEEN ISNULL(@PreviousTerminationDate, PCD.Date) AND @NewHireDate))
		AND (PC.SubstituteDate IS NULL OR (PC.SubstituteDate IS NOT NULL AND PC.SubstituteDate NOT BETWEEN ISNULL(@PreviousTerminationDate, PC.SubstituteDate) AND @NewHireDate))

		--Delete floating holidays if not w2salary.
		DELETE PC
		FROM dbo.PersonCalendar PC
		WHERE PC.PersonId = @PersonId 
		AND ( (PC.Date BETWEEN ISNULL(@PreviousTerminationDate, PC.Date) AND @NewHireDate)
			OR (PC.Date IN (SELECT DD.Parent FROM @DateDetails DD WHERE 
							(PC.Date = DD.Parent AND PC.DayOff = 0)
							OR (PC.Date = DD.SubstituteDate AND DD.SubstituteTimeScale <> @W2SalaryId)
							))
		)

		DELETE TEH
		FROM dbo.TimeEntryHours TEH
		INNER JOIN dbo.TimeEntry TE ON TE.TimeEntryId = TEH.TimeEntryId
		INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
		INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId AND TT.IsAdministrative = 1
		WHERE TE.PersonId = @PersonId 
		AND ( (TE.ChargeCodeDate BETWEEN ISNULL(@PreviousTerminationDate, TE.ChargeCodeDate) AND @NewHireDate)
			OR (TE.ChargeCodeDate IN (SELECT DD.Parent FROM @DateDetails DD 
									WHERE (TE.ChargeCodeDate = DD.Parent AND DD.ParentTimeScale <> @W2SalaryId)
										OR (TE.ChargeCodeDate = DD.SubstituteDate AND DD.SubstituteTimeScale <> @W2SalaryId)
										)
				)
		)
		
		DELETE TE
		FROM dbo.TimeEntry TE
		INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
		INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId AND TT.IsAdministrative = 1
		WHERE TE.PersonId = @PersonId 
		AND ( (TE.ChargeCodeDate BETWEEN ISNULL(@PreviousTerminationDate, TE.ChargeCodeDate) AND @NewHireDate)
			OR (TE.ChargeCodeDate IN (SELECT DD.Parent FROM @DateDetails DD 
									WHERE (TE.ChargeCodeDate = DD.Parent AND DD.ParentTimeScale <> @W2SalaryId)
										OR (TE.ChargeCodeDate = DD.SubstituteDate AND DD.SubstituteTimeScale <> @W2SalaryId)
										)
				)
		)

		--If a floating holiday is present for the deleting date and that floating holiday exists in w2salary period then we need to convert it to PTO timeOff.
		UPDATE PC
		SET TimeTypeId = @PTOTimeTypeId,
			Description = 'PTO.',
			ApprovedBy = NULL,
			IsFromTimeEntry = 0
		FROM PersonCalendar PC
		INNER JOIN @DateDetails DD ON PC.PersonId = @PersonId AND DD.SubstituteDate = PC.Date AND DD.SubstituteTimeScale = @W2SalaryId

		UPDATE TEH
		SET ModifiedBy = @ModifiedBy,
			ModifiedDate = @CurrentPMTime
		FROM dbo.TimeEntryHours TEH
		INNER JOIN dbo.TimeEntry TE ON TE.TimeEntryId = TEH.TimeEntryId
		INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND TE.PersonId = @PersonId
		INNER JOIN @DateDetails DD ON DD.SubstituteDate = TE.ChargeCodeDate AND DD.SubstituteTimeScale = @W2SalaryId AND CC.TimeTypeId = @HolidayTimeTypeId

		UPDATE TE
		SET ChargeCodeId = @PTOChargeCodeId,
			Note = 'PTO.'
		FROM dbo.TimeEntry TE
		INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND TE.PersonId = @PersonId
		INNER JOIN @DateDetails DD ON DD.SubstituteDate = TE.ChargeCodeDate AND DD.SubstituteTimeScale = @W2SalaryId AND CC.TimeTypeId = @HolidayTimeTypeId
				
		--IF @NewHireDate < @LatestCompensationStartDate
		--BEGIN

			--If decreasing the hireDate then add new
			INSERT INTO dbo.TimeEntry(PersonId,
									ChargeCodeId,
									ChargeCodeDate,
									ForecastedHours,
									Note,
									IsCorrect,
									IsAutoGenerated)
			SELECT DISTINCT @PersonId
				,CC.Id
				,C.Date
				,0
				, ISNULL(C.HolidayDescription, ISNULL(PC.Description, ''))
				,1
				,1
			FROM dbo.Calendar C
			LEFT JOIN dbo.PersonCalendar PC ON PC.Date = C.Date AND PC.PersonId = @PersonId
			LEFT JOIN @Datedetails DD ON DD.Parent = C.Date AND DD.ParentTimeScale = @W2SalaryId AND C.DayOff = 1
			INNER JOIN dbo.TimeType TT ON (C.Date BETWEEN @NewHireDate AND @LatestCompensationStartDate
										AND ((C.DayOff = 1  AND DATEPART(DW,C.DATE) NOT IN (1,7) AND TT.TimeTypeId = @HolidayTimeTypeId AND PC.Date IS NULL)
											OR (PC.Date IS NOT NULL AND PC.TimeTypeId = TT.TimeTypeId)
											)
										)
										OR 
										(
											DD.Parent IS NOT NULL AND TT.TimeTypeId = @HolidayTimeTypeId
										)
			INNER JOIN dbo.ChargeCode CC ON CC.TimeTypeId = TT.TimeTypeId
			LEFT JOIN dbo.TimeEntry TE ON TE.PersonId = @PersonId AND TE.ChargeCodeDate = C.Date AND TE.ChargeCodeId = CC.Id
			WHERE (
					TE.TimeEntryId IS NULL
					AND  (
						(@LatestTimeScaleId = @W2SalaryId and TT.IsW2SalaryAllowed = 1)
						OR 
						(@LatestTimeScaleId = @W2HourlyId and TT.IsW2HourlyAllowed = 1)
						OR 
						(
							DD.Parent IS NOT NULL
						)
						)
					)
			--TimeEntry HOurs.
			INSERT INTO dbo.TimeEntryHours(TimeEntryId,
											ActualHours,
											IsChargeable,
											CreateDate,
											ModifiedDate,
											ModifiedBy,
											ReviewStatusId)
			SELECT TE.TimeEntryId
					,CASE WHEN TT.TimeTypeId = @HolidayTimeTypeId THEN 8
							ELSE ISNULL(PC.ActualHours,8) END
					,0 --Non Billable
					,@CurrentPMTime
					,@CurrentPMTime
					,@ModifiedBy
					,CASE WHEN C.DayOff = 1 OR (PC.PersonId IS NOT NULL AND PC.DayOff = 1 AND PC.TimeTypeId <> @HolidayTimeTypeId AND PC.IsFromTimeEntry <> 1) THEN 2 
							ELSE 1 END
			FROM dbo.Calendar C
			LEFT JOIN dbo.PersonCalendar PC ON PC.Date = C.Date AND PC.PersonId = @PersonId
			LEFT JOIN @Datedetails DD ON DD.Parent = C.Date AND DD.ParentTimeScale = @W2SalaryId AND C.DayOff = 1
			INNER JOIN dbo.TimeType TT ON ( C.Date BETWEEN @NewHireDate AND @LatestCompensationStartDate
											AND ((C.DayOff = 1  AND DATEPART(DW,C.DATE) NOT IN (1,7) AND TT.TimeTypeId = @HolidayTimeTypeId AND PC.Date IS NULL)
												OR (PC.Date IS NOT NULL AND PC.TimeTypeId = TT.TimeTypeId)
												)
										)
										OR 
										(
											DD.Parent IS NOT NULL AND TT.TimeTypeId = @HolidayTimeTypeId
										)
			INNER JOIN dbo.ChargeCode CC ON CC.TimeTypeId = TT.TimeTypeId
			INNER JOIN dbo.TimeEntry TE ON TE.PersonId = @PersonId AND TE.ChargeCodeDate = C.Date AND TE.ChargeCodeId = CC.Id
			LEFT JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
			WHERE (
					TEH.TimeEntryId IS NULL
					AND  (
						(@LatestTimeScaleId = @W2SalaryId and TT.IsW2SalaryAllowed = 1)
						OR 
						(@LatestTimeScaleId = @W2HourlyId and TT.IsW2HourlyAllowed = 1)
						OR 
						(
							DD.Parent IS NOT NULL
						)
						)
					)
		--END


	--B.Need to adjust  person status history record if the person is hired for the first time

	;WITH IsFirstHire AS
	(
		SELECT PH.PersonId, MIN(PH.HireDate) 'HireDate', MIN(PH.CreatedDate) AS 'CreatedDate', MIN(PH.TerminationDate) AS 'TerminationDate'
		FROM dbo.v_PersonHistory PH
		WHERE PH.PersonId = @PersonId
		GROUP BY PH.PersonId
		HAVING COUNT(*) = 1 AND MIN(PH.TerminationDate) IS NULL
	)
	,FirstPSHRow  AS
	(
		SELECT MIN(StartDate) AS StartDate,
			@PersonId AS PersonId,
			CONVERT(DATE, MIN(FH.CreatedDate)) AS CreatedDate,
			CONVERT(DATE, MIN(FH.HireDate)) AS HireDate
		FROM dbo.PersonStatusHistory AS PSH
		INNER JOIN IsFirstHire FH ON FH.PersonId = PSH.PersonId
	)

	UPDATE PSH
	SET PSH.startdate = CASE WHEN FPSHR.HireDate < FPSHR.CreatedDate THEN FPSHR.HireDate
										ELSE FPSHR.CreatedDate END
	FROM FirstPSHRow AS FPSHR
	INNER JOIN dbo.PersonStatusHistory AS PSH ON PSH.PersonId = FPSHR.PersonId AND  FPSHR.startdate = PSH.startdate
	WHERE  (FPSHR.HireDate < FPSHR.CreatedDate AND FPSHR.HireDate <> PSH.StartDate) OR (FPSHR.HireDate > FPSHR.CreatedDate AND FPSHR.CreatedDate <> PSH.StartDate)

END
