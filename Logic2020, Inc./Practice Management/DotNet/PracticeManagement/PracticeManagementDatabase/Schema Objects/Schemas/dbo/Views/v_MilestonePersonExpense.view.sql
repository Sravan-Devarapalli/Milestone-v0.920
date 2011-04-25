-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-7-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	8-13-2008
-- Description:	List milestone-person expese by days
-- =============================================
CREATE VIEW [dbo].[v_MilestonePersonExpense]
AS
	SELECT s.PersonId,
	       s.Date,
	       s.MilestoneId,
	       s.ProjectId,
	       ISNULL(o.HourlyRate, 0) + ISNULL(p.HourlyRate, 0) AS HourlyRate,
	       s.HoursPerDay,
	       (ISNULL(o.HourlyRate, 0) + ISNULL(p.HourlyRate, 0)) * s.HoursPerDay AS DailyRate,
	       p.Timescale
	  FROM dbo.v_MilestonePersonSchedule AS s
	       LEFT JOIN (SELECT o.PersonId, o.Date, SUM(o.HourlyRate) AS HourlyRate
	                    FROM dbo.v_PersonOverheadRetrospective AS o
	                  GROUP BY o.PersonId, o.Date
	                 ) AS o ON o.PersonId = s.PersonId AND o.Date = s.Date
	       LEFT JOIN dbo.v_PersonPayRetrospective AS p ON p.PersonId = s.PersonId AND p.Date = s.Date

