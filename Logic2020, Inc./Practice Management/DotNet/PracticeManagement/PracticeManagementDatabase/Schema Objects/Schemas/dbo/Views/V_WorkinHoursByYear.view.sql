CREATE VIEW [dbo].[V_WorkinHoursByYear]
AS 
	SELECT [Year] 
	,CASE WHEN (([Year] % 4) = 0 AND ([Year] % 100 ) <> 0) OR ([Year] % 400) = 0
		  THEN 2088 
		  ELSE 2080 END HoursInYear
	FROM dbo.Calendar 
	GROUP BY [Year]

