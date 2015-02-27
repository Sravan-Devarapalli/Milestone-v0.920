CREATE PROCEDURE [dbo].[DeleteMilestonePersonEntry]
(
	@Id   INT,
	@UserLogin NVARCHAR(255)
)
AS
BEGIN

-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	SET NOCOUNT ON;

	DECLARE @MilestonePersonId   INT,
			@PersonId INT,
			@UpdatedBy INT

	SELECT @UpdatedBy = PersonId FROM dbo.Person WHERE Alias = @UserLogin

	SELECT @MilestonePersonId = MilestonePersonId
	FROM dbo.MilestonePersonEntry
	WHERE Id = @Id
	
	SELECT @PersonId= PersonId
	FROM dbo.MilestonePerson 
	WHERE MilestonePersonId = @MilestonePersonId

	DELETE 
	FROM dbo.MilestonePersonEntry
	WHERE Id = @Id

	EXEC [dbo].[InsertProjectFeedbackByMilestonePersonId] @MilestonePersonId=@MilestonePersonId,@MilestoneId = NULL
	EXEC [dbo].[UpdateMSBadgeDetailsByPersonId] @PersonId = @PersonId, @UpdatedBy = @UpdatedBy
	
	-- End logging session
	EXEC dbo.SessionLogUnprepare

END
