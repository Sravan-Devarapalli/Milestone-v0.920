CREATE VIEW [dbo].[v_PersonPayRetrospective]
AS
	SELECT cal.PersonId,
	       cal.Date,
	       p.Amount AS Rate,
	       p.Timescale,
	       t.Name AS TimescaleName,
	       CASE
	           WHEN p.Timescale IN (1, 3, 4)
	           THEN p.Amount
	           ELSE p.Amount / HY.HoursInYear
	       END AS HourlyRate,
	       p.BonusAmount,
           p.BonusHoursToCollect,
           p.PracticeId,
		   p.VacationDays,
		   CAST(CASE p.BonusHoursToCollect WHEN GHY.HoursPerYear THEN 1 ELSE 0 END AS BIT) AS IsYearBonus
	  FROM dbo.PersonCalendarAuto AS cal
	       INNER JOIN dbo.Pay AS p 
	           ON cal.PersonId = p.Person AND cal.DayOff = 0 AND p.StartDate <= cal.Date AND p.EndDate > cal.date  
	       INNER JOIN dbo.Timescale AS t ON p.Timescale = t.TimescaleId
		   INNER JOIN dbo.GetHoursPerYearTable() GHY ON 1=1--For improving query performance we are using table valued function instead of scalar function.
	       INNER JOIN V_WorkinHoursByYear HY ON HY.[Year] = YEAR(cal.Date)
