CREATE PROCEDURE [dbo].[GetMilestoneAndCSATCountsByProject]
(
	@ProjectId   INT
)
AS
BEGIN

	SELECT	(SELECT COUNT(*) FROM dbo.Milestone M WHERE ProjectId= @ProjectId) AS MilestoneCount,
			(SELECT COUNT(*) FROM dbo.ProjectCSAT PC WHERE ProjectId= @ProjectId) AS CSATCount,
			(SELECT COUNT(*) FROM dbo.Attribution a WHERE ProjectId= @ProjectId) AS AttributionCount

END
