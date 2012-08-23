CREATE PROCEDURE [dbo].[TerminationReportGraph]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME,
	@TimeScaleIds	XML = null,
	@SeniorityIds	XML = null,
	@TerminationReasonIds XML = NULL,
	@PracticeIds	XML = null,
	@ExcludeInternalPractices	BIT
)
AS
BEGIN
	DECLARE @FutureDate DATETIME,@W2SalaryId INT ,@W2HourlyId INT , @1099HourlyId INT , @1099PROId INT
	SET @FutureDate = dbo.GetFutureDate()
	SELECT @W2SalaryId = TimescaleId FROM Timescale WHERE Name = 'W2-Salary'
	SELECT @W2HourlyId = TimescaleId FROM Timescale WHERE Name = 'W2-Hourly'
	SELECT @1099HourlyId = TimescaleId FROM Timescale WHERE Name = '1099 Hourly'
	SELECT @1099PROId = TimescaleId FROM Timescale WHERE Name = '1099/POR'

		;WITH RangeValue
		AS
		(
			SELECT C.MonthStartDate AS StartDate ,
					C.MonthEndDate AS EndDate
			FROM dbo.Calendar C
			WHERE C.Date BETWEEN @StartDate AND @EndDate
			GROUP BY C.MonthStartDate,C.MonthEndDate
		),
		ActivePersonsAtTheBeginningCTE
		AS
		(
			SELECT TT.StartDate,COUNT(PSH.PersonId) AS ActivePersonsAtTheBeginning
			FROM dbo.PersonStatusHistory PSH 
			INNER JOIN RangeValue TT ON TT.StartDate BETWEEN PSH.StartDate AND ISNULL(PSH.EndDate,@FutureDate) 
			GROUP BY TT.StartDate 
		),
		FilteredPersonTerminationHistory
		AS
		(
			SELECT FPH.*,Pay.Timescale
			FROM v_PersonHistory FPH 
			INNER JOIN dbo.Person P ON FPH.PersonId = P.PersonId
			INNER JOIN dbo.TerminationReasons TR ON TR.TerminationReasonId = FPH.TerminationReasonId
			OUTER APPLY (SELECT TOP 1 pa.* FROM dbo.Pay pa WHERE pa.Person = FPH.PersonId AND ISNULL(pa.EndDate,@FutureDate)-1 >= FPH.HireDate AND pa.StartDate <= FPH.TerminationDate ORDER BY pa.StartDate DESC ) pay
			LEFT JOIN dbo.Practice Pra ON Pra.PracticeId = Pay.PracticeId
			WHERE	FPH.TerminationDate BETWEEN @StartDate AND @EndDate
					AND
					(
						@TimeScaleIds IS NULL
						OR ISNULL(Pay.Timescale,0) IN ( SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@TimeScaleIds))
					)
					AND
					( 
						@SeniorityIds IS NULL
						OR ISNULL(Pay.SeniorityId,0) IN (SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@SeniorityIds))
					)
					AND
					( 
						@TerminationReasonIds IS NULL
						OR TR.TerminationReasonId IN (SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@TerminationReasonIds))
					)
					AND 
					(
						@PracticeIds IS NULL
						OR Pay.PracticeId IN ( SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@PracticeIds))
					)
					AND 
					(
						@ExcludeInternalPractices = 0 OR ( @ExcludeInternalPractices = 1 AND Pra.IsCompanyInternal = 0 )
					)
		),
		PersonTerminationInRange
		AS 
		(
			SELECT TT.StartDate,
					SUM(CASE WHEN FPT.Timescale = @W2SalaryId THEN 1 ELSE 0 END) AS TerminationsW2SalaryCountInTheRange,
					SUM(CASE WHEN FPT.Timescale = @W2HourlyId THEN 1 ELSE 0 END) AS TerminationsW2HourlyCountInTheRange,
					SUM(CASE WHEN FPT.Timescale IN (@1099HourlyId,@1099PROId) THEN 1 ELSE 0 END) AS TerminationsContractorsCountInTheRange
			FROM RangeValue TT
			LEFT JOIN  FilteredPersonTerminationHistory FPT  ON FPT.TerminationDate BETWEEN TT.StartDate AND TT.EndDate
			GROUP BY TT.StartDate
		),
		FilteredPersonHireHistory
		AS
		(			
			SELECT CPH.PersonId,
					CPH.HireDate,
					CPH.PersonStatusId,
					CASE WHEN ISNULL(CPH.TerminationDate,@FutureDate) > @EndDate THEN @EndDate ELSE ISNULL(CPH.TerminationDate,@FutureDate) END  AS TerminationDate,
					CPH.Id,
					CPH.DivisionId,
					CPH.TerminationReasonId
			FROM v_PersonHistory CPH 
			WHERE CPH.HireDAte BETWEEN @StartDate AND @EndDate
		),
		FilteredPersonHireHistory1
		AS
		(
			SELECT FPH.*
			FROM FilteredPersonHireHistory FPH
			OUTER APPLY (SELECT TOP 1 pa.* FROM dbo.Pay pa WHERE pa.Person = FPH.PersonId AND ISNULL(pa.EndDate,@FutureDate)-1 >= FPH.HireDate AND pa.StartDate <= FPH.TerminationDate ORDER BY pa.StartDate DESC ) pay
			WHERE	pay.Timescale IN (1,2)
		),
		PersonHiriesInRange
		AS 
		(
			SELECT TT.StartDate , COUNT(FPT.PersonId) AS NewHiredInTheRange
			FROM RangeValue TT
			LEFT JOIN  FilteredPersonHireHistory1 FPT  ON FPT.HireDate BETWEEN TT.StartDate AND TT.EndDate
			GROUP BY TT.StartDate
		)
		SELECT TT.StartDate,
				TT.EndDate,
				FPHH.NewHiredInTheRange,
				FPTH.TerminationsW2SalaryCountInTheRange,
				FPTH.TerminationsW2HourlyCountInTheRange,
				FPTH.TerminationsContractorsCountInTheRange,
				AP.ActivePersonsAtTheBeginning
		FROM RangeValue TT
		INNER JOIN PersonTerminationInRange FPTH ON FPTH.StartDate = TT.StartDate
		INNER JOIN PersonHiriesInRange FPHH ON FPHH.StartDate = TT.StartDate
		LEFT JOIN ActivePersonsAtTheBeginningCTE AP ON AP.StartDate = TT.StartDate

END
