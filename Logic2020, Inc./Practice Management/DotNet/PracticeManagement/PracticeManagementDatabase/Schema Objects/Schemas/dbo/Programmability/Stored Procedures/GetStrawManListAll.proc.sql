CREATE PROCEDURE [dbo].[GetStrawManListAll]
AS
	SELECT	PersonId,
			LastName,
			FirstName
	FROM dbo.Person
	WHERE IsStrawman = 1
RETURN 0
