CREATE PROCEDURE [dbo].[GetTemporaryCredentialsByUserName]
(
	@UserName		NVARCHAR (256)
)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE  @Now	DATETIME
	SELECT @Now = GETDATE()

	DELETE FROM  [dbo].[UserTemporaryCredentials]
	WHERE UserName = @UserName
		  AND ([CreatedDate]+1) <= @Now

	SELECT [UserName],[Password],[PasswordSalt]
	FROM [dbo].[UserTemporaryCredentials]
	WHERE UserName = @UserName
		  AND ([CreatedDate]+1) > @Now

	
END
