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

	INSERT INTO dbo.ExpenseCategory
	            (Name)
	     VALUES (@Name)

