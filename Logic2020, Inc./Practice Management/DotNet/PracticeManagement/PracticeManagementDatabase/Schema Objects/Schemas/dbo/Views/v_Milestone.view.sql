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
	       ISNULL(h.PersonCount, 0) AS PersonCount,
	       (SELECT COUNT(*)
	          FROM dbo.Calendar AS cal
	         WHERE cal.Date BETWEEN m.StartDate AND ProjectedDeliveryDate AND cal.DayOff = 0) AS ProjectedDuration,
	       p.BuyerName,
           p.GroupId,
		   p.IsAllowedToShow,
		   p.ProjectManagerId,
		   p.SalesPersonId,
		   p.PONumber
	  FROM dbo.Milestone AS m
	       INNER JOIN dbo.Project AS p ON m.ProjectId = p.ProjectId
	       INNER JOIN dbo.Client AS c ON p.ClientId = c.ClientId
		   INNER JOIN dbo.ProjectStatus AS ps on p.ProjectStatusId=ps.ProjectStatusId
	       LEFT JOIN dbo.v_MilestoneHours AS h ON m.MilestoneId = h.MilestoneId

