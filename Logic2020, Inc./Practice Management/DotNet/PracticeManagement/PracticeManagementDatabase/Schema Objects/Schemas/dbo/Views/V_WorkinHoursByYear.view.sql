CREATE VIEW [dbo].[V_WorkinHoursByYear]
AS 
	SELECT	cal.[Year],
			CONVERT(INT,cal.[DaysInYear] * Convert(DECIMAL(4,2),s.Value)) AS HoursInYear
	FROM dbo.Calendar cal
	INNER JOIN  Settings s ON s.SettingsKey='DefaultHoursPerDay'
	GROUP BY cal.[Year],cal.[DaysInYear],s.Value
	

