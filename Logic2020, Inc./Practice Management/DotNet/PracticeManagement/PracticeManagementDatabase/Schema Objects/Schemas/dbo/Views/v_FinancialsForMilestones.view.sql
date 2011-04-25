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
			+ ISNULL(f.RecruitingCommissionRate,0)) SLHR,
		   f.PayRate,
		   f.MLFOverheadRate,
		   f.PersonHoursPerDay,
		   f.PracticeManagementCommissionSub,
		   f.PracticeManagementCommissionOwn ,
		   f.PracticeManagerId,
		   f.Discount
	FROM v_FinancialsRetrospective f
	) 
	
	SELECT f.ProjectId,
		   f.MilestoneId,
		   f.PersonId,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), 1) AS FinancialDate,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), dbo.GetDaysInMonth(MIN(f.Date))) AS MonthEnd,

	       SUM(f.PersonMilestoneDailyAmount) AS Revenue,

	       SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount) AS RevenueNet,

		   SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR+f.SCPH >= ISNULL(f.PayRate, 0)+f.MLFOverheadRate 
							  THEN f.SLHR ELSE ISNULL(f.PayRate, 0)+f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0)) GrossMargin,
		   
		   ISNULL(SUM((CASE WHEN f.SLHR+f.SCPH >= f.PayRate + f.MLFOverheadRate THEN f.SLHR+f.SCPH ELSE  f.PayRate + f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)),0) Cogs,

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
	           0.0 AS 'actualhours',
	           0.0 AS 'forecastedhours',
           ISNULL(SUM(vac.VacationHours), 0) AS 'VacationHours'
		   
	  FROM FinancialsRetro AS f
	  LEFT JOIN dbo.v_MilestonePersonVacations AS vac ON f.MilestoneId = vac.MilestoneId AND f.PersonId = vac.PersonId
	  GROUP BY f.PersonId, f.MilestoneId, f.ProjectId
