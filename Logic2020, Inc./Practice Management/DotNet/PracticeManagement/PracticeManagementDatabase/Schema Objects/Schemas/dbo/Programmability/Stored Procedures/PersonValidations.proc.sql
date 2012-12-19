CREATE PROCEDURE [dbo].[PersonValidations]
(
	@FirstName       NVARCHAR(40),
	@LastName        NVARCHAR(40), 
	@Alias           NVARCHAR(100),
	@PersonId        INT = NULL,
	@EmployeeNumber NVARCHAR(12) = NULL
)
AS
BEGIN	
	BEGIN TRY
		DECLARE @ErrorMessage NVARCHAR(2048),
				@Today			DATETIME

		SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))

		IF @FirstName IS NOT NULL AND 
			@LastName IS NOT NULL AND EXISTS(SELECT 1
											FROM dbo.[Person] AS p
											WHERE p.[LastName] = @LastName AND p.[FirstName] = @FirstName AND (@PersonId IS NULL OR p.[PersonId] <> @PersonId))
		BEGIN
			-- Person First and Last Name uniqueness violation
			SELECT @ErrorMessage = [dbo].[GetErrorMessage](70001)
			RAISERROR (@ErrorMessage, 16, 1)
		END
		ELSE IF @Alias IS NOT NULL AND EXISTS(SELECT 1
							FROM dbo.[Person] AS p
						WHERE p.[Alias] = @Alias AND (@PersonId IS NULL OR p.[PersonId] <> @PersonId))
		BEGIN
			-- Person Email uniqueness violation
			SELECT @ErrorMessage = [dbo].[GetErrorMessage](70002)
			RAISERROR (@ErrorMessage, 16, 1)
		END
		ELSE IF @EmployeeNumber IS NOT NULL AND EXISTS(SELECT 1
														FROM dbo.[Person] AS p
													WHERE p.[EmployeeNumber] = @EmployeeNumber AND (@PersonId IS NULL OR p.[PersonId] <> @PersonId))
		BEGIN
			-- Person Employee Number uniqueness violation
			SELECT @ErrorMessage = [dbo].[GetErrorMessage](70009)
			RAISERROR (@ErrorMessage, 16, 1)
		END
	END TRY
	BEGIN CATCH 
		SELECT @ErrorMessage = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 16, 1)
	END CATCH
END
	
