-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-11-30
-- Description:	Remove existing time type
-- =============================================
CREATE PROCEDURE TimeTypeDelete
	@TimeTypeId INT
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
	BEGIN TRAN timeTypeDelete
		--Delete chargeCode records related to the @TimeTypeId
		IF EXISTS (SELECT TOP 1 1 FROM dbo.ChargeCode cc WHERE cc.TimeTypeId = @TimeTypeId)
		BEGIN

			DELETE cch
			FROM dbo.ChargeCodeTurnOffHistory cch INNER JOIN dbo.ChargeCode cc ON cch.ChargeCodeId = cc.Id WHERE cc.TimeTypeId = @TimeTypeId

			DELETE cc 
			FROM dbo.ChargeCode cc WHERE cc.TimeTypeId = @TimeTypeId

		END
			
		DELETE FROM ProjectTimeType WHERE TimeTypeId = @TimeTypeId

		DELETE FROM TimeType WHERE TimeTypeId = @TimeTypeId

	COMMIT TRAN timeTypeDelete
	END TRY
	BEGIN CATCH
	ROLLBACK TRAN timeTypeDelete
	
		DECLARE @ErrorMessage NVARCHAR(MAX)
		SELECT @ErrorMessage = ERROR_MESSAGE()
			
		RAISERROR(@ErrorMessage, 16, 1)
	END CATCH
END

