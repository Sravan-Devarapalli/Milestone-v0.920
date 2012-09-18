CREATE VIEW [dbo].[v_PersonHistoryAndStrawman]
AS 
	WITH PersonHistoryWithRowNo
	AS 
	(
	SELECT ROW_NUMBER () OVER (partition by PH.PersonId ORDER BY PH.id) as RowNumber,
			PersonId,
			HireDate,
			TerminationDate,
			PersonStatusId,
			PH.Id
	FROM dbo.PersonHistory PH
	WHERE PH.IsStrawman = 0 
	)

	SELECT PH1.PersonId,
			PH1.HireDate,
			PH1.PersonStatusId,
			PH1.TerminationDate AS TerminationDate
	FROM PersonHistoryWithRowNo  PH1
	LEFT JOIN PersonHistoryWithRowNo PH2 ON PH1.PersonId = PH2.PersonId AND PH1.RowNumber + 1 = PH2.RowNumber
	WHERE PH2.PersonId IS NULL OR (PH1.PersonStatusId = 2 AND  PH1.TerminationDate < PH2.HireDate)
	UNION ALL
	SELECT P.PersonId,
			P.HireDate,
			P.PersonStatusId,
			P.TerminationDate
	FROM dbo.Person P
	WHERE IsStrawman = 1
