CREATE PROCEDURE [dbo].[AttainmentFinancialListByProject]
(
	@ProjectId   VARCHAR(2500),
	@StartDate   DATETIME,
	@EndDate     DATETIME,
	@CalculateMonthValues BIT = 0,
	@CalculateQuarterValues BIT = 0,
	@CalculateYearToDateValues BIT = 0,
	@IsSummaryCache BIT = 0
) WITH RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	SET ANSI_WARNINGS OFF
	/*Quarters StartDate and Quarters EndDate is calculated based on following ways(This is to calculate only for Quarters)
	If it is for summary cache i.e.@IsSummaryCache=1 then QuartersStartDate and QuartersEndDate will be equal to provided StartDate and EndDate
	Otherwise then we check any CurrentYearDate overlaps with StartDate and EndDate 
	   If any CurrentYearDate overlaps then QuartersStartDate = CurrentYearStartDate (Ex:1/1/2013) and QuartersEndDate = CurrentYearLastDate based on EndDate value(Ex:If EndDate=4/4/2014 then QuartersEndDate=12/31/2013,If EndDate=4/4/2013 then QuartersEndDate=4/4/2013)
	   Else We will find QuartersStartDate,QuartersEndDate for the StartDate year.
                i.e. QuartersStartDate = YearStartDate of StartDate year (Ex:StartDate = 14/4/2012 then QuartersStartDate = 1/1/2012)
		 QuartersEndDate =  If the StartDate Year = EndDate Year then EndDate will be QuartersEndDate otherwise StartDate YearLastDate (Ex:14/4/2012 then QuartersEndDate=12/31/2012)
	*/
	DECLARE @ProjectIdLocal   VARCHAR(2500),
			@StartDateLocal   DATETIME,
			@EndDateLocal     DATETIME,
			@CurrentYearStartDate DATETIME,
			@CurrentYearEndDate DATETIME,
			@QuartersStarDate DATETIME,
			@QuartersEndDate DATETIME
			

	SELECT @ProjectIdLocal =@ProjectId ,
		   @StartDateLocal=@StartDate ,
		   @EndDateLocal=@EndDate,
		   @CurrentYearStartDate = DATEADD(YEAR, DATEDIFF(YEAR, 0, GETUTCDATE()), 0),
		   @CurrentYearEndDate = DATEADD(MILLISECOND, -3,DATEADD(YEAR, DATEDIFF(YEAR, 0, GETUTCDATE()) + 1, 0))

	IF @IsSummaryCache = 1
	BEGIN
	 SET @QuartersStarDate = @StartDateLocal
	 SET @QuartersEndDate = @EndDateLocal
	END	
    ELSE IF @StartDateLocal <= @CurrentYearEndDate AND @CurrentYearStartDate <= @EndDateLocal
    BEGIN
	  SET	@QuartersStarDate = @CurrentYearStartDate
	  SET  @QuartersEndDate = CASE WHEN @CurrentYearEndDate < @EndDateLocal THEN @CurrentYearEndDate ELSE @EndDateLocal END
    END
    ELSE 
    BEGIN
	 SET  @QuartersStarDate = DATEADD(YEAR, DATEDIFF(YEAR, 0, @StartDateLocal), 0) 
	 SET  @QuartersEndDate = CASE WHEN DATEPART(YEAR,@StartDateLocal) = DATEPART(YEAR,@EndDateLocal) THEN @EndDateLocal
								  ELSE DATEADD(MILLISECOND, -3,DATEADD(YEAR, DATEDIFF(YEAR, 0, @StartDateLocal) + 1, 0)) END
	 
    END

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
	
	DECLARE @Ranges TABLE (StartDate DATETIME,EndDate DATETIME,RangeType NVARCHAR(11),ColOrder INT)
	IF(@CalculateMonthValues = 1)
	BEGIN
	INSERT INTO @Ranges 
	SELECT	MonthStartDate AS StartDate,
			MonthEndDate AS EndDate,
			'M'+CONVERT(NVARCHAR,MonthNumber) AS RangeType,
			DATEPART(MM,MonthStartDate) AS ColOrder
	FROM dbo.Calendar C
	WHERE C.DATE between @StartDateLocal and @EndDateLocal
	GROUP BY C.MonthStartDate,C.MonthEndDate,C.MonthNumber
	END
	
	IF (@CalculateQuarterValues = 1)
	BEGIN
		INSERT INTO @Ranges 
		SELECT QuarterStartDate,CASE WHEN @QuartersEndDate < QuarterEndDate THEN @QuartersEndDate ELSE QuarterEndDate END,'Q'+CONVERT(NVARCHAR,DATEPART(Q,QuarterStartDate)),DATEPART(Q,QuarterStartDate)+12 FROM dbo.Calendar C
		WHERE C.DATE between @QuartersStarDate and @QuartersEndDate 
		GROUP BY C.QuarterStartDate,C.QuarterEndDate
	END

	IF (@CalculateYearToDateValues = 1 AND @IsSummaryCache = 0)
	BEGIN
	
	IF @StartDateLocal <= @CurrentYearEndDate AND @CurrentYearStartDate <= @EndDateLocal
    BEGIN
		INSERT INTO @Ranges 
		SELECT @QuartersStarDate ,DATEADD(dd,-1,@Today),'YTD',17
		END
		ELSE 
		BEGIN
		INSERT INTO @Ranges 
		SELECT @QuartersStarDate ,@QuartersEndDate,'YTD',17
		END
	END
	ELSE IF (@CalculateYearToDateValues = 1 AND @IsSummaryCache = 1)
	BEGIN 
		INSERT INTO @Ranges 
		SELECT CONVERT(NVARCHAR(4),C.Year) + '0101' AS QuarterStartDate,
		CASE WHEN C.Year = YEAR(@Today) THEN DATEADD(dd,-1,@Today) ELSE CONVERT(NVARCHAR(4),C.Year) + '1231' END QuarterEndDate ,'YTD',17
		FROM dbo.Calendar C
		WHERE C.DATE between @QuartersStarDate and @QuartersEndDate 
		GROUP BY C.Year
	END
	
	SELECT @StartDateLocal = MIN(StartDate),@EndDateLocal = MAX(EndDate) 
	FROM @Ranges

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
			(ISNULL(SUM(CASE WHEN f.IsHourlyAmount = 1 THEN f.BillRate* f.ActualHoursPerDay ELSE 0 END),0) / ISNULL(NULLIF(SUM(CASE WHEN f.IsHourlyAmount = 1 THEN f.ActualHoursPerDay ELSE 0 END),0),1)) * MAX(CASE WHEN f.IsHourlyAmount = 1 THEN  f.BillableHOursPerDay ELSE 0 END) HourlyActualRevenuePerDay,
			SUM(CASE WHEN f.IsHourlyAmount = 0 
					THEN 	f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount - (CASE WHEN f.SLHR >= f.PayRate + f.MLFOverheadRate  THEN f.SLHR ELSE f.PayRate + f.MLFOverheadRate END) * ISNULL(f.PersonHoursPerDay, 0)
					ELSE 0
				END) AS FixedActualMarginPerDay,
				((ISNULL(SUM(CASE WHEN f.IsHourlyAmount = 1 THEN f.BillRate* f.ActualHoursPerDay ELSE 0 END),0) / ISNULL(NULLIF(SUM(CASE WHEN f.IsHourlyAmount = 1 THEN f.ActualHoursPerDay ELSE 0 END),0),1)) * MAX(CASE WHEN f.IsHourlyAmount = 1 THEN  f.BillableHOursPerDay ELSE 0 END))
				 -  (
						MAX( CASE WHEN f.IsHourlyAmount = 1 THEN  f.BillableHOursPerDay + f.NonBillableHoursPerDay ELSE 0 END ) * 
						ISNULL( CASE WHEN ISNULL(SUM(CASE WHEN f.IsHourlyAmount = 1 THEN f.ActualHoursPerDay ELSE 0 END),0) > 0 
									THEN SUM((CASE WHEN f.SLHR >=  f.PayRate + f.MLFOverheadRate THEN f.SLHR ELSE  f.PayRate + f.MLFOverheadRate END)* CASE WHEN f.IsHourlyAmount = 1 THEN f.ActualHoursPerDay ELSE 0 END) / SUM(CASE WHEN f.IsHourlyAmount = 1 THEN f.ActualHoursPerDay ELSE 0 END)
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
			R.StartDate AS FinancialDate,
			R.EndDate AS MonthEnd,
			R.RangeType
		FROM dbo.ProjectExpense as pexp
		INNER JOIN dbo.Calendar cal ON cal.Date BETWEEN pexp.StartDate  AND pexp.EndDate
		INNER JOIN @Ranges R ON cal.Date BETWEEN R.StartDate AND R.EndDate
		WHERE ProjectId IN (SELECT * FROM @ProjectIDs)
		GROUP BY pexp.ProjectId, R.StartDate, R.EndDate,R.RangeType
	), 
	ActualAndProjectedValuesMonthly  AS
	(SELECT CT.ProjectId, 
			C.StartDate AS FinancialDate, 
			C.EndDate AS MonthEnd,
			C.RangeType,
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
	INNER JOIN @Ranges C ON CT.Date BETWEEN C.StartDate AND C.EndDate
	GROUP BY CT.ProjectId, C.StartDate, C.EndDate,C.RangeType
	)
	SELECT
		ISNULL(APV.ProjectId,PEM.ProjectId) ProjectId,
		ISNULL(APV.FinancialDate,PEM.FinancialDate) AS FinancialDate,
		ISNULL(APV.MonthEnd,PEM.MonthEnd) MonthEnd,
		ISNULL(APV.RangeType,PEM.RangeType) RangeType,
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
	ORDER BY ProjectId
END

