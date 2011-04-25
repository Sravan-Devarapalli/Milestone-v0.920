-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-11-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	9-25-2008
-- Description:	Selects summary financils for the specified project
-- =============================================
CREATE PROCEDURE dbo.FinancialsGetByProject
(
	@ProjectId   INT
)
AS
	SET NOCOUNT ON;

	WITH FinancialsRetro AS 
	(
	SELECT f.ProjectId,
		   f.Date, 
		   f.PersonMilestoneDailyAmount,
		   f.PersonDiscountDailyAmount,
		   CASE WHEN ISNULL(f.PersonHoursPerDay,0) = 0 THEN 0
				ELSE 
					ISNULL(
							( ((f.PersonMilestoneDailyAmount-f.PersonDiscountDailyAmount)/f.PersonHoursPerDay)-
								(ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+f.BonusRate+f.VacationRate + ISNULL(f.RecruitingCommissionRate,0))
							)* (SELECT SUM(c.FractionOfMargin) 
							  FROM dbo.Commission AS  c 
							  WHERE c.ProjectId = f.ProjectId 
									AND c.CommissionType = 1
								)/100,0
							)
				END SCPH
			/*
		   GrossMargin-Semi = Revenue-SCogs
		   SCPH = GrossMargin-Semi*SC%
		   */,
		   ISNULL((SELECT SUM(c.FractionOfMargin) 
							  FROM dbo.Commission AS  c 
							  WHERE c.ProjectId = f.ProjectId 
									AND c.CommissionType = 1
								),0) ProjectSalesCommisionFraction,
		   (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0)
			+ISNULL(f.RecruitingCommissionRate,0)) SLHR,
		   ISNULL(f.PayRate, 0) PayRate,
		   f.MLFOverheadRate,
		   f.PersonHoursPerDay,
		   f.PracticeManagementCommissionSub,
		   f.PracticeManagementCommissionOwn ,
		   f.PracticeManagerId,
		   f.PersonId,
		   f.Discount
	FROM v_FinancialsRetrospective f
	WHERE f.ProjectId = @ProjectId
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
						(CASE WHEN f.SLHR+f.SCPH >= f.PayRate + f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate + f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0)) GrossMargin,
		   
		   ISNULL(SUM((CASE WHEN f.SLHR+f.SCPH >= f.PayRate + f.MLFOverheadRate THEN f.SLHR+f.SCPH ELSE f.PayRate + f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)),0) Cogs,

	       ISNULL(SUM(f.PersonHoursPerDay), 0) AS Hours,
	       
	       (SUM(
				 CASE WHEN f.SLHR+f.SCPH >=  f.PayRate +f.MLFOverheadRate THEN f.SCPH * ISNULL(f.PersonHoursPerDay, 0)
				    ELSE (f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount-((f.PayRate + f.MLFOverheadRate)*ISNULL(f.PersonHoursPerDay, 0)))
						* (f.ProjectSalesCommisionFraction/100)
					END
				 )) SalesCommission,

	       SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
	            (f.SLHR) * ISNULL(f.PersonHoursPerDay, 0)) *
	           (f.PracticeManagementCommissionSub + CASE f.PracticeManagerId WHEN f.PersonId THEN f.PracticeManagementCommissionOwn ELSE 0 END)) / 100 AS PracticeManagementCommission,
		   min(case when pe.ExpenseSum is null then 0 else pe.ExpenseSum end) as 'Expense',
		   min(case when pe.ReimbursedExpenseSum is null then 0 else pe.ReimbursedExpenseSum end) as 'ReimbursedExpense',
		   min(f.Discount) as Discount
	  FROM FinancialsRetro AS f
	  LEFT JOIN v_ProjectTotalExpenses as pe on f.ProjectId = pe.ProjectId
	 WHERE f.ProjectId = @ProjectId
	GROUP BY f.ProjectId
	)
	SELECT
		pf.ProjectId,
		pf.FinancialDate,
		pf.MonthEnd,
		ISNULL(pf.Revenue,0) as 'Revenue',
		ISNULL(pf.RevenueNet+(pf.ReimbursedExpense * (1 - pf.Discount/100)),0)  as 'RevenueNet',
		CASE WHEN (pr.IsCompanyInternal = 1) THEN 0
		ELSE pf.Cogs END AS 'Cogs',
		ISNULL((pf.GrossMargin+(pf.ReimbursedExpense * (1 - pf.Discount/100)) - pf.Expense),0)  as 'GrossMargin',
		pf.Hours,
		pf.SalesCommission,
		pf.PracticeManagementCommission,
		pf.Expense,
		pf.ReimbursedExpense
	FROM ProjectFinancials pf
	JOIN Project p on (p.ProjectId = pf.ProjectId)
	JOIN Practice pr on (pr.PracticeId = p.PracticeId)
	
