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

	BEGIN TRY
	BEGIN TRAN  MilestoneEntryDelete

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

	IF @MilestonePersonId IS NOT NULL
	BEGIN	
		EXEC [dbo].[InsertProjectFeedbackByMilestonePersonId] @MilestonePersonId=@MilestonePersonId,@MilestoneId = NULL
	END
	
	IF @PersonId IS NOT NULL
	BEGIN
		EXEC [dbo].[UpdateMSBadgeDetailsByPersonId] @PersonId = @PersonId, @UpdatedBy = @UpdatedBy
	END

		-- End logging session
	EXEC dbo.SessionLogUnprepare

	COMMIT TRAN MilestoneEntryDelete
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN MilestoneEntryDelete
		
		DECLARE @ErrorMessage NVARCHAR(MAX)
		SET @ErrorMessage = ERROR_MESSAGE()

		RAISERROR(@ErrorMessage, 16, 1)
	END CATCH
END

