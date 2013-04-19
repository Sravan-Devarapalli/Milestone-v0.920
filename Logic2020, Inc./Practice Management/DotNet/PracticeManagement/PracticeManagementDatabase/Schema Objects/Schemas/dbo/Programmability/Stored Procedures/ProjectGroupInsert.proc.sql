CREATE PROCEDURE ProjectGroupInsert
	@GroupId	INT OUT,
	@ClientId	INT,
	@Name		NVARCHAR(100),
	@IsActive   BIT,
	@UserLogin          NVARCHAR(255),
	@BusinessGroupId INT 
AS
	SET NOCOUNT ON
	IF NOT EXISTS(SELECT 1 FROM Client WHERE ClientId=@ClientId)
		OR EXISTS(SELECT 1 FROM ProjectGroup WHERE ClientId=@ClientId AND Name=@Name)
	BEGIN
		SET @GroupId = -1
	END
	ELSE
	BEGIN
		
		BEGIN TRY
		-- Start logging session
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
		DECLARE @IsInternal BIT,
				@ProjectGroupCode NVARCHAR (5)
				
		SELECT @IsInternal = c.IsInternal FROM dbo.Client c  WHERE c.ClientId = @ClientId

		EXEC [GenerateNewProjectGroupCode] @IsInternal, @ProjectGroupCode OUTPUT

		INSERT ProjectGroup(ClientId, Name,Active,Code,BusinessGroupId) 
		VALUES (@ClientId, @Name,@IsActive,@ProjectGroupCode,@BusinessGroupId)

		SET @GroupId = SCOPE_IDENTITY()

		-- End logging session
		EXEC dbo.SessionLogUnprepare

		END TRY
		BEGIN CATCH
			DECLARE @ErrorMessage NVARCHAR(2048)
			SELECT @ErrorMessage = ERROR_MESSAGE()
			
			RAISERROR (@ErrorMessage, 16, 1)
		END CATCH
	END
	 

