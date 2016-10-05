CREATE PROCEDURE [dbo].[ExpenseSummaryGroupedByProject]
(
	 @StartDate DATETIME 
	,@EndDate DATETIME 
	,@ClientIds NVARCHAR(MAX) = NULL 
	,@ShowProjected		BIT = 0
	,@ShowCompleted		BIT = 0
	,@ShowActive			BIT = 0
	,@ShowExperimental	BIT = 0
	,@ShowProposed		BIT=0
	,@ShowInactive		BIT = 0
	,@ShowAtRisk		BIT = 0
	,@DivisionIds NVARCHAR(MAX) = NULL 
	,@PracticeIds NVARCHAR(MAX) = NULL
	,@ProjectIds NVARCHAR(MAX) = NULL
	)
AS 
BEGIN
	
	DECLARE @StartDateLocal   DATETIME,
			@EndDateLocal     DATETIME
			

	SELECT  @StartDateLocal=@StartDate ,
		   @EndDateLocal=@EndDate

		DECLARE @ClientIdsTable TABLE ( Id INT )
		INSERT  INTO @ClientIdsTable
				SELECT  ResultId
				FROM    [dbo].[ConvertStringListIntoTable](@ClientIds)

	  DECLARE @PracticeIdsTable TABLE ( Id INT )
		INSERT  INTO @PracticeIdsTable
				SELECT  ResultId
				FROM    [dbo].[ConvertStringListIntoTable](@PracticeIds)

	  DECLARE @DivisionIdsTable TABLE ( Id INT )
		INSERT  INTO @DivisionIdsTable
				SELECT  ResultId
				FROM    [dbo].[ConvertStringListIntoTable](@DivisionIds)

		DECLARE @ProjectIdsTable TABLE ( Id INT )
		INSERT  INTO @ProjectIdsTable
				SELECT  ResultId
				FROM    [dbo].[ConvertStringListIntoTable](@ProjectIds)

	DECLARE @Projects TABLE
	(
		ResultId INT
	)

	INSERT INTO @Projects
	SELECT  Distinct (pexp.ProjectId)
		FROM dbo.ProjectExpense as pexp
		WHERE pexp.StartDate <= @EndDateLocal AND pexp.EndDate >= @StartDateLocal

	SELECT	vp.ProjectId,
			vp.ProjectNumber,
			vp.Name as 'ProjectName',
			vp.ProjectStatusId,
		    vp.ProjectStatusName,
			vp.ClientId,
			vp.ClientName,
			vp.GroupId,
			pg.Name as 'GroupName',
			vp.PracticeId,
			vp.PracticeName,
			vp.DivisionId,
			vp.DivisionName,
			vp.ProjectManagerId,
			mngr.LastName+' '+mngr.FirstName as 'ProjectManagerName',
			vp.ExecutiveInChargeId,
			exi.LastName+' '+exi.FirstName as 'ExecutiveInChargeName'
	FROM @Projects pr
	JOIN v_Project as vp on vp.ProjectId=pr.ResultId
	LEFT JOIN dbo.Person as mngr on mngr.PersonId=vp.PracticeManagerId
	LEFT JOIN dbo.Person as exi on exi.PersonId=vp.ExecutiveInChargeId
	LEFT JOIN dbo.ProjectGroup as pg on pg.GroupId=vp.GroupId
	WHERE (@ClientIds IS NULL OR vp.ClientId IN (select Id FROM @ClientIdsTable))
	AND (@DivisionIds IS NULL OR vp.DivisionId IN (select Id from @DivisionIdsTable))
	AND (@PracticeIds IS NULL OR vp.PracticeId IN (select Id from @PracticeIdsTable))
	AND (@ProjectIds IS NULL OR vp.ProjectId IN (select Id from @ProjectIdsTable))
	AND (    ( @ShowProjected = 1 AND vp.ProjectStatusId = 2 )
				  OR ( @ShowActive = 1 AND vp.ProjectStatusId = 3 )
				  OR ( @ShowCompleted = 1 AND vp.ProjectStatusId = 4 )
				  OR ( @ShowExperimental = 1 AND vp.ProjectStatusId = 5 ) 
				  OR ( @ShowProposed = 1 AND vp.ProjectStatusId = 7 ) -- Proposed
				  OR ( @ShowInactive = 1 AND vp.ProjectStatusId = 1 ) -- Inactive
				  OR ( @ShowAtRisk = 1 AND vp.ProjectStatusId = 8 )  -- At Risk
				  ) 
	ORDER BY vp.ProjectId


	SELECT  pexp.ProjectId,
			CONVERT(DECIMAL(18,2),SUM(pexp.Amount/((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) Expense,
			CONVERT(DECIMAL(18,2),SUM(pexp.ExpectedAmount/((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) ExpectedExpenseAmount,
			CONVERT(DECIMAL(18,2),SUM(pexp.Reimbursement*0.01*pexp.Amount /((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) ReimbursedExpense,
			C.MonthStartDate AS FinancialDate,
			C.MonthEndDate AS MonthEnd,
			C.MonthNumber
		FROM dbo.ProjectExpense as pexp
		JOIN @Projects as pr on pr.ResultId=pexp.ProjectId
		JOIN dbo.Calendar c ON c.Date BETWEEN pexp.StartDate AND pexp.EndDate
		WHERE  c.Date BETWEEN @StartDateLocal AND @EndDateLocal
		GROUP BY pexp.ProjectId, C.MonthStartDate, C.MonthEndDate,C.MonthNumber

END
