-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-21-1008
-- Description:	Mark a project inactive
-- =============================================
CREATE PROCEDURE dbo.ProjectInactivate
(
	@ProjectId   INT
)
AS
	SET NOCOUNT ON

	UPDATE dbo.Project
	   SET ProjectStatusId = 3
	 WHERE ProjectId = @ProjectId

