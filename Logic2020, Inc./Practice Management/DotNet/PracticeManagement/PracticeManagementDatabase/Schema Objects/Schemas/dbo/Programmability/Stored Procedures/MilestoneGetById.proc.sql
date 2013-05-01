CREATE PROCEDURE dbo.MilestoneGetById
(
	@MilestoneId   INT
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT m.ClientId,
	       m.ProjectId,
	       m.MilestoneId,
	       m.Description,
	       m.Amount,
	       m.StartDate,
		   m.ProjectStatusId,
		   m.ProjectStatusName,
	       m.ProjectedDeliveryDate,
	       m.IsHourlyAmount,
	       m.ProjectName,
	       m.ProjectStartDate,
	       m.ProjectEndDate,
	       m.Discount,
	       m.ClientName,
	       m.ExpectedHours,
	       m.SalesCommission,
	       m.PersonCount,
	       m.ProjectedDuration,
	       m.ConsultantsCanAdjust,
	       m.ClientIsChargeable,
	       m.ProjectIsChargeable,
	       m.MilestoneIsChargeable,		  
		   m.IsMarginColorInfoEnabled
	  FROM dbo.v_Milestone AS m
	 WHERE m.MilestoneId = @MilestoneId
END

