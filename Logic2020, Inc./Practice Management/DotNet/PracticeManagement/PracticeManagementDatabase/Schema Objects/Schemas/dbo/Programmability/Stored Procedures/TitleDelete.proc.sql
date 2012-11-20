CREATE PROCEDURE [dbo].[TitleDelete]
(
	@TitleId		INT,
	@UserLogin  NVARCHAR(255) 
)
AS
BEGIN
	EXEC SessionLogPrepare @UserLogin = @UserLogin

	DELETE dbo.Title
	WHERE TitleId = @TitleId

	EXEC SessionLogUnprepare 
END
