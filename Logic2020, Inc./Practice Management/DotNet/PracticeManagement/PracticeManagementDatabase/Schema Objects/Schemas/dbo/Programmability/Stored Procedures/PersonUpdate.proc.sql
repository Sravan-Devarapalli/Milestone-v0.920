﻿	-- =============================================
	-- Author:		Anatoliy Lokshin
	-- Create date: 8-04-2008
	-- Updated by:	sainathc
	-- Update date: 05-31-2012
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
				@W2SalaryId INT,
				@IsHireDateChanged BIT

		SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETDATE())))
		SELECT @W2SalaryId = TimescaleId FROM Timescale WHERE Name = 'W2-Salary'

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

				EXEC [dbo].[PersonTermination] @PersonId = @PersonId , @TerminationDate = @TerminationDate

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
				
				;WITH FirstPSHRow  AS 
				(
				 SELECT MIN(StartDate) AS StartDate,
						@PersonId AS PersonId				 
				 FROM dbo.PersonStatusHistory AS PSH 
				 WHERE  PSH.PersonId = @PersonId
				)

				UPDATE PSH 
				SET PSH.startdate = CONVERT(DATE,P.HireDate)
				FROM dbo.Person AS P 
				INNER JOIN FirstPSHRow AS FPSHR ON P.PersonId = FPSHR.PersonId
				INNER JOIN dbo.PersonStatusHistory AS PSH ON P.PersonId = FPSHR.PersonId   AND  FPSHR.startdate = PSH.startdate    
				WHERE  P.HireDate < PSH.startdate  

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
		
			DECLARE @PTOTimeTypeId INT,
					@HolidayTimeTypeId INT,
					@PTOChargeCodeId INT,
					@HolidayChargeCodeId INT

				
			SELECT @PTOTimeTypeId = dbo.GetPTOTimeTypeId(), @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId()
			SELECT @PTOChargeCodeId = Id FROM ChargeCode CC WHERE CC.TimeTypeId = @PTOTimeTypeId
			SELECT @HolidayChargeCodeId = Id FROM ChargeCode CC WHERE CC.TimeTypeId = @HolidayTimeTypeId
		
			IF @TerminationDate IS NOT NULL AND (@TerminationDate < @PreviousTerminationDate OR @PreviousTerminationDate IS NULL)
			BEGIN
				--Delete all PTO/Holiday timeEntries greater than @TerminationDate.
				DELETE TEH
				FROM TimeEntry TE
				JOIN ChargeCode CC ON TE.PersonId = @PersonId  AND CC.Id = TE.ChargeCodeId AND TE.ChargeCodeDate >= @TerminationDate
				JOIN TimeType TT ON TT.TimeTypeId = CC.TimeTypeId AND TT.IsAdministrative = 1
				JOIN TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
			
				DELETE TE
				FROM TimeEntry TE
				JOIN ChargeCode CC ON TE.PersonId = @PersonId  AND CC.Id = TE.ChargeCodeId AND TE.ChargeCodeDate >= @TerminationDate
				JOIN TimeType TT ON TT.TimeTypeId = CC.TimeTypeId AND TT.IsAdministrative = 1

			END
			ELSE IF @PreviousTerminationDate IS NOT NULL AND (@TerminationDate > @PreviousTerminationDate OR @TerminationDate IS NULL)
			BEGIN
				DECLARE @CurrentPMTime DATETIME,
						@PersonUserId INT
				SET @CurrentPMTime = dbo.InsertingTime()
				SELECT @PersonUserId = PersonId FROM Person WHERE Alias = @UserLogin
			
				INSERT  INTO [dbo].[TimeEntry]
								( [PersonId],
									[ChargeCodeId],
									[ChargeCodeDate],
									[Note],
									[ForecastedHours],
									[IsCorrect],
									[IsAutoGenerated]
								)
				SELECT @PersonId,
						--CASE WHEN PC.PersonId IS NOT NULL AND PC.DayOff = 1 AND ISNULL(PC.IsFloatingHoliday,0) = 0 THEN @PTOChargeCodeId ELSE @HolidayChargeCodeId END,
						CC.Id,
						C.Date,
						ISNULL(C.HolidayDescription, ISNULL(PC.Description, '')),
						--CASE WHEN PC.PersonId IS NOT NULL AND PC.DayOff = 1 AND ISNULL(PC.IsFloatingHoliday,0) = 0 THEN 'PTO'
						--	WHEN PC.PersonId IS NOT NULL AND PC.DayOff = 1 AND PC.IsFloatingHoliday = 1 THEN 'Floating Holiday'
						--	ELSE ISNULL(C.HolidayDescription,'') END,
						0,
						1,
						1
				FROM dbo.Calendar C
				JOIN dbo.Pay p ON C.Date BETWEEN p.StartDate and ( CASE WHEN @TerminationDate IS NOT NULL AND @TerminationDate < p.EndDate - 1 THEN @TerminationDate
																	ELSE p.EndDate- 1 END) AND p.Person = @PersonId AND p.Timescale = @W2SalaryId
				LEFT JOIN dbo.PersonCalendar PC ON PC.Date = C.Date AND PC.PersonId = P.Person AND PC.DayOff = 1
				INNER JOIN dbo.TimeType TT ON ( (C.DayOff = 1  AND DATEPART(DW,C.DATE) NOT IN (1,7) AND TT.TimeTypeId = @HolidayTimeTypeId) 
												OR (PC.PersonId IS NOT NULL AND PC.DayOff = 1 AND TT.TimeTypeId = PC.TimeTypeId) 
												)
				INNER JOIN dbo.ChargeCode CC ON CC.TimeTypeId = TT.TimeTypeId
				LEFT JOIN dbo.TimeEntry TE ON TE.PersonId = p.Person AND TE.ChargeCodeId = CC.Id AND TE.ChargeCodeDate = C.Date
				WHERE TE.TimeEntryId IS NULL

				INSERT INTO [dbo].[TimeEntryHours] 
										( [TimeEntryId],
											[ActualHours],
											[CreateDate],
											[ModifiedDate],
											[ModifiedBy],
											[IsChargeable],
											[ReviewStatusId]
										)
				SELECT TE.TimeEntryId,
						CASE WHEN TT.TimeTypeId = @HolidayTimeTypeId THEN 8 
							ELSE ISNULL(PC.ActualHours,8) END,
						@CurrentPMTime,
						@CurrentPMTime,
						@PersonUserId,
						0, --NON billable.
						CASE WHEN C.DayOff = 1 OR (PC.PersonId IS NOT NULL AND PC.DayOff = 1 AND PC.TimeTypeId <> @HolidayTimeTypeId AND PC.IsFromTimeEntry <> 1) THEN 2 
							ELSE 1 END
				FROM dbo.Calendar C
				JOIN dbo.Pay p ON C.Date BETWEEN p.StartDate and ( CASE WHEN @TerminationDate IS NOT NULL AND @TerminationDate < p.EndDate - 1 THEN @TerminationDate
																	ELSE p.EndDate- 1 END) AND p.Person = @PersonId AND p.Timescale = @W2SalaryId
				LEFT JOIN dbo.PersonCalendar PC ON PC.Date = C.Date AND PC.PersonId = P.Person AND PC.DayOff = 1
				INNER JOIN dbo.TimeType TT ON ( (C.DayOff = 1  AND DATEPART(DW,C.DATE) NOT IN (1,7) AND TT.TimeTypeId = @HolidayTimeTypeId) 
												OR (PC.PersonId IS NOT NULL AND PC.DayOff = 1 AND TT.TimeTypeId = PC.TimeTypeId) 
												)
				INNER JOIN dbo.ChargeCode CC ON CC.TimeTypeId = TT.TimeTypeId
				INNER JOIN dbo.TimeEntry TE ON TE.PersonId = p.Person AND TE.ChargeCodeId = CC.Id AND TE.ChargeCodeDate = C.Date
				LEFT JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
				WHERE TEH.Id IS NULL
			END

			-- End logging session
			EXEC dbo.SessionLogUnprepare
		END

