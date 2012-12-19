CREATE VIEW [dbo].[v_PersonHistory]
	AS 
	WITH PersonHistoryWithRowNo
	AS 
	(
	SELECT ROW_NUMBER () OVER (partition by PH.PersonId ORDER BY PH.id) as RowNumber,
			PersonId,
			HireDate,
			TerminationDate,
			TerminationReasonId,
			PersonStatusId,
			PH.Id,
			PH.DivisionId,
			PH.RecruiterId,
			PH.CreatedDate,
			PH.TitleId
	FROM dbo.PersonHistory PH
	WHERE PH.IsStrawman = 0 
	)

	SELECT PH1.PersonId,
			PH1.HireDate,
			PH1.PersonStatusId,
			PH1.TerminationDate AS TerminationDate,
			PH1.TerminationReasonId ,
			PH1.id,
			PH1.DivisionId,
			PH1.RecruiterId,
			PH1.CreatedDate,
			PH1.TitleId
	FROM PersonHistoryWithRowNo  PH1
	LEFT JOIN PersonHistoryWithRowNo PH2 ON PH1.PersonId = PH2.PersonId AND PH1.RowNumber + 1 = PH2.RowNumber
	WHERE PH2.PersonId IS NULL OR (PH1.PersonStatusId = 2 AND  PH1.TerminationDate < PH2.HireDate)

