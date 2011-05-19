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

	IF EXISTS (SELECT ExpenseCategoryId FROM ExpenseCategory WHERE Name = @Name)
	BEGIN
		DECLARE @Error NVARCHAR(200)
		SET @Error = 'This Expense Category already exists. Please add a different Expense Category.'
		RAISERROR(@Error,16,1)
	END
	ELSE
	BEGIN	
	UPDATE dbo.ExpenseCategory
	   SET Name = @Name
	 WHERE ExpenseCategoryId = @ExpenseCategoryId
	 
	END



