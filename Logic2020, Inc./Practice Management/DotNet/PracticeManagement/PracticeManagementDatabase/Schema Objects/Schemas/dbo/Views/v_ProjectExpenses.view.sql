CREATE view dbo.v_ProjectExpenses
as
	SELECT 
	   pexp.[Id] as 'ExpenseId'
      ,pexp.[Name] as 'ExpenseName'
      ,pexp.[Amount] as 'ExpenseAmount'
      ,pexp.[Reimbursement] as 'ExpenseReimbursement'
	  ,pexp.[Reimbursement] * 0.01 * pexp.[Amount] as 'ExpenseReimbursementAmount'
	  ,m.ProjectId
      ,m.[MilestoneId] as 'MilestoneId'
	  ,m.[Description] as 'MilestoneName'
	from dbo.ProjectExpense as pexp
	inner join milestone as m on pexp.milestoneId = m.milestoneId

