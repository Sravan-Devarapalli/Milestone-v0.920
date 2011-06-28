-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 8-04-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 9-09-2008
-- Description:	Updates the Person.
-- =============================================
CREATE PROCEDURE [dbo].[PersonUpdate]
(
	@PersonId        INT,
	@FirstName       NVARCHAR(40),
	@LastName        NVARCHAR(40),
	@PTODaysPerAnnum INT,
	@HireDate        DATETIME,
	@TerminationDate DATETIME,
	@Alias           NVARCHAR(100),
	@DefaultPractice INT,
	@PersonStatusId	 INT,
	@EmployeeNumber	 NVARCHAR(12),
	@SeniorityId     INT,
	@UserLogin       NVARCHAR(255),
	@ManagerId		 INT = NULL,
	@PracticeOwnedId	INT = NULL, 
	@TelephoneNumber	NVARCHAR(20) = NULL 
)
AS
	SET NOCOUNT ON
	DECLARE @ErrorMessage NVARCHAR(2048),
			@Today			DATETIME,
			@CurrentPayEndDate DATETIME

	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETDATE())))

	SELECT @PersonStatusId = CASE WHEN @TerminationDate<= @Today THEN 2 ELSE @PersonStatusId END

	IF EXISTS(SELECT 1
	            FROM dbo.[Person] AS p
	           WHERE p.[LastName] = @LastName AND p.[FirstName] = @FirstName AND p.[PersonId] <> @PersonId)
	BEGIN
		-- Person First and Last Name uniqueness violation
		SELECT @ErrorMessage = [dbo].[GetErrorMessage](70001)
		RAISERROR (@ErrorMessage, 16, 1)
	END
	ELSE IF EXISTS(SELECT 1
	                 FROM dbo.[Person] AS p
	                WHERE p.[Alias] = @Alias AND p.[PersonId] <> @PersonId)
	BEGIN
		-- Person Email uniqueness violation
		SELECT @ErrorMessage = [dbo].[GetErrorMessage](70002)
		RAISERROR (@ErrorMessage, 16, 1)
	END
	ELSE IF EXISTS(SELECT 1
	                 FROM dbo.[Person] AS p
	                WHERE p.[EmployeeNumber] = @EmployeeNumber AND p.[PersonId] <> @PersonId)
	BEGIN
		-- Person Employee Number uniqueness violation
		SELECT @ErrorMessage = [dbo].[GetErrorMessage](70009)
		RAISERROR (@ErrorMessage, 16, 1)
	END
	ELSE
	BEGIN
		-- Start logging session
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
		
		-- Try to get default manager id
		--SELECT @ManagerId = p.PersonId FROM person AS p WHERE p.IsDefaultManager = 1

		IF @PersonStatusId = 2
		BEGIN
			SELECT @CurrentPayEndDate = EndDate
			FROM dbo.Pay
			WHERE Person = @PersonId AND EndDate >= @TerminationDate
					AND StartDate < @TerminationDate
			-- Close a current compensation for the terminated persons
			UPDATE dbo.Pay
			   SET EndDate = @TerminationDate
			 WHERE Person = @PersonId AND EndDate > @TerminationDate
					AND StartDate < @TerminationDate

			--Delete all the Compensation records later @TerminationDate
			DELETE FROM dbo.Pay
			WHERE Person = @PersonId AND StartDate >= @TerminationDate
			 

		END

		IF @PersonStatusId <> 1
		BEGIN
			-- SET new manager for subordinates
			DECLARE @currentPractice INT
			DECLARE @newManager INT

			SELECT @currentPractice = DefaultPractice
			FROM Person 
			WHERE PersonId = @PersonId

			SELECT @newManager = PersonId
			FROM Person P
			JOIN dbo.Seniority S ON S.SeniorityId = P.SeniorityId
			where DefaultPractice = @currentPractice
			AND S.SeniorityValue <= 65 AND PersonId != @PersonId AND PersonStatusId = 1 -- ActivePerson

			IF (@newManager IS NULL)
				SELECT @newManager = PersonId
				FROM Person 
				WHERE IsDefaultManager = 1 AND PersonId != @PersonId AND PersonStatusId = 1 -- ActivePerson

			IF (@newManager IS NULL)
				SELECT @newManager = PracticeManagerId
				FROM Practice 
				WHERE PracticeId = @currentPractice

			UPDATE dbo.Person
			SET ManagerId = @newManager
			WHERE ManagerId = @PersonId
		END

		DECLARE @ExistingSeniorityId INT,
				@ExistingPracticeId  INT

		SELECT @ExistingSeniorityId = SeniorityId,
				@ExistingPracticeId = DefaultPractice
		FROM dbo.Person P
		WHERE PersonId = @PersonId
		IF(@ExistingSeniorityId <> @SeniorityId 
					OR (@ExistingSeniorityId IS NULL AND  @SeniorityId IS NOT NULL)
					OR (@ExistingSeniorityId IS NOT NULL AND @SeniorityId IS NULL)
			OR @ExistingPracticeId <> @DefaultPractice
					OR (@ExistingPracticeId IS NULL AND  @DefaultPractice IS NOT NULL)
					OR (@ExistingPracticeId IS NOT NULL AND @DefaultPractice IS NULL)
			)
		BEGIN


			SELECT @CurrentPayEndDate = EndDate
			FROM dbo.Pay P
			WHERE Person = @PersonId  
					AND @Today BETWEEN StartDate AND EndDate-1
			BEGIN TRY
					
				UPDATE dbo.Pay
				SET EndDate = @Today
				WHERE Person = @PersonId 
						AND @Today BETWEEN StartDate AND EndDate-1

				INSERT INTO [dbo].[Pay]
							   ([Person]
							   ,[StartDate]
							   ,[EndDate]
							   ,[Amount]
							   ,[Timescale]
							   ,[TimesPaidPerMonth]
							   ,[Terms]
							   ,[VacationDays]
							   ,[BonusAmount]
							   ,[BonusHoursToCollect]
							   ,[DefaultHoursPerDay]
							   ,[SeniorityId]
							   ,[PracticeId])
				SELECT 
						Person
						,@Today
						,@CurrentPayEndDate
						,[Amount]
						,[Timescale]
						,[TimesPaidPerMonth]
						,[Terms]
						,[VacationDays]
						,[BonusAmount]
						,[BonusHoursToCollect]
						,[DefaultHoursPerDay]
						,@SeniorityId
						,@DefaultPractice
				FROM dbo.Pay
				WHERE Person = @PersonId 
							AND EndDate = @Today
			END TRY
			BEGIN CATCH
				
				UPDATE dbo.Pay
				SET SeniorityId = @SeniorityId,
					PracticeId = @DefaultPractice
				WHERE Person = @PersonId 
						AND @Today BETWEEN StartDate AND EndDate-1

			END CATCH
		END

		UPDATE Person
		   SET FirstName = @FirstName,
			   LastName = @LastName,
			   PTODaysPerAnnum = ISNULL(@PTODaysPerAnnum, PTODaysPerAnnum),
			   HireDate = @HireDate, 
			   TerminationDate = @TerminationDate,
			   Alias = @Alias,
			   DefaultPractice = @DefaultPractice,
			   PersonStatusId = @PersonStatusId,
			   EmployeeNumber = @EmployeeNumber,
		       SeniorityId = @SeniorityId,
		       ManagerId = @ManagerId,
		       PracticeOwnedId = @PracticeOwnedId,
		       TelephoneNumber = @TelephoneNumber
		 WHERE PersonId = @PersonId

		 EXEC dbo.PersonStatusHistoryUpdate
				@PersonId = @PersonId,
				@PersonStatusId = @PersonStatusId

		IF(ISNULL(@Alias,'') <> '' AND NOT EXISTS (SELECT 1 FROM dbo.aspnet_Users WHERE UserName = @Alias))
		BEGIN

			DECLARE @UserId uniqueidentifier,
					@UTCNow DATETIME
			SELECT @UTCNow = GETUTCDATE()
			
			
			EXECUTE dbo.[aspnet_Membership_CreateUser]
			 @ApplicationName  = 'PracticeManagement',
			 @UserName         = @Alias,
			 @Password         = '1ry+v0QiDZcuh8TaPGoiHxPps3E=',
			 @PasswordSalt     = '8LOUC9LPUunGvEIDlIJnfQ==',
			 @Email            = @Alias,
			 @PasswordQuestion = NULL,
			 @PasswordAnswer   = NULL,
			 @IsApproved       = 1,
			 @CurrentTimeUtc   = @UTCNow,
			 @CreateDate       = @UTCNow,
			 @UniqueEmail      = 1,
			 @PasswordFormat   = 1,
			 @UserId           = @UserId OUTPUT
		END

		-- End logging session
		EXEC dbo.SessionLogUnprepare
	END

