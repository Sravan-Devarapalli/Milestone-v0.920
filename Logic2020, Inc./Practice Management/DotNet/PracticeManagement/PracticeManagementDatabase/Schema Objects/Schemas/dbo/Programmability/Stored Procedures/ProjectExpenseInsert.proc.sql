-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-04-20
-- =============================================
CREATE PROCEDURE dbo.ProjectExpenseInsert 
    @ExpenseId int out,
    @ExpenseName nvarchar(50),
    @ExpenseAmount decimal(18, 2),
    @ExpenseReimbursement decimal(18, 2),
	@MilestoneId int
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ProjectExpense]
           ([Name]
           ,[Amount]
           ,[Reimbursement]
           ,[MilestoneId])
     VALUES
           (@ExpenseName 
           ,@ExpenseAmount
           ,@ExpenseReimbursement
           ,@MilestoneId)
	SELECT @ExpenseId = SCOPE_IDENTITY()

	SELECT @ExpenseId
END

