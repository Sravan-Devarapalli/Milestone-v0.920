CREATE PROCEDURE [dbo].[CapabilityDelete]
(
	@CapabilityId INT
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN CapabilityDelete_Tran;

			DELETE [dbo].PracticeCapabilities
			WHERE CapabilityId = @CapabilityId

		COMMIT TRAN CapabilityDelete_Tran;
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN CapabilityDelete_Tran;
		DECLARE	 @ERROR_STATE	tinyint
				,@ERROR_SEVERITY		tinyint
				,@ERROR_MESSAGE		    nvarchar(2000)
				,@InitialTranCount		tinyint

		SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		SET  @ERROR_SEVERITY	= ERROR_SEVERITY()
		SET  @ERROR_STATE		= ERROR_STATE()
		RAISERROR ('%s', @ERROR_SEVERITY, @ERROR_STATE, @ERROR_MESSAGE)
	END CATCH

END
