CREATE PROCEDURE [dbo].[BusinessGroupInsert]
(
	@BusinessGroupId	INT OUT,
	@ClientId	INT,
	@Name		NVARCHAR(100),
	@IsActive   BIT ,
	@UserLogin          NVARCHAR(255)
)
AS
BEGIN
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	BEGIN TRY
	DECLARE @IsInternal BIT,
			@BusinessGroupCode NVARCHAR (6)
				
	SELECT @IsInternal = c.IsInternal FROM dbo.Client c  WHERE c.ClientId = @ClientId

	EXEC [GenerateNewBusinessGroupCode] @IsInternal, @BusinessGroupCode OUTPUT

	INSERT BusinessGroup(ClientId, Name,Active,Code) 
	VALUES (@ClientId, @Name,@IsActive,@BusinessGroupCode)

	SET @BusinessGroupId = SCOPE_IDENTITY()

	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(2048)
		SELECT @ErrorMessage = ERROR_MESSAGE()
			
		RAISERROR (@ErrorMessage, 16, 1)
	END CATCH

		-- End logging session
	EXEC dbo.SessionLogUnprepare
END

