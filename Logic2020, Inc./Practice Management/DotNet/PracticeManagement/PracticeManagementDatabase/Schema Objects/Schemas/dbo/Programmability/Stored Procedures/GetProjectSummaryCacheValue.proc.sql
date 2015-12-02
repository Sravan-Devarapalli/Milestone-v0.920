CREATE PROCEDURE [dbo].[GetProjectSummaryCacheValue]
(
	@ProjectId   NVARCHAR(MAX),
	@StartDate   DATETIME = NULL,
	@EndDate     DATETIME = NULL,
	@IsMonthlyReport BIT = 0 ,
	@IsAttainmentReport  BIT = 0 
)
AS
BEGIN
	DECLARE @InsertingTime DATETIME
	SELECT	@InsertingTime = MAX(CacheDate) FROM ProjectSummaryCache
	DECLARE @ProjectIDs TABLE
	(
		ResultId INT
	)
	INSERT INTO @ProjectIDs
	SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectId)

	SELECT [ProjectId]
		  ,[MonthStartDate] AS FinancialDate
		  ,[MonthEndDate] AS MonthEnd
		  ,RangeType 
		  ,[ProjectRevenue] AS Revenue
		  ,[ProjectRevenueNet] AS RevenueNet
		  ,[Cogs]
		  ,[GrossMargin]
		  ,[ProjectedhoursperMonth] AS Hours
		  ,[Expense]
		  ,[ReimbursedExpense]
		  ,[ActualRevenue]
		  ,[ActualGrossMargin]
		  ,[PreviousMonthActualRevenue]
		  ,[PreviousMonthActualGrossMargin]
	FROM [dbo].[ProjectSummaryCache]
	WHERE CacheDate = CONVERT(DATE ,@InsertingTime )
			AND (
					(@StartDate IS NULL AND @EndDate IS NULL) 
					OR  
					([MonthStartDate] BETWEEN @StartDate AND @EndDate)
				) 
			AND [ProjectId] IN (SELECT * FROM @ProjectIDs)
			AND IsMonthlyRecord = @IsMonthlyReport
			AND ((IsMonthlyRecord = 1 AND @IsAttainmentReport = 0 AND RangeType LIKE 'M%') OR IsMonthlyRecord = 0 OR @IsAttainmentReport = 1)
END

