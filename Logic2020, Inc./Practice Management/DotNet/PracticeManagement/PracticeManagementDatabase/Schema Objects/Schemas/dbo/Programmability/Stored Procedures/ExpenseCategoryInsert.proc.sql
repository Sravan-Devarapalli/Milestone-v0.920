-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-22-2008
-- Updated by:	
-- Update date:	
-- Description:	Inserts a new Expense Category
-- =============================================
CREATE PROCEDURE [dbo].[ExpenseCategoryInsert]
(
	@Name   NVARCHAR(25)
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
		INSERT INTO dbo.ExpenseCategory
				    (Name)
			 VALUES (@Name)
	END

GO



