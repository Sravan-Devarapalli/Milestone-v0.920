CREATE PROCEDURE [dbo].[AttachProjectToOpportunity]
(
	@ProjectId			INT,
	@PriorityId			INT,
	@OpportunityId      INT,
	@UserLogin          NVARCHAR(255)
)
AS
BEGIN
	SET NOCOUNT ON;
	-- Start logging session
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
		DECLARE @ErrorMessage NVARCHAR(MAX)
	
		BEGIN TRY
			UPDATE dbo.Opportunity
			SET   ProjectId = @ProjectId,
				  PriorityId = @PriorityId,
				  LastUpdated = GETUTCDATE()
			WHERE OpportunityId = @OpportunityId
		END TRY
		BEGIN CATCH
			
			SELECT @ErrorMessage = ERROR_MESSAGE()
			RAISERROR(@ErrorMessage, 16, 1) 

		END CATCH

		-- End logging session
		EXEC dbo.SessionLogUnprepare
END
