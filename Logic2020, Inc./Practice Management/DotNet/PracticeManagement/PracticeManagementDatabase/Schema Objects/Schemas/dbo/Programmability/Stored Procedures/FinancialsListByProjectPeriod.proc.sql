-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-11-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	9-18-2008
-- Description:	Selects financils for the specified project and period grouped by months
-- =============================================
CREATE PROCEDURE [dbo].[FinancialsListByProjectPeriod]
(
	@ProjectId   VARCHAR(2500),
	@StartDate   DATETIME,
	@EndDate     DATETIME
)
AS
	SET NOCOUNT ON
	
	DECLARE @ProjectIDs TABLE
	(
		ResultId INT
	)
	
	INSERT INTO @ProjectIDs
	SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectId)

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
	WHERE f.ProjectId  IN (SELECT * FROM @ProjectIDs) 
	),
	ProjectFinancials
	AS
	(
	SELECT f.ProjectId,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), 1) AS FinancialDate,
	       dbo.MakeDate(YEAR(MIN(f.Date)), MONTH(MIN(f.Date)), dbo.GetDaysInMonth(MIN(f.Date))) AS MonthEnd,

	       SUM(f.PersonMilestoneDailyAmount) AS Revenue,

	       SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount) AS RevenueNet,
	       
		   SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >= f.PayRate + f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate + f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0)) GrossMargin,
		   
		   ISNULL(SUM((CASE WHEN f.SLHR >=  f.PayRate + f.MLFOverheadRate THEN f.SLHR ELSE  f.PayRate + f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)),0) Cogs,
	       ISNULL(SUM(f.PersonHoursPerDay), 0) AS Hours,
	       
	       (SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
						(CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) 
					    *ISNULL(f.PersonHoursPerDay, 0))* (f.ProjectSalesCommisionFraction/100))) SalesCommission,

	       SUM((f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount -
	            (CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
							  THEN f.SLHR ELSE f.PayRate +f.MLFOverheadRate END) * ISNULL(f.PersonHoursPerDay, 0)) *
	           (f.PracticeManagementCommissionSub + CASE f.PracticeManagerId WHEN f.PersonId THEN f.PracticeManagementCommissionOwn ELSE 0 END)) / 100 AS PracticeManagementCommission,
		  		   min(f.Discount) as Discount
	  FROM FinancialsRetro AS f
	  WHERE  
		f.ProjectId IN (SELECT * FROM @ProjectIDs) 
		AND f.Date BETWEEN @StartDate AND @EndDate
	GROUP BY f.ProjectId, YEAR(f.Date), MONTH(f.Date)
	),
	ProjectExpensesMonthly
	AS
	(
		SELECT pexp.ProjectId,
			CONVERT(DECIMAL(18,2),SUM(pexp.Amount/((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) Expense,
			CONVERT(DECIMAL(18,2),SUM(pexp.Reimbursement*0.01*pexp.Amount /((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) Reimbursement,
			dbo.MakeDate(YEAR(MIN(c.Date)), MONTH(MIN(c.Date)), 1) AS FinancialDate,
	       dbo.MakeDate(YEAR(MIN(c.Date)), MONTH(MIN(c.Date)), dbo.GetDaysInMonth(MIN(C.Date))) AS MonthEnd
		FROM dbo.ProjectExpense as pexp
		JOIN dbo.Calendar c ON c.Date BETWEEN pexp.StartDate AND pexp.EndDate
		WHERE ProjectId IN (SELECT * FROM @ProjectIDs) AND c.Date BETWEEN @StartDate	AND @EndDate
		GROUP BY pexp.ProjectId,MONTH(c.Date),YEAR(c.Date)
	)
	
	SELECT
		ISNULL(pf.ProjectId,PEM.ProjectId) ProjectId,
		ISNULL(pf.FinancialDate,PEM.FinancialDate) FinancialDate,
		ISNULL(pf.MonthEnd,PEM.MonthEnd) MonthEnd,
		ISNULL(pf.Revenue,0)+ISNULL(PEM.Reimbursement,0)-ISNULL(PEM.Expense,0) AS 'Revenue',
		ISNULL(pf.RevenueNet,0)+ISNULL(PEM.Reimbursement,0)-ISNULL(PEM.Expense,0)   as 'RevenueNet',
		ISNULL(pf.Cogs,0) Cogs ,
		ISNULL(pf.GrossMargin,0)+(ISNULL(PEM.Reimbursement,0)-ISNULL(PEM.Expense,0))* (1 - ISNULL(pf.Discount,0)/100)  as 'GrossMargin',
		ISNULL(pf.Hours,0) Hours,
		ISNULL(pf.SalesCommission,0) SalesCommission,
		ISNULL(pf.PracticeManagementCommission,0) PracticeManagementCommission,
		ISNULL(PEM.Expense,0) Expense,
		ISNULL(PEM.Reimbursement,0) ReimbursedExpense
	FROM ProjectFinancials pf
	FULL JOIN ProjectExpensesMonthly PEM 
	ON PEM.ProjectId = pf.ProjectId AND pf.FinancialDate = PEM.FinancialDate  AND Pf.MonthEnd = PEM.MonthEnd
	

