﻿	-- =============================================
	-- Author:		Anatoliy Lokshin
	-- Create date: 8-04-2008
	-- Last Updated by:	ThulasiRam.P
	-- Last Update date: 07-06-2012
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
		@TelephoneNumber	NVARCHAR(20) = NULL,
		@PaychexID		 NVARCHAR(20),
		@IsOffshore	     BIT,
		@PersonDivisionId	INT,
		@TerminationReasonId	INT
	)
	AS
		SET NOCOUNT ON
		DECLARE @ErrorMessage NVARCHAR(2048),
				@Today			DATETIME,
				@CurrentPayEndDate DATETIME,
				@CurrentSalesCommEndDate DATETIME,
				@IsHireDateChanged BIT

		SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))
		


		SELECT @PersonStatusId = CASE WHEN @TerminationDate < @Today THEN 2 
								      ELSE @PersonStatusId 
							     END

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
		
			IF @PersonStatusId = 2 OR @PersonStatusId = 5 OR ( @PersonStatusId = 3 AND @TerminationDate IS NOT NULL)
			BEGIN

				EXEC [dbo].[PersonTermination] @PersonId = @PersonId , @TerminationDate = @TerminationDate , @PersonStatusId = @PersonStatusId

			END

			IF @PersonStatusId <> 1
			BEGIN
				-- SET new manager for subordinates
				DECLARE @CurrentPractice INT
				DECLARE @NewManager INT

				SELECT @CurrentPractice = p.DefaultPractice
				FROM [dbo].Person AS P
				WHERE P.PersonId = @PersonId

				SELECT @NewManager = P.PersonId
				FROM [dbo].Person AS P
				INNER JOIN dbo.Seniority S ON S.SeniorityId = P.SeniorityId
				WHERE P.DefaultPractice = @CurrentPractice
				AND S.SeniorityValue <= 65 AND P.PersonId != @PersonId AND P.PersonStatusId = 1 -- ActivePerson

				IF (@NewManager IS NULL)
					SELECT @NewManager = P.PersonId
					FROM dbo.Person AS P
					WHERE P.IsDefaultManager = 1 AND P.PersonId != @PersonId AND P.PersonStatusId = 1 -- ActivePerson

				IF (@NewManager IS NULL)
					SELECT @NewManager = pr.PracticeManagerId
					FROM Practice AS pr
					WHERE pr.PracticeId = @currentPractice

				UPDATE dbo.Person
				SET ManagerId = @NewManager
				WHERE ManagerId = @PersonId
			END

			DECLARE @ExistingSeniorityId INT,
					@ExistingPracticeId  INT,
					@PreviousTerminationDate DATETIME
				

			SELECT @ExistingSeniorityId = SeniorityId,
					@ExistingPracticeId = DefaultPractice,
					@PreviousTerminationDate = TerminationDate
			FROM dbo.Person AS P
			WHERE P.PersonId = @PersonId

			IF(@ExistingSeniorityId <> @SeniorityId 
						OR (@ExistingSeniorityId IS NULL AND  @SeniorityId IS NOT NULL)
						OR (@ExistingSeniorityId IS NOT NULL AND @SeniorityId IS NULL)
				OR @ExistingPracticeId <> @DefaultPractice
						OR (@ExistingPracticeId IS NULL AND  @DefaultPractice IS NOT NULL)
						OR (@ExistingPracticeId IS NOT NULL AND @DefaultPractice IS NULL)
				)
			BEGIN


				SELECT @CurrentPayEndDate = pay.EndDate
				FROM dbo.Pay AS pay
				WHERE pay.Person = @PersonId  
						AND @Today BETWEEN pay.StartDate AND pay.EndDate-1
			
				SELECT @CurrentSalesCommEndDate = P.EndDate
				FROM dbo.[DefaultCommission] P
				WHERE P.PersonId = @PersonId  
					 AND P.[type] = 1 -- Sales Commission
					 AND @Today BETWEEN P.StartDate AND P.EndDate-1

				BEGIN TRY
					
					UPDATE dbo.Pay
					SET EndDate = @Today
					WHERE Person = @PersonId 
							AND @Today BETWEEN StartDate AND EndDate-1

					UPDATE dbo.[DefaultCommission]
					SET EndDate = @Today
					WHERE PersonId = @PersonId
						   AND [Type] = 1 -- Sales Commission
						   AND @Today BETWEEN StartDate AND EndDate-1
				
					INSERT INTO dbo.[DefaultCommission]
							(PersonId, StartDate, EndDate, FractionOfMargin, [type], MarginTypeId)
					SELECT [PersonId]
							,@Today
							,@CurrentSalesCommEndDate
							,[FractionOfMargin]
							,[type]
							,[MarginTypeId]
					FROM [dbo].[DefaultCommission]
					WHERE  PersonId = @PersonId 
								AND EndDate = @Today
								AND [Type] = 1

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
			
			EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

			SET @IsHireDateChanged =  0

			IF NOT EXISTS(SELECT 1 FROM dbo.Person AS P WHERE P.PersonId = @PersonId AND P.HireDate = @HireDate)
			BEGIN
			 SET @IsHireDateChanged =  1
			END

			UPDATE dbo.Person
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
				   TelephoneNumber = @TelephoneNumber,
				   PaychexID= @PaychexID,
				   IsOffshore = @IsOffshore,
				   DivisionId = @PersonDivisionId,
				   TerminationReasonId = @TerminationReasonId
			 WHERE PersonId = @PersonId

			 EXEC dbo.PersonStatusHistoryUpdate
					@PersonId = @PersonId,
					@PersonStatusId = @PersonStatusId

			IF(@IsHireDateChanged =  1)
			BEGIN
				EXEC dbo.OnPersonHireDateChange	@PersonId = @PersonId , @NewHireDate = @HireDate
			END


			IF(ISNULL(@Alias,'') <> '' AND NOT EXISTS (SELECT 1 FROM dbo.aspnet_Users WHERE UserName = @Alias))
			BEGIN

				DECLARE @UserId UNIQUEIDENTIFIER,
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
		
			EXEC [dbo].[AdjustTimeEntriesForTerminationDateChanged] @PersonId = @PersonId, @TerminationDate = @TerminationDate, @PreviousTerminationDate = @PreviousTerminationDate,@UserLogin = @UserLogin		
		
			-- End logging session
			EXEC dbo.SessionLogUnprepare
		END

