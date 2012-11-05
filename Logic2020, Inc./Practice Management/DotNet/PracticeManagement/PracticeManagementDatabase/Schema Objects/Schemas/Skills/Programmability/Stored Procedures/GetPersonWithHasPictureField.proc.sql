CREATE PROCEDURE [Skills].[GetPersonWithHasPictureField]
(
	@PersonId         INT
)
AS
BEGIN

	SELECT p.PersonId,
	       p.LastName,
		   p.FirstName,
		   CASE WHEN p.PersonPicture IS NULL THEN 0
		        ELSE 1 END AS 'HasPicture'		    
	FROM dbo.[Person] p
	WHERE p.PersonId = @PersonId
END
