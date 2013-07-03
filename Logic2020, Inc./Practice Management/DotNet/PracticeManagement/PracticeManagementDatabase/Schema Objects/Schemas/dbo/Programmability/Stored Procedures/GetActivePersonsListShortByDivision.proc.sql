CREATE PROCEDURE [dbo].[GetActivePersonsListShortByDivision]
(
	@DivisionId INT
)
AS
BEGIN
  
  SELECT	P.PersonId,
			P.FirstName,
			P.LastName
  FROM		dbo.Person P
  WHERE		P.DivisionId = @DivisionId AND P.PersonStatusId IN (1,5) --Active and Terminated Status

END
