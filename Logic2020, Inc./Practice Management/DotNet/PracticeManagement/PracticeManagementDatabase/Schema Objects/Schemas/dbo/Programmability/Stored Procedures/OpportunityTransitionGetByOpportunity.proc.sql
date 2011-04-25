-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 11-04-2008
-- Updated by:	
-- Update date: 
-- Description:	Retrives a list of transitions for a specified opportunity.
-- =============================================
CREATE PROCEDURE dbo.OpportunityTransitionGetByOpportunity
(
	@OpportunityId   INT,
	@OpportunityTransitionStatusId INT = NULL
)
AS
	SET NOCOUNT ON

	SELECT t.OpportunityTransitionId,
	       t.OpportunityId,
	       t.OpportunityTransitionStatusId,
	       t.TransitionDate,
	       t.PersonId,
	       t.NoteText,
	       s.Name AS OpportunityTransitionStatusName,
	       p.FirstName,
	       p.LastName,
		   t.TargetPersonId as TargetPersonId,
	       target.FirstName as TargetFirstName,
	       target.LastName as TargetLastName
	  FROM dbo.OpportunityTransition AS t
	       INNER JOIN dbo.OpportunityTransitionStatus AS s
	           ON t.OpportunityTransitionStatusId = s.OpportunityTransitionStatusId
	       INNER JOIN dbo.Person AS p ON t.PersonId = p.PersonId
		   Left Join dbo.Person as target ON t.TargetPersonId = target.PersonId
	 WHERE t.OpportunityId = @OpportunityId
			AND (@OpportunityTransitionStatusId IS NULL OR @OpportunityTransitionStatusId = t.OpportunityTransitionStatusId)
	ORDER BY t.TransitionDate DESC

