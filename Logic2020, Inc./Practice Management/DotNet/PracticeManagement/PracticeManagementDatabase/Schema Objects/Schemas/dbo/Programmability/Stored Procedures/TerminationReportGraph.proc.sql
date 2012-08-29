CREATE PROCEDURE [dbo].[TerminationReportGraph]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME
)
AS
BEGIN
	SELECT @StartDate = CONVERT(DATETIME,CONVERT(DATE,@StartDate)),@EndDate = CONVERT(DATETIME,CONVERT(DATE,@EndDate))
	DECLARE @FutureDate DATETIME,@W2SalaryId INT ,@W2HourlyId INT , @1099HourlyId INT , @1099PROId INT,@FormalyInactive INT
	SET @FutureDate = dbo.GetFutureDate()
	SELECT @W2SalaryId = TimescaleId FROM Timescale WHERE Name = 'W2-Salary'
	SELECT @W2HourlyId = TimescaleId FROM Timescale WHERE Name = 'W2-Hourly'
	SELECT @1099HourlyId = TimescaleId FROM Timescale WHERE Name = '1099 Hourly'
	SELECT @1099PROId = TimescaleId FROM Timescale WHERE Name = '1099/POR'
	SELECT @FormalyInactive = TerminationReasonId  FROM dbo.TerminationReasons WHERE TerminationReason = 'Formerly Inactive status '

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
			SELECT TT.StartDate,COUNT(DISTINCT PSH.PersonId) AS ActivePersonsAtTheBeginning
			FROM dbo.PersonStatusHistory PSH 
			INNER JOIN dbo.Person P ON PSH.PersonId = P.PersonId AND P.IsStrawman = 0 AND PSH.personstatusId = 1
			INNER JOIN RangeValue TT ON TT.StartDate BETWEEN PSH.StartDate AND ISNULL(PSH.EndDate,@FutureDate) 
			INNER JOIN dbo.Pay pa ON pa.Person = PSH.PersonId AND TT.StartDate  BETWEEN pa.StartDate  AND ISNULL(pa.EndDate,@FutureDate) AND pa .Timescale IN (@W2SalaryId,@W2HourlyId)
			INNER JOIN dbo.v_PersonHistory PH ON PH.PersonId = PSH.PersonId AND TT.StartDate BETWEEN PH.HireDate AND ISNULL(PH.TerminationDate,@FutureDate) -- if status start date is less then person hire date we need to consider only hire date
			GROUP BY TT.StartDate 
		),
		FilteredPersonTerminationHistory
		AS
		(
			SELECT FPH.*,Pay.Timescale
			FROM v_PersonHistory FPH 
			OUTER APPLY (SELECT TOP 1 pa.* FROM dbo.Pay pa WHERE pa.Person = FPH.PersonId AND ISNULL(pa.EndDate,@FutureDate)-1 >= FPH.HireDate AND pa.StartDate <= FPH.TerminationDate ORDER BY pa.StartDate DESC ) pay
			WHERE FPH.TerminationDate BETWEEN @StartDate AND @EndDate AND ISNULL(FPH.TerminationReasonId,0) != @FormalyInactive
		),
		PersonTerminationInRange
		AS 
		(
			SELECT TT.StartDate,
					SUM(CASE WHEN FPT.Timescale = @W2SalaryId THEN 1 ELSE 0 END) AS TerminationsW2SalaryCountInTheRange,
					SUM(CASE WHEN FPT.Timescale = @W2HourlyId THEN 1 ELSE 0 END) AS TerminationsW2HourlyCountInTheRange,
					SUM(CASE WHEN FPT.Timescale = @1099HourlyId THEN 1 ELSE 0 END) AS Terminations1099HourlyCountInTheRange,
					SUM(CASE WHEN FPT.Timescale = @1099PROId THEN 1 ELSE 0 END) AS Terminations1099PORCountInTheRange,
					COUNT(FPT.PersonId) AS TerminationsCountInTheRange
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
			AND CPH.PersonStatusId != 3 --Projected status
		),
		FilteredPersonHireHistory1
		AS
		(
			SELECT FPH.*
			FROM FilteredPersonHireHistory FPH
			OUTER APPLY (SELECT TOP 1 pa.* FROM dbo.Pay pa WHERE pa.Person = FPH.PersonId AND ISNULL(pa.EndDate,@FutureDate)-1 >= FPH.HireDate AND pa.StartDate <= FPH.TerminationDate ORDER BY pa.StartDate DESC ) pay
			WHERE pay.Timescale IN (@W2SalaryId,@W2HourlyId) 
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
				FPTH.Terminations1099HourlyCountInTheRange,
				FPTH.Terminations1099PORCountInTheRange,
				FPTH.TerminationsCountInTheRange,
				AP.ActivePersonsAtTheBeginning
		FROM RangeValue TT
		INNER JOIN PersonTerminationInRange FPTH ON FPTH.StartDate = TT.StartDate
		INNER JOIN PersonHiriesInRange FPHH ON FPHH.StartDate = TT.StartDate
		LEFT JOIN ActivePersonsAtTheBeginningCTE AP ON AP.StartDate = TT.StartDate

END

