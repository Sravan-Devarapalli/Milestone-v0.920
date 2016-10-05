CREATE PROCEDURE [dbo].[ExpenseSummaryDetails]
(
     @StartDate DATETIME
    ,@EndDate DATETIME 
    ,@ClientIds NVARCHAR(MAX) = NULL 
	,@ShowProjected		BIT = 1
	,@ShowCompleted		BIT = 1
	,@ShowActive		BIT = 1
	,@ShowExperimental	BIT = 1
	,@ShowProposed		BIT = 1
	,@ShowInactive		BIT = 1
	,@ShowAtRisk		BIT = 1
	,@DivisionIds NVARCHAR(MAX) = NULL 
	,@PracticeIds NVARCHAR(MAX) = NULL 
	,@ExpenseTypes  NVARCHAR(MAX) = NULL 
	,@ProjectIds NVARCHAR(MAX) = NULL
)
AS 
BEGIN
	
		DECLARE @StartDateLocal   DATETIME,
			    @EndDateLocal     DATETIME
			
		SELECT @StartDateLocal=@StartDate ,
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

		DECLARE @TypeIdsTable TABLE ( Id INT )
		INSERT  INTO @TypeIdsTable
				SELECT  ResultId
				FROM    [dbo].[ConvertStringListIntoTable](@ExpenseTypes)

		DECLARE @ProjectIdsTable TABLE ( Id INT )
		INSERT  INTO @ProjectIdsTable
				SELECT  ResultId
				FROM    [dbo].[ConvertStringListIntoTable](@ProjectIds)


		;WITH ExpensesByMilestone AS (
		SELECT  pexp.Id as 'ExpenseId',
				CONVERT(DECIMAL(18,2),SUM(pexp.Amount/((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) Expense,
				CONVERT(DECIMAL(18,2),SUM(pexp.ExpectedAmount/((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) ExpectedExpenseAmount,
				C.MonthStartDate AS FinancialDate,
				C.MonthEndDate AS MonthEnd,
				C.MonthNumber
		FROM dbo.ProjectExpense as pexp
		JOIN v_Project as vp on vp.ProjectId=pexp.ProjectId
		JOIN dbo.Calendar c ON c.Date BETWEEN pexp.StartDate AND pexp.EndDate
		WHERE  c.Date BETWEEN @StartDate AND @EndDate
			AND	(@ClientIds IS NULL OR vp.ClientId IN (select Id FROM @ClientIdsTable))
			AND (@DivisionIds IS NULL OR vp.DivisionId IN (select Id from @DivisionIdsTable))
			AND (@PracticeIds IS NULL OR vp.PracticeId IN (select Id from @PracticeIdsTable))
			AND (@ProjectIds IS NULL OR vp.ProjectId IN (select Id from @ProjectIdsTable))
			AND (@ExpenseTypes IS NULL OR pexp.ExpenseTypeId IN (SELECT Id FROM @TypeIdsTable))
			AND (    ( @ShowProjected = 1 AND vp.ProjectStatusId = 2 )
				  OR ( @ShowActive = 1 AND vp.ProjectStatusId = 3 )
				  OR ( @ShowCompleted = 1 AND vp.ProjectStatusId = 4 )
				  OR ( @ShowExperimental = 1 AND vp.ProjectStatusId = 5 ) 
				  OR ( @ShowProposed = 1 AND vp.ProjectStatusId = 7 )
				  OR ( @ShowInactive = 1 AND vp.ProjectStatusId = 1 )
				  or ( @ShowAtRisk = 1 AND vp.ProjectStatusId = 8)
				  ) 

		GROUP BY pexp.Id, C.MonthStartDate, C.MonthEndDate,C.MonthNumber
		)

	SELECT  EM.ExpenseId,
			pexp.Name,
			pexp.ExpenseTypeId,
			ET.Name as 'ExpenseTypeName',
			pexp.StartDate,
			pexp.EndDate,
			M.MilestoneId,
			M.Description as 'MilestoneName',
			P.ProjectId,
			p.ProjectNumber,
			p.Name as 'ProjectName',
			EM.Expense,
			EM.ExpectedExpenseAmount,
			pexp.Reimbursement as ReimbursedExpense,
			EM.FinancialDate,
			EM.MonthEnd,
			EM.MonthNumber
	FROM ExpensesByMilestone EM
	JOIN ProjectExpense as pexp on em.ExpenseId=pexp.Id
	JOIN Milestone as M on M.MilestoneId=pexp.MilestoneId
	JOIN Project as P on P.ProjectId=pexp.ProjectId
	JOIN ExpenseType as ET on Et.Id=pexp.ExpenseTypeId

END
