-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-22-2008
-- Updated by:	
-- Update date:	
-- Description:	Retrieves the list of the categories for the fixed expenses
-- =============================================
CREATE PROCEDURE [dbo].[ExpenseCategoryListAll]
AS
	SET NOCOUNT ON

	SELECT ec.ExpenseCategoryId, ec.Name
	  FROM dbo.ExpenseCategory AS ec
	ORDER BY ec.Name

