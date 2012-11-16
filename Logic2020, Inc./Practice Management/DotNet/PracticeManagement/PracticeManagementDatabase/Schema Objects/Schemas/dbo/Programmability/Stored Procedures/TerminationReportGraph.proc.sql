CREATE PROCEDURE [dbo].[TerminationReportGraph]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME
)
AS
BEGIN
	SELECT @StartDate = CONVERT(DATETIME,CONVERT(DATE,@StartDate)),@EndDate = CONVERT(DATETIME,CONVERT(DATE,@EndDate))
	DECLARE @FutureDate DATETIME,@W2SalaryId INT ,@W2HourlyId INT , @1099HourlyId INT , @1099PROId INT,@FormalyInactive INT,@ActivePersonCount INT
	SET @FutureDate = dbo.GetFutureDate()
	SELECT @W2SalaryId = TimescaleId FROM Timescale WHERE Name = 'W2-Salary'
	SELECT @W2HourlyId = TimescaleId FROM Timescale WHERE Name = 'W2-Hourly'
	SELECT @1099HourlyId = TimescaleId FROM Timescale WHERE Name = '1099 Hourly'
	SELECT @1099PROId = TimescaleId FROM Timescale WHERE Name = '1099/POR'
	SELECT @FormalyInactive = TerminationReasonId  FROM dbo.TerminationReasons WHERE TerminationReason = 'Formerly Inactive status '
	
		SELECT @ActivePersonCount = COUNT(DISTINCT PSH.PersonId) 
		FROM dbo.PersonStatusHistory PSH 
		INNER JOIN dbo.Person P ON PSH.PersonId = P.PersonId AND P.IsStrawman = 0 AND PSH.personstatusId = 1
		INNER JOIN dbo.Pay pa ON pa.Person = PSH.PersonId AND @StartDate  BETWEEN pa.StartDate  AND ISNULL(pa.EndDate,@FutureDate)-1 AND pa .Timescale IN (@W2SalaryId,@W2HourlyId)
		INNER JOIN dbo.v_PersonHistory PH ON PH.PersonId = PSH.PersonId AND @StartDate BETWEEN PH.HireDate AND ISNULL(PH.TerminationDate,@FutureDate) -- if status start date is less then person hire date we need to consider only hire date
		WHERE @StartDate BETWEEN PSH.StartDate AND ISNULL(PSH.EndDate,@FutureDate) 

		;WITH RangeValue
		AS
		(
			SELECT C.MonthStartDate AS StartDate ,
					C.MonthEndDate AS EndDate
			FROM dbo.Calendar C
			WHERE C.Date BETWEEN @StartDate AND @EndDate
			GROUP BY C.MonthStartDate,C.MonthEndDate
		),
		FilteredPersonTerminationHistory
		AS
		(
			SELECT FPH.*,TR.IsContingentRule
			FROM v_PersonHistory FPH 
			INNER JOIN dbo.TerminationReasons TR ON TR.TerminationReasonId = FPH.TerminationReasonId
			WHERE FPH.TerminationDate BETWEEN @StartDate AND @EndDate AND ISNULL(FPH.TerminationReasonId,0) != @FormalyInactive
		),
		PersonTerminationInRange
		AS 
		(
			SELECT TT.StartDate,
					SUM(CASE WHEN pay.Timescale IN (@W2SalaryId, @W2HourlyId) AND FPT.IsContingentRule = 0 THEN 1 ELSE 0 END) AS TerminationsCumulativeEmployeeCountInTheRange,
					COUNT(FPT.PersonId) AS TerminationsCountInTheRange,
					SUM(CASE WHEN FPT.TerminationDate >= TT.StartDate AND pay.Timescale = @W2SalaryId THEN 1 ELSE 0 END) AS TerminationsW2SalaryCountInTheRange,
					SUM(CASE WHEN FPT.TerminationDate >= TT.StartDate AND pay.Timescale = @W2HourlyId THEN 1 ELSE 0 END) AS TerminationsW2HourlyCountInTheRange,
					SUM(CASE WHEN FPT.TerminationDate >= TT.StartDate AND pay.Timescale = @1099HourlyId THEN 1 ELSE 0 END) AS Terminations1099HourlyCountInTheRange,
					SUM(CASE WHEN FPT.TerminationDate >= TT.StartDate AND pay.Timescale = @1099PROId THEN 1 ELSE 0 END) AS Terminations1099PORCountInTheRange
			FROM RangeValue TT
			LEFT JOIN  FilteredPersonTerminationHistory FPT  ON FPT.TerminationDate BETWEEN @StartDate AND TT.EndDate
			OUTER APPLY (SELECT TOP 1 pa.* FROM dbo.Pay pa WHERE pa.Person = FPT.PersonId AND ISNULL(pa.EndDate,@FutureDate)-1 >= FPT.HireDate AND pa.StartDate <= CASE WHEN FPT.TerminationDate < TT.EndDate THEN FPT.TerminationDate ELSE TT.EndDate END ORDER BY pa.StartDate DESC ) pay

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
			LEFT JOIN dbo.TerminationReasons TR ON TR.TerminationReasonId = CPH.TerminationReasonId
			WHERE CPH.HireDAte BETWEEN @StartDate AND @EndDate 
			AND CPH.PersonStatusId != 3 --Projected status
			AND ISNULL(TR.IsContingentRule,0) = 0
		),
		PersonHiriesInRange
		AS 
		(
			SELECT TT.StartDate , COUNT(FPT.PersonId) AS NewHiredCumulativeInTheRange
				, SUM(CASE WHEN FPT.HireDate >= TT.StartDate THEN 1 ELSE 0 END ) AS NewHiredInTheRange
			FROM RangeValue TT
			LEFT JOIN  FilteredPersonHireHistory FPT  ON FPT.HireDate BETWEEN @StartDate AND TT.EndDate
			OUTER APPLY (SELECT TOP 1 pa.* FROM dbo.Pay pa WHERE pa.Person = FPT.PersonId AND ISNULL(pa.EndDate,@FutureDate)-1 >= FPT.HireDate AND pa.StartDate <= (CASE WHEN FPT.TerminationDate < TT.EndDate THEN FPT.TerminationDate ELSE TT.EndDate END)  ORDER BY pa.StartDate DESC ) pay
			WHERE pay.Timescale IN (@W2SalaryId,@W2HourlyId)
			GROUP BY TT.StartDate
		)
		SELECT TT.StartDate,
				TT.EndDate,
				ISNULL(FPHH.NewHiredInTheRange,0) AS NewHiredInTheRange,
				ISNULL(FPHH.NewHiredCumulativeInTheRange,0) AS NewHiredCumulativeInTheRange,
				FPTH.TerminationsW2SalaryCountInTheRange,
				FPTH.TerminationsW2HourlyCountInTheRange,
				FPTH.Terminations1099HourlyCountInTheRange,
				FPTH.Terminations1099PORCountInTheRange,
				FPTH.TerminationsCountInTheRange,
				ISNULL(@ActivePersonCount,0) AS ActivePersonsAtTheBeginning,
				FPTH.TerminationsCumulativeEmployeeCountInTheRange
		FROM RangeValue TT
		INNER JOIN PersonTerminationInRange FPTH ON FPTH.StartDate = TT.StartDate
		LEFT JOIN PersonHiriesInRange FPHH ON FPHH.StartDate = TT.StartDate
		ORDER BY TT.StartDate

END

