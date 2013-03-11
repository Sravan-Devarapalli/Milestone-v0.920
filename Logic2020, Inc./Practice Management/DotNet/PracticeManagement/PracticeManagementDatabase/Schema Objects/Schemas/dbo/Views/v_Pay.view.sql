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
	           WHEN p.EndDate < FT.FutureDate
	           THEN p.EndDate
	           ELSE CAST(NULL AS DATETIME)
	       END AS EndDate,
	       p.EndDate AS EndDateOrig,
	       p.Amount,
	       p.Timescale,
	       CAST(CASE p.Timescale
	                WHEN 2 THEN p.Amount / WHY.HoursInYear
	                ELSE p.Amount
	            END AS DECIMAL(18,2)) AS AmountHourly,
	       p.VacationDays,
	       p.BonusAmount,
	       p.BonusHoursToCollect,
	       CAST(CASE p.BonusHoursToCollect WHEN HPY.HoursPerYear THEN 1 ELSE 0 END AS BIT) AS IsYearBonus,
	       t.Name AS TimescaleName,
		   p.PracticeId,
		   p.TitleId,
		   p.SLTApproval,
		   p.SLTPTOApproval
	  FROM dbo.Pay AS p
		   INNER JOIN dbo.GetFutureDateTable() FT ON 1 = 1
		   INNER JOIN dbo.[BonusHoursPerYearTable]() HPY ON 1 = 1
		   INNER JOIN  dbo.V_WorkinHoursByYear WHY ON WHY.Year = YEAR(dbo.GettingPMTime(GETUTCDATE()))
	       INNER JOIN dbo.Timescale AS t ON p.Timescale = t.TimescaleId

