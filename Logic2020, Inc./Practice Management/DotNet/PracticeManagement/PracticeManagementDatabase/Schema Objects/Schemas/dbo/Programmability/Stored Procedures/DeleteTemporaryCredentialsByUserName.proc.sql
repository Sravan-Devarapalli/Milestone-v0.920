CREATE PROCEDURE [dbo].[DeleteTemporaryCredentialsByUserName]
(
	@UserName		NVARCHAR (256) 
)
AS
BEGIN

	DELETE
	FROM [dbo].[UserTemporaryCredentials]
	WHERE UserName = @UserName

END
