-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-04-20
-- =============================================
CREATE PROCEDURE dbo.ProjectExpenseInsert 
    @ExpenseId int out,
    @ExpenseName nvarchar(50),
    @ExpenseAmount decimal(18, 2),
	@ExpectedExpenseAmount decimal(18, 2),
    @ExpenseReimbursement decimal(18, 2),
	@ProjectId INT,
	@StartDate	DATETIME,
	@EndDate	DATETIME,
	@MilestoneId	INT,
	@ExpenseTypeId  INT
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ProjectExpense]
           ([Name]
           ,[Amount]
		   ,[ExpectedAmount]
           ,[Reimbursement]
           ,[ProjectId]
           ,[StartDate]
           ,[EndDate]
		   ,[MilestoneId]
		   ,[ExpenseTypeId])
     VALUES
           (@ExpenseName 
           ,@ExpenseAmount
		   ,@ExpectedExpenseAmount
           ,@ExpenseReimbursement
           ,@ProjectId
		   ,@StartDate	
		   ,@EndDate
		   ,@MilestoneId
		   ,@ExpenseTypeId
		   )
	SELECT @ExpenseId = SCOPE_IDENTITY()

	SELECT @ExpenseId
END

