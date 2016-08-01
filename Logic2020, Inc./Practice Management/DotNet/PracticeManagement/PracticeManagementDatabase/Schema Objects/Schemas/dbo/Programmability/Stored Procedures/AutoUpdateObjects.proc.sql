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

			EXEC dbo.UpdatePersonTitleAndPracticeFromCurrentPay
 

			EXECUTE dbo.[SaveSchedularLog]
			   @LastRun = @LastRun
			  ,@Status = 'Success'
			  ,@Comments ='Successfully comepleted running the procedure "dbo.UpdatePersonTitleAndPracticeFromCurrentPay"'
			  ,@NextRun = @NextRun

			COMMIT TRANSACTION SetPersonSeniorityPractice_Tran

	END TRY
	BEGIN CATCH

		SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		ROLLBACK TRANSACTION  SetPersonSeniorityPractice_Tran

		  SELECT @Error = 'Failed running the procedure "dbo.UpdatePersonTitleAndPracticeFromCurrentPay". The Error message is :'+@Error
		  EXECUTE [dbo].[SaveSchedularLog] 
			   @LastRun = @LastRun
			  ,@Status = 'Failed'
			  ,@Comments =@Error
			  ,@NextRun = @NextRun
 

	END CATCH

--update MS badge dates with deactivation dates
	BEGIN TRY

	BEGIN TRANSACTION UpdateMSBadgeDeactivation_Tran

		DECLARE @temp TABLE(ID INT not null identity(1,1), personID INT)

		DECLARE @Today DATETIME
		SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))

		INSERT INTO @temp 
		SELECT M.PersonId
		FROM MSBadge M
		WHERE M.OrganicBreakEndDate <= (@Today-1)

		DECLARE @Id int = (select min(Id) from @temp)

		WHILE (@Id IS NOT NULL)
		BEGIN
		DECLARE @PersonId INT = (select personid from @temp where ID=@Id)
			EXEC [dbo].[UpdateMSBadgeDetailsByPersonId] @PersonId = @PersonId, @UpdatedBy = NULL

			set @Id = (select Id from @temp where Id = @Id + 1)
		END

		EXECUTE dbo.[SaveSchedularLog]
			   @LastRun = @LastRun
			  ,@Status = 'Success'
			  ,@Comments ='Successfully comepleted running the procedure "dbo.UpdateMSBadgeDetailsByPersonId'
			  ,@NextRun = @NextRun

	COMMIT TRANSACTION UpdateMSBadgeDeactivation_Tran

	END TRY
	BEGIN CATCH
		
		SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		ROLLBACK TRANSACTION  UpdateMSBadgeDeactivation_Tran
		  SELECT @Error = 'Failed running the procedure "dbo.UpdateMSBadgeDetailsByPersonId". The Error message is :'+@Error
		  EXECUTE [dbo].[SaveSchedularLog] 
			   @LastRun = @LastRun
			  ,@Status = 'Failed'
			  ,@Comments =@Error
			  ,@NextRun = @NextRun
	END CATCH

	--Clear expiried user report filter values
	BEGIN TRY

	BEGIN TRANSACTION ClearExpiredUserFiltersData_Tran

	DECLARE @Now DATETIME =CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE()))

	DELETE FROM dbo.ReportFilterValues
	WHERE ExpiresOn IS NULL OR @Now > CONVERT(DATE,ExpiresOn)

	EXECUTE dbo.[SaveSchedularLog]
			   @LastRun = @LastRun
			  ,@Status = 'Success'
			  ,@Comments ='Successfully cleaned the expired Report Filter Values'
			  ,@NextRun = @NextRun

	COMMIT TRANSACTION ClearExpiredUserFiltersData_Tran

	END TRY
	BEGIN CATCH

	SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		ROLLBACK TRANSACTION  ClearExpiredUserFiltersData_Tran
		  SELECT @Error = 'Failed to clean report filter values. The Error message is :'+@Error
		  EXECUTE [dbo].[SaveSchedularLog] 
			   @LastRun = @LastRun
			  ,@Status = 'Failed'
			  ,@Comments =@Error
			  ,@NextRun = @NextRun

	END CATCH

END
 

