CREATE PROCEDURE [dbo].[ExpenseDetailReport]
(
  @StartDate DATETIME ,
  @EndDate DATETIME ,
  @ExpenseTypeId INT = NULL,
  @ProjectId INT = NULL
)
AS 
BEGIN

	DECLARE @ProjectIdLocal INT = 0,
			@ExpenseTypeIdLocal INT = 0
			

	IF(@ProjectId IS NOT NULL)
	BEGIN
		SET @ProjectIdLocal = @ProjectId
	END

	IF(@ExpenseTypeId IS NOT NULL)
	BEGIN
		SET @ExpenseTypeIdLocal = @ExpenseTypeId
	END


	;WITH ExpensesByMilestone AS (
		SELECT  pexp.Id as 'ExpenseId',
				CONVERT(DECIMAL(18,2),SUM(pexp.Amount/((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) Expense,
				CONVERT(DECIMAL(18,2),SUM(pexp.ExpectedAmount/((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)))) ExpectedExpenseAmount,
				C.MonthStartDate AS FinancialDate,
				C.MonthEndDate AS MonthEnd,
				C.MonthNumber
		FROM dbo.ProjectExpense as pexp
		JOIN dbo.Calendar c ON c.Date BETWEEN pexp.StartDate AND pexp.EndDate
		WHERE  c.Date BETWEEN @StartDate AND @EndDate
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
	WHERE (@ProjectId IS NULL OR P.ProjectId=@ProjectIdLocal)
	   AND (@ExpenseTypeId IS NULL OR pexp.ExpenseTypeId=@ExpenseTypeIdLocal)

END
