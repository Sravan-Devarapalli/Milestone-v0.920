-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-11-2008
-- Updated by:	Sainath
-- Update date:	
-- Description:	Selects financils for the specified project and period grouped by months
-- =============================================
CREATE PROCEDURE [dbo].[FinancialsListByProjectPeriod] 
(
	@ProjectId   VARCHAR(2500),
	@StartDate   DATETIME,
	@EndDate     DATETIME,
	@UseActuals	 BIT = 0
) WITH RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	SET ANSI_WARNINGS OFF

	DECLARE @ProjectIdLocal   VARCHAR(2500),
			@StartDateLocal   DATETIME,
			@EndDateLocal     DATETIME
			

	SELECT @ProjectIdLocal =@ProjectId ,
		   @StartDateLocal=@StartDate ,
		   @EndDateLocal=@EndDate

	DECLARE @ProjectIDs TABLE
	(
		ResultId INT
	)
	DECLARE @Today DATETIME, @CurrentMonthStartDate DATETIME,@InsertingTime DATETIME

	SELECT @Today = CONVERT(DATE, dbo.GettingPMTime(GETUTCDATE())),@InsertingTime = dbo.InsertingTime()
	SELECT @CurrentMonthStartDate = C.MonthStartDate
	FROM dbo.Calendar C
	WHERE C.Date = @Today

	INSERT INTO @ProjectIDs
	SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectIdLocal)

	;WITH FinancialsRetro AS 
	(
	SELECT f.ProjectId,
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
		   f.PersonId,
		   f.Discount,
		   f.IsHourlyAmount,
		   f.BillableHOursPerDay,
  		   f.NonBillableHoursPerDay,
		   f.ActualHoursPerDay,
		   f.BillRate,
		   f.OverheadRate,
		   f.BonusRate,
		   f.VacationRate
	FROM [v_FinancialsRetrospectiveActualHours] f
	WHERE f.ProjectId  IN (SELECT * FROM @ProjectIDs) 
		AND f.Date BETWEEN @StartDateLocal AND @EndDateLocal
	),
	ActualAndProjectedValuesDaily
	AS
	(
	SELECT	f.ProjectId,
			f.PersonId,
			f.Date,
			SUM(f.PersonMilestoneDailyAmount) AS ProjectedRevenueperDay,
			SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount) AS ProjectedRevenueNet,
			SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount - (
																				CASE WHEN f.SLHR >= f.PayRate + f.MLFOverheadRate THEN f.SLHR 
																				ELSE f.PayRate + f.MLFOverheadRate END
																			) * ISNULL(f.PersonHoursPerDay, 0)) AS  ProjectedGrossMargin,
			ISNULL(SUM((CASE WHEN f.SLHR >=  f.PayRate + f.MLFOverheadRate THEN f.SLHR ELSE  f.PayRate + f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)),0) ProjectedCogsperDay,
			SUM(CASE WHEN f.IsHourlyAmount = 0 THEN f.PersonMilestoneDailyAmount ELSE 0 END ) AS FixedActualRevenuePerDay,
			(ISNULL(SUM(f.BillRate* f.ActualHoursPerDay),0) / ISNULL(NULLIF(SUM(f.ActualHoursPerDay),0),1)) * MAX(CASE WHEN f.IsHourlyAmount = 1 THEN  f.BillableHOursPerDay ELSE 0 END) HourlyActualRevenuePerDay,
			SUM(CASE WHEN f.IsHourlyAmount = 0 
					THEN 	f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount - (CASE WHEN f.SLHR >= f.PayRate + f.MLFOverheadRate  THEN f.SLHR ELSE f.PayRate + f.MLFOverheadRate END) * ISNULL(f.PersonHoursPerDay, 0)
					ELSE 0
				END) AS FixedActualMarginPerDay,
				((ISNULL(SUM(f.BillRate* f.ActualHoursPerDay),0) / ISNULL(NULLIF(SUM(f.ActualHoursPerDay),0),1)) * MAX(CASE WHEN f.IsHourlyAmount = 1 THEN  f.BillableHOursPerDay ELSE 0 END))
				 -  (
						MAX( CASE WHEN f.IsHourlyAmount = 1 THEN  f.BillableHOursPerDay + f.NonBillableHoursPerDay ELSE 0 END ) * 
						ISNULL( CASE WHEN ISNULL(SUM(f.ActualHoursPerDay),0) > 0 
									THEN SUM((CASE WHEN f.SLHR >=  f.PayRate + f.MLFOverheadRate THEN f.SLHR ELSE  f.PayRate + f.MLFOverheadRate END)* f.ActualHoursPerDay) / SUM(f.ActualHoursPerDay)
									ELSE SUM((CASE WHEN f.SLHR >=  f.PayRate + f.MLFOverheadRate THEN f.SLHR ELSE  f.PayRate + f.MLFOverheadRate END)) / ISNULL(COUNT(f.PayRate),1)  
								END ,0)
					) AS HourlyActualMarginPerDay,
			ISNULL(SUM(f.PersonHoursPerDay), 0) AS ProjectedHoursPerDay,
			(SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0))* (f.ProjectSalesCommisionFraction/100))) SalesCommission,
			0 AS PracticeManagementCommission,
		  	min(f.Discount) as Discount,
			MIN(CONVERT(INT, f.IsHourlyAmount)) as IsHourlyAmount
	FROM FinancialsRetro AS f
	GROUP BY f.ProjectId, f.PersonId, f.Date
	),
	ProjectExpensesMonthly
	AS
	(
		SELECT pexp.ProjectId,
			CONVERT(DECIMAL(18,2),SUM(pexp.Amount/((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) Expense,
			CONVERT(DECIMAL(18,2),SUM(pexp.Reimbursement*0.01*pexp.Amount /((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) Reimbursement,
			C.MonthStartDate AS FinancialDate,
			C.MonthEndDate AS MonthEnd
		FROM dbo.ProjectExpense as pexp
		JOIN dbo.Calendar c ON c.Date BETWEEN pexp.StartDate AND pexp.EndDate
		WHERE ProjectId IN (SELECT * FROM @ProjectIDs) AND c.Date BETWEEN @StartDateLocal AND @EndDateLocal
		GROUP BY pexp.ProjectId, C.MonthStartDate, C.MonthEndDate
	), 
	ActualAndProjectedValuesMonthly  AS
	(SELECT CT.ProjectId, 
			C.MonthStartDate AS FinancialDate, 
			C.MonthEndDate AS MonthEnd,
			SUM(ISNULL(CT.ProjectedRevenueperDay, 0)) AS ProjectedRevenue,
			SUM(ISNULL(CT.ProjectedRevenueNet, 0)) as ProjectedRevenueNet,
			SUM(ISNULL(CT.ProjectedGrossMargin, 0)) as ProjectedGrossMargin,
			SUM(ISNULL(CT.HourlyActualRevenuePerDay, 0) + ISNULL(CT.FixedActualRevenuePerDay, 0)) AS ActualRevenue,
			SUM(ISNULL(CT.HourlyActualMarginPerDay, 0) + ISNULL(CT.FixedActualMarginPerDay, 0)) as ActualMargin,
			SUM(ISNULL(CT.ProjectedCogsperDay, 0)) as ProjectedCogs,
			MAX(ISNULL(CT.Discount, 0)) Discount,
			SUM(ISNULL(CT.ProjectedHoursPerDay, 0)) as ProjectedHoursPerMonth,
			SUM(ISNULL(CT.SalesCommission, 0)) as SalesCommission,
			SUM(ISNULL(CT.PracticeManagementCommission, 0)) as PracticeManagementCommission
	FROM ActualAndProjectedValuesDaily CT
	INNER JOIN dbo.Calendar C ON C.Date = CT.Date 
	GROUP BY CT.ProjectId, C.MonthStartDate, C.MonthEndDate
	)
	SELECT
		ISNULL(APV.ProjectId,PEM.ProjectId) ProjectId,
		ISNULL(APV.FinancialDate,PEM.FinancialDate) FinancialDate,
		ISNULL(APV.MonthEnd,PEM.MonthEnd) MonthEnd,
		CONVERT(DECIMAL(18,2),ISNULL(APV.ProjectedRevenue,0)) AS 'Revenue',
		CONVERT(DECIMAL(18,2),ISNULL(APV.ProjectedRevenueNet,0))   as 'RevenueNet',
		CONVERT(DECIMAL(18,2),ISNULL(APV.ProjectedCogs,0)) Cogs ,
		CONVERT(DECIMAL(18,2),ISNULL(APV.ProjectedGrossMargin,0)+(ISNULL(PEM.Reimbursement,0)-ISNULL(PEM.Expense,0)) * (1 - ISNULL(APV.Discount,0)/100))  as 'GrossMargin',
		CONVERT(DECIMAL(18,2),ISNULL(APV.ProjectedHoursPerMonth,0)) Hours,
		CONVERT(DECIMAL(18,2),ISNULL(APV.SalesCommission,0)) SalesCommission,
		CONVERT(DECIMAL(18,2),ISNULL(APV.PracticeManagementCommission,0)) PracticeManagementCommission,
		CONVERT(DECIMAL(18,2),ISNULL(PEM.Expense,0)) Expense,
		CONVERT(DECIMAL(18,2),ISNULL(PEM.Reimbursement,0)) ReimbursedExpense,
		CASE WHEN ISNULL(APV.FinancialDate,PEM.FinancialDate) < @CurrentMonthStartDate 
			 THEN CONVERT(DECIMAL(18,6), ISNULL(APV.ActualRevenue,0))
			 ELSE CONVERT(DECIMAL(18,6), ISNULL(APV.ProjectedRevenue,0))
			 END ActualRevenue,
		CASE WHEN ISNULL(APV.FinancialDate,PEM.FinancialDate) < @CurrentMonthStartDate 
			 THEN CONVERT(DECIMAL(18,6), ISNULL(APV.ActualMargin,0) - (ISNULL(APV.ActualRevenue,0) * ISNULL(APV.Discount,0)/100) + ((ISNULL(PEM.Reimbursement,0)-ISNULL(PEM.Expense,0)) * (1 - ISNULL(APV.Discount,0)/100)))
			 ELSE CONVERT(DECIMAL(18,6), ISNULL(APV.ProjectedGrossMargin,0) + (ISNULL(PEM.Reimbursement,0)-ISNULL(PEM.Expense,0)) * (1 - ISNULL(APV.Discount,0)/100))
			 END ActualGrossMargin,
		CONVERT(BIT,1) AS IsMonthlyRecord,
		@InsertingTime AS  CreatedDate,	
		CONVERT(DATE,@InsertingTime)  AS CacheDate
	FROM ActualAndProjectedValuesMonthly APV
	FULL JOIN ProjectExpensesMonthly PEM 
	ON PEM.ProjectId = APV.ProjectId AND APV.FinancialDate = PEM.FinancialDate  AND APV.MonthEnd = PEM.MonthEnd

END

