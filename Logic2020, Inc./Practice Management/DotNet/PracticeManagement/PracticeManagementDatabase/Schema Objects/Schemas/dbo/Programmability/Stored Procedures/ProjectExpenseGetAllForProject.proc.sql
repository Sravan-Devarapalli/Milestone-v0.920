CREATE PROCEDURE [dbo].[ProjectExpenseGetAllForProject]
	@ProjectId INT
AS
	SELECT Id ExpenseId,
			Name ExpenseName,
			ProjectId,
			StartDate,
			EndDate,
			[Amount] ExpenseAmount,
			[Reimbursement] ExpenseReimbursement
	FROM dbo.ProjectExpense
	WHERE ProjectId = @ProjectId
