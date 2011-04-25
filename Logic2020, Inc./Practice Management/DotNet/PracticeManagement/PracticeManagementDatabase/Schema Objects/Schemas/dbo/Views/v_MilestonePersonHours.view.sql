-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-11-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	6-12-2008
-- Description:	List milestone-person hours
-- =============================================
CREATE VIEW [dbo].[v_MilestonePersonHours]
AS
	SELECT s.MilestoneId, s.PersonId, SUM(s.HoursPerDay) AS MilestonePersonHours
	  FROM dbo.v_MilestonePersonSchedule AS s
	GROUP BY s.MilestoneId, s.PersonId

