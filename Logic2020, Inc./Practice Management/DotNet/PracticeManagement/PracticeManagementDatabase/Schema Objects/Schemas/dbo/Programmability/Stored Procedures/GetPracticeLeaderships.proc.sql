CREATE PROCEDURE [dbo].[GetPracticeLeaderships]
(
	@DivisionId	INT=NULL	
)
AS
BEGIN

	SELECT P.PersonId,
		   P.LastName,
		   P.FirstName,
		   PD.DivisionId,
		   PD.DivisionName
	FROM dbo.PracticeLeadership PL
	JOIN v_Person P ON P.PersonId = PL.PersonId
	JOIN dbo.PersonDivision PD ON PD.DivisionId = PL.DivisionId
	WHERE (@DivisionId IS NULL OR PL.DivisionId = @DivisionId)

END
