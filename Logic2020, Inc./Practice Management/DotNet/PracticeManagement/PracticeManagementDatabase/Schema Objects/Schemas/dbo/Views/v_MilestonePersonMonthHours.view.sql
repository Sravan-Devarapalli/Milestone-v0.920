-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-12-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	9-09-2008
-- Description:	List milestone-person hours by months
-- =============================================
CREATE VIEW [dbo].[v_MilestonePersonMonthHours]
AS
	SELECT s.[MilestoneId],
	       s.ProjectId,
	       s.PersonId,
	       SUM(s.HoursPerDay) AS MilestonePersonMonthHours,
	       MIN(s.Date) AS [Month],
	       MAX(s.Date) AS [MonthEnd],
	       s.EntryStartDate
	  FROM dbo.v_MilestonePersonSchedule AS s
	GROUP BY s.[MilestoneId], s.ProjectId, s.PersonId, DATEPART(yyyy, s.Date), DATEPART(mm, s.Date), s.EntryStartDate

