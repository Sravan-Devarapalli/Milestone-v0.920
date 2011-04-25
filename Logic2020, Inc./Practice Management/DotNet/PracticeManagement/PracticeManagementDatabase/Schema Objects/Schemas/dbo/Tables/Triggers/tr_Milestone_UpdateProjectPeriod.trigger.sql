
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 8-12-2008
-- Description:	Updates a Project's Start and End Dates according to the data in the Milestone table
-- =============================================
CREATE TRIGGER tr_Milestone_UpdateProjectPeriod
ON dbo.Milestone
AFTER INSERT, UPDATE, DELETE
AS
	SET NOCOUNT ON

	UPDATE dbo.Project
	   SET StartDate = (SELECT MIN(StartDate) FROM dbo.Milestone AS m WHERE m.ProjectId = Project.ProjectId),
	       EndDate = (SELECT MAX(ProjectedDeliveryDate) FROM dbo.Milestone AS m WHERE m.ProjectId = Project.ProjectId)
	 WHERE EXISTS (SELECT 1 FROM inserted AS i WHERE i.ProjectId = Project.ProjectId)
	    OR EXISTS (SELECT 1 FROM deleted AS i WHERE i.ProjectId = Project.ProjectId)

