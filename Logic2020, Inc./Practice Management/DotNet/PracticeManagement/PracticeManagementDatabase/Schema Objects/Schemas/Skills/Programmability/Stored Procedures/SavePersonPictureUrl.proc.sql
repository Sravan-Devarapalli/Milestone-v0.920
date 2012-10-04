CREATE PROCEDURE [Skills].[SavePersonPictureUrl]
(
	@PersonId         INT,		
	@PictureUrl		  NVARCHAR (MAX) = NULL,
	@UserLogin		  NVARCHAR (100)      
)
AS
BEGIN
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
	DECLARE @Now DATETIME
	SET @Now = dbo.InsertingTime()

	UPDATE dbo.Person
	SET PictureUrl = @PictureUrl,
		PictureModifiedDate = @Now
	WHERE PersonId = @PersonId

	EXEC dbo.SessionLogUnprepare
END

