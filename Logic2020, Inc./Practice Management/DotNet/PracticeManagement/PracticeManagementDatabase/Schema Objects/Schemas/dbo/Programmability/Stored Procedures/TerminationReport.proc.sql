CREATE PROCEDURE [dbo].[TerminationReport]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME,
	@TimeScaleIds	XML = null,
	@PersonStatusIds	XML = null,
	@TitleIds	XML = null,
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
	SELECT @StartDate = CONVERT(DATETIME,CONVERT(DATE,@StartDate)),@EndDate = CONVERT(DATETIME,CONVERT(DATE,@EndDate))
	DECLARE @FutureDate DATETIME,@W2SalaryId INT ,@W2HourlyId INT ,@FormalyInactive INT
	SET @FutureDate = dbo.GetFutureDate()
	SELECT @W2SalaryId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Salary'
	SELECT @W2HourlyId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Hourly'
	SELECT @FormalyInactive = TerminationReasonId  FROM dbo.TerminationReasons WHERE TerminationReason = 'Formerly Inactive status '

	;WITH FilteredPersonHistory
	AS
	(
		SELECT CPH.*
		FROM v_PersonHistory CPH
		WHERE CPH.TerminationDate BETWEEN @Startdate AND @Enddate
		AND ISNULL(CPH.TerminationReasonId,0) != @FormalyInactive
	)

	SELECT DISTINCT P.PersonId,
	        P.EmployeeNumber,
			P.FirstName,
			P.LastName,
			Ps.PersonStatusId,
			PS.Name AS PersonStatusName,
			TS.TimescaleId AS Timescale,
			TS.Name AS TimescaleName,
			FPH.RecruiterId,
			RCP.FirstName AS RecruiterFirstName ,
			RCP.LastName RecruiterLastName,
			T.TitleId,
			T.Title,
			TT.TitleTypeId,
			TT.TitleType,
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
	LEFT JOIN dbo.TerminationReasons TR ON TR.TerminationReasonId = FPH.TerminationReasonId
	OUTER APPLY (SELECT TOP 1 pa.* FROM dbo.Pay pa WHERE pa.Person = FPH.PersonId AND ISNULL(pa.EndDate,@FutureDate)-1 >= FPH.HireDate AND pa.StartDate <= FPH.TerminationDate ORDER BY pa.StartDate DESC ) pay
	LEFT JOIN dbo.Timescale TS ON TS.TimescaleId = Pay.Timescale
	LEFT JOIN dbo.Practice Pra ON Pra.PracticeId = Pay.PracticeId
	LEFT JOIN dbo.Person RCP ON FPH.RecruiterId = RCP.PersonId
	LEFT JOIN dbo.Title T ON T.TitleId = Pay.TitleId
	LEFT JOIN dbo.TitleType TT ON TT.TitleTypeId = T.TitleTypeId
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
				@TitleIds IS NULL
				OR ISNULL(T.TitleId,0) IN (SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@TitleIds))
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

END

