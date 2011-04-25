-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-11-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	8-26-2008
-- Description:	List milestone hours
-- =============================================
CREATE VIEW [dbo].[v_MilestoneHours]
AS
	SELECT s.MilestoneId, SUM(s.HoursPerDay) AS MilestoneHours, COUNT(DISTINCT PersonId) AS PersonCount
	  FROM dbo.v_MilestonePersonSchedule AS s
	GROUP BY s.[MilestoneId]

