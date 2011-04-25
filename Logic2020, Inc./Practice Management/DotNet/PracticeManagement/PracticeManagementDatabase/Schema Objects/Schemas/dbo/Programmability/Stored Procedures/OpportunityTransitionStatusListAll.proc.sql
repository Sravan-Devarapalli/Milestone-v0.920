-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 11-04-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 12-03-2008
-- Description:	Retrives a list of opportunity transition statuses.
-- =============================================
CREATE PROCEDURE [dbo].[OpportunityTransitionStatusListAll]
AS
	SET NOCOUNT ON

	SELECT s.OpportunityTransitionStatusId, s.Name
	  FROM dbo.OpportunityTransitionStatus AS s
	 WHERE OpportunityTransitionStatusId NOT IN (1, 3, 4, 5, 6)
	ORDER BY s.OpportunityTransitionStatusId

