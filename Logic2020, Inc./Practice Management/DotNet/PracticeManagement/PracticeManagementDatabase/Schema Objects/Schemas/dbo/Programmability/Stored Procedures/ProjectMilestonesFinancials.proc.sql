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
		   --MAX(f.ProjectSalesCommisionFraction) ProjectSalesCommisionFraction,

	     --  (SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						--(CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
						--	  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) 
					 --   *ISNULL(f.PersonHoursPerDay, 0))* (f.ProjectSalesCommisionFraction/100))) SalesCommission,

	       SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
	            (CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) * ISNULL(f.PersonHoursPerDay, 0)) *
	           (f.PracticeManagementCommissionSub + CASE f.PracticeManagerId WHEN f.PersonId THEN f.PracticeManagementCommissionOwn ELSE 0 END)) / 100 AS PracticeManagementCommission
			   --,
		   --min(case when pe.Expense  is null then 0 else pe.Expense  end) as 'Expense',
		   --min(case when pe.ReimbursedExpense is null then 0 else pe.ReimbursedExpense  end) as 'ReimbursedExpense',
		   --min(f.Discount) as Discount
	  FROM FinancialsRetro AS f
	  LEFT JOIN v_MilestoneExpenses as pe on f.ProjectId = pe.ProjectId AND f.MilestoneId = pe.MilestoneId 
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
		ISNULL(Revenue,0) +ISNULL(Me.ReimbursedExpense,0) -ISNULL(me.Expense,0)   as 'Revenue',
		(ISNULL(RevenueNet,0)+(ISNULL(Me.ReimbursedExpense,0) -ISNULL(me.Expense,0))*(1 - p.Discount/100))  as 'RevenueNet',
		ISNULL(Cogs,0) Cogs,
		ISNULL(GrossMargin,0)+(ISNULL(Me.ReimbursedExpense,0) -ISNULL(me.Expense,0))*(1 - p.Discount/100)  as 'GrossMargin',
		fin.Hours,
		ISNULL(GrossMargin,0)+(ISNULL(Me.ReimbursedExpense,0) -ISNULL(me.Expense,0))*(1 - p.Discount/100)
						*  ISNULL((SELECT SUM(c.FractionOfMargin)  FROM dbo.Commission AS  c   WHERE c.ProjectId = P.ProjectId 
									AND c.CommissionType = 1
								),0)*0.01  SalesCommission,
		ISNULL(fin.PracticeManagementCommission,0),
		ISNULL(me.Expense,0) Expense,
		ISNULL(Me.ReimbursedExpense,0) ReimbursedExpense,
		case 
			when ISNULL(Revenue,0) +ISNULL(Me.ReimbursedExpense,0) -ISNULL(me.Expense,0) <> 0
				then (ISNULL(GrossMargin,0)+(ISNULL(Me.ReimbursedExpense,0) -ISNULL(me.Expense,0))*(1 - p.Discount/100))  * 100 / 
				(ISNULL(Revenue,0) +ISNULL(Me.ReimbursedExpense,0) -ISNULL(me.Expense,0))
			else
				0
		end as 'TargetMargin'
	from dbo.Milestone as m
	left join MilestoneFinancials as fin on m.MilestoneId = fin.MilestoneId
	left Join v_MilestoneExpenses ME ON Me.MilestoneId = m.MilestoneId
	left Join Project p on P.ProjectId = m.ProjectId
	where m.ProjectId = @ProjectIdLocal
END
