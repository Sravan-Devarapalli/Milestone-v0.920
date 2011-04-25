CREATE PROCEDURE [dbo].[UserTemporaryCredentialsInsert]
(
	@UserName		NVARCHAR (256),
	@Password		NVARCHAR (128),
	@PasswordFormat	INT,
	@PasswordSalt	NVARCHAR (128)
)
AS
BEGIN

	DECLARE @LastCreatedDate DATETIME,
			@CreatedDate	DATETIME

	SELECT @CreatedDate = GETDATE()

	SELECT @LastCreatedDate = [CreatedDate]
	FROM [dbo].[UserTemporaryCredentials]
	WHERE UserName = @UserName
	
	IF  @LastCreatedDate IS NULL
	BEGIN
		INSERT INTO [dbo].[UserTemporaryCredentials]
           ([UserName]
           ,[Password]
           ,[PasswordFormat]
           ,[PasswordSalt]
           ,[CreatedDate])
		 VALUES
			   (@UserName
			   ,@Password
			   ,@PasswordFormat
			   ,@PasswordSalt
			   ,@CreatedDate)
	

	END
	ELSE IF (@LastCreatedDate+1) < @CreatedDate
	BEGIN
		UPDATE [dbo].[UserTemporaryCredentials]
		SET [Password] = @Password
           ,[PasswordFormat]= @PasswordFormat
           ,[PasswordSalt] = @PasswordSalt
           ,[CreatedDate] = @CreatedDate
        WHERE [UserName] = @UserName
	END
	ELSE
	BEGIN
		RETURN -1
	END
	
	RETURN 0
END
