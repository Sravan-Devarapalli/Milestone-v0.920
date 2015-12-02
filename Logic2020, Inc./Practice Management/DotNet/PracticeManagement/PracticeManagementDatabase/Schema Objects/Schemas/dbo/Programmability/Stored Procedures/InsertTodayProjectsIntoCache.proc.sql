CREATE PROCEDURE [dbo].[InsertTodayProjectsIntoCache]
AS
BEGIN
	DECLARE @InsertingTime DATETIME,
			@ProjectId   NVARCHAR(2500)

	DECLARE @Today	DATETIME
	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))

	SELECT @InsertingTime = dbo.InsertingTime()

	SELECT  @ProjectId = ISNULL(@ProjectId,'') + ',' + CONVERT(VARCHAR,P.ProjectId)
	FROM	dbo.Project AS P
	WHERE	CONVERT(DATE,P.CreatedDate) = CONVERT(DATE,@Today)
			AND P.StartDate IS NOT NULL AND P.EndDate IS NOT NULL
			AND P.ProjectId NOT IN (SELECT ProjectId FROM dbo.DefaultMilestoneSetting)
			AND P.IsAllowedToShow = 1

    DELETE  PSC
	FROM	[dbo].[ProjectSummaryCache] PSC
	JOIN	dbo.Project AS P ON P.ProjectId = PSC.ProjectId
	WHERE	CONVERT(DATE,P.CreatedDate) = CONVERT(DATE,@Today)
			AND P.StartDate IS NOT NULL AND P.EndDate IS NOT NULL
			AND P.ProjectId NOT IN (SELECT ProjectId FROM dbo.DefaultMilestoneSetting)
			AND P.IsAllowedToShow = 1 AND CacheDate = CONVERT(DATE,@Today)

	INSERT  [dbo].[ProjectSummaryCache]([ProjectId],[MonthStartDate],[MonthEndDate],RangeType,ProjectRevenue,ProjectRevenueNet,Cogs,GrossMargin,ProjectedhoursperMonth,Expense,ReimbursedExpense,ActualRevenue,ActualGrossMargin,PreviousMonthActualRevenue,PreviousMonthActualGrossMargin,IsMonthlyRecord,CreatedDate,CacheDate) 
	EXEC dbo.FinancialsListByProjectPeriodTotal @UseActuals=1,@ProjectId = @ProjectId 
	  
END
