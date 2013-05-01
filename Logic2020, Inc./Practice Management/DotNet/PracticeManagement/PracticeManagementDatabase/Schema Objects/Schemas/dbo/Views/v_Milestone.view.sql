﻿-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-28-2008
-- Updated by:	Sainath C
-- Update date:	06-05-2012
-- Description:	List milestones with their details
-- =============================================
CREATE VIEW dbo.v_Milestone
AS
	SELECT c.ClientId,
		   c.IsMarginColorInfoEnabled,
	       m.ProjectId,
	       m.MilestoneId,
	       m.Description, 
	       m.Amount,
	       m.StartDate,
	       m.ProjectedDeliveryDate,
	       m.IsHourlyAmount,
	       p.Name AS ProjectName,
	       p.StartDate AS ProjectStartDate,
	       p.EndDate AS ProjectEndDate,
	       p.Discount,
		   p.ProjectStatusId,
		   ps.Name AS ProjectStatusName,
	       p.ProjectNumber,
	       c.Name AS ClientName,
	       c.IsChargeable AS 'ClientIsChargeable',
	       p.IsChargeable AS 'ProjectIsChargeable',
	       m.IsChargeable AS 'MilestoneIsChargeable',
	       m.ConsultantsCanAdjust,
	       CAST(ISNULL(h.MilestoneHours, 0) AS DECIMAL(18,2)) ExpectedHours,
	       ISNULL((SELECT SUM(cm.FractionOfMargin)
	                 FROM dbo.Commission AS cm
	                WHERE cm.ProjectId = m.ProjectId AND cm.CommissionType = 1), 0) AS SalesCommission,
	       ISNULL(h.PersonCount, 0) AS PersonCount,
	       (SELECT COUNT(*)
	          FROM dbo.Calendar AS cal
	         WHERE cal.Date BETWEEN m.StartDate AND ProjectedDeliveryDate AND cal.DayOff = 0) AS ProjectedDuration,
	       p.BuyerName,
           p.GroupId,
		   p.IsAllowedToShow,
		   p.ProjectOwnerId
	  FROM dbo.Milestone AS m
	       INNER JOIN dbo.Project AS p ON m.ProjectId = p.ProjectId
	       INNER JOIN dbo.Client AS c ON p.ClientId = c.ClientId
		   INNER JOIN dbo.ProjectStatus AS ps on p.ProjectStatusId=ps.ProjectStatusId
	       LEFT JOIN dbo.v_MilestoneHours AS h ON m.MilestoneId = h.MilestoneId

