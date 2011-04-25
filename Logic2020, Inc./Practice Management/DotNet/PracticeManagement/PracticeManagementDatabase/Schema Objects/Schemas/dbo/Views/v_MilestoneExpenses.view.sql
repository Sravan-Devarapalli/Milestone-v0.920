CREATE view dbo.v_MilestoneExpenses
as
select 
	m.milestoneId,
	m.ProjectId,
	sum(pexp.ExpenseAmount) as 'Expense',
	sum(pexp.ExpenseReimbursementAmount) as 'ReimbursedExpense'
from Milestone as m
inner join v_ProjectExpenses as pexp on pexp.MilestoneId = m.MilestoneId
group by m.ProjectId, m.milestoneId

