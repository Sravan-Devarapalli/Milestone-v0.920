CREATE PROCEDURE [Skills].[SavePersonPicture]
(
	@PersonId         INT,		
	@PictureFileName  NVARCHAR(MAX),
	@PictureData	  VARBINARY(MAX),
	@UserLogin		  NVARCHAR (100)      
)
AS
BEGIN
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
	DECLARE @Now DATETIME
	SET @Now = dbo.InsertingTime()	

	UPDATE dbo.Person
	SET PersonPicture = @PictureData,
	    PictureFileName = @PictureFileName,
		PictureModifiedDate = @Now
	WHERE PersonId = @PersonId 
		AND ISNULL(PersonPicture,0) <> ISNULL(@PictureData,0)

	EXEC dbo.SessionLogUnprepare
END

