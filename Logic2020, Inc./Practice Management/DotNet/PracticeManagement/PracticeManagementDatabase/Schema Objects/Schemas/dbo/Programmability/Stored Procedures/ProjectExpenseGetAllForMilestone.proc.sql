﻿-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-04-20
-- =============================================
CREATE PROCEDURE dbo.ProjectExpenseGetAllForMilestone 
	@MilestoneId int
AS
BEGIN
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	SELECT  0 ExpenseId,
			'' ExpenseName,
			ProjectId,
			CONVERT(DATETIME,'') StartDate,
			CONVERT(DATETIME,'') EndDate,
			Expense ExpenseAmount,
			ReimbursedExpense ExpenseReimbursement
	from v_MilestoneExpenses 
	where MilestoneId = @MilestoneId
END

