--The source is 
-- =============================================
-- Author:		Alexey Zvekov
-- Create date: 7-1-2008
-- Description:	List person statuses
-- =============================================
CREATE PROCEDURE [dbo].[PersonStatusListAll]
AS
	SET NOCOUNT ON
	
	SELECT s.PersonStatusId, s.Name
	  FROM dbo.PersonStatus AS s
	ORDER BY s.PersonStatusId

