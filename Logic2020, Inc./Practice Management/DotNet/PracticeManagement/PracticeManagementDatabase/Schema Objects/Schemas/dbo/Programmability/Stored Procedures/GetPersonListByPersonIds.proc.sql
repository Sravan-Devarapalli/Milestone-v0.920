CREATE PROCEDURE GetPersonListByPersonIds
(
	@PersonIds NVARCHAR(MAX)
)
AS
BEGIN
	SELECT  P.PersonId,
			P.FirstName,
			P.LastName,
			P.IsDefaultManager,
			P.PersonStatusId
	FROM dbo.Person P
	WHERE P.PersonId in (SELECT ResultId FROM dbo.ConvertStringListIntoTable(@PersonIds))
END
