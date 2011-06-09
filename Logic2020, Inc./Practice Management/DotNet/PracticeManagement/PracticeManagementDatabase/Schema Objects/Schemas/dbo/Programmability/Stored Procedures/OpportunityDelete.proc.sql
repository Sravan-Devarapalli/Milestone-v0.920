CREATE PROCEDURE [dbo].[OpportunityDelete]
(
	@OpportunityID INT,
	@UserLogin NVARCHAR
)
AS
BEGIN
	DECLARE @ErrorMessage NVARCHAR(MAX)
	
	IF NOT EXISTS (SELECT 1 FROM Opportunity WHERE OpportunityId = @OpportunityID AND OpportunityStatusId IN (3,5))
	BEGIN
		RAISERROR('To Delete an Opportunity, Opportunity must be Inactive/Experimental', 16,1)
	END
	ELSE
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION opportunityDelete
	
			-- Start logging session
			EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
			
			IF EXISTS (SELECT 1 FROM OpportunityPersons WHERE OpportunityId = @OpportunityID)
			BEGIN
				DELETE OpportunityPersons
				WHERE OpportunityId = @OpportunityID
			END
		
			IF EXISTS (SELECT 1 FROM OpportunityTransition WHERE OpportunityId = @OpportunityID)
			BEGIN
				EXEC OpportunityTransitionDelete @OpportunityID
			END
				
			IF EXISTS (SELECT 1 FROM Note WHERE NoteTargetId = 4 AND TargetId = @OpportunityID)
			BEGIN
				DELETE Note
				WHERE NoteTargetId = 4 AND TargetId = @OpportunityID-- Here 4 is Opportunity in NoteTarget table.
			END
		
			IF EXISTS (SELECT OpportunityId FROM Opportunity WHERE OpportunityId = @OpportunityID)
			BEGIN
				DELETE Opportunity
				WHERE OpportunityId = @OpportunityID
			END
		
			IF EXISTS (SELECT 1 FROM Project WHERE OpportunityId = @OpportunityID)
			BEGIN
				UPDATE Project
				SET OpportunityId = NULL
				WHERE OpportunityId = @OpportunityID
			END
		
			-- End logging session
			EXEC dbo.SessionLogUnprepare
			
			COMMIT TRAN opportunityDelete
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN opportunityDelete
			SELECT @ErrorMessage = ERROR_MESSAGE()
			
			RAISERROR(@ErrorMessage, 16, 1) --To Raise IF any errors in MilestoneDelete,
		END CATCH
	END
END
