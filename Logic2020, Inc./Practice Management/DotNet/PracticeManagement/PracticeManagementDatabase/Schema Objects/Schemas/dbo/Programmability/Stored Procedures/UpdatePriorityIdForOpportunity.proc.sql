CREATE PROCEDURE [dbo].[UpdatePriorityIdForOpportunity]
(
	@PriorityId			INT,
	@OpportunityId      INT,
	@UserLogin          NVARCHAR(255)
)
AS
BEGIN
	SET NOCOUNT ON;
	-- Start logging session
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	UPDATE Opportunity
	SET PriorityId = @PriorityId
	WHERE OpportunityId = @OpportunityId

		-- End logging session
		EXEC dbo.SessionLogUnprepare
END
