CREATE VIEW [dbo].[v_PersonPayRetrospective]
AS
	SELECT cal.PersonId,
	       cal.Date,
	       p.Amount AS Rate,
	       p.Timescale,
	       CASE
	           WHEN p.Timescale IN (1, 3, 4)
	           THEN p.Amount
	           ELSE p.Amount / HY.HoursInYear
	       END AS HourlyRate,
  		   p.BonusAmount / (CASE WHEN p.BonusHoursToCollect = GHY.HoursPerYear THEN HY.HoursInYear ELSE NULLIF(p.BonusHoursToCollect,0) END) AS BonusRate,
		   p.VacationDays
	  FROM dbo.PersonCalendarAuto AS cal
	       INNER JOIN dbo.Pay AS p ON cal.PersonId = p.Person AND p.StartDate <= cal.Date AND p.EndDate > cal.date  
		   INNER JOIN dbo.[BonusHoursPerYearTable]() GHY ON 1=1--For improving query performance we are using table valued function instead of scalar function.
	       INNER JOIN V_WorkinHoursByYear HY ON cal.Date BETWEEN HY.[YearStartDate] AND HY.[YearEndDate]

