-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-04-20
-- =============================================
CREATE PROCEDURE dbo.ProjectExpenseUpdate 
    @ExpenseId int,
    @ExpenseName nvarchar(50),
    @ExpenseAmount decimal(18, 2),
    @ExpenseReimbursement decimal(18, 2),
	@MilestoneId int = null 
AS
BEGIN
	SET NOCOUNT ON;

	if (@MilestoneId is NULL)
	begin
		Update [dbo].[ProjectExpense] Set 
				[Name] = @ExpenseName
			   ,[Amount] = @ExpenseAmount
			   ,[Reimbursement] = @ExpenseReimbursement
		 Where Id = @ExpenseId
	end
	else
	begin
		Update [dbo].[ProjectExpense] Set 
				[Name] = @ExpenseName
			   ,[Amount] = @ExpenseAmount
			   ,[Reimbursement] = @ExpenseReimbursement
			   ,[MilestoneId] = @MilestoneId
		 Where Id = @ExpenseId
	end
END

