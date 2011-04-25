CREATE PROCEDURE GetPersonListByPersonIds
(
	@PersonIds NVARCHAR(MAX)
)
AS
BEGIN
	SELECT  PersonId,
			FirstName,
			LastName,
			IsDefaultManager
	FROM Person
	WHERE PersonId in (SELECT ResultId FROM dbo.ConvertStringListIntoTable(@PersonIds))
END
