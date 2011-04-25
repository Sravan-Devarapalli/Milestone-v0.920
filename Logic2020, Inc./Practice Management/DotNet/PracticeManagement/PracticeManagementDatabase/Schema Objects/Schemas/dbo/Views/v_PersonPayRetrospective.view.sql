CREATE VIEW [dbo].[v_PersonPayRetrospective]
AS
	SELECT cal.PersonId,
	       cal.Date,
	       p.Amount AS Rate,
	       p.Timescale,
	       t.Name AS TimescaleName,
	       p.DefaultHoursPerDay,
	       CASE
	           WHEN p.Timescale IN (1, 3, 4)
	           THEN p.Amount
	           ELSE p.Amount / HY.HoursInYear
	       END AS HourlyRate,
	       p.BonusAmount,
           p.BonusHoursToCollect,
           p.PracticeId,
		   p.VacationDays,
		   CAST(CASE p.BonusHoursToCollect WHEN dbo.GetHoursPerYear() THEN 1 ELSE 0 END AS BIT) AS IsYearBonus
	  FROM dbo.PersonCalendarAuto AS cal
	       INNER JOIN dbo.Pay AS p
	           ON p.StartDate <= cal.Date AND p.EndDate > cal.date AND cal.PersonId = p.Person
	       INNER JOIN dbo.Timescale AS t ON p.Timescale = t.TimescaleId
	       LEFT JOIN V_WorkinHoursByYear HY ON HY.Year = YEAR(cal.Date)
	 WHERE cal.DayOff = 0


