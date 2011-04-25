-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 8-13-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	1-17-2008
-- Description:	List milestone-person associations
-- =============================================
CREATE VIEW [dbo].[v_MilestonePersonLight]
AS
	SELECT mp.MilestonePersonId,
	       mp.MilestoneId,
	       mp.PersonId,
	       mpe.StartDate,
	       ISNULL(mpe.EndDate, m.ProjectedDeliveryDate) AS EndDate,
	       mpe.HoursPerDay,
	       CASE m.IsHourlyAmount
	           WHEN 1
	           THEN mpe.Amount
	           ELSE m.Amount / s.Hours
	       END AS MilestoneHourlyRevenue
	  FROM dbo.MilestonePerson AS mp
	       INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
	       INNER JOIN dbo.Milestone AS m ON mp.MilestoneId = m.MilestoneId
	       LEFT JOIN (SELECT SUM(s.HoursPerDay) AS Hours, s.MilestoneId
	                    FROM dbo.v_MilestonePersonSchedule AS s
	                  GROUP BY s.MilestoneId) AS s ON s.MilestoneId = m.MilestoneId

