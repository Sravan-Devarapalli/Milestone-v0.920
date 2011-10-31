CREATE PROCEDURE [dbo].[PersonFirstLastNameById]
	@PersonId int
AS
	SELECT FirstName, LastName, IsStrawman
	FROM Person 
	WHERE PersonId = @PersonId
