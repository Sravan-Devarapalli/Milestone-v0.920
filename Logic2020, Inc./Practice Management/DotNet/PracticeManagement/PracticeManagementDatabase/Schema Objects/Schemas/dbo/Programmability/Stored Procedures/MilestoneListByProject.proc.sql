CREATE PROCEDURE dbo.MilestoneListByProject
(
	@ProjectId   INT
)
AS
	SET NOCOUNT ON

	SELECT m.ClientId,
	       m.ProjectId,
	       m.MilestoneId,
	       m.Description,
	       m.Amount,
	       m.StartDate,
	       m.ProjectedDeliveryDate,
	       m.IsHourlyAmount,
	       m.ProjectName,
	       m.ProjectStartDate,
	       m.ProjectEndDate,
	       m.Discount,
	       m.ClientName,
	       m.ExpectedHours,
	       m.PersonCount,
	       m.ProjectedDuration,
	       m.ConsultantsCanAdjust,
	       m.ClientIsChargeable,
	       m.ProjectIsChargeable,
	       m.MilestoneIsChargeable
	  FROM dbo.v_Milestone AS m
	 WHERE m.ProjectId = @ProjectId
	ORDER BY m.StartDate, m.ProjectedDeliveryDate

