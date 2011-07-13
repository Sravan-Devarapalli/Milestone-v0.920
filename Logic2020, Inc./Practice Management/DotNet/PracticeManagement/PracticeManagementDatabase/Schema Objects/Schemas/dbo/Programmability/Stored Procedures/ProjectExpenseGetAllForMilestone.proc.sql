-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-04-20
-- =============================================
CREATE PROCEDURE dbo.ProjectExpenseGetAllForMilestone 
	@MilestoneId int
AS
BEGIN
	SET NOCOUNT ON;

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
	SELECT  pexp.Id ExpenseId,
			pexp.Name ExpenseName,
			M.ProjectId,
			pexp.StartDate,
			pexp.EndDate,
			CONVERT(DECIMAL(18,2),SUM(pexp.Amount/ ((DATEDIFF(dd,pexp.StartDate,pexp.EndDate)+1)*MileStonesPerDay))) ExpenseAmount,
			MAX(pexp.Reimbursement)  ExpenseReimbursement
	FROM CTEMileStonesPerDay CTE
	JOIN dbo.ProjectExpense as pexp 
	ON CTE.ProjectId = pexp.ProjectId AND CTE.Date BETWEEN pexp.StartDate AND pexp.EndDate
	JOIN dbo.Milestone M ON M.ProjectId = pexp.ProjectId AND CTE.Date BETWEEN M.StartDate AND M.ProjectedDeliveryDate
	WHERE M.MilestoneId =  @MilestoneId
	GROUP BY M.MilestoneId,M.ProjectId,pexp.Id,pexp.StartDate,pexp.EndDate,pexp.Name
END

