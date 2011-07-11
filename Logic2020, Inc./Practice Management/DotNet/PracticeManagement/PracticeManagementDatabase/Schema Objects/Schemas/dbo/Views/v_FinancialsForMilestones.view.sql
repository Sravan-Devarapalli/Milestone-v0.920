CREATE VIEW [dbo].[v_FinancialsForMilestones]

AS 
	WITH FinancialsRetro AS 
	(
	SELECT f.ProjectId,
		   f.PersonId,
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
		   f.PayRate,
		   f.MLFOverheadRate,
		   f.PersonHoursPerDay,
		   f.PracticeManagementCommissionSub,
		   f.PracticeManagementCommissionOwn,
		   f.PracticeManagerId,
		   f.Discount
	FROM v_FinancialsRetrospective f
	),
	MilestoneFinancials AS
	(
	SELECT f.ProjectId,
		   f.MilestoneId,
		   f.PersonId,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), 1) AS FinancialDate,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), dbo.GetDaysInMonth(MIN(f.Date))) AS MonthEnd,

	       SUM(f.PersonMilestoneDailyAmount) AS Revenue,

	       SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount) AS RevenueNet,

		   SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR  >= ISNULL(f.PayRate, 0)+f.MLFOverheadRate 
							  THEN f.SLHR ELSE ISNULL(f.PayRate, 0)+f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0)) GrossMargin,
		   
		   ISNULL(SUM((CASE WHEN f.SLHR >= f.PayRate + f.MLFOverheadRate THEN f.SLHR  ELSE  f.PayRate + f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)),0) Cogs,

	       ISNULL(SUM(f.PersonHoursPerDay), 0) AS Hours,
	       MAX(f.ProjectSalesCommisionFraction) ProjectSalesCommisionFraction,
	       SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
	            (CASE WHEN f.SLHR  >=  f.PayRate +f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END)  * ISNULL(f.PersonHoursPerDay, 0)) *
	           (f.PracticeManagementCommissionSub + CASE f.PracticeManagerId WHEN f.PersonId THEN f.PracticeManagementCommissionOwn ELSE 0 END)) / 100 AS PracticeManagementCommission,
	           0.0 AS 'actualhours',
	           0.0 AS 'forecastedhours',
           ISNULL(SUM(vac.VacationHours), 0) AS 'VacationHours',
            min(f.Discount) Discount
		    
	  FROM FinancialsRetro AS f
	  LEFT JOIN dbo.v_MilestonePersonVacations AS vac ON f.MilestoneId = vac.MilestoneId AND f.PersonId = vac.PersonId
	  GROUP BY f.PersonId, f.MilestoneId, f.ProjectId
	  ) 
	
	SELECT  
			f.ProjectId,
			f.MilestoneId,
			PersonId,
			FinancialDate,
			MonthEnd,
			ISNULL(Revenue,0)+ISNULL(ReimbursedExpense,0)-ISNULL(Expense,0) as Revenue, 
			ISNULL(RevenueNet,0)+((ISNULL(ReimbursedExpense,0)-ISNULL(Expense,0))*(1 - Discount/100)) RevenueNet,
			ISNULL(GrossMargin + ((ISNULL(ReimbursedExpense,0) - ISNULL(Expense,0))*(1 - Discount/100)),0) as GrossMargin,
			Cogs, 
			[Hours],
			ISNULL(GrossMargin + ((ISNULL(ReimbursedExpense,0) - ISNULL(Expense,0))*(1 - Discount/100)),0) 
			* ProjectSalesCommisionFraction*0.01 SalesCommission,
			PracticeManagementCommission,
			actualhours ,
			forecastedhours,
			VacationHours
	  FROM MilestoneFinancials AS f
	  LEFT JOIN v_MilestoneExpenses as me on f.ProjectId = me.ProjectId AND f.MilestoneId = me.milestoneId
