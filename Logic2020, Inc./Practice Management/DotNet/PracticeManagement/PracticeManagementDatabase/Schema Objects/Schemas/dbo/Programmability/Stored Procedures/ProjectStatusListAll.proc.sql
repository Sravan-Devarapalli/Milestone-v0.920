--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-5-2008
-- Description:	List project statuses
-- =============================================
CREATE PROCEDURE [dbo].[ProjectStatusListAll]
AS
	SET NOCOUNT ON
	
	SELECT s.ProjectStatusId, s.Name
	  FROM dbo.ProjectStatus AS s
	ORDER BY s.ProjectStatusId

