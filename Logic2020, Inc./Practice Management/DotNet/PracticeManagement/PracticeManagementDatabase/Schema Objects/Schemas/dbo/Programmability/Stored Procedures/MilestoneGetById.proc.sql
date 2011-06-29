-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-22-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	8-26-2008
-- Description:	Gets a Milestone
-- =============================================
CREATE PROCEDURE dbo.MilestoneGetById
(
	@MilestoneId   INT
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
	       m.MilestoneIsChargeable,		  
		   m.IsMarginColorInfoEnabled
	  FROM dbo.v_Milestone AS m
	 WHERE m.MilestoneId = @MilestoneId

