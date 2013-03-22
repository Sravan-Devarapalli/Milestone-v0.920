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
		   (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0)) SLHR,
		   ISNULL(f.PayRate,0) PayRate,
		   f.MLFOverheadRate,
		   f.PersonHoursPerDay,
		   f.PersonId,
		   f.Discount
	FROM v_FinancialsRetrospective f
	WHERE f.ProjectId = @ProjectIdLocal
	),
	MilestoneFinancials as 
	(SELECT f.MilestoneId,
	       MIN(C.MonthStartDate) AS FinancialDate,
	       MIN(C.MonthEndDate) AS MonthEnd,

	       SUM(f.PersonMilestoneDailyAmount) AS Revenue,

	       SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount) AS RevenueNet,
	       
		   SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >= f.PayRate+f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0)) GrossMargin,
		   
		   ISNULL(SUM((CASE WHEN f.SLHR >= f.PayRate + f.MLFOverheadRate THEN f.SLHR ELSE  f.PayRate + f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)),0) Cogs,

	       ISNULL(SUM(f.PersonHoursPerDay), 0) AS Hours
		 
	  FROM FinancialsRetro AS f
	  INNER JOIN dbo.Calendar C ON C.Date = f.Date
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
		0 as 'ExpectedHours',
		fin.FinancialDate,
		fin.MonthEnd,
		ISNULL(Revenue,0) as 'Revenue',
		ISNULL(RevenueNet,0) as 'RevenueNet',
		ISNULL(Cogs,0) Cogs,
		ISNULL(GrossMargin,0)+(ISNULL(Me.ReimbursedExpense,0) -ISNULL(me.Expense,0))*(1 - p.Discount/100)  as 'GrossMargin',
		fin.Hours,
		ISNULL(GrossMargin,0)+(ISNULL(Me.ReimbursedExpense,0) -ISNULL(me.Expense,0))*(1 - p.Discount/100)
						*  ISNULL((SELECT SUM(c.FractionOfMargin)  FROM dbo.Commission AS  c   WHERE c.ProjectId = P.ProjectId 
									AND c.CommissionType = 1
								),0)*0.01  SalesCommission,
		0.0 AS PracticeManagementCommission,
		ISNULL(me.Expense,0) Expense,
		ISNULL(Me.ReimbursedExpense,0) ReimbursedExpense,
		case 
			when ISNULL(Revenue,0)  <> 0
				then (ISNULL(GrossMargin,0)+(ISNULL(Me.ReimbursedExpense,0) -ISNULL(me.Expense,0))*(1 - p.Discount/100))  * 100 / 
				ISNULL(Revenue,0)
			else
				0
		end as 'TargetMargin'
	from dbo.Milestone as m
	left join MilestoneFinancials as fin on m.MilestoneId = fin.MilestoneId
	left Join v_MilestoneExpenses ME ON Me.MilestoneId = m.MilestoneId
	left Join Project p on P.ProjectId = m.ProjectId
	where m.ProjectId = @ProjectIdLocal
END

