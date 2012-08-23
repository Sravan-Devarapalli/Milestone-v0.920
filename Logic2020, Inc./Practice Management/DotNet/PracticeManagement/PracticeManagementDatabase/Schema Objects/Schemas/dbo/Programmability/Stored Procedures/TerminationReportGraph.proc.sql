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
		FilteredPersonTerminationHistory
		AS
		(
			SELECT TT.StartDate, COUNT(*) TerminationsCount
			FROM RangeValue TT
			LEFT JOIN v_PersonHistory FPH ON FPH.TerminationDate BETWEEN TT.StartDate AND TT.EndDate
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
			GROUP BY TT.StartDate, TT.EndDate
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
					CPH.TerminationReasonId,
					TT.StartDate,
					TT.EndDate
			FROM RangeValue TT
			LEFT JOIN v_PersonHistory CPH ON CPH.HireDAte BETWEEN TT.StartDate AND TT.EndDate
		),
		FilteredPersonHireHistory1
		AS
		(
			SELECT FPH.StartDate, COUNT(FPH.PersonId) NewHireCount
			FROM FilteredPersonHireHistory FPH
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
			GROUP BY FPH.StartDate, FPH.EndDate
		)


			SELECT TT.StartDate,
					TT.EndDate,
					FPHH.NewHireCount AS NewHiredInTheRange,
					FPTH.TerminationsCount AS TerminationsInTheRange,
					AP.ActivePersonsAtTheBeginning
			FROM RangeValue TT
			INNER JOIN FilteredPersonTerminationHistory FPTH ON FPTH.StartDate = TT.StartDate
			INNER JOIN FilteredPersonHireHistory1 FPHH ON FPHH.StartDate = TT.StartDate
			LEFT JOIN ActivePersonsAtTheBeginningCTE AP ON AP.StartDate = TT.StartDate
END
