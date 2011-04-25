-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-24-2008
-- Updated by:	
-- Update date:	
-- Description:	Deletes a month expense item by the specified name
-- =============================================
CREATE PROCEDURE [dbo].[MonthlyExpenseDeleteItem]
(
	@Name   NVARCHAR(50)
)
AS
	SET NOCOUNT ON

	DELETE
	  FROM dbo.MonthlyExpense
	 WHERE Name = @Name

