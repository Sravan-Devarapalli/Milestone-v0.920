CREATE PROCEDURE [dbo].[NewHireReport]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME,
	@PersonStatusIds	XML = null,
	@TimeScaleIds	XML = null,
	@PracticeIds	XML = null,
	@ExcludeInternalPractices	BIT,
	@PersonDivisionIds	XML = null,
	@TitleIds	XML = null,
	@HireDates	XML = null,
	@RecruiterIds	XML = null
)
AS
BEGIN
	SELECT @StartDate = CONVERT(DATETIME,CONVERT(DATE,@StartDate)),@EndDate = CONVERT(DATETIME,CONVERT(DATE,@EndDate))

	DECLARE @FutureDate DATETIME
	SET @FutureDate = dbo.GetFutureDate()

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
			T.TitleId,
			T.Title,
			TT.TitleTypeId,
			TT.TitleType,
			FPH.DivisionId,
			FPH.HireDate
	FROM FilteredPersonHistory FPH
	INNER JOIN dbo.Calendar C ON C.Date = FPH.HireDate
	INNER JOIN dbo.Person P ON FPH.PersonId = P.PersonId
	INNER JOIN dbo.PersonStatus PS ON PS.PersonStatusId = FPH.PersonStatusId
	OUTER APPLY (SELECT TOP 1 pa.* FROM dbo.Pay pa WHERE pa.Person = FPH.PersonId AND ISNULL(pa.EndDate,@FutureDate)-1  >= FPH.HireDate AND pa.StartDate <= FPH.TerminationDate ORDER BY pa.StartDate DESC ) pay
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
				@RecruiterIds IS NULL
				OR  ISNULL(FPH.RecruiterId,0) IN (SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@RecruiterIds))
			)
			AND 
			(
				@ExcludeInternalPractices = 0 OR ( @ExcludeInternalPractices = 1 AND Pra.IsCompanyInternal = 0 )
			)

END

