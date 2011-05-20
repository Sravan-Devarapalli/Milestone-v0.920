-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-22-2008
-- Updated by:	
-- Update date:	
-- Description:	Updates an Expense Category
-- =============================================
CREATE PROCEDURE [dbo].[ExpenseCategoryUpdate]
(
	@ExpenseCategoryId   INT,
	@Name                NVARCHAR(25)
)
AS
	SET NOCOUNT ON

	UPDATE dbo.ExpenseCategory
	   SET Name = @Name
	 WHERE ExpenseCategoryId = @ExpenseCategoryId
	 


