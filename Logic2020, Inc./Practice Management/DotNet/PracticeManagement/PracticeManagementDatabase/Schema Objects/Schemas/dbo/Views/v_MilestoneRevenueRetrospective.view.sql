-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-10-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	10-13-2008
-- Description:	Selects a revenue day-by-day
-- =============================================
CREATE VIEW dbo.v_MilestoneRevenueRetrospective
AS
	SELECT -- Milestones with a fixed amount
		   m.MilestoneId,
		   m.ProjectId,
		   cal.Date,
		   m.IsHourlyAmount,
	       ISNULL((m.Amount / (SELECT SUM(HoursPerDay) AS TotalMilestoneHours
					    	FROM dbo.v_MilestonePersonSchedule AS s
                            WHERE s.MileStoneId = m.MilestoneId)) * ISNULL(d.HoursPerDay, 0),
	              (CASE (DATEDIFF(dd, m.StartDate, m.ProjectedDeliveryDate) + 1)
	                   WHEN 0 THEN 0
	                   ELSE m.Amount / (DATEDIFF(dd, m.StartDate, m.ProjectedDeliveryDate) + 1)
	               END)) AS MilestoneDailyAmount,
		   m.StartDate AS MilestoneStartDate,
		   m.ProjectedDeliveryDate,
           p.Discount,
	       prac.PracticeManagerId,
	       d.HoursPerDay
	  FROM dbo.Milestone AS m
		   INNER JOIN dbo.Calendar AS cal ON cal.Date BETWEEN m.StartDate AND m.ProjectedDeliveryDate
	       INNER JOIN dbo.Project AS p ON m.ProjectId = p.ProjectId
	       INNER JOIN dbo.Practice AS prac ON p.PracticeId = prac.PracticeId
           LEFT JOIN (SELECT s.Date, s.MilestoneId, SUM(HoursPerDay) AS HoursPerDay
	                    FROM dbo.v_MilestonePersonSchedule AS s
	                  GROUP BY s.Date, s.MilestoneId) d ON d.date = cal.Date and m.MilestoneId = d.MileStoneId
	 WHERE m.IsHourlyAmount = 0
	UNION ALL
	SELECT -- Milestones with a hourly amount
		   mp.MilestoneId,
		   mp.ProjectId,
		   mp.Date,
		   mp.IsHourlyAmount,
		   ISNULL(SUM(mp.Amount * mp.HoursPerDay), 0) AS MilestoneDailyAmount,
		   MAX(mp.StartDate) AS MilestoneStartDate,
		   MAX(mp.ProjectedDeliveryDate) AS ProjectedDeliveryDate,
           MAX(p.Discount) AS Discount,
	       prac.PracticeManagerId,
	       SUM(mp.HoursPerDay) AS HoursPerDay
	  FROM dbo.v_MilestonePersonSchedule mp
	       INNER JOIN dbo.Project AS p ON mp.ProjectId = p.ProjectId
	       INNER JOIN dbo.Practice AS prac ON p.PracticeId = prac.PracticeId
	 WHERE mp.IsHourlyAmount = 1
	GROUP BY mp.MilestoneId, mp.ProjectId, mp.Date, mp.IsHourlyAmount, prac.PracticeManagerId

