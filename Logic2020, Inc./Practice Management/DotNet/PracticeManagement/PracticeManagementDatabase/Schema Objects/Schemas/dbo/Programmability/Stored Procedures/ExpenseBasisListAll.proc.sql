-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-24-2008
-- Updated by:	
-- Update date:	
-- Description:	Retrieves the list of the Expense Bases
-- =============================================
CREATE PROCEDURE [dbo].[ExpenseBasisListAll]
AS
	SET NOCOUNT ON

	SELECT b.ExpenseBasisId, Name
	  FROM dbo.ExpenseBasis AS b
	ORDER BY b.Name

