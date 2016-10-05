-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-04-20
-- =============================================
CREATE PROCEDURE dbo.ProjectExpenseUpdate 
    @ExpenseId int,
    @ExpenseName nvarchar(50),
    @ExpenseAmount decimal(18, 2),
	@ExpectedExpenseAmount decimal(18, 2),
    @ExpenseReimbursement decimal(18, 2),
	@ProjectId int = null,
	@StartDate	datetime,
	@EndDate	datetime,
	@MilestoneId	INT = NULL,
	@ExpenseTypeId  INT
AS
BEGIN
	SET NOCOUNT ON;
		Update [dbo].[ProjectExpense] Set 
				[Name] = @ExpenseName
			   ,[Amount] = @ExpenseAmount
			   ,[ExpectedAmount] =@ExpectedExpenseAmount
			   ,[Reimbursement] = @ExpenseReimbursement
			   ,[ProjectId] = ISNULL(@ProjectId,[ProjectId])
			   ,[StartDate] = @StartDate
			   ,[EndDate] = @EndDate			   
			   ,[MilestoneId] = ISNULL(@MilestoneId,[MilestoneId])
			   ,[ExpenseTypeId]=@ExpenseTypeId
		 Where Id = @ExpenseId
END

