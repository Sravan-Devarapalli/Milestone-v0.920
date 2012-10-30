CREATE PROCEDURE [dbo].[CapabilityInsert]
(
	@Name NVARCHAR(100),
	@PracticeId INT,
	@UserLogin NVARCHAR(MAX)
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN CapabilityInsert_Tran;

		DECLARE @Error NVARCHAR(MAX)
		IF EXISTS(SELECT 1 FROM dbo.[PracticeCapabilities] WHERE CapabilityName = @Name)
		BEGIN
			SET @Error = 'Capability name already exists. Please enter a different capability name.'
			RAISERROR(@Error,16,1)
		END

		EXEC SessionLogPrepare @UserLogin = @UserLogin

		SELECT @Name = REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(@Name)),' ','<>'),'><',''),'<>',' ')

		Insert [dbo].[PracticeCapabilities](CapabilityName,PracticeId)
		VALUES (@Name,@PracticeId)

		COMMIT TRAN CapabilityInsert_Tran;
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN CapabilityInsert_Tran;
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
