
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
	
	DECLARE @StartDate DATETIME,
			@EndDate	DATETIME,
			@ProjectId	INT

	SELECT @ProjectId = P.ProjectId,
			@StartDate = (SELECT MIN(StartDate) FROM dbo.Milestone AS m WHERE m.ProjectId = P.ProjectId),
			@EndDate = (SELECT MAX(ProjectedDeliveryDate) FROM dbo.Milestone AS m WHERE m.ProjectId = P.ProjectId)
	FROM dbo.Project P
	 WHERE EXISTS (SELECT 1 FROM inserted AS i WHERE i.ProjectId = P.ProjectId)
	    OR EXISTS (SELECT 1 FROM deleted AS i WHERE i.ProjectId = P.ProjectId)

	UPDATE dbo.Project
	   SET StartDate = @StartDate,
	       EndDate = @EndDate
	FROM Project P
	WHERE P.ProjectId = @ProjectId

	UPDATE PTRS
		SET EndDate = @EndDate + (7 - DATEPART(dw,EndDate))
	FROM [dbo].[PersonTimeEntryRecursiveSelection] PTRS
	WHERE PTRS.ProjectId = @ProjectId AND PTRS.IsRecursive = 1


