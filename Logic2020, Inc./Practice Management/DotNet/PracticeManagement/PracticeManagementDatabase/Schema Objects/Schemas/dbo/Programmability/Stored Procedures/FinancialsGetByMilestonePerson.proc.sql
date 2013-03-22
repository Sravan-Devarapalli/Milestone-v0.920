-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-15-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	9-22-2008
-- Description:	Selects summary financials for the specified milestone-person association.
-- =============================================
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
		   ISNULL((SELECT SUM(c.FractionOfMargin) 
							  FROM dbo.Commission AS  c 
							  WHERE c.ProjectId = f.ProjectId 
									AND c.CommissionType = 1
								),0) ProjectSalesCommisionFraction,
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
	       
	       (SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0))* (f.ProjectSalesCommisionFraction/100))) SalesCommission,
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
			FRP.SalesCommission,
			0.0 AS PracticeManagementCommission,
			FRP.actualhours,
			FRP.forecastedhours
	  FROM FinancialsRetroByProjectId FRP
	  INNER JOIN dbo.Calendar C ON C.[Date] = FRP.FinancialDate 

