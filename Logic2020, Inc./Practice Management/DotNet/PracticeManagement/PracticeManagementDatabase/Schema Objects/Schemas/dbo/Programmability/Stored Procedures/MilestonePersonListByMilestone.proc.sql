


CREATE PROCEDURE [dbo].[MilestonePersonListByMilestone]
(
	@MilestoneId   INT
)
AS
	SET NOCOUNT ON
	
	SELECT mp.MilestonePersonId,
	       mp.MilestoneId,
	       mp.PersonId,
	       mp.SeniorityId,
	       mp.StartDate,
	       mp.EndDate,
	       mp.HoursPerDay,
	       mp.FirstName,
	       mp.LastName,
	       mp.ProjectId,
	       mp.ProjectName,
	       mp.ProjectStartDate,
	       mp.ProjectEndDate,
	       mp.Discount,
	       mp.ClientId,
	       mp.ClientName,
	       mp.MilestoneName,
	       mp.MilestoneStartDate,
	       mp.MilestoneProjectedDeliveryDate,
	       mp.ExpectedHours,
	       mp.PersonRoleId,
	       mp.RoleName,
	       mp.Amount,
	       mp.IsHourlyAmount,
	       mp.SalesCommission,
	       mp.MilestoneExpectedHours,
	       mp.MilestoneActualDeliveryDate,
	       mp.MilestoneHourlyRevenue
	  FROM dbo.v_MilestonePerson AS mp
	 WHERE mp.MilestoneId = @MilestoneId

