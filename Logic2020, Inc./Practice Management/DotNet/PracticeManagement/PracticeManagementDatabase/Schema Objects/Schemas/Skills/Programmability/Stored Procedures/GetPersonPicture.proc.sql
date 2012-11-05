CREATE PROCEDURE [Skills].[GetPersonPicture]
(
	@PersonId         INT
)
AS
BEGIN

	SELECT p.PersonPicture
	FROM dbo.[Person] p
	WHERE p.PersonId = @PersonId
END
