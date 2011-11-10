﻿CREATE PROCEDURE [dbo].[MilestonePersonListByMilestone]
(
	@MilestoneId   INT
)
AS
	SET NOCOUNT ON
	DECLARE @MilestoneIdLocal INT
	SELECT @MilestoneIdLocal = @MilestoneId
	
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
	       mp.MilestoneHourlyRevenue,
	       p.HireDate,
	       p.TerminationDate,
		   mp.EntryId
	  FROM dbo.v_MilestonePerson AS mp
	  INNER JOIN Person AS p ON mp.PersonId = p.PersonId
	  WHERE mp.MilestoneId = @MilestoneIdLocal

