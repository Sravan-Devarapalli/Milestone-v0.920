-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-11-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	9-24-2008
-- Description:	Selects summary financils for the specified milestone.
-- =============================================
CREATE PROCEDURE dbo.FinancialsGetByMilestone
(
	@MilestoneId      INT
)
AS
	SET NOCOUNT ON;
	DECLARE @MilestoneIdLocal INT
	SELECT @MilestoneIdLocal = @MilestoneId
	;WITH FinancialsRetro AS 
	(
	SELECT f.ProjectId,
		   f.MilestoneId,
		   f.Date, 
		   f.PersonMilestoneDailyAmount,
		   f.PersonDiscountDailyAmount,
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
	WHERE f.MilestoneId = @MilestoneIdLocal
	),
	MilestoneFinancials as 
	(SELECT f.ProjectId,
			f.MilestoneId,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), 1) AS FinancialDate,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), dbo.GetDaysInMonth(MIN(f.Date))) AS MonthEnd,

	       SUM(f.PersonMilestoneDailyAmount) AS Revenue,
		   SUM(f.PersonDiscountDailyAmount) As DiscountAmount,
	       SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount) AS RevenueNet,
	       
		   SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0)) GrossMargin,
		   
		   ISNULL(SUM((CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate THEN f.SLHR ELSE  f.PayRate +f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)),0) Cogs,

	       ISNULL(SUM(f.PersonHoursPerDay), 0) AS Hours, 
		   
	       SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
	            (CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) * ISNULL(f.PersonHoursPerDay, 0)) *
	           (f.PracticeManagementCommissionSub + CASE f.PracticeManagerId WHEN f.PersonId THEN f.PracticeManagementCommissionOwn ELSE 0 END)) / 100 AS PracticeManagementCommission
 
	  FROM FinancialsRetro AS f
	  
	 WHERE f.MilestoneId = @MilestoneIdLocal
	GROUP BY f.ProjectId,f.MilestoneId
	)

	select
		M.ProjectId,
		ISNULL(f.FinancialDate,M.StartDate) FinancialDate,
		ISNULL(f.MonthEnd,M.ProjectedDeliveryDate) MonthEnd,
		(ISNULL(f.Revenue,0) +ISNULL(Me.ReimbursedExpense,0) -ISNULL(ME.Expense,0))   as 'Revenue',
		ISNULL(RevenueNet,0)+((ISNULL(Me.ReimbursedExpense,0) -ISNULL(ME.Expense,0))*(1 - P.Discount/100))  as 'RevenueNet',
		ISNULL(Cogs,0) Cogs,
		ISNULL(GrossMargin,0)+((ISNULL(Me.ReimbursedExpense,0) -ISNULL(ME.Expense,0))*(1 - P.Discount/100)) as 'GrossMargin',
		ISNULL(Hours,0) Hours,
		(ISNULL(GrossMargin,0)+((ISNULL(Me.ReimbursedExpense,0) -ISNULL(ME.Expense,0))*(1 - P.Discount/100)))
			*  ISNULL((SELECT SUM(c.FractionOfMargin)  FROM dbo.Commission AS  c   WHERE c.ProjectId = P.ProjectId 
									AND c.CommissionType = 1
								),0)  *0.01 SalesCommission,
		ISNULL(PracticeManagementCommission,0) PracticeManagementCommission,
		ISNULL(ME.Expense,0) Expense,
		ISNULL(Me.ReimbursedExpense,0) ReimbursedExpense
	FROM Milestone M
	JOIN Project P ON P.ProjectId = M.ProjectId
	LEFT JOIN  MilestoneFinancials f ON f.MilestoneId = M.MilestoneId
	LEFT JOIN v_MilestoneExpenses ME ON Me.MilestoneId = M.MilestoneId
	WHERE M.MilestoneId = @MilestoneIdLocal AND (f.MilestoneId IS NOT NULL OR ME.MilestoneId IS NOT NULL)
	

