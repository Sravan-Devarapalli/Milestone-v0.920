-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-04-20
-- =============================================
CREATE PROCEDURE dbo.ProjectExpenseInsert 
    @ExpenseId int out,
    @ExpenseName nvarchar(50),
    @ExpenseAmount decimal(18, 2),
    @ExpenseReimbursement decimal(18, 2),
	@ProjectId INT,
	@StartDate	DATETIME,
	@EndDate	DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ProjectExpense]
           ([Name]
           ,[Amount]
           ,[Reimbursement]
           ,[ProjectId]
           ,[StartDate]
           ,[EndDate])
     VALUES
           (@ExpenseName 
           ,@ExpenseAmount
           ,@ExpenseReimbursement
           ,@ProjectId
		   ,@StartDate	
		   ,@EndDate
		   )
	SELECT @ExpenseId = SCOPE_IDENTITY()

	SELECT @ExpenseId
END

