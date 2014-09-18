﻿-- =============================================
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
	@EndDate	DATETIME,
	@MilestoneId	INT
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ProjectExpense]
           ([Name]
           ,[Amount]
           ,[Reimbursement]
           ,[ProjectId]
           ,[StartDate]
           ,[EndDate]
		   ,[MilestoneId])
     VALUES
           (@ExpenseName 
           ,@ExpenseAmount
           ,@ExpenseReimbursement
           ,@ProjectId
		   ,@StartDate	
		   ,@EndDate
		   ,@MilestoneId
		   )
	SELECT @ExpenseId = SCOPE_IDENTITY()

	SELECT @ExpenseId
END

