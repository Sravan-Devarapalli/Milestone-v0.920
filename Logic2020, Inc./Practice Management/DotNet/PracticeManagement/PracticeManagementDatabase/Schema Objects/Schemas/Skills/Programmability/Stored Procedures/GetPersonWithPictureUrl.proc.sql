CREATE PROCEDURE [Skills].[GetPersonWithPictureUrl]
(
	@PersonId         INT
)
AS
BEGIN

	SELECT p.PersonId,p.LastName,p.FirstName,p.PictureUrl
	FROM dbo.[Person] p
	WHERE p.PersonId = @PersonId
END
