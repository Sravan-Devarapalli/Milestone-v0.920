-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-04-20
-- =============================================
CREATE PROCEDURE dbo.ProjectExpenseGetAllForMilestone 
	@MilestoneId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT  PE.Id ExpenseId,
			PE.Name ExpenseName,
			PE.ProjectId,
			PE.StartDate,
			PE.EndDate,
			PE.Amount ExpenseAmount,
			PE.ExpectedAmount ExpectedExpenseAmount,
			PE.Reimbursement ExpenseReimbursement,
			M.Description AS MilestoneName,
			PE.ExpenseTypeId,
			ET.Name ExpenseTypeName
	FROM dbo.ProjectExpense PE
	LEFT JOIN dbo.Milestone M ON M.MilestoneId = PE.MilestoneId
	LEFT JOIN dbo.ExpenseType ET ON PE.ExpenseTypeId=ET.Id
	WHERE PE.MilestoneId = @MilestoneId
	ORDER BY PE.StartDate

END

