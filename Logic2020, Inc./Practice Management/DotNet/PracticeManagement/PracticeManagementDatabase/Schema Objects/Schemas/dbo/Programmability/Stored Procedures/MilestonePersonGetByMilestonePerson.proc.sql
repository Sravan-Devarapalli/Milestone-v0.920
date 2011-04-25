-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-3-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 9-09-2008
-- Description:	Selects persons-milestones details for the specified milestone and person.
-- =============================================
CREATE PROCEDURE [dbo].[MilestonePersonGetByMilestonePerson]
(
	@MilestoneId   INT,
	@PersonId      INT
)
AS
	SET NOCOUNT ON
	
	SELECT mp.MilestonePersonId,
	       mp.MilestoneId,
	       mp.PersonId,
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
	   AND mp.PersonId = @PersonId

