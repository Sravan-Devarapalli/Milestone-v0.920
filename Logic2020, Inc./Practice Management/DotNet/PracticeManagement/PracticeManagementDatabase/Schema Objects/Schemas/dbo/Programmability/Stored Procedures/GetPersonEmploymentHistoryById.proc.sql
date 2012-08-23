CREATE PROCEDURE [dbo].[GetPersonEmploymentHistoryById]
(
	@PersonId INT
)
AS
BEGIN
	
	SELECT PH.PersonId,
			PH.HireDate,
			PH.TerminationDate,
			PH.TerminationReasonId
	FROM v_PersonHistory PH
	WHERE PH.PersonId = @PersonId

	--;WITH History AS
	--(
	--	SELECT P.*, ROW_NUMBER() OVER(ORDER BY P.Id) AS RowIndex
	--	FROM dbo.PersonHistory P
	--	WHERE P.PersonId = @PersonId
	--)

	--SELECT H.PersonId,
	--		H.HireDate,
	--		H.TerminationDate,
	--		H.TerminationReasonId
	--FROM History H
	--LEFT JOIN History H2 ON H.RowIndex + 1 = H2.RowIndex
	--WHERE H2.PersonId IS NULL OR (H.PersonStatusId = 2 AND H.TerminationDate < H2.HireDate)

END
