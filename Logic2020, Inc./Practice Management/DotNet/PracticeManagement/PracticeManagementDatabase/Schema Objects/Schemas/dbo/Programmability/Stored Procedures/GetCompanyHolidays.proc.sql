CREATE PROCEDURE [dbo].GetCompanyHolidays
(
@Year INT
)
AS
	SET NOCOUNT ON
SELECT COUNT(*) AS CompanyHolidays
FROM dbo.Calendar 
WHERE DayOff = 1 
		AND DATEPART(DW,[Date]) NOT IN (1,7) 
		AND YEAR([Date]) = @Year 
