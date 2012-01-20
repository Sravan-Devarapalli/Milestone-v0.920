﻿CREATE PROCEDURE ProjectGroupInsert
	@GroupId	INT OUT,
	@ClientId	INT,
	@Name		NVARCHAR(100),
	@IsActive   BIT 
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
		DECLARE @IsInternal BIT,
				@ProjectGroupCode NVARCHAR (5)
				
		SELECT @IsInternal = c.IsInternal FROM dbo.Client c  WHERE c.ClientId = @ClientId

		EXEC [GenerateNewProjectGroupCode] @IsInternal, @ProjectGroupCode OUTPUT

		INSERT ProjectGroup(ClientId, Name,Active,Code) 
		VALUES (@ClientId, @Name,@IsActive,@ProjectGroupCode)

		SET @GroupId = SCOPE_IDENTITY()

		END TRY
		BEGIN CATCH
			DECLARE @ErrorMessage NVARCHAR(2048)
			SELECT @ErrorMessage = ERROR_MESSAGE()
			
			RAISERROR (@ErrorMessage, 16, 1)
		END CATCH
	END
	 

