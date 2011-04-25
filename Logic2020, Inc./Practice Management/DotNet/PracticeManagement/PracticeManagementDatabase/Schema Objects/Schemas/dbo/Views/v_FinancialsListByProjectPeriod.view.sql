CREATE VIEW dbo.v_FinancialsListByProjectPeriod
AS 
	SELECT f.ProjectId,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), 1) AS FinancialDate,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), dbo.GetDaysInMonth(MIN(f.Date))) AS MonthEnd,

	       SUM(f.PersonMilestoneDailyAmount) AS Revenue,

	       SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount) AS RevenueNet,

	       ISNULL(SUM((ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)) * f.PersonHoursPerDay), 0) AS Cogs,

	       SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount) -
	       ISNULL(SUM((ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)) * f.PersonHoursPerDay), 0) AS GrossMargin,

	       ISNULL(SUM(f.PersonHoursPerDay), 0) AS Hours,

	       SUM(f.PersonMilestoneDailyAmount - ISNULL(f.PayRate * f.PersonHoursPerDay, 0)) *
	       (SELECT SUM(c.FractionOfMargin) FROM dbo.Commission AS c WHERE c.ProjectId = f.ProjectId AND c.CommissionType = 1) / 100 AS SalesCommission,

	       SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
	            (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)) * ISNULL(f.PersonHoursPerDay, 0)) *
	           (f.PracticeManagementCommissionSub + CASE f.PracticeManagerId WHEN f.PersonId THEN f.PracticeManagementCommissionOwn ELSE 0 END)) / 100 AS PracticeManagementCommission
	  FROM dbo.v_FinancialsRetrospective AS f
	GROUP BY f.ProjectId, YEAR(f.Date), MONTH(f.Date)

