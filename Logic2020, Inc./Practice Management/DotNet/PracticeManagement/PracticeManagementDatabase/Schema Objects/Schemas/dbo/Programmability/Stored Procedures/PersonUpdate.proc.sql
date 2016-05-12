-- =============================================
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
	@PreferredFirstName        NVARCHAR(40), 
	@HireDate        DATETIME,
	@TerminationDate DATETIME,
	@Alias           NVARCHAR(100),
	@DefaultPractice INT,
	@PersonStatusId	 INT,
	@EmployeeNumber	 NVARCHAR(12),
	@SeniorityId     INT,
	@RecruiterId	 INT,
	@TitleId	     INT,
	@UserLogin       NVARCHAR(255),
	@ManagerId		 INT = NULL,
	@PracticeOwnedId	INT = NULL, 
	@TelephoneNumber	NVARCHAR(20) = NULL,
	@PaychexID		 NVARCHAR(20),
	@IsOffshore	     BIT,
	@PersonDivisionId	INT,
	@TerminationReasonId	INT,
	@SLTApproval		   BIT,
	@SLTPTOApproval			BIT,
	@JobSeekerStatusId INT,
	@SourceRecruitingMetricsId	INT,
	@TargetRecruitingMetricsId	INT,
	@EmployeeReferralId	INT,
	@CohortAssignmentId INT,
	@LocationId			INT,
	@IsMBO				BIT,
	@IsInvestmentResource BIT,
	@PracticeLeadershipId	INT,
	@TargetUtilization INT=NULL
)
AS
SET NOCOUNT ON
SET ARITHABORT ON 
BEGIN TRY
	DECLARE @ErrorMessage NVARCHAR(2048),
			@Today			DATETIME,
			@CurrentPayEndDate DATETIME,
			@PreviousPersonStatusId	INT

	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))
	SELECT @PersonStatusId = CASE WHEN @TerminationDate < @Today THEN 2 ELSE @PersonStatusId END
	SELECT @PreviousPersonStatusId = PersonStatusId FROM dbo.Person WHERE PersonId = @PersonId

	EXEC [dbo].[PersonValidations] @FirstName = @FirstName, @LastName = @LastName, @Alias = @Alias,@PersonId = @PersonId,@EmployeeNumber = @EmployeeNumber

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
		
	IF @PersonStatusId = 2 OR @PersonStatusId = 5 OR ( @PersonStatusId = 3 AND @TerminationDate IS NOT NULL)
	BEGIN
		EXEC [dbo].[PersonTermination] @PersonId = @PersonId , @TerminationDate = @TerminationDate , @PersonStatusId = @PersonStatusId ,@UserLogin = @UserLogin
	END

	-- update with DivisionOwner

	IF @PersonStatusId <> 1
	BEGIN
		-- SET new manager for subordinates as per 3212
		--DECLARE @NewManager INT

		--SELECT @NewManager = PD.DivisionOwnerId
		--FROM dbo.PersonDivision PD 
		--WHERE PD.DivisionId=@PersonDivisionId
		
		
		UPDATE p
		SET p.ManagerId = PD.DivisionOwnerId
		FROM dbo.Person p
		INNER JOIN dbo.PersonDivision PD ON PD.DivisionId=p.DivisionId
		WHERE p.ManagerId = @PersonId
	END

	DECLARE @ExistingTitleId INT,
			@ExistingPracticeId  INT,
			@ExistingDivisionId INT,
			@PreviousTerminationDate DATETIME,
			@PreviousHireDate DATETIME
				

	SELECT @ExistingTitleId = TitleId,
			@ExistingPracticeId = DefaultPractice,
			@ExistingDivisionId=DivisionId,
			@PreviousTerminationDate = TerminationDate,
			@PreviousHireDate = HireDate
	FROM dbo.Person AS P
	WHERE P.PersonId = @PersonId

	IF(ISNULL(@ExistingTitleId,0) <> ISNULL(@TitleId,0) OR ISNULL(@ExistingPracticeId,0) <> ISNULL(@DefaultPractice,0) OR ISNULL(@ExistingDivisionId,0) <> ISNULL(@PersonDivisionId,0))
	BEGIN
		SELECT @CurrentPayEndDate = pay.EndDate FROM dbo.Pay AS pay WHERE pay.Person = @PersonId AND @Today BETWEEN pay.StartDate AND pay.EndDate-1
			
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
							,[VacationDays]
							,[BonusAmount]
							,[BonusHoursToCollect]
							,TitleId
							,[PracticeId]
							,SLTApproval
							,SLTPTOApproval,
							DivisionId)
			SELECT 
					Person
					,@Today
					,@CurrentPayEndDate
					,[Amount]
					,[Timescale]
					,[VacationDays]
					,[BonusAmount]
					,[BonusHoursToCollect]
					,@TitleId
					,@DefaultPractice
					,@SLTApproval
					,@SLTPTOApproval
					,@PersonDivisionId
			FROM dbo.Pay
			WHERE Person = @PersonId 
						AND EndDate = @Today
		END TRY
		BEGIN CATCH
				
			UPDATE dbo.Pay
			SET TitleId = @TitleId,
				PracticeId = @DefaultPractice,
				DivisionId=@PersonDivisionId,
				SLTApproval = @SLTApproval,
				SLTPTOApproval = @SLTPTOApproval
			WHERE Person = @PersonId 
					AND @Today BETWEEN StartDate AND EndDate-1

		END CATCH
	END
			

	IF(@PreviousTerminationDate >= @Today AND (@PersonStatusId = 1 OR (@PersonStatusId = 3 AND @TerminationDate IS NULL)))
	BEGIN
			UPDATE dbo.Pay
			SET EndDate = dbo.GetFutureDate()
			WHERE Person = @PersonId 		
			AND StartDate = (SELECT TOP(1) StartDate FROM dbo.Pay WHERE Person = @PersonId AND  StartDate >= @HireDate ORDER BY StartDate DESC)
	END

	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
			
	UPDATE dbo.Person
		SET FirstName = @FirstName,
			LastName = @LastName,
			PreferredFirstName = @PreferredFirstName,
			HireDate = @HireDate, 
			TerminationDate = @TerminationDate,
			Alias = @Alias,
			DefaultPractice = @DefaultPractice,
			PersonStatusId = @PersonStatusId,
			EmployeeNumber = @EmployeeNumber,
			SeniorityId = @SeniorityId,
			TitleId = @TitleID,
			RecruiterId = @RecruiterId,
			ManagerId = @ManagerId,
			PracticeOwnedId = @PracticeOwnedId,
			TelephoneNumber = @TelephoneNumber,
			PaychexID= @PaychexID,
			IsOffshore = @IsOffshore,
			DivisionId = @PersonDivisionId,
			TerminationReasonId = @TerminationReasonId,
                        IsWelcomeEmailSent = CASE WHEN @PersonStatusId = 2  OR (@PreviousHireDate <> @HireDate AND @HireDate >= @Today) THEN 0
										     ELSE IsWelcomeEmailSent END,
			JobSeekerStatusId = @JobSeekerStatusId,
			SourceId = @SourceRecruitingMetricsId,
			TargetedCompanyId = @TargetRecruitingMetricsId,
			EmployeeReferralId = @EmployeeReferralId,
			CohortAssignmentId = @CohortAssignmentId,
			LocationId	= @LocationId,
			IsMBO = @IsMBO,
			IsInvestmentResource=@IsInvestmentResource,
			PracticeLeadershipId = @PracticeLeadershipId,
			TargetUtilization=@TargetUtilization
		WHERE PersonId = @PersonId

		EXEC dbo.PersonStatusHistoryUpdate
			@PersonId = @PersonId,
			@PersonStatusId = @PersonStatusId

	--Hire Date Changed.
	IF(@PreviousHireDate <> @HireDate)
	BEGIN
		DECLARE @ModifiedBy INT
		SELECT @ModifiedBy = PersonId
		FROM dbo.Person p
		WHERE P.Alias = @UserLogin
 
		EXEC dbo.OnPersonHireDateChange	@PersonId = @PersonId , @NewHireDate = @HireDate, @ModifiedBy = @ModifiedBy
	END

	--STOP FEEDBACK TRIGGERING LOGIC AS PER NICK STATEMENT
	--IF(@PersonStatusId <> 2 =(1,3) AND (@PreviousPersonStatusId <> @PersonStatusId))
	--BEGIN
	--     DELETE PF
	--	 FROM dbo.ProjectFeedback PF
	--	 WHERE PF.PersonId = @PersonId AND 
	--			PF.IsCanceled = 1
	--END

	--to update milestoneperson entries as per #3184
	UPDATE P
	SET CreatedDate = @Today
	FROM dbo.MilestonePersonEntry AS MPE 
	INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	INNER JOIN v_PersonHistory AS PH ON PH.PersonId = MP.PersonId AND (PH.TerminationDate IS NULL OR MPE.StartDate <= PH.TerminationDate) AND PH.HireDate <= MPE.EndDate
	INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
	WHERE MP.PersonId = @PersonId AND
	(
		(
		MPE.StartDate <> CASE WHEN MPE.StartDate > PH.HireDate THEN MPE.StartDate
						ELSE PH.HireDate END
		)
		OR
		(
		MPE.EndDate <> CASE WHEN (MPE.EndDate < PH.TerminationDate) OR PH.TerminationDate IS NULL THEN MPE.EndDate
						ELSE PH.TerminationDate END
		)
	)
	---

	UPDATE	MPE
	SET MPE.StartDate = CASE WHEN MPE.StartDate > PH.HireDate THEN MPE.StartDate
						ELSE PH.HireDate END,
		MPE.EndDate = CASE WHEN (MPE.EndDate < PH.TerminationDate) OR PH.TerminationDate IS NULL THEN MPE.EndDate
						ELSE PH.TerminationDate END
	FROM dbo.MilestonePersonEntry AS MPE 
	INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	INNER JOIN v_PersonHistory AS PH ON PH.PersonId = MP.PersonId AND (PH.TerminationDate IS NULL OR MPE.StartDate <= PH.TerminationDate) AND PH.HireDate <= MPE.EndDate
	WHERE MP.PersonId = @PersonId AND
	(
		(
		MPE.StartDate <> CASE WHEN MPE.StartDate > PH.HireDate THEN MPE.StartDate
						ELSE PH.HireDate END
		)
		OR
		(
		MPE.EndDate <> CASE WHEN (MPE.EndDate < PH.TerminationDate) OR PH.TerminationDate IS NULL THEN MPE.EndDate
						ELSE PH.TerminationDate END
		)
	)
	---to delete milestoneperson entries as per #3184
	UPDATE P
	SET CreatedDate = @Today
	FROM dbo.MilestonePersonEntry AS MPE 
	INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
	LEFT JOIN v_PersonHistory AS PH ON PH.PersonId = MP.PersonId AND (PH.TerminationDate IS NULL OR MPE.StartDate <= PH.TerminationDate) AND PH.HireDate <= MPE.EndDate
	WHERE MP.PersonId = @PersonId AND PH.PersonId IS NULL

	-----------------
	DELETE MPE
	FROM dbo.MilestonePersonEntry AS MPE 
	INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
	INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
	LEFT JOIN v_PersonHistory AS PH ON PH.PersonId = MP.PersonId AND (PH.TerminationDate IS NULL OR MPE.StartDate <= PH.TerminationDate) AND PH.HireDate <= MPE.EndDate
	WHERE MP.PersonId = @PersonId AND PH.PersonId IS NULL

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
	EXEC [dbo].[SetCommissionsAttributions] @PersonId = @PersonId

	IF((ISNULL(@ExistingTitleId,0) <> ISNULL(@TitleId,0)) OR (@PreviousHireDate <> @HireDate))
	BEGIN
	   IF @PersonId IS NOT NULL
	   BEGIN
			EXEC [dbo].[InsertProjectFeedbackByMilestonePersonId] @MilestonePersonId=NULL,@MilestoneId = NULL,@ProjectId = NULL, @PersonId = @PersonId
	   END
	END
	-- End logging session
	EXEC dbo.SessionLogUnprepare
END TRY
BEGIN CATCH 
	SELECT @ErrorMessage = ERROR_MESSAGE()
	RAISERROR (@ErrorMessage, 16, 1)
END CATCH

