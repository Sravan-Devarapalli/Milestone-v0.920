-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-11-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	9-18-2008
-- Description:	Selects financils for the specified project and period grouped by months
-- =============================================
CREATE PROCEDURE [dbo].[FinancialsListByProjectPeriod]
(
	@ProjectId   VARCHAR(2500),
	@StartDate   DATETIME,
	@EndDate     DATETIME
)
AS
	SET NOCOUNT ON
	
	DECLARE @ProjectIDs TABLE
	(
		ResultId INT
	)
	
	INSERT INTO @ProjectIDs
	SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectId)

	;WITH FinancialsRetro AS 
	(
	SELECT f.ProjectId,
		   f.Date, 
		   f.PersonMilestoneDailyAmount,
		   f.PersonDiscountDailyAmount,
		   ISNULL((SELECT SUM(c.FractionOfMargin) 
							  FROM dbo.Commission AS  c 
							  WHERE c.ProjectId = f.ProjectId 
									AND c.CommissionType = 1
								),0) ProjectSalesCommisionFraction,
		   (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0)
			+ISNULL(f.RecruitingCommissionRate,0)) SLHR,
		   ISNULL(f.PayRate,0) PayRate,
		   f.MLFOverheadRate,
		   f.PersonHoursPerDay,
		   f.PracticeManagementCommissionSub,
		   f.PracticeManagementCommissionOwn ,
		   f.PracticeManagerId,
		   f.PersonId,
		   f.Discount
	FROM v_FinancialsRetrospective f
	WHERE f.ProjectId  IN (SELECT * FROM @ProjectIDs) 
	),
	ProjectFinancials
	AS
	(
	SELECT f.ProjectId,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), 1) AS FinancialDate,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), dbo.GetDaysInMonth(MIN(f.Date))) AS MonthEnd,

	       SUM(f.PersonMilestoneDailyAmount) AS Revenue,

	       SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount) AS RevenueNet,
	       
		   SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >= f.PayRate + f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate + f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0)) GrossMargin,
		   
		   ISNULL(SUM((CASE WHEN f.SLHR >=  f.PayRate + f.MLFOverheadRate THEN f.SLHR ELSE  f.PayRate + f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)),0) Cogs,
	       ISNULL(SUM(f.PersonHoursPerDay), 0) AS Hours,
	       
	       (SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0))* (f.ProjectSalesCommisionFraction/100))) SalesCommission,

	       SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
	            (CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) * ISNULL(f.PersonHoursPerDay, 0)) *
	           (f.PracticeManagementCommissionSub + CASE f.PracticeManagerId WHEN f.PersonId THEN f.PracticeManagementCommissionOwn ELSE 0 END)) / 100 AS PracticeManagementCommission,
		   min(case when pe.ExpenseSum is null then 0 else pe.ExpenseSum end) as 'Expense',
		   min(case when pe.ReimbursedExpenseSum is null then 0 else pe.ReimbursedExpenseSum end) as 'ReimbursedExpense',
		   min(f.Discount) as Discount
	  FROM FinancialsRetro AS f
	  LEFT JOIN v_ProjectTotalExpenses as pe on f.ProjectId = pe.ProjectId
	  WHERE  
		f.ProjectId IN (SELECT * FROM @ProjectIDs) 
		AND f.Date BETWEEN @StartDate AND @EndDate
	GROUP BY f.ProjectId, YEAR(f.Date), MONTH(f.Date)
	)
	
	SELECT
		pf.ProjectId,
		pf.FinancialDate,
		pf.MonthEnd,
		ISNULL(pf.Revenue,0) as 'Revenue',
		ISNULL(pf.RevenueNet+(pf.ReimbursedExpense * (1 - pf.Discount/100)),0)  as 'RevenueNet',
		pf.Cogs ,
		ISNULL((pf.GrossMargin+(pf.ReimbursedExpense * (1 - pf.Discount/100)) - pf.Expense),0)  as 'GrossMargin',
		pf.Hours,
		pf.SalesCommission,
		pf.PracticeManagementCommission,
		pf.Expense,
		pf.ReimbursedExpense
	FROM ProjectFinancials pf
	

