CREATE PROCEDURE dbo.FinancialsGetByMilestonePersonEntry
(
	@Id      INT
)
AS
BEGIN
    
	DECLARE @IdLocal INT = @Id

	-- Added @MileStoneId to improve performance of FinancialsRetro CTE
	DECLARE @MileStoneId INT 
	SELECT @MileStoneId = m.MilestoneId
	FROM dbo.MilestonePerson AS mp
	INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId AND mpe.Id = @IdLocal
	INNER JOIN dbo.Milestone AS m ON mp.MilestoneId = m.MilestoneId


	SET NOCOUNT ON;

	;WITH FinancialsRetro AS 
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
		   (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0)) SLHR,
		   ISNULL(f.PayRate,0) PayRate,
		   f.MLFOverheadRate,
		   f.PersonHoursPerDay,
		   f.PracticeManagementCommissionSub,
		   f.PracticeManagementCommissionOwn ,
		   f.PracticeManagerId,
		   f.Discount,
		   f.EntryId
	FROM v_FinancialsRetrospective f
	WHERE f.MilestoneId = @MilestoneId AND f.EntryId = @IdLocal  
	),
	FinancialsRetroByProjectId AS
	(
	SELECT f.ProjectId,
		   MIN(f.Date) AS [Date],

	       ISNULL(SUM(f.PersonMilestoneDailyAmount), 0) AS Revenue,

	       ISNULL(SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount), 0) AS RevenueNet,

		   ISNULL(SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >= f.PayRate + f.MLFOverheadRate 
							  THEN f.SLHR ELSE ISNULL(f.PayRate, 0)+f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0)), 0) GrossMargin,
		   
		   ISNULL(SUM((CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
					 THEN f.SLHR ELSE  f.PayRate +f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)), 0) Cogs,

	       ISNULL(SUM(f.PersonHoursPerDay), 0) AS Hours,
	       
	       (SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
							  THEN f.SLHR  ELSE f.PayRate +f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0))* (f.ProjectSalesCommisionFraction/100))) SalesCommission,

	       SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
	            (CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) * ISNULL(f.PersonHoursPerDay, 0)) *
	           (f.PracticeManagementCommissionSub + CASE f.PracticeManagerId WHEN f.PersonId THEN f.PracticeManagementCommissionOwn ELSE 0 END)) / 100 AS PracticeManagementCommission,
	           0.0 AS 'actualhours',
	           0.0 AS 'forecastedhours'
	  FROM FinancialsRetro AS f
	  GROUP BY f.ProjectId
	  )

	  SELECT ProjectId,
			C.MonthStartDate AS FinancialDate,
			C.MonthEndDate AS MonthEnd,
			Revenue,
			RevenueNet,
			GrossMargin,
			Cogs,
			Hours,
			SalesCommission,
			PracticeManagementCommission,
			actualhours,
			forecastedhours
	  FROM FinancialsRetroByProjectId FRP
	  INNER JOIN dbo.Calendar C ON C.[Date] = FRP.[date]

END
