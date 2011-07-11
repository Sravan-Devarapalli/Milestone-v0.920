CREATE view dbo.v_MilestoneExpenses
as
 With CTEMileStonesPerDay
AS
	(SELECT  
		pexp.ProjectId,
	    c.Date,
	    COUNT(*) MileStonesPerDay 
	from dbo.ProjectExpense as pexp
	JOIN dbo.Calendar c ON c.Date BETWEEN pexp.StartDate AND pexp.EndDate
	JOIN dbo.Milestone M ON M.ProjectId = pexp.ProjectId AND c.Date BETWEEN M.StartDate AND M.ProjectedDeliveryDate
	GROUP BY c.Date,pexp.ProjectId
	)
SELECT M.MilestoneId,
	  M.ProjectId,
	  CONVERT(DECIMAL(18,2),SUM(pexp.Amount/ ((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)*MileStonesPerDay))) Expense,
	  CONVERT(DECIMAL(18,2),SUM(pexp.Reimbursement*0.01*pexp.Amount / ((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)*MileStonesPerDay))) ReimbursedExpense 
FROM CTEMileStonesPerDay CTE
JOIN dbo.ProjectExpense as pexp 
ON CTE.ProjectId = pexp.ProjectId AND CTE.Date BETWEEN pexp.StartDate AND pexp.EndDate
JOIN dbo.Milestone M ON M.ProjectId = pexp.ProjectId AND CTE.Date BETWEEN M.StartDate AND M.ProjectedDeliveryDate
GROUP BY M.MilestoneId,M.ProjectId,M.StartDate,M.ProjectedDeliveryDate 
 
