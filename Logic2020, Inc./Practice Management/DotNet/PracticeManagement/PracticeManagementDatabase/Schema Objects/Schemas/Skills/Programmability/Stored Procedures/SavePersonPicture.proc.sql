CREATE PROCEDURE [Skills].[SavePersonPicture]
(
	@PersonId         INT,		
	@PictureFileName  NVARCHAR(MAX),
	@PictureData	  VARBINARY(MAX) = NULL,
	@UserLogin		  NVARCHAR (100)      
)
AS
BEGIN
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
	DECLARE @Now DATETIME
	SET @Now = dbo.InsertingTime()	

	UPDATE dbo.Person
	SET PictureData = @PictureData,
	    PictureFileName = @PictureFileName,
		PictureModifiedDate = @Now
	WHERE PersonId = @PersonId 
		AND ISNULL(PictureData,  CONVERT(VARBINARY(2), '')) <> ISNULL(@PictureData,  CONVERT(VARBINARY(2), ''))

	EXEC dbo.SessionLogUnprepare
END

