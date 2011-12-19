CREATE PROCEDURE [dbo].[AttachProjectToOpportunity]
(
	@ProjectId			INT,
	@PriorityId			INT,
	@OpportunityId      INT,
	@UserLogin          NVARCHAR(255),
	@IsOpportunityDescriptionSelected BIT
)
AS
BEGIN
	SET NOCOUNT ON;
	-- Start logging session
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
		DECLARE @ErrorMessage NVARCHAR(MAX)
	
		BEGIN TRY
		    
			 DECLARE @Description NVARCHAR(MAX)

			UPDATE dbo.Opportunity
			SET   ProjectId = @ProjectId,
				  PriorityId = @PriorityId,
				  LastUpdated = GETUTCDATE()
			WHERE OpportunityId = @OpportunityId


			IF(@IsOpportunityDescriptionSelected = 1)
			BEGIN
				
				SELECT @Description = op.Description 
				FROM dbo.Opportunity op 
				WHERE op.OpportunityId = @OpportunityId 

				UPDATE dbo.Project 
				SET Description = @Description
				where ProjectId = @ProjectId

			END
			ELSE
			BEGIN
			   
				SELECT @Description = p.Description 
				FROM dbo.Project p 
				WHERE p.ProjectId = @ProjectId 

				UPDATE dbo.Opportunity 
				SET Description = @Description
				where OpportunityId = @OpportunityId
			 
			END


		END TRY
		BEGIN CATCH
			
			SELECT @ErrorMessage = ERROR_MESSAGE()
			RAISERROR(@ErrorMessage, 16, 1) 

		END CATCH

		-- End logging session
		EXEC dbo.SessionLogUnprepare
END
