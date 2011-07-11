-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-04-20
-- =============================================
CREATE PROCEDURE dbo.ProjectExpenseUpdate 
    @ExpenseId int,
    @ExpenseName nvarchar(50),
    @ExpenseAmount decimal(18, 2),
    @ExpenseReimbursement decimal(18, 2),
	@ProjectId int = null,
	@StartDate	datetime,
	@EndDate	datetime
AS
BEGIN
	SET NOCOUNT ON;
		Update [dbo].[ProjectExpense] Set 
				[Name] = @ExpenseName
			   ,[Amount] = @ExpenseAmount
			   ,[Reimbursement] = @ExpenseReimbursement
			   ,[ProjectId] = ISNULL(@ProjectId,[ProjectId])
			   ,[StartDate] = @StartDate
			   ,[EndDate] = @EndDate			   
		 Where Id = @ExpenseId
END

