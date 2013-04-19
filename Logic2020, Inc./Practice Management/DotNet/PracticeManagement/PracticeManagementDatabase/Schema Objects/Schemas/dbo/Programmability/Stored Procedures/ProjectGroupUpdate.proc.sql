CREATE PROCEDURE ProjectGroupUpdate
(
	@ClientId		INT,
	@GroupId    INT ,
	@GroupName	NVARCHAR(100),
	@IsActive   BIT,
	@UserLogin          NVARCHAR(255),
	@BusinessGroupId INT 
)
AS
BEGIN
	BEGIN TRY
	BEGIN TRAN  ProjectGroupUpdate_tran

		-- Start logging session
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

		UPDATE ProjectGroup
		SET Name = @GroupName , Active = @IsActive, BusinessGroupId=@BusinessGroupId
		WHERE ClientId = @ClientId AND GroupId = @GroupId 

		SELECT 0 Result
		-- End logging session
		EXEC dbo.SessionLogUnprepare

	COMMIT TRAN ProjectGroupUpdate_tran
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN ProjectGroupUpdate_tran
		
		DECLARE @ErrorMessage NVARCHAR(MAX)
		SET @ErrorMessage = ERROR_MESSAGE()

		RAISERROR(@ErrorMessage, 16, 1)
	END CATCH
END
