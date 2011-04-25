-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 11-04-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 11-11-2008
-- Description:	Insert a new opportunity transition.
-- =============================================
CREATE PROCEDURE dbo.OpportunityTransitionInsert
(
	@OpportunityId                   INT,
	@OpportunityTransitionStatusId   INT,
	@PersonId                        INT,
	@NoteText                        NVARCHAR(2000),
	@TargetPersonId					 INT = NULL,
	@OpportunityTransitionId		 INT = NULL OUTPUT
)
AS
	SET NOCOUNT ON

	INSERT INTO dbo.OpportunityTransition
	            (OpportunityId, OpportunityTransitionStatusId, PersonId, NoteText, TargetPersonId)
	     VALUES (@OpportunityId, @OpportunityTransitionStatusId, @PersonId, @NoteText, @TargetPersonId)

	IF @OpportunityTransitionStatusId = 7 -- Lost
	BEGIN
		-- Change the opportunity status
		UPDATE dbo.Opportunity
		   SET OpportunityStatusId = 2
		 WHERE OpportunityId = @OpportunityId
	END

	SET @OpportunityTransitionId = SCOPE_IDENTITY()


