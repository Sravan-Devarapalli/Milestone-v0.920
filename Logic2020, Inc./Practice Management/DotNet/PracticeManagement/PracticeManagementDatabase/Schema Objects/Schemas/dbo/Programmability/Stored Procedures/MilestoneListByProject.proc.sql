-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-21-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	8-26-2008
-- Description:	List Milestones by the specified Project
-- =============================================
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
	       m.ActualDeliveryDate,
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
	       m.MilestoneIsChargeable
	  FROM dbo.v_Milestone AS m
	 WHERE m.ProjectId = @ProjectId
	ORDER BY m.StartDate, m.ProjectedDeliveryDate

