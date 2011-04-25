-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-24-2008
-- Updated by:	
-- Update date:	
-- Description:	Retrieves the month expense by the specified Name
-- =============================================
CREATE PROCEDURE [dbo].[MonthlyExpenseListByName]
(
	@Name   NVARCHAR(50)
)
AS
	SET NOCOUNT ON

	SELECT me.MonthlyExpenseId,
	       me.ExpenseCategoryId,
	       me.ExpenseBasisId,
	       me.WeekPaidOptionId,
	       me.Name,
	       me.[Year],
	       me.[Month],
	       me.Amount
	  FROM dbo.MonthlyExpense AS me
	 WHERE me.Name = @Name
	ORDER BY me.[Year], me.[Month]

