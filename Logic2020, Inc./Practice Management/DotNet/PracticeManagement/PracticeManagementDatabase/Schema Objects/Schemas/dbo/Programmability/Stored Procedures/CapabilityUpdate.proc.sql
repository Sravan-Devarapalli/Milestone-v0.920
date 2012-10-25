CREATE PROCEDURE [dbo].[CapabilityUpdate]
(
	@CapabilityId INT, 
	@Name NVARCHAR(100),
	@UserLogin NVARCHAR(MAX)
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN CapabilityUpdate_Tran;

		DECLARE @Error NVARCHAR(MAX)
		IF EXISTS(SELECT 1 FROM dbo.[PracticeCapabilities] WHERE CapabilityName = @Name AND CapabilityId != @CapabilityId )
		BEGIN
			SET @Error = 'This Capability already exists. Please enter a different Capability.'
			RAISERROR(@Error,16,1)
		END

		EXEC SessionLogPrepare @UserLogin = @UserLogin

		UPDATE [dbo].[PracticeCapabilities]
		SET CapabilityName = @Name
		WHERE CapabilityId = @CapabilityId AND CapabilityName != @Name

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
