CREATE PROCEDURE [dbo].[GetListOfRevenueTypes]
AS
BEGIN
	SELECT R.RevenueTypeId,
		   R.RevenueName,
		   R.IsDefault
	FROM Revenue R
	ORDER BY R.RevenueName
END
