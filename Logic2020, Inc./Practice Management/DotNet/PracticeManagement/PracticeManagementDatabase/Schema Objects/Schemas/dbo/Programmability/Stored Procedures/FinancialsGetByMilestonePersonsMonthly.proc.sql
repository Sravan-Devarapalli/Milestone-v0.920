CREATE PROCEDURE dbo.FinancialsGetByMilestonePersonsMonthly
(
	@MilestoneId      INT
)
AS
	SET NOCOUNT ON

	;WITH FinancialsRetro AS 
	(
	SELECT f.ProjectId,
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
	WHERE f.MilestoneId = @MilestoneId
	)
	SELECT f.ProjectId,
		   f.PersonId,
		   MPE.StartDate,
		   MPE.EndDate,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), 1) AS FinancialDate,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), dbo.GetDaysInMonth(MIN(f.Date))) AS MonthEnd,

	       ISNULL(SUM(f.PersonMilestoneDailyAmount), 0) AS Revenue,

	      ISNULL(SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount), 0) AS RevenueNet,
	       

	       ISNULL(SUM((CASE WHEN f.SLHR+f.SCPH >= f.PayRate + f.MLFOverheadRate THEN f.SLHR+f.SCPH ELSE  f.PayRate + f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)),0) Cogs,
	       
	       ISNULL(SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR+f.SCPH >= f.PayRate+f.MLFOverheadRate 
							  THEN f.SLHR ELSE  f.PayRate +f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0)), 0) GrossMargin,


	       ISNULL(SUM(f.PersonHoursPerDay), 0) AS Hours,

	       (SUM(
				 CASE WHEN f.SLHR+f.SCPH >=  f.PayRate +f.MLFOverheadRate THEN f.SCPH * ISNULL(f.PersonHoursPerDay, 0)
				    ELSE (f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount-((f.PayRate + f.MLFOverheadRate)*ISNULL(f.PersonHoursPerDay, 0)))
						* (f.ProjectSalesCommisionFraction/100)
					END
				 )) SalesCommission,

	       SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
	            (f.SLHR) * ISNULL(f.PersonHoursPerDay, 0)) *
	           (f.PracticeManagementCommissionSub + CASE f.PracticeManagerId WHEN f.PersonId THEN f.PracticeManagementCommissionOwn ELSE 0 END)) / 100 AS PracticeManagementCommission
	  FROM FinancialsRetro AS f
	  JOIN MilestonePerson MP ON MP.MilestoneId = f.MilestoneId AND MP.PersonId = f.PersonId
	  JOIN MilestonePersonEntry MPE ON MP.MilestonePersonId = MPE.MilestonePersonId AND f.Date BETWEEN MPE.StartDate AND MPE.EndDate
	 WHERE f.MilestoneId = @MilestoneId AND f.PersonId IS NOT NULL--AND f.PersonId = @PersonId AND f.EntryStartDate = @EntryStartDate
	GROUP BY f.ProjectId, f.PersonId, MPE.StartDate, MPE.EndDate, YEAR(f.Date), MONTH(f.Date)

