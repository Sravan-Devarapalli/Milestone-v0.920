CREATE PROCEDURE [dbo].[ExpenseSummaryGroupedByExpenseType]
(
  @StartDate DATETIME ,
  @EndDate DATETIME ,
  @ExpenseTypes  NVARCHAR(MAX) = NULL 
)
AS 
BEGIN

DECLARE @StartDateLocal   DATETIME,
			@EndDateLocal     DATETIME

	SELECT  @StartDateLocal=@StartDate,
		   @EndDateLocal=@EndDate

	DECLARE @TypeIdsTable TABLE ( Id INT )
		INSERT  INTO @TypeIdsTable
				SELECT  ResultId
				FROM    [dbo].[ConvertStringListIntoTable](@ExpenseTypes)

	DECLARE @ExpensetypeIDs TABLE
	(
		ResultId INT
	)

	INSERT INTO @ExpensetypeIDs
	SELECT  Distinct (pexp.ExpenseTypeId)
		FROM dbo.ProjectExpense as pexp
		WHERE (@ExpenseTypes IS NULL OR pexp.ExpenseTypeId IN (SELECT Id FROM @TypeIdsTable)) AND pexp.StartDate <= @EndDateLocal AND pexp.EndDate >= @StartDateLocal

	SELECT ET.Id as ExpenseTypeId,
		   ET.Name as 'ExpenseTypeName'
	FROM @ExpensetypeIDs as MET
	JOIN dbo.ExpenseType as ET on ET.Id=MET.ResultId
	ORDER BY ET.Name
	
	SELECT  pexp.ExpenseTypeId,
			CONVERT(DECIMAL(18,2),SUM(pexp.Amount/((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) Expense,
			CONVERT(DECIMAL(18,2),SUM(pexp.ExpectedAmount/((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) ExpectedExpenseAmount,
			CONVERT(DECIMAL(18,2),SUM(pexp.Reimbursement*0.01*pexp.Amount /((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) ReimbursedExpense,
			C.MonthStartDate AS FinancialDate,
			C.MonthEndDate AS MonthEnd,
			C.MonthNumber
		FROM dbo.ProjectExpense as pexp
		JOIN @ExpensetypeIDs as et on et.ResultId=pexp.ExpenseTypeId
		JOIN dbo.Calendar c ON c.Date BETWEEN pexp.StartDate AND pexp.EndDate
		WHERE  c.Date BETWEEN @StartDateLocal AND @EndDateLocal
		GROUP BY pexp.ExpenseTypeId, C.MonthStartDate, C.MonthEndDate,C.MonthNumber
	
END
