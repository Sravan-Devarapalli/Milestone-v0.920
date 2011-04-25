CREATE VIEW [dbo].[V_WorkinHoursByYear]
AS 
	SELECT YEAR(DATE) [Year] 
	,CASE WHEN ((YEAR(DATE) % 4) = 0 AND (YEAR(DATE) % 100 ) <> 0) OR (YEAR(DATE) % 400) = 0
		  THEN 2088 
		  ELSE 2080 END HoursInYear
	FROM dbo.Calendar 
	GROUP BY YEAR(DATE)

