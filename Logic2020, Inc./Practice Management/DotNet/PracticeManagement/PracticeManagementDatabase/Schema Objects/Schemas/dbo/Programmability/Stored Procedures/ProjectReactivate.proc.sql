-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-21-1008
-- Description:	Mark a project active
-- =============================================
CREATE PROCEDURE dbo.ProjectReactivate
(
	@ProjectId   INT
)
AS
	SET NOCOUNT ON

	UPDATE dbo.Project
	   SET ProjectStatusId = 1
	 WHERE ProjectId = @ProjectId

