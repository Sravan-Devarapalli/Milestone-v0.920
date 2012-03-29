-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-29-2012
-- Description:	Retrieves the list of Milestones.
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
	       M.MilestoneId
	  FROM dbo.Milestone AS M
	  INNER JOIN dbo.Project AS P ON M.ProjectId = P.ProjectId
	  WHERE P.ProjectNumber = @ProjectNumber
	  ORDER BY m.StartDate, m.ProjectedDeliveryDate
END
