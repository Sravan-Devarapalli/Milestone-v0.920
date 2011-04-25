-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-09-2008
-- Updated by:	
-- Update date: 
-- Description:	Selects a list of the seniorities
-- =============================================
CREATE PROCEDURE [dbo].[SeniorityListAll]
AS
	SET NOCOUNT ON

	SELECT e.SeniorityId, e.Name
	  FROM dbo.Seniority AS e
	ORDER BY e.SeniorityId

