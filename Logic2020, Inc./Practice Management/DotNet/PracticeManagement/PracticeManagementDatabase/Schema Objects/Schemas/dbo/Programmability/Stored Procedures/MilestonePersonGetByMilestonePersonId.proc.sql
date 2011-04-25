
CREATE PROCEDURE [dbo].[MilestonePersonGetByMilestonePersonId]
(
	@MilestonePersonId   INT
)
AS
	SET NOCOUNT ON

	SELECT mp.MilestoneId,
	       mp.PersonId,
	       pers.SeniorityId,
	       mp.MilestonePersonId,
	       m.IsHourlyAmount,
	       m.Description AS MilestoneName,
	       m.StartDate AS MilestoneStartDate,
	       m.ProjectedDeliveryDate AS MilestoneProjectedDeliveryDate,
	       m.ProjectId,
	       p.Name AS ProjectName,
	       p.StartDate AS ProjectStartDate,
	       p.EndDate AS ProjectEndDate,
	       p.ClientId,
	       c.Name AS ClientName
	  FROM dbo.MilestonePerson AS mp
	       INNER JOIN dbo.Milestone AS m ON m.MilestoneId = mp.MilestoneId
	       INNER JOIN dbo.Project AS p ON p.ProjectId = m.ProjectId
	       INNER JOIN dbo.Client AS c ON c.ClientId = p.ClientId
	       INNER JOIN dbo.Person AS pers ON pers.PersonId = mp.PersonId
	 WHERE mp.MilestonePersonId = @MilestonePersonId

