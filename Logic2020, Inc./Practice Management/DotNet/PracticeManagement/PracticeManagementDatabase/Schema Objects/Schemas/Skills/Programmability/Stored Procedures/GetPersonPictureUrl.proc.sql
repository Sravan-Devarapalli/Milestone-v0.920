CREATE PROCEDURE [Skills].[GetPersonPictureUrl]
(
	@PersonId         INT
)
AS
BEGIN

	SELECT p.PictureUrl
	FROM dbo.[Person] p
	WHERE p.PersonId = @PersonId
END
