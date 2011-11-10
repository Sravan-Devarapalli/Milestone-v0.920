-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-10-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	10-13-2008
-- Description:	Selects a revenue day-by-day
-- =============================================
CREATE VIEW dbo.v_MilestoneRevenueRetrospective--Day level Milestone Amount & hours
AS
	SELECT -- Milestones with a fixed amount
		   m.MilestoneId,
		   m.ProjectId,
		   cal.Date,
		   m.IsHourlyAmount,
	       ISNULL((m.Amount/* Milestone fixed amount */ 
						   / 
							(   SELECT SUM(HoursPerDay)
								FROM   MilestonePerson MP  
								JOIN MilestonePersonEntry  MPE ON MP.MileStonePersonId = MPE.MileStonePersonId
								JOIN PersonCalendarAuto cal ON cal.Date BETWEEN mpe.Startdate AND ISNULL(mpe.EndDate, m.ProjectedDeliveryDate) AND cal.DayOff=0
								AND cal.PersonId = mp.PersonId 
								WHERE  mp.MileStoneId = m.MilestoneId
								GROUP BY MP.MilestoneId
							
                            ) /* Milestone Total  Hours */
							
							)* ISNULL(d.HoursPerDay, 0)/* Milestone Total  Hours per day*/,
	              (CASE (DATEDIFF(dd, m.StartDate, m.ProjectedDeliveryDate) + 1)
	                   WHEN 0 THEN 0
	                   ELSE m.Amount / (DATEDIFF(dd, m.StartDate, m.ProjectedDeliveryDate) + 1)
	               END)) AS MilestoneDailyAmount /* ((Milestone fixed amount/Milestone Total  Hours)* Milestone Total  Hours per day)  */,
		   m.StartDate AS MilestoneStartDate,
		   m.ProjectedDeliveryDate,
           p.Discount,
	       prac.PracticeManagerId,
	       d.HoursPerDay/* Milestone Total  Hours per day*/
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
	       SUM(mp.HoursPerDay) AS HoursPerDay/* Milestone Total  Hours per day*/
	  FROM dbo.v_MilestonePersonSchedule mp
	       INNER JOIN dbo.Project AS p ON mp.ProjectId = p.ProjectId
	       INNER JOIN dbo.Practice AS prac ON p.PracticeId = prac.PracticeId
	 WHERE mp.IsHourlyAmount = 1
	GROUP BY mp.MilestoneId, mp.ProjectId, mp.Date, mp.IsHourlyAmount, prac.PracticeManagerId

