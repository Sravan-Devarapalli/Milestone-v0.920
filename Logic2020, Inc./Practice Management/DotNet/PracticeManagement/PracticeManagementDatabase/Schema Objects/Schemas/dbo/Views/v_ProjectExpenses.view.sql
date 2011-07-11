CREATE view dbo.v_ProjectExpenses
as
	SELECT 

      pexp.[Amount] as 'ExpenseAmount'
      ,pexp.[Reimbursement] as 'ExpenseReimbursement'
	  ,pexp.[Reimbursement] * 0.01 * pexp.[Amount] as 'ExpenseReimbursementAmount'
	  ,pexp.ProjectId
	from dbo.ProjectExpense as pexp
	JOIN dbo.Milestone M ON M.ProjectId = pexp.ProjectId

