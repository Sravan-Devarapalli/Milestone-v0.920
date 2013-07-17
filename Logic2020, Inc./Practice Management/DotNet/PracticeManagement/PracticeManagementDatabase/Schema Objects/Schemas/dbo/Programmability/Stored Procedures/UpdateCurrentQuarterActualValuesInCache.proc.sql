CREATE PROCEDURE [dbo].[UpdateCurrentQuarterActualValuesInCache]
AS
BEGIN
	DECLARE @InsertingTime DATETIME,
			@CurrentQuartersStartDate DATETIME,
			@CurrentQuartersEndDate DATETIME,
			@CurrentYearStartDate DATETIME,
			@CurrentMonthEndDate DATETIME,
			@Today DATETIME

	SELECT	@Today = CONVERT(DATE, dbo.GettingPMTime(GETUTCDATE())),
			@InsertingTime = dbo.InsertingTime(),
			@CurrentQuartersStartDate = DATEADD(qq, DATEDIFF(qq, 0, @Today), 0),
			@CurrentQuartersEndDate = DATEADD(d, -1, DATEADD(qq, 1, DATEADD(qq, DATEDIFF(qq, 0, @Today), 0))),
			@CurrentYearStartDate =  DATEADD(YEAR, DATEDIFF(YEAR, 0, GETUTCDATE()), 0)
	SELECT  @CurrentMonthEndDate = C.MonthEndDate
	FROM dbo.Calendar C
	WHERE C.Date = @Today

	--To Update the Current quarter value actual values 
	;WITH CTE
	AS

	(
		SELECT projectid,SUM(ActualRevenue) AS ActualRevenue,SUM(ActualGrossMargin) AS ActualGrossMargin
		FROM ProjectSummaryCache
		Where CacheDate = CONVERT(DATE ,@InsertingTime) 
			  AND [RangeType] LIKE 'M%' 
			  AND [MonthStartDate] BETWEEN @CurrentQuartersStartDate AND @CurrentQuartersEndDate		  
		GROUP BY projectid
	)
	UPDATE qrt
	SET qrt.ActualRevenue = mon.ActualRevenue,
		qrt.ActualGrossMargin = mon.ActualGrossMargin
	--	SELECT qrt.projectid,qrt.ActualRevenue,mon.ActualRevenue,qrt.ActualGrossMargin,mon.ActualGrossMargin
	FROM ProjectSummaryCache qrt
	INNER JOIN CTE mon ON qrt.projectid = mon.projectid
	WHERE qrt.CacheDate = CONVERT(DATE ,@InsertingTime) 
		  AND qrt.[RangeType] LIKE 'Q%' 
		  AND qrt.[MonthStartDate] = @CurrentQuartersStartDate
		  AND qrt.[MonthEndDate] = @CurrentQuartersEndDate

--To Update the Current year YTD value actual values 	
	;WITH CTE
	AS
	(
		SELECT projectid,SUM(ActualRevenue) AS ActualRevenue,SUM(ActualGrossMargin) AS ActualGrossMargin
		FROM ProjectSummaryCache
		Where CacheDate = CONVERT(DATE ,@InsertingTime) 
			  AND [RangeType] LIKE 'M%' 
			  AND [MonthStartDate] BETWEEN @CurrentYearStartDate AND @CurrentMonthEndDate		  
		GROUP BY projectid
	)
	UPDATE qrt
	SET qrt.ActualRevenue = mon.ActualRevenue,
		qrt.ActualGrossMargin = mon.ActualGrossMargin
		--SELECT qrt.projectid,qrt.ActualRevenue,mon.ActualRevenue,qrt.ActualGrossMargin,mon.ActualGrossMargin
	FROM ProjectSummaryCache qrt
	INNER JOIN CTE mon ON qrt.projectid = mon.projectid
	WHERE qrt.CacheDate = CONVERT(DATE ,@InsertingTime) 
		  AND qrt.[RangeType] = 'YTD' 
		  AND qrt.[MonthStartDate] = @CurrentYearStartDate
		  AND qrt.[MonthEndDate] = @CurrentMonthEndDate
END
