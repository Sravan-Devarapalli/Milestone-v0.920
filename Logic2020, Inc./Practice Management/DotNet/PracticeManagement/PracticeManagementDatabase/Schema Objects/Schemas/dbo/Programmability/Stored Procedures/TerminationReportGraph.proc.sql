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
	DECLARE @FutureDate DATETIME
	SET @FutureDate = dbo.GetFutureDate()
		
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
		PersonHistoryWithRowNo
		AS 
		(
		SELECT ROW_NUMBER () OVER (partition by PH.PersonId ORDER BY PH.id) as RowNumber,
			PersonId,
			HireDate,
			TerminationDate,
			PersonStatusId,
			PH.Id,
			PH.DivisionId,
			PH.TerminationReasonId
		FROM dbo.PersonHistory PH
		),
		CorrectPersonHistory
		AS 
		(
		SELECT PH1.PersonId,
			PH1.HireDate,
			PH1.PersonStatusId,
			CASE WHEN ISNULL(PH1.TerminationDate,@FutureDate) > @EndDate THEN @EndDate ELSE ISNULL(PH1.TerminationDate,@FutureDate) END  AS TerminationDate,
			PH1.id,
			PH1.DivisionId,
			PH1.TerminationReasonId
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
		,
		FilteredPersonHistory1
		AS
		(
		SELECT DISTINCT P.PersonId,
				FPH.PersonStatusId,
				RC.RecruiterId,
				Pay.SeniorityId,
				FPH.DivisionId,
				FPH.HireDate,
				FPH.TerminationDate,
				FPH.TerminationReasonId
		--SELECT COUNT(*)	AS NewHiredInTheRange ,SUM(CASE WHEN FPH.PersonStatusId = 2 THEN 1 ELSE 0 END ) AS TerminationsInTheRange
		FROM FilteredPersonHistory FPH
		INNER JOIN dbo.Person P ON FPH.PersonId = P.PersonId
		INNER JOIN dbo.TerminationReasons TR ON TR.TerminationReasonId = FPH.TerminationReasonId
		OUTER APPLY (SELECT TOP 1 pa.* FROM dbo.Pay pa WHERE pa.Person = FPH.PersonId AND ISNULL(pa.EndDate,@FutureDate)-1 >= FPH.HireDate AND pa.StartDate <= FPH.TerminationDate ORDER BY pa.StartDate DESC ) pay
		OUTER APPLY (SELECT TOP 1 RCH.* FROM dbo.RecruiterCommissionHistory RCH WHERE RCH.RecruitId = FPH.PersonId AND ISNULL(RCH.EndDate,@FutureDate) >= FPH.HireDate AND RCH.StartDate <= FPH.TerminationDate ORDER BY RCH.StartDate DESC ) RC
		LEFT JOIN dbo.Practice Pra ON Pra.PracticeId = Pay.PracticeId
		WHERE	(
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
		--GROUP BY P.PersonId,
		--		FPH.PersonStatusId,
		--		RC.RecruiterId,
		--		Pay.SeniorityId,
		--		FPH.DivisionId,
		--		FPH.HireDate,
		--		FPH.TerminationDate,
		--		FPH.TerminationReasonId,
			)

			SELECT TT.StartDate,
					TT.EndDate,
					COUNT(*) AS NewHiredInTheRange ,
					SUM(CASE WHEN FPH.PersonStatusId = 2 THEN 1 ELSE 0 END ) AS TerminationsInTheRange,
					AP.ActivePersonsAtTheBeginning
			FROM RangeValue TT
			LEFT JOIN FilteredPersonHistory1 FPH ON FPH.HireDate BETWEEN TT.StartDate AND TT.EndDate
			LEFT JOIN ActivePersonsAtTheBeginningCTE AP ON AP.StartDate = TT.StartDate
			GROUP BY 
				TT.StartDate,
				TT.EndDate,
				AP.ActivePersonsAtTheBeginning
				--FPH.PersonId,
				--FPH.PersonStatusId,
				--FPH.RecruiterId,
				--FPH.SeniorityId,
				--FPH.DivisionId,
				--FPH.HireDate,
				--FPH.TerminationDate,
				--FPH.TerminationReasonId
END
