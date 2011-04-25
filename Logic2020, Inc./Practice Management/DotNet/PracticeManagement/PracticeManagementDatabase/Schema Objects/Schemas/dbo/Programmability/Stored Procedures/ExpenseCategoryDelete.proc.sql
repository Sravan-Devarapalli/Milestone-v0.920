-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-22-2008
-- Updated by:	
-- Update date:	
-- Description:	Deletes an Expense Category
-- =============================================
CREATE PROCEDURE [dbo].[ExpenseCategoryDelete]
(
	@ExpenseCategoryId   INT
)
AS
	SET NOCOUNT ON
	DECLARE @ErrorMessage NVARCHAR(2048)

	IF EXISTS (SELECT 1 FROM dbo.MonthlyExpense AS e WHERE e.ExpenseCategoryId = @ExpenseCategoryId)
	BEGIN
		SELECT @ErrorMessage = [dbo].[GetErrorMessage](70010)
		RAISERROR (@ErrorMessage, 16, 1)
	END
	ELSE
	BEGIN
		DELETE
		  FROM dbo.ExpenseCategory
		 WHERE ExpenseCategoryId = @ExpenseCategoryId
	END

