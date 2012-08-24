CREATE PROCEDURE [dbo].[TerminationReport]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME,
	@TimeScaleIds	XML = null,
	@PersonStatusIds	XML = null,
	@SeniorityIds	XML = null,
	@TerminationReasonIds XML = NULL,
	@PracticeIds	XML = null,
	@ExcludeInternalPractices	BIT,
	@PersonDivisionIds	XML = null,
	@RecruiterIds	XML = null,
	@HireDates	XML = null,
	@TerminationDates	XML = null
)
AS
BEGIN
	DECLARE @FutureDate DATETIME,@W2SalaryId INT ,@W2HourlyId INT 
	SET @FutureDate = dbo.GetFutureDate()
	SELECT @W2SalaryId = TimescaleId FROM Timescale WHERE Name = 'W2-Salary'
	SELECT @W2HourlyId = TimescaleId FROM Timescale WHERE Name = 'W2-Hourly'

	;WITH FilteredPersonHistory
	AS
	(
		SELECT CPH.*
		FROM v_PersonHistory CPH
		WHERE CPH.TerminationDate BETWEEN @Startdate AND @Enddate
	)

	SELECT DISTINCT P.PersonId,
			P.FirstName,
			P.LastName,
			Ps.PersonStatusId,
			PS.Name AS PersonStatusName,
			TS.TimescaleId AS Timescale,
			TS.Name AS TimescaleName,
			FPH.RecruiterId,
			RCP.FirstName AS RecruiterFirstName ,
			RCP.LastName RecruiterLastName,
			S.SeniorityId,
			S.Name AS SeniorityName,
			FPH.DivisionId,
			FPH.HireDate,
			FPH.TerminationDate,
			FPH.TerminationReasonId,
			TR.TerminationReason
	FROM FilteredPersonHistory FPH
	INNER JOIN dbo.Calendar C ON C.Date = FPH.HireDate
	INNER JOIN dbo.Calendar C1 ON C1.Date = FPH.TerminationDate
	INNER JOIN dbo.Person P ON FPH.PersonId = P.PersonId
	INNER JOIN dbo.PersonStatus PS ON PS.PersonStatusId = FPH.PersonStatusId
	INNER JOIN dbo.TerminationReasons TR ON TR.TerminationReasonId = FPH.TerminationReasonId
	OUTER APPLY (SELECT TOP 1 pa.* FROM dbo.Pay pa WHERE pa.Person = FPH.PersonId AND ISNULL(pa.EndDate,@FutureDate)-1 >= FPH.HireDate AND pa.StartDate <= FPH.TerminationDate ORDER BY pa.StartDate DESC ) pay
	LEFT JOIN dbo.Timescale TS ON TS.TimescaleId = Pay.Timescale
	LEFT JOIN dbo.Practice Pra ON Pra.PracticeId = Pay.PracticeId
	LEFT JOIN dbo.Person RCP ON FPH.RecruiterId = RCP.PersonId
	LEFT JOIN dbo.Seniority S ON S.[SeniorityId] = Pay.[SeniorityId]
	WHERE	(
				@PersonStatusIds IS NULL
				OR PS.PersonStatusId IN ( SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@PersonStatusIds))
			)
			AND 
			(
				@TimeScaleIds IS NULL
				OR ISNULL(TS.TimescaleId,0) IN ( SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@TimeScaleIds))
			)
			AND 
			(
				@PracticeIds IS NULL
				OR Pay.PracticeId IN ( SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@PracticeIds))
			)
			AND 
			(
				@PersonDivisionIds IS NULL
				OR ISNULL(FPH.DivisionId,0) IN ( SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@PersonDivisionIds))
			)
			AND
			( 
				@SeniorityIds IS NULL
				OR ISNULL(S.SeniorityId,0) IN (SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@SeniorityIds))
			)
			AND 
			( 
				@HireDates IS NULL
				OR C.MonthStartDate IN (SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@HireDates))
			)
			AND 
			( 
				@TerminationDates IS NULL
				OR C1.MonthStartDate IN (SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@TerminationDates))
			)
			AND 
			( 
				@RecruiterIds IS NULL
				OR  ISNULL(FPH.RecruiterId,0) IN (SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@RecruiterIds))
			)
			AND
			( 
				@TerminationReasonIds IS NULL
				OR TR.TerminationReasonId IN (SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@TerminationReasonIds))
			)
			AND 
			(
				@ExcludeInternalPractices = 0 OR ( @ExcludeInternalPractices = 1 AND Pra.IsCompanyInternal = 0 )
			)


		DECLARE @ActivePersonsAtTheBeginning INT , @NewHiredInTheRange INT
		
		SELECT @ActivePersonsAtTheBeginning = COUNT(DISTINCT PSH.PersonId)
		FROM dbo.PersonStatusHistory PSH 
		INNER JOIN dbo.Person P ON PSH.PersonId = P.PersonId AND P.IsStrawman = 0 AND PSH.personstatusId = 1 AND @StartDate BETWEEN PSH.StartDate AND ISNULL(PSH.EndDate,@FutureDate) 
		INNER JOIN dbo.Pay pa ON pa.Person = PSH.PersonId AND @StartDate  BETWEEN pa.StartDate  AND ISNULL(pa.EndDate,@FutureDate) AND pa.Timescale IN (@W2SalaryId,@W2HourlyId) 

	;WITH FilteredPersonHistory
	AS
	(
		SELECT CPH.PersonId,
				CPH.HireDate,
				CPH.PersonStatusId,
				CASE WHEN ISNULL(CPH.TerminationDate,@FutureDate) > @EndDate THEN @EndDate ELSE ISNULL(CPH.TerminationDate,@FutureDate) END  AS TerminationDate,
				CPH.Id,
				CPH.DivisionId,
				CPH.TerminationReasonId,
				CPH.RecruiterId
		FROM v_PersonHistory CPH
		WHERE CPH.HireDate BETWEEN @Startdate AND @Enddate
	)

	SELECT @NewHiredInTheRange = COUNT(*)			
	FROM FilteredPersonHistory FPH
	OUTER APPLY (SELECT TOP 1 pa.* FROM dbo.Pay pa WHERE pa.Person = FPH.PersonId AND ISNULL(pa.EndDate,@FutureDate)-1 >= FPH.HireDate AND pa.StartDate <= FPH.TerminationDate ORDER BY pa.StartDate DESC ) pay
	WHERE	pay.Timescale IN (@W2SalaryId,@W2HourlyId) 

	SELECT ISNULL(@ActivePersonsAtTheBeginning,0) AS [ActivePersonsAtTheBeginning], ISNULL(@NewHiredInTheRange,0) AS [NewHiredInTheRange] 

END

