-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-13-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 8-12-2008
-- Description:	Determines the calendar for the persons.
-- =============================================
CREATE VIEW [dbo].[v_PersonCalendar]
AS
	SELECT cal.Date,
	       p.[PersonId],
	       ISNULL(pcal.DayOff, cal.DayOff) AS DayOff,
	       cal.DayOff AS CompanyDayOff
	  FROM dbo.Calendar AS cal
	       INNER JOIN dbo.Person AS p ON cal.Date >= p.HireDate AND cal.Date < ISNULL(p.TerminationDate, dbo.GetFutureDate())
	       LEFT JOIN dbo.PersonCalendar AS pcal ON pcal.Date = cal.Date AND pcal.PersonId = p.PersonId

