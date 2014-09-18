CREATE PROCEDURE [dbo].[ProjectExpenseGetAllForProject]
	@ProjectId INT
AS
BEGIN
	SELECT PE.Id ExpenseId,
			PE.Name ExpenseName,
			PE.ProjectId,
			PE.StartDate,
			PE.EndDate,
			PE.Amount ExpenseAmount,
			PE.Reimbursement ExpenseReimbursement,
			M.Description AS MilestoneName
	FROM dbo.ProjectExpense PE
	LEFT JOIN dbo.Milestone M ON M.MilestoneId = PE.MilestoneId
	WHERE PE.ProjectId = @ProjectId
	ORDER BY PE.StartDate
END

