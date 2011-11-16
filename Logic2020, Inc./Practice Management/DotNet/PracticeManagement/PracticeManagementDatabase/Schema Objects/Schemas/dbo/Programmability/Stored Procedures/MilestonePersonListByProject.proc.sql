
CREATE PROCEDURE dbo.MilestonePersonListByProject
(
	@ProjectId   INT,
	@IncludeStrawman BIT = 1
)
AS
BEGIN
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
	       mp.MilestoneHourlyRevenue,
	       mp.ConsultantsCanAdjust,
	       mp.ClientIsChargeable,
	       mp.ProjectIsChargeable,
	       mp.MilestoneIsChargeable
	  FROM dbo.v_MilestonePerson AS mp
	  INNER JOIN dbo.Person AS P ON (P.PersonId = MP.PersonId AND (@IncludeStrawman = 1  OR P.IsStrawman = 0))
	 WHERE mp.ProjectId = @ProjectId 

END
