CREATE PROCEDURE [dbo].[GetProjectSummaryCacheValue]
(
	@ProjectId   VARCHAR(2500),
	@StartDate   DATETIME = NULL,
	@EndDate     DATETIME = NULL,
	@IsMonthlyReport BIT = 0 
)
AS
BEGIN
	DECLARE @InsertingTime DATETIME
	SELECT	@InsertingTime = CONVERT(DATE, dbo.GettingPMTime(GETUTCDATE()))
	DECLARE @ProjectIDs TABLE
	(
		ResultId INT
	)
	INSERT INTO @ProjectIDs
	SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectId)

	SELECT [ProjectId]
		  ,[MonthStartDate] AS FinancialDate
		  ,[MonthEndDate] AS MonthEnd
		  ,[ProjectRevenue] AS Revenue
		  ,[ProjectRevenueNet] AS RevenueNet
		  ,[Cogs]
		  ,[GrossMargin]
		  ,[ProjectedhoursperMonth] AS Hours
		  ,[SalesCommission]
		  ,CONVERT(DECIMAL,[PracticeManagementCommission]) AS PracticeManagementCommission
		  ,[Expense]
		  ,[ReimbursedExpense]
		  ,[ActualRevenue]
		  ,[ActualGrossMargin]
	FROM [dbo].[ProjectSummaryCache]
	WHERE CacheDate = CONVERT(DATE ,@InsertingTime )
			AND (
					(@StartDate IS NULL AND @EndDate IS NULL) 
					OR  
					([MonthStartDate] BETWEEN @StartDate AND @EndDate)
				) 
			AND [ProjectId] IN (SELECT * FROM @ProjectIDs)
			AND IsMonthlyRecord = @IsMonthlyReport
END
