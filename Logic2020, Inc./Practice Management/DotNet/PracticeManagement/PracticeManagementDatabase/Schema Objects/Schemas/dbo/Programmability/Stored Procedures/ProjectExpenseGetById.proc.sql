-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-04-20
-- =============================================
CREATE PROCEDURE dbo.ProjectExpenseGetById 
    @ExpenseId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * from v_ProjectExpenses where ExpenseId = @ExpenseId
END

