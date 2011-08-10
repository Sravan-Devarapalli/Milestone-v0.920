﻿-- =============================================
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
			@CurrentPayEndDate DATETIME,
			@CurrentSalesCommEndDate DATETIME

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
			
			--Delete all the Auto generated timeEntries later @TerminationDate
			DELETE te
			FROM TimeEntries te
			JOIN MilestonePerson MP ON MP.MilestonePersonId = te.MilestonePersonId
			WHERE MP.PersonId = @PersonId AND te.MilestoneDate > @TerminationDate AND te.IsAutoGenerated = 1			

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
				@ExistingPracticeId  INT,
				@PreviousTerminationDate DATETIME
				

		SELECT @ExistingSeniorityId = SeniorityId,
				@ExistingPracticeId = DefaultPractice,
				@PreviousTerminationDate = TerminationDate
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
			
			SELECT @CurrentSalesCommEndDate = EndDate
			FROM dbo.[DefaultCommission] P
			WHERE PersonId = @PersonId  
				 AND [Type] = 1 -- Sales Commission
				 AND @Today BETWEEN StartDate AND EndDate-1

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
		
		DECLARE @DefaultMilestoneId INT
		SELECT @DefaultMilestoneId = MilestoneId FROM DefaultMilestoneSetting
		
		IF @TerminationDate IS NOT NULL AND (@TerminationDate < @PreviousTerminationDate OR @PreviousTerminationDate IS NULL)
		BEGIN		
			
			DELETE te
			FROM TimeEntries te
			JOIN MilestonePerson mp ON mp.MilestonePersonId = te.MilestonePersonId
			WHERE te.MilestoneDate > @TerminationDate 
					AND mp.PersonId = @PersonId  
					AND mp.MilestoneId = @DefaultMilestoneId
					AND IsAutoGenerated = 1
		END
		ELSE IF @PreviousTerminationDate IS NOT NULL AND (@TerminationDate > @PreviousTerminationDate OR @TerminationDate IS NULL)
		BEGIN
			DECLARE @CurrentPMTime DATETIME,
					@HolidayTimeId INT,
					@PersonUserId INT
			SET @CurrentPMTime = dbo.InsertingTime()
			SET @HolidayTimeId = dbo.GetHolidayTimeTypeId()
			SELECT @PersonUserId = PersonId FROM Person WHERE Alias = @UserLogin
			
			IF NOT EXISTS (SELECT mp.MilestonePersonId
							FROM MilestonePerson mp
							WHERE mp.MilestoneId = @DefaultMilestoneId AND mp.PersonId = @PersonId)
			BEGIN				
				INSERT INTO MilestonePerson(MilestoneId, PersonId)
				VALUES (@DefaultMilestoneId, @PersonId)				
			END
			
			INSERT INTO MilestonePersonEntry(MilestonePersonId, StartDate, EndDate, Amount, HoursPerDay)
			SELECT mp.MilestonePersonId, m.StartDate, m.ProjectedDeliveryDate, 0, 8
			FROM MilestonePerson mp
			JOIN Milestone m ON m.MilestoneId = mp.MilestoneId
			LEFT JOIN MilestonePersonEntry mpe ON mpe.MilestonePersonId = mp.MilestonePersonId
			WHERE mp.MilestoneId = @DefaultMilestoneId AND mp.PersonId = @PersonId AND mpe.MilestonePersonId IS NULL
			
			INSERT  INTO [dbo].[TimeEntries]
		                ( [EntryDate] ,
		                  [MilestoneDate] ,
		                  [ModifiedDate] ,
		                  [MilestonePersonId] ,
		                  [ActualHours] ,
		                  [ForecastedHours] ,
		                  [TimeTypeId] ,
		                  [ModifiedBy] ,
		                  [Note] ,
		                  [IsChargeable] ,
		                  [IsCorrect],
						  [IsAutoGenerated]
		                )
			SELECT @CurrentPMTime AS EntryDate
				,crh.Date AS MilestoneDate
				,@CurrentPMTime AS ModifiedDate
				,mp.MilestonePersonId
				,8
				,mpe.HoursPerDay
				,@HolidayTimeId
				,@PersonUserId
				,crh.Description
				,m.IsChargeable
				,1
				,1 --Here it is Auto generated.
			FROM CompanyRecurringHolidaysByPeriod(@Today, dbo.GetFutureDate() -1) crh
			JOIN Pay p ON crh.Date BETWEEN p.StartDate and ( CASE WHEN @TerminationDate IS NOT NULL AND @TerminationDate < p.EndDate - 1 THEN @TerminationDate
																ELSE p.EndDate- 1 END) AND p.Person = @PersonId
			JOIN MilestonePerson mp ON mp.MilestoneId = @DefaultMilestoneId AND mp.PersonId = p.Person
			JOIN Milestone m ON mp.MilestoneId = m.MilestoneId
			JOIN MilestonePersonEntry mpe ON mpe.MilestonePersonId = mp.MilestonePersonId
			LEFT JOIN TimeEntries te ON te.MilestonePersonId = mpe.MilestonePersonId AND te.MilestoneDate = crh.Date
			WHERE crh.IsSet = 1 AND te.TimeEntryId IS NULL
		END

		-- End logging session
		EXEC dbo.SessionLogUnprepare
	END

