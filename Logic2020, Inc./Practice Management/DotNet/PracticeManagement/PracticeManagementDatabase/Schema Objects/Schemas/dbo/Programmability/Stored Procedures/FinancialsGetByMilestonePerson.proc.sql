CREATE PROCEDURE [dbo].[FinancialsGetByMilestonePerson]
(
	@MilestoneId      INT,
	@PersonId         INT
)
AS
	SET NOCOUNT ON

	;WITH FinancialsRetro AS 
	(
	SELECT f.ProjectId,
		   f.PersonId,
		   f.Date, 
		   f.PersonMilestoneDailyAmount,
		   f.PersonDiscountDailyAmount,
		   (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0)) SLHR,
		   ISNULL(f.PayRate,0) PayRate,
		   f.MLFOverheadRate,
		   f.PersonHoursPerDay
	FROM v_FinancialsRetrospective f
	WHERE f.MilestoneId = @MilestoneId AND f.PersonId = @PersonId         
	),
	FinancialsRetroByProjectId AS
	(
	SELECT f.ProjectId,
	       MIN(f.Date) AS FinancialDate,
	       ISNULL(SUM(f.PersonMilestoneDailyAmount), 0)AS Revenue,

	       ISNULL(SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount), 0) AS RevenueNet,

		   ISNULL(SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >= f.PayRate+f.MLFOverheadRate 
							  THEN f.SLHR ELSE  f.PayRate +f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0)), 0) GrossMargin,
		   
		   ISNULL(SUM((CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
					 THEN f.SLHR ELSE  f.PayRate +f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)), 0) Cogs,

	       ISNULL(SUM(f.PersonHoursPerDay), 0) AS Hours,
	           0.0 AS 'actualhours',
	           0.0 AS 'forecastedhours'
	  FROM FinancialsRetro AS f
	  GROUP BY f.ProjectId
	  )

	  SELECT FRP.ProjectId,
			C.MonthStartDate AS FinancialDate,
			FRP.Revenue,
			FRP.RevenueNet,
			FRP.GrossMargin,
			FRP.Cogs,
			FRP.Hours,
			FRP.actualhours,
			FRP.forecastedhours
	  FROM FinancialsRetroByProjectId FRP
	  INNER JOIN dbo.Calendar C ON C.[Date] = FRP.FinancialDate 


