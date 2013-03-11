CREATE PROCEDURE [dbo].[CapabilityUpdate]
(
	@CapabilityId INT, 
	@Name NVARCHAR(100),
	@IsActive BIT,
	@UserLogin NVARCHAR(MAX)
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN CapabilityUpdate_Tran;

		SELECT @Name = REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(@Name)),' ','<>'),'><',''),'<>',' ')

		DECLARE @Error NVARCHAR(MAX)
		IF EXISTS(SELECT 1 FROM dbo.[PracticeCapabilities] WHERE CapabilityName = @Name AND CapabilityId != @CapabilityId )
		BEGIN
			SET @Error = 'Capability name already exists. Please enter a different capability name.'
			RAISERROR(@Error,16,1)
		END

		EXEC SessionLogPrepare @UserLogin = @UserLogin

		
		UPDATE [dbo].[PracticeCapabilities]
		SET CapabilityName = @Name,
			IsActive = @IsActive 
		WHERE CapabilityId = @CapabilityId AND (CapabilityName != @Name OR IsActive != @IsActive)

		COMMIT TRAN CapabilityUpdate_Tran;
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN CapabilityUpdate_Tran;
		DECLARE	 @ERROR_STATE	tinyint
				,@ERROR_SEVERITY		tinyint
				,@ERROR_MESSAGE		    nvarchar(2000)
				,@InitialTranCount		tinyint

		SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		SET  @ERROR_SEVERITY	= ERROR_SEVERITY()
		SET  @ERROR_STATE		= ERROR_STATE()
		RAISERROR ('%s', @ERROR_SEVERITY, @ERROR_STATE, @ERROR_MESSAGE)
	END CATCH
	EXEC dbo.SessionLogUnprepare
END

