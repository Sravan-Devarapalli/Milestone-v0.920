CREATE PROCEDURE [dbo].[UpdateMilestonePersonEntry]
(
	@Id   INT,
	@UserLogin NVARCHAR(255),
	@PersonId            INT = NULL,
	@MilestonePersonId   INT OUT,
	@StartDate     DATETIME,
	@EndDate       DATETIME,
	@HoursPerDay   DECIMAL(4,2),
	@PersonRoleId  INT,
	@Amount        DECIMAL(18,2),
	@IsBadgeRequired	BIT,
	@BadgeStartDate		DATETIME,
	@BadgeEndDate		DATETIME,
	@IsBadgeException	BIT,
	@IsApproved			BIT,
	@RequestDate		DATETIME
)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
	BEGIN TRAN  Tran_UpdateMilestonePersonEntry

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	DECLARE @MilestoneId INT , @OldPersonId INT, @UpdatedBy INT, @CurrentPMTime DATETIME,@OldRequestDate DATETIME

	DECLARE @Today		DATETIME,
				@ProjectId	INT

	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))


	SELECT @UpdatedBy = PersonId FROM dbo.Person WHERE Alias = @UserLogin
	SELECT @CurrentPMTime = dbo.GettingPMTime(GETUTCDATE())

	SELECT @MilestoneId = mp.MilestoneId,@OldPersonId = mp.PersonId,@OldRequestDate = BadgeRequestDate FROM dbo.MilestonePerson AS mp
	INNER JOIN dbo.MilestonePersonEntry AS mpe ON mpe.MilestonePersonId = mp.MilestonePersonId
	WHERE mp.MilestonePersonId = @MilestonePersonId AND mpe.Id = @Id

	SELECT @ProjectId=ProjectId 
	FROM dbo.Milestone 
	WHERE MilestoneId = @MilestoneId

	SELECT @RequestDate = ISNULL(@RequestDate,@OldRequestDate)	

	UPDATE dbo.MilestonePersonEntry
	SET  StartDate = @StartDate, EndDate = @EndDate, PersonRoleId=@PersonRoleId, Amount=@Amount, HoursPerDay=@HoursPerDay, IsBadgeRequired = @IsBadgeRequired, BadgeStartDate = @BadgeStartDate, BadgeEndDate = @BadgeEndDate, IsBadgeException = @IsBadgeException, IsApproved = @IsApproved, BadgeRequestDate = @RequestDate, Requester = @UpdatedBy
	WHERE Id = @Id          
	     
	IF (@PersonId IS NOT NULL AND @OldPersonId != @PersonId)
	BEGIN 

		IF NOT EXISTS (SELECT 1 FROM dbo.MilestonePerson WHERE MilestoneId = @MilestoneId AND  PersonId = @PersonId )
		BEGIN

	
		    INSERT INTO [dbo].[MilestonePerson]([MilestoneId],[PersonId])
			VALUES(@MilestoneId,@PersonId)

			SET @MilestonePersonId = SCOPE_IDENTITY()

			UPDATE dbo.MilestonePersonEntry
			SET MilestonePersonId = @MilestonePersonId
			WHERE Id=@Id

		END
		ELSE
		BEGIN

			SELECT @MilestonePersonId = MilestonePersonId 
			FROM dbo.MilestonePerson WHERE MilestoneId = @MilestoneId AND  PersonId = @PersonId 


			UPDATE dbo.MilestonePersonEntry
			SET MilestonePersonId = @MilestonePersonId
			WHERE Id=@Id

		END

	END 

	IF @MilestoneId IS NOT NULL
	BEGIN
		EXEC dbo.InsertProjectFeedbackByMilestonePersonId @MilestonePersonId = NULL,@MilestoneId = @MilestoneId
	END

	EXEC UpdateMSBadgeDetailsByPersonId @PersonId = @PersonId,@UpdatedBy=@UpdatedBy

	IF @OldPersonId IS NOT NULL
		EXEC UpdateMSBadgeDetailsByPersonId @PersonId = @OldPersonId,@UpdatedBy = @UpdatedBy
	
	UPDATE dbo.Project
	SET CreatedDate = @Today
	WHERE ProjectId = @ProjectId

	-- End logging session
	EXEC dbo.SessionLogUnprepare

	COMMIT TRAN Tran_UpdateMilestonePersonEntry
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN Tran_UpdateMilestonePersonEntry
		DECLARE @ErrorMessage NVARCHAR(MAX)
		SET @ErrorMessage = ERROR_MESSAGE()
		RAISERROR(@ErrorMessage, 16, 1)
	END CATCH

END

