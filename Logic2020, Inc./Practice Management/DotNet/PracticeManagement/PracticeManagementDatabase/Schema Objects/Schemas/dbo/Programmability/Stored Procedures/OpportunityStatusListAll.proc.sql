-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 11-04-2008
-- Updated by:	
-- Update date: 
-- Description:	Retrives a list of opportunity statuses.
-- =============================================
CREATE PROCEDURE [dbo].[OpportunityStatusListAll]
AS
	SET NOCOUNT ON

	SELECT s.OpportunityStatusId, s.Name
	  FROM dbo.OpportunityStatus AS s
	ORDER BY s.OpportunityStatusId

