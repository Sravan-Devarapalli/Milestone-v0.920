﻿CREATE PROCEDURE [dbo].[NewHireReport]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME,
	@PersonStatusIds	XML = null,
	@TimeScaleIds	XML = null,
	@PracticeIds	XML = null,
	@ExcludeInternalPractices	BIT,
	@PersonDivisionIds	XML = null,
	@SeniorityIds	XML = null,
	@HireDates	XML = null,
	@RecruiterIds	XML = null
)
AS
BEGIN

	DECLARE @FutureDate DATETIME
	SET @FutureDate = dbo.GetFutureDate()

	;WITH PersonHistoryWithRowNo
	AS 
	(
	SELECT ROW_NUMBER () OVER (partition by PH.PersonId ORDER BY PH.id) as RowNumber,
			PersonId,
			HireDate,
			TerminationDate,
			PersonStatusId,
			PH.Id,
			PH.DivisionId
	FROM dbo.PersonHistory PH
	),
	CorrectPersonHistory
	AS 
	(
	SELECT PH1.PersonId,
			PH1.HireDate,
			PH1.PersonStatusId,
			ISNULL(PH1.TerminationDate,@EndDate) AS TerminationDate,
			PH1.id,
			PH1.DivisionId
	FROM PersonHistoryWithRowNo  PH1
	LEFT JOIN PersonHistoryWithRowNo PH2 ON PH1.PersonId = PH2.PersonId AND PH1.RowNumber + 1 = PH2.RowNumber
	WHERE (PH2.PersonId IS NULL) OR (PH1.PersonStatusId = 2 AND PH1.TerminationDate < PH2.HireDate)
	),
	FilteredPersonHistory
	AS
	(
		SELECT CPH.*
		FROM CorrectPErsonHistory CPH
		WHERE CPH.HireDate BETWEEN @Startdate AND @Enddate
	)
	SELECT DISTINCT P.PersonId,
			P.FirstName,
			P.LastName,
			Ps.PersonStatusId,
			PS.Name AS PersonStatusName,
			TS.TimescaleId AS Timescale,
			TS.Name AS TimescaleName,
			RC.RecruiterId,
			RCP.FirstName AS RecruiterFirstName ,
			RCP.LastName RecruiterLastName,
			S.SeniorityId,
			S.Name AS SeniorityName,
			FPH.DivisionId,
			FPH.HireDate
	FROM FilteredPersonHistory FPH
	INNER JOIN dbo.Calendar C ON C.Date = FPH.HireDate
	INNER JOIN dbo.Person P ON FPH.PersonId = P.PersonId
	INNER JOIN dbo.PersonStatus PS ON PS.PersonStatusId = FPH.PersonStatusId
	OUTER APPLY (SELECT TOP 1 pa.* FROM dbo.Pay pa WHERE pa.Person = FPH.PersonId AND pa.EndDate >= FPH.HireDate AND pa.StartDate <= FPH.TerminationDate ORDER BY pa.StartDate DESC ) pay
	LEFT JOIN dbo.Timescale TS ON TS.TimescaleId = Pay.Timescale
	LEFT JOIN dbo.Practice Pra ON Pra.PracticeId = Pay.PracticeId
	OUTER APPLY (SELECT TOP 1 RCH.* FROM dbo.RecruiterCommissionHistory RCH WHERE RCH.RecruitId = FPH.PersonId AND RCH.EndDate >= FPH.HireDate AND RCH.StartDate <= FPH.TerminationDate ORDER BY RCH.StartDate DESC ) RC
	LEFT JOIN dbo.Person RCP ON RC.RecruiterId = RCP.PersonId
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
				@RecruiterIds IS NULL
				OR  ISNULL(RC.RecruiterId,0) IN (SELECT  ResultString FROM    dbo.[ConvertXmlStringInToStringTable](@RecruiterIds))
			)
			AND 
			(
				@ExcludeInternalPractices = 0 OR ( @ExcludeInternalPractices = 1 AND Pra.IsCompanyInternal = 0 )
			)

END

