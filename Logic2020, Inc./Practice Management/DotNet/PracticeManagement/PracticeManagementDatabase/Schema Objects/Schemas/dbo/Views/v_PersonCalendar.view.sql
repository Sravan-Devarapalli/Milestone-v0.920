-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-13-2008
-- Updated by:	Sainath CH
-- Update date: 03-27-2012
-- Description:	Determines the calendar for the persons.
-- =============================================
CREATE VIEW [dbo].[v_PersonCalendar]
AS
	SELECT cal.Date,
	       p.[PersonId],
	       ISNULL(pcal.DayOff, cal.DayOff) AS DayOff,
	       cal.DayOff AS CompanyDayOff,
		   (CASE WHEN ISNULL(pcal.TimeTypeId, 0) = dbo.GetHolidayTimeTypeId()  THEN 1 ELSE 0 END)   AS 'IsFloatingHoliday',
		   pcal.ActualHours,
		   pcal.TimeTypeId,
		   pcal.Description,
		   pcal.ApprovedBy,
		   pcal.SubstituteDate
	  FROM dbo.Calendar AS cal
	       INNER JOIN dbo.Person AS p ON cal.Date >= p.HireDate AND cal.Date < ISNULL(p.TerminationDate, dbo.GetFutureDate())
	       LEFT JOIN dbo.PersonCalendar AS pcal ON pcal.Date = cal.Date AND pcal.PersonId = p.PersonId

