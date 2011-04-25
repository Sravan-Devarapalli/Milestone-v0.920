--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-9-2008
-- Updated by:	
-- Update date:	
-- Description:	List person roles
-- =============================================
CREATE PROCEDURE [dbo].[PersonRoleListAll]
AS
	SET NOCOUNT ON

	SELECT r.PersonRoleId, r.Name
	  FROM dbo.PersonRole AS r
	ORDER BY r.PersonRoleId

