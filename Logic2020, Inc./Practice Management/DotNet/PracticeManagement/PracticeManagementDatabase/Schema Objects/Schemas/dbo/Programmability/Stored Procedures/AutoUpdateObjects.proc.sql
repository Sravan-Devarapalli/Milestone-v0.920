CREATE PROCEDURE dbo.AutoUpdateObjects
(
	@LastRun DATETIME,
	@NextRun DATETIME
)
AS
BEGIN
	 
	DECLARE	 @ERROR_MESSAGE		    nvarchar(2000)


	SELECT	 @ERROR_MESSAGE		    = NULL
	Declare @Error NVARCHAR(2000)
 

 --Main code:
	BEGIN TRY

		 BEGIN TRANSACTION AutoTerminatePersons_Tran

			EXEC dbo.AutoTerminatePersons
 

			EXECUTE [dbo].[SaveSchedularLog] 
			   @LastRun = @LastRun
			  ,@Status = 'Success'
			  ,@Comments ='Successfully comepleted running the procedure "dbo.AutoTerminatePersons"'
			  ,@NextRun = @NextRun

			COMMIT TRANSACTION AutoTerminatePersons_Tran

	END TRY
	BEGIN CATCH

		SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		ROLLBACK TRANSACTION  AutoTerminatePersons_Tran
		  SELECT @Error = 'Failed running the procedure "dbo.AutoTerminatePersons". The Error message is :'+@Error
		  EXECUTE [dbo].[SaveSchedularLog] 
			   @LastRun = @LastRun
			  ,@Status = 'Failed'
			  ,@Comments =@Error
			  ,@NextRun = @NextRun
 

	END CATCH

--Main code:
	BEGIN TRY

		 BEGIN TRANSACTION SetPersonSeniorityPractice_Tran

			EXEC dbo.UpdatePersonSeniorityPracticeFromCurrentPay
 

			EXECUTE dbo.[SaveSchedularLog]
			   @LastRun = @LastRun
			  ,@Status = 'Success'
			  ,@Comments ='Successfully comepleted running the procedure "dbo.UpdatePersonSeniorityPracticeFromCurrentPay"'
			  ,@NextRun = @NextRun

			COMMIT TRANSACTION SetPersonSeniorityPractice_Tran

	END TRY
	BEGIN CATCH

		SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		ROLLBACK TRANSACTION  SetPersonSeniorityPractice_Tran

		  SELECT @Error = 'Failed running the procedure "dbo.UpdatePersonSeniorityPracticeFromCurrentPay". The Error message is :'+@Error
		  EXECUTE [dbo].[SaveSchedularLog] 
			   @LastRun = @LastRun
			  ,@Status = 'Failed'
			  ,@Comments =@Error
			  ,@NextRun = @NextRun
 

	END CATCH


END
 
