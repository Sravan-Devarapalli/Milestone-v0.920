-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-06-11
-- Description:	Project milestones financials
-- =============================================
CREATE PROCEDURE dbo.ProjectMilestonesFinancials 
	@ProjectId INT
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ProjectIdLocal	INT

	SELECT @ProjectIdLocal = @ProjectId
	;WITH FinancialsRetro AS 
	(
	SELECT f.ProjectId,
		   f.MilestoneId,
		   f.Date, 
		   f.PersonMilestoneDailyAmount,
		   f.PersonDiscountDailyAmount,
		   ISNULL((SELECT SUM(c.FractionOfMargin) 
							  FROM dbo.Commission AS  c 
							  WHERE c.ProjectId = f.ProjectId 
									AND c.CommissionType = 1
								),0) ProjectSalesCommisionFraction,
		   (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0) 
			+ ISNULL(f.RecruitingCommissionRate,0)) SLHR,
		   ISNULL(f.PayRate,0) PayRate,
		   f.MLFOverheadRate,
		   f.PersonHoursPerDay,
		   f.PracticeManagementCommissionSub,
		   f.PracticeManagementCommissionOwn ,
		   f.PracticeManagerId,
		   f.PersonId,
		   f.Discount
	FROM v_FinancialsRetrospective f
	WHERE f.ProjectId = @ProjectIdLocal
	),
	MilestoneFinancials as 
	(SELECT f.MilestoneId,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), 1) AS FinancialDate,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), dbo.GetDaysInMonth(MIN(f.Date))) AS MonthEnd,

	       SUM(f.PersonMilestoneDailyAmount) AS Revenue,

	       SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount) AS RevenueNet,
	       
		   SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >= f.PayRate+f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0)) GrossMargin,
		   
		   ISNULL(SUM((CASE WHEN f.SLHR >= f.PayRate + f.MLFOverheadRate THEN f.SLHR ELSE  f.PayRate + f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)),0) Cogs,

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
	 WHERE f.ProjectId = @ProjectIdLocal
	GROUP BY f.ProjectId,f.MilestoneId
	)
	SELECT
		m.MilestoneId,
		m.Description as 'MilestoneName',
		m.IsChargeable,
		m.StartDate,
		m.ProjectedDeliveryDate,
		m.ConsultantsCanAdjust,
		0 as 'ExpectedHours',
		fin.FinancialDate,
		fin.MonthEnd,
		ISNULL(fin.Revenue, 0) as 'Revenue',
		ISNULL(fin.RevenueNet+((fin.ReimbursedExpense) * (1 - fin.Discount/100)),0) as 'RevenueNet',
		fin.Cogs,
		ISNULL((fin.GrossMargin+(fin.ReimbursedExpense* (1 - fin.Discount/100)) - fin.Expense),0)  as 'GrossMargin',
		fin.Hours,
		fin.SalesCommission,
		fin.PracticeManagementCommission,
		fin.Expense,
		fin.ReimbursedExpense,
		case 
			when fin.Revenue <> 0
				then (((fin.Revenue + fin.ReimbursedExpense) * (1 - fin.Discount/100)) - fin.Cogs - fin.Expense) * 100 / fin.Revenue
			else
				0
		end as 'TargetMargin'
	from dbo.Milestone as m
	left join MilestoneFinancials as fin on m.MilestoneId = fin.MilestoneId
	where m.ProjectId = @ProjectIdLocal
END
