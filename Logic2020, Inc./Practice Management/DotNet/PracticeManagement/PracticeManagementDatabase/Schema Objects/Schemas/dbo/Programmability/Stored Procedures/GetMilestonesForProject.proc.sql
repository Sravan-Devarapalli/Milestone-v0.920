-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-29-2012
-- Description:	Retrieves the list of Milestones.
-- Updated by : Sainath.CH
-- Update Date: 03-30-2012
-- =============================================
CREATE PROCEDURE dbo.GetMilestonesForProject
(
	@ProjectNumber   NVARCHAR(12)
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT 
	       M.Description AS MilestoneName,
	       M.MilestoneId,
		   M.StartDate,
		   M.ProjectedDeliveryDate AS EndDate
	  FROM dbo.Milestone AS M
	  INNER JOIN dbo.Project AS P ON M.ProjectId = P.ProjectId
	  WHERE P.ProjectNumber = @ProjectNumber
	  ORDER BY m.StartDate, m.ProjectedDeliveryDate
END
