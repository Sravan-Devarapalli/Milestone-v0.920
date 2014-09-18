CREATE VIEW dbo.v_MilestoneExpenses
AS
 	SELECT MilestoneId
		  ,ProjectId
		  ,Amount as Expense
		  ,[Reimbursement] * 0.01 * [Amount] as ReimbursedExpense
	FROM  dbo.ProjectExpense

