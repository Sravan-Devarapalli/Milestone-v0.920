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
	@Amount        DECIMAL(18,2)
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	DECLARE @MilestoneId INT , @OldPersonId INT

	SELECT @MilestoneId = mp.MilestoneId,@OldPersonId = mp.PersonId  FROM dbo.MilestonePerson AS mp
	INNER JOIN dbo.MilestonePersonEntry AS mpe ON mpe.MilestonePersonId = mp.MilestonePersonId
	WHERE mp.MilestonePersonId = @MilestonePersonId AND mpe.Id = @Id


	UPDATE dbo.MilestonePersonEntry
	SET  StartDate = @StartDate, EndDate = @EndDate, PersonRoleId=@PersonRoleId, Amount=@Amount, HoursPerDay=@HoursPerDay
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

	-- End logging session
	EXEC dbo.SessionLogUnprepare

END
