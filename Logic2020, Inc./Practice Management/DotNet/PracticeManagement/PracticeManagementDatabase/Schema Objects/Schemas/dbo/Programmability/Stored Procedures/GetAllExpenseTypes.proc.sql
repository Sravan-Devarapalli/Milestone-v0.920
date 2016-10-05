CREATE PROCEDURE [dbo].[GetAllExpenseTypes]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ET.Id
		  ,ET.Name
	FROM dbo.ExpenseType ET
	ORDER BY ET.Name
END
