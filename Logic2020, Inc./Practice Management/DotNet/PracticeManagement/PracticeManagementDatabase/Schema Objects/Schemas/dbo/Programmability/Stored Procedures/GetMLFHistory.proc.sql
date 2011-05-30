CREATE PROCEDURE [dbo].[GetMLFHistory]
AS
BEGIN
	SET NOCOUNT ON
	SELECT 
	mlfh.StartDate,
	mlfh.EndDate,
	mlfh.[W2-Hourly] as 'W2Salary_Rate',
	mlfh.[W2-Salary] as 'W2Hourly_Rate',
	mlfh.[1099] as '_1099_Hourly_Rate'
	FROM [dbo].[MinimumLoadFactorHistoryForUI] as mlfh
END
