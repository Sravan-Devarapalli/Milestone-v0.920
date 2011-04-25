--The source is 
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-24-2008
-- Updated by:	
-- Update date: 
-- Description:	Lists the payments
-- =============================================
CREATE VIEW [dbo].[v_Pay]
AS
	SELECT p.Person AS PersonId,
	       p.StartDate,
	       CASE
	           WHEN p.EndDate < dbo.GetFutureDate()
	           THEN p.EndDate
	           ELSE CAST(NULL AS DATETIME)
	       END AS EndDate,
	       p.EndDate AS EndDateOrig,
	       p.Amount,
	       p.Timescale,
	       CAST(CASE p.Timescale
	                WHEN 2 THEN p.Amount / dbo.GetHoursPerYear()
	                ELSE p.Amount
	            END AS DECIMAL(18,2)) AS AmountHourly,
	       p.TimesPaidPerMonth,
	       p.Terms,
	       p.VacationDays,
	       p.BonusAmount,
	       p.BonusHoursToCollect,
	       CAST(CASE p.BonusHoursToCollect WHEN dbo.GetHoursPerYear() THEN 1 ELSE 0 END AS BIT) AS IsYearBonus,
	       p.DefaultHoursPerDay,
	       t.Name AS TimescaleName,
		   p.SeniorityId,
		   p.PracticeId
	  FROM dbo.Pay AS p
	       INNER JOIN dbo.Timescale AS t ON p.Timescale = t.TimescaleId

